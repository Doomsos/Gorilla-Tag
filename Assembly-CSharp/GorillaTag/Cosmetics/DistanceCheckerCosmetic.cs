using System;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010E4 RID: 4324
	public class DistanceCheckerCosmetic : MonoBehaviour, ISpawnable
	{
		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06006C75 RID: 27765 RVA: 0x00239384 File Offset: 0x00237584
		// (set) Token: 0x06006C76 RID: 27766 RVA: 0x0023938C File Offset: 0x0023758C
		public bool IsSpawned { get; set; }

		// Token: 0x17000A4B RID: 2635
		// (get) Token: 0x06006C77 RID: 27767 RVA: 0x00239395 File Offset: 0x00237595
		// (set) Token: 0x06006C78 RID: 27768 RVA: 0x0023939D File Offset: 0x0023759D
		public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06006C79 RID: 27769 RVA: 0x002393A6 File Offset: 0x002375A6
		public void OnSpawn(VRRig rig)
		{
			this.myRig = rig;
		}

		// Token: 0x06006C7A RID: 27770 RVA: 0x00002789 File Offset: 0x00000989
		public void OnDespawn()
		{
		}

		// Token: 0x06006C7B RID: 27771 RVA: 0x002393AF File Offset: 0x002375AF
		private void OnEnable()
		{
			this.currentState = DistanceCheckerCosmetic.State.None;
			this.transferableObject = base.GetComponentInParent<TransferrableObject>();
			if (this.transferableObject != null)
			{
				this.ownerRig = this.transferableObject.ownerRig;
			}
			this.ResetClosestPlayer();
		}

		// Token: 0x06006C7C RID: 27772 RVA: 0x002393E9 File Offset: 0x002375E9
		private void Update()
		{
			this.UpdateDistance();
		}

		// Token: 0x06006C7D RID: 27773 RVA: 0x002393F1 File Offset: 0x002375F1
		private bool IsBelowThreshold(Vector3 distance)
		{
			return distance.IsShorterThan(this.distanceThreshold);
		}

		// Token: 0x06006C7E RID: 27774 RVA: 0x00239404 File Offset: 0x00237604
		private bool IsAboveThreshold(Vector3 distance)
		{
			return distance.IsLongerThan(this.distanceThreshold);
		}

		// Token: 0x06006C7F RID: 27775 RVA: 0x00239418 File Offset: 0x00237618
		private void UpdateClosestPlayer(bool others = false)
		{
			if (!PhotonNetwork.InRoom)
			{
				this.ResetClosestPlayer();
				return;
			}
			VRRig vrrig = this.currentClosestPlayer;
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
			foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
			{
				if (!others || !(this.ownerRig != null) || !(vrrig2 == this.ownerRig))
				{
					Vector3 distance = vrrig2.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(distance) && distance.sqrMagnitude < this.closestDistance.sqrMagnitude)
					{
						this.closestDistance = distance;
						this.currentClosestPlayer = vrrig2;
					}
				}
			}
			if (this.currentClosestPlayer != null && this.currentClosestPlayer != vrrig)
			{
				UnityEvent<VRRig, float> unityEvent = this.onClosestPlayerBelowThresholdChanged;
				if (unityEvent == null)
				{
					return;
				}
				unityEvent.Invoke(this.currentClosestPlayer, this.closestDistance.magnitude);
			}
		}

		// Token: 0x06006C80 RID: 27776 RVA: 0x00239534 File Offset: 0x00237734
		private void ResetClosestPlayer()
		{
			this.closestDistance = Vector3.positiveInfinity;
			this.currentClosestPlayer = null;
		}

		// Token: 0x06006C81 RID: 27777 RVA: 0x00239548 File Offset: 0x00237748
		private void UpdateDistance()
		{
			bool flag = true;
			switch (this.distanceTo)
			{
			case DistanceCheckerCosmetic.DistanceCondition.Owner:
			{
				Vector3 distance = this.myRig.transform.position - this.distanceFrom.position;
				if (this.IsBelowThreshold(distance))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
					return;
				}
				if (this.IsAboveThreshold(distance))
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
				}
				break;
			}
			case DistanceCheckerCosmetic.DistanceCondition.Others:
				this.UpdateClosestPlayer(true);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!(this.ownerRig != null) || !(vrrig == this.ownerRig))
					{
						Vector3 distance2 = vrrig.transform.position - this.distanceFrom.position;
						if (this.IsBelowThreshold(distance2))
						{
							this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
							flag = false;
						}
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			case DistanceCheckerCosmetic.DistanceCondition.Everyone:
				this.UpdateClosestPlayer(false);
				if (!PhotonNetwork.InRoom)
				{
					return;
				}
				foreach (VRRig vrrig2 in GorillaParent.instance.vrrigs)
				{
					Vector3 distance3 = vrrig2.transform.position - this.distanceFrom.position;
					if (this.IsBelowThreshold(distance3))
					{
						this.UpdateState(DistanceCheckerCosmetic.State.BelowThreshold);
						flag = false;
					}
				}
				if (flag)
				{
					this.UpdateState(DistanceCheckerCosmetic.State.AboveThreshold);
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06006C82 RID: 27778 RVA: 0x002396F0 File Offset: 0x002378F0
		private void UpdateState(DistanceCheckerCosmetic.State newState)
		{
			if (this.currentState == newState)
			{
				return;
			}
			this.currentState = newState;
			if (this.currentState != DistanceCheckerCosmetic.State.AboveThreshold)
			{
				if (this.currentState == DistanceCheckerCosmetic.State.BelowThreshold)
				{
					UnityEvent unityEvent = this.onOneIsBelowThreshold;
					if (unityEvent == null)
					{
						return;
					}
					unityEvent.Invoke();
				}
				return;
			}
			UnityEvent unityEvent2 = this.onAllAreAboveThreshold;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke();
		}

		// Token: 0x04007D10 RID: 32016
		[SerializeField]
		private Transform distanceFrom;

		// Token: 0x04007D11 RID: 32017
		[SerializeField]
		private DistanceCheckerCosmetic.DistanceCondition distanceTo;

		// Token: 0x04007D12 RID: 32018
		[Tooltip("Receive events when above or below this distance")]
		public float distanceThreshold;

		// Token: 0x04007D13 RID: 32019
		public UnityEvent onOneIsBelowThreshold;

		// Token: 0x04007D14 RID: 32020
		public UnityEvent onAllAreAboveThreshold;

		// Token: 0x04007D15 RID: 32021
		public UnityEvent<VRRig, float> onClosestPlayerBelowThresholdChanged;

		// Token: 0x04007D16 RID: 32022
		private VRRig myRig;

		// Token: 0x04007D17 RID: 32023
		private DistanceCheckerCosmetic.State currentState;

		// Token: 0x04007D18 RID: 32024
		private Vector3 closestDistance;

		// Token: 0x04007D19 RID: 32025
		private VRRig currentClosestPlayer;

		// Token: 0x04007D1A RID: 32026
		private VRRig ownerRig;

		// Token: 0x04007D1B RID: 32027
		private TransferrableObject transferableObject;

		// Token: 0x020010E5 RID: 4325
		private enum State
		{
			// Token: 0x04007D1F RID: 32031
			AboveThreshold,
			// Token: 0x04007D20 RID: 32032
			BelowThreshold,
			// Token: 0x04007D21 RID: 32033
			None
		}

		// Token: 0x020010E6 RID: 4326
		private enum DistanceCondition
		{
			// Token: 0x04007D23 RID: 32035
			None,
			// Token: 0x04007D24 RID: 32036
			Owner,
			// Token: 0x04007D25 RID: 32037
			Others,
			// Token: 0x04007D26 RID: 32038
			Everyone
		}
	}
}
