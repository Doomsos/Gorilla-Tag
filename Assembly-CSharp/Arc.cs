using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x020009C5 RID: 2501
[Serializable]
public struct Arc
{
	// Token: 0x06003FFA RID: 16378 RVA: 0x00157D6B File Offset: 0x00155F6B
	public Vector3[] GetArcPoints(int count = 12)
	{
		return Arc.ComputeArcPoints(this.start, this.end, new Vector3?(this.control), count);
	}

	// Token: 0x06003FFB RID: 16379 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("UNITY_EDITOR")]
	public void DrawGizmo()
	{
	}

	// Token: 0x06003FFC RID: 16380 RVA: 0x00157D8C File Offset: 0x00155F8C
	public static Arc From(Vector3 start, Vector3 end)
	{
		Vector3 vector = Arc.DeriveArcControlPoint(start, end, default(Vector3?), default(float?));
		return new Arc
		{
			start = start,
			end = end,
			control = vector
		};
	}

	// Token: 0x06003FFD RID: 16381 RVA: 0x00157DD4 File Offset: 0x00155FD4
	public static Vector3[] ComputeArcPoints(Vector3 a, Vector3 b, Vector3? c = null, int count = 12)
	{
		Vector3[] array = new Vector3[count];
		float num = 1f / (float)count;
		Vector3 vector = c.GetValueOrDefault();
		if (c == null)
		{
			vector = Arc.DeriveArcControlPoint(a, b, default(Vector3?), default(float?));
			c = new Vector3?(vector);
		}
		for (int i = 0; i < count; i++)
		{
			float t;
			if (i == 0)
			{
				t = 0f;
			}
			else if (i == count - 1)
			{
				t = 1f;
			}
			else
			{
				t = num * (float)i;
			}
			array[i] = Arc.BezierLerp(a, b, c.Value, t);
		}
		return array;
	}

	// Token: 0x06003FFE RID: 16382 RVA: 0x00157E74 File Offset: 0x00156074
	public static Vector3 BezierLerp(Vector3 a, Vector3 b, Vector3 c, float t)
	{
		Vector3 vector = Vector3.Lerp(a, c, t);
		Vector3 vector2 = Vector3.Lerp(c, b, t);
		return Vector3.Lerp(vector, vector2, t);
	}

	// Token: 0x06003FFF RID: 16383 RVA: 0x00157E9C File Offset: 0x0015609C
	public static Vector3 DeriveArcControlPoint(Vector3 a, Vector3 b, Vector3? dir = null, float? height = null)
	{
		Vector3 vector = (b - a) * 0.5f;
		Vector3 normalized = vector.normalized;
		float num = height.GetValueOrDefault();
		if (height == null)
		{
			num = vector.magnitude;
			height = new float?(num);
		}
		if (dir == null)
		{
			Vector3 vector2 = Vector3.Cross(normalized, Vector3.up);
			dir = new Vector3?(Vector3.Cross(normalized, vector2));
		}
		Vector3 vector3 = dir.Value * -height.Value;
		return a + vector + vector3;
	}

	// Token: 0x0400512D RID: 20781
	public Vector3 start;

	// Token: 0x0400512E RID: 20782
	public Vector3 end;

	// Token: 0x0400512F RID: 20783
	public Vector3 control;
}
