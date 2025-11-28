using System;
using GorillaExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.UI;

// Token: 0x0200091D RID: 2333
public class GorillaPressableButton : MonoBehaviour, IClickable
{
	// Token: 0x14000077 RID: 119
	// (add) Token: 0x06003B95 RID: 15253 RVA: 0x0013AF2C File Offset: 0x0013912C
	// (remove) Token: 0x06003B96 RID: 15254 RVA: 0x0013AF64 File Offset: 0x00139164
	public event Action<GorillaPressableButton, bool> onPressed;

	// Token: 0x06003B97 RID: 15255 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void Start()
	{
	}

	// Token: 0x06003B98 RID: 15256 RVA: 0x0013AF99 File Offset: 0x00139199
	protected virtual void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.RefreshText));
		this.RefreshText();
	}

	// Token: 0x06003B99 RID: 15257 RVA: 0x0013AFB3 File Offset: 0x001391B3
	protected virtual void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.RefreshText));
	}

	// Token: 0x06003B9A RID: 15258 RVA: 0x0013AFC8 File Offset: 0x001391C8
	protected virtual void RefreshText()
	{
		if (this._offLocalizedText == null || this._offLocalizedText.IsEmpty || this._onLocalizedText == null || this._onLocalizedText.IsEmpty)
		{
			return;
		}
		if (!this._useOnOffText)
		{
			return;
		}
		string localizedString;
		if (!this.isOn)
		{
			localizedString = this.offText;
			localizedString = this._offLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for OFF localized text", this);
				localizedString = this.offText;
			}
		}
		else
		{
			localizedString = this.onText;
			localizedString = this._onLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for ON localized text", this);
				localizedString = this.onText;
			}
		}
		if (this._myTxtSet || this.myText.IsNotNull())
		{
			this.myText.text = localizedString;
		}
		if (this._myTmpTxtSet || this.myTmpText.IsNotNull())
		{
			this.myTmpText.text = localizedString;
		}
		if (this._myTmpTxt2Set || this.myTmpText2.IsNotNull())
		{
			this.myTmpText2.text = localizedString;
		}
	}

	// Token: 0x06003B9B RID: 15259 RVA: 0x0013B0D8 File Offset: 0x001392D8
	protected virtual void SetOffText(bool setMyText, bool setMyTmpText = false, bool setMyTmpText2 = false)
	{
		if (!this._useOnOffText)
		{
			return;
		}
		string localizedString = this.offText;
		if (this._offLocalizedText != null && !this._offLocalizedText.IsEmpty)
		{
			localizedString = this._offLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for OFF localized text", this);
				localizedString = this.offText;
			}
		}
		this._myTxtSet = setMyText;
		this._myTmpTxtSet = setMyTmpText;
		this._myTmpTxt2Set = setMyTmpText2;
		if (setMyText)
		{
			this.myText.text = localizedString;
		}
		if (setMyTmpText)
		{
			this.myTmpText.text = localizedString;
		}
		if (setMyTmpText2)
		{
			this.myTmpText2.text = localizedString;
		}
	}

	// Token: 0x06003B9C RID: 15260 RVA: 0x0013B174 File Offset: 0x00139374
	protected virtual void SetOnText(bool setMyText, bool setMyTmpText = false, bool setMyTmpText2 = false)
	{
		if (!this._useOnOffText)
		{
			return;
		}
		string localizedString = this.onText;
		if (this._onLocalizedText != null && !this._onLocalizedText.IsEmpty)
		{
			localizedString = this._onLocalizedText.GetLocalizedString();
			if (string.IsNullOrEmpty(localizedString))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_PRESSABLE_BUTTON] Null or empty string returned for ON localized text", this);
				localizedString = this.onText;
			}
		}
		this._myTxtSet = setMyText;
		this._myTmpTxtSet = setMyTmpText;
		this._myTmpTxt2Set = setMyTmpText2;
		if (setMyText)
		{
			this.myText.text = localizedString;
		}
		if (setMyTmpText)
		{
			this.myTmpText.text = localizedString;
		}
		if (setMyTmpText2)
		{
			this.myTmpText2.text = localizedString;
		}
	}

	// Token: 0x06003B9D RID: 15261 RVA: 0x0013B210 File Offset: 0x00139410
	protected void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator component = collider.gameObject.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (!component)
		{
			return;
		}
		this.PressButton(component.isLeftHand);
	}

	// Token: 0x06003B9E RID: 15262 RVA: 0x0013B25C File Offset: 0x0013945C
	private void PressButton(bool isLeftHand)
	{
		this.touchTime = Time.time;
		UnityEvent unityEvent = this.onPressButton;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		Action<GorillaPressableButton, bool> action = this.onPressed;
		if (action != null)
		{
			action.Invoke(this, isLeftHand);
		}
		this.ButtonActivation();
		this.ButtonActivationWithHand(isLeftHand);
		GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(this.pressButtonSoundIndex, isLeftHand, 0.05f);
		GorillaTagger.Instance.StartVibration(isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
		if (NetworkSystem.Instance.InRoom && GorillaTagger.Instance.myVRRig != null)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_PlayHandTap", 1, new object[]
			{
				67,
				isLeftHand,
				0.05f
			});
		}
	}

	// Token: 0x06003B9F RID: 15263 RVA: 0x0013B33E File Offset: 0x0013953E
	public void Click(bool leftHand = false)
	{
		this.PressButton(leftHand);
	}

	// Token: 0x06003BA0 RID: 15264 RVA: 0x0013B347 File Offset: 0x00139547
	public virtual void UpdateColor()
	{
		this.UpdateColorWithState(this.isOn);
	}

	// Token: 0x06003BA1 RID: 15265 RVA: 0x0013B358 File Offset: 0x00139558
	protected void UpdateColorWithState(bool state)
	{
		if (state)
		{
			this.buttonRenderer.material = this.pressedMaterial;
			this.SetOnText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
			return;
		}
		this.buttonRenderer.material = this.unpressedMaterial;
		this.SetOffText(this.myText.IsNotNull(), this.myTmpText.IsNotNull(), this.myTmpText2.IsNotNull());
	}

	// Token: 0x06003BA2 RID: 15266 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonActivation()
	{
	}

	// Token: 0x06003BA3 RID: 15267 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void ButtonActivationWithHand(bool isLeftHand)
	{
	}

	// Token: 0x06003BA4 RID: 15268 RVA: 0x0013B3D9 File Offset: 0x001395D9
	public virtual void ResetState()
	{
		this.isOn = false;
		this.UpdateColor();
	}

	// Token: 0x06003BA5 RID: 15269 RVA: 0x0013B3E8 File Offset: 0x001395E8
	public void SetText(string newText)
	{
		if (this.myTmpText != null)
		{
			this.myTmpText.text = newText;
		}
		if (this.myTmpText2 != null)
		{
			this.myTmpText2.text = newText;
		}
		if (this.myText != null)
		{
			this.myText.text = newText;
		}
	}

	// Token: 0x04004C21 RID: 19489
	public Material pressedMaterial;

	// Token: 0x04004C22 RID: 19490
	public Material unpressedMaterial;

	// Token: 0x04004C23 RID: 19491
	public MeshRenderer buttonRenderer;

	// Token: 0x04004C24 RID: 19492
	public int pressButtonSoundIndex = 67;

	// Token: 0x04004C25 RID: 19493
	public bool isOn;

	// Token: 0x04004C26 RID: 19494
	public float debounceTime = 0.25f;

	// Token: 0x04004C27 RID: 19495
	public float touchTime;

	// Token: 0x04004C28 RID: 19496
	public bool testPress;

	// Token: 0x04004C29 RID: 19497
	public bool testHandLeft;

	// Token: 0x04004C2A RID: 19498
	[SerializeField]
	private bool _useOnOffText = true;

	// Token: 0x04004C2B RID: 19499
	[TextArea]
	public string offText;

	// Token: 0x04004C2C RID: 19500
	[SerializeField]
	private LocalizedString _offLocalizedText;

	// Token: 0x04004C2D RID: 19501
	[TextArea]
	public string onText;

	// Token: 0x04004C2E RID: 19502
	[SerializeField]
	private LocalizedString _onLocalizedText;

	// Token: 0x04004C2F RID: 19503
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText;

	// Token: 0x04004C30 RID: 19504
	[SerializeField]
	[Tooltip("Use this one when you can. Don't use MyText if you can help it!")]
	public TMP_Text myTmpText2;

	// Token: 0x04004C31 RID: 19505
	public Text myText;

	// Token: 0x04004C32 RID: 19506
	[Space]
	public UnityEvent onPressButton;

	// Token: 0x04004C33 RID: 19507
	protected bool _myTxtSet;

	// Token: 0x04004C34 RID: 19508
	protected bool _myTmpTxtSet;

	// Token: 0x04004C35 RID: 19509
	protected bool _myTmpTxt2Set;
}
