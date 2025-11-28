using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200117B RID: 4475
	public class UFOController : MonoBehaviour
	{
		// Token: 0x060070F2 RID: 28914 RVA: 0x0024F4B4 File Offset: 0x0024D6B4
		private void Start()
		{
			this.m_linearVelocity = Vector3.zero;
			this.m_angularVelocity = 0f;
			this.m_yawAngle = base.transform.rotation.eulerAngles.y * MathUtil.Deg2Rad;
			this.m_hoverCenter = base.transform.position;
			this.m_hoverPhase = 0f;
			this.m_motorAngle = 0f;
			if (this.Eyes != null)
			{
				this.m_eyeInitScale = this.Eyes.localScale;
				this.m_eyeInitPositionLs = this.Eyes.localPosition;
				this.m_blinkTimer = this.BlinkInterval + Random.Range(1f, 2f);
				this.m_lastBlinkWasDouble = false;
				this.m_eyeScaleSpring.Reset(this.m_eyeInitScale);
				this.m_eyePositionLsSpring.Reset(this.m_eyeInitPositionLs);
			}
		}

		// Token: 0x060070F3 RID: 28915 RVA: 0x0024F597 File Offset: 0x0024D797
		private void OnEnable()
		{
			this.Start();
		}

		// Token: 0x060070F4 RID: 28916 RVA: 0x0024F5A0 File Offset: 0x0024D7A0
		private void FixedUpdate()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			Vector3 vector = Vector3.zero;
			if (Input.GetKey(119))
			{
				vector += Vector3.forward;
			}
			if (Input.GetKey(115))
			{
				vector += Vector3.back;
			}
			if (Input.GetKey(97))
			{
				vector += Vector3.left;
			}
			if (Input.GetKey(100))
			{
				vector += Vector3.right;
			}
			if (Input.GetKey(114))
			{
				vector += Vector3.up;
			}
			if (Input.GetKey(102))
			{
				vector += Vector3.down;
			}
			if (vector.sqrMagnitude > MathUtil.Epsilon)
			{
				vector = vector.normalized * this.LinearThrust;
				this.m_linearVelocity += vector * fixedDeltaTime;
				this.m_linearVelocity = VectorUtil.ClampLength(this.m_linearVelocity, 0f, this.MaxLinearSpeed);
			}
			else
			{
				this.m_linearVelocity = VectorUtil.ClampLength(this.m_linearVelocity, 0f, Mathf.Max(0f, this.m_linearVelocity.magnitude - this.LinearDrag * fixedDeltaTime));
			}
			float magnitude = this.m_linearVelocity.magnitude;
			float num = magnitude * MathUtil.InvSafe(this.MaxLinearSpeed);
			Quaternion quaternion = Quaternion.identity;
			float num2 = 0f;
			if (magnitude > MathUtil.Epsilon)
			{
				Vector3 linearVelocity = this.m_linearVelocity;
				linearVelocity.y = 0f;
				float num3 = (this.m_linearVelocity.magnitude > 0.01f) ? (1f - Mathf.Clamp01(Mathf.Abs(this.m_linearVelocity.y) / this.m_linearVelocity.magnitude)) : 0f;
				num2 = Mathf.Min(1f, magnitude / Mathf.Max(MathUtil.Epsilon, this.MaxLinearSpeed)) * num3;
				Vector3 normalized = Vector3.Cross(Vector3.up, linearVelocity).normalized;
				float angle = this.Tilt * MathUtil.Deg2Rad * num2;
				quaternion = QuaternionUtil.AxisAngle(normalized, angle);
			}
			float num4 = 0f;
			if (Input.GetKey(113))
			{
				num4 -= 1f;
			}
			if (Input.GetKey(101))
			{
				num4 += 1f;
			}
			bool key = Input.GetKey(306);
			if (Mathf.Abs(num4) > MathUtil.Epsilon)
			{
				float num5 = this.MaxAngularSpeed * (key ? 2.5f : 1f);
				num4 *= this.AngularThrust * MathUtil.Deg2Rad;
				this.m_angularVelocity += num4 * fixedDeltaTime;
				this.m_angularVelocity = Mathf.Clamp(this.m_angularVelocity, -num5 * MathUtil.Deg2Rad, num5 * MathUtil.Deg2Rad);
			}
			else
			{
				this.m_angularVelocity -= Mathf.Sign(this.m_angularVelocity) * Mathf.Min(Mathf.Abs(this.m_angularVelocity), this.AngularDrag * MathUtil.Deg2Rad * fixedDeltaTime);
			}
			this.m_yawAngle += this.m_angularVelocity * fixedDeltaTime;
			Quaternion quaternion2 = QuaternionUtil.AxisAngle(Vector3.up, this.m_yawAngle);
			this.m_hoverCenter += this.m_linearVelocity * fixedDeltaTime;
			this.m_hoverPhase += Time.deltaTime;
			Vector3 vector2 = 0.05f * Mathf.Sin(1.37f * this.m_hoverPhase) * Vector3.right + 0.05f * Mathf.Sin(1.93f * this.m_hoverPhase + 1.234f) * Vector3.forward + 0.04f * Mathf.Sin(0.97f * this.m_hoverPhase + 4.321f) * Vector3.up;
			vector2 *= this.Hover;
			Quaternion quaternion3 = Quaternion.FromToRotation(Vector3.up, vector2 + Vector3.up);
			base.transform.position = this.m_hoverCenter + vector2;
			base.transform.rotation = quaternion * quaternion2 * quaternion3;
			if (this.Motor != null)
			{
				float num6 = Mathf.Lerp(this.MotorBaseAngularSpeed, this.MotorMaxAngularSpeed, num2);
				this.m_motorAngle += num6 * MathUtil.Deg2Rad * fixedDeltaTime;
				this.Motor.localRotation = QuaternionUtil.AxisAngle(Vector3.up, this.m_motorAngle - this.m_yawAngle);
			}
			if (this.BubbleEmitter != null)
			{
				this.BubbleEmitter.emission.rateOverTime = Mathf.Lerp(this.BubbleBaseEmissionRate, this.BubbleMaxEmissionRate, num);
			}
			if (this.Eyes != null)
			{
				this.m_blinkTimer -= fixedDeltaTime;
				if (this.m_blinkTimer <= 0f)
				{
					bool flag = !this.m_lastBlinkWasDouble && Random.Range(0f, 1f) > 0.75f;
					this.m_blinkTimer = (flag ? 0.2f : (this.BlinkInterval + Random.Range(1f, 2f)));
					this.m_lastBlinkWasDouble = flag;
					this.m_eyeScaleSpring.Value.y = 0f;
					this.m_eyePositionLsSpring.Value.y = this.m_eyePositionLsSpring.Value.y - 0.025f;
				}
				this.Eyes.localScale = this.m_eyeScaleSpring.TrackDampingRatio(this.m_eyeInitScale, 30f, 0.8f, fixedDeltaTime);
				this.Eyes.localPosition = this.m_eyePositionLsSpring.TrackDampingRatio(this.m_eyeInitPositionLs, 30f, 0.8f, fixedDeltaTime);
			}
		}

		// Token: 0x04008107 RID: 33031
		public float LinearThrust = 3f;

		// Token: 0x04008108 RID: 33032
		public float MaxLinearSpeed = 2.5f;

		// Token: 0x04008109 RID: 33033
		public float LinearDrag = 4f;

		// Token: 0x0400810A RID: 33034
		public float Tilt = 15f;

		// Token: 0x0400810B RID: 33035
		public float AngularThrust = 30f;

		// Token: 0x0400810C RID: 33036
		public float MaxAngularSpeed = 30f;

		// Token: 0x0400810D RID: 33037
		public float AngularDrag = 30f;

		// Token: 0x0400810E RID: 33038
		[Range(0f, 1f)]
		public float Hover = 1f;

		// Token: 0x0400810F RID: 33039
		public Transform Eyes;

		// Token: 0x04008110 RID: 33040
		public float BlinkInterval = 5f;

		// Token: 0x04008111 RID: 33041
		private float m_blinkTimer;

		// Token: 0x04008112 RID: 33042
		private bool m_lastBlinkWasDouble;

		// Token: 0x04008113 RID: 33043
		private Vector3 m_eyeInitScale;

		// Token: 0x04008114 RID: 33044
		private Vector3 m_eyeInitPositionLs;

		// Token: 0x04008115 RID: 33045
		private Vector3Spring m_eyeScaleSpring;

		// Token: 0x04008116 RID: 33046
		private Vector3Spring m_eyePositionLsSpring;

		// Token: 0x04008117 RID: 33047
		public Transform Motor;

		// Token: 0x04008118 RID: 33048
		public float MotorBaseAngularSpeed = 10f;

		// Token: 0x04008119 RID: 33049
		public float MotorMaxAngularSpeed = 10f;

		// Token: 0x0400811A RID: 33050
		public ParticleSystem BubbleEmitter;

		// Token: 0x0400811B RID: 33051
		public float BubbleBaseEmissionRate = 10f;

		// Token: 0x0400811C RID: 33052
		public float BubbleMaxEmissionRate = 10f;

		// Token: 0x0400811D RID: 33053
		private Vector3 m_linearVelocity;

		// Token: 0x0400811E RID: 33054
		private float m_angularVelocity;

		// Token: 0x0400811F RID: 33055
		private float m_yawAngle;

		// Token: 0x04008120 RID: 33056
		private Vector3 m_hoverCenter;

		// Token: 0x04008121 RID: 33057
		private float m_hoverPhase;

		// Token: 0x04008122 RID: 33058
		private float m_motorAngle;
	}
}
