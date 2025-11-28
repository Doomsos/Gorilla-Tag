using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000465 RID: 1125
[Obsolete("Replaced with bundlebutton")]
public class EarlyAccessButton : GorillaPressableButton
{
	// Token: 0x06001C7A RID: 7290 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x06001C7B RID: 7291 RVA: 0x00097358 File Offset: 0x00095558
	public void Update()
	{
		if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion)
		{
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.myText.text = "UNAVAILABLE";
		}
	}

	// Token: 0x06001C7C RID: 7292 RVA: 0x000973B2 File Offset: 0x000955B2
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressEarlyAccessButton();
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x000973D3 File Offset: 0x000955D3
	public void AlreadyOwn()
	{
		base.enabled = false;
		base.GetComponent<BoxCollider>().enabled = false;
		this.buttonRenderer.material = this.pressedMaterial;
		this.myText.text = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x00097409 File Offset: 0x00095609
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.debounceTime);
		this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
		yield break;
	}
}
