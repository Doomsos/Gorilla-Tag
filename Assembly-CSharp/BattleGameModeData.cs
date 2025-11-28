using System;
using Fusion;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200050D RID: 1293
[NetworkBehaviourWeaved(31)]
public class BattleGameModeData : FusionGameModeData
{
	// Token: 0x1700037D RID: 893
	// (get) Token: 0x060020FE RID: 8446 RVA: 0x000AE94B File Offset: 0x000ACB4B
	// (set) Token: 0x060020FF RID: 8447 RVA: 0x000AE975 File Offset: 0x000ACB75
	[Networked]
	[NetworkedWeaved(0, 31)]
	private unsafe PaintbrawlData PaintbrawlData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BattleGameModeData.PaintbrawlData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(PaintbrawlData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing BattleGameModeData.PaintbrawlData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(PaintbrawlData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x1700037E RID: 894
	// (get) Token: 0x06002100 RID: 8448 RVA: 0x000AE9A0 File Offset: 0x000ACBA0
	// (set) Token: 0x06002101 RID: 8449 RVA: 0x000AE9AD File Offset: 0x000ACBAD
	public override object Data
	{
		get
		{
			return this.PaintbrawlData;
		}
		set
		{
			this.PaintbrawlData = (PaintbrawlData)value;
		}
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x000AE9BB File Offset: 0x000ACBBB
	public override void Spawned()
	{
		this.serializer = base.GetComponent<GameModeSerializer>();
		this.battleTarget = (GorillaPaintbrawlManager)this.serializer.GameModeInstance;
	}

	// Token: 0x06002103 RID: 8451 RVA: 0x000AE9E0 File Offset: 0x000ACBE0
	[Rpc]
	public unsafe void RPC_ReportSlinshotHit(int taggedPlayerID, Vector3 hitLocation, int projectileCount, RpcInfo rpcInfo = default(RpcInfo))
	{
		if (!this.InvokeRpc)
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != 4)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void BattleGameModeData::RPC_ReportSlinshotHit(System.Int32,UnityEngine.Vector3,System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					int num = 8;
					num += 4;
					num += 12;
					num += 4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void BattleGameModeData::RPC_ReportSlinshotHit(System.Int32,UnityEngine.Vector3,System.Int32,Fusion.RpcInfo)", num);
					}
					else
					{
						if (base.Runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
							int num2 = 8;
							*(int*)(ptr2 + num2) = taggedPlayerID;
							num2 += 4;
							*(Vector3*)(ptr2 + num2) = hitLocation;
							num2 += 12;
							*(int*)(ptr2 + num2) = projectileCount;
							num2 += 4;
							ptr.Offset = num2 * 8;
							base.Runner.SendRpc(ptr);
						}
						if ((localAuthorityMask & 7) != 0)
						{
							rpcInfo = RpcInfo.FromLocal(base.Runner, 0, 0);
							goto IL_12;
						}
					}
				}
			}
			return;
		}
		this.InvokeRpc = false;
		IL_12:
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(rpcInfo);
		GorillaNot.IncrementRPCCall(photonMessageInfoWrapped, "RPC_ReportSlinshotHit");
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(taggedPlayerID);
		this.battleTarget.ReportSlingshotHit(player, hitLocation, projectileCount, photonMessageInfoWrapped);
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x000AEBC8 File Offset: 0x000ACDC8
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.PaintbrawlData = this._PaintbrawlData;
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x000AEBE0 File Offset: 0x000ACDE0
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._PaintbrawlData = this.PaintbrawlData;
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x000AEBF4 File Offset: 0x000ACDF4
	[NetworkRpcWeavedInvoker(1, 7, 7)]
	[Preserve]
	[WeaverGenerated]
	protected unsafe static void RPC_ReportSlinshotHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
		int num = 8;
		int num2 = *(int*)(ptr + num);
		num += 4;
		int taggedPlayerID = num2;
		Vector3 vector = *(Vector3*)(ptr + num);
		num += 12;
		Vector3 hitLocation = vector;
		int num3 = *(int*)(ptr + num);
		num += 4;
		int projectileCount = num3;
		RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, 0);
		behaviour.InvokeRpc = true;
		((BattleGameModeData)behaviour).RPC_ReportSlinshotHit(taggedPlayerID, hitLocation, projectileCount, rpcInfo);
	}

	// Token: 0x04002BB3 RID: 11187
	[WeaverGenerated]
	[DefaultForProperty("PaintbrawlData", 0, 31)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private PaintbrawlData _PaintbrawlData;

	// Token: 0x04002BB4 RID: 11188
	private GorillaPaintbrawlManager battleTarget;

	// Token: 0x04002BB5 RID: 11189
	private GameModeSerializer serializer;
}
