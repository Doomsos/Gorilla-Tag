using System;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000188 RID: 392
[NetworkBehaviourWeaved(11)]
public class SecondLookSkeletonSynchValues : NetworkComponent
{
	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x06000A7D RID: 2685 RVA: 0x00038D92 File Offset: 0x00036F92
	// (set) Token: 0x06000A7E RID: 2686 RVA: 0x00038DBC File Offset: 0x00036FBC
	[Networked]
	[NetworkedWeaved(0, 11)]
	public unsafe SkeletonNetData NetData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing SecondLookSkeletonSynchValues.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(SkeletonNetData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing SecondLookSkeletonSynchValues.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(SkeletonNetData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x00038DE7 File Offset: 0x00036FE7
	protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
	{
		base.OnOwnerSwitched(newOwningPlayer);
		if (newOwningPlayer.IsLocal)
		{
			this.mySkeleton.SetNodes();
			if (this.mySkeleton.currentState != this.currentState)
			{
				this.mySkeleton.ChangeState(this.currentState);
			}
		}
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x00038E27 File Offset: 0x00037027
	public override void WriteDataFusion()
	{
		this.NetData = new SkeletonNetData((int)this.currentState, this.position, this.rotation, this.currentNode, this.nextNode, this.angerPoint);
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x00038E58 File Offset: 0x00037058
	public override void ReadDataFusion()
	{
		this.currentState = (SecondLookSkeleton.GhostState)this.NetData.CurrentState;
		Vector3 vector = this.NetData.Position;
		ref this.position.SetValueSafe(vector);
		Quaternion quaternion = this.NetData.Rotation;
		ref this.rotation.SetValueSafe(quaternion);
		this.currentNode = this.NetData.CurrentNode;
		this.nextNode = this.NetData.NextNode;
		this.angerPoint = this.NetData.AngerPoint;
		if (this.mySkeleton.tapped && this.currentState != this.mySkeleton.currentState)
		{
			this.mySkeleton.ChangeState(this.currentState);
		}
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x00038F20 File Offset: 0x00037120
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.mySkeleton.currentState);
		stream.SendNext(this.mySkeleton.spookyGhost.transform.position);
		stream.SendNext(this.mySkeleton.spookyGhost.transform.rotation);
		stream.SendNext(this.currentNode);
		stream.SendNext(this.nextNode);
		stream.SendNext(this.angerPoint);
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x00038FCC File Offset: 0x000371CC
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		this.currentState = (SecondLookSkeleton.GhostState)stream.ReceiveNext();
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.position.SetValueSafe(vector);
		Quaternion quaternion = (Quaternion)stream.ReceiveNext();
		ref this.rotation.SetValueSafe(quaternion);
		this.currentNode = (int)stream.ReceiveNext();
		this.nextNode = (int)stream.ReceiveNext();
		this.angerPoint = (int)stream.ReceiveNext();
		if (this.mySkeleton.tapped && this.currentState != this.mySkeleton.currentState)
		{
			this.mySkeleton.ChangeState(this.currentState);
		}
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x00039098 File Offset: 0x00037298
	[Rpc(7, 1)]
	public unsafe void RPC_RemoteActiveGhost(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != 4)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) != 0)
				{
					if ((localAuthorityMask & 1) != 1)
					{
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void SecondLookSkeletonSynchValues::RPC_RemoteActiveGhost(Fusion.RpcInfo)", num);
							return;
						}
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
							int num2 = 8;
							ptr.Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 1) == 0)
						{
							return;
						}
					}
					info = RpcInfo.FromLocal(base.Runner, 0, 0);
					goto IL_12;
				}
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void SecondLookSkeletonSynchValues::RPC_RemoteActiveGhost(Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		GorillaNot.IncrementRPCCall(info, "RPC_RemoteActiveGhost");
		if (!base.IsMine)
		{
			return;
		}
		this.mySkeleton.RemoteActivateGhost();
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x000391FC File Offset: 0x000373FC
	[Rpc(7, 7)]
	public unsafe void RPC_RemotePlayerSeen(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != 4)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void SecondLookSkeletonSynchValues::RPC_RemotePlayerSeen(Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					int num = 8;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void SecondLookSkeletonSynchValues::RPC_RemotePlayerSeen(Fusion.RpcInfo)", num);
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
		GorillaNot.IncrementRPCCall(info, "RPC_RemotePlayerSeen");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Source);
		if (!this.mySkeleton.playersSeen.Contains(player))
		{
			this.mySkeleton.RemotePlayerSeen(player);
		}
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x00039370 File Offset: 0x00037570
	[Rpc(7, 1)]
	public unsafe void RPC_RemotePlayerCaught(RpcInfo info = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != 4)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) != 0)
				{
					if ((localAuthorityMask & 1) != 1)
					{
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void SecondLookSkeletonSynchValues::RPC_RemotePlayerCaught(Fusion.RpcInfo)", num);
							return;
						}
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 3);
							int num2 = 8;
							ptr.Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 1) == 0)
						{
							return;
						}
					}
					info = RpcInfo.FromLocal(base.Runner, 0, 0);
					goto IL_12;
				}
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void SecondLookSkeletonSynchValues::RPC_RemotePlayerCaught(Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		GorillaNot.IncrementRPCCall(info, "RPC_RemotePlayerCaught");
		if (!base.IsMine)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Source);
		if (this.mySkeleton.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.mySkeleton.RemotePlayerCaught(player);
		}
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x000394F2 File Offset: 0x000376F2
	[PunRPC]
	public void RemoteActivateGhost(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteActivateGhost");
		if (!base.IsMine)
		{
			return;
		}
		this.mySkeleton.RemoteActivateGhost();
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x00039514 File Offset: 0x00037714
	[PunRPC]
	public void RemotePlayerSeen(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemotePlayerSeen");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (!this.mySkeleton.playersSeen.Contains(player))
		{
			this.mySkeleton.RemotePlayerSeen(player);
		}
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0003955C File Offset: 0x0003775C
	[PunRPC]
	public void RemotePlayerCaught(PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemotePlayerCaught");
		if (!base.IsMine)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.mySkeleton.currentState == SecondLookSkeleton.GhostState.Chasing)
		{
			this.mySkeleton.RemotePlayerCaught(player);
		}
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x000395A8 File Offset: 0x000377A8
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.NetData = this._NetData;
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x000395C0 File Offset: 0x000377C0
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._NetData = this.NetData;
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x000395D4 File Offset: 0x000377D4
	[NetworkRpcWeavedInvoker(1, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemoteActiveGhost@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
		behaviour.InvokeRpc = true;
		((SecondLookSkeletonSynchValues)behaviour).RPC_RemoteActiveGhost(info);
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x00039618 File Offset: 0x00037818
	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemotePlayerSeen@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
		behaviour.InvokeRpc = true;
		((SecondLookSkeletonSynchValues)behaviour).RPC_RemotePlayerSeen(info);
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x0003965C File Offset: 0x0003785C
	[NetworkRpcWeavedInvoker(3, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemotePlayerCaught@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
		behaviour.InvokeRpc = true;
		((SecondLookSkeletonSynchValues)behaviour).RPC_RemotePlayerCaught(info);
	}

	// Token: 0x04000CD4 RID: 3284
	public SecondLookSkeleton.GhostState currentState;

	// Token: 0x04000CD5 RID: 3285
	public Vector3 position;

	// Token: 0x04000CD6 RID: 3286
	public Quaternion rotation;

	// Token: 0x04000CD7 RID: 3287
	public SecondLookSkeleton mySkeleton;

	// Token: 0x04000CD8 RID: 3288
	public int currentNode;

	// Token: 0x04000CD9 RID: 3289
	public int nextNode;

	// Token: 0x04000CDA RID: 3290
	public int angerPoint;

	// Token: 0x04000CDB RID: 3291
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("NetData", 0, 11)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private SkeletonNetData _NetData;
}
