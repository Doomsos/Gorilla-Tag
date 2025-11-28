using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DC8 RID: 3528
	public struct BuilderGridPlaneData
	{
		// Token: 0x0600578C RID: 22412 RVA: 0x001BE364 File Offset: 0x001BC564
		public BuilderGridPlaneData(BuilderAttachGridPlane gridPlane, int pieceIndex)
		{
			gridPlane.center.transform.GetPositionAndRotation(ref this.position, ref this.rotation);
			this.localPosition = gridPlane.pieceToGridPosition;
			this.localRotation = gridPlane.pieceToGridRotation;
			this.width = gridPlane.width;
			this.length = gridPlane.length;
			this.male = gridPlane.male;
			this.pieceId = gridPlane.piece.pieceId;
			this.pieceIndex = pieceIndex;
			this.boundingRadius = gridPlane.boundingRadius;
			this.attachIndex = gridPlane.attachIndex;
		}

		// Token: 0x040064BE RID: 25790
		public int width;

		// Token: 0x040064BF RID: 25791
		public int length;

		// Token: 0x040064C0 RID: 25792
		public bool male;

		// Token: 0x040064C1 RID: 25793
		public int pieceId;

		// Token: 0x040064C2 RID: 25794
		public int pieceIndex;

		// Token: 0x040064C3 RID: 25795
		public float boundingRadius;

		// Token: 0x040064C4 RID: 25796
		public int attachIndex;

		// Token: 0x040064C5 RID: 25797
		public Vector3 position;

		// Token: 0x040064C6 RID: 25798
		public Quaternion rotation;

		// Token: 0x040064C7 RID: 25799
		public Vector3 localPosition;

		// Token: 0x040064C8 RID: 25800
		public Quaternion localRotation;
	}
}
