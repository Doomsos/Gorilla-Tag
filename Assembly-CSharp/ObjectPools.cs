using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C60 RID: 3168
public class ObjectPools : MonoBehaviour, IBuildValidation
{
	// Token: 0x1700073E RID: 1854
	// (get) Token: 0x06004D8D RID: 19853 RVA: 0x00191FCD File Offset: 0x001901CD
	// (set) Token: 0x06004D8E RID: 19854 RVA: 0x00191FD5 File Offset: 0x001901D5
	public bool initialized { get; private set; }

	// Token: 0x06004D8F RID: 19855 RVA: 0x00191FDE File Offset: 0x001901DE
	protected void Awake()
	{
		ObjectPools.instance = this;
	}

	// Token: 0x06004D90 RID: 19856 RVA: 0x00191FE6 File Offset: 0x001901E6
	protected void Start()
	{
		this.InitializePools();
	}

	// Token: 0x06004D91 RID: 19857 RVA: 0x00191FF0 File Offset: 0x001901F0
	public void InitializePools()
	{
		if (this.initialized)
		{
			return;
		}
		this.lookUp = new Dictionary<int, SinglePool>();
		foreach (SinglePool singlePool in this.pools)
		{
			singlePool.Initialize(base.gameObject);
			int num = singlePool.PoolGUID();
			if (this.lookUp.ContainsKey(num))
			{
				using (List<SinglePool>.Enumerator enumerator2 = this.pools.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SinglePool singlePool2 = enumerator2.Current;
						if (singlePool2.PoolGUID() == num)
						{
							Debug.LogError("Pools contain more then one instance of the same object\n" + string.Format("First object in question is {0} tag: {1}\n", singlePool2.objectToPool, singlePool2.objectToPool.tag) + string.Format("Second object is {0} tag: {1}", singlePool.objectToPool, singlePool.objectToPool.tag));
							break;
						}
					}
					continue;
				}
			}
			this.lookUp.Add(singlePool.PoolGUID(), singlePool);
		}
		this.initialized = true;
	}

	// Token: 0x06004D92 RID: 19858 RVA: 0x00192124 File Offset: 0x00190324
	public bool DoesPoolExist(GameObject obj)
	{
		return this.DoesPoolExist(PoolUtils.GameObjHashCode(obj));
	}

	// Token: 0x06004D93 RID: 19859 RVA: 0x00192132 File Offset: 0x00190332
	public bool DoesPoolExist(int hash)
	{
		return this.lookUp.ContainsKey(hash);
	}

	// Token: 0x06004D94 RID: 19860 RVA: 0x00192140 File Offset: 0x00190340
	public SinglePool GetPoolByHash(int hash)
	{
		return this.lookUp[hash];
	}

	// Token: 0x06004D95 RID: 19861 RVA: 0x00192150 File Offset: 0x00190350
	public SinglePool GetPoolByObjectType(GameObject obj)
	{
		int hash = PoolUtils.GameObjHashCode(obj);
		return this.GetPoolByHash(hash);
	}

	// Token: 0x06004D96 RID: 19862 RVA: 0x0019216B File Offset: 0x0019036B
	public GameObject Instantiate(GameObject obj, bool setActive = true)
	{
		return this.GetPoolByObjectType(obj).Instantiate(setActive);
	}

	// Token: 0x06004D97 RID: 19863 RVA: 0x0019217A File Offset: 0x0019037A
	public GameObject Instantiate(int hash, bool setActive = true)
	{
		return this.GetPoolByHash(hash).Instantiate(setActive);
	}

	// Token: 0x06004D98 RID: 19864 RVA: 0x00192189 File Offset: 0x00190389
	public GameObject Instantiate(int hash, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06004D99 RID: 19865 RVA: 0x0019219F File Offset: 0x0019039F
	public GameObject Instantiate(int hash, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(hash, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06004D9A RID: 19866 RVA: 0x001921B7 File Offset: 0x001903B7
	public GameObject Instantiate(GameObject obj, Vector3 position, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.position = position;
		return gameObject;
	}

	// Token: 0x06004D9B RID: 19867 RVA: 0x001921CD File Offset: 0x001903CD
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		return gameObject;
	}

	// Token: 0x06004D9C RID: 19868 RVA: 0x001921E5 File Offset: 0x001903E5
	public GameObject Instantiate(GameObject obj, Vector3 position, Quaternion rotation, float scale, bool setActive = true)
	{
		GameObject gameObject = this.Instantiate(obj, setActive);
		gameObject.transform.SetPositionAndRotation(position, rotation);
		gameObject.transform.localScale = Vector3.one * scale;
		return gameObject;
	}

	// Token: 0x06004D9D RID: 19869 RVA: 0x00192214 File Offset: 0x00190414
	public void Destroy(GameObject obj)
	{
		this.GetPoolByObjectType(obj).Destroy(obj);
	}

	// Token: 0x06004D9E RID: 19870 RVA: 0x00192224 File Offset: 0x00190424
	public bool BuildValidationCheck()
	{
		using (List<SinglePool>.Enumerator enumerator = this.pools.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.objectToPool == null)
				{
					Debug.Log("GlobalObjectPools contains a nullref. Failing build validation.");
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x04005CED RID: 23789
	public static ObjectPools instance;

	// Token: 0x04005CEF RID: 23791
	[SerializeField]
	private List<SinglePool> pools;

	// Token: 0x04005CF0 RID: 23792
	private Dictionary<int, SinglePool> lookUp;
}
