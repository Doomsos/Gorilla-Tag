using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

// Token: 0x02000A2F RID: 2607
public class KIDAgeAppeal : MonoBehaviour
{
	// Token: 0x0600420F RID: 16911 RVA: 0x0015D788 File Offset: 0x0015B988
	public void ShowAgeAppealScreen()
	{
		this._ageSlider = base.GetComponentInChildren<AgeSliderWithProgressBar>(true);
		this._ageSlider.ControllerActive = true;
		base.gameObject.SetActive(true);
		this._inputsContainer.SetActive(true);
		this._monkeLoader.SetActive(false);
	}

	// Token: 0x06004210 RID: 16912 RVA: 0x0015D7C8 File Offset: 0x0015B9C8
	public void OnNewAgeConfirmed()
	{
		KIDAgeAppeal.<OnNewAgeConfirmed>d__6 <OnNewAgeConfirmed>d__;
		<OnNewAgeConfirmed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnNewAgeConfirmed>d__.<>4__this = this;
		<OnNewAgeConfirmed>d__.<>1__state = -1;
		<OnNewAgeConfirmed>d__.<>t__builder.Start<KIDAgeAppeal.<OnNewAgeConfirmed>d__6>(ref <OnNewAgeConfirmed>d__);
	}

	// Token: 0x0400531C RID: 21276
	[SerializeField]
	private TMP_Text _ageText;

	// Token: 0x0400531D RID: 21277
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x0400531E RID: 21278
	[SerializeField]
	private GameObject _inputsContainer;

	// Token: 0x0400531F RID: 21279
	[SerializeField]
	private GameObject _monkeLoader;

	// Token: 0x04005320 RID: 21280
	private AgeSliderWithProgressBar _ageSlider;
}
