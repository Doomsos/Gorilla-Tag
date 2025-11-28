using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001171 RID: 4465
	public struct Vector4Spring
	{
		// Token: 0x060070A9 RID: 28841 RVA: 0x0024E3E3 File Offset: 0x0024C5E3
		public void Reset()
		{
			this.Value = Vector4.zero;
			this.Velocity = Vector4.zero;
		}

		// Token: 0x060070AA RID: 28842 RVA: 0x0024E3FB File Offset: 0x0024C5FB
		public void Reset(Vector4 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector4.zero;
		}

		// Token: 0x060070AB RID: 28843 RVA: 0x0024E40F File Offset: 0x0024C60F
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x060070AC RID: 28844 RVA: 0x0024E420 File Offset: 0x0024C620
		public Vector4 TrackDampingRatio(Vector4 targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = Vector4.zero;
				return this.Value;
			}
			Vector4 vector = targetValue - this.Value;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector4 vector2 = num * this.Value + deltaTime * this.Velocity + num4 * targetValue;
			Vector4 vector3 = this.Velocity + num3 * vector;
			this.Velocity = vector3 * num5;
			this.Value = vector2 * num5;
			if (this.Velocity.magnitude < MathUtil.Epsilon && vector.magnitude < MathUtil.Epsilon)
			{
				this.Velocity = Vector4.zero;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x060070AD RID: 28845 RVA: 0x0024E51C File Offset: 0x0024C71C
		public Vector4 TrackHalfLife(Vector4 targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector4.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float dampingRatio = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, dampingRatio, deltaTime);
		}

		// Token: 0x060070AE RID: 28846 RVA: 0x0024E568 File Offset: 0x0024C768
		public Vector4 TrackExponential(Vector4 targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector4.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float angularFrequency = 0.6931472f / halfLife;
			float dampingRatio = 1f;
			return this.TrackDampingRatio(targetValue, angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x040080DE RID: 32990
		public static readonly int Stride = 32;

		// Token: 0x040080DF RID: 32991
		public Vector4 Value;

		// Token: 0x040080E0 RID: 32992
		public Vector4 Velocity;
	}
}
