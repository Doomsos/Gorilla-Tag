using System;
using UnityEngine;

// Token: 0x020009FF RID: 2559
[Serializable]
public class XformNode
{
	// Token: 0x17000612 RID: 1554
	// (get) Token: 0x0600417D RID: 16765 RVA: 0x0015C224 File Offset: 0x0015A424
	public Vector4 worldPosition
	{
		get
		{
			if (!this.parent)
			{
				return this.localPosition;
			}
			Matrix4x4 localToWorldMatrix = this.parent.localToWorldMatrix;
			Vector4 result = this.localPosition;
			MatrixUtils.MultiplyXYZ3x4(ref localToWorldMatrix, ref result);
			return result;
		}
	}

	// Token: 0x17000613 RID: 1555
	// (get) Token: 0x0600417E RID: 16766 RVA: 0x0015C262 File Offset: 0x0015A462
	// (set) Token: 0x0600417F RID: 16767 RVA: 0x0015C26F File Offset: 0x0015A46F
	public float radius
	{
		get
		{
			return this.localPosition.w;
		}
		set
		{
			this.localPosition.w = value;
		}
	}

	// Token: 0x06004180 RID: 16768 RVA: 0x0015C27D File Offset: 0x0015A47D
	public Matrix4x4 LocalTRS()
	{
		return Matrix4x4.TRS(this.localPosition, Quaternion.identity, Vector3.one);
	}

	// Token: 0x0400525F RID: 21087
	public Vector4 localPosition;

	// Token: 0x04005260 RID: 21088
	public Transform parent;
}
