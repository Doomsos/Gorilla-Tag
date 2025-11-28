using System;
using TMPro;
using UnityEngine;

// Token: 0x020008F9 RID: 2297
public class TextWatcherTMPro : MonoBehaviour
{
	// Token: 0x06003ABD RID: 15037 RVA: 0x001363F2 File Offset: 0x001345F2
	private void Start()
	{
		this.myText = base.GetComponent<TextMeshPro>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x06003ABE RID: 15038 RVA: 0x00136418 File Offset: 0x00134618
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x06003ABF RID: 15039 RVA: 0x00136431 File Offset: 0x00134631
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x04004A25 RID: 18981
	public WatchableStringSO textToCopy;

	// Token: 0x04004A26 RID: 18982
	private TextMeshPro myText;
}
