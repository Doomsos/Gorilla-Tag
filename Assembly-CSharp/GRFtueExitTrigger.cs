using System;
using UnityEngine;

// Token: 0x020006C8 RID: 1736
public class GRFtueExitTrigger : GorillaTriggerBox
{
	// Token: 0x06002C99 RID: 11417 RVA: 0x000F1AA9 File Offset: 0x000EFCA9
	public override void OnBoxTriggered()
	{
		this.startTime = Time.time;
		this.ftueObject.InterruptWaitingTimer();
		this.ftueObject.playerLight.GetComponentInChildren<Light>().intensity = 0.25f;
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x000F1ADB File Offset: 0x000EFCDB
	private void Update()
	{
		if (this.startTime > 0f && Time.time - this.startTime > this.delayTime)
		{
			this.ftueObject.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
			this.startTime = -1f;
		}
	}

	// Token: 0x040039EA RID: 14826
	public GRFirstTimeUserExperience ftueObject;

	// Token: 0x040039EB RID: 14827
	public float delayTime = 5f;

	// Token: 0x040039EC RID: 14828
	private float startTime = -1f;
}
