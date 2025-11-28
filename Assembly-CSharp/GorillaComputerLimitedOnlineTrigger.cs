using System;
using GorillaNetworking;

// Token: 0x0200090A RID: 2314
public class GorillaComputerLimitedOnlineTrigger : GorillaTriggerBox
{
	// Token: 0x06003B21 RID: 15137 RVA: 0x00138D53 File Offset: 0x00136F53
	public override void OnBoxTriggered()
	{
		GorillaComputer.instance.SetLimitOnlineScreens(true);
	}

	// Token: 0x06003B22 RID: 15138 RVA: 0x00138D62 File Offset: 0x00136F62
	public override void OnBoxExited()
	{
		GorillaComputer.instance.SetLimitOnlineScreens(false);
	}
}
