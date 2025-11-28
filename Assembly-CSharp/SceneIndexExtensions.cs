using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Token: 0x0200033C RID: 828
public static class SceneIndexExtensions
{
	// Token: 0x060013EB RID: 5099 RVA: 0x000735AB File Offset: 0x000717AB
	public static SceneIndex GetSceneIndex(this Scene scene)
	{
		return (SceneIndex)scene.buildIndex;
	}

	// Token: 0x060013EC RID: 5100 RVA: 0x000735B4 File Offset: 0x000717B4
	public static SceneIndex GetSceneIndex(this GameObject obj)
	{
		return (SceneIndex)obj.scene.buildIndex;
	}

	// Token: 0x060013ED RID: 5101 RVA: 0x000735D0 File Offset: 0x000717D0
	public static SceneIndex GetSceneIndex(this Component cmp)
	{
		return (SceneIndex)cmp.gameObject.scene.buildIndex;
	}

	// Token: 0x060013EE RID: 5102 RVA: 0x000735F0 File Offset: 0x000717F0
	public static void AddCallbackOnSceneLoad(this SceneIndex scene, Action callback)
	{
		if (SceneIndexExtensions.onSceneLoadCallbacks == null)
		{
			SceneIndexExtensions.onSceneLoadCallbacks = new List<Action>[19];
			for (int i = 0; i < SceneIndexExtensions.onSceneLoadCallbacks.Length; i++)
			{
				SceneIndexExtensions.onSceneLoadCallbacks[i] = new List<Action>();
			}
			SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(SceneIndexExtensions.OnSceneLoad);
		}
		SceneIndexExtensions.onSceneLoadCallbacks[(int)scene].Add(callback);
	}

	// Token: 0x060013EF RID: 5103 RVA: 0x0007364C File Offset: 0x0007184C
	public static void RemoveCallbackOnSceneLoad(this SceneIndex scene, Action callback)
	{
		SceneIndexExtensions.onSceneLoadCallbacks[(int)scene].Remove(callback);
	}

	// Token: 0x060013F0 RID: 5104 RVA: 0x0007365C File Offset: 0x0007185C
	public static void OnSceneLoad(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex != -1)
		{
			foreach (Action action in SceneIndexExtensions.onSceneLoadCallbacks[scene.buildIndex])
			{
				action.Invoke();
			}
		}
	}

	// Token: 0x060013F1 RID: 5105 RVA: 0x000736C0 File Offset: 0x000718C0
	public static void AddCallbackOnSceneUnload(this SceneIndex scene, Action callback)
	{
		if (SceneIndexExtensions.onSceneUnloadCallbacks == null)
		{
			SceneIndexExtensions.onSceneUnloadCallbacks = new List<Action>[19];
			for (int i = 0; i < SceneIndexExtensions.onSceneUnloadCallbacks.Length; i++)
			{
				SceneIndexExtensions.onSceneUnloadCallbacks[i] = new List<Action>();
			}
			SceneManager.sceneUnloaded += new UnityAction<Scene>(SceneIndexExtensions.OnSceneUnload);
		}
		SceneIndexExtensions.onSceneUnloadCallbacks[(int)scene].Add(callback);
	}

	// Token: 0x060013F2 RID: 5106 RVA: 0x0007371C File Offset: 0x0007191C
	public static void RemoveCallbackOnSceneUnload(this SceneIndex scene, Action callback)
	{
		SceneIndexExtensions.onSceneUnloadCallbacks[(int)scene].Remove(callback);
	}

	// Token: 0x060013F3 RID: 5107 RVA: 0x0007372C File Offset: 0x0007192C
	public static void OnSceneUnload(Scene scene)
	{
		if (scene.buildIndex != -1)
		{
			foreach (Action action in SceneIndexExtensions.onSceneUnloadCallbacks[scene.buildIndex])
			{
				action.Invoke();
			}
		}
	}

	// Token: 0x060013F4 RID: 5108 RVA: 0x00073790 File Offset: 0x00071990
	[OnEnterPlay_Run]
	private static void Reset()
	{
		if (SceneIndexExtensions.onSceneLoadCallbacks != null)
		{
			SceneIndexExtensions.onSceneLoadCallbacks = null;
			SceneManager.sceneLoaded -= new UnityAction<Scene, LoadSceneMode>(SceneIndexExtensions.OnSceneLoad);
		}
		if (SceneIndexExtensions.onSceneUnloadCallbacks != null)
		{
			SceneIndexExtensions.onSceneUnloadCallbacks = null;
			SceneManager.sceneUnloaded -= new UnityAction<Scene>(SceneIndexExtensions.OnSceneUnload);
		}
	}

	// Token: 0x04001E8E RID: 7822
	private const int SceneIndex_COUNT = 19;

	// Token: 0x04001E8F RID: 7823
	[OnEnterPlay_SetNull]
	private static List<Action>[] onSceneLoadCallbacks;

	// Token: 0x04001E90 RID: 7824
	[OnEnterPlay_SetNull]
	private static List<Action>[] onSceneUnloadCallbacks;
}
