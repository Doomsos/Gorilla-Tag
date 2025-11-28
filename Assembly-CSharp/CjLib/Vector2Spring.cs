using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x0200116F RID: 4463
	public struct Vector2Spring
	{
		// Token: 0x0600709B RID: 28827 RVA: 0x0024E05A File Offset: 0x0024C25A
		public void Reset()
		{
			this.Value = Vector2.zero;
			this.Velocity = Vector2.zero;
		}

		// Token: 0x0600709C RID: 28828 RVA: 0x0024E072 File Offset: 0x0024C272
		public void Reset(Vector2 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector2.zero;
		}

		// Token: 0x0600709D RID: 28829 RVA: 0x0024E086 File Offset: 0x0024C286
		public void Reset(Vector2 initValue, Vector2 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x0600709E RID: 28830 RVA: 0x0024E098 File Offset: 0x0024C298
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

		// Token: 0x0600709F RID: 28831 RVA: 0x0024E194 File Offset: 0x0024C394
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

		// Token: 0x060070A0 RID: 28832 RVA: 0x0024E1E0 File Offset: 0x0024C3E0
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

		// Token: 0x040080D6 RID: 32982
		public static readonly int Stride = 16;

		// Token: 0x040080D7 RID: 32983
		public Vector2 Value;

		// Token: 0x040080D8 RID: 32984
		public Vector2 Velocity;
	}
}
