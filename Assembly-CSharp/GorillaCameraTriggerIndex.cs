using System;
using UnityEngine;

// Token: 0x0200051D RID: 1309
public class GorillaCameraTriggerIndex : MonoBehaviour
{
	// Token: 0x0600214D RID: 8525 RVA: 0x000AF40E File Offset: 0x000AD60E
	private void Start()
	{
		this.parentTrigger = base.GetComponentInParent<GorillaCameraSceneTrigger>();
	}

	// Token: 0x0600214E RID: 8526 RVA: 0x000AF41C File Offset: 0x000AD61C
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.mostRecentSceneTrigger = this;
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x0600214F RID: 8527 RVA: 0x000AF448 File Offset: 0x000AD648
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("SceneChanger"))
		{
			this.parentTrigger.ChangeScene(this);
		}
	}

	// Token: 0x04002BDE RID: 11230
	public int sceneTriggerIndex;

	// Token: 0x04002BDF RID: 11231
	public GorillaCameraSceneTrigger parentTrigger;
}
