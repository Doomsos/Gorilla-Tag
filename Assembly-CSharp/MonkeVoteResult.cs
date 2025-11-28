using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001DB RID: 475
public class MonkeVoteResult : MonoBehaviour
{
	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000CF8 RID: 3320 RVA: 0x00045E1D File Offset: 0x0004401D
	// (set) Token: 0x06000CF9 RID: 3321 RVA: 0x00045E28 File Offset: 0x00044028
	public string Text
	{
		get
		{
			return this._text;
		}
		set
		{
			TMP_Text optionText = this._optionText;
			this._text = value;
			optionText.text = value;
		}
	}

	// Token: 0x06000CFA RID: 3322 RVA: 0x00045E4C File Offset: 0x0004404C
	public void ShowResult(string questionOption, int percentage, bool showVote, bool showPrediction, bool isWinner)
	{
		this._optionText.text = questionOption;
		this._optionIndicator.SetActive(true);
		this._scoreText.text = ((percentage >= 0) ? string.Format("{0}%", percentage) : "--");
		this._voteIndicator.SetActive(showVote);
		this._guessWinIndicator.SetActive(showPrediction && isWinner);
		this._guessLoseIndicator.SetActive(showPrediction && !isWinner);
		this._youWinIndicator.SetActive(isWinner && showPrediction);
		this._mostPopularIndicator.SetActive(isWinner);
		this.ShowRockPile(percentage);
	}

	// Token: 0x06000CFB RID: 3323 RVA: 0x00045EF0 File Offset: 0x000440F0
	public void HideResult()
	{
		this._optionIndicator.SetActive(false);
		this._voteIndicator.SetActive(false);
		this._guessWinIndicator.SetActive(false);
		this._guessLoseIndicator.SetActive(false);
		this._youWinIndicator.SetActive(false);
		this._mostPopularIndicator.SetActive(false);
		this.ShowRockPile(0);
	}

	// Token: 0x06000CFC RID: 3324 RVA: 0x00045F4C File Offset: 0x0004414C
	private void ShowRockPile(int percentage)
	{
		this._rockPiles.Show(percentage);
	}

	// Token: 0x06000CFD RID: 3325 RVA: 0x00045F5C File Offset: 0x0004415C
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._mostPopularIndicator.SetActive(visible);
		this._voteIndicator.SetActive(visible);
		this._guessWinIndicator.SetActive(visible);
		this._guessLoseIndicator.SetActive(visible);
		this._rockPiles.Show(visible ? 100 : -1);
	}

	// Token: 0x04000FEE RID: 4078
	[SerializeField]
	private GameObject _optionIndicator;

	// Token: 0x04000FEF RID: 4079
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x04000FF0 RID: 4080
	[FormerlySerializedAs("_scoreLabelPost")]
	[SerializeField]
	private GameObject _scoreIndicator;

	// Token: 0x04000FF1 RID: 4081
	[SerializeField]
	private TMP_Text _scoreText;

	// Token: 0x04000FF2 RID: 4082
	[SerializeField]
	private GameObject _voteIndicator;

	// Token: 0x04000FF3 RID: 4083
	[SerializeField]
	private GameObject _guessWinIndicator;

	// Token: 0x04000FF4 RID: 4084
	[SerializeField]
	private GameObject _guessLoseIndicator;

	// Token: 0x04000FF5 RID: 4085
	[SerializeField]
	private GameObject _mostPopularIndicator;

	// Token: 0x04000FF6 RID: 4086
	[SerializeField]
	private GameObject _youWinIndicator;

	// Token: 0x04000FF7 RID: 4087
	[SerializeField]
	private RockPiles _rockPiles;

	// Token: 0x04000FF8 RID: 4088
	private MonkeVoteMachine _machine;

	// Token: 0x04000FF9 RID: 4089
	private string _text = string.Empty;

	// Token: 0x04000FFA RID: 4090
	private bool _canVote;

	// Token: 0x04000FFB RID: 4091
	private float _rockPileHeight;
}
