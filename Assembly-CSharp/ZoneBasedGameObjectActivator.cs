using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000CF2 RID: 3314
public class ZoneBasedGameObjectActivator : MonoBehaviour
{
	// Token: 0x06005078 RID: 20600 RVA: 0x0019E3B6 File Offset: 0x0019C5B6
	private void OnEnable()
	{
		ZoneManagement.OnZoneChange += this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x06005079 RID: 20601 RVA: 0x0019E3C9 File Offset: 0x0019C5C9
	private void OnDisable()
	{
		ZoneManagement.OnZoneChange -= this.ZoneManagement_OnZoneChange;
	}

	// Token: 0x0600507A RID: 20602 RVA: 0x0019E3DC File Offset: 0x0019C5DC
	private void ZoneManagement_OnZoneChange(ZoneData[] zoneData)
	{
		HashSet<GTZone> hashSet = new HashSet<GTZone>(this.zones);
		bool flag = false;
		for (int i = 0; i < zoneData.Length; i++)
		{
			flag |= (zoneData[i].active && hashSet.Contains(zoneData[i].zone));
		}
		for (int j = 0; j < this.gameObjects.Length; j++)
		{
			this.gameObjects[j].SetActive(flag);
		}
	}

	// Token: 0x04005FD7 RID: 24535
	[SerializeField]
	private GTZone[] zones;

	// Token: 0x04005FD8 RID: 24536
	[SerializeField]
	private GameObject[] gameObjects;
}
