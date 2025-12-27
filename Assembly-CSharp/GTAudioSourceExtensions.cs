using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class GTAudioSourceExtensions
{
	[MethodImpl(256)]
	public static void GTPlayOneShot(this AudioSource audioSource, IList<AudioClip> clips, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)], volumeScale);
	}

	[MethodImpl(256)]
	public static void GTPlayOneShot(this AudioSource audioSource, AudioClip clip, float volumeScale = 1f)
	{
		audioSource.PlayOneShot(clip, volumeScale);
	}

	[MethodImpl(256)]
	public static void GTPlay(this AudioSource audioSource)
	{
		audioSource.Play();
	}

	[MethodImpl(256)]
	public static void GTPlay(this AudioSource audioSource, ulong delay)
	{
		audioSource.Play(delay);
	}

	[MethodImpl(256)]
	public static void GTPause(this AudioSource audioSource)
	{
		audioSource.Pause();
	}

	[MethodImpl(256)]
	public static void GTUnPause(this AudioSource audioSource)
	{
		audioSource.UnPause();
	}

	[MethodImpl(256)]
	public static void GTStop(this AudioSource audioSource)
	{
		audioSource.Stop();
	}

	[MethodImpl(256)]
	public static void GTPlayDelayed(this AudioSource audioSource, float delay)
	{
		audioSource.PlayDelayed(delay);
	}

	[MethodImpl(256)]
	public static void GTPlayScheduled(this AudioSource audioSource, double time)
	{
		audioSource.PlayScheduled(time);
	}

	[MethodImpl(256)]
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position)
	{
		AudioSource.PlayClipAtPoint(clip, position);
	}

	[MethodImpl(256)]
	public static void GTPlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
	{
		AudioSource.PlayClipAtPoint(clip, position, volume);
	}

	[Conditional("BETA")]
	[Conditional("UNITY_EDITOR")]
	private static void _BetaLogIfAudioSourceIsNotActiveAndEnabled(AudioSource audioSource)
	{
	}
}
