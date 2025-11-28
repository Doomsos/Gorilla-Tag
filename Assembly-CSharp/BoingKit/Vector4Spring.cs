using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011C1 RID: 4545
	public struct Vector4Spring
	{
		// Token: 0x060072AB RID: 29355 RVA: 0x00259987 File Offset: 0x00257B87
		public void Reset()
		{
			this.Value = Vector4.zero;
			this.Velocity = Vector4.zero;
		}

		// Token: 0x060072AC RID: 29356 RVA: 0x0025999F File Offset: 0x00257B9F
		public void Reset(Vector4 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector4.zero;
		}

		// Token: 0x060072AD RID: 29357 RVA: 0x002599B3 File Offset: 0x00257BB3
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x060072AE RID: 29358 RVA: 0x002599C4 File Offset: 0x00257BC4
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

		// Token: 0x060072AF RID: 29359 RVA: 0x00259AC0 File Offset: 0x00257CC0
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

		// Token: 0x060072B0 RID: 29360 RVA: 0x00259B0C File Offset: 0x00257D0C
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

		// Token: 0x040082F0 RID: 33520
		public static readonly int Stride = 32;

		// Token: 0x040082F1 RID: 33521
		public Vector4 Value;

		// Token: 0x040082F2 RID: 33522
		public Vector4 Velocity;
	}
}
