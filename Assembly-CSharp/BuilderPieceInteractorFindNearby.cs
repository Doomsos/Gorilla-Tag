using System;

// Token: 0x0200055E RID: 1374
public class BuilderPieceInteractorFindNearby : MonoBehaviourPostTick
{
	// Token: 0x060022B7 RID: 8887 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000B5AEA File Offset: 0x000B3CEA
	public override void PostTick()
	{
		if (this.pieceInteractor != null)
		{
			this.pieceInteractor.StartFindNearbyPieces();
		}
	}

	// Token: 0x04002D65 RID: 11621
	public BuilderPieceInteractor pieceInteractor;
}
