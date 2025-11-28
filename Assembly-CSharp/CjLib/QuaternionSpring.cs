using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001172 RID: 4466
	public struct QuaternionSpring
	{
		// Token: 0x17000A82 RID: 2690
		// (get) Token: 0x060070B0 RID: 28848 RVA: 0x0024E5D7 File Offset: 0x0024C7D7
		// (set) Token: 0x060070B1 RID: 28849 RVA: 0x0024E5E5 File Offset: 0x0024C7E5
		public Quaternion ValueQuat
		{
			get
			{
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			set
			{
				this.ValueVec = QuaternionUtil.ToVector4(value);
			}
		}

		// Token: 0x17000A83 RID: 2691
		// (get) Token: 0x060070B2 RID: 28850 RVA: 0x0024E5F3 File Offset: 0x0024C7F3
		// (set) Token: 0x060070B3 RID: 28851 RVA: 0x0024E601 File Offset: 0x0024C801
		public Quaternion VelocityQuat
		{
			get
			{
				return QuaternionUtil.FromVector4(this.VelocityVec, false);
			}
			set
			{
				this.VelocityVec = QuaternionUtil.ToVector4(value);
			}
		}

		// Token: 0x060070B4 RID: 28852 RVA: 0x0024E60F File Offset: 0x0024C80F
		public void Reset()
		{
			this.ValueVec = QuaternionUtil.ToVector4(Quaternion.identity);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x060070B5 RID: 28853 RVA: 0x0024E62C File Offset: 0x0024C82C
		public void Reset(Vector4 initValue)
		{
			this.ValueVec = initValue;
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x060070B6 RID: 28854 RVA: 0x0024E640 File Offset: 0x0024C840
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.ValueVec = initValue;
			this.VelocityVec = initVelocity;
		}

		// Token: 0x060070B7 RID: 28855 RVA: 0x0024E650 File Offset: 0x0024C850
		public void Reset(Quaternion initValue)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x060070B8 RID: 28856 RVA: 0x0024E669 File Offset: 0x0024C869
		public void Reset(Quaternion initValue, Quaternion initVelocity)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = QuaternionUtil.ToVector4(initVelocity);
		}

		// Token: 0x060070B9 RID: 28857 RVA: 0x0024E684 File Offset: 0x0024C884
		public Quaternion TrackDampingRatio(Quaternion targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			Vector4 vector = QuaternionUtil.ToVector4(targetValue);
			if (Vector4.Dot(this.ValueVec, vector) < 0f)
			{
				vector = -vector;
			}
			Vector4 vector2 = vector - this.ValueVec;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector4 vector3 = num * this.ValueVec + deltaTime * this.VelocityVec + num4 * vector;
			Vector4 vector4 = this.VelocityVec + num3 * vector2;
			this.VelocityVec = vector4 * num5;
			this.ValueVec = vector3 * num5;
			if (this.VelocityVec.magnitude < MathUtil.Epsilon && vector2.magnitude < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = vector;
			}
			return QuaternionUtil.FromVector4(this.ValueVec, true);
		}

		// Token: 0x060070BA RID: 28858 RVA: 0x0024E7B8 File Offset: 0x0024C9B8
		public Quaternion TrackHalfLife(Quaternion targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = QuaternionUtil.ToVector4(targetValue);
				return targetValue;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float dampingRatio = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, dampingRatio, deltaTime);
		}

		// Token: 0x060070BB RID: 28859 RVA: 0x0024E808 File Offset: 0x0024CA08
		public Quaternion TrackExponential(Quaternion targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				this.ValueVec = QuaternionUtil.ToVector4(targetValue);
				return targetValue;
			}
			float angularFrequency = 0.6931472f / halfLife;
			float dampingRatio = 1f;
			return this.TrackDampingRatio(targetValue, angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x040080E1 RID: 32993
		public static readonly int Stride = 32;

		// Token: 0x040080E2 RID: 32994
		public Vector4 ValueVec;

		// Token: 0x040080E3 RID: 32995
		public Vector4 VelocityVec;
	}
}
