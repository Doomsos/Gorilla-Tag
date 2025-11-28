using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020002B6 RID: 694
public static class GTAudioSourceExtensions
{
	// Token: 0x0600112E RID: 4398 RVA: 0x0005BE0B File Offset: 0x0005A00B
	[MethodImpl(256)]
	public static void GTPlayOneShot(this AudioSource audioSource, IList<AudioClip> clips, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)], volumeScale);
	}

	// Token: 0x0600112F RID: 4399 RVA: 0x0005BE26 File Offset: 0x0005A026
	[MethodImpl(256)]
	public static void GTPlayOneShot(this AudioSource audioSource, AudioClip clip, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clip, volumeScale);
	}

	// Token: 0x06001130 RID: 4400 RVA: 0x0005BE30 File Offset: 0x0005A030
	[MethodImpl(256)]
	public static void GTPlay(this AudioSource audioSource)
	{
		audioSource.Play();
	}

	// Token: 0x06001131 RID: 4401 RVA: 0x0005BE38 File Offset: 0x0005A038
	[MethodImpl(256)]
	public static void GTPlay(this AudioSource audioSource, ulong delay)
	{
		audioSource.Play(delay);
	}

	// Token: 0x06001132 RID: 4402 RVA: 0x0005BE41 File Offset: 0x0005A041
	[MethodImpl(256)]
	public static void GTPause(this AudioSource audioSource)
	{
		audioSource.Pause();
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x0005BE49 File Offset: 0x0005A049
	[MethodImpl(256)]
	public static void GTUnPause(this AudioSource audioSource)
	{
		audioSource.UnPause();
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0005BE51 File Offset: 0x0005A051
	[MethodImpl(256)]
	public static void GTStop(this AudioSource audioSource)
	{
		audioSource.Stop();
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0005BE59 File Offset: 0x0005A059
	[MethodImpl(256)]
	public static void GTPlayDelayed(this AudioSource audioSource, float delay)
	{
		audioSource.PlayDelayed(delay);
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0005BE62 File Offset: 0x0005A062
	[MethodImpl(256)]
	public static void GTPlayScheduled(this AudioSource audioSource, double time)
	{
		audioSource.PlayScheduled(time);
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0005BE6B File Offset: 0x0005A06B
	[MethodImpl(256)]
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position)
	{
		AudioSource.PlayClipAtPoint(clip, position);
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x0005BE74 File Offset: 0x0005A074
	[MethodImpl(256)]
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
	{
		AudioSource.PlayClipAtPoint(clip, position, volume);
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x00002789 File Offset: 0x00000989
	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	private static void _BetaLogIfAudioSourceIsNotActiveAndEnabled(AudioSource audioSource)
	{
	}
}
