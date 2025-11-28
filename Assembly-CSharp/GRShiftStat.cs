using System;
using System.Collections.Generic;
using System.IO;
using GorillaTagScripts.GhostReactor;

// Token: 0x02000704 RID: 1796
public class GRShiftStat
{
	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x06002E0B RID: 11787 RVA: 0x000FA738 File Offset: 0x000F8938
	public IReadOnlyDictionary<GREnemyType, int> EnemyKills
	{
		get
		{
			return this.enemyKills;
		}
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x000FA740 File Offset: 0x000F8940
	public void Serialize(BinaryWriter writer)
	{
		writer.Write(this.GetShiftStat(GRShiftStatType.EnemyDeaths));
		writer.Write(this.GetShiftStat(GRShiftStatType.PlayerDeaths));
		writer.Write(this.GetShiftStat(GRShiftStatType.CoresCollected));
		writer.Write(this.GetShiftStat(GRShiftStatType.SentientCoresCollected));
		writer.Write(this.enemyKills.Count);
		foreach (KeyValuePair<GREnemyType, int> keyValuePair in this.enemyKills)
		{
			writer.Write((int)keyValuePair.Key);
			writer.Write(keyValuePair.Value);
		}
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x000FA7EC File Offset: 0x000F89EC
	public void Deserialize(BinaryReader reader)
	{
		this.shiftStats[GRShiftStatType.EnemyDeaths] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.PlayerDeaths] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.CoresCollected] = reader.ReadInt32();
		this.shiftStats[GRShiftStatType.SentientCoresCollected] = reader.ReadInt32();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			GREnemyType grenemyType = (GREnemyType)reader.ReadInt32();
			this.enemyKills[grenemyType] = reader.ReadInt32();
		}
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x000FA86D File Offset: 0x000F8A6D
	public void SetShiftStat(GRShiftStatType stat, int newValue)
	{
		this.shiftStats[stat] = newValue;
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000FA88C File Offset: 0x000F8A8C
	public void IncrementShiftStat(GRShiftStatType stat)
	{
		if (this.shiftStats.ContainsKey(stat))
		{
			Dictionary<GRShiftStatType, int> dictionary = this.shiftStats;
			int num = dictionary[stat];
			dictionary[stat] = num + 1;
			return;
		}
		this.shiftStats[stat] = 1;
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x000FA8E0 File Offset: 0x000F8AE0
	public void IncrementEnemyKills(GREnemyType type)
	{
		if (!this.enemyKills.TryAdd(type, 1))
		{
			Dictionary<GREnemyType, int> dictionary = this.enemyKills;
			int num = dictionary[type];
			dictionary[type] = num + 1;
		}
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x000FA924 File Offset: 0x000F8B24
	public void ResetShiftStats()
	{
		this.shiftStats[GRShiftStatType.EnemyDeaths] = 0;
		this.shiftStats[GRShiftStatType.PlayerDeaths] = 0;
		this.shiftStats[GRShiftStatType.CoresCollected] = 0;
		this.shiftStats[GRShiftStatType.SentientCoresCollected] = 0;
		this.enemyKills.Clear();
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x000FA97F File Offset: 0x000F8B7F
	public int GetShiftStat(GRShiftStatType stat)
	{
		if (this.shiftStats.ContainsKey(stat))
		{
			return this.shiftStats[stat];
		}
		return 0;
	}

	// Token: 0x04003C33 RID: 15411
	public Dictionary<GRShiftStatType, int> shiftStats = new Dictionary<GRShiftStatType, int>();

	// Token: 0x04003C34 RID: 15412
	private Dictionary<GREnemyType, int> enemyKills = new Dictionary<GREnemyType, int>();
}
