using System;
using UnityEngine;

// Token: 0x0200003C RID: 60
[Serializable]
public class CritterConfiguration
{
	// Token: 0x060000E5 RID: 229 RVA: 0x00005F88 File Offset: 0x00004188
	public CritterConfiguration()
	{
		this.animalType = CritterConfiguration.AnimalType.UNKNOWN;
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00005FB4 File Offset: 0x000041B4
	public int GetIndex()
	{
		return CrittersManager.instance.creatureIndex.critterTypes.IndexOf(this);
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00005FCD File Offset: 0x000041CD
	private bool RegionMatches(CrittersRegion region)
	{
		return !region || (region.Biome & this.biome) > (CrittersBiome)0;
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00005FE9 File Offset: 0x000041E9
	private bool SpawnCriteriaMatches()
	{
		return !this.spawnCriteria || this.spawnCriteria.CanSpawn();
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x00006005 File Offset: 0x00004205
	public bool CanSpawn()
	{
		return this.SpawnCriteriaMatches();
	}

	// Token: 0x060000EA RID: 234 RVA: 0x0000600D File Offset: 0x0000420D
	public bool CanSpawn(CrittersRegion region)
	{
		return this.RegionMatches(region) && this.SpawnCriteriaMatches();
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00006020 File Offset: 0x00004220
	public bool DateConditionsMet(DateTime utcDate)
	{
		return !this.dateLimit || this.dateLimit.MatchesDate(utcDate);
	}

	// Token: 0x060000EC RID: 236 RVA: 0x0000603D File Offset: 0x0000423D
	public bool ShouldDespawn()
	{
		return !this.SpawnCriteriaMatches();
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00006048 File Offset: 0x00004248
	public void ApplyToCreature(CrittersPawn crittersPawn)
	{
		this.behaviour.ApplyToCritter(crittersPawn);
		if (CrittersManager.instance.LocalAuthority())
		{
			this.ApplyVisualsTo(crittersPawn, true);
			return;
		}
		this.ApplyVisualsTo(crittersPawn, false);
	}

	// Token: 0x060000EE RID: 238 RVA: 0x00006075 File Offset: 0x00004275
	private void ApplyVisualsTo(CrittersPawn critter, bool generateAppearance = true)
	{
		this.ApplyVisualsTo(critter.visuals, generateAppearance);
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00006084 File Offset: 0x00004284
	public void ApplyVisualsTo(CritterVisuals visuals, bool generateAppearance = true)
	{
		visuals.critterType = this.GetIndex();
		visuals.ApplyMesh(CritterIndex.GetMesh(this.animalType));
		visuals.ApplyMaterial(this.critterMat);
		if (generateAppearance)
		{
			visuals.SetAppearance(this.GenerateAppearance());
		}
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x000060C0 File Offset: 0x000042C0
	public CritterAppearance GenerateAppearance()
	{
		string hatName = "";
		if (Random.value <= this.behaviour.GetTemplateValue<float>("hatChance"))
		{
			GameObject[] templateValue = this.behaviour.GetTemplateValue<GameObject[]>("hats");
			if (!templateValue.IsNullOrEmpty<GameObject>())
			{
				hatName = templateValue[Random.Range(0, templateValue.Length)].name;
			}
		}
		float templateValue2 = this.behaviour.GetTemplateValue<float>("minSize");
		float templateValue3 = this.behaviour.GetTemplateValue<float>("maxSize");
		float size = Random.Range(templateValue2, templateValue3);
		return new CritterAppearance(hatName, size);
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x00006148 File Offset: 0x00004348
	public override string ToString()
	{
		return string.Format("{0} B:{1} C:{2}", this.critterName, this.behaviour, this.spawnCriteria);
	}

	// Token: 0x040000F1 RID: 241
	[Tooltip("Basic internal description of critter.  Could be role, purpose, player experience, etc.")]
	public string internalDescription;

	// Token: 0x040000F2 RID: 242
	public string critterName = "UNNAMED CRITTER";

	// Token: 0x040000F3 RID: 243
	public CritterConfiguration.AnimalType animalType;

	// Token: 0x040000F4 RID: 244
	public CritterTemplate behaviour;

	// Token: 0x040000F5 RID: 245
	public CritterSpawnCriteria spawnCriteria;

	// Token: 0x040000F6 RID: 246
	public RealWorldDateTimeWindow dateLimit;

	// Token: 0x040000F7 RID: 247
	public CrittersBiome biome = CrittersBiome.Any;

	// Token: 0x040000F8 RID: 248
	public float spawnWeight = 1f;

	// Token: 0x040000F9 RID: 249
	public Material critterMat;

	// Token: 0x0200003D RID: 61
	public enum AnimalType
	{
		// Token: 0x040000FB RID: 251
		Raccoon,
		// Token: 0x040000FC RID: 252
		Cat,
		// Token: 0x040000FD RID: 253
		Bird,
		// Token: 0x040000FE RID: 254
		Goblin,
		// Token: 0x040000FF RID: 255
		Egg,
		// Token: 0x04000100 RID: 256
		UNKNOWN = -1
	}
}
