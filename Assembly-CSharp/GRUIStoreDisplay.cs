using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000749 RID: 1865
public class GRUIStoreDisplay : MonoBehaviour
{
	// Token: 0x06003031 RID: 12337 RVA: 0x00002789 File Offset: 0x00000989
	public void Awake()
	{
	}

	// Token: 0x06003032 RID: 12338 RVA: 0x00107A7D File Offset: 0x00105C7D
	public void OnEnable()
	{
		this.RefreshUI();
	}

	// Token: 0x06003033 RID: 12339 RVA: 0x00002789 File Offset: 0x00000989
	public void OnDisable()
	{
	}

	// Token: 0x06003034 RID: 12340 RVA: 0x00107A85 File Offset: 0x00105C85
	public void Setup(int playerActorId, GhostReactor reactor)
	{
		this.reactor = reactor;
		this.toolProgressionManager = reactor.toolProgression;
		this.playerActorId = playerActorId;
		this.RefreshUI();
		this.toolProgressionManager.OnProgressionUpdated += new Action(this.onProgressionUpdated);
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x00107A7D File Offset: 0x00105C7D
	private void onProgressionUpdated()
	{
		this.RefreshUI();
	}

	// Token: 0x06003036 RID: 12342 RVA: 0x00107ABE File Offset: 0x00105CBE
	private void RefreshUI()
	{
		this.RefreshItemInfo();
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x00107AC8 File Offset: 0x00105CC8
	public void OnBuy(int playerActorNumber)
	{
		if (playerActorNumber != this.playerActorId)
		{
			return;
		}
		if (GRPlayer.Get(this.playerActorId) == null)
		{
			return;
		}
		if (!this.CanLocalPlayerPurchaseItem())
		{
			if (this.scanner != null)
			{
				UnityEvent onFailed = this.scanner.onFailed;
				if (onFailed == null)
				{
					return;
				}
				onFailed.Invoke();
			}
			return;
		}
		if (this.scanner != null)
		{
			UnityEvent onSucceeded = this.scanner.onSucceeded;
			if (onSucceeded != null)
			{
				onSucceeded.Invoke();
			}
		}
		bool flag;
		if (!this.reactor.grManager.DebugIsToolStationHacked() && (!this.toolProgressionManager.IsPartUnlocked(this.slot.PurchaseID, out flag) || !flag))
		{
			if (this.slot.drillUpgradeLevel == ProgressionManager.DrillUpgradeLevel.Base)
			{
				if (ProgressionManager.Instance.GetShinyRocksTotal() >= 2500)
				{
					ProgressionManager.Instance.PurchaseDrillUpgrade(ProgressionManager.DrillUpgradeLevel.Base);
					return;
				}
			}
			else
			{
				this.toolProgressionManager.AttemptToUnlockPart(this.slot.PurchaseID);
			}
		}
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x00107BB3 File Offset: 0x00105DB3
	private bool CanLocalPlayerPurchaseItem()
	{
		return this.slot.canAfford;
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x00107BC0 File Offset: 0x00105DC0
	public void RefreshItemInfo()
	{
		bool flag = true;
		if (this.toolProgressionManager != null)
		{
			GRToolProgressionManager.ToolProgressionMetaData partMetadata = this.toolProgressionManager.GetPartMetadata(this.slot.PurchaseID);
			if (partMetadata == null)
			{
				this.slot.Name.text = "ERROR";
				return;
			}
			string text = "ERROR";
			string text2 = "";
			Color white = Color.white;
			bool flag2 = true;
			int num = 10000;
			int num2;
			this.toolProgressionManager.GetPlayerShiftCredit(out num2);
			int numberOfResearchPoints = this.toolProgressionManager.GetNumberOfResearchPoints();
			this.slot.canAfford = false;
			this.slot.purchaseText = "LOCKED";
			if (this.slot.Description != null)
			{
				this.slot.Description.text = partMetadata.description;
			}
			bool flag3;
			if (this.toolProgressionManager.IsPartUnlocked(this.slot.PurchaseID, out flag3))
			{
				if (flag3)
				{
					if (this.slot.drillUpgradeLevel != ProgressionManager.DrillUpgradeLevel.None)
					{
						this.slot.Price.color = this.colorCanBuyCredits;
						this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
						this.slot.canAfford = true;
						this.slot.purchaseText = "Purchased";
						text = this.slot.purchaseText;
						this.slot.Price.text = text;
						return;
					}
					if (this.toolProgressionManager.GetShiftCreditCost(this.slot.PurchaseID, out num))
					{
						text = string.Format("⑭ {0}", num);
					}
					bool flag4 = num2 >= num;
					this.slot.Name.text = partMetadata.name;
					this.slot.Name.color = (flag ? this.colorSelectedItem : this.colorUnselectedItem);
					this.slot.Price.text = text;
					this.slot.Price.color = (flag4 ? this.colorCanBuyCredits : this.colorCantBuy);
					this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
					this.slot.canAfford = flag4;
					if (flag4)
					{
						this.slot.purchaseText = string.Format("BUY FOR\n⑭ {0}", num);
						return;
					}
					this.slot.purchaseText = string.Format("NEED\n⑭ {0}", num);
					return;
				}
				else
				{
					this.slot.Name.text = partMetadata.name;
					this.slot.Name.color = (flag ? this.colorUnresearchedItem : this.colorUnselectedUnresearchedItem);
					flag2 = true;
					GRToolProgressionTree.EmployeeLevelRequirement employeeLevelRequirement;
					if (this.toolProgressionManager.GetPartUnlockEmployeeRequiredLevel(this.slot.PurchaseID, out employeeLevelRequirement) && this.toolProgressionManager.GetCurrentEmployeeLevel() < employeeLevelRequirement)
					{
						this.toolProgressionManager.GetEmployeeLevelDisplayName(employeeLevelRequirement);
						text2 += string.Format("⑱ {0}\n", employeeLevelRequirement);
						flag2 = false;
					}
					this.cachedRequiredPartsList.Clear();
					if (this.toolProgressionManager.GetPartUnlockRequiredParentParts(this.slot.PurchaseID, out this.cachedRequiredPartsList))
					{
						foreach (GRToolProgressionManager.ToolParts part in this.cachedRequiredPartsList)
						{
							bool flag5 = false;
							GRToolProgressionManager.ToolProgressionMetaData partMetadata2 = this.toolProgressionManager.GetPartMetadata(part);
							if (partMetadata2 == null)
							{
								text2 += "⑱ ERROR\n";
								flag2 = false;
							}
							else if (!this.toolProgressionManager.IsPartUnlocked(part, out flag5) || !flag5)
							{
								text2 = text2 + "⑱ " + partMetadata2.name + "\n";
								flag2 = false;
							}
						}
					}
					if (!flag2)
					{
						this.slot.Price.text = text2;
						this.slot.Price.color = this.colorCantBuy;
						this.slot.Price.fontSize = ((text2.Length <= 8) ? 2.25f : 1.6f);
						this.slot.canAfford = false;
						this.slot.purchaseText = "LOCKED";
						return;
					}
					if (this.slot.drillUpgradeLevel == ProgressionManager.DrillUpgradeLevel.Base)
					{
						this.slot.Price.color = this.colorCanBuyCredits;
						this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
						this.slot.canAfford = true;
						this.slot.purchaseText = string.Format("Cost {0}⑯ Shiny Rocks", 2500);
						text = this.slot.purchaseText;
						this.slot.Price.text = text;
						return;
					}
					if (this.toolProgressionManager.GetPartUnlockJuiceCost(this.slot.PurchaseID, out num))
					{
						text = string.Format("⑮ {0}", num);
					}
					bool flag4 = numberOfResearchPoints >= num;
					this.slot.Price.text = text;
					this.slot.Price.color = (flag4 ? this.colorCanBuyJuice : this.colorCantBuy);
					this.slot.Price.fontSize = ((text.Length <= 8) ? 2.25f : 1.6f);
					this.slot.canAfford = flag4;
					if (flag4)
					{
						this.slot.purchaseText = string.Format("RESEARCH\n⑮ {0}", num);
						return;
					}
					this.slot.purchaseText = string.Format("NEED\n⑮ {0}", num);
				}
			}
		}
	}

	// Token: 0x04003F2F RID: 16175
	public IDCardScanner scanner;

	// Token: 0x04003F30 RID: 16176
	public GRUIStoreDisplay.GRPurchaseSlot slot;

	// Token: 0x04003F31 RID: 16177
	private GhostReactor reactor;

	// Token: 0x04003F32 RID: 16178
	private GRToolProgressionManager toolProgressionManager;

	// Token: 0x04003F33 RID: 16179
	private int playerActorId;

	// Token: 0x04003F34 RID: 16180
	private Color colorPurchaseButtonCanAfford = GRToolUpgradePurchaseStationFull.ColorFromRGB32(0, 0, 0);

	// Token: 0x04003F35 RID: 16181
	private Color colorCanBuyCredits = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 229, 37);

	// Token: 0x04003F36 RID: 16182
	private Color colorCanBuyJuice = GRToolUpgradePurchaseStationFull.ColorFromRGB32(232, 65, 255);

	// Token: 0x04003F37 RID: 16183
	private Color colorCantBuy = GRToolUpgradePurchaseStationFull.ColorFromRGB32(140, 38, 38);

	// Token: 0x04003F38 RID: 16184
	private Color colorSelectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(251, 240, 229);

	// Token: 0x04003F39 RID: 16185
	private Color colorUnselectedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(147, 145, 140);

	// Token: 0x04003F3A RID: 16186
	private Color colorUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(230, 19, 17);

	// Token: 0x04003F3B RID: 16187
	private Color colorUnselectedUnresearchedItem = GRToolUpgradePurchaseStationFull.ColorFromRGB32(133, 11, 10);

	// Token: 0x04003F3C RID: 16188
	private List<GRToolProgressionManager.ToolParts> cachedRequiredPartsList = new List<GRToolProgressionManager.ToolParts>(5);

	// Token: 0x0200074A RID: 1866
	[Serializable]
	public class GRPurchaseSlot
	{
		// Token: 0x04003F3D RID: 16189
		public TMP_Text Name;

		// Token: 0x04003F3E RID: 16190
		public TMP_Text Price;

		// Token: 0x04003F3F RID: 16191
		public TMP_Text Description;

		// Token: 0x04003F40 RID: 16192
		public GRToolProgressionManager.ToolParts PurchaseID;

		// Token: 0x04003F41 RID: 16193
		[NonSerialized]
		public Material overrideMaterial;

		// Token: 0x04003F42 RID: 16194
		[NonSerialized]
		public bool canAfford;

		// Token: 0x04003F43 RID: 16195
		[NonSerialized]
		public string purchaseText = "";

		// Token: 0x04003F44 RID: 16196
		public ProgressionManager.DrillUpgradeLevel drillUpgradeLevel;
	}
}
