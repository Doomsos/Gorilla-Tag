using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000553 RID: 1363
public class MonkeBallResetGame : MonoBehaviourTick
{
	// Token: 0x0600227A RID: 8826 RVA: 0x000B4938 File Offset: 0x000B2B38
	private void Awake()
	{
		this._resetButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
		if (this._resetButton == null)
		{
			this._buttonOrigin = this._resetButton.transform.position;
		}
	}

	// Token: 0x0600227B RID: 8827 RVA: 0x000B4985 File Offset: 0x000B2B85
	public override void Tick()
	{
		if (this._cooldown)
		{
			this._cooldownTimer -= Time.deltaTime;
			if (this._cooldownTimer <= 0f)
			{
				this.ToggleButton(false, -1);
				this._cooldown = false;
			}
		}
	}

	// Token: 0x0600227C RID: 8828 RVA: 0x000B49C0 File Offset: 0x000B2BC0
	public void ToggleReset(bool toggle, int teamId, bool force = false)
	{
		if (teamId < -1 || teamId >= this.teamMaterials.Length)
		{
			return;
		}
		if (toggle)
		{
			this.ToggleButton(true, teamId);
			this._cooldown = false;
			return;
		}
		if (force)
		{
			this.ToggleButton(false, -1);
			return;
		}
		this._cooldown = true;
		this._cooldownTimer = 3f;
	}

	// Token: 0x0600227D RID: 8829 RVA: 0x000B4A10 File Offset: 0x000B2C10
	private void ToggleButton(bool toggle, int teamId)
	{
		this._resetButton.enabled = toggle;
		this.allowedTeamId = teamId;
		if (!toggle || teamId == -1)
		{
			this.button.sharedMaterial = this.neutralMaterial;
			return;
		}
		this.button.sharedMaterial = this.teamMaterials[teamId];
	}

	// Token: 0x0600227E RID: 8830 RVA: 0x000B4A5C File Offset: 0x000B2C5C
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestResetGame();
	}

	// Token: 0x04002D09 RID: 11529
	[SerializeField]
	private GorillaPressableButton _resetButton;

	// Token: 0x04002D0A RID: 11530
	public Renderer button;

	// Token: 0x04002D0B RID: 11531
	public Vector3 buttonPressOffset;

	// Token: 0x04002D0C RID: 11532
	private Vector3 _buttonOrigin = Vector3.zero;

	// Token: 0x04002D0D RID: 11533
	[Space]
	public Material[] teamMaterials;

	// Token: 0x04002D0E RID: 11534
	public Material neutralMaterial;

	// Token: 0x04002D0F RID: 11535
	public int allowedTeamId = -1;

	// Token: 0x04002D10 RID: 11536
	[SerializeField]
	private TextMeshPro _resetLabel;

	// Token: 0x04002D11 RID: 11537
	private bool _cooldown;

	// Token: 0x04002D12 RID: 11538
	private float _cooldownTimer;
}
