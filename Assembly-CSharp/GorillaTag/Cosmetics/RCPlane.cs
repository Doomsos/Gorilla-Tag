using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200109D RID: 4253
	public class RCPlane : RCVehicle
	{
		// Token: 0x06006A64 RID: 27236 RVA: 0x0022D510 File Offset: 0x0022B710
		protected override void Awake()
		{
			base.Awake();
			this.pitchAccelMinMax.x = this.pitchVelocityTargetMinMax.x / this.pitchVelocityRampTimeMinMax.x;
			this.pitchAccelMinMax.y = this.pitchVelocityTargetMinMax.y / this.pitchVelocityRampTimeMinMax.y;
			this.rollAccel = this.rollVelocityTarget / this.rollVelocityRampTime;
			this.thrustAccel = this.thrustVelocityTarget / this.thrustAccelTime;
		}

		// Token: 0x06006A65 RID: 27237 RVA: 0x0022D590 File Offset: 0x0022B790
		protected override void AuthorityBeginMobilization()
		{
			base.AuthorityBeginMobilization();
			float x = base.transform.lossyScale.x;
			this.rb.linearVelocity = base.transform.forward * this.initialSpeed * x;
		}

		// Token: 0x06006A66 RID: 27238 RVA: 0x0022D5DC File Offset: 0x0022B7DC
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = this.activeInput.trigger;
			}
			this.leftAileronLevel = 0f;
			this.rightAileronLevel = 0f;
			float magnitude = this.activeInput.joystick.magnitude;
			if (magnitude > 0.01f)
			{
				float num = Mathf.Abs(this.activeInput.joystick.x) / magnitude;
				float num2 = Mathf.Abs(this.activeInput.joystick.y) / magnitude;
				this.leftAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * -this.activeInput.joystick.y, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp(num * this.activeInput.joystick.x + num2 * this.activeInput.joystick.y, -1f, 1f);
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = (byte)Mathf.Clamp(Mathf.FloorToInt(this.leftAileronLevel * 126f), -126, 126);
				this.networkSync.syncedState.dataC = (byte)Mathf.Clamp(Mathf.FloorToInt(this.rightAileronLevel * 126f), -126, 126);
			}
		}

		// Token: 0x06006A67 RID: 27239 RVA: 0x0022D780 File Offset: 0x0022B980
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				this.leftAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataB / 126f, -1f, 1f);
				this.rightAileronLevel = Mathf.Clamp((float)this.networkSync.syncedState.dataC / 126f, -1f, 1f);
			}
		}

		// Token: 0x06006A68 RID: 27240 RVA: 0x0022D81C File Offset: 0x0022BA1C
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0.6f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.audioSource.loop = true;
					this.audioSource.clip = this.motorSound;
					this.audioSource.volume = 0f;
					this.audioSource.GTPlay();
				}
				float num = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, num, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 5f, 6.6666665f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.audioSource.GTStop();
					this.audioSource.clip = null;
					this.audioSource.loop = false;
					this.audioSource.volume = this.crashSoundVolume;
					if (this.crashSound != null)
					{
						this.audioSource.GTPlayOneShot(this.crashSound, 1f);
					}
				}
				this.propellerSpinRate = Mathf.MoveTowards(this.propellerSpinRate, 0f, 13.333333f * dt);
				this.propellerAngle += this.propellerSpinRate * 360f * dt;
				this.propeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, this.propellerAngle));
				break;
			}
			float num2 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.leftAileronLevel));
			float num3 = Mathf.Lerp(this.aileronAngularRange.x, this.aileronAngularRange.y, Mathf.InverseLerp(-1f, 1f, this.rightAileronLevel));
			this.leftAileronAngle = Mathf.MoveTowards(this.leftAileronAngle, num2, this.aileronAngularAcc * Time.deltaTime);
			this.rightAileronAngle = Mathf.MoveTowards(this.rightAileronAngle, num3, this.aileronAngularAcc * Time.deltaTime);
			Quaternion localRotation = Quaternion.Euler(0f, -90f, 90f + this.leftAileronAngle);
			Quaternion localRotation2 = Quaternion.Euler(0f, 90f, -90f + this.rightAileronAngle);
			this.leftAileronLower.localRotation = localRotation;
			this.leftAileronUpper.localRotation = localRotation;
			this.rightAileronLower.localRotation = localRotation2;
			this.rightAileronUpper.localRotation = localRotation2;
		}

		// Token: 0x06006A69 RID: 27241 RVA: 0x0022DB84 File Offset: 0x0022BD84
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority || this.localState != RCVehicle.State.Mobilized)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float num = this.thrustVelocityTarget * x;
			float num2 = this.thrustAccel * x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.pitch = base.NormalizeAngle180(this.pitch);
			this.roll = base.NormalizeAngle180(this.roll);
			float num3 = this.pitch;
			float num4 = this.roll;
			if (this.activeInput.joystick.y >= 0f)
			{
				float num5 = this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.y;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, num5, this.pitchAccelMinMax.y * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			else
			{
				float num6 = -this.activeInput.joystick.y * this.pitchVelocityTargetMinMax.x;
				this.pitchVel = Mathf.MoveTowards(this.pitchVel, num6, this.pitchAccelMinMax.x * fixedDeltaTime);
				this.pitch += this.pitchVel * fixedDeltaTime;
			}
			float num7 = -this.activeInput.joystick.x * this.rollVelocityTarget;
			this.rollVel = Mathf.MoveTowards(this.rollVel, num7, this.rollAccel * fixedDeltaTime);
			this.roll += this.rollVel * fixedDeltaTime;
			Quaternion quaternion = Quaternion.Euler(new Vector3(this.pitch - num3, 0f, this.roll - num4));
			base.transform.rotation = base.transform.rotation * quaternion;
			this.rb.angularVelocity = Vector3.zero;
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			float num8 = Mathf.Max(Vector3.Dot(base.transform.forward, linearVelocity), 0f);
			float num9 = this.activeInput.trigger * num;
			float num10 = 0.1f * x;
			if (num9 > num10 && num9 > num8)
			{
				float num11 = Mathf.MoveTowards(num8, num9, num2 * fixedDeltaTime);
				this.rb.AddForce(base.transform.forward * (num11 - num8) * this.rb.mass, 1);
			}
			float num12 = 0.01f * x;
			float num13 = Vector3.Dot(linearVelocity / Mathf.Max(magnitude, num12), base.transform.forward);
			float num14 = this.liftVsAttackCurve.Evaluate(num13);
			float num15 = Mathf.Lerp(this.liftVsSpeedOutput.x, this.liftVsSpeedOutput.y, Mathf.InverseLerp(this.liftVsSpeedInput.x, this.liftVsSpeedInput.y, magnitude / x));
			float num16 = num14 * num15;
			Vector3 vector = Vector3.RotateTowards(linearVelocity, base.transform.forward * magnitude, this.pitchVelocityFollowRateAngle * 0.017453292f * fixedDeltaTime, this.pitchVelocityFollowRateMagnitude * fixedDeltaTime) - linearVelocity;
			this.rb.AddForce(vector * num16 * this.rb.mass, 1);
			float num17 = Vector3.Dot(linearVelocity.normalized, base.transform.up);
			float num18 = this.dragVsAttackCurve.Evaluate(num17);
			this.rb.AddForce(-linearVelocity * this.maxDrag * num18 * this.rb.mass, 0);
			if (this.rb.useGravity)
			{
				float gravityCompensation = Mathf.Lerp(this.gravityCompensationRange.x, this.gravityCompensationRange.y, Mathf.InverseLerp(0f, num, num8 / x));
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, gravityCompensation);
			}
		}

		// Token: 0x06006A6A RID: 27242 RVA: 0x0022DF6C File Offset: 0x0022C16C
		private void OnCollisionEnter(Collision collision)
		{
			if (base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				for (int i = 0; i < collision.contactCount; i++)
				{
					ContactPoint contact = collision.GetContact(i);
					if (!this.nonCrashColliders.Contains(contact.thisCollider))
					{
						this.AuthorityBeginCrash();
					}
				}
				return;
			}
			bool flag = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = collision.collider.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == 4).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (collision.rigidbody != null)
				{
					vector = collision.rigidbody.linearVelocity;
				}
				if (flag || vector.sqrMagnitude > 0.01f)
				{
					if (base.HasLocalAuthority)
					{
						this.AuthorityApplyImpact(vector, flag);
						return;
					}
					if (this.networkSync != null)
					{
						this.networkSync.photonView.RPC("HitRCVehicleRPC", 1, new object[]
						{
							vector,
							flag
						});
					}
				}
			}
		}

		// Token: 0x04007A4C RID: 31308
		public Vector2 pitchVelocityTargetMinMax = new Vector2(-180f, 180f);

		// Token: 0x04007A4D RID: 31309
		public Vector2 pitchVelocityRampTimeMinMax = new Vector2(-0.75f, 0.75f);

		// Token: 0x04007A4E RID: 31310
		public float rollVelocityTarget = 180f;

		// Token: 0x04007A4F RID: 31311
		public float rollVelocityRampTime = 0.75f;

		// Token: 0x04007A50 RID: 31312
		public float thrustVelocityTarget = 15f;

		// Token: 0x04007A51 RID: 31313
		public float thrustAccelTime = 2f;

		// Token: 0x04007A52 RID: 31314
		[SerializeField]
		private float pitchVelocityFollowRateAngle = 60f;

		// Token: 0x04007A53 RID: 31315
		[SerializeField]
		private float pitchVelocityFollowRateMagnitude = 5f;

		// Token: 0x04007A54 RID: 31316
		[SerializeField]
		private float maxDrag = 0.1f;

		// Token: 0x04007A55 RID: 31317
		[SerializeField]
		private Vector2 liftVsSpeedInput = new Vector2(0f, 4f);

		// Token: 0x04007A56 RID: 31318
		[SerializeField]
		private Vector2 liftVsSpeedOutput = new Vector2(0.5f, 1f);

		// Token: 0x04007A57 RID: 31319
		[SerializeField]
		private AnimationCurve liftVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007A58 RID: 31320
		[SerializeField]
		private AnimationCurve dragVsAttackCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x04007A59 RID: 31321
		[SerializeField]
		private Vector2 gravityCompensationRange = new Vector2(0.5f, 1f);

		// Token: 0x04007A5A RID: 31322
		[SerializeField]
		private List<Collider> nonCrashColliders = new List<Collider>();

		// Token: 0x04007A5B RID: 31323
		[SerializeField]
		private Transform propeller;

		// Token: 0x04007A5C RID: 31324
		[SerializeField]
		private Transform leftAileronUpper;

		// Token: 0x04007A5D RID: 31325
		[SerializeField]
		private Transform leftAileronLower;

		// Token: 0x04007A5E RID: 31326
		[SerializeField]
		private Transform rightAileronUpper;

		// Token: 0x04007A5F RID: 31327
		[SerializeField]
		private Transform rightAileronLower;

		// Token: 0x04007A60 RID: 31328
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007A61 RID: 31329
		[SerializeField]
		private AudioClip motorSound;

		// Token: 0x04007A62 RID: 31330
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x04007A63 RID: 31331
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.02f, 0.1f);

		// Token: 0x04007A64 RID: 31332
		[SerializeField]
		private float crashSoundVolume = 0.12f;

		// Token: 0x04007A65 RID: 31333
		private float motorVolumeRampTime = 1f;

		// Token: 0x04007A66 RID: 31334
		private float propellerAngle;

		// Token: 0x04007A67 RID: 31335
		private float propellerSpinRate;

		// Token: 0x04007A68 RID: 31336
		private const float propellerIdleAcc = 1f;

		// Token: 0x04007A69 RID: 31337
		private const float propellerIdleSpinRate = 0.6f;

		// Token: 0x04007A6A RID: 31338
		private const float propellerMaxAcc = 6.6666665f;

		// Token: 0x04007A6B RID: 31339
		private const float propellerMaxSpinRate = 5f;

		// Token: 0x04007A6C RID: 31340
		public float initialSpeed = 3f;

		// Token: 0x04007A6D RID: 31341
		private float pitch;

		// Token: 0x04007A6E RID: 31342
		private float pitchVel;

		// Token: 0x04007A6F RID: 31343
		private Vector2 pitchAccelMinMax;

		// Token: 0x04007A70 RID: 31344
		private float roll;

		// Token: 0x04007A71 RID: 31345
		private float rollVel;

		// Token: 0x04007A72 RID: 31346
		private float rollAccel;

		// Token: 0x04007A73 RID: 31347
		private float thrustAccel;

		// Token: 0x04007A74 RID: 31348
		private float motorLevel;

		// Token: 0x04007A75 RID: 31349
		private float leftAileronLevel;

		// Token: 0x04007A76 RID: 31350
		private float rightAileronLevel;

		// Token: 0x04007A77 RID: 31351
		private Vector2 aileronAngularRange = new Vector2(-30f, 45f);

		// Token: 0x04007A78 RID: 31352
		private float aileronAngularAcc = 120f;

		// Token: 0x04007A79 RID: 31353
		private float leftAileronAngle;

		// Token: 0x04007A7A RID: 31354
		private float rightAileronAngle;
	}
}
