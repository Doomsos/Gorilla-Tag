using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200109C RID: 4252
	public class RCHelicopter : RCVehicle
	{
		// Token: 0x06006A5E RID: 27230 RVA: 0x0022D108 File Offset: 0x0022B308
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.verticalPropeller.localRotation = this.verticalPropellerBaseRotation;
			this.turnPropeller.localRotation = this.turnPropellerBaseRotation;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006A5F RID: 27231 RVA: 0x0022D164 File Offset: 0x0022B364
		protected override void Awake()
		{
			base.Awake();
			this.verticalPropellerBaseRotation = this.verticalPropeller.localRotation;
			this.turnPropellerBaseRotation = this.turnPropeller.localRotation;
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
		}

		// Token: 0x06006A60 RID: 27232 RVA: 0x0022D1D4 File Offset: 0x0022B3D4
		protected override void SharedUpdate(float dt)
		{
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = Mathf.Lerp(this.mainPropellerSpinRateRange.x, this.mainPropellerSpinRateRange.y, this.activeInput.trigger);
				this.verticalPropeller.Rotate(new Vector3(0f, num * dt, 0f), 1);
				this.turnPropeller.Rotate(new Vector3(this.activeInput.joystick.x * this.backPropellerSpinRate * dt, 0f, 0f), 1);
			}
		}

		// Token: 0x06006A61 RID: 27233 RVA: 0x0022D264 File Offset: 0x0022B464
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			float num = this.activeInput.joystick.x * this.maxTurnRate;
			this.turnRate = Mathf.MoveTowards(this.turnRate, num, this.turnAccel * fixedDeltaTime);
			float num2 = this.activeInput.joystick.y * this.maxHorizontalSpeed;
			float num3 = Mathf.Sign(this.activeInput.joystick.y) * Mathf.Lerp(0f, this.maxHorizontalTiltAngle, Mathf.Abs(this.activeInput.joystick.y));
			base.transform.rotation = Quaternion.Euler(new Vector3(num3, this.turnAccel, 0f));
			float num4 = Mathf.Abs(num2);
			Vector3 normalized = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up).normalized;
			float num5 = Vector3.Dot(normalized, linearVelocity);
			if (num4 > 0.01f && ((num2 > 0f && num2 > num5) || (num2 < 0f && num2 < num5)))
			{
				this.rb.AddForce(normalized * Mathf.Sign(num2) * this.horizontalAccel * fixedDeltaTime * this.rb.mass, 0);
			}
			float num6 = this.activeInput.trigger * this.maxAscendSpeed;
			if (num6 > 0.01f && linearVelocity.y < num6)
			{
				this.rb.AddForce(Vector3.up * this.ascendAccel * this.rb.mass, 0);
			}
			if (this.rb.useGravity)
			{
				this.rb.AddForce(-Physics.gravity * this.gravityCompensation * this.rb.mass, 0);
			}
		}

		// Token: 0x06006A62 RID: 27234 RVA: 0x0022D462 File Offset: 0x0022B662
		private void OnTriggerEnter(Collider other)
		{
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
			}
		}

		// Token: 0x04007A3A RID: 31290
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04007A3B RID: 31291
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04007A3C RID: 31292
		[SerializeField]
		private float gravityCompensation = 0.5f;

		// Token: 0x04007A3D RID: 31293
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04007A3E RID: 31294
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04007A3F RID: 31295
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04007A40 RID: 31296
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x04007A41 RID: 31297
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x04007A42 RID: 31298
		[SerializeField]
		private Vector2 mainPropellerSpinRateRange = new Vector2(3f, 15f);

		// Token: 0x04007A43 RID: 31299
		[SerializeField]
		private float backPropellerSpinRate = 5f;

		// Token: 0x04007A44 RID: 31300
		[SerializeField]
		private Transform verticalPropeller;

		// Token: 0x04007A45 RID: 31301
		[SerializeField]
		private Transform turnPropeller;

		// Token: 0x04007A46 RID: 31302
		private Quaternion verticalPropellerBaseRotation;

		// Token: 0x04007A47 RID: 31303
		private Quaternion turnPropellerBaseRotation;

		// Token: 0x04007A48 RID: 31304
		private float turnRate;

		// Token: 0x04007A49 RID: 31305
		private float ascendAccel;

		// Token: 0x04007A4A RID: 31306
		private float turnAccel;

		// Token: 0x04007A4B RID: 31307
		private float horizontalAccel;
	}
}
