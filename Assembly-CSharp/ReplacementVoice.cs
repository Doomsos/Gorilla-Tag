using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020008DB RID: 2267
public class ReplacementVoice : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003A24 RID: 14884 RVA: 0x00011403 File Offset: 0x0000F603
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003A25 RID: 14885 RVA: 0x0001140C File Offset: 0x0000F60C
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06003A26 RID: 14886 RVA: 0x00133638 File Offset: 0x00131838
	public void SliceUpdate()
	{
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.ShouldPlayReplacementVoice())
		{
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < this.loudReplacementVoiceThreshold)
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClips[Random.Range(0, this.replacementVoiceClips.Length - 1)];
				this.replacementVoiceSource.volume = this.normalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = this.replacementVoiceClipsLoud[Random.Range(0, this.replacementVoiceClipsLoud.Length - 1)];
				this.replacementVoiceSource.volume = this.loudVolume;
			}
			this.replacementVoiceSource.GTPlay();
			return;
		}
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (!this.replacementVoiceSource.isPlaying && this.myVRRig.TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE.VoiceOverride, out cosmeticEffect))
		{
			if (this.myVRRig.SpeakingLoudness < this.myVRRig.replacementVoiceLoudnessThreshold)
			{
				return;
			}
			if (!Mathf.Approximately(this.myVRRig.voiceAudio.pitch, this.replacementVoiceSource.pitch))
			{
				this.replacementVoiceSource.pitch = this.myVRRig.voiceAudio.pitch;
			}
			if (this.myVRRig.SpeakingLoudness < cosmeticEffect.voiceOverrideLoudThreshold)
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideNormalClips[Random.Range(0, cosmeticEffect.voiceOverrideNormalClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideNormalVolume;
			}
			else
			{
				this.replacementVoiceSource.clip = cosmeticEffect.voiceOverrideLoudClips[Random.Range(0, cosmeticEffect.voiceOverrideLoudClips.Length - 1)];
				this.replacementVoiceSource.volume = cosmeticEffect.voiceOverrideLoudVolume;
			}
			this.replacementVoiceSource.GTPlay();
		}
	}

	// Token: 0x0400495A RID: 18778
	public AudioSource replacementVoiceSource;

	// Token: 0x0400495B RID: 18779
	public AudioClip[] replacementVoiceClips;

	// Token: 0x0400495C RID: 18780
	public AudioClip[] replacementVoiceClipsLoud;

	// Token: 0x0400495D RID: 18781
	public float loudReplacementVoiceThreshold = 0.1f;

	// Token: 0x0400495E RID: 18782
	public VRRig myVRRig;

	// Token: 0x0400495F RID: 18783
	public float normalVolume = 0.5f;

	// Token: 0x04004960 RID: 18784
	public float loudVolume = 0.8f;
}
