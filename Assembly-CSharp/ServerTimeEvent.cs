using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000CB1 RID: 3249
public class ServerTimeEvent : TimeEvent
{
	// Token: 0x06004F5F RID: 20319 RVA: 0x00199249 File Offset: 0x00197449
	private void Awake()
	{
		this.eventTimes = new HashSet<ServerTimeEvent.EventTime>(this.times);
	}

	// Token: 0x06004F60 RID: 20320 RVA: 0x0019925C File Offset: 0x0019745C
	private void Update()
	{
		if (GorillaComputer.instance == null || Time.time - this.lastQueryTime < this.queryTime)
		{
			return;
		}
		ServerTimeEvent.EventTime eventTime = new ServerTimeEvent.EventTime(GorillaComputer.instance.GetServerTime().Hour, GorillaComputer.instance.GetServerTime().Minute);
		bool flag = this.eventTimes.Contains(eventTime);
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
		this.lastQueryTime = Time.time;
	}

	// Token: 0x04005DE3 RID: 24035
	[SerializeField]
	private ServerTimeEvent.EventTime[] times;

	// Token: 0x04005DE4 RID: 24036
	[SerializeField]
	private float queryTime = 60f;

	// Token: 0x04005DE5 RID: 24037
	private float lastQueryTime;

	// Token: 0x04005DE6 RID: 24038
	private HashSet<ServerTimeEvent.EventTime> eventTimes;

	// Token: 0x02000CB2 RID: 3250
	[Serializable]
	public struct EventTime
	{
		// Token: 0x06004F62 RID: 20322 RVA: 0x0019930B File Offset: 0x0019750B
		public EventTime(int h, int m)
		{
			this.hour = h;
			this.minute = m;
		}

		// Token: 0x04005DE7 RID: 24039
		public int hour;

		// Token: 0x04005DE8 RID: 24040
		public int minute;
	}
}
