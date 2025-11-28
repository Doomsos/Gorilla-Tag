using System;
using UnityEngine;

// Token: 0x02000805 RID: 2053
public class HeightVolume : MonoBehaviour
{
	// Token: 0x06003609 RID: 13833 RVA: 0x001251BD File Offset: 0x001233BD
	private void Awake()
	{
		if (this.targetTransform == null)
		{
			this.targetTransform = Camera.main.transform;
		}
		this.musicSource = this.audioSource.gameObject.GetComponent<MusicSource>();
	}

	// Token: 0x0600360A RID: 13834 RVA: 0x001251F4 File Offset: 0x001233F4
	private void Update()
	{
		if (this.audioSource.gameObject.activeSelf && (!(this.musicSource != null) || !this.musicSource.VolumeOverridden))
		{
			if (this.targetTransform.position.y > this.heightTop.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.baseVolume : this.minVolume);
				return;
			}
			if (this.targetTransform.position.y < this.heightBottom.position.y)
			{
				this.audioSource.volume = ((!this.invertHeightVol) ? this.minVolume : this.baseVolume);
				return;
			}
			this.audioSource.volume = ((!this.invertHeightVol) ? ((this.targetTransform.position.y - this.heightBottom.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume) : ((this.heightTop.position.y - this.targetTransform.position.y) / (this.heightTop.position.y - this.heightBottom.position.y) * (this.baseVolume - this.minVolume) + this.minVolume));
		}
	}

	// Token: 0x0400455D RID: 17757
	public Transform heightTop;

	// Token: 0x0400455E RID: 17758
	public Transform heightBottom;

	// Token: 0x0400455F RID: 17759
	public AudioSource audioSource;

	// Token: 0x04004560 RID: 17760
	public float baseVolume;

	// Token: 0x04004561 RID: 17761
	public float minVolume;

	// Token: 0x04004562 RID: 17762
	public Transform targetTransform;

	// Token: 0x04004563 RID: 17763
	public bool invertHeightVol;

	// Token: 0x04004564 RID: 17764
	private MusicSource musicSource;
}
