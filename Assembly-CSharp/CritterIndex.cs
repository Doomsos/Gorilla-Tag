using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class CritterIndex : ScriptableObject
{
	// Token: 0x17000017 RID: 23
	public CritterConfiguration this[int index]
	{
		get
		{
			if (index < 0 || index >= this.critterTypes.Count)
			{
				return null;
			}
			return this.critterTypes[index];
		}
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00006188 File Offset: 0x00004388
	private void OnEnable()
	{
		CritterIndex._instance = this;
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00006190 File Offset: 0x00004390
	public static Mesh GetMesh(CritterConfiguration.AnimalType animalType)
	{
		if (animalType < CritterConfiguration.AnimalType.Raccoon || animalType >= (CritterConfiguration.AnimalType)CritterIndex._instance.animalMeshes.Count)
		{
			return null;
		}
		return CritterIndex._instance.animalMeshes[(int)animalType].mesh;
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x000061BF File Offset: 0x000043BF
	public int GetRandomCritterType(CrittersRegion region = null)
	{
		return this.critterTypes.IndexOf(this.GetRandomConfiguration(region));
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x000061D4 File Offset: 0x000043D4
	public CritterConfiguration GetRandomConfiguration(CrittersRegion region = null)
	{
		WeightedList<CritterConfiguration> validCritterTypes = this.GetValidCritterTypes(region);
		if (validCritterTypes.Count == 0)
		{
			return null;
		}
		return validCritterTypes.GetRandomItem();
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x000061F9 File Offset: 0x000043F9
	public static DateTime GetCritterDateTime()
	{
		if (!GorillaComputer.instance)
		{
			return DateTime.UtcNow;
		}
		return GorillaComputer.instance.GetServerTime();
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x0000621C File Offset: 0x0000441C
	private WeightedList<CritterConfiguration> GetValidCritterTypes(CrittersRegion region = null)
	{
		this._currentConfigs.Clear();
		DateTime critterDateTime = CritterIndex.GetCritterDateTime();
		foreach (CritterConfiguration critterConfiguration in this.critterTypes)
		{
			if (critterConfiguration.DateConditionsMet(critterDateTime) && critterConfiguration.CanSpawn(region))
			{
				this._currentConfigs.Add(critterConfiguration, critterConfiguration.spawnWeight);
			}
		}
		return this._currentConfigs;
	}

	// Token: 0x04000101 RID: 257
	public List<CritterIndex.AnimalTypeMeshEntry> animalMeshes = new List<CritterIndex.AnimalTypeMeshEntry>();

	// Token: 0x04000102 RID: 258
	public List<CritterConfiguration> critterTypes;

	// Token: 0x04000103 RID: 259
	private WeightedList<CritterConfiguration> _currentConfigs = new WeightedList<CritterConfiguration>();

	// Token: 0x04000104 RID: 260
	private static CritterIndex _instance;

	// Token: 0x0200003F RID: 63
	[Serializable]
	public class AnimalTypeMeshEntry
	{
		// Token: 0x04000105 RID: 261
		public CritterConfiguration.AnimalType animalType;

		// Token: 0x04000106 RID: 262
		public Mesh mesh;
	}
}
