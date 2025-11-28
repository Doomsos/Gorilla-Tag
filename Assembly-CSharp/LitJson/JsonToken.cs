using System;

namespace LitJson
{
	// Token: 0x02000D62 RID: 3426
	public enum JsonToken
	{
		// Token: 0x04006153 RID: 24915
		None,
		// Token: 0x04006154 RID: 24916
		ObjectStart,
		// Token: 0x04006155 RID: 24917
		PropertyName,
		// Token: 0x04006156 RID: 24918
		ObjectEnd,
		// Token: 0x04006157 RID: 24919
		ArrayStart,
		// Token: 0x04006158 RID: 24920
		ArrayEnd,
		// Token: 0x04006159 RID: 24921
		Int,
		// Token: 0x0400615A RID: 24922
		Long,
		// Token: 0x0400615B RID: 24923
		Double,
		// Token: 0x0400615C RID: 24924
		String,
		// Token: 0x0400615D RID: 24925
		Boolean,
		// Token: 0x0400615E RID: 24926
		Null
	}
}
