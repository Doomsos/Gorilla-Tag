using System;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000CD0 RID: 3280
[NetworkBehaviourWeaved(3)]
public class ThrowableBugReliableState : NetworkComponent, IRequestableOwnershipGuardCallbacks
{
	// Token: 0x1700076E RID: 1902
	// (get) Token: 0x06004FFA RID: 20474 RVA: 0x0019B765 File Offset: 0x00199965
	// (set) Token: 0x06004FFB RID: 20475 RVA: 0x0019B78F File Offset: 0x0019998F
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe ThrowableBugReliableState.BugData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(ThrowableBugReliableState.BugData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing ThrowableBugReliableState.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(ThrowableBugReliableState.BugData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06004FFC RID: 20476 RVA: 0x0019B7BA File Offset: 0x001999BA
	public override void WriteDataFusion()
	{
		this.Data = new ThrowableBugReliableState.BugData(this.travelingDirection);
	}

	// Token: 0x06004FFD RID: 20477 RVA: 0x0019B7D0 File Offset: 0x001999D0
	public override void ReadDataFusion()
	{
		this.travelingDirection = this.Data.tDirection;
	}

	// Token: 0x06004FFE RID: 20478 RVA: 0x0019B7F1 File Offset: 0x001999F1
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this.travelingDirection);
	}

	// Token: 0x06004FFF RID: 20479 RVA: 0x0019B804 File Offset: 0x00199A04
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.travelingDirection.SetValueSafe(vector);
	}

	// Token: 0x06005000 RID: 20480 RVA: 0x000029BC File Offset: 0x00000BBC
	public void OnOwnershipTransferred(NetPlayer toPlayer, NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005001 RID: 20481 RVA: 0x000029BC File Offset: 0x00000BBC
	public bool OnOwnershipRequest(NetPlayer fromPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005002 RID: 20482 RVA: 0x000029BC File Offset: 0x00000BBC
	public void OnMyOwnerLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005003 RID: 20483 RVA: 0x000029BC File Offset: 0x00000BBC
	public bool OnMasterClientAssistedTakeoverRequest(NetPlayer fromPlayer, NetPlayer toPlayer)
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005004 RID: 20484 RVA: 0x000029BC File Offset: 0x00000BBC
	public void OnMyCreatorLeft()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06005006 RID: 20486 RVA: 0x0019B83D File Offset: 0x00199A3D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06005007 RID: 20487 RVA: 0x0019B855 File Offset: 0x00199A55
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04005E9A RID: 24218
	public Vector3 travelingDirection = Vector3.zero;

	// Token: 0x04005E9B RID: 24219
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private ThrowableBugReliableState.BugData _Data;

	// Token: 0x02000CD1 RID: 3281
	[NetworkStructWeaved(3)]
	[StructLayout(2, Size = 12)]
	public struct BugData : INetworkStruct
	{
		// Token: 0x1700076F RID: 1903
		// (get) Token: 0x06005008 RID: 20488 RVA: 0x0019B869 File Offset: 0x00199A69
		// (set) Token: 0x06005009 RID: 20489 RVA: 0x0019B87B File Offset: 0x00199A7B
		[Networked]
		[NetworkedWeaved(0, 3)]
		public unsafe Vector3 tDirection
		{
			readonly get
			{
				return *(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection);
			}
			set
			{
				*(Vector3*)Native.ReferenceToPointer<FixedStorage@3>(ref this._tDirection) = value;
			}
		}

		// Token: 0x0600500A RID: 20490 RVA: 0x0019B88E File Offset: 0x00199A8E
		public BugData(Vector3 dir)
		{
			this.tDirection = dir;
		}

		// Token: 0x04005E9C RID: 24220
		[FixedBufferProperty(typeof(Vector3), typeof(UnityValueSurrogate@ElementReaderWriterVector3), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(0)]
		private FixedStorage@3 _tDirection;
	}
}
