using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000C73 RID: 3187
public class SoundBankPlayer : MonoBehaviour
{
	// Token: 0x17000743 RID: 1859
	// (get) Token: 0x06004DCF RID: 19919 RVA: 0x001926FC File Offset: 0x001908FC
	public bool isPlaying
	{
		get
		{
			return Time.realtimeSinceStartup < this.playEndTime;
		}
	}

	// Token: 0x17000744 RID: 1860
	// (get) Token: 0x06004DD0 RID: 19920 RVA: 0x0019270B File Offset: 0x0019090B
	public float NormalizedTime
	{
		get
		{
			if (this.clipDuration != 0f)
			{
				return Mathf.Clamp01(this.CurrentTime / this.clipDuration);
			}
			return 1f;
		}
	}

	// Token: 0x17000745 RID: 1861
	// (get) Token: 0x06004DD1 RID: 19921 RVA: 0x00192732 File Offset: 0x00190932
	public float CurrentTime
	{
		get
		{
			return Time.realtimeSinceStartup - this.playStartTime;
		}
	}

	// Token: 0x06004DD2 RID: 19922 RVA: 0x00192740 File Offset: 0x00190940
	protected void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.gameObject.AddComponent<AudioSource>();
			this.audioSource.outputAudioMixerGroup = this.outputAudioMixerGroup;
			this.audioSource.spatialize = this.spatialize;
			this.audioSource.spatializePostEffects = this.spatializePostEffects;
			this.audioSource.bypassEffects = this.bypassEffects;
			this.audioSource.bypassListenerEffects = this.bypassListenerEffects;
			this.audioSource.bypassReverbZones = this.bypassReverbZones;
			this.audioSource.priority = this.priority;
			this.audioSource.spatialBlend = this.spatialBlend;
			this.audioSource.dopplerLevel = this.dopplerLevel;
			this.audioSource.spread = this.spread;
			this.audioSource.rolloffMode = this.rolloffMode;
			this.audioSource.minDistance = this.minDistance;
			this.audioSource.maxDistance = this.maxDistance;
			this.audioSource.reverbZoneMix = this.reverbZoneMix;
		}
		this.audioSource.volume = 1f;
		this.audioSource.playOnAwake = false;
		if (this.shuffleOrder)
		{
			int[] array = new int[this.soundBank.sounds.Length / 2];
			this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
			for (int i = 0; i < this.playlist.Length; i++)
			{
				int num = 0;
				for (int j = 0; j < 100; j++)
				{
					num = Random.Range(0, this.soundBank.sounds.Length);
					if (Array.IndexOf<int>(array, num) == -1)
					{
						break;
					}
				}
				if (array.Length != 0)
				{
					array[i % array.Length] = num;
				}
				this.playlist[i] = new SoundBankPlayer.PlaylistEntry
				{
					index = num,
					volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
					pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
				};
			}
			return;
		}
		this.playlist = new SoundBankPlayer.PlaylistEntry[this.soundBank.sounds.Length * 8];
		for (int k = 0; k < this.playlist.Length; k++)
		{
			this.playlist[k] = new SoundBankPlayer.PlaylistEntry
			{
				index = k % this.soundBank.sounds.Length,
				volume = Random.Range(this.soundBank.volumeRange.x, this.soundBank.volumeRange.y),
				pitch = Random.Range(this.soundBank.pitchRange.x, this.soundBank.pitchRange.y)
			};
		}
	}

	// Token: 0x06004DD3 RID: 19923 RVA: 0x00192A39 File Offset: 0x00190C39
	protected void OnEnable()
	{
		if (this.playOnEnable)
		{
			this.Play();
		}
	}

	// Token: 0x06004DD4 RID: 19924 RVA: 0x00192A4C File Offset: 0x00190C4C
	public void Play()
	{
		this.Play(default(float?), default(float?));
	}

	// Token: 0x06004DD5 RID: 19925 RVA: 0x00192A74 File Offset: 0x00190C74
	public void Play(float? volumeOverride = null, float? pitchOverride = null)
	{
		if (!base.enabled || this.soundBank.sounds.Length == 0 || this.playlist == null)
		{
			return;
		}
		SoundBankPlayer.PlaylistEntry playlistEntry = this.playlist[this.nextIndex];
		this.audioSource.pitch = ((pitchOverride != null) ? pitchOverride.Value : playlistEntry.pitch);
		AudioClip audioClip = this.soundBank.sounds[playlistEntry.index];
		if (audioClip != null)
		{
			this.audioSource.GTPlayOneShot(audioClip, (volumeOverride != null) ? volumeOverride.Value : playlistEntry.volume);
			this.clipDuration = audioClip.length;
			this.playStartTime = Time.realtimeSinceStartup;
			this.playEndTime = Mathf.Max(this.playEndTime, this.playStartTime + audioClip.length);
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		if (this.missingSoundsAreOk)
		{
			this.clipDuration = 0f;
			this.nextIndex = (this.nextIndex + 1) % this.playlist.Length;
			return;
		}
		Debug.LogErrorFormat("Sounds bank {0} is missing a clip at {1}", new object[]
		{
			base.gameObject.name,
			playlistEntry.index
		});
	}

	// Token: 0x06004DD6 RID: 19926 RVA: 0x00192BB9 File Offset: 0x00190DB9
	public void RestartSequence()
	{
		this.nextIndex = 0;
	}

	// Token: 0x04005CFC RID: 23804
	[Tooltip("Optional. AudioSource Settings will be used if this is not defined.")]
	public AudioSource audioSource;

	// Token: 0x04005CFD RID: 23805
	public bool playOnEnable = true;

	// Token: 0x04005CFE RID: 23806
	public bool shuffleOrder = true;

	// Token: 0x04005CFF RID: 23807
	public bool missingSoundsAreOk;

	// Token: 0x04005D00 RID: 23808
	public SoundBankSO soundBank;

	// Token: 0x04005D01 RID: 23809
	public AudioMixerGroup outputAudioMixerGroup;

	// Token: 0x04005D02 RID: 23810
	public bool spatialize;

	// Token: 0x04005D03 RID: 23811
	public bool spatializePostEffects;

	// Token: 0x04005D04 RID: 23812
	public bool bypassEffects;

	// Token: 0x04005D05 RID: 23813
	public bool bypassListenerEffects;

	// Token: 0x04005D06 RID: 23814
	public bool bypassReverbZones;

	// Token: 0x04005D07 RID: 23815
	public int priority = 128;

	// Token: 0x04005D08 RID: 23816
	[Range(0f, 1f)]
	public float spatialBlend = 1f;

	// Token: 0x04005D09 RID: 23817
	public float reverbZoneMix = 1f;

	// Token: 0x04005D0A RID: 23818
	public float dopplerLevel = 1f;

	// Token: 0x04005D0B RID: 23819
	public float spread;

	// Token: 0x04005D0C RID: 23820
	public AudioRolloffMode rolloffMode;

	// Token: 0x04005D0D RID: 23821
	public float minDistance = 1f;

	// Token: 0x04005D0E RID: 23822
	public float maxDistance = 100f;

	// Token: 0x04005D0F RID: 23823
	public AnimationCurve customRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04005D10 RID: 23824
	private int nextIndex;

	// Token: 0x04005D11 RID: 23825
	private float playStartTime;

	// Token: 0x04005D12 RID: 23826
	private float playEndTime;

	// Token: 0x04005D13 RID: 23827
	private float clipDuration;

	// Token: 0x04005D14 RID: 23828
	private SoundBankPlayer.PlaylistEntry[] playlist;

	// Token: 0x02000C74 RID: 3188
	private struct PlaylistEntry
	{
		// Token: 0x04005D15 RID: 23829
		public int index;

		// Token: 0x04005D16 RID: 23830
		public float volume;

		// Token: 0x04005D17 RID: 23831
		public float pitch;
	}
}
