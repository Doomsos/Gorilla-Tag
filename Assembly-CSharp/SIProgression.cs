using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x02000120 RID: 288
[DefaultExecutionOrder(-100)]
public class SIProgression : MonoBehaviour, IGorillaSliceableSimple, GorillaQuestManager
{
	// Token: 0x1700008B RID: 139
	// (get) Token: 0x0600076F RID: 1903 RVA: 0x00028C93 File Offset: 0x00026E93
	// (set) Token: 0x06000770 RID: 1904 RVA: 0x00028C9A File Offset: 0x00026E9A
	public static SIProgression Instance { get; private set; }

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000771 RID: 1905 RVA: 0x00028CA4 File Offset: 0x00026EA4
	// (remove) Token: 0x06000772 RID: 1906 RVA: 0x00028CDC File Offset: 0x00026EDC
	public event Action OnClientReady;

	// Token: 0x06000773 RID: 1907 RVA: 0x00028D14 File Offset: 0x00026F14
	private void Awake()
	{
		if (SIProgression.Instance == null)
		{
			SIProgression.Instance = this;
		}
		this.emptyNode = default(SIProgression.SINode);
		SIProgression.InitResourceToStringDictionary();
		this.resourceCapsArray = Enumerable.ToArray<int>(Enumerable.Repeat<int>(int.MaxValue, 6));
		for (int i = 0; i < this.resourceCaps.Length; i++)
		{
			this.resourceCapsArray[(int)this.resourceCaps[i].resourceType] = this.resourceCaps[i].resourceMax;
		}
		foreach (object obj in Enum.GetValues(typeof(SITechTreePageId)))
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)obj;
			this.heldOrSnappedByGadgetPageType.Add(sitechTreePageId, 0);
		}
		this.EnsureInitialized();
		SIProgression.InitializeQuests();
		this.ResetTelemetryIntervalData();
		this.LoadSavedTelemetryData();
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00028E0C File Offset: 0x0002700C
	public void OnEnable()
	{
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnTreeUpdated += new Action(this.HandleTreeUpdated);
			ProgressionManager.Instance.OnInventoryUpdated += new Action(this.HandleInventoryUpdated);
			ProgressionManager.Instance.OnNodeUnlocked += new Action<string, string>(this.HandleNodeUnlocked);
		}
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00028E70 File Offset: 0x00027070
	public void OnDisable()
	{
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnTreeUpdated -= new Action(this.HandleTreeUpdated);
			ProgressionManager.Instance.OnInventoryUpdated -= new Action(this.HandleInventoryUpdated);
			ProgressionManager.Instance.OnNodeUnlocked -= new Action<string, string>(this.HandleNodeUnlocked);
		}
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x00028ED3 File Offset: 0x000270D3
	public static string GetResourceString(SIResource.ResourceType resourceType)
	{
		if (SIProgression._resourceToString == null)
		{
			SIProgression.InitResourceToStringDictionary();
		}
		return SIProgression._resourceToString[resourceType];
	}

	// Token: 0x06000777 RID: 1911 RVA: 0x00028EEC File Offset: 0x000270EC
	private static void InitResourceToStringDictionary()
	{
		SIProgression._resourceToString = new Dictionary<SIResource.ResourceType, string>();
		SIProgression._resourceToString[SIResource.ResourceType.TechPoint] = "SI_TechPoints";
		SIProgression._resourceToString[SIResource.ResourceType.StrangeWood] = "SI_StrangeWood";
		SIProgression._resourceToString[SIResource.ResourceType.WeirdGear] = "SI_WeirdGear";
		SIProgression._resourceToString[SIResource.ResourceType.VibratingSpring] = "SI_VibratingSpring";
		SIProgression._resourceToString[SIResource.ResourceType.BouncySand] = "SI_BouncySand";
		SIProgression._resourceToString[SIResource.ResourceType.FloppyMetal] = "SI_FloppyMetal";
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x00028F63 File Offset: 0x00027163
	public void Init()
	{
		SIPlayer.LocalPlayer.SetProgressionLocal();
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.RefreshProgressionTree();
			ProgressionManager.Instance.RefreshUserInventory();
		}
		this.ClearAllQuestEventListeners();
		this.SetupAllQuestEventListeners();
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00028F9C File Offset: 0x0002719C
	public void EnsureInitialized()
	{
		if (this.techTreeSO != null)
		{
			this.techTreeSO.EnsureInitialized();
		}
		if (SIPlayer.progressionSO == null)
		{
			SIPlayer.progressionSO = this.techTreeSO;
		}
		int num = 6;
		if (this.resourceDict == null || this.resourceDict.Count != num)
		{
			this.resourceDict = new Dictionary<SIResource.ResourceType, int>();
			this.resourceDict[SIResource.ResourceType.TechPoint] = 0;
			this.resourceDict[SIResource.ResourceType.StrangeWood] = 0;
			this.resourceDict[SIResource.ResourceType.VibratingSpring] = 0;
			this.resourceDict[SIResource.ResourceType.BouncySand] = 0;
			this.resourceDict[SIResource.ResourceType.FloppyMetal] = 0;
			this.resourceDict[SIResource.ResourceType.WeirdGear] = 0;
		}
		int num2 = 2;
		if (this.limitedDepositTimeArray == null || this.limitedDepositTimeArray.Length != num2)
		{
			this.limitedDepositTimeArray = new int[num2];
		}
		int treePageCount = SIPlayer.progressionSO.TreePageCount;
		if (this.unlockedTechTreeData == null || this.unlockedTechTreeData.Length != treePageCount)
		{
			this.unlockedTechTreeData = new bool[treePageCount][];
		}
		for (int i = 0; i < treePageCount; i++)
		{
			int num3 = SIPlayer.progressionSO.TreeNodeCounts[i];
			if (this.unlockedTechTreeData[i] == null || this.unlockedTechTreeData[i].Length != num3)
			{
				this.unlockedTechTreeData[i] = new bool[num3];
			}
		}
		if (this.activeQuestIds == null || this.activeQuestIds.Length != 3)
		{
			this.activeQuestIds = new int[3];
		}
		if (this.activeQuestProgresses == null || this.activeQuestProgresses.Length != 3)
		{
			this.activeQuestProgresses = new int[3];
		}
		this.CopySaveDataToDiff();
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x0002911C File Offset: 0x0002731C
	private void ApplyServerQuestsStatus(ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
	{
		if (userQuestsStatus == null)
		{
			return;
		}
		this.stashedQuests = userQuestsStatus.TodayClaimableQuests;
		this.stashedBonusPoints = userQuestsStatus.TodayClaimableBonus;
		this.dailyLimitedTurnedIn = (userQuestsStatus.TodayClaimableIdol <= 0);
		this.lastQuestGrantTime = DateTime.UtcNow;
		this.RefreshActiveQuests();
		SIPlayer.SetAndBroadcastProgression();
		if (!this.questsInitialized)
		{
			this.questsInitialized = true;
			this.ClientReady = true;
			Action onClientReady = this.OnClientReady;
			if (onClientReady == null)
			{
				return;
			}
			onClientReady.Invoke();
		}
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00029194 File Offset: 0x00027394
	public int GetCurrencyAmount(SIResource.ResourceType currencyType)
	{
		ProgressionManager.MothershipItemSummary mothershipItemSummary;
		if (!ProgressionManager.Instance.GetInventoryItem(SIProgression._resourceToString[currencyType], out mothershipItemSummary))
		{
			return 0;
		}
		return mothershipItemSummary.Quantity;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x000291C4 File Offset: 0x000273C4
	public bool IsNodeUnlocked(SIUpgradeType upgradeType)
	{
		if (this.siNodes != null)
		{
			SIProgression.SINode sinode;
			return this.siNodes.TryGetValue(upgradeType, ref sinode) && sinode.unlocked;
		}
		ProgressionManager instance = ProgressionManager.Instance;
		UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse = (instance != null) ? instance.GetTree("SI_Gadgets") : null;
		if (userHydratedProgressionTreeResponse != null)
		{
			foreach (UserHydratedNodeDefinition userHydratedNodeDefinition in userHydratedProgressionTreeResponse.Nodes)
			{
				if (userHydratedNodeDefinition.name == upgradeType.ToString())
				{
					return userHydratedNodeDefinition.unlocked;
				}
			}
			return false;
		}
		return false;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x00029270 File Offset: 0x00027470
	public void UnlockNode(SIUpgradeType upgradeType)
	{
		if (this._treeReady && this._inventoryReady)
		{
			ProgressionManager instance = ProgressionManager.Instance;
			UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse = (instance != null) ? instance.GetTree("SI_Gadgets") : null;
			SIProgression.SINode sinode;
			if (this.siNodes == null || !this.siNodes.TryGetValue(upgradeType, ref sinode) || sinode.unlocked)
			{
				return;
			}
			ProgressionManager.Instance.UnlockNode(userHydratedProgressionTreeResponse.Tree.id, sinode.id);
		}
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x000292E0 File Offset: 0x000274E0
	private void HandleTreeUpdated()
	{
		this._treeReady = true;
		this.UpdateTree();
		this.UpdateUnlockOnPlayer();
		if (!this._startingPackageGranted)
		{
			if (this.IsNodeUnlocked(SIUpgradeType.Initialize))
			{
				this._startingPackageGranted = true;
			}
			else
			{
				this.startingPackageBackupAttempts++;
				if (this.startingPackageBackupAttempts > 10)
				{
					this._startingPackageGranted = true;
				}
				else
				{
					base.StartCoroutine(this.TryClaimNewPlayerPackage());
				}
			}
		}
		Action onTreeReady = this.OnTreeReady;
		if (onTreeReady == null)
		{
			return;
		}
		onTreeReady.Invoke();
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x00029358 File Offset: 0x00027558
	private void HandleInventoryUpdated()
	{
		this._inventoryReady = true;
		this.UpdateCurrencyOnPlayer();
		Action onInventoryReady = this.OnInventoryReady;
		if (onInventoryReady == null)
		{
			return;
		}
		onInventoryReady.Invoke();
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x00029377 File Offset: 0x00027577
	private IEnumerator TryClaimNewPlayerPackage()
	{
		yield return new WaitForSeconds(Mathf.Pow((float)this.startingPackageBackupAttempts, 2f));
		if (!this._startingPackageGranted)
		{
			this.TryUnlock(SIUpgradeType.Initialize);
		}
		yield break;
	}

	// Token: 0x06000781 RID: 1921 RVA: 0x00029388 File Offset: 0x00027588
	private void HandleNodeUnlocked(string treeId, string nodeId)
	{
		this.UpdateTree();
		this.UpdateUnlockOnPlayer();
		SIProgression.SINode nodeFromID = this.GetNodeFromID(nodeId);
		if (!string.IsNullOrEmpty(nodeFromID.id))
		{
			Action<SIUpgradeType> onNodeUnlocked = this.OnNodeUnlocked;
			if (onNodeUnlocked == null)
			{
				return;
			}
			onNodeUnlocked.Invoke(nodeFromID.upgradeType);
		}
	}

	// Token: 0x06000782 RID: 1922 RVA: 0x000293CC File Offset: 0x000275CC
	private void UpdateTree()
	{
		ProgressionManager instance = ProgressionManager.Instance;
		UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse = (instance != null) ? instance.GetTree("SI_Gadgets") : null;
		this.siNodes = new Dictionary<SIUpgradeType, SIProgression.SINode>();
		foreach (UserHydratedNodeDefinition userHydratedNodeDefinition in userHydratedProgressionTreeResponse.Nodes)
		{
			SIUpgradeType siupgradeType;
			if (!Enum.TryParse<SIUpgradeType>(userHydratedNodeDefinition.name, ref siupgradeType))
			{
				siupgradeType = SIUpgradeType.InvalidNode;
			}
			Dictionary<SIResource.ResourceType, int> dictionary = new Dictionary<SIResource.ResourceType, int>();
			HydratedProgressionNodeCost cost = userHydratedNodeDefinition.cost;
			if (((cost != null) ? cost.items : null) != null)
			{
				foreach (KeyValuePair<string, MothershipHydratedInventoryChange> keyValuePair in userHydratedNodeDefinition.cost.items)
				{
					foreach (KeyValuePair<SIResource.ResourceType, string> keyValuePair2 in SIProgression._resourceToString)
					{
						if (keyValuePair2.Value == keyValuePair.Key)
						{
							dictionary[keyValuePair2.Key] = keyValuePair.Value.Delta;
							break;
						}
					}
				}
			}
			SIProgression.SINode sinode = new SIProgression.SINode
			{
				id = userHydratedNodeDefinition.id,
				unlocked = userHydratedNodeDefinition.unlocked,
				costs = dictionary,
				parents = new List<SIProgression.SINode>(),
				upgradeType = siupgradeType
			};
			this.siNodes[siupgradeType] = sinode;
		}
	}

	// Token: 0x06000783 RID: 1923 RVA: 0x00029588 File Offset: 0x00027788
	public bool TryUnlock(SIUpgradeType upgrade)
	{
		if (upgrade == SIUpgradeType.Initialize)
		{
			if (!this._startingPackageGranted)
			{
				this.UnlockNode(upgrade);
				return true;
			}
			return false;
		}
		else
		{
			this.techTreeSO.EnsureInitialized();
			GraphNode<SITechTreeNode> graphNode;
			if (!this.techTreeSO.TryGetNode(upgrade, out graphNode))
			{
				return false;
			}
			SIPlayer localPlayer = SIPlayer.LocalPlayer;
			SITechTreeNode value = graphNode.Value;
			if (localPlayer.NodeResearched(upgrade))
			{
				return false;
			}
			if (!this._treeReady)
			{
				ProgressionManager.Instance.RefreshProgressionTree();
			}
			if (!this._inventoryReady)
			{
				ProgressionManager.Instance.RefreshUserInventory();
			}
			if (!localPlayer.NodeParentsUnlocked(upgrade))
			{
				return false;
			}
			foreach (SIResource.ResourceCost resourceCost in value.nodeCost)
			{
				if (resourceCost.amount > this.GetCurrencyAmount(resourceCost.type))
				{
					return false;
				}
			}
			this.UnlockNode(upgrade);
			return true;
		}
	}

	// Token: 0x06000784 RID: 1924 RVA: 0x00029654 File Offset: 0x00027854
	private SIProgression.SINode GetNodeFromID(string id)
	{
		foreach (KeyValuePair<SIUpgradeType, SIProgression.SINode> keyValuePair in this.siNodes)
		{
			if (keyValuePair.Value.id == id)
			{
				return keyValuePair.Value;
			}
		}
		return default(SIProgression.SINode);
	}

	// Token: 0x06000785 RID: 1925 RVA: 0x000296CC File Offset: 0x000278CC
	private void UpdateCurrencyOnPlayer()
	{
		foreach (SIResource.ResourceType resourceType in Enumerable.ToList<SIResource.ResourceType>(this.resourceDict.Keys))
		{
			int num = 0;
			try
			{
				num = this.GetCurrencyAmount(resourceType);
			}
			catch
			{
			}
			this.resourceDict[resourceType] = num;
		}
		SIPlayer.SetAndBroadcastProgression();
		if (!this.ClientReady && this.questSourceList != null)
		{
			this.ClientReady = true;
			Action onClientReady = this.OnClientReady;
			if (onClientReady == null)
			{
				return;
			}
			onClientReady.Invoke();
		}
	}

	// Token: 0x06000786 RID: 1926 RVA: 0x00029778 File Offset: 0x00027978
	private void UpdateUnlockOnPlayer()
	{
		SIPlayer localPlayer = SIPlayer.LocalPlayer;
		this.techTreeSO.EnsureInitialized();
		foreach (KeyValuePair<SIUpgradeType, SIProgression.SINode> keyValuePair in this.siNodes)
		{
			SIUpgradeType key = keyValuePair.Key;
			if (key >= SIUpgradeType.Thruster_Unlock)
			{
				this.unlockedTechTreeData[key.GetPageId()][key.GetNodeId()] = keyValuePair.Value.unlocked;
			}
		}
		SIPlayer.SetAndBroadcastProgression();
		if (!this.ClientReady && this.questSourceList != null)
		{
			this.ClientReady = true;
			Action onClientReady = this.OnClientReady;
			if (onClientReady == null)
			{
				return;
			}
			onClientReady.Invoke();
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x06000787 RID: 1927 RVA: 0x00029830 File Offset: 0x00027A30
	public int[] ActiveQuestIds
	{
		get
		{
			return this.activeQuestIds;
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000788 RID: 1928 RVA: 0x00029838 File Offset: 0x00027A38
	public int[] ActiveQuestProgresses
	{
		get
		{
			return this.activeQuestProgresses;
		}
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x06000789 RID: 1929 RVA: 0x00029840 File Offset: 0x00027A40
	public bool DailyLimitedTurnedIn
	{
		get
		{
			return this.dailyLimitedTurnedIn;
		}
	}

	// Token: 0x0600078A RID: 1930 RVA: 0x00029848 File Offset: 0x00027A48
	public static void InitializeQuests()
	{
		SIProgression.Instance._InitializeQuests();
	}

	// Token: 0x0600078B RID: 1931 RVA: 0x00029854 File Offset: 0x00027A54
	private void ProcessAllQuests(Action<RotatingQuest> action)
	{
		foreach (RotatingQuest rotatingQuest in this.questSourceList.quests)
		{
			action.Invoke(rotatingQuest);
		}
	}

	// Token: 0x0600078C RID: 1932 RVA: 0x000298AC File Offset: 0x00027AAC
	private void QuestLoadPostProcess(RotatingQuest quest)
	{
		quest.SetRequiredZone();
		if (quest.requiredZones.Count == 1 && quest.requiredZones[0] == GTZone.none)
		{
			quest.requiredZones.Clear();
		}
		quest.isQuestActive = true;
	}

	// Token: 0x0600078D RID: 1933 RVA: 0x000298E4 File Offset: 0x00027AE4
	private void QuestSavePreProcess(RotatingQuest quest)
	{
		if (quest.requiredZones.Count == 0)
		{
			quest.requiredZones.Add(GTZone.none);
		}
	}

	// Token: 0x0600078E RID: 1934 RVA: 0x00029900 File Offset: 0x00027B00
	private void _InitializeQuests()
	{
		ProgressionManager.Instance.GetActiveSIQuests(new Action<List<RotatingQuest>>(this.LoadQuestsFromServer), null);
	}

	// Token: 0x0600078F RID: 1935 RVA: 0x0002991C File Offset: 0x00027B1C
	public void LoadQuestsFromServer(List<RotatingQuest> serverQuests)
	{
		if (serverQuests == null || serverQuests.Count == 0)
		{
			Debug.LogError("[SIProgression] Server returned no quests");
			this.LoadQuestsFromLocalJson();
		}
		else
		{
			this.questSourceList = new SIProgression.SIQuestsList
			{
				quests = serverQuests
			};
			this.ProcessAllQuests(new Action<RotatingQuest>(this.QuestLoadPostProcess));
		}
		this.LoadQuestProgress();
		if (!this.questsInitialized)
		{
			ProgressionManager.Instance.GetSIQuestStatus(new Action<ProgressionManager.UserQuestsStatusResponse>(this.ApplyServerQuestsStatus), null);
		}
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x00029990 File Offset: 0x00027B90
	private void LoadQuestsFromLocalJson()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("TestingSuperInfectionQuests");
		this.LoadQuestsFromJson(textAsset.text);
		this.ProcessAllQuests(new Action<RotatingQuest>(this.QuestLoadPostProcess));
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x000299C6 File Offset: 0x00027BC6
	public void SliceUpdate()
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		if (activeSuperInfectionManager == null || !activeSuperInfectionManager.IsZoneReady())
		{
			return;
		}
		if (!this.questsInitialized)
		{
			return;
		}
		this.CheckTimeCrossover();
		this.SaveQuestProgress();
		this.CheckTelemetry();
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x000299FA File Offset: 0x00027BFA
	private void CheckTimeCrossover()
	{
		this.CheckTimeCrossoverServer();
	}

	// Token: 0x06000793 RID: 1939 RVA: 0x00029A04 File Offset: 0x00027C04
	private void CheckTimeCrossoverServer()
	{
		DateTime utcNow = DateTime.UtcNow;
		DateTime dateTime = utcNow.Date + this.CROSSOVER_TIME_OF_DAY;
		if (dateTime > utcNow)
		{
			dateTime = dateTime.AddDays(-1.0);
		}
		if ((dateTime - this.lastQuestGrantTime).Ticks <= 0L)
		{
			return;
		}
		this.lastQuestGrantTime = utcNow.Date + this.CROSSOVER_TIME_OF_DAY;
		ProgressionManager.Instance.GetSIQuestStatus(new Action<ProgressionManager.UserQuestsStatusResponse>(this.ApplyServerQuestsStatus), null);
	}

	// Token: 0x06000794 RID: 1940 RVA: 0x00029A8C File Offset: 0x00027C8C
	public static void StaticSaveQuestProgress()
	{
		SIProgression.Instance.SaveQuestProgress();
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x00029A98 File Offset: 0x00027C98
	public void LoadQuestProgress()
	{
		this.LoadQuestProgressServer();
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x00029AA0 File Offset: 0x00027CA0
	public void SaveQuestProgress()
	{
		this.SaveQuestProgressServer();
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x00029AA8 File Offset: 0x00027CA8
	public void LoadQuestProgressServer()
	{
		int num = 0;
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			int @int = PlayerPrefs.GetInt(string.Format("{0}{1}", "v1_Rotating_Quest_Daily_ID_Key", i), -1);
			int int2 = PlayerPrefs.GetInt(string.Format("{0}{1}", "v1_Rotating_Quest_Daily_Progress_Key", i), -1);
			this.activeQuestIds[i] = @int;
			this.activeQuestProgresses[i] = int2;
			if (@int != -1)
			{
				RotatingQuest questById = this.questSourceList.GetQuestById(@int);
				if (questById == null || !questById.isQuestActive)
				{
					this.activeQuestIds[i] = -1;
					this.activeQuestProgresses[i] = -1;
				}
				else
				{
					num++;
					questById.ApplySavedProgress(int2);
				}
			}
		}
		this.bonusProgress = PlayerPrefs.GetInt("v1_SIProgression:bonusProgress", 0);
		this.CopySaveDataToDiff();
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x00029B70 File Offset: 0x00027D70
	public void SaveQuestProgressServer()
	{
		int num = 0;
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			if (num >= this.stashedQuests)
			{
				this.activeQuestIds[i] = -1;
				this.activeQuestProgresses[i] = 0;
			}
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			if (questById == null || !questById.isQuestActive)
			{
				this.activeQuestIds[i] = -1;
				this.activeQuestProgresses[i] = 0;
			}
			else
			{
				num++;
			}
			int num2 = -1;
			int num3 = 0;
			if (questById != null)
			{
				num2 = questById.questID;
				num3 = questById.GetProgress();
			}
			this.activeQuestProgresses[i] = num3;
			if (num2 != this.activeQuestIdsDiff[i])
			{
				PlayerPrefs.SetInt(string.Format("{0}{1}", "v1_Rotating_Quest_Daily_ID_Key", i), num2);
			}
			if (num3 != this.activeQuestProgressesDiff[i])
			{
				PlayerPrefs.SetInt(string.Format("{0}{1}", "v1_Rotating_Quest_Daily_Progress_Key", i), num3);
			}
		}
		if (this.bonusProgress != this.bonusProgressDiff)
		{
			PlayerPrefs.SetInt("v1_SIProgression:bonusProgress", this.bonusProgress);
		}
		PlayerPrefs.Save();
		this.CopySaveDataToDiff();
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x00029C84 File Offset: 0x00027E84
	public void CopySaveDataToDiff()
	{
		this.lastQuestGrantTimeDiff = this.lastQuestGrantTime;
		this.stashedQuestsDiff = this.stashedQuests;
		this.stashedBonusPointsDiff = this.stashedBonusPoints;
		this.bonusProgressDiff = this.bonusProgress;
		int[] array = new int[6];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.resourceDict[(SIResource.ResourceType)i];
		}
		SIProgression._SafeShallowCopyArray<int>(array, ref this.resourceArrayDiff);
		SIProgression._SafeShallowCopyArray<int>(this.limitedDepositTimeArray, ref this.limitedDepositTimeDiff);
		SIProgression._SafeShallowCopyArray<int>(this.activeQuestIds, ref this.activeQuestIdsDiff);
		SIProgression._SafeShallowCopyArray<int>(this.activeQuestProgresses, ref this.activeQuestProgressesDiff);
		if (this.unlockedTechTreeDataDiff == null || this.unlockedTechTreeDataDiff.Length != this.unlockedTechTreeData.Length)
		{
			this.unlockedTechTreeDataDiff = new bool[this.unlockedTechTreeData.Length][];
		}
		for (int j = 0; j < this.unlockedTechTreeData.Length; j++)
		{
			SIProgression._SafeShallowCopyArray<bool>(this.unlockedTechTreeData[j], ref this.unlockedTechTreeDataDiff[j]);
		}
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x00029D7D File Offset: 0x00027F7D
	private static void _SafeShallowCopyArray<T>(T[] sourceArray, ref T[] ref_destinationArray)
	{
		if (ref_destinationArray == null || ref_destinationArray.Length != sourceArray.Length)
		{
			ref_destinationArray = new T[sourceArray.Length];
		}
		Array.Copy(sourceArray, ref_destinationArray, sourceArray.Length);
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x00029DA4 File Offset: 0x00027FA4
	public int[] GetResourceArray()
	{
		int[] array = new int[this.resourceDict.Count];
		for (int i = 0; i < this.resourceDict.Count; i++)
		{
			array[i] = this.resourceDict[(SIResource.ResourceType)i];
		}
		return array;
	}

	// Token: 0x0600079C RID: 1948 RVA: 0x00029DE8 File Offset: 0x00027FE8
	public void SetResourceArray(int[] resourceArray)
	{
		for (int i = 0; i < resourceArray.Length; i++)
		{
			this.resourceDict[(SIResource.ResourceType)i] = resourceArray[i];
		}
	}

	// Token: 0x0600079D RID: 1949 RVA: 0x00029E12 File Offset: 0x00028012
	public void HandleQuestCompleted(int questID)
	{
		this.UpdateQuestProgresses();
		SIPlayer.SetAndBroadcastProgression();
		SIPlayer.LocalPlayer.questCompleteCelebrate.SetActive(true);
	}

	// Token: 0x0600079E RID: 1950 RVA: 0x00029E30 File Offset: 0x00028030
	public void HandleQuestProgressChanged(bool initialLoad)
	{
		if (this.UpdateQuestProgresses())
		{
			SIPlayer.SetAndBroadcastProgression();
		}
	}

	// Token: 0x0600079F RID: 1951 RVA: 0x00029E40 File Offset: 0x00028040
	private bool UpdateQuestProgresses()
	{
		bool result = false;
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			int num = 0;
			if (questById != null)
			{
				num = questById.GetProgress();
				if (questById.questType != QuestType.moveDistance || this.activeQuestProgresses[i] / 100 != num / 100)
				{
					result = true;
				}
			}
			this.activeQuestProgresses[i] = num;
		}
		this.SaveQuestProgress();
		return result;
	}

	// Token: 0x060007A0 RID: 1952 RVA: 0x00029EB0 File Offset: 0x000280B0
	public void AttemptIncrementResource(SIResource.ResourceType resource)
	{
		ProgressionManager.Instance.IncrementSIResource(resource.ToString(), new Action<string>(this.OnSuccessfulIncrementResource), delegate(string err)
		{
			Debug.LogError(err);
		});
	}

	// Token: 0x060007A1 RID: 1953 RVA: 0x00029EFF File Offset: 0x000280FF
	private void OnSuccessfulIncrementResource(string resourceStr)
	{
		if (Enum.Parse<SIResource.ResourceType>(resourceStr) == SIResource.ResourceType.TechPoint)
		{
			SIPlayer.LocalPlayer.TechPointGrantedCelebrate();
		}
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x060007A2 RID: 1954 RVA: 0x00029F20 File Offset: 0x00028120
	public void AttemptRedeemCompletedQuest(int questIndex)
	{
		RotatingQuest quest = this.questSourceList.GetQuestById(this.activeQuestIds[questIndex]);
		if (quest == null || this.activeQuestIds[questIndex] == -1)
		{
			return;
		}
		if (!quest.isQuestComplete)
		{
			return;
		}
		if (this.redeemingQuestInProgress[questIndex])
		{
			return;
		}
		this.redeemingQuestInProgress[questIndex] = true;
		ProgressionManager.Instance.CompleteSIQuest(quest.questID, delegate(ProgressionManager.UserQuestsStatusResponse status)
		{
			this.OnSuccessfulQuestRedeem(questIndex, quest, status);
		}, delegate(string err)
		{
			if (err.Contains("409") || err.Contains("404"))
			{
				this.OnInvalidQuestRedeemAttempt(questIndex, quest);
			}
			this.redeemingQuestInProgress[questIndex] = false;
			Debug.LogError(err);
		});
	}

	// Token: 0x060007A3 RID: 1955 RVA: 0x00029FD4 File Offset: 0x000281D4
	private void OnSuccessfulQuestRedeem(int questIndex, RotatingQuest quest, ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
	{
		this.activeQuestIds[questIndex] = -1;
		this.activeQuestProgresses[questIndex] = 0;
		quest.ApplySavedProgress(0);
		this.redeemingQuestInProgress[questIndex] = false;
		Dictionary<SIResource.ResourceType, int> dictionary = this.resourceDict;
		int num = dictionary[SIResource.ResourceType.TechPoint];
		dictionary[SIResource.ResourceType.TechPoint] = num + 1;
		this.ApplyServerQuestsStatus(userQuestsStatus);
		SIPlayer.LocalPlayer.TechPointGrantedCelebrate();
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x060007A4 RID: 1956 RVA: 0x0002A035 File Offset: 0x00028235
	private void OnInvalidQuestRedeemAttempt(int questIndex, RotatingQuest quest)
	{
		this.activeQuestIds[questIndex] = -1;
		this.activeQuestProgresses[questIndex] = 0;
		quest.ApplySavedProgress(0);
		ProgressionManager.Instance.GetSIQuestStatus(new Action<ProgressionManager.UserQuestsStatusResponse>(this.ApplyServerQuestsStatus), null);
	}

	// Token: 0x060007A5 RID: 1957 RVA: 0x0002A067 File Offset: 0x00028267
	public void AttemptRedeemBonusPoint()
	{
		ProgressionManager.Instance.CompleteSIBonus(delegate(ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
		{
			this.OnSuccessfulBonusRedeem(userQuestsStatus);
		}, delegate(string err)
		{
			Debug.LogError(err);
		});
	}

	// Token: 0x060007A6 RID: 1958 RVA: 0x0002A09E File Offset: 0x0002829E
	private void OnSuccessfulBonusRedeem(ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
	{
		this.bonusProgress = 0;
		this.ApplyServerQuestsStatus(userQuestsStatus);
		SIPlayer.LocalPlayer.TechPointGrantedCelebrate();
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x060007A7 RID: 1959 RVA: 0x0002A0C2 File Offset: 0x000282C2
	public void AttemptCollectMonkeIdol()
	{
		ProgressionManager.Instance.CollectSIIdol(new Action<ProgressionManager.UserQuestsStatusResponse>(this.OnSuccessfulMonkeIdolRedeem), delegate(string err)
		{
			Debug.LogError(err);
		});
	}

	// Token: 0x060007A8 RID: 1960 RVA: 0x0002A0F9 File Offset: 0x000282F9
	private void OnSuccessfulMonkeIdolRedeem(ProgressionManager.UserQuestsStatusResponse userQuestsStatus)
	{
		this.ApplyServerQuestsStatus(userQuestsStatus);
		this.limitedDepositTimeArray[1] = 1;
		SIPlayer.LocalPlayer.TechPointGrantedCelebrate();
		ProgressionManager.Instance.RefreshUserInventory();
	}

	// Token: 0x060007A9 RID: 1961 RVA: 0x0002A11F File Offset: 0x0002831F
	public void GetBonusProgress()
	{
		this.bonusProgress++;
	}

	// Token: 0x060007AA RID: 1962 RVA: 0x0002A130 File Offset: 0x00028330
	public void SetupAllQuestEventListeners()
	{
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			if (questById != null && this.activeQuestIds[i] != -1)
			{
				questById.questManager = this;
				if (!questById.isQuestComplete)
				{
					questById.AddEventListener();
				}
			}
		}
	}

	// Token: 0x060007AB RID: 1963 RVA: 0x0002A187 File Offset: 0x00028387
	public static void StaticClearAllQuestEventListeners()
	{
		SIProgression.Instance.ClearAllQuestEventListeners();
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x0002A194 File Offset: 0x00028394
	public void ClearAllQuestEventListeners()
	{
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			if (questById != null)
			{
				questById.RemoveEventListener();
			}
		}
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x0002A1D2 File Offset: 0x000283D2
	public void LoadQuestsFromJson(string jsonString)
	{
		this.questSourceList = JsonConvert.DeserializeObject<SIProgression.SIQuestsList>(jsonString);
		this.ProcessAllQuests(new Action<RotatingQuest>(this.QuestLoadPostProcess));
	}

	// Token: 0x060007AE RID: 1966 RVA: 0x0002A1F2 File Offset: 0x000283F2
	public void RefreshActiveQuests()
	{
		this.ClearAllQuestEventListeners();
		this.SelectActiveQuests();
		this.HandleQuestProgressChanged(true);
		this.SetupAllQuestEventListeners();
	}

	// Token: 0x060007AF RID: 1967 RVA: 0x0002A210 File Offset: 0x00028410
	private void SelectActiveQuests()
	{
		int num = 0;
		for (int i = 0; i < this.activeQuestIds.Length; i++)
		{
			RotatingQuest questById = this.questSourceList.GetQuestById(this.activeQuestIds[i]);
			if (questById != null && questById.isQuestActive && num < this.stashedQuests)
			{
				this.activeQuestCategories[i] = questById.category;
				num++;
			}
			else
			{
				this.activeQuestIds[i] = -1;
				this.activeQuestProgresses[i] = 0;
				this.activeQuestCategories[i] = QuestCategory.NONE;
				if (questById != null)
				{
					questById.ApplySavedProgress(0);
				}
			}
		}
		int num2 = Mathf.Max(0, this.stashedQuests);
		int num3 = 0;
		while (num3 < this.activeQuestIds.Length && num < num2)
		{
			RotatingQuest questById2 = this.questSourceList.GetQuestById(this.activeQuestIds[num3]);
			if (questById2 == null || !questById2.isQuestActive)
			{
				int num4 = Random.Range(0, this.questSourceList.quests.Count);
				for (int j = 0; j < this.questSourceList.quests.Count; j++)
				{
					int num5 = (num4 + j) % this.questSourceList.quests.Count;
					RotatingQuest questById3 = this.questSourceList.GetQuestById(num5);
					if (questById3 != null && questById3.isQuestActive && this.<SelectActiveQuests>g__GetMatchingCategoryCount|171_0(questById3) < this.perCategoryQuestLimit)
					{
						bool flag = false;
						for (int k = 0; k < this.activeQuestIds.Length; k++)
						{
							if (num5 == this.activeQuestIds[k])
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							this.activeQuestIds[num3] = num5;
							this.activeQuestCategories[num3] = questById3.category;
							questById3.ApplySavedProgress(0);
							this.activeQuestProgresses[num3] = 0;
							num++;
							break;
						}
					}
				}
			}
			num3++;
		}
		this.SaveQuestProgress();
	}

	// Token: 0x060007B0 RID: 1968 RVA: 0x0002A3D4 File Offset: 0x000285D4
	private void SelectCurrentTurnInDate()
	{
		DateTime dateTime;
		dateTime..ctor(2025, 1, 10, 18, 0, 0, 1);
		TimeSpan timeSpan = TimeSpan.FromHours(-8.0);
		DateTime dateTime2;
		dateTime2..ctor(1, 1, 1, 0, 0, 0);
		DateTime dateTime3;
		dateTime3..ctor(2006, 12, 31, 0, 0, 0);
		TimeSpan timeSpan2 = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 4, 1, 0);
		TimeZoneInfo.TransitionTime transitionTime2 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 10, 5, 0);
		DateTime dateTime4;
		dateTime4..ctor(2007, 1, 1, 0, 0, 0);
		DateTime dateTime5;
		dateTime5..ctor(9999, 12, 31, 0, 0, 0);
		TimeSpan timeSpan3 = TimeSpan.FromHours(1.0);
		TimeZoneInfo.TransitionTime transitionTime3 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 3, 2, 0);
		TimeZoneInfo.TransitionTime transitionTime4 = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 2, 0, 0), 11, 1, 0);
		TimeZoneInfo timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Pacific Standard Time", timeSpan, "Pacific Standard Time", "Pacific Standard Time", "Pacific Standard Time", new TimeZoneInfo.AdjustmentRule[]
		{
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateTime2, dateTime3, timeSpan2, transitionTime, transitionTime2),
			TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(dateTime4, dateTime5, timeSpan3, transitionTime3, transitionTime4)
		});
		if (timeZoneInfo != null && timeZoneInfo.IsDaylightSavingTime(DateTime.UtcNow - timeSpan))
		{
			dateTime -= TimeSpan.FromHours(1.0);
		}
		int days = (DateTime.UtcNow - dateTime).Days;
	}

	// Token: 0x060007B1 RID: 1969 RVA: 0x0002A54C File Offset: 0x0002874C
	public bool TryDepositResources(SIResource.ResourceType type, int count)
	{
		int resourceMaxCap = this.GetResourceMaxCap(type);
		int num = this.resourceDict[type];
		if (resourceMaxCap == num)
		{
			return false;
		}
		count = Math.Min(count, resourceMaxCap - num);
		Dictionary<SIResource.ResourceType, int> dictionary = this.resourceDict;
		dictionary[type] += count;
		this.AttemptIncrementResource(type);
		return true;
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0002A5A0 File Offset: 0x000287A0
	public int GetResourceMaxCap(SIResource.ResourceType type)
	{
		return this.resourceCapsArray[(int)type];
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x0002A5AA File Offset: 0x000287AA
	public bool IsLimitedDepositAvailable(SIResource.LimitedDepositType limitedDepositType)
	{
		return !this.dailyLimitedTurnedIn;
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x0002A5B5 File Offset: 0x000287B5
	public void ApplyLimitedDepositTime(SIResource.LimitedDepositType limitedDepositType)
	{
		if (limitedDepositType == SIResource.LimitedDepositType.None)
		{
			return;
		}
		this.AttemptCollectMonkeIdol();
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x0002A5C1 File Offset: 0x000287C1
	private void OnDestroy()
	{
		this.SaveQuestProgress();
	}

	// Token: 0x060007B6 RID: 1974 RVA: 0x0002A5C9 File Offset: 0x000287C9
	public bool GetOnlineNode(SIUpgradeType type, out SIProgression.SINode node)
	{
		if (!this._treeReady)
		{
			node = this.emptyNode;
			return false;
		}
		return this.siNodes.TryGetValue(type, ref node);
	}

	// Token: 0x060007B7 RID: 1975 RVA: 0x0002A5EE File Offset: 0x000287EE
	public static bool ResourcesMaxed()
	{
		return SIProgression.Instance._ResourcesMaxed();
	}

	// Token: 0x060007B8 RID: 1976 RVA: 0x0002A5FC File Offset: 0x000287FC
	public bool _ResourcesMaxed()
	{
		foreach (KeyValuePair<SIResource.ResourceType, int> keyValuePair in this.resourceDict)
		{
			if (keyValuePair.Key != SIResource.ResourceType.TechPoint && keyValuePair.Value < this.GetResourceMaxCap(keyValuePair.Key))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0002A670 File Offset: 0x00028870
	public void CheckTelemetry()
	{
		SuperInfectionGame instance = SuperInfectionGame.instance;
		if (instance == null)
		{
			return;
		}
		if (!instance.ValidGameMode())
		{
			this.timeTelemetryLastChecked = Time.time;
			return;
		}
		float num = Time.time - this.timeTelemetryLastChecked;
		this.timeTelemetryLastChecked = Time.time;
		this.totalPlayTime += num;
		if (NetworkSystem.Instance.InRoom)
		{
			this.roomPlayTime += num;
		}
		this.intervalPlayTime += num;
		for (int i = 0; i < 11; i++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)i;
			if (SIProgression.Instance.heldOrSnappedByGadgetPageType[sitechTreePageId] > 0)
			{
				Dictionary<SITechTreePageId, float> dictionary = this.timeUsingGadgetTypeInterval;
				SITechTreePageId sitechTreePageId2 = sitechTreePageId;
				dictionary[sitechTreePageId2] += num;
				dictionary = this.timeUsingGadgetTypeTotal;
				sitechTreePageId2 = sitechTreePageId;
				dictionary[sitechTreePageId2] += num;
			}
		}
		if (SIProgression.Instance.heldOrSnappedOwnGadgets > 0)
		{
			this.timeUsingOwnGadgetsInterval += num;
			this.timeUsingOwnGadgetsTotal += num;
		}
		if (SIProgression.Instance.heldOrSnappedOthersGadgets > 0)
		{
			this.timeUsingOthersGadgetsInterval += num;
			this.timeUsingOthersGadgetsTotal += num;
		}
		if (this.lastTelemetrySent + this.telemetryCooldown < Time.time)
		{
			this.lastTelemetrySent = Time.time;
			this.SaveTelemetryData();
			GorillaTelemetry.SuperInfectionEvent(false, this.totalPlayTime, this.roomPlayTime, Time.time, this.intervalPlayTime, this.activeTerminalTimeTotal, this.activeTerminalTimeInterval, this.timeUsingGadgetTypeTotal, this.timeUsingGadgetTypeInterval, this.timeUsingOwnGadgetsTotal, this.timeUsingOwnGadgetsInterval, this.timeUsingOthersGadgetsTotal, this.timeUsingOthersGadgetsInterval, this.tagsUsingGadgetTypeTotal, this.tagsUsingGadgetTypeInterval, this.tagsHoldingOwnGadgetTotal, this.tagsHoldingOwnGadgetInterval, this.tagsHoldingOthersGadgetTotal, this.tagsHoldingOthersGadgetInterval, this.resourcesCollectedTotal, this.resourcesCollectedInterval, this.roundsPlayedTotal, this.roundsPlayedInterval, SIProgression.Instance.unlockedTechTreeData, NetworkSystem.Instance.RoomPlayerCount);
			this.ResetTelemetryIntervalData();
		}
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x0002A86C File Offset: 0x00028A6C
	public void SendTelemetryData()
	{
		if (Time.time < this.lastDisconnectTelemetrySent + this.minDisconnectTelemetryCooldown)
		{
			return;
		}
		this.lastDisconnectTelemetrySent = Time.time;
		this.SaveTelemetryData();
		GorillaTelemetry.SuperInfectionEvent(true, this.totalPlayTime, this.roomPlayTime, Time.time, this.intervalPlayTime, this.activeTerminalTimeTotal, this.activeTerminalTimeInterval, this.timeUsingGadgetTypeTotal, this.timeUsingGadgetTypeInterval, this.timeUsingOwnGadgetsTotal, this.timeUsingOwnGadgetsInterval, this.timeUsingOthersGadgetsTotal, this.timeUsingOthersGadgetsInterval, this.tagsUsingGadgetTypeTotal, this.tagsUsingGadgetTypeInterval, this.tagsHoldingOwnGadgetTotal, this.tagsHoldingOwnGadgetInterval, this.tagsHoldingOthersGadgetTotal, this.tagsHoldingOthersGadgetInterval, this.resourcesCollectedTotal, this.resourcesCollectedInterval, this.roundsPlayedTotal, this.roundsPlayedInterval, SIProgression.Instance.unlockedTechTreeData, NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.RoomPlayerCount : -1);
		this.ResetTelemetryIntervalData();
		this.roomPlayTime = 0f;
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x0002A95C File Offset: 0x00028B5C
	public void SendPurchaseResourcesData()
	{
		this.SaveTelemetryData();
		GorillaTelemetry.SuperInfectionEvent("si_fill_resources", 500, -1, this.totalPlayTime, this.roomPlayTime, Time.time);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x0002A985 File Offset: 0x00028B85
	public void SendPurchaseTechPointsData(int techPointsPurchased)
	{
		this.SaveTelemetryData();
		GorillaTelemetry.SuperInfectionEvent("si_purchase_tech_points", techPointsPurchased * 100, techPointsPurchased, this.totalPlayTime, this.roomPlayTime, Time.time);
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x0002A9B0 File Offset: 0x00028BB0
	public void LoadSavedTelemetryData()
	{
		this.totalPlayTime = PlayerPrefs.GetFloat("super_infection_total_play_time", 0f);
		for (int i = 0; i < 11; i++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)i;
			this.timeUsingGadgetTypeTotal[sitechTreePageId] = PlayerPrefs.GetFloat("super_infection_time_holding_gadget_type_total" + sitechTreePageId.GetName<SITechTreePageId>(), 0f);
			this.tagsUsingGadgetTypeTotal[sitechTreePageId] = PlayerPrefs.GetInt("super_infection_tags_holding_gadget_type_total" + sitechTreePageId.GetName<SITechTreePageId>(), 0);
		}
		this.activeTerminalTimeTotal = PlayerPrefs.GetFloat("super_infection_terminal_total_time", 0f);
		this.tagsHoldingOthersGadgetTotal = PlayerPrefs.GetInt("super_infection_tags_holding_others_gadgets_total", 0);
		this.tagsHoldingOwnGadgetTotal = PlayerPrefs.GetInt("super_infection_tags_holding_own_gadgets_total", 0);
		for (int j = 0; j < 6; j++)
		{
			SIResource.ResourceType resourceType = (SIResource.ResourceType)j;
			this.resourcesCollectedTotal[resourceType] = PlayerPrefs.GetInt("super_infection_resource_type_collected_total" + resourceType.GetName<SIResource.ResourceType>(), 0);
		}
		this.roundsPlayedTotal = PlayerPrefs.GetInt("super_infection_rounds_played_total", 0);
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x0002AAA4 File Offset: 0x00028CA4
	private void SaveTelemetryData()
	{
		PlayerPrefs.SetFloat("super_infection_total_play_time", this.totalPlayTime);
		for (int i = 0; i < 11; i++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)i;
			PlayerPrefs.SetFloat("super_infection_time_holding_gadget_type_total" + sitechTreePageId.GetName<SITechTreePageId>(), this.timeUsingGadgetTypeTotal[sitechTreePageId]);
			PlayerPrefs.SetInt("super_infection_tags_holding_gadget_type_total" + sitechTreePageId.GetName<SITechTreePageId>(), this.tagsUsingGadgetTypeTotal[sitechTreePageId]);
		}
		PlayerPrefs.SetFloat("super_infection_terminal_total_time", this.activeTerminalTimeTotal);
		PlayerPrefs.SetInt("super_infection_tags_holding_others_gadgets_total", this.tagsHoldingOthersGadgetTotal);
		PlayerPrefs.SetInt("super_infection_tags_holding_own_gadgets_total", this.tagsHoldingOwnGadgetTotal);
		for (int j = 0; j < 6; j++)
		{
			SIResource.ResourceType resourceType = (SIResource.ResourceType)j;
			PlayerPrefs.SetInt("super_infection_resource_type_collected_total" + resourceType.GetName<SIResource.ResourceType>(), this.resourcesCollectedTotal[resourceType]);
		}
		PlayerPrefs.SetInt("super_infection_rounds_played_total", this.roundsPlayedTotal);
		PlayerPrefs.Save();
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x0002AB88 File Offset: 0x00028D88
	public void ResetTelemetryIntervalData()
	{
		this.lastTelemetrySent = Time.time;
		this.intervalPlayTime = 0f;
		this.activeTerminalTimeInterval = 0f;
		for (int i = 0; i < 11; i++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)i;
			this.timeUsingGadgetTypeInterval[sitechTreePageId] = 0f;
			this.tagsUsingGadgetTypeInterval[sitechTreePageId] = 0;
		}
		this.timeUsingOwnGadgetsInterval = 0f;
		this.timeUsingOthersGadgetsInterval = 0f;
		this.tagsHoldingOthersGadgetInterval = 0;
		this.tagsHoldingOwnGadgetInterval = 0;
		for (int j = 0; j < 6; j++)
		{
			SIResource.ResourceType resourceType = (SIResource.ResourceType)j;
			this.resourcesCollectedInterval[resourceType] = 0;
		}
		this.roundsPlayedInterval = 0;
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x0002AC2C File Offset: 0x00028E2C
	public void HandleTagTelemetry(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (taggingPlayer.ActorNumber != SIPlayer.LocalPlayer.ActorNr)
		{
			return;
		}
		for (int i = 0; i < 11; i++)
		{
			SITechTreePageId sitechTreePageId = (SITechTreePageId)i;
			if (SIProgression.Instance.heldOrSnappedByGadgetPageType[sitechTreePageId] > 0)
			{
				Dictionary<SITechTreePageId, int> dictionary = this.tagsUsingGadgetTypeTotal;
				SITechTreePageId sitechTreePageId2 = sitechTreePageId;
				int num = dictionary[sitechTreePageId2];
				dictionary[sitechTreePageId2] = num + 1;
				Dictionary<SITechTreePageId, int> dictionary2 = this.tagsUsingGadgetTypeInterval;
				sitechTreePageId2 = sitechTreePageId;
				num = dictionary2[sitechTreePageId2];
				dictionary2[sitechTreePageId2] = num + 1;
			}
		}
		if (SIProgression.Instance.heldOrSnappedOwnGadgets > 0)
		{
			this.tagsHoldingOwnGadgetInterval++;
			this.tagsHoldingOwnGadgetTotal++;
		}
		if (SIProgression.Instance.heldOrSnappedOthersGadgets > 0)
		{
			this.tagsHoldingOthersGadgetInterval++;
			this.tagsHoldingOthersGadgetTotal++;
		}
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x0002ACF4 File Offset: 0x00028EF4
	public void UpdateHeldGadgetsTelemetry(SITechTreePageId id, bool isMine, int changeAmount)
	{
		Dictionary<SITechTreePageId, int> dictionary = SIProgression.Instance.heldOrSnappedByGadgetPageType;
		dictionary[id] += changeAmount;
		if (isMine)
		{
			SIProgression.Instance.heldOrSnappedOwnGadgets += changeAmount;
			return;
		}
		SIProgression.Instance.heldOrSnappedOthersGadgets += changeAmount;
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x0002AD48 File Offset: 0x00028F48
	public void CollectResourceTelemetry(SIResource.ResourceType type, int count)
	{
		Dictionary<SIResource.ResourceType, int> dictionary = this.resourcesCollectedTotal;
		dictionary[type] += count;
		dictionary = this.resourcesCollectedInterval;
		dictionary[type] += count;
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x0002AD87 File Offset: 0x00028F87
	public void AddRoundTelemetry()
	{
		this.roundsPlayedInterval++;
		this.roundsPlayedTotal++;
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x0002AE78 File Offset: 0x00029078
	[CompilerGenerated]
	private int <SelectActiveQuests>g__GetMatchingCategoryCount|171_0(RotatingQuest quest)
	{
		if (quest.category == QuestCategory.NONE)
		{
			return 0;
		}
		int num = 0;
		QuestCategory[] array = this.activeQuestCategories;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == quest.category)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x0400093B RID: 2363
	[SerializeField]
	private SITechTreeSO techTreeSO;

	// Token: 0x0400093C RID: 2364
	[SerializeField]
	private int perCategoryQuestLimit = 1;

	// Token: 0x0400093D RID: 2365
	public Action OnTreeReady;

	// Token: 0x0400093E RID: 2366
	public Action OnInventoryReady;

	// Token: 0x0400093F RID: 2367
	public Action<SIUpgradeType> OnNodeUnlocked;

	// Token: 0x04000941 RID: 2369
	public bool ClientReady;

	// Token: 0x04000942 RID: 2370
	private static Dictionary<SIResource.ResourceType, string> _resourceToString;

	// Token: 0x04000943 RID: 2371
	private const string TREE_NAME = "SI_Gadgets";

	// Token: 0x04000944 RID: 2372
	private Dictionary<SIUpgradeType, SIProgression.SINode> siNodes;

	// Token: 0x04000945 RID: 2373
	private bool _treeReady;

	// Token: 0x04000946 RID: 2374
	private bool _inventoryReady;

	// Token: 0x04000947 RID: 2375
	public Dictionary<SITechTreePageId, int> heldOrSnappedByGadgetPageType = new Dictionary<SITechTreePageId, int>();

	// Token: 0x04000948 RID: 2376
	public int heldOrSnappedOwnGadgets;

	// Token: 0x04000949 RID: 2377
	public int heldOrSnappedOthersGadgets;

	// Token: 0x0400094A RID: 2378
	public float timeTelemetryLastChecked;

	// Token: 0x0400094B RID: 2379
	public float lastTelemetrySent;

	// Token: 0x0400094C RID: 2380
	private float telemetryCooldown = 600f;

	// Token: 0x0400094D RID: 2381
	private float totalPlayTime;

	// Token: 0x0400094E RID: 2382
	private float roomPlayTime;

	// Token: 0x0400094F RID: 2383
	private float intervalPlayTime;

	// Token: 0x04000950 RID: 2384
	[NonSerialized]
	public float activeTerminalTimeTotal;

	// Token: 0x04000951 RID: 2385
	[NonSerialized]
	public float activeTerminalTimeInterval;

	// Token: 0x04000952 RID: 2386
	private Dictionary<SITechTreePageId, float> timeUsingGadgetTypeTotal = new Dictionary<SITechTreePageId, float>();

	// Token: 0x04000953 RID: 2387
	private Dictionary<SITechTreePageId, float> timeUsingGadgetTypeInterval = new Dictionary<SITechTreePageId, float>();

	// Token: 0x04000954 RID: 2388
	private float timeUsingOthersGadgetsTotal;

	// Token: 0x04000955 RID: 2389
	private float timeUsingOthersGadgetsInterval;

	// Token: 0x04000956 RID: 2390
	private float timeUsingOwnGadgetsTotal;

	// Token: 0x04000957 RID: 2391
	private float timeUsingOwnGadgetsInterval;

	// Token: 0x04000958 RID: 2392
	private Dictionary<SITechTreePageId, int> tagsUsingGadgetTypeTotal = new Dictionary<SITechTreePageId, int>();

	// Token: 0x04000959 RID: 2393
	private Dictionary<SITechTreePageId, int> tagsUsingGadgetTypeInterval = new Dictionary<SITechTreePageId, int>();

	// Token: 0x0400095A RID: 2394
	private int tagsHoldingOthersGadgetTotal;

	// Token: 0x0400095B RID: 2395
	private int tagsHoldingOthersGadgetInterval;

	// Token: 0x0400095C RID: 2396
	private int tagsHoldingOwnGadgetTotal;

	// Token: 0x0400095D RID: 2397
	private int tagsHoldingOwnGadgetInterval;

	// Token: 0x0400095E RID: 2398
	private Dictionary<SIResource.ResourceType, int> resourcesCollectedTotal = new Dictionary<SIResource.ResourceType, int>();

	// Token: 0x0400095F RID: 2399
	private Dictionary<SIResource.ResourceType, int> resourcesCollectedInterval = new Dictionary<SIResource.ResourceType, int>();

	// Token: 0x04000960 RID: 2400
	private int roundsPlayedTotal;

	// Token: 0x04000961 RID: 2401
	private int roundsPlayedInterval;

	// Token: 0x04000962 RID: 2402
	private SIProgression.SINode emptyNode;

	// Token: 0x04000963 RID: 2403
	public SIProgression.SIQuestsList questSourceList;

	// Token: 0x04000964 RID: 2404
	private const int STARTING_STASHED_QUESTS = 0;

	// Token: 0x04000965 RID: 2405
	private const int STARTING_STASHED_BONUS_POINTS = 0;

	// Token: 0x04000966 RID: 2406
	public const int SHARED_QUEST_TURNINS_FOR_POINT = 10;

	// Token: 0x04000967 RID: 2407
	public const int NEW_QUESTS_PER_DAY = 3;

	// Token: 0x04000968 RID: 2408
	public const int NEW_BONUS_POINTS_PER_DAY = 1;

	// Token: 0x04000969 RID: 2409
	public const int MAX_STASHED_QUESTS = 6;

	// Token: 0x0400096A RID: 2410
	public const int MAX_STASHED_BONUS_POINTS = 2;

	// Token: 0x0400096B RID: 2411
	public const int MAX_RESOURCE_COUNT = 30;

	// Token: 0x0400096C RID: 2412
	private const int ACTIVE_QUEST_COUNT = 3;

	// Token: 0x0400096D RID: 2413
	private const string kLocalQuestPath = "TestingSuperInfectionQuests";

	// Token: 0x0400096E RID: 2414
	private const string kVersion = "v1_";

	// Token: 0x0400096F RID: 2415
	private const string kLastQuestGrantTime = "v1_SIProgression:lastSharedGrantTime";

	// Token: 0x04000970 RID: 2416
	private const string kBonusProgress = "v1_SIProgression:bonusProgress";

	// Token: 0x04000971 RID: 2417
	private const string kDailyQuestId = "v1_Rotating_Quest_Daily_ID_Key";

	// Token: 0x04000972 RID: 2418
	private const string kDailyQuestProgress = "v1_Rotating_Quest_Daily_Progress_Key";

	// Token: 0x04000973 RID: 2419
	private const string kStashedQuests = "v1_SIProgression:stashedQuests";

	// Token: 0x04000974 RID: 2420
	private const string kStashedBonusPoints = "v1_SIProgression:stashedBonusPoints";

	// Token: 0x04000975 RID: 2421
	private const string kTechTree = "v1_SITechTree:";

	// Token: 0x04000976 RID: 2422
	private const string kLimitedDeposit = "v1_SIResource:LimitedDeposit:";

	// Token: 0x04000977 RID: 2423
	private const string kTechPoints = "v1_SIResource:techPoints";

	// Token: 0x04000978 RID: 2424
	private const string kStrangeWood = "v1_SIResource:strangeWood";

	// Token: 0x04000979 RID: 2425
	private const string kWeirdGear = "v1_SIResource:weirdGear";

	// Token: 0x0400097A RID: 2426
	private const string kVibratingSpring = "v1_SIResource:vibratingSpring";

	// Token: 0x0400097B RID: 2427
	private const string kBouncySand = "v1_SIResource:bouncySand";

	// Token: 0x0400097C RID: 2428
	private const string kFloppyMetal = "v1_SIResource:floppyMetal";

	// Token: 0x0400097D RID: 2429
	private const string kStartingPackageGranted = "v1_SIProgression:startingPackageGranted";

	// Token: 0x0400097E RID: 2430
	public TimeSpan CROSSOVER_TIME_OF_DAY = new TimeSpan(1, 0, 0);

	// Token: 0x0400097F RID: 2431
	public DateTime lastQuestGrantTime;

	// Token: 0x04000980 RID: 2432
	public int stashedQuests;

	// Token: 0x04000981 RID: 2433
	public int completedQuests;

	// Token: 0x04000982 RID: 2434
	public int stashedBonusPoints;

	// Token: 0x04000983 RID: 2435
	public int completedBonusPoints;

	// Token: 0x04000984 RID: 2436
	public int bonusProgress;

	// Token: 0x04000985 RID: 2437
	public int questGrantRefreshCooldown = 28800;

	// Token: 0x04000986 RID: 2438
	public Dictionary<SIResource.ResourceType, int> resourceDict;

	// Token: 0x04000987 RID: 2439
	public int[] limitedDepositTimeArray;

	// Token: 0x04000988 RID: 2440
	public bool[][] unlockedTechTreeData;

	// Token: 0x04000989 RID: 2441
	[SerializeField]
	private int[] activeQuestIds = new int[3];

	// Token: 0x0400098A RID: 2442
	[SerializeField]
	private int[] activeQuestProgresses = new int[3];

	// Token: 0x0400098B RID: 2443
	[SerializeField]
	private QuestCategory[] activeQuestCategories = new QuestCategory[3];

	// Token: 0x0400098C RID: 2444
	private bool dailyLimitedTurnedIn;

	// Token: 0x0400098D RID: 2445
	public SIProgression.SIProgressionResourceCap[] resourceCaps;

	// Token: 0x0400098E RID: 2446
	public int[] resourceCapsArray;

	// Token: 0x0400098F RID: 2447
	private DateTime lastQuestGrantTimeDiff;

	// Token: 0x04000990 RID: 2448
	private int stashedQuestsDiff;

	// Token: 0x04000991 RID: 2449
	private int stashedBonusPointsDiff;

	// Token: 0x04000992 RID: 2450
	private int bonusProgressDiff;

	// Token: 0x04000993 RID: 2451
	private int[] resourceArrayDiff;

	// Token: 0x04000994 RID: 2452
	private int[] limitedDepositTimeDiff;

	// Token: 0x04000995 RID: 2453
	private bool[][] unlockedTechTreeDataDiff;

	// Token: 0x04000996 RID: 2454
	private int[] activeQuestIdsDiff;

	// Token: 0x04000997 RID: 2455
	private int[] activeQuestProgressesDiff;

	// Token: 0x04000998 RID: 2456
	private bool questsInitialized;

	// Token: 0x04000999 RID: 2457
	private bool _startingPackageGranted;

	// Token: 0x0400099A RID: 2458
	private float lastStartingPackageAttemptStarted;

	// Token: 0x0400099B RID: 2459
	private int startingPackageBackupAttempts;

	// Token: 0x0400099C RID: 2460
	private const int STARTING_PACKAGE_MAX_ATTEMPTS = 10;

	// Token: 0x0400099D RID: 2461
	private bool[] redeemingQuestInProgress = new bool[3];

	// Token: 0x0400099E RID: 2462
	private float lastDisconnectTelemetrySent;

	// Token: 0x0400099F RID: 2463
	private float minDisconnectTelemetryCooldown = 60f;

	// Token: 0x02000121 RID: 289
	public struct SINode
	{
		// Token: 0x040009A0 RID: 2464
		public string id;

		// Token: 0x040009A1 RID: 2465
		public bool unlocked;

		// Token: 0x040009A2 RID: 2466
		public Dictionary<SIResource.ResourceType, int> costs;

		// Token: 0x040009A3 RID: 2467
		public List<SIProgression.SINode> parents;

		// Token: 0x040009A4 RID: 2468
		public SIUpgradeType upgradeType;
	}

	// Token: 0x02000122 RID: 290
	[Serializable]
	public class SIQuestsList
	{
		// Token: 0x060007C7 RID: 1991 RVA: 0x0002AEB8 File Offset: 0x000290B8
		public RotatingQuest GetQuestById(int questID)
		{
			foreach (RotatingQuest rotatingQuest in this.quests)
			{
				if (rotatingQuest.questID == questID)
				{
					return rotatingQuest.disable ? null : rotatingQuest;
				}
			}
			return null;
		}

		// Token: 0x040009A5 RID: 2469
		public List<RotatingQuest> quests;
	}

	// Token: 0x02000123 RID: 291
	[Serializable]
	public struct SIProgressionResourceCap
	{
		// Token: 0x040009A6 RID: 2470
		public SIResource.ResourceType resourceType;

		// Token: 0x040009A7 RID: 2471
		public int resourceMax;
	}
}
