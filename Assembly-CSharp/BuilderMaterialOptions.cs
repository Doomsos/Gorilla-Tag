using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200057A RID: 1402
[CreateAssetMenu(fileName = "BuilderMaterialOptions01a", menuName = "Gorilla Tag/Builder/Options", order = 0)]
public class BuilderMaterialOptions : ScriptableObject
{
	// Token: 0x06002361 RID: 9057 RVA: 0x000B988C File Offset: 0x000B7A8C
	public void GetMaterialFromType(int materialType, out Material material, out int soundIndex)
	{
		if (this.options == null)
		{
			material = null;
			soundIndex = -1;
			return;
		}
		foreach (BuilderMaterialOptions.Options options in this.options)
		{
			if (options.materialId.GetHashCode() == materialType)
			{
				material = options.material;
				soundIndex = options.soundIndex;
				return;
			}
		}
		material = null;
		soundIndex = -1;
	}

	// Token: 0x06002362 RID: 9058 RVA: 0x000B9910 File Offset: 0x000B7B10
	public void GetDefaultMaterial(out int materialType, out Material material, out int soundIndex)
	{
		if (this.options.Count > 0)
		{
			materialType = this.options[0].materialId.GetHashCode();
			material = this.options[0].material;
			soundIndex = this.options[0].soundIndex;
			return;
		}
		materialType = -1;
		material = null;
		soundIndex = -1;
	}

	// Token: 0x04002E3A RID: 11834
	public List<BuilderMaterialOptions.Options> options;

	// Token: 0x0200057B RID: 1403
	[Serializable]
	public class Options
	{
		// Token: 0x04002E3B RID: 11835
		public string materialId;

		// Token: 0x04002E3C RID: 11836
		public Material material;

		// Token: 0x04002E3D RID: 11837
		[GorillaSoundLookup]
		public int soundIndex;

		// Token: 0x04002E3E RID: 11838
		[NonSerialized]
		public int materialType;
	}
}
