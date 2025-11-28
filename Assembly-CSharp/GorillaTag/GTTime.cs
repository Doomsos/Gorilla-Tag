using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FD3 RID: 4051
	public static class GTTime
	{
		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x060066B2 RID: 26290 RVA: 0x00217118 File Offset: 0x00215318
		// (set) Token: 0x060066B3 RID: 26291 RVA: 0x0021711F File Offset: 0x0021531F
		public static TimeZoneInfo timeZoneInfoLA { get; private set; }

		// Token: 0x060066B4 RID: 26292 RVA: 0x00217127 File Offset: 0x00215327
		static GTTime()
		{
			GTTime._Init();
		}

		// Token: 0x060066B5 RID: 26293 RVA: 0x00217130 File Offset: 0x00215330
		[RuntimeInitializeOnLoadMethod]
		private static void _Init()
		{
			if (GTTime._isInitialized)
			{
				return;
			}
			try
			{
				GTTime.timeZoneInfoLA = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
			}
			catch
			{
				try
				{
					GTTime.timeZoneInfoLA = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
				}
				catch
				{
					TimeZoneInfo timeZoneInfoLA;
					if (GTTime._TryCreateCustomPST(out timeZoneInfoLA))
					{
						GTTime.timeZoneInfoLA = timeZoneInfoLA;
						Debug.Log("[GTTime]  _Init: Could not get US Pacific Time Zone, so using manual created Pacific time zone instead.");
					}
					else
					{
						Debug.LogError("[GTTime]  ERROR!!!  _Init: Could not get US Pacific Time Zone and manual Pacific time zone creation failed. Using UTC instead.");
						GTTime.timeZoneInfoLA = TimeZoneInfo.Utc;
					}
				}
			}
			finally
			{
				GTTime._isInitialized = true;
			}
		}

		// Token: 0x060066B6 RID: 26294 RVA: 0x002171CC File Offset: 0x002153CC
		private static bool _TryCreateCustomPST(out TimeZoneInfo out_tz)
		{
			TimeZoneInfo.AdjustmentRule[] array = new TimeZoneInfo.AdjustmentRule[]
			{
				TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(2007, 1, 1), DateTime.MaxValue.Date, TimeSpan.FromHours(1.0), TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 2, 0), TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1, 0))
			};
			bool result;
			try
			{
				out_tz = TimeZoneInfo.CreateCustomTimeZone("Custom/America_Los_Angeles", TimeSpan.FromHours(-8.0), "(UTC-08:00) Pacific Time (US & Canada)", "Pacific Standard Time", "Pacific Daylight Time", array, false);
				result = true;
			}
			catch (Exception ex)
			{
				Debug.LogError("[GTTime]  ERROR!!!  _TryCreateCustomPST: Encountered exception: " + ex.Message);
				out_tz = null;
				result = false;
			}
			return result;
		}

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x060066B7 RID: 26295 RVA: 0x00217290 File Offset: 0x00215490
		// (set) Token: 0x060066B8 RID: 26296 RVA: 0x00217297 File Offset: 0x00215497
		public static bool usingServerTime { get; private set; }

		// Token: 0x060066B9 RID: 26297 RVA: 0x0021729F File Offset: 0x0021549F
		[MethodImpl(256)]
		private static long GetServerStartupTimeAsMilliseconds()
		{
			return GorillaComputer.instance.startupMillis;
		}

		// Token: 0x060066BA RID: 26298 RVA: 0x002172B0 File Offset: 0x002154B0
		[MethodImpl(256)]
		private static long GetDeviceStartupTimeAsMilliseconds()
		{
			return (long)(TimeSpan.FromTicks(DateTime.UtcNow.Ticks).TotalMilliseconds - Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x060066BB RID: 26299 RVA: 0x002172E8 File Offset: 0x002154E8
		[MethodImpl(256)]
		public static long GetStartupTimeAsMilliseconds()
		{
			GTTime.usingServerTime = true;
			long num = 0L;
			if (GorillaComputer.hasInstance)
			{
				num = GTTime.GetServerStartupTimeAsMilliseconds();
			}
			if (num == 0L)
			{
				GTTime.usingServerTime = false;
				num = GTTime.GetDeviceStartupTimeAsMilliseconds();
			}
			return num;
		}

		// Token: 0x060066BC RID: 26300 RVA: 0x0021731B File Offset: 0x0021551B
		public static long TimeAsMilliseconds()
		{
			return GTTime.GetStartupTimeAsMilliseconds() + (long)(Time.realtimeSinceStartupAsDouble * 1000.0);
		}

		// Token: 0x060066BD RID: 26301 RVA: 0x00217333 File Offset: 0x00215533
		public static double TimeAsDouble()
		{
			return (double)GTTime.GetStartupTimeAsMilliseconds() / 1000.0 + Time.realtimeSinceStartupAsDouble;
		}

		// Token: 0x060066BE RID: 26302 RVA: 0x0021734B File Offset: 0x0021554B
		public static DateTime GetAAxiomDateTime()
		{
			return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GTTime.timeZoneInfoLA);
		}

		// Token: 0x060066BF RID: 26303 RVA: 0x0021735C File Offset: 0x0021555C
		public static string GetAAxiomDateTimeAsStringForDisplay()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
		}

		// Token: 0x060066C0 RID: 26304 RVA: 0x0021737C File Offset: 0x0021557C
		public static string GetAAxiomDateTimeAsStringForFilename()
		{
			return GTTime.GetAAxiomDateTime().ToString("yyyy-MM-dd_HH-mm-ss-fff");
		}

		// Token: 0x060066C1 RID: 26305 RVA: 0x0021739C File Offset: 0x0021559C
		public static long GetAAxiomDateTimeAsHumanReadableLong()
		{
			return long.Parse(GTTime.GetAAxiomDateTime().ToString("yyyyMMddHHmmssfff00"));
		}

		// Token: 0x060066C2 RID: 26306 RVA: 0x002173C0 File Offset: 0x002155C0
		public static DateTime ConvertDateTimeHumanReadableLongToDateTime(long humanReadableLong)
		{
			return DateTime.ParseExact(humanReadableLong.ToString(), "yyyyMMddHHmmssfff'00'", CultureInfo.InvariantCulture);
		}

		// Token: 0x04007556 RID: 30038
		private const string preLog = "[GTTime]  ";

		// Token: 0x04007557 RID: 30039
		private const string preErr = "[GTTime]  ERROR!!!  ";

		// Token: 0x04007558 RID: 30040
		private static bool _isInitialized;
	}
}
