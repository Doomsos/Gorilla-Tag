using System;
using Fusion;
using Photon.Pun;

// Token: 0x020003C8 RID: 968
[NetworkBehaviourWeaved(0)]
public class NetworkComponentCallbacks : NetworkComponent
{
	// Token: 0x06001763 RID: 5987 RVA: 0x00080BB8 File Offset: 0x0007EDB8
	public override void ReadDataFusion()
	{
		this.ReadData.Invoke();
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x00080BC5 File Offset: 0x0007EDC5
	public override void WriteDataFusion()
	{
		this.WriteData.Invoke();
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x00080BD2 File Offset: 0x0007EDD2
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.ReadPunData.Invoke(stream, info);
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x00080BE1 File Offset: 0x0007EDE1
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		this.WritePunData.Invoke(stream, info);
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x000029CB File Offset: 0x00000BCB
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x000029D7 File Offset: 0x00000BD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}

	// Token: 0x04002140 RID: 8512
	public Action ReadData;

	// Token: 0x04002141 RID: 8513
	public Action WriteData;

	// Token: 0x04002142 RID: 8514
	public Action<PhotonStream, PhotonMessageInfo> ReadPunData;

	// Token: 0x04002143 RID: 8515
	public Action<PhotonStream, PhotonMessageInfo> WritePunData;
}
