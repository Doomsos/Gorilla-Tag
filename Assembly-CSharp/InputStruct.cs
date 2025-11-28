using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x020003C2 RID: 962
[NetworkStructWeaved(37)]
[Serializable]
[StructLayout(2, Size = 148)]
public struct InputStruct : INetworkStruct
{
	// Token: 0x04002115 RID: 8469
	[FieldOffset(0)]
	public int headRotation;

	// Token: 0x04002116 RID: 8470
	[FieldOffset(4)]
	public long rightHandLong;

	// Token: 0x04002117 RID: 8471
	[FieldOffset(12)]
	public long leftHandLong;

	// Token: 0x04002118 RID: 8472
	[FieldOffset(20)]
	public long position;

	// Token: 0x04002119 RID: 8473
	[FieldOffset(28)]
	public int handPosition;

	// Token: 0x0400211A RID: 8474
	[FieldOffset(32)]
	public int packedFields;

	// Token: 0x0400211B RID: 8475
	[FieldOffset(36)]
	public short packedCompetitiveData;

	// Token: 0x0400211C RID: 8476
	[FieldOffset(40)]
	public Vector3 velocity;

	// Token: 0x0400211D RID: 8477
	[FieldOffset(52)]
	public int grabbedRopeIndex;

	// Token: 0x0400211E RID: 8478
	[FieldOffset(56)]
	public int ropeBoneIndex;

	// Token: 0x0400211F RID: 8479
	[FieldOffset(60)]
	public bool ropeGrabIsLeft;

	// Token: 0x04002120 RID: 8480
	[FieldOffset(64)]
	public bool ropeGrabIsBody;

	// Token: 0x04002121 RID: 8481
	[FieldOffset(68)]
	public Vector3 ropeGrabOffset;

	// Token: 0x04002122 RID: 8482
	[FieldOffset(80)]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x04002123 RID: 8483
	[FieldOffset(84)]
	public long hoverboardPosRot;

	// Token: 0x04002124 RID: 8484
	[FieldOffset(92)]
	public short hoverboardColor;

	// Token: 0x04002125 RID: 8485
	[FieldOffset(96)]
	public long propHuntPosRot;

	// Token: 0x04002126 RID: 8486
	[FieldOffset(104)]
	public double serverTimeStamp;

	// Token: 0x04002127 RID: 8487
	[FieldOffset(112)]
	public short taggedById;

	// Token: 0x04002128 RID: 8488
	[FieldOffset(116)]
	public bool isGroundedHand;

	// Token: 0x04002129 RID: 8489
	[FieldOffset(120)]
	public bool isGroundedButt;

	// Token: 0x0400212A RID: 8490
	[FieldOffset(124)]
	public int leftHandGrabbedActorNumber;

	// Token: 0x0400212B RID: 8491
	[FieldOffset(128)]
	public bool leftGrabbedHandIsLeft;

	// Token: 0x0400212C RID: 8492
	[FieldOffset(132)]
	public int rightHandGrabbedActorNumber;

	// Token: 0x0400212D RID: 8493
	[FieldOffset(136)]
	public bool rightGrabbedHandIsLeft;

	// Token: 0x0400212E RID: 8494
	[FieldOffset(140)]
	public float lastTouchedGroundAtTime;

	// Token: 0x0400212F RID: 8495
	[FieldOffset(144)]
	public float lastHandTouchedGroundAtTime;
}
