using System;
using UnityEngine;

// Token: 0x0200056A RID: 1386
public struct BuilderAction
{
	// Token: 0x04002D9A RID: 11674
	public BuilderActionType type;

	// Token: 0x04002D9B RID: 11675
	public int pieceId;

	// Token: 0x04002D9C RID: 11676
	public int parentPieceId;

	// Token: 0x04002D9D RID: 11677
	public Vector3 localPosition;

	// Token: 0x04002D9E RID: 11678
	public Quaternion localRotation;

	// Token: 0x04002D9F RID: 11679
	public byte twist;

	// Token: 0x04002DA0 RID: 11680
	public sbyte bumpOffsetx;

	// Token: 0x04002DA1 RID: 11681
	public sbyte bumpOffsetz;

	// Token: 0x04002DA2 RID: 11682
	public bool isLeftHand;

	// Token: 0x04002DA3 RID: 11683
	public int playerActorNumber;

	// Token: 0x04002DA4 RID: 11684
	public int parentAttachIndex;

	// Token: 0x04002DA5 RID: 11685
	public int attachIndex;

	// Token: 0x04002DA6 RID: 11686
	public SnapBounds attachBounds;

	// Token: 0x04002DA7 RID: 11687
	public SnapBounds parentAttachBounds;

	// Token: 0x04002DA8 RID: 11688
	public Vector3 velocity;

	// Token: 0x04002DA9 RID: 11689
	public Vector3 angVelocity;

	// Token: 0x04002DAA RID: 11690
	public int localCommandId;

	// Token: 0x04002DAB RID: 11691
	public int timeStamp;
}
