using System;
using Photon.Realtime;

namespace GorillaTag
{
	// Token: 0x02001009 RID: 4105
	internal class ReportMuteTimer : TickSystemTimerAbstract, ObjectPoolEvents
	{
		// Token: 0x170009C2 RID: 2498
		// (get) Token: 0x060067E4 RID: 26596 RVA: 0x0021EEE5 File Offset: 0x0021D0E5
		// (set) Token: 0x060067E5 RID: 26597 RVA: 0x0021EEED File Offset: 0x0021D0ED
		public int Muted { get; set; }

		// Token: 0x060067E6 RID: 26598 RVA: 0x0021EEF8 File Offset: 0x0021D0F8
		public override void OnTimedEvent()
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				this.Stop();
				return;
			}
			ReportMuteTimer.content[0] = this.m_playerID;
			ReportMuteTimer.content[1] = this.Muted;
			ReportMuteTimer.content[2] = ((this.m_nickName.Length > 12) ? this.m_nickName.Remove(12) : this.m_nickName);
			ReportMuteTimer.content[3] = NetworkSystem.Instance.LocalPlayer.NickName;
			ReportMuteTimer.content[4] = !NetworkSystem.Instance.SessionIsPrivate;
			ReportMuteTimer.content[5] = NetworkSystem.Instance.RoomStringStripped();
			NetworkSystemRaiseEvent.RaiseEvent(51, ReportMuteTimer.content, ReportMuteTimer.netEventOptions, true);
			this.Stop();
		}

		// Token: 0x060067E7 RID: 26599 RVA: 0x0021EFBA File Offset: 0x0021D1BA
		public void SetReportData(string id, string name, int muted)
		{
			this.Muted = muted;
			this.m_playerID = id;
			this.m_nickName = name;
		}

		// Token: 0x060067E8 RID: 26600 RVA: 0x00002789 File Offset: 0x00000989
		void ObjectPoolEvents.OnTaken()
		{
		}

		// Token: 0x060067E9 RID: 26601 RVA: 0x0021EFD1 File Offset: 0x0021D1D1
		void ObjectPoolEvents.OnReturned()
		{
			if (base.Running)
			{
				this.OnTimedEvent();
			}
			this.m_playerID = string.Empty;
			this.m_nickName = string.Empty;
			this.Muted = 0;
		}

		// Token: 0x040076DC RID: 30428
		private static readonly NetEventOptions netEventOptions = new NetEventOptions
		{
			Flags = new WebFlags(3),
			TargetActors = new int[]
			{
				-1
			}
		};

		// Token: 0x040076DD RID: 30429
		private static readonly object[] content = new object[6];

		// Token: 0x040076DE RID: 30430
		private const byte evCode = 51;

		// Token: 0x040076E0 RID: 30432
		private string m_playerID;

		// Token: 0x040076E1 RID: 30433
		private string m_nickName;
	}
}
