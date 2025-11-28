using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001126 RID: 4390
	public class VoiceBroadcastCosmeticWearable : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06006DD6 RID: 28118 RVA: 0x00241054 File Offset: 0x0023F254
		private void Start()
		{
			VoiceBroadcastCosmetic[] componentsInChildren = base.GetComponentInParent<VRRig>().GetComponentsInChildren<VoiceBroadcastCosmetic>(true);
			this.voiceBroadcasters = new List<VoiceBroadcastCosmetic>();
			foreach (VoiceBroadcastCosmetic voiceBroadcastCosmetic in componentsInChildren)
			{
				if (voiceBroadcastCosmetic.talkingCosmeticType == this.talkingCosmeticType)
				{
					this.voiceBroadcasters.Add(voiceBroadcastCosmetic);
					voiceBroadcastCosmetic.SetWearable(this);
				}
			}
		}

		// Token: 0x06006DD7 RID: 28119 RVA: 0x002410AC File Offset: 0x0023F2AC
		public void OnEnable()
		{
			if (this.playerHeadCollider == null)
			{
				VRRig componentInParent = base.GetComponentInParent<VRRig>();
				this.playerHeadCollider = ((componentInParent != null) ? componentInParent.rigContainer.HeadCollider : null);
			}
			if (this.headDistanceActivation && this.playerHeadCollider != null)
			{
				GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			}
		}

		// Token: 0x06006DD8 RID: 28120 RVA: 0x0001140C File Offset: 0x0000F60C
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06006DD9 RID: 28121 RVA: 0x00241104 File Offset: 0x0023F304
		public void SliceUpdate()
		{
			if (Time.time - this.lastToggleTime >= this.toggleCooldown)
			{
				bool flag = (base.transform.position - this.playerHeadCollider.transform.position).sqrMagnitude <= this.headDistance * this.headDistance;
				if (flag != this.toggleState)
				{
					this.toggleState = flag;
					this.lastToggleTime = Time.time;
					if (flag)
					{
						UnityEvent unityEvent = this.onStartListening;
						if (unityEvent != null)
						{
							unityEvent.Invoke();
						}
					}
					else
					{
						UnityEvent unityEvent2 = this.onStopListening;
						if (unityEvent2 != null)
						{
							unityEvent2.Invoke();
						}
					}
					for (int i = 0; i < this.voiceBroadcasters.Count; i++)
					{
						this.voiceBroadcasters[i].SetListenState(flag);
					}
				}
			}
		}

		// Token: 0x06006DDA RID: 28122 RVA: 0x002411CD File Offset: 0x0023F3CD
		public void OnCosmeticStartListening()
		{
			if (this.headDistanceActivation)
			{
				return;
			}
			UnityEvent unityEvent = this.onStartListening;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x06006DDB RID: 28123 RVA: 0x002411E8 File Offset: 0x0023F3E8
		public void OnCosmeticStopListening()
		{
			if (this.headDistanceActivation)
			{
				return;
			}
			UnityEvent unityEvent = this.onStopListening;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		// Token: 0x04007F77 RID: 32631
		public TalkingCosmeticType talkingCosmeticType;

		// Token: 0x04007F78 RID: 32632
		[SerializeField]
		private bool headDistanceActivation = true;

		// Token: 0x04007F79 RID: 32633
		[SerializeField]
		private float headDistance = 0.4f;

		// Token: 0x04007F7A RID: 32634
		[SerializeField]
		private float toggleCooldown = 0.5f;

		// Token: 0x04007F7B RID: 32635
		private bool toggleState;

		// Token: 0x04007F7C RID: 32636
		private float lastToggleTime;

		// Token: 0x04007F7D RID: 32637
		[SerializeField]
		private UnityEvent onStartListening;

		// Token: 0x04007F7E RID: 32638
		[SerializeField]
		private UnityEvent onStopListening;

		// Token: 0x04007F7F RID: 32639
		private List<VoiceBroadcastCosmetic> voiceBroadcasters;

		// Token: 0x04007F80 RID: 32640
		private Collider playerHeadCollider;
	}
}
