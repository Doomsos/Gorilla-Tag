using System;
using UnityEngine;
using UnityEngine.Events;

namespace Cosmetics
{
	// Token: 0x02000FC5 RID: 4037
	public class CosmeticsLocalHandReactor : MonoBehaviour
	{
		// Token: 0x06006657 RID: 26199 RVA: 0x00215CE0 File Offset: 0x00213EE0
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
			if (this.ownerRig == null)
			{
				GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
				if (componentInParent != null)
				{
					this.ownerRig = componentInParent.offlineVRRig;
					this.ownerIsLocal = (this.ownerRig != null);
				}
			}
			if (this.ownerRig == null)
			{
				Debug.LogError("TriggerToggler: Disabling cannot find VRRig.");
				base.enabled = false;
				return;
			}
		}

		// Token: 0x06006658 RID: 26200 RVA: 0x00215D58 File Offset: 0x00213F58
		protected void LateUpdate()
		{
			if (this.ownerIsLocal)
			{
				if (Time.time < this.lastTriggerTime + this.cooldownTime)
				{
					return;
				}
				Transform transform = base.transform;
				if (Physics.OverlapSphereNonAlloc(base.transform.position, this.proximityThreshold * transform.lossyScale.x, this.colliders, this.handLayer) > 0)
				{
					GorillaTriggerColliderHandIndicator component = this.colliders[0].GetComponent<GorillaTriggerColliderHandIndicator>();
					if (component != null)
					{
						GorillaTagger.Instance.StartVibration(component.isLeftHand, this.hapticStrength, this.hapticDuration);
						UnityEvent<bool> unityEvent = this.onTrigger;
						if (unityEvent != null)
						{
							unityEvent.Invoke(component.isLeftHand);
						}
						this.lastTriggerTime = Time.time;
					}
				}
			}
		}

		// Token: 0x04007504 RID: 29956
		[SerializeField]
		private float hapticStrength = 0.2f;

		// Token: 0x04007505 RID: 29957
		[SerializeField]
		private float hapticDuration = 0.2f;

		// Token: 0x04007506 RID: 29958
		[Tooltip("The distance threshold (in meters) for triggering the interaction.\nIf the hand enters this range, onTrigger is fired.")]
		public float proximityThreshold = 0.15f;

		// Token: 0x04007507 RID: 29959
		[Tooltip("Minimum time (in seconds) between consecutive triggers.\n")]
		[SerializeField]
		private float cooldownTime = 0.5f;

		// Token: 0x04007508 RID: 29960
		public UnityEvent<bool> onTrigger;

		// Token: 0x04007509 RID: 29961
		private VRRig ownerRig;

		// Token: 0x0400750A RID: 29962
		private bool ownerIsLocal;

		// Token: 0x0400750B RID: 29963
		private float lastTriggerTime = float.MinValue;

		// Token: 0x0400750C RID: 29964
		private readonly Collider[] colliders = new Collider[1];

		// Token: 0x0400750D RID: 29965
		private LayerMask handLayer = 1024;
	}
}
