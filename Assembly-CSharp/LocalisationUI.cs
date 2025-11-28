using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000AE3 RID: 2787
public class LocalisationUI : MonoBehaviour
{
	// Token: 0x1700068C RID: 1676
	// (get) Token: 0x06004580 RID: 17792 RVA: 0x00170515 File Offset: 0x0016E715
	public static LocalisationUI Instance
	{
		get
		{
			return LocalisationUI._instance;
		}
	}

	// Token: 0x06004581 RID: 17793 RVA: 0x0017051C File Offset: 0x0016E71C
	private void Awake()
	{
		if (LocalisationUI._instance != null)
		{
			Object.DestroyImmediate(this);
			return;
		}
		LocalisationUI._instance = this;
	}

	// Token: 0x06004582 RID: 17794 RVA: 0x00170538 File Offset: 0x0016E738
	private void Start()
	{
		this.ConstructLocalisationUI();
		this.CheckSelectedLanguage();
	}

	// Token: 0x06004583 RID: 17795 RVA: 0x00170546 File Offset: 0x0016E746
	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		if (this._hasConstructedUI)
		{
			this.CheckSelectedLanguage();
		}
	}

	// Token: 0x06004584 RID: 17796 RVA: 0x00170567 File Offset: 0x0016E767
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004585 RID: 17797 RVA: 0x0017057C File Offset: 0x0016E77C
	public void OnLanguageButtonPressed(KIDUIButton objRef, int languageIndex)
	{
		if (objRef != this._activeButton)
		{
			KIDUIButton activeButton = this._activeButton;
			if (activeButton != null)
			{
				activeButton.SetBorderImage(this._inactiveSprite);
			}
			objRef.SetBorderImage(this._activeSprite);
			this._activeButton = objRef;
		}
		Locale locale;
		if (!LocalisationManager.TryGetLocaleBinding(languageIndex, out locale))
		{
			return;
		}
		LocalisationManager.Instance.OnLanguageButtonPressed(locale.Identifier.Code, false);
	}

	// Token: 0x06004586 RID: 17798 RVA: 0x001705E5 File Offset: 0x0016E7E5
	public void OnContinueButtonPressed()
	{
		HandRayController.Instance.DisableHandRays();
		PrivateUIRoom.RemoveUI(LocalisationUI.GetUITransform());
		LocalisationManager.OnSaveLanguage();
	}

	// Token: 0x06004587 RID: 17799 RVA: 0x00170600 File Offset: 0x0016E800
	private void ConstructLocalisationUI()
	{
		using (Dictionary<int, Locale>.Enumerator enumerator = LocalisationManager.GetAllBindings().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, Locale> item = enumerator.Current;
				KIDUIButton newButton = Object.Instantiate<KIDUIButton>(this._languageButtonPrefab, this._languageButtonGridTransform);
				bool forceEnglishChars = LocalisationManager.CurrentLanguage.Identifier.Code.ToLower() != "ja";
				newButton.SetText(LocalisationManager.LocaleToFriendlyString(item.Value, forceEnglishChars).ToUpper());
				newButton.onClick.AddListener(delegate()
				{
					this.OnLanguageButtonPressed(newButton, item.Key);
				});
				this._languageButtons.Add(newButton);
			}
		}
		this._hasConstructedUI = true;
	}

	// Token: 0x06004588 RID: 17800 RVA: 0x001706F4 File Offset: 0x0016E8F4
	private void CheckSelectedLanguage()
	{
		KIDUIButton kiduibutton = null;
		for (int i = 0; i < this._languageButtons.Count; i++)
		{
			bool forceEnglishChars = LocalisationManager.CurrentLanguage.Identifier.Code.ToLower() != "ja";
			if (!(this._languageButtons[i].GetText() != LocalisationManager.LocaleToFriendlyString(LocalisationManager.CurrentLanguage, forceEnglishChars).ToUpper()))
			{
				kiduibutton = this._languageButtons[i];
				break;
			}
		}
		if (kiduibutton == null)
		{
			return;
		}
		if (this._activeButton != null)
		{
			this._activeButton.SetBorderImage(this._inactiveSprite);
		}
		kiduibutton.SetBorderImage(this._activeSprite);
		this._activeButton = kiduibutton;
	}

	// Token: 0x06004589 RID: 17801 RVA: 0x001707B0 File Offset: 0x0016E9B0
	private void OnLanguageChanged()
	{
		for (int i = 0; i < this._languageButtons.Count; i++)
		{
			bool forceEnglishChar = LocalisationManager.CurrentLanguage.Identifier.Code.ToLower() != "ja";
			this._languageButtons[i].SetText(LocalisationManager.LocaleDisplayNameToFriendlyString(this._languageButtons[i].GetText(), forceEnglishChar).ToUpper());
			if (!(LocalisationManager.CurrentLanguage.Identifier.Code == "ja"))
			{
				this._languageButtons[i].SetFont(this._defaultFont);
			}
			else
			{
				this._languageButtons[i].SetFont(this._japaneseFont);
			}
		}
	}

	// Token: 0x0600458A RID: 17802 RVA: 0x00170878 File Offset: 0x0016EA78
	public static Transform GetUITransform()
	{
		if (LocalisationUI.Instance == null)
		{
			return null;
		}
		if (LocalisationUI.Instance._uiTransform == null)
		{
			LocalisationUI.Instance._uiTransform = LocalisationUI.Instance.transform.GetChild(0);
		}
		return LocalisationUI.Instance._uiTransform;
	}

	// Token: 0x0400577D RID: 22397
	private static LocalisationUI _instance;

	// Token: 0x0400577E RID: 22398
	[Header("Text Components")]
	[SerializeField]
	private TMP_Text _titleTxt;

	// Token: 0x0400577F RID: 22399
	[SerializeField]
	private TMP_Text _confirmBtnTxt;

	// Token: 0x04005780 RID: 22400
	[Header("UI Setup")]
	[SerializeField]
	private KIDUIButton _languageButtonPrefab;

	// Token: 0x04005781 RID: 22401
	[SerializeField]
	private Transform _languageButtonGridTransform;

	// Token: 0x04005782 RID: 22402
	[SerializeField]
	private Sprite _activeSprite;

	// Token: 0x04005783 RID: 22403
	[SerializeField]
	private Sprite _inactiveSprite;

	// Token: 0x04005784 RID: 22404
	[SerializeField]
	private TMP_FontAsset _defaultFont;

	// Token: 0x04005785 RID: 22405
	[SerializeField]
	private TMP_FontAsset _japaneseFont;

	// Token: 0x04005786 RID: 22406
	private Transform _uiTransform;

	// Token: 0x04005787 RID: 22407
	private KIDUIButton _activeButton;

	// Token: 0x04005788 RID: 22408
	private List<KIDUIButton> _languageButtons = new List<KIDUIButton>();

	// Token: 0x04005789 RID: 22409
	private bool _hasConstructedUI;
}
