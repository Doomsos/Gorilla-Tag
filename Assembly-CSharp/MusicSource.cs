using System;
using UnityEngine;

// Token: 0x02000859 RID: 2137
[RequireComponent(typeof(AudioSource))]
public class MusicSource : MonoBehaviour
{
	// Token: 0x17000514 RID: 1300
	// (get) Token: 0x06003848 RID: 14408 RVA: 0x0012D5C8 File Offset: 0x0012B7C8
	public AudioSource AudioSource
	{
		get
		{
			return this.audioSource;
		}
	}

	// Token: 0x17000515 RID: 1301
	// (get) Token: 0x06003849 RID: 14409 RVA: 0x0012D5D0 File Offset: 0x0012B7D0
	public float DefaultVolume
	{
		get
		{
			return this.defaultVolume;
		}
	}

	// Token: 0x17000516 RID: 1302
	// (get) Token: 0x0600384A RID: 14410 RVA: 0x0012D5D8 File Offset: 0x0012B7D8
	public bool VolumeOverridden
	{
		get
		{
			return this.volumeOverride != null;
		}
	}

	// Token: 0x0600384B RID: 14411 RVA: 0x0012D5E5 File Offset: 0x0012B7E5
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		if (this.setDefaultVolumeFromAudioSourceOnAwake)
		{
			this.defaultVolume = this.audioSource.volume;
		}
	}

	// Token: 0x0600384C RID: 14412 RVA: 0x0012D61A File Offset: 0x0012B81A
	private void OnEnable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.RegisterMusicSource(this);
		}
	}

	// Token: 0x0600384D RID: 14413 RVA: 0x0012D638 File Offset: 0x0012B838
	private void OnDisable()
	{
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.UnregisterMusicSource(this);
		}
	}

	// Token: 0x0600384E RID: 14414 RVA: 0x0012D656 File Offset: 0x0012B856
	public void SetVolumeOverride(float volume)
	{
		this.volumeOverride = new float?(volume);
		this.audioSource.volume = this.volumeOverride.Value;
	}

	// Token: 0x0600384F RID: 14415 RVA: 0x0012D67A File Offset: 0x0012B87A
	public void UnsetVolumeOverride()
	{
		this.volumeOverride = default(float?);
		this.audioSource.volume = this.defaultVolume;
	}

	// Token: 0x0400475F RID: 18271
	[SerializeField]
	private float defaultVolume = 1f;

	// Token: 0x04004760 RID: 18272
	[SerializeField]
	private bool setDefaultVolumeFromAudioSourceOnAwake = true;

	// Token: 0x04004761 RID: 18273
	private AudioSource audioSource;

	// Token: 0x04004762 RID: 18274
	private float? volumeOverride;
}
