using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003A0 RID: 928
public class FusionCallbackHandler : SimulationBehaviour, INetworkRunnerCallbacks, IPublicFacingInterface
{
	// Token: 0x06001621 RID: 5665 RVA: 0x0007B300 File Offset: 0x00079500
	public void Setup(NetworkSystemFusion parentController)
	{
		this.parent = parentController;
		this.parent.runner.AddCallbacks(new INetworkRunnerCallbacks[]
		{
			this
		});
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x0007B323 File Offset: 0x00079523
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.RemoveCallbacks();
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x0007B334 File Offset: 0x00079534
	private void RemoveCallbacks()
	{
		FusionCallbackHandler.<RemoveCallbacks>d__3 <RemoveCallbacks>d__;
		<RemoveCallbacks>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RemoveCallbacks>d__.<>4__this = this;
		<RemoveCallbacks>d__.<>1__state = -1;
		<RemoveCallbacks>d__.<>t__builder.Start<FusionCallbackHandler.<RemoveCallbacks>d__3>(ref <RemoveCallbacks>d__);
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x0007B36B File Offset: 0x0007956B
	public void OnConnectedToServer(NetworkRunner runner)
	{
		this.parent.OnJoinedSession();
	}

	// Token: 0x06001625 RID: 5669 RVA: 0x0007B378 File Offset: 0x00079578
	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		this.parent.OnJoinFailed(reason);
	}

	// Token: 0x06001626 RID: 5670 RVA: 0x00002789 File Offset: 0x00000989
	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	// Token: 0x06001627 RID: 5671 RVA: 0x0007B388 File Offset: 0x00079588
	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
		this.parent.CustomAuthenticationResponse(data);
		Debug.Log("Received custom auth response:");
		foreach (KeyValuePair<string, object> keyValuePair in data)
		{
			Debug.Log(keyValuePair.Key + ":" + (keyValuePair.Value as string));
		}
	}

	// Token: 0x06001628 RID: 5672 RVA: 0x0007B408 File Offset: 0x00079608
	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
		this.parent.OnDisconnectedFromSession();
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x0007B415 File Offset: 0x00079615
	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
		this.parent.MigrateHost(runner, hostMigrationToken);
	}

	// Token: 0x0600162A RID: 5674 RVA: 0x0007B424 File Offset: 0x00079624
	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		NetworkedInput input2 = NetInput.GetInput();
		input.Set<NetworkedInput>(input2);
	}

	// Token: 0x0600162B RID: 5675 RVA: 0x00002789 File Offset: 0x00000989
	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x0007B440 File Offset: 0x00079640
	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerJoined(player);
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x0007B44E File Offset: 0x0007964E
	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		this.parent.OnFusionPlayerLeft(player);
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x00002789 File Offset: 0x00000989
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x00002789 File Offset: 0x00000989
	public void OnSceneLoadDone(NetworkRunner runner)
	{
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x00002789 File Offset: 0x00000989
	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x00002789 File Offset: 0x00000989
	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x0007B45C File Offset: 0x0007965C
	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
		this.parent.OnRunnerShutDown();
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x00002789 File Offset: 0x00000989
	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x0007B46C File Offset: 0x0007966C
	[Rpc(Channel = 0)]
	public unsafe static void RPC_OnEventRaisedReliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage != 4)
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3 & -4);
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3 & -4);
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)"));
						int num2 = 8;
						ptr2[num2] = eventCode;
						num2 += (1 + 3 & -4);
						*(int*)(ptr2 + num2) = byteData.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<byte>((void*)(ptr2 + num2), byteData) + 3 & -4) + num2;
						ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), hasOps);
						num2 += 4;
						*(int*)(ptr2 + num2) = netOptsData.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<byte>((void*)(ptr2 + num2), netOptsData) + 3 & -4) + num2;
						ptr.Offset = num2 * 8;
						ptr.SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, 0, 0);
					goto IL_10;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_10:
		object data = byteData.ByteDeserialize();
		NetEventOptions opts = null;
		if (hasOps)
		{
			opts = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, opts, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, data, info.Source.PlayerId);
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x0007B690 File Offset: 0x00079890
	[Rpc(Channel = 1)]
	public unsafe static void RPC_OnEventRaisedUnreliable(NetworkRunner runner, byte eventCode, byte[] byteData, bool hasOps, byte[] netOptsData, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage != 4)
			{
				int num = 8;
				num += 4;
				num += (byteData.Length * 1 + 4 + 3 & -4);
				num += 4;
				num += (netOptsData.Length * 1 + 4 + 3 & -4);
				if (SimulationMessage.CanAllocateUserPayload(num))
				{
					if (runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)"));
						int num2 = 8;
						ptr2[num2] = eventCode;
						num2 += (1 + 3 & -4);
						*(int*)(ptr2 + num2) = byteData.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<byte>((void*)(ptr2 + num2), byteData) + 3 & -4) + num2;
						ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), hasOps);
						num2 += 4;
						*(int*)(ptr2 + num2) = netOptsData.Length;
						num2 += 4;
						num2 = (Native.CopyFromArray<byte>((void*)(ptr2 + num2), netOptsData) + 3 & -4) + num2;
						ptr.Offset = num2 * 8;
						ptr.SetUnreliable();
						ptr.SetStatic();
						runner.SendRpc(ptr);
					}
					info = RpcInfo.FromLocal(runner, 1, 0);
					goto IL_10;
				}
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)", num);
			}
			return;
		}
		IL_10:
		object data = byteData.ByteDeserialize();
		NetEventOptions opts = null;
		if (hasOps)
		{
			opts = (NetEventOptions)netOptsData.ByteDeserialize();
		}
		if (!FusionCallbackHandler.CanRecieveEvent(runner, opts, info))
		{
			return;
		}
		NetworkSystem.Instance.RaiseEvent(eventCode, data, info.Source.PlayerId);
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x0007B8BC File Offset: 0x00079ABC
	private static bool CanRecieveEvent(NetworkRunner runner, NetEventOptions opts, RpcInfo info)
	{
		if (opts != null)
		{
			if (opts.Reciever != NetEventOptions.RecieverTarget.all)
			{
				if (opts.Reciever == NetEventOptions.RecieverTarget.master && !NetworkSystem.Instance.IsMasterClient)
				{
					return false;
				}
				if (info.Source == runner.LocalPlayer)
				{
					return false;
				}
			}
			if (opts.TargetActors != null && !Enumerable.Contains<int>(opts.TargetActors, runner.LocalPlayer.PlayerId))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x00002789 File Offset: 0x00000989
	public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x00002789 File Offset: 0x00000989
	public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
	{
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
	{
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x00002789 File Offset: 0x00000989
	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
	{
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x00002789 File Offset: 0x00000989
	public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
	{
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x0007B930 File Offset: 0x00079B30
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedReliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedReliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		byte b = ptr[num];
		num += (1 + 3 & -4);
		byte eventCode = b;
		byte[] array = new byte[*(int*)(ptr + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array, (void*)(ptr + num)) + 3 & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
		num += 4;
		bool hasOps = flag;
		byte[] array2 = new byte[*(int*)(ptr + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array2, (void*)(ptr + num)) + 3 & -4) + num;
		RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedReliable(runner, eventCode, array, hasOps, array2, info);
	}

	// Token: 0x0600163E RID: 5694 RVA: 0x0007BA40 File Offset: 0x00079C40
	[NetworkRpcStaticWeavedInvoker("System.Void FusionCallbackHandler::RPC_OnEventRaisedUnreliable(Fusion.NetworkRunner,System.Byte,System.Byte[],System.Boolean,System.Byte[],Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_OnEventRaisedUnreliable@Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		byte b = ptr[num];
		num += (1 + 3 & -4);
		byte eventCode = b;
		byte[] array = new byte[*(int*)(ptr + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array, (void*)(ptr + num)) + 3 & -4) + num;
		bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
		num += 4;
		bool hasOps = flag;
		byte[] array2 = new byte[*(int*)(ptr + num)];
		num += 4;
		num = (Native.CopyToArray<byte>(array2, (void*)(ptr + num)) + 3 & -4) + num;
		RpcInfo info = RpcInfo.FromMessage(runner, message, 0);
		NetworkBehaviourUtils.InvokeRpc = true;
		FusionCallbackHandler.RPC_OnEventRaisedUnreliable(runner, eventCode, array, hasOps, array2, info);
	}

	// Token: 0x0400205E RID: 8286
	private NetworkSystemFusion parent;
}
