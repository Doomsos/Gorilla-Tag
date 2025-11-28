using System;
using System.Collections;
using Cosmetics;
using UnityEngine;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F3A RID: 3898
	public class BundlePurchaseButton : GorillaPressableButton, IGorillaSliceableSimple
	{
		// Token: 0x06006198 RID: 24984 RVA: 0x0001773D File Offset: 0x0001593D
		public new void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x06006199 RID: 24985 RVA: 0x00017746 File Offset: 0x00015946
		public new void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		}

		// Token: 0x0600619A RID: 24986 RVA: 0x001F6C84 File Offset: 0x001F4E84
		public void SliceUpdate()
		{
			if (NetworkSystem.Instance != null && NetworkSystem.Instance.WrongVersion && !this.bError)
			{
				base.enabled = false;
				base.GetComponent<BoxCollider>().enabled = false;
				this.buttonRenderer.material = this.pressedMaterial;
				this.myText.text = this.UnavailableText;
			}
		}

		// Token: 0x0600619B RID: 24987 RVA: 0x001F6CE7 File Offset: 0x001F4EE7
		public override void ButtonActivation()
		{
			if (this.bError)
			{
				return;
			}
			base.ButtonActivation();
			BundleManager.instance.BundlePurchaseButtonPressed(this.playfabID, this.codeProvider);
			base.StartCoroutine(this.ButtonColorUpdate());
		}

		// Token: 0x0600619C RID: 24988 RVA: 0x001F6D20 File Offset: 0x001F4F20
		public void AlreadyOwn()
		{
			if (this.bError)
			{
				return;
			}
			base.enabled = false;
			base.GetComponent<BoxCollider>().enabled = false;
			this.buttonRenderer.material = this.pressedMaterial;
			this.onText = this.AlreadyOwnText;
			this.myText.text = this.AlreadyOwnText;
			this.isOn = true;
		}

		// Token: 0x0600619D RID: 24989 RVA: 0x001F6D7E File Offset: 0x001F4F7E
		public void ResetButton()
		{
			if (this.bError)
			{
				return;
			}
			base.enabled = true;
			base.GetComponent<BoxCollider>().enabled = true;
			this.buttonRenderer.material = this.unpressedMaterial;
			this.SetOffText(true, false, false);
			this.isOn = false;
		}

		// Token: 0x0600619E RID: 24990 RVA: 0x001F6DBD File Offset: 0x001F4FBD
		private IEnumerator ButtonColorUpdate()
		{
			this.buttonRenderer.material = this.pressedMaterial;
			yield return new WaitForSeconds(this.debounceTime);
			this.buttonRenderer.material = (this.isOn ? this.pressedMaterial : this.unpressedMaterial);
			yield break;
		}

		// Token: 0x0600619F RID: 24991 RVA: 0x001F6DCC File Offset: 0x001F4FCC
		public void ErrorHappened()
		{
			this.bError = true;
			this.myText.text = this.ErrorText;
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = false;
			this.offText = this.ErrorText;
			this.onText = this.ErrorText;
			this.isOn = false;
		}

		// Token: 0x060061A0 RID: 24992 RVA: 0x001F6E28 File Offset: 0x001F5028
		public void InitializeData()
		{
			if (this.bError)
			{
				return;
			}
			this.SetOffText(true, false, false);
			this.buttonRenderer.material = this.unpressedMaterial;
			base.enabled = true;
			this.isOn = false;
		}

		// Token: 0x060061A1 RID: 24993 RVA: 0x001F6E5B File Offset: 0x001F505B
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (!this.bError)
			{
				this.offText = purchaseText;
				this.UpdateColor();
			}
		}

		// Token: 0x0400705B RID: 28763
		private const string MONKE_BLOCKS_BUNDLE_ALREADY_OWN_KEY = "MONKE_BLOCKS_BUNDLE_ALREADY_OWN";

		// Token: 0x0400705C RID: 28764
		private const string MONKE_BLOCKS_BUNDLE_UNAVAILABLE_KEY = "MONKE_BLOCKS_BUNDLE_UNAVAILABLE";

		// Token: 0x0400705D RID: 28765
		private const string MONKE_BLOCKS_BUNDLE_ERROR_KEY = "MONKE_BLOCKS_BUNDLE_ERROR";

		// Token: 0x0400705E RID: 28766
		public bool bError;

		// Token: 0x0400705F RID: 28767
		public string ErrorText = "ERROR COMPLETING PURCHASE! PLEASE RESTART THE GAME";

		// Token: 0x04007060 RID: 28768
		public string AlreadyOwnText = "YOU OWN THE BUNDLE ALREADY! THANK YOU!";

		// Token: 0x04007061 RID: 28769
		public string UnavailableText = "UNAVAILABLE";

		// Token: 0x04007062 RID: 28770
		public string playfabID = "";

		// Token: 0x04007063 RID: 28771
		public ICreatorCodeProvider codeProvider;
	}
}
