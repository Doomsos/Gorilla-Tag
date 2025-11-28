using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020005DA RID: 1498
public class EnclosedSpaceVolume : GorillaTriggerBox
{
	// Token: 0x060025BE RID: 9662 RVA: 0x000C99EA File Offset: 0x000C7BEA
	private void Awake()
	{
		this.audioSourceInside.volume = this.quietVolume;
		this.audioSourceOutside.volume = this.loudVolume;
	}

	// Token: 0x060025BF RID: 9663 RVA: 0x000C9A0E File Offset: 0x000C7C0E
	private void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.loudVolume;
			this.audioSourceOutside.volume = this.quietVolume;
		}
	}

	// Token: 0x060025C0 RID: 9664 RVA: 0x000C9A45 File Offset: 0x000C7C45
	private void OnTriggerExit(Collider other)
	{
		if (other.attachedRigidbody.GetComponentInParent<GTPlayer>() != null)
		{
			this.audioSourceInside.volume = this.quietVolume;
			this.audioSourceOutside.volume = this.loudVolume;
		}
	}

	// Token: 0x0400316C RID: 12652
	public AudioSource audioSourceInside;

	// Token: 0x0400316D RID: 12653
	public AudioSource audioSourceOutside;

	// Token: 0x0400316E RID: 12654
	public float loudVolume;

	// Token: 0x0400316F RID: 12655
	public float quietVolume;
}
