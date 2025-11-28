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
		// (get) Token: 0x060061A9 RID: 25001 RVA: 0x001F6F3F File Offset: 0x001F513F
		public string playfabBundleID
		{
			get
			{
				return this._bundleDataReference.playfabBundleID;
			}
		}

		// Token: 0x060061AA RID: 25002 RVA: 0x001F6F4C File Offset: 0x001F514C
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

		// Token: 0x060061AB RID: 25003 RVA: 0x001F6F90 File Offset: 0x001F5190
		public void Awake()
		{
			this._bundlePurchaseButton.playfabID = this.playfabBundleID;
			if (this._bundleIcon != null && this._bundleDataReference != null && this._bundleDataReference.bundleImage != null)
			{
				this._bundleIcon.sprite = this._bundleDataReference.bundleImage;
			}
			this._bundlePurchaseButton.codeProvider = this.creatorCodeProvider.GetComponent<ICreatorCodeProvider>();
		}

		// Token: 0x060061AC RID: 25004 RVA: 0x001F7009 File Offset: 0x001F5209
		public void InitializeEventListeners()
		{
			this.AlreadyOwnEvent.AddListener(new UnityAction(this._bundlePurchaseButton.AlreadyOwn));
			this.ErrorHappenedEvent.AddListener(new UnityAction(this._bundlePurchaseButton.ErrorHappened));
		}

		// Token: 0x060061AD RID: 25005 RVA: 0x001F7043 File Offset: 0x001F5243
		public void NotifyAlreadyOwn()
		{
			this.AlreadyOwnEvent.Invoke();
		}

		// Token: 0x060061AE RID: 25006 RVA: 0x001F7050 File Offset: 0x001F5250
		public void ErrorHappened()
		{
			this.ErrorHappenedEvent.Invoke();
		}

		// Token: 0x060061AF RID: 25007 RVA: 0x001F705D File Offset: 0x001F525D
		public void UpdatePurchaseButtonText(string purchaseText)
		{
			if (this._bundlePurchaseButton != null)
			{
				this._bundlePurchaseButton.UpdatePurchaseButtonText(purchaseText);
			}
		}

		// Token: 0x060061B0 RID: 25008 RVA: 0x001F7079 File Offset: 0x001F5279
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
