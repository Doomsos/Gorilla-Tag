using System;
using UnityEngine;

public static class NetInput
{
	public static VRRig LocalPlayerVRRig
	{
		get
		{
			if (NetInput._localPlayerVRRig == null)
			{
				NetInput._localPlayerVRRig = GameObject.Find("Local VRRig").GetComponentInChildren<VRRig>();
			}
			return NetInput._localPlayerVRRig;
		}
	}

	public static NetworkedInput GetInput()
	{
		NetworkedInput result = default(NetworkedInput);
		if (NetInput.LocalPlayerVRRig == null)
		{
			return result;
		}
		result.headRot_LS = NetInput.LocalPlayerVRRig.head.rigTarget.localRotation;
		result.rightHandPos_LS = NetInput.LocalPlayerVRRig.rightHand.rigTarget.localPosition;
		result.rightHandRot_LS = NetInput.LocalPlayerVRRig.rightHand.rigTarget.localRotation;
		result.leftHandPos_LS = NetInput.LocalPlayerVRRig.leftHand.rigTarget.localPosition;
		result.leftHandRot_LS = NetInput.LocalPlayerVRRig.leftHand.rigTarget.localRotation;
		result.handPoseData = NetInput.LocalPlayerVRRig.ReturnHandPosition();
		result.rootPosition = NetInput.LocalPlayerVRRig.transform.position;
		result.rootRotation = NetInput.LocalPlayerVRRig.transform.rotation;
		result.leftThumbTouch = (ControllerInputPoller.PrimaryButtonTouch(4) || ControllerInputPoller.SecondaryButtonTouch(4));
		result.leftThumbPress = (ControllerInputPoller.PrimaryButtonPress(4) || ControllerInputPoller.SecondaryButtonPress(4));
		result.leftIndexValue = ControllerInputPoller.TriggerFloat(4);
		result.leftMiddleValue = ControllerInputPoller.GripFloat(4);
		result.rightThumbTouch = (ControllerInputPoller.PrimaryButtonTouch(5) || ControllerInputPoller.SecondaryButtonPress(5));
		result.rightThumbPress = (ControllerInputPoller.PrimaryButtonPress(5) || ControllerInputPoller.SecondaryButtonPress(5));
		result.rightIndexValue = ControllerInputPoller.TriggerFloat(5);
		result.rightMiddleValue = ControllerInputPoller.GripFloat(5);
		result.scale = NetInput.LocalPlayerVRRig.scaleFactor;
		return result;
	}

	private static VRRig _localPlayerVRRig;
}
