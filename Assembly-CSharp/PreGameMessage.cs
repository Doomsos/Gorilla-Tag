using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000A73 RID: 2675
public class PreGameMessage : MonoBehaviour
{
	// Token: 0x06004346 RID: 17222 RVA: 0x0016541A File Offset: 0x0016361A
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004347 RID: 17223 RVA: 0x0016543E File Offset: 0x0016363E
	private void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
		}
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06004348 RID: 17224 RVA: 0x00165474 File Offset: 0x00163674
	public void ShowMessage(string messageTitle, string messageBody, string messageConfirmation, Action onConfirmationAction, float bodyFontSize = 0.5f, float buttonHideTimer = 0f)
	{
		this._alternativeAction = null;
		this._multiButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageConfirmationTxt.text = messageConfirmation;
		this._confirmationAction = onConfirmationAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null)
		{
			this._confirmButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
	}

	// Token: 0x06004349 RID: 17225 RVA: 0x00165518 File Offset: 0x00163718
	public void ShowMessage(string messageTitle, string messageBody, string messageConfirmationButton, string messageAlternativeButton, Action onConfirmationAction, Action onAlternativeAction, float bodyFontSize = 0.5f)
	{
		this._confirmButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageAlternativeConfirmationTxt.text = messageConfirmationButton;
		this._messageAlternativeButtonTxt.text = messageAlternativeButton;
		this._confirmationAction = onConfirmationAction;
		this._alternativeAction = onAlternativeAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null || this._alternativeAction == null)
		{
			Debug.LogError("[KID] Trying to show a mesasge with multiple buttons, but one or both callbacks are null");
			this._multiButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageAlternativeConfirmationTxt.text) && !string.IsNullOrEmpty(this._messageAlternativeButtonTxt.text))
		{
			this._multiButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
	}

	// Token: 0x0600434A RID: 17226 RVA: 0x001655F0 File Offset: 0x001637F0
	public Task ShowMessageWithAwait(string messageTitle, string messageBody, string messageConfirmation, Action onConfirmationAction, float bodyFontSize = 0.5f, float buttonHideTimer = 0f)
	{
		PreGameMessage.<ShowMessageWithAwait>d__20 <ShowMessageWithAwait>d__;
		<ShowMessageWithAwait>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<ShowMessageWithAwait>d__.<>4__this = this;
		<ShowMessageWithAwait>d__.messageTitle = messageTitle;
		<ShowMessageWithAwait>d__.messageBody = messageBody;
		<ShowMessageWithAwait>d__.messageConfirmation = messageConfirmation;
		<ShowMessageWithAwait>d__.onConfirmationAction = onConfirmationAction;
		<ShowMessageWithAwait>d__.bodyFontSize = bodyFontSize;
		<ShowMessageWithAwait>d__.<>1__state = -1;
		<ShowMessageWithAwait>d__.<>t__builder.Start<PreGameMessage.<ShowMessageWithAwait>d__20>(ref <ShowMessageWithAwait>d__);
		return <ShowMessageWithAwait>d__.<>t__builder.Task;
	}

	// Token: 0x0600434B RID: 17227 RVA: 0x00165660 File Offset: 0x00163860
	public void UpdateMessage(string newMessageBody, string newConfirmButton)
	{
		this._messageBodyTxt.text = newMessageBody;
		this._messageConfirmationTxt.text = newConfirmButton;
		if (string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(false);
			return;
		}
		if (this._confirmationAction != null)
		{
			this._confirmButtonRoot.SetActive(true);
		}
	}

	// Token: 0x0600434C RID: 17228 RVA: 0x001656B8 File Offset: 0x001638B8
	public void CloseMessage()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
	}

	// Token: 0x0600434D RID: 17229 RVA: 0x001656CC File Offset: 0x001638CC
	private Task WaitForCompletion()
	{
		PreGameMessage.<WaitForCompletion>d__23 <WaitForCompletion>d__;
		<WaitForCompletion>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForCompletion>d__.<>4__this = this;
		<WaitForCompletion>d__.<>1__state = -1;
		<WaitForCompletion>d__.<>t__builder.Start<PreGameMessage.<WaitForCompletion>d__23>(ref <WaitForCompletion>d__);
		return <WaitForCompletion>d__.<>t__builder.Task;
	}

	// Token: 0x0600434E RID: 17230 RVA: 0x00165710 File Offset: 0x00163910
	private void PostUpdate()
	{
		bool isLeftStick = ControllerBehaviour.Instance.IsLeftStick;
		bool isRightStick = ControllerBehaviour.Instance.IsRightStick;
		bool buttonDown = ControllerBehaviour.Instance.ButtonDown;
		if (this._multiButtonRoot.activeInHierarchy)
		{
			if (isLeftStick)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBarL.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarR.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBarR.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnConfirmedPressed();
					return;
				}
			}
			else if (isRightStick)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBarR.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBarL.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnAlternativePressed();
					return;
				}
			}
			else
			{
				this.progress = 0f;
				this.progressBarR.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			}
			return;
		}
		if (this._confirmButtonRoot.activeInHierarchy)
		{
			if (buttonDown)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnConfirmedPressed();
					return;
				}
			}
			else
			{
				this.progress = 0f;
				this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			}
			return;
		}
	}

	// Token: 0x0600434F RID: 17231 RVA: 0x001659E7 File Offset: 0x00163BE7
	private void OnConfirmedPressed()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
		this._hasCompleted = true;
		Action confirmationAction = this._confirmationAction;
		if (confirmationAction == null)
		{
			return;
		}
		confirmationAction.Invoke();
	}

	// Token: 0x06004350 RID: 17232 RVA: 0x00165A10 File Offset: 0x00163C10
	private void OnAlternativePressed()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
		this._hasCompleted = true;
		Action alternativeAction = this._alternativeAction;
		if (alternativeAction == null)
		{
			return;
		}
		alternativeAction.Invoke();
	}

	// Token: 0x040054CA RID: 21706
	[SerializeField]
	private GameObject _uiParent;

	// Token: 0x040054CB RID: 21707
	[SerializeField]
	private TMP_Text _messageTitleTxt;

	// Token: 0x040054CC RID: 21708
	[SerializeField]
	private TMP_Text _messageBodyTxt;

	// Token: 0x040054CD RID: 21709
	[SerializeField]
	private GameObject _confirmButtonRoot;

	// Token: 0x040054CE RID: 21710
	[SerializeField]
	private GameObject _multiButtonRoot;

	// Token: 0x040054CF RID: 21711
	[SerializeField]
	private TMP_Text _messageConfirmationTxt;

	// Token: 0x040054D0 RID: 21712
	[SerializeField]
	private TMP_Text _messageAlternativeConfirmationTxt;

	// Token: 0x040054D1 RID: 21713
	[SerializeField]
	private TMP_Text _messageAlternativeButtonTxt;

	// Token: 0x040054D2 RID: 21714
	private Action _confirmationAction;

	// Token: 0x040054D3 RID: 21715
	private Action _alternativeAction;

	// Token: 0x040054D4 RID: 21716
	private bool _hasCompleted;

	// Token: 0x040054D5 RID: 21717
	private float progress;

	// Token: 0x040054D6 RID: 21718
	[SerializeField]
	private float holdTime;

	// Token: 0x040054D7 RID: 21719
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x040054D8 RID: 21720
	[SerializeField]
	private LineRenderer progressBarL;

	// Token: 0x040054D9 RID: 21721
	[SerializeField]
	private LineRenderer progressBarR;
}
