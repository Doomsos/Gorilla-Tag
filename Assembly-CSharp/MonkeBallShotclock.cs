using System;
using TMPro;
using UnityEngine;

// Token: 0x02000556 RID: 1366
public class MonkeBallShotclock : MonoBehaviourTick
{
	// Token: 0x0600228C RID: 8844 RVA: 0x000B4BD0 File Offset: 0x000B2DD0
	public override void Tick()
	{
		if (this._time >= 0f)
		{
			this._time -= Time.deltaTime;
			this.UpdateTimeText(this._time);
			if (this._time < 0f)
			{
				this.SetBackboard(this.neutralMaterial);
			}
		}
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x000B4C24 File Offset: 0x000B2E24
	public void SetTime(int teamId, float time)
	{
		this._time = time;
		if (teamId == -1)
		{
			this._time = 0f;
			this.SetBackboard(this.neutralMaterial);
		}
		else if (teamId >= 0 && teamId < this.teamMaterials.Length)
		{
			this.SetBackboard(this.teamMaterials[teamId]);
		}
		this.UpdateTimeText(time);
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000B4C79 File Offset: 0x000B2E79
	private void SetBackboard(Material teamMaterial)
	{
		if (this.backboard != null)
		{
			this.backboard.material = teamMaterial;
		}
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x000B4C98 File Offset: 0x000B2E98
	private void UpdateTimeText(float time)
	{
		int num = Mathf.CeilToInt(time);
		if (this._timeInt != num)
		{
			this._timeInt = num;
			this.timeRemainingLabel.text = this._timeInt.ToString("#00");
		}
	}

	// Token: 0x04002D22 RID: 11554
	public Renderer backboard;

	// Token: 0x04002D23 RID: 11555
	public Material[] teamMaterials;

	// Token: 0x04002D24 RID: 11556
	public Material neutralMaterial;

	// Token: 0x04002D25 RID: 11557
	public TextMeshPro timeRemainingLabel;

	// Token: 0x04002D26 RID: 11558
	private float _time;

	// Token: 0x04002D27 RID: 11559
	private int _timeInt = -1;
}
