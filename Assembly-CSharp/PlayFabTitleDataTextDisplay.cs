using System;
using GorillaNetworking;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

// Token: 0x0200092B RID: 2347
public class PlayFabTitleDataTextDisplay : MonoBehaviour, IBuildValidation
{
	// Token: 0x1700058D RID: 1421
	// (get) Token: 0x06003BFA RID: 15354 RVA: 0x0013CB4A File Offset: 0x0013AD4A
	public string playFabKeyValue
	{
		get
		{
			return this.playfabKey;
		}
	}

	// Token: 0x06003BFB RID: 15355 RVA: 0x0013CB54 File Offset: 0x0013AD54
	private void Start()
	{
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
		if (!this._hasRegisteredCallback)
		{
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		}
	}

	// Token: 0x06003BFC RID: 15356 RVA: 0x0013CBE9 File Offset: 0x0013ADE9
	private void OnEnable()
	{
		if (LocalisationManager.Instance == null)
		{
			return;
		}
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this._hasRegisteredCallback = true;
	}

	// Token: 0x06003BFD RID: 15357 RVA: 0x0013CC11 File Offset: 0x0013AE11
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		this._hasRegisteredCallback = false;
	}

	// Token: 0x06003BFE RID: 15358 RVA: 0x0013CC2C File Offset: 0x0013AE2C
	private void OnPlayFabError(PlayFabError error)
	{
		if (this.textBox != null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"PlayFabTitleDataTextDisplay: PlayFab error retrieving title data for key ",
				this.playfabKey,
				" displayed ",
				this.fallbackText,
				": ",
				error.GenerateErrorReport()
			}));
			if (this._fallbackLocalizedText == null || this._fallbackLocalizedText.IsEmpty)
			{
				this.textBox.text = this.fallbackText;
				return;
			}
			string text;
			if (!LocalisationManager.TryGetTranslationForCurrentLocaleWithLocString(this._fallbackLocalizedText, out text, this.fallbackText, null))
			{
				Debug.LogError("[LOCALIZATION::PLAYFAB_TITLEDATA_TEXT_DISPLAY] Failed to get key for PlayFab Title Data Text [_fallbackLocalizedText]");
			}
			this.textBox.text = text;
		}
	}

	// Token: 0x06003BFF RID: 15359 RVA: 0x0013CCE0 File Offset: 0x0013AEE0
	private void OnLanguageChanged()
	{
		if (string.IsNullOrEmpty(this._cachedText))
		{
			Debug.LogError("[LOCALIZATION::PLAY_FAB_TITLE_DATA_TEXT_DISPLAY] [_cachedText] is not set yet, is this being called before title data has been obtained?");
			return;
		}
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
	}

	// Token: 0x06003C00 RID: 15360 RVA: 0x0013CD30 File Offset: 0x0013AF30
	private void OnTitleDataRequestComplete(string titleDataResult)
	{
		if (this.textBox != null)
		{
			this._cachedText = titleDataResult;
			string text = titleDataResult.Replace("\\r", "\r").Replace("\\n", "\n");
			if (text.get_Chars(0) == '"' && text.get_Chars(text.Length - 1) == '"')
			{
				text = text.Substring(1, text.Length - 2);
			}
			this.textBox.text = text;
			Debug.Log("PlayFabTitleDataTextDisplay: text: " + text);
		}
	}

	// Token: 0x06003C01 RID: 15361 RVA: 0x0013CDBB File Offset: 0x0013AFBB
	private void OnNewTitleDataAdded(string key)
	{
		if (key == this.playfabKey && this.textBox != null)
		{
			this.textBox.color = this.newUpdateColor;
		}
	}

	// Token: 0x06003C02 RID: 15362 RVA: 0x0013CDEA File Offset: 0x0013AFEA
	private void OnDestroy()
	{
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.RemoveListener(new UnityAction<string>(this.OnNewTitleDataAdded));
	}

	// Token: 0x06003C03 RID: 15363 RVA: 0x0013CE07 File Offset: 0x0013B007
	public bool BuildValidationCheck()
	{
		if (this.textBox == null)
		{
			Debug.LogError("text reference is null! sign text will be broken");
			return false;
		}
		return true;
	}

	// Token: 0x06003C04 RID: 15364 RVA: 0x0013CE24 File Offset: 0x0013B024
	public void ChangeTitleDataAtRuntime(string newTitleDataKey)
	{
		this.playfabKey = newTitleDataKey;
		if (this.textBox != null)
		{
			this.textBox.color = this.defaultTextColor;
		}
		else
		{
			Debug.LogError("The TextBox is null on this PlayFabTitleDataTextDisplay component");
		}
		PlayFabTitleDataCache.Instance.OnTitleDataUpdate.AddListener(new UnityAction<string>(this.OnNewTitleDataAdded));
		PlayFabTitleDataCache.Instance.GetTitleData(this.playfabKey, new Action<string>(this.OnTitleDataRequestComplete), new Action<PlayFabError>(this.OnPlayFabError), false);
	}

	// Token: 0x04004C8A RID: 19594
	[SerializeField]
	private TextMeshPro textBox;

	// Token: 0x04004C8B RID: 19595
	[SerializeField]
	private Color newUpdateColor = Color.magenta;

	// Token: 0x04004C8C RID: 19596
	[SerializeField]
	private Color defaultTextColor = Color.white;

	// Token: 0x04004C8D RID: 19597
	[Tooltip("PlayFab Title Data key from where to pull display text")]
	[SerializeField]
	private string playfabKey;

	// Token: 0x04004C8E RID: 19598
	[Tooltip("Text to display when error occurs during fetch")]
	[TextArea(3, 5)]
	[SerializeField]
	private string fallbackText;

	// Token: 0x04004C8F RID: 19599
	[SerializeField]
	private LocalizedString _fallbackLocalizedText;

	// Token: 0x04004C90 RID: 19600
	private bool _hasRegisteredCallback;

	// Token: 0x04004C91 RID: 19601
	private string _cachedText = string.Empty;
}
