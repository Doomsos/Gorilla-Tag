using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000BDB RID: 3035
public interface IFXEffectContextObject
{
	// Token: 0x170006F5 RID: 1781
	// (get) Token: 0x06004B0C RID: 19212
	List<int> PrefabPoolIds { get; }

	// Token: 0x170006F6 RID: 1782
	// (get) Token: 0x06004B0D RID: 19213
	Vector3 Position { get; }

	// Token: 0x170006F7 RID: 1783
	// (get) Token: 0x06004B0E RID: 19214
	Quaternion Rotation { get; }

	// Token: 0x170006F8 RID: 1784
	// (get) Token: 0x06004B0F RID: 19215
	AudioSource SoundSource { get; }

	// Token: 0x170006F9 RID: 1785
	// (get) Token: 0x06004B10 RID: 19216
	AudioClip Sound { get; }

	// Token: 0x170006FA RID: 1786
	// (get) Token: 0x06004B11 RID: 19217
	float Volume { get; }

	// Token: 0x170006FB RID: 1787
	// (get) Token: 0x06004B12 RID: 19218
	float Pitch { get; }

	// Token: 0x06004B13 RID: 19219
	void OnTriggerActions();

	// Token: 0x06004B14 RID: 19220
	void OnPlayVisualFX(int effectID, GameObject effect);

	// Token: 0x06004B15 RID: 19221
	void OnPlaySoundFX(AudioSource audioSource);
}
