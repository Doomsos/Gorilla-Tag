using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
public class WardrobeFunctionButton : GorillaPressableButton
{
	// Token: 0x06001FCC RID: 8140 RVA: 0x000A982E File Offset: 0x000A7A2E
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressWardrobeFunctionButton(this.function);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x00002789 File Offset: 0x00000989
	public override void UpdateColor()
	{
	}

	// Token: 0x06001FCE RID: 8142 RVA: 0x000A9855 File Offset: 0x000A7A55
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002A2A RID: 10794
	public string function;

	// Token: 0x04002A2B RID: 10795
	public float buttonFadeTime = 0.25f;
}
