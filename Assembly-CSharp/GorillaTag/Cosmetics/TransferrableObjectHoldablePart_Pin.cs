using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200111F RID: 4383
	public class TransferrableObjectHoldablePart_Pin : TransferrableObjectHoldablePart
	{
		// Token: 0x06006DB2 RID: 28082 RVA: 0x0024044D File Offset: 0x0023E64D
		protected void OnEnable()
		{
			UnityEvent onEnableHoldable = this.OnEnableHoldable;
			if (onEnableHoldable == null)
			{
				return;
			}
			onEnableHoldable.Invoke();
		}

		// Token: 0x06006DB3 RID: 28083 RVA: 0x00240460 File Offset: 0x0023E660
		protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
		{
			if (rig.isOfflineVRRig)
			{
				Transform controllerTransform = GTPlayer.Instance.GetControllerTransform(isHeldLeftHand);
				if (GTPlayer.Instance.GetInteractPointVelocityTracker(isHeldLeftHand).GetAverageVelocity(true, 0.15f, false).magnitude > this.breakStrengthThreshold || (controllerTransform.position - this.pin.transform.position).IsLongerThan(this.maxHandSnapDistance))
				{
					this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
					UnityEvent onBreak = this.OnBreak;
					if (onBreak != null)
					{
						onBreak.Invoke();
					}
					if (this.transferrableParentObject && this.transferrableParentObject.IsMyItem())
					{
						UnityEvent onBreakLocal = this.OnBreakLocal;
						if (onBreakLocal == null)
						{
							return;
						}
						onBreakLocal.Invoke();
					}
					return;
				}
				controllerTransform.position = this.pin.position;
			}
		}

		// Token: 0x04007F2E RID: 32558
		[SerializeField]
		private float breakStrengthThreshold = 0.8f;

		// Token: 0x04007F2F RID: 32559
		[SerializeField]
		private float maxHandSnapDistance = 0.5f;

		// Token: 0x04007F30 RID: 32560
		[SerializeField]
		private Transform pin;

		// Token: 0x04007F31 RID: 32561
		public UnityEvent OnBreak;

		// Token: 0x04007F32 RID: 32562
		public UnityEvent OnBreakLocal;

		// Token: 0x04007F33 RID: 32563
		public UnityEvent OnEnableHoldable;
	}
}
