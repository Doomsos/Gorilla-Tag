using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GorillaExtensions
{
	// Token: 0x02000FC1 RID: 4033
	public static class GTTryFindByExactPath
	{
		// Token: 0x0600663C RID: 26172 RVA: 0x00214AF0 File Offset: 0x00212CF0
		public static bool WithSiblingIndexAndTypeName<T>(string path, out T out_component) where T : Component
		{
			out_component = default(T);
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			int num = path.IndexOf("/->/", 4);
			if (num < 0)
			{
				return GTTryFindByExactPath.WithSiblingIndex<T>(path, out out_component);
			}
			string xformPath = path.Substring(0, num);
			string text = path.Substring(num + 4);
			int num2 = -1;
			int num3 = text.IndexOf('#');
			string text2;
			if (num3 >= 0)
			{
				text2 = text.Substring(0, num3);
				if (!int.TryParse(text.Substring(num3 + 1), ref num2))
				{
					num2 = -1;
				}
			}
			else
			{
				text2 = text;
			}
			Transform transform;
			if (!GTTryFindByExactPath.XformWithSiblingIndex(xformPath, out transform))
			{
				return false;
			}
			Type type = typeof(T);
			if (!string.Equals(type.Name, text2, 4))
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				Type type2 = null;
				Assembly[] array = assemblies;
				for (int i = 0; i < array.Length; i++)
				{
					type2 = array[i].GetType(text2);
					if (type2 != null && typeof(Component).IsAssignableFrom(type2))
					{
						type = type2;
						break;
					}
				}
				if (type2 == null)
				{
					out_component = transform.GetComponent<T>();
					return out_component != null;
				}
			}
			Component[] components = transform.GetComponents(type);
			T t = default(T);
			if (components.Length != 0)
			{
				if (num2 < 0)
				{
					t = (components[0] as T);
				}
				else
				{
					if (num2 >= components.Length)
					{
						return false;
					}
					t = (components[num2] as T);
				}
			}
			out_component = t;
			return out_component != null;
		}

		// Token: 0x0600663D RID: 26173 RVA: 0x00214C70 File Offset: 0x00212E70
		private static bool WithSiblingIndex<T>(string xformPath, out T component) where T : Component
		{
			component = default(T);
			Transform transform;
			if (GTTryFindByExactPath.XformWithSiblingIndex(xformPath, out transform))
			{
				component = transform.GetComponent<T>();
				return component != null;
			}
			return false;
		}

		// Token: 0x0600663E RID: 26174 RVA: 0x00214CB0 File Offset: 0x00212EB0
		public static bool XformWithSiblingIndex(string xformPath, out Transform finalXform)
		{
			finalXform = null;
			if (string.IsNullOrEmpty(xformPath))
			{
				return false;
			}
			string[] array = xformPath.Split('/', 1);
			if (array.Length == 0)
			{
				return false;
			}
			Transform transform = null;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				int num = text.IndexOf('|');
				if (num < 0)
				{
					return false;
				}
				string text2 = text.Substring(0, num);
				string text3 = text.Substring(num + 1);
				int num2;
				if (!int.TryParse(text2, ref num2))
				{
					return false;
				}
				if (i == 0)
				{
					Transform transform2 = null;
					int num3 = 0;
					while (num3 < SceneManager.sceneCount && transform2 == null)
					{
						Scene sceneAt = SceneManager.GetSceneAt(num3);
						if (sceneAt.IsValid() && sceneAt.isLoaded)
						{
							GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
							if (num2 >= 0 && num2 < rootGameObjects.Length)
							{
								Transform transform3 = rootGameObjects[num2].transform;
								if (string.Equals(transform3.name, text3, 4))
								{
									transform2 = transform3;
								}
							}
						}
						num3++;
					}
					if (transform2 == null)
					{
						return false;
					}
					transform = transform2;
				}
				else
				{
					if (num2 < 0 || num2 >= transform.childCount)
					{
						return false;
					}
					Transform child = transform.GetChild(num2);
					if (!string.Equals(child.name, text3, 4))
					{
						return false;
					}
					transform = child;
				}
			}
			finalXform = transform;
			return finalXform != null;
		}
	}
}
