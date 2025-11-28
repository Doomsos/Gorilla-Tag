using System;
using UnityEngine;

// Token: 0x02000C09 RID: 3081
internal abstract class TickSystemPreTickMono : MonoBehaviour, ITickSystemPre
{
	// Token: 0x17000723 RID: 1827
	// (get) Token: 0x06004C16 RID: 19478 RVA: 0x0018C946 File Offset: 0x0018AB46
	// (set) Token: 0x06004C17 RID: 19479 RVA: 0x0018C94E File Offset: 0x0018AB4E
	public bool PreTickRunning { get; set; }

	// Token: 0x06004C18 RID: 19480 RVA: 0x0018C957 File Offset: 0x0018AB57
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPreTickCallback(this);
	}

	// Token: 0x06004C19 RID: 19481 RVA: 0x0018C95F File Offset: 0x0018AB5F
	public void OnDisable()
	{
		TickSystem<object>.RemovePreTickCallback(this);
	}

	// Token: 0x06004C1A RID: 19482 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void PreTick()
	{
	}
}
