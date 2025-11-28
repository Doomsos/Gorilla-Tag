using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000567 RID: 1383
public class BuilderWaterVolume : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x060022D2 RID: 8914 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPieceDestroy()
	{
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x000B5F48 File Offset: 0x000B4148
	public void OnPiecePlacementDeserialized()
	{
		bool flag = (double)Vector3.Dot(Vector3.up, base.transform.up) > 0.5 && !this.piece.IsPieceMoving();
		this.waterVolume.SetActive(flag);
		this.waterMesh.SetActive(flag);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = (flag ? this.floating.localPosition : this.sunk.localPosition);
		}
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x000B5FD4 File Offset: 0x000B41D4
	public void OnPieceActivate()
	{
		bool flag = (double)Vector3.Dot(Vector3.up, base.transform.up) > 0.5 && !this.piece.IsPieceMoving();
		this.waterVolume.SetActive(flag);
		this.waterMesh.SetActive(flag);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = (flag ? this.floating.localPosition : this.sunk.localPosition);
		}
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x000B6060 File Offset: 0x000B4260
	public void OnPieceDeactivate()
	{
		this.waterVolume.SetActive(false);
		this.waterMesh.SetActive(true);
		if (this.floatingObjects != null)
		{
			this.floatingObjects.localPosition = this.floating.localPosition;
		}
	}

	// Token: 0x04002D87 RID: 11655
	[SerializeField]
	private BuilderPiece piece;

	// Token: 0x04002D88 RID: 11656
	[SerializeField]
	private GameObject waterVolume;

	// Token: 0x04002D89 RID: 11657
	[SerializeField]
	private GameObject waterMesh;

	// Token: 0x04002D8A RID: 11658
	[FormerlySerializedAs("lillyPads")]
	[SerializeField]
	private Transform floatingObjects;

	// Token: 0x04002D8B RID: 11659
	[SerializeField]
	private Transform floating;

	// Token: 0x04002D8C RID: 11660
	[SerializeField]
	private Transform sunk;
}
