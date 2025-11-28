using System;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020004FC RID: 1276
public class DebugSpawnPointChanger : MonoBehaviour
{
	// Token: 0x060020CE RID: 8398 RVA: 0x000ADD6C File Offset: 0x000ABF6C
	private void AttachSpawnPoint(VRRig rig, Transform[] spawnPts, int locationIndex)
	{
		if (spawnPts == null)
		{
			return;
		}
		GTPlayer gtplayer = Object.FindAnyObjectByType<GTPlayer>();
		if (gtplayer == null)
		{
			return;
		}
		this.lastLocationIndex = locationIndex;
		int i = 0;
		while (i < spawnPts.Length)
		{
			Transform transform = spawnPts[i];
			if (transform.name == this.levelTriggers[locationIndex].levelName)
			{
				rig.transform.position = transform.position;
				rig.transform.rotation = transform.rotation;
				gtplayer.transform.position = transform.position;
				gtplayer.transform.rotation = transform.rotation;
				gtplayer.InitializeValues();
				SpawnPoint component = transform.GetComponent<SpawnPoint>();
				if (component != null)
				{
					gtplayer.SetScaleMultiplier(component.startSize);
					ZoneManagement.SetActiveZone(component.startZone);
					return;
				}
				Debug.LogWarning("Attempt to spawn at transform that does not have SpawnPoint component will be ignored: " + transform.name);
				return;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x060020CF RID: 8399 RVA: 0x000ADE60 File Offset: 0x000AC060
	private void ChangePoint(int index)
	{
		SpawnManager spawnManager = Object.FindAnyObjectByType<SpawnManager>();
		if (spawnManager != null)
		{
			Transform[] spawnPts = spawnManager.ChildrenXfs();
			foreach (VRRig rig in Object.FindObjectsByType<VRRig>(0))
			{
				this.AttachSpawnPoint(rig, spawnPts, index);
			}
		}
	}

	// Token: 0x060020D0 RID: 8400 RVA: 0x000ADEA7 File Offset: 0x000AC0A7
	public List<string> GetPlausibleJumpLocation()
	{
		return Enumerable.ToList<string>(Enumerable.Select<int, string>(this.levelTriggers[this.lastLocationIndex].canJumpToIndex, (int index) => this.levelTriggers[index].levelName));
	}

	// Token: 0x060020D1 RID: 8401 RVA: 0x000ADED8 File Offset: 0x000AC0D8
	public void JumpTo(int canJumpIndex)
	{
		DebugSpawnPointChanger.GeoTriggersGroup geoTriggersGroup = this.levelTriggers[this.lastLocationIndex];
		this.ChangePoint(geoTriggersGroup.canJumpToIndex[canJumpIndex]);
	}

	// Token: 0x060020D2 RID: 8402 RVA: 0x000ADF08 File Offset: 0x000AC108
	public void SetLastLocation(string levelName)
	{
		for (int i = 0; i < this.levelTriggers.Length; i++)
		{
			if (!(this.levelTriggers[i].levelName != levelName))
			{
				this.lastLocationIndex = i;
				return;
			}
		}
	}

	// Token: 0x04002B61 RID: 11105
	[SerializeField]
	private DebugSpawnPointChanger.GeoTriggersGroup[] levelTriggers;

	// Token: 0x04002B62 RID: 11106
	private int lastLocationIndex;

	// Token: 0x020004FD RID: 1277
	[Serializable]
	private struct GeoTriggersGroup
	{
		// Token: 0x04002B63 RID: 11107
		public string levelName;

		// Token: 0x04002B64 RID: 11108
		public GorillaGeoHideShowTrigger enterTrigger;

		// Token: 0x04002B65 RID: 11109
		public GorillaGeoHideShowTrigger[] leaveTrigger;

		// Token: 0x04002B66 RID: 11110
		public int[] canJumpToIndex;
	}
}
