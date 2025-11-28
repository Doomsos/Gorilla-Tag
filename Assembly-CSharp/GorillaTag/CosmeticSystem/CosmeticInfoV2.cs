using System;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.CosmeticSystem
{
	// Token: 0x02001058 RID: 4184
	[Serializable]
	public struct CosmeticInfoV2 : ISerializationCallbackReceiver
	{
		// Token: 0x170009E8 RID: 2536
		// (get) Token: 0x06006946 RID: 26950 RVA: 0x002240E0 File Offset: 0x002222E0
		public bool hasHoldableParts
		{
			get
			{
				CosmeticPart[] array = this.holdableParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x170009E9 RID: 2537
		// (get) Token: 0x06006947 RID: 26951 RVA: 0x00224100 File Offset: 0x00222300
		public bool hasWardrobeParts
		{
			get
			{
				CosmeticPart[] array = this.wardrobeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x170009EA RID: 2538
		// (get) Token: 0x06006948 RID: 26952 RVA: 0x00224120 File Offset: 0x00222320
		public bool hasStoreParts
		{
			get
			{
				CosmeticPart[] array = this.storeParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x170009EB RID: 2539
		// (get) Token: 0x06006949 RID: 26953 RVA: 0x00224140 File Offset: 0x00222340
		public bool hasFunctionalParts
		{
			get
			{
				CosmeticPart[] array = this.functionalParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x170009EC RID: 2540
		// (get) Token: 0x0600694A RID: 26954 RVA: 0x00224160 File Offset: 0x00222360
		public bool hasFirstPersonViewParts
		{
			get
			{
				CosmeticPart[] array = this.firstPersonViewParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x170009ED RID: 2541
		// (get) Token: 0x0600694B RID: 26955 RVA: 0x00224180 File Offset: 0x00222380
		public bool hasLocalRigParts
		{
			get
			{
				CosmeticPart[] array = this.localRigParts;
				return array != null && array.Length > 0;
			}
		}

		// Token: 0x0600694C RID: 26956 RVA: 0x002241A0 File Offset: 0x002223A0
		public CosmeticInfoV2(string displayName)
		{
			this.enabled = true;
			this.season = null;
			this.displayName = displayName;
			this.playFabID = "";
			this.category = CosmeticsController.CosmeticCategory.None;
			this.icon = null;
			this.isHoldable = false;
			this.isThrowable = false;
			this.usesBothHandSlots = false;
			this.hideWardrobeMannequin = false;
			this.holdableParts = new CosmeticPart[0];
			this.functionalParts = new CosmeticPart[0];
			this.wardrobeParts = new CosmeticPart[0];
			this.storeParts = new CosmeticPart[0];
			this.firstPersonViewParts = new CosmeticPart[0];
			this.localRigParts = new CosmeticPart[0];
			this.setCosmetics = new CosmeticSO[0];
			this.anchorAntiIntersectOffsets = default(CosmeticAnchorAntiIntersectOffsets);
			this.debugCosmeticSOName = "__UNINITIALIZED__";
		}

		// Token: 0x0600694D RID: 26957 RVA: 0x00002789 File Offset: 0x00000989
		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		// Token: 0x0600694E RID: 26958 RVA: 0x00224268 File Offset: 0x00222468
		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			this._OnAfterDeserialize_InitializePartsArray(ref this.holdableParts, ECosmeticPartType.Holdable);
			this._OnAfterDeserialize_InitializePartsArray(ref this.functionalParts, ECosmeticPartType.Functional);
			this._OnAfterDeserialize_InitializePartsArray(ref this.wardrobeParts, ECosmeticPartType.Wardrobe);
			this._OnAfterDeserialize_InitializePartsArray(ref this.storeParts, ECosmeticPartType.Store);
			this._OnAfterDeserialize_InitializePartsArray(ref this.firstPersonViewParts, ECosmeticPartType.FirstPerson);
			this._OnAfterDeserialize_InitializePartsArray(ref this.localRigParts, ECosmeticPartType.LocalRig);
			if (this.setCosmetics == null)
			{
				this.setCosmetics = Array.Empty<CosmeticSO>();
			}
		}

		// Token: 0x0600694F RID: 26959 RVA: 0x002242D8 File Offset: 0x002224D8
		private void _OnAfterDeserialize_InitializePartsArray(ref CosmeticPart[] parts, ECosmeticPartType partType)
		{
			for (int i = 0; i < parts.Length; i++)
			{
				parts[i].partType = partType;
				ref CosmeticAttachInfo[] ptr = ref parts[i].attachAnchors;
				if (ptr == null)
				{
					ptr = Array.Empty<CosmeticAttachInfo>();
				}
			}
		}

		// Token: 0x04007810 RID: 30736
		public bool enabled;

		// Token: 0x04007811 RID: 30737
		[Tooltip("// TODO: (2024-09-27 MattO) season will determine what addressables bundle it will be in and wheter it should be active based on release time of season.\n\nThe assigned season will determine what folder the Cosmetic will go in and how it will be listed in the Cosmetic Browser.")]
		[Delayed]
		public SeasonSO season;

		// Token: 0x04007812 RID: 30738
		[Tooltip("Name that is displayed in the store during purchasing.")]
		[Delayed]
		public string displayName;

		// Token: 0x04007813 RID: 30739
		[Tooltip("ID used on the PlayFab servers that must be unique. If this does not exist on the playfab servers then an error will be thrown. In notion search for \"Cosmetics - Adding a PlayFab ID\".")]
		[Delayed]
		public string playFabID;

		// Token: 0x04007814 RID: 30740
		public Sprite icon;

		// Token: 0x04007815 RID: 30741
		[Tooltip("Category determines which category button in the user's wardrobe (which are the two rows of buttons with equivalent names) have to be pressed to access the cosmetic along with others in the same category.")]
		public StringEnum<CosmeticsController.CosmeticCategory> category;

		// Token: 0x04007816 RID: 30742
		[Obsolete("(2024-08-13 MattO) Will be removed after holdables array is fully implemented. Check length of `holdableParts` instead.")]
		[HideInInspector]
		public bool isHoldable;

		// Token: 0x04007817 RID: 30743
		public bool isThrowable;

		// Token: 0x04007818 RID: 30744
		public bool usesBothHandSlots;

		// Token: 0x04007819 RID: 30745
		public bool hideWardrobeMannequin;

		// Token: 0x0400781A RID: 30746
		public const string holdableParts_infoBoxShortMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).";

		// Token: 0x0400781B RID: 30747
		public const string holdableParts_infoBoxDetailedMsg = "\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components. The prefab asset's transform will be moved between the listed \n attach points on \"Gorilla Player Networked.prefab\" when grabbed by the player \n";

		// Token: 0x0400781C RID: 30748
		[Space]
		[Tooltip("\"Holdable Parts\" must have a Holdable component (or inherits like TransferrableObject).\n\nHoldables are prefabs that have Holdable components. The prefab asset's transform will be moved between the listed \n attach points on \"Gorilla Player Networked.prefab\" when grabbed by the player \n")]
		public CosmeticPart[] holdableParts;

		// Token: 0x0400781D RID: 30749
		public const string functionalParts_infoBoxShortMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.";

		// Token: 0x0400781E RID: 30750
		public const string functionalParts_infoBoxDetailedMsg = "\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nIf a \"First Person View\" part or \"Local Rig Part\" is set it will be enabled instead of the wearable parts for the local player";

		// Token: 0x0400781F RID: 30751
		[Space]
		[Tooltip("\"Wearable Parts\" will be attached to \"Gorilla Player Networked.prefab\" instances.\n\nThese individual parts which also handle the core functionality of the cosmetic. In most cases there will only be one part, there can be multiple parts for cases like rings which might be on both left and right hands.\n\nThese parts will be parented to the bones of  \"Gorilla Player Networked.prefab\" instances which includes the VRRig component.\n\nIf a \"First Person View\" part or \"Local Rig Part\" is set it will be enabled instead of the wearable parts for the local player")]
		public CosmeticPart[] functionalParts;

		// Token: 0x04007820 RID: 30752
		public const string wardrobeParts_infoBoxShortMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.";

		// Token: 0x04007821 RID: 30753
		public const string wardrobeParts_infoBoxDetailedMsg = "\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\nThese prefabs are shown on the satellite wardrobe, and in the store (if \"Store Parts\" is left empty)";

		// Token: 0x04007822 RID: 30754
		[Space]
		[Tooltip("\"Wardrobe Parts\" will be attached to \"Head Model.prefab\" instances.\n\nThese parts should be static meshes not skinned and not have any scripts attached. They should only be simple visual representations.\n\nThese prefabs are shown on the satellite wardrobe, and in the store (if \"Store Parts\" is left empty)")]
		public CosmeticPart[] wardrobeParts;

		// Token: 0x04007823 RID: 30755
		public const string storeParts_infoBoxShortMsg = "\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.";

		// Token: 0x04007824 RID: 30756
		public const string storeParts_infoBoxDetailedMsg = "\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.\nStore parts only need to be specified if the store display should be different than the wardrobe display";

		// Token: 0x04007825 RID: 30757
		[Space]
		[Tooltip("\"Store Parts\" are spawned into the Dynamic Cosmetic Stands in city.\nStore parts only need to be specified if the store display should be different than the wardrobe display")]
		public CosmeticPart[] storeParts;

		// Token: 0x04007826 RID: 30758
		public const string firstPersonViewParts_infoBoxShortMsg = "\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player";

		// Token: 0x04007827 RID: 30759
		public const string firstPersonViewParts_infoBoxDetailedMsg = "\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player\nThese are used for any peripheral view meshes on the No Mirror layer, usually on HAT or FACE items";

		// Token: 0x04007828 RID: 30760
		[Space]
		[Tooltip("\"First Person View Parts\" will be attached to the local monke's camera.\nFirst person parts are enabled instead of \"Wearable Parts\" for the local player\nThese are used for any peripheral view meshes on the No Mirror layer, usually on HAT or FACE items")]
		public CosmeticPart[] firstPersonViewParts;

		// Token: 0x04007829 RID: 30761
		public const string localRigParts_infoBoxShortMsg = "\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".";

		// Token: 0x0400782A RID: 30762
		public const string localRigParts_infoBoxDetailedMsg = "\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".\nThese objects can be used in addition to first person view parts.\nThese can be used for mirror view meshes (usually HAT or FACE items)\nAny item with GTPosRotConstraints should be parented to the rig and not the camera";

		// Token: 0x0400782B RID: 30763
		[Space]
		[Tooltip("\"Local Mirror Parts\" will be attached to the local player's rig instead of \"Wearable Parts\".\nThese objects can be used in addition to first person view parts.\nThese can be used for mirror view meshes (usually HAT or FACE items)\nAny item with GTPosRotConstraints should be parented to the rig and not the camera")]
		public CosmeticPart[] localRigParts;

		// Token: 0x0400782C RID: 30764
		[Space]
		[Tooltip("When this cosmetic is equipped, these offsets will be applied to the other objects on the player that are likely to clip\nSHIRT items ususally offset the badge, nametag, and chest items\n PAW items usually offset the hunt computer and builder watch")]
		public CosmeticAnchorAntiIntersectOffsets anchorAntiIntersectOffsets;

		// Token: 0x0400782D RID: 30765
		[Space]
		[Tooltip("TODO COMMENT")]
		public CosmeticSO[] setCosmetics;

		// Token: 0x0400782E RID: 30766
		[NonSerialized]
		public string debugCosmeticSOName;
	}
}
