using System;
using GorillaExtensions;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010BD RID: 4285
	public class HeadlessHead : HoldableObject
	{
		// Token: 0x06006B4D RID: 27469 RVA: 0x002339A0 File Offset: 0x00231BA0
		protected void Awake()
		{
			this.ownerRig = base.GetComponentInParent<VRRig>();
			if (this.ownerRig == null)
			{
				this.ownerRig = GorillaTagger.Instance.offlineVRRig;
			}
			this.isLocal = this.ownerRig.isOfflineVRRig;
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
			this.baseLocalPosition = base.transform.localPosition;
			this.hasFirstPersonRenderer = (this.firstPersonRenderer != null);
		}

		// Token: 0x06006B4E RID: 27470 RVA: 0x00233A24 File Offset: 0x00231C24
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("HeadlessHead \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.NoHead);
		}

		// Token: 0x06006B4F RID: 27471 RVA: 0x00233A7D File Offset: 0x00231C7D
		private void OnDisable()
		{
			this.ownerRig.bodyRenderer.SetCosmeticBodyType(GorillaBodyType.Default);
		}

		// Token: 0x06006B50 RID: 27472 RVA: 0x00233A90 File Offset: 0x00231C90
		protected virtual void LateUpdate()
		{
			if (this.isLocal)
			{
				this.LateUpdateLocal();
			}
			else
			{
				this.LateUpdateReplicated();
			}
			this.LateUpdateShared();
		}

		// Token: 0x06006B51 RID: 27473 RVA: 0x00233AAE File Offset: 0x00231CAE
		protected virtual void LateUpdateLocal()
		{
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, (this.isHeld ? 1 : 0) + (this.isHeldLeftHand ? 2 : 0));
		}

		// Token: 0x06006B52 RID: 27474 RVA: 0x00233AEC File Offset: 0x00231CEC
		protected virtual void LateUpdateReplicated()
		{
			int num = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
			this.isHeld = (num != 0);
			this.isHeldLeftHand = ((num & 2) != 0);
		}

		// Token: 0x06006B53 RID: 27475 RVA: 0x00233B38 File Offset: 0x00231D38
		protected virtual void LateUpdateShared()
		{
			if (this.isHeld != this.wasHeld || this.isHeldLeftHand != this.wasHeldLeftHand)
			{
				this.blendingFromPosition = base.transform.position;
				this.blendingFromRotation = base.transform.rotation;
				this.blendFraction = 0f;
			}
			Quaternion quaternion;
			Vector3 vector;
			if (this.isHeldLeftHand)
			{
				quaternion = this.ownerRig.leftHandTransform.rotation * this.rotationFromLeftHand;
				vector = this.ownerRig.leftHandTransform.TransformPoint(this.offsetFromLeftHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else if (this.isHeld)
			{
				quaternion = this.ownerRig.rightHandTransform.rotation * this.rotationFromRightHand;
				vector = this.ownerRig.rightHandTransform.TransformPoint(this.offsetFromRightHand) - quaternion * this.holdAnchorPoint.transform.localPosition;
			}
			else
			{
				quaternion = base.transform.parent.rotation;
				vector = base.transform.parent.TransformPoint(this.baseLocalPosition);
			}
			if (this.blendFraction < 1f)
			{
				this.blendFraction += Time.deltaTime / this.blendDuration;
				quaternion = Quaternion.Lerp(this.blendingFromRotation, quaternion, this.blendFraction);
				vector = Vector3.Lerp(this.blendingFromPosition, vector, this.blendFraction);
			}
			base.transform.rotation = quaternion;
			base.transform.position = vector;
			if (this.hasFirstPersonRenderer)
			{
				float x = base.transform.lossyScale.x;
				this.firstPersonRenderer.enabled = (this.firstPersonHideCenter.transform.position - GTPlayer.Instance.headCollider.transform.position).IsLongerThan(this.firstPersonHiddenRadius * x);
			}
			this.wasHeld = this.isHeld;
			this.wasHeldLeftHand = this.isHeldLeftHand;
		}

		// Token: 0x06006B54 RID: 27476 RVA: 0x00002789 File Offset: 0x00000989
		public override void OnHover(InteractionPoint pointHovered, GameObject hoveringHand)
		{
		}

		// Token: 0x06006B55 RID: 27477 RVA: 0x00233D3F File Offset: 0x00231F3F
		public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
		{
			this.isHeld = true;
			this.isHeldLeftHand = (grabbingHand == EquipmentInteractor.instance.leftHand);
			EquipmentInteractor.instance.UpdateHandEquipment(this, this.isHeldLeftHand);
		}

		// Token: 0x06006B56 RID: 27478 RVA: 0x00233D73 File Offset: 0x00231F73
		public override void DropItemCleanup()
		{
			this.isHeld = false;
			this.isHeldLeftHand = false;
		}

		// Token: 0x06006B57 RID: 27479 RVA: 0x00233D84 File Offset: 0x00231F84
		public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
		{
			if (EquipmentInteractor.instance.rightHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.rightHand)
			{
				return false;
			}
			if (EquipmentInteractor.instance.leftHandHeldEquipment == this && releasingHand != EquipmentInteractor.instance.leftHand)
			{
				return false;
			}
			EquipmentInteractor.instance.UpdateHandEquipment(null, this.isHeldLeftHand);
			this.isHeld = false;
			this.isHeldLeftHand = false;
			return true;
		}

		// Token: 0x04007BBC RID: 31676
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.Face;

		// Token: 0x04007BBD RID: 31677
		[SerializeField]
		private Vector3 offsetFromLeftHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04007BBE RID: 31678
		[SerializeField]
		private Vector3 offsetFromRightHand = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04007BBF RID: 31679
		[SerializeField]
		private Quaternion rotationFromLeftHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x04007BC0 RID: 31680
		[SerializeField]
		private Quaternion rotationFromRightHand = Quaternion.Euler(14.063973f, 52.56744f, 10.067408f);

		// Token: 0x04007BC1 RID: 31681
		private Vector3 baseLocalPosition;

		// Token: 0x04007BC2 RID: 31682
		private VRRig ownerRig;

		// Token: 0x04007BC3 RID: 31683
		private bool isLocal;

		// Token: 0x04007BC4 RID: 31684
		private bool isHeld;

		// Token: 0x04007BC5 RID: 31685
		private bool isHeldLeftHand;

		// Token: 0x04007BC6 RID: 31686
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x04007BC7 RID: 31687
		[SerializeField]
		private MeshRenderer firstPersonRenderer;

		// Token: 0x04007BC8 RID: 31688
		[SerializeField]
		private float firstPersonHiddenRadius;

		// Token: 0x04007BC9 RID: 31689
		[SerializeField]
		private Transform firstPersonHideCenter;

		// Token: 0x04007BCA RID: 31690
		[SerializeField]
		private Transform holdAnchorPoint;

		// Token: 0x04007BCB RID: 31691
		private bool hasFirstPersonRenderer;

		// Token: 0x04007BCC RID: 31692
		private Vector3 blendingFromPosition;

		// Token: 0x04007BCD RID: 31693
		private Quaternion blendingFromRotation;

		// Token: 0x04007BCE RID: 31694
		private float blendFraction;

		// Token: 0x04007BCF RID: 31695
		private bool wasHeld;

		// Token: 0x04007BD0 RID: 31696
		private bool wasHeldLeftHand;

		// Token: 0x04007BD1 RID: 31697
		[SerializeField]
		private float blendDuration = 0.3f;
	}
}
