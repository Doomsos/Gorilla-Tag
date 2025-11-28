using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000723 RID: 1827
public class GRToolProgressionManager : MonoBehaviourTick
{
	// Token: 0x14000050 RID: 80
	// (add) Token: 0x06002EFB RID: 12027 RVA: 0x000FF130 File Offset: 0x000FD330
	// (remove) Token: 0x06002EFC RID: 12028 RVA: 0x000FF168 File Offset: 0x000FD368
	public event Action OnProgressionUpdated;

	// Token: 0x06002EFD RID: 12029 RVA: 0x000FF19D File Offset: 0x000FD39D
	public void SetPendingTreeToProcess()
	{
		this.pendingTreeToProcess = true;
	}

	// Token: 0x06002EFE RID: 12030 RVA: 0x000FF1A6 File Offset: 0x000FD3A6
	public void UpdateInventory()
	{
		this.pendingUpdateInventory = true;
	}

	// Token: 0x06002EFF RID: 12031 RVA: 0x000FF1B0 File Offset: 0x000FD3B0
	public void Init(GhostReactor ghostReactor)
	{
		this.reactor = ghostReactor;
		this.PopulateToolPartMetadata();
		this.PopulateEmployeeLevelMetadata();
		if (this.researchStations != null)
		{
			foreach (GRResearchStation grresearchStation in this.researchStations)
			{
				grresearchStation.Init(this, ghostReactor);
			}
		}
		if (this.toolUpgradeStations != null)
		{
			foreach (GRToolUpgradeStation grtoolUpgradeStation in this.toolUpgradeStations)
			{
				grtoolUpgradeStation.Init(this, ghostReactor);
			}
		}
		this.toolProgressionTree.Init(this.reactor, this);
		ProgressionManager.Instance.OnNodeUnlocked += delegate(string a, string b)
		{
			this.NodeUnlocked();
		};
	}

	// Token: 0x06002F00 RID: 12032 RVA: 0x000FF290 File Offset: 0x000FD490
	private void NodeUnlocked()
	{
		this.toolProgressionTree.RefreshUserInventory();
		this.toolProgressionTree.RefreshProgressionTree();
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x000FF2A8 File Offset: 0x000FD4A8
	public override void Tick()
	{
		if (this.sendUpdate)
		{
			Action onProgressionUpdated = this.OnProgressionUpdated;
			if (onProgressionUpdated != null)
			{
				onProgressionUpdated.Invoke();
			}
			this.sendUpdate = false;
		}
		if (this.pendingTreeToProcess)
		{
			this.toolProgressionTree.RefreshProgressionTree();
			this.pendingTreeToProcess = false;
		}
		if (this.pendingUpdateInventory)
		{
			this.toolProgressionTree.RefreshUserInventory();
			this.pendingUpdateInventory = false;
		}
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x000FF309 File Offset: 0x000FD509
	public void SendMothershipUpdated()
	{
		this.sendUpdate = true;
	}

	// Token: 0x06002F03 RID: 12035 RVA: 0x000FF314 File Offset: 0x000FD514
	public GRToolProgressionManager.ToolProgressionMetaData GetPartMetadata(GRToolProgressionManager.ToolParts part)
	{
		GRToolProgressionManager.ToolProgressionMetaData result;
		this.partMetadata.TryGetValue(part, ref result);
		return result;
	}

	// Token: 0x06002F04 RID: 12036 RVA: 0x000FF334 File Offset: 0x000FD534
	private void PopulateToolPartMetadata()
	{
		this.PopulateClubPartMetadata();
		this.PopulateFlashPartMetadata();
		this.PopulateCollectorPartMetadata();
		this.PopulateLanternPartMetadata();
		this.PopulateShieldGunPartMetadata();
		this.PopulateDirectionalShieldPartMetadata();
		this.PopulateEnergyEfficiencyPartMetadata();
		this.PopulateRevivePartMetadata();
		this.PopulateDockWristPartMetadata();
		this.PopulateDropPodPartMetadata();
		this.PopulateHocketStickMetadata();
	}

	// Token: 0x06002F05 RID: 12037 RVA: 0x000FF384 File Offset: 0x000FD584
	private void PopulateEmployeeLevelMetadata()
	{
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.None] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "None",
			level = 0
		};
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.Intern] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "Intern",
			level = 2
		};
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.PartTime] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "Part Time",
			level = 3
		};
		this.employeeLevelMetadata[GRToolProgressionTree.EmployeeLevelRequirement.FullTime] = new GRToolProgressionManager.EmployeeMetadata
		{
			name = "Full Time",
			level = 4
		};
	}

	// Token: 0x06002F06 RID: 12038 RVA: 0x000FF438 File Offset: 0x000FD638
	private void PopulateClubPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Baton] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Charge Baton",
			description = "50,000 volts of ghost-zapping power",
			annotation = "Impact Power: ❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.BatonDamage1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Lead Core",
			description = "Conductive lead sheath",
			annotation = "Attaches to Charge Baton. Impact Power: ❶❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.BatonDamage2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Osmium Core",
			description = "More mass for more win",
			annotation = "Attaches to Charge Baton. Impact Power: ❶❶❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.BatonDamage3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Electrified Spikes",
			description = "Impales, shocks, and crushes simultaneously",
			annotation = "Attaches to Charge Baton. Impact Power: ❶❶❶❶❶"
		};
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x000FF538 File Offset: 0x000FD738
	private void PopulateFlashPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Flash] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Spectral Flash",
			description = "Makes strong ghosts vulnerable",
			annotation = "Damages ghost armor."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.FlashDamage1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Spectral Lens",
			description = "Safety through momentary paralysis",
			annotation = "Attaches to Spectral Flash. Stuns enemies."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.FlashDamage2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Parabolic Focuser",
			description = "When you want ghosts to feel it",
			annotation = "Attaches to Spectral Flash. Stuns enemies. Disintegrates armor."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.FlashDamage3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Beta Wave Amplifier",
			description = "Exposure with explosive results",
			annotation = "Attaches to Spectral Flash. Stuns enemies. Shatters armor."
		};
	}

	// Token: 0x06002F08 RID: 12040 RVA: 0x000FF638 File Offset: 0x000FD838
	private void PopulateCollectorPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Collector] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 50,
			name = "Collector",
			description = "Every team needs a sucker",
			annotation = "Collects essence and recharges tools"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.CollectorBonus1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Vortex Intake",
			description = "Harvests ambient essence",
			annotation = "Attaches to Collector.  Recharges over time."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.CollectorBonus2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Cyclone Intake",
			description = "Creates a wormhole to a twin universe",
			annotation = "Attaches to Collector. 2x collection bonus."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.CollectorBonus3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Hurricane Intake",
			description = "A Category 5 commitment to teamwork",
			annotation = "Attaches to Collector. 2x collection bonus.  Area recharge."
		};
	}

	// Token: 0x06002F09 RID: 12041 RVA: 0x000FF73C File Offset: 0x000FD93C
	private void PopulateLanternPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Lantern] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 50,
			name = "Lantern",
			description = "Creates the gentle glow of safety",
			annotation = "Illuminates dark areas."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.LanternIntensity1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Kinetic Power",
			description = "Saves batteries to optimize shareholder value",
			annotation = "Attaches to Lantern. Doesn't need recharge."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.LanternIntensity2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Flare Discharge",
			description = "Blaze the trail for your team",
			annotation = "Attaches to Lantern. Drops long-lasting flares."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.LanternIntensity3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Gamma Burster",
			description = "See through walls. Do not aim at important body parts",
			annotation = "Attaches to Lantern. X-ray ghost vision."
		};
	}

	// Token: 0x06002F0A RID: 12042 RVA: 0x000FF840 File Offset: 0x000FDA40
	private void PopulateShieldGunPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGun] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Forcefield Gun",
			description = "Corporate armor for fragile assets",
			annotation = "Gives forcefields."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGunStrength1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Truebright Nozzle",
			description = "Nuclear protection",
			annotation = "Attaches to Forcefield Gun. Increases light."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGunStrength2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Stealth Nozzle",
			description = "Protection they'll never see coming",
			annotation = "Attaches to Forcefield Gun. Gives temporary stealth."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.ShieldGunStrength3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Medic Nozzle",
			description = "Restores productivity through impact therapy",
			annotation = "Attaches to Forcefield Gun. Heals to full."
		};
	}

	// Token: 0x06002F0B RID: 12043 RVA: 0x000FF944 File Offset: 0x000FDB44
	private void PopulateDirectionalShieldPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShield] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Umbrella Shield",
			description = "Protects company property",
			annotation = "Blocks attacks."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShieldSize1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Sling Shield",
			description = "Deflects danger and liability",
			annotation = "Attaches to Umbrella Shield. Reflects projectiles."
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShieldSize2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Harmshadow",
			description = "The best defense is a good offense",
			annotation = "Attaches to Umbrella Shield. Impact Power: ❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DirectionalShieldSize3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Total Defense Array",
			description = "The only safety device with a kill count",
			annotation = "Attaches to Shield. Reflects projectiles. Impact power: ❶❶"
		};
	}

	// Token: 0x06002F0C RID: 12044 RVA: 0x000FFA48 File Offset: 0x000FDC48
	private void PopulateEnergyEfficiencyPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Flash",
			description = "Lead Core Does things!"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Regulator",
			description = "Do more with less",
			annotation = "Attaches to most tools. Efficiency: +❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Optimizer",
			description = "Half the juice, double the morale",
			annotation = "Attaches to most tools. Efficiency: +❶❶"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.EnergyEff3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Peak Power",
			description = "Efficiency that borders on spiritual enlightenment",
			annotation = "Attaches to most tools. Efficiency: +❶❶❶"
		};
	}

	// Token: 0x06002F0D RID: 12045 RVA: 0x000FFB3F File Offset: 0x000FDD3F
	private void PopulateRevivePartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.Revive] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Revive",
			description = "Turns fatal injuries into teachable moments",
			annotation = "Brings defeated employees back to life."
		};
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x000FFB7C File Offset: 0x000FDD7C
	private void PopulateDockWristPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.DockWrist] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 500,
			name = "Wrist Dock",
			description = "Wearable storage that maximizes output per limb",
			annotation = "Extra storage slot"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.StatusWatch] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Ecto Watch",
			description = "Keep track of your location and statistics",
			annotation = "Compass and stat tracker"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.RattyBackpack] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 300,
			name = "Ratty Backpack",
			description = "Torn up backpack we found laying around. Can store one item.",
			annotation = "Worn on the back. It's a backpack."
		};
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x000FFC44 File Offset: 0x000FDE44
	private void PopulateDropPodPartMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodBasic] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 100,
			name = "Starter Pod",
			description = "Descend with confidence in a personal drop pod!\nSupports drops to 5000m\nUpgradable for deeper drops",
			annotation = "DropPodBasic"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodChassis1] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 200,
			name = "Reinforced Pod Chassis",
			description = "Upgrade your drop pod to support drops to 10000m",
			annotation = "DropPodChassis1"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodChassis2] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 600,
			name = "Iron Pod Chassis",
			description = "Upgrade your drop pod to support drops to 15000m",
			annotation = "DropPodChassis2"
		};
		this.partMetadata[GRToolProgressionManager.ToolParts.DropPodChassis3] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 1400,
			name = "Steel Pod Chassis",
			description = "Upgrade your drop pod to support drops to 20000m",
			annotation = "DropPodChassis3"
		};
	}

	// Token: 0x06002F10 RID: 12048 RVA: 0x000FFD46 File Offset: 0x000FDF46
	private void PopulateHocketStickMetadata()
	{
		this.partMetadata[GRToolProgressionManager.ToolParts.HockeyStick] = new GRToolProgressionManager.ToolProgressionMetaData
		{
			shiftCreditCost = 10,
			name = "Hockey Stick",
			description = "A Used Hockey Stick",
			annotation = "Hit things with it?"
		};
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x000FFD83 File Offset: 0x000FDF83
	public int GetRequiredEmployeeLevel(GRToolProgressionTree.EmployeeLevelRequirement employeeLevel)
	{
		return this.employeeLevelMetadata[employeeLevel].level;
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x000FFD96 File Offset: 0x000FDF96
	public string GetEmployeeLevelDisplayName(GRToolProgressionTree.EmployeeLevelRequirement employeeLevel)
	{
		return this.employeeLevelMetadata[employeeLevel].name;
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x000FFDA9 File Offset: 0x000FDFA9
	public int GetNumberOfResearchPoints()
	{
		return this.toolProgressionTree.GetNumberOfResearchPoints();
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x000FFDB6 File Offset: 0x000FDFB6
	public List<GRTool.GRToolType> GetSupportedTools()
	{
		return this.toolProgressionTree.GetSupportedTools();
	}

	// Token: 0x06002F15 RID: 12053 RVA: 0x000FFDC3 File Offset: 0x000FDFC3
	public List<GRToolProgressionTree.GRToolProgressionNode> GetToolUpgrades(GRTool.GRToolType tool)
	{
		return this.toolProgressionTree.GetToolUpgrades(tool);
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x000FFDD4 File Offset: 0x000FDFD4
	public int GetRecycleShiftCredit(GRTool.GRToolType tool)
	{
		if (tool == GRTool.GRToolType.HockeyStick)
		{
			return (int)(10f / (float)this.reactor.vrRigs.Count);
		}
		GRToolProgressionTree.GRToolProgressionNode toolNode = this.toolProgressionTree.GetToolNode(tool);
		if (toolNode != null)
		{
			return (int)((float)(toolNode.partMetadata.shiftCreditCost / 2) / (float)this.reactor.vrRigs.Count);
		}
		return 0;
	}

	// Token: 0x06002F17 RID: 12055 RVA: 0x000FFE32 File Offset: 0x000FE032
	public bool GetShiftCreditCost(GRToolProgressionManager.ToolParts part, out int shiftCreditCost)
	{
		shiftCreditCost = 0;
		if (this.partMetadata.ContainsKey(part))
		{
			shiftCreditCost += this.partMetadata[part].shiftCreditCost;
			return true;
		}
		return false;
	}

	// Token: 0x06002F18 RID: 12056 RVA: 0x000FFE60 File Offset: 0x000FE060
	public void AttemptToUnlockPart(GRToolProgressionManager.ToolParts part)
	{
		bool flag;
		if (!this.IsPartUnlocked(part, out flag))
		{
			return;
		}
		if (!flag)
		{
			int numberOfResearchPoints = this.GetNumberOfResearchPoints();
			int num;
			if (!this.GetPartUnlockJuiceCost(part, out num))
			{
				return;
			}
			if (numberOfResearchPoints < num)
			{
				return;
			}
			GRToolProgressionTree.EmployeeLevelRequirement employeeLevel;
			if (!this.GetPartUnlockEmployeeRequiredLevel(part, out employeeLevel))
			{
				return;
			}
			int requiredEmployeeLevel = this.GetRequiredEmployeeLevel(this.GetCurrentEmployeeLevel());
			int requiredEmployeeLevel2 = this.GetRequiredEmployeeLevel(employeeLevel);
			if (requiredEmployeeLevel < requiredEmployeeLevel2)
			{
				return;
			}
			this.toolProgressionTree.AttemptToUnlockPart(part);
		}
	}

	// Token: 0x06002F19 RID: 12057 RVA: 0x000FFEC8 File Offset: 0x000FE0C8
	public bool IsPartUnlocked(GRToolProgressionManager.ToolParts part, out bool unlocked)
	{
		unlocked = false;
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		unlocked = partNode.unlocked;
		return true;
	}

	// Token: 0x06002F1A RID: 12058 RVA: 0x000FFEF4 File Offset: 0x000FE0F4
	public bool GetPartUnlockEmployeeRequiredLevel(GRToolProgressionManager.ToolParts part, out GRToolProgressionTree.EmployeeLevelRequirement level)
	{
		level = GRToolProgressionTree.EmployeeLevelRequirement.None;
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		level = partNode.requiredEmployeeLevel;
		return true;
	}

	// Token: 0x06002F1B RID: 12059 RVA: 0x000FFF20 File Offset: 0x000FE120
	public bool GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts part, out int juiceCost)
	{
		juiceCost = 0;
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		juiceCost = partNode.researchCost;
		return true;
	}

	// Token: 0x06002F1C RID: 12060 RVA: 0x000FFF4C File Offset: 0x000FE14C
	public bool GetPartUnlockRequiredParentParts(GRToolProgressionManager.ToolParts part, out List<GRToolProgressionManager.ToolParts> requiredParts)
	{
		requiredParts = new List<GRToolProgressionManager.ToolParts>();
		GRToolProgressionTree.GRToolProgressionNode partNode = this.toolProgressionTree.GetPartNode(part);
		if (partNode == null)
		{
			return false;
		}
		foreach (GRToolProgressionTree.GRToolProgressionNode grtoolProgressionNode in partNode.parents)
		{
			requiredParts.Add(grtoolProgressionNode.type);
		}
		return true;
	}

	// Token: 0x06002F1D RID: 12061 RVA: 0x000FFFC0 File Offset: 0x000FE1C0
	public bool GetPlayerShiftCredit(out int playerShiftCredit)
	{
		playerShiftCredit = 0;
		if (VRRig.LocalRig != null)
		{
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			if (grplayer != null)
			{
				playerShiftCredit = grplayer.ShiftCredits;
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002F1E RID: 12062 RVA: 0x000FFFFC File Offset: 0x000FE1FC
	public GRToolProgressionTree.EmployeeLevelRequirement GetCurrentEmployeeLevel()
	{
		return this.toolProgressionTree.GetCurrentEmploymentLevel();
	}

	// Token: 0x06002F1F RID: 12063 RVA: 0x00100009 File Offset: 0x000FE209
	public string GetTreeId()
	{
		return this.toolProgressionTree.GetTreeId();
	}

	// Token: 0x06002F20 RID: 12064 RVA: 0x00100018 File Offset: 0x000FE218
	public int GetDropPodLevel()
	{
		bool flag;
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodBasic, out flag) && flag)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002F21 RID: 12065 RVA: 0x00100038 File Offset: 0x000FE238
	public int GetDropPodChasisLevel()
	{
		bool flag;
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis3, out flag) && flag)
		{
			return 3;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis2, out flag) && flag)
		{
			return 2;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis1, out flag) && flag)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06002F22 RID: 12066 RVA: 0x00100078 File Offset: 0x000FE278
	public ProgressionManager.DrillUpgradeLevel GetDrillLevel()
	{
		bool flag;
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis3, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Upgrade3;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis2, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Upgrade2;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodChassis1, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Upgrade1;
		}
		if (this.IsPartUnlocked(GRToolProgressionManager.ToolParts.DropPodBasic, out flag) && flag)
		{
			return ProgressionManager.DrillUpgradeLevel.Base;
		}
		return ProgressionManager.DrillUpgradeLevel.None;
	}

	// Token: 0x06002F23 RID: 12067 RVA: 0x001000C8 File Offset: 0x000FE2C8
	public int GetJuiceCostForDrillUpgrade(ProgressionManager.DrillUpgradeLevel upgradeLevel)
	{
		int result = 0;
		switch (upgradeLevel)
		{
		case ProgressionManager.DrillUpgradeLevel.Base:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodBasic, out result);
			break;
		case ProgressionManager.DrillUpgradeLevel.Upgrade1:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodChassis1, out result);
			break;
		case ProgressionManager.DrillUpgradeLevel.Upgrade2:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodChassis2, out result);
			break;
		case ProgressionManager.DrillUpgradeLevel.Upgrade3:
			this.GetPartUnlockJuiceCost(GRToolProgressionManager.ToolParts.DropPodChassis3, out result);
			break;
		}
		return result;
	}

	// Token: 0x06002F24 RID: 12068 RVA: 0x00100124 File Offset: 0x000FE324
	public int GetSRCostForDrillUpgradeLevel(ProgressionManager.DrillUpgradeLevel level)
	{
		switch (level)
		{
		case ProgressionManager.DrillUpgradeLevel.Base:
			return 3600;
		case ProgressionManager.DrillUpgradeLevel.Upgrade1:
			return 0;
		case ProgressionManager.DrillUpgradeLevel.Upgrade2:
			return 0;
		case ProgressionManager.DrillUpgradeLevel.Upgrade3:
			return 0;
		default:
			return 0;
		}
	}

	// Token: 0x04003D5E RID: 15710
	[NonSerialized]
	private Dictionary<GRToolProgressionTree.EmployeeLevelRequirement, GRToolProgressionManager.EmployeeMetadata> employeeLevelMetadata = new Dictionary<GRToolProgressionTree.EmployeeLevelRequirement, GRToolProgressionManager.EmployeeMetadata>();

	// Token: 0x04003D5F RID: 15711
	[NonSerialized]
	private Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionManager.ToolProgressionMetaData> partMetadata = new Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionManager.ToolProgressionMetaData>();

	// Token: 0x04003D60 RID: 15712
	[NonSerialized]
	private GRToolProgressionTree toolProgressionTree = new GRToolProgressionTree();

	// Token: 0x04003D61 RID: 15713
	[NonSerialized]
	private GhostReactor reactor;

	// Token: 0x04003D62 RID: 15714
	[SerializeField]
	private List<GRResearchStation> researchStations;

	// Token: 0x04003D63 RID: 15715
	[SerializeField]
	private List<GRToolUpgradeStation> toolUpgradeStations;

	// Token: 0x04003D64 RID: 15716
	[NonSerialized]
	private bool pendingTreeToProcess;

	// Token: 0x04003D65 RID: 15717
	[NonSerialized]
	private bool pendingUpdateInventory;

	// Token: 0x04003D67 RID: 15719
	private bool sendUpdate;

	// Token: 0x02000724 RID: 1828
	public class ToolProgressionMetaData
	{
		// Token: 0x04003D68 RID: 15720
		public string name;

		// Token: 0x04003D69 RID: 15721
		public string description;

		// Token: 0x04003D6A RID: 15722
		public string annotation;

		// Token: 0x04003D6B RID: 15723
		public int shiftCreditCost;
	}

	// Token: 0x02000725 RID: 1829
	public struct EmployeeMetadata
	{
		// Token: 0x04003D6C RID: 15724
		public string name;

		// Token: 0x04003D6D RID: 15725
		public int level;
	}

	// Token: 0x02000726 RID: 1830
	public enum ToolParts
	{
		// Token: 0x04003D6F RID: 15727
		None,
		// Token: 0x04003D70 RID: 15728
		Baton,
		// Token: 0x04003D71 RID: 15729
		BatonDamage1,
		// Token: 0x04003D72 RID: 15730
		BatonDamage2,
		// Token: 0x04003D73 RID: 15731
		BatonDamage3,
		// Token: 0x04003D74 RID: 15732
		Flash,
		// Token: 0x04003D75 RID: 15733
		FlashDamage1,
		// Token: 0x04003D76 RID: 15734
		FlashDamage2,
		// Token: 0x04003D77 RID: 15735
		FlashDamage3,
		// Token: 0x04003D78 RID: 15736
		Collector,
		// Token: 0x04003D79 RID: 15737
		CollectorBonus1,
		// Token: 0x04003D7A RID: 15738
		CollectorBonus2,
		// Token: 0x04003D7B RID: 15739
		CollectorBonus3,
		// Token: 0x04003D7C RID: 15740
		Lantern,
		// Token: 0x04003D7D RID: 15741
		LanternIntensity1,
		// Token: 0x04003D7E RID: 15742
		LanternIntensity2,
		// Token: 0x04003D7F RID: 15743
		LanternIntensity3,
		// Token: 0x04003D80 RID: 15744
		ShieldGun,
		// Token: 0x04003D81 RID: 15745
		ShieldGunStrength1,
		// Token: 0x04003D82 RID: 15746
		ShieldGunStrength2,
		// Token: 0x04003D83 RID: 15747
		ShieldGunStrength3,
		// Token: 0x04003D84 RID: 15748
		DirectionalShield,
		// Token: 0x04003D85 RID: 15749
		DirectionalShieldSize1,
		// Token: 0x04003D86 RID: 15750
		DirectionalShieldSize2,
		// Token: 0x04003D87 RID: 15751
		DirectionalShieldSize3,
		// Token: 0x04003D88 RID: 15752
		EnergyEff,
		// Token: 0x04003D89 RID: 15753
		EnergyEff1,
		// Token: 0x04003D8A RID: 15754
		EnergyEff2,
		// Token: 0x04003D8B RID: 15755
		EnergyEff3,
		// Token: 0x04003D8C RID: 15756
		DockWrist,
		// Token: 0x04003D8D RID: 15757
		Revive,
		// Token: 0x04003D8E RID: 15758
		DropPodBasic,
		// Token: 0x04003D8F RID: 15759
		DropPodChassis1,
		// Token: 0x04003D90 RID: 15760
		DropPodChassis2,
		// Token: 0x04003D91 RID: 15761
		DropPodChassis3,
		// Token: 0x04003D92 RID: 15762
		StatusWatch,
		// Token: 0x04003D93 RID: 15763
		RattyBackpack,
		// Token: 0x04003D94 RID: 15764
		HockeyStick
	}
}
