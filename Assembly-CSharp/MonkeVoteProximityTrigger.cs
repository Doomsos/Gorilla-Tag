using System;
using UnityEngine;

// Token: 0x020001DA RID: 474
public class MonkeVoteProximityTrigger : GorillaTriggerBox
{
	// Token: 0x14000017 RID: 23
	// (add) Token: 0x06000CF1 RID: 3313 RVA: 0x00045D40 File Offset: 0x00043F40
	// (remove) Token: 0x06000CF2 RID: 3314 RVA: 0x00045D78 File Offset: 0x00043F78
	public event Action OnEnter;

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000CF3 RID: 3315 RVA: 0x00045DAD File Offset: 0x00043FAD
	// (set) Token: 0x06000CF4 RID: 3316 RVA: 0x00045DB5 File Offset: 0x00043FB5
	public bool isPlayerNearby { get; private set; }

	// Token: 0x06000CF5 RID: 3317 RVA: 0x00045DBE File Offset: 0x00043FBE
	public override void OnBoxTriggered()
	{
		this.isPlayerNearby = true;
		if (this.triggerTime + this.retriggerDelay < Time.unscaledTime)
		{
			this.triggerTime = Time.unscaledTime;
			Action onEnter = this.OnEnter;
			if (onEnter == null)
			{
				return;
			}
			onEnter.Invoke();
		}
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x00045DF6 File Offset: 0x00043FF6
	public override void OnBoxExited()
	{
		this.isPlayerNearby = false;
	}

	// Token: 0x04000FEC RID: 4076
	private float triggerTime = float.MinValue;

	// Token: 0x04000FED RID: 4077
	private float retriggerDelay = 0.25f;
}
