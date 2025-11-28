using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000594 RID: 1428
public class BuilderRenderer : MonoBehaviourPostTick
{
	// Token: 0x0600240B RID: 9227 RVA: 0x000BF62B File Offset: 0x000BD82B
	private void Awake()
	{
		this.InitIfNeeded();
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x000BF634 File Offset: 0x000BD834
	public void InitIfNeeded()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		this.snapPieceShader = Shader.Find("GorillaTag/SnapPiece");
		if (this.renderData == null)
		{
			this.renderData = new BuilderTableDataRenderData();
		}
		this.renderData.materialToIndex = new Dictionary<Material, int>(256);
		this.renderData.materials = new List<Material>(256);
		if (this.renderData.meshToIndex == null)
		{
			this.renderData.meshToIndex = new Dictionary<Mesh, int>(1024);
		}
		if (this.renderData.meshInstanceCount == null)
		{
			this.renderData.meshInstanceCount = new List<int>(1024);
		}
		if (this.renderData.meshes == null)
		{
			this.renderData.meshes = new List<Mesh>(4096);
		}
		if (this.renderData.textureToIndex == null)
		{
			this.renderData.textureToIndex = new Dictionary<Texture2D, int>(256);
		}
		if (this.renderData.textures == null)
		{
			this.renderData.textures = new List<Texture2D>(256);
		}
		if (this.renderData.perTextureMaterial == null)
		{
			this.renderData.perTextureMaterial = new List<Material>(256);
		}
		if (this.renderData.perTexturePropertyBlock == null)
		{
			this.renderData.perTexturePropertyBlock = new List<MaterialPropertyBlock>(256);
		}
		if (this.renderData.sharedMaterial == null)
		{
			this.renderData.sharedMaterial = new Material(this.sharedMaterialBase);
		}
		if (this.renderData.sharedMaterialIndirect == null)
		{
			this.renderData.sharedMaterialIndirect = new Material(this.sharedMaterialIndirectBase);
		}
		this.built = false;
		this.showing = false;
	}

	// Token: 0x0600240D RID: 9229 RVA: 0x000BF7EC File Offset: 0x000BD9EC
	public void Show(bool show)
	{
		this.showing = show;
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x000BF7F8 File Offset: 0x000BD9F8
	public void BuildRenderer(List<BuilderPiece> piecePrefabs)
	{
		this.InitIfNeeded();
		for (int i = 0; i < piecePrefabs.Count; i++)
		{
			if (piecePrefabs[i] != null)
			{
				this.AddPrefab(piecePrefabs[i]);
			}
			else
			{
				Debug.LogErrorFormat("Prefab at {0} is null", new object[]
				{
					i
				});
			}
		}
		this.BuildSharedMaterial();
		this.BuildSharedMesh();
		this.BuildBuffer();
		this.built = true;
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x000BF86C File Offset: 0x000BDA6C
	public void LogDraws()
	{
		Debug.LogFormat("Builder Renderer Counts {0} {1} {2} {3}", new object[]
		{
			this.renderData.subMeshes.Length,
			this.renderData.textures.Count,
			this.renderData.dynamicBatch.totalInstances,
			this.renderData.staticBatch.totalInstances
		});
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x000BF8E9 File Offset: 0x000BDAE9
	public override void PostTick()
	{
		if (!this.built || !this.showing)
		{
			return;
		}
		this.RenderIndirect();
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x000BF904 File Offset: 0x000BDB04
	public void WriteSerializedData()
	{
		if (this.renderData == null)
		{
			return;
		}
		if (this.renderData.sharedMesh != null)
		{
			this.serializeMeshToIndexKeys = new List<Mesh>(this.renderData.meshToIndex.Count);
			this.serializeMeshToIndexValues = new List<int>(this.renderData.meshToIndex.Count);
			foreach (KeyValuePair<Mesh, int> keyValuePair in this.renderData.meshToIndex)
			{
				this.serializeMeshToIndexKeys.Add(keyValuePair.Key);
				this.serializeMeshToIndexValues.Add(keyValuePair.Value);
			}
			this.serializeMeshes = this.renderData.meshes;
			this.serializeMeshInstanceCount = this.renderData.meshInstanceCount;
			this.serializeSubMeshes = new List<BuilderTableSubMesh>(512);
			foreach (BuilderTableSubMesh builderTableSubMesh in this.renderData.subMeshes)
			{
				this.serializeSubMeshes.Add(builderTableSubMesh);
			}
			this.serializeSharedMesh = this.renderData.sharedMesh;
		}
		if (this.renderData.sharedMaterial != null)
		{
			this.serializeTextureToIndexKeys = new List<Texture2D>(this.renderData.textureToIndex.Count);
			this.serializeTextureToIndexValues = new List<int>(this.renderData.textureToIndex.Count);
			foreach (KeyValuePair<Texture2D, int> keyValuePair2 in this.renderData.textureToIndex)
			{
				this.serializeTextureToIndexKeys.Add(keyValuePair2.Key);
				this.serializeTextureToIndexValues.Add(keyValuePair2.Value);
			}
			this.serializeTextures = this.renderData.textures;
			this.serializePerTextureMaterial = this.renderData.perTextureMaterial;
			this.serializePerTexturePropertyBlock = this.renderData.perTexturePropertyBlock;
			this.serializeSharedTexArray = this.renderData.sharedTexArray;
			this.serializeSharedMaterial = this.renderData.sharedMaterial;
			this.serializeSharedMaterialIndirect = this.renderData.sharedMaterialIndirect;
		}
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x000BFB78 File Offset: 0x000BDD78
	private void ApplySerializedData()
	{
		if (this.serializeSharedMesh != null)
		{
			if (this.renderData == null)
			{
				this.renderData = new BuilderTableDataRenderData();
			}
			this.renderData.meshToIndex = new Dictionary<Mesh, int>(1024);
			for (int i = 0; i < this.serializeMeshToIndexKeys.Count; i++)
			{
				this.renderData.meshToIndex.Add(this.serializeMeshToIndexKeys[i], this.serializeMeshToIndexValues[i]);
			}
			this.renderData.meshes = this.serializeMeshes;
			this.renderData.meshInstanceCount = this.serializeMeshInstanceCount;
			this.renderData.subMeshes = new NativeList<BuilderTableSubMesh>(512, 4);
			foreach (BuilderTableSubMesh builderTableSubMesh in this.serializeSubMeshes)
			{
				this.renderData.subMeshes.AddNoResize(builderTableSubMesh);
			}
			this.renderData.sharedMesh = this.serializeSharedMesh;
		}
		if (this.serializeSharedMaterial != null)
		{
			if (this.renderData == null)
			{
				this.renderData = new BuilderTableDataRenderData();
			}
			this.renderData.textureToIndex = new Dictionary<Texture2D, int>(256);
			for (int j = 0; j < this.serializeTextureToIndexKeys.Count; j++)
			{
				this.renderData.textureToIndex.Add(this.serializeTextureToIndexKeys[j], this.serializeTextureToIndexValues[j]);
			}
			this.renderData.textures = this.serializeTextures;
			this.renderData.perTextureMaterial = this.serializePerTextureMaterial;
			this.renderData.perTexturePropertyBlock = this.serializePerTexturePropertyBlock;
			this.renderData.sharedTexArray = this.serializeSharedTexArray;
			this.renderData.sharedMaterial = this.serializeSharedMaterial;
			this.renderData.sharedMaterialIndirect = this.serializeSharedMaterialIndirect;
		}
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000BFD78 File Offset: 0x000BDF78
	public void AddPrefab(BuilderPiece prefab)
	{
		BuilderRenderer.meshRenderers.Clear();
		prefab.GetComponentsInChildren<MeshRenderer>(true, BuilderRenderer.meshRenderers);
		for (int i = 0; i < BuilderRenderer.meshRenderers.Count; i++)
		{
			MeshRenderer meshRenderer = BuilderRenderer.meshRenderers[i];
			Material sharedMaterial = meshRenderer.sharedMaterial;
			if (sharedMaterial == null)
			{
				if (!prefab.suppressMaterialWarnings)
				{
					Debug.LogErrorFormat("{0} {1} is missing a buidler material", new object[]
					{
						prefab.name,
						meshRenderer.name
					});
				}
			}
			else if (!this.AddMaterial(sharedMaterial, prefab.suppressMaterialWarnings))
			{
				if (!prefab.suppressMaterialWarnings)
				{
					Debug.LogWarningFormat("{0} {1} failed to add builder material", new object[]
					{
						prefab.name,
						meshRenderer.name
					});
				}
			}
			else if (this.renderData.sharedMesh == null)
			{
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (component != null)
				{
					Mesh sharedMesh = component.sharedMesh;
					int num;
					if (sharedMesh != null && !this.renderData.meshToIndex.TryGetValue(sharedMesh, ref num))
					{
						this.renderData.meshToIndex.Add(sharedMesh, this.renderData.meshToIndex.Count);
						this.renderData.meshInstanceCount.Add(0);
						for (int j = 0; j < 1; j++)
						{
							this.renderData.meshes.Add(sharedMesh);
						}
					}
				}
			}
		}
		if (prefab.materialOptions != null)
		{
			for (int k = 0; k < prefab.materialOptions.options.Count; k++)
			{
				Material material = prefab.materialOptions.options[k].material;
				if (!this.AddMaterial(material, prefab.suppressMaterialWarnings) && !prefab.suppressMaterialWarnings)
				{
					Debug.LogWarningFormat("builder material options {0} bad material index {1}", new object[]
					{
						prefab.materialOptions.name,
						k
					});
				}
			}
		}
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x000BFF70 File Offset: 0x000BE170
	private bool AddMaterial(Material material, bool suppressWarnings = false)
	{
		if (material == null)
		{
			return false;
		}
		if (material.shader != this.snapPieceShader)
		{
			if (!suppressWarnings)
			{
				Debug.LogWarningFormat("builder: material {0} uses non snap piece shader {1}", new object[]
				{
					material.name,
					material.shader.name
				});
			}
			return false;
		}
		if (!material.HasTexture("_BaseMap"))
		{
			if (!suppressWarnings)
			{
				Debug.LogWarningFormat("builder material {0} does not have texture property {1}", new object[]
				{
					material.name,
					"_BaseMap"
				});
			}
			return false;
		}
		Texture texture = material.GetTexture("_BaseMap");
		if (texture == null)
		{
			if (!suppressWarnings)
			{
				Debug.LogWarningFormat("builder material {0} null texture", new object[]
				{
					material.name
				});
			}
			return false;
		}
		Texture2D texture2D = texture as Texture2D;
		if (texture2D == null)
		{
			if (!suppressWarnings)
			{
				Debug.LogWarningFormat("builder material {0} no texture2d type is {1}", new object[]
				{
					material.name,
					texture.GetType()
				});
			}
			return false;
		}
		if (texture2D.width != 256 || texture2D.height != 256)
		{
			if (!suppressWarnings)
			{
				Debug.LogWarningFormat("builder texture {0} unexpected size {1} {2}", new object[]
				{
					texture2D.name,
					texture2D.width,
					texture2D.height
				});
			}
			return false;
		}
		int num;
		if (!this.renderData.materialToIndex.TryGetValue(material, ref num))
		{
			this.renderData.materialToIndex.Add(material, this.renderData.materials.Count);
			this.renderData.materials.Add(material);
		}
		int num2;
		if (!this.renderData.textureToIndex.TryGetValue(texture2D, ref num2))
		{
			this.renderData.textureToIndex.Add(texture2D, this.renderData.textures.Count);
			this.renderData.textures.Add(texture2D);
			if (this.renderData.textures.Count == 1)
			{
				this.renderData.textureFormat = texture2D.format;
				this.renderData.texWidth = texture2D.width;
				this.renderData.texHeight = texture2D.height;
			}
		}
		return true;
	}

	// Token: 0x06002415 RID: 9237 RVA: 0x000C0194 File Offset: 0x000BE394
	public void BuildSharedMaterial()
	{
		if (this.renderData.sharedTexArray != null)
		{
			Debug.Log("Already have shared material. Not building new one.");
			return;
		}
		TextureFormat textureFormat = 4;
		this.renderData.sharedTexArray = new Texture2DArray(this.renderData.texWidth, this.renderData.texHeight, this.renderData.textures.Count, textureFormat, true);
		this.renderData.sharedTexArray.filterMode = 0;
		for (int i = 0; i < this.renderData.textures.Count; i++)
		{
			this.renderData.sharedTexArray.SetPixels(this.renderData.textures[i].GetPixels(), i);
		}
		this.renderData.sharedTexArray.Apply(true, true);
		this.renderData.sharedMaterial.SetTexture("_BaseMapArray", this.renderData.sharedTexArray);
		this.renderData.sharedMaterialIndirect.SetTexture("_BaseMapArray", this.renderData.sharedTexArray);
		this.renderData.sharedMaterialIndirect.enableInstancing = true;
		for (int j = 0; j < this.renderData.textures.Count; j++)
		{
			Material material = new Material(this.renderData.sharedMaterial);
			material.SetInt("_BaseMapArrayIndex", j);
			this.renderData.perTextureMaterial.Add(material);
			MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
			materialPropertyBlock.SetInt("_BaseMapArrayIndex", j);
			this.renderData.perTexturePropertyBlock.Add(materialPropertyBlock);
		}
	}

	// Token: 0x06002416 RID: 9238 RVA: 0x000C0320 File Offset: 0x000BE520
	public void BuildSharedMesh()
	{
		if (this.renderData.sharedMesh != null)
		{
			Debug.Log("Already have shared mesh. Not building new one.");
			return;
		}
		this.renderData.sharedMesh = new Mesh();
		this.renderData.sharedMesh.indexFormat = 1;
		BuilderRenderer.verticesAll.Clear();
		BuilderRenderer.normalsAll.Clear();
		BuilderRenderer.uv1All.Clear();
		BuilderRenderer.trianglesAll.Clear();
		this.renderData.subMeshes = new NativeList<BuilderTableSubMesh>(512, 4);
		for (int i = 0; i < this.renderData.meshes.Count; i++)
		{
			Mesh mesh = this.renderData.meshes[i];
			int count = BuilderRenderer.trianglesAll.Count;
			int count2 = BuilderRenderer.verticesAll.Count;
			BuilderRenderer.vertices.Clear();
			BuilderRenderer.normals.Clear();
			BuilderRenderer.uv1.Clear();
			BuilderRenderer.triangles.Clear();
			mesh.GetVertices(BuilderRenderer.vertices);
			mesh.GetNormals(BuilderRenderer.normals);
			mesh.GetUVs(0, BuilderRenderer.uv1);
			mesh.GetTriangles(BuilderRenderer.triangles, 0);
			BuilderRenderer.verticesAll.AddRange(BuilderRenderer.vertices);
			BuilderRenderer.normalsAll.AddRange(BuilderRenderer.normals);
			BuilderRenderer.uv1All.AddRange(BuilderRenderer.uv1);
			BuilderRenderer.trianglesAll.AddRange(BuilderRenderer.triangles);
			int indexCount = BuilderRenderer.trianglesAll.Count - count;
			BuilderTableSubMesh builderTableSubMesh = new BuilderTableSubMesh
			{
				startIndex = count,
				indexCount = indexCount,
				startVertex = count2
			};
			this.renderData.subMeshes.Add(ref builderTableSubMesh);
		}
		this.renderData.sharedMesh.SetVertices(BuilderRenderer.verticesAll);
		this.renderData.sharedMesh.SetNormals(BuilderRenderer.normalsAll);
		this.renderData.sharedMesh.SetUVs(0, BuilderRenderer.uv1All);
		this.renderData.sharedMesh.SetTriangles(BuilderRenderer.trianglesAll, 0);
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x000C0524 File Offset: 0x000BE724
	public void BuildBuffer()
	{
		this.renderData.dynamicBatch = new BuilderTableDataRenderIndirectBatch();
		BuilderRenderer.BuildBatch(this.renderData.dynamicBatch, this.renderData.meshes.Count, 8192, this.renderData.sharedMaterialIndirect);
		this.renderData.staticBatch = new BuilderTableDataRenderIndirectBatch();
		BuilderRenderer.BuildBatch(this.renderData.staticBatch, this.renderData.meshes.Count, 8192, this.renderData.sharedMaterialIndirect);
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x000C05B4 File Offset: 0x000BE7B4
	public static void BuildBatch(BuilderTableDataRenderIndirectBatch indirectBatch, int meshCount, int maxInstances, Material sharedMaterialIndirect)
	{
		indirectBatch.totalInstances = 0;
		indirectBatch.commandCount = meshCount;
		indirectBatch.commandBuf = new GraphicsBuffer(256, indirectBatch.commandCount, 20);
		indirectBatch.commandData = new NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs>(indirectBatch.commandCount, 4, 1);
		indirectBatch.matrixBuf = new GraphicsBuffer(16, maxInstances * 2, 64);
		indirectBatch.texIndexBuf = new GraphicsBuffer(16, maxInstances * 2, 4);
		indirectBatch.tintBuf = new GraphicsBuffer(16, maxInstances * 2, 4);
		indirectBatch.instanceTransform = new TransformAccessArray(maxInstances, 3);
		indirectBatch.instanceTransformIndexToDataIndex = new NativeArray<int>(maxInstances, 4, 1);
		for (int i = 0; i < maxInstances; i++)
		{
			indirectBatch.instanceTransformIndexToDataIndex[i] = -1;
		}
		indirectBatch.pieceIDPerTransform = new List<int>(maxInstances);
		indirectBatch.instanceObjectToWorld = new NativeArray<Matrix4x4>(maxInstances * 2, 4, 1);
		indirectBatch.instanceTexIndex = new NativeArray<int>(maxInstances * 2, 4, 1);
		indirectBatch.instanceTint = new NativeArray<float>(maxInstances * 2, 4, 1);
		indirectBatch.renderMeshes = new NativeList<BuilderTableMeshInstances>(512, 4);
		for (int j = 0; j < meshCount; j++)
		{
			BuilderTableMeshInstances builderTableMeshInstances = new BuilderTableMeshInstances
			{
				transforms = new TransformAccessArray(maxInstances, 3),
				texIndex = new NativeList<int>(4),
				tint = new NativeList<float>(4)
			};
			indirectBatch.renderMeshes.Add(ref builderTableMeshInstances);
		}
		indirectBatch.rp = new RenderParams(sharedMaterialIndirect);
		indirectBatch.rp.worldBounds = new Bounds(Vector3.zero, 10000f * Vector3.one);
		indirectBatch.rp.matProps = new MaterialPropertyBlock();
		indirectBatch.rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.identity);
		indirectBatch.matrixBuf.SetData<Matrix4x4>(indirectBatch.instanceObjectToWorld);
		indirectBatch.texIndexBuf.SetData<int>(indirectBatch.instanceTexIndex);
		indirectBatch.tintBuf.SetData<float>(indirectBatch.instanceTint);
		indirectBatch.rp.matProps.SetBuffer("_TransformMatrix", indirectBatch.matrixBuf);
		indirectBatch.rp.matProps.SetBuffer("_TexIndex", indirectBatch.texIndexBuf);
		indirectBatch.rp.matProps.SetBuffer("_Tint", indirectBatch.tintBuf);
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x000C07E7 File Offset: 0x000BE9E7
	private void OnDestroy()
	{
		this.DestroyBuffer();
		this.renderData.subMeshes.Dispose();
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x000C07FF File Offset: 0x000BE9FF
	public void DestroyBuffer()
	{
		BuilderRenderer.DestroyBatch(this.renderData.staticBatch);
		BuilderRenderer.DestroyBatch(this.renderData.dynamicBatch);
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x000C0824 File Offset: 0x000BEA24
	public static void DestroyBatch(BuilderTableDataRenderIndirectBatch indirectBatch)
	{
		indirectBatch.commandBuf.Dispose();
		indirectBatch.commandData.Dispose();
		indirectBatch.matrixBuf.Dispose();
		indirectBatch.texIndexBuf.Dispose();
		indirectBatch.tintBuf.Dispose();
		indirectBatch.instanceTransform.Dispose();
		indirectBatch.instanceTransformIndexToDataIndex.Dispose();
		indirectBatch.instanceObjectToWorld.Dispose();
		indirectBatch.instanceTexIndex.Dispose();
		indirectBatch.instanceTint.Dispose();
		foreach (BuilderTableMeshInstances builderTableMeshInstances in indirectBatch.renderMeshes)
		{
			TransformAccessArray transforms = builderTableMeshInstances.transforms;
			transforms.Dispose();
			NativeList<int> texIndex = builderTableMeshInstances.texIndex;
			texIndex.Dispose();
			NativeList<float> tint = builderTableMeshInstances.tint;
			tint.Dispose();
		}
		indirectBatch.renderMeshes.Dispose();
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000C0914 File Offset: 0x000BEB14
	public void PreRenderIndirect()
	{
		if (!this.built || !this.showing)
		{
			return;
		}
		this.renderData.setupInstancesJobs = default(JobHandle);
		BuilderRenderer.SetupIndirectBatchArgs(this.renderData.staticBatch, this.renderData.subMeshes);
		BuilderRenderer.SetupInstanceDataForMeshStatic setupInstanceDataForMeshStatic = new BuilderRenderer.SetupInstanceDataForMeshStatic
		{
			transformIndexToDataIndex = this.renderData.staticBatch.instanceTransformIndexToDataIndex,
			objectToWorld = this.renderData.staticBatch.instanceObjectToWorld
		};
		this.renderData.setupInstancesJobs = IJobParallelForTransformExtensions.ScheduleReadOnly<BuilderRenderer.SetupInstanceDataForMeshStatic>(setupInstanceDataForMeshStatic, this.renderData.staticBatch.instanceTransform, 32, default(JobHandle));
		JobHandle.ScheduleBatchedJobs();
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x000C09C7 File Offset: 0x000BEBC7
	public void RenderIndirect()
	{
		this.renderData.setupInstancesJobs.Complete();
		this.RenderIndirectBatch(this.renderData.staticBatch);
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000C09EC File Offset: 0x000BEBEC
	private static void SetupIndirectBatchArgs(BuilderTableDataRenderIndirectBatch indirectBatch, NativeList<BuilderTableSubMesh> subMeshes)
	{
		uint num = 0U;
		for (int i = 0; i < indirectBatch.commandCount; i++)
		{
			BuilderTableMeshInstances builderTableMeshInstances = indirectBatch.renderMeshes[i];
			BuilderTableSubMesh builderTableSubMesh = subMeshes[i];
			GraphicsBuffer.IndirectDrawIndexedArgs indirectDrawIndexedArgs = default(GraphicsBuffer.IndirectDrawIndexedArgs);
			indirectDrawIndexedArgs.indexCountPerInstance = (uint)builderTableSubMesh.indexCount;
			indirectDrawIndexedArgs.startIndex = (uint)builderTableSubMesh.startIndex;
			indirectDrawIndexedArgs.baseVertexIndex = (uint)builderTableSubMesh.startVertex;
			indirectDrawIndexedArgs.startInstance = num;
			indirectDrawIndexedArgs.instanceCount = (uint)(builderTableMeshInstances.transforms.length * 2);
			num += indirectDrawIndexedArgs.instanceCount;
			indirectBatch.commandData[i] = indirectDrawIndexedArgs;
		}
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x000C0A8C File Offset: 0x000BEC8C
	private void RenderIndirectBatch(BuilderTableDataRenderIndirectBatch indirectBatch)
	{
		indirectBatch.matrixBuf.SetData<Matrix4x4>(indirectBatch.instanceObjectToWorld);
		indirectBatch.texIndexBuf.SetData<int>(indirectBatch.instanceTexIndex);
		indirectBatch.tintBuf.SetData<float>(indirectBatch.instanceTint);
		indirectBatch.commandBuf.SetData<GraphicsBuffer.IndirectDrawIndexedArgs>(indirectBatch.commandData);
		Graphics.RenderMeshIndirect(ref indirectBatch.rp, this.renderData.sharedMesh, indirectBatch.commandBuf, indirectBatch.commandCount, 0);
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x000C0B00 File Offset: 0x000BED00
	public void AddPiece(BuilderPiece piece)
	{
		bool isStatic = piece.isStatic;
		BuilderRenderer.meshRenderers.Clear();
		piece.GetComponentsInChildren<MeshRenderer>(false, BuilderRenderer.meshRenderers);
		for (int i = 0; i < BuilderRenderer.meshRenderers.Count; i++)
		{
			MeshRenderer meshRenderer = BuilderRenderer.meshRenderers[i];
			if (meshRenderer.enabled)
			{
				Material material = meshRenderer.material;
				if (material.HasTexture("_BaseMap"))
				{
					Texture2D texture2D = material.GetTexture("_BaseMap") as Texture2D;
					if (!(texture2D == null))
					{
						int num;
						if (!this.renderData.textureToIndex.TryGetValue(texture2D, ref num))
						{
							if (!piece.suppressMaterialWarnings)
							{
								Debug.LogWarningFormat("builder piece {0} material {1} texture not found in render data", new object[]
								{
									piece.displayName,
									material.name
								});
							}
						}
						else
						{
							MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
							if (!(component == null))
							{
								Mesh sharedMesh = component.sharedMesh;
								if (!(sharedMesh == null))
								{
									int num2;
									if (!this.renderData.meshToIndex.TryGetValue(sharedMesh, ref num2))
									{
										Debug.LogWarningFormat("builder piece {0} mesh {1} not found in render data", new object[]
										{
											piece.displayName,
											meshRenderer.name
										});
									}
									else
									{
										int num3 = this.renderData.meshInstanceCount[num2] % 1;
										this.renderData.meshInstanceCount[num2] = this.renderData.meshInstanceCount[num2] + 1;
										num2 += num3;
										int num4 = -1;
										if (isStatic)
										{
											NativeArray<int> instanceTransformIndexToDataIndex = this.renderData.staticBatch.instanceTransformIndexToDataIndex;
											int length = this.renderData.staticBatch.instanceTransform.length;
											if (length + 2 >= instanceTransformIndexToDataIndex.Length)
											{
												GTDev.LogError<string>("Too Many Builder Mesh Instances", null);
												return;
											}
											num4 = length;
											BuilderTableMeshInstances builderTableMeshInstances = this.renderData.staticBatch.renderMeshes[num2];
											int num5 = 0;
											for (int j = 0; j <= num2; j++)
											{
												num5 += this.renderData.staticBatch.renderMeshes[j].transforms.length * 2;
											}
											for (int k = 0; k < length; k++)
											{
												if (this.renderData.staticBatch.instanceTransformIndexToDataIndex[k] >= num5)
												{
													this.renderData.staticBatch.instanceTransformIndexToDataIndex[k] = this.renderData.staticBatch.instanceTransformIndexToDataIndex[k] + 2;
												}
											}
											this.renderData.staticBatch.pieceIDPerTransform.Add(piece.pieceId);
											this.renderData.staticBatch.instanceTransform.Add(meshRenderer.transform);
											this.renderData.staticBatch.instanceTransformIndexToDataIndex[num4] = num5;
											builderTableMeshInstances.transforms.Add(meshRenderer.transform);
											builderTableMeshInstances.texIndex.Add(ref num);
											builderTableMeshInstances.tint.Add(ref piece.tint);
											int num6 = this.renderData.staticBatch.totalInstances - 1;
											for (int l = num6; l >= num5; l--)
											{
												this.renderData.staticBatch.instanceTexIndex[l + 2] = this.renderData.staticBatch.instanceTexIndex[l];
											}
											for (int m = num6; m >= num5; m--)
											{
												this.renderData.staticBatch.instanceObjectToWorld[m + 2] = this.renderData.staticBatch.instanceObjectToWorld[m];
											}
											for (int n = num6; n >= num5; n--)
											{
												this.renderData.staticBatch.instanceTint[n + 2] = this.renderData.staticBatch.instanceTint[n];
											}
											for (int num7 = 0; num7 < 2; num7++)
											{
												this.renderData.staticBatch.instanceObjectToWorld[num5 + num7] = meshRenderer.transform.localToWorldMatrix;
												this.renderData.staticBatch.instanceTexIndex[num5 + num7] = num;
												this.renderData.staticBatch.instanceTint[num5 + num7] = 1f;
												this.renderData.staticBatch.totalInstances++;
											}
										}
										else
										{
											BuilderTableMeshInstances builderTableMeshInstances2 = this.renderData.dynamicBatch.renderMeshes[num2];
											builderTableMeshInstances2.transforms.Add(meshRenderer.transform);
											builderTableMeshInstances2.texIndex.Add(ref num);
											builderTableMeshInstances2.tint.Add(ref piece.tint);
											this.renderData.dynamicBatch.totalInstances++;
										}
										piece.renderingIndirect.Add(meshRenderer);
										piece.renderingDirect.Remove(meshRenderer);
										piece.renderingIndirectTransformIndex.Add(num4);
										meshRenderer.enabled = false;
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x000C1004 File Offset: 0x000BF204
	public void RemovePiece(BuilderPiece piece)
	{
		bool isStatic = piece.isStatic;
		for (int i = 0; i < piece.renderingIndirect.Count; i++)
		{
			MeshRenderer meshRenderer = piece.renderingIndirect[i];
			if (!(meshRenderer == null))
			{
				Material sharedMaterial = meshRenderer.sharedMaterial;
				if (sharedMaterial.HasTexture("_BaseMap"))
				{
					Texture2D texture2D = sharedMaterial.GetTexture("_BaseMap") as Texture2D;
					int num;
					if (!(texture2D == null) && this.renderData.textureToIndex.TryGetValue(texture2D, ref num))
					{
						MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
						if (!(component == null))
						{
							Mesh sharedMesh = component.sharedMesh;
							int num2;
							if (this.renderData.meshToIndex.TryGetValue(sharedMesh, ref num2))
							{
								Transform transform = meshRenderer.transform;
								bool flag = false;
								int num3 = 0;
								int num4 = -1;
								if (isStatic)
								{
									for (int j = 0; j < num2; j++)
									{
										num3 += this.renderData.staticBatch.renderMeshes[j].transforms.length;
									}
									TransformAccessArray instanceTransform = this.renderData.staticBatch.instanceTransform;
									int length = instanceTransform.length;
									int num5 = piece.renderingIndirectTransformIndex[i];
									num4 = this.renderData.staticBatch.instanceTransformIndexToDataIndex[num5];
									int num6 = this.renderData.staticBatch.instanceTransform.length - 1;
									int pieceId = this.renderData.staticBatch.pieceIDPerTransform[num6];
									this.renderData.staticBatch.instanceTransform.RemoveAtSwapBack(num5);
									ListExtensions.RemoveAtSwapBack<int>(this.renderData.staticBatch.pieceIDPerTransform, num5);
									this.renderData.staticBatch.instanceTransformIndexToDataIndex[num5] = this.renderData.staticBatch.instanceTransformIndexToDataIndex[num6];
									this.renderData.staticBatch.instanceTransformIndexToDataIndex[num6] = -1;
									BuilderPiece piece2 = piece.GetTable().GetPiece(pieceId);
									if (piece2 != null)
									{
										for (int k = 0; k < piece2.renderingIndirectTransformIndex.Count; k++)
										{
											if (piece2.renderingIndirectTransformIndex[k] == num6)
											{
												piece2.renderingIndirectTransformIndex[k] = num5;
											}
										}
									}
									for (int l = 0; l < length; l++)
									{
										if (this.renderData.staticBatch.instanceTransformIndexToDataIndex[l] > num4)
										{
											this.renderData.staticBatch.instanceTransformIndexToDataIndex[l] = this.renderData.staticBatch.instanceTransformIndexToDataIndex[l] - 2;
										}
									}
								}
								for (int m = 0; m < 1; m++)
								{
									int num7 = num2 + m;
									if (isStatic)
									{
										BuilderTableMeshInstances builderTableMeshInstances = this.renderData.staticBatch.renderMeshes[num7];
										for (int n = 0; n < builderTableMeshInstances.transforms.length; n++)
										{
											if (builderTableMeshInstances.transforms[n] == transform)
											{
												num3 += n;
												BuilderRenderer.RemoveAt(builderTableMeshInstances.transforms, n);
												builderTableMeshInstances.texIndex.RemoveAt(n);
												builderTableMeshInstances.tint.RemoveAt(n);
												flag = true;
												this.renderData.staticBatch.totalInstances -= 2;
												break;
											}
										}
									}
									else
									{
										BuilderTableMeshInstances builderTableMeshInstances2 = this.renderData.dynamicBatch.renderMeshes[num7];
										for (int num8 = 0; num8 < builderTableMeshInstances2.transforms.length; num8++)
										{
											if (builderTableMeshInstances2.transforms[num8] == transform)
											{
												BuilderRenderer.RemoveAt(builderTableMeshInstances2.transforms, num8);
												builderTableMeshInstances2.texIndex.RemoveAt(num8);
												builderTableMeshInstances2.tint.RemoveAt(num8);
												flag = true;
												this.renderData.dynamicBatch.totalInstances--;
												break;
											}
										}
									}
									if (flag)
									{
										piece.renderingDirect.Add(meshRenderer);
										break;
									}
								}
								if (flag && isStatic)
								{
									int num9 = this.renderData.staticBatch.totalInstances + 1;
									for (int num10 = num4; num10 < num9; num10++)
									{
										this.renderData.staticBatch.instanceTexIndex[num10] = this.renderData.staticBatch.instanceTexIndex[num10 + 2];
									}
									for (int num11 = num4; num11 < num9; num11++)
									{
										this.renderData.staticBatch.instanceObjectToWorld[num11] = this.renderData.staticBatch.instanceObjectToWorld[num11 + 2];
									}
									for (int num12 = num4; num12 < num9; num12++)
									{
										this.renderData.staticBatch.instanceTint[num12] = this.renderData.staticBatch.instanceTint[num12 + 2];
									}
								}
								meshRenderer.enabled = true;
							}
						}
					}
				}
			}
		}
		piece.renderingIndirect.Clear();
		piece.renderingIndirectTransformIndex.Clear();
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x000C1528 File Offset: 0x000BF728
	public void ChangePieceIndirectMaterial(BuilderPiece piece, List<MeshRenderer> targetRenderers, Material targetMaterial)
	{
		if (targetMaterial == null)
		{
			return;
		}
		if (!targetMaterial.HasTexture("_BaseMap"))
		{
			Debug.LogError("New Material is missing a texture");
			return;
		}
		Texture2D texture2D = targetMaterial.GetTexture("_BaseMap") as Texture2D;
		if (texture2D == null)
		{
			Debug.LogError("New Material does not have a \"_BaseMap\" property");
			return;
		}
		int num;
		if (!this.renderData.textureToIndex.TryGetValue(texture2D, ref num))
		{
			Debug.LogError("New Material is not in the texture array");
			return;
		}
		bool isStatic = piece.isStatic;
		for (int i = 0; i < piece.renderingIndirect.Count; i++)
		{
			MeshRenderer meshRenderer = piece.renderingIndirect[i];
			if (!targetRenderers.Contains(meshRenderer))
			{
				Debug.Log("renderer not in target list");
			}
			else
			{
				meshRenderer.material = targetMaterial;
				MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
				if (!(component == null))
				{
					Mesh sharedMesh = component.sharedMesh;
					int num2;
					if (this.renderData.meshToIndex.TryGetValue(sharedMesh, ref num2))
					{
						Transform transform = meshRenderer.transform;
						bool flag = false;
						if (isStatic)
						{
							int num3 = piece.renderingIndirectTransformIndex[i];
							int num4 = this.renderData.staticBatch.instanceTransformIndexToDataIndex[num3];
							if (num4 >= 0)
							{
								for (int j = 0; j < 2; j++)
								{
									this.renderData.staticBatch.instanceTexIndex[num4 + j] = num;
								}
							}
						}
						else
						{
							for (int k = 0; k < 1; k++)
							{
								int num5 = num2 + k;
								BuilderTableMeshInstances builderTableMeshInstances = this.renderData.dynamicBatch.renderMeshes[num5];
								for (int l = 0; l < builderTableMeshInstances.transforms.length; l++)
								{
									if (builderTableMeshInstances.transforms[l] == transform)
									{
										this.renderData.dynamicBatch.renderMeshes.ElementAt(num5).texIndex[l] = num;
										flag = true;
										break;
									}
								}
								if (flag)
								{
									break;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x000C172C File Offset: 0x000BF92C
	public static void RemoveAt(TransformAccessArray a, int i)
	{
		int length = a.length;
		for (int j = i; j < length - 1; j++)
		{
			a[j] = a[j + 1];
		}
		a.RemoveAtSwapBack(length - 1);
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x000C176C File Offset: 0x000BF96C
	public void SetPieceTint(BuilderPiece piece, float tint)
	{
		for (int i = 0; i < piece.renderingIndirect.Count; i++)
		{
			MeshRenderer meshRenderer = piece.renderingIndirect[i];
			Material sharedMaterial = meshRenderer.sharedMaterial;
			if (sharedMaterial.HasTexture("_BaseMap"))
			{
				Texture2D texture2D = sharedMaterial.GetTexture("_BaseMap") as Texture2D;
				int num;
				if (!(texture2D == null) && this.renderData.textureToIndex.TryGetValue(texture2D, ref num))
				{
					MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
					if (!(component == null))
					{
						Mesh sharedMesh = component.sharedMesh;
						int num2;
						if (this.renderData.meshToIndex.TryGetValue(sharedMesh, ref num2))
						{
							Transform transform = meshRenderer.transform;
							if (piece.isStatic)
							{
								int num3 = piece.renderingIndirectTransformIndex[i];
								int num4 = this.renderData.staticBatch.instanceTransformIndexToDataIndex[num3];
								if (num4 >= 0)
								{
									for (int j = 0; j < 2; j++)
									{
										this.renderData.staticBatch.instanceTint[num4 + j] = tint;
									}
								}
							}
							else
							{
								for (int k = 0; k < 1; k++)
								{
									int num5 = num2 + k;
									BuilderTableMeshInstances builderTableMeshInstances = this.renderData.dynamicBatch.renderMeshes[num5];
									for (int l = 0; l < builderTableMeshInstances.transforms.length; l++)
									{
										if (builderTableMeshInstances.transforms[l] == transform)
										{
											builderTableMeshInstances.tint[l] = tint;
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x04002F46 RID: 12102
	public Material sharedMaterialBase;

	// Token: 0x04002F47 RID: 12103
	public Material sharedMaterialIndirectBase;

	// Token: 0x04002F48 RID: 12104
	public const int TEX_SIZE = 256;

	// Token: 0x04002F49 RID: 12105
	private Shader snapPieceShader;

	// Token: 0x04002F4A RID: 12106
	public BuilderTableDataRenderData renderData;

	// Token: 0x04002F4B RID: 12107
	[SerializeField]
	[HideInInspector]
	private List<Mesh> serializeMeshToIndexKeys;

	// Token: 0x04002F4C RID: 12108
	[SerializeField]
	[HideInInspector]
	private List<int> serializeMeshToIndexValues;

	// Token: 0x04002F4D RID: 12109
	[SerializeField]
	[HideInInspector]
	private List<Mesh> serializeMeshes;

	// Token: 0x04002F4E RID: 12110
	[SerializeField]
	[HideInInspector]
	private List<int> serializeMeshInstanceCount;

	// Token: 0x04002F4F RID: 12111
	[SerializeField]
	[HideInInspector]
	private List<BuilderTableSubMesh> serializeSubMeshes;

	// Token: 0x04002F50 RID: 12112
	[SerializeField]
	[HideInInspector]
	private Mesh serializeSharedMesh;

	// Token: 0x04002F51 RID: 12113
	[SerializeField]
	[HideInInspector]
	private List<Texture2D> serializeTextureToIndexKeys;

	// Token: 0x04002F52 RID: 12114
	[SerializeField]
	[HideInInspector]
	private List<int> serializeTextureToIndexValues;

	// Token: 0x04002F53 RID: 12115
	[SerializeField]
	[HideInInspector]
	private List<Texture2D> serializeTextures;

	// Token: 0x04002F54 RID: 12116
	[SerializeField]
	[HideInInspector]
	private List<Material> serializePerTextureMaterial;

	// Token: 0x04002F55 RID: 12117
	[SerializeField]
	[HideInInspector]
	private List<MaterialPropertyBlock> serializePerTexturePropertyBlock;

	// Token: 0x04002F56 RID: 12118
	[SerializeField]
	[HideInInspector]
	private Texture2DArray serializeSharedTexArray;

	// Token: 0x04002F57 RID: 12119
	[SerializeField]
	[HideInInspector]
	private Material serializeSharedMaterial;

	// Token: 0x04002F58 RID: 12120
	[SerializeField]
	[HideInInspector]
	private Material serializeSharedMaterialIndirect;

	// Token: 0x04002F59 RID: 12121
	private const string texturePropName = "_BaseMap";

	// Token: 0x04002F5A RID: 12122
	private const string textureArrayPropName = "_BaseMapArray";

	// Token: 0x04002F5B RID: 12123
	private const string textureArrayIndexPropName = "_BaseMapArrayIndex";

	// Token: 0x04002F5C RID: 12124
	private const string transformMatrixPropName = "_TransformMatrix";

	// Token: 0x04002F5D RID: 12125
	private const string texIndexPropName = "_TexIndex";

	// Token: 0x04002F5E RID: 12126
	private const string tintPropName = "_Tint";

	// Token: 0x04002F5F RID: 12127
	public const int MAX_STATIC_INSTANCES = 8192;

	// Token: 0x04002F60 RID: 12128
	public const int MAX_DYNAMIC_INSTANCES = 8192;

	// Token: 0x04002F61 RID: 12129
	public const int INSTANCES_PER_TRANSFORM = 2;

	// Token: 0x04002F62 RID: 12130
	private bool initialized;

	// Token: 0x04002F63 RID: 12131
	private bool built;

	// Token: 0x04002F64 RID: 12132
	private bool showing;

	// Token: 0x04002F65 RID: 12133
	private static List<MeshRenderer> meshRenderers = new List<MeshRenderer>(128);

	// Token: 0x04002F66 RID: 12134
	private const int MAX_TOTAL_VERTS = 65536;

	// Token: 0x04002F67 RID: 12135
	private const int MAX_TOTAL_TRIS = 65536;

	// Token: 0x04002F68 RID: 12136
	private static List<Vector3> verticesAll = new List<Vector3>(65536);

	// Token: 0x04002F69 RID: 12137
	private static List<Vector3> normalsAll = new List<Vector3>(65536);

	// Token: 0x04002F6A RID: 12138
	private static List<Vector2> uv1All = new List<Vector2>(65536);

	// Token: 0x04002F6B RID: 12139
	private static List<int> trianglesAll = new List<int>(65536);

	// Token: 0x04002F6C RID: 12140
	private static List<Vector3> vertices = new List<Vector3>(65536);

	// Token: 0x04002F6D RID: 12141
	private static List<Vector3> normals = new List<Vector3>(65536);

	// Token: 0x04002F6E RID: 12142
	private static List<Vector2> uv1 = new List<Vector2>(65536);

	// Token: 0x04002F6F RID: 12143
	private static List<int> triangles = new List<int>(65536);

	// Token: 0x02000595 RID: 1429
	[BurstCompile]
	public struct SetupInstanceDataForMesh : IJobParallelForTransform
	{
		// Token: 0x06002427 RID: 9255 RVA: 0x000C1998 File Offset: 0x000BFB98
		public void Execute(int index, TransformAccess transform)
		{
			int num = index + (int)this.commandData.startInstance;
			this.objectToWorld[num] = transform.localToWorldMatrix;
			this.instanceTexIndex[num] = this.texIndex[index];
			this.instanceTint[num] = this.tint[index];
		}

		// Token: 0x04002F70 RID: 12144
		[ReadOnly]
		public NativeList<int> texIndex;

		// Token: 0x04002F71 RID: 12145
		[ReadOnly]
		public NativeList<float> tint;

		// Token: 0x04002F72 RID: 12146
		[ReadOnly]
		public GraphicsBuffer.IndirectDrawIndexedArgs commandData;

		// Token: 0x04002F73 RID: 12147
		[ReadOnly]
		public Vector3 cameraPos;

		// Token: 0x04002F74 RID: 12148
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<int> instanceTexIndex;

		// Token: 0x04002F75 RID: 12149
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<Matrix4x4> objectToWorld;

		// Token: 0x04002F76 RID: 12150
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<float> instanceTint;

		// Token: 0x04002F77 RID: 12151
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<int> lodLevel;

		// Token: 0x04002F78 RID: 12152
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<int> lodDirty;
	}

	// Token: 0x02000596 RID: 1430
	[BurstCompile]
	public struct SetupInstanceDataForMeshStatic : IJobParallelForTransform
	{
		// Token: 0x06002428 RID: 9256 RVA: 0x000C19F8 File Offset: 0x000BFBF8
		public void Execute(int index, TransformAccess transform)
		{
			if (transform.isValid)
			{
				int num = this.transformIndexToDataIndex[index];
				for (int i = 0; i < 2; i++)
				{
					this.objectToWorld[num + i] = transform.localToWorldMatrix;
				}
			}
		}

		// Token: 0x04002F79 RID: 12153
		[ReadOnly]
		public NativeArray<int> transformIndexToDataIndex;

		// Token: 0x04002F7A RID: 12154
		[NativeDisableContainerSafetyRestriction]
		public NativeArray<Matrix4x4> objectToWorld;
	}
}
