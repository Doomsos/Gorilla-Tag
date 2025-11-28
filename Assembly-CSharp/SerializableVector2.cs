using System;
using UnityEngine;

// Token: 0x0200039E RID: 926
[Serializable]
public struct SerializableVector2
{
	// Token: 0x0600161B RID: 5659 RVA: 0x0007B281 File Offset: 0x00079481
	public SerializableVector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x0600161C RID: 5660 RVA: 0x0007B291 File Offset: 0x00079491
	public static implicit operator SerializableVector2(Vector2 v)
	{
		return new SerializableVector2(v.x, v.y);
	}

	// Token: 0x0600161D RID: 5661 RVA: 0x0007B2A4 File Offset: 0x000794A4
	public static implicit operator Vector2(SerializableVector2 v)
	{
		return new Vector2(v.x, v.y);
	}

	// Token: 0x04002059 RID: 8281
	public float x;

	// Token: 0x0400205A RID: 8282
	public float y;
}
