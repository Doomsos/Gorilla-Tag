using System;
using TMPro;
using UnityEngine;

// Token: 0x02000554 RID: 1364
public class MonkeBallScoreboard : MonoBehaviour
{
	// Token: 0x06002280 RID: 8832 RVA: 0x000B4A82 File Offset: 0x000B2C82
	public void Setup(MonkeBallGame game)
	{
		this.game = game;
	}

	// Token: 0x06002281 RID: 8833 RVA: 0x000B4A8C File Offset: 0x000B2C8C
	public void RefreshScore()
	{
		for (int i = 0; i < this.game.team.Count; i++)
		{
			this.teamDisplays[i].scoreLabel.text = this.game.team[i].score.ToString();
		}
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x000B4AE1 File Offset: 0x000B2CE1
	public void RefreshTeamPlayers(int teamId, int numPlayers)
	{
		this.teamDisplays[teamId].playersLabel.text = string.Format("PLAYERS: {0}", Mathf.Clamp(numPlayers, 0, 99));
	}

	// Token: 0x06002283 RID: 8835 RVA: 0x000B4B0D File Offset: 0x000B2D0D
	public void PlayScoreFx()
	{
		this.PlayFX(this.scoreSound, this.scoreSoundVolume);
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x000B4B21 File Offset: 0x000B2D21
	public void PlayPlayerJoinFx()
	{
		this.PlayFX(this.playerJoinSound, 0.5f);
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x000B4B34 File Offset: 0x000B2D34
	public void PlayPlayerLeaveFx()
	{
		this.PlayFX(this.playerLeaveSound, 0.5f);
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x000B4B47 File Offset: 0x000B2D47
	public void PlayGameStartFx()
	{
		this.PlayFX(this.gameStartSound, this.gameStartVolume);
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x000B4B5B File Offset: 0x000B2D5B
	public void PlayGameEndFx()
	{
		this.PlayFX(this.gameEndSound, this.gameEndVolume);
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x000B4B6F File Offset: 0x000B2D6F
	private void PlayFX(AudioClip clip, float volume)
	{
		if (this.audioSource != null)
		{
			this.audioSource.clip = clip;
			this.audioSource.volume = volume;
			this.audioSource.Play();
		}
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x000B4BA2 File Offset: 0x000B2DA2
	public void RefreshTime(string timeString)
	{
		this.timeRemainingLabel.text = timeString;
	}

	// Token: 0x04002D13 RID: 11539
	private MonkeBallGame game;

	// Token: 0x04002D14 RID: 11540
	public MonkeBallScoreboard.TeamDisplay[] teamDisplays;

	// Token: 0x04002D15 RID: 11541
	public TextMeshPro timeRemainingLabel;

	// Token: 0x04002D16 RID: 11542
	public AudioSource audioSource;

	// Token: 0x04002D17 RID: 11543
	public AudioClip scoreSound;

	// Token: 0x04002D18 RID: 11544
	public float scoreSoundVolume;

	// Token: 0x04002D19 RID: 11545
	public AudioClip playerJoinSound;

	// Token: 0x04002D1A RID: 11546
	public AudioClip playerLeaveSound;

	// Token: 0x04002D1B RID: 11547
	public AudioClip gameStartSound;

	// Token: 0x04002D1C RID: 11548
	public float gameStartVolume;

	// Token: 0x04002D1D RID: 11549
	public AudioClip gameEndSound;

	// Token: 0x04002D1E RID: 11550
	public float gameEndVolume;

	// Token: 0x02000555 RID: 1365
	[Serializable]
	public class TeamDisplay
	{
		// Token: 0x04002D1F RID: 11551
		public TextMeshPro nameLabel;

		// Token: 0x04002D20 RID: 11552
		public TextMeshPro scoreLabel;

		// Token: 0x04002D21 RID: 11553
		public TextMeshPro playersLabel;
	}
}
