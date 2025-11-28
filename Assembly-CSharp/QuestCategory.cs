using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

// Token: 0x020001FA RID: 506
[JsonConverter(typeof(StringEnumConverter))]
[Serializable]
public enum QuestCategory
{
	// Token: 0x040010C1 RID: 4289
	NONE,
	// Token: 0x040010C2 RID: 4290
	Social,
	// Token: 0x040010C3 RID: 4291
	Exploration,
	// Token: 0x040010C4 RID: 4292
	Gameplay,
	// Token: 0x040010C5 RID: 4293
	GameRound,
	// Token: 0x040010C6 RID: 4294
	Tag
}
