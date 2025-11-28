using System;
using System.Threading;
using UnityEngine;

// Token: 0x02000A7E RID: 2686
public class KIDUI_AgeAppealScreen : MonoBehaviour
{
	// Token: 0x0600437F RID: 17279 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x06004380 RID: 17280 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnable()
	{
	}

	// Token: 0x06004381 RID: 17281 RVA: 0x001646EC File Offset: 0x001628EC
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x06004382 RID: 17282 RVA: 0x00166472 File Offset: 0x00164672
	public void ShowRestrictedAccessScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004383 RID: 17283 RVA: 0x000396A0 File Offset: 0x000378A0
	public void OnChangeAgePressed()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04005502 RID: 21762
	[SerializeField]
	private KIDUIButton _changeAgeButton;

	// Token: 0x04005503 RID: 21763
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x04005504 RID: 21764
	private string _submittedEmailAddress;

	// Token: 0x04005505 RID: 21765
	private CancellationTokenSource _cancellationTokenSource;
}
