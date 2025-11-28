using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x020005BC RID: 1468
public class ControllerInputPoller : MonoBehaviour
{
	// Token: 0x170003AE RID: 942
	// (get) Token: 0x060024FC RID: 9468 RVA: 0x000C6DCE File Offset: 0x000C4FCE
	[DebugReadout]
	public bool leftIndexPressed
	{
		get
		{
			return this._leftIndexPressed;
		}
	}

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x060024FD RID: 9469 RVA: 0x000C6DD6 File Offset: 0x000C4FD6
	[DebugReadout]
	public bool leftIndexReleased
	{
		get
		{
			return this._leftIndexReleased;
		}
	}

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x060024FE RID: 9470 RVA: 0x000C6DDE File Offset: 0x000C4FDE
	[DebugReadout]
	public bool rightIndexPressed
	{
		get
		{
			return this._rightIndexPressed;
		}
	}

	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x060024FF RID: 9471 RVA: 0x000C6DE6 File Offset: 0x000C4FE6
	[DebugReadout]
	public bool rightIndexReleased
	{
		get
		{
			return this._rightIndexReleased;
		}
	}

	// Token: 0x170003B2 RID: 946
	// (get) Token: 0x06002500 RID: 9472 RVA: 0x000C6DEE File Offset: 0x000C4FEE
	[DebugReadout]
	public bool leftIndexPressedThisFrame
	{
		get
		{
			return this._leftIndexPressedThisFrame;
		}
	}

	// Token: 0x170003B3 RID: 947
	// (get) Token: 0x06002501 RID: 9473 RVA: 0x000C6DF6 File Offset: 0x000C4FF6
	[DebugReadout]
	public bool leftIndexReleasedThisFrame
	{
		get
		{
			return this._leftIndexReleasedThisFrame;
		}
	}

	// Token: 0x170003B4 RID: 948
	// (get) Token: 0x06002502 RID: 9474 RVA: 0x000C6DFE File Offset: 0x000C4FFE
	[DebugReadout]
	public bool rightIndexPressedThisFrame
	{
		get
		{
			return this._rightIndexPressedThisFrame;
		}
	}

	// Token: 0x170003B5 RID: 949
	// (get) Token: 0x06002503 RID: 9475 RVA: 0x000C6E06 File Offset: 0x000C5006
	[DebugReadout]
	public bool rightIndexReleasedThisFrame
	{
		get
		{
			return this._rightIndexReleasedThisFrame;
		}
	}

	// Token: 0x170003B6 RID: 950
	// (get) Token: 0x06002504 RID: 9476 RVA: 0x000C6E0E File Offset: 0x000C500E
	[DebugReadout]
	public Vector3 leftVelocity
	{
		get
		{
			return this._leftVelocity;
		}
	}

	// Token: 0x170003B7 RID: 951
	// (get) Token: 0x06002505 RID: 9477 RVA: 0x000C6E16 File Offset: 0x000C5016
	[DebugReadout]
	public Vector3 rightVelocity
	{
		get
		{
			return this._rightVelocity;
		}
	}

	// Token: 0x170003B8 RID: 952
	// (get) Token: 0x06002506 RID: 9478 RVA: 0x000C6E1E File Offset: 0x000C501E
	[DebugReadout]
	public Vector3 leftAngularVelocity
	{
		get
		{
			return this._leftAngularVelocity;
		}
	}

	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x06002507 RID: 9479 RVA: 0x000C6E26 File Offset: 0x000C5026
	[DebugReadout]
	public Vector3 rightAngularVelocity
	{
		get
		{
			return this._rightAngularVelocity;
		}
	}

	// Token: 0x170003BA RID: 954
	// (get) Token: 0x06002508 RID: 9480 RVA: 0x000C6E2E File Offset: 0x000C502E
	// (set) Token: 0x06002509 RID: 9481 RVA: 0x000C6E36 File Offset: 0x000C5036
	public GorillaControllerType controllerType { get; private set; }

	// Token: 0x0600250A RID: 9482 RVA: 0x000C6E3F File Offset: 0x000C503F
	private void Awake()
	{
		if (ControllerInputPoller.instance == null)
		{
			ControllerInputPoller.instance = this;
			return;
		}
		if (ControllerInputPoller.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x000C6E74 File Offset: 0x000C5074
	public static void AddUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Add(callback);
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x000C6EDC File Offset: 0x000C50DC
	public static void RemoveUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Remove(callback);
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x000C6F48 File Offset: 0x000C5148
	public void LateUpdate()
	{
		if (!this.leftControllerDevice.isValid)
		{
			this.leftControllerDevice = InputDevices.GetDeviceAtXRNode(4);
			if (this.leftControllerDevice.isValid)
			{
				this.controllerType = GorillaControllerType.OCULUS_DEFAULT;
				if (this.leftControllerDevice.name.ToLower().Contains("knuckles"))
				{
					this.controllerType = GorillaControllerType.INDEX;
				}
				Debug.Log(string.Format("Found left controller: {0} ControllerType: {1}", this.leftControllerDevice.name, this.controllerType));
			}
		}
		if (!this.rightControllerDevice.isValid)
		{
			this.rightControllerDevice = InputDevices.GetDeviceAtXRNode(5);
		}
		if (!this.headDevice.isValid)
		{
			this.headDevice = InputDevices.GetDeviceAtXRNode(2);
		}
		InputDevice inputDevice = this.leftControllerDevice;
		InputDevice inputDevice2 = this.rightControllerDevice;
		InputDevice inputDevice3 = this.headDevice;
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, ref this.leftControllerPrimaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, ref this.leftControllerSecondaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, ref this.leftControllerPrimaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, ref this.leftControllerSecondaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.grip, ref this.leftControllerGripFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.trigger, ref this.leftControllerIndexFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, ref this.leftControllerPosition);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, ref this.leftControllerRotation);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, ref this.leftControllerPrimary2DAxis);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, ref this.leftControllerTriggerButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, ref this.rightControllerPrimaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, ref this.rightControllerSecondaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, ref this.rightControllerPrimaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, ref this.rightControllerSecondaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.grip, ref this.rightControllerGripFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.trigger, ref this.rightControllerIndexFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, ref this.rightControllerPosition);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, ref this.rightControllerRotation);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, ref this.rightControllerPrimary2DAxis);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, ref this.rightControllerTriggerButton);
		this.leftControllerPrimaryButton = SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(1);
		this.leftControllerSecondaryButton = SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(1);
		this.leftControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_LeftPrimaryTouch.GetState(1);
		this.leftControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_LeftSecondaryTouch.GetState(1);
		this.leftControllerGripFloat = SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(1);
		this.leftControllerIndexFloat = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(1);
		this.leftControllerTriggerButton = SteamVR_Actions.gorillaTag_LeftTriggerClick.GetState(1);
		this.leftControllerPrimary2DAxis = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(1);
		this.rightControllerPrimaryButton = SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(2);
		this.rightControllerSecondaryButton = SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(2);
		this.rightControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_RightPrimaryTouch.GetState(2);
		this.rightControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_RightSecondaryTouch.GetState(2);
		this.rightControllerGripFloat = SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(2);
		this.rightControllerIndexFloat = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(2);
		this.rightControllerTriggerButton = SteamVR_Actions.gorillaTag_RightTriggerClick.GetState(2);
		this.rightControllerPrimary2DAxis = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(2);
		this.headDevice.TryGetFeatureValue(CommonUsages.devicePosition, ref this.headPosition);
		this.headDevice.TryGetFeatureValue(CommonUsages.deviceRotation, ref this.headRotation);
		this.CalculateGrabState(this.leftControllerIndexFloat, ref this._leftIndexPressed, ref this._leftIndexReleased, out this._leftIndexPressedThisFrame, out this._leftIndexReleasedThisFrame, 0.75f, 0.65f);
		this.CalculateGrabState(this.rightControllerIndexFloat, ref this._rightIndexPressed, ref this._rightIndexReleased, out this._rightIndexPressedThisFrame, out this._rightIndexReleasedThisFrame, 0.75f, 0.65f);
		if (this.controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.75f, 0.65f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.75f, 0.65f);
		}
		else if (this.controllerType == GorillaControllerType.INDEX)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.1f, 0.01f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.1f, 0.01f);
		}
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, ref this._leftVelocity);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, ref this._leftAngularVelocity);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, ref this._rightVelocity);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, ref this._rightAngularVelocity);
		this._UpdatePressFlags();
		if (this.didModifyOnUpdate)
		{
			List<Action> list = this.onUpdateNext;
			List<Action> list2 = this.onUpdate;
			this.onUpdate = list;
			this.onUpdateNext = list2;
			this.didModifyOnUpdate = false;
		}
		foreach (Action action in this.onUpdate)
		{
			action.Invoke();
		}
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x000C7514 File Offset: 0x000C5714
	private void CalculateGrabState(float grabValue, ref bool grab, ref bool grabRelease, out bool grabMomentary, out bool grabReleaseMomentary, float grabThreshold, float grabReleaseThreshold)
	{
		bool flag = grabValue >= grabThreshold;
		bool flag2 = grabValue <= grabReleaseThreshold;
		grabMomentary = (flag && !grab);
		grabReleaseMomentary = (flag2 && !grabRelease);
		grab = flag;
		grabRelease = flag2;
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x000C7558 File Offset: 0x000C5758
	public void RecalculateGrabState()
	{
		this.CalculateGrabState(this.leftControllerIndexFloat, ref this._leftIndexPressed, ref this._leftIndexReleased, out this._leftIndexPressedThisFrame, out this._leftIndexReleasedThisFrame, 0.75f, 0.65f);
		this.CalculateGrabState(this.rightControllerIndexFloat, ref this._rightIndexPressed, ref this._rightIndexReleased, out this._rightIndexPressedThisFrame, out this._rightIndexReleasedThisFrame, 0.75f, 0.65f);
		if (this.controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.75f, 0.65f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.75f, 0.65f);
			return;
		}
		if (this.controllerType == GorillaControllerType.INDEX)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.1f, 0.01f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.1f, 0.01f);
		}
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x000C768B File Offset: 0x000C588B
	public static bool GetIndexPressed(XRNode node)
	{
		if (node != 4)
		{
			return node == 5 && ControllerInputPoller.instance.rightIndexPressed;
		}
		return ControllerInputPoller.instance.leftIndexPressed;
	}

	// Token: 0x06002511 RID: 9489 RVA: 0x000C76B0 File Offset: 0x000C58B0
	public static bool GetIndexReleased(XRNode node)
	{
		if (node != 4)
		{
			return node == 5 && ControllerInputPoller.instance.rightIndexReleased;
		}
		return ControllerInputPoller.instance.leftIndexReleased;
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x000C76D5 File Offset: 0x000C58D5
	public static bool GetIndexPressedThisFrame(XRNode node)
	{
		if (node != 4)
		{
			return node == 5 && ControllerInputPoller.instance.leftIndexPressedThisFrame;
		}
		return ControllerInputPoller.instance.leftIndexPressedThisFrame;
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x000C76FA File Offset: 0x000C58FA
	public static bool GetIndexReleasedThisFrame(XRNode node)
	{
		if (node != 4)
		{
			return node == 5 && ControllerInputPoller.instance.leftIndexReleasedThisFrame;
		}
		return ControllerInputPoller.instance.leftIndexReleasedThisFrame;
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x000C771F File Offset: 0x000C591F
	public static bool GetGrab(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftGrab;
		}
		return node == 5 && ControllerInputPoller.instance.rightGrab;
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x000C7744 File Offset: 0x000C5944
	public static bool GetGrabRelease(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftGrabRelease;
		}
		return node == 5 && ControllerInputPoller.instance.rightGrabRelease;
	}

	// Token: 0x06002516 RID: 9494 RVA: 0x000C7769 File Offset: 0x000C5969
	public static bool GetGrabMomentary(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftGrabMomentary;
		}
		return node == 5 && ControllerInputPoller.instance.rightGrabMomentary;
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x000C778E File Offset: 0x000C598E
	public static bool GetGrabReleaseMomentary(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftGrabReleaseMomentary;
		}
		return node == 5 && ControllerInputPoller.instance.rightGrabReleaseMomentary;
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x000C77B3 File Offset: 0x000C59B3
	public static Vector2 Primary2DAxis(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerPrimary2DAxis;
		}
		return ControllerInputPoller.instance.rightControllerPrimary2DAxis;
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x000C77D2 File Offset: 0x000C59D2
	public static bool PrimaryButtonPress(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButton;
		}
		return node == 5 && ControllerInputPoller.instance.rightControllerPrimaryButton;
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000C77F7 File Offset: 0x000C59F7
	public static bool SecondaryButtonPress(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButton;
		}
		return node == 5 && ControllerInputPoller.instance.rightControllerSecondaryButton;
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x000C781C File Offset: 0x000C5A1C
	public static bool PrimaryButtonTouch(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
		}
		return node == 5 && ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
	}

	// Token: 0x0600251C RID: 9500 RVA: 0x000C7841 File Offset: 0x000C5A41
	public static bool SecondaryButtonTouch(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		return node == 5 && ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000C7866 File Offset: 0x000C5A66
	public static float GripFloat(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerGripFloat;
		}
		if (node == 5)
		{
			return ControllerInputPoller.instance.rightControllerGripFloat;
		}
		return 0f;
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x000C788F File Offset: 0x000C5A8F
	public static float TriggerFloat(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerIndexFloat;
		}
		if (node == 5)
		{
			return ControllerInputPoller.instance.rightControllerIndexFloat;
		}
		return 0f;
	}

	// Token: 0x0600251F RID: 9503 RVA: 0x000C78B8 File Offset: 0x000C5AB8
	public static float TriggerTouch(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerIndexTouch;
		}
		if (node == 5)
		{
			return ControllerInputPoller.instance.rightControllerIndexTouch;
		}
		return 0f;
	}

	// Token: 0x06002520 RID: 9504 RVA: 0x000C78E1 File Offset: 0x000C5AE1
	public static Vector3 DevicePosition(XRNode node)
	{
		if (node == 3)
		{
			return ControllerInputPoller.instance.headPosition;
		}
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerPosition;
		}
		if (node == 5)
		{
			return ControllerInputPoller.instance.rightControllerPosition;
		}
		return Vector3.zero;
	}

	// Token: 0x06002521 RID: 9505 RVA: 0x000C791B File Offset: 0x000C5B1B
	public static Quaternion DeviceRotation(XRNode node)
	{
		if (node == 3)
		{
			return ControllerInputPoller.instance.headRotation;
		}
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerRotation;
		}
		if (node == 5)
		{
			return ControllerInputPoller.instance.rightControllerRotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x06002522 RID: 9506 RVA: 0x000C7955 File Offset: 0x000C5B55
	public static Vector3 DeviceVelocity(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftVelocity;
		}
		if (node == 5)
		{
			return ControllerInputPoller.instance.rightVelocity;
		}
		return Vector3.zero;
	}

	// Token: 0x06002523 RID: 9507 RVA: 0x000C797E File Offset: 0x000C5B7E
	public static Vector3 DeviceAngularVelocity(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftAngularVelocity;
		}
		if (node == 5)
		{
			return ControllerInputPoller.instance.rightAngularVelocity;
		}
		return Vector3.zero;
	}

	// Token: 0x06002524 RID: 9508 RVA: 0x000C79A8 File Offset: 0x000C5BA8
	public static bool PositionValid(XRNode node)
	{
		if (node == 3)
		{
			return ControllerInputPoller.instance.headDevice.isValid;
		}
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftControllerDevice.isValid;
		}
		return node == 5 && ControllerInputPoller.instance.rightControllerDevice.isValid;
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x000C79F8 File Offset: 0x000C5BF8
	public static bool HasPressFlags(XRNode node, EControllerInputPressFlags inputStateFlags)
	{
		EControllerInputPressFlags inputStateFlags2 = ControllerInputPoller.GetInputStateFlags(node);
		return inputStateFlags != EControllerInputPressFlags.None && (inputStateFlags2 & inputStateFlags) == inputStateFlags;
	}

	// Token: 0x170003BB RID: 955
	// (get) Token: 0x06002526 RID: 9510 RVA: 0x000C7A17 File Offset: 0x000C5C17
	// (set) Token: 0x06002527 RID: 9511 RVA: 0x000C7A1F File Offset: 0x000C5C1F
	public EControllerInputPressFlags leftPressFlags { get; private set; }

	// Token: 0x170003BC RID: 956
	// (get) Token: 0x06002528 RID: 9512 RVA: 0x000C7A28 File Offset: 0x000C5C28
	// (set) Token: 0x06002529 RID: 9513 RVA: 0x000C7A30 File Offset: 0x000C5C30
	public EControllerInputPressFlags rightPressFlags { get; private set; }

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x0600252A RID: 9514 RVA: 0x000C7A39 File Offset: 0x000C5C39
	// (set) Token: 0x0600252B RID: 9515 RVA: 0x000C7A41 File Offset: 0x000C5C41
	public EControllerInputPressFlags leftPressFlagsLastFrame { get; private set; }

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x0600252C RID: 9516 RVA: 0x000C7A4A File Offset: 0x000C5C4A
	// (set) Token: 0x0600252D RID: 9517 RVA: 0x000C7A52 File Offset: 0x000C5C52
	public EControllerInputPressFlags rightPressFlagsLastFrame { get; private set; }

	// Token: 0x0600252E RID: 9518 RVA: 0x000C7A5B File Offset: 0x000C5C5B
	public static EControllerInputPressFlags GetInputStateFlags(XRNode node)
	{
		if (node == 4)
		{
			return ControllerInputPoller.instance.leftPressFlags;
		}
		if (node != 5)
		{
			return EControllerInputPressFlags.None;
		}
		return ControllerInputPoller.instance.rightPressFlags;
	}

	// Token: 0x0600252F RID: 9519 RVA: 0x000C7A80 File Offset: 0x000C5C80
	public static void AddCallbackOnPressStart(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._AddInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressStart, flags, callback);
	}

	// Token: 0x06002530 RID: 9520 RVA: 0x000C7A8E File Offset: 0x000C5C8E
	public static void AddCallbackOnPressEnd(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._AddInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressEnd, flags, callback);
	}

	// Token: 0x06002531 RID: 9521 RVA: 0x000C7A9C File Offset: 0x000C5C9C
	public static void AddCallbackOnPressUpdate(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._AddInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressUpdate, flags, callback);
	}

	// Token: 0x06002532 RID: 9522 RVA: 0x000C7AAC File Offset: 0x000C5CAC
	private static void _AddInputStateCallback(ref ControllerInputPoller._InputCallbacksCadenceInfo ref_callbacksInfo, EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		if (callback == null || flags == EControllerInputPressFlags.None)
		{
			return;
		}
		if (ref_callbacksInfo.list.Capacity <= ref_callbacksInfo.list.Count)
		{
			ref_callbacksInfo.list.Capacity = ref_callbacksInfo.list.Count * 2;
		}
		ref_callbacksInfo.list.Add(new ControllerInputPoller._InputCallback(flags, callback));
	}

	// Token: 0x06002533 RID: 9523 RVA: 0x000C7B02 File Offset: 0x000C5D02
	public static void RemoveCallbackOnPressStart(Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._RemoveInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressStart, callback);
	}

	// Token: 0x06002534 RID: 9524 RVA: 0x000C7B0F File Offset: 0x000C5D0F
	public static void RemoveCallbackOnPressEnd(Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._RemoveInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressEnd, callback);
	}

	// Token: 0x06002535 RID: 9525 RVA: 0x000C7B1C File Offset: 0x000C5D1C
	public static void RemoveCallbackOnPressUpdate(Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._RemoveInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressUpdate, callback);
	}

	// Token: 0x06002536 RID: 9526 RVA: 0x000C7B2C File Offset: 0x000C5D2C
	private static void _RemoveInputStateCallback(ref ControllerInputPoller._InputCallbacksCadenceInfo ref_callbacksInfo, Action<EHandednessFlags> callback)
	{
		if (callback == null)
		{
			return;
		}
		ref_callbacksInfo.list.RemoveAll((ControllerInputPoller._InputCallback sub) => sub.callback == callback);
	}

	// Token: 0x06002537 RID: 9527 RVA: 0x000C7B68 File Offset: 0x000C5D68
	private void _UpdatePressFlags()
	{
		this.leftPressFlagsLastFrame = this.leftPressFlags;
		this.leftPressFlags = ((this.leftIndexPressed ? EControllerInputPressFlags.Index : EControllerInputPressFlags.None) | (this.leftGrab ? EControllerInputPressFlags.Grip : EControllerInputPressFlags.None) | (this.leftControllerPrimaryButton ? EControllerInputPressFlags.Primary : EControllerInputPressFlags.None) | (this.leftControllerSecondaryButton ? EControllerInputPressFlags.Secondary : EControllerInputPressFlags.None));
		this.rightPressFlagsLastFrame = this.rightPressFlags;
		this.rightPressFlags = ((this.rightIndexPressed ? EControllerInputPressFlags.Index : EControllerInputPressFlags.None) | (this.rightGrab ? EControllerInputPressFlags.Grip : EControllerInputPressFlags.None) | (this.rightControllerPrimaryButton ? EControllerInputPressFlags.Primary : EControllerInputPressFlags.None) | (this.rightControllerSecondaryButton ? EControllerInputPressFlags.Secondary : EControllerInputPressFlags.None));
		ControllerInputPoller._UpdatePressFlags_Callbacks(ref ControllerInputPoller._g_callbacks_onPressStart, ControllerInputPoller._EPressCadence.Start, this.leftPressFlags, this.leftPressFlagsLastFrame, this.rightPressFlags, this.rightPressFlagsLastFrame);
		ControllerInputPoller._UpdatePressFlags_Callbacks(ref ControllerInputPoller._g_callbacks_onPressEnd, ControllerInputPoller._EPressCadence.End, this.leftPressFlags, this.leftPressFlagsLastFrame, this.rightPressFlags, this.rightPressFlagsLastFrame);
		ControllerInputPoller._UpdatePressFlags_Callbacks(ref ControllerInputPoller._g_callbacks_onPressUpdate, ControllerInputPoller._EPressCadence.Held, this.leftPressFlags, this.leftPressFlagsLastFrame, this.rightPressFlags, this.rightPressFlagsLastFrame);
	}

	// Token: 0x06002538 RID: 9528 RVA: 0x000C7C68 File Offset: 0x000C5E68
	[MethodImpl(256)]
	private static void _UpdatePressFlags_Callbacks(ref ControllerInputPoller._InputCallbacksCadenceInfo callbacksInfo, ControllerInputPoller._EPressCadence cadence, EControllerInputPressFlags lFlags_now, EControllerInputPressFlags lFlags_old, EControllerInputPressFlags rFlags_now, EControllerInputPressFlags rFlags_old)
	{
		for (int i = 0; i < callbacksInfo.list.Count; i++)
		{
			EControllerInputPressFlags flags = callbacksInfo.list[i].flags;
			Action<EHandednessFlags> callback = callbacksInfo.list[i].callback;
			EHandednessFlags ehandednessFlags = ControllerInputPoller._IsHandContributingToPressCadence(EHandednessFlags.Left, cadence, flags, lFlags_now, lFlags_old) | ControllerInputPoller._IsHandContributingToPressCadence(EHandednessFlags.Right, cadence, flags, rFlags_now, rFlags_old);
			if (ehandednessFlags != EHandednessFlags.None && callback != null)
			{
				try
				{
					callbacksInfo.list[i].callback.Invoke(ehandednessFlags);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x000C7D00 File Offset: 0x000C5F00
	[MethodImpl(256)]
	private static EHandednessFlags _IsHandContributingToPressCadence(EHandednessFlags hand, ControllerInputPoller._EPressCadence pressCadence, EControllerInputPressFlags cbFlags, EControllerInputPressFlags flags_now, EControllerInputPressFlags flags_old)
	{
		if ((pressCadence != ControllerInputPoller._EPressCadence.Held || (cbFlags & flags_now) != cbFlags) && (pressCadence != ControllerInputPoller._EPressCadence.Start || (cbFlags & flags_now) != cbFlags || (cbFlags & flags_old) == cbFlags) && (pressCadence != ControllerInputPoller._EPressCadence.End || (cbFlags & flags_now) == cbFlags || (cbFlags & flags_old) != cbFlags))
		{
			return EHandednessFlags.None;
		}
		return hand;
	}

	// Token: 0x040030A0 RID: 12448
	public const int k_defaultExecutionOrder = -400;

	// Token: 0x040030A1 RID: 12449
	[OnEnterPlay_SetNull]
	public static volatile ControllerInputPoller instance;

	// Token: 0x040030A2 RID: 12450
	public float leftControllerIndexFloat;

	// Token: 0x040030A3 RID: 12451
	public float leftControllerGripFloat;

	// Token: 0x040030A4 RID: 12452
	public float rightControllerIndexFloat;

	// Token: 0x040030A5 RID: 12453
	public float rightControllerGripFloat;

	// Token: 0x040030A6 RID: 12454
	public float leftControllerIndexTouch;

	// Token: 0x040030A7 RID: 12455
	public float rightControllerIndexTouch;

	// Token: 0x040030A8 RID: 12456
	public float rightStickLRFloat;

	// Token: 0x040030A9 RID: 12457
	public Vector3 leftControllerPosition;

	// Token: 0x040030AA RID: 12458
	public Vector3 rightControllerPosition;

	// Token: 0x040030AB RID: 12459
	public Vector3 headPosition;

	// Token: 0x040030AC RID: 12460
	public Quaternion leftControllerRotation;

	// Token: 0x040030AD RID: 12461
	public Quaternion rightControllerRotation;

	// Token: 0x040030AE RID: 12462
	public Quaternion headRotation;

	// Token: 0x040030AF RID: 12463
	public InputDevice leftControllerDevice;

	// Token: 0x040030B0 RID: 12464
	public InputDevice rightControllerDevice;

	// Token: 0x040030B1 RID: 12465
	public InputDevice headDevice;

	// Token: 0x040030B2 RID: 12466
	public bool leftControllerPrimaryButton;

	// Token: 0x040030B3 RID: 12467
	public bool leftControllerSecondaryButton;

	// Token: 0x040030B4 RID: 12468
	public bool rightControllerPrimaryButton;

	// Token: 0x040030B5 RID: 12469
	public bool rightControllerSecondaryButton;

	// Token: 0x040030B6 RID: 12470
	public bool leftControllerPrimaryButtonTouch;

	// Token: 0x040030B7 RID: 12471
	public bool leftControllerSecondaryButtonTouch;

	// Token: 0x040030B8 RID: 12472
	public bool rightControllerPrimaryButtonTouch;

	// Token: 0x040030B9 RID: 12473
	public bool rightControllerSecondaryButtonTouch;

	// Token: 0x040030BA RID: 12474
	public bool leftControllerTriggerButton;

	// Token: 0x040030BB RID: 12475
	public bool rightControllerTriggerButton;

	// Token: 0x040030BC RID: 12476
	public bool leftGrab;

	// Token: 0x040030BD RID: 12477
	public bool leftGrabRelease;

	// Token: 0x040030BE RID: 12478
	public bool rightGrab;

	// Token: 0x040030BF RID: 12479
	public bool rightGrabRelease;

	// Token: 0x040030C0 RID: 12480
	public bool leftGrabMomentary;

	// Token: 0x040030C1 RID: 12481
	public bool leftGrabReleaseMomentary;

	// Token: 0x040030C2 RID: 12482
	public bool rightGrabMomentary;

	// Token: 0x040030C3 RID: 12483
	public bool rightGrabReleaseMomentary;

	// Token: 0x040030C4 RID: 12484
	private bool _leftIndexPressed;

	// Token: 0x040030C5 RID: 12485
	private bool _leftIndexReleased;

	// Token: 0x040030C6 RID: 12486
	private bool _rightIndexPressed;

	// Token: 0x040030C7 RID: 12487
	private bool _rightIndexReleased;

	// Token: 0x040030C8 RID: 12488
	private bool _leftIndexPressedThisFrame;

	// Token: 0x040030C9 RID: 12489
	private bool _leftIndexReleasedThisFrame;

	// Token: 0x040030CA RID: 12490
	private bool _rightIndexPressedThisFrame;

	// Token: 0x040030CB RID: 12491
	private bool _rightIndexReleasedThisFrame;

	// Token: 0x040030CC RID: 12492
	private Vector3 _leftVelocity;

	// Token: 0x040030CD RID: 12493
	private Vector3 _rightVelocity;

	// Token: 0x040030CE RID: 12494
	private Vector3 _leftAngularVelocity;

	// Token: 0x040030CF RID: 12495
	private Vector3 _rightAngularVelocity;

	// Token: 0x040030D1 RID: 12497
	public Vector2 leftControllerPrimary2DAxis;

	// Token: 0x040030D2 RID: 12498
	public Vector2 rightControllerPrimary2DAxis;

	// Token: 0x040030D3 RID: 12499
	private List<Action> onUpdate = new List<Action>();

	// Token: 0x040030D4 RID: 12500
	private List<Action> onUpdateNext = new List<Action>();

	// Token: 0x040030D5 RID: 12501
	private bool didModifyOnUpdate;

	// Token: 0x040030DA RID: 12506
	private static ControllerInputPoller._InputCallbacksCadenceInfo _g_callbacks_onPressStart = new ControllerInputPoller._InputCallbacksCadenceInfo(32);

	// Token: 0x040030DB RID: 12507
	private static ControllerInputPoller._InputCallbacksCadenceInfo _g_callbacks_onPressEnd = new ControllerInputPoller._InputCallbacksCadenceInfo(32);

	// Token: 0x040030DC RID: 12508
	private static ControllerInputPoller._InputCallbacksCadenceInfo _g_callbacks_onPressUpdate = new ControllerInputPoller._InputCallbacksCadenceInfo(32);

	// Token: 0x020005BD RID: 1469
	private enum _EPressCadence
	{
		// Token: 0x040030DE RID: 12510
		Start,
		// Token: 0x040030DF RID: 12511
		End,
		// Token: 0x040030E0 RID: 12512
		Held
	}

	// Token: 0x020005BE RID: 1470
	private struct _InputCallback
	{
		// Token: 0x0600253C RID: 9532 RVA: 0x000C7D74 File Offset: 0x000C5F74
		public _InputCallback(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
		{
			this.flags = flags;
			this.callback = callback;
		}

		// Token: 0x040030E1 RID: 12513
		public readonly EControllerInputPressFlags flags;

		// Token: 0x040030E2 RID: 12514
		public readonly Action<EHandednessFlags> callback;
	}

	// Token: 0x020005BF RID: 1471
	private struct _InputCallbacksCadenceInfo
	{
		// Token: 0x0600253D RID: 9533 RVA: 0x000C7D84 File Offset: 0x000C5F84
		public _InputCallbacksCadenceInfo(int initialCapacity)
		{
			this.list = new List<ControllerInputPoller._InputCallback>(initialCapacity);
		}

		// Token: 0x040030E3 RID: 12515
		public readonly List<ControllerInputPoller._InputCallback> list;
	}
}
