using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008F8 RID: 2296
public class TextWatcher : MonoBehaviour
{
	// Token: 0x06003AB9 RID: 15033 RVA: 0x00136385 File Offset: 0x00134585
	private void Start()
	{
		this.myText = base.GetComponent<Text>();
		this.textToCopy.AddCallback(new Action<string>(this.OnTextChanged), true);
	}

	// Token: 0x06003ABA RID: 15034 RVA: 0x001363AB File Offset: 0x001345AB
	private void OnDestroy()
	{
		this.textToCopy.RemoveCallback(new Action<string>(this.OnTextChanged));
	}

	// Token: 0x06003ABB RID: 15035 RVA: 0x001363C4 File Offset: 0x001345C4
	private void OnTextChanged(string newText)
	{
		this.myText.text = newText;
	}

	// Token: 0x04004A23 RID: 18979
	public WatchableStringSO textToCopy;

	// Token: 0x04004A24 RID: 18980
	private Text myText;
}
