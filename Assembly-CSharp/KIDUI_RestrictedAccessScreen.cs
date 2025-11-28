using System;
using UnityEngine;

// Token: 0x02000AB7 RID: 2743
public class KIDUI_RestrictedAccessScreen : MonoBehaviour
{
	// Token: 0x060044C9 RID: 17609 RVA: 0x0016C910 File Offset: 0x0016AB10
	public void ShowRestrictedAccessScreen(SessionStatus? sessionStatus)
	{
		base.gameObject.SetActive(true);
		this._pendingStatusIndicator.SetActive(false);
		this._prohibitedStatusIndicator.SetActive(false);
		if (sessionStatus == null)
		{
			return;
		}
		if (sessionStatus != null)
		{
			switch (sessionStatus.GetValueOrDefault())
			{
			case SessionStatus.PASS:
			case SessionStatus.CHALLENGE:
			case SessionStatus.CHALLENGE_SESSION_UPGRADE:
				break;
			case SessionStatus.PROHIBITED:
				this._prohibitedStatusIndicator.SetActive(true);
				return;
			case SessionStatus.PENDING_AGE_APPEAL:
				this._pendingStatusIndicator.SetActive(true);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060044CA RID: 17610 RVA: 0x0016C990 File Offset: 0x0016AB90
	public void OnChangeAgePressed()
	{
		PrivateUIRoom.RemoveUI(base.transform);
		base.gameObject.SetActive(false);
		this._ageAppealScreen.ShowAgeAppealScreen();
	}

	// Token: 0x060044CB RID: 17611 RVA: 0x001646EC File Offset: 0x001628EC
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x0400567E RID: 22142
	[SerializeField]
	private KIDAgeAppeal _ageAppealScreen;

	// Token: 0x0400567F RID: 22143
	[SerializeField]
	private GameObject _pendingStatusIndicator;

	// Token: 0x04005680 RID: 22144
	[SerializeField]
	private GameObject _prohibitedStatusIndicator;
}
