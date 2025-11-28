using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000ACA RID: 2762
public class LegalAgreementBodyText : MonoBehaviour
{
	// Token: 0x06004512 RID: 17682 RVA: 0x0016DFC6 File Offset: 0x0016C1C6
	private void Awake()
	{
		this.textCollection.Add(this.textBox);
	}

	// Token: 0x06004513 RID: 17683 RVA: 0x0016DFDC File Offset: 0x0016C1DC
	public void SetText(string text)
	{
		text = Regex.Unescape(text);
		string[] array = text.Split(new string[]
		{
			Environment.NewLine,
			"\\r\\n",
			"\n"
		}, 0);
		for (int i = 0; i < array.Length; i++)
		{
			Text text2;
			if (i >= this.textCollection.Count)
			{
				text2 = Object.Instantiate<Text>(this.textBox, base.transform);
				this.textCollection.Add(text2);
			}
			else
			{
				text2 = this.textCollection[i];
			}
			text2.text = array[i];
		}
	}

	// Token: 0x06004514 RID: 17684 RVA: 0x0016E06C File Offset: 0x0016C26C
	public void ClearText()
	{
		foreach (Text text in this.textCollection)
		{
			text.text = string.Empty;
		}
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x06004515 RID: 17685 RVA: 0x0016E0C8 File Offset: 0x0016C2C8
	public Task<bool> UpdateTextFromPlayFabTitleData(string key, string version)
	{
		LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10 <UpdateTextFromPlayFabTitleData>d__;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<UpdateTextFromPlayFabTitleData>d__.<>4__this = this;
		<UpdateTextFromPlayFabTitleData>d__.key = key;
		<UpdateTextFromPlayFabTitleData>d__.version = version;
		<UpdateTextFromPlayFabTitleData>d__.<>1__state = -1;
		<UpdateTextFromPlayFabTitleData>d__.<>t__builder.Start<LegalAgreementBodyText.<UpdateTextFromPlayFabTitleData>d__10>(ref <UpdateTextFromPlayFabTitleData>d__);
		return <UpdateTextFromPlayFabTitleData>d__.<>t__builder.Task;
	}

	// Token: 0x06004516 RID: 17686 RVA: 0x0016E11B File Offset: 0x0016C31B
	private void OnPlayFabError(PlayFabError obj)
	{
		Debug.LogError("ERROR: " + obj.ErrorMessage);
		this.state = LegalAgreementBodyText.State.Error;
	}

	// Token: 0x06004517 RID: 17687 RVA: 0x0016E139 File Offset: 0x0016C339
	private void OnTitleDataReceived(string text)
	{
		this.cachedText = text;
		this.state = LegalAgreementBodyText.State.Ready;
	}

	// Token: 0x17000680 RID: 1664
	// (get) Token: 0x06004518 RID: 17688 RVA: 0x0016E14C File Offset: 0x0016C34C
	public float Height
	{
		get
		{
			return this.rectTransform.rect.height;
		}
	}

	// Token: 0x040056DD RID: 22237
	[SerializeField]
	private Text textBox;

	// Token: 0x040056DE RID: 22238
	[SerializeField]
	private TextAsset textAsset;

	// Token: 0x040056DF RID: 22239
	[SerializeField]
	private RectTransform rectTransform;

	// Token: 0x040056E0 RID: 22240
	private List<Text> textCollection = new List<Text>();

	// Token: 0x040056E1 RID: 22241
	private string cachedText;

	// Token: 0x040056E2 RID: 22242
	private LegalAgreementBodyText.State state;

	// Token: 0x02000ACB RID: 2763
	private enum State
	{
		// Token: 0x040056E4 RID: 22244
		Ready,
		// Token: 0x040056E5 RID: 22245
		Loading,
		// Token: 0x040056E6 RID: 22246
		Error
	}
}
