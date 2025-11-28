using System;
using Fusion;

// Token: 0x02000512 RID: 1298
[NetworkBehaviourWeaved(0)]
public abstract class FusionGameModeData : NetworkBehaviour
{
	// Token: 0x17000381 RID: 897
	// (get) Token: 0x0600211D RID: 8477
	// (set) Token: 0x0600211E RID: 8478
	public abstract object Data { get; set; }

	// Token: 0x06002120 RID: 8480 RVA: 0x00002789 File Offset: 0x00000989
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x00002789 File Offset: 0x00000989
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x04002BB8 RID: 11192
	protected INetworkStruct data;
}
