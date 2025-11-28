using System;
using GorillaTagScripts;
using UnityEngine;

public class BuilderArmShelf : MonoBehaviour
{
	private void Start()
	{
		this.ownerRig = base.GetComponentInParent<VRRig>();
	}

	public bool IsOwnedLocally()
	{
		return this.ownerRig != null && this.ownerRig.isLocal;
	}

	public bool CanAttachToArmPiece()
	{
		return this.ownerRig != null && this.ownerRig.scaleFactor >= 1f;
	}

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

	[HideInInspector]
	public BuilderPiece piece;

	public Transform pieceAnchor;

	private VRRig ownerRig;
}
