using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000C91 RID: 3217
public static class UnityEngineUtils
{
	// Token: 0x06004E9B RID: 20123 RVA: 0x0019708A File Offset: 0x0019528A
	[MethodImpl(256)]
	public static bool EqualsColor(this Color32 c, Color32 other)
	{
		return c.r == other.r && c.g == other.g && c.b == other.b && c.a == other.a;
	}

	// Token: 0x06004E9C RID: 20124 RVA: 0x001970C8 File Offset: 0x001952C8
	public static Color32 IdToColor32(this Object obj, int alpha = -1, bool distinct = true)
	{
		if (!(obj == null))
		{
			return obj.GetInstanceID().IdToColor32(alpha, distinct);
		}
		return default(Color32);
	}

	// Token: 0x06004E9D RID: 20125 RVA: 0x001970F8 File Offset: 0x001952F8
	public unsafe static Color32 IdToColor32(this int id, int alpha = -1, bool distinct = true)
	{
		if (distinct)
		{
			id = StaticHash.ComputeTriple32(id);
		}
		Color32 result = *Unsafe.As<int, Color32>(ref id);
		if (alpha > -1)
		{
			result.a = (byte)Math.Clamp(alpha, 0, 255);
		}
		return result;
	}

	// Token: 0x06004E9E RID: 20126 RVA: 0x00197138 File Offset: 0x00195338
	public static Color32 ToHighViz(this Color32 c)
	{
		float num;
		float num2;
		float num3;
		Color.RGBToHSV(c, ref num, ref num2, ref num3);
		return Color.HSVToRGB(num, 1f, 1f);
	}

	// Token: 0x06004E9F RID: 20127 RVA: 0x0019716C File Offset: 0x0019536C
	public unsafe static int Color32ToId(this Color32 c, bool distinct = true)
	{
		int num = *Unsafe.As<Color32, int>(ref c);
		if (distinct)
		{
			num = StaticHash.ReverseTriple32(num);
		}
		return num;
	}

	// Token: 0x06004EA0 RID: 20128 RVA: 0x00197190 File Offset: 0x00195390
	public static Hash128 QuantizedHash128(this Matrix4x4 m)
	{
		Hash128 result = default(Hash128);
		HashUtilities.QuantisedMatrixHash(ref m, ref result);
		return result;
	}

	// Token: 0x06004EA1 RID: 20129 RVA: 0x001971B0 File Offset: 0x001953B0
	public static Hash128 QuantizedHash128(this Vector3 v)
	{
		Hash128 result = default(Hash128);
		HashUtilities.QuantisedVectorHash(ref v, ref result);
		return result;
	}

	// Token: 0x06004EA2 RID: 20130 RVA: 0x001971CF File Offset: 0x001953CF
	public static Id128 QuantizedId128(this Vector3 v)
	{
		return v.QuantizedHash128();
	}

	// Token: 0x06004EA3 RID: 20131 RVA: 0x001971DC File Offset: 0x001953DC
	public static Id128 QuantizedId128(this Matrix4x4 m)
	{
		return m.QuantizedHash128();
	}

	// Token: 0x06004EA4 RID: 20132 RVA: 0x001971EC File Offset: 0x001953EC
	public static Id128 QuantizedId128(this Quaternion q)
	{
		int a = (int)((double)q.x * 1000.0 + 0.5);
		int b = (int)((double)q.y * 1000.0 + 0.5);
		int c = (int)((double)q.z * 1000.0 + 0.5);
		int d = (int)((double)q.w * 1000.0 + 0.5);
		return new Id128(a, b, c, d);
	}

	// Token: 0x06004EA5 RID: 20133 RVA: 0x00197274 File Offset: 0x00195474
	[MethodImpl(256)]
	public static long QuantizedHash64(this Vector4 v)
	{
		int a = (int)((double)v.x * 1000.0 + 0.5);
		int b = (int)((double)v.y * 1000.0 + 0.5);
		int a2 = (int)((double)v.z * 1000.0 + 0.5);
		int b2 = (int)((double)v.w * 1000.0 + 0.5);
		ulong a3 = UnityEngineUtils.MergeTo64(a, b);
		ulong b3 = UnityEngineUtils.MergeTo64(a2, b2);
		return StaticHash.Compute128To64(a3, b3);
	}

	// Token: 0x06004EA6 RID: 20134 RVA: 0x00197308 File Offset: 0x00195508
	[MethodImpl(256)]
	public unsafe static long QuantizedHash64(this Matrix4x4 m)
	{
		m4x4 m4x = *m4x4.From(ref m);
		long a = m4x.r0.QuantizedHash64();
		long b = m4x.r1.QuantizedHash64();
		long a2 = m4x.r2.QuantizedHash64();
		long b2 = m4x.r3.QuantizedHash64();
		long a3 = StaticHash.Compute128To64(a, b);
		long b3 = StaticHash.Compute128To64(a2, b2);
		return StaticHash.Compute128To64(a3, b3);
	}

	// Token: 0x06004EA7 RID: 20135 RVA: 0x00197368 File Offset: 0x00195568
	[MethodImpl(256)]
	private static ulong MergeTo64(int a, int b)
	{
		return (ulong)b << 32 | (ulong)a;
	}

	// Token: 0x06004EA8 RID: 20136 RVA: 0x0019737F File Offset: 0x0019557F
	[MethodImpl(256)]
	public unsafe static Vector4 ToVector(this Quaternion q)
	{
		return *Unsafe.As<Quaternion, Vector4>(ref q);
	}

	// Token: 0x06004EA9 RID: 20137 RVA: 0x0019738D File Offset: 0x0019558D
	[MethodImpl(256)]
	public static void CopyTo(this Quaternion q, ref Vector4 v)
	{
		v.x = q.x;
		v.y = q.y;
		v.z = q.z;
		v.w = q.w;
	}
}
