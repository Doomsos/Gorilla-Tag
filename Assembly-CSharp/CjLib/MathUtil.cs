using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200116D RID: 4461
	public class MathUtil
	{
		// Token: 0x0600708F RID: 28815 RVA: 0x0024DD58 File Offset: 0x0024BF58
		public static float AsinSafe(float x)
		{
			return Mathf.Asin(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06007090 RID: 28816 RVA: 0x0024DD6F File Offset: 0x0024BF6F
		public static float AcosSafe(float x)
		{
			return Mathf.Acos(Mathf.Clamp(x, -1f, 1f));
		}

		// Token: 0x06007091 RID: 28817 RVA: 0x0024DD88 File Offset: 0x0024BF88
		public static float CatmullRom(float p0, float p1, float p2, float p3, float t)
		{
			float num = t * t;
			return 0.5f * (2f * p1 + (-p0 + p2) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (-p0 + 3f * p1 - 3f * p2 + p3) * num * t);
		}

		// Token: 0x040080C4 RID: 32964
		public static readonly float Pi = 3.1415927f;

		// Token: 0x040080C5 RID: 32965
		public static readonly float TwoPi = 6.2831855f;

		// Token: 0x040080C6 RID: 32966
		public static readonly float HalfPi = 1.5707964f;

		// Token: 0x040080C7 RID: 32967
		public static readonly float ThirdPi = 1.0471976f;

		// Token: 0x040080C8 RID: 32968
		public static readonly float QuarterPi = 0.7853982f;

		// Token: 0x040080C9 RID: 32969
		public static readonly float FifthPi = 0.62831855f;

		// Token: 0x040080CA RID: 32970
		public static readonly float SixthPi = 0.5235988f;

		// Token: 0x040080CB RID: 32971
		public static readonly float Sqrt2 = Mathf.Sqrt(2f);

		// Token: 0x040080CC RID: 32972
		public static readonly float Sqrt2Inv = 1f / Mathf.Sqrt(2f);

		// Token: 0x040080CD RID: 32973
		public static readonly float Sqrt3 = Mathf.Sqrt(3f);

		// Token: 0x040080CE RID: 32974
		public static readonly float Sqrt3Inv = 1f / Mathf.Sqrt(3f);

		// Token: 0x040080CF RID: 32975
		public static readonly float Epsilon = 1E-09f;

		// Token: 0x040080D0 RID: 32976
		public static readonly float EpsilonComp = 1f - MathUtil.Epsilon;

		// Token: 0x040080D1 RID: 32977
		public static readonly float Rad2Deg = 57.295776f;

		// Token: 0x040080D2 RID: 32978
		public static readonly float Deg2Rad = 0.017453292f;
	}
}
