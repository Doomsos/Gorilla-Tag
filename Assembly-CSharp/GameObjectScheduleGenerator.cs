using System;
using System.Globalization;
using GameObjectScheduling;
using UnityEngine;

// Token: 0x02000097 RID: 151
[CreateAssetMenu(fileName = "New Game Object Schedule Generator", menuName = "Game Object Scheduling/Game Object Schedule Generator")]
public class GameObjectScheduleGenerator : ScriptableObject
{
	// Token: 0x060003C8 RID: 968 RVA: 0x00017144 File Offset: 0x00015344
	private void GenerateSchedule()
	{
		DateTime startDate;
		try
		{
			startDate = DateTime.Parse(this.scheduleStart, CultureInfo.InvariantCulture);
		}
		catch
		{
			Debug.LogError("Don't understand Start Date " + this.scheduleStart);
			return;
		}
		DateTime endDate;
		try
		{
			endDate = DateTime.Parse(this.scheduleEnd, CultureInfo.InvariantCulture);
		}
		catch
		{
			Debug.LogError("Don't understand End Date " + this.scheduleEnd);
			return;
		}
		if (this.scheduleType == GameObjectScheduleGenerator.ScheduleType.DailyShuffle)
		{
			GameObjectSchedule.GenerateDailyShuffle(startDate, endDate, this.schedules);
		}
	}

	// Token: 0x0400043A RID: 1082
	[SerializeField]
	private GameObjectSchedule[] schedules;

	// Token: 0x0400043B RID: 1083
	[SerializeField]
	private string scheduleStart = "1/1/0001 00:00:00";

	// Token: 0x0400043C RID: 1084
	[SerializeField]
	private string scheduleEnd = "1/1/0001 00:00:00";

	// Token: 0x0400043D RID: 1085
	[SerializeField]
	private GameObjectScheduleGenerator.ScheduleType scheduleType;

	// Token: 0x02000098 RID: 152
	private enum ScheduleType
	{
		// Token: 0x0400043F RID: 1087
		DailyShuffle
	}
}
