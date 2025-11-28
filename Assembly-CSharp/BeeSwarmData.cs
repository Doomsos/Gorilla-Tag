using System;
using System.Runtime.InteropServices;
using Fusion;

// Token: 0x020001B1 RID: 433
[NetworkStructWeaved(3)]
[StructLayout(2, Size = 12)]
public struct BeeSwarmData : INetworkStruct
{
	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000B98 RID: 2968 RVA: 0x0003F134 File Offset: 0x0003D334
	// (set) Token: 0x06000B99 RID: 2969 RVA: 0x0003F13C File Offset: 0x0003D33C
	public int TargetActorNumber { readonly get; set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000B9A RID: 2970 RVA: 0x0003F145 File Offset: 0x0003D345
	// (set) Token: 0x06000B9B RID: 2971 RVA: 0x0003F14D File Offset: 0x0003D34D
	public int CurrentState { readonly get; set; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000B9C RID: 2972 RVA: 0x0003F156 File Offset: 0x0003D356
	// (set) Token: 0x06000B9D RID: 2973 RVA: 0x0003F15E File Offset: 0x0003D35E
	public float CurrentSpeed { readonly get; set; }

	// Token: 0x06000B9E RID: 2974 RVA: 0x0003F167 File Offset: 0x0003D367
	public BeeSwarmData(int actorNr, int state, float speed)
	{
		this.TargetActorNumber = actorNr;
		this.CurrentState = state;
		this.CurrentSpeed = speed;
	}
}
