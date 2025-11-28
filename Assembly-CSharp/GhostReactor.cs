using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Fusion;
using GorillaTag.Rendering;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000638 RID: 1592
public class GhostReactor : MonoBehaviourTick, IBuildValidation
{
	// Token: 0x06002895 RID: 10389 RVA: 0x000D7DB0 File Offset: 0x000D5FB0
	public static GhostReactor Get(GameEntity gameEntity)
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(gameEntity);
		if (ghostReactorManager == null)
		{
			return null;
		}
		return ghostReactorManager.reactor;
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x000D7DD8 File Offset: 0x000D5FD8
	private void Awake()
	{
		GhostReactor.instance = this;
		this.reviveStations = new List<GRReviveStation>();
		base.GetComponentsInChildren<GRReviveStation>(this.reviveStations);
		for (int i = 0; i < this.reviveStations.Count; i++)
		{
			this.reviveStations[i].Init(this, i);
		}
		this.vrRigs = new List<VRRig>();
		for (int j = 0; j < this.itemPurchaseStands.Count; j++)
		{
			if (this.itemPurchaseStands[j] == null)
			{
				Debug.LogErrorFormat("Null Item Purchase Stand {0}", new object[]
				{
					j
				});
			}
			else
			{
				this.itemPurchaseStands[j].Setup(j);
			}
		}
		for (int k = 0; k < this.toolPurchasingStations.Count; k++)
		{
			if (this.toolPurchasingStations[k] == null)
			{
				Debug.LogErrorFormat("Null Tool Purchasing Station {0}", new object[]
				{
					k
				});
			}
			else
			{
				this.toolPurchasingStations[k].PurchaseStationId = k;
			}
		}
		if (this.promotionBot != null)
		{
			this.promotionBot.Init(this);
		}
		this.randomGenerator = new SRand(Random.Range(0, int.MaxValue));
		this.handPrintMPB = new MaterialPropertyBlock();
		this.handPrintMPB.SetFloatArray("_HandPrintData", new float[1024]);
		this.bays = new List<GRBay>(32);
		base.GetComponentsInChildren<GRBay>(false, this.bays);
		this.storeDisplays = new List<GRUIStoreDisplay>();
		base.GetComponentsInChildren<GRUIStoreDisplay>(false, this.storeDisplays);
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x000D7F6C File Offset: 0x000D616C
	private new void OnEnable()
	{
		base.OnEnable();
		if (this.zone == GTZone.customMaps)
		{
			return;
		}
		GTDev.Log<string>(string.Format("GhostReactor::OnEnable getting manager for zone {0}", this.zone), null);
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
		if (managerForZone == null)
		{
			Debug.LogErrorFormat("No GameEntityManager found for zone {0}", new object[]
			{
				this.zone
			});
			return;
		}
		this.grManager = managerForZone.ghostReactorManager;
		if (this.grManager == null)
		{
			Debug.LogErrorFormat("No GhostReactorManager found for zone {0}", new object[]
			{
				this.zone
			});
			return;
		}
		this.grManager.reactor = this;
		this.grManager.gameEntityManager.zoneLimit = this.zoneLimit;
		if (GameLightingManager.instance != null && this.zone != GTZone.customMaps)
		{
			GameLightingManager.instance.ZoneEnableCustomDynamicLighting(true);
		}
		VRRigCache.OnRigActivated += new Action<RigContainer>(this.OnVRRigsChanged);
		VRRigCache.OnRigDeactivated += new Action<RigContainer>(this.OnVRRigsChanged);
		VRRigCache.OnRigNameChanged += new Action<RigContainer>(this.OnVRRigsChanged);
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.OnLocalPlayerConnectedToRoom);
		}
		for (int i = 0; i < this.toolPurchasingStations.Count; i++)
		{
			this.toolPurchasingStations[i].Init(this.grManager, this);
		}
		if (this.debugUpgradeKiosk != null)
		{
			this.debugUpgradeKiosk.Init(this.grManager, this);
		}
		if (this.currencyDepositor != null)
		{
			this.currencyDepositor.Init(this);
		}
		if (this.distillery != null)
		{
			this.distillery.Init(this);
		}
		if (this.seedExtractor != null)
		{
			this.seedExtractor.Init(this.toolProgression, this);
		}
		if (this.levelGenerator != null)
		{
			this.levelGenerator.Init(this);
		}
		if (this.employeeBadges != null)
		{
			this.employeeBadges.Init(this);
		}
		if (this.toolProgression != null)
		{
			this.toolProgression.Init(this);
			this.toolProgression.OnProgressionUpdated += new Action(this.OnProgressionUpdated);
		}
		if (this.shiftManager != null)
		{
			this.shiftManager.Init(this.grManager);
		}
		for (int j = 0; j < this.toolUpgradePurchaseStationsFull.Count; j++)
		{
			this.toolUpgradePurchaseStationsFull[j].Init(this.toolProgression, this);
		}
		GRElevatorManager._instance.InitShuttles(this);
		if (this.recycler != null)
		{
			this.recycler.Init(this);
		}
		if (this.zoneShaderSettings != null)
		{
			this.zoneShaderSettings.BecomeActiveInstance(true);
		}
		for (int k = 0; k < this.bays.Count; k++)
		{
			this.bays[k].Setup(this);
		}
		for (int l = 0; l < this.storeDisplays.Count; l++)
		{
			this.storeDisplays[l].Setup(-1, this);
		}
		this.RefreshDepth();
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x000D82AA File Offset: 0x000D64AA
	public void EnableGhostReactorForVirtualStump()
	{
		GhostReactor.instance = this;
		this.RefreshReviveStations(false);
		this.OnEnable();
	}

	// Token: 0x06002899 RID: 10393 RVA: 0x000D82C0 File Offset: 0x000D64C0
	public void RefreshReviveStations(bool searchScene = false)
	{
		this.reviveStations = new List<GRReviveStation>();
		base.GetComponentsInChildren<GRReviveStation>(this.reviveStations);
		if (searchScene)
		{
			this.reviveStations.AddRange(Object.FindObjectsByType<GRReviveStation>(1, 0));
		}
		for (int i = 0; i < this.reviveStations.Count; i++)
		{
			this.reviveStations[i].Init(this, i);
		}
	}

	// Token: 0x0600289A RID: 10394 RVA: 0x000D8324 File Offset: 0x000D6524
	private new void OnDisable()
	{
		base.OnDisable();
		if (this.zone == GTZone.customMaps)
		{
			return;
		}
		GameLightingManager.instance.ZoneEnableCustomDynamicLighting(false);
		VRRigCache.OnRigActivated -= new Action<RigContainer>(this.OnVRRigsChanged);
		VRRigCache.OnRigDeactivated -= new Action<RigContainer>(this.OnVRRigsChanged);
		VRRigCache.OnRigNameChanged -= new Action<RigContainer>(this.OnVRRigsChanged);
		if (this.toolProgression != null)
		{
			this.toolProgression.OnProgressionUpdated -= new Action(this.OnProgressionUpdated);
		}
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnMultiplayerStarted -= new Action(this.OnLocalPlayerConnectedToRoom);
		}
	}

	// Token: 0x0600289B RID: 10395 RVA: 0x000D83D5 File Offset: 0x000D65D5
	private void OnProgressionUpdated()
	{
		if (this.toolProgression != null)
		{
			this.UpdateLocalPlayerFromProgression();
		}
	}

	// Token: 0x0600289C RID: 10396 RVA: 0x000D83EC File Offset: 0x000D65EC
	public void UpdateLocalPlayerFromProgression()
	{
		GRPlayer local = GRPlayer.GetLocal();
		if (local != null)
		{
			int dropPodLevel = this.toolProgression.GetDropPodLevel();
			if (local.dropPodLevel != dropPodLevel)
			{
				local.dropPodLevel = dropPodLevel;
				Debug.LogFormat("Drop Pod UpdateLocalPlayerFromProgression Level {0} {1} {2}", new object[]
				{
					this.grManager.IsZoneActive(),
					local.dropPodLevel,
					local.dropPodChasisLevel
				});
				if (this.grManager.IsZoneActive())
				{
					this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodLevel, dropPodLevel);
				}
			}
			int dropPodChasisLevel = this.toolProgression.GetDropPodChasisLevel();
			if (local.dropPodChasisLevel != dropPodChasisLevel)
			{
				local.dropPodChasisLevel = dropPodChasisLevel;
				Debug.LogFormat("Drop Pod UpdateLocalPlayerFromProgression Level {0} {1} {2}", new object[]
				{
					this.grManager.IsZoneActive(),
					local.dropPodLevel,
					local.dropPodChasisLevel
				});
				if (this.grManager.IsZoneActive())
				{
					this.grManager.RequestPlayerAction(GhostReactorManager.GRPlayerAction.SetPodChassisLevel, dropPodChasisLevel);
				}
			}
			if (local.badge)
			{
				local.badge.RefreshText(PhotonNetwork.LocalPlayer);
			}
			this.RefreshStore();
		}
	}

	// Token: 0x0600289D RID: 10397 RVA: 0x000D851F File Offset: 0x000D671F
	public GRPatrolPath GetPatrolPath(long createData)
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		return this.levelGenerator.GetPatrolPath(createData);
	}

	// Token: 0x0600289E RID: 10398 RVA: 0x000D8540 File Offset: 0x000D6740
	public override void Tick()
	{
		if (this.grManager == null)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		if (this.grManager.gameEntityManager.IsAuthority())
		{
			if (Time.timeAsDouble - this.lastCollectibleDispenserUpdateTime > (double)this.collectibleDispenserUpdateFrequency)
			{
				this.lastCollectibleDispenserUpdateTime = Time.timeAsDouble;
				for (int i = 0; i < this.collectibleDispensers.Count; i++)
				{
					if (this.collectibleDispensers[i] != null && this.collectibleDispensers[i].ReadyToDispenseNewCollectible)
					{
						this.collectibleDispensers[i].RequestDispenseCollectible();
					}
				}
			}
			if (this.sleepableEntities.Count > 0)
			{
				this.sentientCoreUpdateIndex = Mathf.Max(0, this.sentientCoreUpdateIndex % this.sleepableEntities.Count);
				if (this.sentientCoreUpdateIndex < this.sleepableEntities.Count)
				{
					IGRSleepableEntity igrsleepableEntity = this.sleepableEntities[this.sentientCoreUpdateIndex];
					float num = igrsleepableEntity.WakeUpRadius * igrsleepableEntity.WakeUpRadius;
					float num2 = (igrsleepableEntity.WakeUpRadius + 0.5f) * (igrsleepableEntity.WakeUpRadius + 0.5f);
					bool flag = false;
					bool flag2 = false;
					for (int j = 0; j < this.vrRigs.Count; j++)
					{
						GRPlayer component = this.vrRigs[j].GetComponent<GRPlayer>();
						if (!(component == null) && component.State != GRPlayer.GRPlayerState.Ghost)
						{
							float sqrMagnitude = (igrsleepableEntity.Position - this.vrRigs[j].bodyTransform.position).sqrMagnitude;
							if (sqrMagnitude < num2)
							{
								flag = true;
							}
							if (sqrMagnitude < num)
							{
								flag2 = true;
								break;
							}
						}
					}
					bool flag3 = igrsleepableEntity.IsSleeping();
					if (flag3 && flag2)
					{
						igrsleepableEntity.WakeUp();
					}
					else if (!flag3 && !flag)
					{
						igrsleepableEntity.Sleep();
					}
					this.sentientCoreUpdateIndex++;
				}
			}
		}
		bool flag4 = false;
		foreach (GhostReactor.EntityTypeRespawnTracker entityTypeRespawnTracker in this.respawnQueue)
		{
			entityTypeRespawnTracker.entityNextRespawnTime -= Time.deltaTime;
			if (entityTypeRespawnTracker.entityNextRespawnTime < 0f)
			{
				entityTypeRespawnTracker.entityNextRespawnTime = 0f;
				flag4 = true;
				if (this.grManager.gameEntityManager.IsAuthority())
				{
					this.levelGenerator.RespawnEntity(entityTypeRespawnTracker.entityTypeID, entityTypeRespawnTracker.entityCreateData);
				}
			}
		}
		if (flag4)
		{
			this.respawnQueue.RemoveAll((GhostReactor.EntityTypeRespawnTracker e) => e.entityNextRespawnTime <= 0f);
		}
		this.UpdateHandprints(Time.deltaTime);
	}

	// Token: 0x0600289F RID: 10399 RVA: 0x000D87F4 File Offset: 0x000D69F4
	private void OnLocalPlayerConnectedToRoom()
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null)
		{
			grplayer.Reset();
		}
		if (this.shiftManager != null)
		{
			this.shiftManager.shiftStats.ResetShiftStats();
			this.shiftManager.RefreshShiftStatsDisplay();
		}
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x000D8844 File Offset: 0x000D6A44
	private void OnVRRigsChanged(RigContainer container)
	{
		this.VRRigRefresh();
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x000D884C File Offset: 0x000D6A4C
	public void VRRigRefresh()
	{
		if (this.isRefreshing)
		{
			return;
		}
		this.isRefreshing = true;
		this.vrRigs.Clear();
		this.vrRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.vrRigs.Sort(delegate(VRRig a, VRRig b)
		{
			if (a == null || a.OwningNetPlayer == null)
			{
				return 1;
			}
			if (b == null || b.OwningNetPlayer == null)
			{
				return -1;
			}
			return a.OwningNetPlayer.ActorNumber.CompareTo(b.OwningNetPlayer.ActorNumber);
		});
		if (this.promotionBot != null)
		{
			this.promotionBot.Refresh();
		}
		this.RefreshScoreboards();
		this.RefreshDepth();
		this.RefreshStore();
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null && this.vrRigs.Count > grplayer.maxNumberOfPlayersInShift)
		{
			grplayer.maxNumberOfPlayersInShift = this.vrRigs.Count;
		}
		this.isRefreshing = false;
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000D8928 File Offset: 0x000D6B28
	public void UpdateScoreboardScreen(GRUIScoreboard.ScoreboardScreen newScreen)
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			this.scoreboards[i].SwitchToScreen(newScreen);
		}
		this.RefreshScoreboards();
	}

	// Token: 0x060028A3 RID: 10403 RVA: 0x000D8964 File Offset: 0x000D6B64
	public void RefreshScoreboards()
	{
		for (int i = 0; i < this.scoreboards.Count; i++)
		{
			if (!(this.scoreboards[i] == null))
			{
				this.scoreboards[i].Refresh(this.vrRigs);
				if (this.shiftManager != null)
				{
					if (this.shiftManager.ShiftActive)
					{
						this.scoreboards[i].total.text = "-AWAITING SHIFT END-";
					}
					else if (this.shiftManager.ShiftTotalEarned < 0)
					{
						this.scoreboards[i].total.text = "-SHIFT NOT ACTIVE-";
					}
					else
					{
						this.scoreboards[i].total.text = this.shiftManager.ShiftTotalEarned.ToString();
					}
				}
			}
		}
	}

	// Token: 0x060028A4 RID: 10404 RVA: 0x000D8A48 File Offset: 0x000D6C48
	public int GetItemCost(int entityTypeId)
	{
		int result;
		if (!this.grManager.gameEntityManager.PriceLookup(entityTypeId, out result))
		{
			return 100;
		}
		return result;
	}

	// Token: 0x060028A5 RID: 10405 RVA: 0x000D8A70 File Offset: 0x000D6C70
	public void UpdateRemoteScoreboardScreen(GRUIScoreboard.ScoreboardScreen scoreboardPage)
	{
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
		if (managerForZone != null && managerForZone.ghostReactorManager != null)
		{
			managerForZone.ghostReactorManager.photonView.RPC("BroadcastScoreboardPage", 1, new object[]
			{
				scoreboardPage
			});
		}
	}

	// Token: 0x060028A6 RID: 10406 RVA: 0x000D8AC8 File Offset: 0x000D6CC8
	public void SetNextDelveDepth(int newLevel, int newDepthConfigIndex)
	{
		this.depthLevel = newLevel;
		this.depthLevel = Mathf.Clamp(this.depthLevel, 0, this.levelGenerator.depthConfigs.Count);
		if (this.depthLevel >= 0 && this.zone == GTZone.ghostReactorDrill && PhotonNetwork.InRoom && !NetworkSystem.Instance.SessionIsPrivate && this.grManager.IsAuthority())
		{
			int joinDepthSectionFromLevel = GhostReactor.GetJoinDepthSectionFromLevel(this.depthLevel);
			Hashtable hashtable = new Hashtable();
			hashtable.Add("ghostReactorDepth", joinDepthSectionFromLevel.ToString());
			Hashtable hashtable2 = hashtable;
			Debug.LogFormat("GR Room Param Set {0} {1}", new object[]
			{
				"ghostReactorDepth",
				hashtable2["ghostReactorDepth"]
			});
			PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable2, null, null);
		}
		this.depthConfigIndex = newDepthConfigIndex;
	}

	// Token: 0x060028A7 RID: 10407 RVA: 0x000D8B93 File Offset: 0x000D6D93
	public static int GetJoinDepthSectionFromLevel(int depthLevel)
	{
		if (depthLevel < 4)
		{
			return 0;
		}
		if (depthLevel < 10)
		{
			return 1;
		}
		if (depthLevel < 15)
		{
			return 2;
		}
		if (depthLevel < 20)
		{
			return 3;
		}
		if (depthLevel < 25)
		{
			return 5;
		}
		return 6;
	}

	// Token: 0x060028A8 RID: 10408 RVA: 0x000D8BB8 File Offset: 0x000D6DB8
	public void DelveToNextDepth()
	{
		if (this.shiftManager != null)
		{
			this.shiftManager.authorizedToDelveDeeper = false;
		}
		this.RefreshDepth();
	}

	// Token: 0x060028A9 RID: 10409 RVA: 0x000D8BDC File Offset: 0x000D6DDC
	public int PickLevelConfigForDepth(int depthLevel)
	{
		if (this.zone == GTZone.customMaps)
		{
			return 0;
		}
		GhostReactorLevelDepthConfig depthLevelConfig = this.GetDepthLevelConfig(depthLevel);
		int num = 0;
		for (int i = 0; i < depthLevelConfig.options.Count; i++)
		{
			num += depthLevelConfig.options[i].weight;
		}
		int num2 = Random.Range(0, num + 1);
		for (int j = 0; j < depthLevelConfig.options.Count; j++)
		{
			if (depthLevelConfig.options[j].weight >= num2)
			{
				return j;
			}
			num2 -= depthLevelConfig.options[j].weight;
		}
		return 0;
	}

	// Token: 0x060028AA RID: 10410 RVA: 0x000D8C7B File Offset: 0x000D6E7B
	public void RefreshDepth()
	{
		if (this.shiftManager != null)
		{
			this.shiftManager.RefreshDepthDisplay();
		}
		this.RefreshBays();
	}

	// Token: 0x060028AB RID: 10411 RVA: 0x000D8C9C File Offset: 0x000D6E9C
	public int GetDepthLevel()
	{
		return this.depthLevel;
	}

	// Token: 0x060028AC RID: 10412 RVA: 0x000D8CA4 File Offset: 0x000D6EA4
	public int GetDepthConfigIndex()
	{
		return this.depthConfigIndex;
	}

	// Token: 0x060028AD RID: 10413 RVA: 0x000D8CAC File Offset: 0x000D6EAC
	public GhostReactorLevelDepthConfig GetDepthLevelConfig(int level)
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		level = Mathf.Clamp(level, 0, this.levelGenerator.depthConfigs.Count - 1);
		return this.levelGenerator.depthConfigs[level];
	}

	// Token: 0x060028AE RID: 10414 RVA: 0x000D8CEC File Offset: 0x000D6EEC
	public GhostReactorLevelGenConfig GetCurrLevelGenConfig()
	{
		if (this.levelGenerator == null)
		{
			return null;
		}
		int num = this.GetDepthLevel();
		num = Mathf.Clamp(num, 0, this.levelGenerator.depthConfigs.Count - 1);
		this.depthConfigIndex = Mathf.Clamp(this.depthConfigIndex, 0, this.levelGenerator.depthConfigs[num].options.Count - 1);
		return this.levelGenerator.depthConfigs[num].options[this.depthConfigIndex].levelConfig;
	}

	// Token: 0x060028AF RID: 10415 RVA: 0x000D8D80 File Offset: 0x000D6F80
	public void RefreshStore()
	{
		for (int i = 0; i < this.storeDisplays.Count; i++)
		{
			this.storeDisplays[i].Setup(PhotonNetwork.LocalPlayer.ActorNumber, this);
		}
	}

	// Token: 0x060028B0 RID: 10416 RVA: 0x000D8DC0 File Offset: 0x000D6FC0
	public void RefreshBays()
	{
		for (int i = 0; i < this.bays.Count; i++)
		{
			this.bays[i].Refresh();
		}
	}

	// Token: 0x060028B1 RID: 10417 RVA: 0x000D8DF4 File Offset: 0x000D6FF4
	public void UpdateHandprints(float deltaTime)
	{
		int num = this.handPrintData.Count - 1000;
		if (num > 0)
		{
			this.handPrintData.RemoveRange(0, num);
			this.handPrintLocations.RemoveRange(0, num);
		}
		float time = Time.time;
		int i = this.handPrintData.Count - 1;
		while (i >= 0)
		{
			this.handPrintData[i] = this.handPrintData[i] - deltaTime;
			if (i + this.handPrintCombineTestDelta >= this.handPrintData.Count)
			{
				goto IL_13E;
			}
			if (this.handPrintData[i + this.handPrintCombineTestDelta] <= this.handPrintFadeTime - 3f)
			{
				Matrix4x4 matrix4x = this.handPrintLocations[i];
				Matrix4x4 matrix4x2 = this.handPrintLocations[i + this.handPrintCombineTestDelta];
				Vector3 vector;
				vector..ctor(matrix4x.m03 - matrix4x2.m03, matrix4x.m13 - matrix4x2.m13, matrix4x.m23 - matrix4x2.m23);
				if (vector.sqrMagnitude < this.handPrintScale * this.handPrintScale)
				{
					List<float> list = this.handPrintData;
					int num2 = i;
					list[num2] -= deltaTime * (float)this.handPrintData.Count * 50f;
					goto IL_13E;
				}
				goto IL_13E;
			}
			IL_169:
			i--;
			continue;
			IL_13E:
			if (this.handPrintData[i] < 0f)
			{
				this.handPrintData.RemoveAt(i);
				this.handPrintLocations.RemoveAt(i);
				goto IL_169;
			}
			goto IL_169;
		}
		if (this.handPrintData.Count > 0)
		{
			this.handPrintCombineTestDelta = (this.handPrintCombineTestDelta + 1) % this.handPrintData.Count;
			if (this.handPrintCombineTestDelta == 0)
			{
				this.handPrintCombineTestDelta = 1;
			}
		}
		else
		{
			this.handPrintCombineTestDelta = 1;
		}
		if (this.handPrintMaterial != null)
		{
			this.handPrintMaterial.SetFloat("_FadeDuration", this.handPrintFadeTime);
			this.handPrintMaterial.enableInstancing = true;
		}
		int num3 = Mathf.Min(Math.Min(1000, 1023), this.handPrintLocations.Count);
		if (num3 > 0)
		{
			this.handPrintMPB.Clear();
			this.handPrintMPB.SetFloatArray("_HandPrintData", this.handPrintData.GetRange(0, num3));
			this.handPrintMPB.SetFloat("_FadeDuration", this.handPrintFadeTime);
			RenderParams renderParams;
			renderParams..ctor(this.handPrintMaterial);
			renderParams.shadowCastingMode = 0;
			renderParams.receiveShadows = false;
			renderParams.layer = base.gameObject.layer;
			renderParams.matProps = this.handPrintMPB;
			renderParams.worldBounds = new Bounds(Vector3.zero, Vector3.one * 2000f);
			RenderParams renderParams2 = renderParams;
			Graphics.RenderMeshInstanced<Matrix4x4>(ref renderParams2, this.handPrintMesh, 0, this.handPrintLocations.GetRange(0, num3), -1, 0);
		}
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer != null)
		{
			if (Time.time - this.handPrintTimeLeft >= this.handPrintInkTime)
			{
				grplayer.SetGooParticleSystemEnabled(true, false);
			}
			if (Time.time - this.handPrintTimeRight >= this.handPrintInkTime)
			{
				grplayer.SetGooParticleSystemEnabled(false, false);
			}
		}
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x000D9114 File Offset: 0x000D7314
	public void OnTapLocal(bool isLeftHand, Vector3 pos, Quaternion orient, GorillaSurfaceOverride surfaceOverride)
	{
		GRPlayer grplayer = GRPlayer.Get(VRRig.LocalRig);
		if (grplayer == null)
		{
			return;
		}
		if (!(surfaceOverride != null) || surfaceOverride.overrideIndex != 79)
		{
			float num = isLeftHand ? this.handPrintTimeLeft : this.handPrintTimeRight;
			if (Time.time - num < this.handPrintInkTime && (Time.time < this.lastBroadcastHandTapTime || Time.time > this.lastBroadcastHandTapTime + this.broadcastHandTapDelay))
			{
				GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.zone);
				if (managerForZone != null && managerForZone.ghostReactorManager != null)
				{
					managerForZone.ghostReactorManager.photonView.RPC("BroadcastHandprint", 0, new object[]
					{
						pos,
						orient
					});
				}
				this.lastBroadcastHandTapTime = Time.time;
			}
			return;
		}
		grplayer.SetGooParticleSystemEnabled(isLeftHand, true);
		if (isLeftHand)
		{
			this.handPrintTimeLeft = Time.time;
			return;
		}
		this.handPrintTimeRight = Time.time;
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x000D920C File Offset: 0x000D740C
	public void AddHandprint(Vector3 pos, Quaternion orient)
	{
		Matrix4x4 matrix4x = default(Matrix4x4);
		matrix4x.SetTRS(pos, orient * Quaternion.Euler(90f, 0f, 180f), Vector3.one * this.handPrintScale);
		this.handPrintLocations.Add(matrix4x);
		this.handPrintData.Add(this.handPrintFadeTime);
	}

	// Token: 0x060028B4 RID: 10420 RVA: 0x000D9270 File Offset: 0x000D7470
	public void ClearAllHandprints()
	{
		this.handPrintData.Clear();
		this.handPrintLocations.Clear();
	}

	// Token: 0x170003EE RID: 1006
	// (get) Token: 0x060028B5 RID: 10421 RVA: 0x000D9288 File Offset: 0x000D7488
	public int NumActivePlayers
	{
		get
		{
			return this.vrRigs.Count;
		}
	}

	// Token: 0x060028B6 RID: 10422 RVA: 0x000D9298 File Offset: 0x000D7498
	public void OnAbilityDie(GameEntity entity)
	{
		GhostReactor.EnemyEntityCreateData enemyEntityCreateData = GhostReactor.EnemyEntityCreateData.Unpack(entity.createData);
		if (enemyEntityCreateData.respawnCount == 0)
		{
			return;
		}
		GhostReactor.EntityTypeRespawnTracker entityTypeRespawnTracker = new GhostReactor.EntityTypeRespawnTracker();
		entityTypeRespawnTracker.entityTypeID = entity.typeId;
		entityTypeRespawnTracker.entityCreateData = enemyEntityCreateData.Pack();
		entityTypeRespawnTracker.entityNextRespawnTime = this.respawnTime;
		this.respawnQueue.Add(entityTypeRespawnTracker);
	}

	// Token: 0x060028B7 RID: 10423 RVA: 0x000D92F1 File Offset: 0x000D74F1
	public void ClearAllRespawns()
	{
		this.respawnQueue.Clear();
	}

	// Token: 0x060028B8 RID: 10424 RVA: 0x00027DED File Offset: 0x00025FED
	bool IBuildValidation.BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x04003408 RID: 13320
	public static GhostReactor instance;

	// Token: 0x04003409 RID: 13321
	public GTZone zone;

	// Token: 0x0400340A RID: 13322
	public Transform restartMarker;

	// Token: 0x0400340B RID: 13323
	public PhotonView photonView;

	// Token: 0x0400340C RID: 13324
	public AudioSource entryRoomAudio;

	// Token: 0x0400340D RID: 13325
	public AudioClip entryRoomDeathSound;

	// Token: 0x0400340E RID: 13326
	public BoxCollider zoneLimit;

	// Token: 0x0400340F RID: 13327
	public BoxCollider safeZoneLimit;

	// Token: 0x04003410 RID: 13328
	public List<GhostReactor.TempEnemySpawnInfo> tempSpawnEnemies;

	// Token: 0x04003411 RID: 13329
	public GameEntity overrideEnemySpawn;

	// Token: 0x04003412 RID: 13330
	public List<GameEntity> tempSpawnItems;

	// Token: 0x04003413 RID: 13331
	public Transform tempSpawnItemsMarker;

	// Token: 0x04003414 RID: 13332
	public List<GRUIBuyItem> itemPurchaseStands;

	// Token: 0x04003415 RID: 13333
	public List<GRToolPurchaseStation> toolPurchasingStations;

	// Token: 0x04003416 RID: 13334
	public GRDebugUpgradeKiosk debugUpgradeKiosk;

	// Token: 0x04003417 RID: 13335
	public List<GRUIScoreboard> scoreboards;

	// Token: 0x04003418 RID: 13336
	public List<GRCollectibleDispenser> collectibleDispensers = new List<GRCollectibleDispenser>();

	// Token: 0x04003419 RID: 13337
	public List<IGRSleepableEntity> sleepableEntities = new List<IGRSleepableEntity>();

	// Token: 0x0400341A RID: 13338
	private List<GRBay> bays;

	// Token: 0x0400341B RID: 13339
	private List<GRUIStoreDisplay> storeDisplays;

	// Token: 0x0400341C RID: 13340
	public GRUIStationEmployeeBadges employeeBadges;

	// Token: 0x0400341D RID: 13341
	public GRUIEmployeeTerminal employeeTerminal;

	// Token: 0x0400341E RID: 13342
	public GhostReactorShiftManager shiftManager;

	// Token: 0x0400341F RID: 13343
	public GhostReactorLevelGenerator levelGenerator;

	// Token: 0x04003420 RID: 13344
	public GRCurrencyDepositor currencyDepositor;

	// Token: 0x04003421 RID: 13345
	public GRSeedExtractor seedExtractor;

	// Token: 0x04003422 RID: 13346
	public GRDistillery distillery;

	// Token: 0x04003423 RID: 13347
	public GRToolProgressionManager toolProgression;

	// Token: 0x04003424 RID: 13348
	public GRToolUpgradeStation upgradeStation;

	// Token: 0x04003425 RID: 13349
	public List<GRToolUpgradePurchaseStationFull> toolUpgradePurchaseStationsFull;

	// Token: 0x04003426 RID: 13350
	public GRRecycler recycler;

	// Token: 0x04003427 RID: 13351
	public List<GhostReactor.EntityTypeRespawnTracker> respawnQueue = new List<GhostReactor.EntityTypeRespawnTracker>();

	// Token: 0x04003428 RID: 13352
	public List<float> difficultyScalingPerPlayer = new List<float>(10);

	// Token: 0x04003429 RID: 13353
	public float respawnTime = 10f;

	// Token: 0x0400342A RID: 13354
	public float respawnMinDistToPlayer = 8f;

	// Token: 0x0400342B RID: 13355
	public float difficultyScalingForCurrentFloor = 1f;

	// Token: 0x0400342C RID: 13356
	public LayerMask envLayerMask;

	// Token: 0x0400342D RID: 13357
	public Material handPrintMaterial;

	// Token: 0x0400342E RID: 13358
	public Mesh handPrintMesh;

	// Token: 0x0400342F RID: 13359
	public float handPrintScale;

	// Token: 0x04003430 RID: 13360
	public float handPrintInkTime = 30f;

	// Token: 0x04003431 RID: 13361
	public float handPrintFadeTime = 600f;

	// Token: 0x04003432 RID: 13362
	private const int handPrintMaxCount = 1000;

	// Token: 0x04003433 RID: 13363
	private List<Matrix4x4> handPrintLocations = new List<Matrix4x4>(1000);

	// Token: 0x04003434 RID: 13364
	private List<float> handPrintData = new List<float>(1000);

	// Token: 0x04003435 RID: 13365
	private MaterialPropertyBlock handPrintMPB;

	// Token: 0x04003436 RID: 13366
	[ReadOnly]
	public List<GRReviveStation> reviveStations;

	// Token: 0x04003437 RID: 13367
	public List<GRVendingMachine> vendingMachines;

	// Token: 0x04003438 RID: 13368
	public List<VRRig> vrRigs;

	// Token: 0x04003439 RID: 13369
	private float collectibleDispenserUpdateFrequency = 3f;

	// Token: 0x0400343A RID: 13370
	private double lastCollectibleDispenserUpdateTime = -10.0;

	// Token: 0x0400343B RID: 13371
	private int sentientCoreUpdateIndex;

	// Token: 0x0400343C RID: 13372
	private SRand randomGenerator;

	// Token: 0x0400343D RID: 13373
	[ReadOnly]
	public int depthLevel;

	// Token: 0x0400343E RID: 13374
	[ReadOnly]
	public int depthConfigIndex;

	// Token: 0x0400343F RID: 13375
	public Dictionary<int, double> playerProgressionData;

	// Token: 0x04003440 RID: 13376
	public GRDropZone dropZone;

	// Token: 0x04003441 RID: 13377
	public static float DROP_ZONE_REPEL = 2.25f;

	// Token: 0x04003442 RID: 13378
	public ZoneShaderSettings zoneShaderSettings;

	// Token: 0x04003443 RID: 13379
	public GRUIPromotionBot promotionBot;

	// Token: 0x04003444 RID: 13380
	private bool isRefreshing;

	// Token: 0x04003445 RID: 13381
	public GhostReactorManager grManager;

	// Token: 0x04003446 RID: 13382
	private float handPrintTimeLeft = -1000f;

	// Token: 0x04003447 RID: 13383
	private float handPrintTimeRight = -1000f;

	// Token: 0x04003448 RID: 13384
	private int handPrintCombineTestDelta = 1;

	// Token: 0x04003449 RID: 13385
	private float lastBroadcastHandTapTime;

	// Token: 0x0400344A RID: 13386
	private float broadcastHandTapDelay = 0.3f;

	// Token: 0x02000639 RID: 1593
	[Serializable]
	public class TempEnemySpawnInfo
	{
		// Token: 0x0400344B RID: 13387
		public GameEntity prefab;

		// Token: 0x0400344C RID: 13388
		public Transform spawnMarker;

		// Token: 0x0400344D RID: 13389
		public int patrolPath;
	}

	// Token: 0x0200063A RID: 1594
	public class EntityTypeRespawnTracker
	{
		// Token: 0x0400344E RID: 13390
		public int entityTypeID;

		// Token: 0x0400344F RID: 13391
		public long entityCreateData;

		// Token: 0x04003450 RID: 13392
		public float entityNextRespawnTime;
	}

	// Token: 0x0200063B RID: 1595
	public enum EntityGroupTypes
	{
		// Token: 0x04003452 RID: 13394
		EnemyChaser,
		// Token: 0x04003453 RID: 13395
		EnemyChaserArmored,
		// Token: 0x04003454 RID: 13396
		EnemyRanged,
		// Token: 0x04003455 RID: 13397
		EnemyRangedArmored,
		// Token: 0x04003456 RID: 13398
		CollectibleFlower,
		// Token: 0x04003457 RID: 13399
		BarrierEnergyCostGate,
		// Token: 0x04003458 RID: 13400
		BarrierSpectralWall,
		// Token: 0x04003459 RID: 13401
		HazardSpectralLiquid
	}

	// Token: 0x0200063C RID: 1596
	public enum EnemyType
	{
		// Token: 0x0400345B RID: 13403
		Chaser,
		// Token: 0x0400345C RID: 13404
		Ranged,
		// Token: 0x0400345D RID: 13405
		Phantom,
		// Token: 0x0400345E RID: 13406
		Environment,
		// Token: 0x0400345F RID: 13407
		CustomMapsEnemy
	}

	// Token: 0x0200063D RID: 1597
	public struct EnemyEntityCreateData
	{
		// Token: 0x060028BD RID: 10429 RVA: 0x000D93E6 File Offset: 0x000D75E6
		private static long PackData(int value, int nbits, int shift)
		{
			return ((long)value & (long)((1 << nbits) - 1)) << shift;
		}

		// Token: 0x060028BE RID: 10430 RVA: 0x000D93F9 File Offset: 0x000D75F9
		private static int UnpackData(long createData, int nbits, int shift)
		{
			return (int)(createData >> shift & (long)((1 << nbits) - 1));
		}

		// Token: 0x060028BF RID: 10431 RVA: 0x000D940C File Offset: 0x000D760C
		public static GhostReactor.EnemyEntityCreateData Unpack(long bits)
		{
			return new GhostReactor.EnemyEntityCreateData
			{
				respawnCount = GhostReactor.EnemyEntityCreateData.UnpackData(bits, 8, 16),
				sectionIndex = GhostReactor.EnemyEntityCreateData.UnpackData(bits, 8, 8),
				patrolIndex = GhostReactor.EnemyEntityCreateData.UnpackData(bits, 8, 0)
			};
		}

		// Token: 0x060028C0 RID: 10432 RVA: 0x000D9450 File Offset: 0x000D7650
		public long Pack()
		{
			return GhostReactor.EnemyEntityCreateData.PackData(this.respawnCount, 8, 16) | GhostReactor.EnemyEntityCreateData.PackData(this.sectionIndex, 8, 8) | GhostReactor.EnemyEntityCreateData.PackData(this.patrolIndex, 8, 0);
		}

		// Token: 0x04003460 RID: 13408
		public int respawnCount;

		// Token: 0x04003461 RID: 13409
		public int sectionIndex;

		// Token: 0x04003462 RID: 13410
		public int patrolIndex;
	}

	// Token: 0x0200063E RID: 1598
	public struct ToolEntityCreateData
	{
		// Token: 0x060028C1 RID: 10433 RVA: 0x000D93E6 File Offset: 0x000D75E6
		private static long PackData(int value, int nbits, int shift)
		{
			return ((long)value & (long)((1 << nbits) - 1)) << shift;
		}

		// Token: 0x060028C2 RID: 10434 RVA: 0x000D93F9 File Offset: 0x000D75F9
		private static int UnpackData(long createData, int nbits, int shift)
		{
			return (int)(createData >> shift & (long)((1 << nbits) - 1));
		}

		// Token: 0x060028C3 RID: 10435 RVA: 0x000D947C File Offset: 0x000D767C
		public static GhostReactor.ToolEntityCreateData Unpack(long bits)
		{
			GhostReactor.ToolEntityCreateData result = default(GhostReactor.ToolEntityCreateData);
			result.stationIndex = GhostReactor.ToolEntityCreateData.UnpackData(bits, 8, 0) - 1;
			int num = GhostReactor.ToolEntityCreateData.UnpackData(bits, 8, 8);
			result.decayTime = 5f * (float)num;
			return result;
		}

		// Token: 0x060028C4 RID: 10436 RVA: 0x000D94BB File Offset: 0x000D76BB
		public long Pack()
		{
			long result = GhostReactor.ToolEntityCreateData.PackData(this.stationIndex + 1, 8, 0);
			GhostReactor.ToolEntityCreateData.PackData((int)(this.decayTime / 5f), 8, 8);
			return result;
		}

		// Token: 0x04003463 RID: 13411
		public int stationIndex;

		// Token: 0x04003464 RID: 13412
		public float decayTime;
	}
}
