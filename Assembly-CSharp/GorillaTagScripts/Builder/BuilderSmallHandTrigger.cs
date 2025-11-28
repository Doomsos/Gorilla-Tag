using System;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E65 RID: 3685
	public class BuilderSmallHandTrigger : MonoBehaviour
	{
		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x06005C25 RID: 23589 RVA: 0x001D942C File Offset: 0x001D762C
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x06005C26 RID: 23590 RVA: 0x001D943C File Offset: 0x001D763C
		private void OnTriggerEnter(Collider other)
		{
			if (!base.enabled)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = other.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent == null)
			{
				return;
			}
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
			if (this.onlySmallHands && !this.ignoreScale && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			if (this.requireMinimumVelocity)
			{
				float num = this.minimumVelocityMagnitude * GorillaTagger.Instance.offlineVRRig.scaleFactor;
				if (GTPlayer.Instance.GetHandVelocityTracker(componentInParent.isLeftHand).GetAverageVelocity(true, 0.1f, false).sqrMagnitude < num * num)
				{
					return;
				}
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			this.lastTriggeredFrame = Time.frameCount;
			UnityEvent triggeredEvent = this.TriggeredEvent;
			if (triggeredEvent != null)
			{
				triggeredEvent.Invoke();
			}
			if (this.timeline != null && (this.timeline.time == 0.0 || this.timeline.time >= this.timeline.duration))
			{
				this.timeline.Play();
			}
			if (this.animation != null && this.animation.clip != null)
			{
				this.animation.Play();
			}
		}

		// Token: 0x0400698F RID: 27023
		[Tooltip("Optional timeline to play to animate the thing getting activated, play sound, particles, etc...")]
		public PlayableDirector timeline;

		// Token: 0x04006990 RID: 27024
		[Tooltip("Optional animation to play")]
		public Animation animation;

		// Token: 0x04006991 RID: 27025
		private int lastTriggeredFrame = -1;

		// Token: 0x04006992 RID: 27026
		public bool onlySmallHands;

		// Token: 0x04006993 RID: 27027
		[SerializeField]
		protected bool requireMinimumVelocity;

		// Token: 0x04006994 RID: 27028
		[SerializeField]
		protected float minimumVelocityMagnitude = 0.1f;

		// Token: 0x04006995 RID: 27029
		private bool hasCheckedZone;

		// Token: 0x04006996 RID: 27030
		private bool ignoreScale;

		// Token: 0x04006997 RID: 27031
		internal UnityEvent TriggeredEvent = new UnityEvent();

		// Token: 0x04006998 RID: 27032
		[SerializeField]
		private BuilderPiece myPiece;
	}
}
