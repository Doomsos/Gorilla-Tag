using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02001069 RID: 4201
	internal static class GTAudioOneShot
	{
		// Token: 0x170009EF RID: 2543
		// (get) Token: 0x06006979 RID: 27001 RVA: 0x0022509F File Offset: 0x0022329F
		// (set) Token: 0x0600697A RID: 27002 RVA: 0x002250A6 File Offset: 0x002232A6
		internal static bool isInitialized { get; private set; }

		// Token: 0x0600697B RID: 27003 RVA: 0x002250B0 File Offset: 0x002232B0
		[RuntimeInitializeOnLoadMethod(1)]
		private static void Initialize()
		{
			if (GTAudioOneShot.isInitialized)
			{
				return;
			}
			AudioSource audioSource = Resources.Load<AudioSource>("AudioSourceSingleton_Prefab");
			if (audioSource == null)
			{
				Debug.LogError("GTAudioOneShot: Failed to load AudioSourceSingleton_Prefab from resources!!!");
				return;
			}
			GTAudioOneShot.audioSource = Object.Instantiate<AudioSource>(audioSource);
			GTAudioOneShot.defaultCurve = GTAudioOneShot.audioSource.GetCustomCurve(0);
			Object.DontDestroyOnLoad(GTAudioOneShot.audioSource);
			GTAudioOneShot.isInitialized = true;
		}

		// Token: 0x0600697C RID: 27004 RVA: 0x0022510F File Offset: 0x0022330F
		internal static void Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.pitch = pitch;
			GTAudioOneShot.audioSource.transform.position = position;
			GTAudioOneShot.audioSource.GTPlayOneShot(clip, volume);
		}

		// Token: 0x0600697D RID: 27005 RVA: 0x00225147 File Offset: 0x00223347
		internal static void Play(AudioClip clip, Vector3 position, AnimationCurve curve, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.SetCustomCurve(0, curve);
			GTAudioOneShot.Play(clip, position, volume, pitch);
			GTAudioOneShot.audioSource.SetCustomCurve(0, GTAudioOneShot.defaultCurve);
		}

		// Token: 0x040078CC RID: 30924
		[OnEnterPlay_SetNull]
		internal static AudioSource audioSource;

		// Token: 0x040078CD RID: 30925
		[OnEnterPlay_SetNull]
		internal static AnimationCurve defaultCurve;
	}
}
