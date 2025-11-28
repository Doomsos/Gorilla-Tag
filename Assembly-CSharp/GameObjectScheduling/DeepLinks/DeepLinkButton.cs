using System;
using System.Collections;
using UnityEngine;

namespace GameObjectScheduling.DeepLinks
{
	// Token: 0x0200115E RID: 4446
	public class DeepLinkButton : GorillaPressableButton
	{
		// Token: 0x0600701E RID: 28702 RVA: 0x0024768A File Offset: 0x0024588A
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			this.sendingDeepLink = DeepLinkSender.SendDeepLink(this.deepLinkAppID, this.deepLinkPayload, new Action<string>(this.OnDeepLinkSent));
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x0600701F RID: 28703 RVA: 0x002476C2 File Offset: 0x002458C2
		private void OnDeepLinkSent(string message)
		{
			this.sendingDeepLink = false;
			if (!this.isOn)
			{
				this.UpdateColor();
			}
		}

		// Token: 0x06007020 RID: 28704 RVA: 0x002476D9 File Offset: 0x002458D9
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.pressedTime);
			this.isOn = false;
			if (!this.sendingDeepLink)
			{
				this.UpdateColor();
			}
			yield break;
		}

		// Token: 0x04008073 RID: 32883
		[SerializeField]
		private ulong deepLinkAppID;

		// Token: 0x04008074 RID: 32884
		[SerializeField]
		private string deepLinkPayload = "";

		// Token: 0x04008075 RID: 32885
		[SerializeField]
		private float pressedTime = 0.2f;

		// Token: 0x04008076 RID: 32886
		private bool sendingDeepLink;
	}
}
