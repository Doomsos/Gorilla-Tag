using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000699 RID: 1689
public class GRDebugUpgradeKiosk : MonoBehaviour
{
	// Token: 0x06002B0F RID: 11023 RVA: 0x000E769F File Offset: 0x000E589F
	public void Init(GhostReactorManager grManager, GhostReactor reactor)
	{
		this.grManager = grManager;
		this.reactor = reactor;
	}

	// Token: 0x06002B10 RID: 11024 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x000E76AF File Offset: 0x000E58AF
	public void OnButtonSpawnClub()
	{
		this.OnButtonSpawnEntity("GhostReactorToolClub", this.toolSpawnNode);
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x000E76C2 File Offset: 0x000E58C2
	public void OnButtonSpawnCollector()
	{
		this.OnButtonSpawnEntity("GhostReactorToolCollector", this.toolSpawnNode);
	}

	// Token: 0x06002B13 RID: 11027 RVA: 0x000E76D5 File Offset: 0x000E58D5
	public void OnButtonSpawnLantern()
	{
		this.OnButtonSpawnEntity("GhostReactorToolLantern", this.toolSpawnNode);
	}

	// Token: 0x06002B14 RID: 11028 RVA: 0x000E76E8 File Offset: 0x000E58E8
	public void OnButtonSpawnFlash()
	{
		this.OnButtonSpawnEntity("GhostReactorToolFlash", this.toolSpawnNode);
	}

	// Token: 0x06002B15 RID: 11029 RVA: 0x000E76FB File Offset: 0x000E58FB
	public void OnButtonSpawnShieldGun()
	{
		this.OnButtonSpawnEntity("GhostReactorToolShieldGun", this.toolSpawnNode);
	}

	// Token: 0x06002B16 RID: 11030 RVA: 0x000E770E File Offset: 0x000E590E
	public void OnButtonSpawnRevive()
	{
		this.OnButtonSpawnEntity("GhostReactorToolRevive", this.toolSpawnNode);
	}

	// Token: 0x06002B17 RID: 11031 RVA: 0x000E7721 File Offset: 0x000E5921
	public void OnButtonSpawnDirectionalShield()
	{
		this.OnButtonSpawnEntity("GhostReactorToolDirectionalShield", this.toolSpawnNode);
	}

	// Token: 0x06002B18 RID: 11032 RVA: 0x000E7734 File Offset: 0x000E5934
	public void OnButtonSpawnStatusWatch()
	{
		this.OnButtonSpawnEntity("GhostReactorToolStatusWatch", this.toolSpawnNode);
	}

	// Token: 0x06002B19 RID: 11033 RVA: 0x000E7747 File Offset: 0x000E5947
	public void OnButtonSpawnDockWrist()
	{
		this.OnButtonSpawnEntity("GhostReactorToolDockWrist", this.toolSpawnNode);
	}

	// Token: 0x06002B1A RID: 11034 RVA: 0x000E775A File Offset: 0x000E595A
	public void OnButtonSpawnSmallBackpack()
	{
		this.OnButtonSpawnEntity("GhostReactorToolSmallBackpack", this.toolSpawnNode);
	}

	// Token: 0x06002B1B RID: 11035 RVA: 0x000E776D File Offset: 0x000E596D
	public void OnButtonKillAllEnemies()
	{
		this.KillAllEnemies();
	}

	// Token: 0x06002B1C RID: 11036 RVA: 0x000E7775 File Offset: 0x000E5975
	public void OnButtonSpawnPest()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyPest", this.enemySpawnNode);
	}

	// Token: 0x06002B1D RID: 11037 RVA: 0x000E7788 File Offset: 0x000E5988
	public void OnButtonSpawnChaser()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyChaser", this.enemySpawnNode);
	}

	// Token: 0x06002B1E RID: 11038 RVA: 0x000E779B File Offset: 0x000E599B
	public void OnButtonSpawnPhantom()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyPhantom", this.enemySpawnNode);
	}

	// Token: 0x06002B1F RID: 11039 RVA: 0x000E77AE File Offset: 0x000E59AE
	public void OnButtonSpawnRanged()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyRanged", this.enemySpawnNode);
	}

	// Token: 0x06002B20 RID: 11040 RVA: 0x000E77C1 File Offset: 0x000E59C1
	public void OnButtonSpawnSummoner()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemySummoner", this.enemySpawnNode);
	}

	// Token: 0x06002B21 RID: 11041 RVA: 0x000E77D4 File Offset: 0x000E59D4
	public void OnButtonSpawnIceRanged()
	{
		this.OnButtonSpawnEntity("GhostReactorEnemyRangedIce", this.enemySpawnNode);
	}

	// Token: 0x06002B22 RID: 11042 RVA: 0x000E77E7 File Offset: 0x000E59E7
	public void OnButtonSpawnUpgEff1()
	{
		this.OnButtonSpawnEntity("GRUPowerEff1", this.upgradeSpawnNode);
	}

	// Token: 0x06002B23 RID: 11043 RVA: 0x000E77FA File Offset: 0x000E59FA
	public void OnButtonSpawnUpgEff2()
	{
		this.OnButtonSpawnEntity("GRUPowerEff2", this.upgradeSpawnNode);
	}

	// Token: 0x06002B24 RID: 11044 RVA: 0x000E780D File Offset: 0x000E5A0D
	public void OnButtonSpawnUpgEff3()
	{
		this.OnButtonSpawnEntity("GRUPowerEff3", this.upgradeSpawnNode);
	}

	// Token: 0x06002B25 RID: 11045 RVA: 0x000E7820 File Offset: 0x000E5A20
	public void OnButtonSpawnUpgBatonDmg1()
	{
		this.OnButtonSpawnEntity("GRUBatonDamage1", this.upgradeSpawnNode);
	}

	// Token: 0x06002B26 RID: 11046 RVA: 0x000E7833 File Offset: 0x000E5A33
	public void OnButtonSpawnUpgBatonDmg2()
	{
		this.OnButtonSpawnEntity("GRUBatonDamage2", this.upgradeSpawnNode);
	}

	// Token: 0x06002B27 RID: 11047 RVA: 0x000E7846 File Offset: 0x000E5A46
	public void OnButtonSpawnUpgBatonDmg3()
	{
		this.OnButtonSpawnEntity("GRUBatonDamage3", this.upgradeSpawnNode);
	}

	// Token: 0x06002B28 RID: 11048 RVA: 0x000E77E7 File Offset: 0x000E59E7
	public void OnButtonSpawnUpgEfficiency1()
	{
		this.OnButtonSpawnEntity("GRUPowerEff1", this.upgradeSpawnNode);
	}

	// Token: 0x06002B29 RID: 11049 RVA: 0x000E77FA File Offset: 0x000E59FA
	public void OnButtonSpawnUpgEfficiency2()
	{
		this.OnButtonSpawnEntity("GRUPowerEff2", this.upgradeSpawnNode);
	}

	// Token: 0x06002B2A RID: 11050 RVA: 0x000E780D File Offset: 0x000E5A0D
	public void OnButtonSpawnUpgEfficiency3()
	{
		this.OnButtonSpawnEntity("GRUPowerEff3", this.upgradeSpawnNode);
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x000E7859 File Offset: 0x000E5A59
	public void OnButtonSpawnChaosSeed()
	{
		this.OnButtonSpawnEntity("GhostReactorCollectibleSentientCore", this.enemySpawnNode);
	}

	// Token: 0x06002B2C RID: 11052 RVA: 0x000E786C File Offset: 0x000E5A6C
	public void OnButtonSpawnEntity(string entityName, Transform location)
	{
		if (location == null)
		{
			return;
		}
		Debug.Log("GRDebugUpgradeKiosk attempting to spawn " + entityName);
		int staticHash = entityName.GetStaticHash();
		GameEntityId gameEntityId = this.grManager.gameEntityManager.RequestCreateItem(staticHash, location.position, Quaternion.identity, 0L);
		GameAgent component = this.grManager.gameEntityManager.GetGameEntity(gameEntityId).gameObject.GetComponent<GameAgent>();
		if (component != null)
		{
			if (entityName.Contains("enemy", 5))
			{
				GhostReactorManager.entityDebugEnabled = true;
			}
			this.spawnedEntities.Add(gameEntityId);
			component.ApplyDestination(location.position);
			return;
		}
		Debug.Log("GRDebugUpgradeKiosk failed to spawn " + entityName);
	}

	// Token: 0x06002B2D RID: 11053 RVA: 0x000E791C File Offset: 0x000E5B1C
	public void KillAllEnemies()
	{
		foreach (GameEntityId entityId in this.spawnedEntities)
		{
			this.grManager.gameEntityManager.RequestDestroyItem(entityId);
		}
		this.spawnedEntities.Clear();
	}

	// Token: 0x0400378C RID: 14220
	public Transform upgradeSpawnNode;

	// Token: 0x0400378D RID: 14221
	public Transform toolSpawnNode;

	// Token: 0x0400378E RID: 14222
	public Transform enemySpawnNode;

	// Token: 0x0400378F RID: 14223
	private GhostReactorManager grManager;

	// Token: 0x04003790 RID: 14224
	private GhostReactor reactor;

	// Token: 0x04003791 RID: 14225
	private List<GameEntityId> spawnedEntities = new List<GameEntityId>();
}
