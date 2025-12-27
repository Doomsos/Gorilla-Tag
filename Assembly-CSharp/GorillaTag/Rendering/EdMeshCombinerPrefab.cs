using System;
using System.Collections.Generic;
using GorillaExtensions;
using TMPro;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace GorillaTag.Rendering
{
	[DefaultExecutionOrder(-2147482648)]
	public class EdMeshCombinerPrefab : MonoBehaviour
	{
		private void Awake()
		{
			if (this.combinedData == null)
			{
				this.combinedData = new EdMeshCombinedPrefabData();
			}
			EdMeshCombinerPrefab.CombineMeshesRuntime(this, false, this.combinedData);
		}

		private static void Special_MarkDoNotCombine(Component component)
		{
			if (component != null)
			{
				GameObject gameObject = component.gameObject;
				if (gameObject.GetComponent<EdDoNotMeshCombine>() == null)
				{
					gameObject.AddComponent<EdDoNotMeshCombine>();
				}
			}
		}

		public static void CombineMeshesRuntime(EdMeshCombinerPrefab combiner, bool undo = false, EdMeshCombinedPrefabData combinedPrefabData = null)
		{
			bool flag = true;
			foreach (Campfire campfire in combiner.GetComponentsInChildren<Campfire>(true))
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(campfire.baseFire);
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(campfire.middleFire);
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(campfire.topFire);
			}
			GameEntity[] componentsInChildren2 = combiner.GetComponentsInChildren<GameEntity>(true);
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChildren2[i]);
			}
			StaticLodGroup[] componentsInChildren3 = combiner.GetComponentsInChildren<StaticLodGroup>(true);
			for (int i = 0; i < componentsInChildren3.Length; i++)
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChildren3[i]);
			}
			GorillaCaveCrystalVisuals[] componentsInChildren4 = combiner.GetComponentsInChildren<GorillaCaveCrystalVisuals>(false);
			for (int i = 0; i < componentsInChildren4.Length; i++)
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChildren4[i]);
			}
			WaterSurfaceMaterialController[] componentsInChildren5 = combiner.GetComponentsInChildren<WaterSurfaceMaterialController>(false);
			for (int i = 0; i < componentsInChildren5.Length; i++)
			{
				EdMeshCombinerPrefab.Special_MarkDoNotCombine(componentsInChildren5[i]);
			}
			List<Renderer> componentsInChildrenUntil = combiner.GetComponentsInChildrenUntil(false, false, 64);
			List<Renderer> list = new List<Renderer>(componentsInChildrenUntil.Count);
			foreach (Renderer renderer in componentsInChildrenUntil)
			{
				if (renderer is SkinnedMeshRenderer || renderer is MeshRenderer)
				{
					list.Add(renderer);
				}
			}
			Dictionary<EdMeshCombinerPrefab.CombinerCriteria, List<List<EdMeshCombinerPrefab.CombinerInfo>>> dictionary = new Dictionary<EdMeshCombinerPrefab.CombinerCriteria, List<List<EdMeshCombinerPrefab.CombinerInfo>>>(list.Count);
			List<Transform> list2 = new List<Transform>(list.Count);
			EdMeshCombinerPrefab.CombinerCriteria combinerCriteria;
			foreach (Renderer renderer2 in list)
			{
				if (renderer2.enabled)
				{
					GameObject gameObject = renderer2.gameObject;
					int staticFlags = gameObject.isStatic ? 1 : 0;
					if (gameObject.isStatic)
					{
						SkinnedMeshRenderer skinnedMeshRenderer = renderer2 as SkinnedMeshRenderer;
						bool flag2 = skinnedMeshRenderer != null;
						MeshFilter meshFilter = null;
						Mesh sharedMesh;
						if (flag2)
						{
							sharedMesh = skinnedMeshRenderer.sharedMesh;
						}
						else
						{
							meshFilter = renderer2.GetComponent<MeshFilter>();
							if (meshFilter == null)
							{
								continue;
							}
							sharedMesh = meshFilter.sharedMesh;
						}
						if (!(sharedMesh == null) && (long)sharedMesh.vertexCount < 65535L)
						{
							MeshCollider component = renderer2.GetComponent<MeshCollider>();
							bool flag3 = component != null;
							if (flag || !flag3 || (!(component.sharedMesh == null) && !component.convex && !(component.sharedMesh != sharedMesh)))
							{
								GorillaSurfaceOverride component2 = renderer2.GetComponent<GorillaSurfaceOverride>();
								int num = (component2 != null) ? component2.overrideIndex : 0;
								int num2 = Mathf.Min(renderer2.sharedMaterials.Length, sharedMesh.subMeshCount);
								if (num2 != 0)
								{
									int num3 = 0;
									int num4 = 0;
									for (int j = 0; j < num2; j++)
									{
										num3 += ((sharedMesh.GetSubMesh(j).topology != null) ? 1 : 0);
										num4 += ((renderer2.sharedMaterials[j] == null) ? 1 : 0);
									}
									if (num3 > 0)
									{
										string text = "?????";
										Debug.LogError(string.Concat(new string[]
										{
											string.Format("Cannot combine mesh \"{0}\" because it has {1} submeshes with ", sharedMesh.name, num3),
											"a non-triangle topology. Verify FBX import settings does not have \"Keep Quads\" on.\n  - Asset path=\"",
											text,
											"\"\n  - Path in scene=",
											renderer2.transform.GetPathQ()
										}), sharedMesh);
									}
									else if (num4 > 0)
									{
										Debug.LogError("EdMeshCombinerPrefab: Cannot combine Renderer \"" + combiner.name + "\" because it does not have " + string.Format("{0} materials assigned. Path in scene={1}", num4, combiner.transform.GetPathQ()), combiner);
									}
									else
									{
										for (int k = 0; k < num2; k++)
										{
											Material mat = renderer2.sharedMaterials[k];
											int layer = renderer2.gameObject.layer;
											combinerCriteria = new EdMeshCombinerPrefab.CombinerCriteria
											{
												mat = mat,
												staticFlags = staticFlags,
												lightmapIndex = renderer2.lightmapIndex,
												hasMeshCollider = (!flag && flag3),
												meshCollPhysicsMat = (flag ? null : (flag3 ? component.sharedMaterial : null)),
												surfOverrideIndex = (flag ? 0 : num),
												surfExtraVelMultiplier = (flag ? 0f : ((component2 != null) ? component2.extraVelMultiplier : 1f)),
												surfExtraVelMaxMultiplier = (flag ? 0f : ((component2 != null) ? component2.extraVelMaxMultiplier : 1f)),
												surfSendOnTapEvent = (!flag && component2 != null && component2.sendOnTapEvent),
												objectLayer = ((layer == 27) ? UnityLayer.NoMirror : UnityLayer.Default)
											};
											EdMeshCombinerPrefab.CombinerCriteria combinerCriteria2 = combinerCriteria;
											List<List<EdMeshCombinerPrefab.CombinerInfo>> list3;
											if (!dictionary.TryGetValue(combinerCriteria2, ref list3))
											{
												List<List<EdMeshCombinerPrefab.CombinerInfo>> list4 = new List<List<EdMeshCombinerPrefab.CombinerInfo>>();
												list4.Add(new List<EdMeshCombinerPrefab.CombinerInfo>(1));
												list3 = list4;
												dictionary[combinerCriteria2] = list3;
											}
											int num5 = list3.Count - 1;
											int num6 = sharedMesh.vertexCount;
											foreach (EdMeshCombinerPrefab.CombinerInfo combinerInfo in list3[num5])
											{
												if (combinerInfo.isSkinnedMesh)
												{
													SkinnedMeshRenderer skinnedMeshRenderer2 = (SkinnedMeshRenderer)combinerInfo.renderer;
													num6 += skinnedMeshRenderer2.sharedMesh.vertexCount;
												}
												else
												{
													num6 += combinerInfo.meshFilter.sharedMesh.vertexCount;
												}
											}
											if ((long)num6 >= 65535L)
											{
												num5 = list3.Count;
												list3.Add(new List<EdMeshCombinerPrefab.CombinerInfo>(1));
											}
											list2.Add(gameObject.transform);
											list3[num5].Add(new EdMeshCombinerPrefab.CombinerInfo
											{
												meshFilter = meshFilter,
												renderer = renderer2,
												uvOffsetModifier = renderer2.GetComponent<EdMeshCombinerModifierUVOffset>(),
												subMeshIndex = k,
												isSkinnedMesh = flag2,
												layer = renderer2.sortingLayerID
											});
										}
									}
								}
							}
						}
					}
				}
			}
			Matrix4x4 worldToLocalMatrix = combiner.transform.worldToLocalMatrix;
			PerSceneRenderData perSceneRenderData = null;
			bool flag4 = false;
			new Random(6746U);
			foreach (KeyValuePair<EdMeshCombinerPrefab.CombinerCriteria, List<List<EdMeshCombinerPrefab.CombinerInfo>>> keyValuePair in dictionary)
			{
				List<List<EdMeshCombinerPrefab.CombinerInfo>> list5;
				keyValuePair.Deconstruct(ref combinerCriteria, ref list5);
				EdMeshCombinerPrefab.CombinerCriteria combinerCriteria3 = combinerCriteria;
				List<List<EdMeshCombinerPrefab.CombinerInfo>> list6 = list5;
				bool isCandleFlame = false;
				foreach (List<EdMeshCombinerPrefab.CombinerInfo> list7 in list6)
				{
					List<Mesh> list8 = new List<Mesh>(list7.Count);
					List<int> list9 = new List<int>(list7.Count);
					List<Matrix4x4> list10 = new List<Matrix4x4>(list7.Count);
					List<Color> list11 = new List<Color>(list7.Count);
					List<int> list12 = new List<int>(list7.Count);
					List<float4> list13 = new List<float4>(list7.Count);
					List<float4> list14 = new List<float4>(list7.Count);
					Dictionary<ValueTuple<Renderer, int>, ValueTuple<Color, int>> dictionary2 = new Dictionary<ValueTuple<Renderer, int>, ValueTuple<Color, int>>();
					foreach (EdMeshCombinerPrefab.CombinerInfo combinerInfo2 in list7)
					{
						MaterialCombinerPerRendererMono materialCombinerPerRendererMono;
						MaterialCombinerPerRendererInfo materialCombinerPerRendererInfo;
						if (combinerInfo2.renderer.TryGetComponent<MaterialCombinerPerRendererMono>(ref materialCombinerPerRendererMono) && materialCombinerPerRendererMono.TryGetData(combinerInfo2.renderer, combinerInfo2.subMeshIndex, out materialCombinerPerRendererInfo))
						{
							dictionary2[new ValueTuple<Renderer, int>(combinerInfo2.renderer, combinerInfo2.subMeshIndex)] = new ValueTuple<Color, int>(materialCombinerPerRendererInfo.baseColor, materialCombinerPerRendererInfo.sliceIndex);
						}
						else
						{
							dictionary2[new ValueTuple<Renderer, int>(combinerInfo2.renderer, combinerInfo2.subMeshIndex)] = new ValueTuple<Color, int>(Color.white, -1);
						}
					}
					for (int l = 0; l < list7.Count; l++)
					{
						EdMeshCombinerPrefab.CombinerInfo combinerInfo3 = list7[l];
						Mesh mesh;
						if (combinerInfo3.isSkinnedMesh)
						{
							SkinnedMeshRenderer skinnedMeshRenderer3 = (SkinnedMeshRenderer)combinerInfo3.renderer;
							mesh = new Mesh();
							skinnedMeshRenderer3.BakeMesh(mesh, true);
						}
						else
						{
							mesh = combinerInfo3.meshFilter.sharedMesh;
						}
						if (mesh.vertexCount != 0)
						{
							if (perSceneRenderData != null && perSceneRenderData.representativeRenderer == combinerInfo3.renderer)
							{
								flag4 = true;
							}
							list8.Add(mesh);
							list9.Add(combinerInfo3.subMeshIndex);
							list10.Add(worldToLocalMatrix * combinerInfo3.renderer.transform.localToWorldMatrix);
							list14.Add((combinerInfo3.uvOffsetModifier == null) ? float4.zero : new float4(combinerInfo3.uvOffsetModifier.minUvOffset.x, combinerInfo3.uvOffsetModifier.minUvOffset.y, combinerInfo3.uvOffsetModifier.maxUvOffset.x, combinerInfo3.uvOffsetModifier.maxUvOffset.y));
							list13.Add(combinerInfo3.renderer.lightmapScaleOffset);
							ValueTuple<Color, int> valueTuple = dictionary2[new ValueTuple<Renderer, int>(combinerInfo3.renderer, combinerInfo3.subMeshIndex)];
							Color item = valueTuple.Item1;
							int item2 = valueTuple.Item2;
							list11.Add(item);
							list12.Add(item2);
						}
					}
					using (Mesh.MeshDataArray meshDataArray = Mesh.AcquireReadOnlyMeshData(list8))
					{
						int num7 = 0;
						int num8 = 0;
						for (int m = 0; m < meshDataArray.Length; m++)
						{
							Mesh.MeshData meshData = meshDataArray[m];
							num7 += meshData.vertexCount;
							num8 += meshData.GetSubMesh(list9[m]).indexCount;
						}
						Mesh.MeshDataArray meshDataArray2 = Mesh.AllocateWritableMeshData(1);
						Mesh.MeshData meshData2 = meshDataArray2[0];
						IndexFormat indexFormat = (num7 > 65535) ? 1 : 0;
						GTVertexDataStreams_Descriptors.DoSetVertexBufferParams(ref meshData2, num7);
						meshData2.SetIndexBufferParams(num8, indexFormat);
						meshData2.subMeshCount = 1;
						NativeArray<int> idxDst = default(NativeArray<int>);
						NativeArray<ushort> idxDst2 = default(NativeArray<ushort>);
						if (indexFormat == 1)
						{
							idxDst = meshData2.GetIndexData<int>();
						}
						else
						{
							idxDst2 = meshData2.GetIndexData<ushort>();
						}
						EdMeshCombinerPrefab.CopyMeshJob copyMeshJob = new EdMeshCombinerPrefab.CopyMeshJob
						{
							meshDataArray = meshDataArray,
							sourceSubmeshIndices = new NativeArray<int>(list9.ToArray(), 3),
							sourceTransforms = new NativeArray<Matrix4x4>(list10.ToArray(), 3),
							lightmapScaleOffsets = new NativeArray<float4>(list13.ToArray(), 3),
							baseColors = new NativeArray<Color>(list11.ToArray(), 3),
							atlasSlices = new NativeArray<int>(list12.ToArray(), 3),
							uvModifiersMinMax = new NativeArray<float4>(list14.ToArray(), 3),
							isCandleFlame = isCandleFlame,
							randSeed = 6746U,
							dst0 = meshData2.GetVertexData<GTVertexDataStream0>(0),
							dst1 = meshData2.GetVertexData<GTVertexDataStream1>(1),
							idxDst32 = idxDst,
							idxDst16 = idxDst2,
							use32BitIndices = (indexFormat == 1)
						};
						IJobExtensions.Schedule<EdMeshCombinerPrefab.CopyMeshJob>(copyMeshJob, default(JobHandle)).Complete();
						copyMeshJob.sourceSubmeshIndices.Dispose();
						copyMeshJob.sourceTransforms.Dispose();
						copyMeshJob.baseColors.Dispose();
						copyMeshJob.atlasSlices.Dispose();
						copyMeshJob.uvModifiersMinMax.Dispose();
						meshData2.SetSubMesh(0, new SubMeshDescriptor(0, num8, 0), 0);
						Mesh mesh2 = new Mesh();
						Mesh.ApplyAndDisposeWritableMeshData(meshDataArray2, mesh2, 0);
						mesh2.RecalculateBounds();
						GameObject gameObject2 = new GameObject(combinerCriteria3.mat.name + " (combined by EdMeshCombinerPrefab)");
						if (combinedPrefabData != null)
						{
							combinedPrefabData.combined.Add(gameObject2);
						}
						if (combiner.transform != null)
						{
							gameObject2.transform.parent = combiner.transform;
						}
						else
						{
							SceneManager.MoveGameObjectToScene(gameObject2, combiner.gameObject.scene);
						}
						gameObject2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
						gameObject2.transform.localScale = Vector3.one;
						gameObject2.isStatic = true;
						gameObject2.layer = (int)combinerCriteria3.objectLayer;
						MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
						meshRenderer.sharedMaterial = combinerCriteria3.mat;
						meshRenderer.lightmapIndex = combinerCriteria3.lightmapIndex;
						if (flag4)
						{
							perSceneRenderData.representativeRenderer = meshRenderer;
						}
						if (perSceneRenderData != null)
						{
							perSceneRenderData.AddMeshToList(gameObject2, meshRenderer);
						}
						MeshFilter meshFilter2 = gameObject2.AddComponent<MeshFilter>();
						meshFilter2.sharedMesh = mesh2;
						if (!flag && combinerCriteria3.hasMeshCollider)
						{
							MeshCollider meshCollider = gameObject2.AddComponent<MeshCollider>();
							meshCollider.sharedMesh = meshFilter2.sharedMesh;
							meshCollider.convex = false;
							meshCollider.sharedMaterial = combinerCriteria3.meshCollPhysicsMat;
							GorillaSurfaceOverride gorillaSurfaceOverride = gameObject2.AddComponent<GorillaSurfaceOverride>();
							gorillaSurfaceOverride.overrideIndex = combinerCriteria3.surfOverrideIndex;
							gorillaSurfaceOverride.extraVelMultiplier = combinerCriteria3.surfExtraVelMultiplier;
							gorillaSurfaceOverride.extraVelMaxMultiplier = combinerCriteria3.surfExtraVelMaxMultiplier;
							gorillaSurfaceOverride.sendOnTapEvent = combinerCriteria3.surfSendOnTapEvent;
						}
					}
				}
			}
			list2.Sort((Transform a, Transform b) => -a.GetDepth().CompareTo(b.GetDepth()));
			foreach (Transform transform in list2)
			{
				if (!(transform == null) && combinedPrefabData != null)
				{
					MeshRenderer component3 = transform.GetComponent<MeshRenderer>();
					if (component3 != null)
					{
						component3.enabled = false;
						combinedPrefabData.disabled.Add(component3);
					}
					SkinnedMeshRenderer component4 = transform.GetComponent<SkinnedMeshRenderer>();
					if (component4 != null)
					{
						component4.enabled = false;
						combinedPrefabData.disabled.Add(component4);
					}
				}
			}
		}

		protected void OnEnable()
		{
		}

		public EdMeshCombinedPrefabData combinedData;

		private const uint _k_maxVertsForUInt16 = 65535U;

		private const uint _k_maxVertsForUInt32 = 4294967295U;

		private const uint _k_maxVertCount = 65535U;

		[Serializable]
		public struct CombinerInfo
		{
			public MeshFilter meshFilter;

			public Renderer renderer;

			public EdMeshCombinerModifierUVOffset uvOffsetModifier;

			public int subMeshIndex;

			public bool isSkinnedMesh;

			public int layer;
		}

		private struct CombinerCriteria
		{
			public override int GetHashCode()
			{
				return HashCode.Combine<int, int, int, bool, int, float, float, bool>(this.mat.GetInstanceID(), this.staticFlags, this.lightmapIndex, this.hasMeshCollider, this.surfOverrideIndex, this.surfExtraVelMultiplier, this.surfExtraVelMaxMultiplier, this.surfSendOnTapEvent);
			}

			public Material mat;

			public int staticFlags;

			public int lightmapIndex;

			public bool hasMeshCollider;

			public PhysicsMaterial meshCollPhysicsMat;

			public int surfOverrideIndex;

			public float surfExtraVelMultiplier;

			public float surfExtraVelMaxMultiplier;

			public bool surfSendOnTapEvent;

			public UnityLayer objectLayer;
		}

		[BurstCompile]
		private struct CopyMeshJob : IJob
		{
			public void Execute()
			{
				int num = 0;
				int num2 = 0;
				Random random;
				random..ctor(this.randSeed);
				for (int i = 0; i < this.meshDataArray.Length; i++)
				{
					Mesh.MeshData meshData = this.meshDataArray[i];
					int num3 = this.sourceSubmeshIndices[i];
					SubMeshDescriptor subMesh = meshData.GetSubMesh(num3);
					int vertexCount = meshData.vertexCount;
					int indexCount = subMesh.indexCount;
					Matrix4x4 matrix4x = this.sourceTransforms[i];
					bool flag = math.determinant(matrix4x) < 0f;
					NativeArray<Vector3> nativeArray = new NativeArray<Vector3>(vertexCount, 2, 0);
					if (meshData.HasVertexAttribute(0))
					{
						meshData.GetVertices(nativeArray);
					}
					else
					{
						for (int j = 0; j < vertexCount; j++)
						{
							nativeArray[j] = Vector3.zero;
						}
					}
					NativeArray<Vector3> nativeArray2 = new NativeArray<Vector3>(vertexCount, 2, 1);
					if (meshData.HasVertexAttribute(1))
					{
						meshData.GetNormals(nativeArray2);
					}
					else
					{
						for (int k = 0; k < vertexCount; k++)
						{
							nativeArray2[k] = Vector3.up;
						}
					}
					NativeArray<Vector4> nativeArray3 = new NativeArray<Vector4>(vertexCount, 2, 1);
					if (meshData.HasVertexAttribute(2))
					{
						meshData.GetTangents(nativeArray3);
					}
					else
					{
						for (int l = 0; l < vertexCount; l++)
						{
							nativeArray3[l] = new Vector4(1f, 0f, 0f, 1f);
						}
					}
					NativeArray<Color> nativeArray4 = new NativeArray<Color>(vertexCount, 2, 1);
					if (meshData.HasVertexAttribute(3))
					{
						meshData.GetColors(nativeArray4);
					}
					else
					{
						for (int m = 0; m < vertexCount; m++)
						{
							nativeArray4[m] = Color.white;
						}
					}
					NativeArray<Vector2> nativeArray5 = new NativeArray<Vector2>(vertexCount, 2, 1);
					if (meshData.HasVertexAttribute(4))
					{
						meshData.GetUVs(0, nativeArray5);
					}
					else
					{
						for (int n = 0; n < vertexCount; n++)
						{
							nativeArray5[n] = Vector2.zero;
						}
					}
					NativeArray<Vector2> nativeArray6 = new NativeArray<Vector2>(vertexCount, 2, 1);
					if (meshData.HasVertexAttribute(5))
					{
						meshData.GetUVs(1, nativeArray6);
					}
					else
					{
						for (int num4 = 0; num4 < vertexCount; num4++)
						{
							nativeArray6[num4] = Vector2.zero;
						}
					}
					Color color = this.baseColors[i];
					int num5 = this.atlasSlices[i];
					Vector4 vector = this.uvModifiersMinMax[i];
					Vector2 vector2;
					vector2..ctor(random.NextFloat(vector.x, vector.z), random.NextFloat(vector.y, vector.w));
					float num6 = this.isCandleFlame ? random.NextFloat(0f, 1f) : 1f;
					Matrix4x4 transpose = matrix4x.inverse.transpose;
					for (int num7 = 0; num7 < vertexCount; num7++)
					{
						Vector3 vector3 = nativeArray[num7];
						Vector3 vector4 = nativeArray2[num7];
						Vector4 vector5 = nativeArray3[num7];
						Color color2 = nativeArray4[num7];
						Vector2 vector6 = nativeArray5[num7];
						Vector3 vector7 = matrix4x.MultiplyPoint3x4(vector3);
						Vector3 vector8 = transpose.MultiplyVector(vector4).normalized;
						Vector3 vector9 = transpose.MultiplyVector(new Vector3(vector5.x, vector5.y, vector5.z)).normalized;
						if (flag)
						{
							vector8 = -vector8;
							vector9 = -vector9;
							vector5.w = -vector5.w;
						}
						GTVertexDataStream0 gtvertexDataStream = new GTVertexDataStream0
						{
							position = vector7,
							color = new Color(color2.r * color.r, color2.g * color.g, color2.b * color.b, this.isCandleFlame ? num6 : (color2.a * color.a)),
							uv1 = new half4((half)(vector6.x + vector2.x), (half)(vector6.y + vector2.y), (half)((float)num5), (half)num6),
							lightmapUv = new half2((half)(nativeArray6[num7].x * this.lightmapScaleOffsets[i].x + this.lightmapScaleOffsets[i].z), (half)(nativeArray6[num7].y * this.lightmapScaleOffsets[i].y + this.lightmapScaleOffsets[i].w))
						};
						this.dst0[num + num7] = gtvertexDataStream;
						GTVertexDataStream1 gtvertexDataStream2 = new GTVertexDataStream1
						{
							normal = vector8,
							tangent = new Color(vector9.x, vector9.y, vector9.z, vector5.w)
						};
						this.dst1[num + num7] = gtvertexDataStream2;
					}
					if (this.use32BitIndices)
					{
						NativeArray<int> nativeArray7 = new NativeArray<int>(indexCount, 2, 1);
						meshData.GetIndices(nativeArray7, num3, true);
						if (!flag)
						{
							for (int num8 = 0; num8 < indexCount; num8++)
							{
								this.idxDst32[num2 + num8] = num + nativeArray7[num8];
							}
						}
						else
						{
							for (int num9 = 0; num9 < indexCount; num9 += 3)
							{
								this.idxDst32[num2 + num9] = num + nativeArray7[num9 + 2];
								this.idxDst32[num2 + num9 + 1] = num + nativeArray7[num9 + 1];
								this.idxDst32[num2 + num9 + 2] = num + nativeArray7[num9];
							}
						}
						nativeArray7.Dispose();
					}
					else
					{
						NativeArray<ushort> nativeArray8 = new NativeArray<ushort>(indexCount, 2, 1);
						meshData.GetIndices(nativeArray8, num3, true);
						if (!flag)
						{
							for (int num10 = 0; num10 < indexCount; num10++)
							{
								this.idxDst16[num2 + num10] = (ushort)(num + (int)nativeArray8[num10]);
							}
						}
						else
						{
							for (int num11 = 0; num11 < indexCount; num11 += 3)
							{
								this.idxDst16[num2 + num11] = (ushort)(num + (int)nativeArray8[num11 + 2]);
								this.idxDst16[num2 + num11 + 1] = (ushort)(num + (int)nativeArray8[num11 + 1]);
								this.idxDst16[num2 + num11 + 2] = (ushort)(num + (int)nativeArray8[num11]);
							}
						}
						nativeArray8.Dispose();
					}
					nativeArray.Dispose();
					nativeArray2.Dispose();
					nativeArray3.Dispose();
					nativeArray4.Dispose();
					nativeArray5.Dispose();
					nativeArray6.Dispose();
					num += vertexCount;
					num2 += indexCount;
				}
			}

			[ReadOnly]
			public Mesh.MeshDataArray meshDataArray;

			[ReadOnly]
			public NativeArray<int> sourceSubmeshIndices;

			[ReadOnly]
			public NativeArray<Matrix4x4> sourceTransforms;

			[ReadOnly]
			public NativeArray<float4> lightmapScaleOffsets;

			[ReadOnly]
			public NativeArray<Color> baseColors;

			[ReadOnly]
			public NativeArray<int> atlasSlices;

			[ReadOnly]
			public NativeArray<float4> uvModifiersMinMax;

			public bool isCandleFlame;

			public uint randSeed;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<GTVertexDataStream0> dst0;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<GTVertexDataStream1> dst1;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<int> idxDst32;

			[WriteOnly]
			[NativeDisableContainerSafetyRestriction]
			public NativeArray<ushort> idxDst16;

			public bool use32BitIndices;
		}
	}
}
