using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000F5F RID: 3935
	[AddComponentMenu("MT Assets/Easy Mesh Combiner/Runtime Mesh Combiner")]
	public class RuntimeMeshCombiner : MonoBehaviour
	{
		// Token: 0x06006288 RID: 25224 RVA: 0x001FB5CD File Offset: 0x001F97CD
		private void Awake()
		{
			if (this.combineMeshesAtStartUp == RuntimeMeshCombiner.CombineOnStart.OnAwake)
			{
				if (this.showDebugLogs)
				{
					Debug.Log("The merge started in Runtime Combiner \"" + base.gameObject.name + "\".");
				}
				this.CombineMeshes();
			}
		}

		// Token: 0x06006289 RID: 25225 RVA: 0x001FB606 File Offset: 0x001F9806
		private void Start()
		{
			if (this.combineMeshesAtStartUp == RuntimeMeshCombiner.CombineOnStart.OnStart)
			{
				if (this.showDebugLogs)
				{
					Debug.Log("The merge started in Runtime Combiner \"" + base.gameObject.name + "\".");
				}
				this.CombineMeshes();
			}
		}

		// Token: 0x0600628A RID: 25226 RVA: 0x001FB640 File Offset: 0x001F9840
		private RuntimeMeshCombiner.GameObjectWithMesh[] GetValidatedTargetGameObjects()
		{
			List<Transform> list = new List<Transform>();
			for (int i = 0; i < this.targetMeshes.Count; i++)
			{
				if (!(this.targetMeshes[i] == null))
				{
					if (this.combineInChildren)
					{
						foreach (Transform transform in this.targetMeshes[i].GetComponentsInChildren<Transform>(true))
						{
							if (!list.Contains(transform))
							{
								list.Add(transform);
							}
						}
					}
					if (!this.combineInChildren)
					{
						Transform component = this.targetMeshes[i].GetComponent<Transform>();
						if (!list.Contains(component))
						{
							list.Add(component);
						}
					}
				}
			}
			List<RuntimeMeshCombiner.GameObjectWithMesh> list2 = new List<RuntimeMeshCombiner.GameObjectWithMesh>();
			for (int k = 0; k < list.Count; k++)
			{
				MeshFilter component2 = list[k].GetComponent<MeshFilter>();
				MeshRenderer component3 = list[k].GetComponent<MeshRenderer>();
				if ((component2 != null || component3 != null) && (this.combineInactives || component3.enabled) && (this.combineInactives || list[k].gameObject.activeSelf) && (this.combineInactives || list[k].gameObject.activeInHierarchy))
				{
					list2.Add(new RuntimeMeshCombiner.GameObjectWithMesh(list[k].gameObject, component2, component3));
				}
			}
			List<RuntimeMeshCombiner.GameObjectWithMesh> list3 = new List<RuntimeMeshCombiner.GameObjectWithMesh>();
			for (int l = 0; l < list2.Count; l++)
			{
				bool flag = true;
				if (list2[l].meshFilter == null)
				{
					if (this.showDebugLogs)
					{
						Debug.LogError("GameObject \"" + list2[l].gameObject.name + "\" does not have the Mesh Filter component, so it is not a valid mesh and will be ignored in the merge process.");
					}
					flag = false;
				}
				if (list2[l].meshRenderer == null)
				{
					if (this.showDebugLogs)
					{
						Debug.LogError("GameObject \"" + list2[l].gameObject.name + "\" does not have the Mesh Renderer component, so it is not a valid mesh and will be ignored in the merge process.");
					}
					flag = false;
				}
				if (list2[l].meshFilter != null && list2[l].meshFilter.sharedMesh == null)
				{
					if (this.showDebugLogs)
					{
						Debug.LogError("GameObject \"" + list2[l].gameObject.name + "\" does not have a Mesh in Mesh Filter component, so it is not a valid mesh and will be ignored in the merge process.");
					}
					flag = false;
				}
				if (list2[l].meshFilter != null && list2[l].meshRenderer != null && list2[l].meshFilter.sharedMesh != null && list2[l].meshFilter.sharedMesh.subMeshCount != list2[l].meshRenderer.sharedMaterials.Length)
				{
					if (this.showDebugLogs)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"The Mesh Renderer component found in GameObject \"",
							list2[l].gameObject.name,
							"\" has more or less material needed. The mesh that is in this GameObject has ",
							list2[l].meshFilter.sharedMesh.subMeshCount.ToString(),
							" submeshes, but has a number of ",
							list2[l].meshRenderer.sharedMaterials.Length.ToString(),
							" materials. This mesh will be ignored during the merge process."
						}));
					}
					flag = false;
				}
				if (list2[l].meshRenderer != null)
				{
					for (int m = 0; m < list2[l].meshRenderer.sharedMaterials.Length; m++)
					{
						if (list2[l].meshRenderer.sharedMaterials[m] == null)
						{
							if (this.showDebugLogs)
							{
								Debug.LogError(string.Concat(new string[]
								{
									"Material ",
									m.ToString(),
									" in Mesh Renderer present in component \"",
									list2[l].gameObject.name,
									"\" is null. For the merge process to work well, all materials must be completed. This GameObject will be ignored in the merge process."
								}));
							}
							flag = false;
						}
					}
				}
				if (list2[l].gameObject.GetComponent<CombinedMeshesManager>() != null)
				{
					if (this.showDebugLogs)
					{
						Debug.LogError("GameObject \"" + list2[l].gameObject.name + "\" is the result of a previous merge, so it will be ignored by this merge.");
					}
					flag = false;
				}
				if (flag)
				{
					list3.Add(list2[l]);
				}
			}
			return list3.ToArray();
		}

		// Token: 0x0600628B RID: 25227 RVA: 0x001FBAE4 File Offset: 0x001F9CE4
		public bool CombineMeshes()
		{
			if (this.isTargetMeshesMerged())
			{
				if (this.showDebugLogs)
				{
					Debug.Log("The Runtime Combiner \"" + base.gameObject.name + "\" meshes are already combined!");
				}
				return true;
			}
			if (this.isTargetMeshesMerged())
			{
				return false;
			}
			if (base.gameObject.GetComponent<MeshFilter>() != null || base.gameObject.GetComponent<MeshRenderer>() != null)
			{
				if (this.showDebugLogs)
				{
					Debug.LogError("Unable to merge. Apparently the GameObject \"" + base.gameObject.name + "\" already contains the Mesh Filter and/or Mesh Renderer component. The Runtime Mesh Combiner needs a GameObject that does not contain these two components. Please remove them or place the Runtime Mesh Combiner in a new GameObject and try again.");
				}
				return false;
			}
			this.originalPosition = base.gameObject.transform.position;
			this.originalEulerAngles = base.gameObject.transform.eulerAngles;
			this.originalScale = base.gameObject.transform.lossyScale;
			base.gameObject.transform.position = Vector3.zero;
			base.gameObject.transform.eulerAngles = Vector3.zero;
			base.gameObject.transform.localScale = Vector3.one;
			RuntimeMeshCombiner.GameObjectWithMesh[] validatedTargetGameObjects = this.GetValidatedTargetGameObjects();
			if (validatedTargetGameObjects.Length == 0)
			{
				if (this.showDebugLogs)
				{
					Debug.LogError("No valid, meshed GameObjects were found in the target GameObjects list. Therefore the merge was interrupted.");
				}
				return false;
			}
			Dictionary<Material, List<RuntimeMeshCombiner.SubMeshToCombine>> dictionary = new Dictionary<Material, List<RuntimeMeshCombiner.SubMeshToCombine>>();
			foreach (RuntimeMeshCombiner.GameObjectWithMesh gameObjectWithMesh in validatedTargetGameObjects)
			{
				for (int j = 0; j < gameObjectWithMesh.meshFilter.sharedMesh.subMeshCount; j++)
				{
					Material material = gameObjectWithMesh.meshRenderer.sharedMaterials[j];
					if (dictionary.ContainsKey(material))
					{
						dictionary[material].Add(new RuntimeMeshCombiner.SubMeshToCombine(gameObjectWithMesh.gameObject.transform, gameObjectWithMesh.meshFilter, gameObjectWithMesh.meshRenderer, j));
					}
					if (!dictionary.ContainsKey(material))
					{
						Dictionary<Material, List<RuntimeMeshCombiner.SubMeshToCombine>> dictionary2 = dictionary;
						Material material2 = material;
						List<RuntimeMeshCombiner.SubMeshToCombine> list = new List<RuntimeMeshCombiner.SubMeshToCombine>();
						list.Add(new RuntimeMeshCombiner.SubMeshToCombine(gameObjectWithMesh.gameObject.transform, gameObjectWithMesh.meshFilter, gameObjectWithMesh.meshRenderer, j));
						dictionary2.Add(material2, list);
					}
				}
			}
			MeshFilter meshFilter = base.gameObject.AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
			int num = 0;
			foreach (RuntimeMeshCombiner.GameObjectWithMesh gameObjectWithMesh2 in validatedTargetGameObjects)
			{
				num += gameObjectWithMesh2.meshFilter.sharedMesh.vertexCount;
			}
			List<Mesh> list2 = new List<Mesh>();
			foreach (KeyValuePair<Material, List<RuntimeMeshCombiner.SubMeshToCombine>> keyValuePair in dictionary)
			{
				List<RuntimeMeshCombiner.SubMeshToCombine> value = keyValuePair.Value;
				List<CombineInstance> list3 = new List<CombineInstance>();
				for (int l = 0; l < value.Count; l++)
				{
					CombineInstance combineInstance = default(CombineInstance);
					combineInstance.mesh = value[l].meshFilter.sharedMesh;
					combineInstance.subMeshIndex = value[l].subMeshIndex;
					combineInstance.transform = value[l].transform.localToWorldMatrix;
					list3.Add(combineInstance);
				}
				Mesh mesh = new Mesh();
				if (num <= this.MAX_VERTICES_FOR_16BITS_MESH)
				{
					mesh.indexFormat = 0;
				}
				if (num > this.MAX_VERTICES_FOR_16BITS_MESH)
				{
					mesh.indexFormat = 1;
				}
				mesh.CombineMeshes(list3.ToArray(), true, true);
				list2.Add(mesh);
			}
			List<CombineInstance> list4 = new List<CombineInstance>();
			foreach (Mesh mesh2 in list2)
			{
				CombineInstance combineInstance2 = default(CombineInstance);
				combineInstance2.mesh = mesh2;
				combineInstance2.subMeshIndex = 0;
				combineInstance2.transform = Matrix4x4.identity;
				list4.Add(combineInstance2);
			}
			Mesh mesh3 = new Mesh();
			if (num <= this.MAX_VERTICES_FOR_16BITS_MESH)
			{
				mesh3.indexFormat = 0;
			}
			if (num > this.MAX_VERTICES_FOR_16BITS_MESH)
			{
				mesh3.indexFormat = 1;
			}
			mesh3.name = base.gameObject.name + " (Temp Merge)";
			mesh3.CombineMeshes(list4.ToArray(), false);
			mesh3.RecalculateBounds();
			if (this.recalculateNormals)
			{
				mesh3.RecalculateNormals();
			}
			if (this.recalculateTangents)
			{
				mesh3.RecalculateTangents();
			}
			if (this.optimizeResultingMesh)
			{
				mesh3.Optimize();
			}
			meshFilter.sharedMesh = mesh3;
			List<Material> list5 = new List<Material>();
			foreach (KeyValuePair<Material, List<RuntimeMeshCombiner.SubMeshToCombine>> keyValuePair2 in dictionary)
			{
				list5.Add(keyValuePair2.Key);
			}
			meshRenderer.sharedMaterials = list5.ToArray();
			if (this.afterMerge == RuntimeMeshCombiner.AfterMerge.DeactiveOriginalGameObjects)
			{
				foreach (RuntimeMeshCombiner.GameObjectWithMesh gameObjectWithMesh3 in validatedTargetGameObjects)
				{
					this.originalGameObjectsWithMeshToRestore.Add(new RuntimeMeshCombiner.OriginalGameObjectWithMesh(gameObjectWithMesh3.gameObject, gameObjectWithMesh3.gameObject.activeSelf, gameObjectWithMesh3.meshRenderer, gameObjectWithMesh3.meshRenderer.enabled));
					gameObjectWithMesh3.gameObject.SetActive(false);
				}
				if (this.addMeshColliderAfter)
				{
					base.gameObject.AddComponent<MeshCollider>();
				}
			}
			if (this.afterMerge == RuntimeMeshCombiner.AfterMerge.DisableOriginalMeshes)
			{
				foreach (RuntimeMeshCombiner.GameObjectWithMesh gameObjectWithMesh4 in validatedTargetGameObjects)
				{
					this.originalGameObjectsWithMeshToRestore.Add(new RuntimeMeshCombiner.OriginalGameObjectWithMesh(gameObjectWithMesh4.gameObject, gameObjectWithMesh4.gameObject.activeSelf, gameObjectWithMesh4.meshRenderer, gameObjectWithMesh4.meshRenderer.enabled));
					gameObjectWithMesh4.meshRenderer.enabled = false;
				}
			}
			RuntimeMeshCombiner.AfterMerge afterMerge = this.afterMerge;
			base.gameObject.transform.position = this.originalPosition;
			base.gameObject.transform.eulerAngles = this.originalEulerAngles;
			base.gameObject.transform.localScale = this.originalScale;
			if (this.showDebugLogs)
			{
				Debug.Log("The merge has been successfully completed in Runtime Combiner \"" + base.gameObject.name + "\"!");
			}
			if (this.onDoneMerge != null)
			{
				this.onDoneMerge.Invoke();
			}
			this.targetMeshesMerged = true;
			return true;
		}

		// Token: 0x0600628C RID: 25228 RVA: 0x001FC118 File Offset: 0x001FA318
		public bool UndoMerge()
		{
			if (!this.isTargetMeshesMerged())
			{
				if (this.showDebugLogs)
				{
					Debug.Log("The Runtime Combiner \"" + base.gameObject.name + "\" meshes are already uncombined!");
				}
				return true;
			}
			if (this.isTargetMeshesMerged())
			{
				if (this.afterMerge == RuntimeMeshCombiner.AfterMerge.DisableOriginalMeshes)
				{
					foreach (RuntimeMeshCombiner.OriginalGameObjectWithMesh originalGameObjectWithMesh in this.originalGameObjectsWithMeshToRestore)
					{
						if (!(originalGameObjectWithMesh.meshRenderer == null))
						{
							originalGameObjectWithMesh.meshRenderer.enabled = originalGameObjectWithMesh.originalMrState;
						}
					}
				}
				if (this.afterMerge == RuntimeMeshCombiner.AfterMerge.DeactiveOriginalGameObjects)
				{
					foreach (RuntimeMeshCombiner.OriginalGameObjectWithMesh originalGameObjectWithMesh2 in this.originalGameObjectsWithMeshToRestore)
					{
						if (!(originalGameObjectWithMesh2.gameObject == null))
						{
							originalGameObjectWithMesh2.gameObject.SetActive(originalGameObjectWithMesh2.originalGoState);
						}
					}
					if (this.addMeshColliderAfter)
					{
						MeshCollider component = base.GetComponent<MeshCollider>();
						if (component != null)
						{
							Object.Destroy(component);
						}
					}
				}
				RuntimeMeshCombiner.AfterMerge afterMerge = this.afterMerge;
				this.originalGameObjectsWithMeshToRestore.Clear();
				Object.Destroy(base.GetComponent<MeshRenderer>());
				Object.Destroy(base.GetComponent<MeshFilter>());
				if (this.garbageCollectorAfterUndo)
				{
					Resources.UnloadUnusedAssets();
					GC.Collect();
				}
				if (this.showDebugLogs)
				{
					Debug.Log("The Runtime Combiner \"" + base.gameObject.name + "\" merge was successfully undone!");
				}
				if (this.onDoneUnmerge != null)
				{
					this.onDoneUnmerge.Invoke();
				}
				this.targetMeshesMerged = false;
				return true;
			}
			return false;
		}

		// Token: 0x0600628D RID: 25229 RVA: 0x001FC2CC File Offset: 0x001FA4CC
		public bool isTargetMeshesMerged()
		{
			return this.targetMeshesMerged;
		}

		// Token: 0x0400710E RID: 28942
		private int MAX_VERTICES_FOR_16BITS_MESH = 50000;

		// Token: 0x0400710F RID: 28943
		private Vector3 originalPosition = Vector3.zero;

		// Token: 0x04007110 RID: 28944
		private Vector3 originalEulerAngles = Vector3.zero;

		// Token: 0x04007111 RID: 28945
		private Vector3 originalScale = Vector3.zero;

		// Token: 0x04007112 RID: 28946
		private List<RuntimeMeshCombiner.OriginalGameObjectWithMesh> originalGameObjectsWithMeshToRestore = new List<RuntimeMeshCombiner.OriginalGameObjectWithMesh>();

		// Token: 0x04007113 RID: 28947
		private bool targetMeshesMerged;

		// Token: 0x04007114 RID: 28948
		[HideInInspector]
		public RuntimeMeshCombiner.AfterMerge afterMerge;

		// Token: 0x04007115 RID: 28949
		[HideInInspector]
		public bool addMeshColliderAfter = true;

		// Token: 0x04007116 RID: 28950
		[HideInInspector]
		public RuntimeMeshCombiner.CombineOnStart combineMeshesAtStartUp;

		// Token: 0x04007117 RID: 28951
		[HideInInspector]
		public bool combineInChildren;

		// Token: 0x04007118 RID: 28952
		[HideInInspector]
		public bool combineInactives;

		// Token: 0x04007119 RID: 28953
		[HideInInspector]
		public bool recalculateNormals = true;

		// Token: 0x0400711A RID: 28954
		[HideInInspector]
		public bool recalculateTangents = true;

		// Token: 0x0400711B RID: 28955
		[HideInInspector]
		public bool optimizeResultingMesh;

		// Token: 0x0400711C RID: 28956
		[HideInInspector]
		public List<GameObject> targetMeshes = new List<GameObject>();

		// Token: 0x0400711D RID: 28957
		[HideInInspector]
		public bool showDebugLogs = true;

		// Token: 0x0400711E RID: 28958
		[HideInInspector]
		public bool garbageCollectorAfterUndo = true;

		// Token: 0x0400711F RID: 28959
		public UnityEvent onDoneMerge;

		// Token: 0x04007120 RID: 28960
		public UnityEvent onDoneUnmerge;

		// Token: 0x02000F60 RID: 3936
		private class GameObjectWithMesh
		{
			// Token: 0x0600628F RID: 25231 RVA: 0x001FC34C File Offset: 0x001FA54C
			public GameObjectWithMesh(GameObject gameObject, MeshFilter meshFilter, MeshRenderer meshRenderer)
			{
				this.gameObject = gameObject;
				this.meshFilter = meshFilter;
				this.meshRenderer = meshRenderer;
			}

			// Token: 0x04007121 RID: 28961
			public GameObject gameObject;

			// Token: 0x04007122 RID: 28962
			public MeshFilter meshFilter;

			// Token: 0x04007123 RID: 28963
			public MeshRenderer meshRenderer;
		}

		// Token: 0x02000F61 RID: 3937
		private class OriginalGameObjectWithMesh
		{
			// Token: 0x06006290 RID: 25232 RVA: 0x001FC369 File Offset: 0x001FA569
			public OriginalGameObjectWithMesh(GameObject gameObject, bool originalGoState, MeshRenderer meshRenderer, bool originalMrState)
			{
				this.gameObject = gameObject;
				this.originalGoState = originalGoState;
				this.meshRenderer = meshRenderer;
				this.originalMrState = originalMrState;
			}

			// Token: 0x04007124 RID: 28964
			public GameObject gameObject;

			// Token: 0x04007125 RID: 28965
			public bool originalGoState;

			// Token: 0x04007126 RID: 28966
			public MeshRenderer meshRenderer;

			// Token: 0x04007127 RID: 28967
			public bool originalMrState;
		}

		// Token: 0x02000F62 RID: 3938
		private class SubMeshToCombine
		{
			// Token: 0x06006291 RID: 25233 RVA: 0x001FC38E File Offset: 0x001FA58E
			public SubMeshToCombine(Transform transform, MeshFilter meshFilter, MeshRenderer meshRenderer, int subMeshIndex)
			{
				this.transform = transform;
				this.meshFilter = meshFilter;
				this.meshRenderer = meshRenderer;
				this.subMeshIndex = subMeshIndex;
			}

			// Token: 0x04007128 RID: 28968
			public Transform transform;

			// Token: 0x04007129 RID: 28969
			public MeshFilter meshFilter;

			// Token: 0x0400712A RID: 28970
			public MeshRenderer meshRenderer;

			// Token: 0x0400712B RID: 28971
			public int subMeshIndex;
		}

		// Token: 0x02000F63 RID: 3939
		public enum CombineOnStart
		{
			// Token: 0x0400712D RID: 28973
			Disabled,
			// Token: 0x0400712E RID: 28974
			OnStart,
			// Token: 0x0400712F RID: 28975
			OnAwake
		}

		// Token: 0x02000F64 RID: 3940
		public enum AfterMerge
		{
			// Token: 0x04007131 RID: 28977
			DisableOriginalMeshes,
			// Token: 0x04007132 RID: 28978
			DeactiveOriginalGameObjects,
			// Token: 0x04007133 RID: 28979
			DoNothing
		}
	}
}
