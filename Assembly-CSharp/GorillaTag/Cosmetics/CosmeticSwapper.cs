using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020010E1 RID: 4321
	public class CosmeticSwapper : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000A48 RID: 2632
		// (get) Token: 0x06006C60 RID: 27744 RVA: 0x00238DDF File Offset: 0x00236FDF
		private int CosmeticStepIndex
		{
			get
			{
				return this.newSwappedCosmetics.Count;
			}
		}

		// Token: 0x06006C61 RID: 27745 RVA: 0x00238DEC File Offset: 0x00236FEC
		private void Awake()
		{
			this.controller = CosmeticsController.instance;
		}

		// Token: 0x06006C62 RID: 27746 RVA: 0x00238DFB File Offset: 0x00236FFB
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticsGlobal(this.cosmeticIDs);
		}

		// Token: 0x06006C63 RID: 27747 RVA: 0x00238E0E File Offset: 0x0023700E
		private void OnDisable()
		{
			PlayerCosmeticsSystem.LockTemporaryCosmeticsGlobal(this.cosmeticIDs);
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06006C64 RID: 27748 RVA: 0x00238E21 File Offset: 0x00237021
		public void SwapInCosmetic(VRRig vrRig)
		{
			this.TriggerSwap(vrRig);
		}

		// Token: 0x06006C65 RID: 27749 RVA: 0x00238E2A File Offset: 0x0023702A
		private CosmeticSwapper.SwapMode GetCurrentMode()
		{
			return this.swapMode;
		}

		// Token: 0x06006C66 RID: 27750 RVA: 0x00238E32 File Offset: 0x00237032
		private bool ShouldHoldFinalStep()
		{
			return this.holdFinalStep;
		}

		// Token: 0x06006C67 RID: 27751 RVA: 0x00238E3A File Offset: 0x0023703A
		public int GetCurrentStepIndex(VRRig rig)
		{
			if (rig == null)
			{
				return 0;
			}
			return this.CosmeticStepIndex;
		}

		// Token: 0x06006C68 RID: 27752 RVA: 0x00238E4D File Offset: 0x0023704D
		public int GetNumberOfSteps()
		{
			return this.cosmeticIDs.Count;
		}

		// Token: 0x06006C69 RID: 27753 RVA: 0x00238E5C File Offset: 0x0023705C
		private void TriggerSwap(VRRig rig)
		{
			if (GorillaGameManager.instance != null && this.gameModeExclusion.Contains(GorillaGameManager.instance.GameType()))
			{
				return;
			}
			if (rig == null || this.controller == null || this.cosmeticIDs.Count == 0)
			{
				return;
			}
			if (rig != GorillaTagger.Instance.offlineVRRig)
			{
				return;
			}
			if (this.swapMode == CosmeticSwapper.SwapMode.AllAtOnce)
			{
				foreach (string nameOrId in this.cosmeticIDs)
				{
					CosmeticSwapper.CosmeticState? cosmeticState = this.SwapInCosmeticWithReturn(nameOrId, rig);
					if (cosmeticState != null)
					{
						this.AddNewSwappedCosmetic(cosmeticState.Value);
					}
				}
				return;
			}
			int cosmeticStepIndex = this.CosmeticStepIndex;
			if (cosmeticStepIndex < 0 || cosmeticStepIndex >= this.cosmeticIDs.Count)
			{
				return;
			}
			string nameOrId2 = this.cosmeticIDs[cosmeticStepIndex];
			CosmeticSwapper.CosmeticState? cosmeticState2 = this.SwapInCosmeticWithReturn(nameOrId2, rig);
			if (cosmeticState2 != null)
			{
				this.AddNewSwappedCosmetic(cosmeticState2.Value);
				if (cosmeticStepIndex == this.cosmeticIDs.Count - 1)
				{
					if (this.holdFinalStep)
					{
						this.MarkFinalCosmeticStep();
					}
					if (this.OnSwappingSequenceCompleted != null)
					{
						this.OnSwappingSequenceCompleted.Invoke(rig);
						return;
					}
				}
				else
				{
					this.UnmarkFinalCosmeticStep();
				}
			}
		}

		// Token: 0x06006C6A RID: 27754 RVA: 0x00238FB0 File Offset: 0x002371B0
		private CosmeticSwapper.CosmeticState? SwapInCosmeticWithReturn(string nameOrId, VRRig rig)
		{
			if (this.controller == null)
			{
				return default(CosmeticSwapper.CosmeticState?);
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(nameOrId);
			if (cosmeticItem.isNullItem)
			{
				Debug.LogWarning("Cosmetic not found: " + nameOrId);
				return default(CosmeticSwapper.CosmeticState?);
			}
			bool isLeftHand;
			CosmeticsController.CosmeticSlots cosmeticSlot = this.GetCosmeticSlot(cosmeticItem, out isLeftHand);
			if (cosmeticSlot == CosmeticsController.CosmeticSlots.Count)
			{
				Debug.LogWarning("Could not determine slot for: " + cosmeticItem.displayName);
				return default(CosmeticSwapper.CosmeticState?);
			}
			CosmeticsController.CosmeticItem replacedItem = this.controller.currentWornSet.items[(int)cosmeticSlot];
			this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, cosmeticItem, isLeftHand, false);
			this.controller.UpdateWornCosmetics(true);
			return new CosmeticSwapper.CosmeticState?(new CosmeticSwapper.CosmeticState
			{
				cosmeticId = nameOrId,
				replacedItem = replacedItem,
				slot = cosmeticSlot,
				isLeftHand = isLeftHand
			});
		}

		// Token: 0x06006C6B RID: 27755 RVA: 0x0023909C File Offset: 0x0023729C
		private void RestorePreviousCosmetic(CosmeticSwapper.CosmeticState state)
		{
			if (this.controller == null)
			{
				return;
			}
			CosmeticsController.CosmeticItem cosmeticItem = this.FindItem(state.cosmeticId);
			if (cosmeticItem.isNullItem)
			{
				return;
			}
			this.controller.RemoveCosmeticItemFromSet(this.controller.tempUnlockedSet, cosmeticItem.displayName, false);
			if (!state.replacedItem.isNullItem)
			{
				this.controller.ApplyCosmeticItemToSet(this.controller.tempUnlockedSet, state.replacedItem, state.isLeftHand, false);
			}
			this.controller.UpdateWornCosmetics(true);
		}

		// Token: 0x06006C6C RID: 27756 RVA: 0x00239128 File Offset: 0x00237328
		private CosmeticsController.CosmeticItem FindItem(string nameOrId)
		{
			CosmeticsController.CosmeticItem result;
			if (this.controller.allCosmeticsDict.TryGetValue(nameOrId, ref result))
			{
				return result;
			}
			string itemID;
			if (this.controller.allCosmeticsItemIDsfromDisplayNamesDict.TryGetValue(nameOrId, ref itemID))
			{
				return this.controller.GetItemFromDict(itemID);
			}
			return this.controller.nullItem;
		}

		// Token: 0x06006C6D RID: 27757 RVA: 0x0023917C File Offset: 0x0023737C
		private CosmeticsController.CosmeticSlots GetCosmeticSlot(CosmeticsController.CosmeticItem item, out bool isLeftHand)
		{
			isLeftHand = false;
			if (!item.isHoldable)
			{
				return CosmeticsController.CategoryToNonTransferrableSlot(item.itemCategory);
			}
			CosmeticsController.CosmeticSet currentWornSet = this.controller.currentWornSet;
			CosmeticsController.CosmeticItem cosmeticItem = currentWornSet.items[7];
			CosmeticsController.CosmeticItem cosmeticItem2 = currentWornSet.items[8];
			if (cosmeticItem.isNullItem || (!cosmeticItem2.isNullItem && item.itemName == cosmeticItem.itemName))
			{
				isLeftHand = true;
			}
			if (!isLeftHand)
			{
				return CosmeticsController.CosmeticSlots.HandRight;
			}
			return CosmeticsController.CosmeticSlots.HandLeft;
		}

		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06006C6E RID: 27758 RVA: 0x002391F1 File Offset: 0x002373F1
		// (set) Token: 0x06006C6F RID: 27759 RVA: 0x002391F9 File Offset: 0x002373F9
		public bool TickRunning { get; set; }

		// Token: 0x06006C70 RID: 27760 RVA: 0x00239204 File Offset: 0x00237404
		public void Tick()
		{
			if (this.newSwappedCosmetics.Count > 0)
			{
				if (this.GetCurrentMode() == CosmeticSwapper.SwapMode.StepByStep)
				{
					if (this.isAtFinalCosmeticStep && this.ShouldHoldFinalStep())
					{
						if (Time.time - this.lastCosmeticSwapTime <= this.stepTimeout)
						{
							return;
						}
						this.isAtFinalCosmeticStep = false;
					}
					if (Time.time - this.lastCosmeticSwapTime > this.stepTimeout)
					{
						while (this.newSwappedCosmetics.Count > 0)
						{
							CosmeticSwapper.CosmeticState state = this.newSwappedCosmetics.Pop();
							this.RestorePreviousCosmetic(state);
						}
						this.isAtFinalCosmeticStep = false;
						this.lastCosmeticSwapTime = float.PositiveInfinity;
						return;
					}
				}
				else if (this.GetCurrentMode() == CosmeticSwapper.SwapMode.AllAtOnce && Time.time - this.lastCosmeticSwapTime > this.stepTimeout)
				{
					while (this.newSwappedCosmetics.Count > 0)
					{
						CosmeticSwapper.CosmeticState state2 = this.newSwappedCosmetics.Pop();
						this.RestorePreviousCosmetic(state2);
					}
					this.lastCosmeticSwapTime = float.PositiveInfinity;
					this.isAtFinalCosmeticStep = false;
				}
			}
		}

		// Token: 0x06006C71 RID: 27761 RVA: 0x002392F5 File Offset: 0x002374F5
		private void AddNewSwappedCosmetic(CosmeticSwapper.CosmeticState state)
		{
			this.newSwappedCosmetics.Push(state);
			this.lastCosmeticSwapTime = Time.time;
		}

		// Token: 0x06006C72 RID: 27762 RVA: 0x0023930E File Offset: 0x0023750E
		private void MarkFinalCosmeticStep()
		{
			this.isAtFinalCosmeticStep = true;
			this.lastCosmeticSwapTime = Time.time;
		}

		// Token: 0x06006C73 RID: 27763 RVA: 0x00239322 File Offset: 0x00237522
		private void UnmarkFinalCosmeticStep()
		{
			this.isAtFinalCosmeticStep = false;
		}

		// Token: 0x04007CFE RID: 31998
		[SerializeField]
		private List<string> cosmeticIDs = new List<string>();

		// Token: 0x04007CFF RID: 31999
		[SerializeField]
		private CosmeticSwapper.SwapMode swapMode = CosmeticSwapper.SwapMode.StepByStep;

		// Token: 0x04007D00 RID: 32000
		[SerializeField]
		private float stepTimeout = 10f;

		// Token: 0x04007D01 RID: 32001
		[Tooltip("Hold final step as long as the swapper is being called within the timeframe")]
		[SerializeField]
		private bool holdFinalStep = true;

		// Token: 0x04007D02 RID: 32002
		[SerializeField]
		private UnityEvent<VRRig> OnSwappingSequenceCompleted;

		// Token: 0x04007D03 RID: 32003
		[SerializeField]
		private List<GameModeType> gameModeExclusion = new List<GameModeType>();

		// Token: 0x04007D04 RID: 32004
		private CosmeticsController controller;

		// Token: 0x04007D05 RID: 32005
		private Stack<CosmeticSwapper.CosmeticState> newSwappedCosmetics = new Stack<CosmeticSwapper.CosmeticState>();

		// Token: 0x04007D06 RID: 32006
		private float lastCosmeticSwapTime = float.PositiveInfinity;

		// Token: 0x04007D07 RID: 32007
		private bool isAtFinalCosmeticStep;

		// Token: 0x020010E2 RID: 4322
		private enum SwapMode
		{
			// Token: 0x04007D0A RID: 32010
			AllAtOnce,
			// Token: 0x04007D0B RID: 32011
			StepByStep
		}

		// Token: 0x020010E3 RID: 4323
		private struct CosmeticState
		{
			// Token: 0x04007D0C RID: 32012
			public string cosmeticId;

			// Token: 0x04007D0D RID: 32013
			public CosmeticsController.CosmeticItem replacedItem;

			// Token: 0x04007D0E RID: 32014
			public CosmeticsController.CosmeticSlots slot;

			// Token: 0x04007D0F RID: 32015
			public bool isLeftHand;
		}
	}
}
