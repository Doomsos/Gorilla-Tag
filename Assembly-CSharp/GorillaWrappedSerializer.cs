using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200075A RID: 1882
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaWrappedSerializer : NetworkBehaviour, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x060030A5 RID: 12453 RVA: 0x00109FA4 File Offset: 0x001081A4
	public NetworkView NetView
	{
		get
		{
			return this.netView;
		}
	}

	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x060030A6 RID: 12454 RVA: 0x00109FAC File Offset: 0x001081AC
	// (set) Token: 0x060030A7 RID: 12455 RVA: 0x00109FB4 File Offset: 0x001081B4
	protected virtual object data { get; set; }

	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x060030A8 RID: 12456 RVA: 0x00109FBD File Offset: 0x001081BD
	public bool IsLocallyOwned
	{
		get
		{
			return this.netView.IsMine;
		}
	}

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x060030A9 RID: 12457 RVA: 0x00109FCA File Offset: 0x001081CA
	public bool IsValid
	{
		get
		{
			return this.netView.IsValid;
		}
	}

	// Token: 0x060030AA RID: 12458 RVA: 0x00109FD7 File Offset: 0x001081D7
	private void Awake()
	{
		if (this.netView == null)
		{
			this.netView = base.GetComponent<NetworkView>();
		}
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x00109FF4 File Offset: 0x001081F4
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.netView == null || !this.netView.IsValid)
		{
			return;
		}
		PhotonMessageInfoWrapped wrappedInfo = new PhotonMessageInfoWrapped(info);
		this.ProcessSpawn(wrappedInfo);
	}

	// Token: 0x060030AC RID: 12460 RVA: 0x0010A02C File Offset: 0x0010822C
	public override void Spawned()
	{
		PhotonMessageInfoWrapped wrappedInfo = new PhotonMessageInfoWrapped(base.Object.StateAuthority.PlayerId, base.Runner.Tick.Raw);
		this.ProcessSpawn(wrappedInfo);
	}

	// Token: 0x060030AD RID: 12461 RVA: 0x0010A06C File Offset: 0x0010826C
	private void ProcessSpawn(PhotonMessageInfoWrapped wrappedInfo)
	{
		this.successfullInstantiate = this.OnSpawnSetupCheck(wrappedInfo, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			GameObject gameObject = this.targetObject;
			IWrappedSerializable wrappedSerializable = ((gameObject != null) ? gameObject.GetComponent(this.targetType) : null) as IWrappedSerializable;
			if (wrappedSerializable != null)
			{
				this.serializeTarget = wrappedSerializable;
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccesfullySpawned(wrappedInfo);
			return;
		}
		this.FailedToSpawn();
	}

	// Token: 0x060030AE RID: 12462 RVA: 0x0010A0E7 File Offset: 0x001082E7
	protected virtual bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IWrappedSerializable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x060030AF RID: 12463
	protected abstract void OnSuccesfullySpawned(PhotonMessageInfoWrapped info);

	// Token: 0x060030B0 RID: 12464 RVA: 0x0010A100 File Offset: 0x00108300
	private void FailedToSpawn()
	{
		Debug.LogError("Failed to network instantiate");
		if (this.netView.IsMine)
		{
			PhotonNetwork.Destroy(this.netView.GetView);
			return;
		}
		this.netView.GetView.ObservedComponents.Remove(this);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060030B1 RID: 12465
	protected abstract void OnFailedSpawn();

	// Token: 0x060030B2 RID: 12466 RVA: 0x00109DAD File Offset: 0x00107FAD
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x0010A158 File Offset: 0x00108358
	public override void FixedUpdateNetwork()
	{
		this.data = this.serializeTarget.OnSerializeWrite();
	}

	// Token: 0x060030B4 RID: 12468 RVA: 0x0010A16B File Offset: 0x0010836B
	public override void Render()
	{
		if (!base.Object.HasStateAuthority)
		{
			this.serializeTarget.OnSerializeRead(this.data);
		}
	}

	// Token: 0x060030B5 RID: 12469 RVA: 0x0010A18C File Offset: 0x0010838C
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || this.serializeTarget == null || !this.ValidOnSerialize(stream, info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			this.serializeTarget.OnSerializeWrite(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeRead(stream, info);
	}

	// Token: 0x060030B6 RID: 12470 RVA: 0x0010A1D8 File Offset: 0x001083D8
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x0010A1D8 File Offset: 0x001083D8
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060030B8 RID: 12472
	protected abstract void OnBeforeDespawn();

	// Token: 0x060030B9 RID: 12473 RVA: 0x0010A1E0 File Offset: 0x001083E0
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T t = base.gameObject.AddComponent<T>();
		this.netView.GetView.RefreshRpcMonoBehaviourCache();
		t.SetClassTarget(this.serializeTarget, this);
		return t;
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x0010A210 File Offset: 0x00108410
	public void SendRPC(string rpcName, bool targetOthers, params object[] data)
	{
		RpcTarget target = targetOthers ? 1 : 2;
		this.netView.SendRPC(rpcName, target, data);
	}

	// Token: 0x060030BB RID: 12475 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void FusionDataRPC(string method, RpcTarget target, params object[] parameters)
	{
	}

	// Token: 0x060030BC RID: 12476 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void FusionDataRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
	}

	// Token: 0x060030BD RID: 12477 RVA: 0x0010A233 File Offset: 0x00108433
	public void SendRPC(string rpcName, NetPlayer targetPlayer, params object[] data)
	{
		this.netView.GetView.RPC(rpcName, ((PunNetPlayer)targetPlayer).PlayerRef, data);
	}

	// Token: 0x060030BF RID: 12479 RVA: 0x00002789 File Offset: 0x00000989
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x060030C0 RID: 12480 RVA: 0x00002789 File Offset: 0x00000989
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x04003FA7 RID: 16295
	protected bool successfullInstantiate;

	// Token: 0x04003FA8 RID: 16296
	protected IWrappedSerializable serializeTarget;

	// Token: 0x04003FA9 RID: 16297
	private Type targetType;

	// Token: 0x04003FAA RID: 16298
	protected GameObject targetObject;

	// Token: 0x04003FAB RID: 16299
	[SerializeField]
	protected NetworkView netView;
}
