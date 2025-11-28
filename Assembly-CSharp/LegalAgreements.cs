using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000ACE RID: 2766
[DefaultExecutionOrder(1)]
public class LegalAgreements : MonoBehaviour
{
	// Token: 0x17000681 RID: 1665
	// (get) Token: 0x0600451D RID: 17693 RVA: 0x0016E2CA File Offset: 0x0016C4CA
	// (set) Token: 0x0600451E RID: 17694 RVA: 0x0016E2D1 File Offset: 0x0016C4D1
	public static LegalAgreements instance { get; private set; }

	// Token: 0x0600451F RID: 17695 RVA: 0x0016E2DC File Offset: 0x0016C4DC
	protected virtual void Awake()
	{
		if (LegalAgreements.instance != null)
		{
			Debug.LogError("Trying to set [LegalAgreements] instance but it is not null", this);
			base.gameObject.SetActive(false);
			return;
		}
		LegalAgreements.instance = this;
		this.stickHeldDuration = 0f;
		this.scrollSpeed = this._minScrollSpeed;
		base.enabled = false;
	}

	// Token: 0x06004520 RID: 17696 RVA: 0x0016E334 File Offset: 0x0016C534
	private void Update()
	{
		if (!this.legalAgreementsStarted)
		{
			return;
		}
		float num = Time.deltaTime * this.scrollSpeed;
		if (ControllerBehaviour.Instance.IsUpStick || ControllerBehaviour.Instance.IsDownStick)
		{
			if (ControllerBehaviour.Instance.IsDownStick)
			{
				num *= -1f;
			}
			this.scrollBar.value = Mathf.Clamp(this.scrollBar.value + num, 0f, 1f);
			if (this.scrollBar.value > 0f && this.scrollBar.value < 1f)
			{
				HandRayController.Instance.PulseActiveHandray(this._stickVibrationStrength, this._stickVibrationDuration);
			}
			this.stickHeldDuration += Time.deltaTime;
			this.scrollTime = Mathf.Clamp01(this.stickHeldDuration / this._scrollInterpTime);
			this.scrollSpeed = Mathf.Lerp(this._minScrollSpeed, this._maxScrollSpeed, this._scrollInterpCurve.Evaluate(this.scrollTime));
			this.scrollSpeed *= Mathf.Abs(ControllerBehaviour.Instance.StickYValue);
		}
		else
		{
			this.stickHeldDuration = 0f;
			this.scrollSpeed = this._minScrollSpeed;
		}
		if (this._scrollToBottomText)
		{
			if ((double)this.scrollBar.value < 0.001)
			{
				this._scrollToBottomText.gameObject.SetActive(false);
				this._pressAndHoldToConfirmButton.gameObject.SetActive(true);
				return;
			}
			this._scrollToBottomText.text = LegalAgreements.SCROLL_TO_END_MESSAGE;
			this._scrollToBottomText.gameObject.SetActive(true);
			this._pressAndHoldToConfirmButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004521 RID: 17697 RVA: 0x0016E4E8 File Offset: 0x0016C6E8
	public virtual Task StartLegalAgreements()
	{
		LegalAgreements.<StartLegalAgreements>d__24 <StartLegalAgreements>d__;
		<StartLegalAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartLegalAgreements>d__.<>4__this = this;
		<StartLegalAgreements>d__.<>1__state = -1;
		<StartLegalAgreements>d__.<>t__builder.Start<LegalAgreements.<StartLegalAgreements>d__24>(ref <StartLegalAgreements>d__);
		return <StartLegalAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x06004522 RID: 17698 RVA: 0x0016E52B File Offset: 0x0016C72B
	public void OnAccepted(int currentAge)
	{
		this._accepted = true;
	}

	// Token: 0x06004523 RID: 17699 RVA: 0x0016E534 File Offset: 0x0016C734
	protected Task WaitForAcknowledgement()
	{
		LegalAgreements.<WaitForAcknowledgement>d__27 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<LegalAgreements.<WaitForAcknowledgement>d__27>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	// Token: 0x06004524 RID: 17700 RVA: 0x0016E578 File Offset: 0x0016C778
	private Task<bool> UpdateText(LegalAgreementTextAsset asset, string version)
	{
		LegalAgreements.<UpdateText>d__28 <UpdateText>d__;
		<UpdateText>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateText>d__.<>4__this = this;
		<UpdateText>d__.asset = asset;
		<UpdateText>d__.version = version;
		<UpdateText>d__.<>1__state = -1;
		<UpdateText>d__.<>t__builder.Start<LegalAgreements.<UpdateText>d__28>(ref <UpdateText>d__);
		return <UpdateText>d__.<>t__builder.Task;
	}

	// Token: 0x06004525 RID: 17701 RVA: 0x0016E5CC File Offset: 0x0016C7CC
	public Task<bool> UpdateTextFromPlayFabTitleData(string key, string version, TMP_Text target)
	{
		LegalAgreements.<UpdateTextFromPlayFabTitleData>d__33 <UpdateTextFromPlayFabTitleData>d__;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromPlayFabTitleData>d__.<>4__this = this;
		<UpdateTextFromPlayFabTitleData>d__.key = key;
		<UpdateTextFromPlayFabTitleData>d__.version = version;
		<UpdateTextFromPlayFabTitleData>d__.target = target;
		<UpdateTextFromPlayFabTitleData>d__.<>1__state = -1;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder.Start<LegalAgreements.<UpdateTextFromPlayFabTitleData>d__33>(ref <UpdateTextFromPlayFabTitleData>d__);
		return <UpdateTextFromPlayFabTitleData>d__.<>t__builder.Task;
	}

	// Token: 0x06004526 RID: 17702 RVA: 0x0016E627 File Offset: 0x0016C827
	private void OnPlayFabError(PlayFabError error)
	{
		this.state = -1;
	}

	// Token: 0x06004527 RID: 17703 RVA: 0x0016E630 File Offset: 0x0016C830
	private void OnTitleDataReceived(string obj)
	{
		this.cachedText = obj;
		this.state = 1;
	}

	// Token: 0x06004528 RID: 17704 RVA: 0x0016E640 File Offset: 0x0016C840
	private Task<string> GetTitleDataAsync(string key)
	{
		LegalAgreements.<GetTitleDataAsync>d__36 <GetTitleDataAsync>d__;
		<GetTitleDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetTitleDataAsync>d__.key = key;
		<GetTitleDataAsync>d__.<>1__state = -1;
		<GetTitleDataAsync>d__.<>t__builder.Start<LegalAgreements.<GetTitleDataAsync>d__36>(ref <GetTitleDataAsync>d__);
		return <GetTitleDataAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06004529 RID: 17705 RVA: 0x0016E684 File Offset: 0x0016C884
	private Task<Dictionary<string, string>> GetAcceptedAgreements(LegalAgreementTextAsset[] agreements)
	{
		LegalAgreements.<GetAcceptedAgreements>d__37 <GetAcceptedAgreements>d__;
		<GetAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder<Dictionary<string, string>>.Create();
		<GetAcceptedAgreements>d__.agreements = agreements;
		<GetAcceptedAgreements>d__.<>1__state = -1;
		<GetAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<GetAcceptedAgreements>d__37>(ref <GetAcceptedAgreements>d__);
		return <GetAcceptedAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x0600452A RID: 17706 RVA: 0x0016E6C8 File Offset: 0x0016C8C8
	private Task SubmitAcceptedAgreements(Dictionary<string, string> agreements)
	{
		LegalAgreements.<SubmitAcceptedAgreements>d__38 <SubmitAcceptedAgreements>d__;
		<SubmitAcceptedAgreements>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SubmitAcceptedAgreements>d__.agreements = agreements;
		<SubmitAcceptedAgreements>d__.<>1__state = -1;
		<SubmitAcceptedAgreements>d__.<>t__builder.Start<LegalAgreements.<SubmitAcceptedAgreements>d__38>(ref <SubmitAcceptedAgreements>d__);
		return <SubmitAcceptedAgreements>d__.<>t__builder.Task;
	}

	// Token: 0x0600452B RID: 17707 RVA: 0x0016470C File Offset: 0x0016290C
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x040056F0 RID: 22256
	private static string SCROLL_TO_END_MESSAGE = "<b>Scroll to the bottom</b> to continue.";

	// Token: 0x040056F1 RID: 22257
	[Header("Scroll Behavior")]
	[SerializeField]
	protected float _minScrollSpeed = 0.02f;

	// Token: 0x040056F2 RID: 22258
	[SerializeField]
	private float _maxScrollSpeed = 3f;

	// Token: 0x040056F3 RID: 22259
	[SerializeField]
	private float _scrollInterpTime = 3f;

	// Token: 0x040056F4 RID: 22260
	[SerializeField]
	private AnimationCurve _scrollInterpCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040056F6 RID: 22262
	[SerializeField]
	protected Transform uiParent;

	// Token: 0x040056F7 RID: 22263
	[SerializeField]
	protected TMP_Text tmpBody;

	// Token: 0x040056F8 RID: 22264
	[SerializeField]
	protected TMP_Text tmpTitle;

	// Token: 0x040056F9 RID: 22265
	[SerializeField]
	protected Scrollbar scrollBar;

	// Token: 0x040056FA RID: 22266
	[SerializeField]
	private LegalAgreementTextAsset[] legalAgreementScreens;

	// Token: 0x040056FB RID: 22267
	[SerializeField]
	protected KIDUIButton _pressAndHoldToConfirmButton;

	// Token: 0x040056FC RID: 22268
	[SerializeField]
	private TMP_Text _scrollToBottomText;

	// Token: 0x040056FD RID: 22269
	[SerializeField]
	private float _stickVibrationStrength = 0.1f;

	// Token: 0x040056FE RID: 22270
	[SerializeField]
	private float _stickVibrationDuration = 0.05f;

	// Token: 0x040056FF RID: 22271
	protected float stickHeldDuration;

	// Token: 0x04005700 RID: 22272
	protected float scrollSpeed;

	// Token: 0x04005701 RID: 22273
	private float scrollTime;

	// Token: 0x04005702 RID: 22274
	protected bool legalAgreementsStarted;

	// Token: 0x04005703 RID: 22275
	protected bool _accepted;

	// Token: 0x04005704 RID: 22276
	private string cachedText;

	// Token: 0x04005705 RID: 22277
	private int state;

	// Token: 0x04005706 RID: 22278
	private bool optIn;

	// Token: 0x04005707 RID: 22279
	private bool optional;
}
