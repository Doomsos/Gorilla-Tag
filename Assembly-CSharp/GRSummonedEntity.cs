using System;
using UnityEngine;

// Token: 0x0200070D RID: 1805
public class GRSummonedEntity : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002E57 RID: 11863 RVA: 0x000FBECD File Offset: 0x000FA0CD
	private void Awake()
	{
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x000FBEDC File Offset: 0x000FA0DC
	public void OnEntityInit()
	{
		this.summonerNetID = (int)this.entity.createData;
		if (this.summonerNetID != 0)
		{
			this.summoner = this.FindSummoner();
			if (this.summoner != null)
			{
				this.summoner.OnSummonedEntityInit(this.entity);
			}
		}
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x000FBF28 File Offset: 0x000FA128
	public int GetSummonerNetID()
	{
		return this.summonerNetID;
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x000FBF30 File Offset: 0x000FA130
	public void OnEntityDestroy()
	{
		if (this.summoner != null)
		{
			this.summoner.OnSummonedEntityDestroy(this.entity);
		}
	}

	// Token: 0x06002E5B RID: 11867 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002E5C RID: 11868 RVA: 0x000FBF4C File Offset: 0x000FA14C
	private IGRSummoningEntity FindSummoner()
	{
		if (this.summonerNetID != 0)
		{
			GameEntityManager gameEntityManager = GhostReactorManager.Get(this.entity).gameEntityManager;
			GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(this.summonerNetID);
			GameEntity gameEntity = gameEntityManager.GetGameEntity(entityIdFromNetId);
			if (gameEntity != null)
			{
				return gameEntity.GetComponent<IGRSummoningEntity>();
			}
		}
		return null;
	}

	// Token: 0x04003C7B RID: 15483
	private int summonerNetID;

	// Token: 0x04003C7C RID: 15484
	private GameEntity entity;

	// Token: 0x04003C7D RID: 15485
	private IGRSummoningEntity summoner;
}
