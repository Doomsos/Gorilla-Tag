using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006C RID: 108
public class CrittersPool : MonoBehaviour
{
	// Token: 0x060002A2 RID: 674 RVA: 0x000107D4 File Offset: 0x0000E9D4
	public static GameObject GetPooled(GameObject prefab)
	{
		CrittersPool crittersPool = CrittersPool.instance;
		if (crittersPool == null)
		{
			return null;
		}
		return crittersPool.GetInstance(prefab);
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x000107E7 File Offset: 0x0000E9E7
	public static void Return(GameObject pooledGO)
	{
		CrittersPool crittersPool = CrittersPool.instance;
		if (crittersPool == null)
		{
			return;
		}
		crittersPool.ReturnInstance(pooledGO);
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x000107F9 File Offset: 0x0000E9F9
	private void Awake()
	{
		if (CrittersPool.instance != null)
		{
			Object.Destroy(this);
			return;
		}
		CrittersPool.instance = this;
		this.SetupPools();
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x0001081C File Offset: 0x0000EA1C
	private void SetupPools()
	{
		this.pools = new Dictionary<GameObject, List<GameObject>>();
		this.poolParent = new GameObject("CrittersPool")
		{
			transform = 
			{
				parent = base.transform
			}
		}.transform;
		for (int i = 0; i < this.eventEffects.Length; i++)
		{
			CrittersPool.CrittersPoolSettings crittersPoolSettings = this.eventEffects[i];
			if (crittersPoolSettings.poolObject == null || crittersPoolSettings.poolSize <= 0)
			{
				GTDev.Log<string>("CrittersPool.SetupPools Failed. Pool has no poolObject or has size 0.", null);
			}
			else
			{
				List<GameObject> list = new List<GameObject>();
				for (int j = 0; j < crittersPoolSettings.poolSize; j++)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(crittersPoolSettings.poolObject);
					gameObject.transform.SetParent(this.poolParent);
					GameObject gameObject2 = gameObject;
					gameObject2.name += j.ToString();
					gameObject.SetActive(false);
					list.Add(gameObject);
				}
				this.pools.Add(crittersPoolSettings.poolObject, list);
			}
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x00010918 File Offset: 0x0000EB18
	private GameObject GetInstance(GameObject prefab)
	{
		List<GameObject> list;
		if (this.pools.TryGetValue(prefab, ref list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null && !list[i].activeSelf)
				{
					list[i].SetActive(true);
					return list[i];
				}
			}
			GTDev.Log<string>("CrittersPool.GetInstance Failed. No available instance.", null);
			return null;
		}
		GTDev.LogError<string>("CrittersPool.GetInstance Failed. Prefab doesn't have a valid pool setup.", null);
		return null;
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x00010991 File Offset: 0x0000EB91
	private void ReturnInstance(GameObject instance)
	{
		instance.transform.SetParent(this.poolParent);
		instance.SetActive(false);
	}

	// Token: 0x0400031D RID: 797
	private static CrittersPool instance;

	// Token: 0x0400031E RID: 798
	public CrittersPool.CrittersPoolSettings[] eventEffects;

	// Token: 0x0400031F RID: 799
	private Dictionary<GameObject, List<GameObject>> pools;

	// Token: 0x04000320 RID: 800
	public Transform poolParent;

	// Token: 0x0200006D RID: 109
	[Serializable]
	public class CrittersPoolSettings
	{
		// Token: 0x04000321 RID: 801
		public GameObject poolObject;

		// Token: 0x04000322 RID: 802
		public int poolSize = 20;
	}
}
