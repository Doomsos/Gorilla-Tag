using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004A9 RID: 1193
[DefaultExecutionOrder(1549)]
public class TransferrableObjectManager : MonoBehaviour
{
	// Token: 0x06001ED3 RID: 7891 RVA: 0x000A3AAA File Offset: 0x000A1CAA
	protected void Awake()
	{
		if (TransferrableObjectManager.hasInstance && TransferrableObjectManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		TransferrableObjectManager.SetInstance(this);
	}

	// Token: 0x06001ED4 RID: 7892 RVA: 0x000A3ACD File Offset: 0x000A1CCD
	protected void OnDestroy()
	{
		if (TransferrableObjectManager.instance == this)
		{
			TransferrableObjectManager.hasInstance = false;
			TransferrableObjectManager.instance = null;
		}
	}

	// Token: 0x06001ED5 RID: 7893 RVA: 0x000A3AE8 File Offset: 0x000A1CE8
	protected void LateUpdate()
	{
		for (int i = 0; i < TransferrableObjectManager.transObs.Count; i++)
		{
			TransferrableObjectManager.transObs[i].TriggeredLateUpdate();
		}
	}

	// Token: 0x06001ED6 RID: 7894 RVA: 0x000A3B1A File Offset: 0x000A1D1A
	private static void CreateManager()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TransferrableObjectManager.SetInstance(new GameObject("TransferrableObjectManager").AddComponent<TransferrableObjectManager>());
	}

	// Token: 0x06001ED7 RID: 7895 RVA: 0x000A3B38 File Offset: 0x000A1D38
	private static void SetInstance(TransferrableObjectManager manager)
	{
		TransferrableObjectManager.instance = manager;
		TransferrableObjectManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001ED8 RID: 7896 RVA: 0x000A3B53 File Offset: 0x000A1D53
	public static void Register(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (!TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Add(transOb);
		}
	}

	// Token: 0x06001ED9 RID: 7897 RVA: 0x000A3B79 File Offset: 0x000A1D79
	public static void Unregister(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Remove(transOb);
		}
	}

	// Token: 0x04002928 RID: 10536
	public static TransferrableObjectManager instance;

	// Token: 0x04002929 RID: 10537
	public static bool hasInstance = false;

	// Token: 0x0400292A RID: 10538
	public static readonly List<TransferrableObject> transObs = new List<TransferrableObject>(1024);
}
