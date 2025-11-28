using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200109E RID: 4254
	public class RCRemoteHoldable : TransferrableObject, ISnapTurnOverride
	{
		// Token: 0x170009FF RID: 2559
		// (get) Token: 0x06006A6C RID: 27244 RVA: 0x0022E228 File Offset: 0x0022C428
		public XRNode XRNode
		{
			get
			{
				return this.xrNode;
			}
		}

		// Token: 0x17000A00 RID: 2560
		// (get) Token: 0x06006A6D RID: 27245 RVA: 0x0022E230 File Offset: 0x0022C430
		public RCVehicle Vehicle
		{
			get
			{
				return this.targetVehicle;
			}
		}

		// Token: 0x06006A6E RID: 27246 RVA: 0x0022E238 File Offset: 0x0022C438
		public bool TurnOverrideActive()
		{
			return base.gameObject.activeSelf && this.currentlyHeld && this.xrNode == 5;
		}

		// Token: 0x06006A6F RID: 27247 RVA: 0x0022E25C File Offset: 0x0022C45C
		protected override void Awake()
		{
			base.Awake();
			this.initialJoystickRotation = this.joystickTransform.localRotation;
			this.initialTriggerRotation = this.triggerTransform.localRotation;
			if (this.buttonTransform != null)
			{
				this.initialButtonRotation = this.buttonTransform.localRotation;
				this.initialButtonPosition = this.buttonTransform.localPosition;
			}
		}

		// Token: 0x06006A70 RID: 27248 RVA: 0x0022E2C4 File Offset: 0x0022C4C4
		internal override void OnEnable()
		{
			base.OnEnable();
			if (!this._TryFindRemoteVehicle())
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (this._events.IsNotNull() || base.gameObject.TryGetComponent<RubberDuckEvents>(ref this._events))
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
				else
				{
					Debug.LogError("Failed to get a reference to the Photon Player needed to hook up the cosmetic event");
				}
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnStartConnectionEvent);
			}
			this.WakeUpRemoteVehicle();
		}

		// Token: 0x06006A71 RID: 27249 RVA: 0x0022E3B4 File Offset: 0x0022C5B4
		internal override void OnDisable()
		{
			base.OnDisable();
			GorillaSnapTurn gorillaSnapTurn = (GorillaTagger.Instance != null) ? GorillaTagger.Instance.GetComponent<GorillaSnapTurn>() : null;
			if (gorillaSnapTurn != null)
			{
				gorillaSnapTurn.UnsetTurningOverride(this);
			}
			if (this.networkSync != null && this.networkSync.photonView.IsMine)
			{
				PhotonNetwork.Destroy(this.networkSync.gameObject);
				this.networkSync = null;
			}
			if (this._events.IsNotNull())
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnStartConnectionEvent);
			}
		}

		// Token: 0x06006A72 RID: 27250 RVA: 0x0022E458 File Offset: 0x0022C658
		protected override void OnDestroy()
		{
			base.OnDestroy();
			GorillaSnapTurn gorillaSnapTurn = (GorillaTagger.Instance != null) ? GorillaTagger.Instance.GetComponent<GorillaSnapTurn>() : null;
			if (gorillaSnapTurn != null)
			{
				gorillaSnapTurn.UnsetTurningOverride(this);
			}
		}

		// Token: 0x06006A73 RID: 27251 RVA: 0x0022E498 File Offset: 0x0022C698
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			base.OnGrab(pointGrabbed, grabbingHand);
			if (PhotonNetwork.InRoom && this.networkSync != null && this.networkSync.photonView.Owner == null)
			{
				PhotonNetwork.Destroy(this.networkSync.gameObject);
				this.networkSync = null;
			}
			if (this.networkSync == null && PhotonNetwork.InRoom)
			{
				object[] array = new object[]
				{
					this.myIndex
				};
				GameObject gameObject = PhotonNetwork.Instantiate(this.networkSyncPrefabName, Vector3.zero, Quaternion.identity, 0, array);
				this.networkSync = ((gameObject != null) ? gameObject.GetComponent<RCCosmeticNetworkSync>() : null);
			}
			this.currentlyHeld = true;
			bool flag = grabbingHand == EquipmentInteractor.instance.rightHand;
			this.xrNode = (flag ? 5 : 4);
			GorillaSnapTurn component = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			if (flag)
			{
				component.SetTurningOverride(this);
			}
			else
			{
				component.UnsetTurningOverride(this);
			}
			if (this.targetVehicle != null)
			{
				this.targetVehicle.StartConnection(this, this.networkSync);
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(this.emptyArgs);
			}
		}

		// Token: 0x06006A74 RID: 27252 RVA: 0x0022E5E8 File Offset: 0x0022C7E8
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			this.currentlyHeld = false;
			this.currentInput = default(RCRemoteHoldable.RCInput);
			if (this.targetVehicle != null)
			{
				this.targetVehicle.EndConnection();
			}
			this.joystickTransform.localRotation = this.initialJoystickRotation;
			this.triggerTransform.localRotation = this.initialTriggerRotation;
			GorillaTagger.Instance.GetComponent<GorillaSnapTurn>().UnsetTurningOverride(this);
			return true;
		}

		// Token: 0x06006A75 RID: 27253 RVA: 0x0022E660 File Offset: 0x0022C860
		private void Update()
		{
			if (this.currentlyHeld)
			{
				this.currentInput.joystick = ControllerInputPoller.Primary2DAxis(this.xrNode);
				this.currentInput.trigger = ControllerInputPoller.TriggerFloat(this.xrNode);
				this.currentInput.buttons = (ControllerInputPoller.PrimaryButtonPress(this.xrNode) ? 1 : 0);
				if (this.targetVehicle != null)
				{
					this.targetVehicle.ApplyRemoteControlInput(this.currentInput);
				}
				this.joystickTransform.localRotation = this.initialJoystickRotation * Quaternion.Euler(this.joystickLeanDegrees * this.currentInput.joystick.y, 0f, -this.joystickLeanDegrees * this.currentInput.joystick.x);
				this.triggerTransform.localRotation = this.initialTriggerRotation * Quaternion.Euler(this.triggerPullDegrees * this.currentInput.trigger, 0f, 0f);
				if (this.buttonTransform != null)
				{
					this.buttonTransform.localPosition = this.initialButtonPosition + this.initialButtonRotation * new Vector3(0f, 0f, -this.buttonPressDepth * (float)((this.currentInput.buttons > 0) ? 1 : 0));
				}
			}
		}

		// Token: 0x06006A76 RID: 27254 RVA: 0x0022E7BF File Offset: 0x0022C9BF
		public void OnStartConnectionEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			this.WakeUpRemoteVehicle();
		}

		// Token: 0x06006A77 RID: 27255 RVA: 0x0022E7E6 File Offset: 0x0022C9E6
		public void WakeUpRemoteVehicle()
		{
			if (this.networkSync != null && this.targetVehicle.IsNotNull() && !this.targetVehicle.HasLocalAuthority)
			{
				this.targetVehicle.WakeUpRemote(this.networkSync);
			}
		}

		// Token: 0x06006A78 RID: 27256 RVA: 0x0022E824 File Offset: 0x0022CA24
		private bool _TryFindRemoteVehicle()
		{
			if (this.targetVehicle != null)
			{
				return true;
			}
			VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
			if (componentInParent.IsNull())
			{
				Debug.LogError("RCRemoteHoldable: unable to find parent vrrig");
				return false;
			}
			CosmeticItemInstance cosmeticItemInstance = componentInParent.cosmeticsObjectRegistry.Cosmetic(base.name);
			int instanceID = base.gameObject.GetInstanceID();
			return this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceID, cosmeticItemInstance.objects) || this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceID, cosmeticItemInstance.leftObjects) || this._TryFindRemoteVehicle_InCosmeticInstanceArray(instanceID, cosmeticItemInstance.rightObjects);
		}

		// Token: 0x06006A79 RID: 27257 RVA: 0x0022E8B0 File Offset: 0x0022CAB0
		private bool _TryFindRemoteVehicle_InCosmeticInstanceArray(int thisGobjInstId, List<GameObject> gameObjects)
		{
			foreach (GameObject gameObject in gameObjects)
			{
				if (gameObject.GetInstanceID() != thisGobjInstId)
				{
					this.targetVehicle = gameObject.GetComponentInChildren<RCVehicle>(true);
					if (this.targetVehicle != null)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x04007A7B RID: 31355
		[SerializeField]
		private Transform joystickTransform;

		// Token: 0x04007A7C RID: 31356
		[SerializeField]
		private Transform triggerTransform;

		// Token: 0x04007A7D RID: 31357
		[SerializeField]
		private Transform buttonTransform;

		// Token: 0x04007A7E RID: 31358
		private RCVehicle targetVehicle;

		// Token: 0x04007A7F RID: 31359
		private float joystickLeanDegrees = 30f;

		// Token: 0x04007A80 RID: 31360
		private float triggerPullDegrees = 40f;

		// Token: 0x04007A81 RID: 31361
		private float buttonPressDepth = 0.005f;

		// Token: 0x04007A82 RID: 31362
		private Quaternion initialJoystickRotation;

		// Token: 0x04007A83 RID: 31363
		private Quaternion initialTriggerRotation;

		// Token: 0x04007A84 RID: 31364
		private Quaternion initialButtonRotation;

		// Token: 0x04007A85 RID: 31365
		private Vector3 initialButtonPosition;

		// Token: 0x04007A86 RID: 31366
		private bool currentlyHeld;

		// Token: 0x04007A87 RID: 31367
		private XRNode xrNode;

		// Token: 0x04007A88 RID: 31368
		private RCRemoteHoldable.RCInput currentInput;

		// Token: 0x04007A89 RID: 31369
		[HideInInspector]
		public RCCosmeticNetworkSync networkSync;

		// Token: 0x04007A8A RID: 31370
		private string networkSyncPrefabName = "RCCosmeticNetworkSync";

		// Token: 0x04007A8B RID: 31371
		private RubberDuckEvents _events;

		// Token: 0x04007A8C RID: 31372
		private object[] emptyArgs = new object[0];

		// Token: 0x0200109F RID: 4255
		public struct RCInput
		{
			// Token: 0x04007A8D RID: 31373
			public Vector2 joystick;

			// Token: 0x04007A8E RID: 31374
			public float trigger;

			// Token: 0x04007A8F RID: 31375
			public byte buttons;
		}
	}
}
