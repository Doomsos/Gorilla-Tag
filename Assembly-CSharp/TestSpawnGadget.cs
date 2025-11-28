using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class TestSpawnGadget : MonoBehaviour
{
	// Token: 0x0600097B RID: 2427 RVA: 0x0003322C File Offset: 0x0003142C
	public void Spawn(GameEntityManager gameEntityManager)
	{
		SIUpgradeSet upgrades = default(SIUpgradeSet);
		foreach (TestSpawnGadget.SpawnTypeWithUpgrades spawnTypeWithUpgrades in this.testSpawnList)
		{
			if (!(spawnTypeWithUpgrades.prefab == null))
			{
				upgrades.Clear();
				foreach (SIUpgradeType upgrade in spawnTypeWithUpgrades.upgrades)
				{
					upgrades.Add(upgrade);
				}
				this.SpawnGadgetBatch(gameEntityManager, spawnTypeWithUpgrades.prefab, upgrades);
			}
		}
		if (!this.spawnAllGadgets)
		{
			return;
		}
		upgrades.Clear();
		foreach (GameEntity gameEntity in gameEntityManager.tempFactoryItems)
		{
			if (!this.skipEntityList.Contains(gameEntity))
			{
				this.SpawnGadgetBatch(gameEntityManager, gameEntity, upgrades);
			}
		}
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x00033334 File Offset: 0x00031534
	private void SpawnGadgetBatch(GameEntityManager gameEntityManager, GameEntity entityToSpawn, SIUpgradeSet upgrades)
	{
		for (int i = 0; i < this.spawnBatchSize; i++)
		{
			gameEntityManager.RequestCreateItem(entityToSpawn.gameObject.name.GetStaticHash(), base.transform.position + Random.insideUnitSphere, base.transform.rotation, (long)upgrades.GetBits() << 32);
		}
	}

	// Token: 0x04000B8F RID: 2959
	public int spawnBatchSize = 4;

	// Token: 0x04000B90 RID: 2960
	public List<TestSpawnGadget.SpawnTypeWithUpgrades> testSpawnList = new List<TestSpawnGadget.SpawnTypeWithUpgrades>();

	// Token: 0x04000B91 RID: 2961
	public bool spawnAllGadgets;

	// Token: 0x04000B92 RID: 2962
	public List<GameEntity> skipEntityList = new List<GameEntity>();

	// Token: 0x02000161 RID: 353
	[Serializable]
	public struct SpawnTypeWithUpgrades
	{
		// Token: 0x04000B93 RID: 2963
		public GameEntity prefab;

		// Token: 0x04000B94 RID: 2964
		public SIUpgradeType[] upgrades;
	}
}
