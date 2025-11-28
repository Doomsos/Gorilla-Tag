using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000648 RID: 1608
public class GhostReactorLevelSection : MonoBehaviour
{
	// Token: 0x170003F0 RID: 1008
	// (get) Token: 0x060028DF RID: 10463 RVA: 0x000DAF23 File Offset: 0x000D9123
	public Transform Anchor
	{
		get
		{
			return this.anchorTransform;
		}
	}

	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x060028E0 RID: 10464 RVA: 0x000DAF2B File Offset: 0x000D912B
	public List<Transform> Anchors
	{
		get
		{
			return this.anchors;
		}
	}

	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x060028E1 RID: 10465 RVA: 0x000DAF33 File Offset: 0x000D9133
	public GhostReactorLevelSection.SectionType Type
	{
		get
		{
			return this.sectionType;
		}
	}

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x060028E2 RID: 10466 RVA: 0x000DAF3B File Offset: 0x000D913B
	public BoxCollider BoundingCollider
	{
		get
		{
			return this.boundingCollider;
		}
	}

	// Token: 0x060028E3 RID: 10467 RVA: 0x000DAF44 File Offset: 0x000D9144
	private void Awake()
	{
		this.spawnPointGroupLookup = new GhostReactorLevelSection.SpawnPointGroup[10];
		for (int i = 0; i < this.spawnPointGroups.Count; i++)
		{
			this.spawnPointGroups[i].SpawnPointIndexes = new List<int>();
			int type = (int)this.spawnPointGroups[i].type;
			if (type < this.spawnPointGroupLookup.Length)
			{
				this.spawnPointGroupLookup[type] = this.spawnPointGroups[i];
			}
		}
		this.hazardousMaterials = new List<GRHazardousMaterial>(32);
		base.GetComponentsInChildren<GRHazardousMaterial>(this.hazardousMaterials);
		for (int j = 0; j < this.patrolPaths.Count; j++)
		{
			if (this.patrolPaths[j] == null)
			{
				Debug.LogErrorFormat("Why does {0} have a null patrol path at index {1}", new object[]
				{
					base.gameObject.name,
					j
				});
			}
			else
			{
				this.patrolPaths[j].index = j;
			}
		}
		this.prePlacedGameEntities = new List<GameEntity>(128);
		base.GetComponentsInChildren<GameEntity>(this.prePlacedGameEntities);
		for (int k = 0; k < this.prePlacedGameEntities.Count; k++)
		{
			this.prePlacedGameEntities[k].gameObject.SetActive(false);
		}
		this.renderers = new List<Renderer>(512);
		this.hidden = false;
		base.GetComponentsInChildren<Renderer>(false, this.renderers);
		for (int l = this.renderers.Count - 1; l >= 0; l--)
		{
			if (this.renderers[l] == null || !this.renderers[l].enabled)
			{
				this.renderers.RemoveAt(l);
			}
		}
		if (this.boundingCollider == null)
		{
			Debug.LogWarningFormat("Missing Bounding Collider for section {0}", new object[]
			{
				base.gameObject.name
			});
		}
	}

	// Token: 0x060028E4 RID: 10468 RVA: 0x000DB124 File Offset: 0x000D9324
	public static void RandomizeIndices(List<int> list, int count, ref SRand randomGenerator)
	{
		list.Clear();
		for (int i = 0; i < count; i++)
		{
			list.Add(i);
		}
		randomGenerator.Shuffle<int>(list);
	}

	// Token: 0x060028E5 RID: 10469 RVA: 0x000DB154 File Offset: 0x000D9354
	public void InitLevelSection(int sectionIndex, GhostReactor reactor)
	{
		this.index = sectionIndex;
		for (int i = 0; i < this.hazardousMaterials.Count; i++)
		{
			this.hazardousMaterials[i].Init(reactor);
		}
	}

	// Token: 0x060028E6 RID: 10470 RVA: 0x000DB190 File Offset: 0x000D9390
	public void SpawnSectionEntities(ref SRand randomGenerator, GameEntityManager gameEntityManager, GhostReactor reactor, List<GhostReactorSpawnConfig> spawnConfigs, float respawnCount)
	{
		if (spawnConfigs == null)
		{
			spawnConfigs = this.spawnConfigs;
		}
		if (spawnConfigs != null && spawnConfigs.Count > 0)
		{
			GhostReactorSpawnConfig ghostReactorSpawnConfig = spawnConfigs[randomGenerator.NextInt(spawnConfigs.Count)];
			Debug.LogFormat("Spawn Ghost Reactor Level Section {0} {1}", new object[]
			{
				base.gameObject.name,
				ghostReactorSpawnConfig.name
			});
			for (int i = 0; i < this.spawnPointGroups.Count; i++)
			{
				this.spawnPointGroups[i].CurrentIndex = 0;
				this.spawnPointGroups[i].NeedsRandomization = true;
			}
			for (int j = 0; j < ghostReactorSpawnConfig.entitySpawnGroups.Count; j++)
			{
				int num = ghostReactorSpawnConfig.entitySpawnGroups[j].spawnCount;
				if (num > 0)
				{
					int spawnPointType = (int)ghostReactorSpawnConfig.entitySpawnGroups[j].spawnPointType;
					if (spawnPointType < this.spawnPointGroupLookup.Length)
					{
						GhostReactorLevelSection.SpawnPointGroup spawnPointGroup = this.spawnPointGroupLookup[spawnPointType];
						if (spawnPointGroup != null)
						{
							if (spawnPointGroup.NeedsRandomization)
							{
								spawnPointGroup.NeedsRandomization = false;
								GhostReactorLevelSection.RandomizeIndices(spawnPointGroup.SpawnPointIndexes, spawnPointGroup.spawnPoints.Count, ref randomGenerator);
							}
							num = Mathf.Min(num, spawnPointGroup.spawnPoints.Count);
							for (int k = 0; k < num; k++)
							{
								int currentIndex = spawnPointGroup.CurrentIndex;
								GREntitySpawnPoint nextSpawnPoint = spawnPointGroup.GetNextSpawnPoint();
								nextSpawnPoint == null;
								GameEntity entity = ghostReactorSpawnConfig.entitySpawnGroups[j].entity;
								if (ghostReactorSpawnConfig.entitySpawnGroups[j].randomEntity != null)
								{
									ghostReactorSpawnConfig.entitySpawnGroups[j].randomEntity.TryForRandomItem(reactor, ref randomGenerator, out entity, 0);
								}
								if (!(entity == null))
								{
									int staticHash = entity.name.GetStaticHash();
									long createData = -1L;
									if (nextSpawnPoint.applyScale)
									{
										createData = BitPackUtils.PackWorldPosForNetwork(nextSpawnPoint.transform.localScale);
									}
									else if (spawnPointGroup.type == GhostReactorSpawnConfig.SpawnPointType.Enemy || spawnPointGroup.type == GhostReactorSpawnConfig.SpawnPointType.Pest || nextSpawnPoint.patrolPath != null)
									{
										int patrolIndex = 255;
										if (nextSpawnPoint.patrolPath != null)
										{
											patrolIndex = nextSpawnPoint.patrolPath.index;
										}
										int num2 = (int)respawnCount;
										if (randomGenerator.NextFloat() < respawnCount - (float)num2)
										{
											num2++;
										}
										GhostReactor.EnemyEntityCreateData enemyEntityCreateData;
										enemyEntityCreateData.respawnCount = num2;
										enemyEntityCreateData.sectionIndex = this.index;
										enemyEntityCreateData.patrolIndex = patrolIndex;
										createData = enemyEntityCreateData.Pack();
									}
									GameEntityCreateData gameEntityCreateData = new GameEntityCreateData
									{
										entityTypeId = staticHash,
										position = nextSpawnPoint.transform.position,
										rotation = nextSpawnPoint.transform.rotation,
										createData = createData
									};
									GhostReactorLevelSection.tempCreateEntitiesList.Add(gameEntityCreateData);
									if (GhostReactorLevelSection.tempCreateEntitiesList.Count > 25)
									{
										gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
										GhostReactorLevelSection.tempCreateEntitiesList.Clear();
									}
								}
							}
						}
					}
				}
			}
			for (int l = 0; l < this.prePlacedGameEntities.Count; l++)
			{
				int staticHash2 = this.prePlacedGameEntities[l].gameObject.name.GetStaticHash();
				if (!gameEntityManager.FactoryHasEntity(staticHash2))
				{
					Debug.LogErrorFormat("Cannot Find Entity in Factory {0} {1} Trying to spawn in {2}", new object[]
					{
						this.prePlacedGameEntities[l].gameObject.name,
						staticHash2,
						base.gameObject.name
					});
				}
				else
				{
					GameEntityCreateData gameEntityCreateData2 = new GameEntityCreateData
					{
						entityTypeId = staticHash2,
						position = this.prePlacedGameEntities[l].transform.position,
						rotation = this.prePlacedGameEntities[l].transform.rotation,
						createData = 0L
					};
					GhostReactorLevelSection.tempCreateEntitiesList.Add(gameEntityCreateData2);
					if (GhostReactorLevelSection.tempCreateEntitiesList.Count > 25)
					{
						gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
						GhostReactorLevelSection.tempCreateEntitiesList.Clear();
					}
				}
			}
		}
	}

	// Token: 0x060028E7 RID: 10471 RVA: 0x000DB5A4 File Offset: 0x000D97A4
	public void RespawnEntity(ref SRand randomGenerator, GameEntityManager gameEntityManager, int entityId, long entityCreateData)
	{
		if (0 > this.spawnPointGroupLookup.Length)
		{
			return;
		}
		GhostReactorLevelSection.SpawnPointGroup spawnPointGroup = this.spawnPointGroupLookup[0];
		int count = spawnPointGroup.spawnPoints.Count;
		if (count > 3)
		{
			this.rotatingIndexForRespawn = (this.rotatingIndexForRespawn + randomGenerator.NextInt(1, 1 + spawnPointGroup.spawnPoints.Count / 2)) % spawnPointGroup.spawnPoints.Count;
		}
		else if (count > 1)
		{
			this.rotatingIndexForRespawn = (this.rotatingIndexForRespawn + 1) % count;
		}
		else
		{
			this.rotatingIndexForRespawn = 0;
		}
		GREntitySpawnPoint grentitySpawnPoint = spawnPointGroup.spawnPoints[this.rotatingIndexForRespawn];
		GhostReactor.EnemyEntityCreateData enemyEntityCreateData = GhostReactor.EnemyEntityCreateData.Unpack(entityCreateData);
		enemyEntityCreateData.patrolIndex = ((grentitySpawnPoint.patrolPath != null) ? grentitySpawnPoint.patrolPath.index : 255);
		long createData = enemyEntityCreateData.Pack();
		gameEntityManager.RequestCreateItem(entityId, grentitySpawnPoint.transform.position, grentitySpawnPoint.transform.rotation, createData);
	}

	// Token: 0x060028E8 RID: 10472 RVA: 0x000DB68E File Offset: 0x000D988E
	public GRPatrolPath GetPatrolPath(int patrolPathIndex)
	{
		if (patrolPathIndex >= 0 && patrolPathIndex < this.patrolPaths.Count)
		{
			return this.patrolPaths[patrolPathIndex];
		}
		return null;
	}

	// Token: 0x060028E9 RID: 10473 RVA: 0x000DB6B0 File Offset: 0x000D98B0
	public void Hide(bool hide)
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (!(this.renderers[i] == null))
			{
				this.renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x000DB6FC File Offset: 0x000D98FC
	public void UpdateDisable(Vector3 playerPos)
	{
		if (this.boundingCollider == null)
		{
			return;
		}
		float distSq = this.GetDistSq(playerPos);
		float num = 1024f;
		float num2 = 1296f;
		if (this.hidden && distSq < num)
		{
			this.hidden = false;
			this.Hide(false);
			return;
		}
		if (!this.hidden && distSq > num2)
		{
			this.hidden = true;
			this.Hide(true);
		}
	}

	// Token: 0x060028EB RID: 10475 RVA: 0x000DB764 File Offset: 0x000D9964
	public float GetDistSq(Vector3 pos)
	{
		return (this.boundingCollider.ClosestPoint(pos) - pos).sqrMagnitude;
	}

	// Token: 0x060028EC RID: 10476 RVA: 0x000DB78B File Offset: 0x000D998B
	public Transform GetAnchor(int anchorIndex)
	{
		return this.anchors[anchorIndex];
	}

	// Token: 0x040034A5 RID: 13477
	private const float SHOW_DIST = 32f;

	// Token: 0x040034A6 RID: 13478
	private const float HIDE_DIST = 36f;

	// Token: 0x040034A7 RID: 13479
	private const int MAX_CREATE_PER_RPC = 25;

	// Token: 0x040034A8 RID: 13480
	[SerializeField]
	private GhostReactorLevelSection.SectionType sectionType;

	// Token: 0x040034A9 RID: 13481
	[SerializeField]
	[Tooltip("Single Anchor Transform used for End Caps and Blockers")]
	private Transform anchorTransform;

	// Token: 0x040034AA RID: 13482
	[SerializeField]
	[Tooltip("A List of Anchors used as in and out connections for Hubs")]
	private List<Transform> anchors = new List<Transform>();

	// Token: 0x040034AB RID: 13483
	[SerializeField]
	private List<GhostReactorLevelSection.SpawnPointGroup> spawnPointGroups;

	// Token: 0x040034AC RID: 13484
	[SerializeField]
	private List<GhostReactorSpawnConfig> spawnConfigs;

	// Token: 0x040034AD RID: 13485
	[SerializeField]
	private List<GRPatrolPath> patrolPaths;

	// Token: 0x040034AE RID: 13486
	[SerializeField]
	private BoxCollider boundingCollider;

	// Token: 0x040034AF RID: 13487
	private List<Renderer> renderers;

	// Token: 0x040034B0 RID: 13488
	private bool hidden;

	// Token: 0x040034B1 RID: 13489
	private List<GRHazardousMaterial> hazardousMaterials;

	// Token: 0x040034B2 RID: 13490
	[HideInInspector]
	public GhostReactorLevelSectionConnector sectionConnector;

	// Token: 0x040034B3 RID: 13491
	[HideInInspector]
	public int hubAnchorIndex;

	// Token: 0x040034B4 RID: 13492
	private int index;

	// Token: 0x040034B5 RID: 13493
	private GhostReactorLevelSection.SpawnPointGroup[] spawnPointGroupLookup;

	// Token: 0x040034B6 RID: 13494
	private List<GameEntity> prePlacedGameEntities;

	// Token: 0x040034B7 RID: 13495
	public static List<GameEntityCreateData> tempCreateEntitiesList = new List<GameEntityCreateData>(32);

	// Token: 0x040034B8 RID: 13496
	private int rotatingIndexForRespawn;

	// Token: 0x02000649 RID: 1609
	public enum SectionType
	{
		// Token: 0x040034BA RID: 13498
		Hub,
		// Token: 0x040034BB RID: 13499
		EndCap,
		// Token: 0x040034BC RID: 13500
		Blocker
	}

	// Token: 0x0200064A RID: 1610
	[Serializable]
	public class SpawnPointGroup
	{
		// Token: 0x170003F4 RID: 1012
		// (get) Token: 0x060028EF RID: 10479 RVA: 0x000DB7BA File Offset: 0x000D99BA
		// (set) Token: 0x060028F0 RID: 10480 RVA: 0x000DB7C2 File Offset: 0x000D99C2
		public bool NeedsRandomization
		{
			get
			{
				return this.needsRandomization;
			}
			set
			{
				this.needsRandomization = value;
			}
		}

		// Token: 0x170003F5 RID: 1013
		// (get) Token: 0x060028F1 RID: 10481 RVA: 0x000DB7CB File Offset: 0x000D99CB
		// (set) Token: 0x060028F2 RID: 10482 RVA: 0x000DB7D3 File Offset: 0x000D99D3
		public int CurrentIndex
		{
			get
			{
				return this.currentIndex;
			}
			set
			{
				this.currentIndex = value;
			}
		}

		// Token: 0x170003F6 RID: 1014
		// (get) Token: 0x060028F3 RID: 10483 RVA: 0x000DB7DC File Offset: 0x000D99DC
		// (set) Token: 0x060028F4 RID: 10484 RVA: 0x000DB7E4 File Offset: 0x000D99E4
		public List<int> SpawnPointIndexes
		{
			get
			{
				return this.spawnPointIndexes;
			}
			set
			{
				this.spawnPointIndexes = value;
			}
		}

		// Token: 0x060028F5 RID: 10485 RVA: 0x000DB7ED File Offset: 0x000D99ED
		public GREntitySpawnPoint GetNextSpawnPoint()
		{
			GREntitySpawnPoint result = this.spawnPoints[this.spawnPointIndexes[this.currentIndex]];
			this.currentIndex = (this.currentIndex + 1) % this.spawnPointIndexes.Count;
			return result;
		}

		// Token: 0x040034BD RID: 13501
		public GhostReactorSpawnConfig.SpawnPointType type;

		// Token: 0x040034BE RID: 13502
		public List<GREntitySpawnPoint> spawnPoints;

		// Token: 0x040034BF RID: 13503
		private List<int> spawnPointIndexes;

		// Token: 0x040034C0 RID: 13504
		private bool needsRandomization;

		// Token: 0x040034C1 RID: 13505
		private int currentIndex;
	}
}
