using System;
using Fusion;

// Token: 0x02000511 RID: 1297
[NetworkBehaviourWeaved(1)]
public class CasualGameModeData : FusionGameModeData
{
	// Token: 0x1700037F RID: 895
	// (get) Token: 0x06002116 RID: 8470 RVA: 0x000AECEF File Offset: 0x000ACEEF
	// (set) Token: 0x06002117 RID: 8471 RVA: 0x00002789 File Offset: 0x00000989
	public override object Data
	{
		get
		{
			return this.casualData;
		}
		set
		{
		}
	}

	// Token: 0x17000380 RID: 896
	// (get) Token: 0x06002118 RID: 8472 RVA: 0x000AECFC File Offset: 0x000ACEFC
	// (set) Token: 0x06002119 RID: 8473 RVA: 0x000AED26 File Offset: 0x000ACF26
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe CasualData casualData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(CasualData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing CasualGameModeData.casualData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(CasualData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x000AED51 File Offset: 0x000ACF51
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.casualData = this._casualData;
	}

	// Token: 0x0600211C RID: 8476 RVA: 0x000AED69 File Offset: 0x000ACF69
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._casualData = this.casualData;
	}

	// Token: 0x04002BB7 RID: 11191
	[WeaverGenerated]
	[DefaultForProperty("casualData", 0, 1)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private CasualData _casualData;
}
