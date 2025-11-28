using System;
using UnityEngine;

// Token: 0x02000C0A RID: 3082
internal abstract class TickSystemTickMono : MonoBehaviour, ITickSystemTick
{
	// Token: 0x17000724 RID: 1828
	// (get) Token: 0x06004C1C RID: 19484 RVA: 0x0018C947 File Offset: 0x0018AB47
	// (set) Token: 0x06004C1D RID: 19485 RVA: 0x0018C94F File Offset: 0x0018AB4F
	public bool TickRunning { get; set; }

	// Token: 0x06004C1E RID: 19486 RVA: 0x0001877F File Offset: 0x0001697F
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06004C1F RID: 19487 RVA: 0x00018787 File Offset: 0x00016987
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06004C20 RID: 19488 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void Tick()
	{
	}
}
