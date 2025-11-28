using System;
using TMPro;
using UnityEngine;

// Token: 0x020004E8 RID: 1256
public class RandomizeLabel : MonoBehaviour
{
	// Token: 0x06002051 RID: 8273 RVA: 0x000AB642 File Offset: 0x000A9842
	public void Randomize()
	{
		this.strings.distinct = this.distinct;
		this.label.text = this.strings.NextItem();
	}

	// Token: 0x04002AC2 RID: 10946
	public TMP_Text label;

	// Token: 0x04002AC3 RID: 10947
	public RandomStrings strings;

	// Token: 0x04002AC4 RID: 10948
	public bool distinct;
}
