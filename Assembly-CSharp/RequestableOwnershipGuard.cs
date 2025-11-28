using System;
using System.Collections;
using System.Collections.Generic;
using GorillaExtensions;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000310 RID: 784
[RequireComponent(typeof(NetworkView))]
public class RequestableOwnershipGuard : MonoBehaviourPunCallbacks, ISelfValidator
{
	// Token: 0x06001309 RID: 4873 RVA: 0x0006E54F File Offset: 0x0006C74F
	private void SetViewToRequest()
	{
		base.GetComponent<NetworkView>().OwnershipTransfer = 2;
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x0600130A RID: 4874 RVA: 0x0006E55D File Offset: 0x0006C75D
	private NetworkView netView
	{
		get
		{
			if (this.netViews == null)
			{
				return null;
			}
			if (this.netViews.Length == 0)
			{
				return null;
			}
			return this.netViews[0];
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x0600130B RID: 4875 RVA: 0x0006E57C File Offset: 0x0006C77C
	[DevInspectorShow]
	public bool isTrulyMine
	{
		get
		{
			return object.Equals(this.actualOwner, NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x0600130C RID: 4876 RVA: 0x0006E593 File Offset: 0x0006C793
	public bool isMine
	{
		get
		{
			return object.Equals(this.currentOwner, NetworkSystem.Instance.LocalPlayer);
		}
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x0006E5AA File Offset: 0x0006C7AA
	private void BindNetworkViews()
	{
		this.netViews = base.GetComponents<NetworkView>();
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x0006E5B8 File Offset: 0x0006C7B8
	public override void OnDisable()
	{
		base.OnDisable();
		RequestableOwnershipGaurdHandler.RemoveViews(this.netViews, this);
		NetworkSystem.Instance.OnPlayerJoined -= new Action<NetPlayer>(this.PlayerEnteredRoom);
		NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(this.PlayerLeftRoom);
		NetworkSystem.Instance.OnJoinedRoomEvent -= new Action(this.JoinedRoom);
		NetworkSystem.Instance.OnMasterClientSwitchedEvent -= new Action<NetPlayer>(this.MasterClientSwitch);
		this.currentMasterClient = null;
		this.currentOwner = null;
		this.actualOwner = null;
		this.creator = NetworkSystem.Instance.LocalPlayer;
		this.currentState = NetworkingState.IsOwner;
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x0006E688 File Offset: 0x0006C888
	public override void OnEnable()
	{
		base.OnEnable();
		if (this.autoRegister)
		{
			this.BindNetworkViews();
		}
		if (this.netViews == null)
		{
			return;
		}
		RequestableOwnershipGaurdHandler.RegisterViews(this.netViews, this);
		NetworkSystem.Instance.OnPlayerJoined += new Action<NetPlayer>(this.PlayerEnteredRoom);
		NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.PlayerLeftRoom);
		NetworkSystem.Instance.OnJoinedRoomEvent += new Action(this.JoinedRoom);
		NetworkSystem.Instance.OnMasterClientSwitchedEvent += new Action<NetPlayer>(this.MasterClientSwitch);
		NetworkSystem instance = NetworkSystem.Instance;
		if (instance == null || !instance.InRoom)
		{
			GorillaTagger.OnPlayerSpawned(delegate
			{
				this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			});
			return;
		}
		this.currentMasterClient = NetworkSystem.Instance.MasterClient;
		int creatorActorNr = this.netView.GetView.CreatorActorNr;
		NetPlayer netPlayer = this.currentMasterClient;
		int? num = (netPlayer != null) ? new int?(netPlayer.ActorNumber) : default(int?);
		if (!(creatorActorNr == num.GetValueOrDefault() & num != null))
		{
			this.SetOwnership(NetworkSystem.Instance.GetPlayer(this.netView.GetView.CreatorActorNr), false, false);
			return;
		}
		if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			this.currentState = NetworkingState.IsOwner;
			return;
		}
		this.currentState = NetworkingState.IsBlindClient;
		this.SetOwnership(null, false, false);
		this.RequestTheCurrentOwnerFromAuthority();
	}

	// Token: 0x06001310 RID: 4880 RVA: 0x0006E824 File Offset: 0x0006CA24
	private void PlayerEnteredRoom(NetPlayer player)
	{
		try
		{
			if (!player.IsLocal)
			{
				if (NetworkSystem.Instance.InRoom && this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
				{
					this.netView.SendRPC("SetOwnershipFromMasterClient", player, new object[]
					{
						this.currentOwner.GetPlayerRef()
					});
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x06001311 RID: 4881 RVA: 0x0006E898 File Offset: 0x0006CA98
	public override void OnPreLeavingRoom()
	{
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		case NetworkingState.IsClient:
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
			{
				callback.OnMyOwnerLeft();
			});
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
	}

	// Token: 0x06001312 RID: 4882 RVA: 0x0006E91C File Offset: 0x0006CB1C
	private void JoinedRoom()
	{
		this.currentMasterClient = NetworkSystem.Instance.MasterClient;
		if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			this.currentState = NetworkingState.IsOwner;
			return;
		}
		this.currentState = NetworkingState.IsBlindClient;
		this.SetOwnership(null, false, false);
	}

	// Token: 0x06001313 RID: 4883 RVA: 0x0006E978 File Offset: 0x0006CB78
	private void PlayerLeftRoom(NetPlayer otherPlayer)
	{
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		case NetworkingState.IsBlindClient:
			if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
			{
				this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
				return;
			}
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		case NetworkingState.IsClient:
			if (this.creator != null && object.Equals(this.creator, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyCreatorLeft();
				});
			}
			if (object.Equals(this.actualOwner, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyOwnerLeft();
				});
				if (this.fallbackOwner != null)
				{
					this.SetOwnership(this.fallbackOwner, false, false);
					return;
				}
				this.SetOwnership(this.currentMasterClient, false, false);
				return;
			}
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			if (this.creator != null && object.Equals(this.creator, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyCreatorLeft();
				});
			}
			if (this.currentState == NetworkingState.ForcefullyTakingOver && object.Equals(this.currentOwner, otherPlayer))
			{
				this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks callback)
				{
					callback.OnMyOwnerLeft();
				});
			}
			if (object.Equals(this.actualOwner, otherPlayer))
			{
				if (this.fallbackOwner != null)
				{
					this.SetOwnership(this.fallbackOwner, false, false);
					if (object.Equals(this.fallbackOwner, PhotonNetwork.LocalPlayer))
					{
						Action action = this.ownershipRequestAccepted;
						if (action == null)
						{
							return;
						}
						action.Invoke();
						return;
					}
					else
					{
						Action action2 = this.ownershipDenied;
						if (action2 == null)
						{
							return;
						}
						action2.Invoke();
						return;
					}
				}
				else if (object.Equals(this.currentMasterClient, PhotonNetwork.LocalPlayer))
				{
					Action action3 = this.ownershipRequestAccepted;
					if (action3 == null)
					{
						return;
					}
					action3.Invoke();
					return;
				}
				else
				{
					Action action4 = this.ownershipDenied;
					if (action4 == null)
					{
						return;
					}
					action4.Invoke();
					return;
				}
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001314 RID: 4884 RVA: 0x0006EB98 File Offset: 0x0006CD98
	private void MasterClientSwitch(NetPlayer newMaster)
	{
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsClient:
			if (this.actualOwner == null && this.currentMasterClient == null)
			{
				this.SetOwnership(newMaster, false, false);
			}
			break;
		case NetworkingState.IsBlindClient:
			if (object.Equals(newMaster, NetworkSystem.Instance.LocalPlayer))
			{
				this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			}
			else
			{
				this.RequestTheCurrentOwnerFromAuthority();
			}
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		this.currentMasterClient = newMaster;
	}

	// Token: 0x06001315 RID: 4885 RVA: 0x0006EC28 File Offset: 0x0006CE28
	[PunRPC]
	public void RequestCurrentOwnerFromAuthorityRPC(PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		GorillaNot.IncrementRPCCall(info, "RequestCurrentOwnerFromAuthorityRPC");
		if (!this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 22, info.SentServerTime))
		{
			return;
		}
		this.netView.SendRPC("SetOwnershipFromMasterClient", player, new object[]
		{
			this.actualOwner.GetPlayerRef()
		});
	}

	// Token: 0x06001316 RID: 4886 RVA: 0x0006ECB4 File Offset: 0x0006CEB4
	[PunRPC]
	public void TransferOwnershipFromToRPC([CanBeNull] Player nextplayer, string nonce, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "TransferOwnershipFromToRPC");
		if (nextplayer == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(nextplayer);
		NetPlayer player2 = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (!this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer) && base.photonView.OwnerActorNr != info.Sender.ActorNumber)
		{
			NetPlayer netPlayer = this.currentOwner;
			int? num = (netPlayer != null) ? new int?(netPlayer.ActorNumber) : default(int?);
			int actorNumber = info.Sender.ActorNumber;
			if (!(num.GetValueOrDefault() == actorNumber & num != null))
			{
				NetPlayer netPlayer2 = this.actualOwner;
				num = ((netPlayer2 != null) ? new int?(netPlayer2.ActorNumber) : default(int?));
				actorNumber = info.Sender.ActorNumber;
				if (!(num.GetValueOrDefault() == actorNumber & num != null))
				{
					return;
				}
			}
		}
		if (this.currentOwner == null)
		{
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(player2, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 22, info.SentServerTime))
			{
				return;
			}
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		}
		else
		{
			if (this.currentOwner.ActorNumber != base.photonView.OwnerActorNr)
			{
				return;
			}
			if (this.actualOwner.ActorNumber == player.ActorNumber)
			{
				return;
			}
			switch (this.currentState)
			{
			case NetworkingState.IsClient:
				this.SetOwnership(player, false, false);
				return;
			case NetworkingState.ForcefullyTakingOver:
			case NetworkingState.RequestingOwnership:
				if (this.ownershipRequestNonce == nonce)
				{
					this.ownershipRequestNonce = "";
					this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
					return;
				}
				this.actualOwner = player;
				return;
			case NetworkingState.RequestingOwnershipWaitingForSight:
			case NetworkingState.ForcefullyTakingOverWaitingForSight:
				this.RequestTheCurrentOwnerFromAuthority();
				return;
			}
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001317 RID: 4887 RVA: 0x0006EE80 File Offset: 0x0006D080
	[PunRPC]
	public void SetOwnershipFromMasterClient([CanBeNull] Player nextMaster, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "SetOwnershipFromMasterClient");
		if (nextMaster == null)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(nextMaster);
		NetPlayer player2 = NetworkSystem.Instance.GetPlayer(info.Sender);
		this.SetOwnershipFromMasterClient(player, player2);
	}

	// Token: 0x06001318 RID: 4888 RVA: 0x0006EEC4 File Offset: 0x0006D0C4
	public void SetOwnershipFromMasterClient([CanBeNull] NetPlayer nextMaster, NetPlayer sender)
	{
		if (nextMaster == null)
		{
			return;
		}
		if (!this.PlayerHasAuthority(sender))
		{
			GorillaNot.instance.SendReport("Sent an SetOwnershipFromMasterClient when they weren't the master client", sender.UserId, sender.NickName);
			return;
		}
		NetworkingState networkingState;
		if (this.currentOwner == null)
		{
			networkingState = this.currentState;
			if (networkingState != NetworkingState.IsBlindClient)
			{
				int num = networkingState - NetworkingState.RequestingOwnershipWaitingForSight;
			}
		}
		networkingState = this.currentState;
		if (networkingState - NetworkingState.ForcefullyTakingOver <= 3 && object.Equals(nextMaster, PhotonNetwork.LocalPlayer))
		{
			Action action = this.ownershipRequestAccepted;
			if (action != null)
			{
				action.Invoke();
			}
			this.SetOwnership(nextMaster, false, false);
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			this.SetOwnership(nextMaster, false, false);
			return;
		case NetworkingState.ForcefullyTakingOver:
			this.actualOwner = nextMaster;
			this.currentState = NetworkingState.ForcefullyTakingOver;
			return;
		case NetworkingState.RequestingOwnership:
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			this.currentState = NetworkingState.RequestingOwnership;
			return;
		case NetworkingState.RequestingOwnershipWaitingForSight:
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, false, false);
			this.currentState = NetworkingState.RequestingOwnership;
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			return;
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.actualOwner = nextMaster;
			this.currentState = NetworkingState.ForcefullyTakingOver;
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001319 RID: 4889 RVA: 0x0006F058 File Offset: 0x0006D258
	[PunRPC]
	public void OwnershipRequested(string nonce, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		GorillaNot.IncrementRPCCall(info, "OwnershipRequested");
		if (nonce != null && nonce.Length > 68)
		{
			return;
		}
		if (info.Sender == PhotonNetwork.LocalPlayer)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[8].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		bool flag = true;
		using (List<IRequestableOwnershipGuardCallbacks>.Enumerator enumerator = this.callbacksList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.OnOwnershipRequest(player))
				{
					flag = false;
				}
			}
		}
		if (!flag)
		{
			this.netView.SendRPC("OwnershipRequestDenied", player, new object[]
			{
				nonce
			});
			return;
		}
		this.TransferOwnership(player, nonce);
	}

	// Token: 0x0600131A RID: 4890 RVA: 0x0006F144 File Offset: 0x0006D344
	private void TransferOwnershipWithID(int id)
	{
		this.TransferOwnership(NetworkSystem.Instance.GetPlayer(id), "");
	}

	// Token: 0x0600131B RID: 4891 RVA: 0x0006F15C File Offset: 0x0006D35C
	public void TransferOwnership(NetPlayer player, string Nonce = "")
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.SetOwnership(player, false, false);
			return;
		}
		if (base.photonView.IsMine)
		{
			this.SetOwnership(player, false, false);
			this.netView.SendRPC("TransferOwnershipFromToRPC", 1, new object[]
			{
				player.GetPlayerRef(),
				Nonce
			});
			return;
		}
		if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			this.SetOwnership(player, false, false);
			this.netView.SendRPC("SetOwnershipFromMasterClient", 1, new object[]
			{
				player.GetPlayerRef()
			});
			return;
		}
		Debug.LogError("Tried to transfer ownership when im not the owner or a master client");
	}

	// Token: 0x0600131C RID: 4892 RVA: 0x0006F203 File Offset: 0x0006D403
	public void RequestTheCurrentOwnerFromAuthority()
	{
		this.netView.SendRPC("RequestCurrentOwnerFromAuthorityRPC", this.GetAuthoritativePlayer(), Array.Empty<object>());
	}

	// Token: 0x0600131D RID: 4893 RVA: 0x0006F220 File Offset: 0x0006D420
	protected void SetCurrentOwner(NetPlayer player)
	{
		if (player == null)
		{
			this.currentOwner = null;
		}
		else
		{
			this.currentOwner = player;
		}
		foreach (NetworkView networkView in this.netViews)
		{
			if (player == null)
			{
				networkView.OwnerActorNr = -1;
				networkView.ControllerActorNr = -1;
			}
			else
			{
				networkView.OwnerActorNr = player.ActorNumber;
				networkView.ControllerActorNr = player.ActorNumber;
			}
		}
	}

	// Token: 0x0600131E RID: 4894 RVA: 0x0006F284 File Offset: 0x0006D484
	protected internal void SetOwnership(NetPlayer player, bool isLocalOnly = false, bool dontPropigate = false)
	{
		if (!object.Equals(player, this.currentOwner) && !dontPropigate)
		{
			this.callbacksList.ForEachBackwards(delegate(IRequestableOwnershipGuardCallbacks actualOwner)
			{
				actualOwner.OnOwnershipTransferred(player, this.currentOwner);
			});
		}
		this.SetCurrentOwner(player);
		if (isLocalOnly)
		{
			return;
		}
		this.actualOwner = player;
		if (player == null)
		{
			return;
		}
		if (player.ActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber)
		{
			this.currentState = NetworkingState.IsOwner;
			return;
		}
		this.currentState = NetworkingState.IsClient;
	}

	// Token: 0x0600131F RID: 4895 RVA: 0x0006F322 File Offset: 0x0006D522
	public NetPlayer GetAuthoritativePlayer()
	{
		if (this.giveCreatorAbsoluteAuthority)
		{
			return this.creator;
		}
		return NetworkSystem.Instance.MasterClient;
	}

	// Token: 0x06001320 RID: 4896 RVA: 0x0006F340 File Offset: 0x0006D540
	[PunRPC]
	public void OwnershipRequestDenied(string nonce, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		GorillaNot.IncrementRPCCall(info, "OwnershipRequestDenied");
		int actorNumber = info.Sender.ActorNumber;
		NetPlayer netPlayer = this.actualOwner;
		int? num = (netPlayer != null) ? new int?(netPlayer.ActorNumber) : default(int?);
		if (!(actorNumber == num.GetValueOrDefault() & num != null) && !this.PlayerHasAuthority(player))
		{
			return;
		}
		Action action = this.ownershipDenied;
		if (action != null)
		{
			action.Invoke();
		}
		this.ownershipDenied = null;
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.currentState = NetworkingState.IsClient;
			this.SetOwnership(this.actualOwner, false, false);
			return;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x0006F436 File Offset: 0x0006D636
	public IEnumerator RequestTimeout()
	{
		Debug.Log(string.Format("Timeout request started...  {0} ", this.currentState));
		yield return new WaitForSecondsRealtime(2f);
		Debug.Log(string.Format("Timeout request ended! {0} ", this.currentState));
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		case NetworkingState.IsBlindClient:
		case NetworkingState.IsClient:
			break;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.currentState = NetworkingState.IsClient;
			this.SetOwnership(this.actualOwner, false, false);
			break;
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		yield break;
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x0006F448 File Offset: 0x0006D648
	public void RequestOwnership(Action onRequestSuccess, Action onRequestFailed)
	{
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
			return;
		case NetworkingState.IsBlindClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.currentState = NetworkingState.RequestingOwnershipWaitingForSight;
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.IsClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.currentState = NetworkingState.RequestingOwnership;
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
		case NetworkingState.RequestingOwnershipWaitingForSight:
		case NetworkingState.ForcefullyTakingOverWaitingForSight:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			base.StartCoroutine("RequestTimeout");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x0006F544 File Offset: 0x0006D744
	public void RequestOwnershipImmediately(Action onRequestFailed)
	{
		Debug.Log("WorldShareable RequestOwnershipImmediately");
		if (this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			this.RequestOwnershipImmediatelyWithGuaranteedAuthority();
			return;
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
		{
			bool inRoom = NetworkSystem.Instance.InRoom;
			return;
		}
		case NetworkingState.IsBlindClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.currentState = NetworkingState.ForcefullyTakingOverWaitingForSight;
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, true, false);
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		case NetworkingState.IsClient:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.ownershipRequestNonce = Guid.NewGuid().ToString();
			this.currentState = NetworkingState.ForcefullyTakingOver;
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, true, false);
			this.netView.SendRPC("OwnershipRequested", this.actualOwner, new object[]
			{
				this.ownershipRequestNonce
			});
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.ownershipDenied = (Action)Delegate.Combine(this.ownershipDenied, onRequestFailed);
			this.currentState = NetworkingState.ForcefullyTakingOver;
			base.StartCoroutine("RequestTimeout");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x0006F688 File Offset: 0x0006D888
	public void RequestOwnershipImmediatelyWithGuaranteedAuthority()
	{
		Debug.Log("WorldShareable RequestOwnershipImmediatelyWithGuaranteedAuthority");
		if (!this.PlayerHasAuthority(NetworkSystem.Instance.LocalPlayer))
		{
			Debug.LogError("Tried to request ownership immediately with guaranteed authority without acutely having authority ");
		}
		switch (this.currentState)
		{
		case NetworkingState.IsOwner:
			return;
		case NetworkingState.IsBlindClient:
			this.currentState = NetworkingState.ForcefullyTakingOverWaitingForSight;
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, true, false);
			this.RequestTheCurrentOwnerFromAuthority();
			return;
		case NetworkingState.IsClient:
			this.currentState = NetworkingState.ForcefullyTakingOver;
			this.SetOwnership(NetworkSystem.Instance.LocalPlayer, true, false);
			this.netView.SendRPC("SetOwnershipFromMasterClient", 0, new object[]
			{
				PhotonNetwork.LocalPlayer
			});
			base.StartCoroutine("RequestTimeout");
			return;
		case NetworkingState.ForcefullyTakingOver:
		case NetworkingState.RequestingOwnership:
			this.currentState = NetworkingState.ForcefullyTakingOver;
			base.StartCoroutine("RequestTimeout");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x0006F75D File Offset: 0x0006D95D
	public void AddCallbackTarget(IRequestableOwnershipGuardCallbacks callbackObject)
	{
		if (!this.callbacksList.Contains(callbackObject))
		{
			this.callbacksList.Add(callbackObject);
			if (this.currentOwner != null)
			{
				callbackObject.OnOwnershipTransferred(this.currentOwner, null);
			}
		}
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x0006F78E File Offset: 0x0006D98E
	public void RemoveCallbackTarget(IRequestableOwnershipGuardCallbacks callbackObject)
	{
		if (this.callbacksList.Contains(callbackObject))
		{
			this.callbacksList.Remove(callbackObject);
			if (this.currentOwner != null)
			{
				callbackObject.OnOwnershipTransferred(null, this.currentOwner);
			}
		}
	}

	// Token: 0x06001327 RID: 4903 RVA: 0x0006F7C0 File Offset: 0x0006D9C0
	public void SetCreator(NetPlayer player)
	{
		this.creator = player;
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06001328 RID: 4904 RVA: 0x0006F7C9 File Offset: 0x0006D9C9
	private NetworkingState EdCurrentState
	{
		get
		{
			return this.currentState;
		}
	}

	// Token: 0x06001329 RID: 4905 RVA: 0x00002789 File Offset: 0x00000989
	public void Validate(SelfValidationResult result)
	{
	}

	// Token: 0x0600132A RID: 4906 RVA: 0x0006F7D1 File Offset: 0x0006D9D1
	public bool PlayerHasAuthority(NetPlayer player)
	{
		return object.Equals(this.GetAuthoritativePlayer(), player);
	}

	// Token: 0x04001CAC RID: 7340
	[DevInspectorShow]
	[DevInspectorColor("#ff5")]
	public NetworkingState currentState;

	// Token: 0x04001CAD RID: 7341
	[FormerlySerializedAs("NetworkView")]
	[SerializeField]
	private NetworkView[] netViews;

	// Token: 0x04001CAE RID: 7342
	[DevInspectorHide]
	[SerializeField]
	private bool autoRegister = true;

	// Token: 0x04001CAF RID: 7343
	[DevInspectorShow]
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	public NetPlayer currentOwner;

	// Token: 0x04001CB0 RID: 7344
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	private NetPlayer currentMasterClient;

	// Token: 0x04001CB1 RID: 7345
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	private NetPlayer fallbackOwner;

	// Token: 0x04001CB2 RID: 7346
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	public NetPlayer creator;

	// Token: 0x04001CB3 RID: 7347
	public bool giveCreatorAbsoluteAuthority;

	// Token: 0x04001CB4 RID: 7348
	public bool attemptMasterAssistedTakeoverOnDeny;

	// Token: 0x04001CB5 RID: 7349
	private Action ownershipDenied;

	// Token: 0x04001CB6 RID: 7350
	private Action ownershipRequestAccepted;

	// Token: 0x04001CB7 RID: 7351
	[CanBeNull]
	[SerializeField]
	[SerializeReference]
	[DevInspectorShow]
	public NetPlayer actualOwner;

	// Token: 0x04001CB8 RID: 7352
	public string ownershipRequestNonce;

	// Token: 0x04001CB9 RID: 7353
	public List<IRequestableOwnershipGuardCallbacks> callbacksList = new List<IRequestableOwnershipGuardCallbacks>();
}
