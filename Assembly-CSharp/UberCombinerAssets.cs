using System;
using UnityEngine;

// Token: 0x02000CD9 RID: 3289
public class UberCombinerAssets : ScriptableObject
{
	// Token: 0x17000775 RID: 1909
	// (get) Token: 0x06005047 RID: 20551 RVA: 0x0019CB77 File Offset: 0x0019AD77
	public static UberCombinerAssets Instance
	{
		get
		{
			UberCombinerAssets.gInstance == null;
			return UberCombinerAssets.gInstance;
		}
	}

	// Token: 0x06005048 RID: 20552 RVA: 0x0019CB8A File Offset: 0x0019AD8A
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x06005049 RID: 20553 RVA: 0x00002789 File Offset: 0x00000989
	private void Setup()
	{
	}

	// Token: 0x0600504A RID: 20554 RVA: 0x00002789 File Offset: 0x00000989
	public void ClearMaterialAssets()
	{
	}

	// Token: 0x0600504B RID: 20555 RVA: 0x00002789 File Offset: 0x00000989
	public void ClearPrefabAssets()
	{
	}

	// Token: 0x04005EE4 RID: 24292
	[SerializeField]
	private Object _rootFolder;

	// Token: 0x04005EE5 RID: 24293
	[SerializeField]
	private Object _resourcesFolder;

	// Token: 0x04005EE6 RID: 24294
	[SerializeField]
	private Object _materialsFolder;

	// Token: 0x04005EE7 RID: 24295
	[SerializeField]
	private Object _prefabsFolder;

	// Token: 0x04005EE8 RID: 24296
	[Space]
	public Object MeshBakerDefaultCustomizer;

	// Token: 0x04005EE9 RID: 24297
	public Material ReferenceUberMaterial;

	// Token: 0x04005EEA RID: 24298
	public Shader TextureArrayCapableShader;

	// Token: 0x04005EEB RID: 24299
	[Space]
	public string RootFolderPath;

	// Token: 0x04005EEC RID: 24300
	public string ResourcesFolderPath;

	// Token: 0x04005EED RID: 24301
	public string MaterialsFolderPath;

	// Token: 0x04005EEE RID: 24302
	public string PrefabsFolderPath;

	// Token: 0x04005EEF RID: 24303
	private static UberCombinerAssets gInstance;
}
