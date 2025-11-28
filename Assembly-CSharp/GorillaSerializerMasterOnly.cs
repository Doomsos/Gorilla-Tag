using System;
using Fusion;
using Photon.Pun;

// Token: 0x02000758 RID: 1880
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaSerializerMasterOnly : GorillaWrappedSerializer
{
	// Token: 0x06003097 RID: 12439 RVA: 0x00109DF3 File Offset: 0x00107FF3
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == PhotonNetwork.MasterClient;
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x00109E0D File Offset: 0x0010800D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
	}

	// Token: 0x0600309A RID: 12442 RVA: 0x00109E19 File Offset: 0x00108019
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
	}
}
