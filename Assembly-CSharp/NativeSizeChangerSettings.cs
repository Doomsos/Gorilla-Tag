using System;
using UnityEngine;

// Token: 0x02000398 RID: 920
[Serializable]
public class NativeSizeChangerSettings
{
	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06001601 RID: 5633 RVA: 0x0007AB71 File Offset: 0x00078D71
	// (set) Token: 0x06001602 RID: 5634 RVA: 0x0007AB79 File Offset: 0x00078D79
	public Vector3 WorldPosition
	{
		get
		{
			return this.worldPosition;
		}
		set
		{
			this.worldPosition = value;
		}
	}

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x06001603 RID: 5635 RVA: 0x0007AB82 File Offset: 0x00078D82
	// (set) Token: 0x06001604 RID: 5636 RVA: 0x0007AB8A File Offset: 0x00078D8A
	public float ActivationTime
	{
		get
		{
			return this.activationTime;
		}
		set
		{
			this.activationTime = value;
		}
	}

	// Token: 0x04002042 RID: 8258
	public const float MinAllowedSize = 0.1f;

	// Token: 0x04002043 RID: 8259
	public const float MaxAllowedSize = 10f;

	// Token: 0x04002044 RID: 8260
	private Vector3 worldPosition;

	// Token: 0x04002045 RID: 8261
	private float activationTime;

	// Token: 0x04002046 RID: 8262
	[Range(0.1f, 10f)]
	public float playerSizeScale = 1f;

	// Token: 0x04002047 RID: 8263
	public bool ExpireOnRoomJoin = true;

	// Token: 0x04002048 RID: 8264
	public bool ExpireInWater = true;

	// Token: 0x04002049 RID: 8265
	public float ExpireAfterSeconds;

	// Token: 0x0400204A RID: 8266
	public float ExpireOnDistance;
}
