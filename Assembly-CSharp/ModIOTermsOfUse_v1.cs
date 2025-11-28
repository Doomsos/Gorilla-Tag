using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000989 RID: 2441
public class ModIOTermsOfUse_v1 : MonoBehaviour
{
	// Token: 0x06003E29 RID: 15913 RVA: 0x0014B2F1 File Offset: 0x001494F1
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06003E2A RID: 15914 RVA: 0x0014B315 File Offset: 0x00149515
	private void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06003E2B RID: 15915 RVA: 0x0014B339 File Offset: 0x00149539
	private void PostUpdate()
	{
		if (ControllerBehaviour.Instance.IsLeftStick)
		{
			this.TurnPage(-1);
		}
		if (ControllerBehaviour.Instance.IsRightStick)
		{
			this.TurnPage(1);
		}
		if (this.waitingForAcknowledge)
		{
			this.acceptButtonDown = ControllerBehaviour.Instance.ButtonDown;
		}
	}

	// Token: 0x06003E2C RID: 15916 RVA: 0x0014B37C File Offset: 0x0014957C
	private void Start()
	{
		ModIOTermsOfUse_v1.<Start>d__19 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<Start>d__19>(ref <Start>d__);
	}

	// Token: 0x06003E2D RID: 15917 RVA: 0x0014B3B4 File Offset: 0x001495B4
	private Task<bool> UpdateTextFromTerms()
	{
		ModIOTermsOfUse_v1.<UpdateTextFromTerms>d__20 <UpdateTextFromTerms>d__;
		<UpdateTextFromTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromTerms>d__.<>4__this = this;
		<UpdateTextFromTerms>d__.<>1__state = -1;
		<UpdateTextFromTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<UpdateTextFromTerms>d__20>(ref <UpdateTextFromTerms>d__);
		return <UpdateTextFromTerms>d__.<>t__builder.Task;
	}

	// Token: 0x06003E2E RID: 15918 RVA: 0x0014B3F8 File Offset: 0x001495F8
	public Task<bool> UpdateTextWithFullTerms()
	{
		ModIOTermsOfUse_v1.<UpdateTextWithFullTerms>d__21 <UpdateTextWithFullTerms>d__;
		<UpdateTextWithFullTerms>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextWithFullTerms>d__.<>1__state = -1;
		<UpdateTextWithFullTerms>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<UpdateTextWithFullTerms>d__21>(ref <UpdateTextWithFullTerms>d__);
		return <UpdateTextWithFullTerms>d__.<>t__builder.Task;
	}

	// Token: 0x06003E2F RID: 15919 RVA: 0x0014B434 File Offset: 0x00149634
	private string GetStringForListItemIdx_LowerAlpha(int idx)
	{
		switch (idx)
		{
		case 0:
			return "  a. <indent=5%>";
		case 1:
			return "  b. <indent=5%>";
		case 2:
			return "  c. <indent=5%>";
		case 3:
			return "  d. <indent=5%>";
		case 4:
			return "  e. <indent=5%>";
		case 5:
			return "  f. <indent=5%>";
		case 6:
			return "  g. <indent=5%>";
		case 7:
			return "  h. <indent=5%>";
		case 8:
			return "  i. <indent=5%>";
		case 9:
			return "  j. <indent=5%>";
		case 10:
			return "  k. <indent=5%>";
		case 11:
			return "  l. <indent=5%>";
		case 12:
			return "  m. <indent=5%>";
		case 13:
			return "  n. <indent=5%>";
		case 14:
			return "  o. <indent=5%>";
		case 15:
			return "  p. <indent=5%>";
		case 16:
			return "  q. <indent=5%>";
		case 17:
			return "  r. <indent=5%>";
		case 18:
			return "  s. <indent=5%>";
		case 19:
			return "  t. <indent=5%>";
		case 20:
			return "  u. <indent=5%>";
		case 21:
			return "  v. <indent=5%>";
		case 22:
			return "  w. <indent=5%>";
		case 23:
			return "  x. <indent=5%>";
		case 24:
			return "  y. <indent=5%>";
		case 25:
			return "  z. <indent=5%>";
		default:
			return "";
		}
	}

	// Token: 0x06003E30 RID: 15920 RVA: 0x0014B558 File Offset: 0x00149758
	private Task WaitForAcknowledgement()
	{
		ModIOTermsOfUse_v1.<WaitForAcknowledgement>d__23 <WaitForAcknowledgement>d__;
		<WaitForAcknowledgement>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForAcknowledgement>d__.<>4__this = this;
		<WaitForAcknowledgement>d__.<>1__state = -1;
		<WaitForAcknowledgement>d__.<>t__builder.Start<ModIOTermsOfUse_v1.<WaitForAcknowledgement>d__23>(ref <WaitForAcknowledgement>d__);
		return <WaitForAcknowledgement>d__.<>t__builder.Task;
	}

	// Token: 0x06003E31 RID: 15921 RVA: 0x0014B59C File Offset: 0x0014979C
	public void TurnPage(int i)
	{
		this.tmpBody.pageToDisplay = Mathf.Clamp(this.tmpBody.pageToDisplay + i, 1, this.tmpBody.textInfo.pageCount);
		this.tmpPage.text = string.Format("page {0} of {1}", this.tmpBody.pageToDisplay, this.tmpBody.textInfo.pageCount);
		this.nextButton.SetActive(this.tmpBody.pageToDisplay < this.tmpBody.textInfo.pageCount);
		this.prevButton.SetActive(this.tmpBody.pageToDisplay > 1);
		this.ActivateAcceptButtonGroup();
	}

	// Token: 0x06003E32 RID: 15922 RVA: 0x0014B658 File Offset: 0x00149858
	private void ActivateAcceptButtonGroup()
	{
		bool active = this.tmpBody.pageToDisplay == this.tmpBody.textInfo.pageCount;
		this.yesNoButtons.SetActive(active);
		this.waitingForAcknowledge = active;
	}

	// Token: 0x06003E33 RID: 15923 RVA: 0x0014B696 File Offset: 0x00149896
	public void Acknowledge(bool didAccept)
	{
		this.accepted = didAccept;
	}

	// Token: 0x04004EF8 RID: 20216
	[SerializeField]
	private Transform uiParent;

	// Token: 0x04004EF9 RID: 20217
	[SerializeField]
	private string title;

	// Token: 0x04004EFA RID: 20218
	[SerializeField]
	private TMP_Text tmpBody;

	// Token: 0x04004EFB RID: 20219
	[SerializeField]
	private TMP_Text tmpTitle;

	// Token: 0x04004EFC RID: 20220
	[SerializeField]
	private TMP_Text tmpPage;

	// Token: 0x04004EFD RID: 20221
	[SerializeField]
	public GameObject yesNoButtons;

	// Token: 0x04004EFE RID: 20222
	[SerializeField]
	public GameObject nextButton;

	// Token: 0x04004EFF RID: 20223
	[SerializeField]
	public GameObject prevButton;

	// Token: 0x04004F00 RID: 20224
	private bool hasTermsOfUse;

	// Token: 0x04004F01 RID: 20225
	private Action<bool> termsAcknowledgedCallback;

	// Token: 0x04004F02 RID: 20226
	private string cachedTermsText;

	// Token: 0x04004F03 RID: 20227
	private bool waitingForAcknowledge;

	// Token: 0x04004F04 RID: 20228
	private bool accepted;

	// Token: 0x04004F05 RID: 20229
	private bool acceptButtonDown;

	// Token: 0x04004F06 RID: 20230
	[SerializeField]
	private float holdTime = 5f;

	// Token: 0x04004F07 RID: 20231
	[SerializeField]
	private LineRenderer progressBar;
}
