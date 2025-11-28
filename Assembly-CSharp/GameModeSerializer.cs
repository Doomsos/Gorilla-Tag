using System;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000754 RID: 1876
[NetworkBehaviourWeaved(1)]
internal class GameModeSerializer : GorillaSerializerMasterOnly, IStateAuthorityChanged, IPublicFacingInterface
{
	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x06003061 RID: 12385 RVA: 0x00108CF8 File Offset: 0x00106EF8
	// (set) Token: 0x06003062 RID: 12386 RVA: 0x00108D1E File Offset: 0x00106F1E
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe int gameModeKeyInt
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameModeSerializer.gameModeKeyInt. Networked properties can only be accessed when Spawned() has been called.");
			}
			return this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameModeSerializer.gameModeKeyInt. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = value;
		}
	}

	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x06003063 RID: 12387 RVA: 0x00108D45 File Offset: 0x00106F45
	public GorillaGameManager GameModeInstance
	{
		get
		{
			return this.gameModeInstance;
		}
	}

	// Token: 0x06003064 RID: 12388 RVA: 0x00108D50 File Offset: 0x00106F50
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (player != null)
		{
			GorillaNot.IncrementRPCCall(wrappedInfo, "OnSpawnSetupCheck");
		}
		GameModeSerializer activeNetworkHandler = GameMode.ActiveNetworkHandler;
		if (player != null && player.InRoom)
		{
			if (!player.IsMasterClient)
			{
				GTDev.LogError<string>("SPAWN FAIL NOT MASTER :" + player.UserId + player.NickName, null);
				GorillaNot.instance.SendReport("trying to inappropriately create game managers", player.UserId, player.NickName);
				return false;
			}
			if (!this.netView.IsRoomView)
			{
				GTDev.LogError<string>("SPAWN FAIL ROOM VIEW" + player.UserId + player.NickName, null);
				GorillaNot.instance.SendReport("creating game manager as player object", player.UserId, player.NickName);
				return false;
			}
			if (activeNetworkHandler.IsNotNull() && activeNetworkHandler != this)
			{
				GTDev.LogError<string>("DUPLICATE CHECK" + player.UserId + player.NickName, null);
				GorillaNot.instance.SendReport("trying to create multiple game managers", player.UserId, player.NickName);
				return false;
			}
		}
		else if ((activeNetworkHandler.IsNotNull() && activeNetworkHandler != this) || !this.netView.IsRoomView)
		{
			GTDev.LogError<string>("ACTIVE HANDLER CHECK FAIL" + ((player != null) ? player.UserId : null) + ((player != null) ? player.NickName : null), null);
			GTDev.LogError<string>("existing game manager! destroying newly created manager", null);
			return false;
		}
		object[] instantiationData = wrappedInfo.punInfo.photonView.InstantiationData;
		if (instantiationData != null && instantiationData.Length >= 1)
		{
			object obj = instantiationData[0];
			if (obj is int)
			{
				int num = (int)obj;
				this.gameModeKey = (GameModeType)num;
				this.gameModeInstance = GameMode.GetGameModeInstance(this.gameModeKey);
				if (this.gameModeInstance.IsNull() || !this.gameModeInstance.ValidGameMode())
				{
					return false;
				}
				this.serializeTarget = this.gameModeInstance;
				base.transform.parent = VRRigCache.Instance.NetworkParent;
				return true;
			}
		}
		GTDev.LogError<string>("missing instantiation data", null);
		return false;
	}

	// Token: 0x06003065 RID: 12389 RVA: 0x00108F5E File Offset: 0x0010715E
	internal void Init(int gameModeType)
	{
		Debug.Log("<color=red>Init called</color>");
		this.gameModeKeyInt = gameModeType;
	}

	// Token: 0x06003066 RID: 12390 RVA: 0x00108F71 File Offset: 0x00107171
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		this.netView.GetView.AddCallbackTarget(this);
		GameMode.SetupGameModeRemote(this);
	}

	// Token: 0x06003067 RID: 12391 RVA: 0x00108F8A File Offset: 0x0010718A
	protected override void OnBeforeDespawn()
	{
		GameMode.RemoveNetworkLink(this);
	}

	// Token: 0x06003068 RID: 12392 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06003069 RID: 12393 RVA: 0x00108F92 File Offset: 0x00107192
	[PunRPC]
	internal void RPC_ReportTag(int taggedPlayer, PhotonMessageInfo info)
	{
		this.ReportTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600306A RID: 12394 RVA: 0x00108FAB File Offset: 0x001071AB
	[PunRPC]
	internal void RPC_ReportHit(PhotonMessageInfo info)
	{
		this.ReportHit(new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600306B RID: 12395 RVA: 0x00108FBC File Offset: 0x001071BC
	[Rpc(7, 7)]
	internal unsafe void RPC_ReportTag(int taggedPlayer, RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != 4)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameModeSerializer::RPC_ReportTag(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					int num = 8;
					num += 4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameModeSerializer::RPC_ReportTag(System.Int32,Fusion.RpcInfo)", num);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
							int num2 = 8;
							*(int*)(ptr2 + num2) = taggedPlayer;
							num2 += 4;
							ptr.Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, 0, 0);
							goto IL_12;
						}
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		this.ReportTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600306C RID: 12396 RVA: 0x00109128 File Offset: 0x00107328
	[Rpc(7, 7)]
	internal unsafe void RPC_ReportHit(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != 4)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameModeSerializer::RPC_ReportHit(Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					int num = 8;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameModeSerializer::RPC_ReportHit(Fusion.RpcInfo)", num);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2);
							int num2 = 8;
							ptr.Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							info = RpcInfo.FromLocal(base.Runner, 0, 0);
							goto IL_12;
						}
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		this.ReportHit(new PhotonMessageInfoWrapped(info));
	}

	// Token: 0x0600306D RID: 12397 RVA: 0x00109268 File Offset: 0x00107468
	private void ReportTag(NetPlayer taggedPlayer, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportTag");
		NetPlayer sender = info.Sender;
		this.gameModeInstance.ReportTag(taggedPlayer, sender);
	}

	// Token: 0x0600306E RID: 12398 RVA: 0x00109294 File Offset: 0x00107494
	private void ReportHit(PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "ReportContactWithLavaRPC");
		bool flag = ZoneManagement.instance.IsZoneActive(GTZone.customMaps);
		bool flag2 = false;
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			InfectionLavaController instance = InfectionLavaController.Instance;
			flag2 = (instance != null && instance.LavaCurrentlyActivated && (instance.SurfaceCenter - rigContainer.Rig.syncPos).sqrMagnitude < 2500f && instance.LavaPlane.GetDistanceToPoint(rigContainer.Rig.syncPos) < 5f);
		}
		if (flag || flag2)
		{
			this.GameModeInstance.HitPlayer(info.Sender);
		}
	}

	// Token: 0x0600306F RID: 12399 RVA: 0x00109344 File Offset: 0x00107544
	[PunRPC]
	internal void RPC_BroadcastRoundComplete(PhotonMessageInfo info)
	{
		this.BroadcastRoundComplete(info);
	}

	// Token: 0x06003070 RID: 12400 RVA: 0x00109352 File Offset: 0x00107552
	private void BroadcastRoundComplete(PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "BroadcastRoundComplete");
		if (info.Sender.IsMasterClient)
		{
			this.gameModeInstance.HandleRoundComplete();
		}
	}

	// Token: 0x06003071 RID: 12401 RVA: 0x00109377 File Offset: 0x00107577
	[PunRPC]
	internal void RPC_BroadcastTag(int taggedPlayer, int taggingPlayer, PhotonMessageInfo info)
	{
		this.BroadcastTag(NetworkSystem.Instance.GetPlayer(taggedPlayer), NetworkSystem.Instance.GetPlayer(taggingPlayer), info);
	}

	// Token: 0x06003072 RID: 12402 RVA: 0x00109398 File Offset: 0x00107598
	private void BroadcastTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "BroadcastTag");
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		if (taggedPlayer == null || taggingPlayer == null)
		{
			return;
		}
		if (!this.broadcastTagCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		this.gameModeInstance.HandleTagBroadcast(taggedPlayer, taggingPlayer);
	}

	// Token: 0x06003073 RID: 12403 RVA: 0x001093E5 File Offset: 0x001075E5
	protected override void FusionDataRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
		Debug.Log(this.gameModeData.GetType().Name);
	}

	// Token: 0x06003074 RID: 12404 RVA: 0x001093FC File Offset: 0x001075FC
	protected override void FusionDataRPC(string method, RpcTarget target, params object[] parameters)
	{
		base.FusionDataRPC(method, target, parameters);
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x00109407 File Offset: 0x00107607
	void IStateAuthorityChanged.StateAuthorityChanged()
	{
		GameModeSerializer.FusionGameModeOwnerChanged.Invoke(NetworkSystem.Instance.GetPlayer(base.Object.StateAuthority));
	}

	// Token: 0x06003077 RID: 12407 RVA: 0x00109447 File Offset: 0x00107647
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.gameModeKeyInt = this._gameModeKeyInt;
	}

	// Token: 0x06003078 RID: 12408 RVA: 0x0010945F File Offset: 0x0010765F
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._gameModeKeyInt = this.gameModeKeyInt;
	}

	// Token: 0x06003079 RID: 12409 RVA: 0x00109474 File Offset: 0x00107674
	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportTag@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int taggedPlayer = num2;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
		behaviour.InvokeRpc = true;
		((GameModeSerializer)behaviour).RPC_ReportTag(taggedPlayer, info);
	}

	// Token: 0x0600307A RID: 12410 RVA: 0x001094D4 File Offset: 0x001076D4
	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
		behaviour.InvokeRpc = true;
		((GameModeSerializer)behaviour).RPC_ReportHit(info);
	}

	// Token: 0x04003F7E RID: 16254
	[WeaverGenerated]
	[DefaultForProperty("gameModeKeyInt", 0, 1)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private int _gameModeKeyInt;

	// Token: 0x04003F7F RID: 16255
	private GameModeType gameModeKey;

	// Token: 0x04003F80 RID: 16256
	private GorillaGameManager gameModeInstance;

	// Token: 0x04003F81 RID: 16257
	private FusionGameModeData gameModeData;

	// Token: 0x04003F82 RID: 16258
	private Type currentGameDataType;

	// Token: 0x04003F83 RID: 16259
	private CallLimiter broadcastTagCallLimit = new CallLimiter(12, 5f, 0.5f);

	// Token: 0x04003F84 RID: 16260
	public static Action<NetPlayer> FusionGameModeOwnerChanged;
}
