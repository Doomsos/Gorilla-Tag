using System;

// Token: 0x02000BD8 RID: 3032
public interface IFXContext
{
	// Token: 0x170006F3 RID: 1779
	// (get) Token: 0x06004B07 RID: 19207
	FXSystemSettings settings { get; }

	// Token: 0x06004B08 RID: 19208
	void OnPlayFX();
}
