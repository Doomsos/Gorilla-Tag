using System;

// Token: 0x02000BDA RID: 3034
public interface IFXContextParems<T> where T : FXSArgs
{
	// Token: 0x170006F4 RID: 1780
	// (get) Token: 0x06004B0A RID: 19210
	FXSystemSettings settings { get; }

	// Token: 0x06004B0B RID: 19211
	void OnPlayFX(T parems);
}
