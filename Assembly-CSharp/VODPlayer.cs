using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;
using UnityEngine.Video;

public class VODPlayer : MonoBehaviour, IGorillaSliceableSimple
{
	public void OnEnable()
	{
		VODPlayer.<OnEnable>d__19 <OnEnable>d__;
		<OnEnable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnEnable>d__.<>4__this = this;
		<OnEnable>d__.<>1__state = -1;
		<OnEnable>d__.<>t__builder.Start<VODPlayer.<OnEnable>d__19>(ref <OnEnable>d__);
	}

	private void waitOnServerTime()
	{
		VODPlayer.<waitOnServerTime>d__20 <waitOnServerTime>d__;
		<waitOnServerTime>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<waitOnServerTime>d__.<>4__this = this;
		<waitOnServerTime>d__.<>1__state = -1;
		<waitOnServerTime>d__.<>t__builder.Start<VODPlayer.<waitOnServerTime>d__20>(ref <waitOnServerTime>d__);
	}

	private void VODTarget_AlertEnabled(VODTarget o)
	{
		if (!this.targets.Contains(o))
		{
			this.targets.Add(o);
			o.gameObject.SetActive(this.state != VODPlayer.State.CRASHED);
			if (this.state == VODPlayer.State.RUNNING && this.player.isPlaying)
			{
				o.Renderer.material = this.playBackMaterial;
				return;
			}
			o.Renderer.material = ((o.StandbyOverride == null) ? this.standbyMaterial : o.StandbyOverride);
		}
	}

	private void VODTarget_AlertDisabled(VODTarget o)
	{
		if (this.targets.Contains(o))
		{
			this.targets.Remove(o);
			o.Renderer.material = ((o.StandbyOverride == null) ? this.disconnectedMaterial : o.StandbyOverride);
			if (o.UpNextText != null)
			{
				o.UpNextText.text = string.Empty;
			}
		}
	}

	private void Player_loopPointReached(VideoPlayer source)
	{
		if (!this.playerBusy)
		{
			this.player.Stop();
			this.currentStreamPrio = 0;
			for (int i = 0; i < this.targets.Count; i++)
			{
				this.targets[i].Renderer.material = ((this.targets[i].StandbyOverride == null) ? this.standbyMaterial : this.targets[i].StandbyOverride);
			}
			this.nextStream = this.NextStream();
		}
	}

	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		this.player.loopPointReached -= new VideoPlayer.EventHandler(this.Player_loopPointReached);
		VODTarget.AlertEnabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertEnabled, new Action<VODTarget>(this.VODTarget_AlertEnabled));
		VODTarget.AlertDisabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertDisabled, new Action<VODTarget>(this.VODTarget_AlertDisabled));
	}

	private void OnDestroy()
	{
		VODTarget.AlertEnabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertEnabled, new Action<VODTarget>(this.VODTarget_AlertEnabled));
		VODTarget.AlertDisabled = (Action<VODTarget>)Delegate.Remove(VODTarget.AlertDisabled, new Action<VODTarget>(this.VODTarget_AlertDisabled));
	}

	void IGorillaSliceableSimple.SliceUpdate()
	{
		switch (this.state)
		{
		case VODPlayer.State.INITIALIZING:
		case VODPlayer.State.CRASHED:
			return;
		case VODPlayer.State.IDLE:
			if (this.targets.Count > 0)
			{
				this.nextStream = this.NextStream();
				this.PlayPreviouStream();
				this.state = VODPlayer.State.RUNNING;
			}
			return;
		case VODPlayer.State.RUNNING:
		{
			if (this.targets.Count == 0)
			{
				if (!this.playerBusy)
				{
					this.player.Stop();
				}
				this.state = VODPlayer.State.IDLE;
				return;
			}
			if (this.player.isPlaying)
			{
				this.PositionAudio();
			}
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			int dayOfWeek = serverTime.DayOfWeek;
			int hour = serverTime.Hour;
			int minute = serverTime.Minute;
			if (this.nextStream != null && !this.playerBusy && !this.player.isPlaying && this.nextStream.Title != string.Empty)
			{
				TimeSpan timeSpan = this.nextStream.StartTime - serverTime;
				if (timeSpan.TotalSeconds > 0.0 && timeSpan.TotalSeconds <= 3600.0)
				{
					for (int i = 0; i < this.targets.Count; i++)
					{
						if (this.targets[i].UpNextText != null)
						{
							this.targets[i].UpNextText.text = string.Format("next: {0} - {1:00}:{2:00}", this.nextStream.Title, timeSpan.TotalSeconds / 60.0, timeSpan.TotalSeconds % 60.0);
						}
					}
				}
			}
			if (minute == this.lastCheck)
			{
				return;
			}
			this.lastCheck = minute;
			for (int j = 0; j < this.schedule.weekly.Length; j++)
			{
				if (1440 * (this.schedule.weekly[j].day - dayOfWeek) + 60 * (this.schedule.weekly[j].hour - hour) + (this.schedule.weekly[j].minute - minute) == 0)
				{
					this.StartPlayback(this.schedule.weekly[j].stream.url, 3, 0.0);
					break;
				}
			}
			for (int k = 0; k < this.schedule.daily.Length; k++)
			{
				if (60 * (this.schedule.daily[k].hour - hour) + (this.schedule.daily[k].minute - minute) == 0)
				{
					this.StartPlayback(this.schedule.daily[k].stream.url, 2, 0.0);
					break;
				}
			}
			for (int l = 0; l < this.schedule.hourly.Length; l++)
			{
				if (this.schedule.hourly[l].minute - minute == 0)
				{
					this.StartPlayback(this.schedule.hourly[l].stream.url, 1, 0.0);
					return;
				}
			}
			return;
		}
		default:
			return;
		}
	}

	private VODPlayer.VODNextStream NextStream()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		List<VODPlayer.VODNextStream> list = new List<VODPlayer.VODNextStream>();
		for (int i = 0; i < this.schedule.weekly.Length; i++)
		{
			if (i == 0)
			{
				list.Add(new VODPlayer.VODNextStream(3, this.schedule.weekly[i].stream.name, new DateTime(serverTime.Year, serverTime.Month, 7 + serverTime.Day + (this.schedule.weekly[i].day - serverTime.DayOfWeek), this.schedule.weekly[i].hour, this.schedule.weekly[i].minute, 0)));
			}
			list.Add(new VODPlayer.VODNextStream(3, this.schedule.weekly[i].stream.name, new DateTime(serverTime.Year, serverTime.Month, serverTime.Day + (this.schedule.weekly[i].day - serverTime.DayOfWeek), this.schedule.weekly[i].hour, this.schedule.weekly[i].minute, 0)));
		}
		for (int j = 0; j < this.schedule.daily.Length; j++)
		{
			if (j == 0)
			{
				list.Add(new VODPlayer.VODNextStream(2, this.schedule.daily[j].stream.name, new DateTime(serverTime.Year, serverTime.Month, serverTime.Day + 1, this.schedule.daily[j].hour, this.schedule.daily[j].minute, 0)));
			}
			list.Add(new VODPlayer.VODNextStream(2, this.schedule.daily[j].stream.name, new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, this.schedule.daily[j].hour, this.schedule.daily[j].minute, 0)));
		}
		for (int k = 0; k < this.schedule.hourly.Length; k++)
		{
			if (k == 0)
			{
				list.Add(new VODPlayer.VODNextStream(2, this.schedule.hourly[k].stream.name, new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour + 1, this.schedule.hourly[k].minute, 0)));
			}
			list.Add(new VODPlayer.VODNextStream(2, this.schedule.hourly[k].stream.name, new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, this.schedule.hourly[k].minute, 0)));
		}
		list.Sort();
		for (int l = 0; l < list.Count; l++)
		{
			if (list[l].StartTime > serverTime)
			{
				return list[l];
			}
		}
		return null;
	}

	private void PositionAudio()
	{
		float num = float.MaxValue;
		VODTarget vodtarget = null;
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].AudioSettings.volume > 0f)
			{
				float sqrMagnitude = (VRRig.LocalRig.transform.position - this.targets[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					vodtarget = this.targets[i];
					num = sqrMagnitude;
				}
			}
		}
		if (vodtarget == null)
		{
			return;
		}
		this.audioSource.transform.parent = vodtarget.transform;
		this.audioSource.transform.localPosition = Vector3.zero;
		this.audioSource.volume = vodtarget.AudioSettings.volume;
		this.audioSource.dopplerLevel = vodtarget.AudioSettings.dopplerLevel;
		this.audioSource.spread = vodtarget.AudioSettings.spread;
		this.audioSource.rolloffMode = vodtarget.AudioSettings.rolloffMode;
		this.audioSource.minDistance = vodtarget.AudioSettings.minDistance;
		this.audioSource.maxDistance = vodtarget.AudioSettings.maxDistance;
	}

	private void PlayPreviouStream()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		int dayOfWeek = serverTime.DayOfWeek;
		int hour = serverTime.Hour;
		int minute = serverTime.Minute;
		DateTime dateTime;
		dateTime..ctor(serverTime.Year, serverTime.Month, serverTime.Day, hour, minute, 0);
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		for (int i = 0; i < this.schedule.weekly.Length; i++)
		{
			if (this.schedule.weekly[i].day <= dayOfWeek && this.schedule.weekly[i].hour <= hour && this.schedule.weekly[i].minute <= minute)
			{
				num = i;
			}
		}
		for (int j = 0; j < this.schedule.daily.Length; j++)
		{
			if (this.schedule.daily[j].hour <= hour && this.schedule.daily[j].minute <= minute)
			{
				num2 = j;
			}
		}
		for (int k = 0; k < this.schedule.hourly.Length; k++)
		{
			if (this.schedule.hourly[k].minute <= minute)
			{
				num3 = k;
			}
		}
		int num4 = int.MaxValue;
		int num5 = int.MaxValue;
		int num6 = int.MaxValue;
		if (num >= 0)
		{
			int num7 = 1440 * (dayOfWeek - this.schedule.weekly[num].day) + 60 * (hour - this.schedule.weekly[num].hour) + (minute - this.schedule.weekly[num].minute);
			if (num7 < num4)
			{
				num4 = num7;
			}
		}
		else if (this.schedule.weekly.Length != 0)
		{
			num = this.schedule.weekly.Length;
			int num8 = 10080 - (1440 * (dayOfWeek - this.schedule.weekly[num].day) + 60 * (hour - this.schedule.weekly[num].hour) + (minute - this.schedule.weekly[num].minute));
			if (num8 < num4)
			{
				num4 = num8;
			}
		}
		if (num2 >= 0)
		{
			int num9 = 60 * (hour - this.schedule.daily[num2].hour) + (minute - this.schedule.daily[num2].minute);
			if (num9 < num5)
			{
				num5 = num9;
			}
		}
		else if (this.schedule.daily.Length != 0)
		{
			num2 = this.schedule.daily.Length - 1;
			int num10 = 1440 - (60 * (hour - this.schedule.daily[num2].hour) + (minute - this.schedule.daily[num2].minute));
			if (num10 < num5)
			{
				num5 = num10;
			}
		}
		if (num3 >= 0)
		{
			int num11 = minute - this.schedule.hourly[num3].minute;
			if (num11 < num6)
			{
				num6 = num11;
			}
		}
		else if (this.schedule.daily.Length != 0)
		{
			num3 = this.schedule.hourly.Length - 1;
			int num12 = 60 - (minute - this.schedule.hourly[num3].minute);
			if (num12 < num6)
			{
				num6 = num12;
			}
		}
		if (num3 >= 0 && num6 < num5 && num6 < num4)
		{
			this.StartPlayback(this.schedule.hourly[num3].stream.url, 1, serverTime.Subtract(dateTime.AddMinutes((double)(-(double)num6))).TotalSeconds);
			return;
		}
		if (num2 >= 0 && num5 < num4)
		{
			this.StartPlayback(this.schedule.hourly[num2].stream.url, 2, serverTime.Subtract(dateTime.AddMinutes((double)(-(double)num5))).TotalSeconds);
			return;
		}
		if (num >= 0)
		{
			this.StartPlayback(this.schedule.hourly[num].stream.url, 3, serverTime.Subtract(dateTime.AddMinutes((double)(-(double)num4))).TotalSeconds);
			return;
		}
	}

	private void StartPlayback(string url, int priority, double time = 0.0)
	{
		VODPlayer.<StartPlayback>d__32 <StartPlayback>d__;
		<StartPlayback>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartPlayback>d__.<>4__this = this;
		<StartPlayback>d__.url = url;
		<StartPlayback>d__.priority = priority;
		<StartPlayback>d__.time = time;
		<StartPlayback>d__.<>1__state = -1;
		<StartPlayback>d__.<>t__builder.Start<VODPlayer.<StartPlayback>d__32>(ref <StartPlayback>d__);
	}

	private void onTD(string s)
	{
		if (s.IsNullOrEmpty())
		{
			this.Crash("No schedule data");
			return;
		}
		try
		{
			this.schedule = JsonConvert.DeserializeObject<VODPlayer.VODStreamSchedule>(s);
		}
		catch (Exception)
		{
			this.Crash("Malformed schedule data");
			return;
		}
		if (this.schedule.weekly.Length + this.schedule.daily.Length + this.schedule.hourly.Length == 0)
		{
			this.Crash("Nothing scheduled in title data");
			return;
		}
		this.waitOnServerTime();
	}

	private void Crash(string msg)
	{
		this.state = VODPlayer.State.CRASHED;
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].gameObject.SetActive(false);
		}
	}

	private void onTDError(PlayFabError error)
	{
		this.Crash(error.ErrorMessage);
	}

	private VideoPlayer player;

	private AudioSource audioSource;

	private VODPlayer.VODNextStream nextStream;

	[SerializeField]
	private VODPlayer.VODStreamSchedule schedule;

	[SerializeField]
	private string titleDataKey;

	[SerializeField]
	private Material standbyMaterial;

	[SerializeField]
	private Material playBackMaterial;

	[SerializeField]
	private Material disconnectedMaterial;

	[SerializeField]
	private Material busyMaterial;

	private List<VODTarget> targets = new List<VODTarget>();

	private int lastCheck;

	private VODPlayer.State state;

	private bool playerBusy;

	private int currentStreamPrio;

	private enum State
	{
		INITIALIZING,
		IDLE,
		RUNNING,
		CRASHED
	}

	[Serializable]
	public class VODNextStream : IComparable<VODPlayer.VODNextStream>
	{
		public VODNextStream(int prio, string name, DateTime startTime)
		{
			this.Prio = prio;
			this.Title = name;
			this.StartTime = startTime;
		}

		int IComparable<VODPlayer.VODNextStream>.CompareTo(VODPlayer.VODNextStream other)
		{
			return (int)(this.StartTime - other.StartTime).TotalSeconds - (this.Prio - other.Prio);
		}

		public int Prio;

		public string Title;

		public DateTime StartTime;
	}

	[Serializable]
	public struct VODStreamSchedule
	{
		public VODPlayer.VODWeeklyStream[] weekly;

		public VODPlayer.VODDailyStream[] daily;

		public VODPlayer.VODHourlyStream[] hourly;
	}

	[Serializable]
	public struct VODStream
	{
		public string name;

		public string url;
	}

	[Serializable]
	public struct VODWeeklyStream : IComparable<VODPlayer.VODWeeklyStream>
	{
		public int CompareTo(VODPlayer.VODWeeklyStream other)
		{
			return this.day + this.hour + this.minute - (other.day + other.hour + other.minute);
		}

		public VODPlayer.VODStream stream;

		[Range(0f, 6f)]
		public int day;

		[Range(0f, 23f)]
		public int hour;

		[Range(0f, 59f)]
		public int minute;
	}

	[Serializable]
	public struct VODDailyStream : IComparable<VODPlayer.VODDailyStream>
	{
		public int CompareTo(VODPlayer.VODDailyStream other)
		{
			return this.hour + this.minute - (other.hour + other.minute);
		}

		public VODPlayer.VODStream stream;

		[Range(0f, 23f)]
		public int hour;

		[Range(0f, 59f)]
		public int minute;
	}

	[Serializable]
	public struct VODHourlyStream : IComparable<VODPlayer.VODHourlyStream>
	{
		public int CompareTo(VODPlayer.VODHourlyStream other)
		{
			return this.minute - other.minute;
		}

		public VODPlayer.VODStream stream;

		[Range(0f, 59f)]
		public int minute;
	}
}
