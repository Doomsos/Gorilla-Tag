using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011BE RID: 4542
	public struct FloatSpring
	{
		// Token: 0x06007296 RID: 29334 RVA: 0x00259457 File Offset: 0x00257657
		public void Reset()
		{
			this.Value = 0f;
			this.Velocity = 0f;
		}

		// Token: 0x06007297 RID: 29335 RVA: 0x0025946F File Offset: 0x0025766F
		public void Reset(float initValue)
		{
			this.Value = initValue;
			this.Velocity = 0f;
		}

		// Token: 0x06007298 RID: 29336 RVA: 0x00259483 File Offset: 0x00257683
		public void Reset(float initValue, float initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x06007299 RID: 29337 RVA: 0x00259494 File Offset: 0x00257694
		public float TrackDampingRatio(float targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = 0f;
				return this.Value;
			}
			float num = targetValue - this.Value;
			float num2 = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num3 = angularFrequency * angularFrequency;
			float num4 = deltaTime * num3;
			float num5 = deltaTime * num4;
			float num6 = 1f / (num2 + num5);
			float num7 = num2 * this.Value + deltaTime * this.Velocity + num5 * targetValue;
			float num8 = this.Velocity + num4 * num;
			this.Velocity = num8 * num6;
			this.Value = num7 * num6;
			if (Mathf.Abs(this.Velocity) < MathUtil.Epsilon && Mathf.Abs(num) < MathUtil.Epsilon)
			{
				this.Velocity = 0f;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x0600729A RID: 29338 RVA: 0x00259564 File Offset: 0x00257764
		public float TrackHalfLife(float targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = 0f;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float dampingRatio = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, dampingRatio, deltaTime);
		}

		// Token: 0x0600729B RID: 29339 RVA: 0x002595B0 File Offset: 0x002577B0
		public float TrackExponential(float targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = 0f;
				this.Value = targetValue;
				return this.Value;
			}
			float angularFrequency = 0.6931472f / halfLife;
			float dampingRatio = 1f;
			return this.TrackDampingRatio(targetValue, angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x040082E5 RID: 33509
		public static readonly int Stride = 8;

		// Token: 0x040082E6 RID: 33510
		public float Value;

		// Token: 0x040082E7 RID: 33511
		public float Velocity;
	}
}
