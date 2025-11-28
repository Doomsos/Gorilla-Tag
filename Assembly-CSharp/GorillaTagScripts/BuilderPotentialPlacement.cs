using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DBC RID: 3516
	public struct BuilderPotentialPlacement
	{
		// Token: 0x0600568E RID: 22158 RVA: 0x001B3C50 File Offset: 0x001B1E50
		public void Reset()
		{
			this.attachPiece = null;
			this.parentPiece = null;
			this.attachIndex = -1;
			this.parentAttachIndex = -1;
			this.localPosition = Vector3.zero;
			this.localRotation = Quaternion.identity;
			this.attachDistance = float.MaxValue;
			this.attachPlaneNormal = Vector3.zero;
			this.score = float.MinValue;
			this.twist = 0;
			this.bumpOffsetX = 0;
			this.bumpOffsetZ = 0;
		}

		// Token: 0x040063BF RID: 25535
		public BuilderPiece attachPiece;

		// Token: 0x040063C0 RID: 25536
		public BuilderPiece parentPiece;

		// Token: 0x040063C1 RID: 25537
		public int attachIndex;

		// Token: 0x040063C2 RID: 25538
		public int parentAttachIndex;

		// Token: 0x040063C3 RID: 25539
		public Vector3 localPosition;

		// Token: 0x040063C4 RID: 25540
		public Quaternion localRotation;

		// Token: 0x040063C5 RID: 25541
		public Vector3 attachPlaneNormal;

		// Token: 0x040063C6 RID: 25542
		public float attachDistance;

		// Token: 0x040063C7 RID: 25543
		public float score;

		// Token: 0x040063C8 RID: 25544
		public SnapBounds attachBounds;

		// Token: 0x040063C9 RID: 25545
		public SnapBounds parentAttachBounds;

		// Token: 0x040063CA RID: 25546
		public byte twist;

		// Token: 0x040063CB RID: 25547
		public sbyte bumpOffsetX;

		// Token: 0x040063CC RID: 25548
		public sbyte bumpOffsetZ;
	}
}
