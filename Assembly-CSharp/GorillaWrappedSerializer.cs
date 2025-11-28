using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200075A RID: 1882
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaWrappedSerializer : NetworkBehaviour, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x060030A5 RID: 12453 RVA: 0x00109F84 File Offset: 0x00108184
	public NetworkView NetView
	{
		get
		{
			return this.netView;
		}
	}

	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x060030A6 RID: 12454 RVA: 0x00109F8C File Offset: 0x0010818C
	// (set) Token: 0x060030A7 RID: 12455 RVA: 0x00109F94 File Offset: 0x00108194
	protected virtual object data { get; set; }

	// Token: 0x17000444 RID: 1092
	// (get) Token: 0x060030A8 RID: 12456 RVA: 0x00109F9D File Offset: 0x0010819D
	public bool IsLocallyOwned
	{
		get
		{
			return this.netView.IsMine;
		}
	}

	// Token: 0x17000445 RID: 1093
	// (get) Token: 0x060030A9 RID: 12457 RVA: 0x00109FAA File Offset: 0x001081AA
	public bool IsValid
	{
		get
		{
			return this.netView.IsValid;
		}
	}

	// Token: 0x060030AA RID: 12458 RVA: 0x00109FB7 File Offset: 0x001081B7
	private void Awake()
	{
		if (this.netView == null)
		{
			this.netView = base.GetComponent<NetworkView>();
		}
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x00109FD4 File Offset: 0x001081D4
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.netView == null || !this.netView.IsValid)
		{
			return;
		}
		PhotonMessageInfoWrapped wrappedInfo = new PhotonMessageInfoWrapped(info);
		this.ProcessSpawn(wrappedInfo);
	}

	// Token: 0x060030AC RID: 12460 RVA: 0x0010A00C File Offset: 0x0010820C
	public override void Spawned()
	{
		PhotonMessageInfoWrapped wrappedInfo = new PhotonMessageInfoWrapped(base.Object.StateAuthority.PlayerId, base.Runner.Tick.Raw);
		this.ProcessSpawn(wrappedInfo);
	}

	// Token: 0x060030AD RID: 12461 RVA: 0x0010A04C File Offset: 0x0010824C
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

	// Token: 0x060030AE RID: 12462 RVA: 0x0010A0C7 File Offset: 0x001082C7
	protected virtual bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IWrappedSerializable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x060030AF RID: 12463
	protected abstract void OnSuccesfullySpawned(PhotonMessageInfoWrapped info);

	// Token: 0x060030B0 RID: 12464 RVA: 0x0010A0E0 File Offset: 0x001082E0
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

	// Token: 0x060030B2 RID: 12466 RVA: 0x00109D8D File Offset: 0x00107F8D
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x0010A138 File Offset: 0x00108338
	public override void FixedUpdateNetwork()
	{
		this.data = this.serializeTarget.OnSerializeWrite();
	}

	// Token: 0x060030B4 RID: 12468 RVA: 0x0010A14B File Offset: 0x0010834B
	public override void Render()
	{
		if (!base.Object.HasStateAuthority)
		{
			this.serializeTarget.OnSerializeRead(this.data);
		}
	}

	// Token: 0x060030B5 RID: 12469 RVA: 0x0010A16C File Offset: 0x0010836C
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

	// Token: 0x060030B6 RID: 12470 RVA: 0x0010A1B8 File Offset: 0x001083B8
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x0010A1B8 File Offset: 0x001083B8
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060030B8 RID: 12472
	protected abstract void OnBeforeDespawn();

	// Token: 0x060030B9 RID: 12473 RVA: 0x0010A1C0 File Offset: 0x001083C0
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T t = base.gameObject.AddComponent<T>();
		this.netView.GetView.RefreshRpcMonoBehaviourCache();
		t.SetClassTarget(this.serializeTarget, this);
		return t;
	}

	// Token: 0x060030BA RID: 12474 RVA: 0x0010A1F0 File Offset: 0x001083F0
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

	// Token: 0x060030BD RID: 12477 RVA: 0x0010A213 File Offset: 0x00108413
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
