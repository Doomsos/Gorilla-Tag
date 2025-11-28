using System;

// Token: 0x02000329 RID: 809
[Flags]
public enum UnityLayerMask
{
	// Token: 0x04001D4D RID: 7501
	Everything = -1,
	// Token: 0x04001D4E RID: 7502
	Nothing = 0,
	// Token: 0x04001D4F RID: 7503
	Default = 1,
	// Token: 0x04001D50 RID: 7504
	TransparentFX = 2,
	// Token: 0x04001D51 RID: 7505
	IgnoreRaycast = 4,
	// Token: 0x04001D52 RID: 7506
	Zone = 8,
	// Token: 0x04001D53 RID: 7507
	Water = 16,
	// Token: 0x04001D54 RID: 7508
	UI = 32,
	// Token: 0x04001D55 RID: 7509
	MeshBakerAtlas = 64,
	// Token: 0x04001D56 RID: 7510
	GorillaEquipment = 128,
	// Token: 0x04001D57 RID: 7511
	GorillaBodyCollider = 256,
	// Token: 0x04001D58 RID: 7512
	GorillaObject = 512,
	// Token: 0x04001D59 RID: 7513
	GorillaHand = 1024,
	// Token: 0x04001D5A RID: 7514
	GorillaTrigger = 2048,
	// Token: 0x04001D5B RID: 7515
	MetaReportScreen = 4096,
	// Token: 0x04001D5C RID: 7516
	GorillaHead = 8192,
	// Token: 0x04001D5D RID: 7517
	GorillaTagCollider = 16384,
	// Token: 0x04001D5E RID: 7518
	GorillaBoundary = 32768,
	// Token: 0x04001D5F RID: 7519
	GorillaEquipmentContainer = 65536,
	// Token: 0x04001D60 RID: 7520
	LCKHide = 131072,
	// Token: 0x04001D61 RID: 7521
	GorillaInteractable = 262144,
	// Token: 0x04001D62 RID: 7522
	FirstPersonOnly = 524288,
	// Token: 0x04001D63 RID: 7523
	GorillaParticle = 1048576,
	// Token: 0x04001D64 RID: 7524
	GorillaCosmetics = 2097152,
	// Token: 0x04001D65 RID: 7525
	MirrorOnly = 4194304,
	// Token: 0x04001D66 RID: 7526
	GorillaThrowable = 8388608,
	// Token: 0x04001D67 RID: 7527
	GorillaHandSocket = 16777216,
	// Token: 0x04001D68 RID: 7528
	GorillaCosmeticParticle = 33554432,
	// Token: 0x04001D69 RID: 7529
	BuilderProp = 67108864,
	// Token: 0x04001D6A RID: 7530
	NoMirror = 134217728,
	// Token: 0x04001D6B RID: 7531
	GorillaSlingshotCollider = 268435456,
	// Token: 0x04001D6C RID: 7532
	RopeSwing = 536870912,
	// Token: 0x04001D6D RID: 7533
	Prop = 1073741824,
	// Token: 0x04001D6E RID: 7534
	Bake = -2147483648
}
