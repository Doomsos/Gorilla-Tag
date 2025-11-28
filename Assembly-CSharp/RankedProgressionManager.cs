using System;
using System.Collections;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000846 RID: 2118
public class RankedProgressionManager : MonoBehaviour
{
	// Token: 0x17000501 RID: 1281
	// (get) Token: 0x060037B4 RID: 14260 RVA: 0x0012B946 File Offset: 0x00129B46
	// (set) Token: 0x060037B5 RID: 14261 RVA: 0x0012B94E File Offset: 0x00129B4E
	public int MaxRank { get; private set; }

	// Token: 0x17000502 RID: 1282
	// (get) Token: 0x060037B6 RID: 14262 RVA: 0x0012B957 File Offset: 0x00129B57
	// (set) Token: 0x060037B7 RID: 14263 RVA: 0x0012B95F File Offset: 0x00129B5F
	public float LowTierThreshold { get; set; }

	// Token: 0x17000503 RID: 1283
	// (get) Token: 0x060037B8 RID: 14264 RVA: 0x0012B968 File Offset: 0x00129B68
	// (set) Token: 0x060037B9 RID: 14265 RVA: 0x0012B970 File Offset: 0x00129B70
	public float HighTierThreshold { get; set; }

	// Token: 0x17000504 RID: 1284
	// (get) Token: 0x060037BA RID: 14266 RVA: 0x0012B979 File Offset: 0x00129B79
	// (set) Token: 0x060037BB RID: 14267 RVA: 0x00002789 File Offset: 0x00000989
	public List<RankedProgressionManager.RankedProgressionTier> MajorTiers
	{
		get
		{
			return this.majorTiers;
		}
		private set
		{
		}
	}

	// Token: 0x060037BC RID: 14268 RVA: 0x00002789 File Offset: 0x00000989
	private void DebugSetELO()
	{
	}

	// Token: 0x060037BD RID: 14269 RVA: 0x00002789 File Offset: 0x00000989
	[ContextMenu("Reset ELO")]
	private void DebugResetELO()
	{
	}

	// Token: 0x060037BE RID: 14270 RVA: 0x0012B981 File Offset: 0x00129B81
	private void Awake()
	{
		if (RankedProgressionManager.Instance)
		{
			GTDev.LogError<string>("Duplicate RankedProgressionManager detected. Destroying self.", base.gameObject, null);
			Object.Destroy(this);
			return;
		}
		RankedProgressionManager.Instance = this;
	}

	// Token: 0x060037BF RID: 14271 RVA: 0x0012B9B0 File Offset: 0x00129BB0
	private void Start()
	{
		if (this.majorTiers.Count < 3)
		{
			GTDev.LogWarning<string>("At least 3 MMR tiers must be defined.", null);
			return;
		}
		GameMode.OnStartGameMode += this.OnJoinedRoom;
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoined);
		float minThreshold = 100f;
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			this.majorTiers[i].SetMinThreshold((i == 0) ? 100f : this.majorTiers[i - 1].thresholdMax);
			for (int j = 0; j < this.majorTiers[i].subTiers.Count; j++)
			{
				num++;
				this.majorTiers[i].subTiers[j].SetMinThreshold(minThreshold);
				minThreshold = this.majorTiers[i].subTiers[j].thresholdMax;
			}
		}
		this.MaxRank = num - 1;
		this.LowTierThreshold = this.majorTiers[0].thresholdMax;
		List<RankedProgressionManager.RankedProgressionTier> list = this.majorTiers;
		this.HighTierThreshold = list[list.Count - 1].GetMinThreshold();
		this.EloScorePC = new RankedMultiplayerStatisticFloat(RankedProgressionManager.RANKED_ELO_PC_KEY, 100f, 100f, 4000f, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.EloScoreQuest = new RankedMultiplayerStatisticFloat(RankedProgressionManager.RANKED_ELO_KEY, 100f, 100f, 4000f, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.NewTierGracePeriodIdxPC = new RankedMultiplayerStatisticInt(RankedProgressionManager.RANKED_PROGRESSION_GRACE_PERIOD_KEY, 0, -1, int.MaxValue, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
		this.NewTierGracePeriodIdxQuest = new RankedMultiplayerStatisticInt(RankedProgressionManager.RANKED_PROGRESSION_GRACE_PERIOD_PC_KEY, 0, -1, int.MaxValue, RankedMultiplayerStatistic.SerializationType.PlayerPrefs);
	}

	// Token: 0x060037C0 RID: 14272 RVA: 0x0012BB62 File Offset: 0x00129D62
	private void OnDestroy()
	{
		GameMode.OnStartGameMode += this.OnJoinedRoom;
		RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoined);
	}

	// Token: 0x060037C1 RID: 14273 RVA: 0x0012BB90 File Offset: 0x00129D90
	public void RequestUnlockCompetitiveQueue(bool unlock)
	{
		GorillaTagCompetitiveServerApi.Instance.RequestUnlockCompetitiveQueue(unlock, delegate
		{
			this.AcquireLocalPlayerRankInformation();
		});
	}

	// Token: 0x060037C2 RID: 14274 RVA: 0x0012BBA9 File Offset: 0x00129DA9
	public IEnumerator LoadStatsWhenReady()
	{
		yield return new WaitUntil(() => NetworkSystem.Instance.LocalPlayer.UserId != null);
		if (this.HasUnlockedCompetitiveQueue())
		{
			this.RequestUnlockCompetitiveQueue(true);
		}
		else
		{
			this.AcquireLocalPlayerRankInformation();
		}
		yield break;
	}

	// Token: 0x060037C3 RID: 14275 RVA: 0x0012BBB8 File Offset: 0x00129DB8
	private void OnJoinedRoom(GameModeType newGameModeType)
	{
		if (newGameModeType == GameModeType.InfectionCompetitive)
		{
			this.AcquireRoomRankInformation(false);
		}
	}

	// Token: 0x060037C4 RID: 14276 RVA: 0x0012BBC6 File Offset: 0x00129DC6
	private void OnPlayerJoined(NetPlayer player)
	{
		if (GorillaGameManager.instance != null && GorillaGameManager.instance.GameType() == GameModeType.InfectionCompetitive)
		{
			this.AcquireSinglePlayerRankInformation(player);
		}
	}

	// Token: 0x060037C5 RID: 14277 RVA: 0x0012BBEC File Offset: 0x00129DEC
	private void AcquireLocalPlayerRankInformation()
	{
		List<string> list = new List<string>();
		list.Add(NetworkSystem.Instance.LocalPlayer.UserId);
		GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnLocalPlayerRankedInformationAcquired));
	}

	// Token: 0x060037C6 RID: 14278 RVA: 0x0012BC2C File Offset: 0x00129E2C
	private void AcquireSinglePlayerRankInformation(NetPlayer player)
	{
		if (player == null)
		{
			return;
		}
		List<string> list = new List<string>();
		list.Add(player.UserId);
		GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnPlayersRankedInformationAcquired));
	}

	// Token: 0x060037C7 RID: 14279 RVA: 0x0012BC68 File Offset: 0x00129E68
	public void AcquireRoomRankInformation(bool includeLocalPlayer = true)
	{
		List<string> list = new List<string>();
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (includeLocalPlayer || !netPlayer.IsLocal)
			{
				list.Add(netPlayer.UserId);
			}
		}
		if (list.Count > 0)
		{
			GorillaTagCompetitiveServerApi.Instance.RequestGetRankInformation(list, new Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(this.OnPlayersRankedInformationAcquired));
		}
	}

	// Token: 0x060037C8 RID: 14280 RVA: 0x0012BCF0 File Offset: 0x00129EF0
	private void OnPlayersRankedInformationAcquired(GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData)
	{
		foreach (GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData rankedModePlayerProgressionData in rankedModeProgressionData.playerData)
		{
			if (rankedModePlayerProgressionData != null && rankedModePlayerProgressionData.platformData != null && rankedModePlayerProgressionData.platformData.Length >= 2)
			{
				int num = -1;
				foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
				{
					if (netPlayer.UserId == rankedModePlayerProgressionData.playfabID)
					{
						num = netPlayer.ActorNumber;
						break;
					}
				}
				if (num >= 0)
				{
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = rankedModePlayerProgressionData.platformData[1];
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData2 = rankedModePlayerProgressionData.platformData[0];
					GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData3 = rankedModeProgressionPlatformData2;
					int rankFromTiers = RankedProgressionManager.Instance.GetRankFromTiers(rankedModeProgressionPlatformData3.majorTier, rankedModeProgressionPlatformData3.minorTier);
					Action<int, float, int> onPlayerEloAcquired = this.OnPlayerEloAcquired;
					if (onPlayerEloAcquired != null)
					{
						onPlayerEloAcquired.Invoke(num, rankedModeProgressionPlatformData3.elo, rankFromTiers);
					}
					if (num == NetworkSystem.Instance.LocalPlayerID)
					{
						this.SetLocalProgressionData(rankedModePlayerProgressionData);
					}
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(num, out rigContainer))
					{
						VRRig rig = rigContainer.Rig;
						if (rig != null)
						{
							int rankFromTiers2 = this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
							int rankFromTiers3 = RankedProgressionManager.Instance.GetRankFromTiers(rankedModeProgressionPlatformData2.majorTier, rankedModeProgressionPlatformData2.minorTier);
							rig.SetRankedInfo(rankedModeProgressionPlatformData3.elo, rankFromTiers2, rankFromTiers3, false);
						}
					}
				}
			}
		}
	}

	// Token: 0x060037C9 RID: 14281 RVA: 0x0012BE80 File Offset: 0x0012A080
	private void OnLocalPlayerRankedInformationAcquired(GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData)
	{
		if (rankedModeProgressionData.playerData.Count > 0)
		{
			this.SetLocalProgressionData(rankedModeProgressionData.playerData[0]);
			float eloScore = this.GetEloScore();
			int progressionRankIndexQuest = this.GetProgressionRankIndexQuest();
			int progressionRankIndexPC = this.GetProgressionRankIndexPC();
			int tier = progressionRankIndexPC;
			this.HandlePlayerRankedInfoReceived(NetworkSystem.Instance.LocalPlayer.ActorNumber, eloScore, tier);
			VRRig.LocalRig.SetRankedInfo(eloScore, progressionRankIndexQuest, progressionRankIndexPC, true);
		}
	}

	// Token: 0x060037CA RID: 14282 RVA: 0x0012BEEB File Offset: 0x0012A0EB
	public bool AreValuesValid(float elo, int questTier, int pcTier)
	{
		return elo >= 100f && elo <= 4000f && questTier >= 0 && questTier <= this.MaxRank && pcTier >= 0 && pcTier <= this.MaxRank;
	}

	// Token: 0x060037CB RID: 14283 RVA: 0x0012BF1A File Offset: 0x0012A11A
	public void HandlePlayerRankedInfoReceived(int actorNum, float elo, int tier)
	{
		Action<int, float, int> onPlayerEloAcquired = this.OnPlayerEloAcquired;
		if (onPlayerEloAcquired == null)
		{
			return;
		}
		onPlayerEloAcquired.Invoke(actorNum, elo, tier);
	}

	// Token: 0x060037CC RID: 14284 RVA: 0x0012BF2F File Offset: 0x0012A12F
	public void SetLocalProgressionData(GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData data)
	{
		this.ProgressionData = data;
	}

	// Token: 0x060037CD RID: 14285 RVA: 0x0012BF38 File Offset: 0x0012A138
	public void LoadStats()
	{
		base.StartCoroutine(this.LoadStatsWhenReady());
	}

	// Token: 0x060037CE RID: 14286 RVA: 0x0012BF47 File Offset: 0x0012A147
	public float GetEloScore()
	{
		return this.GetEloScorePC();
	}

	// Token: 0x060037CF RID: 14287 RVA: 0x0012BF4F File Offset: 0x0012A14F
	public void SetEloScore(float val)
	{
		GorillaTagCompetitiveServerApi.Instance.RequestSetEloValue(val, delegate
		{
			this.AcquireLocalPlayerRankInformation();
		});
	}

	// Token: 0x060037D0 RID: 14288 RVA: 0x0012BF68 File Offset: 0x0012A168
	public float GetEloScorePC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 100f;
		}
		return this.ProgressionData.platformData[0].elo;
	}

	// Token: 0x060037D1 RID: 14289 RVA: 0x0012BFA7 File Offset: 0x0012A1A7
	public float GetEloScoreQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 100f;
		}
		return this.ProgressionData.platformData[1].elo;
	}

	// Token: 0x060037D2 RID: 14290 RVA: 0x0012BFE6 File Offset: 0x0012A1E6
	private int GetNewTierGracePeriodIdx()
	{
		return this.NewTierGracePeriodIdxPC;
	}

	// Token: 0x060037D3 RID: 14291 RVA: 0x0012BFF3 File Offset: 0x0012A1F3
	private void SetNewTierGracePeriodIdx(int val)
	{
		this.NewTierGracePeriodIdxPC.Set(val);
	}

	// Token: 0x060037D4 RID: 14292 RVA: 0x0012C001 File Offset: 0x0012A201
	private void IncrementNewTierGracePeriodIdx()
	{
		this.NewTierGracePeriodIdxPC.Increment();
	}

	// Token: 0x060037D5 RID: 14293 RVA: 0x0012C00E File Offset: 0x0012A20E
	public bool TryGetProgressionSubTier(out RankedProgressionManager.RankedProgressionSubTier subTier, out int index)
	{
		subTier = null;
		index = -1;
		return this.TryGetProgressionSubTier(this.GetEloScore(), out subTier, out index);
	}

	// Token: 0x060037D6 RID: 14294 RVA: 0x0012C024 File Offset: 0x0012A224
	public bool TryGetProgressionSubTier(float elo, out RankedProgressionManager.RankedProgressionSubTier subTier, out int index)
	{
		int num = 0;
		subTier = null;
		index = -1;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			float num2 = (i < this.majorTiers.Count - 1) ? this.majorTiers[i].thresholdMax : 4000.1f;
			if (elo < num2)
			{
				int j = 0;
				while (j < this.majorTiers[i].subTiers.Count)
				{
					float num3 = (j < this.majorTiers[i].subTiers.Count - 1) ? this.majorTiers[i].subTiers[j].thresholdMax : num2;
					if (elo < num3)
					{
						subTier = this.majorTiers[i].subTiers[j];
						index = num;
						return true;
					}
					j++;
					num++;
				}
			}
			else
			{
				num += this.majorTiers[i].subTiers.Count;
			}
		}
		return false;
	}

	// Token: 0x060037D7 RID: 14295 RVA: 0x0012C128 File Offset: 0x0012A328
	private RankedProgressionManager.RankedProgressionTier GetProgressionMajorTierBySubTierIndex(int idx)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == idx)
				{
					return this.majorTiers[i];
				}
				j++;
				num++;
			}
		}
		return null;
	}

	// Token: 0x060037D8 RID: 14296 RVA: 0x0012C184 File Offset: 0x0012A384
	private RankedProgressionManager.RankedProgressionSubTier GetProgressionSubTierByIndex(int idx)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == idx)
				{
					return this.majorTiers[i].subTiers[j];
				}
				j++;
				num++;
			}
		}
		return null;
	}

	// Token: 0x060037D9 RID: 14297 RVA: 0x0012C1EC File Offset: 0x0012A3EC
	private RankedProgressionManager.RankedProgressionSubTier GetNextProgressionSubTierByIndex(int idx)
	{
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(idx + 1);
		if (progressionSubTierByIndex != null)
		{
			return progressionSubTierByIndex;
		}
		return this.GetProgressionSubTierByIndex(idx);
	}

	// Token: 0x060037DA RID: 14298 RVA: 0x0012C210 File Offset: 0x0012A410
	private RankedProgressionManager.RankedProgressionSubTier GetPrevProgressionSubTierByIndex(int idx)
	{
		if (idx > 0)
		{
			RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(idx - 1);
			if (progressionSubTierByIndex != null)
			{
				return progressionSubTierByIndex;
			}
		}
		return this.GetProgressionSubTierByIndex(idx);
	}

	// Token: 0x060037DB RID: 14299 RVA: 0x0012C237 File Offset: 0x0012A437
	public string GetProgressionRankName()
	{
		return this.GetProgressionRankName(this.GetEloScore());
	}

	// Token: 0x060037DC RID: 14300 RVA: 0x0012C248 File Offset: 0x0012A448
	public string GetProgressionRankName(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int num;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out num))
		{
			return rankedProgressionSubTier.name;
		}
		return string.Empty;
	}

	// Token: 0x060037DD RID: 14301 RVA: 0x0012C270 File Offset: 0x0012A470
	public string GetNextProgressionRankName(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier nextProgressionSubTierByIndex = this.GetNextProgressionSubTierByIndex(subTierIdx);
		if (nextProgressionSubTierByIndex != null)
		{
			return nextProgressionSubTierByIndex.name;
		}
		return null;
	}

	// Token: 0x060037DE RID: 14302 RVA: 0x0012C290 File Offset: 0x0012A490
	public string GetPrevProgressionRankName(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier prevProgressionSubTierByIndex = this.GetPrevProgressionSubTierByIndex(subTierIdx);
		if (prevProgressionSubTierByIndex != null)
		{
			return prevProgressionSubTierByIndex.name;
		}
		return null;
	}

	// Token: 0x060037DF RID: 14303 RVA: 0x0012C2B0 File Offset: 0x0012A4B0
	public int GetProgressionRankIndex()
	{
		return this.GetProgressionRankIndexPC();
	}

	// Token: 0x060037E0 RID: 14304 RVA: 0x0012C2B8 File Offset: 0x0012A4B8
	public RankedProgressionManager.RankedProgressionSubTier GetProgressionSubTier()
	{
		return this.GetProgressionSubTierByIndex(this.GetProgressionRankIndex());
	}

	// Token: 0x060037E1 RID: 14305 RVA: 0x0012C2C8 File Offset: 0x0012A4C8
	public int GetProgressionRankIndexQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0;
		}
		GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = this.ProgressionData.platformData[1];
		return this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
	}

	// Token: 0x060037E2 RID: 14306 RVA: 0x0012C31C File Offset: 0x0012A51C
	public int GetProgressionRankIndexPC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0;
		}
		GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData rankedModeProgressionPlatformData = this.ProgressionData.platformData[0];
		return this.GetRankFromTiers(rankedModeProgressionPlatformData.majorTier, rankedModeProgressionPlatformData.minorTier);
	}

	// Token: 0x060037E3 RID: 14307 RVA: 0x0012C370 File Offset: 0x0012A570
	public int GetRankFromTiers(int majorTier, int minorTier)
	{
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			for (int j = 0; j < this.majorTiers[i].subTiers.Count; j++)
			{
				if (i == majorTier && j == minorTier)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x060037E4 RID: 14308 RVA: 0x0012C3C8 File Offset: 0x0012A5C8
	public int GetProgressionRankIndex(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int result;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out result))
		{
			return result;
		}
		return -1;
	}

	// Token: 0x060037E5 RID: 14309 RVA: 0x0012C3E5 File Offset: 0x0012A5E5
	public float GetProgressionRankProgress()
	{
		return this.GetProgressionRankProgressPC();
	}

	// Token: 0x060037E6 RID: 14310 RVA: 0x0012C3ED File Offset: 0x0012A5ED
	public float GetProgressionRankProgressQuest()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0f;
		}
		return this.ProgressionData.platformData[1].rankProgress;
	}

	// Token: 0x060037E7 RID: 14311 RVA: 0x0012C42C File Offset: 0x0012A62C
	public float GetProgressionRankProgressPC()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return 0f;
		}
		return this.ProgressionData.platformData[0].rankProgress;
	}

	// Token: 0x060037E8 RID: 14312 RVA: 0x0012C46C File Offset: 0x0012A66C
	public int ClampProgressionRankIndex(int subTierIdx)
	{
		if (subTierIdx < 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < this.majorTiers.Count; i++)
		{
			int j = 0;
			while (j < this.majorTiers[i].subTiers.Count)
			{
				if (num == subTierIdx)
				{
					return subTierIdx;
				}
				j++;
				num++;
			}
		}
		return num - 1;
	}

	// Token: 0x060037E9 RID: 14313 RVA: 0x0012C4C8 File Offset: 0x0012A6C8
	public Sprite GetProgressionRankIcon()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return null;
		}
		int num = (this.ProgressionData == null) ? 0 : this.ProgressionData.platformData[0].minorTier;
		int num2 = (this.ProgressionData == null) ? 0 : this.ProgressionData.platformData[0].majorTier;
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = this.majorTiers[num2].subTiers[num];
		if (rankedProgressionSubTier == null)
		{
			return null;
		}
		return rankedProgressionSubTier.icon;
	}

	// Token: 0x060037EA RID: 14314 RVA: 0x0012C560 File Offset: 0x0012A760
	public string GetRankedProgressionTierName()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return "None";
		}
		int minorTier = this.ProgressionData.platformData[0].minorTier;
		int majorTier = this.ProgressionData.platformData[0].majorTier;
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = this.majorTiers[majorTier].subTiers[minorTier];
		if (rankedProgressionSubTier != null)
		{
			return rankedProgressionSubTier.name;
		}
		return "None";
	}

	// Token: 0x060037EB RID: 14315 RVA: 0x0012C5EC File Offset: 0x0012A7EC
	public Sprite GetProgressionRankIcon(float elo)
	{
		RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier;
		int num;
		if (this.TryGetProgressionSubTier(elo, out rankedProgressionSubTier, out num))
		{
			return rankedProgressionSubTier.icon;
		}
		return null;
	}

	// Token: 0x060037EC RID: 14316 RVA: 0x0012C610 File Offset: 0x0012A810
	public Sprite GetProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(subTierIdx);
		if (progressionSubTierByIndex != null)
		{
			return progressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x060037ED RID: 14317 RVA: 0x0012C630 File Offset: 0x0012A830
	public Sprite GetNextProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier nextProgressionSubTierByIndex = this.GetNextProgressionSubTierByIndex(subTierIdx);
		if (nextProgressionSubTierByIndex != null)
		{
			return nextProgressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x0012C650 File Offset: 0x0012A850
	public Sprite GetPrevProgressionRankIcon(int subTierIdx)
	{
		RankedProgressionManager.RankedProgressionSubTier prevProgressionSubTierByIndex = this.GetPrevProgressionSubTierByIndex(subTierIdx);
		if (prevProgressionSubTierByIndex != null)
		{
			return prevProgressionSubTierByIndex.icon;
		}
		return null;
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x0012C670 File Offset: 0x0012A870
	public float GetCurrentELO()
	{
		return this.GetEloScore();
	}

	// Token: 0x060037F0 RID: 14320 RVA: 0x0012C678 File Offset: 0x0012A878
	public void GetSubtierRankThresholds(int subTierIdx, out float minThreshold, out float maxThreshold)
	{
		minThreshold = 0f;
		maxThreshold = 1f;
		RankedProgressionManager.RankedProgressionSubTier progressionSubTierByIndex = this.GetProgressionSubTierByIndex(subTierIdx);
		if (progressionSubTierByIndex != null)
		{
			maxThreshold = progressionSubTierByIndex.thresholdMax;
			if (maxThreshold <= 0f)
			{
				RankedProgressionManager.RankedProgressionTier progressionMajorTierBySubTierIndex = this.GetProgressionMajorTierBySubTierIndex(subTierIdx);
				if (progressionMajorTierBySubTierIndex != null)
				{
					maxThreshold = progressionMajorTierBySubTierIndex.thresholdMax;
					if (maxThreshold <= 0f)
					{
						maxThreshold = 4000f;
					}
				}
			}
			minThreshold = progressionSubTierByIndex.GetMinThreshold();
			if (minThreshold <= 0f)
			{
				RankedProgressionManager.RankedProgressionTier progressionMajorTierBySubTierIndex2 = this.GetProgressionMajorTierBySubTierIndex(subTierIdx);
				if (progressionMajorTierBySubTierIndex2 != null)
				{
					minThreshold = progressionMajorTierBySubTierIndex2.GetMinThreshold();
					if (minThreshold <= 0f)
					{
						minThreshold = 100f;
					}
				}
			}
		}
	}

	// Token: 0x060037F1 RID: 14321 RVA: 0x0012C706 File Offset: 0x0012A906
	public static float GetEloWinProbability(float ratingPlayer1, float ratingPlayer2)
	{
		return 1f / (1f + Mathf.Pow(10f, (ratingPlayer1 - ratingPlayer2) / 400f));
	}

	// Token: 0x060037F2 RID: 14322 RVA: 0x0012C727 File Offset: 0x0012A927
	public static float UpdateEloScore(float eloScore, float expectedResult, float actualResult, float k)
	{
		return Mathf.Clamp(eloScore + k * (actualResult - expectedResult), 100f, 4000f);
	}

	// Token: 0x060037F3 RID: 14323 RVA: 0x0012C73F File Offset: 0x0012A93F
	public RankedProgressionManager.ERankedMatchmakingTier GetRankedMatchmakingTier()
	{
		if (this.ProgressionData == null || this.ProgressionData.platformData == null || this.ProgressionData.platformData.Length < 2)
		{
			return RankedProgressionManager.ERankedMatchmakingTier.Low;
		}
		return (RankedProgressionManager.ERankedMatchmakingTier)this.ProgressionData.platformData[0].majorTier;
	}

	// Token: 0x17000505 RID: 1285
	// (get) Token: 0x060037F4 RID: 14324 RVA: 0x0012C77A File Offset: 0x0012A97A
	public float CompetitiveQueueEloFloor
	{
		get
		{
			return this.LowTierThreshold;
		}
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x0012C782 File Offset: 0x0012A982
	private bool HasUnlockedCompetitiveQueue()
	{
		return GorillaComputer.instance.allowedInCompetitive;
	}

	// Token: 0x040046FE RID: 18174
	public static RankedProgressionManager Instance;

	// Token: 0x040046FF RID: 18175
	public const float DEFAULT_ELO = 100f;

	// Token: 0x04004700 RID: 18176
	public const float MIN_ELO = 100f;

	// Token: 0x04004701 RID: 18177
	public const float MAX_ELO = 4000f;

	// Token: 0x04004702 RID: 18178
	public const float MAJOR_TIER_MIN_RANGE = 200f;

	// Token: 0x04004703 RID: 18179
	public const float SUB_TIER_MIN_RANGE = 20f;

	// Token: 0x04004704 RID: 18180
	public static string RANKED_ELO_KEY = "RankedElo";

	// Token: 0x04004705 RID: 18181
	public static string RANKED_PROGRESSION_GRACE_PERIOD_KEY = "RankedProgGracePeriod";

	// Token: 0x04004706 RID: 18182
	public static string RANKED_ELO_PC_KEY = "RankedEloPC";

	// Token: 0x04004707 RID: 18183
	public static string RANKED_PROGRESSION_GRACE_PERIOD_PC_KEY = "RankedProgGracePeriodPC";

	// Token: 0x04004708 RID: 18184
	private RankedMultiplayerStatisticFloat EloScorePC;

	// Token: 0x04004709 RID: 18185
	private RankedMultiplayerStatisticFloat EloScoreQuest;

	// Token: 0x0400470A RID: 18186
	private RankedMultiplayerStatisticInt NewTierGracePeriodIdxPC;

	// Token: 0x0400470B RID: 18187
	private RankedMultiplayerStatisticInt NewTierGracePeriodIdxQuest;

	// Token: 0x0400470C RID: 18188
	private GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData ProgressionData;

	// Token: 0x0400470D RID: 18189
	[SerializeField]
	private List<RankedProgressionManager.RankedProgressionTier> majorTiers = new List<RankedProgressionManager.RankedProgressionTier>();

	// Token: 0x0400470E RID: 18190
	[SerializeField]
	private int newTierGracePeriod = 3;

	// Token: 0x0400470F RID: 18191
	public float MaxEloConstant = 90f;

	// Token: 0x04004711 RID: 18193
	private RankedProgressionManager.RankedProgressionEvent ProgressionEvent;

	// Token: 0x04004712 RID: 18194
	public Action<int, float, int> OnPlayerEloAcquired;

	// Token: 0x04004715 RID: 18197
	[Space]
	[ContextMenuItem("Set ELO", "DebugSetELO")]
	public int debugEloPoints = 100;

	// Token: 0x02000847 RID: 2119
	public enum ERankedMatchmakingTier
	{
		// Token: 0x04004717 RID: 18199
		Low,
		// Token: 0x04004718 RID: 18200
		Medium,
		// Token: 0x04004719 RID: 18201
		High
	}

	// Token: 0x02000848 RID: 2120
	public enum ERankedProgressionEventType
	{
		// Token: 0x0400471B RID: 18203
		None,
		// Token: 0x0400471C RID: 18204
		Progress,
		// Token: 0x0400471D RID: 18205
		Promotion,
		// Token: 0x0400471E RID: 18206
		Relegation
	}

	// Token: 0x02000849 RID: 2121
	public class RankedProgressionEvent
	{
		// Token: 0x060037FA RID: 14330 RVA: 0x0012C7F0 File Offset: 0x0012A9F0
		public override string ToString()
		{
			string text = "Progression Info\n";
			text += string.Format("Event Type: {0}\n", this.evtType.ToString());
			text += string.Format("Left Tier: {0}\n", this.leftName);
			text += string.Format("Right Tier: {0}\n", this.rightName);
			text += string.Format("Left Value: {0}\n", this.minVal.ToString("N0"));
			text += string.Format("Right Value: {0}\n", this.maxVal.ToString("N0"));
			text += string.Format("Elo Delta: {0}\n", this.delta.ToString("N0"));
			if (this.evtType == RankedProgressionManager.ERankedProgressionEventType.Promotion || this.evtType == RankedProgressionManager.ERankedProgressionEventType.Relegation)
			{
				text += string.Format("Fanfare Tier: {0}\n", this.newTierName);
			}
			return text;
		}

		// Token: 0x0400471F RID: 18207
		public RankedProgressionManager.ERankedProgressionEventType evtType;

		// Token: 0x04004720 RID: 18208
		public Sprite progressIconLeft;

		// Token: 0x04004721 RID: 18209
		public Sprite progressIconRight;

		// Token: 0x04004722 RID: 18210
		public Sprite newTierIcon;

		// Token: 0x04004723 RID: 18211
		public string leftName;

		// Token: 0x04004724 RID: 18212
		public string rightName;

		// Token: 0x04004725 RID: 18213
		public string newTierName;

		// Token: 0x04004726 RID: 18214
		public float minVal;

		// Token: 0x04004727 RID: 18215
		public float maxVal;

		// Token: 0x04004728 RID: 18216
		public float delta;
	}

	// Token: 0x0200084A RID: 2122
	public abstract class RankedProgressionTierBase
	{
		// Token: 0x060037FC RID: 14332 RVA: 0x0012C8E0 File Offset: 0x0012AAE0
		public void SetMinThreshold(float val)
		{
			this.thresholdMin = val;
		}

		// Token: 0x060037FD RID: 14333 RVA: 0x0012C8E9 File Offset: 0x0012AAE9
		public float GetMinThreshold()
		{
			if (this.thresholdMin < 0f)
			{
				GTDev.LogError<string>("Tier min threshold not initialized. Can only be used at runtime.", null);
			}
			return this.thresholdMin;
		}

		// Token: 0x04004729 RID: 18217
		public string name;

		// Token: 0x0400472A RID: 18218
		public Color color = Color.white;

		// Token: 0x0400472B RID: 18219
		public float thresholdMax;

		// Token: 0x0400472C RID: 18220
		private float thresholdMin = -1f;
	}

	// Token: 0x0200084B RID: 2123
	[Serializable]
	public class RankedProgressionSubTier : RankedProgressionManager.RankedProgressionTierBase
	{
		// Token: 0x0400472D RID: 18221
		public Sprite icon;
	}

	// Token: 0x0200084C RID: 2124
	[Serializable]
	public class RankedProgressionTier : RankedProgressionManager.RankedProgressionTierBase
	{
		// Token: 0x06003800 RID: 14336 RVA: 0x0012C930 File Offset: 0x0012AB30
		public void InsertSubTierAt(int idx, float tierMin)
		{
			RankedProgressionManager.RankedProgressionSubTier rankedProgressionSubTier = new RankedProgressionManager.RankedProgressionSubTier
			{
				name = "NewTier"
			};
			this.subTiers.Insert(idx, rankedProgressionSubTier);
			this.EnforceSubTierValidity(tierMin);
		}

		// Token: 0x06003801 RID: 14337 RVA: 0x0012C964 File Offset: 0x0012AB64
		public void EnforceSubTierValidity(float thresholdMin)
		{
			float num = (((this.thresholdMax == 0f) ? 4000f : this.thresholdMax) - thresholdMin) / (float)this.subTiers.Count;
			for (int i = 0; i < this.subTiers.Count - 1; i++)
			{
				float num2 = thresholdMin + (float)(i + 1) * num;
				num2 = Mathf.Round(num2 / 10f);
				this.subTiers[i].thresholdMax = num2 * 10f;
			}
		}

		// Token: 0x0400472E RID: 18222
		public List<RankedProgressionManager.RankedProgressionSubTier> subTiers = new List<RankedProgressionManager.RankedProgressionSubTier>();
	}
}
