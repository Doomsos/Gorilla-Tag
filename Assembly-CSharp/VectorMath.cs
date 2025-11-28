using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

// Token: 0x02000C9A RID: 3226
public static class VectorMath
{
	// Token: 0x06004EC4 RID: 20164 RVA: 0x00197790 File Offset: 0x00195990
	[MethodImpl(256)]
	public static Vector3Int Clamped(this Vector3Int v, int min, int max)
	{
		v.x = Math.Clamp(v.x, min, max);
		v.y = Math.Clamp(v.y, min, max);
		v.z = Math.Clamp(v.z, min, max);
		return v;
	}

	// Token: 0x06004EC5 RID: 20165 RVA: 0x001977DD File Offset: 0x001959DD
	[MethodImpl(256)]
	public static void SetXYZ(this Vector3 v, float f)
	{
		v.x = f;
		v.y = f;
		v.z = f;
	}

	// Token: 0x06004EC6 RID: 20166 RVA: 0x001977F4 File Offset: 0x001959F4
	[MethodImpl(256)]
	public static Vector3Int Abs(this Vector3Int v)
	{
		v.x = Math.Abs(v.x);
		v.y = Math.Abs(v.y);
		v.z = Math.Abs(v.z);
		return v;
	}

	// Token: 0x06004EC7 RID: 20167 RVA: 0x00197830 File Offset: 0x00195A30
	[MethodImpl(256)]
	public static Vector3 Abs(this Vector3 v)
	{
		v.x = Math.Abs(v.x);
		v.y = Math.Abs(v.y);
		v.z = Math.Abs(v.z);
		return v;
	}

	// Token: 0x06004EC8 RID: 20168 RVA: 0x00197869 File Offset: 0x00195A69
	[MethodImpl(256)]
	public static Vector3 Min(this Vector3 v, Vector3 other)
	{
		return new Vector3(Math.Min(v.x, other.x), Math.Min(v.y, other.y), Math.Min(v.z, other.z));
	}

	// Token: 0x06004EC9 RID: 20169 RVA: 0x001978A3 File Offset: 0x00195AA3
	[MethodImpl(256)]
	public static Vector3 Max(this Vector3 v, Vector3 other)
	{
		return new Vector3(Math.Max(v.x, other.x), Math.Max(v.y, other.y), Math.Max(v.z, other.z));
	}

	// Token: 0x06004ECA RID: 20170 RVA: 0x001978DD File Offset: 0x00195ADD
	[MethodImpl(256)]
	public static Vector3 Add(this Vector3 v, float amount)
	{
		v.x += amount;
		v.y += amount;
		v.z += amount;
		return v;
	}

	// Token: 0x06004ECB RID: 20171 RVA: 0x00197904 File Offset: 0x00195B04
	[MethodImpl(256)]
	public static Vector3 Sub(this Vector3 v, float amount)
	{
		v.x -= amount;
		v.y -= amount;
		v.z -= amount;
		return v;
	}

	// Token: 0x06004ECC RID: 20172 RVA: 0x0019792B File Offset: 0x00195B2B
	[MethodImpl(256)]
	public static Vector3 Mul(this Vector3 v, float amount)
	{
		v.x *= amount;
		v.y *= amount;
		v.z *= amount;
		return v;
	}

	// Token: 0x06004ECD RID: 20173 RVA: 0x00197954 File Offset: 0x00195B54
	[MethodImpl(256)]
	public static Vector3 Div(this Vector3 v, float amount)
	{
		float num = 1f / amount;
		v.x *= num;
		v.y *= num;
		v.z *= num;
		return v;
	}

	// Token: 0x06004ECE RID: 20174 RVA: 0x00197990 File Offset: 0x00195B90
	[MethodImpl(256)]
	public static Vector3 Max(this Vector3 v)
	{
		float num = Math.Max(Math.Max(v.x, v.y), v.z);
		v.x = num;
		v.y = num;
		v.z = num;
		return v;
	}

	// Token: 0x06004ECF RID: 20175 RVA: 0x001979D4 File Offset: 0x00195BD4
	[MethodImpl(256)]
	public static Vector3 Max(this Vector3 v, float max)
	{
		float num = Math.Max(Math.Max(Math.Max(v.x, v.y), v.z), max);
		v.x = num;
		v.y = num;
		v.z = num;
		return v;
	}

	// Token: 0x06004ED0 RID: 20176 RVA: 0x00197A20 File Offset: 0x00195C20
	[MethodImpl(256)]
	public static float3 Max(this float3 v)
	{
		float num = Math.Max(v.x, Math.Max(v.y, v.z));
		v.x = num;
		v.y = num;
		v.z = num;
		return v;
	}

	// Token: 0x06004ED1 RID: 20177 RVA: 0x00197A63 File Offset: 0x00195C63
	[MethodImpl(256)]
	public static bool IsFinite(this Vector3 v)
	{
		return float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
	}

	// Token: 0x06004ED2 RID: 20178 RVA: 0x00197A8C File Offset: 0x00195C8C
	[MethodImpl(256)]
	public static Vector3 Clamped(this Vector3 v, Vector3 min, Vector3 max)
	{
		v.x = Math.Clamp(v.x, min.x, max.x);
		v.y = Math.Clamp(v.y, min.y, max.y);
		v.z = Math.Clamp(v.z, min.z, max.z);
		return v;
	}

	// Token: 0x06004ED3 RID: 20179 RVA: 0x00197AF4 File Offset: 0x00195CF4
	[MethodImpl(256)]
	public static bool Approx0(this Vector3 v, float epsilon = 1E-05f)
	{
		float x = v.x;
		float y = v.y;
		float z = v.z;
		return x * x + y * y + z * z <= epsilon * epsilon;
	}

	// Token: 0x06004ED4 RID: 20180 RVA: 0x00197B28 File Offset: 0x00195D28
	[MethodImpl(256)]
	public static bool Approx1(this Vector3 v, float epsilon = 1E-05f)
	{
		float num = v.x - 1f;
		float num2 = v.y - 1f;
		float num3 = v.z - 1f;
		return num * num + num2 * num2 + num3 * num3 <= epsilon * epsilon;
	}

	// Token: 0x06004ED5 RID: 20181 RVA: 0x00197B70 File Offset: 0x00195D70
	[MethodImpl(256)]
	public static bool Approx(this Vector3 a, Vector3 b, float epsilon = 1E-05f)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		return num * num + num2 * num2 + num3 * num3 <= epsilon * epsilon;
	}

	// Token: 0x06004ED6 RID: 20182 RVA: 0x00197BB8 File Offset: 0x00195DB8
	[MethodImpl(256)]
	public static bool Approx(this Vector4 a, Vector4 b, float epsilon = 1E-05f)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		float num4 = a.w - b.w;
		return num * num + num2 * num2 + num3 * num3 + num4 * num4 <= epsilon * epsilon;
	}
}
