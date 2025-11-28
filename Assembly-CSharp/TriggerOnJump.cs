using System;
using GorillaExtensions;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200028A RID: 650
public class TriggerOnJump : MonoBehaviour, ITickSystemTick
{
	// Token: 0x060010B7 RID: 4279 RVA: 0x000571D0 File Offset: 0x000553D0
	private void OnEnable()
	{
		if (this.myRig.IsNull())
		{
			this.myRig = base.GetComponentInParent<VRRig>();
		}
		if (this._events == null && this.myRig != null && this.myRig.Creator != null)
		{
			this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
			this._events.Init(this.myRig.creator);
		}
		if (this._events != null)
		{
			this._events.Activate += new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnActivate);
		}
		bool flag = !PhotonNetwork.InRoom && this.myRig != null && this.myRig.isOfflineVRRig;
		RigContainer rigContainer;
		bool flag2 = PhotonNetwork.InRoom && this.myRig != null && VRRigCache.Instance.TryGetVrrig(PhotonNetwork.LocalPlayer, out rigContainer) && rigContainer != null && rigContainer.Rig != null && rigContainer.Rig == this.myRig;
		if (flag || flag2)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	// Token: 0x060010B8 RID: 4280 RVA: 0x000572F8 File Offset: 0x000554F8
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
		this.playerOnGround = false;
		this.jumpStartTime = 0f;
		this.lastActivationTime = 0f;
		this.waitingForGrounding = false;
		if (this._events != null)
		{
			this._events.Activate -= new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnActivate);
			Object.Destroy(this._events);
			this._events = null;
		}
	}

	// Token: 0x060010B9 RID: 4281 RVA: 0x00057371 File Offset: 0x00055571
	private void OnActivate(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnJumpActivate");
		if (info.senderID != this.myRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		this.onJumping.Invoke();
	}

	// Token: 0x060010BA RID: 4282 RVA: 0x000573AC File Offset: 0x000555AC
	public void Tick()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			bool flag = this.playerOnGround;
			this.playerOnGround = (instance.BodyOnGround || instance.IsHandTouching(true) || instance.IsHandTouching(false));
			float time = Time.time;
			if (this.playerOnGround)
			{
				this.waitingForGrounding = false;
			}
			if (!this.playerOnGround && flag)
			{
				this.jumpStartTime = time;
			}
			if (!this.playerOnGround && !this.waitingForGrounding && instance.RigidbodyVelocity.sqrMagnitude > this.minJumpStrength * this.minJumpStrength && instance.RigidbodyVelocity.y > this.minJumpVertical && time > this.jumpStartTime + this.minJumpTime)
			{
				this.waitingForGrounding = true;
				if (time > this.lastActivationTime + this.cooldownTime)
				{
					this.lastActivationTime = time;
					if (PhotonNetwork.InRoom)
					{
						this._events.Activate.RaiseAll(Array.Empty<object>());
						return;
					}
					this.onJumping.Invoke();
				}
			}
		}
	}

	// Token: 0x17000198 RID: 408
	// (get) Token: 0x060010BB RID: 4283 RVA: 0x000574B8 File Offset: 0x000556B8
	// (set) Token: 0x060010BC RID: 4284 RVA: 0x000574C0 File Offset: 0x000556C0
	public bool TickRunning { get; set; }

	// Token: 0x040014D4 RID: 5332
	[SerializeField]
	private float minJumpStrength = 1f;

	// Token: 0x040014D5 RID: 5333
	[SerializeField]
	private float minJumpVertical = 1f;

	// Token: 0x040014D6 RID: 5334
	[SerializeField]
	private float cooldownTime = 1f;

	// Token: 0x040014D7 RID: 5335
	[SerializeField]
	private UnityEvent onJumping;

	// Token: 0x040014D8 RID: 5336
	private RubberDuckEvents _events;

	// Token: 0x040014D9 RID: 5337
	private bool playerOnGround;

	// Token: 0x040014DA RID: 5338
	private float minJumpTime = 0.05f;

	// Token: 0x040014DB RID: 5339
	private bool waitingForGrounding;

	// Token: 0x040014DC RID: 5340
	private float jumpStartTime;

	// Token: 0x040014DD RID: 5341
	private float lastActivationTime;

	// Token: 0x040014DE RID: 5342
	private VRRig myRig;
}
