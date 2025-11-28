using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020005B9 RID: 1465
internal class ConnectedControllerHandler : MonoBehaviour
{
	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x060024E5 RID: 9445 RVA: 0x000C67BA File Offset: 0x000C49BA
	// (set) Token: 0x060024E6 RID: 9446 RVA: 0x000C67C1 File Offset: 0x000C49C1
	public static ConnectedControllerHandler Instance { get; private set; }

	// Token: 0x170003AA RID: 938
	// (get) Token: 0x060024E7 RID: 9447 RVA: 0x000C67C9 File Offset: 0x000C49C9
	public bool RightValid
	{
		get
		{
			return this.rightValid;
		}
	}

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x060024E8 RID: 9448 RVA: 0x000C67D1 File Offset: 0x000C49D1
	public bool LeftValid
	{
		get
		{
			return this.leftValid;
		}
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x000C67DC File Offset: 0x000C49DC
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

	// Token: 0x060024EA RID: 9450 RVA: 0x000C6904 File Offset: 0x000C4B04
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

	// Token: 0x060024EB RID: 9451 RVA: 0x000C698B File Offset: 0x000C4B8B
	private void OnEnable()
	{
		base.StartCoroutine(this.ControllerValidator());
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x000C699A File Offset: 0x000C4B9A
	private void OnDisable()
	{
		base.StopCoroutine(this.ControllerValidator());
	}

	// Token: 0x060024ED RID: 9453 RVA: 0x000C69A8 File Offset: 0x000C4BA8
	private void OnDestroy()
	{
		if (ConnectedControllerHandler.Instance != null && ConnectedControllerHandler.Instance == this)
		{
			ConnectedControllerHandler.Instance = null;
		}
		InputDevices.deviceConnected -= new Action<InputDevice>(this.DeviceConnected);
		InputDevices.deviceDisconnected -= new Action<InputDevice>(this.DeviceDisconnected);
	}

	// Token: 0x060024EE RID: 9454 RVA: 0x000C69F7 File Offset: 0x000C4BF7
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

	// Token: 0x060024EF RID: 9455 RVA: 0x000C6A1F File Offset: 0x000C4C1F
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

	// Token: 0x060024F0 RID: 9456 RVA: 0x000C6A2E File Offset: 0x000C4C2E
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

	// Token: 0x060024F1 RID: 9457 RVA: 0x000C6A6C File Offset: 0x000C4C6C
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

	// Token: 0x060024F2 RID: 9458 RVA: 0x000C6AAC File Offset: 0x000C4CAC
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

	// Token: 0x060024F3 RID: 9459 RVA: 0x000C6B4C File Offset: 0x000C4D4C
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

	// Token: 0x060024F4 RID: 9460 RVA: 0x000C6BB0 File Offset: 0x000C4DB0
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

	// Token: 0x04003084 RID: 12420
	[SerializeField]
	private HandTransformFollowOffest rightHandFollower;

	// Token: 0x04003085 RID: 12421
	[SerializeField]
	private HandTransformFollowOffest leftHandFollower;

	// Token: 0x04003086 RID: 12422
	[SerializeField]
	private XRController rightXRController;

	// Token: 0x04003087 RID: 12423
	[SerializeField]
	private XRController leftXRController;

	// Token: 0x04003088 RID: 12424
	[SerializeField]
	private GorillaSnapTurn snapTurnController;

	// Token: 0x04003089 RID: 12425
	private List<XRController> rightControllerList;

	// Token: 0x0400308A RID: 12426
	private List<XRController> leftcontrollerList;

	// Token: 0x0400308B RID: 12427
	private const InputDeviceCharacteristics rightCharecteristics = 576;

	// Token: 0x0400308C RID: 12428
	private const InputDeviceCharacteristics leftCharecteristics = 320;

	// Token: 0x0400308D RID: 12429
	private bool rightControllerValid = true;

	// Token: 0x0400308E RID: 12430
	private bool leftControllerValid = true;

	// Token: 0x0400308F RID: 12431
	[SerializeField]
	private bool rightValid = true;

	// Token: 0x04003090 RID: 12432
	[SerializeField]
	private bool leftValid = true;

	// Token: 0x04003091 RID: 12433
	[SerializeField]
	private Vector3 lastRightPos;

	// Token: 0x04003092 RID: 12434
	[SerializeField]
	private Vector3 lastLeftPos;

	// Token: 0x04003093 RID: 12435
	private Vector3 tempRightPos;

	// Token: 0x04003094 RID: 12436
	private Vector3 tempLeftPos;

	// Token: 0x04003095 RID: 12437
	private bool updateControllers;

	// Token: 0x04003096 RID: 12438
	private GTPlayer playerHandler;

	// Token: 0x04003097 RID: 12439
	[Tooltip("The rate at which controllers are checked to be moving, if they not moving, overrides and enables one hand mode")]
	[SerializeField]
	private float overridePollRate = 15f;

	// Token: 0x04003098 RID: 12440
	[SerializeField]
	private bool overrideEnabled;

	// Token: 0x04003099 RID: 12441
	[SerializeField]
	private OverrideControllers overrideController;
}
