using System;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010C1 RID: 4289
	public class ChickenSword : MonoBehaviour
	{
		// Token: 0x06006B96 RID: 27542 RVA: 0x00234D58 File Offset: 0x00232F58
		private void Awake()
		{
			this.lastHitTime = float.PositiveInfinity;
			this.SwitchState(ChickenSword.SwordState.Ready);
		}

		// Token: 0x06006B97 RID: 27543 RVA: 0x00234D6C File Offset: 0x00232F6C
		internal void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? ((this.transferrableObject.myRig.creator != null) ? this.transferrableObject.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
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
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReachedLastTransformationStep);
			}
		}

		// Token: 0x06006B98 RID: 27544 RVA: 0x00234E50 File Offset: 0x00233050
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnReachedLastTransformationStep);
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006B99 RID: 27545 RVA: 0x00234EA0 File Offset: 0x002330A0
		private void Update()
		{
			ChickenSword.SwordState swordState = this.currentState;
			if (swordState != ChickenSword.SwordState.Ready)
			{
				if (swordState != ChickenSword.SwordState.Deflated)
				{
					return;
				}
				if (Time.time - this.lastHitTime > this.rechargeCooldown)
				{
					this.lastHitTime = float.PositiveInfinity;
					this.SwitchState(ChickenSword.SwordState.Ready);
					UnityEvent onRechargedShared = this.OnRechargedShared;
					if (onRechargedShared != null)
					{
						onRechargedShared.Invoke();
					}
					if (this.transferrableObject && this.transferrableObject.IsMyItem())
					{
						UnityEvent<bool> onRechargedLocal = this.OnRechargedLocal;
						if (onRechargedLocal == null)
						{
							return;
						}
						onRechargedLocal.Invoke(this.transferrableObject.InLeftHand());
					}
				}
			}
			else if (this.hitReceievd)
			{
				this.hitReceievd = false;
				this.lastHitTime = Time.time;
				this.SwitchState(ChickenSword.SwordState.Deflated);
				UnityEvent onDeflatedShared = this.OnDeflatedShared;
				if (onDeflatedShared != null)
				{
					onDeflatedShared.Invoke();
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					UnityEvent<bool> onDeflatedLocal = this.OnDeflatedLocal;
					if (onDeflatedLocal == null)
					{
						return;
					}
					onDeflatedLocal.Invoke(this.transferrableObject.InLeftHand());
					return;
				}
			}
		}

		// Token: 0x06006B9A RID: 27546 RVA: 0x00234F9C File Offset: 0x0023319C
		public void OnHitTargetSync(VRRig playerRig)
		{
			if (this.velocityTracker == null)
			{
				return;
			}
			Vector3 averageVelocity = this.velocityTracker.GetAverageVelocity(true, 0.15f, false);
			if (this.currentState == ChickenSword.SwordState.Ready && averageVelocity.magnitude > this.hitVelocityThreshold)
			{
				this.hitReceievd = true;
				UnityEvent<VRRig> onHitTargetShared = this.OnHitTargetShared;
				if (onHitTargetShared != null)
				{
					onHitTargetShared.Invoke(playerRig);
				}
				if (this.transferrableObject && this.transferrableObject.IsMyItem())
				{
					bool flag = this.transferrableObject.InLeftHand();
					UnityEvent<bool> onHitTargetLocal = this.OnHitTargetLocal;
					if (onHitTargetLocal != null)
					{
						onHitTargetLocal.Invoke(flag);
					}
				}
				if (this.cosmeticSwapper != null && playerRig == GorillaTagger.Instance.offlineVRRig && this.cosmeticSwapper.GetCurrentStepIndex(playerRig) >= this.cosmeticSwapper.GetNumberOfSteps() && PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
				{
					this._events.Activate.RaiseAll(Array.Empty<object>());
				}
			}
		}

		// Token: 0x06006B9B RID: 27547 RVA: 0x002350B0 File Offset: 0x002332B0
		private void OnReachedLastTransformationStep(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnReachedLastTransformationStep");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(info.Sender.ActorNumber), out rigContainer) && rigContainer.Rig.IsPositionInRange(base.transform.position, 6f))
			{
				UnityEvent<VRRig> onReachedLastTransformationStepShared = this.OnReachedLastTransformationStepShared;
				if (onReachedLastTransformationStepShared == null)
				{
					return;
				}
				onReachedLastTransformationStepShared.Invoke(rigContainer.Rig);
			}
		}

		// Token: 0x06006B9C RID: 27548 RVA: 0x00235138 File Offset: 0x00233338
		private void SwitchState(ChickenSword.SwordState newState)
		{
			this.currentState = newState;
		}

		// Token: 0x04007C01 RID: 31745
		[SerializeField]
		private float rechargeCooldown;

		// Token: 0x04007C02 RID: 31746
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04007C03 RID: 31747
		[SerializeField]
		private float hitVelocityThreshold;

		// Token: 0x04007C04 RID: 31748
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04007C05 RID: 31749
		[SerializeField]
		private CosmeticSwapper cosmeticSwapper;

		// Token: 0x04007C06 RID: 31750
		[Space]
		[Space]
		public UnityEvent OnDeflatedShared;

		// Token: 0x04007C07 RID: 31751
		public UnityEvent<bool> OnDeflatedLocal;

		// Token: 0x04007C08 RID: 31752
		public UnityEvent OnRechargedShared;

		// Token: 0x04007C09 RID: 31753
		public UnityEvent<bool> OnRechargedLocal;

		// Token: 0x04007C0A RID: 31754
		public UnityEvent<VRRig> OnHitTargetShared;

		// Token: 0x04007C0B RID: 31755
		public UnityEvent<bool> OnHitTargetLocal;

		// Token: 0x04007C0C RID: 31756
		public UnityEvent<VRRig> OnReachedLastTransformationStepShared;

		// Token: 0x04007C0D RID: 31757
		private float lastHitTime;

		// Token: 0x04007C0E RID: 31758
		private ChickenSword.SwordState currentState;

		// Token: 0x04007C0F RID: 31759
		private bool hitReceievd;

		// Token: 0x04007C10 RID: 31760
		private RubberDuckEvents _events;

		// Token: 0x04007C11 RID: 31761
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x020010C2 RID: 4290
		private enum SwordState
		{
			// Token: 0x04007C13 RID: 31763
			Ready,
			// Token: 0x04007C14 RID: 31764
			Deflated
		}
	}
}
