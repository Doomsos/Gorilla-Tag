using System;
using System.Collections.Generic;
using System.IO;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200011E RID: 286
public class SIPlayer : MonoBehaviour
{
	// Token: 0x17000088 RID: 136
	// (get) Token: 0x06000741 RID: 1857 RVA: 0x00027CB9 File Offset: 0x00025EB9
	public static SIPlayer LocalPlayer
	{
		get
		{
			return SIPlayer.Get(NetworkSystem.Instance.LocalPlayer.ActorNumber);
		}
	}

	// Token: 0x17000089 RID: 137
	// (get) Token: 0x06000742 RID: 1858 RVA: 0x00027CCF File Offset: 0x00025ECF
	public int ActorNr
	{
		get
		{
			if (!this.gamePlayer.rig.isOfflineVRRig)
			{
				return this.gamePlayer.rig.OwningNetPlayer.ActorNumber;
			}
			return NetworkSystem.Instance.LocalPlayerID;
		}
	}

	// Token: 0x1700008A RID: 138
	// (get) Token: 0x06000743 RID: 1859 RVA: 0x00027D03 File Offset: 0x00025F03
	public SIPlayer.ProgressionData CurrentProgression
	{
		get
		{
			return this.currentProgression;
		}
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x00027D0C File Offset: 0x00025F0C
	private void Awake()
	{
		this.activePlayerGadgets = new List<int>();
		SIPlayer.progressionSO = this.progressionSORef;
		this.clientToAuthorityRPCLimiter = new CallLimiter(25, 1f, 0.5f);
		this.clientToClientRPCLimiter = new CallLimiter(25, 1f, 0.5f);
		this.authorityToClientRPCLimiter = new CallLimiter(25, 1f, 0.5f);
		this.currentProgression = new SIPlayer.ProgressionData(true);
		GamePlayer gamePlayer = this.gamePlayer;
		gamePlayer.OnPlayerLeftZone = (Action)Delegate.Combine(gamePlayer.OnPlayerLeftZone, new Action(this.ClearGadgetsOnLeaveZone));
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x00027DA7 File Offset: 0x00025FA7
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Reset();
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x00027DB7 File Offset: 0x00025FB7
	public void Reset()
	{
		if (SIPlayer.LocalPlayer == this)
		{
			SIProgression.StaticSaveQuestProgress();
			SIProgression.StaticClearAllQuestEventListeners();
		}
		this.lastQuestsAvailableToClaim = 999;
		this.tpParticleSystem.Stop();
		this.netInitialized = false;
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x00002789 File Offset: 0x00000989
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x00027DED File Offset: 0x00025FED
	public bool ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		return true;
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x00027DF0 File Offset: 0x00025FF0
	public static SIPlayer Get(int actorNumber)
	{
		SIPlayer result;
		if (SIPlayer.siPlayerByActorNr.TryGetValue(actorNumber, ref result))
		{
			return result;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(actorNumber);
		if (gamePlayer == null)
		{
			return null;
		}
		SIPlayer.siPlayerByActorNr.Add(actorNumber, gamePlayer.GetComponent<SIPlayer>());
		return SIPlayer.siPlayerByActorNr[actorNumber];
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00027E3C File Offset: 0x0002603C
	public static void ClearPlayerCache()
	{
		SIPlayer.siPlayerByActorNr.Clear();
	}

	// Token: 0x0600074B RID: 1867 RVA: 0x00027E48 File Offset: 0x00026048
	public static SIPlayer Get(VRRig vrRig)
	{
		if (!vrRig)
		{
			return null;
		}
		return vrRig.GetComponent<SIPlayer>();
	}

	// Token: 0x0600074C RID: 1868 RVA: 0x00027E5C File Offset: 0x0002605C
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player)
	{
		for (int i = 0; i < 6; i++)
		{
			writer.Write(this.CurrentProgression.resourceArray[i]);
		}
		for (int j = 0; j < 2; j++)
		{
			writer.Write(this.CurrentProgression.limitedDepositTimeArray[j]);
		}
		for (int k = 0; k < SIPlayer.progressionSO.TreePageCount; k++)
		{
			for (int l = 0; l < SIPlayer.progressionSO.TreeNodeCounts[k]; l++)
			{
				writer.Write(this.CurrentProgression.techTreeData[k][l]);
			}
		}
		writer.Write((byte)this.CurrentProgression.stashedQuests);
		writer.Write((byte)this.CurrentProgression.stashedBonusPoints);
		writer.Write((byte)this.CurrentProgression.bonusProgress);
		for (int m = 0; m < this.CurrentProgression.currentQuestIds.Length; m++)
		{
			writer.Write(this.CurrentProgression.currentQuestIds[m]);
			writer.Write(this.CurrentProgression.currentQuestProgresses[m]);
		}
	}

	// Token: 0x0600074D RID: 1869 RVA: 0x00027F64 File Offset: 0x00026164
	public static void DeserializeNetworkStateAndBurn(BinaryReader reader, SIPlayer player, SuperInfectionManager siManager)
	{
		if (player == null || player == SIPlayer.LocalPlayer)
		{
			for (int i = 0; i < 6; i++)
			{
				reader.ReadInt16();
			}
			for (int j = 0; j < 2; j++)
			{
				reader.ReadInt32();
			}
			for (int k = 0; k < SIPlayer.progressionSO.TreePageCount; k++)
			{
				for (int l = 0; l < SIPlayer.progressionSO.TreeNodeCounts[k]; l++)
				{
					reader.ReadBoolean();
				}
			}
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadByte();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			return;
		}
		int[] array = new int[6];
		int[] array2 = new int[2];
		bool[][] array3 = new bool[SIPlayer.progressionSO.TreePageCount][];
		for (int m = 0; m < 6; m++)
		{
			array[m] = reader.ReadInt32();
		}
		for (int n = 0; n < 2; n++)
		{
			array2[n] = reader.ReadInt32();
		}
		for (int num = 0; num < SIPlayer.progressionSO.TreePageCount; num++)
		{
			array3[num] = new bool[SIPlayer.progressionSO.TreeNodeCounts[num]];
			for (int num2 = 0; num2 < SIPlayer.progressionSO.TreeNodeCounts[num]; num2++)
			{
				array3[num][num2] = reader.ReadBoolean();
			}
		}
		int stashedQuests = (int)reader.ReadByte();
		int stashedBonusPoints = (int)reader.ReadByte();
		int bonusProgress = (int)reader.ReadByte();
		int[] array4 = new int[3];
		int[] array5 = new int[3];
		for (int num3 = 0; num3 < 3; num3++)
		{
			array4[num3] = reader.ReadInt32();
			array5[num3] = reader.ReadInt32();
		}
		player.UpdateProgression(array, array2, array3, stashedQuests, stashedBonusPoints, bonusProgress, array4, array5);
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x0002813D File Offset: 0x0002633D
	public bool HasLimitedResourceBeenDeposited(SIResource.LimitedDepositType limitedDepositType)
	{
		if (limitedDepositType == SIResource.LimitedDepositType.None)
		{
			return false;
		}
		if (this == SIPlayer.LocalPlayer)
		{
			return SIProgression.Instance.DailyLimitedTurnedIn;
		}
		return this.CurrentProgression.limitedDepositTimeArray[(int)limitedDepositType] > 0;
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x0002816C File Offset: 0x0002636C
	public bool CanLimitedResourceBeDeposited(SIResource.LimitedDepositType limitedDepositType)
	{
		return limitedDepositType == SIResource.LimitedDepositType.None || (SIProgression.Instance && SIProgression.Instance.IsLimitedDepositAvailable(limitedDepositType));
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x0002818C File Offset: 0x0002638C
	public void GatherResource(SIResource.ResourceType type, SIResource.LimitedDepositType limitedDepositType, int count)
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		Dictionary<SIResource.ResourceType, int> resourceDict = SIProgression.Instance.resourceDict;
		resourceDict[type] += count;
		SIProgression.Instance.ApplyLimitedDepositTime(limitedDepositType);
		bool flag = type == SIResource.ResourceType.TechPoint || (limitedDepositType == SIResource.LimitedDepositType.MonkeIdol && SIProgression.Instance.limitedDepositTimeArray[(int)limitedDepositType] != 1) || SIProgression.Instance.TryDepositResources(type, count);
		if (type == SIResource.ResourceType.StrangeWood)
		{
			PlayerGameEvents.MiscEvent("SIStrangeWoodCollect", 1);
		}
		else if (type == SIResource.ResourceType.WeirdGear)
		{
			PlayerGameEvents.MiscEvent("SISWeirdGearCollect", 1);
		}
		else if (type == SIResource.ResourceType.FloppyMetal)
		{
			PlayerGameEvents.MiscEvent("SIFloppyMetalCollect", 1);
		}
		else if (type == SIResource.ResourceType.BouncySand)
		{
			PlayerGameEvents.MiscEvent("SIBouncySandCollect", 1);
		}
		else if (type == SIResource.ResourceType.VibratingSpring)
		{
			PlayerGameEvents.MiscEvent("SIVibratingSpringCollect", 1);
		}
		if (activeSuperInfectionManager != null && activeSuperInfectionManager.zoneSuperInfection != null && flag)
		{
			SIProgression.Instance.CollectResourceTelemetry(type, count);
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00028268 File Offset: 0x00026468
	public void GetBonusProgress(SuperInfectionManager manager)
	{
		if (SIProgression.Instance.stashedBonusPoints <= 0)
		{
			return;
		}
		SIProgression.Instance.GetBonusProgress();
		this.BonusProgressCelebrate();
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0002828D File Offset: 0x0002648D
	public int GetResourceAmount(SIResource.ResourceType type)
	{
		return this.CurrentProgression.resourceArray[(int)type];
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0002829C File Offset: 0x0002649C
	public void SetProgressionLocal()
	{
		this.currentProgression = new SIPlayer.ProgressionData(SIProgression.Instance);
		this.gamePlayer.SetInitializePlayer(true);
		this.UpdateVisualsForAvailableQuestRedemption();
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x000282C0 File Offset: 0x000264C0
	public void UpdateProgression(int[] resourceArray, int[] limitedDepositTimeArray, bool[][] techTreeData, int _stashedQuests, int _stashedBonusPoints, int _bonusProgress, int[] _currentQuestIds, int[] _currentQuestProgresses)
	{
		SIPlayer.ProgressionData newProgression = new SIPlayer.ProgressionData(resourceArray, limitedDepositTimeArray, techTreeData, _stashedQuests, _stashedBonusPoints, _bonusProgress, _currentQuestIds, _currentQuestProgresses);
		if (this.netInitialized)
		{
			this.CelebrateIfQuestProgressMade(newProgression);
		}
		else
		{
			this.netInitialized = true;
			this.currentProgression = newProgression;
			this.gamePlayer.SetInitializePlayer(true);
		}
		this.currentProgression = newProgression;
		this.UpdateVisualsForAvailableQuestRedemption();
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x0002831C File Offset: 0x0002651C
	public void CelebrateIfQuestProgressMade(SIPlayer.ProgressionData newProgression)
	{
		int num = this.QuestsAvailableToClaim();
		if (this.currentProgression.bonusProgress < newProgression.bonusProgress && this.currentProgression.stashedBonusPoints == newProgression.stashedBonusPoints && this.currentProgression.stashedBonusPoints > 0)
		{
			this.BonusProgressCelebrate();
		}
		bool flag = num > 0 && this.currentProgression.stashedQuests > newProgression.stashedQuests;
		bool flag2 = this.currentProgression.bonusProgress >= 10 && this.currentProgression.stashedBonusPoints > newProgression.stashedBonusPoints;
		bool flag3 = this.currentProgression.limitedDepositTimeArray[1] == 0 && newProgression.limitedDepositTimeArray[1] == 1;
		if ((flag || flag2 || flag3) && this.currentProgression.resourceArray[0] < newProgression.resourceArray[0])
		{
			this.TechPointGrantedCelebrate();
		}
		if (num > this.lastQuestsAvailableToClaim)
		{
			this.questCompleteCelebrate.SetActive(true);
		}
		this.lastQuestsAvailableToClaim = num;
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00028408 File Offset: 0x00026608
	public void TechPointGrantedCelebrate()
	{
		SIQuestBoard questBoard = SuperInfectionManager.activeSuperInfectionManager.zoneSuperInfection.questBoard;
		if (this != SIPlayer.LocalPlayer)
		{
			questBoard.GrantBonusPointProgress();
		}
		if (this.techPointGainedCelebrate != null)
		{
			this.techPointGainedCelebrate.SetActive(false);
			this.techPointGainedCelebrate.SetActive(true);
		}
		else
		{
			Debug.LogError("[SIPlayer]  ERROR!!!  Null reference: `techPointGainedCelebrate`.");
		}
		if (!questBoard.celebrateParticle)
		{
			Debug.LogError("[SIPlayer]  ERROR!!!  SuperInfectionManager.zoneSuperInfection.questBoard.celebrateParticle != null");
		}
		questBoard.celebrateParticle.Play();
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0002848C File Offset: 0x0002668C
	public void BonusProgressCelebrate()
	{
		this.bonusProgressionCelebrate.SetActive(false);
		this.bonusProgressionCelebrate.SetActive(true);
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x000284A8 File Offset: 0x000266A8
	public bool AttemptUnlockNode(SIUpgradeType upgrade, SuperInfectionManager manager)
	{
		if (this.CurrentProgression.techTreeData[upgrade.GetPageId()][upgrade.GetNodeId()])
		{
			return false;
		}
		SITechTreeNode treeNode = SIPlayer.progressionSO.GetTreeNode(upgrade);
		if (!this.PlayerCanAffordNode(treeNode))
		{
			return false;
		}
		this.PurchaseNode(treeNode);
		return true;
	}

	// Token: 0x06000759 RID: 1881 RVA: 0x000284F4 File Offset: 0x000266F4
	public bool PlayerCanAffordNode(SITechTreeNode node)
	{
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			using (Dictionary<SIResource.ResourceType, int>.Enumerator enumerator = sinode.costs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SIResource.ResourceType, int> keyValuePair = enumerator.Current;
					if (this.CurrentProgression.resourceArray[(int)keyValuePair.Key] < keyValuePair.Value)
					{
						return false;
					}
				}
				return true;
			}
		}
		foreach (SIResource.ResourceCost resourceCost in node.nodeCost)
		{
			if (this.CurrentProgression.resourceArray[(int)resourceCost.type] < resourceCost.amount)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600075A RID: 1882 RVA: 0x000285BC File Offset: 0x000267BC
	public void PurchaseNode(SITechTreeNode node)
	{
		SIProgression.SINode sinode;
		if (SIProgression.Instance.GetOnlineNode(node.upgradeType, out sinode))
		{
			using (Dictionary<SIResource.ResourceType, int>.Enumerator enumerator = sinode.costs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SIResource.ResourceType, int> keyValuePair = enumerator.Current;
					SIProgression.Instance.GetResourceArray()[(int)keyValuePair.Key] -= keyValuePair.Value;
				}
				goto IL_A8;
			}
		}
		foreach (SIResource.ResourceCost resourceCost in node.nodeCost)
		{
			SIProgression.Instance.GetResourceArray()[(int)resourceCost.type] -= resourceCost.amount;
		}
		IL_A8:
		SIProgression.Instance.unlockedTechTreeData[node.upgradeType.GetPageId()][node.upgradeType.GetNodeId()] = true;
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x000286AC File Offset: 0x000268AC
	public bool NodeResearched(SIUpgradeType upgrade)
	{
		return this.CurrentProgression.techTreeData[upgrade.GetPageId()][upgrade.GetNodeId()];
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x000286C8 File Offset: 0x000268C8
	public SIUpgradeSet GetUpgrades(SITechTreePageId pageId)
	{
		SIUpgradeSet result = default(SIUpgradeSet);
		bool[] array = this.CurrentProgression.techTreeData[(int)pageId];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i])
			{
				result.Add(i);
			}
		}
		return result;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x00028708 File Offset: 0x00026908
	public bool NodeParentsUnlocked(SIUpgradeType upgrade)
	{
		SITechTreeNode treeNode = SIPlayer.progressionSO.GetTreeNode(upgrade);
		for (int i = 0; i < treeNode.parentUpgrades.Length; i++)
		{
			if (!this.NodeResearched(treeNode.parentUpgrades[i]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00028748 File Offset: 0x00026948
	public void ResetTechTree()
	{
		SIProgression.Instance.unlockedTechTreeData = new bool[SIPlayer.progressionSO.TreePageCount][];
		for (int i = 0; i < SIPlayer.progressionSO.TreePageCount; i++)
		{
			SIProgression.Instance.unlockedTechTreeData[i] = new bool[SIPlayer.progressionSO.TreeNodeCounts[i]];
		}
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x000287A5 File Offset: 0x000269A5
	public void ResetResources()
	{
		SIProgression.Instance.resourceDict = null;
		SIProgression.Instance.EnsureInitialized();
		SIProgression.Instance.limitedDepositTimeArray = new int[2];
		SIPlayer.SetAndBroadcastProgression();
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x000287D1 File Offset: 0x000269D1
	public static void SetAndBroadcastProgression()
	{
		SIPlayer.LocalPlayer.SetAndBroadcastProgressionLocal();
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x000287E0 File Offset: 0x000269E0
	public void SetAndBroadcastProgressionLocal()
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		this.SetProgressionLocal();
		if (!NetworkSystem.Instance.InRoom || activeSuperInfectionManager == null)
		{
			return;
		}
		activeSuperInfectionManager.CallRPC(SuperInfectionManager.ClientToClientRPC.BroadcastProgression, new object[]
		{
			SIPlayer.LocalPlayer.CurrentProgression.resourceArray,
			SIPlayer.LocalPlayer.currentProgression.limitedDepositTimeArray,
			SIPlayer.LocalPlayer.currentProgression.techTreeData,
			SIPlayer.LocalPlayer.currentProgression.stashedQuests,
			SIPlayer.LocalPlayer.currentProgression.stashedBonusPoints,
			SIPlayer.LocalPlayer.currentProgression.bonusProgress,
			SIPlayer.LocalPlayer.currentProgression.currentQuestIds,
			SIPlayer.LocalPlayer.currentProgression.currentQuestProgresses
		});
		if (activeSuperInfectionManager.zoneSuperInfection != null)
		{
			activeSuperInfectionManager.zoneSuperInfection.RefreshStations(SIPlayer.LocalPlayer.ActorNr);
		}
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x000288E0 File Offset: 0x00026AE0
	public void UpdateVisualsForAvailableQuestRedemption()
	{
		bool flag = SuperInfectionManager.activeSuperInfectionManager != null && SuperInfectionManager.activeSuperInfectionManager.IsZoneReady() && (this.QuestsAvailableToClaim() > 0 || (this.currentProgression.bonusProgress >= 10 && this.currentProgression.stashedBonusPoints > 0));
		if (this.tpParticleSystem.isPlaying && !flag)
		{
			this.tpParticleSystem.Stop();
			return;
		}
		if (!this.tpParticleSystem.isPlaying && flag)
		{
			this.tpParticleSystem.Play();
		}
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x00028970 File Offset: 0x00026B70
	public int QuestsAvailableToClaim()
	{
		int num = 0;
		for (int i = 0; i < this.currentProgression.currentQuestIds.Length; i++)
		{
			if (SIProgression.Instance.questSourceList.GetQuestById(this.currentProgression.currentQuestIds[i]) != null && this.currentProgression.currentQuestProgresses[i] >= SIProgression.Instance.questSourceList.GetQuestById(this.currentProgression.currentQuestIds[i]).requiredOccurenceCount)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x000289EC File Offset: 0x00026BEC
	public bool QuestAvailableToClaim(int questIndex)
	{
		return SIProgression.Instance.questSourceList.GetQuestById(this.currentProgression.currentQuestIds[questIndex]) != null && this.currentProgression.currentQuestProgresses[questIndex] >= SIProgression.Instance.questSourceList.GetQuestById(this.currentProgression.currentQuestIds[questIndex]).requiredOccurenceCount;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x00028A4C File Offset: 0x00026C4C
	public void TriggerIdolDepositedCelebration(Vector3 position)
	{
		SuperInfectionManager activeSuperInfectionManager = SuperInfectionManager.activeSuperInfectionManager;
		if (activeSuperInfectionManager.gameEntityManager.IsAuthority())
		{
			activeSuperInfectionManager.CallRPC(SuperInfectionManager.AuthorityToClientRPC.TriggerMonkeIdolDepositCelebration, new object[]
			{
				position
			});
		}
		this.monkeIdolDepositCelebrate.transform.position = position;
		this.monkeIdolDepositCelebrate.SetActive(false);
		this.monkeIdolDepositCelebrate.SetActive(true);
	}

	// Token: 0x06000766 RID: 1894 RVA: 0x00028AAB File Offset: 0x00026CAB
	public void ClearGadgetsOnLeaveZone()
	{
		if (!SuperInfectionManager.activeSuperInfectionManager.gameEntityManager.IsAuthority())
		{
			return;
		}
		SuperInfectionManager.activeSuperInfectionManager.ClearPlayerGadgets(this);
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00028ACA File Offset: 0x00026CCA
	public void PlayerKnockback(Vector3 directionAndMagnitude, bool forceOffGround = true, bool applyExclusionZone = true)
	{
		if (applyExclusionZone && this.exclusionZoneCount > 0)
		{
			return;
		}
		GTPlayer.Instance.ApplyClampedKnockback(directionAndMagnitude.normalized, directionAndMagnitude.magnitude, 1.5f, forceOffGround);
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x00028AF7 File Offset: 0x00026CF7
	public void PlayerHandHaptic(bool isLeft, float hapticStrength, float hapticDuration, bool applyExclusionZone = true)
	{
		if (applyExclusionZone && this.exclusionZoneCount > 0)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(isLeft, hapticStrength, hapticDuration);
	}

	// Token: 0x0400091E RID: 2334
	private const string preLog = "[SIPlayer]  ";

	// Token: 0x0400091F RID: 2335
	private const string preErr = "[SIPlayer]  ERROR!!!  ";

	// Token: 0x04000920 RID: 2336
	public GamePlayer gamePlayer;

	// Token: 0x04000921 RID: 2337
	private static Dictionary<int, SIPlayer> siPlayerByActorNr = new Dictionary<int, SIPlayer>();

	// Token: 0x04000922 RID: 2338
	public CallLimiter clientToAuthorityRPCLimiter;

	// Token: 0x04000923 RID: 2339
	public CallLimiter clientToClientRPCLimiter;

	// Token: 0x04000924 RID: 2340
	public CallLimiter authorityToClientRPCLimiter;

	// Token: 0x04000925 RID: 2341
	public static SITechTreeSO progressionSO;

	// Token: 0x04000926 RID: 2342
	public SITechTreeSO progressionSORef;

	// Token: 0x04000927 RID: 2343
	public ParticleSystem tpParticleSystem;

	// Token: 0x04000928 RID: 2344
	public GameObject bonusProgressionCelebrate;

	// Token: 0x04000929 RID: 2345
	[FormerlySerializedAs("testPointGainedCelebrate")]
	public GameObject techPointGainedCelebrate;

	// Token: 0x0400092A RID: 2346
	public GameObject monkeIdolDepositCelebrate;

	// Token: 0x0400092B RID: 2347
	public GameObject questCompleteCelebrate;

	// Token: 0x0400092C RID: 2348
	private int lastQuestsAvailableToClaim;

	// Token: 0x0400092D RID: 2349
	[NonSerialized]
	public int totalGadgetLimit = 3;

	// Token: 0x0400092E RID: 2350
	[NonSerialized]
	public int exclusionZoneCount;

	// Token: 0x0400092F RID: 2351
	public bool netInitialized;

	// Token: 0x04000930 RID: 2352
	private SIPlayer.ProgressionData currentProgression;

	// Token: 0x04000931 RID: 2353
	public List<int> activePlayerGadgets = new List<int>();

	// Token: 0x0200011F RID: 287
	[Serializable]
	public struct ProgressionData
	{
		// Token: 0x0600076B RID: 1899 RVA: 0x00028B3C File Offset: 0x00026D3C
		public ProgressionData(bool itsNullLol)
		{
			this.resourceArray = new int[6];
			this.limitedDepositTimeArray = new int[2];
			this.techTreeData = new bool[SIPlayer.progressionSO.TreePageCount][];
			for (int i = 0; i < SIPlayer.progressionSO.TreePageCount; i++)
			{
				this.techTreeData[i] = new bool[SIPlayer.progressionSO.TreeNodeCounts[i]];
			}
			this.stashedQuests = 0;
			this.stashedBonusPoints = 0;
			this.bonusProgress = 0;
			this.currentQuestIds = new int[3];
			this.currentQuestProgresses = new int[3];
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x00028BD1 File Offset: 0x00026DD1
		public ProgressionData(int[] _resourceArray, int[] _limitedDepositTimeArray, bool[][] _techTreeData, int _stashedQuests, int _stashedBonusPoints, int _bonusProgress, int[] _currentQuestIds, int[] _currentQuestProgresses)
		{
			this.resourceArray = _resourceArray;
			this.limitedDepositTimeArray = _limitedDepositTimeArray;
			this.techTreeData = _techTreeData;
			this.stashedQuests = _stashedQuests;
			this.stashedBonusPoints = _stashedBonusPoints;
			this.bonusProgress = _bonusProgress;
			this.currentQuestIds = _currentQuestIds;
			this.currentQuestProgresses = _currentQuestProgresses;
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x00028C10 File Offset: 0x00026E10
		public ProgressionData(SIProgression siProgression)
		{
			this.resourceArray = siProgression.GetResourceArray();
			this.limitedDepositTimeArray = siProgression.limitedDepositTimeArray;
			this.techTreeData = siProgression.unlockedTechTreeData;
			this.stashedQuests = siProgression.stashedQuests;
			this.stashedBonusPoints = siProgression.stashedBonusPoints;
			this.bonusProgress = siProgression.bonusProgress;
			this.currentQuestIds = siProgression.ActiveQuestIds;
			this.currentQuestProgresses = siProgression.ActiveQuestProgresses;
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x00028C7D File Offset: 0x00026E7D
		public bool IsUnlocked(SIUpgradeType upgradeType)
		{
			return this.techTreeData[upgradeType.GetPageId()][upgradeType.GetNodeId()];
		}

		// Token: 0x04000932 RID: 2354
		public int[] resourceArray;

		// Token: 0x04000933 RID: 2355
		public int[] limitedDepositTimeArray;

		// Token: 0x04000934 RID: 2356
		public bool[][] techTreeData;

		// Token: 0x04000935 RID: 2357
		public int stashedQuests;

		// Token: 0x04000936 RID: 2358
		public int stashedBonusPoints;

		// Token: 0x04000937 RID: 2359
		public int bonusProgress;

		// Token: 0x04000938 RID: 2360
		public int[] currentQuestIds;

		// Token: 0x04000939 RID: 2361
		public int[] currentQuestProgresses;
	}
}
