using System;
using Fusion;

// Token: 0x02000516 RID: 1302
[NetworkBehaviourWeaved(12)]
public class TagGameModeData : FusionGameModeData
{
	// Token: 0x17000388 RID: 904
	// (get) Token: 0x0600212E RID: 8494 RVA: 0x000AEEA4 File Offset: 0x000AD0A4
	// (set) Token: 0x0600212F RID: 8495 RVA: 0x000AEEB1 File Offset: 0x000AD0B1
	public override object Data
	{
		get
		{
			return this.tagData;
		}
		set
		{
			this.tagData = (TagData)value;
		}
	}

	// Token: 0x17000389 RID: 905
	// (get) Token: 0x06002130 RID: 8496 RVA: 0x000AEEBF File Offset: 0x000AD0BF
	// (set) Token: 0x06002131 RID: 8497 RVA: 0x000AEEE9 File Offset: 0x000AD0E9
	[Networked]
	[NetworkedWeaved(0, 12)]
	private unsafe TagData tagData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(TagData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing TagGameModeData.tagData. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(TagData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002133 RID: 8499 RVA: 0x000AEF14 File Offset: 0x000AD114
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.tagData = this._tagData;
	}

	// Token: 0x06002134 RID: 8500 RVA: 0x000AEF2C File Offset: 0x000AD12C
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._tagData = this.tagData;
	}

	// Token: 0x04002BC2 RID: 11202
	[WeaverGenerated]
	[DefaultForProperty("tagData", 0, 12)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private TagData _tagData;
}
