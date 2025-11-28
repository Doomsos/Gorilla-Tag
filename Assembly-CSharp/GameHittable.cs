using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200061F RID: 1567
public class GameHittable : MonoBehaviour
{
	// Token: 0x060027D6 RID: 10198 RVA: 0x000D407A File Offset: 0x000D227A
	private void Awake()
	{
		this.components = new List<IGameHittable>(1);
		base.GetComponentsInChildren<IGameHittable>(this.components);
	}

	// Token: 0x060027D7 RID: 10199 RVA: 0x000D4094 File Offset: 0x000D2294
	public void RequestHit(GameHitData hitData)
	{
		hitData.hitEntityId = this.gameEntity.id;
		this.gameEntity.manager.RequestHit(hitData);
	}

	// Token: 0x060027D8 RID: 10200 RVA: 0x000D40BC File Offset: 0x000D22BC
	public void ApplyHit(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			this.components[i].OnHit(hitData);
		}
		GameHitter component = this.gameEntity.manager.GetGameEntity(hitData.hitByEntityId).GetComponent<GameHitter>();
		if (component != null)
		{
			component.ApplyHit(hitData);
		}
	}

	// Token: 0x060027D9 RID: 10201 RVA: 0x000D4120 File Offset: 0x000D2320
	public bool IsHitValid(GameHitData hitData)
	{
		for (int i = 0; i < this.components.Count; i++)
		{
			if (!this.components[i].IsHitValid(hitData))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400334E RID: 13134
	public GameEntity gameEntity;

	// Token: 0x0400334F RID: 13135
	private List<IGameHittable> components;
}
