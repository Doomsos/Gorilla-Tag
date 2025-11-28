using System;

// Token: 0x02000697 RID: 1687
public class GRDebugGodmodeButton : GorillaPressableReleaseButton
{
	// Token: 0x06002B05 RID: 11013 RVA: 0x000396A0 File Offset: 0x000378A0
	private void Awake()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002B06 RID: 11014 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPressedButton()
	{
	}

	// Token: 0x06002B07 RID: 11015 RVA: 0x000E7663 File Offset: 0x000E5863
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.UpdateColor();
	}

	// Token: 0x06002B08 RID: 11016 RVA: 0x000E7671 File Offset: 0x000E5871
	public override void ButtonDeactivation()
	{
		base.ButtonDeactivation();
		this.UpdateColor();
	}
}
