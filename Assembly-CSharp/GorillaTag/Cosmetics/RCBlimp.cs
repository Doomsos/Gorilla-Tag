using System;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001098 RID: 4248
	public class RCBlimp : RCVehicle
	{
		// Token: 0x06006A42 RID: 27202 RVA: 0x0022B700 File Offset: 0x00229900
		protected override void AuthorityBeginDocked()
		{
			base.AuthorityBeginDocked();
			this.turnRate = 0f;
			this.turnAngle = Vector3.SignedAngle(Vector3.forward, Vector3.ProjectOnPlane(base.transform.forward, Vector3.up), Vector3.up);
			this.motorLevel = 0f;
			if (this.connectedRemote == null)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x06006A43 RID: 27203 RVA: 0x0022B770 File Offset: 0x00229970
		protected override void Awake()
		{
			base.Awake();
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
			this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
		}

		// Token: 0x06006A44 RID: 27204 RVA: 0x0022B7CF File Offset: 0x002299CF
		protected override void OnDisable()
		{
			base.OnDisable();
			this.audioSource.GTStop();
		}

		// Token: 0x06006A45 RID: 27205 RVA: 0x0022B7E4 File Offset: 0x002299E4
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
			}
		}

		// Token: 0x06006A46 RID: 27206 RVA: 0x0022B88C File Offset: 0x00229A8C
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.localState == RCVehicle.State.Mobilized && this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
			}
		}

		// Token: 0x06006A47 RID: 27207 RVA: 0x0022B8DC File Offset: 0x00229ADC
		protected override void SharedUpdate(float dt)
		{
			base.SharedUpdate(dt);
			switch (this.localState)
			{
			case RCVehicle.State.Disabled:
				break;
			case RCVehicle.State.DockedLeft:
			case RCVehicle.State.DockedRight:
				if (this.localStatePrev != RCVehicle.State.DockedLeft && this.localStatePrev != RCVehicle.State.DockedRight)
				{
					this.audioSource.GTStop();
					this.blimpDeflateBlendWeight = 0f;
					this.blimpMesh.SetBlendShapeWeight(0, 0f);
					this.crashCollider.enabled = false;
				}
				this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, 0.6f, 6.6666665f * dt);
				this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, 0.6f, 6.6666665f * dt);
				this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
				this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
				this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0f, -90f));
				this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0f, 90f));
				return;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized)
				{
					this.audioSource.loop = true;
					this.audioSource.clip = this.motorSound;
					this.audioSource.volume = 0f;
					this.audioSource.GTPlay();
					this.blimpDeflateBlendWeight = 0f;
					this.blimpMesh.SetBlendShapeWeight(0, 0f);
					this.crashCollider.enabled = false;
				}
				float num = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, num, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				this.blimpDeflateBlendWeight = 0f;
				float num2 = this.activeInput.joystick.y * 5f;
				float num3 = this.activeInput.joystick.x * 5f;
				float num4 = Mathf.Clamp(num3 + num2 + 0.6f, -5f, 5f);
				float num5 = Mathf.Clamp(-num3 + num2 + 0.6f, -5f, 5f);
				this.leftPropellerSpinRate = Mathf.MoveTowards(this.leftPropellerSpinRate, num4, 6.6666665f * dt);
				this.rightPropellerSpinRate = Mathf.MoveTowards(this.rightPropellerSpinRate, num5, 6.6666665f * dt);
				this.leftPropellerAngle += this.leftPropellerSpinRate * 360f * dt;
				this.rightPropellerAngle += this.rightPropellerSpinRate * 360f * dt;
				this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.leftPropellerAngle, 0f, -90f));
				this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(this.rightPropellerAngle, 0f, 90f));
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.audioSource.GTStop();
					this.audioSource.clip = null;
					this.audioSource.loop = false;
					this.audioSource.volume = this.deflateSoundVolume;
					if (this.deflateSound != null)
					{
						this.audioSource.GTPlayOneShot(this.deflateSound, 1f);
					}
					this.leftPropellerSpinRate = 0f;
					this.rightPropellerSpinRate = 0f;
					this.leftPropellerAngle = 0f;
					this.rightPropellerAngle = 0f;
					this.leftPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
					this.rightPropeller.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
					this.crashCollider.enabled = true;
				}
				this.blimpDeflateBlendWeight = Mathf.Lerp(1f, this.blimpDeflateBlendWeight, Mathf.Exp(-this.deflateRate * dt));
				this.blimpMesh.SetBlendShapeWeight(0, this.blimpDeflateBlendWeight * 100f);
				return;
			default:
				return;
			}
		}

		// Token: 0x06006A48 RID: 27208 RVA: 0x0022BD2C File Offset: 0x00229F2C
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority)
			{
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			float x = base.transform.lossyScale.x;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = this.maxAscendSpeed * x;
				float num2 = this.maxHorizontalSpeed * x;
				float num3 = this.ascendAccel * x;
				Vector3 linearVelocity = this.rb.linearVelocity;
				Vector3 normalized = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
				this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
				this.tiltAngle = Vector3.SignedAngle(normalized, base.transform.forward, base.transform.right);
				float num4 = this.activeInput.joystick.x * this.maxTurnRate;
				this.turnRate = Mathf.MoveTowards(this.turnRate, num4, this.turnAccel * fixedDeltaTime);
				this.turnAngle += this.turnRate * fixedDeltaTime;
				float num5 = Vector3.Dot(normalized, linearVelocity);
				float num6 = Mathf.InverseLerp(-num2, num2, num5);
				float num7 = Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, num6);
				this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, num7, this.tiltAccel * fixedDeltaTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0f));
				Vector3 vector;
				vector..ctor(linearVelocity.x, 0f, linearVelocity.z);
				Vector3 vector2 = Vector3.Lerp(normalized * this.activeInput.joystick.y * num2, vector, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime));
				this.rb.AddForce((vector2 - vector) * this.rb.mass, 1);
				float num8 = this.activeInput.trigger * num;
				if (num8 > 0.01f && linearVelocity.y < num8)
				{
					this.rb.AddForce(Vector3.up * num3 * this.rb.mass, 0);
				}
				if (this.rb.useGravity)
				{
					RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.gravityCompensation);
					return;
				}
			}
			else if (this.localState == RCVehicle.State.Crashed && this.rb.useGravity)
			{
				RCVehicle.AddScaledGravityCompensationForce(this.rb, x, this.crashedGravityCompensation);
			}
		}

		// Token: 0x06006A49 RID: 27209 RVA: 0x0022BFBC File Offset: 0x0022A1BC
		private void OnTriggerEnter(Collider other)
		{
			bool flag = other.gameObject.IsOnLayer(UnityLayer.GorillaThrowable);
			bool flag2 = other.gameObject.IsOnLayer(UnityLayer.GorillaHand);
			if (!other.isTrigger && base.HasLocalAuthority && this.localState == RCVehicle.State.Mobilized)
			{
				this.AuthorityBeginCrash();
				return;
			}
			if ((flag || flag2) && this.localState == RCVehicle.State.Mobilized)
			{
				Vector3 vector = Vector3.zero;
				if (flag2)
				{
					GorillaHandClimber component = other.gameObject.GetComponent<GorillaHandClimber>();
					if (component != null)
					{
						vector = GTPlayer.Instance.GetHandVelocityTracker(component.xrNode == 4).GetAverageVelocity(true, 0.15f, false);
					}
				}
				else if (other.attachedRigidbody != null)
				{
					vector = other.attachedRigidbody.linearVelocity;
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

		// Token: 0x040079DF RID: 31199
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x040079E0 RID: 31200
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x040079E1 RID: 31201
		[SerializeField]
		private float gravityCompensation = 0.9f;

		// Token: 0x040079E2 RID: 31202
		[SerializeField]
		private float crashedGravityCompensation = 0.5f;

		// Token: 0x040079E3 RID: 31203
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x040079E4 RID: 31204
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x040079E5 RID: 31205
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x040079E6 RID: 31206
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x040079E7 RID: 31207
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x040079E8 RID: 31208
		[SerializeField]
		private float horizontalTiltTime = 2f;

		// Token: 0x040079E9 RID: 31209
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);

		// Token: 0x040079EA RID: 31210
		[SerializeField]
		private float deflateSoundVolume = 0.1f;

		// Token: 0x040079EB RID: 31211
		[SerializeField]
		private Collider crashCollider;

		// Token: 0x040079EC RID: 31212
		[SerializeField]
		private Transform leftPropeller;

		// Token: 0x040079ED RID: 31213
		[SerializeField]
		private Transform rightPropeller;

		// Token: 0x040079EE RID: 31214
		[SerializeField]
		private SkinnedMeshRenderer blimpMesh;

		// Token: 0x040079EF RID: 31215
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040079F0 RID: 31216
		[SerializeField]
		private AudioClip motorSound;

		// Token: 0x040079F1 RID: 31217
		[SerializeField]
		private AudioClip deflateSound;

		// Token: 0x040079F2 RID: 31218
		private float turnRate;

		// Token: 0x040079F3 RID: 31219
		private float turnAngle;

		// Token: 0x040079F4 RID: 31220
		private float tiltAngle;

		// Token: 0x040079F5 RID: 31221
		private float ascendAccel;

		// Token: 0x040079F6 RID: 31222
		private float turnAccel;

		// Token: 0x040079F7 RID: 31223
		private float tiltAccel;

		// Token: 0x040079F8 RID: 31224
		private float horizontalAccel;

		// Token: 0x040079F9 RID: 31225
		private float leftPropellerAngle;

		// Token: 0x040079FA RID: 31226
		private float rightPropellerAngle;

		// Token: 0x040079FB RID: 31227
		private float leftPropellerSpinRate;

		// Token: 0x040079FC RID: 31228
		private float rightPropellerSpinRate;

		// Token: 0x040079FD RID: 31229
		private float blimpDeflateBlendWeight;

		// Token: 0x040079FE RID: 31230
		private float deflateRate = Mathf.Exp(1f);

		// Token: 0x040079FF RID: 31231
		private const float propellerIdleAcc = 1f;

		// Token: 0x04007A00 RID: 31232
		private const float propellerIdleSpinRate = 0.6f;

		// Token: 0x04007A01 RID: 31233
		private const float propellerMaxAcc = 6.6666665f;

		// Token: 0x04007A02 RID: 31234
		private const float propellerMaxSpinRate = 5f;

		// Token: 0x04007A03 RID: 31235
		private float motorVolumeRampTime = 1f;

		// Token: 0x04007A04 RID: 31236
		private float motorLevel;
	}
}
