using System;
using UnityEngine;

// Token: 0x02000813 RID: 2067
public interface IRangedVariable<T> : IVariable<T>, IVariable
{
	// Token: 0x170004D8 RID: 1240
	// (get) Token: 0x06003664 RID: 13924
	// (set) Token: 0x06003665 RID: 13925
	T Min { get; set; }

	// Token: 0x170004D9 RID: 1241
	// (get) Token: 0x06003666 RID: 13926
	// (set) Token: 0x06003667 RID: 13927
	T Max { get; set; }

	// Token: 0x170004DA RID: 1242
	// (get) Token: 0x06003668 RID: 13928
	T Range { get; }

	// Token: 0x170004DB RID: 1243
	// (get) Token: 0x06003669 RID: 13929
	AnimationCurve Curve { get; }
}
