using System;
using UnityEngine;

// Token: 0x02000C00 RID: 3072
public abstract class MonoBehaviourTick : MonoBehaviour, ITickSystemTick
{
	// Token: 0x1700071D RID: 1821
	// (get) Token: 0x06004BE1 RID: 19425 RVA: 0x0018C506 File Offset: 0x0018A706
	// (set) Token: 0x06004BE2 RID: 19426 RVA: 0x0018C50E File Offset: 0x0018A70E
	public bool TickRunning { get; set; }

	// Token: 0x06004BE3 RID: 19427 RVA: 0x0001877F File Offset: 0x0001697F
	public void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06004BE4 RID: 19428 RVA: 0x00018787 File Offset: 0x00016987
	public void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06004BE5 RID: 19429
	public abstract void Tick();
}
