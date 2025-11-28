using System;
using UnityEngine;

// Token: 0x02000C0B RID: 3083
internal abstract class TickSystemPostTickMono : MonoBehaviour, ITickSystemPost
{
	// Token: 0x17000725 RID: 1829
	// (get) Token: 0x06004C22 RID: 19490 RVA: 0x0018C958 File Offset: 0x0018AB58
	// (set) Token: 0x06004C23 RID: 19491 RVA: 0x0018C960 File Offset: 0x0018AB60
	public bool PostTickRunning { get; set; }

	// Token: 0x06004C24 RID: 19492 RVA: 0x001807F9 File Offset: 0x0017E9F9
	public virtual void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06004C25 RID: 19493 RVA: 0x001338D3 File Offset: 0x00131AD3
	public virtual void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06004C26 RID: 19494 RVA: 0x00002789 File Offset: 0x00000989
	public virtual void PostTick()
	{
	}
}
