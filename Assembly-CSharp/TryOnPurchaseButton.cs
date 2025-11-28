using System;
using System.Collections;
using GorillaNetworking.Store;
using UnityEngine;

// Token: 0x020004C4 RID: 1220
public class TryOnPurchaseButton : GorillaPressableButton
{
	// Token: 0x06001F87 RID: 8071 RVA: 0x000A7C04 File Offset: 0x000A5E04
	public void Update()
	{
		if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x000A7C66 File Offset: 0x000A5E66
	public override void ButtonActivation()
	{
		if (this.bError)
		{
			return;
		}
		base.ButtonActivation();
		BundleManager.instance.PressPurchaseTryOnBundleButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x000A7C90 File Offset: 0x000A5E90
	public void AlreadyOwn()
	{
		if (this.bError)
		{
			return;
		}
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = this.AlreadyOwnText;
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x000A7CD0 File Offset: 0x000A5ED0
	public void ResetButton()
	{
		if (this.bError)
		{
			return;
		}
		base.enabled = true;
		base.GetComponent<BoxCollider>().enabled = true;
		this.buttonRenderer.material = this.unpressedMaterial;
		this.SetOffText(true, false, false);
	}

	// Token: 0x06001F8B RID: 8075 RVA: 0x000A7D08 File Offset: 0x000A5F08
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}

	// Token: 0x06001F8C RID: 8076 RVA: 0x000A7D17 File Offset: 0x000A5F17
	public void ErrorHappened()
	{
		this.bError = true;
		this.myText.text = this.ErrorText;
		this.buttonRenderer.material = this.unpressedMaterial;
		base.enabled = false;
		this.isOn = false;
	}

	// Token: 0x040029DF RID: 10719
	public bool bError;

	// Token: 0x040029E0 RID: 10720
	public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

	// Token: 0x040029E1 RID: 10721
	public string AlreadyOwnText;
}
