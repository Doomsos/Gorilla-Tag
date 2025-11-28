using System;
using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking.Store
{
	// Token: 0x02000F40 RID: 3904
	public class DynamicCosmeticStand : MonoBehaviour, iFlagForBaking
	{
		// Token: 0x060061C6 RID: 25030 RVA: 0x001F758C File Offset: 0x001F578C
		public virtual void SetForBaking()
		{
			this.GorillaHeadModel.SetActive(true);
			this.GorillaTorsoModel.SetActive(true);
			this.GorillaTorsoPostModel.SetActive(true);
			this.GorillaMannequinModel.SetActive(true);
			this.JeweleryBoxModel.SetActive(true);
			this.root.SetActive(true);
			this.DisplayHeadModel.gameObject.SetActive(false);
		}

		// Token: 0x060061C7 RID: 25031 RVA: 0x001F75F2 File Offset: 0x001F57F2
		public void OnEnable()
		{
			this.addToCartTextTMP.gameObject.SetActive(true);
			this.slotPriceTextTMP.gameObject.SetActive(true);
		}

		// Token: 0x060061C8 RID: 25032 RVA: 0x001F7616 File Offset: 0x001F5816
		public void OnDisable()
		{
			this.addToCartTextTMP.gameObject.SetActive(false);
			this.slotPriceTextTMP.gameObject.SetActive(false);
		}

		// Token: 0x060061C9 RID: 25033 RVA: 0x001F763A File Offset: 0x001F583A
		public virtual void SetForGame()
		{
			this.DisplayHeadModel.gameObject.SetActive(true);
			this.SetStandType(this.DisplayHeadModel.bustType);
		}

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x060061CA RID: 25034 RVA: 0x001F765E File Offset: 0x001F585E
		// (set) Token: 0x060061CB RID: 25035 RVA: 0x001F7666 File Offset: 0x001F5866
		public string thisCosmeticName
		{
			get
			{
				return this._thisCosmeticName;
			}
			set
			{
				this._thisCosmeticName = value;
			}
		}

		// Token: 0x060061CC RID: 25036 RVA: 0x001F7670 File Offset: 0x001F5870
		public void InitializeCosmetic()
		{
			this.thisCosmeticItem = CosmeticsController.instance.allCosmetics.Find((CosmeticsController.CosmeticItem x) => this.thisCosmeticName == x.displayName || this.thisCosmeticName == x.overrideDisplayName || this.thisCosmeticName == x.itemName);
			if (this.slotPriceText != null)
			{
				this.slotPriceText.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
			}
			if (this.slotPriceTextTMP != null)
			{
				this.slotPriceTextTMP.text = this.thisCosmeticItem.itemCategory.ToString().ToUpper() + " " + this.thisCosmeticItem.cost.ToString();
			}
		}

		// Token: 0x060061CD RID: 25037 RVA: 0x001F773C File Offset: 0x001F593C
		public void SpawnItemOntoStand(string PlayFabID)
		{
			this.ClearCosmetics();
			if (PlayFabID.IsNullOrEmpty())
			{
				GTDev.LogWarning<string>("ManuallyInitialize: PlayFabID is null or empty for " + this.StandName, null);
				return;
			}
			if (StoreController.instance.IsNotNull() && Application.isPlaying)
			{
				StoreController.instance.RemoveStandFromPlayFabIDDictionary(this);
			}
			this.thisCosmeticName = PlayFabID;
			if (this.thisCosmeticName.Length == 5)
			{
				this.thisCosmeticName += ".";
			}
			if (Application.isPlaying)
			{
				this.DisplayHeadModel.LoadCosmeticPartsV2(this.thisCosmeticName, false);
			}
			else
			{
				this.DisplayHeadModel.LoadCosmeticParts(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName), false);
			}
			if (StoreController.instance.IsNotNull() && Application.isPlaying)
			{
				StoreController.instance.AddStandToPlayfabIDDictionary(this);
			}
		}

		// Token: 0x060061CE RID: 25038 RVA: 0x001F780F File Offset: 0x001F5A0F
		public void ClearCosmetics()
		{
			this.thisCosmeticName = "";
			this.DisplayHeadModel.ClearManuallySpawnedCosmeticParts();
			this.DisplayHeadModel.ClearCosmetics();
		}

		// Token: 0x060061CF RID: 25039 RVA: 0x001F7834 File Offset: 0x001F5A34
		public void SetStandType(HeadModel_CosmeticStand.BustType newBustType)
		{
			this.DisplayHeadModel.SetStandType(newBustType);
			this.GorillaHeadModel.SetActive(false);
			this.GorillaTorsoModel.SetActive(false);
			this.GorillaTorsoPostModel.SetActive(false);
			this.GorillaMannequinModel.SetActive(false);
			this.GuitarStandModel.SetActive(false);
			this.JeweleryBoxModel.SetActive(false);
			this.AddToCartButton.gameObject.SetActive(true);
			Text text = this.slotPriceText;
			if (text != null)
			{
				text.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text = this.slotPriceTextTMP;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(true);
			}
			Text text2 = this.addToCartText;
			if (text2 != null)
			{
				text2.gameObject.SetActive(true);
			}
			TMP_Text tmp_Text2 = this.addToCartTextTMP;
			if (tmp_Text2 != null)
			{
				tmp_Text2.gameObject.SetActive(true);
			}
			switch (newBustType)
			{
			case HeadModel_CosmeticStand.BustType.Disabled:
			{
				this.ClearCosmetics();
				this.thisCosmeticName = "";
				this.AddToCartButton.gameObject.SetActive(false);
				Text text3 = this.slotPriceText;
				if (text3 != null)
				{
					text3.gameObject.SetActive(false);
				}
				TMP_Text tmp_Text3 = this.slotPriceTextTMP;
				if (tmp_Text3 != null)
				{
					tmp_Text3.gameObject.SetActive(false);
				}
				Text text4 = this.addToCartText;
				if (text4 != null)
				{
					text4.gameObject.SetActive(false);
				}
				TMP_Text tmp_Text4 = this.addToCartTextTMP;
				if (tmp_Text4 != null)
				{
					tmp_Text4.gameObject.SetActive(false);
				}
				this.DisplayHeadModel.transform.localPosition = Vector3.zero;
				this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
				this.root.SetActive(false);
				break;
			}
			case HeadModel_CosmeticStand.BustType.GorillaHead:
				this.root.SetActive(true);
				this.GorillaHeadModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaHeadModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaHeadModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorso:
				this.root.SetActive(true);
				this.GorillaTorsoModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaTorsoPost:
				this.root.SetActive(true);
				this.GorillaTorsoPostModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaTorsoPostModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaTorsoPostModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GorillaMannequin:
				this.root.SetActive(true);
				this.GorillaMannequinModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GorillaMannequinModel.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GorillaMannequinModel.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.GuitarStand:
				this.root.SetActive(true);
				this.GuitarStandModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.GuitarStandMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.GuitarStandMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.JewelryBox:
				this.root.SetActive(true);
				this.JeweleryBoxModel.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.JeweleryBoxMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.JeweleryBoxMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.Table:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.TableMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.TableMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.PinDisplay:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = this.PinDisplayMount.transform.localPosition;
				this.DisplayHeadModel.transform.localRotation = this.PinDisplayMount.transform.localRotation;
				break;
			case HeadModel_CosmeticStand.BustType.TagEffectDisplay:
				this.root.SetActive(true);
				break;
			default:
				this.root.SetActive(true);
				this.DisplayHeadModel.transform.localPosition = Vector3.zero;
				this.DisplayHeadModel.transform.localRotation = Quaternion.identity;
				break;
			}
			this.SpawnItemOntoStand(this.thisCosmeticName);
		}

		// Token: 0x060061D0 RID: 25040 RVA: 0x001F7D10 File Offset: 0x001F5F10
		public void CopyChildsName()
		{
			foreach (DynamicCosmeticStand dynamicCosmeticStand in base.gameObject.GetComponentsInChildren<DynamicCosmeticStand>(true))
			{
				if (dynamicCosmeticStand != this)
				{
					this.StandName = dynamicCosmeticStand.StandName;
				}
			}
		}

		// Token: 0x060061D1 RID: 25041 RVA: 0x001F7D54 File Offset: 0x001F5F54
		public void PressCosmeticStandButton()
		{
			this.searchIndex = CosmeticsController.instance.currentCart.IndexOf(this.thisCosmeticItem);
			if (this.searchIndex != -1)
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_remove, this.thisCosmeticItem);
				CosmeticsController.instance.currentCart.RemoveAt(this.searchIndex);
				foreach (DynamicCosmeticStand dynamicCosmeticStand in StoreController.instance.StandsByPlayfabID[this.thisCosmeticItem.itemName])
				{
					dynamicCosmeticStand.AddToCartButton.isOn = false;
					dynamicCosmeticStand.AddToCartButton.UpdateColor();
				}
				for (int i = 0; i < 16; i++)
				{
					if (this.thisCosmeticItem.itemName == CosmeticsController.instance.tryOnSet.items[i].itemName)
					{
						CosmeticsController.instance.tryOnSet.items[i] = CosmeticsController.instance.nullItem;
					}
				}
			}
			else
			{
				GorillaTelemetry.PostShopEvent(GorillaTagger.Instance.offlineVRRig, GTShopEventType.cart_item_add, this.thisCosmeticItem);
				CosmeticsController.instance.currentCart.Insert(0, this.thisCosmeticItem);
				foreach (DynamicCosmeticStand dynamicCosmeticStand2 in StoreController.instance.StandsByPlayfabID[this.thisCosmeticName])
				{
					dynamicCosmeticStand2.AddToCartButton.isOn = true;
					dynamicCosmeticStand2.AddToCartButton.UpdateColor();
				}
				if (CosmeticsController.instance.currentCart.Count > CosmeticsController.instance.numFittingRoomButtons)
				{
					foreach (DynamicCosmeticStand dynamicCosmeticStand3 in StoreController.instance.StandsByPlayfabID[CosmeticsController.instance.currentCart[CosmeticsController.instance.numFittingRoomButtons].itemName])
					{
						dynamicCosmeticStand3.AddToCartButton.isOn = false;
						dynamicCosmeticStand3.AddToCartButton.UpdateColor();
					}
					CosmeticsController.instance.currentCart.RemoveAt(CosmeticsController.instance.numFittingRoomButtons);
				}
			}
			CosmeticsController.instance.UpdateShoppingCart();
		}

		// Token: 0x060061D2 RID: 25042 RVA: 0x001F7FDC File Offset: 0x001F61DC
		public void SetStandTypeString(string bustTypeString)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(bustTypeString);
			if (num <= 1590453963U)
			{
				if (num <= 1121133049U)
				{
					if (num != 214514339U)
					{
						if (num == 1121133049U)
						{
							if (bustTypeString == "GuitarStand")
							{
								this.SetStandType(HeadModel_CosmeticStand.BustType.GuitarStand);
								return;
							}
						}
					}
					else if (bustTypeString == "GorillaHead")
					{
						this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaHead);
						return;
					}
				}
				else if (num != 1364530810U)
				{
					if (num != 1520673798U)
					{
						if (num == 1590453963U)
						{
							if (bustTypeString == "GorillaMannequin")
							{
								this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaMannequin);
								return;
							}
						}
					}
					else if (bustTypeString == "JewelryBox")
					{
						this.SetStandType(HeadModel_CosmeticStand.BustType.JewelryBox);
						return;
					}
				}
				else if (bustTypeString == "PinDisplay")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.PinDisplay);
					return;
				}
			}
			else if (num <= 2111326094U)
			{
				if (num != 1952506660U)
				{
					if (num == 2111326094U)
					{
						if (bustTypeString == "GorillaTorsoPost")
						{
							this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorsoPost);
							return;
						}
					}
				}
				else if (bustTypeString == "GorillaTorso")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.GorillaTorso);
					return;
				}
			}
			else if (num != 3217987877U)
			{
				if (num != 3607948159U)
				{
					if (num == 3845287012U)
					{
						if (bustTypeString == "TagEffectDisplay")
						{
							this.SetStandType(HeadModel_CosmeticStand.BustType.TagEffectDisplay);
							return;
						}
					}
				}
				else if (bustTypeString == "Table")
				{
					this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
					return;
				}
			}
			else if (bustTypeString == "Disabled")
			{
				this.SetStandType(HeadModel_CosmeticStand.BustType.Disabled);
				return;
			}
			this.SetStandType(HeadModel_CosmeticStand.BustType.Table);
		}

		// Token: 0x060061D3 RID: 25043 RVA: 0x001F818A File Offset: 0x001F638A
		public void UpdateCosmeticsMountPositions()
		{
			this.DisplayHeadModel.UpdateCosmeticsMountPositions(StoreController.FindCosmeticInAllCosmeticsArraySO(this.thisCosmeticName));
		}

		// Token: 0x060061D4 RID: 25044 RVA: 0x001F81A4 File Offset: 0x001F63A4
		public void InitializeForCustomMapCosmeticItem(GTObjectPlaceholder.ECustomMapCosmeticItem cosmeticItemSlot, Scene scene)
		{
			this.StandName = "CustomMapCosmeticItemStand-" + cosmeticItemSlot.ToString();
			this.customMapScene = scene;
			this.ClearCosmetics();
			CustomMapCosmeticItem customMapCosmeticItem;
			if (CosmeticsController.instance.customMapCosmeticsData.TryGetItem(cosmeticItemSlot, out customMapCosmeticItem))
			{
				this.thisCosmeticName = customMapCosmeticItem.playFabID;
				this.SetStandType(customMapCosmeticItem.bustType);
				this.InitializeCosmetic();
			}
		}

		// Token: 0x060061D5 RID: 25045 RVA: 0x001F820F File Offset: 0x001F640F
		public bool IsFromCustomMapScene(Scene scene)
		{
			return this.customMapScene == scene;
		}

		// Token: 0x0400707D RID: 28797
		public HeadModel_CosmeticStand DisplayHeadModel;

		// Token: 0x0400707E RID: 28798
		public GorillaPressableButton AddToCartButton;

		// Token: 0x0400707F RID: 28799
		[HideInInspector]
		public Text slotPriceText;

		// Token: 0x04007080 RID: 28800
		[HideInInspector]
		public Text addToCartText;

		// Token: 0x04007081 RID: 28801
		public TMP_Text slotPriceTextTMP;

		// Token: 0x04007082 RID: 28802
		public TMP_Text addToCartTextTMP;

		// Token: 0x04007083 RID: 28803
		private CosmeticsController.CosmeticItem thisCosmeticItem;

		// Token: 0x04007084 RID: 28804
		[FormerlySerializedAs("StandID")]
		public string StandName;

		// Token: 0x04007085 RID: 28805
		public string _thisCosmeticName = "";

		// Token: 0x04007086 RID: 28806
		public GameObject GorillaHeadModel;

		// Token: 0x04007087 RID: 28807
		public GameObject GorillaTorsoModel;

		// Token: 0x04007088 RID: 28808
		public GameObject GorillaTorsoPostModel;

		// Token: 0x04007089 RID: 28809
		public GameObject GorillaMannequinModel;

		// Token: 0x0400708A RID: 28810
		public GameObject GuitarStandModel;

		// Token: 0x0400708B RID: 28811
		public GameObject GuitarStandMount;

		// Token: 0x0400708C RID: 28812
		public GameObject JeweleryBoxModel;

		// Token: 0x0400708D RID: 28813
		public GameObject JeweleryBoxMount;

		// Token: 0x0400708E RID: 28814
		public GameObject TableMount;

		// Token: 0x0400708F RID: 28815
		[FormerlySerializedAs("PinDisplayMounnt")]
		[FormerlySerializedAs("PinDisplayMountn")]
		public GameObject PinDisplayMount;

		// Token: 0x04007090 RID: 28816
		public GameObject root;

		// Token: 0x04007091 RID: 28817
		public GameObject TagEffectDisplayMount;

		// Token: 0x04007092 RID: 28818
		public GameObject TageEffectDisplayModel;

		// Token: 0x04007093 RID: 28819
		private Scene customMapScene;

		// Token: 0x04007094 RID: 28820
		private int searchIndex;
	}
}
