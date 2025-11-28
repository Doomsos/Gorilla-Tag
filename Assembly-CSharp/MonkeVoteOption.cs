using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001D9 RID: 473
public class MonkeVoteOption : MonoBehaviour
{
	// Token: 0x14000016 RID: 22
	// (add) Token: 0x06000CE1 RID: 3297 RVA: 0x00045B54 File Offset: 0x00043D54
	// (remove) Token: 0x06000CE2 RID: 3298 RVA: 0x00045B8C File Offset: 0x00043D8C
	public event Action<MonkeVoteOption, Collider> OnVote;

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x06000CE3 RID: 3299 RVA: 0x00045BC1 File Offset: 0x00043DC1
	// (set) Token: 0x06000CE4 RID: 3300 RVA: 0x00045BCC File Offset: 0x00043DCC
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

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000CE5 RID: 3301 RVA: 0x00045BEE File Offset: 0x00043DEE
	// (set) Token: 0x06000CE6 RID: 3302 RVA: 0x00045BF8 File Offset: 0x00043DF8
	public bool CanVote
	{
		get
		{
			return this._canVote;
		}
		set
		{
			Collider trigger = this._trigger;
			this._canVote = value;
			trigger.enabled = value;
		}
	}

	// Token: 0x06000CE7 RID: 3303 RVA: 0x00045C1A File Offset: 0x00043E1A
	private void Reset()
	{
		this.Configure();
	}

	// Token: 0x06000CE8 RID: 3304 RVA: 0x00045C24 File Offset: 0x00043E24
	private void Configure()
	{
		foreach (Collider collider in base.GetComponentsInChildren<Collider>())
		{
			if (collider.isTrigger)
			{
				this._trigger = collider;
				break;
			}
		}
		if (!this._optionText)
		{
			this._optionText = base.GetComponentInChildren<TMP_Text>();
		}
	}

	// Token: 0x06000CE9 RID: 3305 RVA: 0x00045C74 File Offset: 0x00043E74
	private void OnTriggerEnter(Collider other)
	{
		if (!this.IsValidVotingRock(other))
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote.Invoke(this, other);
	}

	// Token: 0x06000CEA RID: 3306 RVA: 0x00045C94 File Offset: 0x00043E94
	private bool IsValidVotingRock(Collider other)
	{
		SlingshotProjectile component = other.GetComponent<SlingshotProjectile>();
		return component && component.projectileOwner.IsLocal;
	}

	// Token: 0x06000CEB RID: 3307 RVA: 0x00045CBD File Offset: 0x00043EBD
	public void ResetState()
	{
		this.OnVote = null;
		this.ShowIndicators(false, false, true);
	}

	// Token: 0x06000CEC RID: 3308 RVA: 0x00045CCF File Offset: 0x00043ECF
	public void ShowIndicators(bool showVote, bool showPrediction, bool instant = true)
	{
		this._voteIndicator.SetVisible(showVote, instant);
		this._guessIndicator.SetVisible(showPrediction, instant);
	}

	// Token: 0x06000CED RID: 3309 RVA: 0x00045CEB File Offset: 0x00043EEB
	private void Vote()
	{
		this.SendVote(null);
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x00045CF4 File Offset: 0x00043EF4
	private void SendVote(Collider other)
	{
		if (!this._canVote)
		{
			return;
		}
		Action<MonkeVoteOption, Collider> onVote = this.OnVote;
		if (onVote == null)
		{
			return;
		}
		onVote.Invoke(this, other);
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x00045D11 File Offset: 0x00043F11
	public void SetDynamicMeshesVisible(bool visible)
	{
		this._voteIndicator.SetVisible(visible, true);
		this._guessIndicator.SetVisible(visible, true);
	}

	// Token: 0x04000FE3 RID: 4067
	[SerializeField]
	private Collider _trigger;

	// Token: 0x04000FE4 RID: 4068
	[SerializeField]
	private TMP_Text _optionText;

	// Token: 0x04000FE5 RID: 4069
	[SerializeField]
	private VotingCard _voteIndicator;

	// Token: 0x04000FE6 RID: 4070
	[FormerlySerializedAs("_predictionIndicator")]
	[SerializeField]
	private VotingCard _guessIndicator;

	// Token: 0x04000FE8 RID: 4072
	private string _text = string.Empty;

	// Token: 0x04000FE9 RID: 4073
	private bool _canVote;
}
