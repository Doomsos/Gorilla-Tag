using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000C5A RID: 3162
public static class MathUtils
{
	// Token: 0x06004D5A RID: 19802 RVA: 0x00190B7B File Offset: 0x0018ED7B
	[MethodImpl(256)]
	public static float Xlerp(float a, float b, float dt, float decay = 16f)
	{
		return b + (a - b) * Mathf.Exp(-decay * dt);
	}

	// Token: 0x06004D5B RID: 19803 RVA: 0x00190B8C File Offset: 0x0018ED8C
	[MethodImpl(256)]
	public static Vector3 Xlerp(Vector3 a, Vector3 b, float dt, float decay = 16f)
	{
		return b + (a - b) * Mathf.Exp(-decay * dt);
	}

	// Token: 0x06004D5C RID: 19804 RVA: 0x00190BA9 File Offset: 0x0018EDA9
	[MethodImpl(256)]
	public static float SafeDivide(this float f, float d, float eps = 1E-06f)
	{
		if (Math.Abs(d) < eps)
		{
			return 0f;
		}
		if (float.IsNaN(f))
		{
			return 0f;
		}
		return f / d;
	}

	// Token: 0x06004D5D RID: 19805 RVA: 0x00190BCC File Offset: 0x0018EDCC
	[MethodImpl(256)]
	public static Vector3 SafeDivide(this Vector3 v, float d)
	{
		v.x = v.x.SafeDivide(d, 1E-05f);
		v.y = v.y.SafeDivide(d, 1E-05f);
		v.z = v.z.SafeDivide(d, 1E-05f);
		return v;
	}

	// Token: 0x06004D5E RID: 19806 RVA: 0x00190C24 File Offset: 0x0018EE24
	[MethodImpl(256)]
	public static Vector3 SafeDivide(this Vector3 v, Vector3 d)
	{
		v.x = v.x.SafeDivide(d.x, 1E-05f);
		v.y = v.y.SafeDivide(d.y, 1E-05f);
		v.z = v.z.SafeDivide(d.z, 1E-05f);
		return v;
	}

	// Token: 0x06004D5F RID: 19807 RVA: 0x00190C89 File Offset: 0x0018EE89
	[MethodImpl(256)]
	public static float Saturate(this float f, float eps = 1E-06f)
	{
		return Math.Min(Math.Max(f, 0f), 1f - eps);
	}

	// Token: 0x06004D60 RID: 19808 RVA: 0x00190CA2 File Offset: 0x0018EEA2
	[MethodImpl(256)]
	public static Vector3 Sin(this Vector3 v)
	{
		v.x = Mathf.Sin(v.x);
		v.y = Mathf.Sin(v.y);
		v.z = Mathf.Sin(v.z);
		return v;
	}

	// Token: 0x06004D61 RID: 19809 RVA: 0x00190CDB File Offset: 0x0018EEDB
	[MethodImpl(256)]
	public static float Quantize(this float f, float step)
	{
		return MathF.Round(f / step) * step;
	}

	// Token: 0x06004D62 RID: 19810 RVA: 0x00190CE7 File Offset: 0x0018EEE7
	[MethodImpl(256)]
	public static bool Approx(this Quaternion a, Quaternion b, float epsilon = 1E-06f)
	{
		return Math.Abs(Quaternion.Dot(a, b)) > 1f - epsilon;
	}

	// Token: 0x06004D63 RID: 19811 RVA: 0x00190D00 File Offset: 0x0018EF00
	[MethodImpl(256)]
	public static Vector3[] BoxCorners(Vector3 center, Vector3 size)
	{
		Vector3 vector;
		vector..ctor(size.x * 0.5f, 0f, 0f);
		Vector3 vector2;
		vector2..ctor(0f, size.y * 0.5f, 0f);
		Vector3 vector3;
		vector3..ctor(0f, 0f, size.z * 0.5f);
		return new Vector3[]
		{
			center + vector + vector2 + vector3,
			center + vector + vector2 - vector3,
			center - vector + vector2 - vector3,
			center - vector + vector2 + vector3,
			center + vector - vector2 + vector3,
			center + vector - vector2 - vector3,
			center - vector - vector2 - vector3,
			center - vector - vector2 + vector3
		};
	}

	// Token: 0x06004D64 RID: 19812 RVA: 0x00190E3C File Offset: 0x0018F03C
	[MethodImpl(256)]
	public static void BoxCornersNonAlloc(Vector3 center, Vector3 size, Vector3[] array, int index = 0)
	{
		Vector3 vector;
		vector..ctor(size.x * 0.5f, 0f, 0f);
		Vector3 vector2;
		vector2..ctor(0f, size.y * 0.5f, 0f);
		Vector3 vector3;
		vector3..ctor(0f, 0f, size.z * 0.5f);
		array[index] = center + vector + vector2 + vector3;
		array[index + 1] = center + vector + vector2 - vector3;
		array[index + 2] = center - vector + vector2 - vector3;
		array[index + 3] = center - vector + vector2 + vector3;
		array[index + 4] = center + vector - vector2 + vector3;
		array[index + 5] = center + vector - vector2 - vector3;
		array[index + 6] = center - vector - vector2 - vector3;
		array[index + 7] = center - vector - vector2 + vector3;
	}

	// Token: 0x06004D65 RID: 19813 RVA: 0x00190F80 File Offset: 0x0018F180
	[MethodImpl(256)]
	public static Vector3[] OrientedBoxCorners(Vector3 center, Vector3 size, Quaternion angles)
	{
		Vector3 vector = angles * new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 vector2 = angles * new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 vector3 = angles * new Vector3(0f, 0f, size.z * 0.5f);
		return new Vector3[]
		{
			center + vector + vector2 + vector3,
			center + vector + vector2 - vector3,
			center - vector + vector2 - vector3,
			center - vector + vector2 + vector3,
			center + vector - vector2 + vector3,
			center + vector - vector2 - vector3,
			center - vector - vector2 - vector3,
			center - vector - vector2 + vector3
		};
	}

	// Token: 0x06004D66 RID: 19814 RVA: 0x001910CC File Offset: 0x0018F2CC
	[MethodImpl(256)]
	public static void OrientedBoxCornersNonAlloc(Vector3 center, Vector3 size, Quaternion angles, Vector3[] array, int index = 0)
	{
		Vector3 vector = angles * new Vector3(size.x * 0.5f, 0f, 0f);
		Vector3 vector2 = angles * new Vector3(0f, size.y * 0.5f, 0f);
		Vector3 vector3 = angles * new Vector3(0f, 0f, size.z * 0.5f);
		array[index] = center + vector + vector2 + vector3;
		array[index + 1] = center + vector + vector2 - vector3;
		array[index + 2] = center - vector + vector2 - vector3;
		array[index + 3] = center - vector + vector2 + vector3;
		array[index + 4] = center + vector - vector2 + vector3;
		array[index + 5] = center + vector - vector2 - vector3;
		array[index + 6] = center - vector - vector2 - vector3;
		array[index + 7] = center - vector - vector2 + vector3;
	}

	// Token: 0x06004D67 RID: 19815 RVA: 0x00191220 File Offset: 0x0018F420
	[MethodImpl(256)]
	public static bool OrientedBoxContains(Vector3 point, Vector3 boxCenter, Vector3 boxSize, Quaternion boxAngles)
	{
		Vector3 vector = Matrix4x4.TRS(boxCenter, boxAngles, Vector3.one).inverse.MultiplyPoint3x4(point);
		Vector3 vector2 = boxSize * 0.5f;
		vector.x = Mathf.Abs(vector.x);
		vector.y = Mathf.Abs(vector.y);
		vector.z = Mathf.Abs(vector.z);
		return (Mathf.Approximately(vector.x, vector2.x) && Mathf.Approximately(vector.y, vector2.y) && Mathf.Approximately(vector.z, vector2.z)) || (vector.x < vector2.x && vector.y < vector2.y && vector.z < vector2.z);
	}

	// Token: 0x06004D68 RID: 19816 RVA: 0x001912F8 File Offset: 0x0018F4F8
	[MethodImpl(256)]
	public static int OrientedBoxSphereOverlap(Vector3 center, float radius, Vector3 boxCenter, Vector3 boxSize, Quaternion boxAngles)
	{
		Matrix4x4 matrix4x = Matrix4x4.Inverse(Matrix4x4.TRS(boxCenter, boxAngles, Vector3.one));
		Vector3 vector = boxSize * 0.5f;
		Vector3 vector2 = matrix4x.MultiplyPoint3x4(center);
		Vector3 vector3 = Vector3.right * radius;
		float magnitude = matrix4x.MultiplyVector(vector3).magnitude;
		Vector3 vector4 = -vector;
		Vector3 vector5 = vector2.Clamped(vector4, vector);
		if ((vector2 - vector5).sqrMagnitude > magnitude * magnitude)
		{
			return -1;
		}
		if (vector4.x + magnitude <= vector2.x && vector2.x <= vector.x - magnitude && vector.x - vector4.x > magnitude && vector4.y + magnitude <= vector2.y && vector2.y <= vector.y - magnitude && vector.y - vector4.y > magnitude && vector4.z + magnitude <= vector2.z && vector2.z <= vector.z - magnitude && vector.z - vector4.z > magnitude)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06004D69 RID: 19817 RVA: 0x00191420 File Offset: 0x0018F620
	[MethodImpl(256)]
	public static Vector3 Clamp(ref Vector3 v, ref Vector3 min, ref Vector3 max)
	{
		float num = v.x;
		num = ((num > max.x) ? max.x : num);
		num = ((num < min.x) ? min.x : num);
		float num2 = v.y;
		num2 = ((num2 > max.y) ? max.y : num2);
		num2 = ((num2 < min.y) ? min.y : num2);
		float num3 = v.z;
		num3 = ((num3 > max.z) ? max.z : num3);
		num3 = ((num3 < min.z) ? min.z : num3);
		return new Vector3(num, num2, num3);
	}

	// Token: 0x06004D6A RID: 19818 RVA: 0x001914BC File Offset: 0x0018F6BC
	[MethodImpl(256)]
	public static Bounds[] Subdivide(Bounds b, int x = 1, int y = 1, int z = 1)
	{
		if (x < 1)
		{
			x = 1;
		}
		if (y < 1)
		{
			y = 1;
		}
		if (z < 1)
		{
			z = 1;
		}
		int num = x * y * z;
		if (num == 1)
		{
			return new Bounds[]
			{
				b
			};
		}
		Vector3 size = b.size;
		float num2 = size.x * 0.5f;
		float num3 = size.y * 0.5f;
		float num4 = size.z * 0.5f;
		float num5 = size.x / (float)x;
		float num6 = size.y / (float)y;
		float num7 = size.z / (float)z;
		Vector3 size2;
		size2..ctor(num5, num6, num7);
		Bounds[] array = new Bounds[num];
		for (int i = 0; i < num; i++)
		{
			int num8;
			int num9;
			int num10;
			SpatialUtils.FlatIndexToXYZ(i, x, y, out num8, out num9, out num10);
			float num11 = num5 * (float)num8;
			float num12 = num5 * (float)(num8 + 1);
			float num13 = (num11 + num12) * 0.5f - num2;
			float num14 = num6 * (float)num9;
			float num15 = num6 * (float)(num9 + 1);
			float num16 = (num14 + num15) * 0.5f - num3;
			float num17 = num7 * (float)num10;
			float num18 = num7 * (float)(num10 + 1);
			float num19 = (num17 + num18) * 0.5f - num4;
			array[i].center = new Vector3(num13, num16, num19);
			array[i].size = size2;
		}
		return array;
	}

	// Token: 0x06004D6B RID: 19819 RVA: 0x00191601 File Offset: 0x0018F801
	[MethodImpl(256)]
	public static float ClampToReal(this float f, float min, float max, float epsilon = 1E-06f)
	{
		if (float.IsNaN(f))
		{
			f = 0f;
		}
		if (float.IsNegativeInfinity(min))
		{
			min = float.MinValue;
		}
		if (float.IsPositiveInfinity(max))
		{
			max = float.MaxValue;
		}
		return f.ClampApprox(min, max, epsilon);
	}

	// Token: 0x06004D6C RID: 19820 RVA: 0x00191639 File Offset: 0x0018F839
	[MethodImpl(256)]
	public static float ClampApprox(this float f, float min, float max, float epsilon = 1E-06f)
	{
		if (f < min || f.Approx(min, epsilon))
		{
			return min;
		}
		if (f > max || f.Approx(max, epsilon))
		{
			return max;
		}
		return f;
	}

	// Token: 0x06004D6D RID: 19821 RVA: 0x0019165C File Offset: 0x0018F85C
	[MethodImpl(256)]
	public static bool Approx(this float a, float b, float epsilon = 1E-06f)
	{
		return Math.Abs(a - b) < epsilon;
	}

	// Token: 0x06004D6E RID: 19822 RVA: 0x00191669 File Offset: 0x0018F869
	[MethodImpl(256)]
	public static bool Approx1(this float a, float epsilon = 1E-06f)
	{
		return Math.Abs(a - 1f) < epsilon;
	}

	// Token: 0x06004D6F RID: 19823 RVA: 0x0019167A File Offset: 0x0018F87A
	[MethodImpl(256)]
	public static bool Approx0(this float a, float epsilon = 1E-06f)
	{
		return Math.Abs(a) < epsilon;
	}

	// Token: 0x06004D70 RID: 19824 RVA: 0x00191688 File Offset: 0x0018F888
	[MethodImpl(256)]
	public static float GetScaledRadius(float radius, Vector3 scale)
	{
		float num = Math.Abs(scale.x);
		float num2 = Math.Abs(scale.y);
		float num3 = Math.Abs(scale.z);
		return Math.Max(Math.Abs(Math.Max(num, Math.Max(num2, num3)) * radius), 0f);
	}

	// Token: 0x06004D71 RID: 19825 RVA: 0x001916D8 File Offset: 0x0018F8D8
	public static float Linear(float value, float min, float max, float newMin, float newMax)
	{
		float num = (value - min) / (max - min) * (newMax - newMin) + newMin;
		if (num < newMin)
		{
			return newMin;
		}
		if (num > newMax)
		{
			return newMax;
		}
		return num;
	}

	// Token: 0x06004D72 RID: 19826 RVA: 0x00191703 File Offset: 0x0018F903
	public static float LinearUnclamped(float value, float min, float max, float newMin, float newMax)
	{
		return (value - min) / (max - min) * (newMax - newMin) + newMin;
	}

	// Token: 0x06004D73 RID: 19827 RVA: 0x00191714 File Offset: 0x0018F914
	public static float GetCircleValue(float degrees)
	{
		if (degrees > 90f)
		{
			degrees -= 180f;
		}
		else if (degrees < -90f)
		{
			degrees += 180f;
		}
		if (degrees > 180f)
		{
			degrees -= 270f;
		}
		else if (degrees < -180f)
		{
			degrees += 270f;
		}
		return degrees / 90f;
	}

	// Token: 0x06004D74 RID: 19828 RVA: 0x00191770 File Offset: 0x0018F970
	public static Vector3 WeightedMaxVector(Vector3 a, Vector3 b, float eps = 0.0001f)
	{
		float magnitude = a.magnitude;
		float magnitude2 = b.magnitude;
		if (magnitude < eps || magnitude2 < eps)
		{
			return Vector3.zero;
		}
		a / magnitude;
		b / magnitude2;
		Vector3 vector = a * (magnitude / (magnitude + magnitude2)) + b * (magnitude2 / (magnitude + magnitude2));
		float num = Mathf.Max(magnitude, magnitude2);
		return vector * num;
	}

	// Token: 0x06004D75 RID: 19829 RVA: 0x001917D4 File Offset: 0x0018F9D4
	public static Vector3 MatchMagnitudeInDirection(Vector3 input, Vector3 target, float eps = 0.0001f)
	{
		Vector3 result = input;
		float magnitude = target.magnitude;
		if (magnitude > eps)
		{
			Vector3 vector = target / magnitude;
			float num = Vector3.Dot(input, vector);
			float num2 = magnitude - num;
			if (num2 > 0f)
			{
				result = input + num2 * vector;
			}
		}
		return result;
	}

	// Token: 0x06004D76 RID: 19830 RVA: 0x00191820 File Offset: 0x0018FA20
	public static int CalculateAgeFromDateTime(DateTime Dob)
	{
		return new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
	}

	// Token: 0x06004D77 RID: 19831 RVA: 0x00191854 File Offset: 0x0018FA54
	public static int PositiveModulo(this int x, int m)
	{
		int num = x % m;
		if (num >= 0)
		{
			return num;
		}
		return num + m;
	}

	// Token: 0x06004D78 RID: 19832 RVA: 0x00191870 File Offset: 0x0018FA70
	public static float PositiveModulo(this float x, float m)
	{
		float num = x % m;
		if ((num < 0f && m > 0f) || (num > 0f && m < 0f))
		{
			num += m;
		}
		return num;
	}

	// Token: 0x04005CE1 RID: 23777
	private const float kDecay = 16f;

	// Token: 0x04005CE2 RID: 23778
	public const float kFloatEpsilon = 1E-06f;
}
