using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004C1 RID: 1217
public class CreatorCodeSmallDisplay : MonoBehaviour
{
	// Token: 0x06001F66 RID: 8038 RVA: 0x000A7156 File Offset: 0x000A5356
	private void Awake()
	{
		this.codeText.text = "CREATOR CODE: <NONE>";
		ATM_Manager.instance.smallDisplays.Add(this);
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x000A717A File Offset: 0x000A537A
	public void SetCode(string code)
	{
		if (code == "")
		{
			this.codeText.text = "CREATOR CODE: <NONE>";
			return;
		}
		this.codeText.text = "CREATOR CODE: " + code;
	}

	// Token: 0x06001F68 RID: 8040 RVA: 0x000A71B0 File Offset: 0x000A53B0
	public void SuccessfulPurchase(string memberName)
	{
		if (!string.IsNullOrWhiteSpace(memberName))
		{
			this.codeText.text = "SUPPORTED: " + memberName + "!";
		}
	}

	// Token: 0x040029C7 RID: 10695
	public Text codeText;

	// Token: 0x040029C8 RID: 10696
	private const string CreatorCode = "CREATOR CODE: ";

	// Token: 0x040029C9 RID: 10697
	private const string CreatorSupported = "SUPPORTED: ";

	// Token: 0x040029CA RID: 10698
	private const string NoCreator = "<NONE>";
}
