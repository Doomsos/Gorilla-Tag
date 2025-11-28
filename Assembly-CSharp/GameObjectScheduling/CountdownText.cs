using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace GameObjectScheduling
{
	// Token: 0x02001150 RID: 4432
	public class CountdownText : MonoBehaviour
	{
		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x06006FD5 RID: 28629 RVA: 0x002467C9 File Offset: 0x002449C9
		private bool ShouldLocalize
		{
			get
			{
				return this.shouldLocalize && (this._locTextComp != null && this._countdownLocStr != null && this._timeCountdownVar != null && this._timescaleCountdownVar != null) && this._isValidVar != null;
			}
		}

		// Token: 0x17000A70 RID: 2672
		// (get) Token: 0x06006FD6 RID: 28630 RVA: 0x00246806 File Offset: 0x00244A06
		// (set) Token: 0x06006FD7 RID: 28631 RVA: 0x00246810 File Offset: 0x00244A10
		public CountdownTextDate Countdown
		{
			get
			{
				return this.CountdownTo;
			}
			set
			{
				this.CountdownTo = value;
				if (this.CountdownTo.FormatString.Length > 0)
				{
					this.displayTextFormat = this.CountdownTo.FormatString;
				}
				this.displayText.text = this.CountdownTo.DefaultString;
				if (base.gameObject.activeInHierarchy && !this.useExternalTime && this.monitor == null && this.CountdownTo != null)
				{
					this.monitor = base.StartCoroutine(this.MonitorTime());
				}
			}
		}

		// Token: 0x06006FD8 RID: 28632 RVA: 0x0024689C File Offset: 0x00244A9C
		private void Awake()
		{
			this.displayText = base.GetComponent<TMP_Text>();
			this.displayTextFormat = string.Empty;
			this.displayText.text = string.Empty;
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.displayTextFormat.Length == 0 && this.CountdownTo.FormatString.Length > 0)
			{
				this.displayTextFormat = this.CountdownTo.FormatString;
			}
			this.displayText.text = this.CountdownTo.DefaultString;
			if (!this.shouldLocalize)
			{
				return;
			}
			this._locTextComp = base.GetComponent<LocalizedText>();
			if (this._locTextComp == null)
			{
				Debug.LogError("[LOCALIZATION::COUNTDOWN_TEXT] There is no [LocalizedText] component on [" + base.name + "]!", this);
				return;
			}
			this._countdownLocStr = this._locTextComp.StringReference;
			if (this._locTextComp.StringReference == null || this._locTextComp.StringReference.IsEmpty)
			{
				Debug.LogError("[LOCALIZATION::COUNTDOWN_TEXT] There is no [StringReference] assigned on [" + base.name + "]!", this);
				return;
			}
			this._timeCountdownVar = (this._countdownLocStr["time-value"] as IntVariable);
			this._timescaleCountdownVar = (this._countdownLocStr["timescale-index"] as IntVariable);
			this._isValidVar = (this._countdownLocStr["is-valid"] as BoolVariable);
		}

		// Token: 0x06006FD9 RID: 28633 RVA: 0x00246A02 File Offset: 0x00244C02
		private void OnEnable()
		{
			if (this.CountdownTo == null)
			{
				return;
			}
			if (this.monitor == null && !this.useExternalTime)
			{
				this.monitor = base.StartCoroutine(this.MonitorTime());
			}
		}

		// Token: 0x06006FDA RID: 28634 RVA: 0x00246A35 File Offset: 0x00244C35
		private void OnDisable()
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
		}

		// Token: 0x06006FDB RID: 28635 RVA: 0x00246A43 File Offset: 0x00244C43
		private IEnumerator MonitorTime()
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = this.TryParseDateTime();
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x06006FDC RID: 28636 RVA: 0x00246A52 File Offset: 0x00244C52
		private IEnumerator MonitorExternalTime(DateTime countdown)
		{
			while (GorillaComputer.instance == null || GorillaComputer.instance.startupMillis == 0L)
			{
				yield return null;
			}
			this.monitor = null;
			this.targetTime = countdown;
			if (this.updateDisplay)
			{
				this.StartDisplayRefresh();
			}
			else
			{
				this.RefreshDisplay();
			}
			yield break;
		}

		// Token: 0x06006FDD RID: 28637 RVA: 0x00246A68 File Offset: 0x00244C68
		private void StopMonitorTime()
		{
			if (this.monitor != null)
			{
				base.StopCoroutine(this.monitor);
			}
			this.monitor = null;
		}

		// Token: 0x06006FDE RID: 28638 RVA: 0x00246A85 File Offset: 0x00244C85
		public void SetCountdownTime(DateTime countdown)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.monitor = base.StartCoroutine(this.MonitorExternalTime(countdown));
		}

		// Token: 0x06006FDF RID: 28639 RVA: 0x00246AA6 File Offset: 0x00244CA6
		public void SetFixedText(string text)
		{
			this.StopMonitorTime();
			this.StopDisplayRefresh();
			this.displayText.text = text;
		}

		// Token: 0x06006FE0 RID: 28640 RVA: 0x00246AC0 File Offset: 0x00244CC0
		private void StartDisplayRefresh()
		{
			this.StopDisplayRefresh();
			this.displayRefresh = base.StartCoroutine(this.WaitForDisplayRefresh());
		}

		// Token: 0x06006FE1 RID: 28641 RVA: 0x00246ADA File Offset: 0x00244CDA
		private void StopDisplayRefresh()
		{
			if (this.displayRefresh != null)
			{
				base.StopCoroutine(this.displayRefresh);
			}
			this.displayRefresh = null;
		}

		// Token: 0x06006FE2 RID: 28642 RVA: 0x00246AF7 File Offset: 0x00244CF7
		private IEnumerator WaitForDisplayRefresh()
		{
			for (;;)
			{
				this.RefreshDisplay();
				TimeSpan timeSpan;
				if (this.countdownTime.Days > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromDays((double)this.countdownTime.Days);
				}
				else if (this.countdownTime.Hours > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromHours((double)this.countdownTime.Hours);
				}
				else if (this.countdownTime.Minutes > 0)
				{
					timeSpan = this.countdownTime - TimeSpan.FromMinutes((double)this.countdownTime.Minutes);
				}
				else
				{
					if (this.countdownTime.Seconds <= 0)
					{
						break;
					}
					timeSpan = this.countdownTime - TimeSpan.FromSeconds((double)this.countdownTime.Seconds);
				}
				yield return new WaitForSeconds((float)timeSpan.TotalSeconds);
			}
			yield break;
		}

		// Token: 0x06006FE3 RID: 28643 RVA: 0x00246B08 File Offset: 0x00244D08
		private void RefreshDisplay()
		{
			this.countdownTime = this.targetTime.Subtract(GorillaComputer.instance.GetServerTime());
			ValueTuple<string, int, int, bool> timeDisplay = CountdownText.GetTimeDisplay(this.countdownTime, this.displayTextFormat, this.CountdownTo.DaysThreshold, string.Empty, this.CountdownTo.DefaultString);
			string item = timeDisplay.Item1;
			int item2 = timeDisplay.Item2;
			int item3 = timeDisplay.Item3;
			bool item4 = timeDisplay.Item4;
			if (!this.ShouldLocalize)
			{
				this.displayText.text = item;
				return;
			}
			this._timescaleCountdownVar.Value = item2;
			this._timeCountdownVar.Value = item3;
			this._isValidVar.Value = item4;
		}

		// Token: 0x06006FE4 RID: 28644 RVA: 0x00246BB2 File Offset: 0x00244DB2
		public static string GetTimeDisplay(TimeSpan ts, string format)
		{
			return CountdownText.GetTimeDisplay(ts, format, int.MaxValue, string.Empty, string.Empty).Item1;
		}

		// Token: 0x06006FE5 RID: 28645 RVA: 0x00246BD0 File Offset: 0x00244DD0
		[return: TupleElementNames(new string[]
		{
			"msg",
			"timescaleVar",
			"countdownVar",
			"valid"
		})]
		public static ValueTuple<string, int, int, bool> GetTimeDisplay(TimeSpan ts, string format, int maxDaysToDisplay, string elapsedString, string overMaxString)
		{
			string text = overMaxString;
			int num = 0;
			int num2 = ts.Days;
			bool flag = false;
			if (ts.TotalSeconds < 0.0)
			{
				return new ValueTuple<string, int, int, bool>(elapsedString, num, num2, flag);
			}
			if (ts.TotalDays < (double)maxDaysToDisplay)
			{
				if (ts.Days > 0)
				{
					num = 3;
					num2 = ts.Days;
					flag = true;
					text = string.Format(format, ts.Days, CountdownText.getTimeChunkString(CountdownText.TimeChunk.DAY, ts.Days));
				}
				else if (ts.Hours > 0)
				{
					num = 2;
					num2 = ts.Hours;
					flag = true;
					text = string.Format(format, ts.Hours, CountdownText.getTimeChunkString(CountdownText.TimeChunk.HOUR, ts.Hours));
				}
				else if (ts.Minutes > 0)
				{
					num = 1;
					num2 = ts.Minutes;
					flag = true;
					text = string.Format(format, ts.Minutes, CountdownText.getTimeChunkString(CountdownText.TimeChunk.MINUTE, ts.Minutes));
				}
				else if (ts.Seconds > 0)
				{
					num = 0;
					num2 = ts.Seconds;
					flag = true;
					text = string.Format(format, ts.Seconds, CountdownText.getTimeChunkString(CountdownText.TimeChunk.SECOND, ts.Seconds));
				}
			}
			return new ValueTuple<string, int, int, bool>(text, num, num2, flag);
		}

		// Token: 0x06006FE6 RID: 28646 RVA: 0x00246D04 File Offset: 0x00244F04
		private static string getTimeChunkString(CountdownText.TimeChunk chunk, int n)
		{
			switch (chunk)
			{
			case CountdownText.TimeChunk.DAY:
				if (n == 1)
				{
					return "DAY";
				}
				return "DAYS";
			case CountdownText.TimeChunk.HOUR:
				if (n == 1)
				{
					return "HOUR";
				}
				return "HOURS";
			case CountdownText.TimeChunk.MINUTE:
				if (n == 1)
				{
					return "MINUTE";
				}
				return "MINUTES";
			case CountdownText.TimeChunk.SECOND:
				if (n == 1)
				{
					return "SECOND";
				}
				return "SECONDS";
			default:
				return string.Empty;
			}
		}

		// Token: 0x06006FE7 RID: 28647 RVA: 0x00246D70 File Offset: 0x00244F70
		private DateTime TryParseDateTime()
		{
			DateTime result;
			try
			{
				result = DateTime.Parse(this.CountdownTo.CountdownTo, CultureInfo.InvariantCulture);
			}
			catch
			{
				result = DateTime.MinValue;
			}
			return result;
		}

		// Token: 0x04008038 RID: 32824
		[SerializeField]
		private CountdownTextDate CountdownTo;

		// Token: 0x04008039 RID: 32825
		[SerializeField]
		private bool updateDisplay;

		// Token: 0x0400803A RID: 32826
		[SerializeField]
		private bool useExternalTime;

		// Token: 0x0400803B RID: 32827
		[SerializeField]
		private bool shouldLocalize = true;

		// Token: 0x0400803C RID: 32828
		private TMP_Text displayText;

		// Token: 0x0400803D RID: 32829
		private string displayTextFormat;

		// Token: 0x0400803E RID: 32830
		private DateTime targetTime;

		// Token: 0x0400803F RID: 32831
		private TimeSpan countdownTime;

		// Token: 0x04008040 RID: 32832
		private Coroutine monitor;

		// Token: 0x04008041 RID: 32833
		private Coroutine displayRefresh;

		// Token: 0x04008042 RID: 32834
		private LocalizedText _locTextComp;

		// Token: 0x04008043 RID: 32835
		private LocalizedString _countdownLocStr;

		// Token: 0x04008044 RID: 32836
		private IntVariable _timeCountdownVar;

		// Token: 0x04008045 RID: 32837
		private IntVariable _timescaleCountdownVar;

		// Token: 0x04008046 RID: 32838
		private BoolVariable _isValidVar;

		// Token: 0x02001151 RID: 4433
		private enum TimeChunk
		{
			// Token: 0x04008048 RID: 32840
			DAY,
			// Token: 0x04008049 RID: 32841
			HOUR,
			// Token: 0x0400804A RID: 32842
			MINUTE,
			// Token: 0x0400804B RID: 32843
			SECOND
		}
	}
}
