using System;
using UnityEngine;
using UnityEngine.Localization;

// Token: 0x02000AEB RID: 2795
[Serializable]
public struct LocalizedStringContainer
{
	// Token: 0x06004598 RID: 17816 RVA: 0x00170B3C File Offset: 0x0016ED3C
	public string GetName()
	{
		string localizedString = this.StringReference.GetLocalizedString();
		string text = (localizedString != null) ? localizedString.ToUpper() : null;
		if (string.IsNullOrEmpty(text) || text.ToLower().Contains("no translation found"))
		{
			return this.FallbackName;
		}
		return text;
	}

	// Token: 0x04005799 RID: 22425
	[SerializeField]
	private LocalizedString StringReference;

	// Token: 0x0400579A RID: 22426
	[SerializeField]
	private string FallbackName;
}
