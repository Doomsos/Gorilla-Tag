using System;

// Token: 0x0200004C RID: 76
public class CrittersAttachPointSettings : CrittersActorSettings
{
	// Token: 0x06000170 RID: 368 RVA: 0x000096BD File Offset: 0x000078BD
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersAttachPoint crittersAttachPoint = (CrittersAttachPoint)this.parentActor;
		crittersAttachPoint.anchorLocation = this.anchoredLocation;
		crittersAttachPoint.rb.isKinematic = true;
		crittersAttachPoint.isLeft = this.isLeft;
	}

	// Token: 0x0400019B RID: 411
	public bool isLeft;

	// Token: 0x0400019C RID: 412
	public CrittersAttachPoint.AnchoredLocationTypes anchoredLocation;
}
