using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTag.Cosmetics;

public class EnvironmentProximityReactorManager : NetworkSceneObject
{
	private static EnvironmentProximityReactorManager instance;

	[SerializeField]
	private List<EnvironmentProximityReactor> reactors = new List<EnvironmentProximityReactor>();

	private readonly HashSet<int> idSet = new HashSet<int>();

	private static readonly HashSet<EnvironmentProximityReactor> registry = new HashSet<EnvironmentProximityReactor>();

	public static EnvironmentProximityReactorManager Instance => instance;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			GTDev.LogWarning("[EnvironmentProximityReactorManager] Duplicate instance — destroying.");
			UnityEngine.Object.Destroy(this);
			return;
		}
		instance = this;
		foreach (EnvironmentProximityReactor item in registry)
		{
			if (item != null)
			{
				RegisterInstance(item);
			}
		}
		registry.Clear();
	}

	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (instance == this)
		{
			instance = null;
		}
	}

	private void RegisterInstance(EnvironmentProximityReactor reactor)
	{
		if (!(reactor == null) && idSet.Add(reactor.reactorId))
		{
			reactors.Add(reactor);
		}
	}

	private void UnregisterInstance(EnvironmentProximityReactor reactor)
	{
		if (!(reactor == null) && idSet.Remove(reactor.reactorId))
		{
			reactors.Remove(reactor);
		}
	}

	public static void Register(EnvironmentProximityReactor reactor)
	{
		if (instance != null)
		{
			instance.RegisterInstance(reactor);
		}
		else
		{
			registry.Add(reactor);
		}
	}

	public static void Unregister(EnvironmentProximityReactor reactor)
	{
		if (instance != null)
		{
			instance.UnregisterInstance(reactor);
		}
		else
		{
			registry.Remove(reactor);
		}
	}

	public void BroadcastProximityState(int reactorId, int blockIndex, bool isBelow)
	{
		_ = NetworkSystem.Instance.InRoom;
	}

	public void ProximityStateRPC(int reactorId, int blockIndex, bool isBelow, PhotonMessageInfo info)
	{
		ApplyProximityStateShared(reactorId, blockIndex, isBelow, new PhotonMessageInfoWrapped(info));
	}

	[Rpc]
	public unsafe static void RPC_ProximityState(NetworkRunner runner, int reactorId, int blockIndex, bool isBelow, RpcInfo info = default(RpcInfo))
	{
		if (NetworkBehaviourUtils.InvokeRpc)
		{
			NetworkBehaviourUtils.InvokeRpc = false;
		}
		else
		{
			if ((object)runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.Cosmetics.EnvironmentProximityReactorManager::RPC_ProximityState(Fusion.NetworkRunner,System.Int32,System.Int32,System.Boolean,Fusion.RpcInfo)", num);
				return;
			}
			if (runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
				byte* ptr2 = (byte*)ptr + 28;
				*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTag.Cosmetics.EnvironmentProximityReactorManager::RPC_ProximityState(Fusion.NetworkRunner,System.Int32,System.Int32,System.Boolean,Fusion.RpcInfo)"));
				int num2 = 8;
				*(int*)(ptr2 + num2) = reactorId;
				num2 += 4;
				*(int*)(ptr2 + num2) = blockIndex;
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), isBelow);
				num2 += 4;
				ptr->Offset = num2 * 8;
				ptr->SetStatic();
				runner.SendRpc(ptr);
			}
			info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
		}
		if (!(instance == null))
		{
			instance.ApplyProximityStateShared(reactorId, blockIndex, isBelow, new PhotonMessageInfoWrapped(info));
		}
	}

	private void ApplyProximityStateShared(int reactorId, int blockIndex, bool isBelow, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "ApplyProximityStateShared");
		if (blockIndex < 0)
		{
			return;
		}
		for (int i = 0; i < reactors.Count; i++)
		{
			EnvironmentProximityReactor environmentProximityReactor = reactors[i];
			if (!(environmentProximityReactor == null) && environmentProximityReactor.reactorId == reactorId)
			{
				environmentProximityReactor.ApplySharedProximity(blockIndex, isBelow);
				break;
			}
		}
	}

	[NetworkRpcStaticWeavedInvoker("System.Void GorillaTag.Cosmetics.EnvironmentProximityReactorManager::RPC_ProximityState(Fusion.NetworkRunner,System.Int32,System.Int32,System.Boolean,Fusion.RpcInfo)")]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ProximityState_0040Invoker(NetworkRunner runner, SimulationMessage* message)
	{
		byte* ptr = (byte*)message + 28;
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int reactorId = num2;
		int num3 = *(int*)(ptr + num);
		num += 4;
		int blockIndex = num3;
		bool num4 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
		num += 4;
		bool isBelow = num4;
		RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
		NetworkBehaviourUtils.InvokeRpc = true;
		RPC_ProximityState(runner, reactorId, blockIndex, isBelow, info);
	}
}
