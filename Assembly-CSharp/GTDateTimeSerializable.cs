using System;
using System.Globalization;
using UnityEngine;

// Token: 0x02000294 RID: 660
[Serializable]
public struct GTDateTimeSerializable : ISerializationCallbackReceiver
{
	// Token: 0x1700019F RID: 415
	// (get) Token: 0x060010EA RID: 4330 RVA: 0x0005B372 File Offset: 0x00059572
	// (set) Token: 0x060010EB RID: 4331 RVA: 0x0005B37A File Offset: 0x0005957A
	public DateTime dateTime
	{
		get
		{
			return this._dateTime;
		}
		set
		{
			this._dateTime = value;
			this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
		}
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x0005B394 File Offset: 0x00059594
	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x0005B3A8 File Offset: 0x000595A8
	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		DateTime dateTime;
		if (GTDateTimeSerializable.TryParseDateTime(this._dateTimeString, out dateTime))
		{
			this._dateTime = dateTime;
		}
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x0005B3CC File Offset: 0x000595CC
	public GTDateTimeSerializable(int dummyValue)
	{
		DateTime now = DateTime.Now;
		this._dateTime = new DateTime(now.Year, now.Month, now.Day, 11, 0, 0);
		this._dateTimeString = GTDateTimeSerializable.FormatDateTime(this._dateTime);
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x0005B414 File Offset: 0x00059614
	private static string FormatDateTime(DateTime dateTime)
	{
		return dateTime.ToString("yyyy-MM-dd HH:mm");
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x0005B424 File Offset: 0x00059624
	private static bool TryParseDateTime(string value, out DateTime result)
	{
		if (DateTime.TryParseExact(value, new string[]
		{
			"yyyy-MM-dd HH:mm",
			"yyyy-MM-dd",
			"yyyy-MM"
		}, CultureInfo.InvariantCulture, 0, ref result))
		{
			DateTime dateTime = result;
			if (dateTime.Hour == 0 && dateTime.Minute == 0)
			{
				result = result.AddHours(11.0);
			}
			return true;
		}
		return false;
	}

	// Token: 0x04001534 RID: 5428
	[HideInInspector]
	[SerializeField]
	private string _dateTimeString;

	// Token: 0x04001535 RID: 5429
	private DateTime _dateTime;
}
