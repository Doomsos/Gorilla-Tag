using System;
using Fusion;

// Token: 0x02000514 RID: 1300
[NetworkBehaviourWeaved(23)]
public class HuntGameModeData : FusionGameModeData
{
	// Token: 0x17000384 RID: 900
	// (get) Token: 0x06002124 RID: 8484 RVA: 0x000AEDAF File Offset: 0x000ACFAF
	// (set) Token: 0x06002125 RID: 8485 RVA: 0x000AEDBC File Offset: 0x000ACFBC
	public override object Data
	{
		get
		{
			return this.huntdata;
		}
		set
		{
			this.huntdata = (HuntData)value;
		}
	}

	// Token: 0x17000385 RID: 901
	// (get) Token: 0x06002126 RID: 8486 RVA: 0x000AEDCA File Offset: 0x000ACFCA
	// (set) Token: 0x06002127 RID: 8487 RVA: 0x000AEDF4 File Offset: 0x000ACFF4
	[Networked]
	[NetworkedWeaved(0, 23)]
	private unsafe HuntData huntdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HuntData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HuntGameModeData.huntdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HuntData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002129 RID: 8489 RVA: 0x000AEE1F File Offset: 0x000AD01F
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.huntdata = this._huntdata;
	}

	// Token: 0x0600212A RID: 8490 RVA: 0x000AEE37 File Offset: 0x000AD037
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._huntdata = this.huntdata;
	}

	// Token: 0x04002BBE RID: 11198
	[WeaverGenerated]
	[DefaultForProperty("huntdata", 0, 23)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private HuntData _huntdata;
}
