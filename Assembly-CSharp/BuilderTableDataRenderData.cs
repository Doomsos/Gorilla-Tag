using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000593 RID: 1427
public class BuilderTableDataRenderData
{
	// Token: 0x04002F31 RID: 12081
	public const int NUM_SPLIT_MESH_INSTANCE_GROUPS = 1;

	// Token: 0x04002F32 RID: 12082
	public int texWidth;

	// Token: 0x04002F33 RID: 12083
	public int texHeight;

	// Token: 0x04002F34 RID: 12084
	public TextureFormat textureFormat;

	// Token: 0x04002F35 RID: 12085
	public Dictionary<Material, int> materialToIndex;

	// Token: 0x04002F36 RID: 12086
	public List<Material> materials;

	// Token: 0x04002F37 RID: 12087
	public Material sharedMaterial;

	// Token: 0x04002F38 RID: 12088
	public Material sharedMaterialIndirect;

	// Token: 0x04002F39 RID: 12089
	public Dictionary<Texture2D, int> textureToIndex;

	// Token: 0x04002F3A RID: 12090
	public List<Texture2D> textures;

	// Token: 0x04002F3B RID: 12091
	public List<Material> perTextureMaterial;

	// Token: 0x04002F3C RID: 12092
	public List<MaterialPropertyBlock> perTexturePropertyBlock;

	// Token: 0x04002F3D RID: 12093
	public Texture2DArray sharedTexArray;

	// Token: 0x04002F3E RID: 12094
	public Dictionary<Mesh, int> meshToIndex;

	// Token: 0x04002F3F RID: 12095
	public List<Mesh> meshes;

	// Token: 0x04002F40 RID: 12096
	public List<int> meshInstanceCount;

	// Token: 0x04002F41 RID: 12097
	public NativeList<BuilderTableSubMesh> subMeshes;

	// Token: 0x04002F42 RID: 12098
	public Mesh sharedMesh;

	// Token: 0x04002F43 RID: 12099
	public BuilderTableDataRenderIndirectBatch dynamicBatch;

	// Token: 0x04002F44 RID: 12100
	public BuilderTableDataRenderIndirectBatch staticBatch;

	// Token: 0x04002F45 RID: 12101
	public JobHandle setupInstancesJobs;
}
