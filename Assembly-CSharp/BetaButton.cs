using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000455 RID: 1109
public class BetaButton : GorillaPressableButton
{
	// Token: 0x06001C3F RID: 7231 RVA: 0x000962B0 File Offset: 0x000944B0
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.count++;
		base.StartCoroutine(this.ButtonColorUpdate());
		if (this.count >= 10)
		{
			this.betaParent.SetActive(false);
			PlayerPrefs.SetString("CheckedBox2", "true");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06001C40 RID: 7232 RVA: 0x00096308 File Offset: 0x00094508
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002649 RID: 9801
	public GameObject betaParent;

	// Token: 0x0400264A RID: 9802
	public int count;

	// Token: 0x0400264B RID: 9803
	public float buttonFadeTime = 0.25f;

	// Token: 0x0400264C RID: 9804
	public Text messageText;
}
