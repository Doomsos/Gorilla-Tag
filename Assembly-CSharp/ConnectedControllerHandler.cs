using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

internal class ConnectedControllerHandler : MonoBehaviour
{
	public static ConnectedControllerHandler Instance { get; private set; }

	public bool RightValid
	{
		get
		{
			return this.rightValid;
		}
	}

	public bool LeftValid
	{
		get
		{
			return this.leftValid;
		}
	}

	private void Awake()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		ConnectedControllerHandler.Instance = this;
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.rightXRController == null || this.leftXRController == null || this.snapTurnController == null)
		{
			base.enabled = false;
			return;
		}
		this.rightControllerList = new List<XRController>();
		this.leftcontrollerList = new List<XRController>();
		this.rightControllerList.Add(this.rightXRController);
		this.leftcontrollerList.Add(this.leftXRController);
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(5);
		InputDevice deviceAtXRNode2 = InputDevices.GetDeviceAtXRNode(4);
		Debug.Log(string.Format("right controller? {0}", (deviceAtXRNode.characteristics & 576) == 576));
		this.rightControllerValid = deviceAtXRNode.isValid;
		this.leftControllerValid = deviceAtXRNode2.isValid;
		InputDevices.deviceConnected += new Action<InputDevice>(this.DeviceConnected);
		InputDevices.deviceDisconnected += new Action<InputDevice>(this.DeviceDisconnected);
		this.UpdateControllerStates();
	}

	private void Start()
	{
		if (this.leftHandFollower == null || this.rightHandFollower == null || this.leftXRController == null || this.rightXRController == null || this.snapTurnController == null)
		{
			return;
		}
		this.playerHandler = GTPlayer.Instance;
		this.rightHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
		this.leftHandFollower.followTransform = GorillaTagger.Instance.offlineVRRig.transform;
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.ControllerValidator());
	}

	private void OnDisable()
	{
		base.StopCoroutine(this.ControllerValidator());
	}

	private void OnDestroy()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance == this)
		{
			ConnectedControllerHandler.Instance = null;
		}
		InputDevices.deviceConnected -= new Action<InputDevice>(this.DeviceConnected);
		InputDevices.deviceDisconnected -= new Action<InputDevice>(this.DeviceDisconnected);
	}

	private void LateUpdate()
	{
		if (!this.rightValid)
		{
			this.rightHandFollower.UpdatePositionRotation();
		}
		if (!this.leftValid)
		{
			this.leftHandFollower.UpdatePositionRotation();
		}
	}

	private IEnumerator ControllerValidator()
	{
		yield return null;
		this.lastRightPos = ControllerInputPoller.DevicePosition(5);
		this.lastLeftPos = ControllerInputPoller.DevicePosition(4);
		for (;;)
		{
			yield return new WaitForSeconds(this.overridePollRate);
			this.updateControllers = false;
			if (!this.playerHandler.inOverlay)
			{
				if (this.rightControllerValid)
				{
					this.tempRightPos = ControllerInputPoller.DevicePosition(5);
					if (this.tempRightPos == this.lastRightPos)
					{
						if ((this.overrideController & OverrideControllers.RightController) != OverrideControllers.RightController)
						{
							this.overrideController |= OverrideControllers.RightController;
							this.updateControllers = true;
						}
					}
					else if ((this.overrideController & OverrideControllers.RightController) == OverrideControllers.RightController)
					{
						this.overrideController &= ~OverrideControllers.RightController;
						this.updateControllers = true;
					}
					this.lastRightPos = this.tempRightPos;
				}
				if (this.leftControllerValid)
				{
					this.tempLeftPos = ControllerInputPoller.DevicePosition(4);
					if (this.tempLeftPos == this.lastLeftPos)
					{
						if ((this.overrideController & OverrideControllers.LeftController) != OverrideControllers.LeftController)
						{
							this.overrideController |= OverrideControllers.LeftController;
							this.updateControllers = true;
						}
					}
					else if ((this.overrideController & OverrideControllers.LeftController) == OverrideControllers.LeftController)
					{
						this.overrideController &= ~OverrideControllers.LeftController;
						this.updateControllers = true;
					}
					this.lastLeftPos = this.tempLeftPos;
				}
				if (this.updateControllers)
				{
					this.overrideEnabled = (this.overrideController > OverrideControllers.None);
					this.UpdateControllerStates();
				}
			}
		}
		yield break;
	}

	private void DeviceDisconnected(InputDevice device)
	{
		if ((device.characteristics & 576) == 576)
		{
			this.rightControllerValid = false;
		}
		if ((device.characteristics & 320) == 320)
		{
			this.leftControllerValid = false;
		}
		this.UpdateControllerStates();
	}

	private void DeviceConnected(InputDevice device)
	{
		if ((device.characteristics & 576) == 576)
		{
			this.rightControllerValid = true;
		}
		if ((device.characteristics & 320) == 320)
		{
			this.leftControllerValid = true;
		}
		this.UpdateControllerStates();
	}

	private void UpdateControllerStates()
	{
		if (this.overrideEnabled && this.overrideController != OverrideControllers.None)
		{
			this.rightValid = (this.rightControllerValid && (this.overrideController & OverrideControllers.RightController) != OverrideControllers.RightController);
			this.leftValid = (this.leftControllerValid && (this.overrideController & OverrideControllers.LeftController) != OverrideControllers.LeftController);
		}
		else
		{
			this.rightValid = this.rightControllerValid;
			this.leftValid = this.leftControllerValid;
		}
		this.rightXRController.enabled = this.rightValid;
		this.leftXRController.enabled = this.leftValid;
		this.AssignSnapturnController();
	}

	private void AssignSnapturnController()
	{
		if (!this.leftValid && this.rightValid)
		{
			this.snapTurnController.controllers = this.rightControllerList;
			return;
		}
		if (!this.rightValid && this.leftValid)
		{
			this.snapTurnController.controllers = this.leftcontrollerList;
			return;
		}
		this.snapTurnController.controllers = this.rightControllerList;
	}

	public bool GetValidForXRNode(XRNode controllerNode)
	{
		bool result;
		if (controllerNode != 4)
		{
			result = (controllerNode != 5 || this.rightValid);
		}
		else
		{
			result = this.leftValid;
		}
		return result;
	}

	[SerializeField]
	private HandTransformFollowOffest rightHandFollower;

	[SerializeField]
	private HandTransformFollowOffest leftHandFollower;

	[SerializeField]
	private XRController rightXRController;

	[SerializeField]
	private XRController leftXRController;

	[SerializeField]
	private GorillaSnapTurn snapTurnController;

	private List<XRController> rightControllerList;

	private List<XRController> leftcontrollerList;

	private const InputDeviceCharacteristics rightCharecteristics = 576;

	private const InputDeviceCharacteristics leftCharecteristics = 320;

	private bool rightControllerValid = true;

	private bool leftControllerValid = true;

	[SerializeField]
	private bool rightValid = true;

	[SerializeField]
	private bool leftValid = true;

	[SerializeField]
	private Vector3 lastRightPos;

	[SerializeField]
	private Vector3 lastLeftPos;

	private Vector3 tempRightPos;

	private Vector3 tempLeftPos;

	private bool updateControllers;

	private GTPlayer playerHandler;

	[Tooltip("The rate at which controllers are checked to be moving, if they not moving, overrides and enables one hand mode")]
	[SerializeField]
	private float overridePollRate = 15f;

	[SerializeField]
	private bool overrideEnabled;

	[SerializeField]
	private OverrideControllers overrideController;
}
