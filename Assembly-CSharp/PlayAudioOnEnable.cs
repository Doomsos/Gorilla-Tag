using System;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class PlayAudioOnEnable : MonoBehaviour
{
	// Token: 0x060006EF RID: 1775 RVA: 0x000262E3 File Offset: 0x000244E3
	private void OnEnable()
	{
		this.audioSource.clip = this.audioClips[Random.Range(0, this.audioClips.Length)];
		this.audioSource.GTPlay();
	}

	// Token: 0x040008C1 RID: 2241
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040008C2 RID: 2242
	[SerializeField]
	private AudioClip[] audioClips;
}
