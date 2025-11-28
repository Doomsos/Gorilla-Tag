using System;
using System.Collections.Generic;
using System.IO;
using GorillaTagScripts.GhostReactor;

// Token: 0x02000704 RID: 1796
public class GRShiftStat
{
	// Token: 0x17000432 RID: 1074
	// (get) Token: 0x06002E0B RID: 11787 RVA: 0x000FA758 File Offset: 0x000F8958
	public IReadOnlyDictionary<GREnemyType, int> EnemyKills
	{
		get
		{
			return this.enemyKills;
		}
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x000FA760 File Offset: 0x000F8960
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

	// Token: 0x06002E0D RID: 11789 RVA: 0x000FA80C File Offset: 0x000F8A0C
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

	// Token: 0x06002E0E RID: 11790 RVA: 0x000FA88D File Offset: 0x000F8A8D
	public void SetShiftStat(GRShiftStatType stat, int newValue)
	{
		this.shiftStats[stat] = newValue;
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000FA8AC File Offset: 0x000F8AAC
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

	// Token: 0x06002E10 RID: 11792 RVA: 0x000FA900 File Offset: 0x000F8B00
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

	// Token: 0x06002E11 RID: 11793 RVA: 0x000FA944 File Offset: 0x000F8B44
	public void ResetShiftStats()
	{
		this.shiftStats[GRShiftStatType.EnemyDeaths] = 0;
		this.shiftStats[GRShiftStatType.PlayerDeaths] = 0;
		this.shiftStats[GRShiftStatType.CoresCollected] = 0;
		this.shiftStats[GRShiftStatType.SentientCoresCollected] = 0;
		this.enemyKills.Clear();
		GhostReactor.instance.shiftManager.RefreshDepthDisplay();
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x000FA99F File Offset: 0x000F8B9F
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
