using System;
using Fusion;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Scripting;

// Token: 0x02000180 RID: 384
[NetworkBehaviourWeaved(11)]
public class GhostLabReliableState : NetworkComponent
{
	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x06000A30 RID: 2608 RVA: 0x00036E00 File Offset: 0x00035000
	// (set) Token: 0x06000A31 RID: 2609 RVA: 0x00036E2A File Offset: 0x0003502A
	[Networked]
	[NetworkedWeaved(0, 11)]
	private unsafe GhostLabData NetData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GhostLabReliableState.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GhostLabData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GhostLabReliableState.NetData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GhostLabData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00036E55 File Offset: 0x00035055
	protected override void Awake()
	{
		base.Awake();
		this.singleDoorOpen = new bool[this.singleDoorCount];
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x00036E6E File Offset: 0x0003506E
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		Player localPlayer = PhotonNetwork.LocalPlayer;
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x00036E80 File Offset: 0x00035080
	public override void WriteDataFusion()
	{
		this.NetData = new GhostLabData((int)this.doorState, this.singleDoorOpen);
	}

	// Token: 0x06000A35 RID: 2613 RVA: 0x00036E9C File Offset: 0x0003509C
	public override void ReadDataFusion()
	{
		this.doorState = (GhostLab.EntranceDoorsState)this.NetData.DoorState;
		for (int i = 0; i < this.singleDoorCount; i++)
		{
			this.singleDoorOpen[i] = this.NetData.OpenDoors[i];
		}
	}

	// Token: 0x06000A36 RID: 2614 RVA: 0x00036EF4 File Offset: 0x000350F4
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.doorState);
		for (int i = 0; i < this.singleDoorOpen.Length; i++)
		{
			stream.SendNext(this.singleDoorOpen[i]);
		}
	}

	// Token: 0x06000A37 RID: 2615 RVA: 0x00036F50 File Offset: 0x00035150
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!base.IsMine && !info.Sender.IsMasterClient)
		{
			return;
		}
		this.doorState = (GhostLab.EntranceDoorsState)stream.ReceiveNext();
		for (int i = 0; i < this.singleDoorOpen.Length; i++)
		{
			this.singleDoorOpen[i] = (bool)stream.ReceiveNext();
		}
	}

	// Token: 0x06000A38 RID: 2616 RVA: 0x00036FAC File Offset: 0x000351AC
	public void UpdateEntranceDoorsState(GhostLab.EntranceDoorsState newState)
	{
		if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
		{
			this.doorState = newState;
			return;
		}
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.IsMasterClient)
		{
			base.SendRPC("RemoteEntranceDoorState", 2, new object[]
			{
				newState
			});
		}
	}

	// Token: 0x06000A39 RID: 2617 RVA: 0x0003700C File Offset: 0x0003520C
	public void UpdateSingleDoorState(int singleDoorIndex)
	{
		if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
		{
			this.singleDoorOpen[singleDoorIndex] = !this.singleDoorOpen[singleDoorIndex];
			return;
		}
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.IsMasterClient)
		{
			base.SendRPC("RemoteSingleDoorState", 2, new object[]
			{
				singleDoorIndex
			});
		}
	}

	// Token: 0x06000A3A RID: 2618 RVA: 0x00037078 File Offset: 0x00035278
	[Rpc(7, 1)]
	public unsafe void RPC_RemoteEntranceDoorState(GhostLab.EntranceDoorsState newState, RpcInfo info = default(RpcInfo))
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
						num += 4;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GhostLabReliableState::RPC_RemoteEntranceDoorState(GhostLab/EntranceDoorsState,Fusion.RpcInfo)", num);
							return;
						}
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
							int num2 = 8;
							*(GhostLab.EntranceDoorsState*)(ptr2 + num2) = newState;
							num2 += 4;
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GhostLabReliableState::RPC_RemoteEntranceDoorState(GhostLab/EntranceDoorsState,Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		GorillaNot.IncrementRPCCall(info, "RPC_RemoteEntranceDoorState");
		if (!base.IsMine)
		{
			return;
		}
		this.doorState = newState;
	}

	// Token: 0x06000A3B RID: 2619 RVA: 0x000371FC File Offset: 0x000353FC
	[Rpc(7, 1)]
	public unsafe void RPC_RemoteSingleDoorState(int doorIndex, RpcInfo info = default(RpcInfo))
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
						num += 4;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GhostLabReliableState::RPC_RemoteSingleDoorState(System.Int32,Fusion.RpcInfo)", num);
							return;
						}
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2);
							int num2 = 8;
							*(int*)(ptr2 + num2) = doorIndex;
							num2 += 4;
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
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GhostLabReliableState::RPC_RemoteSingleDoorState(System.Int32,Fusion.RpcInfo)", base.Object, 7);
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		GorillaNot.IncrementRPCCall(info, "RPC_RemoteSingleDoorState");
		if (!base.IsMine)
		{
			return;
		}
		if (doorIndex >= this.singleDoorCount)
		{
			return;
		}
		this.singleDoorOpen[doorIndex] = !this.singleDoorOpen[doorIndex];
	}

	// Token: 0x06000A3C RID: 2620 RVA: 0x00037392 File Offset: 0x00035592
	[PunRPC]
	public void RemoteEntranceDoorState(GhostLab.EntranceDoorsState newState, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteEntranceDoorState");
		if (!base.IsMine)
		{
			return;
		}
		this.doorState = newState;
	}

	// Token: 0x06000A3D RID: 2621 RVA: 0x000373AF File Offset: 0x000355AF
	[PunRPC]
	public void RemoteSingleDoorState(int doorIndex, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "RemoteSingleDoorState");
		if (!base.IsMine)
		{
			return;
		}
		if (doorIndex >= this.singleDoorCount)
		{
			return;
		}
		this.singleDoorOpen[doorIndex] = !this.singleDoorOpen[doorIndex];
	}

	// Token: 0x06000A3F RID: 2623 RVA: 0x000373E2 File Offset: 0x000355E2
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.NetData = this._NetData;
	}

	// Token: 0x06000A40 RID: 2624 RVA: 0x000373FA File Offset: 0x000355FA
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._NetData = this.NetData;
	}

	// Token: 0x06000A41 RID: 2625 RVA: 0x00037410 File Offset: 0x00035610
	[NetworkRpcWeavedInvoker(1, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemoteEntranceDoorState@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		GhostLab.EntranceDoorsState entranceDoorsState = *(GhostLab.EntranceDoorsState*)(ptr + num);
		num += 4;
		GhostLab.EntranceDoorsState newState = entranceDoorsState;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
		behaviour.InvokeRpc = true;
		((GhostLabReliableState)behaviour).RPC_RemoteEntranceDoorState(newState, info);
	}

	// Token: 0x06000A42 RID: 2626 RVA: 0x00037474 File Offset: 0x00035674
	[NetworkRpcWeavedInvoker(2, 7, 1)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_RemoteSingleDoorState@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int doorIndex = num2;
		RpcInfo info = RpcInfo.FromMessage(behaviour.Runner, message, 0);
		behaviour.InvokeRpc = true;
		((GhostLabReliableState)behaviour).RPC_RemoteSingleDoorState(doorIndex, info);
	}

	// Token: 0x04000C7D RID: 3197
	public GhostLab.EntranceDoorsState doorState;

	// Token: 0x04000C7E RID: 3198
	public int singleDoorCount;

	// Token: 0x04000C7F RID: 3199
	public bool[] singleDoorOpen;

	// Token: 0x04000C80 RID: 3200
	[WeaverGenerated]
	[DefaultForProperty("NetData", 0, 11)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private GhostLabData _NetData;
}
