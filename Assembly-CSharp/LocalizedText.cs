using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Events;

// Token: 0x02000AEC RID: 2796
[DisallowMultipleComponent]
public class LocalizedText : LocalizeStringEvent
{
	// Token: 0x06004599 RID: 17817 RVA: 0x00170B69 File Offset: 0x0016ED69
	public bool HasFontOverrides()
	{
		return this._localisationFontsOverrides.Count > 0;
	}

	// Token: 0x1700068E RID: 1678
	// (get) Token: 0x0600459A RID: 17818 RVA: 0x00170B79 File Offset: 0x0016ED79
	private TextComponentLegacySupportStore TextComponent
	{
		get
		{
			if (!this._textComponent.IsValid)
			{
				this._textComponent = new TextComponentLegacySupportStore(base.transform);
			}
			return this._textComponent;
		}
	}

	// Token: 0x0600459B RID: 17819 RVA: 0x00170BA0 File Offset: 0x0016EDA0
	private void Awake()
	{
		this._textComponent = new TextComponentLegacySupportStore(base.transform);
		base.OnUpdateString = new UnityEventString();
		base.OnUpdateString.AddListener(delegate(string val)
		{
			this.OnLocaleChanged(val);
		});
		if (!this.TextComponent.IsValid)
		{
			base.gameObject.AddComponent<TMP_Text>();
			this._textComponent = new TextComponentLegacySupportStore(base.transform);
		}
	}

	// Token: 0x0600459C RID: 17820 RVA: 0x00170C10 File Offset: 0x0016EE10
	protected override void UpdateString(string value)
	{
		LocalizedText.<UpdateString>d__11 <UpdateString>d__;
		<UpdateString>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<UpdateString>d__.<>4__this = this;
		<UpdateString>d__.value = value;
		<UpdateString>d__.<>1__state = -1;
		<UpdateString>d__.<>t__builder.Start<LocalizedText.<UpdateString>d__11>(ref <UpdateString>d__);
	}

	// Token: 0x0600459D RID: 17821 RVA: 0x00170C50 File Offset: 0x0016EE50
	private void OnLocaleChanged(string newText)
	{
		LocalizedText.<OnLocaleChanged>d__12 <OnLocaleChanged>d__;
		<OnLocaleChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnLocaleChanged>d__.<>4__this = this;
		<OnLocaleChanged>d__.newText = newText;
		<OnLocaleChanged>d__.<>1__state = -1;
		<OnLocaleChanged>d__.<>t__builder.Start<LocalizedText.<OnLocaleChanged>d__12>(ref <OnLocaleChanged>d__);
	}

	// Token: 0x0600459E RID: 17822 RVA: 0x00170C90 File Offset: 0x0016EE90
	private bool GetLocalizedFonts(out LocalisationFontPair fontData)
	{
		fontData = default(LocalisationFontPair);
		if (!this.HasFontOverrides())
		{
			return LocalisationManager.GetFontAssetForCurrentLocale(out fontData);
		}
		for (int i = 0; i < this._localisationFontsOverrides.Count; i++)
		{
			if (this._localisationFontsOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
			{
				fontData = new LocalisationFontPair
				{
					fontAsset = this._localisationFontsOverrides[i].fontAsset,
					legacyFontAsset = this._localisationFontsOverrides[i].legacyFontAsset,
					charSpacing = this._localisationFontsOverrides[i].charSpacing
				};
				return true;
			}
		}
		return LocalisationManager.GetFontAssetForCurrentLocale(out fontData);
	}

	// Token: 0x0400579B RID: 22427
	[SerializeField]
	private bool _isLocalized;

	// Token: 0x0400579C RID: 22428
	[SerializeField]
	private bool _isNewKey;

	// Token: 0x0400579D RID: 22429
	[SerializeField]
	private string _newKeyName;

	// Token: 0x0400579E RID: 22430
	[SerializeField]
	private ELocale _previewLocale;

	// Token: 0x0400579F RID: 22431
	[SerializeField]
	private List<LocalisationFontPair> _localisationFontsOverrides = new List<LocalisationFontPair>();

	// Token: 0x040057A0 RID: 22432
	private static List<ELocale> _cachedELocalesList = new List<ELocale>();

	// Token: 0x040057A1 RID: 22433
	private TextComponentLegacySupportStore _textComponent;
}
