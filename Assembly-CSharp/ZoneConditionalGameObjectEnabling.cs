using System;
using UnityEngine;

// Token: 0x02000341 RID: 833
public class ZoneConditionalGameObjectEnabling : MonoBehaviour
{
	// Token: 0x0600140D RID: 5133 RVA: 0x00073D5A File Offset: 0x00071F5A
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x00073D88 File Offset: 0x00071F88
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x00073DB0 File Offset: 0x00071FB0
	private void OnZoneChanged()
	{
		if (this.invisibleWhileLoaded)
		{
			if (this.gameObjects != null)
			{
				for (int i = 0; i < this.gameObjects.Length; i++)
				{
					this.gameObjects[i].SetActive(!ZoneManagement.IsInZone(this.zone));
				}
				return;
			}
		}
		else if (this.gameObjects != null)
		{
			for (int j = 0; j < this.gameObjects.Length; j++)
			{
				this.gameObjects[j].SetActive(ZoneManagement.IsInZone(this.zone));
			}
		}
	}

	// Token: 0x04001E9F RID: 7839
	[SerializeField]
	private GTZone zone;

	// Token: 0x04001EA0 RID: 7840
	[SerializeField]
	private bool invisibleWhileLoaded;

	// Token: 0x04001EA1 RID: 7841
	[SerializeField]
	private GameObject[] gameObjects;
}
