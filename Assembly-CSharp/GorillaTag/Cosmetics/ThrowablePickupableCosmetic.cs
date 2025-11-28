using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010B8 RID: 4280
	public class ThrowablePickupableCosmetic : TransferrableObject
	{
		// Token: 0x06006B32 RID: 27442 RVA: 0x0023291D File Offset: 0x00230B1D
		private new void Awake()
		{
			this.transferrableObject = base.GetComponent<TransferrableObject>();
		}

		// Token: 0x06006B33 RID: 27443 RVA: 0x0023292C File Offset: 0x00230B2C
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				this.owner = ((this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null));
				if (this.owner != null)
				{
					this._events.Init(this.owner);
					this.isLocal = this.owner.IsLocal;
				}
			}
			if (this._events != null)
			{
				this._events.Activate.reliable = true;
				this._events.Deactivate.reliable = true;
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReleaseEvent);
				this._events.Deactivate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReturnToDockEvent);
			}
		}

		// Token: 0x06006B34 RID: 27444 RVA: 0x00232A60 File Offset: 0x00230C60
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReleaseEvent);
				this._events.Deactivate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReturnToDockEvent);
				this._events.Dispose();
				this._events = null;
			}
			if (this.pickupableVariant != null && this.pickupableVariant.enabled)
			{
				this.pickupableVariant.DelayedPickup();
			}
		}

		// Token: 0x06006B35 RID: 27445 RVA: 0x00232B00 File Offset: 0x00230D00
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return;
			}
			if (this.pickupableVariant.enabled)
			{
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					this._events.Activate.RaiseOthers(new object[]
					{
						false
					});
				}
				base.transform.position = this.pickupableVariant.transform.position;
				base.transform.rotation = this.pickupableVariant.transform.rotation;
				this.pickupableVariant.Pickup(false);
				if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState == TransferrableObject.PositionState.OnLeftArm)
				{
					this.canAutoGrabLeft = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InRightHand;
				}
				else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState == TransferrableObject.PositionState.OnRightArm)
				{
					this.canAutoGrabRight = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InLeftHand;
				}
			}
			UnityEvent onGrabLocal = this.OnGrabLocal;
			if (onGrabLocal != null)
			{
				onGrabLocal.Invoke();
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x06006B36 RID: 27446 RVA: 0x00232C44 File Offset: 0x00230E44
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (!(VRRigCache.Instance.localRig.Rig == this.ownerRig))
			{
				return false;
			}
			Vector3 position = base.transform.position;
			bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
			Vector3 averageVelocity = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand).GetAverageVelocity(true, 0.15f, false);
			float scale = GTPlayer.Instance.scale;
			bool flag = this.DistanceToDock() > this.returnToDockDistanceThreshold;
			if (PhotonNetwork.InRoom && this._events != null)
			{
				if (flag && this._events.Activate != null)
				{
					this._events.Activate.RaiseAll(new object[]
					{
						true,
						position,
						averageVelocity,
						scale
					});
				}
				else if (!flag && this._events.Deactivate != null)
				{
					this._events.Deactivate.RaiseAll(Array.Empty<object>());
					UnityEvent onReturnToDockPositionLocal = this.OnReturnToDockPositionLocal;
					if (onReturnToDockPositionLocal != null)
					{
						onReturnToDockPositionLocal.Invoke();
					}
				}
			}
			else if (flag)
			{
				this.OnReleaseEventLocal(position, averageVelocity, scale);
			}
			else
			{
				UnityEvent onReturnToDockPositionLocal2 = this.OnReturnToDockPositionLocal;
				if (onReturnToDockPositionLocal2 != null)
				{
					onReturnToDockPositionLocal2.Invoke();
				}
				UnityEvent onReturnToDockPositionShared = this.OnReturnToDockPositionShared;
				if (onReturnToDockPositionShared != null)
				{
					onReturnToDockPositionShared.Invoke();
				}
			}
			return true;
		}

		// Token: 0x06006B37 RID: 27447 RVA: 0x00232DAC File Offset: 0x00230FAC
		private void OnReleaseEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnReleaseEvent");
			if (!this.callLimiterRelease.CheckCallTime(Time.time))
			{
				return;
			}
			object obj = args[0];
			if (obj is bool)
			{
				bool flag = (bool)obj;
				if (flag)
				{
					obj = args[1];
					if (obj is Vector3)
					{
						Vector3 vector = (Vector3)obj;
						obj = args[2];
						if (obj is Vector3)
						{
							Vector3 inVel = (Vector3)obj;
							obj = args[3];
							if (obj is float)
							{
								float value = (float)obj;
								Vector3 position = base.transform.position;
								Vector3 releaseVelocity = base.transform.forward;
								ref position.SetValueSafe(vector);
								if (this.ownerRig.IsPositionInRange(position, 20f))
								{
									releaseVelocity = this.ownerRig.ClampVelocityRelativeToPlayerSafe(inVel, 50f, 100f);
									float playerScale = value.ClampSafe(0.01f, 1f);
									this.OnReleaseEventLocal(position, releaseVelocity, playerScale);
									return;
								}
								return;
							}
						}
					}
					return;
				}
				this.pickupableVariant.Pickup(false);
				return;
			}
		}

		// Token: 0x06006B38 RID: 27448 RVA: 0x00232ECC File Offset: 0x002310CC
		private void OnReturnToDockEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnReturnToDockEvent");
			if (!this.callLimiterReturn.CheckCallTime(Time.time))
			{
				return;
			}
			UnityEvent onReturnToDockPositionShared = this.OnReturnToDockPositionShared;
			if (onReturnToDockPositionShared == null)
			{
				return;
			}
			onReturnToDockPositionShared.Invoke();
		}

		// Token: 0x06006B39 RID: 27449 RVA: 0x00232F27 File Offset: 0x00231127
		private void OnReleaseEventLocal(Vector3 startPosition, Vector3 releaseVelocity, float playerScale)
		{
			this.pickupableVariant.Release(this, startPosition, releaseVelocity, playerScale);
		}

		// Token: 0x06006B3A RID: 27450 RVA: 0x00232F38 File Offset: 0x00231138
		private float DistanceToDock()
		{
			float result = 0f;
			if (this.currentState == TransferrableObject.PositionState.OnRightShoulder)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightBackTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftShoulder)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftBackTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnRightArm)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.rightArmTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnLeftArm)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.leftArmTransform.position, base.transform.position);
			}
			else if (this.currentState == TransferrableObject.PositionState.OnChest)
			{
				result = Vector3.Distance(this.ownerRig.myBodyDockPositions.chestTransform.position, base.transform.position);
			}
			return result;
		}

		// Token: 0x04007B86 RID: 31622
		[Tooltip("Child object with the PickupableCosmetic script")]
		[SerializeField]
		private PickupableVariant pickupableVariant;

		// Token: 0x04007B87 RID: 31623
		[Tooltip("cosmetics released at a greater distance from the dock than the threshold will be placed in world instead of returning to the dock")]
		[SerializeField]
		private float returnToDockDistanceThreshold = 0.7f;

		// Token: 0x04007B88 RID: 31624
		[FormerlySerializedAs("OnReturnToDockPosition")]
		[Space]
		public UnityEvent OnReturnToDockPositionLocal;

		// Token: 0x04007B89 RID: 31625
		public UnityEvent OnReturnToDockPositionShared;

		// Token: 0x04007B8A RID: 31626
		[FormerlySerializedAs("OnGrabFromDockPosition")]
		public UnityEvent OnGrabLocal;

		// Token: 0x04007B8B RID: 31627
		private RubberDuckEvents _events;

		// Token: 0x04007B8C RID: 31628
		private TransferrableObject transferrableObject;

		// Token: 0x04007B8D RID: 31629
		private bool isLocal;

		// Token: 0x04007B8E RID: 31630
		private NetPlayer owner;

		// Token: 0x04007B8F RID: 31631
		private CallLimiter callLimiterRelease = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04007B90 RID: 31632
		private CallLimiter callLimiterReturn = new CallLimiter(10, 2f, 0.5f);
	}
}
