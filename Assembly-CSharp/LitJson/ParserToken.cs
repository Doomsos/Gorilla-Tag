using System;

namespace LitJson
{
	// Token: 0x02000D6A RID: 3434
	internal enum ParserToken
	{
		// Token: 0x04006196 RID: 24982
		None = 65536,
		// Token: 0x04006197 RID: 24983
		Number,
		// Token: 0x04006198 RID: 24984
		True,
		// Token: 0x04006199 RID: 24985
		False,
		// Token: 0x0400619A RID: 24986
		Null,
		// Token: 0x0400619B RID: 24987
		CharSeq,
		// Token: 0x0400619C RID: 24988
		Char,
		// Token: 0x0400619D RID: 24989
		Text,
		// Token: 0x0400619E RID: 24990
		Object,
		// Token: 0x0400619F RID: 24991
		ObjectPrime,
		// Token: 0x040061A0 RID: 24992
		Pair,
		// Token: 0x040061A1 RID: 24993
		PairRest,
		// Token: 0x040061A2 RID: 24994
		Array,
		// Token: 0x040061A3 RID: 24995
		ArrayPrime,
		// Token: 0x040061A4 RID: 24996
		Value,
		// Token: 0x040061A5 RID: 24997
		ValueRest,
		// Token: 0x040061A6 RID: 24998
		String,
		// Token: 0x040061A7 RID: 24999
		End,
		// Token: 0x040061A8 RID: 25000
		Epsilon
	}
}
