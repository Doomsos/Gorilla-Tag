using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001094 RID: 4244
	public class DiceHoldable : TransferrableObject
	{
		// Token: 0x06006A32 RID: 27186 RVA: 0x0022A570 File Offset: 0x00228770
		internal override void OnEnable()
		{
			base.OnEnable();
			if (this._events == null)
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
			}
			if (this._events != null)
			{
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnDiceEvent);
			}
		}

		// Token: 0x06006A33 RID: 27187 RVA: 0x0022A640 File Offset: 0x00228840
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnDiceEvent);
				Object.Destroy(this._events);
				this._events = null;
			}
		}

		// Token: 0x06006A34 RID: 27188 RVA: 0x0022A698 File Offset: 0x00228898
		private void OnDiceEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			GorillaNot.IncrementRPCCall(info, "OnDiceEvent");
			if (sender != target)
			{
				return;
			}
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			if ((bool)args[0])
			{
				Vector3 position = base.transform.position;
				Vector3 forward = base.transform.forward;
				Vector3 vector = (Vector3)args[1];
				ref position.SetValueSafe(vector);
				vector = (Vector3)args[2];
				ref forward.SetValueSafe(vector);
				float playerScale = ((float)args[3]).ClampSafe(0.01f, 1f);
				int landingSide = Mathf.Clamp((int)args[4], 1, 20);
				double finite = ((double)args[5]).GetFinite();
				this.ThrowDiceLocal(position, forward, playerScale, landingSide, finite);
				return;
			}
			this.dicePhysics.EndThrow();
		}

		// Token: 0x06006A35 RID: 27189 RVA: 0x0022A76C File Offset: 0x0022896C
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			if (this.dicePhysics.enabled)
			{
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						false
					};
					this._events.Activate.RaiseOthers(args);
				}
				base.transform.position = this.dicePhysics.transform.position;
				base.transform.rotation = this.dicePhysics.transform.rotation;
				this.dicePhysics.EndThrow();
				if (grabbingHand == EquipmentInteractor.instance.leftHand && this.currentState == TransferrableObject.PositionState.OnLeftArm)
				{
					this.canAutoGrabLeft = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InLeftHand;
				}
				else if (grabbingHand == EquipmentInteractor.instance.rightHand && this.currentState == TransferrableObject.PositionState.OnRightArm)
				{
					this.canAutoGrabRight = false;
					this.interpState = TransferrableObject.InterpolateState.None;
					this.currentState = TransferrableObject.PositionState.InLeftHand;
				}
			}
			base.OnGrab(pointGrabbed, grabbingHand);
		}

		// Token: 0x06006A36 RID: 27190 RVA: 0x0022A884 File Offset: 0x00228A84
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (zoneReleased == null)
			{
				Vector3 position = base.transform.position;
				bool isLeftHand = releasingHand == EquipmentInteractor.instance.leftHand;
				Vector3 averageVelocity = GTPlayer.Instance.GetInteractPointVelocityTracker(isLeftHand).GetAverageVelocity(true, 0.15f, false);
				int randomSide = this.dicePhysics.GetRandomSide();
				double num = PhotonNetwork.InRoom ? PhotonNetwork.Time : -1.0;
				float scale = GTPlayer.Instance.scale;
				if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					object[] args = new object[]
					{
						true,
						position,
						averageVelocity,
						scale,
						randomSide,
						num
					};
					this._events.Activate.RaiseOthers(args);
				}
				this.ThrowDiceLocal(position, averageVelocity, scale, randomSide, num);
			}
			return true;
		}

		// Token: 0x06006A37 RID: 27191 RVA: 0x0022A999 File Offset: 0x00228B99
		private void ThrowDiceLocal(Vector3 startPosition, Vector3 throwVelocity, float playerScale, int landingSide, double startTime)
		{
			this.dicePhysics.StartThrow(this, startPosition, throwVelocity, playerScale, landingSide, startTime);
		}

		// Token: 0x040079B9 RID: 31161
		[SerializeField]
		private DicePhysics dicePhysics;

		// Token: 0x040079BA RID: 31162
		private RubberDuckEvents _events;
	}
}
