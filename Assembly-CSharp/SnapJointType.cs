using System;

// Token: 0x02000631 RID: 1585
[Flags]
public enum SnapJointType
{
	// Token: 0x040033D7 RID: 13271
	None = 0,
	// Token: 0x040033D8 RID: 13272
	HandL = 1,
	// Token: 0x040033D9 RID: 13273
	HandR = 4,
	// Token: 0x040033DA RID: 13274
	Chest = 8,
	// Token: 0x040033DB RID: 13275
	Back = 16,
	// Token: 0x040033DC RID: 13276
	Head = 32,
	// Token: 0x040033DD RID: 13277
	Holster = 64,
	// Token: 0x040033DE RID: 13278
	ForearmL = 128,
	// Token: 0x040033DF RID: 13279
	ForearmR = 256,
	// Token: 0x040033E0 RID: 13280
	AuxHead = 512,
	// Token: 0x040033E1 RID: 13281
	AuxBody1 = 1024,
	// Token: 0x040033E2 RID: 13282
	AuxBody2 = 2048,
	// Token: 0x040033E3 RID: 13283
	AuxShoulderL = 4096,
	// Token: 0x040033E4 RID: 13284
	AuxShoulderR = 8192,
	// Token: 0x040033E5 RID: 13285
	Max = 16384
}
