using System;
using UnityEngine;

// Token: 0x02000C08 RID: 3080
internal abstract class TickSystemMono : MonoBehaviour, ITickSystem, ITickSystemPre, ITickSystemTick, ITickSystemPost
{
	// Token: 0x17000720 RID: 1824
	// (get) Token: 0x06004C0A RID: 19466 RVA: 0x0018C903 File Offset: 0x0018AB03
	// (set) Token: 0x06004C0B RID: 19467 RVA: 0x0018C90B File Offset: 0x0018AB0B
	public bool PreTickRunning { get; set; }

	// Token: 0x17000721 RID: 1825
	// (get) Token: 0x06004C0C RID: 19468 RVA: 0x0018C914 File Offset: 0x0018AB14
	// (set) Token: 0x06004C0D RID: 19469 RVA: 0x0018C91C File Offset: 0x0018AB1C
	public bool TickRunning { get; set; }

	// Token: 0x17000722 RID: 1826
	// (get) Token: 0x06004C0E RID: 19470 RVA: 0x0018C925 File Offset: 0x0018AB25
	// (set) Token: 0x06004C0F RID: 19471 RVA: 0x0018C92D File Offset: 0x0018AB2D
	public bool PostTickRunning { get; set; }

	// Token: 0x06004C10 RID: 19472 RVA: 0x0018C936 File Offset: 0x0018AB36
	public virtual void OnEnable()
	{
		TickSystem<object>.AddTickSystemCallBack(this);
	}

	// Token: 0x06004C11 RID: 19473 RVA: 0x0018C93E File Offset: 0x0018AB3E
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
