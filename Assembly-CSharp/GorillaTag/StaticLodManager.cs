using System;
using System.Collections.Generic;
using System.Diagnostics;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.UI;

namespace GorillaTag
{
	// Token: 0x02000FE6 RID: 4070
	[DefaultExecutionOrder(2000)]
	public class StaticLodManager : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x060066ED RID: 26349 RVA: 0x0021789A File Offset: 0x00215A9A
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this.mainCamera = Camera.main;
			this.hasMainCamera = (this.mainCamera != null);
		}

		// Token: 0x060066EE RID: 26350 RVA: 0x0001140C File Offset: 0x0000F60C
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x060066EF RID: 26351 RVA: 0x002178C0 File Offset: 0x00215AC0
		public static int Register(StaticLodGroup lodGroup)
		{
			int count;
			if (StaticLodManager.freeSlots.TryPop(ref count))
			{
				StaticLodManager.groupMonoBehaviours[count] = lodGroup;
				StaticLodManager.groupInfos[count] = default(StaticLodManager.GroupInfo);
			}
			else
			{
				count = StaticLodManager.groupMonoBehaviours.Count;
				StaticLodManager.groupMonoBehaviours.Add(lodGroup);
				StaticLodManager.groupInfos.Add(default(StaticLodManager.GroupInfo));
			}
			StaticLodManager._groupInstId_to_index[lodGroup.GetInstanceID()] = count;
			StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[count];
			groupInfo.isLoaded = true;
			groupInfo.componentEnabled = lodGroup.isActiveAndEnabled;
			groupInfo.uiEnabled = true;
			groupInfo.collidersEnabled = true;
			groupInfo.uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax;
			groupInfo.collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance;
			StaticLodManager.groupInfos[count] = groupInfo;
			StaticLodManager._TryAddMembersToLodGroup(true, count);
			groupInfo = StaticLodManager.groupInfos[count];
			if (Mathf.Approximately(groupInfo.radiusSq, 0f))
			{
				groupInfo.bounds = new Bounds(lodGroup.transform.position, Vector3.one * 0.01f);
				groupInfo.center = groupInfo.bounds.center;
				groupInfo.radiusSq = groupInfo.bounds.extents.sqrMagnitude;
				StaticLodManager.groupInfos[count] = groupInfo;
			}
			return count;
		}

		// Token: 0x060066F0 RID: 26352 RVA: 0x00217A24 File Offset: 0x00215C24
		public static int OldRegister(StaticLodGroup lodGroup)
		{
			StaticLodGroupExcluder componentInParent = lodGroup.GetComponentInParent<StaticLodGroupExcluder>();
			List<Graphic> list;
			int num;
			using (lodGroup.GTGetComponentsListPool(true, out list))
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					StaticLodGroupExcluder componentInParent2 = list[i].GetComponentInParent<StaticLodGroupExcluder>(true);
					if (componentInParent2 != null && componentInParent2 != componentInParent)
					{
						list.RemoveAt(i);
					}
				}
				Graphic[] array = list.ToArray();
				List<Renderer> list2;
				using (lodGroup.GTGetComponentsListPool(true, out list2))
				{
					for (int j = list2.Count - 1; j >= 0; j--)
					{
						num = list2[j].gameObject.layer;
						if ((num != 5 && num != 18) || !list2[j].enabled)
						{
							list2.RemoveAt(j);
						}
						else
						{
							StaticLodGroupExcluder componentInParent3 = list[j].GetComponentInParent<StaticLodGroupExcluder>(true);
							if (componentInParent3 != null && componentInParent3 != componentInParent)
							{
								list2.RemoveAt(j);
							}
						}
					}
					Renderer[] array2 = list2.ToArray();
					List<Collider> list3;
					using (lodGroup.GTGetComponentsListPool(true, out list3))
					{
						for (int k = 0; k < list3.Count; k++)
						{
							Collider collider = list3[k];
							if (!collider.gameObject.IsOnLayer(UnityLayer.GorillaInteractable))
							{
								list3.RemoveAt(k);
							}
							else
							{
								StaticLodGroupExcluder componentInParent4 = collider.GetComponentInParent<StaticLodGroupExcluder>();
								if (componentInParent4 != null && componentInParent4 != componentInParent)
								{
									list3.RemoveAt(k);
								}
							}
						}
						Collider[] array3 = list3.ToArray();
						Bounds bounds = (array2.Length != 0) ? array2[0].bounds : ((array3.Length != 0) ? array3[0].bounds : ((array.Length != 0) ? new Bounds(array[0].transform.position, Vector3.one * 0.01f) : new Bounds(lodGroup.transform.position, Vector3.one * 0.01f)));
						for (int l = 0; l < array.Length; l++)
						{
							bounds.Encapsulate(array[l].transform.position);
						}
						for (int m = 0; m < array2.Length; m++)
						{
							bounds.Encapsulate(array2[m].bounds);
						}
						for (int n = 0; n < array3.Length; n++)
						{
							bounds.Encapsulate(array3[n].bounds);
						}
						StaticLodManager.GroupInfo groupInfo = new StaticLodManager.GroupInfo
						{
							isLoaded = true,
							componentEnabled = lodGroup.isActiveAndEnabled,
							center = bounds.center,
							radiusSq = bounds.extents.sqrMagnitude,
							uiEnabled = true,
							uiEnableDistanceSq = lodGroup.uiFadeDistanceMax * lodGroup.uiFadeDistanceMax,
							uiGraphics = array,
							renderers = array2,
							collidersEnabled = true,
							collisionEnableDistanceSq = lodGroup.collisionEnableDistance * lodGroup.collisionEnableDistance,
							interactableColliders = array3
						};
						int count;
						if (StaticLodManager.freeSlots.TryPop(ref count))
						{
							StaticLodManager.groupMonoBehaviours[count] = lodGroup;
							StaticLodManager.groupInfos[count] = groupInfo;
						}
						else
						{
							count = StaticLodManager.groupMonoBehaviours.Count;
							StaticLodManager.groupMonoBehaviours.Add(lodGroup);
							StaticLodManager.groupInfos.Add(groupInfo);
						}
						StaticLodManager._groupInstId_to_index[lodGroup.GetInstanceID()] = count;
						num = count;
					}
				}
			}
			return num;
		}

		// Token: 0x060066F1 RID: 26353 RVA: 0x00217DE4 File Offset: 0x00215FE4
		public static void Unregister(int lodGroupIndex)
		{
			StaticLodGroup staticLodGroup = StaticLodManager.groupMonoBehaviours[lodGroupIndex];
			if (staticLodGroup != null)
			{
				StaticLodManager._groupInstId_to_index.Remove(staticLodGroup.GetInstanceID());
			}
			StaticLodManager.groupMonoBehaviours[lodGroupIndex] = null;
			StaticLodManager.groupInfos[lodGroupIndex] = default(StaticLodManager.GroupInfo);
			StaticLodManager.freeSlots.Push(lodGroupIndex);
		}

		// Token: 0x060066F2 RID: 26354 RVA: 0x00217E44 File Offset: 0x00216044
		public static bool TryAddLateInstantiatedMembers(GameObject root)
		{
			StaticLodGroup componentInParent = root.GetComponentInParent<StaticLodGroup>(true);
			if (componentInParent == null)
			{
				return false;
			}
			int groupIndex;
			if (!StaticLodManager._groupInstId_to_index.TryGetValue(componentInParent.GetInstanceID(), ref groupIndex))
			{
				return false;
			}
			if (componentInParent.gameObject != root)
			{
				StaticLodGroupExcluder componentInParent2 = root.GetComponentInParent<StaticLodGroupExcluder>(true);
				if (componentInParent2 != null && componentInParent.transform.GetDepth() < componentInParent2.transform.GetDepth())
				{
					return false;
				}
			}
			return StaticLodManager._TryAddMembersToLodGroup(false, groupIndex);
		}

		// Token: 0x060066F3 RID: 26355 RVA: 0x00217EBC File Offset: 0x002160BC
		private static bool _TryAddMembersToLodGroup(bool isNew, int groupIndex)
		{
			bool flag = false;
			StaticLodGroup staticLodGroup = StaticLodManager.groupMonoBehaviours[groupIndex];
			StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[groupIndex];
			StaticLodGroupExcluder componentInParent = staticLodGroup.GetComponentInParent<StaticLodGroupExcluder>();
			bool result = flag | StaticLodManager._TryAddComponentsToGroup<Collider>(staticLodGroup, componentInParent, ref groupInfo, ref groupInfo.interactableColliders, (Collider coll) => coll.gameObject.IsOnLayer(UnityLayer.GorillaInteractable), (Collider coll) => coll.bounds) | StaticLodManager._TryAddComponentsToGroup<Renderer>(staticLodGroup, componentInParent, ref groupInfo, ref groupInfo.renderers, delegate(Renderer rend)
			{
				int layer = rend.gameObject.layer;
				return (layer == 5 || layer == 18) && rend.enabled;
			}, (Renderer rend) => rend.bounds) | StaticLodManager._TryAddComponentsToGroup<Graphic>(staticLodGroup, componentInParent, ref groupInfo, ref groupInfo.uiGraphics, (Graphic _) => true, (Graphic gfx) => new Bounds(gfx.transform.position, Vector3.one * 0.01f));
			StaticLodManager.groupInfos[groupIndex] = groupInfo;
			return result;
		}

		// Token: 0x060066F4 RID: 26356 RVA: 0x00217FE4 File Offset: 0x002161E4
		private static bool _TryAddComponentsToGroup<T>(StaticLodGroup lodGroup, StaticLodGroupExcluder excluderAboveGroup, ref StaticLodManager.GroupInfo ref_groupInfo, ref T[] ref_components, Predicate<T> includeIf, StaticLodManager._GetBoundsDelegate<T> getBounds) where T : Component
		{
			List<T> list;
			bool result;
			using (lodGroup.GTGetComponentsListPool(true, out list))
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					if (!includeIf.Invoke(list[i]))
					{
						list.RemoveAt(i);
					}
					else
					{
						StaticLodGroupExcluder componentInParent = list[i].GetComponentInParent<StaticLodGroupExcluder>(true);
						if (componentInParent != null && componentInParent != excluderAboveGroup)
						{
							list.RemoveAt(i);
						}
					}
				}
				if (list.Count == 0)
				{
					if (ref_components == null)
					{
						ref_components = Array.Empty<T>();
					}
					result = false;
				}
				else
				{
					T[] array = ref_components;
					int num = (array != null) ? array.Length : 0;
					if (num == 0)
					{
						ref_components = list.ToArray();
					}
					else
					{
						Array.Resize<T>(ref ref_components, num + list.Count);
						for (int j = num; j < ref_components.Length; j++)
						{
							ref_components[j] = list[j - num];
						}
					}
					if (Mathf.Approximately(ref_groupInfo.radiusSq, 0f))
					{
						ref_groupInfo.bounds = getBounds(ref_components[0]);
					}
					for (int k = num; k < ref_components.Length; k++)
					{
						ref_groupInfo.bounds.Encapsulate(getBounds(ref_components[k]));
					}
					ref_groupInfo.center = ref_groupInfo.bounds.center;
					ref_groupInfo.radiusSq = ref_groupInfo.bounds.extents.sqrMagnitude;
					result = true;
				}
			}
			return result;
		}

		// Token: 0x060066F5 RID: 26357 RVA: 0x00002789 File Offset: 0x00000989
		[Conditional("UNITY_EDITOR")]
		private static void _EdAddPathsToGroup<T>(T[] components, ref string[] ref_edDebugPaths) where T : Component
		{
		}

		// Token: 0x060066F6 RID: 26358 RVA: 0x00218170 File Offset: 0x00216370
		public static void SetEnabled(int index, bool enable)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[index];
			groupInfo.componentEnabled = enable;
			StaticLodManager.groupInfos[index] = groupInfo;
		}

		// Token: 0x060066F7 RID: 26359 RVA: 0x002181A8 File Offset: 0x002163A8
		public void SliceUpdate()
		{
			if (!this.hasMainCamera)
			{
				return;
			}
			Vector3 position = this.mainCamera.transform.position;
			for (int i = 0; i < StaticLodManager.groupInfos.Count; i++)
			{
				StaticLodManager.GroupInfo groupInfo = StaticLodManager.groupInfos[i];
				if (groupInfo.isLoaded && groupInfo.componentEnabled)
				{
					float num = Mathf.Max(0f, (groupInfo.center - position).sqrMagnitude - groupInfo.radiusSq);
					float num2 = groupInfo.uiEnabled ? 0.010000001f : 0f;
					bool flag = num < groupInfo.uiEnableDistanceSq + num2;
					if (flag != groupInfo.uiEnabled)
					{
						for (int j = 0; j < groupInfo.uiGraphics.Length; j++)
						{
							Graphic graphic = groupInfo.uiGraphics[j];
							if (!(graphic == null))
							{
								graphic.enabled = flag;
							}
						}
						for (int k = 0; k < groupInfo.renderers.Length; k++)
						{
							Renderer renderer = groupInfo.renderers[k];
							if (!(renderer == null))
							{
								renderer.enabled = flag;
							}
						}
					}
					groupInfo.uiEnabled = flag;
					num2 = (groupInfo.collidersEnabled ? 0.010000001f : 0f);
					bool flag2 = num < groupInfo.collisionEnableDistanceSq + num2;
					if (flag2 != groupInfo.collidersEnabled)
					{
						for (int l = 0; l < groupInfo.interactableColliders.Length; l++)
						{
							if (!(groupInfo.interactableColliders[l] == null))
							{
								groupInfo.interactableColliders[l].enabled = flag2;
							}
						}
					}
					groupInfo.collidersEnabled = flag2;
					StaticLodManager.groupInfos[i] = groupInfo;
				}
			}
		}

		// Token: 0x04007570 RID: 30064
		[OnEnterPlay_Clear]
		private static readonly List<StaticLodGroup> groupMonoBehaviours = new List<StaticLodGroup>(32);

		// Token: 0x04007571 RID: 30065
		[OnEnterPlay_Clear]
		private static readonly Dictionary<int, int> _groupInstId_to_index = new Dictionary<int, int>(32);

		// Token: 0x04007572 RID: 30066
		[DebugReadout]
		[OnEnterPlay_Clear]
		private static readonly List<StaticLodManager.GroupInfo> groupInfos = new List<StaticLodManager.GroupInfo>(32);

		// Token: 0x04007573 RID: 30067
		[OnEnterPlay_Clear]
		private static readonly Stack<int> freeSlots = new Stack<int>();

		// Token: 0x04007574 RID: 30068
		private Camera mainCamera;

		// Token: 0x04007575 RID: 30069
		private bool hasMainCamera;

		// Token: 0x02000FE7 RID: 4071
		private struct GroupInfo
		{
			// Token: 0x04007576 RID: 30070
			public bool isLoaded;

			// Token: 0x04007577 RID: 30071
			public bool componentEnabled;

			// Token: 0x04007578 RID: 30072
			public Vector3 center;

			// Token: 0x04007579 RID: 30073
			public float radiusSq;

			// Token: 0x0400757A RID: 30074
			public Bounds bounds;

			// Token: 0x0400757B RID: 30075
			public bool uiEnabled;

			// Token: 0x0400757C RID: 30076
			public float uiEnableDistanceSq;

			// Token: 0x0400757D RID: 30077
			public Graphic[] uiGraphics;

			// Token: 0x0400757E RID: 30078
			public Renderer[] renderers;

			// Token: 0x0400757F RID: 30079
			public bool collidersEnabled;

			// Token: 0x04007580 RID: 30080
			public float collisionEnableDistanceSq;

			// Token: 0x04007581 RID: 30081
			public Collider[] interactableColliders;
		}

		// Token: 0x02000FE8 RID: 4072
		// (Invoke) Token: 0x060066FB RID: 26363
		private delegate Bounds _GetBoundsDelegate<in T>(T t) where T : Component;
	}
}
