using System;
using System.Collections.Generic;

// Token: 0x02000050 RID: 80
public static class CrittersBiomeExtensions
{
	// Token: 0x0600017E RID: 382 RVA: 0x00009BBC File Offset: 0x00007DBC
	static CrittersBiomeExtensions()
	{
		CrittersBiomeExtensions._allScannableBiomes = new List<CrittersBiome>();
		foreach (object obj in Enum.GetValues(typeof(CrittersBiome)))
		{
			CrittersBiome crittersBiome = (CrittersBiome)obj;
			if (crittersBiome != CrittersBiome.Any && crittersBiome != CrittersBiome.IntroArea)
			{
				CrittersBiomeExtensions._allScannableBiomes.Add(crittersBiome);
			}
		}
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00009C44 File Offset: 0x00007E44
	public static string GetHabitatDescription(this CrittersBiome biome)
	{
		string text;
		if (!CrittersBiomeExtensions._habitatLookup.TryGetValue(biome, ref text))
		{
			if (biome == CrittersBiome.Any)
			{
				text = "Any";
			}
			else
			{
				if (CrittersBiomeExtensions._habitatBiomes == null)
				{
					CrittersBiomeExtensions._habitatBiomes = new List<CrittersBiome>();
				}
				CrittersBiomeExtensions._habitatBiomes.Clear();
				for (int i = 0; i < CrittersBiomeExtensions._allScannableBiomes.Count; i++)
				{
					if (biome.HasFlag(CrittersBiomeExtensions._allScannableBiomes[i]))
					{
						CrittersBiomeExtensions._habitatBiomes.Add(CrittersBiomeExtensions._allScannableBiomes[i]);
					}
				}
			}
			text = ((CrittersBiomeExtensions._habitatBiomes.Count > 3) ? "Various" : string.Join<CrittersBiome>(", ", CrittersBiomeExtensions._habitatBiomes));
			CrittersBiomeExtensions._habitatLookup[biome] = text;
		}
		return text;
	}

	// Token: 0x040001BA RID: 442
	private static List<CrittersBiome> _allScannableBiomes;

	// Token: 0x040001BB RID: 443
	private static Dictionary<CrittersBiome, string> _habitatLookup = new Dictionary<CrittersBiome, string>();

	// Token: 0x040001BC RID: 444
	private static List<CrittersBiome> _habitatBiomes;
}
