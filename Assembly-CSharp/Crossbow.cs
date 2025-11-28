using System;
using UnityEngine;

// Token: 0x02000247 RID: 583
public class Crossbow : ProjectileWeapon
{
	// Token: 0x06000F35 RID: 3893 RVA: 0x00050D7C File Offset: 0x0004EF7C
	protected override void Awake()
	{
		base.Awake();
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCrank));
		}
		this.SetReloadFraction(0f);
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x00050DC4 File Offset: 0x0004EFC4
	public void SetReloadFraction(float newFraction)
	{
		this.loadFraction = Mathf.Clamp01(newFraction);
		this.animator.SetFloat(this.ReloadFractionHashID, this.loadFraction);
		if (this.loadFraction == 1f && !this.dummyProjectile.enabled)
		{
			this.shootSfx.GTPlayOneShot(this.reloadComplete_audioClip, 1f);
			this.dummyProjectile.enabled = true;
			return;
		}
		if (this.loadFraction < 1f && this.dummyProjectile.enabled)
		{
			this.dummyProjectile.enabled = false;
		}
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x00050E5C File Offset: 0x0004F05C
	private void OnCrank(float degrees)
	{
		if (this.loadFraction == 1f)
		{
			return;
		}
		this.totalCrankDegrees += degrees;
		this.crankSoundDegrees += degrees;
		if (Mathf.Abs(this.crankSoundDegrees) > this.crankSoundDegreesThreshold)
		{
			this.playingCrankSoundUntilTimestamp = Time.time + this.crankSoundContinueDuration;
			this.crankSoundDegrees = 0f;
		}
		if (!this.reloadAudio.isPlaying && Time.time < this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTPlay();
		}
		this.SetReloadFraction(Mathf.Abs(this.totalCrankDegrees / this.crankTotalDegreesToReload));
		if (this.loadFraction >= 1f)
		{
			this.totalCrankDegrees = 0f;
		}
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x00050F18 File Offset: 0x0004F118
	protected override Vector3 GetLaunchPosition()
	{
		return this.launchPosition.position;
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x00050F25 File Offset: 0x0004F125
	protected override Vector3 GetLaunchVelocity()
	{
		return this.launchPosition.forward * this.launchSpeed * base.myRig.scaleFactor;
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x00050F50 File Offset: 0x0004F150
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			this.wasPressingTrigger = false;
			return;
		}
		if ((base.InLeftHand() ? base.myRig.leftIndex.calcT : base.myRig.rightIndex.calcT) > 0.5f)
		{
			if (this.loadFraction == 1f && !this.wasPressingTrigger)
			{
				this.SetReloadFraction(0f);
				this.animator.SetTrigger(this.FireHashID);
				base.LaunchProjectile();
			}
			this.wasPressingTrigger = true;
		}
		else
		{
			this.wasPressingTrigger = false;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (this.loadFraction < 1f)
			{
				this.itemState &= (TransferrableObject.ItemStates)(-2);
				return;
			}
		}
		else if (this.loadFraction == 1f)
		{
			this.itemState |= TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x00051040 File Offset: 0x0004F240
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			this.SetReloadFraction(1f);
			return;
		}
		if (this.loadFraction == 1f)
		{
			this.SetReloadFraction(0f);
		}
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x00051098 File Offset: 0x0004F298
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.reloadAudio.isPlaying && Time.time > this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTStop();
		}
	}

	// Token: 0x040012B7 RID: 4791
	[SerializeField]
	private Transform launchPosition;

	// Token: 0x040012B8 RID: 4792
	[SerializeField]
	private float launchSpeed;

	// Token: 0x040012B9 RID: 4793
	[SerializeField]
	private Animator animator;

	// Token: 0x040012BA RID: 4794
	[SerializeField]
	private float crankTotalDegreesToReload;

	// Token: 0x040012BB RID: 4795
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x040012BC RID: 4796
	[SerializeField]
	private MeshRenderer dummyProjectile;

	// Token: 0x040012BD RID: 4797
	[SerializeField]
	private AudioSource reloadAudio;

	// Token: 0x040012BE RID: 4798
	[SerializeField]
	private AudioClip reloadComplete_audioClip;

	// Token: 0x040012BF RID: 4799
	[SerializeField]
	private float crankSoundContinueDuration = 0.1f;

	// Token: 0x040012C0 RID: 4800
	[SerializeField]
	private float crankSoundDegreesThreshold = 0.1f;

	// Token: 0x040012C1 RID: 4801
	private AnimHashId FireHashID = "Fire";

	// Token: 0x040012C2 RID: 4802
	private AnimHashId ReloadFractionHashID = "ReloadFraction";

	// Token: 0x040012C3 RID: 4803
	private float totalCrankDegrees;

	// Token: 0x040012C4 RID: 4804
	private float loadFraction;

	// Token: 0x040012C5 RID: 4805
	private float playingCrankSoundUntilTimestamp;

	// Token: 0x040012C6 RID: 4806
	private float crankSoundDegrees;

	// Token: 0x040012C7 RID: 4807
	private bool wasPressingTrigger;
}
