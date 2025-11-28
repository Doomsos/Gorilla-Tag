using System;
using System.Collections;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000852 RID: 2130
public class MusicFadeArea : MonoBehaviour
{
	// Token: 0x06003822 RID: 14370 RVA: 0x0012CDAC File Offset: 0x0012AFAC
	private void Awake()
	{
		for (int i = 0; i < this.sourcesToFadeIn.Count; i++)
		{
			this.sourcesToFadeIn[i].audioSource.Stop();
			this.sourcesToFadeIn[i].audioSource.volume = 0f;
		}
	}

	// Token: 0x06003823 RID: 14371 RVA: 0x0012CE00 File Offset: 0x0012B000
	private void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			MusicManager.Instance.FadeOutMusic(this.fadeDuration);
			if (this.fadeCoroutine != null)
			{
				base.StopCoroutine(this.fadeCoroutine);
			}
			if (this.sourcesToFadeIn.Count > 0)
			{
				this.fadeCoroutine = base.StartCoroutine(this.FadeInSources());
			}
		}
	}

	// Token: 0x06003824 RID: 14372 RVA: 0x0012CE68 File Offset: 0x0012B068
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			MusicManager.Instance.FadeInMusic(this.fadeDuration);
			if (this.fadeCoroutine != null)
			{
				base.StopCoroutine(this.fadeCoroutine);
			}
			if (this.sourcesToFadeIn.Count > 0)
			{
				this.fadeCoroutine = base.StartCoroutine(this.FadeOutSources());
			}
		}
	}

	// Token: 0x06003825 RID: 14373 RVA: 0x0012CECD File Offset: 0x0012B0CD
	private IEnumerator FadeInSources()
	{
		for (int i = 0; i < this.sourcesToFadeIn.Count; i++)
		{
			this.sourcesToFadeIn[i].audioSource.Play();
			this.sourcesToFadeIn[i].audioSource.volume = this.sourcesToFadeIn[i].maxVolume * this.fadeProgress;
		}
		while (this.fadeProgress < 1f)
		{
			for (int j = 0; j < this.sourcesToFadeIn.Count; j++)
			{
				this.sourcesToFadeIn[j].audioSource.volume = this.sourcesToFadeIn[j].maxVolume * this.fadeProgress;
			}
			yield return null;
			this.fadeProgress = Mathf.MoveTowards(this.fadeProgress, 1f, Time.deltaTime / this.fadeDuration);
		}
		for (int k = 0; k < this.sourcesToFadeIn.Count; k++)
		{
			this.sourcesToFadeIn[k].audioSource.volume = this.sourcesToFadeIn[k].maxVolume;
		}
		yield break;
	}

	// Token: 0x06003826 RID: 14374 RVA: 0x0012CEDC File Offset: 0x0012B0DC
	private IEnumerator FadeOutSources()
	{
		for (int i = 0; i < this.sourcesToFadeIn.Count; i++)
		{
			this.sourcesToFadeIn[i].audioSource.volume = this.sourcesToFadeIn[i].maxVolume * this.fadeProgress;
		}
		while (this.fadeProgress > 0f)
		{
			for (int j = 0; j < this.sourcesToFadeIn.Count; j++)
			{
				this.sourcesToFadeIn[j].audioSource.volume = this.sourcesToFadeIn[j].maxVolume * this.fadeProgress;
			}
			yield return null;
			this.fadeProgress = Mathf.MoveTowards(this.fadeProgress, 0f, Time.deltaTime / this.fadeDuration);
		}
		for (int k = 0; k < this.sourcesToFadeIn.Count; k++)
		{
			this.sourcesToFadeIn[k].audioSource.Stop();
			this.sourcesToFadeIn[k].audioSource.volume = 0f;
		}
		yield break;
	}

	// Token: 0x04004747 RID: 18247
	[SerializeField]
	private List<MusicFadeArea.AudioSourceEntry> sourcesToFadeIn = new List<MusicFadeArea.AudioSourceEntry>();

	// Token: 0x04004748 RID: 18248
	[SerializeField]
	private float fadeDuration = 3f;

	// Token: 0x04004749 RID: 18249
	private float fadeProgress;

	// Token: 0x0400474A RID: 18250
	private Coroutine fadeCoroutine;

	// Token: 0x02000853 RID: 2131
	[Serializable]
	public struct AudioSourceEntry
	{
		// Token: 0x0400474B RID: 18251
		public AudioSource audioSource;

		// Token: 0x0400474C RID: 18252
		public float maxVolume;
	}
}
