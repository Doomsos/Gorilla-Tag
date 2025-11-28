using System;
using GorillaNetworking;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010BE RID: 4286
	public class NetworkedWearable : MonoBehaviour, ISpawnable, ITickSystemTick
	{
		// Token: 0x06006B59 RID: 27481 RVA: 0x00233E70 File Offset: 0x00232070
		private void Awake()
		{
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw)
			{
				this.isTwoHanded = false;
			}
			this.wearableSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, true);
			this.leftSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, true);
			this.rightSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, false);
		}

		// Token: 0x06006B5A RID: 27482 RVA: 0x00233EC6 File Offset: 0x002320C6
		private void OnEnable()
		{
			if (!this.IsSpawned)
			{
				return;
			}
			if (this.isLocal && !this.listenForChangesLocal)
			{
				this.SetWearableStateBool(this.startTrue);
				return;
			}
			if (!this.TickRunning)
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06006B5B RID: 27483 RVA: 0x00233EFC File Offset: 0x002320FC
		public void ToggleWearableStateBool()
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling ToggleWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling ToggleWearableStateBool on two handed object " + base.gameObject.name + ". please use ToggleLeftWearableStateBool or ToggleRightWearableStateBool instead", null);
				this.ToggleLeftWearableStateBool();
				return;
			}
			this.value = !this.value;
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.wearableSlot, this.value);
			this.OnWearableStateChanged();
		}

		// Token: 0x06006B5C RID: 27484 RVA: 0x00233FD4 File Offset: 0x002321D4
		public void SetWearableStateBool(bool newState)
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling SetWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling SetWearableStateBool on two handed object " + base.gameObject.name + ". please use SetLeftWearableStateBool or SetRightWearableStateBool instead", null);
				this.SetLeftWearableStateBool(newState);
				return;
			}
			if (this.value != newState)
			{
				this.value = newState;
				this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.wearableSlot, this.value);
				this.OnWearableStateChanged();
			}
		}

		// Token: 0x06006B5D RID: 27485 RVA: 0x002340B0 File Offset: 0x002322B0
		public void ToggleLeftWearableStateBool()
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling ToggleLeftWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling ToggleLeftWearableStateBool on one handed object " + base.gameObject.name + ". Please use ToggleWearableStateBool instead", null);
				this.ToggleWearableStateBool();
				return;
			}
			this.leftHandValue = !this.leftHandValue;
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.leftSlot, this.leftHandValue);
			this.OnLeftStateChanged();
		}

		// Token: 0x06006B5E RID: 27486 RVA: 0x00234188 File Offset: 0x00232388
		public void ToggleRightWearableStateBool()
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling ToggleRightWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling ToggleRightWearableStateBool on one handed object " + base.gameObject.name + ". Please use ToggleWearableStateBool instead", null);
				this.ToggleWearableStateBool();
				return;
			}
			this.rightHandValue = !this.rightHandValue;
			this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.rightSlot, this.rightHandValue);
			this.OnRightStateChanged();
		}

		// Token: 0x06006B5F RID: 27487 RVA: 0x00234260 File Offset: 0x00232460
		public void SetLeftWearableStateBool(bool newState)
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling SetLeftWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling SetLeftWearableStateBool on one handed object " + base.gameObject.name + ". Please use SetWearableStateBool instead", null);
				this.SetWearableStateBool(newState);
				return;
			}
			if (this.leftHandValue != newState)
			{
				this.leftHandValue = newState;
				this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.leftSlot, this.leftHandValue);
				this.OnLeftStateChanged();
			}
		}

		// Token: 0x06006B60 RID: 27488 RVA: 0x0023433C File Offset: 0x0023253C
		public void SetRightWearableStateBool(bool newState)
		{
			if (!this.isLocal || !this.IsSpawned)
			{
				return;
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				return;
			}
			if (this.myRig == null)
			{
				return;
			}
			if (this.listenForChangesLocal)
			{
				GTDev.LogError<string>("NetworkedWearable with listenForChangesLocal calling SetRightWearableStateBool on object " + base.gameObject.name + ".You should not change state from a listener", null);
				return;
			}
			if (this.assignedSlot != CosmeticsController.CosmeticCategory.Paw || !this.isTwoHanded)
			{
				GTDev.LogWarning<string>("NetworkedWearable calling SetRightWearableStateBool on one handed object " + base.gameObject.name + ". Please use SetWearableStateBool instead", null);
				this.SetWearableStateBool(newState);
				return;
			}
			if (this.rightHandValue != newState)
			{
				this.rightHandValue = newState;
				this.myRig.WearablePackedStates = GTBitOps.WriteBit(this.myRig.WearablePackedStates, (int)this.rightSlot, this.rightHandValue);
				this.OnRightStateChanged();
			}
		}

		// Token: 0x06006B61 RID: 27489 RVA: 0x00234416 File Offset: 0x00232616
		public void OnDisable()
		{
			if (this.isLocal && !this.listenForChangesLocal)
			{
				this.SetWearableStateBool(false);
				return;
			}
			if (this.TickRunning)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
		}

		// Token: 0x06006B62 RID: 27490 RVA: 0x0023443E File Offset: 0x0023263E
		private void OnWearableStateChanged()
		{
			if (this.value)
			{
				UnityEvent onWearableStateTrue = this.OnWearableStateTrue;
				if (onWearableStateTrue == null)
				{
					return;
				}
				onWearableStateTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onWearableStateFalse = this.OnWearableStateFalse;
				if (onWearableStateFalse == null)
				{
					return;
				}
				onWearableStateFalse.Invoke();
				return;
			}
		}

		// Token: 0x06006B63 RID: 27491 RVA: 0x00234469 File Offset: 0x00232669
		private void OnLeftStateChanged()
		{
			if (this.leftHandValue)
			{
				UnityEvent onLeftWearableStateTrue = this.OnLeftWearableStateTrue;
				if (onLeftWearableStateTrue == null)
				{
					return;
				}
				onLeftWearableStateTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onLeftWearableStateFalse = this.OnLeftWearableStateFalse;
				if (onLeftWearableStateFalse == null)
				{
					return;
				}
				onLeftWearableStateFalse.Invoke();
				return;
			}
		}

		// Token: 0x06006B64 RID: 27492 RVA: 0x00234494 File Offset: 0x00232694
		private void OnRightStateChanged()
		{
			if (this.rightHandValue)
			{
				UnityEvent onRightWearableStateTrue = this.OnRightWearableStateTrue;
				if (onRightWearableStateTrue == null)
				{
					return;
				}
				onRightWearableStateTrue.Invoke();
				return;
			}
			else
			{
				UnityEvent onRightWearableStateFalse = this.OnRightWearableStateFalse;
				if (onRightWearableStateFalse == null)
				{
					return;
				}
				onRightWearableStateFalse.Invoke();
				return;
			}
		}

		// Token: 0x17000A10 RID: 2576
		// (get) Token: 0x06006B65 RID: 27493 RVA: 0x002344BF File Offset: 0x002326BF
		// (set) Token: 0x06006B66 RID: 27494 RVA: 0x002344C7 File Offset: 0x002326C7
		public bool IsSpawned { get; set; }

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x06006B67 RID: 27495 RVA: 0x002344D0 File Offset: 0x002326D0
		// (set) Token: 0x06006B68 RID: 27496 RVA: 0x002344D8 File Offset: 0x002326D8
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06006B69 RID: 27497 RVA: 0x002344E4 File Offset: 0x002326E4
		public void OnSpawn(VRRig rig)
		{
			if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.CosmeticSelectedSide == ECosmeticSelectSide.Both)
			{
				GTDev.LogWarning<string>(string.Format("NetworkedWearable: Cosmetic {0} with category {1} has select side Both, assuming left side!", base.gameObject.name, this.assignedSlot), null);
			}
			if (!NetworkedWearable.IsCategoryValid(this.assignedSlot))
			{
				GTDev.LogError<string>(string.Format("NetworkedWearable: Cosmetic {0} spawned with invalid category {1}!", base.gameObject.name, this.assignedSlot), null);
			}
			this.myRig = rig;
			this.isLocal = rig.isLocal;
			this.wearableSlot = this.CosmeticCategoryToWearableSlot(this.assignedSlot, this.CosmeticSelectedSide != ECosmeticSelectSide.Right);
			Debug.Log(string.Format("Networked Wearable {0} Select Side {1} slot {2}", base.gameObject.name, this.CosmeticSelectedSide, this.wearableSlot));
		}

		// Token: 0x06006B6A RID: 27498 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDespawn()
		{
		}

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x06006B6B RID: 27499 RVA: 0x002345BC File Offset: 0x002327BC
		// (set) Token: 0x06006B6C RID: 27500 RVA: 0x002345C4 File Offset: 0x002327C4
		public bool TickRunning { get; set; }

		// Token: 0x06006B6D RID: 27501 RVA: 0x002345D0 File Offset: 0x002327D0
		public void Tick()
		{
			if ((!this.isLocal || this.listenForChangesLocal) && this.IsSpawned)
			{
				if (this.assignedSlot == CosmeticsController.CosmeticCategory.Paw && this.isTwoHanded)
				{
					bool flag = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int)this.leftSlot);
					if (this.leftHandValue != flag)
					{
						this.leftHandValue = flag;
						this.OnLeftStateChanged();
					}
					flag = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int)this.rightSlot);
					if (this.rightHandValue != flag)
					{
						this.rightHandValue = flag;
						this.OnRightStateChanged();
						return;
					}
				}
				else
				{
					bool flag2 = GTBitOps.ReadBit(this.myRig.WearablePackedStates, (int)this.wearableSlot);
					if (this.value != flag2)
					{
						this.value = flag2;
						this.OnWearableStateChanged();
					}
				}
			}
		}

		// Token: 0x06006B6E RID: 27502 RVA: 0x00234694 File Offset: 0x00232894
		public static bool IsCategoryValid(CosmeticsController.CosmeticCategory category)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
			case CosmeticsController.CosmeticCategory.Badge:
			case CosmeticsController.CosmeticCategory.Face:
			case CosmeticsController.CosmeticCategory.Paw:
			case CosmeticsController.CosmeticCategory.Fur:
			case CosmeticsController.CosmeticCategory.Shirt:
			case CosmeticsController.CosmeticCategory.Pants:
				return true;
			}
			return false;
		}

		// Token: 0x06006B6F RID: 27503 RVA: 0x002346CC File Offset: 0x002328CC
		private VRRig.WearablePackedStateSlots CosmeticCategoryToWearableSlot(CosmeticsController.CosmeticCategory category, bool isLeft)
		{
			switch (category)
			{
			case CosmeticsController.CosmeticCategory.Hat:
				return VRRig.WearablePackedStateSlots.Hat;
			case CosmeticsController.CosmeticCategory.Badge:
				return VRRig.WearablePackedStateSlots.Badge;
			case CosmeticsController.CosmeticCategory.Face:
				return VRRig.WearablePackedStateSlots.Face;
			case CosmeticsController.CosmeticCategory.Paw:
				if (!isLeft)
				{
					return VRRig.WearablePackedStateSlots.RightHand;
				}
				return VRRig.WearablePackedStateSlots.LeftHand;
			case CosmeticsController.CosmeticCategory.Fur:
				return VRRig.WearablePackedStateSlots.Fur;
			case CosmeticsController.CosmeticCategory.Shirt:
				return VRRig.WearablePackedStateSlots.Shirt;
			case CosmeticsController.CosmeticCategory.Pants:
				return VRRig.WearablePackedStateSlots.Pants1;
			}
			GTDev.LogWarning<string>(string.Format("NetworkedWearable: {0} item cannot set wearable state", category), null);
			return VRRig.WearablePackedStateSlots.Hat;
		}

		// Token: 0x04007BD2 RID: 31698
		[Tooltip("Whether the wearable state is toggled on by default.")]
		[SerializeField]
		private bool startTrue;

		// Token: 0x04007BD3 RID: 31699
		[Tooltip("This is to determine what bit to change in VRRig.WearablesPackedStates.")]
		[SerializeField]
		private CosmeticsController.CosmeticCategory assignedSlot;

		// Token: 0x04007BD4 RID: 31700
		[FormerlySerializedAs("IsTwoHanded")]
		[SerializeField]
		private bool isTwoHanded;

		// Token: 0x04007BD5 RID: 31701
		private const string listenInfo = "listenForChangesLocal should be false in most cases";

		// Token: 0x04007BD6 RID: 31702
		private const string listenDetails = "listenForChangesLocal should be false in most cases\nIf you have a first person part and a local rig part that both need to react to a state change\ncall the Toggle/Set functions to change the state from one prefab and check \nlistenForChangesLocal on the other prefab ";

		// Token: 0x04007BD7 RID: 31703
		[SerializeField]
		private bool listenForChangesLocal;

		// Token: 0x04007BD8 RID: 31704
		private VRRig.WearablePackedStateSlots wearableSlot;

		// Token: 0x04007BD9 RID: 31705
		private VRRig.WearablePackedStateSlots leftSlot = VRRig.WearablePackedStateSlots.LeftHand;

		// Token: 0x04007BDA RID: 31706
		private VRRig.WearablePackedStateSlots rightSlot = VRRig.WearablePackedStateSlots.RightHand;

		// Token: 0x04007BDB RID: 31707
		private VRRig myRig;

		// Token: 0x04007BDC RID: 31708
		private bool isLocal;

		// Token: 0x04007BDD RID: 31709
		private bool value;

		// Token: 0x04007BDE RID: 31710
		private bool leftHandValue;

		// Token: 0x04007BDF RID: 31711
		private bool rightHandValue;

		// Token: 0x04007BE0 RID: 31712
		[SerializeField]
		protected UnityEvent OnWearableStateTrue;

		// Token: 0x04007BE1 RID: 31713
		[SerializeField]
		protected UnityEvent OnWearableStateFalse;

		// Token: 0x04007BE2 RID: 31714
		[SerializeField]
		protected UnityEvent OnLeftWearableStateTrue;

		// Token: 0x04007BE3 RID: 31715
		[SerializeField]
		protected UnityEvent OnLeftWearableStateFalse;

		// Token: 0x04007BE4 RID: 31716
		[SerializeField]
		protected UnityEvent OnRightWearableStateTrue;

		// Token: 0x04007BE5 RID: 31717
		[SerializeField]
		protected UnityEvent OnRightWearableStateFalse;
	}
}
