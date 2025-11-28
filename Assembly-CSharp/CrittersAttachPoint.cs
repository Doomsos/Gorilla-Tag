using System;

// Token: 0x0200004A RID: 74
public class CrittersAttachPoint : CrittersActor
{
	// Token: 0x0600016E RID: 366 RVA: 0x00002789 File Offset: 0x00000989
	public override void ProcessRemote()
	{
	}

	// Token: 0x04000194 RID: 404
	public bool fixedOrientation = true;

	// Token: 0x04000195 RID: 405
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x04000196 RID: 406
	public bool isLeft;

	// Token: 0x0200004B RID: 75
	public enum AnchoredLocationTypes
	{
		// Token: 0x04000198 RID: 408
		Arm,
		// Token: 0x04000199 RID: 409
		Chest,
		// Token: 0x0400019A RID: 410
		Back
	}
}
