using System;
using System.Collections.Generic;
using GorillaNetworking;

// Token: 0x02000727 RID: 1831
public class GRToolProgressionTree
{
	// Token: 0x06002F28 RID: 12072 RVA: 0x001001A0 File Offset: 0x000FE3A0
	public GRToolProgressionTree()
	{
		this.InitializeToolMapping();
		this.InitializeClubPartMapping();
		this.InitializeFlashPartMapping();
		this.InitializeRevivePartMapping();
		this.InitializeCollectorPartMapping();
		this.InitializeLanternPartMapping();
		this.InitializeShieldGunPartMapping();
		this.InitializeDirectionalShieldPartMapping();
		this.InitializeEnergyEfficiencyPartMapping();
		this.InitializeDockWristPartMapping();
		this.InitializeDropPodPartMapping();
	}

	// Token: 0x06002F29 RID: 12073 RVA: 0x0010027C File Offset: 0x000FE47C
	public void Init(GhostReactor ghostReactor, GRToolProgressionManager toolManager)
	{
		this.reactor = ghostReactor;
		this.manager = toolManager;
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnTreeUpdated += new Action(this.OnProgressionTreeUpdate);
			ProgressionManager.Instance.OnInventoryUpdated += new Action(this.OnInventoryUpdated);
		}
		this.RefreshProgressionTree();
		this.RefreshUserInventory();
	}

	// Token: 0x06002F2A RID: 12074 RVA: 0x001002DC File Offset: 0x000FE4DC
	public string GetTreeId()
	{
		return this.treeId;
	}

	// Token: 0x06002F2B RID: 12075 RVA: 0x001002E4 File Offset: 0x000FE4E4
	public List<GRTool.GRToolType> GetSupportedTools()
	{
		List<GRTool.GRToolType> list = new List<GRTool.GRToolType>();
		foreach (GRTool.GRToolType grtoolType in this.toolTree.Keys)
		{
			list.Add(grtoolType);
		}
		return list;
	}

	// Token: 0x06002F2C RID: 12076 RVA: 0x00100344 File Offset: 0x000FE544
	public List<GRToolProgressionTree.GRToolProgressionNode> GetToolUpgrades(GRTool.GRToolType tool)
	{
		List<GRToolProgressionTree.GRToolProgressionNode> result = new List<GRToolProgressionTree.GRToolProgressionNode>();
		this.AddToolProgressionChildren(this.toolTree[tool], ref result);
		return result;
	}

	// Token: 0x06002F2D RID: 12077 RVA: 0x0010036C File Offset: 0x000FE56C
	public GRToolProgressionTree.GRToolProgressionNode GetToolNode(GRTool.GRToolType tool)
	{
		if (this.toolTree.ContainsKey(tool))
		{
			return this.toolTree[tool];
		}
		return null;
	}

	// Token: 0x06002F2E RID: 12078 RVA: 0x0010038A File Offset: 0x000FE58A
	public GRToolProgressionTree.GRToolProgressionNode GetPartNode(GRToolProgressionManager.ToolParts part)
	{
		if (this.partTree.ContainsKey(part))
		{
			return this.partTree[part];
		}
		return null;
	}

	// Token: 0x06002F2F RID: 12079 RVA: 0x001003A8 File Offset: 0x000FE5A8
	public void RefreshProgressionTree()
	{
		ProgressionManager.Instance.RefreshProgressionTree();
	}

	// Token: 0x06002F30 RID: 12080 RVA: 0x001003B4 File Offset: 0x000FE5B4
	public void RefreshUserInventory()
	{
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x06002F31 RID: 12081 RVA: 0x001003C0 File Offset: 0x000FE5C0
	private void OnProgressionTreeUpdate()
	{
		UserHydratedProgressionTreeResponse tree = ProgressionManager.Instance.GetTree(this.treeName);
		if (tree != null)
		{
			this.ProcessToolProgressionTree(tree);
		}
		GRToolProgressionManager grtoolProgressionManager = this.manager;
		if (grtoolProgressionManager == null)
		{
			return;
		}
		grtoolProgressionManager.SendMothershipUpdated();
	}

	// Token: 0x06002F32 RID: 12082 RVA: 0x001003F8 File Offset: 0x000FE5F8
	private void OnInventoryUpdated()
	{
		ProgressionManager.MothershipItemSummary mothershipItemSummary;
		if (ProgressionManager.Instance.GetInventoryItem(this.researchPointsEntitlement, out mothershipItemSummary))
		{
			this.currentResearchPoints = mothershipItemSummary.Quantity;
		}
		ProgressionManager.MothershipItemSummary mothershipItemSummary2;
		ProgressionManager.MothershipItemSummary mothershipItemSummary3;
		ProgressionManager.MothershipItemSummary mothershipItemSummary4;
		if (ProgressionManager.Instance.GetInventoryItem(this.fullTimeEntitlement, out mothershipItemSummary2))
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.FullTime;
		}
		else if (ProgressionManager.Instance.GetInventoryItem(this.partTimeEntitlement, out mothershipItemSummary3))
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.PartTime;
		}
		else if (ProgressionManager.Instance.GetInventoryItem(this.internEntitlement, out mothershipItemSummary4))
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.Intern;
		}
		else
		{
			this.currentEmploymentLevel = GRToolProgressionTree.EmployeeLevelRequirement.None;
		}
		GRToolProgressionManager grtoolProgressionManager = this.manager;
		if (grtoolProgressionManager == null)
		{
			return;
		}
		grtoolProgressionManager.SendMothershipUpdated();
	}

	// Token: 0x06002F33 RID: 12083 RVA: 0x00100493 File Offset: 0x000FE693
	public GRToolProgressionTree.EmployeeLevelRequirement GetCurrentEmploymentLevel()
	{
		return this.currentEmploymentLevel;
	}

	// Token: 0x06002F34 RID: 12084 RVA: 0x0010049C File Offset: 0x000FE69C
	private void AddToolProgressionChildren(GRToolProgressionTree.GRToolProgressionNode currentNode, ref List<GRToolProgressionTree.GRToolProgressionNode> list)
	{
		foreach (GRToolProgressionTree.GRToolProgressionNode grtoolProgressionNode in currentNode.children)
		{
			list.Add(grtoolProgressionNode);
			this.AddToolProgressionChildren(grtoolProgressionNode, ref list);
		}
	}

	// Token: 0x06002F35 RID: 12085 RVA: 0x001004F8 File Offset: 0x000FE6F8
	public int GetNumberOfResearchPoints()
	{
		return this.currentResearchPoints;
	}

	// Token: 0x06002F36 RID: 12086 RVA: 0x00100500 File Offset: 0x000FE700
	private void InitializeToolMapping()
	{
		this.toolMapping["ChargeBaton"] = GRTool.GRToolType.Club;
		this.toolMapping["FlashTool"] = GRTool.GRToolType.Flash;
		this.toolMapping["Revive"] = GRTool.GRToolType.Revive;
		this.toolMapping["Collector"] = GRTool.GRToolType.Collector;
		this.toolMapping["Lantern"] = GRTool.GRToolType.Lantern;
		this.toolMapping["ShieldGun"] = GRTool.GRToolType.ShieldGun;
		this.toolMapping["DirectionalShield"] = GRTool.GRToolType.DirectionalShield;
		this.toolMapping["DockWrist"] = GRTool.GRToolType.DockWrist;
		this.toolMapping["EnergyEfficiency"] = GRTool.GRToolType.EnergyEfficiency;
		this.toolMapping["DropPodBasic"] = GRTool.GRToolType.DropPod;
	}

	// Token: 0x06002F37 RID: 12087 RVA: 0x001005BC File Offset: 0x000FE7BC
	private void InitializeClubPartMapping()
	{
		this.partMapping["ChargeBaton"] = GRToolProgressionManager.ToolParts.Baton;
		this.partMapping["BatonDamage1"] = GRToolProgressionManager.ToolParts.BatonDamage1;
		this.partMapping["BatonDamage2"] = GRToolProgressionManager.ToolParts.BatonDamage2;
		this.partMapping["BatonDamage3"] = GRToolProgressionManager.ToolParts.BatonDamage3;
	}

	// Token: 0x06002F38 RID: 12088 RVA: 0x00100610 File Offset: 0x000FE810
	private void InitializeFlashPartMapping()
	{
		this.partMapping["FlashTool"] = GRToolProgressionManager.ToolParts.Flash;
		this.partMapping["FlashDamage1"] = GRToolProgressionManager.ToolParts.FlashDamage1;
		this.partMapping["FlashDamage2"] = GRToolProgressionManager.ToolParts.FlashDamage2;
		this.partMapping["FlashDamage3"] = GRToolProgressionManager.ToolParts.FlashDamage3;
	}

	// Token: 0x06002F39 RID: 12089 RVA: 0x00100664 File Offset: 0x000FE864
	private void InitializeCollectorPartMapping()
	{
		this.partMapping["Collector"] = GRToolProgressionManager.ToolParts.Collector;
		this.partMapping["CollectorBonus1"] = GRToolProgressionManager.ToolParts.CollectorBonus1;
		this.partMapping["CollectorBonus2"] = GRToolProgressionManager.ToolParts.CollectorBonus2;
		this.partMapping["CollectorBonus3"] = GRToolProgressionManager.ToolParts.CollectorBonus3;
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x001006B9 File Offset: 0x000FE8B9
	private void InitializeRevivePartMapping()
	{
		this.partMapping["Revive"] = GRToolProgressionManager.ToolParts.Revive;
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x001006D0 File Offset: 0x000FE8D0
	private void InitializeLanternPartMapping()
	{
		this.partMapping["Lantern"] = GRToolProgressionManager.ToolParts.Lantern;
		this.partMapping["LanternIntensity1"] = GRToolProgressionManager.ToolParts.LanternIntensity1;
		this.partMapping["LanternIntensity2"] = GRToolProgressionManager.ToolParts.LanternIntensity2;
		this.partMapping["LanternIntensity3"] = GRToolProgressionManager.ToolParts.LanternIntensity3;
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x00100728 File Offset: 0x000FE928
	private void InitializeShieldGunPartMapping()
	{
		this.partMapping["ShieldGun"] = GRToolProgressionManager.ToolParts.ShieldGun;
		this.partMapping["ShieldGunStrength1"] = GRToolProgressionManager.ToolParts.ShieldGunStrength1;
		this.partMapping["ShieldGunStrength2"] = GRToolProgressionManager.ToolParts.ShieldGunStrength2;
		this.partMapping["ShieldGunStrength3"] = GRToolProgressionManager.ToolParts.ShieldGunStrength3;
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x00100780 File Offset: 0x000FE980
	private void InitializeDirectionalShieldPartMapping()
	{
		this.partMapping["DirectionalShield"] = GRToolProgressionManager.ToolParts.DirectionalShield;
		this.partMapping["DirectionalShieldSize1"] = GRToolProgressionManager.ToolParts.DirectionalShieldSize1;
		this.partMapping["DirectionalShieldSize2"] = GRToolProgressionManager.ToolParts.DirectionalShieldSize2;
		this.partMapping["DirectionalShieldSize3"] = GRToolProgressionManager.ToolParts.DirectionalShieldSize3;
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x001007D8 File Offset: 0x000FE9D8
	private void InitializeEnergyEfficiencyPartMapping()
	{
		this.partMapping["EnergyEfficiency"] = GRToolProgressionManager.ToolParts.EnergyEff;
		this.partMapping["EnergyEff1"] = GRToolProgressionManager.ToolParts.EnergyEff1;
		this.partMapping["EnergyEff2"] = GRToolProgressionManager.ToolParts.EnergyEff2;
		this.partMapping["EnergyEff3"] = GRToolProgressionManager.ToolParts.EnergyEff3;
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x0010082D File Offset: 0x000FEA2D
	private void InitializeDockWristPartMapping()
	{
		this.partMapping["DockWrist"] = GRToolProgressionManager.ToolParts.DockWrist;
		this.partMapping["StatusWatch"] = GRToolProgressionManager.ToolParts.StatusWatch;
		this.partMapping["RattyBackpack"] = GRToolProgressionManager.ToolParts.RattyBackpack;
	}

	// Token: 0x06002F40 RID: 12096 RVA: 0x00100868 File Offset: 0x000FEA68
	private void InitializeDropPodPartMapping()
	{
		this.partMapping["DropPodBasic"] = GRToolProgressionManager.ToolParts.DropPodBasic;
		this.partMapping["DropPodChassis01"] = GRToolProgressionManager.ToolParts.DropPodChassis1;
		this.partMapping["DropPodChassis02"] = GRToolProgressionManager.ToolParts.DropPodChassis2;
		this.partMapping["DropPodChassis03"] = GRToolProgressionManager.ToolParts.DropPodChassis3;
	}

	// Token: 0x06002F41 RID: 12097 RVA: 0x001008C0 File Offset: 0x000FEAC0
	private void AddFakeNodes()
	{
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.Club))
		{
			this.toolTree[GRTool.GRToolType.Club] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "Baton",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.Baton,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.Baton),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.Baton))
		{
			this.partTree[GRToolProgressionManager.ToolParts.Baton] = this.toolTree[GRTool.GRToolType.Club];
		}
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.EnergyEfficiency))
		{
			this.toolTree[GRTool.GRToolType.EnergyEfficiency] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "EnergyEfficiency",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.EnergyEff,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.EnergyEff),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.EnergyEff))
		{
			this.partTree[GRToolProgressionManager.ToolParts.EnergyEff] = this.toolTree[GRTool.GRToolType.EnergyEfficiency];
		}
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.Collector))
		{
			this.toolTree[GRTool.GRToolType.Collector] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "Collector",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.Collector,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.Collector),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.Collector))
		{
			this.partTree[GRToolProgressionManager.ToolParts.Collector] = this.toolTree[GRTool.GRToolType.Collector];
		}
		if (!this.toolTree.ContainsKey(GRTool.GRToolType.Lantern))
		{
			this.toolTree[GRTool.GRToolType.Lantern] = new GRToolProgressionTree.GRToolProgressionNode
			{
				name = "Lantern",
				unlocked = true,
				researchCost = 0,
				rootNode = true,
				type = GRToolProgressionManager.ToolParts.Lantern,
				partMetadata = this.manager.GetPartMetadata(GRToolProgressionManager.ToolParts.Lantern),
				requiredEmployeeLevel = GRToolProgressionTree.EmployeeLevelRequirement.None
			};
		}
		if (!this.partTree.ContainsKey(GRToolProgressionManager.ToolParts.Lantern))
		{
			this.partTree[GRToolProgressionManager.ToolParts.Lantern] = this.toolTree[GRTool.GRToolType.Lantern];
		}
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x00100AF0 File Offset: 0x000FECF0
	private void ProcessNodes()
	{
		foreach (KeyValuePair<string, GRToolProgressionTree.GRToolProgressionRawNode> keyValuePair in this.nodeTree)
		{
			GRToolProgressionTree.GRToolProgressionRawNode value = keyValuePair.Value;
			foreach (string text in value.requiredByIds)
			{
				if (this.nodeTree.ContainsKey(text))
				{
					this.nodeTree[text].progressionNode.children.Add(value.progressionNode);
					value.progressionNode.parents.Add(this.nodeTree[text].progressionNode);
				}
			}
			value.progressionNode.requiredEmployeeLevel = this.GetEmployeeLevel(value.requiredEntitlements);
			string text2 = value.progressionNode.name.Trim();
			if (this.toolMapping.ContainsKey(text2))
			{
				GRTool.GRToolType grtoolType = this.toolMapping[text2];
				value.progressionNode.rootNode = true;
				if (!value.progressionNode.unlocked && this.autoUnlockNodeId == string.Empty && value.progressionNode.researchCost == 0 && value.progressionNode.requiredEmployeeLevel == GRToolProgressionTree.EmployeeLevelRequirement.None)
				{
					this.autoUnlockNodeId = value.progressionNode.id;
				}
				this.toolTree[grtoolType] = value.progressionNode;
			}
			this.partTree[value.progressionNode.type] = value.progressionNode;
		}
	}

	// Token: 0x06002F43 RID: 12099 RVA: 0x00100CBC File Offset: 0x000FEEBC
	private void PopulateMetadata()
	{
		foreach (KeyValuePair<string, GRToolProgressionTree.GRToolProgressionRawNode> keyValuePair in this.nodeTree)
		{
			keyValuePair.Value.progressionNode.partMetadata = this.manager.GetPartMetadata(keyValuePair.Value.progressionNode.type);
		}
	}

	// Token: 0x06002F44 RID: 12100 RVA: 0x00100D38 File Offset: 0x000FEF38
	private GRToolProgressionTree.EmployeeLevelRequirement GetEmployeeLevel(List<string> rawRequiredEntitlements)
	{
		foreach (string text in rawRequiredEntitlements)
		{
			string text2 = text.Trim();
			if (text2 == "Intern")
			{
				return GRToolProgressionTree.EmployeeLevelRequirement.Intern;
			}
			if (text2 == "PartTime")
			{
				return GRToolProgressionTree.EmployeeLevelRequirement.PartTime;
			}
			if (text2 == "FullTime")
			{
				return GRToolProgressionTree.EmployeeLevelRequirement.FullTime;
			}
		}
		return GRToolProgressionTree.EmployeeLevelRequirement.None;
	}

	// Token: 0x06002F45 RID: 12101 RVA: 0x00100DBC File Offset: 0x000FEFBC
	private void ProcessTreeNode(UserHydratedNodeDefinition treeNode)
	{
		GRToolProgressionTree.GRToolProgressionRawNode grtoolProgressionRawNode = new GRToolProgressionTree.GRToolProgressionRawNode();
		grtoolProgressionRawNode.progressionNode.id = treeNode.id;
		grtoolProgressionRawNode.progressionNode.name = treeNode.name;
		grtoolProgressionRawNode.progressionNode.unlocked = treeNode.unlocked;
		if (this.partMapping.ContainsKey(grtoolProgressionRawNode.progressionNode.name))
		{
			if (this.toolMapping.ContainsKey(grtoolProgressionRawNode.progressionNode.name))
			{
				grtoolProgressionRawNode.progressionNode.rootNode = true;
			}
			grtoolProgressionRawNode.progressionNode.type = this.partMapping[grtoolProgressionRawNode.progressionNode.name];
		}
		if (treeNode.cost != null && treeNode.cost.items != null)
		{
			foreach (KeyValuePair<string, MothershipHydratedInventoryChange> keyValuePair in treeNode.cost.items)
			{
				if (keyValuePair.Key.Trim() == this.researchPointsEntitlement)
				{
					grtoolProgressionRawNode.progressionNode.researchCost = keyValuePair.Value.Delta;
				}
			}
		}
		foreach (MothershipEntitlementCatalogItem mothershipEntitlementCatalogItem in treeNode.prerequisite_entitlements)
		{
			grtoolProgressionRawNode.requiredEntitlements.Add(mothershipEntitlementCatalogItem.name);
		}
		foreach (SWIGTYPE_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t swigtype_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t in treeNode.prerequisite_nodes.nodes)
		{
			ComplexPrerequisiteNodes complexPrerequisiteNodes = new ComplexPrerequisiteNodes();
			NodeReference nodeReference = new NodeReference();
			if (!MothershipApi.TryGetComplexPrerequisiteNodeFromVariant(swigtype_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t, complexPrerequisiteNodes) && MothershipApi.TryGetNodeReferenceFromVariant(swigtype_p_std__variantT_MothershipApiShared__NodeReference_MothershipApiShared__ComplexPrerequisiteNodes_t, nodeReference))
			{
				grtoolProgressionRawNode.requiredByIds.Add(nodeReference.node_id);
			}
		}
		if (this.pendingPartUnlock != GRToolProgressionManager.ToolParts.None && this.pendingPartUnlock == grtoolProgressionRawNode.progressionNode.type)
		{
			GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
			if (this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodBasic || this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodChassis1 || this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodChassis2 || this.pendingPartUnlock == GRToolProgressionManager.ToolParts.DropPodChassis3)
			{
				if (this.pendingPartUnlock != GRToolProgressionManager.ToolParts.DropPodBasic)
				{
					grplayer.SendPodUpgradeTelemetry(grtoolProgressionRawNode.progressionNode.name, treeNode.prerequisite_entitlements.Count, 0, grtoolProgressionRawNode.progressionNode.researchCost);
				}
			}
			else
			{
				grplayer.SendToolUpgradeTelemetry("Research", grtoolProgressionRawNode.progressionNode.name, treeNode.prerequisite_entitlements.Count, grtoolProgressionRawNode.progressionNode.researchCost, 0, 0);
			}
			this.pendingPartUnlock = GRToolProgressionManager.ToolParts.None;
		}
		this.nodeTree[grtoolProgressionRawNode.progressionNode.id] = grtoolProgressionRawNode;
	}

	// Token: 0x06002F46 RID: 12102 RVA: 0x00101078 File Offset: 0x000FF278
	private void ProcessToolProgressionTree(UserHydratedProgressionTreeResponse tree)
	{
		if (tree.Tree.name != this.treeName)
		{
			return;
		}
		this.toolTree = new Dictionary<GRTool.GRToolType, GRToolProgressionTree.GRToolProgressionNode>();
		this.nodeTree = new Dictionary<string, GRToolProgressionTree.GRToolProgressionRawNode>();
		this.treeId = tree.Tree.id;
		foreach (UserHydratedNodeDefinition treeNode in tree.Nodes)
		{
			this.ProcessTreeNode(treeNode);
		}
		this.PopulateMetadata();
		this.ProcessNodes();
		this.AddFakeNodes();
		if (this.autoUnlockNodeId != string.Empty)
		{
			string nodeId = this.autoUnlockNodeId;
			this.autoUnlockNodeId = string.Empty;
			GhostReactorProgression.instance.UnlockProgressionTreeNode(this.treeId, nodeId, this.reactor);
		}
		GRToolProgressionManager grtoolProgressionManager = this.manager;
		if (grtoolProgressionManager == null)
		{
			return;
		}
		grtoolProgressionManager.SendMothershipUpdated();
	}

	// Token: 0x06002F47 RID: 12103 RVA: 0x00101164 File Offset: 0x000FF364
	public void AttemptToUnlockPart(GRToolProgressionManager.ToolParts part)
	{
		if (this.partTree.ContainsKey(part))
		{
			this.pendingPartUnlock = part;
			GhostReactorProgression.instance.UnlockProgressionTreeNode(this.treeId, this.partTree[part].id, this.reactor);
		}
	}

	// Token: 0x04003D95 RID: 15765
	private string treeName = "GRTools";

	// Token: 0x04003D96 RID: 15766
	private string treeId = string.Empty;

	// Token: 0x04003D97 RID: 15767
	private string researchPointsEntitlement = "GR_ResearchPoints";

	// Token: 0x04003D98 RID: 15768
	private Dictionary<GRTool.GRToolType, GRToolProgressionTree.GRToolProgressionNode> toolTree = new Dictionary<GRTool.GRToolType, GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x04003D99 RID: 15769
	private Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionTree.GRToolProgressionNode> partTree = new Dictionary<GRToolProgressionManager.ToolParts, GRToolProgressionTree.GRToolProgressionNode>();

	// Token: 0x04003D9A RID: 15770
	private Dictionary<string, GRToolProgressionTree.GRToolProgressionRawNode> nodeTree = new Dictionary<string, GRToolProgressionTree.GRToolProgressionRawNode>();

	// Token: 0x04003D9B RID: 15771
	private Dictionary<string, GRTool.GRToolType> toolMapping = new Dictionary<string, GRTool.GRToolType>();

	// Token: 0x04003D9C RID: 15772
	private Dictionary<string, GRToolProgressionManager.ToolParts> partMapping = new Dictionary<string, GRToolProgressionManager.ToolParts>();

	// Token: 0x04003D9D RID: 15773
	private string autoUnlockNodeId = string.Empty;

	// Token: 0x04003D9E RID: 15774
	private int currentResearchPoints;

	// Token: 0x04003D9F RID: 15775
	[NonSerialized]
	private GhostReactor reactor;

	// Token: 0x04003DA0 RID: 15776
	[NonSerialized]
	private GRToolProgressionManager manager;

	// Token: 0x04003DA1 RID: 15777
	[NonSerialized]
	private GRToolProgressionTree.EmployeeLevelRequirement currentEmploymentLevel;

	// Token: 0x04003DA2 RID: 15778
	private string internEntitlement = "Intern";

	// Token: 0x04003DA3 RID: 15779
	private string partTimeEntitlement = "PartTime";

	// Token: 0x04003DA4 RID: 15780
	private string fullTimeEntitlement = "FullTime";

	// Token: 0x04003DA5 RID: 15781
	private GRToolProgressionManager.ToolParts pendingPartUnlock;

	// Token: 0x02000728 RID: 1832
	public enum EmployeeLevelRequirement
	{
		// Token: 0x04003DA7 RID: 15783
		None,
		// Token: 0x04003DA8 RID: 15784
		Intern,
		// Token: 0x04003DA9 RID: 15785
		PartTime,
		// Token: 0x04003DAA RID: 15786
		FullTime
	}

	// Token: 0x02000729 RID: 1833
	public class GRToolProgressionNode
	{
		// Token: 0x04003DAB RID: 15787
		public string id;

		// Token: 0x04003DAC RID: 15788
		public string name;

		// Token: 0x04003DAD RID: 15789
		public bool unlocked;

		// Token: 0x04003DAE RID: 15790
		public int researchCost;

		// Token: 0x04003DAF RID: 15791
		public bool rootNode;

		// Token: 0x04003DB0 RID: 15792
		public GRToolProgressionManager.ToolParts type;

		// Token: 0x04003DB1 RID: 15793
		public GRToolProgressionManager.ToolProgressionMetaData partMetadata;

		// Token: 0x04003DB2 RID: 15794
		public List<GRToolProgressionTree.GRToolProgressionNode> children = new List<GRToolProgressionTree.GRToolProgressionNode>();

		// Token: 0x04003DB3 RID: 15795
		public List<GRToolProgressionTree.GRToolProgressionNode> parents = new List<GRToolProgressionTree.GRToolProgressionNode>();

		// Token: 0x04003DB4 RID: 15796
		public GRToolProgressionTree.EmployeeLevelRequirement requiredEmployeeLevel;
	}

	// Token: 0x0200072A RID: 1834
	private class GRToolProgressionRawNode
	{
		// Token: 0x04003DB5 RID: 15797
		public GRToolProgressionTree.GRToolProgressionNode progressionNode = new GRToolProgressionTree.GRToolProgressionNode();

		// Token: 0x04003DB6 RID: 15798
		public List<string> requiredByIds = new List<string>();

		// Token: 0x04003DB7 RID: 15799
		public List<string> requiredEntitlements = new List<string>();
	}
}
