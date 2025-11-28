using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011BF RID: 4543
	public struct Vector2Spring
	{
		// Token: 0x0600729D RID: 29341 RVA: 0x002595FE File Offset: 0x002577FE
		public void Reset()
		{
			this.Value = Vector2.zero;
			this.Velocity = Vector2.zero;
		}

		// Token: 0x0600729E RID: 29342 RVA: 0x00259616 File Offset: 0x00257816
		public void Reset(Vector2 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector2.zero;
		}

		// Token: 0x0600729F RID: 29343 RVA: 0x0025962A File Offset: 0x0025782A
		public void Reset(Vector2 initValue, Vector2 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x060072A0 RID: 29344 RVA: 0x0025963C File Offset: 0x0025783C
		public Vector2 TrackDampingRatio(Vector2 targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = Vector2.zero;
				return this.Value;
			}
			Vector2 vector = targetValue - this.Value;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector2 vector2 = num * this.Value + deltaTime * this.Velocity + num4 * targetValue;
			Vector2 vector3 = this.Velocity + num3 * vector;
			this.Velocity = vector3 * num5;
			this.Value = vector2 * num5;
			if (this.Velocity.magnitude < MathUtil.Epsilon && vector.magnitude < MathUtil.Epsilon)
			{
				this.Velocity = Vector2.zero;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x060072A1 RID: 29345 RVA: 0x00259738 File Offset: 0x00257938
		public Vector2 TrackHalfLife(Vector2 targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector2.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float dampingRatio = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, dampingRatio, deltaTime);
		}

		// Token: 0x060072A2 RID: 29346 RVA: 0x00259784 File Offset: 0x00257984
		public Vector2 TrackExponential(Vector2 targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector2.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float angularFrequency = 0.6931472f / halfLife;
			float dampingRatio = 1f;
			return this.TrackDampingRatio(targetValue, angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x040082E8 RID: 33512
		public static readonly int Stride = 16;

		// Token: 0x040082E9 RID: 33513
		public Vector2 Value;

		// Token: 0x040082EA RID: 33514
		public Vector2 Velocity;
	}
}
