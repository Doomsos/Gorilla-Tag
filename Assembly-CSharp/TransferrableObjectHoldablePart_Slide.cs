using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000434 RID: 1076
public class TransferrableObjectHoldablePart_Slide : TransferrableObjectHoldablePart
{
	// Token: 0x06001A62 RID: 6754 RVA: 0x0008C5F4 File Offset: 0x0008A7F4
	protected override void UpdateHeld(VRRig rig, bool isHeldLeftHand)
	{
		int num = isHeldLeftHand ? 0 : 1;
		GTPlayer instance = GTPlayer.Instance;
		if (!rig.isOfflineVRRig)
		{
			Vector3 vector = instance.GetHandOffset(isHeldLeftHand) * rig.scaleFactor;
			VRMap vrmap = isHeldLeftHand ? rig.leftHand : rig.rightHand;
			this._snapToLine.target.position = vrmap.GetExtrapolatedControllerPosition() - vector;
			return;
		}
		Transform controllerTransform = instance.GetControllerTransform(num == 0);
		Vector3 position = controllerTransform.position;
		Vector3 snappedPoint = this._snapToLine.GetSnappedPoint(position);
		if (this._maxHandSnapDistance > 0f && (controllerTransform.position - snappedPoint).IsLongerThan(this._maxHandSnapDistance))
		{
			this.OnRelease(null, isHeldLeftHand ? EquipmentInteractor.instance.leftHand : EquipmentInteractor.instance.rightHand);
			return;
		}
		controllerTransform.position = snappedPoint;
		this._snapToLine.target.position = snappedPoint;
	}

	// Token: 0x040023F3 RID: 9203
	[SerializeField]
	private float _maxHandSnapDistance;

	// Token: 0x040023F4 RID: 9204
	[SerializeField]
	private SnapXformToLine _snapToLine;

	// Token: 0x040023F5 RID: 9205
	private const int LEFT = 0;

	// Token: 0x040023F6 RID: 9206
	private const int RIGHT = 1;
}
