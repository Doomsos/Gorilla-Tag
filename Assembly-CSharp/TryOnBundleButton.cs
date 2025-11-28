using System;
using GorillaNetworking.Store;

// Token: 0x020004C2 RID: 1218
public class TryOnBundleButton : GorillaPressableButton
{
	// Token: 0x06001F6A RID: 8042 RVA: 0x000A71D5 File Offset: 0x000A53D5
	public override void ButtonActivationWithHand(bool isLeftHand)
	{
		base.ButtonActivationWithHand(isLeftHand);
		BundleManager.instance.PressTryOnBundleButton(this, isLeftHand);
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x000A71EC File Offset: 0x000A53EC
	public override void UpdateColor()
	{
		if (this.playfabBundleID == "NULL")
		{
			this.buttonRenderer.material = this.unpressedMaterial;
			if (this.myText != null)
			{
				this.myText.text = "";
			}
			return;
		}
		base.UpdateColor();
	}

	// Token: 0x040029CB RID: 10699
	public int buttonIndex;

	// Token: 0x040029CC RID: 10700
	public string playfabBundleID = "NULL";
}
