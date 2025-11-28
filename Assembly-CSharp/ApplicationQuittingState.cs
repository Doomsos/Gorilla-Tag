using System;
using UnityEngine;

// Token: 0x0200022E RID: 558
public static class ApplicationQuittingState
{
	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000EE3 RID: 3811 RVA: 0x0004F146 File Offset: 0x0004D346
	// (set) Token: 0x06000EE4 RID: 3812 RVA: 0x0004F14D File Offset: 0x0004D34D
	public static bool IsQuitting { get; private set; }

	// Token: 0x06000EE5 RID: 3813 RVA: 0x0004F155 File Offset: 0x0004D355
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		Application.quitting += new Action(ApplicationQuittingState.HandleApplicationQuitting);
	}

	// Token: 0x06000EE6 RID: 3814 RVA: 0x0004F168 File Offset: 0x0004D368
	private static void HandleApplicationQuitting()
	{
		ApplicationQuittingState.IsQuitting = true;
	}
}
