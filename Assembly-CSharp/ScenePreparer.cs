using System;
using UnityEngine;

// Token: 0x02000316 RID: 790
[DefaultExecutionOrder(-9999)]
public class ScenePreparer : MonoBehaviour
{
	// Token: 0x06001345 RID: 4933 RVA: 0x0006FA34 File Offset: 0x0006DC34
	protected void Awake()
	{
		bool flag = false;
		GameObject[] array = this.betaEnableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		array = this.betaDisableObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!flag);
		}
	}

	// Token: 0x04001CC8 RID: 7368
	public OVRManager ovrManager;

	// Token: 0x04001CC9 RID: 7369
	public GameObject[] betaDisableObjects;

	// Token: 0x04001CCA RID: 7370
	public GameObject[] betaEnableObjects;
}
