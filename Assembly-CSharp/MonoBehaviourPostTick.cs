using System;
using UnityEngine;

// Token: 0x02000C01 RID: 3073
public abstract class MonoBehaviourPostTick : MonoBehaviour, ITickSystemPost
{
	// Token: 0x1700071E RID: 1822
	// (get) Token: 0x06004BE7 RID: 19431 RVA: 0x0018C517 File Offset: 0x0018A717
	// (set) Token: 0x06004BE8 RID: 19432 RVA: 0x0018C51F File Offset: 0x0018A71F
	public bool PostTickRunning { get; set; }

	// Token: 0x06004BE9 RID: 19433 RVA: 0x00180819 File Offset: 0x0017EA19
	public void OnEnable()
	{
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06004BEA RID: 19434 RVA: 0x001338F3 File Offset: 0x00131AF3
	public void OnDisable()
	{
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06004BEB RID: 19435
	public abstract void PostTick();
}
