using System;
using GorillaNetworking;

// Token: 0x02000933 RID: 2355
public class UnlockCompButton : GorillaPressableButton
{
	// Token: 0x06003C2E RID: 15406 RVA: 0x0013DE9F File Offset: 0x0013C09F
	public override void Start()
	{
		this.initialized = false;
	}

	// Token: 0x06003C2F RID: 15407 RVA: 0x0013DEA8 File Offset: 0x0013C0A8
	public void Update()
	{
		if (this.testPress)
		{
			this.testPress = false;
			this.ButtonActivation();
		}
		if (!this.initialized && GorillaComputer.instance != null)
		{
			this.isOn = GorillaComputer.instance.allowedInCompetitive;
			this.UpdateColor();
			this.initialized = true;
		}
	}

	// Token: 0x06003C30 RID: 15408 RVA: 0x0013DF00 File Offset: 0x0013C100
	public override void ButtonActivation()
	{
		if (!this.isOn)
		{
			base.ButtonActivation();
			GorillaComputer.instance.CompQueueUnlockButtonPress();
			this.isOn = true;
			this.UpdateColor();
		}
	}

	// Token: 0x04004CD6 RID: 19670
	public string gameMode;

	// Token: 0x04004CD7 RID: 19671
	private bool initialized;
}
