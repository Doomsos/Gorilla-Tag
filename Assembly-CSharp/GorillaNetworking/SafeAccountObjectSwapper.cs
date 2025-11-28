using System;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EDD RID: 3805
	public class SafeAccountObjectSwapper : MonoBehaviour
	{
		// Token: 0x06005F1F RID: 24351 RVA: 0x001E93B6 File Offset: 0x001E75B6
		public void Start()
		{
			if (PlayFabAuthenticator.instance.GetSafety())
			{
				this.SwitchToSafeMode();
			}
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.SafeAccountUpdated));
		}

		// Token: 0x06005F20 RID: 24352 RVA: 0x001E93F4 File Offset: 0x001E75F4
		public void SafeAccountUpdated(bool isSafety)
		{
			if (isSafety)
			{
				this.SwitchToSafeMode();
			}
		}

		// Token: 0x06005F21 RID: 24353 RVA: 0x001E9400 File Offset: 0x001E7600
		public void SwitchToSafeMode()
		{
			foreach (GameObject gameObject in this.UnSafeGameObjects)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			foreach (GameObject gameObject2 in this.UnSafeTexts)
			{
				if (gameObject2 != null)
				{
					gameObject2.SetActive(false);
				}
			}
			foreach (GameObject gameObject3 in this.SafeTexts)
			{
				if (gameObject3 != null)
				{
					gameObject3.SetActive(true);
				}
			}
			foreach (GameObject gameObject4 in this.SafeModeObjects)
			{
				if (gameObject4 != null)
				{
					gameObject4.SetActive(true);
				}
			}
		}

		// Token: 0x04006D02 RID: 27906
		public GameObject[] UnSafeGameObjects;

		// Token: 0x04006D03 RID: 27907
		public GameObject[] UnSafeTexts;

		// Token: 0x04006D04 RID: 27908
		public GameObject[] SafeTexts;

		// Token: 0x04006D05 RID: 27909
		public GameObject[] SafeModeObjects;
	}
}
