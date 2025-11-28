using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x0200095F RID: 2399
public class CustomMapsGameManager : MonoBehaviour, IGameEntityZoneComponent
{
	// Token: 0x06003D6A RID: 15722 RVA: 0x00145E09 File Offset: 0x00144009
	private void Awake()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			Object.Destroy(this);
			return;
		}
		CustomMapsGameManager.instance = this;
		this.customMapsAgents = new Dictionary<int, AIAgent>(Constants.aiAgentLimit);
		CustomMapsGameManager.tempCreateEntitiesList = new List<GameEntityCreateData>(Constants.aiAgentLimit);
	}

	// Token: 0x06003D6B RID: 15723 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x06003D6C RID: 15724 RVA: 0x00145E44 File Offset: 0x00144044
	public void CreatePlacedEntities(List<MapEntity> entities)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("CustomMapsManager::CreateAIAgents not the authority", null);
			return;
		}
		int gameAgentCount = this.gameAgentManager.GetGameAgentCount();
		if (gameAgentCount >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>("[CustomMapsGameManager::CreateAIAgents] Failed to create agent. Max Agent count " + string.Format("({0}) has been reached!", Constants.aiAgentLimit), null);
			return;
		}
		CustomMapsGameManager.tempCreateEntitiesList.Clear();
		int num = (Constants.aiAgentLimit - gameAgentCount < 0) ? 0 : (Constants.aiAgentLimit - gameAgentCount);
		int num2 = Mathf.Min(entities.Count, num);
		if (num2 < entities.Count)
		{
			GTDev.LogWarning<string>(string.Format("[CustomMapsGameManager::CreateAIAgents] Only creating {0} out of the ", num2) + string.Format("requested {0} agents. Max Agent count ({1}) has been reached.!", entities.Count, Constants.aiAgentLimit), null);
		}
		for (int i = 0; i < num2; i++)
		{
			if (entities[i].IsNull())
			{
				Debug.Log(string.Format("[CustomMapsGameManager::CreateAIAgents] Requested entity to create is null! {0}/{1}", i, entities.Count));
			}
			else
			{
				int num3 = (entities[i] is AIAgent) ? "CustomMapsAIAgent".GetStaticHash() : "CustomMapsGrabbableEntity".GetStaticHash();
				if (!this.gameEntityManager.FactoryHasEntity(num3))
				{
					Debug.LogErrorFormat("[CustomMapsManager::CreateAIAgents] Cannot Find Entity in Factory {0} {1}", new object[]
					{
						entities[i].gameObject.name,
						num3
					});
				}
				else
				{
					GameEntityCreateData gameEntityCreateData = new GameEntityCreateData
					{
						entityTypeId = num3,
						position = entities[i].transform.position,
						rotation = entities[i].transform.rotation,
						createData = entities[i].GetPackedCreateData()
					};
					CustomMapsGameManager.tempCreateEntitiesList.Add(gameEntityCreateData);
				}
			}
		}
		if (CustomMapsGameManager.tempCreateEntitiesList.Count > 0)
		{
			this.gameEntityManager.RequestCreateItems(CustomMapsGameManager.tempCreateEntitiesList);
			CustomMapsGameManager.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x06003D6D RID: 15725 RVA: 0x00146047 File Offset: 0x00144247
	public void TEST_Spawning()
	{
		GTDev.Log<string>("CustomMapsGameManager::TEST_Spawn starting spawn", null);
		base.StartCoroutine(this.TEST_Spawn());
	}

	// Token: 0x06003D6E RID: 15726 RVA: 0x00146061 File Offset: 0x00144261
	private IEnumerator TEST_Spawn()
	{
		while (this.spawnCount < 10)
		{
			yield return new WaitForSeconds(5f);
			GTDev.Log<string>("CustomMapsGameManager::TEST_Spawn spawning enemy", null);
			this.TEST_index = ((this.TEST_index == 5) ? 3 : 5);
			this.SpawnEnemyFromPoint("79e43963", this.TEST_index);
			this.spawnCount++;
		}
		yield break;
	}

	// Token: 0x06003D6F RID: 15727 RVA: 0x00146070 File Offset: 0x00144270
	public GameEntityId SpawnEnemyFromPoint(string spawnPointId, int enemyTypeId)
	{
		AISpawnPoint aispawnPoint;
		if (!AISpawnManager.instance.GetSpawnPoint(spawnPointId, ref aispawnPoint))
		{
			GTDev.LogError<string>("CustomMapsGameManager::SpawnEnemyFromPoint cannot find spawn point", null);
			return GameEntityId.Invalid;
		}
		return this.SpawnEnemyAtLocation(enemyTypeId, aispawnPoint.transform.position, aispawnPoint.transform.rotation);
	}

	// Token: 0x06003D70 RID: 15728 RVA: 0x001460BC File Offset: 0x001442BC
	public GameEntityId SpawnEnemyAtLocation(int enemyTypeId, Vector3 position, Quaternion rotation)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed: Not Authority", null);
			return GameEntityId.Invalid;
		}
		if (this.gameEntityManager.GetGameEntities().Count >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>(string.Format("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed: Max Agents ({0}) reached.", Constants.aiAgentLimit), null);
			return GameEntityId.Invalid;
		}
		int staticHash = "CustomMapsAIAgent".GetStaticHash();
		if (!this.gameEntityManager.FactoryHasEntity(staticHash))
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnEnemyAtLocation] Failed cannot find entity type", null);
			return GameEntityId.Invalid;
		}
		return this.gameEntityManager.RequestCreateItem(staticHash, position, rotation, (long)enemyTypeId);
	}

	// Token: 0x06003D71 RID: 15729 RVA: 0x00146158 File Offset: 0x00144358
	public void SpawnEnemyClient(int enemyTypeId, int agentId)
	{
		if (this.gameEntityManager.IsAuthority())
		{
			return;
		}
		if (enemyTypeId == -1)
		{
			return;
		}
		AIAgent aiagent;
		if (AISpawnManager.HasInstance && AISpawnManager.instance.SpawnEnemy(enemyTypeId, ref aiagent))
		{
			aiagent.transform.parent = AISpawnManager.instance.transform;
			this.customMapsAgents[agentId] = aiagent;
			return;
		}
		MapEntity mapEntity;
		if (MapSpawnManager.instance.SpawnEntity(enemyTypeId, ref mapEntity))
		{
			aiagent = (AIAgent)mapEntity;
			aiagent.transform.parent = AISpawnManager.instance.transform;
			this.customMapsAgents[agentId] = aiagent;
			return;
		}
	}

	// Token: 0x06003D72 RID: 15730 RVA: 0x001461EC File Offset: 0x001443EC
	public GameEntityId SpawnGrabbableAtLocation(int enemyTypeId, Vector3 position, Quaternion rotation)
	{
		if (!this.gameEntityManager.IsAuthority())
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnGrabbableAtLocation] Failed: Not Authority", null);
			return GameEntityId.Invalid;
		}
		if (this.gameEntityManager.GetGameEntities().Count >= Constants.aiAgentLimit)
		{
			GTDev.LogError<string>(string.Format("[CustomMapsGameManager::SpawnGrabbableAtLocation] Failed: Max Entities ({0}) reached.", Constants.aiAgentLimit), null);
			return GameEntityId.Invalid;
		}
		int staticHash = "CustomMapsGrabbableEntity".GetStaticHash();
		if (!this.gameEntityManager.FactoryHasEntity(staticHash))
		{
			GTDev.LogError<string>("[CustomMapsGameManager::SpawnGrabbableAtLocation] Failed cannot find entity type", null);
			return GameEntityId.Invalid;
		}
		return this.gameEntityManager.RequestCreateItem(staticHash, position, rotation, (long)enemyTypeId);
	}

	// Token: 0x06003D73 RID: 15731 RVA: 0x000DF85B File Offset: 0x000DDA5B
	public long ProcessMigratedGameEntityCreateData(GameEntity entity, long createData)
	{
		return createData;
	}

	// Token: 0x06003D74 RID: 15732 RVA: 0x00002076 File Offset: 0x00000276
	public bool ValidateMigratedGameEntity(int netId, int entityTypeId, Vector3 position, Quaternion rotation, long createData, int actorNr)
	{
		return false;
	}

	// Token: 0x06003D75 RID: 15733 RVA: 0x00146288 File Offset: 0x00144488
	private bool IsAuthority()
	{
		return this.gameEntityManager.IsAuthority();
	}

	// Token: 0x06003D76 RID: 15734 RVA: 0x00146295 File Offset: 0x00144495
	private bool IsDriver()
	{
		return CustomMapsTerminal.IsDriver;
	}

	// Token: 0x06003D77 RID: 15735 RVA: 0x00002789 File Offset: 0x00000989
	public void OnZoneCreate()
	{
	}

	// Token: 0x06003D78 RID: 15736 RVA: 0x0014629C File Offset: 0x0014449C
	public void OnZoneInit()
	{
		if (CustomMapsGameManager.agentsToCreateOnZoneInit.IsNullOrEmpty<MapEntity>())
		{
			return;
		}
		this.CreatePlacedEntities(CustomMapsGameManager.agentsToCreateOnZoneInit);
		CustomMapsGameManager.agentsToCreateOnZoneInit.Clear();
	}

	// Token: 0x06003D79 RID: 15737 RVA: 0x00002789 File Offset: 0x00000989
	public void OnZoneClear(ZoneClearReason reason)
	{
	}

	// Token: 0x06003D7A RID: 15738 RVA: 0x00027DED File Offset: 0x00025FED
	public bool ShouldClearZone()
	{
		return true;
	}

	// Token: 0x06003D7B RID: 15739 RVA: 0x001462C0 File Offset: 0x001444C0
	public bool IsZoneReady()
	{
		return CustomMapLoader.CanLoadEntities && NetworkSystem.Instance.InRoom;
	}

	// Token: 0x06003D7C RID: 15740 RVA: 0x00002789 File Offset: 0x00000989
	public void OnCreateGameEntity(GameEntity entity)
	{
	}

	// Token: 0x06003D7D RID: 15741 RVA: 0x00002789 File Offset: 0x00000989
	private void SetupCollisions(GameObject go)
	{
	}

	// Token: 0x06003D7E RID: 15742 RVA: 0x00002789 File Offset: 0x00000989
	public void SerializeZoneData(BinaryWriter writer)
	{
	}

	// Token: 0x06003D7F RID: 15743 RVA: 0x00002789 File Offset: 0x00000989
	public void DeserializeZoneData(BinaryReader reader)
	{
	}

	// Token: 0x06003D80 RID: 15744 RVA: 0x00002789 File Offset: 0x00000989
	public void SerializeZoneEntityData(BinaryWriter writer, GameEntity entity)
	{
	}

	// Token: 0x06003D81 RID: 15745 RVA: 0x00002789 File Offset: 0x00000989
	public void DeserializeZoneEntityData(BinaryReader reader, GameEntity entity)
	{
	}

	// Token: 0x06003D82 RID: 15746 RVA: 0x00002789 File Offset: 0x00000989
	public void SerializeZonePlayerData(BinaryWriter writer, int actorNumber)
	{
	}

	// Token: 0x06003D83 RID: 15747 RVA: 0x00002789 File Offset: 0x00000989
	public void DeserializeZonePlayerData(BinaryReader reader, int actorNumber)
	{
	}

	// Token: 0x06003D84 RID: 15748 RVA: 0x001462D5 File Offset: 0x001444D5
	public static GameEntityManager GetEntityManager()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			return CustomMapsGameManager.instance.gameEntityManager;
		}
		return null;
	}

	// Token: 0x06003D85 RID: 15749 RVA: 0x001462EF File Offset: 0x001444EF
	public static GameAgentManager GetAgentManager()
	{
		if (CustomMapsGameManager.instance.IsNotNull())
		{
			return CustomMapsGameManager.instance.gameAgentManager;
		}
		return null;
	}

	// Token: 0x06003D86 RID: 15750 RVA: 0x0014630C File Offset: 0x0014450C
	public static CustomMapsAIBehaviourController GetBehaviorControllerForEntity(GameEntityId entityId)
	{
		GameEntityManager entityManager = CustomMapsGameManager.GetEntityManager();
		if (entityManager.IsNull())
		{
			return null;
		}
		GameEntity gameEntity = entityManager.GetGameEntity(entityId);
		if (gameEntity.IsNull())
		{
			return null;
		}
		return gameEntity.gameObject.GetComponent<CustomMapsAIBehaviourController>();
	}

	// Token: 0x06003D87 RID: 15751 RVA: 0x00146346 File Offset: 0x00144546
	public static void AddAgentsToCreate(List<MapEntity> entitiesToCreate)
	{
		if (CustomMapsGameManager.instance.IsNull())
		{
			return;
		}
		if (entitiesToCreate.IsNullOrEmpty<MapEntity>())
		{
			return;
		}
		CustomMapsGameManager.agentsToCreateOnZoneInit.AddRange(entitiesToCreate);
	}

	// Token: 0x06003D88 RID: 15752 RVA: 0x00146369 File Offset: 0x00144569
	public void OnPlayerHit(GameEntityId hitByEntityId, GRPlayer player, Vector3 hitPosition)
	{
		this.ghostReactorManager.RequestEnemyHitPlayer(GhostReactor.EnemyType.CustomMapsEnemy, hitByEntityId, player, hitPosition);
	}

	// Token: 0x04004E1F RID: 19999
	public GameEntityManager gameEntityManager;

	// Token: 0x04004E20 RID: 20000
	public GameAgentManager gameAgentManager;

	// Token: 0x04004E21 RID: 20001
	public GhostReactorManager ghostReactorManager;

	// Token: 0x04004E22 RID: 20002
	public static CustomMapsGameManager instance;

	// Token: 0x04004E23 RID: 20003
	private const string AGENT_PREFAB_NAME = "CustomMapsAIAgent";

	// Token: 0x04004E24 RID: 20004
	private const string GRABBABLE_PREFAB_NAME = "CustomMapsGrabbableEntity";

	// Token: 0x04004E25 RID: 20005
	private Dictionary<int, AIAgent> customMapsAgents;

	// Token: 0x04004E26 RID: 20006
	private static List<GameEntityCreateData> tempCreateEntitiesList = new List<GameEntityCreateData>(128);

	// Token: 0x04004E27 RID: 20007
	private static List<MapEntity> agentsToCreateOnZoneInit = new List<MapEntity>(128);

	// Token: 0x04004E28 RID: 20008
	private int TEST_index;

	// Token: 0x04004E29 RID: 20009
	private int spawnCount;
}
