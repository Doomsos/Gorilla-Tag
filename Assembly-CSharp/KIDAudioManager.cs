using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000A3C RID: 2620
[DefaultExecutionOrder(0)]
public class KIDAudioManager : MonoBehaviour
{
	// Token: 0x1700063D RID: 1597
	// (get) Token: 0x06004244 RID: 16964 RVA: 0x0015E905 File Offset: 0x0015CB05
	public static KIDAudioManager Instance
	{
		get
		{
			if (!KIDAudioManager._instance)
			{
				if (!ApplicationQuittingState.IsQuitting)
				{
					Debug.LogError("No KIDAudioManager instance found in scene!");
				}
				return null;
			}
			return KIDAudioManager._instance;
		}
	}

	// Token: 0x06004245 RID: 16965 RVA: 0x0015E92C File Offset: 0x0015CB2C
	private void Awake()
	{
		if (KIDAudioManager._instance == null)
		{
			KIDAudioManager._instance = this;
			base.transform.parent = null;
			Object.DontDestroyOnLoad(base.gameObject);
			this.ConfigureAudioSource();
			this.InitializeSoundClips();
			this.mainMixer.GetFloat("Game_Volume", ref this.cachedGameVolume);
			return;
		}
		if (KIDAudioManager._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06004246 RID: 16966 RVA: 0x0015E9A0 File Offset: 0x0015CBA0
	private void ConfigureAudioSource()
	{
		if (this.audioSource != null)
		{
			this.audioSource.outputAudioMixerGroup = this.kidUIGroup;
			this.audioSource.playOnAwake = false;
			this.audioSource.spatialBlend = 0f;
			this.audioSource.volume = 1f;
			this.audioSource.enabled = true;
		}
		if (this.loopingAudioSource != null)
		{
			this.loopingAudioSource.outputAudioMixerGroup = this.kidUIGroup;
			this.loopingAudioSource.playOnAwake = false;
			this.loopingAudioSource.spatialBlend = 0f;
			this.loopingAudioSource.volume = 1f;
			this.loopingAudioSource.loop = true;
			this.loopingAudioSource.enabled = true;
		}
	}

	// Token: 0x06004247 RID: 16967 RVA: 0x0015EA68 File Offset: 0x0015CC68
	private void InitializeSoundClips()
	{
		Dictionary<KIDAudioManager.KIDSoundType, AudioClip> dictionary = new Dictionary<KIDAudioManager.KIDSoundType, AudioClip>();
		dictionary.Add(KIDAudioManager.KIDSoundType.ButtonClick, this.buttonClickSound);
		dictionary.Add(KIDAudioManager.KIDSoundType.Denied, this.deniedSound);
		dictionary.Add(KIDAudioManager.KIDSoundType.Success, this.successSound);
		dictionary.Add(KIDAudioManager.KIDSoundType.Hover, this.buttonHoverSound);
		dictionary.Add(KIDAudioManager.KIDSoundType.ButtonHeld, this.buttonHeldSound);
		dictionary.Add(KIDAudioManager.KIDSoundType.PageTransition, this.pageTransitionSound);
		dictionary.Add(KIDAudioManager.KIDSoundType.InputBack, this.inputBackSound);
		dictionary.Add(KIDAudioManager.KIDSoundType.TurnOffPermission, this.turnOffPermissionSound);
		this.soundClips = dictionary;
	}

	// Token: 0x06004248 RID: 16968 RVA: 0x0015EAE8 File Offset: 0x0015CCE8
	public void SetKIDUIAudioActive(bool active)
	{
		if (!this.IsInstanceValid() || this.isKIDUIActive == active)
		{
			return;
		}
		this.isKIDUIActive = active;
		if (!active)
		{
			this.StopButtonHeldSound();
		}
		if (active)
		{
			this.KIDSnapshot.TransitionTo(0f);
			return;
		}
		this.normalSnapshot.TransitionTo(0f);
	}

	// Token: 0x06004249 RID: 16969 RVA: 0x0015EB3C File Offset: 0x0015CD3C
	public void PlaySound(KIDAudioManager.KIDSoundType soundType)
	{
		if (!this.IsInstanceValid())
		{
			return;
		}
		if (soundType == KIDAudioManager.KIDSoundType.ButtonHeld)
		{
			Debug.LogWarning("[KIDAudioManager] Button held sound is already playing, skipping delayed sound.");
			return;
		}
		AudioClip audioClip;
		if (this.soundClips.TryGetValue(soundType, ref audioClip) && audioClip != null)
		{
			this.audioSource.PlayOneShot(audioClip);
			return;
		}
		Debug.LogWarning(string.Format("[KIDAudioManager] Sound clip for {0} is null or not found!", soundType));
	}

	// Token: 0x0600424A RID: 16970 RVA: 0x0015EB9C File Offset: 0x0015CD9C
	public void StartButtonHeldSound()
	{
		if (!this.IsInstanceValid() || this.buttonHeldSound == null || this.isHoldSoundPlaying)
		{
			return;
		}
		this.loopingAudioSource.clip = this.buttonHeldSound;
		this.loopingAudioSource.Play();
		this.isHoldSoundPlaying = true;
	}

	// Token: 0x0600424B RID: 16971 RVA: 0x0015EBEB File Offset: 0x0015CDEB
	public void StopButtonHeldSound()
	{
		if (!this.IsInstanceValid() || !this.isHoldSoundPlaying)
		{
			return;
		}
		if (this.loopingAudioSource.clip == this.buttonHeldSound)
		{
			this.loopingAudioSource.Stop();
		}
		this.isHoldSoundPlaying = false;
	}

	// Token: 0x0600424C RID: 16972 RVA: 0x0015EC28 File Offset: 0x0015CE28
	private bool IsInstanceValid()
	{
		return !(KIDAudioManager._instance == null) && !(KIDAudioManager._instance != this) && !(this.audioSource == null) && !(this.loopingAudioSource == null);
	}

	// Token: 0x0600424D RID: 16973 RVA: 0x0015EC63 File Offset: 0x0015CE63
	public bool IsKIDUIActive()
	{
		return this.isKIDUIActive;
	}

	// Token: 0x0600424E RID: 16974 RVA: 0x0015EC6B File Offset: 0x0015CE6B
	public void PlaySoundWithDelay(KIDAudioManager.KIDSoundType soundType)
	{
		base.StartCoroutine(this.PlayDelayedSound(soundType, 0.05f));
	}

	// Token: 0x0600424F RID: 16975 RVA: 0x0015EC80 File Offset: 0x0015CE80
	private IEnumerator PlayDelayedSound(KIDAudioManager.KIDSoundType soundType, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.PlaySound(soundType);
		yield break;
	}

	// Token: 0x04005363 RID: 21347
	private static KIDAudioManager _instance;

	// Token: 0x04005364 RID: 21348
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04005365 RID: 21349
	[SerializeField]
	private AudioSource loopingAudioSource;

	// Token: 0x04005366 RID: 21350
	[SerializeField]
	private AudioMixer mainMixer;

	// Token: 0x04005367 RID: 21351
	[SerializeField]
	private AudioMixerSnapshot KIDSnapshot;

	// Token: 0x04005368 RID: 21352
	[SerializeField]
	private AudioMixerSnapshot normalSnapshot;

	// Token: 0x04005369 RID: 21353
	[SerializeField]
	private AudioMixerGroup kidUIGroup;

	// Token: 0x0400536A RID: 21354
	[SerializeField]
	private AudioClip buttonClickSound;

	// Token: 0x0400536B RID: 21355
	[SerializeField]
	private AudioClip deniedSound;

	// Token: 0x0400536C RID: 21356
	[SerializeField]
	private AudioClip successSound;

	// Token: 0x0400536D RID: 21357
	[SerializeField]
	private AudioClip buttonHoverSound;

	// Token: 0x0400536E RID: 21358
	[SerializeField]
	private AudioClip buttonHeldSound;

	// Token: 0x0400536F RID: 21359
	[SerializeField]
	private AudioClip pageTransitionSound;

	// Token: 0x04005370 RID: 21360
	[SerializeField]
	private AudioClip inputBackSound;

	// Token: 0x04005371 RID: 21361
	[SerializeField]
	private AudioClip turnOffPermissionSound;

	// Token: 0x04005372 RID: 21362
	private const string GAME_VOLUME = "Game_Volume";

	// Token: 0x04005373 RID: 21363
	private const string KID_VOLUME = "KID_UI_Volume";

	// Token: 0x04005374 RID: 21364
	private const float MUTED_VALUE = -80f;

	// Token: 0x04005375 RID: 21365
	private const float UNMUTED_VALUE = 0f;

	// Token: 0x04005376 RID: 21366
	private bool isKIDUIActive;

	// Token: 0x04005377 RID: 21367
	private float cachedGameVolume;

	// Token: 0x04005378 RID: 21368
	private bool isHoldSoundPlaying;

	// Token: 0x04005379 RID: 21369
	private Dictionary<KIDAudioManager.KIDSoundType, AudioClip> soundClips;

	// Token: 0x02000A3D RID: 2621
	public enum KIDSoundType
	{
		// Token: 0x0400537B RID: 21371
		ButtonClick,
		// Token: 0x0400537C RID: 21372
		Hover,
		// Token: 0x0400537D RID: 21373
		Success,
		// Token: 0x0400537E RID: 21374
		Denied,
		// Token: 0x0400537F RID: 21375
		InputBack,
		// Token: 0x04005380 RID: 21376
		TurnOffPermission,
		// Token: 0x04005381 RID: 21377
		PageTransition,
		// Token: 0x04005382 RID: 21378
		ButtonHeld
	}
}
