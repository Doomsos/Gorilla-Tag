using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000285 RID: 645
public class SpinWithGorillaSpeed : MonoBehaviour
{
	// Token: 0x06001097 RID: 4247 RVA: 0x000567EF File Offset: 0x000549EF
	private void Awake()
	{
		this.rig = base.GetComponentInParent<VRRig>();
		this.initialRotation = base.transform.localRotation;
		this.spinAxis = this.initialRotation * this.axisOfRotation * Vector3.forward;
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x00056830 File Offset: 0x00054A30
	private void Update()
	{
		Vector3 vector = (this.optionalVelocityEstimator != null) ? this.optionalVelocityEstimator.linearVelocity : this.rig.LatestVelocity();
		vector.y *= this.verticalSpeedInfluence;
		float num = vector.magnitude / this.maxSpeed;
		float num2 = Time.deltaTime * this.degreesPerSecondAtSpeed.Evaluate(num) * (this.clockwise ? -1f : 1f);
		this.currentAngle = Mathf.Repeat(this.currentAngle + num2, 360f);
		Quaternion quaternion = this.initialRotation * Quaternion.AngleAxis(this.currentAngle, this.spinAxis);
		base.transform.SetLocalPositionAndRotation(quaternion * this.centerOfRotation, quaternion);
		if (this.tickSound != null && this.tickClips.Length != 0)
		{
			this.tickAngle += num2;
			if (this.tickAngle >= this.tickSoundDegrees)
			{
				this.tickSound.pitch = this.tickPitchAtSpeed.Evaluate(num);
				this.tickSound.volume = this.tickVolumeAtSpeed.Evaluate(num);
				this.tickSound.clip = this.tickClips.GetRandomItem<AudioClip>();
				this.tickSound.GTPlay();
				this.tickAngle = Mathf.Repeat(this.tickAngle, this.tickSoundDegrees);
			}
		}
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x00056998 File Offset: 0x00054B98
	private void OnDisable()
	{
		this.currentAngle = 0f;
		this.tickAngle = 0f;
	}

	// Token: 0x0400149B RID: 5275
	[Tooltip("Get the velocity from this component when determining the spin speed. If this is unset, it will use the unsmoothed velocity of the parent VRRig component.")]
	[SerializeField]
	private GorillaVelocityEstimator optionalVelocityEstimator;

	// Token: 0x0400149C RID: 5276
	[SerializeField]
	private Quaternion axisOfRotation = Quaternion.identity;

	// Token: 0x0400149D RID: 5277
	[SerializeField]
	private Vector3 centerOfRotation = Vector3.zero;

	// Token: 0x0400149E RID: 5278
	[Tooltip("The reported speed will be divided by this value before being used to sample AnimationCurves, to allow them to be in the range 0-1.")]
	[SerializeField]
	private float maxSpeed;

	// Token: 0x0400149F RID: 5279
	[SerializeField]
	private AnimationCurve degreesPerSecondAtSpeed;

	// Token: 0x040014A0 RID: 5280
	[SerializeField]
	private bool clockwise;

	// Token: 0x040014A1 RID: 5281
	[Tooltip("The Y component of the reported speed will be multiplied by this value. At 0, falling will have no effect on the rotation speed.")]
	[SerializeField]
	private float verticalSpeedInfluence = 1f;

	// Token: 0x040014A2 RID: 5282
	[Header("Ticking sound")]
	[Tooltip("After this many degrees of rotation, a \"tick\" sound will play.")]
	[SerializeField]
	private float tickSoundDegrees = 360f;

	// Token: 0x040014A3 RID: 5283
	[SerializeField]
	private AnimationCurve tickVolumeAtSpeed;

	// Token: 0x040014A4 RID: 5284
	[SerializeField]
	private AnimationCurve tickPitchAtSpeed;

	// Token: 0x040014A5 RID: 5285
	[SerializeField]
	private AudioSource tickSound;

	// Token: 0x040014A6 RID: 5286
	[SerializeField]
	private AudioClip[] tickClips;

	// Token: 0x040014A7 RID: 5287
	private VRRig rig;

	// Token: 0x040014A8 RID: 5288
	private Quaternion initialRotation;

	// Token: 0x040014A9 RID: 5289
	private Vector3 spinAxis;

	// Token: 0x040014AA RID: 5290
	private float currentAngle;

	// Token: 0x040014AB RID: 5291
	private float tickAngle;
}
