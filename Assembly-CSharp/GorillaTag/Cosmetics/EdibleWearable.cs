using System;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010BB RID: 4283
	public class EdibleWearable : MonoBehaviour
	{
		// Token: 0x06006B45 RID: 27461 RVA: 0x00233418 File Offset: 0x00231618
		protected void Awake()
		{
			this.edibleState = 0;
			this.previousEdibleState = 0;
			this.ownerRig = base.GetComponentInParent<VRRig>();
			this.isLocal = (this.ownerRig != null && this.ownerRig.isOfflineVRRig);
			this.isHandSlot = (this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand || this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.RightHand);
			this.isLeftHand = (this.wearablePackedStateSlot == VRRig.WearablePackedStateSlots.LeftHand);
			this.stateBitsWriteInfo = VRRig.WearablePackedStatesBitWriteInfos[(int)this.wearablePackedStateSlot];
		}

		// Token: 0x06006B46 RID: 27462 RVA: 0x002334A4 File Offset: 0x002316A4
		protected void OnEnable()
		{
			if (this.ownerRig == null)
			{
				Debug.LogError("EdibleWearable \"" + base.transform.GetPath() + "\": Deactivating because ownerRig is null.", this);
				base.gameObject.SetActive(false);
				return;
			}
			for (int i = 0; i < this.edibleStateInfos.Length; i++)
			{
				this.edibleStateInfos[i].gameObject.SetActive(i == this.edibleState);
			}
		}

		// Token: 0x06006B47 RID: 27463 RVA: 0x0023351E File Offset: 0x0023171E
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

		// Token: 0x06006B48 RID: 27464 RVA: 0x0023353C File Offset: 0x0023173C
		protected virtual void LateUpdateLocal()
		{
			if (this.edibleState == this.edibleStateInfos.Length - 1)
			{
				if (!this.isNonRespawnable && Time.time > this.lastFullyEatenTime + this.respawnTime)
				{
					this.edibleState = 0;
					this.previousEdibleState = 0;
					this.OnEdibleHoldableStateChange();
				}
				if (this.isNonRespawnable && Time.time > this.lastFullyEatenTime)
				{
					this.edibleState = 0;
					this.previousEdibleState = 0;
					this.OnEdibleHoldableStateChange();
					GorillaGameManager.instance.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer).netView.SendRPC("EnableNonCosmeticHandItemRPC", 0, new object[]
					{
						false,
						this.isLeftHand
					});
				}
			}
			else if (Time.time > this.lastEatTime + this.biteCooldown)
			{
				Vector3 vector = base.transform.TransformPoint(this.edibleBiteOffset);
				bool flag = false;
				float num = this.biteDistance * this.biteDistance;
				if (!GorillaParent.hasInstance)
				{
					return;
				}
				if ((GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - vector).sqrMagnitude < num)
				{
					flag = true;
				}
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!flag)
					{
						if (vrrig.head == null)
						{
							break;
						}
						if (vrrig.head.rigTarget == null)
						{
							break;
						}
						if ((vrrig.head.rigTarget.transform.TransformPoint(this.gorillaHeadMouthOffset) - vector).sqrMagnitude < num)
						{
							flag = true;
						}
					}
				}
				if (flag && !this.wasInBiteZoneLastFrame && this.edibleState < this.edibleStateInfos.Length)
				{
					this.edibleState++;
					this.lastEatTime = Time.time;
					this.lastFullyEatenTime = Time.time;
				}
				this.wasInBiteZoneLastFrame = flag;
			}
			this.ownerRig.WearablePackedStates = GTBitOps.WriteBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo, this.edibleState);
		}

		// Token: 0x06006B49 RID: 27465 RVA: 0x00233780 File Offset: 0x00231980
		protected virtual void LateUpdateReplicated()
		{
			this.edibleState = GTBitOps.ReadBits(this.ownerRig.WearablePackedStates, this.stateBitsWriteInfo.index, this.stateBitsWriteInfo.valueMask);
		}

		// Token: 0x06006B4A RID: 27466 RVA: 0x002337B0 File Offset: 0x002319B0
		protected virtual void LateUpdateShared()
		{
			int num = this.edibleState;
			if (num != this.previousEdibleState)
			{
				this.OnEdibleHoldableStateChange();
			}
			this.previousEdibleState = num;
		}

		// Token: 0x06006B4B RID: 27467 RVA: 0x002337DC File Offset: 0x002319DC
		protected virtual void OnEdibleHoldableStateChange()
		{
			if (this.previousEdibleState >= 0 && this.previousEdibleState < this.edibleStateInfos.Length)
			{
				this.edibleStateInfos[this.previousEdibleState].gameObject.SetActive(false);
			}
			if (this.edibleState >= 0 && this.edibleState < this.edibleStateInfos.Length)
			{
				this.edibleStateInfos[this.edibleState].gameObject.SetActive(true);
			}
			if (this.edibleState > 0 && this.edibleState < this.edibleStateInfos.Length && this.audioSource != null)
			{
				this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState].sound, this.volume);
			}
			if (this.edibleState == this.edibleStateInfos.Length && this.audioSource != null)
			{
				this.audioSource.GTPlayOneShot(this.edibleStateInfos[this.edibleState - 1].sound, this.volume);
			}
			float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			if (this.isLocal && this.isHandSlot)
			{
				GorillaTagger.Instance.StartVibration(this.isLeftHand, amplitude, fixedDeltaTime);
			}
		}

		// Token: 0x04007BA6 RID: 31654
		[Tooltip("Check when using non cosmetic edible items like honeycomb")]
		public bool isNonRespawnable;

		// Token: 0x04007BA7 RID: 31655
		[Tooltip("Eating sounds are played through this AudioSource using PlayOneShot.")]
		public AudioSource audioSource;

		// Token: 0x04007BA8 RID: 31656
		[Tooltip("Volume each bite should play at.")]
		public float volume = 0.08f;

		// Token: 0x04007BA9 RID: 31657
		[Tooltip("The slot this cosmetic resides.")]
		public VRRig.WearablePackedStateSlots wearablePackedStateSlot = VRRig.WearablePackedStateSlots.LeftHand;

		// Token: 0x04007BAA RID: 31658
		[Tooltip("Time between bites.")]
		public float biteCooldown = 1f;

		// Token: 0x04007BAB RID: 31659
		[Tooltip("How long it takes to pop back to the uneaten state after being fully eaten.")]
		public float respawnTime = 7f;

		// Token: 0x04007BAC RID: 31660
		[Tooltip("Distance from mouth to item required to trigger a bite.")]
		public float biteDistance = 0.5f;

		// Token: 0x04007BAD RID: 31661
		[Tooltip("Offset from Gorilla's head to mouth.")]
		public Vector3 gorillaHeadMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04007BAE RID: 31662
		[Tooltip("Offset from edible's transform to the bite point.")]
		public Vector3 edibleBiteOffset = new Vector3(0f, 0f, 0f);

		// Token: 0x04007BAF RID: 31663
		public EdibleWearable.EdibleStateInfo[] edibleStateInfos;

		// Token: 0x04007BB0 RID: 31664
		private VRRig ownerRig;

		// Token: 0x04007BB1 RID: 31665
		private bool isLocal;

		// Token: 0x04007BB2 RID: 31666
		private bool isHandSlot;

		// Token: 0x04007BB3 RID: 31667
		private bool isLeftHand;

		// Token: 0x04007BB4 RID: 31668
		private GTBitOps.BitWriteInfo stateBitsWriteInfo;

		// Token: 0x04007BB5 RID: 31669
		private int edibleState;

		// Token: 0x04007BB6 RID: 31670
		private int previousEdibleState;

		// Token: 0x04007BB7 RID: 31671
		private float lastEatTime;

		// Token: 0x04007BB8 RID: 31672
		private float lastFullyEatenTime;

		// Token: 0x04007BB9 RID: 31673
		private bool wasInBiteZoneLastFrame;

		// Token: 0x020010BC RID: 4284
		[Serializable]
		public struct EdibleStateInfo
		{
			// Token: 0x04007BBA RID: 31674
			[Tooltip("Will be activated when this stage is reached.")]
			public GameObject gameObject;

			// Token: 0x04007BBB RID: 31675
			[Tooltip("Will be played when this stage is reached.")]
			public AudioClip sound;
		}
	}
}
