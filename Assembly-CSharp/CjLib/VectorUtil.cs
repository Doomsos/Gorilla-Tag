using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001175 RID: 4469
	public class VectorUtil
	{
		// Token: 0x060070CF RID: 28879 RVA: 0x0024ECB4 File Offset: 0x0024CEB4
		public static Vector3 Rotate2D(Vector3 v, float deg)
		{
			Vector3 result = v;
			float num = Mathf.Cos(MathUtil.Deg2Rad * deg);
			float num2 = Mathf.Sin(MathUtil.Deg2Rad * deg);
			result.x = num * v.x - num2 * v.y;
			result.y = num2 * v.x + num * v.y;
			return result;
		}

		// Token: 0x060070D0 RID: 28880 RVA: 0x0024ED0E File Offset: 0x0024CF0E
		public static Vector3 NormalizeSafe(Vector3 v, Vector3 fallback)
		{
			if (v.sqrMagnitude <= MathUtil.Epsilon)
			{
				return fallback;
			}
			return v.normalized;
		}

		// Token: 0x060070D1 RID: 28881 RVA: 0x0024ED28 File Offset: 0x0024CF28
		public static Vector3 FindOrthogonal(Vector3 v)
		{
			if (Mathf.Abs(v.x) >= MathUtil.Sqrt3Inv)
			{
				return Vector3.Normalize(new Vector3(v.y, -v.x, 0f));
			}
			return Vector3.Normalize(new Vector3(0f, v.z, -v.y));
		}

		// Token: 0x060070D2 RID: 28882 RVA: 0x0024ED80 File Offset: 0x0024CF80
		public static void FormOrthogonalBasis(Vector3 v, out Vector3 a, out Vector3 b)
		{
			a = VectorUtil.FindOrthogonal(v);
			b = Vector3.Cross(a, v);
		}

		// Token: 0x060070D3 RID: 28883 RVA: 0x0024EDA0 File Offset: 0x0024CFA0
		public static Vector3 Integrate(Vector3 x, Vector3 v, float dt)
		{
			return x + v * dt;
		}

		// Token: 0x060070D4 RID: 28884 RVA: 0x0024EDB0 File Offset: 0x0024CFB0
		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			float num = Vector3.Dot(a, b);
			if (num > 0.99999f)
			{
				return Vector3.Lerp(a, b, t);
			}
			if (num < -0.99999f)
			{
				Vector3 vector = VectorUtil.FindOrthogonal(a);
				return Quaternion.AngleAxis(180f * t, vector) * a;
			}
			float num2 = MathUtil.AcosSafe(num);
			return (Mathf.Sin((1f - t) * num2) * a + Mathf.Sin(t * num2) * b) / Mathf.Sin(num2);
		}

		// Token: 0x060070D5 RID: 28885 RVA: 0x0024EE34 File Offset: 0x0024D034
		public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}
	}
}
