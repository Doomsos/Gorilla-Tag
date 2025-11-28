using System;

namespace GorillaTagScripts
{
	// Token: 0x02000DC9 RID: 3529
	public struct BuilderPieceData
	{
		// Token: 0x0600578D RID: 22413 RVA: 0x001BE41C File Offset: 0x001BC61C
		public BuilderPieceData(BuilderPiece piece)
		{
			this.pieceId = piece.pieceId;
			this.pieceIndex = piece.pieceDataIndex;
			BuilderPiece parentPiece = piece.parentPiece;
			this.parentPieceIndex = ((parentPiece == null) ? -1 : parentPiece.pieceDataIndex);
			BuilderPiece requestedParentPiece = piece.requestedParentPiece;
			this.requestedParentPieceIndex = ((requestedParentPiece == null) ? -1 : requestedParentPiece.pieceDataIndex);
			this.preventSnapUntilMoved = piece.preventSnapUntilMoved;
			this.isBuiltIntoTable = piece.isBuiltIntoTable;
			this.state = piece.state;
			this.privatePlotIndex = piece.privatePlotIndex;
			this.isArmPiece = piece.isArmShelf;
			this.heldByActorNumber = piece.heldByPlayerActorNumber;
		}

		// Token: 0x040064C9 RID: 25801
		public int pieceId;

		// Token: 0x040064CA RID: 25802
		public int pieceIndex;

		// Token: 0x040064CB RID: 25803
		public int parentPieceIndex;

		// Token: 0x040064CC RID: 25804
		public int requestedParentPieceIndex;

		// Token: 0x040064CD RID: 25805
		public int heldByActorNumber;

		// Token: 0x040064CE RID: 25806
		public int preventSnapUntilMoved;

		// Token: 0x040064CF RID: 25807
		public bool isBuiltIntoTable;

		// Token: 0x040064D0 RID: 25808
		public BuilderPiece.State state;

		// Token: 0x040064D1 RID: 25809
		public int privatePlotIndex;

		// Token: 0x040064D2 RID: 25810
		public bool isArmPiece;
	}
}
