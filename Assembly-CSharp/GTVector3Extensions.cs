using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020002B7 RID: 695
public static class GTVector3Extensions
{
	// Token: 0x0600113A RID: 4410 RVA: 0x0005BE7E File Offset: 0x0005A07E
	[MethodImpl(256)]
	public static Vector3 X_Z(this Vector3 vector)
	{
		return new Vector3(vector.x, 0f, vector.z);
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x0005BE98 File Offset: 0x0005A098
	public static Vector3 Sum(this IEnumerable<Vector3> vecs)
	{
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < Enumerable.Count<Vector3>(vecs); i++)
		{
			vector += Enumerable.ElementAt<Vector3>(vecs, i);
		}
		return vector;
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x0005BECB File Offset: 0x0005A0CB
	public static Vector3 Average(this IEnumerable<Vector3> vecs)
	{
		return vecs.Sum() / (float)Enumerable.Count<Vector3>(vecs);
	}
}
