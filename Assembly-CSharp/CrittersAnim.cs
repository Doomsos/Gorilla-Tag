using System;
using UnityEngine;

// Token: 0x02000049 RID: 73
[Serializable]
public class CrittersAnim
{
	// Token: 0x0600016B RID: 363 RVA: 0x00009638 File Offset: 0x00007838
	public bool IsModified()
	{
		return (this.squashAmount != null && this.squashAmount.length > 1) || (this.forwardOffset != null && this.forwardOffset.length > 1) || (this.horizontalOffset != null && this.horizontalOffset.length > 1) || (this.verticalOffset != null && this.verticalOffset.length > 1);
	}

	// Token: 0x0600016C RID: 364 RVA: 0x000096A1 File Offset: 0x000078A1
	public static bool IsModified(CrittersAnim anim)
	{
		return anim != null && anim.IsModified();
	}

	// Token: 0x0400018F RID: 399
	public AnimationCurve squashAmount;

	// Token: 0x04000190 RID: 400
	public AnimationCurve forwardOffset;

	// Token: 0x04000191 RID: 401
	public AnimationCurve horizontalOffset;

	// Token: 0x04000192 RID: 402
	public AnimationCurve verticalOffset;

	// Token: 0x04000193 RID: 403
	public float playSpeed;
}
