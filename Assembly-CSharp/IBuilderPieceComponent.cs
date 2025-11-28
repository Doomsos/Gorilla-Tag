using System;

// Token: 0x0200057C RID: 1404
public interface IBuilderPieceComponent
{
	// Token: 0x06002365 RID: 9061
	void OnPieceCreate(int pieceType, int pieceId);

	// Token: 0x06002366 RID: 9062
	void OnPieceDestroy();

	// Token: 0x06002367 RID: 9063
	void OnPiecePlacementDeserialized();

	// Token: 0x06002368 RID: 9064
	void OnPieceActivate();

	// Token: 0x06002369 RID: 9065
	void OnPieceDeactivate();
}
