using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000C4E RID: 3150
public class HideInQuest1AtRuntime : MonoBehaviour
{
	// Token: 0x06004D17 RID: 19735 RVA: 0x0019019E File Offset: 0x0018E39E
	private void OnEnable()
	{
		if (PlayFabAuthenticator.instance != null && "Quest1" == PlayFabAuthenticator.instance.platform.ToString())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
