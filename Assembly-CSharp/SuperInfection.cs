using System;
using System.Collections.Generic;
using GorillaGameModes;
using TMPro;
using UnityEngine;

// Token: 0x02000156 RID: 342
[DefaultExecutionOrder(1)]
public class SuperInfection : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x170000BC RID: 188
	// (get) Token: 0x0600090F RID: 2319 RVA: 0x00030D35 File Offset: 0x0002EF35
	public bool IsAuthorityAndActive
	{
		get
		{
			return this.siManager.gameEntityManager.IsAuthority() && this.siManager.gameEntityManager.IsZoneActive();
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000910 RID: 2320 RVA: 0x00030D5B File Offset: 0x0002EF5B
	public float ResourceSpawnInterval
	{
		get
		{
			if (!Application.isPlaying)
			{
				return 0f;
			}
			return this.GetResourceSpawnInterval();
		}
	}

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06000911 RID: 2321 RVA: 0x00030D70 File Offset: 0x0002EF70
	public float TimeSinceLastSpawn
	{
		get
		{
			return Time.time - this._lastResourceSpawnTime;
		}
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000912 RID: 2322 RVA: 0x00030D7E File Offset: 0x0002EF7E
	public float TimeToNextSpawn
	{
		get
		{
			if (!Application.isPlaying)
			{
				return 0f;
			}
			if (this._lastResourceSpawnTime <= 0f)
			{
				return 0f;
			}
			return this.GetResourceSpawnInterval() - (Time.time - this._lastResourceSpawnTime);
		}
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x00030DB4 File Offset: 0x0002EFB4
	private void Awake()
	{
		this.resourceRegions = this.resourceNodeParent.GetComponentsInChildren<SIResourceRegion>(true);
		this._resourcePrefabs = new List<SIResource>();
		foreach (SIResourceRegion siresourceRegion in this.resourceRegions)
		{
			if (!this._resourcePrefabs.Contains(siresourceRegion.resourcePrefab))
			{
				this._resourcePrefabs.Add(siresourceRegion.resourcePrefab);
			}
		}
		this.perRoundResourceRegions = this.perRoundResourceNodeParent.GetComponentsInChildren<SIResourceRegion>(true);
		this.resourceResetHeight = this.resourceResetLoc.position.y;
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00030E44 File Offset: 0x0002F044
	public void OnEnable()
	{
		this.siManager = SuperInfectionManager.GetSIManagerForZone(this.zone);
		if (this.siManager != null)
		{
			Debug.Log(string.Format("$OnEnable: {0} zoneSuperInfection = {1}", this.siManager, this));
			this.siManager.OnEnableZoneSuperInfection(this);
		}
		if (this.siManager.isActiveAndEnabled)
		{
			this.DisableStations();
		}
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnPlayerLeft += new Action<NetPlayer>(this.RemovePlayerGadgetsOnLeave);
		}
		for (int i = 0; i < this.siTerminals.Length; i++)
		{
			this.siTerminals[i].index = i;
		}
		for (int j = 0; j < this.siDeposits.Length; j++)
		{
			this.siDeposits[j].index = j;
		}
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00030F1C File Offset: 0x0002F11C
	public void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.siManager)
		{
			this.siManager.zoneSuperInfection = null;
		}
		this.DisableStations();
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnPlayerLeft -= new Action<NetPlayer>(this.RemovePlayerGadgetsOnLeave);
		}
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00030F85 File Offset: 0x0002F185
	public void OnZoneInit()
	{
		this.EnableStations();
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x00030F8D File Offset: 0x0002F18D
	public void OnZoneClear(ZoneClearReason reason)
	{
		if (reason != ZoneClearReason.JoinZone)
		{
			this.DisableStations();
			SIProgression.Instance.SendTelemetryData();
		}
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x00030FA4 File Offset: 0x0002F1A4
	private void EnableStations()
	{
		for (int i = 0; i < this.siTerminals.Length; i++)
		{
			this.siTerminals[i].gameObject.SetActive(true);
		}
		for (int j = 0; j < this.siDeposits.Length; j++)
		{
			this.siDeposits[j].gameObject.SetActive(true);
		}
		this.questBoard.gameObject.SetActive(true);
		if (this.purchaseTerminal != null)
		{
			this.purchaseTerminal.gameObject.SetActive(true);
		}
		for (int k = 0; k < this.zoneObjects.Length; k++)
		{
			GameObject gameObject = this.zoneObjects[k];
			if (gameObject != null)
			{
				gameObject.SetActive(true);
			}
			else
			{
				Debug.LogError("[GT/SuperInfection]  ERROR!!!  " + string.Format("null ref at `zoneObjects[{0}]`.", k));
			}
		}
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x0003107C File Offset: 0x0002F27C
	private void DisableStations()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		for (int i = 0; i < this.siTerminals.Length; i++)
		{
			this.siTerminals[i].gameObject.SetActive(false);
			this.siTerminals[i].Reset();
		}
		for (int j = 0; j < this.siDeposits.Length; j++)
		{
			this.siDeposits[j].gameObject.SetActive(false);
		}
		this.questBoard.gameObject.SetActive(false);
		if (this.purchaseTerminal != null)
		{
			this.purchaseTerminal.gameObject.SetActive(false);
		}
		for (int k = 0; k < this.zoneObjects.Length; k++)
		{
			GameObject gameObject = this.zoneObjects[k];
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
			else
			{
				Debug.LogError("[GT/SuperInfection]  ERROR!!!  " + string.Format("null ref at `zoneObjects[{0}]`.", k));
			}
		}
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00031168 File Offset: 0x0002F368
	public void Update()
	{
		if (!this.IsAuthorityAndActive)
		{
			return;
		}
		if (this.retryCreatePerRoundResources)
		{
			this.CreatePerRoundResources();
		}
		if (Time.time >= this._nextResourceUpdateTime)
		{
			this.GetResourceSpawnInterval();
			foreach (SIResourceRegion siresourceRegion in this.resourceRegions)
			{
				for (int j = siresourceRegion.ItemCount - 1; j >= 0; j--)
				{
					GameEntity gameEntity = siresourceRegion.Items[j];
					if (!gameEntity)
					{
						GTDev.Log<string>(string.Format("Removing null item at {0}", j), null);
						siresourceRegion.Items.RemoveAt(j);
					}
					else if (gameEntity.transform.position.y < this.resourceResetHeight)
					{
						this.siManager.gameEntityManager.RequestDestroyItem(gameEntity.id);
					}
				}
			}
			this.CheckResourceSpawn();
			this._nextResourceUpdateTime = Time.time + 1f;
		}
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x00031258 File Offset: 0x0002F458
	private void CheckResourceSpawn()
	{
		if (Time.time >= this.GetNextResourceSpawnTime())
		{
			SIResourceRegion siresourceRegion = null;
			float num = float.MaxValue;
			foreach (SIResourceRegion siresourceRegion2 in this.resourceRegions)
			{
				if (siresourceRegion2.ItemCount < siresourceRegion2.MaxItems && siresourceRegion2.LastSpawnTime < num)
				{
					siresourceRegion = siresourceRegion2;
					num = siresourceRegion2.LastSpawnTime;
				}
			}
			if (!siresourceRegion)
			{
				this._lastResourceSpawnTime = Time.time;
				return;
			}
			ValueTuple<bool, Vector3, Vector3> spawnPointWithNormal = siresourceRegion.GetSpawnPointWithNormal(5);
			if (!spawnPointWithNormal.Item1)
			{
				GTDev.Log<string>(string.Format("[{0}] Couldn't find a valid {1} spawn point in {2}", base.name, siresourceRegion.resourcePrefab.name, siresourceRegion), null);
				return;
			}
			if (siresourceRegion.resourcePrefab == null)
			{
				GTDev.Log<string>("No resourceprefab set for region", null);
				return;
			}
			float spawnPitchVariance = siresourceRegion.resourcePrefab.spawnPitchVariance;
			Quaternion quaternion = Quaternion.Euler(Random.Range(-spawnPitchVariance, spawnPitchVariance), (float)Random.Range(0, 360), Random.Range(-spawnPitchVariance, spawnPitchVariance));
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, spawnPointWithNormal.Item3), spawnPointWithNormal.Item3) * quaternion;
			GameEntity gameEntity = this.siManager.gameEntityManager.GetGameEntity(this.siManager.gameEntityManager.RequestCreateItem(siresourceRegion.resourcePrefab.gameObject.name.GetStaticHash(), spawnPointWithNormal.Item2, rotation, 0L));
			if (gameEntity)
			{
				GTDev.Log<string>(string.Format("Spawned {0} at {1}", gameEntity.name, spawnPointWithNormal.Item2), gameEntity, null);
				siresourceRegion.AddItem(gameEntity);
				siresourceRegion.LastSpawnTime = (this._lastResourceSpawnTime = Time.time);
				return;
			}
			GTDev.LogError<string>(string.Format("Failed to spawn {0} at {1}", siresourceRegion.resourcePrefab.gameObject.name, spawnPointWithNormal.Item2), null);
		}
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00031430 File Offset: 0x0002F630
	private float GetNextResourceSpawnTime()
	{
		if (this._lastResourceSpawnTime <= 0f)
		{
			return 0f;
		}
		return this._lastResourceSpawnTime + this.GetResourceSpawnInterval();
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00031452 File Offset: 0x0002F652
	private float GetResourceSpawnInterval()
	{
		return 3600f / (float)(this.perPlayerHourlyResourceRate * Mathf.Max(GameMode.ParticipatingPlayers.Count, this.minRoomPopulation));
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00031478 File Offset: 0x0002F678
	public void RemovePlayerGadgetsOnLeave(NetPlayer player)
	{
		SIPlayer siplayer = SIPlayer.Get(player.ActorNumber);
		if (siplayer == null)
		{
			return;
		}
		if (this.siManager.gameEntityManager.IsAuthority())
		{
			for (int i = siplayer.activePlayerGadgets.Count - 1; i >= 0; i--)
			{
				this.siManager.gameEntityManager.RequestDestroyItem(this.siManager.gameEntityManager.GetGameEntityFromNetId(siplayer.activePlayerGadgets[i]).id);
			}
		}
		siplayer.activePlayerGadgets.Clear();
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x00031504 File Offset: 0x0002F704
	public void RefreshStations(int actorNr)
	{
		for (int i = 0; i < this.siTerminals.Length; i++)
		{
			if (!(this.siTerminals[i].activePlayer == null) && this.siTerminals[i].activePlayer.gameObject.activeInHierarchy && this.siTerminals[i].activePlayer.ActorNr == actorNr)
			{
				this.siTerminals[i].techTree.UpdateState(this.siTerminals[i].techTree.currentState);
				this.siTerminals[i].resourceCollection.UpdateState(this.siTerminals[i].resourceCollection.currentState);
				this.siTerminals[i].dispenser.UpdateState(this.siTerminals[i].dispenser.currentState);
			}
		}
		if (SIPlayer.LocalPlayer.ActorNr == actorNr && this.purchaseTerminal != null)
		{
			this.purchaseTerminal.UpdateCurrentTechPoints();
		}
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x00031608 File Offset: 0x0002F808
	public void SliceUpdate()
	{
		if (this.siManager.gameEntityManager.IsAuthority())
		{
			for (int i = this.activeGadgets.Count - 1; i >= 0; i--)
			{
				if (this.activeGadgets[i] == null)
				{
					this.activeGadgets.RemoveAt(i);
				}
				else if (this.activeGadgets[i].transform.position.y < this.resourceResetHeight)
				{
					this.siManager.gameEntityManager.RequestDestroyItem(this.activeGadgets[i].gameEntity.id);
				}
			}
		}
	}

	// Token: 0x170000C0 RID: 192
	// (get) Token: 0x06000921 RID: 2337 RVA: 0x000316AD File Offset: 0x0002F8AD
	public List<SIResource> ResourcePrefabs
	{
		get
		{
			return this._resourcePrefabs;
		}
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x000316B5 File Offset: 0x0002F8B5
	public void AddGadget(SIGadget gadget)
	{
		this.activeGadgets.Add(gadget);
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x000316C3 File Offset: 0x0002F8C3
	public void RemoveGadget(SIGadget gadget)
	{
		this.activeGadgets.Remove(gadget);
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x000316D2 File Offset: 0x0002F8D2
	public void ResetPerRoundResources()
	{
		this.ClearPerRoundResources();
		this.CreatePerRoundResources();
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x000316E0 File Offset: 0x0002F8E0
	private void CreatePerRoundResources()
	{
		if (!this.siManager.gameEntityManager.IsZoneActive())
		{
			this.retryCreatePerRoundResources = true;
			return;
		}
		this.retryCreatePerRoundResources = false;
		foreach (SIResourceRegion siresourceRegion in this.perRoundResourceRegions)
		{
			for (int j = siresourceRegion.ItemCount; j < siresourceRegion.MaxItems; j++)
			{
				ValueTuple<bool, Vector3, Vector3> spawnPointWithNormal = siresourceRegion.GetSpawnPointWithNormal(5);
				Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Vector3.forward, spawnPointWithNormal.Item3), spawnPointWithNormal.Item3) * Quaternion.Euler(0f, (float)Random.Range(0, 360), 0f);
				GameEntity gameEntity = this.siManager.gameEntityManager.GetGameEntity(this.siManager.gameEntityManager.RequestCreateItem(siresourceRegion.resourcePrefab.gameObject.name.GetStaticHash(), spawnPointWithNormal.Item2, rotation, 0L));
				if (gameEntity)
				{
					siresourceRegion.AddItem(gameEntity);
					if (!spawnPointWithNormal.Item1)
					{
						Rigidbody component = gameEntity.GetComponent<Rigidbody>();
						if (component != null)
						{
							component.isKinematic = false;
						}
					}
				}
				else
				{
					GTDev.LogError<string>(string.Format("Failed to spawn {0} at {1}", siresourceRegion.resourcePrefab.gameObject.name, spawnPointWithNormal.Item2), null);
				}
			}
		}
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x00031830 File Offset: 0x0002FA30
	private void ClearPerRoundResources()
	{
		foreach (SIResourceRegion siresourceRegion in this.perRoundResourceRegions)
		{
			for (int j = siresourceRegion.ItemCount - 1; j >= 0; j--)
			{
				GameEntity gameEntity = siresourceRegion.Items[j];
				if (!gameEntity)
				{
					siresourceRegion.Items.RemoveAt(j);
				}
				else if (gameEntity.lastHeldByActorNumber == 0 || !(SIPlayer.Get(gameEntity.lastHeldByActorNumber) != null))
				{
					this.siManager.gameEntityManager.RequestDestroyItem(gameEntity.id);
				}
			}
		}
	}

	// Token: 0x04000B20 RID: 2848
	private const string preLog = "[GT/SuperInfection]  ";

	// Token: 0x04000B21 RID: 2849
	private const string preErr = "[GT/SuperInfection]  ERROR!!!  ";

	// Token: 0x04000B22 RID: 2850
	public SICombinedTerminal[] siTerminals;

	// Token: 0x04000B23 RID: 2851
	public SIResourceDeposit[] siDeposits;

	// Token: 0x04000B24 RID: 2852
	public SIQuestBoard questBoard;

	// Token: 0x04000B25 RID: 2853
	public SIPurchaseTerminal purchaseTerminal;

	// Token: 0x04000B26 RID: 2854
	[Tooltip("Add miscellaneous zone objects here.  They'll be disabled when not in this mode.")]
	public GameObject[] zoneObjects;

	// Token: 0x04000B27 RID: 2855
	public Transform resourceNodeParent;

	// Token: 0x04000B28 RID: 2856
	public SIResourceRegion[] resourceRegions;

	// Token: 0x04000B29 RID: 2857
	public int perPlayerHourlyResourceRate = 20;

	// Token: 0x04000B2A RID: 2858
	[Tooltip("Resource generation rate varies based on population.  We'll assume at least this many players are present.")]
	public int minRoomPopulation = 4;

	// Token: 0x04000B2B RID: 2859
	public Transform perRoundResourceNodeParent;

	// Token: 0x04000B2C RID: 2860
	public SIResourceRegion[] perRoundResourceRegions;

	// Token: 0x04000B2D RID: 2861
	public SuperInfectionManager siManager;

	// Token: 0x04000B2E RID: 2862
	public Transform resourceResetLoc;

	// Token: 0x04000B2F RID: 2863
	private float resourceResetHeight;

	// Token: 0x04000B30 RID: 2864
	public List<SIGadget> activeGadgets = new List<SIGadget>();

	// Token: 0x04000B31 RID: 2865
	public GTZone zone;

	// Token: 0x04000B32 RID: 2866
	public SITechTreeSO techTreeSO;

	// Token: 0x04000B33 RID: 2867
	private bool retryCreatePerRoundResources;

	// Token: 0x04000B34 RID: 2868
	private float _nextResourceUpdateTime;

	// Token: 0x04000B35 RID: 2869
	private float _lastResourceSpawnTime;

	// Token: 0x04000B36 RID: 2870
	private int authorityActorNumber;

	// Token: 0x04000B37 RID: 2871
	public TextMeshProUGUI authorityName;

	// Token: 0x04000B38 RID: 2872
	private List<SIResource> _resourcePrefabs;
}
