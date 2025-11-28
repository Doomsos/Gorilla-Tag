using System;
using UnityEngine;

// Token: 0x02000CB4 RID: 3252
public class TimeOfDayEvent : TimeEvent
{
	// Token: 0x17000761 RID: 1889
	// (get) Token: 0x06004F66 RID: 20326 RVA: 0x0019932D File Offset: 0x0019752D
	public float currentTime
	{
		get
		{
			return this._currentTime;
		}
	}

	// Token: 0x17000762 RID: 1890
	// (get) Token: 0x06004F67 RID: 20327 RVA: 0x00199335 File Offset: 0x00197535
	// (set) Token: 0x06004F68 RID: 20328 RVA: 0x0019933D File Offset: 0x0019753D
	public float timeStart
	{
		get
		{
			return this._timeStart;
		}
		set
		{
			this._timeStart = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000763 RID: 1891
	// (get) Token: 0x06004F69 RID: 20329 RVA: 0x0019934B File Offset: 0x0019754B
	// (set) Token: 0x06004F6A RID: 20330 RVA: 0x00199353 File Offset: 0x00197553
	public float timeEnd
	{
		get
		{
			return this._timeEnd;
		}
		set
		{
			this._timeEnd = Mathf.Clamp01(value);
		}
	}

	// Token: 0x17000764 RID: 1892
	// (get) Token: 0x06004F6B RID: 20331 RVA: 0x00199361 File Offset: 0x00197561
	public bool isOngoing
	{
		get
		{
			return this._ongoing;
		}
	}

	// Token: 0x06004F6C RID: 20332 RVA: 0x0019936C File Offset: 0x0019756C
	private void Start()
	{
		if (!this._dayNightManager)
		{
			this._dayNightManager = BetterDayNightManager.instance;
		}
		if (!this._dayNightManager)
		{
			return;
		}
		for (int i = 0; i < this._dayNightManager.timeOfDayRange.Length; i++)
		{
			this._totalSecondsInRange += this._dayNightManager.timeOfDayRange[i] * 3600.0;
		}
		this._totalSecondsInRange = Math.Floor(this._totalSecondsInRange);
	}

	// Token: 0x06004F6D RID: 20333 RVA: 0x001993EE File Offset: 0x001975EE
	private void Update()
	{
		this._elapsed += Time.deltaTime;
		if (this._elapsed < 1f)
		{
			return;
		}
		this._elapsed = 0f;
		this.UpdateTime();
	}

	// Token: 0x06004F6E RID: 20334 RVA: 0x00199424 File Offset: 0x00197624
	private void UpdateTime()
	{
		this._currentSeconds = ((ITimeOfDaySystem)this._dayNightManager).currentTimeInSeconds;
		this._currentSeconds = Math.Floor(this._currentSeconds);
		this._currentTime = (float)(this._currentSeconds / this._totalSecondsInRange);
		bool flag = this._currentTime >= 0f && this._currentTime >= this._timeStart && this._currentTime <= this._timeEnd;
		if (!this._ongoing && flag)
		{
			base.StartEvent();
		}
		if (this._ongoing && !flag)
		{
			base.StopEvent();
		}
	}

	// Token: 0x06004F6F RID: 20335 RVA: 0x001994BB File Offset: 0x001976BB
	public static implicit operator bool(TimeOfDayEvent ev)
	{
		return ev && ev.isOngoing;
	}

	// Token: 0x04005DEC RID: 24044
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeStart;

	// Token: 0x04005DED RID: 24045
	[SerializeField]
	[Range(0f, 1f)]
	private float _timeEnd = 1f;

	// Token: 0x04005DEE RID: 24046
	[SerializeField]
	private float _currentTime = -1f;

	// Token: 0x04005DEF RID: 24047
	[Space]
	[SerializeField]
	private double _currentSeconds = -1.0;

	// Token: 0x04005DF0 RID: 24048
	[SerializeField]
	private double _totalSecondsInRange = -1.0;

	// Token: 0x04005DF1 RID: 24049
	[NonSerialized]
	private float _elapsed = -1f;

	// Token: 0x04005DF2 RID: 24050
	[SerializeField]
	private BetterDayNightManager _dayNightManager;
}
