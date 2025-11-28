using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class PlayableBoundaryTracker : MonoBehaviour
{
	// Token: 0x17000156 RID: 342
	// (get) Token: 0x06000E4E RID: 3662 RVA: 0x0004BF73 File Offset: 0x0004A173
	// (set) Token: 0x06000E4F RID: 3663 RVA: 0x0004BF7B File Offset: 0x0004A17B
	public float signedDistanceToBoundary { get; private set; }

	// Token: 0x17000157 RID: 343
	// (get) Token: 0x06000E50 RID: 3664 RVA: 0x0004BF84 File Offset: 0x0004A184
	// (set) Token: 0x06000E51 RID: 3665 RVA: 0x0004BF8C File Offset: 0x0004A18C
	public float prevSignedDistanceToBoundary { get; private set; }

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000E52 RID: 3666 RVA: 0x0004BF95 File Offset: 0x0004A195
	// (set) Token: 0x06000E53 RID: 3667 RVA: 0x0004BF9D File Offset: 0x0004A19D
	public float timeSinceCrossingBorder { get; private set; }

	// Token: 0x06000E54 RID: 3668 RVA: 0x0004BFA6 File Offset: 0x0004A1A6
	[MethodImpl(256)]
	public bool IsInsideZone()
	{
		return Mathf.Sign(this.signedDistanceToBoundary) < 0f;
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x0004BFBC File Offset: 0x0004A1BC
	public void UpdateSignedDistanceToBoundary(float newDistance, float elapsed)
	{
		this.prevSignedDistanceToBoundary = this.signedDistanceToBoundary;
		this.signedDistanceToBoundary = newDistance;
		if ((int)Mathf.Sign(this.prevSignedDistanceToBoundary) != (int)Mathf.Sign(this.signedDistanceToBoundary))
		{
			this.timeSinceCrossingBorder = 0f;
			return;
		}
		this.timeSinceCrossingBorder += elapsed;
	}

	// Token: 0x06000E56 RID: 3670 RVA: 0x0004C010 File Offset: 0x0004A210
	internal void ResetValues()
	{
		this.timeSinceCrossingBorder = 0f;
	}

	// Token: 0x04001158 RID: 4440
	public float radius = 1f;
}
