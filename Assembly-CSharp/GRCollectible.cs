using System;
using UnityEngine;

// Token: 0x02000692 RID: 1682
public class GRCollectible : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x06002AEB RID: 10987 RVA: 0x00002789 File Offset: 0x00000989
	private void Awake()
	{
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x000E7000 File Offset: 0x000E5200
	public void OnEntityInit()
	{
		GameEntityManager manager = this.entity.manager;
		GameEntity gameEntity = manager.GetGameEntity(manager.GetEntityIdFromNetId((int)this.entity.createData));
		if (gameEntity != null)
		{
			GRCollectibleDispenser component = gameEntity.GetComponent<GRCollectibleDispenser>();
			if (component != null)
			{
				component.GetSpawnedCollectible(this);
			}
		}
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x000E7050 File Offset: 0x000E5250
	public void InvokeOnCollected()
	{
		Action onCollected = this.OnCollected;
		if (onCollected == null)
		{
			return;
		}
		onCollected.Invoke();
	}

	// Token: 0x04003767 RID: 14183
	public GameEntity entity;

	// Token: 0x04003768 RID: 14184
	public int energyValue = 100;

	// Token: 0x04003769 RID: 14185
	public ProgressionManager.CoreType type;

	// Token: 0x0400376A RID: 14186
	public Action OnCollected;
}
