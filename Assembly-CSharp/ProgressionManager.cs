using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200085E RID: 2142
public class ProgressionManager : MonoBehaviour
{
	// Token: 0x17000517 RID: 1303
	// (get) Token: 0x06003864 RID: 14436 RVA: 0x0012D8F8 File Offset: 0x0012BAF8
	// (set) Token: 0x06003865 RID: 14437 RVA: 0x0012D8FF File Offset: 0x0012BAFF
	public static ProgressionManager Instance { get; private set; }

	// Token: 0x14000064 RID: 100
	// (add) Token: 0x06003866 RID: 14438 RVA: 0x0012D908 File Offset: 0x0012BB08
	// (remove) Token: 0x06003867 RID: 14439 RVA: 0x0012D940 File Offset: 0x0012BB40
	public event Action OnTreeUpdated;

	// Token: 0x14000065 RID: 101
	// (add) Token: 0x06003868 RID: 14440 RVA: 0x0012D978 File Offset: 0x0012BB78
	// (remove) Token: 0x06003869 RID: 14441 RVA: 0x0012D9B0 File Offset: 0x0012BBB0
	public event Action OnInventoryUpdated;

	// Token: 0x14000066 RID: 102
	// (add) Token: 0x0600386A RID: 14442 RVA: 0x0012D9E8 File Offset: 0x0012BBE8
	// (remove) Token: 0x0600386B RID: 14443 RVA: 0x0012DA20 File Offset: 0x0012BC20
	public event Action<string, int> OnTrackRead;

	// Token: 0x14000067 RID: 103
	// (add) Token: 0x0600386C RID: 14444 RVA: 0x0012DA58 File Offset: 0x0012BC58
	// (remove) Token: 0x0600386D RID: 14445 RVA: 0x0012DA90 File Offset: 0x0012BC90
	public event Action<string, int> OnTrackSet;

	// Token: 0x14000068 RID: 104
	// (add) Token: 0x0600386E RID: 14446 RVA: 0x0012DAC8 File Offset: 0x0012BCC8
	// (remove) Token: 0x0600386F RID: 14447 RVA: 0x0012DB00 File Offset: 0x0012BD00
	public event Action<string, string> OnNodeUnlocked;

	// Token: 0x14000069 RID: 105
	// (add) Token: 0x06003870 RID: 14448 RVA: 0x0012DB38 File Offset: 0x0012BD38
	// (remove) Token: 0x06003871 RID: 14449 RVA: 0x0012DB70 File Offset: 0x0012BD70
	public event Action<string, int> OnGetShiftCredit;

	// Token: 0x1400006A RID: 106
	// (add) Token: 0x06003872 RID: 14450 RVA: 0x0012DBA8 File Offset: 0x0012BDA8
	// (remove) Token: 0x06003873 RID: 14451 RVA: 0x0012DBE0 File Offset: 0x0012BDE0
	public event Action<string, int, int> OnGetShiftCreditCapData;

	// Token: 0x1400006B RID: 107
	// (add) Token: 0x06003874 RID: 14452 RVA: 0x0012DC18 File Offset: 0x0012BE18
	// (remove) Token: 0x06003875 RID: 14453 RVA: 0x0012DC50 File Offset: 0x0012BE50
	public event Action<bool> OnPurchaseShiftCreditCapIncrease;

	// Token: 0x1400006C RID: 108
	// (add) Token: 0x06003876 RID: 14454 RVA: 0x0012DC88 File Offset: 0x0012BE88
	// (remove) Token: 0x06003877 RID: 14455 RVA: 0x0012DCC0 File Offset: 0x0012BEC0
	public event Action<bool> OnPurchaseShiftCredit;

	// Token: 0x1400006D RID: 109
	// (add) Token: 0x06003878 RID: 14456 RVA: 0x0012DCF8 File Offset: 0x0012BEF8
	// (remove) Token: 0x06003879 RID: 14457 RVA: 0x0012DD30 File Offset: 0x0012BF30
	public event Action<bool> OnChaosDepositSuccess;

	// Token: 0x1400006E RID: 110
	// (add) Token: 0x0600387A RID: 14458 RVA: 0x0012DD68 File Offset: 0x0012BF68
	// (remove) Token: 0x0600387B RID: 14459 RVA: 0x0012DDA0 File Offset: 0x0012BFA0
	public event Action<ProgressionManager.JuicerStatusResponse> OnJucierStatusUpdated;

	// Token: 0x1400006F RID: 111
	// (add) Token: 0x0600387C RID: 14460 RVA: 0x0012DDD8 File Offset: 0x0012BFD8
	// (remove) Token: 0x0600387D RID: 14461 RVA: 0x0012DE10 File Offset: 0x0012C010
	public event Action<bool> OnPurchaseOverdrive;

	// Token: 0x14000070 RID: 112
	// (add) Token: 0x0600387E RID: 14462 RVA: 0x0012DE48 File Offset: 0x0012C048
	// (remove) Token: 0x0600387F RID: 14463 RVA: 0x0012DE80 File Offset: 0x0012C080
	public event Action<ProgressionManager.DockWristStatusResponse> OnDockWristStatusUpdated;

	// Token: 0x14000071 RID: 113
	// (add) Token: 0x06003880 RID: 14464 RVA: 0x0012DEB8 File Offset: 0x0012C0B8
	// (remove) Token: 0x06003881 RID: 14465 RVA: 0x0012DEF0 File Offset: 0x0012C0F0
	public event Action<ProgressionManager.GhostReactorStatsResponse> OnGhostReactorStatsUpdated;

	// Token: 0x14000072 RID: 114
	// (add) Token: 0x06003882 RID: 14466 RVA: 0x0012DF28 File Offset: 0x0012C128
	// (remove) Token: 0x06003883 RID: 14467 RVA: 0x0012DF60 File Offset: 0x0012C160
	public event Action<ProgressionManager.GhostReactorInventoryResponse> OnGhostReactorInventoryUpdated;

	// Token: 0x06003884 RID: 14468 RVA: 0x0012DF95 File Offset: 0x0012C195
	private void Awake()
	{
		if (ProgressionManager.Instance == null)
		{
			ProgressionManager.Instance = this;
		}
	}

	// Token: 0x06003885 RID: 14469 RVA: 0x0012DFAC File Offset: 0x0012C1AC
	public void RefreshProgressionTree()
	{
		ProgressionManager.<RefreshProgressionTree>d__60 <RefreshProgressionTree>d__;
		<RefreshProgressionTree>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RefreshProgressionTree>d__.<>4__this = this;
		<RefreshProgressionTree>d__.<>1__state = -1;
		<RefreshProgressionTree>d__.<>t__builder.Start<ProgressionManager.<RefreshProgressionTree>d__60>(ref <RefreshProgressionTree>d__);
	}

	// Token: 0x06003886 RID: 14470 RVA: 0x0012DFE4 File Offset: 0x0012C1E4
	public void RefreshUserInventory()
	{
		ProgressionManager.<RefreshUserInventory>d__61 <RefreshUserInventory>d__;
		<RefreshUserInventory>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RefreshUserInventory>d__.<>4__this = this;
		<RefreshUserInventory>d__.<>1__state = -1;
		<RefreshUserInventory>d__.<>t__builder.Start<ProgressionManager.<RefreshUserInventory>d__61>(ref <RefreshUserInventory>d__);
	}

	// Token: 0x06003887 RID: 14471 RVA: 0x0012E01C File Offset: 0x0012C21C
	public UserHydratedProgressionTreeResponse GetTree(string treeName)
	{
		UserHydratedProgressionTreeResponse result;
		this._trees.TryGetValue(treeName, ref result);
		return result;
	}

	// Token: 0x06003888 RID: 14472 RVA: 0x0012E039 File Offset: 0x0012C239
	public bool GetInventoryItem(string inventoryKey, out ProgressionManager.MothershipItemSummary item)
	{
		return this._inventory.TryGetValue((inventoryKey != null) ? inventoryKey.Trim() : null, ref item);
	}

	// Token: 0x06003889 RID: 14473 RVA: 0x0012E054 File Offset: 0x0012C254
	public int GetNodeCost(string treeName, string nodeId, string currencyKey)
	{
		UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse;
		if (!this._trees.TryGetValue(treeName, ref userHydratedProgressionTreeResponse) || userHydratedProgressionTreeResponse == null || string.IsNullOrEmpty(nodeId) || string.IsNullOrEmpty(currencyKey))
		{
			return 0;
		}
		foreach (UserHydratedNodeDefinition userHydratedNodeDefinition in userHydratedProgressionTreeResponse.Nodes)
		{
			if (userHydratedNodeDefinition.id == nodeId && userHydratedNodeDefinition.cost != null && userHydratedNodeDefinition.cost.items != null)
			{
				using (HydratedInventoryChangeMap.HydratedInventoryChangeMapEnumerator enumerator2 = userHydratedNodeDefinition.cost.items.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						KeyValuePair<string, MothershipHydratedInventoryChange> keyValuePair = enumerator2.Current;
						string key = keyValuePair.Key;
						if (string.Equals((key != null) ? key.Trim() : null, currencyKey.Trim(), 4))
						{
							return keyValuePair.Value.Delta;
						}
					}
					break;
				}
			}
		}
		return 0;
	}

	// Token: 0x0600388A RID: 14474 RVA: 0x0012E158 File Offset: 0x0012C358
	public void GetProgression(string trackId)
	{
		ProgressionManager.<GetProgression>d__65 <GetProgression>d__;
		<GetProgression>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<GetProgression>d__.<>4__this = this;
		<GetProgression>d__.trackId = trackId;
		<GetProgression>d__.<>1__state = -1;
		<GetProgression>d__.<>t__builder.Start<ProgressionManager.<GetProgression>d__65>(ref <GetProgression>d__);
	}

	// Token: 0x0600388B RID: 14475 RVA: 0x0012E198 File Offset: 0x0012C398
	public void SetProgression(string trackId, int progress)
	{
		ProgressionManager.<SetProgression>d__66 <SetProgression>d__;
		<SetProgression>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetProgression>d__.<>4__this = this;
		<SetProgression>d__.trackId = trackId;
		<SetProgression>d__.progress = progress;
		<SetProgression>d__.<>1__state = -1;
		<SetProgression>d__.<>t__builder.Start<ProgressionManager.<SetProgression>d__66>(ref <SetProgression>d__);
	}

	// Token: 0x0600388C RID: 14476 RVA: 0x0012E1E0 File Offset: 0x0012C3E0
	public void UnlockNode(string treeId, string nodeId)
	{
		ProgressionManager.<UnlockNode>d__67 <UnlockNode>d__;
		<UnlockNode>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<UnlockNode>d__.<>4__this = this;
		<UnlockNode>d__.treeId = treeId;
		<UnlockNode>d__.nodeId = nodeId;
		<UnlockNode>d__.<>1__state = -1;
		<UnlockNode>d__.<>t__builder.Start<ProgressionManager.<UnlockNode>d__67>(ref <UnlockNode>d__);
	}

	// Token: 0x0600388D RID: 14477 RVA: 0x0012E228 File Offset: 0x0012C428
	public void IncrementSIResource(string resourceName, Action<string> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<IncrementSIResource>d__68 <IncrementSIResource>d__;
		<IncrementSIResource>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<IncrementSIResource>d__.<>4__this = this;
		<IncrementSIResource>d__.resourceName = resourceName;
		<IncrementSIResource>d__.OnSuccess = OnSuccess;
		<IncrementSIResource>d__.OnFailure = OnFailure;
		<IncrementSIResource>d__.<>1__state = -1;
		<IncrementSIResource>d__.<>t__builder.Start<ProgressionManager.<IncrementSIResource>d__68>(ref <IncrementSIResource>d__);
	}

	// Token: 0x0600388E RID: 14478 RVA: 0x0012E278 File Offset: 0x0012C478
	public void CompleteSIQuest(int questID, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<CompleteSIQuest>d__69 <CompleteSIQuest>d__;
		<CompleteSIQuest>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<CompleteSIQuest>d__.<>4__this = this;
		<CompleteSIQuest>d__.questID = questID;
		<CompleteSIQuest>d__.OnSuccess = OnSuccess;
		<CompleteSIQuest>d__.OnFailure = OnFailure;
		<CompleteSIQuest>d__.<>1__state = -1;
		<CompleteSIQuest>d__.<>t__builder.Start<ProgressionManager.<CompleteSIQuest>d__69>(ref <CompleteSIQuest>d__);
	}

	// Token: 0x0600388F RID: 14479 RVA: 0x0012E2C8 File Offset: 0x0012C4C8
	public void CompleteSIBonus(Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<CompleteSIBonus>d__70 <CompleteSIBonus>d__;
		<CompleteSIBonus>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<CompleteSIBonus>d__.<>4__this = this;
		<CompleteSIBonus>d__.OnSuccess = OnSuccess;
		<CompleteSIBonus>d__.OnFailure = OnFailure;
		<CompleteSIBonus>d__.<>1__state = -1;
		<CompleteSIBonus>d__.<>t__builder.Start<ProgressionManager.<CompleteSIBonus>d__70>(ref <CompleteSIBonus>d__);
	}

	// Token: 0x06003890 RID: 14480 RVA: 0x0012E310 File Offset: 0x0012C510
	public void CollectSIIdol(Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<CollectSIIdol>d__71 <CollectSIIdol>d__;
		<CollectSIIdol>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<CollectSIIdol>d__.<>4__this = this;
		<CollectSIIdol>d__.OnSuccess = OnSuccess;
		<CollectSIIdol>d__.OnFailure = OnFailure;
		<CollectSIIdol>d__.<>1__state = -1;
		<CollectSIIdol>d__.<>t__builder.Start<ProgressionManager.<CollectSIIdol>d__71>(ref <CollectSIIdol>d__);
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x0012E358 File Offset: 0x0012C558
	public void GetActiveSIQuests(Action<List<RotatingQuest>> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<GetActiveSIQuests>d__72 <GetActiveSIQuests>d__;
		<GetActiveSIQuests>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<GetActiveSIQuests>d__.<>4__this = this;
		<GetActiveSIQuests>d__.OnSuccess = OnSuccess;
		<GetActiveSIQuests>d__.OnFailure = OnFailure;
		<GetActiveSIQuests>d__.<>1__state = -1;
		<GetActiveSIQuests>d__.<>t__builder.Start<ProgressionManager.<GetActiveSIQuests>d__72>(ref <GetActiveSIQuests>d__);
	}

	// Token: 0x06003892 RID: 14482 RVA: 0x0012E3A0 File Offset: 0x0012C5A0
	public void GetSIQuestStatus(Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<GetSIQuestStatus>d__73 <GetSIQuestStatus>d__;
		<GetSIQuestStatus>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<GetSIQuestStatus>d__.<>4__this = this;
		<GetSIQuestStatus>d__.OnSuccess = OnSuccess;
		<GetSIQuestStatus>d__.OnFailure = OnFailure;
		<GetSIQuestStatus>d__.<>1__state = -1;
		<GetSIQuestStatus>d__.<>t__builder.Start<ProgressionManager.<GetSIQuestStatus>d__73>(ref <GetSIQuestStatus>d__);
	}

	// Token: 0x06003893 RID: 14483 RVA: 0x0012E3E8 File Offset: 0x0012C5E8
	public void PurchaseTechPoints(int amount, Action OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<PurchaseTechPoints>d__74 <PurchaseTechPoints>d__;
		<PurchaseTechPoints>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<PurchaseTechPoints>d__.<>4__this = this;
		<PurchaseTechPoints>d__.amount = amount;
		<PurchaseTechPoints>d__.OnSuccess = OnSuccess;
		<PurchaseTechPoints>d__.OnFailure = OnFailure;
		<PurchaseTechPoints>d__.<>1__state = -1;
		<PurchaseTechPoints>d__.<>t__builder.Start<ProgressionManager.<PurchaseTechPoints>d__74>(ref <PurchaseTechPoints>d__);
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x0012E438 File Offset: 0x0012C638
	public void PurchaseResources(Action<ProgressionManager.UserInventory> OnSuccess = null, Action<string> OnFailure = null)
	{
		ProgressionManager.<PurchaseResources>d__75 <PurchaseResources>d__;
		<PurchaseResources>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<PurchaseResources>d__.<>4__this = this;
		<PurchaseResources>d__.OnSuccess = OnSuccess;
		<PurchaseResources>d__.OnFailure = OnFailure;
		<PurchaseResources>d__.<>1__state = -1;
		<PurchaseResources>d__.<>t__builder.Start<ProgressionManager.<PurchaseResources>d__75>(ref <PurchaseResources>d__);
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x0012E47F File Offset: 0x0012C67F
	public void PurchaseShiftCreditCapIncrease()
	{
		this.PurchaseShiftCreditCapIncreaseInternal(false);
	}

	// Token: 0x06003896 RID: 14486 RVA: 0x0012E488 File Offset: 0x0012C688
	private void PurchaseShiftCreditCapIncreaseInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoPurchaseShiftCreditCapIncrease(new ProgressionManager.PurchaseShiftCreditCapIncreaseRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x0012E4E5 File Offset: 0x0012C6E5
	public void PurchaseShiftCredit()
	{
		this.PurchaseShiftCreditInternal(false);
	}

	// Token: 0x06003898 RID: 14488 RVA: 0x0012E4F0 File Offset: 0x0012C6F0
	private void PurchaseShiftCreditInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoPurchaseShiftCredit(new ProgressionManager.PurchaseShiftCreditRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x06003899 RID: 14489 RVA: 0x0012E550 File Offset: 0x0012C750
	public void GetShiftCredit(string mothershipId)
	{
		base.StartCoroutine(this.DoGetShiftCredit(new ProgressionManager.GetShiftCreditRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			TargetMothershipId = mothershipId
		}));
	}

	// Token: 0x0600389A RID: 14490 RVA: 0x0012E5AD File Offset: 0x0012C7AD
	public void GetJuicerStatus()
	{
		this.GetJuicerStatusInternal(false);
	}

	// Token: 0x0600389B RID: 14491 RVA: 0x0012E5B8 File Offset: 0x0012C7B8
	private void GetJuicerStatusInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoGetJuicerStatus(new ProgressionManager.GetJuicerStatusRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x0600389C RID: 14492 RVA: 0x0012E615 File Offset: 0x0012C815
	public void DepositCore(ProgressionManager.CoreType coreType)
	{
		this.DepositCoreInternal(coreType, false);
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x0012E620 File Offset: 0x0012C820
	private void DepositCoreInternal(ProgressionManager.CoreType coreType, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoDepositCore(new ProgressionManager.DepositCoreRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			CoreBeingDeposited = coreType,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x0012E684 File Offset: 0x0012C884
	public void PurchaseOverdrive()
	{
		this.PurchaseOverdriveInternal(false);
	}

	// Token: 0x0600389F RID: 14495 RVA: 0x0012E690 File Offset: 0x0012C890
	private void PurchaseOverdriveInternal(bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoPurchaseOverdrive(new ProgressionManager.PurchaseOverdriveRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x060038A0 RID: 14496 RVA: 0x0012E6ED File Offset: 0x0012C8ED
	public void SubtractShiftCredit(int creditsToSubtract)
	{
		this.SubtractShiftCreditInternal(creditsToSubtract, false);
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x0012E6F8 File Offset: 0x0012C8F8
	private void SubtractShiftCreditInternal(int creditsToSubtract, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoSubtractShiftCredit(new ProgressionManager.SubtractShiftCreditRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ShiftCreditToRemove = creditsToSubtract,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x060038A2 RID: 14498 RVA: 0x0012E75C File Offset: 0x0012C95C
	public void AdvanceDockWristUpgradeLevel(ProgressionManager.WristDockUpgradeType upgrade)
	{
		this.AdvanceDockWristUpgradeLevelInternal(upgrade, false);
	}

	// Token: 0x060038A3 RID: 14499 RVA: 0x0012E768 File Offset: 0x0012C968
	private void AdvanceDockWristUpgradeLevelInternal(ProgressionManager.WristDockUpgradeType upgrade, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoAdvanceDockWristUpgradeLevel(new ProgressionManager.AdvanceDockWristUpgradeRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			Upgrade = upgrade,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x060038A4 RID: 14500 RVA: 0x0012E7CC File Offset: 0x0012C9CC
	public void GetDockWristUpgradeStatus()
	{
		base.StartCoroutine(this.DoGetDockWristUpgradeStatus(new ProgressionManager.DockWristUpgradeStatusRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token
		}));
	}

	// Token: 0x060038A5 RID: 14501 RVA: 0x0012E824 File Offset: 0x0012CA24
	public void PurchaseDrillUpgrade(ProgressionManager.DrillUpgradeLevel upgrade)
	{
		base.StartCoroutine(this.DoPurchaseDrillUpgrade(new ProgressionManager.PurchaseDrillUpgradeRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			Upgrade = upgrade
		}));
	}

	// Token: 0x060038A6 RID: 14502 RVA: 0x0012E884 File Offset: 0x0012CA84
	public void RecycleTool(GRTool.GRToolType toolBeingRecycled, int numberOfPlayers)
	{
		base.StartCoroutine(this.DoRecycleTool(new ProgressionManager.RecycleToolRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ToolBeingRecycled = toolBeingRecycled,
			NumberOfPlayers = numberOfPlayers
		}));
	}

	// Token: 0x060038A7 RID: 14503 RVA: 0x0012E8E8 File Offset: 0x0012CAE8
	public void StartOfShift(string shiftId, int coresRequired, int numberOfPlayers, int depth)
	{
		base.StartCoroutine(this.DoStartOfShift(new ProgressionManager.StartOfShiftRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ShiftId = shiftId,
			CoresRequired = coresRequired,
			NumberOfPlayers = numberOfPlayers,
			Depth = depth
		}));
	}

	// Token: 0x060038A8 RID: 14504 RVA: 0x0012E95B File Offset: 0x0012CB5B
	public void EndOfShiftReward(string shiftId)
	{
		this.EndOfShiftRewardInternal(shiftId, false);
	}

	// Token: 0x060038A9 RID: 14505 RVA: 0x0012E968 File Offset: 0x0012CB68
	private void EndOfShiftRewardInternal(string shiftId, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoEndOfShiftReward(new ProgressionManager.EndOfShiftRewardRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			ShiftId = shiftId,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x060038AA RID: 14506 RVA: 0x0012E9CC File Offset: 0x0012CBCC
	public void GetGhostReactorStats()
	{
		base.StartCoroutine(this.DoGetGhostReactorStats(new ProgressionManager.GhostReactorStatsRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token
		}));
	}

	// Token: 0x060038AB RID: 14507 RVA: 0x0012EA24 File Offset: 0x0012CC24
	public void GetGhostReactorInventory()
	{
		base.StartCoroutine(this.DoGetGhostReactorInventory(new ProgressionManager.GhostReactorInventoryRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token
		}));
	}

	// Token: 0x060038AC RID: 14508 RVA: 0x0012EA7A File Offset: 0x0012CC7A
	public void SetGhostReactorInventory(string jsonInventory)
	{
		this.SetGhostReactorInventoryInternal(jsonInventory, false);
	}

	// Token: 0x060038AD RID: 14509 RVA: 0x0012EA84 File Offset: 0x0012CC84
	private void SetGhostReactorInventoryInternal(string jsonInventory, bool skipUserDataCache = false)
	{
		base.StartCoroutine(this.DoSetGhostReactorInventory(new ProgressionManager.SetGhostReactorInventoryRequest
		{
			MothershipId = MothershipClientContext.MothershipId,
			MothershipTitleId = MothershipClientApiUnity.TitleId,
			MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
			MothershipToken = MothershipClientContext.Token,
			InventoryJson = jsonInventory,
			SkipUserDataCache = skipUserDataCache
		}));
	}

	// Token: 0x060038AE RID: 14510 RVA: 0x0012EAE8 File Offset: 0x0012CCE8
	private IEnumerator HandleWebRequestRetries<T>(ProgressionManager.RequestType requestType, T data, Action<T> actionToTake, Action failureActionToTake = null)
	{
		if (!this.retryCounters.ContainsKey(requestType))
		{
			this.retryCounters[requestType] = 0;
		}
		if (this.retryCounters[requestType] < this.maxRetriesOnFail)
		{
			float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.retryCounters[requestType] + 1)));
			Debug.LogWarning(string.Format("PM: Retrying ... attempt #{0}, waiting {1}s", this.retryCounters[requestType] + 1, num));
			Dictionary<ProgressionManager.RequestType, int> dictionary = this.retryCounters;
			int num2 = dictionary[requestType];
			dictionary[requestType] = num2 + 1;
			yield return new WaitForSeconds(num);
			actionToTake.Invoke(data);
		}
		else
		{
			Debug.LogError("PM: Maximum retries attempted.");
			this.retryCounters[requestType] = 0;
			if (failureActionToTake != null)
			{
				failureActionToTake.Invoke();
			}
		}
		yield break;
	}

	// Token: 0x060038AF RID: 14511 RVA: 0x0012EB14 File Offset: 0x0012CD14
	private bool HandleWebRequestFailures(UnityWebRequest request, bool retryOnConflict = false)
	{
		bool result = false;
		Debug.LogError(string.Format("PM: HandleWebRequestFailures Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
		if (request.result != 3)
		{
			result = true;
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L)
			{
				if (responseCode >= 600L)
				{
					goto IL_6A;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_6A;
			}
			bool flag = true;
			goto IL_6C;
			IL_6A:
			flag = false;
			IL_6C:
			if (flag || (retryOnConflict && request.responseCode == 409L))
			{
				result = true;
				Debug.LogError(string.Format("PM: HTTP {0} error: {1}", request.responseCode, request.error));
			}
		}
		return result;
	}

	// Token: 0x060038B0 RID: 14512 RVA: 0x0012EBC4 File Offset: 0x0012CDC4
	private IEnumerator DoGetProgression(ProgressionManager.GetProgressionRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetProgressionRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetProgression);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			int num = int.Parse(request.downloadHandler.text);
			this._tracks[data.TrackId] = num;
			Debug.Log("PM: GetProgression Success: track is " + data.TrackId + " and progress is " + num.ToString());
			this.retryCounters[ProgressionManager.RequestType.GetProgression] = 0;
			Action<string, int> onTrackRead = this.OnTrackRead;
			if (onTrackRead != null)
			{
				onTrackRead.Invoke(data.TrackId, num);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<string>(ProgressionManager.RequestType.GetProgression, data.TrackId, delegate(string x)
		{
			this.GetProgression(x);
		}, null);
		yield break;
	}

	// Token: 0x060038B1 RID: 14513 RVA: 0x0012EBDA File Offset: 0x0012CDDA
	private IEnumerator DoSetProgression(ProgressionManager.SetProgressionRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetProgressionRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.SetProgression);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.GetProgressionResponse getProgressionResponse = JsonConvert.DeserializeObject<ProgressionManager.GetProgressionResponse>(request.downloadHandler.text);
			this._tracks[data.TrackId] = getProgressionResponse.Progress;
			this.retryCounters[ProgressionManager.RequestType.SetProgression] = 0;
			Action<string, int> onTrackSet = this.OnTrackSet;
			if (onTrackSet != null)
			{
				onTrackSet.Invoke(data.TrackId, getProgressionResponse.Progress);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ValueTuple<string, int>>(ProgressionManager.RequestType.SetProgression, new ValueTuple<string, int>(data.TrackId, data.Progress), delegate([TupleElementNames(new string[]
		{
			"TrackId",
			"Progress"
		})] ValueTuple<string, int> x)
		{
			this.SetProgression(x.Item1, x.Item2);
		}, null);
		yield break;
	}

	// Token: 0x060038B2 RID: 14514 RVA: 0x0012EBF0 File Offset: 0x0012CDF0
	private IEnumerator DoUnlockNode(ProgressionManager.UnlockNodeRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.UnlockNodeRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.UnlockProgressionTreeNode);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			this.retryCounters[ProgressionManager.RequestType.UnlockProgressionTreeNode] = 0;
			this.RefreshProgressionTree();
			this.RefreshUserInventory();
			Action<string, string> onNodeUnlocked = this.OnNodeUnlocked;
			if (onNodeUnlocked != null)
			{
				onNodeUnlocked.Invoke(data.TreeId, data.NodeId);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ValueTuple<string, string>>(ProgressionManager.RequestType.UnlockProgressionTreeNode, new ValueTuple<string, string>(data.TreeId, data.NodeId), delegate([TupleElementNames(new string[]
		{
			"TreeId",
			"NodeId"
		})] ValueTuple<string, string> x)
		{
			this.UnlockNode(x.Item1, x.Item2);
		}, null);
		yield break;
	}

	// Token: 0x060038B3 RID: 14515 RVA: 0x0012EC06 File Offset: 0x0012CE06
	private IEnumerator DoIncrementSIResource(ProgressionManager.IncrementSIResourceRequest data, Action<string> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.IncrementSIResourceRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.IncrementSIResource);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.IncrementSIResourceResponse incrementSIResourceResponse = JsonConvert.DeserializeObject<ProgressionManager.IncrementSIResourceResponse>(request.downloadHandler.text);
			Action<string> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess.Invoke(incrementSIResourceResponse.ResourceType);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure.Invoke(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.IncrementSIResourceRequest>(ProgressionManager.RequestType.IncrementSIResource, data, delegate(ProgressionManager.IncrementSIResourceRequest x)
		{
			this.IncrementSIResource(data.ResourceType, OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2.Invoke(request.error);
		});
		yield break;
	}

	// Token: 0x060038B4 RID: 14516 RVA: 0x0012EC2A File Offset: 0x0012CE2A
	private IEnumerator DoQuestCompleteReward(ProgressionManager.SetSIQuestCompleteRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetSIQuestCompleteRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.CompleteSIQuest);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess.Invoke(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure.Invoke(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetSIQuestCompleteRequest>(ProgressionManager.RequestType.CompleteSIQuest, data, delegate(ProgressionManager.SetSIQuestCompleteRequest x)
		{
			this.CompleteSIQuest(data.QuestID, OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2.Invoke(request.error);
		});
		yield break;
	}

	// Token: 0x060038B5 RID: 14517 RVA: 0x0012EC4E File Offset: 0x0012CE4E
	private IEnumerator DoBonusCompleteReward(ProgressionManager.SetSIBonusCompleteRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetSIBonusCompleteRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.CompleteSIBonus);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess.Invoke(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure.Invoke(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetSIBonusCompleteRequest>(ProgressionManager.RequestType.CompleteSIBonus, data, delegate(ProgressionManager.SetSIBonusCompleteRequest x)
		{
			this.CompleteSIBonus(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2.Invoke(request.error);
		});
		yield break;
	}

	// Token: 0x060038B6 RID: 14518 RVA: 0x0012EC72 File Offset: 0x0012CE72
	private IEnumerator DoIdolCollectReward(ProgressionManager.SetSIIdolCollectRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetSIIdolCollectRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.CollectSIIdol);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess.Invoke(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure.Invoke(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetSIIdolCollectRequest>(ProgressionManager.RequestType.CollectSIIdol, data, delegate(ProgressionManager.SetSIIdolCollectRequest x)
		{
			this.CollectSIIdol(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2.Invoke(request.error);
		});
		yield break;
	}

	// Token: 0x060038B7 RID: 14519 RVA: 0x0012EC96 File Offset: 0x0012CE96
	private IEnumerator DoGetActiveSIQuests(ProgressionManager.GetActiveSIQuestsRequest data, Action<List<RotatingQuest>> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetActiveSIQuestsRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.GetActiveSIQuests);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetActiveSIQuestsResponse getActiveSIQuestsResponse = JsonConvert.DeserializeObject<ProgressionManager.GetActiveSIQuestsResponse>(request.downloadHandler.text);
			Action<List<RotatingQuest>> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess.Invoke(getActiveSIQuestsResponse.Result.Quests);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure.Invoke(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetActiveSIQuestsRequest>(ProgressionManager.RequestType.GetActiveSIQuests, data, delegate(ProgressionManager.GetActiveSIQuestsRequest x)
		{
			this.GetActiveSIQuests(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2.Invoke(request.error);
		});
		yield break;
	}

	// Token: 0x060038B8 RID: 14520 RVA: 0x0012ECBA File Offset: 0x0012CEBA
	private IEnumerator DoGetSIQuestsStatus(ProgressionManager.GetSIQuestsStatusRequest data, Action<ProgressionManager.UserQuestsStatusResponse> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetSIQuestsStatusRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.GetSIQuestsStatus);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.GetSIQuestsStatusResponse getSIQuestsStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.GetSIQuestsStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserQuestsStatusResponse> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess.Invoke(getSIQuestsStatusResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure.Invoke(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetSIQuestsStatusRequest>(ProgressionManager.RequestType.GetSIQuestsStatus, data, delegate(ProgressionManager.GetSIQuestsStatusRequest x)
		{
			this.GetSIQuestStatus(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2.Invoke(request.error);
		});
		yield break;
	}

	// Token: 0x060038B9 RID: 14521 RVA: 0x0012ECDE File Offset: 0x0012CEDE
	private IEnumerator DoPurchaseTechPoints(ProgressionManager.PurchaseTechPointsRequest data, Action OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseTechPointsRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.PurchaseTechPoints);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			Action onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess.Invoke();
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure.Invoke(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseTechPointsRequest>(ProgressionManager.RequestType.PurchaseTechPoints, data, delegate(ProgressionManager.PurchaseTechPointsRequest x)
		{
			this.PurchaseTechPoints(data.TechPointsAmount, OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2.Invoke(request.error);
		});
		yield break;
	}

	// Token: 0x060038BA RID: 14522 RVA: 0x0012ED02 File Offset: 0x0012CF02
	private IEnumerator DoPurchaseResources(ProgressionManager.PurchaseResourcesRequest data, Action<ProgressionManager.UserInventory> OnSuccess, Action<string> OnFailure)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseResourcesRequest>(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl, data, ProgressionManager.RequestType.PurchaseResources);
		yield return request.SendWebRequest();
		if (this.IsSuccessResponse(request.responseCode))
		{
			ProgressionManager.UserInventoryResponse userInventoryResponse = JsonConvert.DeserializeObject<ProgressionManager.UserInventoryResponse>(request.downloadHandler.text);
			Action<ProgressionManager.UserInventory> onSuccess = OnSuccess;
			if (onSuccess != null)
			{
				onSuccess.Invoke(userInventoryResponse.Result);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			Action<string> onFailure = OnFailure;
			if (onFailure != null)
			{
				onFailure.Invoke(request.error);
			}
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseResourcesRequest>(ProgressionManager.RequestType.PurchaseResources, data, delegate(ProgressionManager.PurchaseResourcesRequest x)
		{
			this.PurchaseResources(OnSuccess, OnFailure);
		}, delegate
		{
			Action<string> onFailure2 = OnFailure;
			if (onFailure2 == null)
			{
				return;
			}
			onFailure2.Invoke(request.error);
		});
		yield break;
	}

	// Token: 0x060038BB RID: 14523 RVA: 0x0012ED26 File Offset: 0x0012CF26
	private IEnumerator DoPurchaseShiftCreditCapIncrease(ProgressionManager.PurchaseShiftCreditCapIncreaseRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseShiftCreditCapIncreaseRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.PurchaseShiftCreditCapIncreaseResponse purchaseShiftCreditCapIncreaseResponse = JsonConvert.DeserializeObject<ProgressionManager.PurchaseShiftCreditCapIncreaseResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease] = 0;
			this.RefreshShinyRocksTotal();
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData.Invoke(purchaseShiftCreditCapIncreaseResponse.TargetMothershipId, purchaseShiftCreditCapIncreaseResponse.CurrentShiftCreditCapIncreases, purchaseShiftCreditCapIncreaseResponse.CurrentShiftCreditCapIncreasesMax);
			}
			Action<bool> onPurchaseShiftCreditCapIncrease = this.OnPurchaseShiftCreditCapIncrease;
			if (onPurchaseShiftCreditCapIncrease != null)
			{
				onPurchaseShiftCreditCapIncrease.Invoke(true);
			}
			yield break;
		}
		if (request.responseCode == 400L && request.downloadHandler.text == "User Already Has Purchased Max Shift Credit Cap")
		{
			Action<bool> onPurchaseShiftCreditCapIncrease2 = this.OnPurchaseShiftCreditCapIncrease;
			if (onPurchaseShiftCreditCapIncrease2 != null)
			{
				onPurchaseShiftCreditCapIncrease2.Invoke(false);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseShiftCreditCapIncreaseRequest>(ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease, data, delegate(ProgressionManager.PurchaseShiftCreditCapIncreaseRequest x)
		{
			this.PurchaseShiftCreditCapIncreaseInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x060038BC RID: 14524 RVA: 0x0012ED3C File Offset: 0x0012CF3C
	private IEnumerator DoPurchaseShiftCredit(ProgressionManager.PurchaseShiftCreditRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseShiftCreditRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseShiftCredit);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.PurchaseShiftCreditResponse purchaseShiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.PurchaseShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.PurchaseShiftCredit] = 0;
			this.RefreshShinyRocksTotal();
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit.Invoke(purchaseShiftCreditResponse.TargetMothershipId, purchaseShiftCreditResponse.CurrentShiftCredits);
			}
			Action<bool> onPurchaseShiftCredit = this.OnPurchaseShiftCredit;
			if (onPurchaseShiftCredit != null)
			{
				onPurchaseShiftCredit.Invoke(true);
			}
			GRPlayer local = GRPlayer.GetLocal();
			if (local != null)
			{
				local.SendCreditsRefilledTelemetry(100, purchaseShiftCreditResponse.CurrentShiftCredits);
			}
			yield break;
		}
		if (request.responseCode == 400L && request.downloadHandler.text == "User Already at Max Shift Credit")
		{
			Action<bool> onPurchaseShiftCredit2 = this.OnPurchaseShiftCredit;
			if (onPurchaseShiftCredit2 != null)
			{
				onPurchaseShiftCredit2.Invoke(false);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseShiftCreditRequest>(ProgressionManager.RequestType.PurchaseShiftCredit, data, delegate(ProgressionManager.PurchaseShiftCreditRequest x)
		{
			this.PurchaseShiftCreditInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x060038BD RID: 14525 RVA: 0x0012ED52 File Offset: 0x0012CF52
	private IEnumerator DoGetShiftCredit(ProgressionManager.GetShiftCreditRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetShiftCreditRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetShiftCredit);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetShiftCredit] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit.Invoke(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData.Invoke(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetShiftCreditRequest>(ProgressionManager.RequestType.GetShiftCredit, data, delegate(ProgressionManager.GetShiftCreditRequest x)
		{
			this.GetShiftCredit(x.TargetMothershipId);
		}, null);
		yield break;
	}

	// Token: 0x060038BE RID: 14526 RVA: 0x0012ED68 File Offset: 0x0012CF68
	private IEnumerator DoGetJuicerStatus(ProgressionManager.GetJuicerStatusRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GetJuicerStatusRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetJuicerStatus);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			this.retryCounters[ProgressionManager.RequestType.GetJuicerStatus] = 0;
			ProgressionManager.JuicerStatusResponse juicerStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.JuicerStatusResponse>(request.downloadHandler.text);
			Action<ProgressionManager.JuicerStatusResponse> onJucierStatusUpdated = this.OnJucierStatusUpdated;
			if (onJucierStatusUpdated != null)
			{
				onJucierStatusUpdated.Invoke(juicerStatusResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GetJuicerStatusRequest>(ProgressionManager.RequestType.GetJuicerStatus, data, delegate(ProgressionManager.GetJuicerStatusRequest x)
		{
			this.GetJuicerStatusInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x060038BF RID: 14527 RVA: 0x0012ED7E File Offset: 0x0012CF7E
	private IEnumerator DoDepositCore(ProgressionManager.DepositCoreRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.DepositCoreRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.DepositCore);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			this.retryCounters[ProgressionManager.RequestType.DepositCore] = 0;
			if (data.CoreBeingDeposited == ProgressionManager.CoreType.ChaosSeed)
			{
				Action<bool> onChaosDepositSuccess = this.OnChaosDepositSuccess;
				if (onChaosDepositSuccess != null)
				{
					onChaosDepositSuccess.Invoke(true);
				}
				this.GetJuicerStatus();
			}
			else
			{
				ProgressionManager.DepositCoreResponse depositCoreResponse = JsonConvert.DeserializeObject<ProgressionManager.DepositCoreResponse>(request.downloadHandler.text);
				Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
				if (onGetShiftCredit != null)
				{
					onGetShiftCredit.Invoke(data.MothershipId, depositCoreResponse.CurrentShiftCredits);
				}
			}
			yield break;
		}
		if (request.responseCode == 400L && request.downloadHandler.text == "DepositGRCore already at seed cap")
		{
			if (data.CoreBeingDeposited == ProgressionManager.CoreType.ChaosSeed)
			{
				Action<bool> onChaosDepositSuccess2 = this.OnChaosDepositSuccess;
				if (onChaosDepositSuccess2 != null)
				{
					onChaosDepositSuccess2.Invoke(false);
				}
				this.GetJuicerStatus();
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.DepositCoreRequest>(ProgressionManager.RequestType.DepositCore, data, delegate(ProgressionManager.DepositCoreRequest x)
		{
			this.DepositCoreInternal(x.CoreBeingDeposited, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x060038C0 RID: 14528 RVA: 0x0012ED94 File Offset: 0x0012CF94
	private IEnumerator DoPurchaseOverdrive(ProgressionManager.PurchaseOverdriveRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseOverdriveRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseOverdrive);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			this.retryCounters[ProgressionManager.RequestType.PurchaseOverdrive] = 0;
			this.GetJuicerStatus();
			this.RefreshShinyRocksTotal();
			Action<bool> onPurchaseOverdrive = this.OnPurchaseOverdrive;
			if (onPurchaseOverdrive != null)
			{
				onPurchaseOverdrive.Invoke(true);
			}
			yield break;
		}
		if (request.responseCode == 400L && (request.downloadHandler.text == "User Already At Overdrive Cap" || request.downloadHandler.text == "User would exceed Overdrive Cap"))
		{
			Action<bool> onPurchaseOverdrive2 = this.OnPurchaseOverdrive;
			if (onPurchaseOverdrive2 != null)
			{
				onPurchaseOverdrive2.Invoke(false);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseOverdriveRequest>(ProgressionManager.RequestType.PurchaseOverdrive, data, delegate(ProgressionManager.PurchaseOverdriveRequest x)
		{
			this.PurchaseOverdriveInternal(request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x060038C1 RID: 14529 RVA: 0x0012EDAA File Offset: 0x0012CFAA
	private IEnumerator DoSubtractShiftCredit(ProgressionManager.SubtractShiftCreditRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SubtractShiftCreditRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.SubtractShiftCredit);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.SubtractShiftCredit] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit.Invoke(data.MothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData.Invoke(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SubtractShiftCreditRequest>(ProgressionManager.RequestType.SubtractShiftCredit, data, delegate(ProgressionManager.SubtractShiftCreditRequest x)
		{
			this.SubtractShiftCreditInternal(data.ShiftCreditToRemove, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x060038C2 RID: 14530 RVA: 0x0012EDC0 File Offset: 0x0012CFC0
	private IEnumerator DoAdvanceDockWristUpgradeLevel(ProgressionManager.AdvanceDockWristUpgradeRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.AdvanceDockWristUpgradeRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.AdvanceDockWristUpgrade);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.DockWristStatusResponse dockWristStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.DockWristStatusResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.AdvanceDockWristUpgrade] = 0;
			Action<ProgressionManager.DockWristStatusResponse> onDockWristStatusUpdated = this.OnDockWristStatusUpdated;
			if (onDockWristStatusUpdated != null)
			{
				onDockWristStatusUpdated.Invoke(dockWristStatusResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.AdvanceDockWristUpgradeRequest>(ProgressionManager.RequestType.AdvanceDockWristUpgrade, data, delegate(ProgressionManager.AdvanceDockWristUpgradeRequest x)
		{
			this.AdvanceDockWristUpgradeLevelInternal(data.Upgrade, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x060038C3 RID: 14531 RVA: 0x0012EDD6 File Offset: 0x0012CFD6
	private IEnumerator DoGetDockWristUpgradeStatus(ProgressionManager.DockWristUpgradeStatusRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.DockWristUpgradeStatusRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetDockWristUpgradeStatus);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.DockWristStatusResponse dockWristStatusResponse = JsonConvert.DeserializeObject<ProgressionManager.DockWristStatusResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetDockWristUpgradeStatus] = 0;
			Action<ProgressionManager.DockWristStatusResponse> onDockWristStatusUpdated = this.OnDockWristStatusUpdated;
			if (onDockWristStatusUpdated != null)
			{
				onDockWristStatusUpdated.Invoke(dockWristStatusResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.DockWristUpgradeStatusRequest>(ProgressionManager.RequestType.GetDockWristUpgradeStatus, data, delegate(ProgressionManager.DockWristUpgradeStatusRequest x)
		{
			this.GetDockWristUpgradeStatus();
		}, null);
		yield break;
	}

	// Token: 0x060038C4 RID: 14532 RVA: 0x0012EDEC File Offset: 0x0012CFEC
	private IEnumerator DoPurchaseDrillUpgrade(ProgressionManager.PurchaseDrillUpgradeRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.PurchaseDrillUpgradeRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.PurchaseDrillUpgrade);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			this.retryCounters[ProgressionManager.RequestType.PurchaseDrillUpgrade] = 0;
			this.RefreshUserInventory();
			Action<string, string> onNodeUnlocked = this.OnNodeUnlocked;
			if (onNodeUnlocked != null)
			{
				onNodeUnlocked.Invoke("", "");
			}
			if (data.Upgrade == ProgressionManager.DrillUpgradeLevel.Base)
			{
				GRPlayer local = GRPlayer.GetLocal();
				if (local != null)
				{
					local.SendPodUpgradeTelemetry(ProgressionManager.DrillUpgradeLevel.Base.ToString(), 0, 2500, 0);
				}
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.PurchaseDrillUpgradeRequest>(ProgressionManager.RequestType.PurchaseDrillUpgrade, data, delegate(ProgressionManager.PurchaseDrillUpgradeRequest x)
		{
			this.PurchaseDrillUpgrade(data.Upgrade);
		}, null);
		yield break;
	}

	// Token: 0x060038C5 RID: 14533 RVA: 0x0012EE02 File Offset: 0x0012D002
	private IEnumerator DoRecycleTool(ProgressionManager.RecycleToolRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.RecycleToolRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.RecycleTool);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.RecycleTool] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit.Invoke(data.MothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData.Invoke(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.RecycleToolRequest>(ProgressionManager.RequestType.RecycleTool, data, delegate(ProgressionManager.RecycleToolRequest x)
		{
			this.RecycleTool(data.ToolBeingRecycled, data.NumberOfPlayers);
		}, null);
		yield break;
	}

	// Token: 0x060038C6 RID: 14534 RVA: 0x0012EE18 File Offset: 0x0012D018
	private IEnumerator DoStartOfShift(ProgressionManager.StartOfShiftRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.StartOfShiftRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.StartOfShift);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			this.retryCounters[ProgressionManager.RequestType.StartOfShift] = 0;
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.StartOfShiftRequest>(ProgressionManager.RequestType.StartOfShift, data, delegate(ProgressionManager.StartOfShiftRequest x)
		{
			this.StartOfShift(data.ShiftId, data.CoresRequired, data.NumberOfPlayers, data.Depth);
		}, null);
		yield break;
	}

	// Token: 0x060038C7 RID: 14535 RVA: 0x0012EE2E File Offset: 0x0012D02E
	private IEnumerator DoEndOfShiftReward(ProgressionManager.EndOfShiftRewardRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.EndOfShiftRewardRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.EndOfShiftReward);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.ShiftCreditResponse shiftCreditResponse = JsonConvert.DeserializeObject<ProgressionManager.ShiftCreditResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.EndOfShiftReward] = 0;
			Action<string, int> onGetShiftCredit = this.OnGetShiftCredit;
			if (onGetShiftCredit != null)
			{
				onGetShiftCredit.Invoke(data.MothershipId, shiftCreditResponse.CurrentShiftCredits);
			}
			Action<string, int, int> onGetShiftCreditCapData = this.OnGetShiftCreditCapData;
			if (onGetShiftCreditCapData != null)
			{
				onGetShiftCreditCapData.Invoke(shiftCreditResponse.TargetMothershipId, shiftCreditResponse.CurrentShiftCreditCapIncreases, shiftCreditResponse.CurrentShiftCreditCapIncreasesMax);
			}
			yield break;
		}
		if (request.responseCode == 400L && request.error == "EndOfShiftReward Unknown Shift or Mothership Failure.")
		{
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.EndOfShiftRewardRequest>(ProgressionManager.RequestType.EndOfShiftReward, data, delegate(ProgressionManager.EndOfShiftRewardRequest x)
		{
			this.EndOfShiftRewardInternal(data.ShiftId, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x060038C8 RID: 14536 RVA: 0x0012EE44 File Offset: 0x0012D044
	private IEnumerator DoGetGhostReactorStats(ProgressionManager.GhostReactorStatsRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GhostReactorStatsRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetGhostReactorStats);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.GhostReactorStatsResponse ghostReactorStatsResponse = JsonConvert.DeserializeObject<ProgressionManager.GhostReactorStatsResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetGhostReactorStats] = 0;
			Action<ProgressionManager.GhostReactorStatsResponse> onGhostReactorStatsUpdated = this.OnGhostReactorStatsUpdated;
			if (onGhostReactorStatsUpdated != null)
			{
				onGhostReactorStatsUpdated.Invoke(ghostReactorStatsResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GhostReactorStatsRequest>(ProgressionManager.RequestType.GetGhostReactorStats, data, delegate(ProgressionManager.GhostReactorStatsRequest x)
		{
			this.GetGhostReactorStats();
		}, null);
		yield break;
	}

	// Token: 0x060038C9 RID: 14537 RVA: 0x0012EE5A File Offset: 0x0012D05A
	private IEnumerator DoGetGhostReactorInventory(ProgressionManager.GhostReactorInventoryRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.GhostReactorInventoryRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.GetGhostReactorInventory);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionManager.GhostReactorInventoryResponse ghostReactorInventoryResponse = JsonConvert.DeserializeObject<ProgressionManager.GhostReactorInventoryResponse>(request.downloadHandler.text);
			this.retryCounters[ProgressionManager.RequestType.GetGhostReactorInventory] = 0;
			Action<ProgressionManager.GhostReactorInventoryResponse> onGhostReactorInventoryUpdated = this.OnGhostReactorInventoryUpdated;
			if (onGhostReactorInventoryUpdated != null)
			{
				onGhostReactorInventoryUpdated.Invoke(ghostReactorInventoryResponse);
			}
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, false))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.GhostReactorInventoryRequest>(ProgressionManager.RequestType.GetGhostReactorInventory, data, delegate(ProgressionManager.GhostReactorInventoryRequest x)
		{
			this.GetGhostReactorInventory();
		}, null);
		yield break;
	}

	// Token: 0x060038CA RID: 14538 RVA: 0x0012EE70 File Offset: 0x0012D070
	private IEnumerator DoSetGhostReactorInventory(ProgressionManager.SetGhostReactorInventoryRequest data)
	{
		UnityWebRequest request = this.FormatWebRequest<ProgressionManager.SetGhostReactorInventoryRequest>(PlayFabAuthenticatorSettings.ProgressionApiBaseUrl, data, ProgressionManager.RequestType.SetGhostReactorInventory);
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			this.retryCounters[ProgressionManager.RequestType.SetGhostReactorInventory] = 0;
			yield break;
		}
		if (!this.HandleWebRequestFailures(request, true))
		{
			yield break;
		}
		yield return this.HandleWebRequestRetries<ProgressionManager.SetGhostReactorInventoryRequest>(ProgressionManager.RequestType.SetGhostReactorInventory, data, delegate(ProgressionManager.SetGhostReactorInventoryRequest x)
		{
			this.SetGhostReactorInventoryInternal(data.InventoryJson, request.responseCode == 409L);
		}, null);
		yield break;
	}

	// Token: 0x060038CB RID: 14539 RVA: 0x0012EE86 File Offset: 0x0012D086
	private bool IsSuccessResponse(long code)
	{
		return code >= 200L && code < 300L;
	}

	// Token: 0x060038CC RID: 14540 RVA: 0x0012EE9C File Offset: 0x0012D09C
	private UnityWebRequest FormatWebRequest<T>(string url, T pendingRequest, ProgressionManager.RequestType type)
	{
		string text = "";
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(pendingRequest));
		switch (type)
		{
		case ProgressionManager.RequestType.GetProgression:
			text = "/api/GetProgression";
			break;
		case ProgressionManager.RequestType.SetProgression:
			text = "/api/SetProgression";
			break;
		case ProgressionManager.RequestType.UnlockProgressionTreeNode:
			text = "/api/UnlockProgressionTreeNode";
			break;
		case ProgressionManager.RequestType.IncrementSIResource:
			text = "/api/IncrementSIResource";
			break;
		case ProgressionManager.RequestType.CompleteSIQuest:
			text = "/api/SetSIQuestComplete";
			break;
		case ProgressionManager.RequestType.CompleteSIBonus:
			text = "/api/SetSIBonusComplete";
			break;
		case ProgressionManager.RequestType.CollectSIIdol:
			text = "/api/SetSIIdolCollect";
			break;
		case ProgressionManager.RequestType.GetActiveSIQuests:
			text = "/api/GetActiveSIQuests";
			break;
		case ProgressionManager.RequestType.GetSIQuestsStatus:
			text = "/api/GetSIQuestsStatus";
			break;
		case ProgressionManager.RequestType.ResetSIQuestsStatus:
			text = "/api/ResetSIQuestsStatus";
			break;
		case ProgressionManager.RequestType.PurchaseTechPoints:
			text = "/api/PurchaseTechPoints";
			break;
		case ProgressionManager.RequestType.PurchaseResources:
			text = "/api/PurchaseResources";
			break;
		case ProgressionManager.RequestType.PurchaseShiftCreditCapIncrease:
			text = "/api/PurchaseShiftCreditCapIncrease";
			break;
		case ProgressionManager.RequestType.PurchaseShiftCredit:
			text = "/api/PurchaseShiftCredit";
			break;
		case ProgressionManager.RequestType.GetJuicerStatus:
			text = "/api/GetJuicerStatus";
			break;
		case ProgressionManager.RequestType.DepositCore:
			text = "/api/DepositGRCore";
			break;
		case ProgressionManager.RequestType.PurchaseOverdrive:
			text = "/api/PurchaseOverdrive";
			break;
		case ProgressionManager.RequestType.GetShiftCredit:
			text = "/api/GetShiftCredit";
			break;
		case ProgressionManager.RequestType.SubtractShiftCredit:
			text = "/api/SubtractShiftCredit";
			break;
		case ProgressionManager.RequestType.AdvanceDockWristUpgrade:
			text = "/api/AdvanceDockWristUpgrade";
			break;
		case ProgressionManager.RequestType.GetDockWristUpgradeStatus:
			text = "/api/GetDockWristUpgradeStatus";
			break;
		case ProgressionManager.RequestType.PurchaseDrillUpgrade:
			text = "/api/PurchaseDrillUpgrade";
			break;
		case ProgressionManager.RequestType.RecycleTool:
			text = "/api/RecycleTool";
			break;
		case ProgressionManager.RequestType.StartOfShift:
			text = "/api/StartOfShift";
			break;
		case ProgressionManager.RequestType.EndOfShiftReward:
			text = "/api/EndOfShiftReward";
			break;
		case ProgressionManager.RequestType.GetGhostReactorStats:
			text = "/api/GetGhostReactorStats";
			break;
		case ProgressionManager.RequestType.GetGhostReactorInventory:
			text = "/api/GetGhostReactorInventory";
			break;
		case ProgressionManager.RequestType.SetGhostReactorInventory:
			text = "/api/SetGhostReactorInventory";
			break;
		}
		UnityWebRequest unityWebRequest = new UnityWebRequest(url + text, "POST");
		unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
		unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
		unityWebRequest.SetRequestHeader("Content-Type", "application/json");
		return unityWebRequest;
	}

	// Token: 0x060038CD RID: 14541 RVA: 0x0012F07C File Offset: 0x0012D27C
	private void OnGetTrees(GetProgressionTreesForPlayerResponse response)
	{
		if (((response != null) ? response.Results : null) == null)
		{
			return;
		}
		this._trees.Clear();
		foreach (UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse in response.Results)
		{
			UserHydratedProgressionTreeResponse userHydratedProgressionTreeResponse2 = new UserHydratedProgressionTreeResponse();
			userHydratedProgressionTreeResponse2.Tree = userHydratedProgressionTreeResponse.Tree;
			userHydratedProgressionTreeResponse2.Track = userHydratedProgressionTreeResponse.Track;
			userHydratedProgressionTreeResponse2.Nodes = userHydratedProgressionTreeResponse.Nodes;
			this._trees[userHydratedProgressionTreeResponse.Tree.name] = userHydratedProgressionTreeResponse2;
		}
		Action onTreeUpdated = this.OnTreeUpdated;
		if (onTreeUpdated == null)
		{
			return;
		}
		onTreeUpdated.Invoke();
	}

	// Token: 0x060038CE RID: 14542 RVA: 0x0012F130 File Offset: 0x0012D330
	private void OnGetInventory(MothershipGetInventoryResponse response)
	{
		if (((response != null) ? response.Results : null) == null)
		{
			return;
		}
		this._inventory.Clear();
		foreach (KeyValuePair<string, MothershipPlayerInventorySummary> keyValuePair in response.Results)
		{
			MothershipPlayerInventorySummary value = keyValuePair.Value;
			if (((value != null) ? value.entitlements : null) != null)
			{
				foreach (MothershipInventoryItemSummary mothershipInventoryItemSummary in keyValuePair.Value.entitlements)
				{
					string name = mothershipInventoryItemSummary.name;
					string text = (name != null) ? name.Trim() : null;
					this._inventory[text] = new ProgressionManager.MothershipItemSummary
					{
						EntitlementId = mothershipInventoryItemSummary.entitlement_id,
						InGameId = mothershipInventoryItemSummary.in_game_id,
						Name = mothershipInventoryItemSummary.name,
						Quantity = mothershipInventoryItemSummary.quantity
					};
				}
			}
		}
		Action onInventoryUpdated = this.OnInventoryUpdated;
		if (onInventoryUpdated == null)
		{
			return;
		}
		onInventoryUpdated.Invoke();
	}

	// Token: 0x060038CF RID: 14543 RVA: 0x0012F258 File Offset: 0x0012D458
	public int GetShinyRocksTotal()
	{
		if (CosmeticsController.instance != null)
		{
			return CosmeticsController.instance.CurrencyBalance;
		}
		return 0;
	}

	// Token: 0x060038D0 RID: 14544 RVA: 0x0012F277 File Offset: 0x0012D477
	public void RefreshShinyRocksTotal()
	{
		if (CosmeticsController.instance != null)
		{
			CosmeticsController.instance.GetCurrencyBalance();
		}
	}

	// Token: 0x060038D1 RID: 14545 RVA: 0x0012F294 File Offset: 0x0012D494
	public static void GetMothershipFailure(MothershipError callError, int errorCode)
	{
		Debug.LogError("Progression: GetMothershipFailure: " + callError.MothershipErrorCode + ":" + callError.Message);
	}

	// Token: 0x04004781 RID: 18305
	private readonly Dictionary<string, UserHydratedProgressionTreeResponse> _trees = new Dictionary<string, UserHydratedProgressionTreeResponse>();

	// Token: 0x04004782 RID: 18306
	private readonly Dictionary<string, ProgressionManager.MothershipItemSummary> _inventory = new Dictionary<string, ProgressionManager.MothershipItemSummary>();

	// Token: 0x04004783 RID: 18307
	private readonly Dictionary<string, int> _tracks = new Dictionary<string, int>();

	// Token: 0x04004784 RID: 18308
	private Dictionary<ProgressionManager.RequestType, int> retryCounters = new Dictionary<ProgressionManager.RequestType, int>();

	// Token: 0x04004785 RID: 18309
	private int maxRetriesOnFail = 4;

	// Token: 0x0200085F RID: 2143
	public struct MothershipItemSummary
	{
		// Token: 0x04004786 RID: 18310
		public string Name;

		// Token: 0x04004787 RID: 18311
		public string EntitlementId;

		// Token: 0x04004788 RID: 18312
		public string InGameId;

		// Token: 0x04004789 RID: 18313
		public int Quantity;
	}

	// Token: 0x02000860 RID: 2144
	private enum RequestType
	{
		// Token: 0x0400478B RID: 18315
		GetProgression,
		// Token: 0x0400478C RID: 18316
		SetProgression,
		// Token: 0x0400478D RID: 18317
		UnlockProgressionTreeNode,
		// Token: 0x0400478E RID: 18318
		IncrementSIResource,
		// Token: 0x0400478F RID: 18319
		CompleteSIQuest,
		// Token: 0x04004790 RID: 18320
		CompleteSIBonus,
		// Token: 0x04004791 RID: 18321
		CollectSIIdol,
		// Token: 0x04004792 RID: 18322
		GetActiveSIQuests,
		// Token: 0x04004793 RID: 18323
		GetSIQuestsStatus,
		// Token: 0x04004794 RID: 18324
		ResetSIQuestsStatus,
		// Token: 0x04004795 RID: 18325
		PurchaseTechPoints,
		// Token: 0x04004796 RID: 18326
		PurchaseResources,
		// Token: 0x04004797 RID: 18327
		PurchaseShiftCreditCapIncrease,
		// Token: 0x04004798 RID: 18328
		PurchaseShiftCredit,
		// Token: 0x04004799 RID: 18329
		RegisterToGRShift,
		// Token: 0x0400479A RID: 18330
		GetJuicerStatus,
		// Token: 0x0400479B RID: 18331
		DepositCore,
		// Token: 0x0400479C RID: 18332
		PurchaseOverdrive,
		// Token: 0x0400479D RID: 18333
		GetShiftCredit,
		// Token: 0x0400479E RID: 18334
		SubtractShiftCredit,
		// Token: 0x0400479F RID: 18335
		AdvanceDockWristUpgrade,
		// Token: 0x040047A0 RID: 18336
		GetDockWristUpgradeStatus,
		// Token: 0x040047A1 RID: 18337
		PurchaseDrillUpgrade,
		// Token: 0x040047A2 RID: 18338
		RecycleTool,
		// Token: 0x040047A3 RID: 18339
		StartOfShift,
		// Token: 0x040047A4 RID: 18340
		EndOfShiftReward,
		// Token: 0x040047A5 RID: 18341
		GetGhostReactorStats,
		// Token: 0x040047A6 RID: 18342
		GetGhostReactorInventory,
		// Token: 0x040047A7 RID: 18343
		SetGhostReactorInventory
	}

	// Token: 0x02000861 RID: 2145
	public enum WristDockUpgradeType
	{
		// Token: 0x040047A9 RID: 18345
		None,
		// Token: 0x040047AA RID: 18346
		Upgrade1,
		// Token: 0x040047AB RID: 18347
		Upgrade2,
		// Token: 0x040047AC RID: 18348
		Upgrade3
	}

	// Token: 0x02000862 RID: 2146
	public enum DrillUpgradeLevel
	{
		// Token: 0x040047AE RID: 18350
		None,
		// Token: 0x040047AF RID: 18351
		Base,
		// Token: 0x040047B0 RID: 18352
		Upgrade1,
		// Token: 0x040047B1 RID: 18353
		Upgrade2,
		// Token: 0x040047B2 RID: 18354
		Upgrade3
	}

	// Token: 0x02000863 RID: 2147
	public enum CoreType
	{
		// Token: 0x040047B4 RID: 18356
		None,
		// Token: 0x040047B5 RID: 18357
		Core,
		// Token: 0x040047B6 RID: 18358
		SuperCore,
		// Token: 0x040047B7 RID: 18359
		ChaosSeed
	}

	// Token: 0x02000864 RID: 2148
	[Serializable]
	private class GetProgressionRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x040047B8 RID: 18360
		public string TrackId;
	}

	// Token: 0x02000865 RID: 2149
	[Serializable]
	private class GetProgressionResponse
	{
		// Token: 0x040047B9 RID: 18361
		public string Track;

		// Token: 0x040047BA RID: 18362
		public int Progress;

		// Token: 0x040047BB RID: 18363
		public int StatusCode;

		// Token: 0x040047BC RID: 18364
		public string Error;
	}

	// Token: 0x02000866 RID: 2150
	[Serializable]
	private class SetProgressionRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x040047BD RID: 18365
		public string TrackId;

		// Token: 0x040047BE RID: 18366
		public int Progress;
	}

	// Token: 0x02000867 RID: 2151
	[Serializable]
	private class SetProgressionResponse
	{
		// Token: 0x040047BF RID: 18367
		public string Track;

		// Token: 0x040047C0 RID: 18368
		public int Progress;

		// Token: 0x040047C1 RID: 18369
		public int StatusCode;

		// Token: 0x040047C2 RID: 18370
		public string Error;
	}

	// Token: 0x02000868 RID: 2152
	[Serializable]
	private class UnlockNodeRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x040047C3 RID: 18371
		public string TreeId;

		// Token: 0x040047C4 RID: 18372
		public string NodeId;
	}

	// Token: 0x02000869 RID: 2153
	[Serializable]
	private class UnlockNodeResponse
	{
		// Token: 0x040047C5 RID: 18373
		public UserHydratedProgressionTreeResponse Tree;

		// Token: 0x040047C6 RID: 18374
		public int StatusCode;

		// Token: 0x040047C7 RID: 18375
		public string Error;
	}

	// Token: 0x0200086A RID: 2154
	[Serializable]
	private class IncrementSIResourceRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x040047C8 RID: 18376
		public string ResourceType;
	}

	// Token: 0x0200086B RID: 2155
	[Serializable]
	private class IncrementSIResourceResponse : ProgressionManager.UserInventoryResponse
	{
		// Token: 0x040047C9 RID: 18377
		public string ResourceType;
	}

	// Token: 0x0200086C RID: 2156
	[Serializable]
	private class GetActiveSIQuestsRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x0200086D RID: 2157
	[Serializable]
	private class GetActiveSIQuestsResponse
	{
		// Token: 0x040047CA RID: 18378
		public ProgressionManager.GetActiveSIQuestsResult Result;

		// Token: 0x040047CB RID: 18379
		public int StatusCode;

		// Token: 0x040047CC RID: 18380
		public string Error;
	}

	// Token: 0x0200086E RID: 2158
	[Serializable]
	public class GetActiveSIQuestsResult
	{
		// Token: 0x040047CD RID: 18381
		public List<RotatingQuest> Quests;
	}

	// Token: 0x0200086F RID: 2159
	[Serializable]
	private class GetSIQuestsStatusRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000870 RID: 2160
	[Serializable]
	private class ResetSIQuestsStatusRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000871 RID: 2161
	[Serializable]
	private class PurchaseTechPointsRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x040047CE RID: 18382
		public int TechPointsAmount;
	}

	// Token: 0x02000872 RID: 2162
	private class PurchaseResourcesRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000873 RID: 2163
	[Serializable]
	private class GetSIQuestsStatusResponse
	{
		// Token: 0x040047CF RID: 18383
		public ProgressionManager.UserQuestsStatusResponse Result;
	}

	// Token: 0x02000874 RID: 2164
	[Serializable]
	private class UserInventoryResponse
	{
		// Token: 0x040047D0 RID: 18384
		public ProgressionManager.UserInventory Result;
	}

	// Token: 0x02000875 RID: 2165
	[Serializable]
	public class UserInventory
	{
		// Token: 0x040047D1 RID: 18385
		public Dictionary<string, int> Inventory;
	}

	// Token: 0x02000876 RID: 2166
	[Serializable]
	private class SetSIQuestCompleteRequest : ProgressionManager.RewardRequest
	{
		// Token: 0x040047D2 RID: 18386
		public int QuestID;
	}

	// Token: 0x02000877 RID: 2167
	[Serializable]
	private class SetSIBonusCompleteRequest : ProgressionManager.RewardRequest
	{
	}

	// Token: 0x02000878 RID: 2168
	[Serializable]
	private class SetSIIdolCollectRequest : ProgressionManager.RewardRequest
	{
	}

	// Token: 0x02000879 RID: 2169
	[Serializable]
	private class RewardRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x0200087A RID: 2170
	[Serializable]
	private class MothershipRequest
	{
		// Token: 0x040047D3 RID: 18387
		public string MothershipId;

		// Token: 0x040047D4 RID: 18388
		public string MothershipToken;

		// Token: 0x040047D5 RID: 18389
		public string MothershipEnvId;

		// Token: 0x040047D6 RID: 18390
		public string MothershipTitleId;

		// Token: 0x040047D7 RID: 18391
		public string MothershipDeploymentId;
	}

	// Token: 0x0200087B RID: 2171
	[Serializable]
	private class MothershipUserDataWriteRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x040047D8 RID: 18392
		public bool SkipUserDataCache;
	}

	// Token: 0x0200087C RID: 2172
	[Serializable]
	public class UserQuestsStatusResponse
	{
		// Token: 0x040047D9 RID: 18393
		public int TodayClaimableQuests;

		// Token: 0x040047DA RID: 18394
		public int TodayClaimableBonus;

		// Token: 0x040047DB RID: 18395
		public int TodayClaimableIdol;
	}

	// Token: 0x0200087D RID: 2173
	[Serializable]
	private class PurchaseShiftCreditCapIncreaseRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x0200087E RID: 2174
	[Serializable]
	private class PurchaseShiftCreditCapIncreaseResponse
	{
		// Token: 0x040047DC RID: 18396
		public int StatusCode;

		// Token: 0x040047DD RID: 18397
		public string Error;

		// Token: 0x040047DE RID: 18398
		public int CurrentShiftCreditCapIncreases;

		// Token: 0x040047DF RID: 18399
		public int CurrentShiftCreditCapIncreasesMax;

		// Token: 0x040047E0 RID: 18400
		public string TargetMothershipId;
	}

	// Token: 0x0200087F RID: 2175
	[Serializable]
	private class PurchaseShiftCreditRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000880 RID: 2176
	[Serializable]
	private class PurchaseShiftCreditResponse
	{
		// Token: 0x040047E1 RID: 18401
		public int StatusCode;

		// Token: 0x040047E2 RID: 18402
		public string Error;

		// Token: 0x040047E3 RID: 18403
		public int CurrentShiftCredits;

		// Token: 0x040047E4 RID: 18404
		public string TargetMothershipId;
	}

	// Token: 0x02000881 RID: 2177
	[Serializable]
	private class GetShiftCreditRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x040047E5 RID: 18405
		public string TargetMothershipId;
	}

	// Token: 0x02000882 RID: 2178
	[Serializable]
	public class ShiftCreditResponse
	{
		// Token: 0x040047E6 RID: 18406
		public int StatusCode;

		// Token: 0x040047E7 RID: 18407
		public string Error;

		// Token: 0x040047E8 RID: 18408
		public int CurrentShiftCredits;

		// Token: 0x040047E9 RID: 18409
		public int CurrentShiftCreditCapIncreases;

		// Token: 0x040047EA RID: 18410
		public int CurrentShiftCreditCapIncreasesMax;

		// Token: 0x040047EB RID: 18411
		public string TargetMothershipId;
	}

	// Token: 0x02000883 RID: 2179
	[Serializable]
	private class GetJuicerStatusRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000884 RID: 2180
	[Serializable]
	private class DepositCoreRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x040047EC RID: 18412
		public ProgressionManager.CoreType CoreBeingDeposited;
	}

	// Token: 0x02000885 RID: 2181
	[Serializable]
	private class DepositCoreResponse
	{
		// Token: 0x040047ED RID: 18413
		public int StatusCode;

		// Token: 0x040047EE RID: 18414
		public string Error;

		// Token: 0x040047EF RID: 18415
		public int CurrentShiftCredits;
	}

	// Token: 0x02000886 RID: 2182
	[Serializable]
	private class PurchaseOverdriveRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
	}

	// Token: 0x02000887 RID: 2183
	[Serializable]
	public class JuicerStatusResponse
	{
		// Token: 0x040047F0 RID: 18416
		public string MothershipId;

		// Token: 0x040047F1 RID: 18417
		public int StatusCode;

		// Token: 0x040047F2 RID: 18418
		public string Error;

		// Token: 0x040047F3 RID: 18419
		public int CurrentCoreCount;

		// Token: 0x040047F4 RID: 18420
		public int CoreProcessingTimeSec;

		// Token: 0x040047F5 RID: 18421
		public float CoreProcessingPercent;

		// Token: 0x040047F6 RID: 18422
		public int OverdriveSupply;

		// Token: 0x040047F7 RID: 18423
		public int OverdriveCap;

		// Token: 0x040047F8 RID: 18424
		public int CoresProcessedByOverdrive;

		// Token: 0x040047F9 RID: 18425
		public bool RefreshJuice;
	}

	// Token: 0x02000888 RID: 2184
	[Serializable]
	private class SubtractShiftCreditRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x040047FA RID: 18426
		public int ShiftCreditToRemove;
	}

	// Token: 0x02000889 RID: 2185
	[Serializable]
	private class AdvanceDockWristUpgradeRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x040047FB RID: 18427
		public ProgressionManager.WristDockUpgradeType Upgrade;
	}

	// Token: 0x0200088A RID: 2186
	[Serializable]
	private class DockWristUpgradeStatusRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x0200088B RID: 2187
	[Serializable]
	public class DockWristStatusResponse
	{
		// Token: 0x040047FC RID: 18428
		public int CurrentUpgrade1Level;

		// Token: 0x040047FD RID: 18429
		public int CurrentUpgrade2Level;

		// Token: 0x040047FE RID: 18430
		public int CurrentUpgrade3Level;

		// Token: 0x040047FF RID: 18431
		public int Upgrade1LevelMax;

		// Token: 0x04004800 RID: 18432
		public int Upgrade2LevelMax;

		// Token: 0x04004801 RID: 18433
		public int Upgrade3LevelMax;
	}

	// Token: 0x0200088C RID: 2188
	[Serializable]
	private class PurchaseDrillUpgradeRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004802 RID: 18434
		public ProgressionManager.DrillUpgradeLevel Upgrade;
	}

	// Token: 0x0200088D RID: 2189
	[Serializable]
	private class PurchaseDrillUpgradeResponse
	{
		// Token: 0x04004803 RID: 18435
		public int StatusCode;

		// Token: 0x04004804 RID: 18436
		public string Error;
	}

	// Token: 0x0200088E RID: 2190
	[Serializable]
	private class RecycleToolRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004805 RID: 18437
		public GRTool.GRToolType ToolBeingRecycled;

		// Token: 0x04004806 RID: 18438
		public int NumberOfPlayers;
	}

	// Token: 0x0200088F RID: 2191
	[Serializable]
	private class StartOfShiftRequest : ProgressionManager.MothershipRequest
	{
		// Token: 0x04004807 RID: 18439
		public string ShiftId;

		// Token: 0x04004808 RID: 18440
		public int CoresRequired;

		// Token: 0x04004809 RID: 18441
		public int NumberOfPlayers;

		// Token: 0x0400480A RID: 18442
		public int Depth;
	}

	// Token: 0x02000890 RID: 2192
	[Serializable]
	private class EndOfShiftRewardRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x0400480B RID: 18443
		public string ShiftId;
	}

	// Token: 0x02000891 RID: 2193
	[Serializable]
	private class GhostReactorStatsRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000892 RID: 2194
	[Serializable]
	public class GhostReactorStatsResponse
	{
		// Token: 0x0400480C RID: 18444
		public string MothershipId;

		// Token: 0x0400480D RID: 18445
		public int MaxDepthReached;
	}

	// Token: 0x02000893 RID: 2195
	[Serializable]
	private class GhostReactorInventoryRequest : ProgressionManager.MothershipRequest
	{
	}

	// Token: 0x02000894 RID: 2196
	[Serializable]
	public class GhostReactorInventoryResponse
	{
		// Token: 0x0400480E RID: 18446
		public string MothershipId;

		// Token: 0x0400480F RID: 18447
		public string InventoryJson;
	}

	// Token: 0x02000895 RID: 2197
	[Serializable]
	private class SetGhostReactorInventoryRequest : ProgressionManager.MothershipUserDataWriteRequest
	{
		// Token: 0x04004810 RID: 18448
		public string InventoryJson;
	}

	// Token: 0x02000896 RID: 2198
	[Serializable]
	public class SetGhostReactorInventoryResponse
	{
		// Token: 0x04004811 RID: 18449
		public string MothershipId;
	}
}
