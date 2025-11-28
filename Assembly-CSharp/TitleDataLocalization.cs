using System;
using UnityEngine;

// Token: 0x02000AF0 RID: 2800
[Serializable]
public class TitleDataLocalization
{
	// Token: 0x060045B3 RID: 17843 RVA: 0x00171310 File Offset: 0x0016F510
	public string GetLocalizedText()
	{
		Debug.Log("TODO: JH - Review localization method");
		string code = LocalisationManager.CurrentLanguage.Identifier.Code;
		if (!(code == "en"))
		{
			if (code == "fr")
			{
				return this.French;
			}
			if (code == "es")
			{
				return this.Spanish;
			}
			if (code == "it")
			{
				return this.Italian;
			}
			if (code == "de")
			{
				return this.German;
			}
			if (code == "ja")
			{
				return this.Japanese;
			}
		}
		return this.English;
	}

	// Token: 0x040057B0 RID: 22448
	public string English;

	// Token: 0x040057B1 RID: 22449
	public string French;

	// Token: 0x040057B2 RID: 22450
	public string German;

	// Token: 0x040057B3 RID: 22451
	public string Spanish;

	// Token: 0x040057B4 RID: 22452
	public string Italian;

	// Token: 0x040057B5 RID: 22453
	public string Japanese;
}
