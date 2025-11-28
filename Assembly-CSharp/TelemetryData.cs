using System;
using System.Collections.Generic;

// Token: 0x02000A27 RID: 2599
public struct TelemetryData
{
	// Token: 0x040052FC RID: 21244
	public string EventName;

	// Token: 0x040052FD RID: 21245
	public string[] CustomTags;

	// Token: 0x040052FE RID: 21246
	public Dictionary<string, string> BodyData;
}
