using System;
using UnityEngine;

// Token: 0x02000CFE RID: 3326
[Serializable]
public struct MatrixZonePair
{
	// Token: 0x0400600A RID: 24586
	[SerializeField]
	public Matrix4x4 matrix;

	// Token: 0x0400600B RID: 24587
	[SerializeField]
	public int zoneIndex;
}
