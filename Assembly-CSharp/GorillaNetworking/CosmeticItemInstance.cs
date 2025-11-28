using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000EBC RID: 3772
	public class CosmeticItemInstance
	{
		// Token: 0x06005E06 RID: 24070 RVA: 0x001E1D24 File Offset: 0x001DFF24
		private void EnableItem(GameObject obj, bool enable)
		{
			try
			{
				obj.SetActive(enable);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Exception while enabling cosmetic: {0}", ex));
			}
		}

		// Token: 0x06005E07 RID: 24071 RVA: 0x001E1D60 File Offset: 0x001DFF60
		private void ApplyClippingOffsets(bool itemEnabled)
		{
			if (this._bodyDockPositions == null)
			{
				return;
			}
			if (this._anchorOverrides != null)
			{
				if (this.clippingOffsets.nameTag.enabled)
				{
					this._anchorOverrides.UpdateNameTagOffset(itemEnabled ? this.clippingOffsets.nameTag.offset : XformOffset.Identity, itemEnabled, this.activeSlot);
				}
				if (this.clippingOffsets.leftArm.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnLeftArm, this.clippingOffsets.leftArm.offset, itemEnabled, this._bodyDockPositions.leftArmTransform);
				}
				if (this.clippingOffsets.rightArm.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnRightArm, this.clippingOffsets.rightArm.offset, itemEnabled, this._bodyDockPositions.rightArmTransform);
				}
				if (this.clippingOffsets.chest.enabled)
				{
					this._anchorOverrides.ApplyAntiClippingOffsets(TransferrableObject.PositionState.OnChest, this.clippingOffsets.chest.offset, itemEnabled, this._anchorOverrides.chestDefaultTransform);
				}
				if (this.clippingOffsets.huntComputer.enabled)
				{
					this._anchorOverrides.UpdateHuntWatchOffset(this.clippingOffsets.huntComputer.offset, itemEnabled);
				}
				if (this.clippingOffsets.badge.enabled)
				{
					this._anchorOverrides.UpdateBadgeOffset(itemEnabled ? this.clippingOffsets.badge.offset : XformOffset.Identity, itemEnabled, this.activeSlot);
				}
				if (this.clippingOffsets.builderWatch.enabled)
				{
					this._anchorOverrides.UpdateBuilderWatchOffset(this.clippingOffsets.builderWatch.offset, itemEnabled);
				}
				if (this.clippingOffsets.friendshipBraceletLeft.enabled)
				{
					this._anchorOverrides.UpdateFriendshipBraceletOffset(this.clippingOffsets.friendshipBraceletLeft.offset, true, itemEnabled);
				}
				if (this.clippingOffsets.friendshipBraceletRight.enabled)
				{
					this._anchorOverrides.UpdateFriendshipBraceletOffset(this.clippingOffsets.friendshipBraceletRight.offset, false, itemEnabled);
				}
			}
		}

		// Token: 0x06005E08 RID: 24072 RVA: 0x001E1F74 File Offset: 0x001E0174
		public void DisableItem(CosmeticsController.CosmeticSlots cosmeticSlot)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			foreach (GameObject obj in this.objects)
			{
				this.EnableItem(obj, false);
			}
			if (flag)
			{
				foreach (GameObject obj2 in this.leftObjects)
				{
					this.EnableItem(obj2, false);
				}
			}
			if (flag2)
			{
				foreach (GameObject obj3 in this.rightObjects)
				{
					this.EnableItem(obj3, false);
				}
			}
			this.ApplyClippingOffsets(false);
		}

		// Token: 0x06005E09 RID: 24073 RVA: 0x001E2070 File Offset: 0x001E0270
		public void EnableItem(CosmeticsController.CosmeticSlots cosmeticSlot, VRRig rig)
		{
			bool flag = CosmeticsController.CosmeticSet.IsSlotLeftHanded(cosmeticSlot);
			bool flag2 = CosmeticsController.CosmeticSet.IsSlotRightHanded(cosmeticSlot);
			this.activeSlot = cosmeticSlot;
			if (rig != null && this._anchorOverrides == null)
			{
				this._anchorOverrides = rig.gameObject.GetComponent<VRRigAnchorOverrides>();
				this._bodyDockPositions = rig.GetComponent<BodyDockPositions>();
			}
			foreach (GameObject gameObject in this.objects)
			{
				this.EnableItem(gameObject, true);
				if (cosmeticSlot == CosmeticsController.CosmeticSlots.Badge)
				{
					if (this.objects.Count > 1)
					{
						GTHardCodedBones.EBone ebone;
						Transform transform;
						if (GTHardCodedBones.TryGetFirstBoneInParents(gameObject.transform, out ebone, out transform) && ebone == GTHardCodedBones.EBone.body)
						{
							this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
						}
					}
					else
					{
						this._anchorOverrides.CurrentBadgeTransform = gameObject.transform;
					}
				}
			}
			if (flag)
			{
				foreach (GameObject obj in this.leftObjects)
				{
					this.EnableItem(obj, true);
				}
			}
			if (flag2)
			{
				foreach (GameObject obj2 in this.rightObjects)
				{
					this.EnableItem(obj2, true);
				}
			}
			this.ApplyClippingOffsets(true);
		}

		// Token: 0x04006BF9 RID: 27641
		public List<GameObject> leftObjects = new List<GameObject>();

		// Token: 0x04006BFA RID: 27642
		public List<GameObject> rightObjects = new List<GameObject>();

		// Token: 0x04006BFB RID: 27643
		public List<GameObject> objects = new List<GameObject>();

		// Token: 0x04006BFC RID: 27644
		public List<GameObject> holdableObjects = new List<GameObject>();

		// Token: 0x04006BFD RID: 27645
		public CosmeticAnchorAntiIntersectOffsets clippingOffsets;

		// Token: 0x04006BFE RID: 27646
		public bool isHoldableItem;

		// Token: 0x04006BFF RID: 27647
		public string dbgname;

		// Token: 0x04006C00 RID: 27648
		private BodyDockPositions _bodyDockPositions;

		// Token: 0x04006C01 RID: 27649
		private VRRigAnchorOverrides _anchorOverrides;

		// Token: 0x04006C02 RID: 27650
		private CosmeticsController.CosmeticSlots activeSlot;
	}
}
