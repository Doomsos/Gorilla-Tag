using System;
using UnityEngine;

// Token: 0x0200080D RID: 2061
public class HoverboardAudio : MonoBehaviour
{
	// Token: 0x0600363E RID: 13886 RVA: 0x0012638F File Offset: 0x0012458F
	private void Start()
	{
		this.Stop();
	}

	// Token: 0x0600363F RID: 13887 RVA: 0x00126397 File Offset: 0x00124597
	public void PlayTurnSound(float angle)
	{
		if (Time.time > this.turnSoundCooldownUntilTimestamp && angle > this.minAngleDeltaForTurnSound)
		{
			this.turnSoundCooldownUntilTimestamp = Time.time + this.turnSoundCooldownDuration;
			this.turnSounds.Play();
		}
	}

	// Token: 0x06003640 RID: 13888 RVA: 0x001263CC File Offset: 0x001245CC
	public void UpdateAudioLoop(float speed, float airspeed, float strainLevel, float grindLevel)
	{
		this.motorAnimator.UpdateValue(speed, false);
		this.windRushAnimator.UpdateValue(airspeed, false);
		if (grindLevel > 0f)
		{
			this.grindAnimator.UpdatePitchAndVolume(speed, grindLevel + 0.5f, false);
		}
		else
		{
			this.grindAnimator.UpdatePitchAndVolume(0f, 0f, false);
		}
		strainLevel = Mathf.Clamp01(strainLevel * 10f);
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = Mathf.MoveTowards(this.hum1.volume, this.hum1BaseVolume * strainLevel, this.fadeSpeed * Time.deltaTime);
	}

	// Token: 0x06003641 RID: 13889 RVA: 0x00126488 File Offset: 0x00124688
	public void Stop()
	{
		if (!this.didInitHum1BaseVolume)
		{
			this.hum1BaseVolume = this.hum1.volume;
			this.didInitHum1BaseVolume = true;
		}
		this.hum1.volume = 0f;
		this.windRushAnimator.UpdateValue(0f, true);
		this.motorAnimator.UpdateValue(0f, true);
		this.grindAnimator.UpdateValue(0f, true);
	}

	// Token: 0x0400459C RID: 17820
	[SerializeField]
	private AudioSource hum1;

	// Token: 0x0400459D RID: 17821
	[SerializeField]
	private SoundBankPlayer turnSounds;

	// Token: 0x0400459E RID: 17822
	private bool didInitHum1BaseVolume;

	// Token: 0x0400459F RID: 17823
	private float hum1BaseVolume;

	// Token: 0x040045A0 RID: 17824
	[SerializeField]
	private float fadeSpeed;

	// Token: 0x040045A1 RID: 17825
	[SerializeField]
	private AudioAnimator windRushAnimator;

	// Token: 0x040045A2 RID: 17826
	[SerializeField]
	private AudioAnimator motorAnimator;

	// Token: 0x040045A3 RID: 17827
	[SerializeField]
	private AudioAnimator grindAnimator;

	// Token: 0x040045A4 RID: 17828
	[SerializeField]
	private float turnSoundCooldownDuration;

	// Token: 0x040045A5 RID: 17829
	[SerializeField]
	private float minAngleDeltaForTurnSound;

	// Token: 0x040045A6 RID: 17830
	private float turnSoundCooldownUntilTimestamp;
}
