using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200109B RID: 4251
	public class RCDragon : RCVehicle
	{
		// Token: 0x06006A50 RID: 27216 RVA: 0x0022C520 File Offset: 0x0022A720
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

		// Token: 0x06006A51 RID: 27217 RVA: 0x0022C590 File Offset: 0x0022A790
		protected override void Awake()
		{
			base.Awake();
			this.ascendAccel = this.maxAscendSpeed / this.ascendAccelTime;
			this.turnAccel = this.maxTurnRate / this.turnAccelTime;
			this.horizontalAccel = this.maxHorizontalSpeed / this.horizontalAccelTime;
			this.tiltAccel = this.maxHorizontalTiltAngle / this.horizontalTiltTime;
			this.shouldFlap = false;
			this.isFlapping = false;
			this.StopBreathFire();
			if (this.animation != null)
			{
				this.animation[this.wingFlapAnimName].speed = this.wingFlapAnimSpeed;
				this.animation[this.crashAnimName].speed = this.crashAnimSpeed;
				this.animation[this.mouthClosedAnimName].layer = 1;
				this.animation[this.mouthBreathFireAnimName].layer = 1;
			}
			this.nextFlapEventAnimTime = this.flapAnimEventTime;
		}

		// Token: 0x06006A52 RID: 27218 RVA: 0x0022C683 File Offset: 0x0022A883
		protected override void OnDisable()
		{
			base.OnDisable();
			this.audioSource.GTStop();
		}

		// Token: 0x06006A53 RID: 27219 RVA: 0x0022C698 File Offset: 0x0022A898
		public void StartBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthBreathFireAnimName))
			{
				this.animation.CrossFade(this.mouthBreathFireAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(true);
			}
			this.PlayRandomSound(this.breathFireSound, this.breathFireVolume);
			this.fireBreathTimeRemaining = this.fireBreathDuration;
		}

		// Token: 0x06006A54 RID: 27220 RVA: 0x0022C700 File Offset: 0x0022A900
		public void StopBreathFire()
		{
			if (!string.IsNullOrEmpty(this.mouthClosedAnimName))
			{
				this.animation.CrossFade(this.mouthClosedAnimName, 0.1f);
			}
			if (this.fireBreath != null)
			{
				this.fireBreath.SetActive(false);
			}
			this.fireBreathTimeRemaining = -1f;
		}

		// Token: 0x06006A55 RID: 27221 RVA: 0x0022C755 File Offset: 0x0022A955
		public bool IsBreathingFire()
		{
			return this.fireBreathTimeRemaining >= 0f;
		}

		// Token: 0x06006A56 RID: 27222 RVA: 0x0022C767 File Offset: 0x0022A967
		private void PlayRandomSound(List<AudioClip> clips, float volume)
		{
			if (clips == null || clips.Count == 0)
			{
				return;
			}
			this.PlaySound(clips[Random.Range(0, clips.Count)], volume);
		}

		// Token: 0x06006A57 RID: 27223 RVA: 0x0022C790 File Offset: 0x0022A990
		private void PlaySound(AudioClip clip, float volume)
		{
			if (this.audioSource == null || clip == null)
			{
				return;
			}
			this.audioSource.GTStop();
			this.audioSource.clip = null;
			this.audioSource.loop = false;
			this.audioSource.volume = volume;
			this.audioSource.GTPlayOneShot(clip, 1f);
		}

		// Token: 0x06006A58 RID: 27224 RVA: 0x0022C7F8 File Offset: 0x0022A9F8
		protected override void AuthorityUpdate(float dt)
		{
			base.AuthorityUpdate(dt);
			this.motorLevel = 0f;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				this.motorLevel = Mathf.Max(Mathf.Max(Mathf.Abs(this.activeInput.joystick.y), Mathf.Abs(this.activeInput.joystick.x)), this.activeInput.trigger);
				if (!this.IsBreathingFire() && this.activeInput.buttons > 0)
				{
					this.StartBreathFire();
				}
			}
			if (this.networkSync != null)
			{
				this.networkSync.syncedState.dataA = (byte)Mathf.Clamp(Mathf.FloorToInt(this.motorLevel * 255f), 0, 255);
				this.networkSync.syncedState.dataB = this.activeInput.buttons;
				this.networkSync.syncedState.dataC = (this.shouldFlap ? 1 : 0);
			}
		}

		// Token: 0x06006A59 RID: 27225 RVA: 0x0022C8F4 File Offset: 0x0022AAF4
		protected override void RemoteUpdate(float dt)
		{
			base.RemoteUpdate(dt);
			if (this.localState == RCVehicle.State.Mobilized && this.networkSync != null)
			{
				this.motorLevel = Mathf.Clamp01((float)this.networkSync.syncedState.dataA / 255f);
				if (!this.IsBreathingFire() && this.networkSync.syncedState.dataB > 0)
				{
					this.StartBreathFire();
				}
				this.shouldFlap = (this.networkSync.syncedState.dataC > 0);
			}
		}

		// Token: 0x06006A5A RID: 27226 RVA: 0x0022C97C File Offset: 0x0022AB7C
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
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = false;
					}
					if (this.animation != null)
					{
						this.animation.Play(this.dockedAnimName);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			case RCVehicle.State.Mobilized:
			{
				if (this.localStatePrev != RCVehicle.State.Mobilized && this.crashCollider != null)
				{
					this.crashCollider.enabled = false;
				}
				if (this.animation != null)
				{
					if (!this.isFlapping && this.shouldFlap)
					{
						this.animation.CrossFade(this.wingFlapAnimName, 0.1f);
						this.nextFlapEventAnimTime = this.flapAnimEventTime;
					}
					else if (this.isFlapping && !this.shouldFlap)
					{
						this.animation.CrossFade(this.idleAnimName, 0.15f);
					}
					this.isFlapping = this.shouldFlap;
					if (this.isFlapping && !this.IsBreathingFire())
					{
						AnimationState animationState = this.animation[this.wingFlapAnimName];
						if (animationState.normalizedTime * animationState.length > this.nextFlapEventAnimTime)
						{
							this.PlayRandomSound(this.wingFlapSound, this.wingFlapVolume);
							this.nextFlapEventAnimTime = (Mathf.Floor(animationState.normalizedTime) + 1f) * animationState.length + this.flapAnimEventTime;
						}
					}
				}
				GTTime.TimeAsDouble();
				if (this.IsBreathingFire())
				{
					this.fireBreathTimeRemaining -= dt;
					if (this.fireBreathTimeRemaining <= 0f)
					{
						this.StopBreathFire();
					}
				}
				float num = Mathf.Lerp(this.motorSoundVolumeMinMax.x, this.motorSoundVolumeMinMax.y, this.motorLevel);
				this.audioSource.volume = Mathf.MoveTowards(this.audioSource.volume, num, this.motorSoundVolumeMinMax.y / this.motorVolumeRampTime * dt);
				break;
			}
			case RCVehicle.State.Crashed:
				if (this.localStatePrev != RCVehicle.State.Crashed)
				{
					this.PlaySound(this.crashSound, this.crashSoundVolume);
					if (this.crashCollider != null)
					{
						this.crashCollider.enabled = true;
					}
					if (this.animation != null)
					{
						this.animation.CrossFade(this.crashAnimName, 0.05f);
					}
					if (this.IsBreathingFire())
					{
						this.StopBreathFire();
						return;
					}
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006A5B RID: 27227 RVA: 0x0022CC18 File Offset: 0x0022AE18
		private void FixedUpdate()
		{
			if (!base.HasLocalAuthority)
			{
				return;
			}
			float x = base.transform.lossyScale.x;
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.shouldFlap = false;
			if (this.localState == RCVehicle.State.Mobilized)
			{
				float num = this.maxAscendSpeed * x;
				float num2 = this.maxHorizontalSpeed * x;
				float num3 = this.ascendAccel * x;
				float num4 = this.ascendWhileFlyingAccelBoost * x;
				float num5 = 0.5f * x;
				float num6 = 45f;
				Vector3 linearVelocity = this.rb.linearVelocity;
				Vector3 normalized = new Vector3(base.transform.forward.x, 0f, base.transform.forward.z).normalized;
				this.turnAngle = Vector3.SignedAngle(Vector3.forward, normalized, Vector3.up);
				this.tiltAngle = Vector3.SignedAngle(normalized, base.transform.forward, base.transform.right);
				float num7 = this.activeInput.joystick.x * this.maxTurnRate;
				this.turnRate = Mathf.MoveTowards(this.turnRate, num7, this.turnAccel * fixedDeltaTime);
				this.turnAngle += this.turnRate * fixedDeltaTime;
				float num8 = Vector3.Dot(normalized, linearVelocity);
				float num9 = Mathf.InverseLerp(-num2, num2, num8);
				float num10 = Mathf.Lerp(-this.maxHorizontalTiltAngle, this.maxHorizontalTiltAngle, num9);
				this.tiltAngle = Mathf.MoveTowards(this.tiltAngle, num10, this.tiltAccel * fixedDeltaTime);
				base.transform.rotation = Quaternion.Euler(new Vector3(this.tiltAngle, this.turnAngle, 0f));
				Vector3 vector;
				vector..ctor(linearVelocity.x, 0f, linearVelocity.z);
				Vector3 vector2 = Vector3.Lerp(normalized * this.activeInput.joystick.y * num2, vector, Mathf.Exp(-this.horizontalAccelTime * fixedDeltaTime));
				this.rb.AddForce((vector2 - vector) * this.rb.mass, 1);
				float num11 = this.activeInput.trigger * num;
				if (num11 > 0.01f && linearVelocity.y < num11)
				{
					this.rb.AddForce(Vector3.up * num3 * this.rb.mass, 0);
				}
				bool flag = Mathf.Abs(num8) > num5;
				bool flag2 = Mathf.Abs(this.turnRate) > num6;
				if (flag || flag2)
				{
					this.rb.AddForce(Vector3.up * num4 * this.rb.mass, 0);
				}
				this.shouldFlap = (num11 > 0.01f || flag || flag2);
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

		// Token: 0x06006A5C RID: 27228 RVA: 0x0022CF2C File Offset: 0x0022B12C
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

		// Token: 0x04007A0D RID: 31245
		[SerializeField]
		private float maxAscendSpeed = 6f;

		// Token: 0x04007A0E RID: 31246
		[SerializeField]
		private float ascendAccelTime = 3f;

		// Token: 0x04007A0F RID: 31247
		[SerializeField]
		private float ascendWhileFlyingAccelBoost;

		// Token: 0x04007A10 RID: 31248
		[SerializeField]
		private float gravityCompensation = 0.9f;

		// Token: 0x04007A11 RID: 31249
		[SerializeField]
		private float crashedGravityCompensation = 0.5f;

		// Token: 0x04007A12 RID: 31250
		[SerializeField]
		private float maxTurnRate = 90f;

		// Token: 0x04007A13 RID: 31251
		[SerializeField]
		private float turnAccelTime = 0.75f;

		// Token: 0x04007A14 RID: 31252
		[SerializeField]
		private float maxHorizontalSpeed = 6f;

		// Token: 0x04007A15 RID: 31253
		[SerializeField]
		private float horizontalAccelTime = 2f;

		// Token: 0x04007A16 RID: 31254
		[SerializeField]
		private float maxHorizontalTiltAngle = 45f;

		// Token: 0x04007A17 RID: 31255
		[SerializeField]
		private float horizontalTiltTime = 2f;

		// Token: 0x04007A18 RID: 31256
		[SerializeField]
		private Vector2 motorSoundVolumeMinMax = new Vector2(0.1f, 0.8f);

		// Token: 0x04007A19 RID: 31257
		[SerializeField]
		private float crashSoundVolume = 0.1f;

		// Token: 0x04007A1A RID: 31258
		[SerializeField]
		private float breathFireVolume = 0.5f;

		// Token: 0x04007A1B RID: 31259
		[SerializeField]
		private float wingFlapVolume = 0.1f;

		// Token: 0x04007A1C RID: 31260
		[SerializeField]
		private Animation animation;

		// Token: 0x04007A1D RID: 31261
		[SerializeField]
		private string wingFlapAnimName;

		// Token: 0x04007A1E RID: 31262
		[SerializeField]
		private float wingFlapAnimSpeed = 1f;

		// Token: 0x04007A1F RID: 31263
		[SerializeField]
		private string dockedAnimName;

		// Token: 0x04007A20 RID: 31264
		[SerializeField]
		private string idleAnimName;

		// Token: 0x04007A21 RID: 31265
		[SerializeField]
		private string crashAnimName;

		// Token: 0x04007A22 RID: 31266
		[SerializeField]
		private float crashAnimSpeed = 1f;

		// Token: 0x04007A23 RID: 31267
		[SerializeField]
		private string mouthClosedAnimName;

		// Token: 0x04007A24 RID: 31268
		[SerializeField]
		private string mouthBreathFireAnimName;

		// Token: 0x04007A25 RID: 31269
		private bool shouldFlap;

		// Token: 0x04007A26 RID: 31270
		private bool isFlapping;

		// Token: 0x04007A27 RID: 31271
		private float nextFlapEventAnimTime;

		// Token: 0x04007A28 RID: 31272
		[SerializeField]
		private float flapAnimEventTime = 0.25f;

		// Token: 0x04007A29 RID: 31273
		[SerializeField]
		private GameObject fireBreath;

		// Token: 0x04007A2A RID: 31274
		[SerializeField]
		private float fireBreathDuration;

		// Token: 0x04007A2B RID: 31275
		private float fireBreathTimeRemaining;

		// Token: 0x04007A2C RID: 31276
		[SerializeField]
		private Collider crashCollider;

		// Token: 0x04007A2D RID: 31277
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04007A2E RID: 31278
		[SerializeField]
		private List<AudioClip> breathFireSound;

		// Token: 0x04007A2F RID: 31279
		[SerializeField]
		private List<AudioClip> wingFlapSound;

		// Token: 0x04007A30 RID: 31280
		[SerializeField]
		private AudioClip crashSound;

		// Token: 0x04007A31 RID: 31281
		private float turnRate;

		// Token: 0x04007A32 RID: 31282
		private float turnAngle;

		// Token: 0x04007A33 RID: 31283
		private float tiltAngle;

		// Token: 0x04007A34 RID: 31284
		private float ascendAccel;

		// Token: 0x04007A35 RID: 31285
		private float turnAccel;

		// Token: 0x04007A36 RID: 31286
		private float tiltAccel;

		// Token: 0x04007A37 RID: 31287
		private float horizontalAccel;

		// Token: 0x04007A38 RID: 31288
		private float motorVolumeRampTime = 1f;

		// Token: 0x04007A39 RID: 31289
		private float motorLevel;
	}
}
