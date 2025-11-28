using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E6A RID: 3690
	public class KnockbackTrigger : MonoBehaviour
	{
		// Token: 0x17000887 RID: 2183
		// (get) Token: 0x06005C46 RID: 23622 RVA: 0x001DA3B2 File Offset: 0x001D85B2
		public bool TriggeredThisFrame
		{
			get
			{
				return this.lastTriggeredFrame == Time.frameCount;
			}
		}

		// Token: 0x06005C47 RID: 23623 RVA: 0x001DA3C4 File Offset: 0x001D85C4
		private void CheckZone()
		{
			if (!this.hasCheckedZone)
			{
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(VRRigCache.Instance.localRig.Rig.zoneEntity.currentZone, out builderTable))
				{
					this.ignoreScale = !builderTable.isTableMutable;
				}
				this.hasCheckedZone = true;
			}
		}

		// Token: 0x06005C48 RID: 23624 RVA: 0x001DA414 File Offset: 0x001D8614
		private void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.CheckZone();
			if (!this.ignoreScale && this.onlySmallMonke && (double)VRRigCache.Instance.localRig.Rig.scaleFactor > 0.99)
			{
				return;
			}
			this.collidersEntered.Add(other);
			if (this.collidersEntered.Count > 1)
			{
				return;
			}
			Vector3 vector = this.triggerVolume.ClosestPoint(GorillaTagger.Instance.headCollider.transform.position);
			Vector3 vector2 = vector - base.transform.TransformPoint(this.triggerVolume.center);
			vector2 -= Vector3.Project(vector2, base.transform.TransformDirection(this.localAxis));
			float magnitude = vector2.magnitude;
			Vector3 direction = Vector3.up;
			if (magnitude >= 0.01f)
			{
				direction = vector2 / magnitude;
			}
			GTPlayer.Instance.SetMaximumSlipThisFrame();
			GTPlayer.Instance.ApplyKnockback(direction, this.knockbackVelocity * VRRigCache.Instance.localRig.Rig.scaleFactor, false);
			if (this.impactFX != null)
			{
				ObjectPools.instance.Instantiate(this.impactFX, vector, true);
			}
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength / 2f, Time.fixedDeltaTime);
			this.lastTriggeredFrame = Time.frameCount;
		}

		// Token: 0x06005C49 RID: 23625 RVA: 0x001DA5B6 File Offset: 0x001D87B6
		private void OnTriggerExit(Collider other)
		{
			if (!other.gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHead) && !other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
			{
				return;
			}
			this.collidersEntered.Remove(other);
		}

		// Token: 0x06005C4A RID: 23626 RVA: 0x001DA5F2 File Offset: 0x001D87F2
		private void OnDisable()
		{
			this.collidersEntered.Clear();
		}

		// Token: 0x040069CC RID: 27084
		[SerializeField]
		private BoxCollider triggerVolume;

		// Token: 0x040069CD RID: 27085
		[SerializeField]
		private float knockbackVelocity;

		// Token: 0x040069CE RID: 27086
		[SerializeField]
		private Vector3 localAxis;

		// Token: 0x040069CF RID: 27087
		[SerializeField]
		private GameObject impactFX;

		// Token: 0x040069D0 RID: 27088
		[SerializeField]
		private bool onlySmallMonke;

		// Token: 0x040069D1 RID: 27089
		private bool hasCheckedZone;

		// Token: 0x040069D2 RID: 27090
		private bool ignoreScale;

		// Token: 0x040069D3 RID: 27091
		private int lastTriggeredFrame = -1;

		// Token: 0x040069D4 RID: 27092
		private List<Collider> collidersEntered = new List<Collider>(4);
	}
}
