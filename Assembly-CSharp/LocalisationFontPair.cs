using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000ADF RID: 2783
[Serializable]
public struct LocalisationFontPair
{
	// Token: 0x06004556 RID: 17750 RVA: 0x0016F818 File Offset: 0x0016DA18
	public bool ContainsLocale(Locale locale)
	{
		int count = this.locales.Count;
		for (int i = 0; i < this.locales.Count; i++)
		{
			if (!(this.locales[i] == null) && this.locales[i].Identifier.Code == locale.Identifier.Code)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400575C RID: 22364
	public List<Locale> locales;

	// Token: 0x0400575D RID: 22365
	public TMP_FontAsset fontAsset;

	// Token: 0x0400575E RID: 22366
	public Font legacyFontAsset;

	// Token: 0x0400575F RID: 22367
	public float charSpacing;

	// Token: 0x04005760 RID: 22368
	public float lineSpacing;

	// Token: 0x04005761 RID: 22369
	public float fontSize;
}
