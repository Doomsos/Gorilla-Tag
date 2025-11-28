using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200000F RID: 15
[RequireComponent(typeof(AudioSource))]
public class AudioSourceClipRandomizer : MonoBehaviour
{
	// Token: 0x06000042 RID: 66 RVA: 0x000029DF File Offset: 0x00000BDF
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.playOnAwake = this.source.playOnAwake;
		this.source.playOnAwake = false;
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00002A0C File Offset: 0x00000C0C
	public void Play()
	{
		int num = Random.Range(0, 60);
		if (GorillaComputer.instance != null)
		{
			num = GorillaComputer.instance.GetServerTime().Second;
		}
		this.source.clip = this.clips[num % this.clips.Length];
		this.source.GTPlay();
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00002A6D File Offset: 0x00000C6D
	private void OnEnable()
	{
		if (this.playOnAwake)
		{
			this.Play();
		}
	}

	// Token: 0x04000027 RID: 39
	[SerializeField]
	private AudioClip[] clips;

	// Token: 0x04000028 RID: 40
	private AudioSource source;

	// Token: 0x04000029 RID: 41
	private bool playOnAwake;
}
