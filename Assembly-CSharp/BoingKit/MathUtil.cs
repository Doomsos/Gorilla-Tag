using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011BB RID: 4539
	public class MathUtil
	{
		// Token: 0x06007276 RID: 29302 RVA: 0x0024DD38 File Offset: 0x0024BF38
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06007277 RID: 29303 RVA: 0x0024DD4F File Offset: 0x0024BF4F
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06007278 RID: 29304 RVA: 0x00258E40 File Offset: 0x00257040
		public static float InvSafe(float x)
		{
			return 1f / Mathf.Max(MathUtil.Epsilon, x);
		}

		// Token: 0x06007279 RID: 29305 RVA: 0x00258E54 File Offset: 0x00257054
		public static float PointLineDist(Vector2 point, Vector2 linePos, Vector2 lineDir)
		{
			Vector2 vector = point - linePos;
			return (vector - Vector2.Dot(vector, lineDir) * lineDir).magnitude;
		}

		// Token: 0x0600727A RID: 29306 RVA: 0x00258E84 File Offset: 0x00257084
		public static float PointSegmentDist(Vector2 point, Vector2 segmentPosA, Vector2 segmentPosB)
		{
			Vector2 vector = segmentPosB - segmentPosA;
			float num = 1f / vector.magnitude;
			Vector2 vector2 = vector * num;
			float num2 = Vector2.Dot(point - segmentPosA, vector2) * num;
			return (segmentPosA + Mathf.Clamp(num2, 0f, 1f) * vector - point).magnitude;
		}

		// Token: 0x0600727B RID: 29307 RVA: 0x00258EEC File Offset: 0x002570EC
		public static float Seek(float current, float target, float maxDelta)
		{
			float num = target - current;
			num = Mathf.Sign(num) * Mathf.Min(maxDelta, Mathf.Abs(num));
			return current + num;
		}

		// Token: 0x0600727C RID: 29308 RVA: 0x00258F14 File Offset: 0x00257114
		public static Vector2 Seek(Vector2 current, Vector2 target, float maxDelta)
		{
			Vector2 vector = target - current;
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return target;
			}
			vector = Mathf.Min(maxDelta, magnitude) * vector.normalized;
			return current + vector;
		}

		// Token: 0x0600727D RID: 29309 RVA: 0x00258F56 File Offset: 0x00257156
		public static float Remainder(float a, float b)
		{
			return a - a / b * b;
		}

		// Token: 0x0600727E RID: 29310 RVA: 0x00258F56 File Offset: 0x00257156
		public static int Remainder(int a, int b)
		{
			return a - a / b * b;
		}

		// Token: 0x0600727F RID: 29311 RVA: 0x00258F5F File Offset: 0x0025715F
		public static float Modulo(float a, float b)
		{
			return Mathf.Repeat(a, b);
		}

		// Token: 0x06007280 RID: 29312 RVA: 0x00258F68 File Offset: 0x00257168
		public static int Modulo(int a, int b)
		{
			int num = a % b;
			if (num < 0)
			{
				return num + b;
			}
			return num;
		}

		// Token: 0x040082D6 RID: 33494
		public static readonly float Pi = 3.1415927f;

		// Token: 0x040082D7 RID: 33495
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x040082D8 RID: 33496
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x040082D9 RID: 33497
		public static readonly float QuaterPi = 0.7853982f;

		// Token: 0x040082DA RID: 33498
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x040082DB RID: 33499
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x040082DC RID: 33500
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x040082DD RID: 33501
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x040082DE RID: 33502
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x040082DF RID: 33503
		public static readonly float Epsilon = 1E-06f;

		// Token: 0x040082E0 RID: 33504
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x040082E1 RID: 33505
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
