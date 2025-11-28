using System;

// Token: 0x02000B8B RID: 2955
public class GorillaNetworkLeaveTutorialTrigger : GorillaTriggerBox
{
	// Token: 0x06004917 RID: 18711 RVA: 0x001804DD File Offset: 0x0017E6DD
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		NetworkSystem.Instance.SetMyTutorialComplete();
	}
}
