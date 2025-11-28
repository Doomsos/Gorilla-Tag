using System;
using UnityEngine;

// Token: 0x02000CEE RID: 3310
[Serializable]
public class VoiceLoudnessReactorTransformRotationTarget
{
	// Token: 0x17000777 RID: 1911
	// (get) Token: 0x0600506C RID: 20588 RVA: 0x0019E334 File Offset: 0x0019C534
	// (set) Token: 0x0600506D RID: 20589 RVA: 0x0019E33C File Offset: 0x0019C53C
	public Quaternion Initial
	{
		get
		{
			return this.initial;
		}
		set
		{
			this.initial = value;
		}
	}

	// Token: 0x04005FBF RID: 24511
	public Transform transform;

	// Token: 0x04005FC0 RID: 24512
	private Quaternion initial;

	// Token: 0x04005FC1 RID: 24513
	public Quaternion Max = Quaternion.identity;

	// Token: 0x04005FC2 RID: 24514
	public float Scale = 1f;

	// Token: 0x04005FC3 RID: 24515
	public bool UseSmoothedLoudness;
}
