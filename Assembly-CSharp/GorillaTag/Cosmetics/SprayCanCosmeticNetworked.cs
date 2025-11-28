using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001119 RID: 4377
	public class SprayCanCosmeticNetworked : MonoBehaviour
	{
		// Token: 0x06006D95 RID: 28053 RVA: 0x0023FB34 File Offset: 0x0023DD34
		private void OnEnable()
		{
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShakeEvent);
			}
		}

		// Token: 0x06006D96 RID: 28054 RVA: 0x0023FBFC File Offset: 0x0023DDFC
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnShakeEvent);
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006D97 RID: 28055 RVA: 0x0023FC4C File Offset: 0x0023DE4C
		private void OnShakeEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnShakeEvent");
			NetPlayer sender2 = info.Sender;
			VRRig myOnlineRig = this.transferrableObject.myOnlineRig;
			if (sender2 != ((myOnlineRig != null) ? myOnlineRig.creator : null))
			{
				return;
			}
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			object obj = args[0];
			if (!(obj is bool))
			{
				return;
			}
			bool flag = (bool)obj;
			if (flag)
			{
				UnityEvent handleOnShakeStart = this.HandleOnShakeStart;
				if (handleOnShakeStart == null)
				{
					return;
				}
				handleOnShakeStart.Invoke();
				return;
			}
			else
			{
				UnityEvent handleOnShakeEnd = this.HandleOnShakeEnd;
				if (handleOnShakeEnd == null)
				{
					return;
				}
				handleOnShakeEnd.Invoke();
				return;
			}
		}

		// Token: 0x06006D98 RID: 28056 RVA: 0x0023FCD8 File Offset: 0x0023DED8
		public void OnShakeStart()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					true
				});
			}
			UnityEvent handleOnShakeStart = this.HandleOnShakeStart;
			if (handleOnShakeStart == null)
			{
				return;
			}
			handleOnShakeStart.Invoke();
		}

		// Token: 0x06006D99 RID: 28057 RVA: 0x0023FD3C File Offset: 0x0023DF3C
		public void OnShakeEnd()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					false
				});
			}
			UnityEvent handleOnShakeEnd = this.HandleOnShakeEnd;
			if (handleOnShakeEnd == null)
			{
				return;
			}
			handleOnShakeEnd.Invoke();
		}

		// Token: 0x04007EF8 RID: 32504
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04007EF9 RID: 32505
		private RubberDuckEvents _events;

		// Token: 0x04007EFA RID: 32506
		private CallLimiter callLimiter = new CallLimiter(10, 1f, 0.5f);

		// Token: 0x04007EFB RID: 32507
		public UnityEvent HandleOnShakeStart;

		// Token: 0x04007EFC RID: 32508
		public UnityEvent HandleOnShakeEnd;
	}
}
