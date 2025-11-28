using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Token: 0x020001F9 RID: 505
[JsonConverter(typeof(StringEnumConverter))]
[Serializable]
public enum QuestType
{
	// Token: 0x040010B0 RID: 4272
	none,
	// Token: 0x040010B1 RID: 4273
	gameModeObjective,
	// Token: 0x040010B2 RID: 4274
	gameModeRound,
	// Token: 0x040010B3 RID: 4275
	grabObject,
	// Token: 0x040010B4 RID: 4276
	dropObject,
	// Token: 0x040010B5 RID: 4277
	eatObject,
	// Token: 0x040010B6 RID: 4278
	tapObject,
	// Token: 0x040010B7 RID: 4279
	launchedProjectile,
	// Token: 0x040010B8 RID: 4280
	moveDistance,
	// Token: 0x040010B9 RID: 4281
	swimDistance,
	// Token: 0x040010BA RID: 4282
	triggerHandEffect,
	// Token: 0x040010BB RID: 4283
	enterLocation,
	// Token: 0x040010BC RID: 4284
	misc,
	// Token: 0x040010BD RID: 4285
	critter,
	// Token: 0x040010BE RID: 4286
	fetchObject,
	// Token: 0x040010BF RID: 4287
	playerInteraction
}
