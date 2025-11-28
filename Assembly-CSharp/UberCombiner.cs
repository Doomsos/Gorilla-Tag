using System;
using System.Collections.Generic;
using System.Linq;
using GorillaTag.Rendering;
using MTAssets.EasyMeshCombiner;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

// Token: 0x02000CD6 RID: 3286
[RequireComponent(typeof(RuntimeMeshCombiner))]
public class UberCombiner : MonoBehaviour
{
	// Token: 0x06005029 RID: 20521 RVA: 0x0019C1B0 File Offset: 0x0019A3B0
	private void CollectRenderers()
	{
		MeshRenderer[] array = Enumerable.ToArray<MeshRenderer>(UberCombiner.FilterRenderers(Enumerable.ToArray<MeshRenderer>(Enumerable.SelectMany<GameObject, MeshRenderer>(this.meshSources, (GameObject g) => g.GetComponentsInChildren<MeshRenderer>(this.includeInactive)))).DistinctBy((MeshRenderer mr) => mr.GetInstanceID()));
		this.renderersToCombine = array;
		string.Format("Found {0} renderers to combine.", array.Length).Echo<string>();
	}

	// Token: 0x0600502A RID: 20522 RVA: 0x0019C228 File Offset: 0x0019A428
	private void ValidateRenderers()
	{
		List<GameObject> list = new List<GameObject>(16);
		for (int i = 0; i < this.renderersToCombine.Length; i++)
		{
			MeshRenderer meshRenderer = this.renderersToCombine[i];
			GameObject gameObject = meshRenderer.gameObject;
			string name = gameObject.name;
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			if (meshRenderer == null || component == null)
			{
				Debug.LogError("Ojbect '" + name + "' is missing a MeshRenderer, MeshFilter, or both.", gameObject);
				list.Add(gameObject);
			}
			else
			{
				Mesh sharedMesh = component.sharedMesh;
				if (sharedMesh == null)
				{
					Debug.LogError("MeshFilter for '" + name + "' has no shared mesh.", gameObject);
					list.Add(gameObject);
				}
				else
				{
					int subMeshCount = sharedMesh.subMeshCount;
					if (subMeshCount == 0)
					{
						Debug.LogError("Shared mesh for '" + name + "' has 0 submeshes.", gameObject);
						list.Add(gameObject);
					}
					else if (sharedMesh.vertexCount < 3)
					{
						Debug.LogError("Shared mesh for '" + name + "' has less than 3 vertices.", gameObject);
						list.Add(gameObject);
					}
					else
					{
						Material[] sharedMaterials = meshRenderer.sharedMaterials;
						if (sharedMaterials.IsNullOrEmpty<Material>())
						{
							Debug.LogError("Object '" + name + "' has null or empty shared materials array.", gameObject);
							list.Add(gameObject);
						}
						else
						{
							foreach (Material material in sharedMaterials)
							{
								string name2 = material.name;
								Texture mainTexture = material.mainTexture;
								if (!(mainTexture == null) && mainTexture is RenderTexture)
								{
									Debug.LogError(string.Concat(new string[]
									{
										"Object '",
										name,
										"' has material (",
										name2,
										") that uses a RenderTexture"
									}), gameObject);
									list.Add(gameObject);
									break;
								}
								if (material.HasProperty(UberCombiner._BaseMap))
								{
									Texture texture = material.GetTexture(UberCombiner._BaseMap);
									if (!(texture == null) && texture is RenderTexture)
									{
										Debug.LogError(string.Concat(new string[]
										{
											"Object '",
											name,
											"' has material (",
											name2,
											") that uses a RenderTexture"
										}), gameObject);
										list.Add(gameObject);
										break;
									}
								}
								if (UberShader.IsAnimated(material))
								{
									Debug.LogError(string.Concat(new string[]
									{
										"Object '",
										name,
										"' has a material (",
										name2,
										") that's animated"
									}), gameObject);
									list.Add(gameObject);
									break;
								}
							}
							if (subMeshCount != sharedMaterials.Length)
							{
								Debug.LogError("Object '" + name + "' has mismatched number of materials/submeshes" + string.Format(" Submeshes: {0} Materials: {1}", subMeshCount, sharedMaterials.Length), gameObject);
								list.Add(gameObject);
							}
						}
					}
				}
			}
		}
		this.invalidObjects = Enumerable.ToList<GameObject>(list.DistinctBy((GameObject g) => g.GetHashCode()));
	}

	// Token: 0x0600502B RID: 20523 RVA: 0x0019C524 File Offset: 0x0019A724
	private void SendToCombiner()
	{
		List<GameObject> targetMeshes = Enumerable.ToList<GameObject>(Enumerable.Where<GameObject>(Enumerable.Where<GameObject>(Enumerable.Where<GameObject>(Enumerable.Select<MeshRenderer, GameObject>(this.renderersToCombine, (MeshRenderer r) => r.gameObject), (GameObject g) => !(g == null)), (GameObject g) => !Enumerable.Contains<GameObject>(this.objectsToIgnore, g)), (GameObject g) => !this.invalidObjects.Contains(g)).DistinctBy((GameObject g) => g.GetInstanceID()));
		this._combiner.targetMeshes = targetMeshes;
	}

	// Token: 0x0600502C RID: 20524 RVA: 0x0019C5D7 File Offset: 0x0019A7D7
	private void MergeMeshes()
	{
		this._combiner.CombineMeshes();
	}

	// Token: 0x0600502D RID: 20525 RVA: 0x0019C5E5 File Offset: 0x0019A7E5
	private void UndoMerge()
	{
		this._combiner.UndoMerge();
	}

	// Token: 0x0600502E RID: 20526 RVA: 0x0019C5F3 File Offset: 0x0019A7F3
	private void MergeAndExtractPerMaterialMeshes()
	{
		this._combiner.onDoneMerge.AddListener(new UnityAction(this.OnPostMerge));
		this._combiner.CombineMeshes();
	}

	// Token: 0x0600502F RID: 20527 RVA: 0x0019C61D File Offset: 0x0019A81D
	private void QuickMerge()
	{
		this.CollectRenderers();
		this.ValidateRenderers();
		this.SendToCombiner();
		this.MergeAndExtractPerMaterialMeshes();
	}

	// Token: 0x06005030 RID: 20528 RVA: 0x0019C638 File Offset: 0x0019A838
	private void OnPostMerge()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		MeshRenderer component2 = base.GetComponent<MeshRenderer>();
		Mesh sharedMesh = component.sharedMesh;
		int subMeshCount = sharedMesh.subMeshCount;
		string name = component2.name;
		Material[] sharedMaterials = component2.sharedMaterials;
		GameObject gameObject = new GameObject(name + "_PerMaterialMeshes");
		UberCombinerPerMaterialMeshes uberCombinerPerMaterialMeshes;
		this.GetOrAddComponent(out uberCombinerPerMaterialMeshes);
		uberCombinerPerMaterialMeshes.rootObject = gameObject;
		uberCombinerPerMaterialMeshes.objects = new GameObject[subMeshCount];
		uberCombinerPerMaterialMeshes.filters = new MeshFilter[subMeshCount];
		uberCombinerPerMaterialMeshes.renderers = new MeshRenderer[subMeshCount];
		uberCombinerPerMaterialMeshes.materials = new Material[subMeshCount];
		GTMeshData gtmeshData = GTMeshData.Parse(sharedMesh);
		for (int i = 0; i < subMeshCount; i++)
		{
			GameObject gameObject2 = new GameObject(string.Format("{0}_{1}", i, sharedMaterials[i].name));
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.isStatic = true;
			MeshFilter meshFilter = gameObject2.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
			Mesh sharedMesh2 = gtmeshData.ExtractSubmesh(i, false);
			meshFilter.sharedMesh = sharedMesh2;
			meshRenderer.sharedMaterial = sharedMaterials[i];
			meshRenderer.lightProbeUsage = 0;
			meshRenderer.motionVectorGenerationMode = 2;
			uberCombinerPerMaterialMeshes.objects[i] = gameObject2;
			uberCombinerPerMaterialMeshes.filters[i] = meshFilter;
			uberCombinerPerMaterialMeshes.renderers[i] = meshRenderer;
			uberCombinerPerMaterialMeshes.materials[i] = sharedMaterials[i];
		}
	}

	// Token: 0x06005031 RID: 20529 RVA: 0x0019C790 File Offset: 0x0019A990
	private void OnValidate()
	{
		if (!base.transform.position.Approx0(1E-05f))
		{
			base.transform.position = Vector3.zero;
		}
		if (this._combiner == null)
		{
			this._combiner = base.GetComponent<RuntimeMeshCombiner>();
			this._combiner.recalculateNormals = false;
			this._combiner.recalculateTangents = false;
			this._combiner.combineInactives = false;
			this._combiner.garbageCollectorAfterUndo = true;
			this._combiner.afterMerge = RuntimeMeshCombiner.AfterMerge.DoNothing;
		}
	}

	// Token: 0x06005032 RID: 20530 RVA: 0x0019C81A File Offset: 0x0019AA1A
	private static IEnumerable<MeshRenderer> FilterRenderers(IList<MeshRenderer> renderers)
	{
		Shader uberShader = UberShader.ReferenceShader;
		Shader uberShaderNonSRP = UberShader.ReferenceShaderNonSRP;
		RenderQueueRange transQueue = RenderQueueRange.transparent;
		int num;
		for (int i = 0; i < renderers.Count; i = num)
		{
			MeshRenderer mr = renderers[i];
			if (!(mr == null) && mr.enabled && mr.gameObject.isStatic && !mr.GetComponent<EdDoNotMeshCombine>())
			{
				MeshFilter component = mr.GetComponent<MeshFilter>();
				if (!(component == null))
				{
					Mesh sharedMesh = component.sharedMesh;
					if (!(sharedMesh == null) && sharedMesh.vertexCount >= 3)
					{
						Material[] sharedMats = mr.sharedMaterials;
						if (!sharedMats.IsNullOrEmpty<Material>())
						{
							for (int j = 0; j < sharedMats.Length; j = num)
							{
								Material material = sharedMats[j];
								if (!(material == null))
								{
									int renderQueue = material.renderQueue;
									if ((renderQueue < transQueue.lowerBound || renderQueue > transQueue.upperBound) && (renderQueue < 2450 || renderQueue > 2500))
									{
										Shader shader = material.shader;
										if (shader == uberShader)
										{
											yield return mr;
										}
										else if (shader == uberShaderNonSRP)
										{
											yield return mr;
										}
									}
								}
								num = j + 1;
							}
							mr = null;
							sharedMats = null;
						}
					}
				}
			}
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x04005ECB RID: 24267
	[SerializeField]
	private RuntimeMeshCombiner _combiner;

	// Token: 0x04005ECC RID: 24268
	[Space]
	public GameObject[] meshSources = new GameObject[0];

	// Token: 0x04005ECD RID: 24269
	[Space]
	public GameObject[] objectsToIgnore = new GameObject[0];

	// Token: 0x04005ECE RID: 24270
	[Space]
	[NonSerialized]
	private MeshRenderer[] renderersToCombine = new MeshRenderer[0];

	// Token: 0x04005ECF RID: 24271
	[Space]
	[NonSerialized]
	private List<GameObject> invalidObjects = new List<GameObject>();

	// Token: 0x04005ED0 RID: 24272
	public bool includeInactive;

	// Token: 0x04005ED1 RID: 24273
	private static ShaderHashId _BaseMap = "_BaseMap";
}
