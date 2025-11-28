using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DCC RID: 3532
	public struct BuilderPotentialPlacementData
	{
		// Token: 0x0600578F RID: 22415 RVA: 0x001BE4F0 File Offset: 0x001BC6F0
		public BuilderPotentialPlacement ToPotentialPlacement(BuilderTable table)
		{
			BuilderPotentialPlacement builderPotentialPlacement = new BuilderPotentialPlacement
			{
				attachPiece = table.GetPiece(this.pieceId),
				parentPiece = table.GetPiece(this.parentPieceId),
				score = this.score,
				localPosition = this.localPosition,
				localRotation = this.localRotation,
				attachIndex = this.attachIndex,
				parentAttachIndex = this.parentAttachIndex,
				attachDistance = this.attachDistance,
				attachPlaneNormal = this.attachPlaneNormal,
				attachBounds = this.attachBounds,
				parentAttachBounds = this.parentAttachBounds,
				twist = this.twist,
				bumpOffsetX = this.bumpOffsetX,
				bumpOffsetZ = this.bumpOffsetZ
			};
			if (builderPotentialPlacement.parentPiece != null)
			{
				BuilderAttachGridPlane builderAttachGridPlane = builderPotentialPlacement.parentPiece.gridPlanes[builderPotentialPlacement.parentAttachIndex];
				builderPotentialPlacement.localPosition = builderAttachGridPlane.transform.InverseTransformPoint(builderPotentialPlacement.localPosition);
				builderPotentialPlacement.localRotation = Quaternion.Inverse(builderAttachGridPlane.transform.rotation) * builderPotentialPlacement.localRotation;
			}
			return builderPotentialPlacement;
		}

		// Token: 0x040064D9 RID: 25817
		public int pieceId;

		// Token: 0x040064DA RID: 25818
		public int parentPieceId;

		// Token: 0x040064DB RID: 25819
		public float score;

		// Token: 0x040064DC RID: 25820
		public Vector3 localPosition;

		// Token: 0x040064DD RID: 25821
		public Quaternion localRotation;

		// Token: 0x040064DE RID: 25822
		public int attachIndex;

		// Token: 0x040064DF RID: 25823
		public int parentAttachIndex;

		// Token: 0x040064E0 RID: 25824
		public float attachDistance;

		// Token: 0x040064E1 RID: 25825
		public Vector3 attachPlaneNormal;

		// Token: 0x040064E2 RID: 25826
		public SnapBounds attachBounds;

		// Token: 0x040064E3 RID: 25827
		public SnapBounds parentAttachBounds;

		// Token: 0x040064E4 RID: 25828
		public byte twist;

		// Token: 0x040064E5 RID: 25829
		public sbyte bumpOffsetX;

		// Token: 0x040064E6 RID: 25830
		public sbyte bumpOffsetZ;
	}
}
