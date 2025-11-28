using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001173 RID: 4467
	public class QuaternionUtil
	{
		// Token: 0x060070BD RID: 28861 RVA: 0x0024E83C File Offset: 0x0024CA3C
		public static float Magnitude(Quaternion q)
		{
			return Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
		}

		// Token: 0x060070BE RID: 28862 RVA: 0x0024E87A File Offset: 0x0024CA7A
		public static float MagnitudeSqr(Quaternion q)
		{
			return q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;
		}

		// Token: 0x060070BF RID: 28863 RVA: 0x0024E8B4 File Offset: 0x0024CAB4
		public static Quaternion Normalize(Quaternion q)
		{
			float num = 1f / QuaternionUtil.Magnitude(q);
			return new Quaternion(num * q.x, num * q.y, num * q.z, num * q.w);
		}

		// Token: 0x060070C0 RID: 28864 RVA: 0x0024E8F4 File Offset: 0x0024CAF4
		public static Quaternion AngularVector(Vector3 v)
		{
			float magnitude = v.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return Quaternion.identity;
			}
			v /= magnitude;
			float num = 0.5f * magnitude;
			float num2 = Mathf.Sin(num);
			float num3 = Mathf.Cos(num);
			return new Quaternion(num2 * v.x, num2 * v.y, num2 * v.z, num3);
		}

		// Token: 0x060070C1 RID: 28865 RVA: 0x0024E954 File Offset: 0x0024CB54
		public static Quaternion AxisAngle(Vector3 axis, float angle)
		{
			float num = 0.5f * angle;
			float num2 = Mathf.Sin(num);
			float num3 = Mathf.Cos(num);
			return new Quaternion(num2 * axis.x, num2 * axis.y, num2 * axis.z, num3);
		}

		// Token: 0x060070C2 RID: 28866 RVA: 0x0024E994 File Offset: 0x0024CB94
		public static Vector3 GetAxis(Quaternion q)
		{
			Vector3 vector;
			vector..ctor(q.x, q.y, q.z);
			float magnitude = vector.magnitude;
			if (magnitude < MathUtil.Epsilon)
			{
				return Vector3.left;
			}
			return vector / magnitude;
		}

		// Token: 0x060070C3 RID: 28867 RVA: 0x0024E9D7 File Offset: 0x0024CBD7
		public static float GetAngle(Quaternion q)
		{
			return 2f * Mathf.Acos(Mathf.Clamp(q.w, -1f, 1f));
		}

		// Token: 0x060070C4 RID: 28868 RVA: 0x0024E9FC File Offset: 0x0024CBFC
		public static Quaternion Pow(Quaternion q, float exp)
		{
			Vector3 axis = QuaternionUtil.GetAxis(q);
			float angle = QuaternionUtil.GetAngle(q);
			return QuaternionUtil.AxisAngle(axis, angle * exp);
		}

		// Token: 0x060070C5 RID: 28869 RVA: 0x0024EA1E File Offset: 0x0024CC1E
		public static Quaternion Integrate(Quaternion q, Quaternion v, float dt)
		{
			return QuaternionUtil.Pow(v, dt) * q;
		}

		// Token: 0x060070C6 RID: 28870 RVA: 0x0024EA2D File Offset: 0x0024CC2D
		public static Quaternion Integrate(Quaternion q, Vector3 omega, float dt)
		{
			dt *= 0.5f;
			return QuaternionUtil.Normalize(new Quaternion(omega.x * dt, omega.y * dt, omega.z * dt, 1f) * q);
		}

		// Token: 0x060070C7 RID: 28871 RVA: 0x0024EA65 File Offset: 0x0024CC65
		public static Vector4 ToVector4(Quaternion q)
		{
			return new Vector4(q.x, q.y, q.z, q.w);
		}

		// Token: 0x060070C8 RID: 28872 RVA: 0x0024EA84 File Offset: 0x0024CC84
		public static Quaternion FromVector4(Vector4 v, bool normalize = true)
		{
			if (normalize)
			{
				float sqrMagnitude = v.sqrMagnitude;
				if (sqrMagnitude < MathUtil.Epsilon)
				{
					return Quaternion.identity;
				}
				v /= Mathf.Sqrt(sqrMagnitude);
			}
			return new Quaternion(v.x, v.y, v.z, v.w);
		}

		// Token: 0x060070C9 RID: 28873 RVA: 0x0024EAD8 File Offset: 0x0024CCD8
		public static void DecomposeSwingTwist(Quaternion q, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
		{
			Vector3 vector;
			vector..ctor(q.x, q.y, q.z);
			if (vector.sqrMagnitude < MathUtil.Epsilon)
			{
				Vector3 vector2 = q * twistAxis;
				Vector3 vector3 = Vector3.Cross(twistAxis, vector2);
				if (vector3.sqrMagnitude > MathUtil.Epsilon)
				{
					float num = Vector3.Angle(twistAxis, vector2);
					swing = Quaternion.AngleAxis(num, vector3);
				}
				else
				{
					swing = Quaternion.identity;
				}
				twist = Quaternion.AngleAxis(180f, twistAxis);
				return;
			}
			Vector3 vector4 = Vector3.Project(vector, twistAxis);
			twist = new Quaternion(vector4.x, vector4.y, vector4.z, q.w);
			twist = QuaternionUtil.Normalize(twist);
			swing = q * Quaternion.Inverse(twist);
		}

		// Token: 0x060070CA RID: 28874 RVA: 0x0024EBB4 File Offset: 0x0024CDB4
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float t, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			return QuaternionUtil.Sterp(a, b, twistAxis, t, out quaternion, out quaternion2, mode);
		}

		// Token: 0x060070CB RID: 28875 RVA: 0x0024EBD0 File Offset: 0x0024CDD0
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float t, out Quaternion swing, out Quaternion twist, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			return QuaternionUtil.Sterp(a, b, twistAxis, t, t, out swing, out twist, mode);
		}

		// Token: 0x060070CC RID: 28876 RVA: 0x0024EBE4 File Offset: 0x0024CDE4
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float tSwing, float tTwist, QuaternionUtil.SterpMode mode = QuaternionUtil.SterpMode.Slerp)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			return QuaternionUtil.Sterp(a, b, twistAxis, tSwing, tTwist, out quaternion, out quaternion2, mode);
		}

		// Token: 0x060070CD RID: 28877 RVA: 0x0024EC04 File Offset: 0x0024CE04
		public static Quaternion Sterp(Quaternion a, Quaternion b, Vector3 twistAxis, float tSwing, float tTwist, out Quaternion swing, out Quaternion twist, QuaternionUtil.SterpMode mode)
		{
			Quaternion quaternion;
			Quaternion quaternion2;
			QuaternionUtil.DecomposeSwingTwist(b * Quaternion.Inverse(a), twistAxis, out quaternion, out quaternion2);
			if (mode == QuaternionUtil.SterpMode.Nlerp || mode != QuaternionUtil.SterpMode.Slerp)
			{
				swing = Quaternion.Lerp(Quaternion.identity, quaternion, tSwing);
				twist = Quaternion.Lerp(Quaternion.identity, quaternion2, tTwist);
			}
			else
			{
				swing = Quaternion.Slerp(Quaternion.identity, quaternion, tSwing);
				twist = Quaternion.Slerp(Quaternion.identity, quaternion2, tTwist);
			}
			return twist * swing;
		}

		// Token: 0x02001174 RID: 4468
		public enum SterpMode
		{
			// Token: 0x040080E5 RID: 32997
			Nlerp,
			// Token: 0x040080E6 RID: 32998
			Slerp
		}
	}
}
