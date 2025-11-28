using System;
using System.Collections;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001117 RID: 4375
	public class SnakeInCanHoldable : TransferrableObject
	{
		// Token: 0x06006D86 RID: 28038 RVA: 0x0023F766 File Offset: 0x0023D966
		protected override void Awake()
		{
			base.Awake();
			this.topRigPosition = this.topRigObject.transform.position;
		}

		// Token: 0x06006D87 RID: 28039 RVA: 0x0023F784 File Offset: 0x0023D984
		internal override void OnEnable()
		{
			base.OnEnable();
			this.disableObjectBeforeTrigger.SetActive(false);
			if (this.compressedPoint != null)
			{
				this.topRigObject.transform.position = this.compressedPoint.position;
			}
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnEnableObject);
			}
		}

		// Token: 0x06006D88 RID: 28040 RVA: 0x0023F87C File Offset: 0x0023DA7C
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this._events != null)
			{
				this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnEnableObject);
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x06006D89 RID: 28041 RVA: 0x0023F8D4 File Offset: 0x0023DAD4
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (!base.OnRelease(zoneReleased, releasingHand))
			{
				return false;
			}
			if (VRRigCache.Instance.localRig.Rig != this.ownerRig)
			{
				return false;
			}
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					false
				});
			}
			this.EnableObjectLocal(false);
			return true;
		}

		// Token: 0x06006D8A RID: 28042 RVA: 0x0023F95C File Offset: 0x0023DB5C
		private void OnEnableObject(int sender, int target, object[] arg, PhotonMessageInfoWrapped info)
		{
			if (info.senderID != this.ownerRig.creator.ActorNumber)
			{
				return;
			}
			if (arg.Length != 1 || !(arg[0] is bool))
			{
				return;
			}
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnEnableObject");
			if (!this.snakeInCanCallLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			bool enable = (bool)arg[0];
			this.EnableObjectLocal(enable);
		}

		// Token: 0x06006D8B RID: 28043 RVA: 0x0023F9C8 File Offset: 0x0023DBC8
		private void EnableObjectLocal(bool enable)
		{
			this.disableObjectBeforeTrigger.SetActive(enable);
			if (!enable)
			{
				if (this.compressedPoint != null)
				{
					this.topRigObject.transform.position = this.compressedPoint.position;
				}
				return;
			}
			if (this.stretchedPoint != null)
			{
				base.StartCoroutine(this.SmoothTransition());
				return;
			}
			this.topRigObject.transform.position = this.topRigPosition;
		}

		// Token: 0x06006D8C RID: 28044 RVA: 0x0023FA40 File Offset: 0x0023DC40
		private IEnumerator SmoothTransition()
		{
			while (Vector3.Distance(this.topRigObject.transform.position, this.stretchedPoint.position) > 0.01f)
			{
				this.topRigObject.transform.position = Vector3.MoveTowards(this.topRigObject.transform.position, this.stretchedPoint.position, this.jumpSpeed * Time.deltaTime);
				yield return null;
			}
			this.topRigObject.transform.position = this.stretchedPoint.position;
			yield break;
		}

		// Token: 0x06006D8D RID: 28045 RVA: 0x0023FA4F File Offset: 0x0023DC4F
		public void OnButtonPressed()
		{
			this.EnableObjectLocal(true);
		}

		// Token: 0x04007EEC RID: 32492
		[SerializeField]
		private float jumpSpeed;

		// Token: 0x04007EED RID: 32493
		[SerializeField]
		private Transform stretchedPoint;

		// Token: 0x04007EEE RID: 32494
		[SerializeField]
		private Transform compressedPoint;

		// Token: 0x04007EEF RID: 32495
		[SerializeField]
		private GameObject topRigObject;

		// Token: 0x04007EF0 RID: 32496
		[SerializeField]
		private GameObject disableObjectBeforeTrigger;

		// Token: 0x04007EF1 RID: 32497
		private CallLimiter snakeInCanCallLimiter = new CallLimiter(10, 2f, 0.5f);

		// Token: 0x04007EF2 RID: 32498
		private Vector3 topRigPosition;

		// Token: 0x04007EF3 RID: 32499
		private Vector3 originalTopRigPosition;

		// Token: 0x04007EF4 RID: 32500
		private RubberDuckEvents _events;
	}
}
