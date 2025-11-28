using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020004DF RID: 1247
public class HypnoRing : MonoBehaviour, ISpawnable
{
	// Token: 0x17000364 RID: 868
	// (get) Token: 0x0600200A RID: 8202 RVA: 0x000AA19D File Offset: 0x000A839D
	// (set) Token: 0x0600200B RID: 8203 RVA: 0x000AA1A5 File Offset: 0x000A83A5
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000365 RID: 869
	// (get) Token: 0x0600200C RID: 8204 RVA: 0x000AA1AE File Offset: 0x000A83AE
	// (set) Token: 0x0600200D RID: 8205 RVA: 0x000AA1B6 File Offset: 0x000A83B6
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x0600200E RID: 8206 RVA: 0x00002789 File Offset: 0x00000989
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x0600200F RID: 8207 RVA: 0x000AA1BF File Offset: 0x000A83BF
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.myRig = rig;
	}

	// Token: 0x06002010 RID: 8208 RVA: 0x000AA1C8 File Offset: 0x000A83C8
	private void Update()
	{
		if ((this.attachedToLeftHand ? this.myRig.leftIndex.calcT : this.myRig.rightIndex.calcT) > 0.5f)
		{
			base.transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * this.rotationSpeed, Vector3.up);
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, this.maxVolume, Time.deltaTime / this.fadeInDuration);
			this.audioSource.volume = this.currentVolume;
			if (!this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
				return;
			}
		}
		else
		{
			this.currentVolume = Mathf.MoveTowards(this.currentVolume, 0f, Time.deltaTime / this.fadeOutDuration);
			if (this.audioSource.isPlaying)
			{
				if (this.currentVolume == 0f)
				{
					this.audioSource.GTStop();
					return;
				}
				this.audioSource.volume = this.currentVolume;
			}
		}
	}

	// Token: 0x04002A62 RID: 10850
	[SerializeField]
	private bool attachedToLeftHand;

	// Token: 0x04002A63 RID: 10851
	private VRRig myRig;

	// Token: 0x04002A64 RID: 10852
	[SerializeField]
	private float rotationSpeed;

	// Token: 0x04002A65 RID: 10853
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002A66 RID: 10854
	[SerializeField]
	private float maxVolume = 1f;

	// Token: 0x04002A67 RID: 10855
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x04002A68 RID: 10856
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x04002A6B RID: 10859
	private float currentVolume;
}
