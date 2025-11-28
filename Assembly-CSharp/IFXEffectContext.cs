using System;

// Token: 0x02000BDC RID: 3036
public interface IFXEffectContext<T> where T : IFXEffectContextObject
{
	// Token: 0x170006FC RID: 1788
	// (get) Token: 0x06004B16 RID: 19222
	T effectContext { get; }

	// Token: 0x170006FD RID: 1789
	// (get) Token: 0x06004B17 RID: 19223
	FXSystemSettings settings { get; }
}
