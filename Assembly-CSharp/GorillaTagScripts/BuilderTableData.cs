using System;
using System.Collections.Generic;

namespace GorillaTagScripts
{
	// Token: 0x02000DD4 RID: 3540
	[Serializable]
	public class BuilderTableData
	{
		// Token: 0x060057EC RID: 22508 RVA: 0x001C1AB0 File Offset: 0x001BFCB0
		public BuilderTableData()
		{
			this.version = 4;
			this.numEdits = 0;
			this.numPieces = 0;
			this.pieceType = new List<int>(1024);
			this.pieceId = new List<int>(1024);
			this.parentId = new List<int>(1024);
			this.attachIndex = new List<int>(1024);
			this.parentAttachIndex = new List<int>(1024);
			this.placement = new List<int>(1024);
			this.materialType = new List<int>(1024);
			this.overlapingPieces = new List<int>(1024);
			this.overlappedPieces = new List<int>(1024);
			this.overlapInfo = new List<long>(1024);
			this.timeOffset = new List<int>(1024);
		}

		// Token: 0x060057ED RID: 22509 RVA: 0x001C1B88 File Offset: 0x001BFD88
		public void Clear()
		{
			this.numPieces = 0;
			this.pieceType.Clear();
			this.pieceId.Clear();
			this.parentId.Clear();
			this.attachIndex.Clear();
			this.parentAttachIndex.Clear();
			this.placement.Clear();
			this.materialType.Clear();
			this.overlapingPieces.Clear();
			this.overlappedPieces.Clear();
			this.overlapInfo.Clear();
			this.timeOffset.Clear();
		}

		// Token: 0x04006534 RID: 25908
		public const int BUILDER_TABLE_DATA_VERSION = 4;

		// Token: 0x04006535 RID: 25909
		public int version;

		// Token: 0x04006536 RID: 25910
		public int numEdits;

		// Token: 0x04006537 RID: 25911
		public int numPieces;

		// Token: 0x04006538 RID: 25912
		public List<int> pieceType;

		// Token: 0x04006539 RID: 25913
		public List<int> pieceId;

		// Token: 0x0400653A RID: 25914
		public List<int> parentId;

		// Token: 0x0400653B RID: 25915
		public List<int> attachIndex;

		// Token: 0x0400653C RID: 25916
		public List<int> parentAttachIndex;

		// Token: 0x0400653D RID: 25917
		public List<int> placement;

		// Token: 0x0400653E RID: 25918
		public List<int> materialType;

		// Token: 0x0400653F RID: 25919
		public List<int> overlapingPieces;

		// Token: 0x04006540 RID: 25920
		public List<int> overlappedPieces;

		// Token: 0x04006541 RID: 25921
		public List<long> overlapInfo;

		// Token: 0x04006542 RID: 25922
		public List<int> timeOffset;
	}
}
