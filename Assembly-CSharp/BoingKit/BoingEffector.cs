using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02001185 RID: 4485
	public class BoingEffector : BoingBase
	{
		// Token: 0x17000A93 RID: 2707
		// (get) Token: 0x0600712F RID: 28975 RVA: 0x002518F1 File Offset: 0x0024FAF1
		public Vector3 LinearVelocity
		{
			get
			{
				return this.m_linearVelocity;
			}
		}

		// Token: 0x17000A94 RID: 2708
		// (get) Token: 0x06007130 RID: 28976 RVA: 0x002518F9 File Offset: 0x0024FAF9
		public float LinearSpeed
		{
			get
			{
				return this.m_linearVelocity.magnitude;
			}
		}

		// Token: 0x06007131 RID: 28977 RVA: 0x00251906 File Offset: 0x0024FB06
		public void OnEnable()
		{
			this.m_currPosition = base.transform.position;
			this.m_prevPosition = base.transform.position;
			this.m_linearVelocity = Vector3.zero;
			BoingManager.Register(this);
		}

		// Token: 0x06007132 RID: 28978 RVA: 0x0025193B File Offset: 0x0024FB3B
		public void OnDisable()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x06007133 RID: 28979 RVA: 0x00251944 File Offset: 0x0024FB44
		public void Update()
		{
			float deltaTime = Time.deltaTime;
			if (deltaTime < MathUtil.Epsilon)
			{
				return;
			}
			this.m_linearVelocity = (base.transform.position - this.m_prevPosition) / deltaTime;
			this.m_prevPosition = this.m_currPosition;
			this.m_currPosition = base.transform.position;
		}

		// Token: 0x06007134 RID: 28980 RVA: 0x002519A0 File Offset: 0x0024FBA0
		public void OnDrawGizmosSelected()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (this.FullEffectRadiusRatio < 1f)
			{
				Gizmos.color = new Color(1f, 0.5f, 0.2f, 0.4f);
				Gizmos.DrawWireSphere(base.transform.position, this.Radius);
			}
			Gizmos.color = new Color(1f, 0.5f, 0.2f, 1f);
			Gizmos.DrawWireSphere(base.transform.position, this.Radius * this.FullEffectRadiusRatio);
		}

		// Token: 0x040081A3 RID: 33187
		[Header("Metrics")]
		[Range(0f, 20f)]
		[Tooltip("Maximum radius of influence.")]
		public float Radius = 3f;

		// Token: 0x040081A4 RID: 33188
		[Range(0f, 1f)]
		[Tooltip("Fraction of Radius past which influence begins decaying gradually to zero exactly at Radius.\n\ne.g. With a Radius of 10.0 and FullEffectRadiusRatio of 0.5, reactors within distance of 5.0 will be fully influenced, reactors at distance of 7.5 will experience 50% influence, and reactors past distance of 10.0 will not be influenced at all.")]
		public float FullEffectRadiusRatio = 0.5f;

		// Token: 0x040081A5 RID: 33189
		[Header("Dynamics")]
		[Range(0f, 100f)]
		[Tooltip("Speed of this effector at which impulse effects will be at maximum strength.\n\ne.g. With a MaxImpulseSpeed of 10.0 and an effector traveling at speed of 4.0, impulse effects will be at 40% maximum strength.")]
		public float MaxImpulseSpeed = 5f;

		// Token: 0x040081A6 RID: 33190
		[Tooltip("This affects impulse-related effects.\n\nIf checked, continuous motion will be simulated between frames. This means even if an effector \"teleports\" by moving a huge distance between frames, the effector will still affect all reactors caught on the effector's path in between frames, not just the reactors around the effector's discrete positions at different frames.")]
		public bool ContinuousMotion;

		// Token: 0x040081A7 RID: 33191
		[Header("Position Effect")]
		[Range(-10f, 10f)]
		[Tooltip("Distance to push away reactors at maximum influence.\n\ne.g. With a MoveDistance of 2.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, and a reactor at distance of 7.5 away from effector, the reactor will be pushed away to 50% of maximum influence, i.e. 50% of MoveDistance, which is a distance of 1.0 away from the effector.")]
		public float MoveDistance = 0.5f;

		// Token: 0x040081A8 RID: 33192
		[Range(-200f, 200f)]
		[Tooltip("Under maximum impulse influence (within distance of Radius * FullEffectRadiusRatio and with effector moving at speed faster or equal to MaxImpulaseSpeed), a reactor's movement speed will be maintained to be at least as fast as LinearImpulse (unit: distance per second) in the direction of effector's movement direction.\n\ne.g. With a LinearImpulse of 2.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, and a reactor at distance of 7.5 away from effector, the reactor's movement speed in the direction of effector's movement direction will be maintained to be at least 50% of LinearImpulse, which is 1.0 per second.")]
		public float LinearImpulse = 5f;

		// Token: 0x040081A9 RID: 33193
		[Header("Rotation Effect")]
		[Range(-180f, 180f)]
		[Tooltip("Angle (in degrees) to rotate reactors at maximum influence. The rotation will point reactors' up vectors (defined individually in the reactor component) away from the effector.\n\ne.g. With a RotationAngle of 20.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, and a reactor at distance of 7.5 away from effector, the reactor will be rotated to 50% of maximum influence, i.e. 50% of RotationAngle, which is 10 degrees.")]
		public float RotationAngle = 20f;

		// Token: 0x040081AA RID: 33194
		[Range(-2000f, 2000f)]
		[Tooltip("Under maximum impulse influence (within distance of Radius * FullEffectRadiusRatio and with effector moving at speed faster or equal to MaxImpulaseSpeed), a reactor's rotation speed will be maintained to be at least as fast as AngularImpulse (unit: degrees per second) in the direction of effector's movement direction, i.e. the reactor's up vector will be pulled in the direction of effector's movement direction.\n\ne.g. With a AngularImpulse of 20.0, a Radius of 10.0, a FullEffectRadiusRatio of 0.5, and a reactor at distance of 7.5 away from effector, the reactor's rotation speed in the direction of effector's movement direction will be maintained to be at least 50% of AngularImpulse, which is 10.0 degrees per second.")]
		public float AngularImpulse = 400f;

		// Token: 0x040081AB RID: 33195
		[Header("Debug")]
		[Tooltip("If checked, gizmos of reactor fields affected by this effector will be drawn.")]
		public bool DrawAffectedReactorFieldGizmos;

		// Token: 0x040081AC RID: 33196
		private Vector3 m_currPosition;

		// Token: 0x040081AD RID: 33197
		private Vector3 m_prevPosition;

		// Token: 0x040081AE RID: 33198
		private Vector3 m_linearVelocity;

		// Token: 0x02001186 RID: 4486
		public struct Params
		{
			// Token: 0x06007136 RID: 28982 RVA: 0x00251A94 File Offset: 0x0024FC94
			public Params(BoingEffector effector)
			{
				this.Bits = default(Bits32);
				this.Bits.SetBit(0, effector.ContinuousMotion);
				float num = (effector.MaxImpulseSpeed > MathUtil.Epsilon) ? Mathf.Min(1f, effector.LinearSpeed / effector.MaxImpulseSpeed) : 1f;
				this.PrevPosition = effector.m_prevPosition;
				this.CurrPosition = effector.m_currPosition;
				this.LinearVelocityDir = VectorUtil.NormalizeSafe(effector.LinearVelocity, Vector3.zero);
				this.Radius = effector.Radius;
				this.FullEffectRadius = this.Radius * effector.FullEffectRadiusRatio;
				this.MoveDistance = effector.MoveDistance;
				this.LinearImpulse = num * effector.LinearImpulse;
				this.RotateAngle = effector.RotationAngle * MathUtil.Deg2Rad;
				this.AngularImpulse = num * effector.AngularImpulse * MathUtil.Deg2Rad;
				this.m_padding0 = 0f;
				this.m_padding1 = 0f;
				this.m_padding2 = 0f;
				this.m_padding3 = 0;
			}

			// Token: 0x06007137 RID: 28983 RVA: 0x00251BAF File Offset: 0x0024FDAF
			public void Fill(BoingEffector effector)
			{
				this = new BoingEffector.Params(effector);
			}

			// Token: 0x06007138 RID: 28984 RVA: 0x00251BC0 File Offset: 0x0024FDC0
			private void SuppressWarnings()
			{
				this.m_padding0 = 0f;
				this.m_padding1 = 0f;
				this.m_padding2 = 0f;
				this.m_padding3 = 0;
				this.m_padding0 = this.m_padding1;
				this.m_padding1 = this.m_padding2;
				this.m_padding2 = (float)this.m_padding3;
				this.m_padding3 = (int)this.m_padding0;
			}

			// Token: 0x040081AF RID: 33199
			public static readonly int Stride = 80;

			// Token: 0x040081B0 RID: 33200
			public Vector3 PrevPosition;

			// Token: 0x040081B1 RID: 33201
			private float m_padding0;

			// Token: 0x040081B2 RID: 33202
			public Vector3 CurrPosition;

			// Token: 0x040081B3 RID: 33203
			private float m_padding1;

			// Token: 0x040081B4 RID: 33204
			public Vector3 LinearVelocityDir;

			// Token: 0x040081B5 RID: 33205
			private float m_padding2;

			// Token: 0x040081B6 RID: 33206
			public float Radius;

			// Token: 0x040081B7 RID: 33207
			public float FullEffectRadius;

			// Token: 0x040081B8 RID: 33208
			public float MoveDistance;

			// Token: 0x040081B9 RID: 33209
			public float LinearImpulse;

			// Token: 0x040081BA RID: 33210
			public float RotateAngle;

			// Token: 0x040081BB RID: 33211
			public float AngularImpulse;

			// Token: 0x040081BC RID: 33212
			public Bits32 Bits;

			// Token: 0x040081BD RID: 33213
			private int m_padding3;
		}
	}
}
