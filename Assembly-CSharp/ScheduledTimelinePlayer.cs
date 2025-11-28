using System;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x020001A2 RID: 418
public class ScheduledTimelinePlayer : MonoBehaviour
{
	// Token: 0x06000B3D RID: 2877 RVA: 0x0003D197 File Offset: 0x0003B397
	protected void OnEnable()
	{
		this.scheduledEventID = BetterDayNightManager.RegisterScheduledEvent(this.eventHour, new Action(this.HandleScheduledEvent));
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x0003D1B6 File Offset: 0x0003B3B6
	protected void OnDisable()
	{
		BetterDayNightManager.UnregisterScheduledEvent(this.scheduledEventID);
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x0003D1C3 File Offset: 0x0003B3C3
	private void HandleScheduledEvent()
	{
		this.timeline.Play();
	}

	// Token: 0x04000DAE RID: 3502
	public PlayableDirector timeline;

	// Token: 0x04000DAF RID: 3503
	public int eventHour = 7;

	// Token: 0x04000DB0 RID: 3504
	private int scheduledEventID;
}
