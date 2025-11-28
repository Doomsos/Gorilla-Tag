using System;
using GorillaTagScripts;
using UnityEngine;

// Token: 0x0200056C RID: 1388
public class BuilderArmShelf : MonoBehaviour
{
	// Token: 0x060022E7 RID: 8935 RVA: 0x000B6640 File Offset: 0x000B4840
	private void Start()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x000B664E File Offset: 0x000B484E
	public bool IsOwnedLocally()
	{
		return this.ownerRig != null && this.ownerRig.isLocal;
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000B666B File Offset: 0x000B486B
	public bool CanAttachToArmPiece()
	{
		return this.ownerRig != null && this.ownerRig.scaleFactor >= 1f;
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000B6694 File Offset: 0x000B4894
	public void DropAttachedPieces()
	{
		if (this.ownerRig != null && this.piece != null)
		{
			Vector3 velocity = Vector3.zero;
			if (this.piece.firstChildPiece == null)
			{
				return;
			}
			BuilderTable table = this.piece.GetTable();
			Vector3 vector = table.roomCenter.position - this.piece.transform.position;
			vector.Normalize();
			Vector3 vector2 = Quaternion.Euler(0f, 180f, 0f) * vector;
			velocity = BuilderTable.DROP_ZONE_REPEL * vector2;
			BuilderPiece builderPiece = this.piece.firstChildPiece;
			while (builderPiece != null)
			{
				table.RequestDropPiece(builderPiece, builderPiece.transform.position + vector2 * 0.1f, builderPiece.transform.rotation, velocity, Vector3.zero);
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
	}

	// Token: 0x04002DAC RID: 11692
	[HideInInspector]
	public BuilderPiece piece;

	// Token: 0x04002DAD RID: 11693
	public Transform pieceAnchor;

	// Token: 0x04002DAE RID: 11694
	private VRRig ownerRig;
}
