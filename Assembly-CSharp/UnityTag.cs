using System;

// Token: 0x0200032B RID: 811
public enum UnityTag
{
	// Token: 0x04001D70 RID: 7536
	Invalid = -1,
	// Token: 0x04001D71 RID: 7537
	Untagged,
	// Token: 0x04001D72 RID: 7538
	Respawn,
	// Token: 0x04001D73 RID: 7539
	Finish,
	// Token: 0x04001D74 RID: 7540
	EditorOnly,
	// Token: 0x04001D75 RID: 7541
	MainCamera,
	// Token: 0x04001D76 RID: 7542
	Player,
	// Token: 0x04001D77 RID: 7543
	GameController,
	// Token: 0x04001D78 RID: 7544
	SceneChanger,
	// Token: 0x04001D79 RID: 7545
	PlayerOffset,
	// Token: 0x04001D7A RID: 7546
	GorillaTagManager,
	// Token: 0x04001D7B RID: 7547
	GorillaTagCollider,
	// Token: 0x04001D7C RID: 7548
	GorillaPlayer,
	// Token: 0x04001D7D RID: 7549
	GorillaObject,
	// Token: 0x04001D7E RID: 7550
	GorillaGameManager,
	// Token: 0x04001D7F RID: 7551
	GorillaCosmetic,
	// Token: 0x04001D80 RID: 7552
	projectile,
	// Token: 0x04001D81 RID: 7553
	FxTemporaire,
	// Token: 0x04001D82 RID: 7554
	SlingshotProjectile,
	// Token: 0x04001D83 RID: 7555
	SlingshotProjectileTrail,
	// Token: 0x04001D84 RID: 7556
	SlingshotProjectilePlayerImpactFX,
	// Token: 0x04001D85 RID: 7557
	SlingshotProjectileSurfaceImpactFX,
	// Token: 0x04001D86 RID: 7558
	BalloonPopFX,
	// Token: 0x04001D87 RID: 7559
	WorldShareableItem,
	// Token: 0x04001D88 RID: 7560
	HornsSlingshotProjectile,
	// Token: 0x04001D89 RID: 7561
	HornsSlingshotProjectileTrail,
	// Token: 0x04001D8A RID: 7562
	HornsSlingshotProjectilePlayerImpactFX,
	// Token: 0x04001D8B RID: 7563
	HornsSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04001D8C RID: 7564
	FryingPan,
	// Token: 0x04001D8D RID: 7565
	LeafPileImpactFX,
	// Token: 0x04001D8E RID: 7566
	BalloonPopFx,
	// Token: 0x04001D8F RID: 7567
	CloudSlingshotProjectile,
	// Token: 0x04001D90 RID: 7568
	CloudSlingshotProjectileTrail,
	// Token: 0x04001D91 RID: 7569
	CloudSlingshotProjectilePlayerImpactFX,
	// Token: 0x04001D92 RID: 7570
	CloudSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04001D93 RID: 7571
	SnowballProjectile,
	// Token: 0x04001D94 RID: 7572
	SnowballProjectileImpactFX,
	// Token: 0x04001D95 RID: 7573
	CupidBowProjectile,
	// Token: 0x04001D96 RID: 7574
	CupidBowProjectileTrail,
	// Token: 0x04001D97 RID: 7575
	CupidBowProjectileSurfaceImpactFX,
	// Token: 0x04001D98 RID: 7576
	NoCrazyCheck,
	// Token: 0x04001D99 RID: 7577
	IceSlingshotProjectile,
	// Token: 0x04001D9A RID: 7578
	IceSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04001D9B RID: 7579
	IceSlingshotProjectileTrail,
	// Token: 0x04001D9C RID: 7580
	ElfBowProjectile,
	// Token: 0x04001D9D RID: 7581
	ElfBowProjectileSurfaceImpactFX,
	// Token: 0x04001D9E RID: 7582
	ElfBowProjectileTrail,
	// Token: 0x04001D9F RID: 7583
	RenderIfSmall,
	// Token: 0x04001DA0 RID: 7584
	DeleteOnNonBetaBuild,
	// Token: 0x04001DA1 RID: 7585
	DeleteOnNonDebugBuild,
	// Token: 0x04001DA2 RID: 7586
	FlagColoringCauldon,
	// Token: 0x04001DA3 RID: 7587
	WaterRippleEffect,
	// Token: 0x04001DA4 RID: 7588
	WaterSplashEffect,
	// Token: 0x04001DA5 RID: 7589
	FireworkMortarProjectile,
	// Token: 0x04001DA6 RID: 7590
	FireworkMortarProjectileImpactFX,
	// Token: 0x04001DA7 RID: 7591
	WaterBalloonProjectile,
	// Token: 0x04001DA8 RID: 7592
	WaterBalloonProjectileImpactFX,
	// Token: 0x04001DA9 RID: 7593
	PlayerHeadTrigger,
	// Token: 0x04001DAA RID: 7594
	WizardStaff,
	// Token: 0x04001DAB RID: 7595
	LurkerGhost,
	// Token: 0x04001DAC RID: 7596
	HauntedObject,
	// Token: 0x04001DAD RID: 7597
	WanderingGhost,
	// Token: 0x04001DAE RID: 7598
	LavaSurfaceRock,
	// Token: 0x04001DAF RID: 7599
	LavaRockProjectile,
	// Token: 0x04001DB0 RID: 7600
	LavaRockProjectileImpactFX,
	// Token: 0x04001DB1 RID: 7601
	MoltenSlingshotProjectile,
	// Token: 0x04001DB2 RID: 7602
	MoltenSlingshotProjectileTrail,
	// Token: 0x04001DB3 RID: 7603
	MoltenSlingshotProjectileSurfaceImpactFX,
	// Token: 0x04001DB4 RID: 7604
	MoltenSlingshotProjectilePlayerImpactFX,
	// Token: 0x04001DB5 RID: 7605
	SpiderBowProjectile,
	// Token: 0x04001DB6 RID: 7606
	SpiderBowProjectileTrail,
	// Token: 0x04001DB7 RID: 7607
	SpiderBowProjectileSurfaceImpactFX,
	// Token: 0x04001DB8 RID: 7608
	SpiderBowProjectilePlayerImpactFX,
	// Token: 0x04001DB9 RID: 7609
	ZoneRoot,
	// Token: 0x04001DBA RID: 7610
	DontProcessMaterials,
	// Token: 0x04001DBB RID: 7611
	OrnamentProjectileSurfaceImpactFX,
	// Token: 0x04001DBC RID: 7612
	BucketGiftCane,
	// Token: 0x04001DBD RID: 7613
	BucketGiftCoal,
	// Token: 0x04001DBE RID: 7614
	BucketGiftRoll,
	// Token: 0x04001DBF RID: 7615
	BucketGiftRound,
	// Token: 0x04001DC0 RID: 7616
	BucketGiftSquare,
	// Token: 0x04001DC1 RID: 7617
	OrnamentProjectile,
	// Token: 0x04001DC2 RID: 7618
	OrnamentShatterFX,
	// Token: 0x04001DC3 RID: 7619
	ScienceCandyProjectile,
	// Token: 0x04001DC4 RID: 7620
	ScienceCandyImpactFX,
	// Token: 0x04001DC5 RID: 7621
	PaperAirplaneProjectile,
	// Token: 0x04001DC6 RID: 7622
	DevilBowProjectile,
	// Token: 0x04001DC7 RID: 7623
	DevilBowProjectileTrail,
	// Token: 0x04001DC8 RID: 7624
	DevilBowProjectileSurfaceImpactFX,
	// Token: 0x04001DC9 RID: 7625
	DevilBowProjectilePlayerImpactFX,
	// Token: 0x04001DCA RID: 7626
	FireFX,
	// Token: 0x04001DCB RID: 7627
	FishFood,
	// Token: 0x04001DCC RID: 7628
	FishFoodImpactFX,
	// Token: 0x04001DCD RID: 7629
	LeafNinjaStarProjectile,
	// Token: 0x04001DCE RID: 7630
	LeafNinjaStarProjectileC1,
	// Token: 0x04001DCF RID: 7631
	LeafNinjaStarProjectileC2,
	// Token: 0x04001DD0 RID: 7632
	SamuraiBowProjectile,
	// Token: 0x04001DD1 RID: 7633
	SamuraiBowProjectileTrail,
	// Token: 0x04001DD2 RID: 7634
	SamuraiBowProjectileSurfaceImpactFX,
	// Token: 0x04001DD3 RID: 7635
	SamuraiBowProjectilePlayerImpactFX,
	// Token: 0x04001DD4 RID: 7636
	DragonSlingProjectile,
	// Token: 0x04001DD5 RID: 7637
	DragonSlingProjectileTrail,
	// Token: 0x04001DD6 RID: 7638
	DragonSlingProjectileSurfaceImpactFX,
	// Token: 0x04001DD7 RID: 7639
	DragonSlingProjectilePlayerImpactFX,
	// Token: 0x04001DD8 RID: 7640
	FireballProjectile,
	// Token: 0x04001DD9 RID: 7641
	StealthHandTapFX,
	// Token: 0x04001DDA RID: 7642
	EnvPieceTree01,
	// Token: 0x04001DDB RID: 7643
	FxSnapPiecePlaced,
	// Token: 0x04001DDC RID: 7644
	FxSnapPieceDisconnected,
	// Token: 0x04001DDD RID: 7645
	FxSnapPieceGrabbed,
	// Token: 0x04001DDE RID: 7646
	FxSnapPieceLocationLock,
	// Token: 0x04001DDF RID: 7647
	CyberNinjaStarProjectile,
	// Token: 0x04001DE0 RID: 7648
	RoomLight,
	// Token: 0x04001DE1 RID: 7649
	SamplesInfoPanel,
	// Token: 0x04001DE2 RID: 7650
	GorillaHandLeft,
	// Token: 0x04001DE3 RID: 7651
	GorillaHandRight,
	// Token: 0x04001DE4 RID: 7652
	GorillaHandSocket,
	// Token: 0x04001DE5 RID: 7653
	PlayingCardProjectile,
	// Token: 0x04001DE6 RID: 7654
	RottenPumpkinProjectile,
	// Token: 0x04001DE7 RID: 7655
	FxSnapPieceRecycle,
	// Token: 0x04001DE8 RID: 7656
	FxSnapPieceDispenser,
	// Token: 0x04001DE9 RID: 7657
	AppleProjectile,
	// Token: 0x04001DEA RID: 7658
	AppleProjectileSurfaceImpactFX,
	// Token: 0x04001DEB RID: 7659
	RecyclerForceVolumeFX,
	// Token: 0x04001DEC RID: 7660
	FxSnapPieceTooHeavy,
	// Token: 0x04001DED RID: 7661
	FxBuilderPrivatePlotClaimed,
	// Token: 0x04001DEE RID: 7662
	TrickTreatCandy,
	// Token: 0x04001DEF RID: 7663
	TrickTreatEyeball,
	// Token: 0x04001DF0 RID: 7664
	TrickTreatBat,
	// Token: 0x04001DF1 RID: 7665
	TrickTreatBomb,
	// Token: 0x04001DF2 RID: 7666
	TrickTreatSurfaceImpact,
	// Token: 0x04001DF3 RID: 7667
	TrickTreatBatImpact,
	// Token: 0x04001DF4 RID: 7668
	TrickTreatBombImpact,
	// Token: 0x04001DF5 RID: 7669
	GuardianSlapFX,
	// Token: 0x04001DF6 RID: 7670
	GuardianSlamFX,
	// Token: 0x04001DF7 RID: 7671
	GuardianIdolLandedFX,
	// Token: 0x04001DF8 RID: 7672
	GuardianIdolFallFX,
	// Token: 0x04001DF9 RID: 7673
	GuardianIdolTappedFX,
	// Token: 0x04001DFA RID: 7674
	VotingRockProjectile,
	// Token: 0x04001DFB RID: 7675
	LeafPileImpactFXMedium,
	// Token: 0x04001DFC RID: 7676
	LeafPileImpactFXSmall,
	// Token: 0x04001DFD RID: 7677
	WoodenSword,
	// Token: 0x04001DFE RID: 7678
	WoodenShield,
	// Token: 0x04001DFF RID: 7679
	FxBuilderShrink,
	// Token: 0x04001E00 RID: 7680
	FxBuilderGrow,
	// Token: 0x04001E01 RID: 7681
	FxSnapPieceWreathJump,
	// Token: 0x04001E02 RID: 7682
	ElfLauncherElf,
	// Token: 0x04001E03 RID: 7683
	RubberBandCar,
	// Token: 0x04001E04 RID: 7684
	SnowPileImpactFX,
	// Token: 0x04001E05 RID: 7685
	FirecrackersProjectile,
	// Token: 0x04001E06 RID: 7686
	PaperAirplaneSquareProjectile,
	// Token: 0x04001E07 RID: 7687
	SmokeBombProjectile,
	// Token: 0x04001E08 RID: 7688
	ThrowableHeartProjectile,
	// Token: 0x04001E09 RID: 7689
	SunFlowers,
	// Token: 0x04001E0A RID: 7690
	RobotCannonProjectile,
	// Token: 0x04001E0B RID: 7691
	RobotCannonProjectileImpact,
	// Token: 0x04001E0C RID: 7692
	SmokeBombExplosionEffect,
	// Token: 0x04001E0D RID: 7693
	FireCrackerExplosionEffect,
	// Token: 0x04001E0E RID: 7694
	GorillaMouth
}
