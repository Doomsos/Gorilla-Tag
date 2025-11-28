using System;
using System.Runtime.InteropServices;
using Fusion;
using UnityEngine;

// Token: 0x020003AB RID: 939
[NetworkInputWeaved(35)]
[StructLayout(2, Size = 140)]
public struct NetworkedInput : INetworkInput
{
	// Token: 0x0400207B RID: 8315
	[FieldOffset(0)]
	public Quaternion headRot_LS;

	// Token: 0x0400207C RID: 8316
	[FieldOffset(16)]
	public Vector3 rightHandPos_LS;

	// Token: 0x0400207D RID: 8317
	[FieldOffset(28)]
	public Quaternion rightHandRot_LS;

	// Token: 0x0400207E RID: 8318
	[FieldOffset(44)]
	public Vector3 leftHandPos_LS;

	// Token: 0x0400207F RID: 8319
	[FieldOffset(56)]
	public Quaternion leftHandRot_LS;

	// Token: 0x04002080 RID: 8320
	[FieldOffset(72)]
	public Vector3 rootPosition;

	// Token: 0x04002081 RID: 8321
	[FieldOffset(84)]
	public Quaternion rootRotation;

	// Token: 0x04002082 RID: 8322
	[FieldOffset(100)]
	public bool leftThumbTouch;

	// Token: 0x04002083 RID: 8323
	[FieldOffset(104)]
	public bool leftThumbPress;

	// Token: 0x04002084 RID: 8324
	[FieldOffset(108)]
	public float leftIndexValue;

	// Token: 0x04002085 RID: 8325
	[FieldOffset(112)]
	public float leftMiddleValue;

	// Token: 0x04002086 RID: 8326
	[FieldOffset(116)]
	public bool rightThumbTouch;

	// Token: 0x04002087 RID: 8327
	[FieldOffset(120)]
	public bool rightThumbPress;

	// Token: 0x04002088 RID: 8328
	[FieldOffset(124)]
	public float rightIndexValue;

	// Token: 0x04002089 RID: 8329
	[FieldOffset(128)]
	public float rightMiddleValue;

	// Token: 0x0400208A RID: 8330
	[FieldOffset(132)]
	public float scale;

	// Token: 0x0400208B RID: 8331
	[FieldOffset(136)]
	public int handPoseData;
}
