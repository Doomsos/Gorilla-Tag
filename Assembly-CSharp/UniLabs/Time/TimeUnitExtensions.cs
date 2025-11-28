using System;

namespace UniLabs.Time
{
	// Token: 0x02000D73 RID: 3443
	public static class TimeUnitExtensions
	{
		// Token: 0x06005449 RID: 21577 RVA: 0x001AA1B8 File Offset: 0x001A83B8
		public static string ToShortString(this TimeUnit timeUnit)
		{
			string result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = "";
				break;
			case TimeUnit.Milliseconds:
				result = "ms";
				break;
			case TimeUnit.Seconds:
				result = "s";
				break;
			case TimeUnit.Minutes:
				result = "m";
				break;
			case TimeUnit.Hours:
				result = "h";
				break;
			case TimeUnit.Days:
				result = "D";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600544A RID: 21578 RVA: 0x001AA228 File Offset: 0x001A8428
		public static string ToSeparatorString(this TimeUnit timeUnit)
		{
			string result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = "";
				break;
			case TimeUnit.Milliseconds:
				result = "";
				break;
			case TimeUnit.Seconds:
				result = ".";
				break;
			case TimeUnit.Minutes:
				result = ":";
				break;
			case TimeUnit.Hours:
				result = ":";
				break;
			case TimeUnit.Days:
				result = ".";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600544B RID: 21579 RVA: 0x001AA298 File Offset: 0x001A8498
		public static double GetUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			int num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0;
				break;
			case TimeUnit.Milliseconds:
				num = timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				num = timeSpan.Seconds;
				break;
			case TimeUnit.Minutes:
				num = timeSpan.Minutes;
				break;
			case TimeUnit.Hours:
				num = timeSpan.Hours;
				break;
			case TimeUnit.Days:
				num = timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return (double)num;
		}

		// Token: 0x0600544C RID: 21580 RVA: 0x001AA310 File Offset: 0x001A8510
		public static TimeSpan WithUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan.Add(TimeSpan.FromMilliseconds(value - (double)timeSpan.Milliseconds));
				break;
			case TimeUnit.Seconds:
				result = timeSpan.Add(TimeSpan.FromSeconds(value - (double)timeSpan.Seconds));
				break;
			case TimeUnit.Minutes:
				result = timeSpan.Add(TimeSpan.FromMinutes(value - (double)timeSpan.Minutes));
				break;
			case TimeUnit.Hours:
				result = timeSpan.Add(TimeSpan.FromHours(value - (double)timeSpan.Hours));
				break;
			case TimeUnit.Days:
				result = timeSpan.Add(TimeSpan.FromDays(value - (double)timeSpan.Days));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600544D RID: 21581 RVA: 0x001AA3D4 File Offset: 0x001A85D4
		public static double GetLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = 0.0;
				break;
			case TimeUnit.Milliseconds:
				result = (double)timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalHours;
				break;
			case TimeUnit.Days:
				result = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600544E RID: 21582 RVA: 0x001AA4B0 File Offset: 0x001A86B0
		public static TimeSpan WithLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, (int)value);
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, 0, 0, 0).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				result = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x0600544F RID: 21583 RVA: 0x001AA59C File Offset: 0x001A879C
		public static double GetHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = 0.0;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).TotalHours;
				break;
			case TimeUnit.Days:
				result = (double)timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005450 RID: 21584 RVA: 0x001AA678 File Offset: 0x001A8878
		public static TimeSpan WithHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(0, 0, 0, 0, timeSpan.Milliseconds).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				result = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromDays(value));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005451 RID: 21585 RVA: 0x001AA778 File Offset: 0x001A8978
		public static double GetSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.Milliseconds:
				result = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				result = timeSpan.TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = timeSpan.TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = timeSpan.TotalHours;
				break;
			case TimeUnit.Days:
				result = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005452 RID: 21586 RVA: 0x001AA7E8 File Offset: 0x001A89E8
		public static TimeSpan FromSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = TimeSpan.Zero;
				break;
			case TimeUnit.Milliseconds:
				result = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				result = TimeSpan.FromSeconds(value);
				break;
			case TimeUnit.Minutes:
				result = TimeSpan.FromMinutes(value);
				break;
			case TimeUnit.Hours:
				result = TimeSpan.FromHours(value);
				break;
			case TimeUnit.Days:
				result = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x06005453 RID: 21587 RVA: 0x001AA860 File Offset: 0x001A8A60
		public static TimeSpan SnapToUnit(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);
				break;
			case TimeUnit.Days:
				result = new TimeSpan(timeSpan.Days, 0, 0, 0);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		// Token: 0x02000D74 RID: 3444
		// (Invoke) Token: 0x06005455 RID: 21589
		public delegate TimeSpan WithUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit, double value);

		// Token: 0x02000D75 RID: 3445
		// (Invoke) Token: 0x06005459 RID: 21593
		public delegate double GetUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit);
	}
}
