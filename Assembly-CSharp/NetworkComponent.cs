using System;
using ExitGames.Client.Photon;
using Fusion;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x020003C7 RID: 967
[NetworkBehaviourWeaved(0)]
public abstract class NetworkComponent : NetworkView, IPunObservable, IStateAuthorityChanged, IPublicFacingInterface, IOnPhotonViewOwnerChange, IPhotonViewCallback, IInRoomCallbacks, IPunInstantiateMagicCallback
{
	// Token: 0x06001745 RID: 5957 RVA: 0x00080A43 File Offset: 0x0007EC43
	internal virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.AddToNetwork();
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x00080A51 File Offset: 0x0007EC51
	internal virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		PhotonNetwork.RemoveCallbackTarget(this);
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x00080A5F File Offset: 0x0007EC5F
	protected override void Start()
	{
		base.Start();
		this.AddToNetwork();
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x00080A6D File Offset: 0x0007EC6D
	private void AddToNetwork()
	{
		PhotonNetwork.AddCallbackTarget(this);
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x00080A75 File Offset: 0x0007EC75
	public override void Spawned()
	{
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnSpawned();
		}
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x00080A89 File Offset: 0x0007EC89
	public override void FixedUpdateNetwork()
	{
		this.WriteDataFusion();
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x00080A91 File Offset: 0x0007EC91
	public override void Render()
	{
		if (!base.HasStateAuthority)
		{
			this.ReadDataFusion();
		}
	}

	// Token: 0x0600174C RID: 5964
	public abstract void WriteDataFusion();

	// Token: 0x0600174D RID: 5965
	public abstract void ReadDataFusion();

	// Token: 0x0600174E RID: 5966 RVA: 0x00080AA1 File Offset: 0x0007ECA1
	public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		this.OnSpawned();
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x00080AA9 File Offset: 0x0007ECA9
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			this.WriteDataPUN(stream, info);
			return;
		}
		if (stream.IsReading)
		{
			this.ReadDataPUN(stream, info);
		}
	}

	// Token: 0x06001750 RID: 5968
	protected abstract void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06001751 RID: 5969
	protected abstract void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info);

	// Token: 0x06001752 RID: 5970 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnSpawned()
	{
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnOwnerSwitched(NetPlayer newOwningPlayer)
	{
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x00080ACC File Offset: 0x0007ECCC
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(newMasterClient));
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x00080AE0 File Offset: 0x0007ECE0
	public override void StateAuthorityChanged()
	{
		base.StateAuthorityChanged();
		if (base.Object == null)
		{
			return;
		}
		if (base.Object.StateAuthority == default(PlayerRef))
		{
			return;
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnOwnerSwitched(NetworkSystem.Instance.GetPlayer(base.Object.StateAuthority));
			return;
		}
		this.OnOwnerSwitched(NetworkSystem.Instance.LocalPlayer);
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x00080B56 File Offset: 0x0007ED56
	public void OnMasterClientSwitch(NetPlayer newMaster)
	{
		this.StateAuthorityChanged();
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x00002789 File Offset: 0x00000989
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnOwnerChange(Player newOwner, Player previousOwner)
	{
	}

	// Token: 0x1700025A RID: 602
	// (get) Token: 0x0600175C RID: 5980 RVA: 0x00080B5E File Offset: 0x0007ED5E
	public bool IsLocallyOwned
	{
		get
		{
			return base.IsMine;
		}
	}

	// Token: 0x1700025B RID: 603
	// (get) Token: 0x0600175D RID: 5981 RVA: 0x00080B66 File Offset: 0x0007ED66
	public bool ShouldWriteObjectData
	{
		get
		{
			return NetworkSystem.Instance.ShouldWriteObjectData(base.gameObject);
		}
	}

	// Token: 0x1700025C RID: 604
	// (get) Token: 0x0600175E RID: 5982 RVA: 0x00080B78 File Offset: 0x0007ED78
	public bool ShouldUpdateobject
	{
		get
		{
			return NetworkSystem.Instance.ShouldUpdateObject(base.gameObject);
		}
	}

	// Token: 0x1700025D RID: 605
	// (get) Token: 0x0600175F RID: 5983 RVA: 0x00080B8A File Offset: 0x0007ED8A
	public int OwnerID
	{
		get
		{
			return NetworkSystem.Instance.GetOwningPlayerID(base.gameObject);
		}
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x00080BA4 File Offset: 0x0007EDA4
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x00080BB0 File Offset: 0x0007EDB0
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
