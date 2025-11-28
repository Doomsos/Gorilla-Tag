using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000BA5 RID: 2981
[NetworkBehaviourWeaved(1)]
public class TransformViewTeleportSerializer : NetworkComponent
{
	// Token: 0x06004989 RID: 18825 RVA: 0x00181919 File Offset: 0x0017FB19
	protected override void Start()
	{
		base.Start();
		this.transformView = base.GetComponent<GorillaNetworkTransform>();
	}

	// Token: 0x0600498A RID: 18826 RVA: 0x0018192D File Offset: 0x0017FB2D
	public void SetWillTeleport()
	{
		this.willTeleport = true;
	}

	// Token: 0x170006D9 RID: 1753
	// (get) Token: 0x0600498B RID: 18827 RVA: 0x00181936 File Offset: 0x0017FB36
	// (set) Token: 0x0600498C RID: 18828 RVA: 0x00181960 File Offset: 0x0017FB60
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe NetworkBool Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TransformViewTeleportSerializer.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600498D RID: 18829 RVA: 0x0018198B File Offset: 0x0017FB8B
	public override void WriteDataFusion()
	{
		this.Data = this.willTeleport;
		this.willTeleport = false;
	}

	// Token: 0x0600498E RID: 18830 RVA: 0x001819A5 File Offset: 0x0017FBA5
	public override void ReadDataFusion()
	{
		if (this.Data)
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x0600498F RID: 18831 RVA: 0x001819BF File Offset: 0x0017FBBF
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		stream.SendNext(this.willTeleport);
		this.willTeleport = false;
	}

	// Token: 0x06004990 RID: 18832 RVA: 0x001819FA File Offset: 0x0017FBFA
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.transformView.RespectOwnership && info.Sender != info.photonView.Owner)
		{
			return;
		}
		if ((bool)stream.ReceiveNext())
		{
			this.transformView.GTAddition_DoTeleport();
		}
	}

	// Token: 0x06004992 RID: 18834 RVA: 0x00181A35 File Offset: 0x0017FC35
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06004993 RID: 18835 RVA: 0x00181A4D File Offset: 0x0017FC4D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04005A10 RID: 23056
	private bool willTeleport;

	// Token: 0x04005A11 RID: 23057
	private GorillaNetworkTransform transformView;

	// Token: 0x04005A12 RID: 23058
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private NetworkBool _Data;
}
