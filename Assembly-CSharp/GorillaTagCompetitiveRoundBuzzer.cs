using System;
using UnityEngine;

// Token: 0x020007BD RID: 1981
public class GorillaTagCompetitiveRoundBuzzer : MonoBehaviour
{
	// Token: 0x06003431 RID: 13361 RVA: 0x001185F7 File Offset: 0x001167F7
	private void OnEnable()
	{
		GorillaTagCompetitiveManager.onStateChanged += new Action<GorillaTagCompetitiveManager.GameState>(this.OnStateChanged);
		GorillaTagCompetitiveManager.onUpdateRemainingTime += new Action<float>(this.OnUpdateRemainingTime);
	}

	// Token: 0x06003432 RID: 13362 RVA: 0x0011861B File Offset: 0x0011681B
	private void OnDisable()
	{
		GorillaTagCompetitiveManager.onStateChanged -= new Action<GorillaTagCompetitiveManager.GameState>(this.OnStateChanged);
		GorillaTagCompetitiveManager.onUpdateRemainingTime -= new Action<float>(this.OnUpdateRemainingTime);
	}

	// Token: 0x06003433 RID: 13363 RVA: 0x00118640 File Offset: 0x00116840
	private void OnStateChanged(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			this.PlaySFX(this.needMorePlayerClip);
			break;
		case GorillaTagCompetitiveManager.GameState.Playing:
			this.PlaySFX(this.roundStartClip);
			break;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			this.PlaySFX(this.roundEndClip);
			break;
		}
		this.lastState = newState;
	}

	// Token: 0x06003434 RID: 13364 RVA: 0x00118698 File Offset: 0x00116898
	private void OnUpdateRemainingTime(float remainingTime)
	{
		int num = Mathf.CeilToInt(remainingTime);
		int num2 = Mathf.CeilToInt(this.lastStateRemainingTime);
		if (num != num2)
		{
			GorillaTagCompetitiveManager.GameState gameState = this.lastState;
			if (gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown)
			{
				if (gameState == GorillaTagCompetitiveManager.GameState.Playing)
				{
					if (num > 0 && num <= this.roundEndCountdownDuration)
					{
						this.PlaySFX(this.roundEndingCountdownClip);
					}
				}
			}
			else if (num > 0)
			{
				this.PlaySFX(this.roundCountdownClip);
			}
		}
		this.lastStateRemainingTime = remainingTime;
	}

	// Token: 0x06003435 RID: 13365 RVA: 0x001186FF File Offset: 0x001168FF
	private void PlaySFX(AudioClip clip)
	{
		this.PlaySFX(clip, 1f);
	}

	// Token: 0x06003436 RID: 13366 RVA: 0x0011870D File Offset: 0x0011690D
	private void PlaySFX(AudioClip clip, float volume)
	{
		this.audioSource.PlayOneShot(clip, volume);
	}

	// Token: 0x0400427B RID: 17019
	public AudioSource audioSource;

	// Token: 0x0400427C RID: 17020
	public AudioClip roundCountdownClip;

	// Token: 0x0400427D RID: 17021
	public AudioClip roundStartClip;

	// Token: 0x0400427E RID: 17022
	public AudioClip roundEndingCountdownClip;

	// Token: 0x0400427F RID: 17023
	public int roundEndCountdownDuration = 5;

	// Token: 0x04004280 RID: 17024
	public AudioClip roundEndClip;

	// Token: 0x04004281 RID: 17025
	public AudioClip needMorePlayerClip;

	// Token: 0x04004282 RID: 17026
	private GorillaTagCompetitiveManager.GameState lastState;

	// Token: 0x04004283 RID: 17027
	private float lastStateRemainingTime = -1f;
}
