using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020003DB RID: 987
[RequireComponent(typeof(PhotonView), typeof(NetworkObject))]
[NetworkBehaviourWeaved(0)]
public class NetworkView : NetworkBehaviour, IStateAuthorityChanged, IPublicFacingInterface, IPunOwnershipCallbacks
{
	// Token: 0x17000283 RID: 643
	// (get) Token: 0x06001819 RID: 6169 RVA: 0x00081A01 File Offset: 0x0007FC01
	public bool IsMine
	{
		get
		{
			return this.punView != null && this.punView.IsMine;
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x0600181A RID: 6170 RVA: 0x00081A1E File Offset: 0x0007FC1E
	public bool IsValid
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x0600181B RID: 6171 RVA: 0x00081A1E File Offset: 0x0007FC1E
	public bool HasView
	{
		get
		{
			return this.punView != null;
		}
	}

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x0600181C RID: 6172 RVA: 0x00081A2C File Offset: 0x0007FC2C
	public bool IsRoomView
	{
		get
		{
			return this.punView.IsRoomView;
		}
	}

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x0600181D RID: 6173 RVA: 0x00081A39 File Offset: 0x0007FC39
	public PhotonView GetView
	{
		get
		{
			return this.punView;
		}
	}

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x0600181E RID: 6174 RVA: 0x00081A41 File Offset: 0x0007FC41
	public NetPlayer Owner
	{
		get
		{
			return NetworkSystem.Instance.GetPlayer(this.punView.Owner);
		}
	}

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x0600181F RID: 6175 RVA: 0x00081A58 File Offset: 0x0007FC58
	public int ViewID
	{
		get
		{
			return this.punView.ViewID;
		}
	}

	// Token: 0x1700028A RID: 650
	// (get) Token: 0x06001820 RID: 6176 RVA: 0x00081A65 File Offset: 0x0007FC65
	// (set) Token: 0x06001821 RID: 6177 RVA: 0x00081A72 File Offset: 0x0007FC72
	internal OwnershipOption OwnershipTransfer
	{
		get
		{
			return this.punView.OwnershipTransfer;
		}
		set
		{
			this.punView.OwnershipTransfer = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnershipTransfer = value;
			}
		}
	}

	// Token: 0x1700028B RID: 651
	// (get) Token: 0x06001822 RID: 6178 RVA: 0x00081A9A File Offset: 0x0007FC9A
	// (set) Token: 0x06001823 RID: 6179 RVA: 0x00081AA7 File Offset: 0x0007FCA7
	public int OwnerActorNr
	{
		get
		{
			return this.punView.OwnerActorNr;
		}
		set
		{
			this.punView.OwnerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.OwnerActorNr = value;
			}
		}
	}

	// Token: 0x1700028C RID: 652
	// (get) Token: 0x06001824 RID: 6180 RVA: 0x00081ACF File Offset: 0x0007FCCF
	// (set) Token: 0x06001825 RID: 6181 RVA: 0x00081ADC File Offset: 0x0007FCDC
	public int ControllerActorNr
	{
		get
		{
			return this.punView.ControllerActorNr;
		}
		set
		{
			this.punView.ControllerActorNr = value;
			if (this.reliableView != null)
			{
				this.reliableView.ControllerActorNr = value;
			}
		}
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x00081B04 File Offset: 0x0007FD04
	private void GetViews()
	{
		PhotonView[] components = base.GetComponents<PhotonView>();
		if (components.Length > 1)
		{
			if (components[0].Synchronization == 3)
			{
				this.punView = components[0];
				this.reliableView = components[1];
			}
			else if (components[0].Synchronization == 1)
			{
				this.reliableView = components[0];
				this.punView = components[1];
			}
		}
		else
		{
			this.punView = components[0];
		}
		if (this.punView == null)
		{
			this.punView = base.GetComponent<PhotonView>();
		}
		if (this.fusionView == null)
		{
			this.fusionView = base.GetComponent<NetworkObject>();
		}
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x00081B99 File Offset: 0x0007FD99
	protected virtual void Awake()
	{
		this.GetViews();
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x00081BA1 File Offset: 0x0007FDA1
	protected virtual void Start()
	{
		if (this._sceneObject)
		{
			NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		}
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x00081BBC File Offset: 0x0007FDBC
	public void SendRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
		Player playerRef = (targetPlayer as PunNetPlayer).PlayerRef;
		this.punView.RPC(method, playerRef, parameters);
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x00081BE3 File Offset: 0x0007FDE3
	public void SendRPC(string method, RpcTarget target, params object[] parameters)
	{
		this.punView.RPC(method, target, parameters);
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x00081BF4 File Offset: 0x0007FDF4
	public void SendRPC(string method, int target, params object[] parameters)
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom == null || !currentRoom.Players.ContainsKey(target))
		{
			return;
		}
		this.punView.RPC(method, currentRoom.Players[target], parameters);
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x00081C32 File Offset: 0x0007FE32
	public override void Spawned()
	{
		base.Spawned();
		this._spawned = true;
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x00081C41 File Offset: 0x0007FE41
	public void RequestOwnership()
	{
		this.GetView.RequestOwnership();
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x00081C4E File Offset: 0x0007FE4E
	public void ReleaseOwnership()
	{
		this.changingStatAuth = true;
		base.Object.ReleaseStateAuthority();
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x00081C62 File Offset: 0x0007FE62
	public virtual void StateAuthorityChanged()
	{
		if (this.changingStatAuth)
		{
			this.changingStatAuth = false;
		}
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x00002789 File Offset: 0x00000989
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x00002789 File Offset: 0x00000989
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x04002191 RID: 8593
	[SerializeField]
	private PhotonView punView;

	// Token: 0x04002192 RID: 8594
	[SerializeField]
	private PhotonView reliableView;

	// Token: 0x04002193 RID: 8595
	[SerializeField]
	internal NetworkObject fusionView;

	// Token: 0x04002194 RID: 8596
	[SerializeField]
	protected bool _sceneObject;

	// Token: 0x04002195 RID: 8597
	private bool _spawned;

	// Token: 0x04002196 RID: 8598
	private bool changingStatAuth;
}
