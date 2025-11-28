using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200116E RID: 4462
	public struct FloatSpring
	{
		// Token: 0x06007094 RID: 28820 RVA: 0x0024DE91 File Offset: 0x0024C091
		public void Reset()
		{
			this.Value = 0f;
			this.Velocity = 0f;
		}

		// Token: 0x06007095 RID: 28821 RVA: 0x0024DEA9 File Offset: 0x0024C0A9
		public void Reset(float initValue)
		{
			this.Value = initValue;
			this.Velocity = 0f;
		}

		// Token: 0x06007096 RID: 28822 RVA: 0x0024DEBD File Offset: 0x0024C0BD
		public void Reset(float initValue, float initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x06007097 RID: 28823 RVA: 0x0024DED0 File Offset: 0x0024C0D0
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

		// Token: 0x06007098 RID: 28824 RVA: 0x0024DFA0 File Offset: 0x0024C1A0
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

		// Token: 0x06007099 RID: 28825 RVA: 0x0024DFEC File Offset: 0x0024C1EC
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

		// Token: 0x040080D3 RID: 32979
		public static readonly int Stride = 8;

		// Token: 0x040080D4 RID: 32980
		public float Value;

		// Token: 0x040080D5 RID: 32981
		public float Velocity;
	}
}
