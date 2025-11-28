using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004B5 RID: 1205
public class PurchaseCurrencyButton : GorillaPressableButton
{
	// Token: 0x06001F1E RID: 7966 RVA: 0x000A554A File Offset: 0x000A374A
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		ATM_Manager.instance.PressCurrencyPurchaseButton(this.purchaseCurrencySize);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x000A5571 File Offset: 0x000A3771
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.sharedMaterial = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.sharedMaterial = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002978 RID: 10616
	public string purchaseCurrencySize;

	// Token: 0x04002979 RID: 10617
	public float buttonFadeTime = 0.25f;
}
