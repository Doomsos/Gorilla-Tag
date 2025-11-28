using System;
using UnityEngine.UI;

// Token: 0x02000578 RID: 1400
public class BuilderKioskButton : GorillaPressableButton
{
	// Token: 0x06002359 RID: 9049 RVA: 0x000B97E5 File Offset: 0x000B79E5
	public override void Start()
	{
		this.currentPieceSet = BuilderKiosk.nullItem;
	}

	// Token: 0x0600235A RID: 9050 RVA: 0x000B97F2 File Offset: 0x000B79F2
	public override void UpdateColor()
	{
		if (this.currentPieceSet.isNullItem)
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			this.myText.text = "";
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x0600235B RID: 9051 RVA: 0x000B9829 File Offset: 0x000B7A29
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivation();
	}

	// Token: 0x04002E36 RID: 11830
	public BuilderSetManager.BuilderSetStoreItem currentPieceSet;

	// Token: 0x04002E37 RID: 11831
	public BuilderKiosk kiosk;

	// Token: 0x04002E38 RID: 11832
	public Text setNameText;
}
