using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000457 RID: 1111
public class BetaChecker : MonoBehaviour
{
	// Token: 0x06001C48 RID: 7240 RVA: 0x000963D3 File Offset: 0x000945D3
	private void Start()
	{
		if (PlayerPrefs.GetString("CheckedBox2") == "true")
		{
			this.doNotEnable = true;
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001C49 RID: 7241 RVA: 0x00096400 File Offset: 0x00094600
	private void Update()
	{
		if (!this.doNotEnable)
		{
			if (CosmeticsController.instance.confirmedDidntPlayInBeta)
			{
				PlayerPrefs.SetString("CheckedBox2", "true");
				PlayerPrefs.Save();
				base.gameObject.SetActive(false);
				return;
			}
			if (CosmeticsController.instance.playedInBeta)
			{
				GameObject[] array = this.objectsToEnable;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].SetActive(true);
				}
				this.doNotEnable = true;
			}
		}
	}

	// Token: 0x04002650 RID: 9808
	public GameObject[] objectsToEnable;

	// Token: 0x04002651 RID: 9809
	public bool doNotEnable;
}
