using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000856 RID: 2134
public class MusicManager : MonoBehaviour
{
	// Token: 0x06003834 RID: 14388 RVA: 0x0012D1E3 File Offset: 0x0012B3E3
	private void Awake()
	{
		if (MusicManager.Instance == null)
		{
			MusicManager.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06003835 RID: 14389 RVA: 0x0012D203 File Offset: 0x0012B403
	public void RegisterMusicSource(MusicSource musicSource)
	{
		if (!this.activeSources.Contains(musicSource))
		{
			this.activeSources.Add(musicSource);
		}
	}

	// Token: 0x06003836 RID: 14390 RVA: 0x0012D220 File Offset: 0x0012B420
	public void UnregisterMusicSource(MusicSource musicSource)
	{
		if (this.activeSources.Contains(musicSource))
		{
			this.activeSources.Remove(musicSource);
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x06003837 RID: 14391 RVA: 0x0012D244 File Offset: 0x0012B444
	public void FadeOutMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeOutVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.SetVolumeOverride(0f);
		}
	}

	// Token: 0x06003838 RID: 14392 RVA: 0x0012D2B8 File Offset: 0x0012B4B8
	public void FadeInMusic(float duration = 3f)
	{
		base.StopAllCoroutines();
		if (duration > 0f)
		{
			base.StartCoroutine(this.FadeInVolumeCoroutine(duration));
			return;
		}
		foreach (MusicSource musicSource in this.activeSources)
		{
			musicSource.UnsetVolumeOverride();
		}
	}

	// Token: 0x06003839 RID: 14393 RVA: 0x0012D328 File Offset: 0x0012B528
	private IEnumerator FadeInVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float volumeOverride = Mathf.MoveTowards(musicSource.AudioSource.volume, musicSource.DefaultVolume, num * deltaTime);
				musicSource.SetVolumeOverride(volumeOverride);
				if (musicSource.AudioSource.volume != musicSource.DefaultVolume)
				{
					complete = false;
				}
			}
			yield return null;
		}
		using (HashSet<MusicSource>.Enumerator enumerator = this.activeSources.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MusicSource musicSource2 = enumerator.Current;
				musicSource2.UnsetVolumeOverride();
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x0600383A RID: 14394 RVA: 0x0012D33E File Offset: 0x0012B53E
	private IEnumerator FadeOutVolumeCoroutine(float duration)
	{
		bool complete = false;
		while (!complete)
		{
			complete = true;
			float deltaTime = Time.deltaTime;
			foreach (MusicSource musicSource in this.activeSources)
			{
				float num = musicSource.DefaultVolume / duration;
				float volumeOverride = Mathf.MoveTowards(musicSource.AudioSource.volume, 0f, num * deltaTime);
				musicSource.SetVolumeOverride(volumeOverride);
				if (musicSource.AudioSource.volume != 0f)
				{
					complete = false;
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04004753 RID: 18259
	[OnEnterPlay_SetNull]
	public static volatile MusicManager Instance;

	// Token: 0x04004754 RID: 18260
	private HashSet<MusicSource> activeSources = new HashSet<MusicSource>();
}
