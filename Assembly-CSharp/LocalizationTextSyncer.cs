using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000AE8 RID: 2792
public class LocalizationTextSyncer : MonoBehaviour
{
	// Token: 0x0600458F RID: 17807 RVA: 0x001708FB File Offset: 0x0016EAFB
	private void Start()
	{
		this.OnLanguageChanged();
	}

	// Token: 0x06004590 RID: 17808 RVA: 0x00170903 File Offset: 0x0016EB03
	private void OnEnable()
	{
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.OnLanguageChanged));
		if (LocalisationManager.Instance == null)
		{
			return;
		}
		this.OnLanguageChanged();
	}

	// Token: 0x06004591 RID: 17809 RVA: 0x0017092A File Offset: 0x0016EB2A
	private void OnDisable()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004592 RID: 17810 RVA: 0x0017092A File Offset: 0x0016EB2A
	private void OnDestroy()
	{
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.OnLanguageChanged));
	}

	// Token: 0x06004593 RID: 17811 RVA: 0x00170940 File Offset: 0x0016EB40
	private void OnLanguageChanged()
	{
		LocalisationFontPair localisationFontPair;
		LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair);
		LocalisationFontPair localisationFontPair2;
		bool flag = this.TryGetFontDataOverride(out localisationFontPair2);
		if (!flag && !LocalisationManager.GetFontAssetForCurrentLocale(out localisationFontPair2))
		{
			return;
		}
		foreach (LocalizationTextSyncer.TextCompSyncData textCompSyncData in this._textComponentsToSync)
		{
			if (!(textCompSyncData.textComponent == null))
			{
				LocalisationFontPair localisationFontPair3;
				if (textCompSyncData.overrideLanguageSettings && textCompSyncData.GetOverrideForLanguage(out localisationFontPair3))
				{
					localisationFontPair2 = localisationFontPair3;
				}
				if (localisationFontPair2.fontAsset != null)
				{
					textCompSyncData.textComponent.font = localisationFontPair2.fontAsset;
				}
				else
				{
					textCompSyncData.textComponent.font = localisationFontPair.fontAsset;
				}
				if (flag)
				{
					textCompSyncData.textComponent.characterSpacing = localisationFontPair2.charSpacing;
					textCompSyncData.textComponent.lineSpacing = localisationFontPair2.lineSpacing;
					if (localisationFontPair2.fontSize != 0f)
					{
						textCompSyncData.textComponent.fontSize = (textCompSyncData.textComponent.fontSizeMax = localisationFontPair2.fontSize);
					}
				}
			}
		}
	}

	// Token: 0x06004594 RID: 17812 RVA: 0x00170A68 File Offset: 0x0016EC68
	private bool TryGetFontDataOverride(out LocalisationFontPair fontDataOverride)
	{
		fontDataOverride = default(LocalisationFontPair);
		for (int i = 0; i < this._universalFontOverrides.Count; i++)
		{
			if (this._universalFontOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
			{
				fontDataOverride = this._universalFontOverrides[i];
				return true;
			}
		}
		return false;
	}

	// Token: 0x04005794 RID: 22420
	[SerializeField]
	[Tooltip("List of all the Text Components - and optional overrides - that will be updated when langauge changes")]
	private List<LocalizationTextSyncer.TextCompSyncData> _textComponentsToSync = new List<LocalizationTextSyncer.TextCompSyncData>();

	// Token: 0x04005795 RID: 22421
	[SerializeField]
	[Tooltip("List of optional overrides that will be applied to ALL Text Components on this object")]
	private List<LocalisationFontPair> _universalFontOverrides = new List<LocalisationFontPair>();

	// Token: 0x02000AE9 RID: 2793
	[Serializable]
	public struct TextCompSyncData
	{
		// Token: 0x06004596 RID: 17814 RVA: 0x00170AE0 File Offset: 0x0016ECE0
		public bool GetOverrideForLanguage(out LocalisationFontPair fontData)
		{
			fontData = default(LocalisationFontPair);
			for (int i = 0; i < this._fontOverrides.Count; i++)
			{
				if (this._fontOverrides[i].ContainsLocale(LocalisationManager.CurrentLanguage))
				{
					fontData = this._fontOverrides[i];
					return true;
				}
			}
			return false;
		}

		// Token: 0x04005796 RID: 22422
		public TMP_Text textComponent;

		// Token: 0x04005797 RID: 22423
		public bool overrideLanguageSettings;

		// Token: 0x04005798 RID: 22424
		public List<LocalisationFontPair> _fontOverrides;
	}
}
