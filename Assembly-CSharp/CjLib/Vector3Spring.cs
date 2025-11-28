using System;
using UnityEngine;

namespace CjLib
{
	// Token: 0x02001170 RID: 4464
	public struct Vector3Spring
	{
		// Token: 0x060070A2 RID: 28834 RVA: 0x0024E20F File Offset: 0x0024C40F
		public void Reset()
		{
			this.Value = Vector3.zero;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x060070A3 RID: 28835 RVA: 0x0024E227 File Offset: 0x0024C427
		public void Reset(Vector3 initValue)
		{
			this.Value = initValue;
			this.Velocity = Vector3.zero;
		}

		// Token: 0x060070A4 RID: 28836 RVA: 0x0024E23B File Offset: 0x0024C43B
		public void Reset(Vector3 initValue, Vector3 initVelocity)
		{
			this.Value = initValue;
			this.Velocity = initVelocity;
		}

		// Token: 0x060070A5 RID: 28837 RVA: 0x0024E24C File Offset: 0x0024C44C
		public Vector3 TrackDampingRatio(Vector3 targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				return this.Value;
			}
			Vector3 vector = targetValue - this.Value;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector3 vector2 = num * this.Value + deltaTime * this.Velocity + num4 * targetValue;
			Vector3 vector3 = this.Velocity + num3 * vector;
			this.Velocity = vector3 * num5;
			this.Value = vector2 * num5;
			if (this.Velocity.magnitude < MathUtil.Epsilon && vector.magnitude < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
			}
			return this.Value;
		}

		// Token: 0x060070A6 RID: 28838 RVA: 0x0024E348 File Offset: 0x0024C548
		public Vector3 TrackHalfLife(Vector3 targetValue, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float dampingRatio = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValue, num, dampingRatio, deltaTime);
		}

		// Token: 0x060070A7 RID: 28839 RVA: 0x0024E394 File Offset: 0x0024C594
		public Vector3 TrackExponential(Vector3 targetValue, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.Velocity = Vector3.zero;
				this.Value = targetValue;
				return this.Value;
			}
			float angularFrequency = 0.6931472f / halfLife;
			float dampingRatio = 1f;
			return this.TrackDampingRatio(targetValue, angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x040080D9 RID: 32985
		public static readonly int Stride = 32;

		// Token: 0x040080DA RID: 32986
		public Vector3 Value;

		// Token: 0x040080DB RID: 32987
		private float m_padding0;

		// Token: 0x040080DC RID: 32988
		public Vector3 Velocity;

		// Token: 0x040080DD RID: 32989
		private float m_padding1;
	}
}
