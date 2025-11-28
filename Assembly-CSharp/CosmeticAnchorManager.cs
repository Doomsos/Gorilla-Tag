using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000476 RID: 1142
public class CosmeticAnchorManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001D01 RID: 7425 RVA: 0x0009957D File Offset: 0x0009777D
	protected void Awake()
	{
		if (CosmeticAnchorManager.hasInstance && CosmeticAnchorManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		CosmeticAnchorManager.SetInstance(this);
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x000995A0 File Offset: 0x000977A0
	public static void CreateManager()
	{
		CosmeticAnchorManager.SetInstance(new GameObject("CosmeticAnchorManager").AddComponent<CosmeticAnchorManager>());
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x000995B6 File Offset: 0x000977B6
	private static void SetInstance(CosmeticAnchorManager manager)
	{
		CosmeticAnchorManager.instance = manager;
		CosmeticAnchorManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x06001D04 RID: 7428 RVA: 0x000995D1 File Offset: 0x000977D1
	public static void RegisterCosmeticAnchor(CosmeticAnchors cA)
	{
		if (!CosmeticAnchorManager.hasInstance)
		{
			CosmeticAnchorManager.CreateManager();
		}
		if ((cA.AffectedByHunt() || cA.AffectedByBuilder()) && !CosmeticAnchorManager.allAnchors.Contains(cA))
		{
			CosmeticAnchorManager.allAnchors.Add(cA);
		}
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x00099607 File Offset: 0x00097807
	public static void UnregisterCosmeticAnchor(CosmeticAnchors cA)
	{
		if (!CosmeticAnchorManager.hasInstance)
		{
			CosmeticAnchorManager.CreateManager();
		}
		if ((cA.AffectedByHunt() || cA.AffectedByBuilder()) && CosmeticAnchorManager.allAnchors.Contains(cA))
		{
			CosmeticAnchorManager.allAnchors.Remove(cA);
		}
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001D07 RID: 7431 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06001D08 RID: 7432 RVA: 0x00099640 File Offset: 0x00097840
	public void SliceUpdate()
	{
		for (int i = 0; i < CosmeticAnchorManager.allAnchors.Count; i++)
		{
			CosmeticAnchorManager.allAnchors[i].TryUpdate();
		}
	}

	// Token: 0x040026F2 RID: 9970
	public static CosmeticAnchorManager instance;

	// Token: 0x040026F3 RID: 9971
	public static bool hasInstance = false;

	// Token: 0x040026F4 RID: 9972
	public static List<CosmeticAnchors> allAnchors = new List<CosmeticAnchors>();
}
