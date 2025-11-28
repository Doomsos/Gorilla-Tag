using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000298 RID: 664
public class DampenVolumeSpace : MonoBehaviour
{
	// Token: 0x060010F8 RID: 4344 RVA: 0x0005B5D6 File Offset: 0x000597D6
	private void Awake()
	{
		if (this.audioSource == null)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x0005B5F0 File Offset: 0x000597F0
	private void OnTriggerEnter(Collider other)
	{
		GTPlayer componentInParent = other.GetComponentInParent<GTPlayer>();
		if (componentInParent != null && componentInParent == GTPlayer.Instance)
		{
			this.audioSource.volume = this.setVolume;
		}
	}

	// Token: 0x04001541 RID: 5441
	public AudioSource audioSource;

	// Token: 0x04001542 RID: 5442
	public float setVolume;
}
