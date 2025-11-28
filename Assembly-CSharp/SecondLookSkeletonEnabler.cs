using System;
using UnityEngine;

// Token: 0x02000186 RID: 390
public class SecondLookSkeletonEnabler : Tappable
{
	// Token: 0x06000A6D RID: 2669 RVA: 0x00038C47 File Offset: 0x00036E47
	private void Awake()
	{
		this.isTapped = false;
		this.skele = Object.FindFirstObjectByType<SecondLookSkeleton>();
		this.skele.spookyText = this.spookyText;
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x00038C6C File Offset: 0x00036E6C
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!this.isTapped)
		{
			base.OnTapLocal(tapStrength, tapTime, info);
			if (this.skele != null)
			{
				this.skele.tapped = true;
			}
			base.gameObject.SetActive(false);
			this.isTapped = true;
			this.playOnDisappear.GTPlay();
			this.particles.Play();
		}
	}

	// Token: 0x04000CC9 RID: 3273
	public bool isTapped;

	// Token: 0x04000CCA RID: 3274
	public AudioSource playOnDisappear;

	// Token: 0x04000CCB RID: 3275
	public ParticleSystem particles;

	// Token: 0x04000CCC RID: 3276
	public GameObject spookyText;

	// Token: 0x04000CCD RID: 3277
	private SecondLookSkeleton skele;
}
