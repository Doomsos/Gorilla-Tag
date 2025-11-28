using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000564 RID: 1380
[CreateAssetMenu(fileName = "BuilderMaterialResourceColors", menuName = "Gorilla Tag/Builder/ResourceColors", order = 0)]
public class BuilderResourceColors : ScriptableObject
{
	// Token: 0x060022CA RID: 8906 RVA: 0x000B5D44 File Offset: 0x000B3F44
	public Color GetColorForType(BuilderResourceType type)
	{
		foreach (BuilderResourceColor builderResourceColor in this.colors)
		{
			if (builderResourceColor.type == type)
			{
				return builderResourceColor.color;
			}
		}
		return Color.black;
	}

	// Token: 0x04002D71 RID: 11633
	public List<BuilderResourceColor> colors;
}
