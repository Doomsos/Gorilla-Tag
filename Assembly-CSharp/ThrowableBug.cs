using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000CC7 RID: 3271
public class ThrowableBug : TransferrableObject, ITickSystemTick
{
	// Token: 0x17000769 RID: 1897
	// (get) Token: 0x06004FC0 RID: 20416 RVA: 0x0019A7CE File Offset: 0x001989CE
	// (set) Token: 0x06004FC1 RID: 20417 RVA: 0x0019A7D6 File Offset: 0x001989D6
	public bool TickRunning { get; set; }

	// Token: 0x06004FC2 RID: 20418 RVA: 0x0019A7E0 File Offset: 0x001989E0
	protected override void Start()
	{
		base.Start();
		float num = Random.Range(0f, 6.2831855f);
		this.targetVelocity = new Vector3(Mathf.Sin(num) * this.maxNaturalSpeed, 0f, Mathf.Cos(num) * this.maxNaturalSpeed);
		this.currentState = TransferrableObject.PositionState.Dropped;
		this.rayCastNonAllocColliders = new RaycastHit[5];
		this.rayCastNonAllocColliders2 = new RaycastHit[5];
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
	}

	// Token: 0x06004FC3 RID: 20419 RVA: 0x0019A85C File Offset: 0x00198A5C
	internal override void OnEnable()
	{
		base.OnEnable();
		ThrowableBugBeacon.OnCall += this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss += this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock += this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock += this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier += this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06004FC4 RID: 20420 RVA: 0x0019A8CC File Offset: 0x00198ACC
	internal override void OnDisable()
	{
		base.OnDisable();
		ThrowableBugBeacon.OnCall -= this.ThrowableBugBeacon_OnCall;
		ThrowableBugBeacon.OnDismiss -= this.ThrowableBugBeacon_OnDismiss;
		ThrowableBugBeacon.OnLock -= this.ThrowableBugBeacon_OnLock;
		ThrowableBugBeacon.OnUnlock -= this.ThrowableBugBeacon_OnUnlock;
		ThrowableBugBeacon.OnChangeSpeedMultiplier -= this.ThrowableBugBeacon_OnChangeSpeedMultiplier;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06004FC5 RID: 20421 RVA: 0x0019A93C File Offset: 0x00198B3C
	private bool isValid(ThrowableBugBeacon tbb)
	{
		return tbb.BugName == this.bugName && (tbb.Range <= 0f || Vector3.Distance(tbb.transform.position, base.transform.position) <= tbb.Range);
	}

	// Token: 0x06004FC6 RID: 20422 RVA: 0x0019A98E File Offset: 0x00198B8E
	private void ThrowableBugBeacon_OnCall(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
		}
	}

	// Token: 0x06004FC7 RID: 20423 RVA: 0x0019A9C0 File Offset: 0x00198BC0
	private void ThrowableBugBeacon_OnLock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = tbb.transform.position - base.transform.position;
			this.lockedTarget = tbb.transform;
			this.locked = true;
		}
	}

	// Token: 0x06004FC8 RID: 20424 RVA: 0x0019AA0F File Offset: 0x00198C0F
	private void ThrowableBugBeacon_OnDismiss(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.reliableState.travelingDirection = base.transform.position - tbb.transform.position;
			this.locked = false;
		}
	}

	// Token: 0x06004FC9 RID: 20425 RVA: 0x0019AA47 File Offset: 0x00198C47
	private void ThrowableBugBeacon_OnUnlock(ThrowableBugBeacon tbb)
	{
		if (this.isValid(tbb))
		{
			this.locked = false;
		}
	}

	// Token: 0x06004FCA RID: 20426 RVA: 0x0019AA59 File Offset: 0x00198C59
	private void ThrowableBugBeacon_OnChangeSpeedMultiplier(ThrowableBugBeacon tbb, float f)
	{
		if (this.isValid(tbb))
		{
			this.speedMultiplier = f;
		}
	}

	// Token: 0x06004FCB RID: 20427 RVA: 0x00027DED File Offset: 0x00025FED
	public override bool ShouldBeKinematic()
	{
		return true;
	}

	// Token: 0x06004FCC RID: 20428 RVA: 0x0019AA6C File Offset: 0x00198C6C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.raycastFrameCounter = (this.raycastFrameCounter + 1) % this.raycastFramePeriod;
		bool flag = this.currentState == TransferrableObject.PositionState.InLeftHand || this.currentState == TransferrableObject.PositionState.InRightHand;
		if (this.animator.enabled)
		{
			this.animator.SetBool(ThrowableBug._g_IsHeld, flag);
		}
		if (!this.audioSource)
		{
			return;
		}
		switch (this.currentAudioState)
		{
		case ThrowableBug.AudioState.JustGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			if (this.grabBugAudioClip && this.audioSource.clip != this.grabBugAudioClip)
			{
				this.audioSource.clip = this.grabBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			else if (!this.audioSource.isPlaying)
			{
				this.currentAudioState = ThrowableBug.AudioState.ContinuallyGrabbed;
				return;
			}
			break;
		case ThrowableBug.AudioState.ContinuallyGrabbed:
			if (!flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustReleased;
				return;
			}
			break;
		case ThrowableBug.AudioState.JustReleased:
			if (!flag)
			{
				if (this.releaseBugAudioClip && this.audioSource.clip != this.releaseBugAudioClip)
				{
					this.audioSource.clip = this.releaseBugAudioClip;
					this.audioSource.time = 0f;
					if (this.audioSource.isActiveAndEnabled)
					{
						this.audioSource.GTPlay();
						return;
					}
				}
				else if (!this.audioSource.isPlaying)
				{
					this.currentAudioState = ThrowableBug.AudioState.NotHeld;
					return;
				}
			}
			else
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
			}
			break;
		case ThrowableBug.AudioState.NotHeld:
			if (flag)
			{
				this.currentAudioState = ThrowableBug.AudioState.JustGrabbed;
				return;
			}
			if (this.flyingBugAudioClip && !this.audioSource.isPlaying)
			{
				this.audioSource.clip = this.flyingBugAudioClip;
				this.audioSource.time = 0f;
				if (this.audioSource.isActiveAndEnabled)
				{
					this.audioSource.GTPlay();
					return;
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06004FCD RID: 20429 RVA: 0x0019AC70 File Offset: 0x00198E70
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!this.reliableState)
		{
			return;
		}
		if ((this.currentState & TransferrableObject.PositionState.Dropped) == TransferrableObject.PositionState.None)
		{
			return;
		}
		if (this.locked && Vector3.Distance(this.lockedTarget.position, base.transform.position) > 0.1f)
		{
			this.reliableState.travelingDirection = this.lockedTarget.position - base.transform.position;
		}
		if (this.slowingDownProgress < 1f)
		{
			this.slowingDownProgress += this.slowdownAcceleration * Time.deltaTime;
			this.reliableState.travelingDirection = Vector3.Slerp(this.thrownVeloicity, this.targetVelocity, Mathf.SmoothStep(0f, 1f, this.slowingDownProgress));
		}
		else
		{
			this.reliableState.travelingDirection = this.reliableState.travelingDirection.normalized * this.maxNaturalSpeed;
		}
		this.bobingFrequency = (this.shouldRandomizeFrequency ? this.RandomizeBobingFrequency() : this.bobbingDefaultFrequency);
		float num = this.bobingState + this.bobingSpeed * Time.deltaTime;
		float num2 = Mathf.Sin(num / this.bobingFrequency) - Mathf.Sin(this.bobingState / this.bobingFrequency);
		Vector3 vector = Vector3.up * (num2 * this.bobMagnintude);
		this.bobingState = num;
		if (this.bobingState > 6.2831855f)
		{
			this.bobingState -= 6.2831855f;
		}
		vector += this.reliableState.travelingDirection * Time.deltaTime;
		float num3 = this.isTooHighTravelingDown ? this.minimumHeightOffOfTheGroundBeforeStoppingDescent : this.maximumHeightOffOfTheGroundBeforeStartingDescent;
		float num4 = this.isTooLowTravelingUp ? this.maximumHeightOffOfTheGroundBeforeStoppingAscent : this.minimumHeightOffOfTheGroundBeforeStartingAscent;
		if (this.raycastFrameCounter == 0)
		{
			if (Physics.RaycastNonAlloc(base.transform.position, Vector3.down, this.rayCastNonAllocColliders2, num3, this.collisionCheckMask) > 0)
			{
				this.isTooHighTravelingDown = false;
				if (this.descentSlerp > 0f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp - this.descentSlerpRate * Time.deltaTime);
				}
				RaycastHit raycastHit = this.rayCastNonAllocColliders2[0];
				this.isTooLowTravelingUp = (raycastHit.distance < num4);
				if (this.isTooLowTravelingUp)
				{
					if (this.ascentSlerp < 1f)
					{
						this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp + this.ascentSlerpRate * Time.deltaTime);
					}
				}
				else if (this.ascentSlerp > 0f)
				{
					this.ascentSlerp = Mathf.Clamp01(this.ascentSlerp - this.ascentSlerpRate * Time.deltaTime);
				}
			}
			else
			{
				this.isTooHighTravelingDown = true;
				if (this.descentSlerp < 1f)
				{
					this.descentSlerp = Mathf.Clamp01(this.descentSlerp + this.descentSlerpRate * Time.deltaTime);
				}
			}
		}
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.descentSlerp) * this.descentRate * Vector3.down;
		vector += Time.deltaTime * Mathf.SmoothStep(0f, 1f, this.ascentSlerp) * this.ascentRate * Vector3.up;
		float num5;
		Vector3 vector2;
		Quaternion.FromToRotation(base.transform.rotation * Vector3.up, Quaternion.identity * Vector3.up).ToAngleAxis(ref num5, ref vector2);
		Quaternion quaternion = Quaternion.AngleAxis(num5 * 0.02f, vector2);
		float num6;
		Vector3 vector3;
		Quaternion.FromToRotation(base.transform.rotation * Vector3.forward, this.reliableState.travelingDirection.normalized).ToAngleAxis(ref num6, ref vector3);
		Quaternion quaternion2 = Quaternion.AngleAxis(num6 * 0.005f, vector3);
		quaternion = quaternion2 * quaternion;
		vector = quaternion * quaternion * quaternion * quaternion * vector;
		vector *= this.speedMultiplier;
		this.speedMultiplier = Mathf.MoveTowards(this.speedMultiplier, 1f, Time.deltaTime);
		if (this.raycastFrameCounter == 0)
		{
			if (Physics.SphereCastNonAlloc(base.transform.position, this.collisionHitRadius, vector.normalized, this.rayCastNonAllocColliders, vector.magnitude, this.collisionCheckMask) > 0)
			{
				Vector3 normal = this.rayCastNonAllocColliders[0].normal;
				this.reliableState.travelingDirection = Vector3.Reflect(this.reliableState.travelingDirection, normal).x0z();
				base.transform.position += Vector3.Reflect(vector, normal);
				this.thrownVeloicity = Vector3.Reflect(this.thrownVeloicity, normal);
				this.targetVelocity = Vector3.Reflect(this.targetVelocity, normal).x0z();
			}
			else
			{
				base.transform.position += vector;
			}
		}
		else
		{
			base.transform.position += vector;
		}
		this.bugRotationalVelocity = quaternion * this.bugRotationalVelocity;
		float num7;
		Vector3 vector4;
		this.bugRotationalVelocity.ToAngleAxis(ref num7, ref vector4);
		this.bugRotationalVelocity = Quaternion.AngleAxis(num7 * 0.9f, vector4);
		base.transform.rotation = this.bugRotationalVelocity * base.transform.rotation;
	}

	// Token: 0x06004FCE RID: 20430 RVA: 0x0019B1F5 File Offset: 0x001993F5
	private float RandomizeBobingFrequency()
	{
		return Random.Range(this.minRandFrequency, this.maxRandFrequency);
	}

	// Token: 0x06004FCF RID: 20431 RVA: 0x0019B208 File Offset: 0x00199408
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		this.slowingDownProgress = 0f;
		Vector3 linearVelocity = this.velocityEstimator.linearVelocity;
		this.thrownVeloicity = linearVelocity;
		this.reliableState.travelingDirection = linearVelocity;
		this.bugRotationalVelocity = Quaternion.Euler(this.velocityEstimator.angularVelocity);
		this.startingSpeed = linearVelocity.magnitude;
		Vector3 normalized = this.reliableState.travelingDirection.x0z().normalized;
		this.targetVelocity = normalized * this.maxNaturalSpeed;
		return true;
	}

	// Token: 0x06004FD0 RID: 20432 RVA: 0x0019B29A File Offset: 0x0019949A
	public void OnCollisionEnter(Collision collision)
	{
		this.reliableState.travelingDirection *= -1f;
	}

	// Token: 0x06004FD1 RID: 20433 RVA: 0x0019B2B8 File Offset: 0x001994B8
	public void Tick()
	{
		if (this.updateMultiplier > 0)
		{
			for (int i = 0; i < this.updateMultiplier; i++)
			{
				this.LateUpdateLocal();
			}
		}
	}

	// Token: 0x04005E4B RID: 24139
	public ThrowableBugReliableState reliableState;

	// Token: 0x04005E4C RID: 24140
	public float slowingDownProgress;

	// Token: 0x04005E4D RID: 24141
	public float startingSpeed;

	// Token: 0x04005E4E RID: 24142
	public int raycastFramePeriod = 5;

	// Token: 0x04005E4F RID: 24143
	private int raycastFrameCounter;

	// Token: 0x04005E50 RID: 24144
	public float bobingSpeed = 1f;

	// Token: 0x04005E51 RID: 24145
	public float bobMagnintude = 0.1f;

	// Token: 0x04005E52 RID: 24146
	public bool shouldRandomizeFrequency;

	// Token: 0x04005E53 RID: 24147
	public float minRandFrequency = 0.008f;

	// Token: 0x04005E54 RID: 24148
	public float maxRandFrequency = 1f;

	// Token: 0x04005E55 RID: 24149
	public float bobingFrequency = 1f;

	// Token: 0x04005E56 RID: 24150
	public float bobingState;

	// Token: 0x04005E57 RID: 24151
	public float thrownYVelocity;

	// Token: 0x04005E58 RID: 24152
	public float collisionHitRadius;

	// Token: 0x04005E59 RID: 24153
	public LayerMask collisionCheckMask;

	// Token: 0x04005E5A RID: 24154
	public Vector3 thrownVeloicity;

	// Token: 0x04005E5B RID: 24155
	public Vector3 targetVelocity;

	// Token: 0x04005E5C RID: 24156
	public Quaternion bugRotationalVelocity;

	// Token: 0x04005E5D RID: 24157
	private RaycastHit[] rayCastNonAllocColliders;

	// Token: 0x04005E5E RID: 24158
	private RaycastHit[] rayCastNonAllocColliders2;

	// Token: 0x04005E5F RID: 24159
	public VRRig followingRig;

	// Token: 0x04005E60 RID: 24160
	public bool isTooHighTravelingDown;

	// Token: 0x04005E61 RID: 24161
	public float descentSlerp;

	// Token: 0x04005E62 RID: 24162
	public float ascentSlerp;

	// Token: 0x04005E63 RID: 24163
	public float maxNaturalSpeed;

	// Token: 0x04005E64 RID: 24164
	public float slowdownAcceleration;

	// Token: 0x04005E65 RID: 24165
	public float maximumHeightOffOfTheGroundBeforeStartingDescent = 5f;

	// Token: 0x04005E66 RID: 24166
	public float minimumHeightOffOfTheGroundBeforeStoppingDescent = 3f;

	// Token: 0x04005E67 RID: 24167
	public float descentRate = 0.2f;

	// Token: 0x04005E68 RID: 24168
	public float descentSlerpRate = 0.2f;

	// Token: 0x04005E69 RID: 24169
	public float minimumHeightOffOfTheGroundBeforeStartingAscent = 0.5f;

	// Token: 0x04005E6A RID: 24170
	public float maximumHeightOffOfTheGroundBeforeStoppingAscent = 0.75f;

	// Token: 0x04005E6B RID: 24171
	public float ascentRate = 0.4f;

	// Token: 0x04005E6C RID: 24172
	public float ascentSlerpRate = 1f;

	// Token: 0x04005E6D RID: 24173
	private bool isTooLowTravelingUp;

	// Token: 0x04005E6E RID: 24174
	public Animator animator;

	// Token: 0x04005E6F RID: 24175
	[FormerlySerializedAs("grabBugAudioSource")]
	public AudioClip grabBugAudioClip;

	// Token: 0x04005E70 RID: 24176
	[FormerlySerializedAs("releaseBugAudioSource")]
	public AudioClip releaseBugAudioClip;

	// Token: 0x04005E71 RID: 24177
	[FormerlySerializedAs("flyingBugAudioSource")]
	public AudioClip flyingBugAudioClip;

	// Token: 0x04005E72 RID: 24178
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04005E73 RID: 24179
	private float bobbingDefaultFrequency = 1f;

	// Token: 0x04005E74 RID: 24180
	public int updateMultiplier;

	// Token: 0x04005E75 RID: 24181
	private ThrowableBug.AudioState currentAudioState;

	// Token: 0x04005E76 RID: 24182
	private float speedMultiplier = 1f;

	// Token: 0x04005E77 RID: 24183
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04005E79 RID: 24185
	[SerializeField]
	private ThrowableBug.BugName bugName;

	// Token: 0x04005E7A RID: 24186
	private Transform lockedTarget;

	// Token: 0x04005E7B RID: 24187
	private bool locked;

	// Token: 0x04005E7C RID: 24188
	private static readonly int _g_IsHeld = Animator.StringToHash("isHeld");

	// Token: 0x02000CC8 RID: 3272
	public enum BugName
	{
		// Token: 0x04005E7E RID: 24190
		NONE,
		// Token: 0x04005E7F RID: 24191
		DougTheBug,
		// Token: 0x04005E80 RID: 24192
		MattTheBat
	}

	// Token: 0x02000CC9 RID: 3273
	private enum AudioState
	{
		// Token: 0x04005E82 RID: 24194
		JustGrabbed,
		// Token: 0x04005E83 RID: 24195
		ContinuallyGrabbed,
		// Token: 0x04005E84 RID: 24196
		JustReleased,
		// Token: 0x04005E85 RID: 24197
		NotHeld
	}
}
