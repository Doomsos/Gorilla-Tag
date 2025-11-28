using System;
using UnityEngine;

// Token: 0x02000C08 RID: 3080
internal abstract class TickSystemMono : MonoBehaviour, ITickSystem, ITickSystemPre, ITickSystemTick, ITickSystemPost
{
	// Token: 0x17000720 RID: 1824
	// (get) Token: 0x06004C0A RID: 19466 RVA: 0x0018C8E3 File Offset: 0x0018AAE3
	// (set) Token: 0x06004C0B RID: 19467 RVA: 0x0018C8EB File Offset: 0x0018AAEB
	public bool PreTickRunning { get; set; }

	// Token: 0x17000721 RID: 1825
	// (get) Token: 0x06004C0C RID: 19468 RVA: 0x0018C8F4 File Offset: 0x0018AAF4
	// (set) Token: 0x06004C0D RID: 19469 RVA: 0x0018C8FC File Offset: 0x0018AAFC
	public bool TickRunning { get; set; }

	// Token: 0x17000722 RID: 1826
	// (get) Token: 0x06004C0E RID: 19470 RVA: 0x0018C905 File Offset: 0x0018AB05
	// (set) Token: 0x06004C0F RID: 19471 RVA: 0x0018C90D File Offset: 0x0018AB0D
	public bool PostTickRunning { get; set; }

	// Token: 0x06004C10 RID: 19472 RVA: 0x0018C916 File Offset: 0x0018AB16
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickSystemCallBack(this);
	}

	// Token: 0x06004C11 RID: 19473 RVA: 0x0018C91E File Offset: 0x0018AB1E
	public virtual void OnDisable()
	{
		TickSystem<object>.RemoveTickSystemCallback(this);
	}

	// Token: 0x06004C12 RID: 19474 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void PreTick()
	{
	}

	// Token: 0x06004C13 RID: 19475 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void Tick()
	{
	}

	// Token: 0x06004C14 RID: 19476 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void PostTick()
	{
	}
}
