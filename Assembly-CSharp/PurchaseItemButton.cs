using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020004B7 RID: 1207
public class PurchaseItemButton : GorillaPressableButton
{
	// Token: 0x06001F27 RID: 7975 RVA: 0x000A55FB File Offset: 0x000A37FB
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressPurchaseItemButton(this, isLeftHand);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000A561E File Offset: 0x000A381E
	private IEnumerator ButtonColorUpdate()
	{
		Debug.Log("did this happen?");
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x0400297D RID: 10621
	public string buttonSide;
}
