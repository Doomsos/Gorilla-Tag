using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020004AD RID: 1197
public class VRRigAnchorOverrides : MonoBehaviour
{
	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06001EEF RID: 7919 RVA: 0x000A3F5D File Offset: 0x000A215D
	// (set) Token: 0x06001EF0 RID: 7920 RVA: 0x000A3F68 File Offset: 0x000A2168
	[DebugOption]
	public Transform CurrentBadgeTransform
	{
		get
		{
			return this.currentBadgeTransform;
		}
		set
		{
			if (value != this.currentBadgeTransform)
			{
				this.ResetBadge();
				this.currentBadgeTransform = value;
				this.badgeDefaultRot = this.currentBadgeTransform.localRotation;
				this.badgeDefaultPos = this.currentBadgeTransform.localPosition;
				this.UpdateBadge();
			}
		}
	}

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x06001EF1 RID: 7921 RVA: 0x000A3FB8 File Offset: 0x000A21B8
	public Transform HuntDefaultAnchor
	{
		get
		{
			return this.huntComputerDefaultAnchor;
		}
	}

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x06001EF2 RID: 7922 RVA: 0x000A3FC0 File Offset: 0x000A21C0
	public Transform HuntComputer
	{
		get
		{
			return this.huntComputer;
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x06001EF3 RID: 7923 RVA: 0x000A3FC8 File Offset: 0x000A21C8
	public Transform BuilderWatchAnchor
	{
		get
		{
			return this.builderResizeButtonDefaultAnchor;
		}
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x06001EF4 RID: 7924 RVA: 0x000A3FD0 File Offset: 0x000A21D0
	public Transform BuilderWatch
	{
		get
		{
			return this.builderResizeButton;
		}
	}

	// Token: 0x06001EF5 RID: 7925 RVA: 0x000A3FD8 File Offset: 0x000A21D8
	private void Awake()
	{
		for (int i = 0; i < 8; i++)
		{
			this.overrideAnchors[i] = null;
		}
		int num = this.MapPositionToIndex(TransferrableObject.PositionState.OnChest);
		this.overrideAnchors[num] = this.chestDefaultTransform;
		this.huntDefaultTransform = this.huntComputer;
		this.builderResizeButtonDefaultTransform = this.builderResizeButton;
		this.activeAntiClippingOffsets = default(CosmeticAnchorAntiIntersectOffsets);
	}

	// Token: 0x06001EF6 RID: 7926 RVA: 0x000A4038 File Offset: 0x000A2238
	private void OnEnable()
	{
		if (this.nameDefaultAnchor && this.nameDefaultAnchor.parent)
		{
			this.nameTransform.parent = this.nameDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `nameTransform` because `nameDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.huntComputer = this.huntDefaultTransform;
		if (this.huntComputerDefaultAnchor && this.huntComputerDefaultAnchor.parent)
		{
			this.huntComputer.parent = this.huntComputerDefaultAnchor.parent;
		}
		else
		{
			Debug.LogError("VRRigAnchorOverrides: could not set parent `huntComputer` because `huntComputerDefaultAnchor` or its parent was null!" + base.transform.GetPathQ(), this);
		}
		this.builderResizeButton = this.builderResizeButtonDefaultTransform;
		if (this.builderResizeButtonDefaultAnchor && this.builderResizeButtonDefaultAnchor.parent)
		{
			this.builderResizeButton.parent = this.builderResizeButtonDefaultAnchor.parent;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent `builderResizeButton` because `builderResizeButtonDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x000A4154 File Offset: 0x000A2354
	private int MapPositionToIndex(TransferrableObject.PositionState pos)
	{
		int num = (int)pos;
		int num2 = 0;
		while ((num >>= 1) != 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06001EF8 RID: 7928 RVA: 0x000A4174 File Offset: 0x000A2374
	public void ApplyAntiClippingOffsets(TransferrableObject.PositionState pos, XformOffset offset, bool enable, Transform defaultAnchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					GTDev.LogWarning<string>(string.Format("Anti Clipping offset for position {0} is not implemented", pos), null);
					return;
				}
				this.activeAntiClippingOffsets.chest.enabled = enable;
				this.activeAntiClippingOffsets.chest.offset = (enable ? offset : XformOffset.Identity);
			}
			else
			{
				this.activeAntiClippingOffsets.rightArm.enabled = enable;
				this.activeAntiClippingOffsets.rightArm.offset = (enable ? offset : XformOffset.Identity);
			}
		}
		else
		{
			this.activeAntiClippingOffsets.leftArm.enabled = enable;
			this.activeAntiClippingOffsets.leftArm.offset = (enable ? offset : XformOffset.Identity);
		}
		if (enable && (this.overrideAnchors[num] == null || (pos == TransferrableObject.PositionState.OnChest && this.overrideAnchors[num] == this.chestDefaultTransform)))
		{
			if (this.clippingOffsetTransforms[num] == null)
			{
				GameObject gameObject = new GameObject("Anti Clipping Offset");
				gameObject.transform.SetParent(defaultAnchor);
				this.clippingOffsetTransforms[num] = gameObject.transform;
			}
			Transform transform = this.clippingOffsetTransforms[num];
			transform.SetParent(defaultAnchor);
			transform.localPosition = offset.pos;
			transform.localRotation = offset.rot;
			transform.localScale = Vector3.one;
			this.OverrideAnchor(pos, transform);
			return;
		}
		if (!enable && this.overrideAnchors[num] == this.clippingOffsetTransforms[num])
		{
			if (pos == TransferrableObject.PositionState.OnChest)
			{
				this.OverrideAnchor(pos, this.chestDefaultTransform);
				return;
			}
			this.OverrideAnchor(pos, null);
		}
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x000A4318 File Offset: 0x000A2518
	public void OverrideAnchor(TransferrableObject.PositionState pos, Transform anchor)
	{
		int num = this.MapPositionToIndex(pos);
		if (this.overrideAnchors[num] == this.chestDefaultTransform)
		{
			foreach (object obj in this.overrideAnchors[num])
			{
				Transform transform = (Transform)obj;
				if (!transform.name.Equals("DropZoneChest") && transform != anchor)
				{
					transform.parent = null;
				}
			}
			this.overrideAnchors[num] = anchor;
			return;
		}
		if (this.overrideAnchors[num])
		{
			foreach (object obj2 in this.overrideAnchors[num])
			{
				Transform transform2 = (Transform)obj2;
				if (transform2 != anchor)
				{
					transform2.parent = null;
				}
			}
		}
		this.overrideAnchors[num] = anchor;
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x000A4424 File Offset: 0x000A2624
	public Transform AnchorOverride(TransferrableObject.PositionState pos, Transform fallback)
	{
		int num = this.MapPositionToIndex(pos);
		Transform transform = this.overrideAnchors[num];
		if (transform != null)
		{
			return transform;
		}
		return fallback;
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x000A4448 File Offset: 0x000A2648
	public void UpdateHuntWatchOffset(XformOffset offset, bool enable)
	{
		this.activeAntiClippingOffsets.huntComputer.enabled = enable;
		this.activeAntiClippingOffsets.huntComputer.offset = (enable ? offset : XformOffset.Identity);
		this.huntComputer.parent = this.HuntDefaultAnchor;
		this.huntComputer.localPosition = this.activeAntiClippingOffsets.huntComputer.offset.pos;
		this.huntComputer.localRotation = this.activeAntiClippingOffsets.huntComputer.offset.rot;
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x000A44D4 File Offset: 0x000A26D4
	public void UpdateBuilderWatchOffset(XformOffset offset, bool enable)
	{
		this.activeAntiClippingOffsets.builderWatch.enabled = enable;
		this.activeAntiClippingOffsets.builderWatch.offset = (enable ? offset : XformOffset.Identity);
		this.BuilderWatch.parent = this.BuilderWatchAnchor;
		this.BuilderWatch.localPosition = this.activeAntiClippingOffsets.builderWatch.offset.pos;
		this.BuilderWatch.localRotation = this.activeAntiClippingOffsets.builderWatch.offset.rot;
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x000A4560 File Offset: 0x000A2760
	public void UpdateFriendshipBraceletOffset(XformOffset offset, bool left, bool enable)
	{
		if (left)
		{
			this.activeAntiClippingOffsets.friendshipBraceletLeft.enabled = enable;
			this.activeAntiClippingOffsets.friendshipBraceletLeft.offset = (enable ? offset : XformOffset.Identity);
			this.friendshipBraceletLeftAnchor.parent = this.friendshipBraceletLeftDefaultAnchor;
			this.friendshipBraceletLeftAnchor.localPosition = this.activeAntiClippingOffsets.friendshipBraceletLeft.offset.pos;
			this.friendshipBraceletLeftAnchor.localRotation = this.activeAntiClippingOffsets.friendshipBraceletLeft.offset.rot;
			this.friendshipBraceletLeftAnchor.localScale = this.activeAntiClippingOffsets.friendshipBraceletLeft.offset.scale;
			return;
		}
		this.activeAntiClippingOffsets.friendshipBraceletRight.enabled = enable;
		this.activeAntiClippingOffsets.friendshipBraceletRight.offset = (enable ? offset : XformOffset.Identity);
		this.friendshipBraceletRightAnchor.parent = this.friendshipBraceletRightDefaultAnchor;
		this.friendshipBraceletRightAnchor.localPosition = this.activeAntiClippingOffsets.friendshipBraceletRight.offset.pos;
		this.friendshipBraceletRightAnchor.localRotation = this.activeAntiClippingOffsets.friendshipBraceletRight.offset.rot;
		this.friendshipBraceletRightAnchor.localScale = this.activeAntiClippingOffsets.friendshipBraceletRight.offset.scale;
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x000A46B0 File Offset: 0x000A28B0
	public void UpdateNameTagOffset(XformOffset offset, bool enable, CosmeticsController.CosmeticSlots slot)
	{
		switch (slot)
		{
		case CosmeticsController.CosmeticSlots.Hat:
			this.nameOffsets[5].enabled = enable;
			this.nameOffsets[5].offset = offset;
			break;
		case CosmeticsController.CosmeticSlots.Badge:
			this.nameOffsets[6].enabled = enable;
			this.nameOffsets[6].offset = offset;
			break;
		case CosmeticsController.CosmeticSlots.Face:
			this.nameOffsets[4].enabled = enable;
			this.nameOffsets[4].offset = offset;
			break;
		default:
			switch (slot)
			{
			case CosmeticsController.CosmeticSlots.Fur:
				this.nameOffsets[1].enabled = enable;
				this.nameOffsets[1].offset = offset;
				break;
			case CosmeticsController.CosmeticSlots.Shirt:
				this.nameOffsets[0].enabled = enable;
				this.nameOffsets[0].offset = offset;
				break;
			case CosmeticsController.CosmeticSlots.Pants:
				this.nameOffsets[2].enabled = enable;
				this.nameOffsets[2].offset = offset;
				break;
			case CosmeticsController.CosmeticSlots.Back:
				this.nameOffsets[3].enabled = enable;
				this.nameOffsets[3].offset = offset;
				break;
			}
			break;
		}
		this.UpdateName();
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x000A4804 File Offset: 0x000A2A04
	[Obsolete("Use UpdateNameOffset", true)]
	public void UpdateNameAnchor(GameObject nameAnchor, CosmeticsController.CosmeticSlots slot)
	{
		if (slot != CosmeticsController.CosmeticSlots.Badge)
		{
			if (slot != CosmeticsController.CosmeticSlots.Face)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Fur:
					this.nameAnchors[1] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Shirt:
					this.nameAnchors[0] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Pants:
					this.nameAnchors[2] = nameAnchor;
					break;
				case CosmeticsController.CosmeticSlots.Back:
					this.nameAnchors[3] = nameAnchor;
					break;
				}
			}
			else
			{
				this.nameAnchors[4] = nameAnchor;
			}
		}
		else
		{
			this.nameAnchors[5] = nameAnchor;
		}
		this.UpdateName();
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x000A487C File Offset: 0x000A2A7C
	private void UpdateName()
	{
		for (int i = 0; i < this.nameOffsets.Length; i++)
		{
			if (this.nameOffsets[i].enabled)
			{
				this.nameTransform.parent = this.nameDefaultAnchor;
				this.nameTransform.localRotation = this.nameOffsets[i].offset.rot;
				this.nameTransform.localPosition = this.nameOffsets[i].offset.pos;
				return;
			}
		}
		if (this.nameDefaultAnchor)
		{
			this.nameTransform.parent = this.nameDefaultAnchor;
			this.nameTransform.localRotation = Quaternion.identity;
			this.nameTransform.localPosition = Vector3.zero;
			return;
		}
		Debug.LogError("VRRigAnchorOverrides: could not set parent for `nameTransform` because `nameDefaultAnchor` or its parent was null! Path: " + base.transform.GetPathQ(), this);
	}

	// Token: 0x06001F01 RID: 7937 RVA: 0x000A4960 File Offset: 0x000A2B60
	public void UpdateBadgeOffset(XformOffset offset, bool enable, CosmeticsController.CosmeticSlots slot)
	{
		if (slot != CosmeticsController.CosmeticSlots.Hat)
		{
			if (slot != CosmeticsController.CosmeticSlots.Face)
			{
				switch (slot)
				{
				case CosmeticsController.CosmeticSlots.Fur:
					this.badgeOffsets[1].enabled = enable;
					this.badgeOffsets[1].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Shirt:
					this.badgeOffsets[0].enabled = enable;
					this.badgeOffsets[0].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Pants:
					this.badgeOffsets[2].enabled = enable;
					this.badgeOffsets[2].offset = offset;
					break;
				case CosmeticsController.CosmeticSlots.Back:
					this.badgeOffsets[3].enabled = enable;
					this.badgeOffsets[3].offset = offset;
					break;
				}
			}
			else
			{
				this.badgeOffsets[4].enabled = enable;
				this.badgeOffsets[4].offset = offset;
			}
		}
		else
		{
			this.badgeOffsets[5].enabled = enable;
			this.badgeOffsets[5].offset = offset;
		}
		this.UpdateBadge();
	}

	// Token: 0x06001F02 RID: 7938 RVA: 0x000A4A88 File Offset: 0x000A2C88
	[Obsolete("Use UpdateBadgeOffset", true)]
	public void UpdateBadgeAnchor(GameObject badgeAnchor, CosmeticsController.CosmeticSlots slot)
	{
		switch (slot)
		{
		case CosmeticsController.CosmeticSlots.Fur:
			this.badgeAnchors[1] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Shirt:
			this.badgeAnchors[0] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Pants:
			this.badgeAnchors[2] = badgeAnchor;
			break;
		case CosmeticsController.CosmeticSlots.Back:
			this.badgeAnchors[3] = badgeAnchor;
			break;
		}
		this.UpdateBadge();
	}

	// Token: 0x06001F03 RID: 7939 RVA: 0x000A4AE0 File Offset: 0x000A2CE0
	private void UpdateBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		for (int i = 0; i < this.badgeOffsets.Length; i++)
		{
			if (this.badgeOffsets[i].enabled)
			{
				Matrix4x4 matrix4x = Matrix4x4.TRS(this.badgeDefaultPos, this.badgeDefaultRot, this.currentBadgeTransform.localScale);
				Matrix4x4 matrix = Matrix4x4.TRS(this.badgeOffsets[i].offset.pos, this.badgeOffsets[i].offset.rot, Vector3.one) * matrix4x;
				this.currentBadgeTransform.localRotation = matrix.rotation;
				this.currentBadgeTransform.localPosition = matrix.Position();
				return;
			}
		}
		foreach (GameObject gameObject in this.badgeAnchors)
		{
			if (gameObject)
			{
				this.currentBadgeTransform.localRotation = gameObject.transform.localRotation;
				this.currentBadgeTransform.localPosition = gameObject.transform.localPosition;
				return;
			}
		}
		this.ResetBadge();
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x000A4C02 File Offset: 0x000A2E02
	private void ResetBadge()
	{
		if (!this.currentBadgeTransform)
		{
			return;
		}
		this.currentBadgeTransform.localRotation = this.badgeDefaultRot;
		this.currentBadgeTransform.localPosition = this.badgeDefaultPos;
	}

	// Token: 0x06001F05 RID: 7941 RVA: 0x000A4C34 File Offset: 0x000A2E34
	private void OnDestroy()
	{
		for (int i = 0; i < this.clippingOffsetTransforms.Length; i++)
		{
			if (this.clippingOffsetTransforms[i] != null)
			{
				foreach (object obj in this.clippingOffsetTransforms[i])
				{
					((Transform)obj).parent = null;
				}
				Object.Destroy(this.clippingOffsetTransforms[i].gameObject);
			}
		}
	}

	// Token: 0x0400293C RID: 10556
	[SerializeField]
	public Transform nameDefaultAnchor;

	// Token: 0x0400293D RID: 10557
	[SerializeField]
	public Transform nameTransform;

	// Token: 0x0400293E RID: 10558
	[SerializeField]
	public Transform chestDefaultTransform;

	// Token: 0x0400293F RID: 10559
	[SerializeField]
	public Transform huntComputer;

	// Token: 0x04002940 RID: 10560
	[SerializeField]
	public Transform huntComputerDefaultAnchor;

	// Token: 0x04002941 RID: 10561
	public Transform huntDefaultTransform;

	// Token: 0x04002942 RID: 10562
	[SerializeField]
	protected Transform builderResizeButton;

	// Token: 0x04002943 RID: 10563
	[SerializeField]
	protected Transform builderResizeButtonDefaultAnchor;

	// Token: 0x04002944 RID: 10564
	private Transform builderResizeButtonDefaultTransform;

	// Token: 0x04002945 RID: 10565
	private readonly Transform[] overrideAnchors = new Transform[8];

	// Token: 0x04002946 RID: 10566
	private CosmeticAnchorAntiIntersectOffsets activeAntiClippingOffsets;

	// Token: 0x04002947 RID: 10567
	private Transform[] clippingOffsetTransforms = new Transform[8];

	// Token: 0x04002948 RID: 10568
	private GameObject nameLastObjectToAttach;

	// Token: 0x04002949 RID: 10569
	private Transform currentBadgeTransform;

	// Token: 0x0400294A RID: 10570
	private Vector3 badgeDefaultPos;

	// Token: 0x0400294B RID: 10571
	private Quaternion badgeDefaultRot;

	// Token: 0x0400294C RID: 10572
	private GameObject[] badgeAnchors = new GameObject[4];

	// Token: 0x0400294D RID: 10573
	private GameObject[] nameAnchors = new GameObject[6];

	// Token: 0x0400294E RID: 10574
	private CosmeticAnchorAntiClipEntry[] badgeOffsets = new CosmeticAnchorAntiClipEntry[6];

	// Token: 0x0400294F RID: 10575
	private CosmeticAnchorAntiClipEntry[] nameOffsets = new CosmeticAnchorAntiClipEntry[7];

	// Token: 0x04002950 RID: 10576
	[SerializeField]
	public Transform friendshipBraceletLeftDefaultAnchor;

	// Token: 0x04002951 RID: 10577
	public Transform friendshipBraceletLeftAnchor;

	// Token: 0x04002952 RID: 10578
	[SerializeField]
	public Transform friendshipBraceletRightDefaultAnchor;

	// Token: 0x04002953 RID: 10579
	public Transform friendshipBraceletRightAnchor;
}
