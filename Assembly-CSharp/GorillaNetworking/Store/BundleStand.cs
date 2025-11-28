using System;
using Cosmetics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F3C RID: 3900
	public class BundleStand : MonoBehaviour, IBuildValidation
	{
		// Token: 0x1700090F RID: 2319
		// (get) Token: 0x060061A9 RID: 25001 RVA: 0x001F6F5F File Offset: 0x001F515F
		public string playfabBundleID
		{
			get
			{
				return this._bundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x060061AA RID: 25002 RVA: 0x001F6F6C File Offset: 0x001F516C
		bool IBuildValidation.BuildValidationCheck()
		{
			ICreatorCodeProvider creatorCodeProvider;
			if (this.creatorCodeProvider == null || !this.creatorCodeProvider.TryGetComponent<ICreatorCodeProvider>(ref creatorCodeProvider))
			{
				Debug.LogError(base.name + " has no Creator Code Provider. This will break bundle purchasing.");
				return false;
			}
			return true;
		}

		// Token: 0x060061AB RID: 25003 RVA: 0x001F6FB0 File Offset: 0x001F51B0
		public void Awake()
		{
			this._bundlePurchaseButton.playfabID = this.playfabBundleID;
			if (this._bundleIcon != null && this._bundleDataReference != null && this._bundleDataReference.bundleImage != null)
			{
				this._bundleIcon.sprite = this._bundleDataReference.bundleImage;
			}
			this._bundlePurchaseButton.codeProvider = this.creatorCodeProvider.GetComponent<ICreatorCodeProvider>();
		}

		// Token: 0x060061AC RID: 25004 RVA: 0x001F7029 File Offset: 0x001F5229
		public void InitializeEventListeners()
		{
			this.AlreadyOwnEvent.AddListener(new UnityAction(this._bundlePurchaseButton.AlreadyOwn));
			this.ErrorHappenedEvent.AddListener(new UnityAction(this._bundlePurchaseButton.ErrorHappened));
		}

		// Token: 0x060061AD RID: 25005 RVA: 0x001F7063 File Offset: 0x001F5263
		public void NotifyAlreadyOwn()
		{
			this.AlreadyOwnEvent.Invoke();
		}

		// Token: 0x060061AE RID: 25006 RVA: 0x001F7070 File Offset: 0x001F5270
		public void ErrorHappened()
		{
			this.ErrorHappenedEvent.Invoke();
		}

		// Token: 0x060061AF RID: 25007 RVA: 0x001F707D File Offset: 0x001F527D
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (this._bundlePurchaseButton != null)
			{
				this._bundlePurchaseButton.UpdatePurchaseButtonText(purchaseText);
			}
		}

		// Token: 0x060061B0 RID: 25008 RVA: 0x001F7099 File Offset: 0x001F5299
		public void UpdateDescriptionText(string descriptionText)
		{
			if (this._bundleDescriptionText != null)
			{
				this._bundleDescriptionText.text = descriptionText;
			}
		}

		// Token: 0x04007067 RID: 28775
		public BundlePurchaseButton _bundlePurchaseButton;

		// Token: 0x04007068 RID: 28776
		[SerializeField]
		public StoreBundleData _bundleDataReference;

		// Token: 0x04007069 RID: 28777
		[SerializeField]
		private GameObject creatorCodeProvider;

		// Token: 0x0400706A RID: 28778
		public GameObject[] EditorOnlyObjects;

		// Token: 0x0400706B RID: 28779
		public Text _bundleDescriptionText;

		// Token: 0x0400706C RID: 28780
		public Image _bundleIcon;

		// Token: 0x0400706D RID: 28781
		public UnityEvent AlreadyOwnEvent;

		// Token: 0x0400706E RID: 28782
		public UnityEvent ErrorHappenedEvent;
	}
}
