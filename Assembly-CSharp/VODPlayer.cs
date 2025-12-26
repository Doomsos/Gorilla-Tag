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
		VODPlayer.<OnEnable>d__18 <OnEnable>d__;
		<OnEnable>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnEnable>d__.<>4__this = this;
		<OnEnable>d__.<>1__state = -1;
		<OnEnable>d__.<>t__builder.Start<VODPlayer.<OnEnable>d__18>(ref <OnEnable>d__);
	}

	private void waitOnServerTime()
	{
		VODPlayer.<waitOnServerTime>d__19 <waitOnServerTime>d__;
		<waitOnServerTime>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<waitOnServerTime>d__.<>1__state = -1;
		<waitOnServerTime>d__.<>t__builder.Start<VODPlayer.<waitOnServerTime>d__19>(ref <waitOnServerTime>d__);
	}

	private void VODTarget_AlertEnabled(VODTarget o)
	{
		if (!this.targets.Contains(o))
		{
			this.targets.Add(o);
			o.gameObject.SetActive(VODPlayer.state != VODPlayer.State.CRASHED);
			if (VODPlayer.state == VODPlayer.State.RUNNING && this.player.isPlaying)
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
		this.player.loopPointReached -= this.Player_loopPointReached;
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
		switch (VODPlayer.state)
		{
		case VODPlayer.State.INITIALIZING:
		case VODPlayer.State.CRASHED:
			return;
		case VODPlayer.State.IDLE:
			if (this.targets.Count > 0)
			{
				this.nextStream = this.NextStream();
				this.PlayPreviouStream();
				VODPlayer.state = VODPlayer.State.RUNNING;
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
				VODPlayer.state = VODPlayer.State.IDLE;
				return;
			}
			if (this.player.isPlaying)
			{
				this.PositionAudio();
			}
			DateTime serverTime = GorillaComputer.instance.GetServerTime();
			DayOfWeek dayOfWeek = serverTime.DayOfWeek;
			int hour = serverTime.Hour;
			int minute = serverTime.Minute;
			if (this.nextStream != null && !this.playerBusy && !this.player.isPlaying && this.nextStream.Title != string.Empty)
			{
				TimeSpan timeSpan = this.nextStream.StartTime - serverTime;
				if (timeSpan.TotalMinutes > 0.0 && timeSpan.TotalMinutes <= 60.0)
				{
					for (int i = 0; i < this.targets.Count; i++)
					{
						if (this.targets[i].UpNextText != null)
						{
							this.targets[i].UpNextText.text = string.Format("next: {0} - {1:00}:{2:00}", this.nextStream.Title, timeSpan.Minutes, timeSpan.Seconds);
						}
					}
				}
			}
			if (minute == this.lastCheck)
			{
				return;
			}
			this.lastCheck = minute;
			for (int j = 0; j < this.schedule.hourly.Length; j++)
			{
				if (this.schedule.hourly[j].minute - minute == 0 && this.schedule.hourly[j].IsDateInRange(serverTime))
				{
					this.StartPlayback(this.schedule.hourly[j].stream.url, 1.0);
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
		for (int i = 0; i < this.schedule.hourly.Length; i++)
		{
			if (i == 0)
			{
				list.Add(new VODPlayer.VODNextStream(2, this.schedule.hourly[i].stream.name, new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour + 1, this.schedule.hourly[i].minute, 0)));
			}
			list.Add(new VODPlayer.VODNextStream(2, this.schedule.hourly[i].stream.name, new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, this.schedule.hourly[i].minute, 0)));
		}
		list.Sort();
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].StartTime > serverTime)
			{
				return list[j];
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
		this.audioSource.rolloffMode = vodtarget.AudioSettings.rolloffMode;
		this.audioSource.minDistance = vodtarget.AudioSettings.minDistance;
		this.audioSource.maxDistance = vodtarget.AudioSettings.maxDistance;
	}

	private void PlayPreviouStream()
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		int hour = serverTime.Hour;
		int minute = serverTime.Minute;
		DateTime dateTime = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, hour, minute, 0);
		int num = -1;
		for (int i = 0; i < this.schedule.hourly.Length; i++)
		{
			if (this.schedule.hourly[i].minute <= minute && this.schedule.hourly[i].IsDateInRange(serverTime))
			{
				num = i;
			}
		}
		if (num >= 0)
		{
			int num2 = minute - this.schedule.hourly[num].minute;
			this.StartPlayback(this.schedule.hourly[num].stream.url, serverTime.Subtract(dateTime.AddMinutes((double)(-(double)num2))).TotalSeconds);
		}
	}

	private void StartPlayback(string url, double time = 0.0)
	{
		VODPlayer.<StartPlayback>d__30 <StartPlayback>d__;
		<StartPlayback>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<StartPlayback>d__.<>4__this = this;
		<StartPlayback>d__.url = url;
		<StartPlayback>d__.time = time;
		<StartPlayback>d__.<>1__state = -1;
		<StartPlayback>d__.<>t__builder.Start<VODPlayer.<StartPlayback>d__30>(ref <StartPlayback>d__);
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
		if (this.schedule.hourly.Length == 0)
		{
			this.Crash("Nothing scheduled in title data");
			return;
		}
		this.waitOnServerTime();
	}

	private void Crash(string msg)
	{
		VODPlayer.state = VODPlayer.State.CRASHED;
		if (VODPlayer.OnCrash != null)
		{
			VODPlayer.OnCrash();
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			this.targets[i].gameObject.SetActive(false);
		}
	}

	private void onTDError(PlayFabError error)
	{
		this.Crash(error.ErrorMessage);
	}

	public static Action OnCrash;

	public static VODPlayer.State state;

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

	private bool playerBusy;

	public enum State
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
		public VODPlayer.VODHourlyStream[] hourly;
	}

	[Serializable]
	public struct VODStream
	{
		public string name;

		public string url;
	}

	[Serializable]
	public struct VODHourlyStream : IComparable<VODPlayer.VODHourlyStream>
	{
		public int CompareTo(VODPlayer.VODHourlyStream other)
		{
			return this.minute - other.minute;
		}

		public void ValidateDate()
		{
			try
			{
				this.startDT = DateTime.Parse(this.startDateTime);
			}
			catch
			{
				this.startDT = DateTime.Parse("1/1/0001");
			}
			try
			{
				this.endDT = DateTime.Parse(this.endDateTime);
			}
			catch
			{
				this.endDT = DateTime.Parse("1/1/3001");
			}
			this.startDateTime = this.startDT.ToString();
			this.endDateTime = this.endDT.ToString();
		}

		internal bool IsDateInRange(DateTime serverTime)
		{
			this.ValidateDate();
			return serverTime >= this.startDT && serverTime <= this.endDT;
		}

		public VODPlayer.VODStream stream;

		[Range(0f, 59f)]
		public int minute;

		public string startDateTime;

		private DateTime startDT;

		public string endDateTime;

		private DateTime endDT;
	}
}
