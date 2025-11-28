using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Cysharp.Text;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace GorillaExtensions
{
	// Token: 0x02000FBC RID: 4028
	public static class GTExt
	{
		// Token: 0x0600650F RID: 25871 RVA: 0x002101D8 File Offset: 0x0020E3D8
		public static T GetComponentInHierarchy<T>(this Scene scene, bool includeInactive = true) where T : Component
		{
			if (!scene.IsValid())
			{
				return default(T);
			}
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				T component = gameObject.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
				Transform[] componentsInChildren = gameObject.GetComponentsInChildren<Transform>(includeInactive);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					component = componentsInChildren[j].GetComponent<T>();
					if (component != null)
					{
						return component;
					}
				}
			}
			return default(T);
		}

		// Token: 0x06006510 RID: 25872 RVA: 0x00210270 File Offset: 0x0020E470
		public static List<T> GetComponentsInHierarchy<T>(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			List<T> list = new List<T>(capacity);
			if (!scene.IsValid())
			{
				return list;
			}
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				T[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren<T>(includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list;
		}

		// Token: 0x06006511 RID: 25873 RVA: 0x002102B8 File Offset: 0x0020E4B8
		public static List<Object> GetComponentsInHierarchy(this Scene scene, Type type, bool includeInactive = true, int capacity = 64)
		{
			List<Object> list = new List<Object>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				Component[] componentsInChildren = rootGameObjects[i].GetComponentsInChildren(type, includeInactive);
				list.AddRange(componentsInChildren);
			}
			return list;
		}

		// Token: 0x06006512 RID: 25874 RVA: 0x002102F5 File Offset: 0x0020E4F5
		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, bool includeInactive = true, int capacity = 64)
		{
			return scene.GetComponentsInHierarchy(includeInactive, capacity);
		}

		// Token: 0x06006513 RID: 25875 RVA: 0x00210300 File Offset: 0x0020E500
		public static List<T> GetComponentsInHierarchyUntil<T, TStop1>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		// Token: 0x06006514 RID: 25876 RVA: 0x00210344 File Offset: 0x0020E544
		public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		// Token: 0x06006515 RID: 25877 RVA: 0x00210388 File Offset: 0x0020E588
		public static List<T> GetComponentsInHierarchyUntil<T, TStop1, TStop2, TStop3>(this Scene scene, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			List<T> list = new List<T>(capacity);
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				List<T> componentsInChildrenUntil = rootGameObjects[i].transform.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
				list.AddRange(componentsInChildrenUntil);
			}
			return list;
		}

		// Token: 0x06006516 RID: 25878 RVA: 0x002103CC File Offset: 0x0020E5CC
		public static List<T> GetComponentsInChildrenUntil<T, TStop1>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component
		{
			GTExt.<>c__DisplayClass7_0<T, TStop1> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && root.GetComponent<TStop1>() != null)
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x06006517 RID: 25879 RVA: 0x0021042C File Offset: 0x0020E62C
		public static PooledObject<List<T>> GTGetComponentsListPool<T>(this Component root, bool includeInactive, out List<T> pooledList) where T : Component
		{
			PooledObject<List<T>> result = CollectionPool<List<T>, T>.Get(ref pooledList);
			root.GetComponentsInChildren<T>(includeInactive, pooledList);
			return result;
		}

		// Token: 0x06006518 RID: 25880 RVA: 0x0021043D File Offset: 0x0020E63D
		public static PooledObject<List<T>> GTGetComponentsListPool<T>(this Component root, out List<T> pooledList) where T : Component
		{
			PooledObject<List<T>> result = CollectionPool<List<T>, T>.Get(ref pooledList);
			root.GetComponentsInChildren<T>(pooledList);
			return result;
		}

		// Token: 0x06006519 RID: 25881 RVA: 0x00210450 File Offset: 0x0020E650
		public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component
		{
			GTExt.<>c__DisplayClass10_0<T, TStop1, TStop2> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && (root.GetComponent<TStop1>() != null || root.GetComponent<TStop2>() != null))
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|10_0<T, TStop1, TStop2>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x0600651A RID: 25882 RVA: 0x002104C4 File Offset: 0x0020E6C4
		public static List<T> GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(this Component root, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			GTExt.<>c__DisplayClass11_0<T, TStop1, TStop2, TStop3> CS$<>8__locals1;
			CS$<>8__locals1.includeInactive = includeInactive;
			List<T> list = new List<T>(capacity);
			if (stopAtRoot && (root.GetComponent<TStop1>() != null || root.GetComponent<TStop2>() != null || root.GetComponent<TStop3>() != null))
			{
				return list;
			}
			T component = root.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
			GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|11_0<T, TStop1, TStop2, TStop3>(root.transform, ref list, ref CS$<>8__locals1);
			return list;
		}

		// Token: 0x0600651B RID: 25883 RVA: 0x0021054A File Offset: 0x0020E74A
		public static void GetComponentsInChildrenUntil<T, TStop1, TStop2, TStop3>(this Component root, out List<T> out_included, out HashSet<T> out_excluded, bool includeInactive = false, bool stopAtRoot = true, int capacity = 64) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			out_included = root.GetComponentsInChildrenUntil(includeInactive, stopAtRoot, capacity);
			out_excluded = new HashSet<T>(root.GetComponentsInChildren<T>(includeInactive));
			out_excluded.ExceptWith(new HashSet<T>(out_included));
		}

		// Token: 0x0600651C RID: 25884 RVA: 0x00210578 File Offset: 0x0020E778
		private static void _GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(Transform currentTransform, List<T> included, List<Component> excluded, bool includeInactive) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if (includeInactive || transform.gameObject.activeSelf)
				{
					Component component;
					if (GTExt._HasAnyComponents<TStop1, TStop2, TStop3>(transform, out component))
					{
						excluded.Add(component);
					}
					else
					{
						T component2 = transform.GetComponent<T>();
						if (component2 != null)
						{
							included.Add(component2);
						}
						GTExt._GetComponentsInChildrenUntil_OutExclusions_GetRecursive<T, TStop1, TStop2, TStop3>(transform, included, excluded, includeInactive);
					}
				}
			}
		}

		// Token: 0x0600651D RID: 25885 RVA: 0x00210610 File Offset: 0x0020E810
		private static bool _HasAnyComponents<TStop1, TStop2, TStop3>(Component component, out Component stopComponent) where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			stopComponent = component.GetComponent<TStop1>();
			if (stopComponent != null)
			{
				return true;
			}
			stopComponent = component.GetComponent<TStop2>();
			if (stopComponent != null)
			{
				return true;
			}
			stopComponent = component.GetComponent<TStop3>();
			return stopComponent != null;
		}

		// Token: 0x0600651E RID: 25886 RVA: 0x0021066C File Offset: 0x0020E86C
		public static T GetComponentWithRegex<T>(this Component root, string regexString) where T : Component
		{
			T[] componentsInChildren = root.GetComponentsInChildren<T>();
			Regex regex = new Regex(regexString);
			foreach (T t in componentsInChildren)
			{
				if (regex.IsMatch(t.name))
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x0600651F RID: 25887 RVA: 0x002106BC File Offset: 0x0020E8BC
		private static List<T> GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, string regexString, bool includeInactive, int capacity = 64) where T : Component
		{
			List<T> result = new List<T>(capacity);
			Regex regex = new Regex(regexString);
			GTExt.GetComponentsWithRegex_Internal<T>(allComponents, regex, ref result);
			return result;
		}

		// Token: 0x06006520 RID: 25888 RVA: 0x002106E4 File Offset: 0x0020E8E4
		private static void GetComponentsWithRegex_Internal<T>(IEnumerable<T> allComponents, Regex regex, ref List<T> foundComponents) where T : Component
		{
			foreach (T t in allComponents)
			{
				string name = t.name;
				if (regex.IsMatch(name))
				{
					foundComponents.Add(t);
				}
			}
		}

		// Token: 0x06006521 RID: 25889 RVA: 0x00210744 File Offset: 0x0020E944
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(scene.GetComponentsInHierarchy(includeInactive, capacity), regexString, includeInactive, capacity);
		}

		// Token: 0x06006522 RID: 25890 RVA: 0x00210756 File Offset: 0x0020E956
		public static List<T> GetComponentsWithRegex<T>(this Component root, string regexString, bool includeInactive, int capacity) where T : Component
		{
			return GTExt.GetComponentsWithRegex_Internal<T>(root.GetComponentsInChildren<T>(includeInactive), regexString, includeInactive, capacity);
		}

		// Token: 0x06006523 RID: 25891 RVA: 0x00210768 File Offset: 0x0020E968
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string regexString, bool includeInactive = true, int capacity = 64)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexString, includeInactive, capacity);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x06006524 RID: 25892 RVA: 0x002107D0 File Offset: 0x0020E9D0
		public static void GetComponentsWithRegex_Internal<T>(this List<T> allComponents, Regex[] regexes, int maxCount, ref List<T> foundComponents) where T : Component
		{
			if (maxCount == 0)
			{
				return;
			}
			int num = 0;
			foreach (T t in allComponents)
			{
				for (int i = 0; i < regexes.Length; i++)
				{
					if (regexes[i].IsMatch(t.name))
					{
						foundComponents.Add(t);
						num++;
						if (maxCount > 0 && num >= maxCount)
						{
							return;
						}
					}
				}
			}
		}

		// Token: 0x06006525 RID: 25893 RVA: 0x00210860 File Offset: 0x0020EA60
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1, int capacity = 64) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, capacity);
			List<T> result = new List<T>(componentsInHierarchy.Count);
			Regex[] array = new Regex[regexStrings.Length];
			for (int i = 0; i < regexStrings.Length; i++)
			{
				array[i] = new Regex(regexStrings[i]);
			}
			componentsInHierarchy.GetComponentsWithRegex_Internal(array, maxCount, ref result);
			return result;
		}

		// Token: 0x06006526 RID: 25894 RVA: 0x002108B0 File Offset: 0x0020EAB0
		public static List<T> GetComponentsWithRegex<T>(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1) where T : Component
		{
			List<T> componentsInHierarchy = scene.GetComponentsInHierarchy(includeInactive, 64);
			List<T> list = new List<T>(componentsInHierarchy.Count);
			if (maxCount == 0)
			{
				return list;
			}
			int num = 0;
			foreach (T t in componentsInHierarchy)
			{
				bool flag = false;
				foreach (string text in regexStrings)
				{
					if (!flag && Regex.IsMatch(t.name, text))
					{
						foreach (string text2 in excludeRegexStrings)
						{
							if (!flag)
							{
								flag = Regex.IsMatch(t.name, text2);
							}
						}
						if (!flag)
						{
							list.Add(t);
							num++;
							if (maxCount > 0 && num >= maxCount)
							{
								return list;
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06006527 RID: 25895 RVA: 0x002109B4 File Offset: 0x0020EBB4
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, includeInactive, maxCount, 64);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x06006528 RID: 25896 RVA: 0x00210A20 File Offset: 0x0020EC20
		public static List<GameObject> GetGameObjectsWithRegex(this Scene scene, string[] regexStrings, string[] excludeRegexStrings, bool includeInactive = true, int maxCount = -1)
		{
			List<Transform> componentsWithRegex = scene.GetComponentsWithRegex(regexStrings, excludeRegexStrings, includeInactive, maxCount);
			List<GameObject> list = new List<GameObject>(componentsWithRegex.Count);
			foreach (Transform transform in componentsWithRegex)
			{
				list.Add(transform.gameObject);
			}
			return list;
		}

		// Token: 0x06006529 RID: 25897 RVA: 0x00210A8C File Offset: 0x0020EC8C
		public static List<T> GetComponentsByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
		{
			T[] componentsInChildren = xform.GetComponentsInChildren<T>(includeInactive);
			List<T> list = new List<T>(componentsInChildren.Length);
			foreach (T t in componentsInChildren)
			{
				if (t.name == name)
				{
					list.Add(t);
				}
			}
			return list;
		}

		// Token: 0x0600652A RID: 25898 RVA: 0x00210ADC File Offset: 0x0020ECDC
		public static T GetComponentByName<T>(this Transform xform, string name, bool includeInactive = true) where T : Component
		{
			foreach (T t in xform.GetComponentsInChildren<T>(includeInactive))
			{
				if (t.name == name)
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x0600652B RID: 25899 RVA: 0x00210B28 File Offset: 0x0020ED28
		public static List<GameObject> GetGameObjectsInHierarchy(this Scene scene, string name, bool includeInactive = true)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (GameObject gameObject in scene.GetRootGameObjects())
			{
				if (gameObject.name.Contains(name))
				{
					list.Add(gameObject);
				}
				foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(includeInactive))
				{
					if (transform.name.Contains(name))
					{
						list.Add(transform.gameObject);
					}
				}
			}
			return list;
		}

		// Token: 0x0600652C RID: 25900 RVA: 0x00210BAA File Offset: 0x0020EDAA
		public static T GetOrAddComponent<T>(this GameObject gameObject, ref T component) where T : Component
		{
			if (component == null)
			{
				component = gameObject.GetOrAddComponent<T>();
			}
			return component;
		}

		// Token: 0x0600652D RID: 25901 RVA: 0x00210BD4 File Offset: 0x0020EDD4
		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
		{
			T result;
			if (!gameObject.TryGetComponent<T>(ref result))
			{
				result = gameObject.AddComponent<T>();
			}
			return result;
		}

		// Token: 0x0600652E RID: 25902 RVA: 0x00210BF4 File Offset: 0x0020EDF4
		public static void SetLossyScale(this Transform transform, Vector3 scale)
		{
			scale = transform.InverseTransformVector(scale);
			Vector3 lossyScale = transform.lossyScale;
			transform.localScale = new Vector3(scale.x / lossyScale.x, scale.y / lossyScale.y, scale.z / lossyScale.z);
		}

		// Token: 0x0600652F RID: 25903 RVA: 0x00210C43 File Offset: 0x0020EE43
		public static Quaternion TransformRotation(this Transform transform, Quaternion localRotation)
		{
			return transform.rotation * localRotation;
		}

		// Token: 0x06006530 RID: 25904 RVA: 0x00210C51 File Offset: 0x0020EE51
		public static Quaternion InverseTransformRotation(this Transform transform, Quaternion localRotation)
		{
			return Quaternion.Inverse(transform.rotation) * localRotation;
		}

		// Token: 0x06006531 RID: 25905 RVA: 0x00210C64 File Offset: 0x0020EE64
		public static Vector3 ProjectOnPlane(this Vector3 point, Vector3 planeAnchorPosition, Vector3 planeNormal)
		{
			return planeAnchorPosition + Vector3.ProjectOnPlane(point - planeAnchorPosition, planeNormal);
		}

		// Token: 0x06006532 RID: 25906 RVA: 0x00210C7C File Offset: 0x0020EE7C
		public static void ForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T t = list[i];
				try
				{
					action.Invoke(t);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		// Token: 0x06006533 RID: 25907 RVA: 0x00210CC4 File Offset: 0x0020EEC4
		public static void AddSortedUnique<T>(this List<T> list, T item)
		{
			int num = list.BinarySearch(item);
			if (num < 0)
			{
				list.Insert(~num, item);
			}
		}

		// Token: 0x06006534 RID: 25908 RVA: 0x00210CE8 File Offset: 0x0020EEE8
		public static void RemoveSorted<T>(this List<T> list, T item)
		{
			int num = list.BinarySearch(item);
			if (num >= 0)
			{
				list.RemoveAt(num);
			}
		}

		// Token: 0x06006535 RID: 25909 RVA: 0x00210D08 File Offset: 0x0020EF08
		public static bool ContainsSorted<T>(this List<T> list, T item)
		{
			return list.BinarySearch(item) >= 0;
		}

		// Token: 0x06006536 RID: 25910 RVA: 0x00210D18 File Offset: 0x0020EF18
		public static void SafeForEachBackwards<T>(this List<T> list, Action<T> action)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				T t = list[i];
				try
				{
					action.Invoke(t);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}

		// Token: 0x06006537 RID: 25911 RVA: 0x00210D60 File Offset: 0x0020EF60
		public static T[] Filled<T>(this T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = value;
			}
			return array;
		}

		// Token: 0x06006538 RID: 25912 RVA: 0x00210D84 File Offset: 0x0020EF84
		public static bool CompareAs255Unclamped(this Color a, Color b)
		{
			int num = (int)(a.r * 255f);
			int num2 = (int)(a.g * 255f);
			int num3 = (int)(a.b * 255f);
			int num4 = (int)(a.a * 255f);
			int num5 = (int)(b.r * 255f);
			int num6 = (int)(b.g * 255f);
			int num7 = (int)(b.b * 255f);
			int num8 = (int)(b.a * 255f);
			return num == num5 && num2 == num6 && num3 == num7 && num4 == num8;
		}

		// Token: 0x06006539 RID: 25913 RVA: 0x00210E18 File Offset: 0x0020F018
		public static Quaternion QuaternionFromToVec(Vector3 toVector, Vector3 fromVector)
		{
			Vector3 vector = Vector3.Cross(fromVector, toVector);
			Debug.Log(vector);
			Debug.Log(vector.magnitude);
			Debug.Log(Vector3.Dot(fromVector, toVector) + 1f);
			Quaternion quaternion;
			quaternion..ctor(vector.x, vector.y, vector.z, 1f + Vector3.Dot(toVector, fromVector));
			Debug.Log(quaternion);
			Debug.Log(quaternion.eulerAngles);
			Debug.Log(quaternion.normalized);
			return quaternion.normalized;
		}

		// Token: 0x0600653A RID: 25914 RVA: 0x00210EBC File Offset: 0x0020F0BC
		public static Vector3 Position(this Matrix4x4 matrix)
		{
			float m = matrix.m03;
			float m2 = matrix.m13;
			float m3 = matrix.m23;
			return new Vector3(m, m2, m3);
		}

		// Token: 0x0600653B RID: 25915 RVA: 0x00210EE4 File Offset: 0x0020F0E4
		public static Vector3 Scale(this Matrix4x4 m)
		{
			Vector3 result;
			result..ctor(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
			if (Vector3.Cross(m.GetColumn(0), m.GetColumn(1)).normalized != m.GetColumn(2).normalized)
			{
				result.x *= -1f;
			}
			return result;
		}

		// Token: 0x0600653C RID: 25916 RVA: 0x00002789 File Offset: 0x00000989
		public static void SetLocalRelativeToParentMatrixWithParityAxis(this Matrix4x4 matrix, GTExt.ParityOptions parity = GTExt.ParityOptions.XFlip)
		{
		}

		// Token: 0x0600653D RID: 25917 RVA: 0x00210F7C File Offset: 0x0020F17C
		public static void MultiplyInPlaceWith(this Vector3 a, in Vector3 b)
		{
			a.x *= b.x;
			a.y *= b.y;
			a.z *= b.z;
		}

		// Token: 0x0600653E RID: 25918 RVA: 0x00210FB0 File Offset: 0x0020F1B0
		public static void DecomposeWithXFlip(this Matrix4x4 matrix, out Vector3 transformation, out Quaternion rotation, out Vector3 scale)
		{
			Matrix4x4 matrix2 = matrix;
			bool flag = matrix2.ValidTRS();
			transformation = matrix2.Position();
			Quaternion quaternion;
			if (!flag)
			{
				quaternion = Quaternion.identity;
			}
			else
			{
				int num = 2;
				Vector3 vector = matrix2.GetColumnNoCopy(num);
				int num2 = 1;
				quaternion = Quaternion.LookRotation(vector, matrix2.GetColumnNoCopy(num2));
			}
			rotation = quaternion;
			Vector3 vector2;
			if (!flag)
			{
				vector2 = Vector3.zero;
			}
			else
			{
				Matrix4x4 matrix4x = matrix;
				vector2 = matrix4x.lossyScale;
			}
			scale = vector2;
		}

		// Token: 0x0600653F RID: 25919 RVA: 0x0021102C File Offset: 0x0020F22C
		public static void SetLocalMatrixRelativeToParentWithXParity(this Transform transform, in Matrix4x4 matrix4X4)
		{
			Vector3 localPosition;
			Quaternion localRotation;
			Vector3 localScale;
			matrix4X4.DecomposeWithXFlip(out localPosition, out localRotation, out localScale);
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
			transform.localScale = localScale;
		}

		// Token: 0x06006540 RID: 25920 RVA: 0x0021105C File Offset: 0x0020F25C
		public static Matrix4x4 Matrix4x4Scale(in Vector3 vector)
		{
			Matrix4x4 result;
			result.m00 = vector.x;
			result.m01 = 0f;
			result.m02 = 0f;
			result.m03 = 0f;
			result.m10 = 0f;
			result.m11 = vector.y;
			result.m12 = 0f;
			result.m13 = 0f;
			result.m20 = 0f;
			result.m21 = 0f;
			result.m22 = vector.z;
			result.m23 = 0f;
			result.m30 = 0f;
			result.m31 = 0f;
			result.m32 = 0f;
			result.m33 = 1f;
			return result;
		}

		// Token: 0x06006541 RID: 25921 RVA: 0x00211130 File Offset: 0x0020F330
		public static Vector4 GetColumnNoCopy(this Matrix4x4 matrix, in int index)
		{
			switch (index)
			{
			case 0:
				return new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30);
			case 1:
				return new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31);
			case 2:
				return new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32);
			case 3:
				return new Vector4(matrix.m03, matrix.m13, matrix.m23, matrix.m33);
			default:
				throw new IndexOutOfRangeException("Invalid column index!");
			}
		}

		// Token: 0x06006542 RID: 25922 RVA: 0x002111DC File Offset: 0x0020F3DC
		public static Quaternion RotationWithScaleContext(this Matrix4x4 m, in Vector3 scale)
		{
			Matrix4x4 matrix4x = m * GTExt.Matrix4x4Scale(scale);
			int num = 2;
			Vector3 vector = matrix4x.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(vector, matrix4x.GetColumnNoCopy(num2));
		}

		// Token: 0x06006543 RID: 25923 RVA: 0x00211220 File Offset: 0x0020F420
		public static Quaternion Rotation(this Matrix4x4 m)
		{
			int num = 2;
			Vector3 vector = m.GetColumnNoCopy(num);
			int num2 = 1;
			return Quaternion.LookRotation(vector, m.GetColumnNoCopy(num2));
		}

		// Token: 0x06006544 RID: 25924 RVA: 0x00211250 File Offset: 0x0020F450
		public static Vector3 x0y(this Vector2 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x06006545 RID: 25925 RVA: 0x00211268 File Offset: 0x0020F468
		public static Vector3 x0y(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Token: 0x06006546 RID: 25926 RVA: 0x00211280 File Offset: 0x0020F480
		public static Vector3 xy0(this Vector2 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x06006547 RID: 25927 RVA: 0x00211298 File Offset: 0x0020F498
		public static Vector3 xy0(this Vector3 v)
		{
			return new Vector3(v.x, v.y, 0f);
		}

		// Token: 0x06006548 RID: 25928 RVA: 0x002112B0 File Offset: 0x0020F4B0
		public static Vector3 xz0(this Vector3 v)
		{
			return new Vector3(v.x, v.z, 0f);
		}

		// Token: 0x06006549 RID: 25929 RVA: 0x0005BE7E File Offset: 0x0005A07E
		public static Vector3 x0z(this Vector3 v)
		{
			return new Vector3(v.x, 0f, v.z);
		}

		// Token: 0x0600654A RID: 25930 RVA: 0x002112C8 File Offset: 0x0020F4C8
		public static Matrix4x4 LocalMatrixRelativeToParentNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one);
		}

		// Token: 0x0600654B RID: 25931 RVA: 0x002112E0 File Offset: 0x0020F4E0
		public static Matrix4x4 LocalMatrixRelativeToParentWithScale(this Transform transform)
		{
			if (transform.parent == null)
			{
				return transform.localToWorldMatrix;
			}
			return transform.parent.worldToLocalMatrix * transform.localToWorldMatrix;
		}

		// Token: 0x0600654C RID: 25932 RVA: 0x0021130D File Offset: 0x0020F50D
		public static void SetLocalMatrixRelativeToParent(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
			transform.localScale = matrix.Scale();
		}

		// Token: 0x0600654D RID: 25933 RVA: 0x00211334 File Offset: 0x0020F534
		public static void SetLocalMatrixRelativeToParentNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.localPosition = matrix.Position();
			transform.localRotation = matrix.Rotation();
		}

		// Token: 0x0600654E RID: 25934 RVA: 0x0021134F File Offset: 0x0020F54F
		public static void SetLocalToWorldMatrixNoScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.Rotation();
		}

		// Token: 0x0600654F RID: 25935 RVA: 0x0021136A File Offset: 0x0020F56A
		public static Matrix4x4 localToWorldNoScale(this Transform transform)
		{
			return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
		}

		// Token: 0x06006550 RID: 25936 RVA: 0x00211382 File Offset: 0x0020F582
		public static void SetLocalToWorldMatrixWithScale(this Transform transform, Matrix4x4 matrix)
		{
			transform.position = matrix.Position();
			transform.rotation = matrix.rotation;
			transform.SetLossyScale(matrix.lossyScale);
		}

		// Token: 0x06006551 RID: 25937 RVA: 0x002113AA File Offset: 0x0020F5AA
		public static Matrix4x4 Matrix4X4LerpNoScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.rotation, b.rotation, t), b.lossyScale);
		}

		// Token: 0x06006552 RID: 25938 RVA: 0x002113DE File Offset: 0x0020F5DE
		public static Matrix4x4 LerpTo(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpNoScale(a, b, t);
		}

		// Token: 0x06006553 RID: 25939 RVA: 0x002113E8 File Offset: 0x0020F5E8
		[MethodImpl(256)]
		public static bool IsNaN(this Vector3 v)
		{
			return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
		}

		// Token: 0x06006554 RID: 25940 RVA: 0x00211411 File Offset: 0x0020F611
		[MethodImpl(256)]
		public static bool IsNan(this Quaternion q)
		{
			return float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w);
		}

		// Token: 0x06006555 RID: 25941 RVA: 0x00211447 File Offset: 0x0020F647
		[MethodImpl(256)]
		public static bool IsInfinity(this Vector3 v)
		{
			return float.IsInfinity(v.x) || float.IsInfinity(v.y) || float.IsInfinity(v.z);
		}

		// Token: 0x06006556 RID: 25942 RVA: 0x00211470 File Offset: 0x0020F670
		[MethodImpl(256)]
		public static bool IsInfinity(this Quaternion q)
		{
			return float.IsInfinity(q.x) || float.IsInfinity(q.y) || float.IsInfinity(q.z) || float.IsInfinity(q.w);
		}

		// Token: 0x06006557 RID: 25943 RVA: 0x002114A6 File Offset: 0x0020F6A6
		[MethodImpl(256)]
		public static bool ValuesInRange(this Vector3 v, in float maxVal)
		{
			return Mathf.Abs(v.x) < maxVal && Mathf.Abs(v.y) < maxVal && Mathf.Abs(v.z) < maxVal;
		}

		// Token: 0x06006558 RID: 25944 RVA: 0x002114D7 File Offset: 0x0020F6D7
		[MethodImpl(256)]
		public static bool IsValid(this Vector3 v, in float maxVal = 10000f)
		{
			return !v.IsNaN() && !v.IsInfinity() && v.ValuesInRange(maxVal);
		}

		// Token: 0x06006559 RID: 25945 RVA: 0x002114F4 File Offset: 0x0020F6F4
		[MethodImpl(256)]
		public static Vector3 GetValidWithFallback(this Vector3 v, in Vector3 safeVal)
		{
			float num = 10000f;
			if (!v.IsValid(num))
			{
				return safeVal;
			}
			return v;
		}

		// Token: 0x0600655A RID: 25946 RVA: 0x00211520 File Offset: 0x0020F720
		[MethodImpl(256)]
		public static void SetValueSafe(this Vector3 v, in Vector3 newVal)
		{
			float num = 10000f;
			if (newVal.IsValid(num))
			{
				v = newVal;
			}
		}

		// Token: 0x0600655B RID: 25947 RVA: 0x00211549 File Offset: 0x0020F749
		[MethodImpl(256)]
		public static bool IsValid(this Quaternion q)
		{
			return !q.IsNan() && !q.IsInfinity();
		}

		// Token: 0x0600655C RID: 25948 RVA: 0x0021155E File Offset: 0x0020F75E
		[MethodImpl(256)]
		public static Quaternion GetValidWithFallback(this Quaternion q, in Quaternion safeVal)
		{
			if (!q.IsValid())
			{
				return safeVal;
			}
			return q;
		}

		// Token: 0x0600655D RID: 25949 RVA: 0x00211575 File Offset: 0x0020F775
		[MethodImpl(256)]
		public static void SetValueSafe(this Quaternion q, in Quaternion newVal)
		{
			if (newVal.IsValid())
			{
				q = newVal;
			}
		}

		// Token: 0x0600655E RID: 25950 RVA: 0x0021158C File Offset: 0x0020F78C
		[MethodImpl(256)]
		public static Vector2 ClampMagnitudeSafe(this Vector2 v2, float magnitude)
		{
			if (!float.IsFinite(v2.x))
			{
				v2.x = 0f;
			}
			if (!float.IsFinite(v2.y))
			{
				v2.y = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			return Vector2.ClampMagnitude(v2, magnitude);
		}

		// Token: 0x0600655F RID: 25951 RVA: 0x002115E4 File Offset: 0x0020F7E4
		[MethodImpl(256)]
		public static void ClampThisMagnitudeSafe(this Vector2 v2, float magnitude)
		{
			if (!float.IsFinite(v2.x))
			{
				v2.x = 0f;
			}
			if (!float.IsFinite(v2.y))
			{
				v2.y = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			v2 = Vector2.ClampMagnitude(v2, magnitude);
		}

		// Token: 0x06006560 RID: 25952 RVA: 0x00211644 File Offset: 0x0020F844
		[MethodImpl(256)]
		public static Vector3 ClampMagnitudeSafe(this Vector3 v3, float magnitude)
		{
			if (!float.IsFinite(v3.x))
			{
				v3.x = 0f;
			}
			if (!float.IsFinite(v3.y))
			{
				v3.y = 0f;
			}
			if (!float.IsFinite(v3.z))
			{
				v3.z = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			return Vector3.ClampMagnitude(v3, magnitude);
		}

		// Token: 0x06006561 RID: 25953 RVA: 0x002116B4 File Offset: 0x0020F8B4
		[MethodImpl(256)]
		public static void ClampThisMagnitudeSafe(this Vector3 v3, float magnitude)
		{
			if (!float.IsFinite(v3.x))
			{
				v3.x = 0f;
			}
			if (!float.IsFinite(v3.y))
			{
				v3.y = 0f;
			}
			if (!float.IsFinite(v3.z))
			{
				v3.z = 0f;
			}
			if (!float.IsFinite(magnitude))
			{
				magnitude = 0f;
			}
			v3 = Vector3.ClampMagnitude(v3, magnitude);
		}

		// Token: 0x06006562 RID: 25954 RVA: 0x0021172A File Offset: 0x0020F92A
		[MethodImpl(256)]
		public static float MinSafe(this float value, float min)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			if (value >= min)
			{
				return min;
			}
			return value;
		}

		// Token: 0x06006563 RID: 25955 RVA: 0x00211751 File Offset: 0x0020F951
		[MethodImpl(256)]
		public static void ThisMinSafe(this float value, float min)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			value = ((value < min) ? value : min);
		}

		// Token: 0x06006564 RID: 25956 RVA: 0x0021177E File Offset: 0x0020F97E
		[MethodImpl(256)]
		public static double MinSafe(this double value, float min)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)min))
			{
				min = 0f;
			}
			if (value >= (double)min)
			{
				return (double)min;
			}
			return value;
		}

		// Token: 0x06006565 RID: 25957 RVA: 0x002117AC File Offset: 0x0020F9AC
		[MethodImpl(256)]
		public static void ThisMinSafe(this double value, float min)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)min))
			{
				min = 0f;
			}
			value = ((value < (double)min) ? value : ((double)min));
		}

		// Token: 0x06006566 RID: 25958 RVA: 0x002117E0 File Offset: 0x0020F9E0
		[MethodImpl(256)]
		public static float MaxSafe(this float value, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			if (value <= max)
			{
				return max;
			}
			return value;
		}

		// Token: 0x06006567 RID: 25959 RVA: 0x00211807 File Offset: 0x0020FA07
		[MethodImpl(256)]
		public static void ThisMaxSafe(this float value, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			value = ((value > max) ? value : max);
		}

		// Token: 0x06006568 RID: 25960 RVA: 0x00211834 File Offset: 0x0020FA34
		[MethodImpl(256)]
		public static double MaxSafe(this double value, float max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)max))
			{
				max = 0f;
			}
			if (value <= (double)max)
			{
				return (double)max;
			}
			return value;
		}

		// Token: 0x06006569 RID: 25961 RVA: 0x00211862 File Offset: 0x0020FA62
		[MethodImpl(256)]
		public static void ThisMaxSafe(this double value, float max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite((double)max))
			{
				max = 0f;
			}
			value = ((value > (double)max) ? value : ((double)max));
		}

		// Token: 0x0600656A RID: 25962 RVA: 0x00211896 File Offset: 0x0020FA96
		[MethodImpl(256)]
		public static float ClampSafe(this float value, float min, float max)
		{
			if (!float.IsFinite(value))
			{
				value = 0f;
			}
			if (!float.IsFinite(min))
			{
				min = 0f;
			}
			if (!float.IsFinite(max))
			{
				max = 0f;
			}
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		// Token: 0x0600656B RID: 25963 RVA: 0x002118D4 File Offset: 0x0020FAD4
		[MethodImpl(256)]
		public static double ClampSafe(this double value, double min, double max)
		{
			if (!double.IsFinite(value))
			{
				value = 0.0;
			}
			if (!double.IsFinite(min))
			{
				min = 0.0;
			}
			if (!double.IsFinite(max))
			{
				max = 0.0;
			}
			if (value > max)
			{
				return max;
			}
			if (value >= min)
			{
				return value;
			}
			return min;
		}

		// Token: 0x0600656C RID: 25964 RVA: 0x00211927 File Offset: 0x0020FB27
		[MethodImpl(256)]
		public static float GetFinite(this float value)
		{
			if (!float.IsFinite(value))
			{
				return 0f;
			}
			return value;
		}

		// Token: 0x0600656D RID: 25965 RVA: 0x00211938 File Offset: 0x0020FB38
		[MethodImpl(256)]
		public static double GetFinite(this double value)
		{
			if (!double.IsFinite(value))
			{
				return 0.0;
			}
			return value;
		}

		// Token: 0x0600656E RID: 25966 RVA: 0x0021194D File Offset: 0x0020FB4D
		public static Matrix4x4 Matrix4X4LerpHandleNegativeScale(Matrix4x4 a, Matrix4x4 b, float t)
		{
			return Matrix4x4.TRS(Vector3.Lerp(a.Position(), b.Position(), t), Quaternion.Slerp(a.Rotation(), b.Rotation(), t), b.lossyScale);
		}

		// Token: 0x0600656F RID: 25967 RVA: 0x00211981 File Offset: 0x0020FB81
		public static Matrix4x4 LerpTo_HandleNegativeScale(this Matrix4x4 a, Matrix4x4 b, float t)
		{
			return GTExt.Matrix4X4LerpHandleNegativeScale(a, b, t);
		}

		// Token: 0x06006570 RID: 25968 RVA: 0x0021198C File Offset: 0x0020FB8C
		public static Vector3 LerpToUnclamped(this Vector3 a, in Vector3 b, float t)
		{
			return new Vector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
		}

		// Token: 0x06006571 RID: 25969 RVA: 0x002119E0 File Offset: 0x0020FBE0
		public static string ToLongString(this Vector3 self)
		{
			return string.Format("[{0}, {1}, {2}]", self.x, self.y, self.z);
		}

		// Token: 0x06006572 RID: 25970 RVA: 0x00211A0D File Offset: 0x0020FC0D
		public static int GetRandomIndex<T>(this IReadOnlyList<T> self)
		{
			return Random.Range(0, self.Count);
		}

		// Token: 0x06006573 RID: 25971 RVA: 0x00211A1B File Offset: 0x0020FC1B
		public static T GetRandomItem<T>(this IReadOnlyList<T> self)
		{
			return self[self.GetRandomIndex<T>()];
		}

		// Token: 0x06006574 RID: 25972 RVA: 0x00211A29 File Offset: 0x0020FC29
		public static Vector2 xx(this float v)
		{
			return new Vector2(v, v);
		}

		// Token: 0x06006575 RID: 25973 RVA: 0x00211A32 File Offset: 0x0020FC32
		public static Vector2 xx(this Vector2 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x06006576 RID: 25974 RVA: 0x00211A45 File Offset: 0x0020FC45
		public static Vector2 xy(this Vector2 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x06006577 RID: 25975 RVA: 0x00211A58 File Offset: 0x0020FC58
		public static Vector2 yy(this Vector2 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x06006578 RID: 25976 RVA: 0x00211A6B File Offset: 0x0020FC6B
		public static Vector2 xx(this Vector3 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x06006579 RID: 25977 RVA: 0x00211A7E File Offset: 0x0020FC7E
		public static Vector2 xy(this Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x0600657A RID: 25978 RVA: 0x00211A91 File Offset: 0x0020FC91
		public static Vector2 xz(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x0600657B RID: 25979 RVA: 0x00211AA4 File Offset: 0x0020FCA4
		public static Vector2 yy(this Vector3 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x0600657C RID: 25980 RVA: 0x00211AB7 File Offset: 0x0020FCB7
		public static Vector2 yz(this Vector3 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x0600657D RID: 25981 RVA: 0x00211ACA File Offset: 0x0020FCCA
		public static Vector2 zz(this Vector3 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x0600657E RID: 25982 RVA: 0x00211ADD File Offset: 0x0020FCDD
		public static Vector2 xx(this Vector4 v)
		{
			return new Vector2(v.x, v.x);
		}

		// Token: 0x0600657F RID: 25983 RVA: 0x00211AF0 File Offset: 0x0020FCF0
		public static Vector2 xy(this Vector4 v)
		{
			return new Vector2(v.x, v.y);
		}

		// Token: 0x06006580 RID: 25984 RVA: 0x00211B03 File Offset: 0x0020FD03
		public static Vector2 xz(this Vector4 v)
		{
			return new Vector2(v.x, v.z);
		}

		// Token: 0x06006581 RID: 25985 RVA: 0x00211B16 File Offset: 0x0020FD16
		public static Vector2 xw(this Vector4 v)
		{
			return new Vector2(v.x, v.w);
		}

		// Token: 0x06006582 RID: 25986 RVA: 0x00211B29 File Offset: 0x0020FD29
		public static Vector2 yy(this Vector4 v)
		{
			return new Vector2(v.y, v.y);
		}

		// Token: 0x06006583 RID: 25987 RVA: 0x00211B3C File Offset: 0x0020FD3C
		public static Vector2 yz(this Vector4 v)
		{
			return new Vector2(v.y, v.z);
		}

		// Token: 0x06006584 RID: 25988 RVA: 0x00211B4F File Offset: 0x0020FD4F
		public static Vector2 yw(this Vector4 v)
		{
			return new Vector2(v.y, v.w);
		}

		// Token: 0x06006585 RID: 25989 RVA: 0x00211B62 File Offset: 0x0020FD62
		public static Vector2 zz(this Vector4 v)
		{
			return new Vector2(v.z, v.z);
		}

		// Token: 0x06006586 RID: 25990 RVA: 0x00211B75 File Offset: 0x0020FD75
		public static Vector2 zw(this Vector4 v)
		{
			return new Vector2(v.z, v.w);
		}

		// Token: 0x06006587 RID: 25991 RVA: 0x00211B88 File Offset: 0x0020FD88
		public static Vector2 ww(this Vector4 v)
		{
			return new Vector2(v.w, v.w);
		}

		// Token: 0x06006588 RID: 25992 RVA: 0x00211B9B File Offset: 0x0020FD9B
		public static Vector3 xxx(this float v)
		{
			return new Vector3(v, v, v);
		}

		// Token: 0x06006589 RID: 25993 RVA: 0x00211BA5 File Offset: 0x0020FDA5
		public static Vector3 xxx(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x0600658A RID: 25994 RVA: 0x00211BBE File Offset: 0x0020FDBE
		public static Vector3 xxy(this Vector2 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x0600658B RID: 25995 RVA: 0x00211BD7 File Offset: 0x0020FDD7
		public static Vector3 xyy(this Vector2 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x0600658C RID: 25996 RVA: 0x00211BF0 File Offset: 0x0020FDF0
		public static Vector3 yyy(this Vector2 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x0600658D RID: 25997 RVA: 0x00211C09 File Offset: 0x0020FE09
		public static Vector3 xxx(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x0600658E RID: 25998 RVA: 0x00211C22 File Offset: 0x0020FE22
		public static Vector3 xxy(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x0600658F RID: 25999 RVA: 0x00211C3B File Offset: 0x0020FE3B
		public static Vector3 xxz(this Vector3 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x06006590 RID: 26000 RVA: 0x00211C54 File Offset: 0x0020FE54
		public static Vector3 xyy(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x06006591 RID: 26001 RVA: 0x00211C6D File Offset: 0x0020FE6D
		public static Vector3 xyz(this Vector3 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x06006592 RID: 26002 RVA: 0x00211C86 File Offset: 0x0020FE86
		public static Vector3 xzz(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x06006593 RID: 26003 RVA: 0x00211C9F File Offset: 0x0020FE9F
		public static Vector3 yyy(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x06006594 RID: 26004 RVA: 0x00211CB8 File Offset: 0x0020FEB8
		public static Vector3 yyz(this Vector3 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x06006595 RID: 26005 RVA: 0x00211CD1 File Offset: 0x0020FED1
		public static Vector3 yzz(this Vector3 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x06006596 RID: 26006 RVA: 0x00211CEA File Offset: 0x0020FEEA
		public static Vector3 zzz(this Vector3 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x06006597 RID: 26007 RVA: 0x00211D03 File Offset: 0x0020FF03
		public static Vector3 xxx(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.x);
		}

		// Token: 0x06006598 RID: 26008 RVA: 0x00211D1C File Offset: 0x0020FF1C
		public static Vector3 xxy(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.y);
		}

		// Token: 0x06006599 RID: 26009 RVA: 0x00211D35 File Offset: 0x0020FF35
		public static Vector3 xxz(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.z);
		}

		// Token: 0x0600659A RID: 26010 RVA: 0x00211D4E File Offset: 0x0020FF4E
		public static Vector3 xxw(this Vector4 v)
		{
			return new Vector3(v.x, v.x, v.w);
		}

		// Token: 0x0600659B RID: 26011 RVA: 0x00211D67 File Offset: 0x0020FF67
		public static Vector3 xyy(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.y);
		}

		// Token: 0x0600659C RID: 26012 RVA: 0x00211D80 File Offset: 0x0020FF80
		public static Vector3 xyz(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		// Token: 0x0600659D RID: 26013 RVA: 0x00211D99 File Offset: 0x0020FF99
		public static Vector3 xyw(this Vector4 v)
		{
			return new Vector3(v.x, v.y, v.w);
		}

		// Token: 0x0600659E RID: 26014 RVA: 0x00211DB2 File Offset: 0x0020FFB2
		public static Vector3 xzz(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.z);
		}

		// Token: 0x0600659F RID: 26015 RVA: 0x00211DCB File Offset: 0x0020FFCB
		public static Vector3 xzw(this Vector4 v)
		{
			return new Vector3(v.x, v.z, v.w);
		}

		// Token: 0x060065A0 RID: 26016 RVA: 0x00211DE4 File Offset: 0x0020FFE4
		public static Vector3 xww(this Vector4 v)
		{
			return new Vector3(v.x, v.w, v.w);
		}

		// Token: 0x060065A1 RID: 26017 RVA: 0x00211DFD File Offset: 0x0020FFFD
		public static Vector3 yyy(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.y);
		}

		// Token: 0x060065A2 RID: 26018 RVA: 0x00211E16 File Offset: 0x00210016
		public static Vector3 yyz(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.z);
		}

		// Token: 0x060065A3 RID: 26019 RVA: 0x00211E2F File Offset: 0x0021002F
		public static Vector3 yyw(this Vector4 v)
		{
			return new Vector3(v.y, v.y, v.w);
		}

		// Token: 0x060065A4 RID: 26020 RVA: 0x00211E48 File Offset: 0x00210048
		public static Vector3 yzz(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.z);
		}

		// Token: 0x060065A5 RID: 26021 RVA: 0x00211E61 File Offset: 0x00210061
		public static Vector3 yzw(this Vector4 v)
		{
			return new Vector3(v.y, v.z, v.w);
		}

		// Token: 0x060065A6 RID: 26022 RVA: 0x00211E7A File Offset: 0x0021007A
		public static Vector3 yww(this Vector4 v)
		{
			return new Vector3(v.y, v.w, v.w);
		}

		// Token: 0x060065A7 RID: 26023 RVA: 0x00211E93 File Offset: 0x00210093
		public static Vector3 zzz(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.z);
		}

		// Token: 0x060065A8 RID: 26024 RVA: 0x00211EAC File Offset: 0x002100AC
		public static Vector3 zzw(this Vector4 v)
		{
			return new Vector3(v.z, v.z, v.w);
		}

		// Token: 0x060065A9 RID: 26025 RVA: 0x00211EC5 File Offset: 0x002100C5
		public static Vector3 zww(this Vector4 v)
		{
			return new Vector3(v.z, v.w, v.w);
		}

		// Token: 0x060065AA RID: 26026 RVA: 0x00211EDE File Offset: 0x002100DE
		public static Vector3 www(this Vector4 v)
		{
			return new Vector3(v.w, v.w, v.w);
		}

		// Token: 0x060065AB RID: 26027 RVA: 0x00211EF7 File Offset: 0x002100F7
		public static Vector4 xxxx(this float v)
		{
			return new Vector4(v, v, v, v);
		}

		// Token: 0x060065AC RID: 26028 RVA: 0x00211F02 File Offset: 0x00210102
		public static Vector4 xxxx(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x060065AD RID: 26029 RVA: 0x00211F21 File Offset: 0x00210121
		public static Vector4 xxxy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x060065AE RID: 26030 RVA: 0x00211F40 File Offset: 0x00210140
		public static Vector4 xxyy(this Vector2 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x060065AF RID: 26031 RVA: 0x00211F5F File Offset: 0x0021015F
		public static Vector4 xyyy(this Vector2 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x060065B0 RID: 26032 RVA: 0x00211F7E File Offset: 0x0021017E
		public static Vector4 yyyy(this Vector2 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x060065B1 RID: 26033 RVA: 0x00211F9D File Offset: 0x0021019D
		public static Vector4 xxxx(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x060065B2 RID: 26034 RVA: 0x00211FBC File Offset: 0x002101BC
		public static Vector4 xxxy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x060065B3 RID: 26035 RVA: 0x00211FDB File Offset: 0x002101DB
		public static Vector4 xxxz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x060065B4 RID: 26036 RVA: 0x00211FFA File Offset: 0x002101FA
		public static Vector4 xxyy(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x060065B5 RID: 26037 RVA: 0x00212019 File Offset: 0x00210219
		public static Vector4 xxyz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x060065B6 RID: 26038 RVA: 0x00212038 File Offset: 0x00210238
		public static Vector4 xxzz(this Vector3 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x060065B7 RID: 26039 RVA: 0x00212057 File Offset: 0x00210257
		public static Vector4 xyyy(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x060065B8 RID: 26040 RVA: 0x00212076 File Offset: 0x00210276
		public static Vector4 xyyz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x060065B9 RID: 26041 RVA: 0x00212095 File Offset: 0x00210295
		public static Vector4 xyzz(this Vector3 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x060065BA RID: 26042 RVA: 0x002120B4 File Offset: 0x002102B4
		public static Vector4 xzzz(this Vector3 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x060065BB RID: 26043 RVA: 0x002120D3 File Offset: 0x002102D3
		public static Vector4 yyyy(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x060065BC RID: 26044 RVA: 0x002120F2 File Offset: 0x002102F2
		public static Vector4 yyyz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x060065BD RID: 26045 RVA: 0x00212111 File Offset: 0x00210311
		public static Vector4 yyzz(this Vector3 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x060065BE RID: 26046 RVA: 0x00212130 File Offset: 0x00210330
		public static Vector4 yzzz(this Vector3 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x060065BF RID: 26047 RVA: 0x0021214F File Offset: 0x0021034F
		public static Vector4 zzzz(this Vector3 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x060065C0 RID: 26048 RVA: 0x0021216E File Offset: 0x0021036E
		public static Vector4 xxxx(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.x);
		}

		// Token: 0x060065C1 RID: 26049 RVA: 0x0021218D File Offset: 0x0021038D
		public static Vector4 xxxy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.y);
		}

		// Token: 0x060065C2 RID: 26050 RVA: 0x002121AC File Offset: 0x002103AC
		public static Vector4 xxxz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.z);
		}

		// Token: 0x060065C3 RID: 26051 RVA: 0x002121CB File Offset: 0x002103CB
		public static Vector4 xxxw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.x, v.w);
		}

		// Token: 0x060065C4 RID: 26052 RVA: 0x002121EA File Offset: 0x002103EA
		public static Vector4 xxyy(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.y);
		}

		// Token: 0x060065C5 RID: 26053 RVA: 0x00212209 File Offset: 0x00210409
		public static Vector4 xxyz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.z);
		}

		// Token: 0x060065C6 RID: 26054 RVA: 0x00212228 File Offset: 0x00210428
		public static Vector4 xxyw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.y, v.w);
		}

		// Token: 0x060065C7 RID: 26055 RVA: 0x00212247 File Offset: 0x00210447
		public static Vector4 xxzz(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.z);
		}

		// Token: 0x060065C8 RID: 26056 RVA: 0x00212266 File Offset: 0x00210466
		public static Vector4 xxzw(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.z, v.w);
		}

		// Token: 0x060065C9 RID: 26057 RVA: 0x00212285 File Offset: 0x00210485
		public static Vector4 xxww(this Vector4 v)
		{
			return new Vector4(v.x, v.x, v.w, v.w);
		}

		// Token: 0x060065CA RID: 26058 RVA: 0x002122A4 File Offset: 0x002104A4
		public static Vector4 xyyy(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.y);
		}

		// Token: 0x060065CB RID: 26059 RVA: 0x002122C3 File Offset: 0x002104C3
		public static Vector4 xyyz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.z);
		}

		// Token: 0x060065CC RID: 26060 RVA: 0x002122E2 File Offset: 0x002104E2
		public static Vector4 xyyw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.y, v.w);
		}

		// Token: 0x060065CD RID: 26061 RVA: 0x00212301 File Offset: 0x00210501
		public static Vector4 xyzz(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.z);
		}

		// Token: 0x060065CE RID: 26062 RVA: 0x00212320 File Offset: 0x00210520
		public static Vector4 xyzw(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.z, v.w);
		}

		// Token: 0x060065CF RID: 26063 RVA: 0x0021233F File Offset: 0x0021053F
		public static Vector4 xyww(this Vector4 v)
		{
			return new Vector4(v.x, v.y, v.w, v.w);
		}

		// Token: 0x060065D0 RID: 26064 RVA: 0x0021235E File Offset: 0x0021055E
		public static Vector4 xzzz(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.z);
		}

		// Token: 0x060065D1 RID: 26065 RVA: 0x0021237D File Offset: 0x0021057D
		public static Vector4 xzzw(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.z, v.w);
		}

		// Token: 0x060065D2 RID: 26066 RVA: 0x0021239C File Offset: 0x0021059C
		public static Vector4 xzww(this Vector4 v)
		{
			return new Vector4(v.x, v.z, v.w, v.w);
		}

		// Token: 0x060065D3 RID: 26067 RVA: 0x002123BB File Offset: 0x002105BB
		public static Vector4 xwww(this Vector4 v)
		{
			return new Vector4(v.x, v.w, v.w, v.w);
		}

		// Token: 0x060065D4 RID: 26068 RVA: 0x002123DA File Offset: 0x002105DA
		public static Vector4 yyyy(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.y);
		}

		// Token: 0x060065D5 RID: 26069 RVA: 0x002123F9 File Offset: 0x002105F9
		public static Vector4 yyyz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.z);
		}

		// Token: 0x060065D6 RID: 26070 RVA: 0x00212418 File Offset: 0x00210618
		public static Vector4 yyyw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.y, v.w);
		}

		// Token: 0x060065D7 RID: 26071 RVA: 0x00212437 File Offset: 0x00210637
		public static Vector4 yyzz(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.z);
		}

		// Token: 0x060065D8 RID: 26072 RVA: 0x00212456 File Offset: 0x00210656
		public static Vector4 yyzw(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.z, v.w);
		}

		// Token: 0x060065D9 RID: 26073 RVA: 0x00212475 File Offset: 0x00210675
		public static Vector4 yyww(this Vector4 v)
		{
			return new Vector4(v.y, v.y, v.w, v.w);
		}

		// Token: 0x060065DA RID: 26074 RVA: 0x00212494 File Offset: 0x00210694
		public static Vector4 yzzz(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.z);
		}

		// Token: 0x060065DB RID: 26075 RVA: 0x002124B3 File Offset: 0x002106B3
		public static Vector4 yzzw(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.z, v.w);
		}

		// Token: 0x060065DC RID: 26076 RVA: 0x002124D2 File Offset: 0x002106D2
		public static Vector4 yzww(this Vector4 v)
		{
			return new Vector4(v.y, v.z, v.w, v.w);
		}

		// Token: 0x060065DD RID: 26077 RVA: 0x002124F1 File Offset: 0x002106F1
		public static Vector4 ywww(this Vector4 v)
		{
			return new Vector4(v.y, v.w, v.w, v.w);
		}

		// Token: 0x060065DE RID: 26078 RVA: 0x00212510 File Offset: 0x00210710
		public static Vector4 zzzz(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.z);
		}

		// Token: 0x060065DF RID: 26079 RVA: 0x0021252F File Offset: 0x0021072F
		public static Vector4 zzzw(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.z, v.w);
		}

		// Token: 0x060065E0 RID: 26080 RVA: 0x0021254E File Offset: 0x0021074E
		public static Vector4 zzww(this Vector4 v)
		{
			return new Vector4(v.z, v.z, v.w, v.w);
		}

		// Token: 0x060065E1 RID: 26081 RVA: 0x0021256D File Offset: 0x0021076D
		public static Vector4 zwww(this Vector4 v)
		{
			return new Vector4(v.z, v.w, v.w, v.w);
		}

		// Token: 0x060065E2 RID: 26082 RVA: 0x0021258C File Offset: 0x0021078C
		public static Vector4 wwww(this Vector4 v)
		{
			return new Vector4(v.w, v.w, v.w, v.w);
		}

		// Token: 0x060065E3 RID: 26083 RVA: 0x002125AB File Offset: 0x002107AB
		public static Vector4 WithX(this Vector4 v, float x)
		{
			return new Vector4(x, v.y, v.z, v.w);
		}

		// Token: 0x060065E4 RID: 26084 RVA: 0x002125C5 File Offset: 0x002107C5
		public static Vector4 WithY(this Vector4 v, float y)
		{
			return new Vector4(v.x, y, v.z, v.w);
		}

		// Token: 0x060065E5 RID: 26085 RVA: 0x002125DF File Offset: 0x002107DF
		public static Vector4 WithZ(this Vector4 v, float z)
		{
			return new Vector4(v.x, v.y, z, v.w);
		}

		// Token: 0x060065E6 RID: 26086 RVA: 0x002125F9 File Offset: 0x002107F9
		public static Vector4 WithW(this Vector4 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x060065E7 RID: 26087 RVA: 0x00212613 File Offset: 0x00210813
		public static Vector3 WithX(this Vector3 v, float x)
		{
			return new Vector3(x, v.y, v.z);
		}

		// Token: 0x060065E8 RID: 26088 RVA: 0x00212627 File Offset: 0x00210827
		public static Vector3 WithY(this Vector3 v, float y)
		{
			return new Vector3(v.x, y, v.z);
		}

		// Token: 0x060065E9 RID: 26089 RVA: 0x0021263B File Offset: 0x0021083B
		public static Vector3 WithZ(this Vector3 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x060065EA RID: 26090 RVA: 0x0021264F File Offset: 0x0021084F
		public static Vector4 WithW(this Vector3 v, float w)
		{
			return new Vector4(v.x, v.y, v.z, w);
		}

		// Token: 0x060065EB RID: 26091 RVA: 0x00212669 File Offset: 0x00210869
		public static Vector2 WithX(this Vector2 v, float x)
		{
			return new Vector2(x, v.y);
		}

		// Token: 0x060065EC RID: 26092 RVA: 0x00212677 File Offset: 0x00210877
		public static Vector2 WithY(this Vector2 v, float y)
		{
			return new Vector2(v.x, y);
		}

		// Token: 0x060065ED RID: 26093 RVA: 0x00212685 File Offset: 0x00210885
		public static Vector3 WithZ(this Vector2 v, float z)
		{
			return new Vector3(v.x, v.y, z);
		}

		// Token: 0x060065EE RID: 26094 RVA: 0x00212699 File Offset: 0x00210899
		public static bool IsShorterThan(this Vector2 v, float len)
		{
			return v.sqrMagnitude < len * len;
		}

		// Token: 0x060065EF RID: 26095 RVA: 0x002126A7 File Offset: 0x002108A7
		public static bool IsShorterThan(this Vector2 v, Vector2 v2)
		{
			return v.sqrMagnitude < v2.sqrMagnitude;
		}

		// Token: 0x060065F0 RID: 26096 RVA: 0x002126B9 File Offset: 0x002108B9
		public static bool IsShorterThan(this Vector3 v, float len)
		{
			return v.sqrMagnitude < len * len;
		}

		// Token: 0x060065F1 RID: 26097 RVA: 0x002126C7 File Offset: 0x002108C7
		public static bool IsShorterThan(this Vector3 v, Vector3 v2)
		{
			return v.sqrMagnitude < v2.sqrMagnitude;
		}

		// Token: 0x060065F2 RID: 26098 RVA: 0x002126D9 File Offset: 0x002108D9
		public static bool IsLongerThan(this Vector2 v, float len)
		{
			return v.sqrMagnitude > len * len;
		}

		// Token: 0x060065F3 RID: 26099 RVA: 0x002126E7 File Offset: 0x002108E7
		public static bool IsLongerThan(this Vector2 v, Vector2 v2)
		{
			return v.sqrMagnitude > v2.sqrMagnitude;
		}

		// Token: 0x060065F4 RID: 26100 RVA: 0x002126F9 File Offset: 0x002108F9
		public static bool IsLongerThan(this Vector3 v, float len)
		{
			return v.sqrMagnitude > len * len;
		}

		// Token: 0x060065F5 RID: 26101 RVA: 0x00212707 File Offset: 0x00210907
		public static bool IsLongerThan(this Vector3 v, Vector3 v2)
		{
			return v.sqrMagnitude > v2.sqrMagnitude;
		}

		// Token: 0x060065F6 RID: 26102 RVA: 0x00212719 File Offset: 0x00210919
		public static Vector3 Normalize(this Vector3 value, out float existingMagnitude)
		{
			existingMagnitude = Vector3.Magnitude(value);
			if (existingMagnitude > 1E-05f)
			{
				return value / existingMagnitude;
			}
			return Vector3.zero;
		}

		// Token: 0x060065F7 RID: 26103 RVA: 0x0021273C File Offset: 0x0021093C
		public static Vector3 GetClosestPoint(this Ray ray, Vector3 target)
		{
			float num = Vector3.Dot(target - ray.origin, ray.direction);
			return ray.origin + ray.direction * num;
		}

		// Token: 0x060065F8 RID: 26104 RVA: 0x0021277C File Offset: 0x0021097C
		public static float GetClosestDistSqr(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).sqrMagnitude;
		}

		// Token: 0x060065F9 RID: 26105 RVA: 0x002127A0 File Offset: 0x002109A0
		public static float GetClosestDistance(this Ray ray, Vector3 target)
		{
			return (ray.GetClosestPoint(target) - target).magnitude;
		}

		// Token: 0x060065FA RID: 26106 RVA: 0x002127C4 File Offset: 0x002109C4
		public static Vector3 ProjectToPlane(this Ray ray, Vector3 planeOrigin, Vector3 planeNormalMustBeLength1)
		{
			Vector3 vector = planeOrigin - ray.origin;
			float num = Vector3.Dot(planeNormalMustBeLength1, vector);
			float num2 = Vector3.Dot(planeNormalMustBeLength1, ray.direction);
			return ray.origin + ray.direction * num / num2;
		}

		// Token: 0x060065FB RID: 26107 RVA: 0x00212814 File Offset: 0x00210A14
		public static Vector3 ProjectToLine(this Ray ray, Vector3 lineStart, Vector3 lineEnd)
		{
			Vector3 normalized = (lineEnd - lineStart).normalized;
			Vector3 normalized2 = Vector3.Cross(Vector3.Cross(ray.direction, normalized), normalized).normalized;
			return ray.ProjectToPlane(lineStart, normalized2);
		}

		// Token: 0x060065FC RID: 26108 RVA: 0x00212855 File Offset: 0x00210A55
		public static bool IsNull(this Object mono)
		{
			return mono == null || !mono;
		}

		// Token: 0x060065FD RID: 26109 RVA: 0x00212865 File Offset: 0x00210A65
		public static bool IsNotNull(this Object mono)
		{
			return !mono.IsNull();
		}

		// Token: 0x060065FE RID: 26110 RVA: 0x00212870 File Offset: 0x00210A70
		public static string GetPath(this Transform transform)
		{
			string text = transform.name;
			while (transform.parent)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return "/" + text;
		}

		// Token: 0x060065FF RID: 26111 RVA: 0x002128B8 File Offset: 0x00210AB8
		public static string GetPathQ(this Transform transform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				transform.GetPathQ(ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06006600 RID: 26112 RVA: 0x002128F8 File Offset: 0x00210AF8
		public static void GetPathQ(this Transform transform, ref Utf16ValueStringBuilder sb)
		{
			sb.Append("\"");
			int length = sb.Length;
			do
			{
				if (sb.Length > length)
				{
					sb.Insert(length, "/");
				}
				sb.Insert(length, transform.name);
				transform = transform.parent;
			}
			while (transform != null);
			sb.Append("\"");
		}

		// Token: 0x06006601 RID: 26113 RVA: 0x00212958 File Offset: 0x00210B58
		public static string GetPath(this Transform transform, int maxDepth)
		{
			string text = transform.name;
			int num = 0;
			while (transform.parent && num < maxDepth)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
				num++;
			}
			return "/" + text;
		}

		// Token: 0x06006602 RID: 26114 RVA: 0x002129AC File Offset: 0x00210BAC
		public static string GetPath(this Transform transform, Transform stopper)
		{
			string text = transform.name;
			while (transform.parent && transform.parent != stopper)
			{
				transform = transform.parent;
				text = transform.name + "/" + text;
			}
			return "/" + text;
		}

		// Token: 0x06006603 RID: 26115 RVA: 0x00212A02 File Offset: 0x00210C02
		public static string GetPath(this GameObject gameObject)
		{
			return gameObject.transform.GetPath();
		}

		// Token: 0x06006604 RID: 26116 RVA: 0x00212A0F File Offset: 0x00210C0F
		public static void GetPath(this GameObject gameObject, ref Utf16ValueStringBuilder sb)
		{
			gameObject.transform.GetPathQ(ref sb);
		}

		// Token: 0x06006605 RID: 26117 RVA: 0x00212A1D File Offset: 0x00210C1D
		public static string GetPath(this GameObject gameObject, int limit)
		{
			return gameObject.transform.GetPath(limit);
		}

		// Token: 0x06006606 RID: 26118 RVA: 0x00212A2C File Offset: 0x00210C2C
		public static string[] GetPaths(this GameObject[] gobj)
		{
			string[] array = new string[gobj.Length];
			for (int i = 0; i < gobj.Length; i++)
			{
				array[i] = gobj[i].GetPath();
			}
			return array;
		}

		// Token: 0x06006607 RID: 26119 RVA: 0x00212A5C File Offset: 0x00210C5C
		public static string[] GetPaths(this Transform[] xform)
		{
			string[] array = new string[xform.Length];
			for (int i = 0; i < xform.Length; i++)
			{
				array[i] = xform[i].GetPath();
			}
			return array;
		}

		// Token: 0x06006608 RID: 26120 RVA: 0x00212A8C File Offset: 0x00210C8C
		[MethodImpl(256)]
		public static void GetRelativePath(string fromPath, string toPath, ref Utf16ValueStringBuilder ZStringBuilder)
		{
			if (string.IsNullOrEmpty(fromPath) || string.IsNullOrEmpty(toPath))
			{
				return;
			}
			int num = 0;
			while (num < fromPath.Length && fromPath.get_Chars(num) == '/')
			{
				num++;
			}
			int num2 = 0;
			while (num2 < toPath.Length && toPath.get_Chars(num2) == '/')
			{
				num2++;
			}
			int num3 = -1;
			int num4 = Mathf.Min(fromPath.Length - num, toPath.Length - num2);
			bool flag = true;
			for (int i = 0; i < num4; i++)
			{
				if (fromPath.get_Chars(num + i) != toPath.get_Chars(num2 + i))
				{
					flag = false;
					break;
				}
				if (fromPath.get_Chars(num + i) == '/')
				{
					num3 = i;
				}
			}
			if (flag && fromPath.Length - num > num4)
			{
				flag = (fromPath.get_Chars(num + num4) == '/');
			}
			else if (flag && toPath.Length - num2 > num4)
			{
				flag = (toPath.get_Chars(num2 + num4) == '/');
			}
			num3 = (flag ? num4 : num3);
			int num5 = (num3 < fromPath.Length - num) ? (num3 + 1) : (fromPath.Length - num);
			int num6 = (num3 < toPath.Length - num2) ? (num3 + 1) : (toPath.Length - num2);
			if (num5 < fromPath.Length - num)
			{
				ZStringBuilder.Append("../");
				for (int j = num5; j < fromPath.Length - num; j++)
				{
					if (fromPath.get_Chars(num + j) == '/')
					{
						ZStringBuilder.Append("../");
					}
				}
			}
			else
			{
				ZStringBuilder.Append((toPath.Length - num2 - num6 > 0) ? "./" : ".");
			}
			ZStringBuilder.Append(toPath, num2 + num6, toPath.Length - (num2 + num6));
		}

		// Token: 0x06006609 RID: 26121 RVA: 0x00212C34 File Offset: 0x00210E34
		public static string GetRelativePath(string fromPath, string toPath)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				GTExt.GetRelativePath(fromPath, toPath, ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
				utf16ValueStringBuilder.Dispose();
			}
			return result;
		}

		// Token: 0x0600660A RID: 26122 RVA: 0x00212C7C File Offset: 0x00210E7C
		[MethodImpl(256)]
		public static void GetRelativePath(this Transform fromXform, Transform toXform, ref Utf16ValueStringBuilder ZStringBuilder)
		{
			GTExt.GetRelativePath(fromXform.GetPath(), toXform.GetPath(), ref ZStringBuilder);
		}

		// Token: 0x0600660B RID: 26123 RVA: 0x00212C90 File Offset: 0x00210E90
		[MethodImpl(256)]
		public static string GetRelativePath(this Transform fromXform, Transform toXform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				fromXform.GetRelativePath(toXform, ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
				utf16ValueStringBuilder.Dispose();
			}
			return result;
		}

		// Token: 0x0600660C RID: 26124 RVA: 0x00212CD8 File Offset: 0x00210ED8
		public static void GetPathWithSiblingIndexes(this Transform transform, ref Utf16ValueStringBuilder strBuilder)
		{
			int length = strBuilder.Length;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x0600660D RID: 26125 RVA: 0x00212D40 File Offset: 0x00210F40
		public static string GetComponentPath(this Component component, int maxDepth = 2147483647)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				component.GetComponentPath(ref utf16ValueStringBuilder, maxDepth);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x0600660E RID: 26126 RVA: 0x00212D80 File Offset: 0x00210F80
		public static string GetComponentPath<T>(this T component, int maxDepth = 2147483647) where T : Component
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				component.GetComponentPath(ref utf16ValueStringBuilder, maxDepth);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x0600660F RID: 26127 RVA: 0x00212DC0 File Offset: 0x00210FC0
		public static void GetComponentPath<T>(this T component, ref Utf16ValueStringBuilder strBuilder, int maxDepth = 2147483647) where T : Component
		{
			Transform transform = component.transform;
			int length = strBuilder.Length;
			if (maxDepth > 0)
			{
				strBuilder.Append("/");
			}
			strBuilder.Append("->/");
			Type typeFromHandle = typeof(T);
			strBuilder.Append(typeFromHandle.Name);
			if (maxDepth <= 0)
			{
				return;
			}
			int num = 0;
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				num++;
				if (maxDepth <= num)
				{
					break;
				}
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x06006610 RID: 26128 RVA: 0x00212E4C File Offset: 0x0021104C
		public static void GetComponentPathWithSiblingIndexes<T>(this T component, ref Utf16ValueStringBuilder strBuilder) where T : Component
		{
			Transform transform = component.transform;
			int length = strBuilder.Length;
			strBuilder.Append("/->/");
			Type typeFromHandle = typeof(T);
			strBuilder.Append(typeFromHandle.Name);
			while (transform != null)
			{
				strBuilder.Insert(length, transform.name);
				strBuilder.Insert(length, "|");
				strBuilder.Insert(length, transform.GetSiblingIndex().ToString("0000"));
				strBuilder.Insert(length, "/");
				transform = transform.parent;
			}
		}

		// Token: 0x06006611 RID: 26129 RVA: 0x00212EE0 File Offset: 0x002110E0
		public static string GetComponentPathWithSiblingIndexes<T>(this T component) where T : Component
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				component.GetComponentPathWithSiblingIndexes(ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06006612 RID: 26130 RVA: 0x00212F20 File Offset: 0x00211120
		public static T GetComponentByPath<T>(this GameObject root, string path) where T : Component
		{
			string[] array = path.Split(new string[]
			{
				"/->/"
			}, 0);
			if (array.Length < 2)
			{
				return default(T);
			}
			string[] array2 = array[0].Split(new string[]
			{
				"/"
			}, 1);
			Transform transform = root.transform;
			for (int i = 1; i < array2.Length; i++)
			{
				string text = array2[i];
				transform = transform.Find(text);
				if (transform == null)
				{
					return default(T);
				}
			}
			Type type = Type.GetType(array[1].Split('#', 0)[0]);
			if (type == null)
			{
				return default(T);
			}
			Component component = transform.GetComponent(type);
			if (component == null)
			{
				return default(T);
			}
			return component as T;
		}

		// Token: 0x06006613 RID: 26131 RVA: 0x00212FFC File Offset: 0x002111FC
		public static int GetDepth(this Transform xform)
		{
			int num = 0;
			Transform parent = xform.parent;
			while (parent != null)
			{
				num++;
				parent = parent.parent;
			}
			return num;
		}

		// Token: 0x06006614 RID: 26132 RVA: 0x0021302C File Offset: 0x0021122C
		public static string GetPathWithSiblingIndexes(this Transform transform)
		{
			Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder();
			string result;
			try
			{
				transform.GetPathWithSiblingIndexes(ref utf16ValueStringBuilder);
			}
			finally
			{
				result = utf16ValueStringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x06006615 RID: 26133 RVA: 0x0021306C File Offset: 0x0021126C
		public static void GetPathWithSiblingIndexes(this GameObject gameObject, ref Utf16ValueStringBuilder stringBuilder)
		{
			gameObject.transform.GetPathWithSiblingIndexes(ref stringBuilder);
		}

		// Token: 0x06006616 RID: 26134 RVA: 0x0021307A File Offset: 0x0021127A
		public static string GetPathWithSiblingIndexes(this GameObject gameObject)
		{
			return gameObject.transform.GetPathWithSiblingIndexes();
		}

		// Token: 0x06006617 RID: 26135 RVA: 0x00213088 File Offset: 0x00211288
		public static void SetFromMatrix(this Transform transform, Matrix4x4 matrix, bool useLocal = false)
		{
			if (useLocal)
			{
				transform.localPosition = matrix.GetPosition();
				transform.localRotation = matrix.rotation;
				transform.localScale = matrix.lossyScale;
				return;
			}
			transform.position = matrix.GetPosition();
			transform.rotation = matrix.rotation;
			transform.SetScaleFromMatrix(matrix);
		}

		// Token: 0x06006618 RID: 26136 RVA: 0x002130E4 File Offset: 0x002112E4
		public static void SetScale(this Transform transform, Vector3 scale)
		{
			if (transform.parent)
			{
				transform.localScale = (transform.parent.worldToLocalMatrix * Matrix4x4.TRS(transform.position, transform.rotation, scale)).lossyScale;
				return;
			}
			transform.localScale = scale;
		}

		// Token: 0x06006619 RID: 26137 RVA: 0x00213138 File Offset: 0x00211338
		public static void SetScaleFromMatrix(this Transform transform, Matrix4x4 matrix)
		{
			if (transform.parent)
			{
				transform.localScale = (transform.parent.worldToLocalMatrix * matrix).lossyScale;
				return;
			}
			transform.localScale = matrix.lossyScale;
		}

		// Token: 0x0600661A RID: 26138 RVA: 0x0021317F File Offset: 0x0021137F
		public static void AddDictValue(Transform xForm, Dictionary<string, Transform> dict)
		{
			GTExt.caseSenseInner.Add(xForm, dict);
		}

		// Token: 0x0600661B RID: 26139 RVA: 0x0021318D File Offset: 0x0021138D
		public static void ClearDicts()
		{
			GTExt.caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
			GTExt.caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();
		}

		// Token: 0x0600661C RID: 26140 RVA: 0x002131A4 File Offset: 0x002113A4
		public static bool TryFindByExactPath([NotNull] string path, out Transform result, FindObjectsInactive findObjectsInactive = 1)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			if (findObjectsInactive != null)
			{
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded && sceneAt.TryFindByExactPath(path, out result))
					{
						return true;
					}
				}
				result = null;
				return false;
			}
			if (path.get_Chars(0) != '/')
			{
				path = "/" + path;
			}
			GameObject gameObject = GameObject.Find(path);
			if (gameObject)
			{
				result = gameObject.transform;
				return true;
			}
			result = null;
			return false;
		}

		// Token: 0x0600661D RID: 26141 RVA: 0x00213230 File Offset: 0x00211430
		public static bool TryFindByExactPath(this Scene scene, string path, out Transform result)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', 1);
			return scene.TryFindByExactPath(splitPath, out result);
		}

		// Token: 0x0600661E RID: 26142 RVA: 0x00213264 File Offset: 0x00211464
		private static bool TryFindByExactPath(this Scene scene, IReadOnlyList<string> splitPath, out Transform result)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (GTExt.TryFindByExactPath_Internal(rootGameObjects[i].transform, splitPath, 0, out result))
				{
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x0600661F RID: 26143 RVA: 0x002132A0 File Offset: 0x002114A0
		public static bool TryFindByExactPath(this Transform rootXform, string path, out Transform result)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("TryFindByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', 1);
			using (IEnumerator enumerator = rootXform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, 0, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06006620 RID: 26144 RVA: 0x00213320 File Offset: 0x00211520
		public static bool TryFindByExactPath(this Transform rootXform, IReadOnlyList<string> splitPath, out Transform result)
		{
			using (IEnumerator enumerator = rootXform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, 0, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06006621 RID: 26145 RVA: 0x00213380 File Offset: 0x00211580
		private static bool TryFindByExactPath_Internal(Transform current, IReadOnlyList<string> splitPath, int index, out Transform result)
		{
			if (current.name != splitPath[index])
			{
				result = null;
				return false;
			}
			if (index == splitPath.Count - 1)
			{
				result = current;
				return true;
			}
			using (IEnumerator enumerator = current.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (GTExt.TryFindByExactPath_Internal((Transform)enumerator.Current, splitPath, index + 1, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06006622 RID: 26146 RVA: 0x0021340C File Offset: 0x0021160C
		public static bool TryFindByPath(string globPath, out Transform result, bool caseSensitive = false)
		{
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return GTExt._TryFindByPath(null, pathPartsRegex, -1, out result, caseSensitive, true, globPath);
		}

		// Token: 0x06006623 RID: 26147 RVA: 0x0021342C File Offset: 0x0021162C
		public static bool TryFindByPath(this Scene scene, string globPath, out Transform result, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
			}
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return scene.TryFindByPath(pathPartsRegex, out result, globPath, caseSensitive);
		}

		// Token: 0x06006624 RID: 26148 RVA: 0x00213460 File Offset: 0x00211660
		private static bool TryFindByPath(this Scene scene, IReadOnlyList<string> pathPartsRegex, out Transform result, string globPath, bool caseSensitive = false)
		{
			GameObject[] rootGameObjects = scene.GetRootGameObjects();
			for (int i = 0; i < rootGameObjects.Length; i++)
			{
				if (GTExt._TryFindByPath(rootGameObjects[i].transform, pathPartsRegex, 0, out result, caseSensitive, false, globPath))
				{
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06006625 RID: 26149 RVA: 0x002134A0 File Offset: 0x002116A0
		public static bool TryFindByPath(this Transform rootXform, string globPath, out Transform result, bool caseSensitive = false)
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("TryFindByPath: Provided path cannot be null or empty.");
			}
			char c = globPath.get_Chars(0);
			if (c != ' ' && c != '\n' && c != '\t')
			{
				c = globPath.get_Chars(globPath.Length - 1);
				if (c != ' ' && c != '\n' && c != '\t')
				{
					string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
					return GTExt._TryFindByPath(rootXform, pathPartsRegex, -1, out result, caseSensitive, false, globPath);
				}
			}
			throw new Exception("TryFindByPath: Provided globPath cannot end or start with whitespace.\nProvided globPath=\"" + globPath + "\"");
		}

		// Token: 0x06006626 RID: 26150 RVA: 0x00213523 File Offset: 0x00211723
		public static List<string> ShowAllStringsUsed()
		{
			return Enumerable.ToList<string>(GTExt.allStringsUsed.Keys);
		}

		// Token: 0x06006627 RID: 26151 RVA: 0x00213534 File Offset: 0x00211734
		private static bool _TryFindByPath(Transform current, IReadOnlyList<string> pathPartsRegex, int index, out Transform result, bool caseSensitive, bool isAtSceneLevel, string joinedPath)
		{
			if (joinedPath != null && !GTExt.allStringsUsed.ContainsKey(joinedPath))
			{
				GTExt.allStringsUsed[joinedPath] = joinedPath;
			}
			if (caseSensitive)
			{
				if (GTExt.caseSenseInner.ContainsKey(current))
				{
					if (GTExt.caseSenseInner[current].ContainsKey(joinedPath))
					{
						result = GTExt.caseSenseInner[current][joinedPath];
						return true;
					}
				}
				else
				{
					GTExt.caseSenseInner[current] = new Dictionary<string, Transform>();
				}
			}
			else if (GTExt.caseInsenseInner.ContainsKey(current))
			{
				if (GTExt.caseInsenseInner[current].ContainsKey(joinedPath))
				{
					result = GTExt.caseInsenseInner[current][joinedPath];
					return true;
				}
			}
			else
			{
				GTExt.caseInsenseInner[current] = new Dictionary<string, Transform>();
			}
			string text;
			if (isAtSceneLevel)
			{
				index = ((index == -1) ? 0 : index);
				text = pathPartsRegex[index];
				if (text == ".." || text == "..**" || text == "**..")
				{
					result = null;
					return false;
				}
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
						for (int j = 0; j < rootGameObjects.Length; j++)
						{
							if (GTExt._TryFindByPath(rootGameObjects[j].transform, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
							{
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
						}
					}
				}
			}
			if (index != -1)
			{
				text = pathPartsRegex[index];
				if (!(text == "."))
				{
					if (!(text == ".."))
					{
						if (text == "**")
						{
							goto IL_50A;
						}
						if (!(text == "..**") && !(text == "**.."))
						{
							if (!Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? 0 : 1))
							{
								goto IL_8CB;
							}
							if (index == pathPartsRegex.Count - 1)
							{
								result = current;
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
							using (IEnumerator enumerator = current.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index + 1, out result, caseSensitive, false, joinedPath))
									{
										if (caseSensitive)
										{
											GTExt.caseSenseInner[current][joinedPath] = result;
										}
										else
										{
											GTExt.caseInsenseInner[current][joinedPath] = result;
										}
										return true;
									}
								}
							}
							goto IL_8CB;
						}
						else
						{
							string text2;
							do
							{
								index++;
								if (index >= pathPartsRegex.Count)
								{
									break;
								}
								text2 = pathPartsRegex[index];
							}
							while (text2 == "..**" || text2 == "**..");
							if (index == pathPartsRegex.Count)
							{
								result = current.root;
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
							Transform parent = current.parent;
							while (parent)
							{
								if (GTExt._TryFindByPath(parent, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
								{
									if (caseSensitive)
									{
										GTExt.caseSenseInner[current][joinedPath] = result;
									}
									else
									{
										GTExt.caseInsenseInner[current][joinedPath] = result;
									}
									return true;
								}
								using (IEnumerator enumerator = parent.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
										{
											if (caseSensitive)
											{
												GTExt.caseSenseInner[current][joinedPath] = result;
											}
											else
											{
												GTExt.caseInsenseInner[current][joinedPath] = result;
											}
											return true;
										}
									}
								}
								parent = parent.parent;
							}
							if (parent != null)
							{
								goto IL_8CB;
							}
							bool result2 = GTExt._TryFindByPath(current.root, pathPartsRegex, index, out result, caseSensitive, true, joinedPath);
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
								return result2;
							}
							GTExt.caseInsenseInner[current][joinedPath] = result;
							return result2;
						}
					}
				}
				else
				{
					while (pathPartsRegex[index] == ".")
					{
						if (index == pathPartsRegex.Count - 1)
						{
							result = current;
							return true;
						}
						index++;
					}
					if (GTExt._TryFindByPath(current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
					{
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
					using (IEnumerator enumerator = current.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index, out result, caseSensitive, false, joinedPath))
							{
								if (caseSensitive)
								{
									GTExt.caseSenseInner[current][joinedPath] = result;
								}
								else
								{
									GTExt.caseInsenseInner[current][joinedPath] = result;
								}
								return true;
							}
						}
						goto IL_8CB;
					}
				}
				Transform transform = current;
				int num = index;
				while (pathPartsRegex[num] == "..")
				{
					if (num + 1 >= pathPartsRegex.Count)
					{
						result = transform.parent;
						return result != null;
					}
					if (transform.parent == null)
					{
						bool result3 = GTExt._TryFindByPath(transform, pathPartsRegex, num + 1, out result, caseSensitive, true, joinedPath);
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
							return result3;
						}
						GTExt.caseInsenseInner[current][joinedPath] = result;
						return result3;
					}
					else
					{
						transform = transform.parent;
						num++;
					}
				}
				using (IEnumerator enumerator = transform.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, num, out result, caseSensitive, false, joinedPath))
						{
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
							}
							else
							{
								GTExt.caseInsenseInner[current][joinedPath] = result;
							}
							return true;
						}
					}
					goto IL_8CB;
				}
				IL_50A:
				if (index == pathPartsRegex.Count - 1)
				{
					result = ((current.childCount > 0) ? current.GetChild(0) : null);
					return current.childCount > 0;
				}
				if (index <= pathPartsRegex.Count - 1 && Regex.IsMatch(current.name, pathPartsRegex[index + 1], caseSensitive ? 0 : 1))
				{
					if (index + 2 == pathPartsRegex.Count)
					{
						result = current;
						return true;
					}
					using (IEnumerator enumerator = current.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
							{
								return true;
							}
						}
					}
				}
				Transform transform2;
				if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out transform2, caseSensitive))
				{
					if (index + 2 == pathPartsRegex.Count)
					{
						result = transform2;
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
					if (GTExt._TryFindByPath(transform2, pathPartsRegex, index + 2, out result, caseSensitive, false, joinedPath))
					{
						if (caseSensitive)
						{
							GTExt.caseSenseInner[current][joinedPath] = result;
						}
						else
						{
							GTExt.caseInsenseInner[current][joinedPath] = result;
						}
						return true;
					}
				}
				IL_8CB:
				result = null;
				if (caseSensitive)
				{
					GTExt.caseSenseInner[current][joinedPath] = result;
				}
				else
				{
					GTExt.caseInsenseInner[current][joinedPath] = result;
				}
				return false;
			}
			if (pathPartsRegex.Count == 0)
			{
				result = null;
				return false;
			}
			text = pathPartsRegex[0];
			if (!(text == ".") && !(text == "..") && !(text == "..**") && !(text == "**.."))
			{
				using (IEnumerator enumerator = current.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (GTExt._TryFindByPath((Transform)enumerator.Current, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath))
						{
							if (caseSensitive)
							{
								GTExt.caseSenseInner[current][joinedPath] = result;
							}
							else
							{
								GTExt.caseInsenseInner[current][joinedPath] = result;
							}
							return true;
						}
					}
				}
				result = null;
				if (caseSensitive)
				{
					GTExt.caseSenseInner[current][joinedPath] = result;
				}
				else
				{
					GTExt.caseInsenseInner[current][joinedPath] = result;
				}
				return false;
			}
			bool result4 = GTExt._TryFindByPath(current, pathPartsRegex, 0, out result, caseSensitive, false, joinedPath);
			if (caseSensitive)
			{
				GTExt.caseSenseInner[current][joinedPath] = result;
				return result4;
			}
			GTExt.caseInsenseInner[current][joinedPath] = result;
			return result4;
		}

		// Token: 0x06006628 RID: 26152 RVA: 0x00213E90 File Offset: 0x00212090
		private static bool _TryBreadthFirstSearchNames(Transform root, string regexPattern, out Transform result, bool caseSensitive)
		{
			Queue<Transform> queue = new Queue<Transform>();
			using (IEnumerator enumerator = root.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Transform transform = (Transform)obj;
					queue.Enqueue(transform);
				}
				goto IL_9B;
			}
			IL_3D:
			Transform transform2 = queue.Dequeue();
			if (Regex.IsMatch(transform2.name, regexPattern, caseSensitive ? 0 : 1))
			{
				result = transform2;
				return true;
			}
			foreach (object obj2 in transform2)
			{
				Transform transform3 = (Transform)obj2;
				queue.Enqueue(transform3);
			}
			IL_9B:
			if (queue.Count <= 0)
			{
				result = null;
				return false;
			}
			goto IL_3D;
		}

		// Token: 0x06006629 RID: 26153 RVA: 0x00213F64 File Offset: 0x00212164
		public static T[] FindComponentsByExactPath<T>(string path) where T : Component
		{
			List<T> list;
			T[] result;
			using (CollectionPool<List<T>, T>.Get(ref list))
			{
				ListExtensions.EnsureCapacity<T>(list, 64);
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						list.AddRange(sceneAt.FindComponentsByExactPath(path));
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x0600662A RID: 26154 RVA: 0x00213FD8 File Offset: 0x002121D8
		public static T[] FindComponentsByExactPath<T>(this Scene scene, string path) where T : Component
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', 1);
			return scene.FindComponentsByExactPath(splitPath);
		}

		// Token: 0x0600662B RID: 26155 RVA: 0x0021400C File Offset: 0x0021220C
		private static T[] FindComponentsByExactPath<T>(this Scene scene, string[] splitPath) where T : Component
		{
			List<T> list;
			T[] result;
			using (CollectionPool<List<T>, T>.Get(ref list))
			{
				ListExtensions.EnsureCapacity<T>(list, 64);
				GameObject[] rootGameObjects = scene.GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					GTExt._FindComponentsByExactPath<T>(rootGameObjects[i].transform, splitPath, 0, list);
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x0600662C RID: 26156 RVA: 0x0021407C File Offset: 0x0021227C
		public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string path) where T : Component
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("FindComponentsByExactPath: Provided path cannot be null or empty.");
			}
			string[] splitPath = path.Split('/', 1);
			List<T> list;
			T[] result;
			using (CollectionPool<List<T>, T>.Get(ref list))
			{
				ListExtensions.EnsureCapacity<T>(list, 64);
				foreach (object obj in rootXform)
				{
					GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, 0, list);
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x0600662D RID: 26157 RVA: 0x00214128 File Offset: 0x00212328
		public static T[] FindComponentsByExactPath<T>(this Transform rootXform, string[] splitPath) where T : Component
		{
			List<T> list;
			T[] result;
			using (CollectionPool<List<T>, T>.Get(ref list))
			{
				ListExtensions.EnsureCapacity<T>(list, 64);
				foreach (object obj in rootXform)
				{
					GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, 0, list);
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x0600662E RID: 26158 RVA: 0x002141B4 File Offset: 0x002123B4
		private static void _FindComponentsByExactPath<T>(Transform current, string[] splitPath, int index, List<T> components) where T : Component
		{
			if (current.name != splitPath[index])
			{
				return;
			}
			if (index == splitPath.Length - 1)
			{
				T component = current.GetComponent<T>();
				if (component)
				{
					components.Add(component);
				}
				return;
			}
			foreach (object obj in current)
			{
				GTExt._FindComponentsByExactPath<T>((Transform)obj, splitPath, index + 1, components);
			}
		}

		// Token: 0x0600662F RID: 26159 RVA: 0x00214240 File Offset: 0x00212440
		public static T[] FindComponentsByPathInLoadedScenes<T>(string wildcardPath, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] result;
			using (CollectionPool<List<T>, T>.Get(ref list))
			{
				ListExtensions.EnsureCapacity<T>(list, 64);
				string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(wildcardPath);
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
						for (int j = 0; j < rootGameObjects.Length; j++)
						{
							GTExt._FindComponentsByPath<T>(rootGameObjects[j].transform, pathPartsRegex, list, caseSensitive);
						}
					}
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06006630 RID: 26160 RVA: 0x002142E0 File Offset: 0x002124E0
		public static T[] FindComponentsByPath<T>(this Scene scene, string globPath, bool caseSensitive = false) where T : Component
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
			}
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return scene.FindComponentsByPath(pathPartsRegex, caseSensitive);
		}

		// Token: 0x06006631 RID: 26161 RVA: 0x00214310 File Offset: 0x00212510
		private static T[] FindComponentsByPath<T>(this Scene scene, string[] pathPartsRegex, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] result;
			using (CollectionPool<List<T>, T>.Get(ref list))
			{
				ListExtensions.EnsureCapacity<T>(list, 64);
				GameObject[] rootGameObjects = scene.GetRootGameObjects();
				for (int i = 0; i < rootGameObjects.Length; i++)
				{
					GTExt._FindComponentsByPath<T>(rootGameObjects[i].transform, pathPartsRegex, list, caseSensitive);
				}
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06006632 RID: 26162 RVA: 0x00214380 File Offset: 0x00212580
		public static T[] FindComponentsByPath<T>(this Transform rootXform, string globPath, bool caseSensitive = false) where T : Component
		{
			if (string.IsNullOrEmpty(globPath))
			{
				throw new Exception("FindComponentsByPath: Provided path cannot be null or empty.");
			}
			string[] pathPartsRegex = GTExt._GlobPathToPathPartsRegex(globPath);
			return rootXform.FindComponentsByPath(pathPartsRegex, caseSensitive);
		}

		// Token: 0x06006633 RID: 26163 RVA: 0x002143B0 File Offset: 0x002125B0
		public static T[] FindComponentsByPath<T>(this Transform rootXform, string[] pathPartsRegex, bool caseSensitive = false) where T : Component
		{
			List<T> list;
			T[] result;
			using (CollectionPool<List<T>, T>.Get(ref list))
			{
				ListExtensions.EnsureCapacity<T>(list, 64);
				GTExt._FindComponentsByPath<T>(rootXform, pathPartsRegex, list, caseSensitive);
				result = list.ToArray();
			}
			return result;
		}

		// Token: 0x06006634 RID: 26164 RVA: 0x00214400 File Offset: 0x00212600
		public static void _FindComponentsByPath<T>(Transform current, string[] pathPartsRegex, List<T> components, bool caseSensitive) where T : Component
		{
			List<Transform> list;
			using (CollectionPool<List<Transform>, Transform>.Get(ref list))
			{
				ListExtensions.EnsureCapacity<Transform>(list, 64);
				if (GTExt._TryFindAllByPath(current, pathPartsRegex, 0, list, caseSensitive, false))
				{
					for (int i = 0; i < list.Count; i++)
					{
						T[] components2 = list[i].GetComponents<T>();
						components.AddRange(components2);
					}
				}
			}
		}

		// Token: 0x06006635 RID: 26165 RVA: 0x00214474 File Offset: 0x00212674
		private static bool _TryFindAllByPath(Transform current, IReadOnlyList<string> pathPartsRegex, int index, List<Transform> results, bool caseSensitive, bool isAtSceneLevel = false)
		{
			bool flag = false;
			string text;
			if (isAtSceneLevel)
			{
				text = pathPartsRegex[index];
				if (text == ".." || text == "..**" || text == "**..")
				{
					return false;
				}
				for (int i = 0; i < SceneManager.sceneCount; i++)
				{
					Scene sceneAt = SceneManager.GetSceneAt(i);
					if (sceneAt.isLoaded)
					{
						foreach (GameObject gameObject in sceneAt.GetRootGameObjects())
						{
							flag |= GTExt._TryFindAllByPath(gameObject.transform, pathPartsRegex, index, results, caseSensitive, false);
						}
					}
				}
			}
			text = pathPartsRegex[index];
			if (!(text == "."))
			{
				if (!(text == ".."))
				{
					Transform transform2;
					if (!(text == "**"))
					{
						if (!(text == "..**") && !(text == "**.."))
						{
							if (Regex.IsMatch(current.name, pathPartsRegex[index], caseSensitive ? 0 : 1))
							{
								if (index == pathPartsRegex.Count - 1)
								{
									results.Add(current);
									return true;
								}
								foreach (object obj in current)
								{
									Transform current2 = (Transform)obj;
									flag |= GTExt._TryFindAllByPath(current2, pathPartsRegex, index + 1, results, caseSensitive, false);
								}
							}
						}
						else
						{
							int k;
							for (k = index + 1; k < pathPartsRegex.Count; k++)
							{
								string text2 = pathPartsRegex[k];
								if (!(text2 == "..**") && !(text2 == "**.."))
								{
									break;
								}
							}
							if (k == pathPartsRegex.Count)
							{
								results.Add(current.root);
								return true;
							}
							Transform transform = current;
							while (transform)
							{
								flag |= GTExt._TryFindAllByPath(transform, pathPartsRegex, index + 1, results, caseSensitive, false);
								transform = transform.parent;
							}
						}
					}
					else if (index == pathPartsRegex.Count - 1)
					{
						for (int l = 0; l < current.childCount; l++)
						{
							results.Add(current.GetChild(l));
							flag = true;
						}
					}
					else if (GTExt._TryBreadthFirstSearchNames(current, pathPartsRegex[index + 1], out transform2, caseSensitive))
					{
						if (index + 2 == pathPartsRegex.Count)
						{
							results.Add(transform2);
							return true;
						}
						flag |= GTExt._TryFindAllByPath(transform2, pathPartsRegex, index + 2, results, caseSensitive, false);
					}
				}
				else if (current.parent)
				{
					if (index == pathPartsRegex.Count - 1)
					{
						results.Add(current.parent);
						return true;
					}
					flag |= GTExt._TryFindAllByPath(current.parent, pathPartsRegex, index + 1, results, caseSensitive, false);
				}
			}
			else
			{
				if (index == pathPartsRegex.Count - 1)
				{
					results.Add(current);
					return true;
				}
				flag |= GTExt._TryFindAllByPath(current, pathPartsRegex, index + 1, results, caseSensitive, false);
			}
			return flag;
		}

		// Token: 0x06006636 RID: 26166 RVA: 0x0021475C File Offset: 0x0021295C
		public static string[] _GlobPathToPathPartsRegex(string path)
		{
			string[] array = path.Split('/', 1);
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (i > 0)
				{
					string text = array[i];
					if (text == "**" || text == "..**" || text == "**..")
					{
						text = array[i - 1];
						if (text == "**" || text == "..**" || text == "**..")
						{
							num++;
						}
					}
				}
				array[i - num] = array[i];
			}
			if (num > 0)
			{
				Array.Resize<string>(ref array, array.Length - num);
			}
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = GTExt._GlobPathPartToRegex(array[j]);
			}
			return array;
		}

		// Token: 0x06006637 RID: 26167 RVA: 0x0021481C File Offset: 0x00212A1C
		private static string _GlobPathPartToRegex(string pattern)
		{
			if (pattern == "." || pattern == ".." || pattern == "**" || pattern == "..**" || pattern == "**.." || pattern.StartsWith("^"))
			{
				return pattern;
			}
			return "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
		}

		// Token: 0x06006639 RID: 26169 RVA: 0x002148C0 File Offset: 0x00212AC0
		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass7_0<T, TStop1> A_2) where T : Component where TStop1 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|7_0<T, TStop1>(transform, ref components, ref A_2);
				}
			}
		}

		// Token: 0x0600663A RID: 26170 RVA: 0x0021495C File Offset: 0x00212B5C
		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|10_0<T, TStop1, TStop2>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass10_0<T, TStop1, TStop2> A_2) where T : Component where TStop1 : Component where TStop2 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null) && !(transform.GetComponent<TStop2>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|10_0<T, TStop1, TStop2>(transform, ref components, ref A_2);
				}
			}
		}

		// Token: 0x0600663B RID: 26171 RVA: 0x00214A0C File Offset: 0x00212C0C
		[CompilerGenerated]
		internal static void <GetComponentsInChildrenUntil>g__GetRecursive|11_0<T, TStop1, TStop2, TStop3>(Transform currentTransform, ref List<T> components, ref GTExt.<>c__DisplayClass11_0<T, TStop1, TStop2, TStop3> A_2) where T : Component where TStop1 : Component where TStop2 : Component where TStop3 : Component
		{
			foreach (object obj in currentTransform)
			{
				Transform transform = (Transform)obj;
				if ((A_2.includeInactive || transform.gameObject.activeSelf) && !(transform.GetComponent<TStop1>() != null) && !(transform.GetComponent<TStop2>() != null) && !(transform.GetComponent<TStop3>() != null))
				{
					T component = transform.GetComponent<T>();
					if (component != null)
					{
						components.Add(component);
					}
					GTExt.<GetComponentsInChildrenUntil>g__GetRecursive|11_0<T, TStop1, TStop2, TStop3>(transform, ref components, ref A_2);
				}
			}
		}

		// Token: 0x040074C2 RID: 29890
		private static Dictionary<Transform, Dictionary<string, Transform>> caseSenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();

		// Token: 0x040074C3 RID: 29891
		private static Dictionary<Transform, Dictionary<string, Transform>> caseInsenseInner = new Dictionary<Transform, Dictionary<string, Transform>>();

		// Token: 0x040074C4 RID: 29892
		public static Dictionary<string, string> allStringsUsed = new Dictionary<string, string>();

		// Token: 0x02000FBD RID: 4029
		public enum ParityOptions
		{
			// Token: 0x040074C6 RID: 29894
			XFlip,
			// Token: 0x040074C7 RID: 29895
			YFlip,
			// Token: 0x040074C8 RID: 29896
			ZFlip,
			// Token: 0x040074C9 RID: 29897
			AllFlip
		}
	}
}
