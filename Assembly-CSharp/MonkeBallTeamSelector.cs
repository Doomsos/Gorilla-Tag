using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000557 RID: 1367
public class MonkeBallTeamSelector : MonoBehaviour
{
	// Token: 0x06002291 RID: 8849 RVA: 0x000B4CC6 File Offset: 0x000B2EC6
	public void Awake()
	{
		this._setTeamButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x000B4CE4 File Offset: 0x000B2EE4
	public void OnDestroy()
	{
		this._setTeamButton.onPressButton.RemoveListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x000B4D02 File Offset: 0x000B2F02
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestSetTeam(this.teamId);
	}

	// Token: 0x04002D28 RID: 11560
	public int teamId;

	// Token: 0x04002D29 RID: 11561
	[SerializeField]
	private GorillaPressableButton _setTeamButton;
}
