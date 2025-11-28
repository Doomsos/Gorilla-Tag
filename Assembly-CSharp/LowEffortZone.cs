using System;
using UnityEngine;

// Token: 0x02000333 RID: 819
public class LowEffortZone : GorillaTriggerBox
{
	// Token: 0x060013C8 RID: 5064 RVA: 0x00072D18 File Offset: 0x00070F18
	private void Awake()
	{
		if (this.triggerOnAwake)
		{
			this.OnBoxTriggered();
		}
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x00072D28 File Offset: 0x00070F28
	public override void OnBoxTriggered()
	{
		for (int i = 0; i < this.objectsToEnable.Length; i++)
		{
			if (this.objectsToEnable[i] != null)
			{
				this.objectsToEnable[i].SetActive(true);
			}
		}
		for (int j = 0; j < this.objectsToDisable.Length; j++)
		{
			if (this.objectsToDisable[j] != null)
			{
				this.objectsToDisable[j].SetActive(false);
			}
		}
	}

	// Token: 0x04001E37 RID: 7735
	public GameObject[] objectsToEnable;

	// Token: 0x04001E38 RID: 7736
	public GameObject[] objectsToDisable;

	// Token: 0x04001E39 RID: 7737
	public bool triggerOnAwake;
}
