using System;
using UnityEngine;

// Token: 0x02000CED RID: 3309
[Serializable]
public class VoiceLoudnessReactorTransformTarget
{
	// Token: 0x17000776 RID: 1910
	// (get) Token: 0x06005069 RID: 20585 RVA: 0x0019E2E5 File Offset: 0x0019C4E5
	// (set) Token: 0x0600506A RID: 20586 RVA: 0x0019E2ED File Offset: 0x0019C4ED
	public Vector3 Initial
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

	// Token: 0x04005FBA RID: 24506
	public Transform transform;

	// Token: 0x04005FBB RID: 24507
	private Vector3 initial;

	// Token: 0x04005FBC RID: 24508
	public Vector3 Max = Vector3.one;

	// Token: 0x04005FBD RID: 24509
	public float Scale = 1f;

	// Token: 0x04005FBE RID: 24510
	public bool UseSmoothedLoudness;
}
