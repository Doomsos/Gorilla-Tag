using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000690 RID: 1680
[CreateAssetMenu(fileName = "GhostReactorBreakableItemSpawnConfig", menuName = "ScriptableObjects/GhostReactorBreakableItemSpawnConfig")]
public class GRBreakableItemSpawnConfig : ScriptableObject
{
	// Token: 0x06002AE5 RID: 10981 RVA: 0x000E6DB8 File Offset: 0x000E4FB8
	public bool TryForRandomItem(GameEntity spawnFromEntity, out GameEntity entity, int sanity = 0)
	{
		GRBreakableItemSpawnConfig @override = this.GetOverride(spawnFromEntity);
		if (sanity <= 5 && @override != null)
		{
			return @override.TryForRandomItem(spawnFromEntity, out entity, sanity + 1);
		}
		if (sanity > 5)
		{
			Debug.LogError("Circular override loop");
		}
		if (Random.Range(0f, 1f) < this.spawnAnythingProbability)
		{
			float num = Random.Range(0f, this.precomputedItemTotalWeight);
			float num2 = 0f;
			for (int i = 0; i < this.perItemProbabilities.Count; i++)
			{
				num2 += this.perItemProbabilities[i].probability;
				if (num2 > num || i == this.perItemProbabilities.Count - 1)
				{
					entity = this.perItemProbabilities[i].entity;
					return true;
				}
			}
		}
		entity = null;
		return false;
	}

	// Token: 0x06002AE6 RID: 10982 RVA: 0x000E6E7C File Offset: 0x000E507C
	public bool TryForRandomItem(GhostReactor reactor, ref SRand srand, out GameEntity entity, int sanity = 0)
	{
		GRBreakableItemSpawnConfig @override = this.GetOverride(reactor);
		if (sanity <= 5 && @override != null)
		{
			return @override.TryForRandomItem(reactor, ref srand, out entity, sanity + 1);
		}
		if (sanity > 5)
		{
			Debug.LogError("Circular override loop");
		}
		if (srand.NextFloat(0f, 1f) < this.spawnAnythingProbability)
		{
			float num = srand.NextFloat(0f, this.precomputedItemTotalWeight);
			float num2 = 0f;
			for (int i = 0; i < this.perItemProbabilities.Count; i++)
			{
				num2 += this.perItemProbabilities[i].probability;
				if (num2 > num || i == this.perItemProbabilities.Count - 1)
				{
					entity = this.perItemProbabilities[i].entity;
					return true;
				}
			}
		}
		entity = null;
		return false;
	}

	// Token: 0x06002AE7 RID: 10983 RVA: 0x000E6F44 File Offset: 0x000E5144
	private GRBreakableItemSpawnConfig GetOverride(GameEntity entity)
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(entity);
		if (ghostReactorManager == null)
		{
			return null;
		}
		return this.GetOverride(ghostReactorManager.reactor);
	}

	// Token: 0x06002AE8 RID: 10984 RVA: 0x000E6F70 File Offset: 0x000E5170
	private GRBreakableItemSpawnConfig GetOverride(GhostReactor reactor)
	{
		if (reactor == null)
		{
			return null;
		}
		GhostReactorLevelGenConfig currLevelGenConfig = reactor.GetCurrLevelGenConfig();
		if (currLevelGenConfig == null || currLevelGenConfig.dropTableOverrides == null)
		{
			return null;
		}
		return currLevelGenConfig.dropTableOverrides.GetOverride(this);
	}

	// Token: 0x06002AE9 RID: 10985 RVA: 0x000E6FB4 File Offset: 0x000E51B4
	private void OnValidate()
	{
		this.precomputedItemTotalWeight = 0f;
		for (int i = 0; i < this.perItemProbabilities.Count; i++)
		{
			this.precomputedItemTotalWeight += this.perItemProbabilities[i].probability;
		}
	}

	// Token: 0x04003762 RID: 14178
	[SerializeField]
	[Range(0f, 1f)]
	public float spawnAnythingProbability = 0.2f;

	// Token: 0x04003763 RID: 14179
	public List<GRBreakableItemSpawnConfig.ItemProbability> perItemProbabilities = new List<GRBreakableItemSpawnConfig.ItemProbability>();

	// Token: 0x04003764 RID: 14180
	[SerializeField]
	[ReadOnly]
	private float precomputedItemTotalWeight;

	// Token: 0x02000691 RID: 1681
	[Serializable]
	public struct ItemProbability
	{
		// Token: 0x04003765 RID: 14181
		public GameEntity entity;

		// Token: 0x04003766 RID: 14182
		public float probability;
	}
}
