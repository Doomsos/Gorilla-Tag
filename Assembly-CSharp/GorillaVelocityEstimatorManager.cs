using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x02000196 RID: 406
public class GorillaVelocityEstimatorManager : MonoBehaviour
{
	// Token: 0x06000AD8 RID: 2776 RVA: 0x0003ABC0 File Offset: 0x00038DC0
	protected void Awake()
	{
		if (GorillaVelocityEstimatorManager.hasInstance && GorillaVelocityEstimatorManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GorillaVelocityEstimatorManager.SetInstance(this);
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0003ABE3 File Offset: 0x00038DE3
	protected void OnDestroy()
	{
		if (GorillaVelocityEstimatorManager.instance == this)
		{
			GorillaVelocityEstimatorManager.hasInstance = false;
			GorillaVelocityEstimatorManager.instance = null;
		}
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0003AC00 File Offset: 0x00038E00
	protected void LateUpdate()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		for (int i = 0; i < GorillaVelocityEstimatorManager.estimators.Count; i++)
		{
			if (GorillaVelocityEstimatorManager.estimators[i] != null)
			{
				GorillaVelocityEstimatorManager.estimators[i].TriggeredLateUpdate();
			}
		}
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0003AC4D File Offset: 0x00038E4D
	public static void CreateManager()
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		GorillaVelocityEstimatorManager.SetInstance(new GameObject("GorillaVelocityEstimatorManager").AddComponent<GorillaVelocityEstimatorManager>());
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0003AC6B File Offset: 0x00038E6B
	private static void SetInstance(GorillaVelocityEstimatorManager manager)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		GorillaVelocityEstimatorManager.instance = manager;
		GorillaVelocityEstimatorManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0003AC8E File Offset: 0x00038E8E
	public static void Register(GorillaVelocityEstimator velEstimator)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!GorillaVelocityEstimatorManager.hasInstance)
		{
			GorillaVelocityEstimatorManager.CreateManager();
		}
		if (!GorillaVelocityEstimatorManager.estimators.Contains(velEstimator))
		{
			GorillaVelocityEstimatorManager.estimators.Add(velEstimator);
		}
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x0003ACBC File Offset: 0x00038EBC
	public static void Unregister(GorillaVelocityEstimator velEstimator)
	{
		if (GTAppState.isQuitting)
		{
			return;
		}
		if (!GorillaVelocityEstimatorManager.hasInstance)
		{
			GorillaVelocityEstimatorManager.CreateManager();
		}
		if (GorillaVelocityEstimatorManager.estimators.Contains(velEstimator))
		{
			GorillaVelocityEstimatorManager.estimators.Remove(velEstimator);
		}
	}

	// Token: 0x04000D4C RID: 3404
	public static GorillaVelocityEstimatorManager instance;

	// Token: 0x04000D4D RID: 3405
	public static bool hasInstance = false;

	// Token: 0x04000D4E RID: 3406
	public static readonly List<GorillaVelocityEstimator> estimators = new List<GorillaVelocityEstimator>(1024);
}
