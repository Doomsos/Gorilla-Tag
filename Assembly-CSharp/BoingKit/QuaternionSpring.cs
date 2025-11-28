using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x020011C2 RID: 4546
	public struct QuaternionSpring
	{
		// Token: 0x17000AB5 RID: 2741
		// (get) Token: 0x060072B2 RID: 29362 RVA: 0x00259B5B File Offset: 0x00257D5B
		// (set) Token: 0x060072B3 RID: 29363 RVA: 0x00259B69 File Offset: 0x00257D69
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

		// Token: 0x060072B4 RID: 29364 RVA: 0x00259B77 File Offset: 0x00257D77
		public void Reset()
		{
			this.ValueVec = QuaternionUtil.ToVector4(Quaternion.identity);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x060072B5 RID: 29365 RVA: 0x00259B94 File Offset: 0x00257D94
		public void Reset(Vector4 initValue)
		{
			this.ValueVec = initValue;
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x060072B6 RID: 29366 RVA: 0x00259BA8 File Offset: 0x00257DA8
		public void Reset(Vector4 initValue, Vector4 initVelocity)
		{
			this.ValueVec = initValue;
			this.VelocityVec = initVelocity;
		}

		// Token: 0x060072B7 RID: 29367 RVA: 0x00259BB8 File Offset: 0x00257DB8
		public void Reset(Quaternion initValue)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = Vector4.zero;
		}

		// Token: 0x060072B8 RID: 29368 RVA: 0x00259BD1 File Offset: 0x00257DD1
		public void Reset(Quaternion initValue, Quaternion initVelocity)
		{
			this.ValueVec = QuaternionUtil.ToVector4(initValue);
			this.VelocityVec = QuaternionUtil.ToVector4(initVelocity);
		}

		// Token: 0x060072B9 RID: 29369 RVA: 0x00259BEC File Offset: 0x00257DEC
		public Quaternion TrackDampingRatio(Vector4 targetValueVec, float angularFrequency, float dampingRatio, float deltaTime)
		{
			if (angularFrequency < MathUtil.Epsilon)
			{
				this.VelocityVec = QuaternionUtil.ToVector4(Quaternion.identity);
				return QuaternionUtil.FromVector4(this.ValueVec, true);
			}
			if (Vector4.Dot(this.ValueVec, targetValueVec) < 0f)
			{
				targetValueVec = -targetValueVec;
			}
			Vector4 vector = targetValueVec - this.ValueVec;
			float num = 1f + 2f * deltaTime * dampingRatio * angularFrequency;
			float num2 = angularFrequency * angularFrequency;
			float num3 = deltaTime * num2;
			float num4 = deltaTime * num3;
			float num5 = 1f / (num + num4);
			Vector4 vector2 = num * this.ValueVec + deltaTime * this.VelocityVec + num4 * targetValueVec;
			Vector4 vector3 = this.VelocityVec + num3 * vector;
			this.VelocityVec = vector3 * num5;
			this.ValueVec = vector2 * num5;
			if (this.VelocityVec.magnitude < MathUtil.Epsilon && vector.magnitude < MathUtil.Epsilon)
			{
				this.VelocityVec = Vector4.zero;
				this.ValueVec = targetValueVec;
			}
			return QuaternionUtil.FromVector4(this.ValueVec, true);
		}

		// Token: 0x060072BA RID: 29370 RVA: 0x00259D11 File Offset: 0x00257F11
		public Quaternion TrackDampingRatio(Quaternion targetValue, float angularFrequency, float dampingRatio, float deltaTime)
		{
			return this.TrackDampingRatio(QuaternionUtil.ToVector4(targetValue), angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x060072BB RID: 29371 RVA: 0x00259D24 File Offset: 0x00257F24
		public Quaternion TrackHalfLife(Vector4 targetValueVec, float frequencyHz, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = Vector4.zero;
				this.ValueVec = targetValueVec;
				return QuaternionUtil.FromVector4(targetValueVec, true);
			}
			float num = frequencyHz * MathUtil.TwoPi;
			float dampingRatio = 0.6931472f / (num * halfLife);
			return this.TrackDampingRatio(targetValueVec, num, dampingRatio, deltaTime);
		}

		// Token: 0x060072BC RID: 29372 RVA: 0x00259D70 File Offset: 0x00257F70
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

		// Token: 0x060072BD RID: 29373 RVA: 0x00259DC0 File Offset: 0x00257FC0
		public Quaternion TrackExponential(Vector4 targetValueVec, float halfLife, float deltaTime)
		{
			if (halfLife < MathUtil.Epsilon)
			{
				this.VelocityVec = Vector4.zero;
				this.ValueVec = targetValueVec;
				return QuaternionUtil.FromVector4(targetValueVec, true);
			}
			float angularFrequency = 0.6931472f / halfLife;
			float dampingRatio = 1f;
			return this.TrackDampingRatio(targetValueVec, angularFrequency, dampingRatio, deltaTime);
		}

		// Token: 0x060072BE RID: 29374 RVA: 0x00259E08 File Offset: 0x00258008
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

		// Token: 0x040082F3 RID: 33523
		public static readonly int Stride = 32;

		// Token: 0x040082F4 RID: 33524
		public Vector4 ValueVec;

		// Token: 0x040082F5 RID: 33525
		public Vector4 VelocityVec;
	}
}
