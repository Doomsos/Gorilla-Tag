using System;
using UnityEngine;

// Token: 0x0200039F RID: 927
[Serializable]
public struct SerializableVector3
{
	// Token: 0x0600161E RID: 5662 RVA: 0x0007B2B7 File Offset: 0x000794B7
	public SerializableVector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x0007B2CE File Offset: 0x000794CE
	public static implicit operator SerializableVector3(Vector3 v)
	{
		return new SerializableVector3(v.x, v.y, v.z);
	}

	// Token: 0x06001620 RID: 5664 RVA: 0x0007B2E7 File Offset: 0x000794E7
	public static implicit operator Vector3(SerializableVector3 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	// Token: 0x0400205B RID: 8283
	public float x;

	// Token: 0x0400205C RID: 8284
	public float y;

	// Token: 0x0400205D RID: 8285
	public float z;
}
