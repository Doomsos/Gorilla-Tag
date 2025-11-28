using System;
using UnityEngine;

// Token: 0x020009EE RID: 2542
[Serializable]
public struct OrientedBounds
{
	// Token: 0x17000601 RID: 1537
	// (get) Token: 0x060040CA RID: 16586 RVA: 0x0015A644 File Offset: 0x00158844
	public static OrientedBounds Empty { get; } = new OrientedBounds
	{
		size = Vector3.zero,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x17000602 RID: 1538
	// (get) Token: 0x060040CB RID: 16587 RVA: 0x0015A64B File Offset: 0x0015884B
	public static OrientedBounds Identity { get; } = new OrientedBounds
	{
		size = Vector3.one,
		center = Vector3.zero,
		rotation = Quaternion.identity
	};

	// Token: 0x060040CC RID: 16588 RVA: 0x0015A652 File Offset: 0x00158852
	public Matrix4x4 TRS()
	{
		return Matrix4x4.TRS(this.center, this.rotation, this.size);
	}

	// Token: 0x04005202 RID: 20994
	public Vector3 size;

	// Token: 0x04005203 RID: 20995
	public Vector3 center;

	// Token: 0x04005204 RID: 20996
	public Quaternion rotation;
}
