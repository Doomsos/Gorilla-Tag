using System;
using UnityEngine;

// Token: 0x0200006A RID: 106
public class CritterSpawnCriteria : ScriptableObject
{
	// Token: 0x0600029C RID: 668 RVA: 0x0001065C File Offset: 0x0000E85C
	public bool CanSpawn()
	{
		if (this.spawnTimings.Length == 0)
		{
			return true;
		}
		string currentTimeOfDay = BetterDayNightManager.instance.currentTimeOfDay;
		string[] array = this.spawnTimings;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == currentTimeOfDay)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04000315 RID: 789
	public string[] spawnTimings;
}
