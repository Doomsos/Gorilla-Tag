using System;
using UnityEngine;

// Token: 0x02000CE7 RID: 3303
public class MaterialMapping : ScriptableObject
{
	// Token: 0x06005058 RID: 20568 RVA: 0x00002789 File Offset: 0x00000989
	public void CleanUpData()
	{
	}

	// Token: 0x04005F8D RID: 24461
	private static string path = "Assets/UberShaderConversion/MaterialMap.asset";

	// Token: 0x04005F8E RID: 24462
	public static string materialDirectory = "Assets/UberShaderConversion/Materials/";

	// Token: 0x04005F8F RID: 24463
	private static MaterialMapping instance;

	// Token: 0x04005F90 RID: 24464
	public ShaderGroup[] map;

	// Token: 0x04005F91 RID: 24465
	public Material mirrorMat;

	// Token: 0x04005F92 RID: 24466
	public RenderTexture mirrorTexture;
}
