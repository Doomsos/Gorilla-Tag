using System;
using UnityEngine;

// Token: 0x020001AA RID: 426
public class VacuumHoldable : TransferrableObject
{
	// Token: 0x06000B5C RID: 2908 RVA: 0x0003DB04 File Offset: 0x0003BD04
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06000B5D RID: 2909 RVA: 0x0003DB14 File Offset: 0x0003BD14
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasAudioSource = (this.audioSource != null && this.audioSource.clip != null);
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x0003DB4C File Offset: 0x0003BD4C
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x06000B5F RID: 2911 RVA: 0x0003DBA0 File Offset: 0x0003BDA0
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x06000B60 RID: 2912 RVA: 0x0003DBEC File Offset: 0x0003BDEC
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000B61 RID: 2913 RVA: 0x0003DBFC File Offset: 0x0003BDFC
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.particleFX.isPlaying)
			{
				this.particleFX.Stop();
			}
			if (this.hasAudioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
				return;
			}
		}
		else
		{
			if (!this.particleFX.isEmitting)
			{
				this.particleFX.Play();
			}
			if (this.hasAudioSource && !this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
			}
			if (this.IsMyItem() && Time.time > this.activationStartTime + this.activationVibrationStartDuration)
			{
				GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationLoopStrength, Time.deltaTime);
			}
		}
	}

	// Token: 0x06000B62 RID: 2914 RVA: 0x0003DCF0 File Offset: 0x0003BEF0
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
		if (this.IsMyItem())
		{
			this.activationStartTime = Time.time;
			GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationStartStrength, this.activationVibrationStartDuration);
		}
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x0003DD3C File Offset: 0x0003BF3C
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x04000DE8 RID: 3560
	[Tooltip("Emission rate will be increase when the trigger button is pressed.")]
	public ParticleSystem particleFX;

	// Token: 0x04000DE9 RID: 3561
	[Tooltip("Sound will loop and fade in/out volume when trigger pressed.")]
	public AudioSource audioSource;

	// Token: 0x04000DEA RID: 3562
	private float activationVibrationStartStrength = 0.8f;

	// Token: 0x04000DEB RID: 3563
	private float activationVibrationStartDuration = 0.05f;

	// Token: 0x04000DEC RID: 3564
	private float activationVibrationLoopStrength = 0.005f;

	// Token: 0x04000DED RID: 3565
	private float activationStartTime;

	// Token: 0x04000DEE RID: 3566
	private bool hasAudioSource;

	// Token: 0x020001AB RID: 427
	private enum VacuumState
	{
		// Token: 0x04000DF0 RID: 3568
		None = 1,
		// Token: 0x04000DF1 RID: 3569
		Active
	}
}
