using System;
using UnityEngine;

// Token: 0x020006A3 RID: 1699
public class GRElevatorButton : MonoBehaviour
{
	// Token: 0x06002B6A RID: 11114 RVA: 0x000E8A40 File Offset: 0x000E6C40
	private void Awake()
	{
		if (this.disableDelayed == null)
		{
			this.disableDelayed = this.buttonLit.GetComponent<DisableGameObjectDelayed>();
		}
		if (this.tempLight)
		{
			this.disableDelayed.enabled = false;
			return;
		}
		this.disableDelayed.delayTime = this.litUpTime;
	}

	// Token: 0x06002B6B RID: 11115 RVA: 0x000E8A92 File Offset: 0x000E6C92
	public void Pressed()
	{
		this.buttonLit.SetActive(true);
	}

	// Token: 0x06002B6C RID: 11116 RVA: 0x000E8AA0 File Offset: 0x000E6CA0
	public void Depressed()
	{
		this.buttonLit.SetActive(false);
	}

	// Token: 0x040037F8 RID: 14328
	public GRElevator.ButtonType buttonType;

	// Token: 0x040037F9 RID: 14329
	public GameObject buttonLit;

	// Token: 0x040037FA RID: 14330
	public float litUpTime;

	// Token: 0x040037FB RID: 14331
	public DisableGameObjectDelayed disableDelayed;

	// Token: 0x040037FC RID: 14332
	public bool tempLight;
}
