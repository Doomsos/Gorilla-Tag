using System;
using UnityEngine;

// Token: 0x02000C01 RID: 3073
public abstract class MonoBehaviourPostTick : MonoBehaviour, ITickSystemPost
{
	// Token: 0x1700071E RID: 1822
	// (get) Token: 0x06004BE7 RID: 19431 RVA: 0x0018C4F7 File Offset: 0x0018A6F7
	// (set) Token: 0x06004BE8 RID: 19432 RVA: 0x0018C4FF File Offset: 0x0018A6FF
	public bool PostTickRunning { get; set; }

	// Token: 0x06004BE9 RID: 19433 RVA: 0x001807F9 File Offset: 0x0017E9F9
	public void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06004BEA RID: 19434 RVA: 0x001338D3 File Offset: 0x00131AD3
	public void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06004BEB RID: 19435
	public abstract void PostTick();
}
