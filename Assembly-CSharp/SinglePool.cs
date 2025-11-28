using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C5F RID: 3167
[Serializable]
public class SinglePool
{
	// Token: 0x06004D84 RID: 19844 RVA: 0x00191DDC File Offset: 0x0018FFDC
	private void PrivAllocPooledObjects()
	{
		int count = this.inactivePool.Count;
		for (int i = count; i < count + this.initAmountToPool; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.objectToPool, this.gameObject.transform, true);
			gameObject.name = this.objectToPool.name + "(PoolIndex=" + i.ToString() + ")";
			gameObject.SetActive(false);
			this.inactivePool.Push(gameObject);
			this.amountAllocatedToPool++;
			int instanceID = gameObject.GetInstanceID();
			this.pooledObjects.Add(instanceID);
		}
	}

	// Token: 0x06004D85 RID: 19845 RVA: 0x00191E7F File Offset: 0x0019007F
	public void Initialize(GameObject gameObject_)
	{
		this.gameObject = gameObject_;
		this.activePool = new Dictionary<int, GameObject>(this.initAmountToPool);
		this.inactivePool = new Stack<GameObject>(this.initAmountToPool);
		this.pooledObjects = new HashSet<int>();
		this.PrivAllocPooledObjects();
	}

	// Token: 0x06004D86 RID: 19846 RVA: 0x00191EBC File Offset: 0x001900BC
	public GameObject Instantiate(bool setActive = true)
	{
		if (this.inactivePool.Count == 0)
		{
			Debug.LogWarning("Pool '" + this.objectToPool.name + "'is expanding consider changing initial pool size");
			this.PrivAllocPooledObjects();
		}
		GameObject gameObject = this.inactivePool.Pop();
		int instanceID = gameObject.GetInstanceID();
		gameObject.SetActive(setActive);
		this.activePool.Add(instanceID, gameObject);
		return gameObject;
	}

	// Token: 0x06004D87 RID: 19847 RVA: 0x00191F24 File Offset: 0x00190124
	public void Destroy(GameObject obj)
	{
		int instanceID = obj.GetInstanceID();
		if (!this.activePool.ContainsKey(instanceID))
		{
			Debug.Log("Failed to destroy Object " + obj.name + " in pool, It is not contained in the activePool");
			return;
		}
		if (!this.pooledObjects.Contains(instanceID))
		{
			Debug.Log("Failed to destroy Object " + obj.name + " in pool, It is not contained in the pooledObjects");
			return;
		}
		obj.SetActive(false);
		this.inactivePool.Push(obj);
		this.activePool.Remove(instanceID);
	}

	// Token: 0x06004D88 RID: 19848 RVA: 0x00191FAA File Offset: 0x001901AA
	public int PoolGUID()
	{
		return PoolUtils.GameObjHashCode(this.objectToPool);
	}

	// Token: 0x06004D89 RID: 19849 RVA: 0x00191FB7 File Offset: 0x001901B7
	public int GetTotalCount()
	{
		return this.pooledObjects.Count;
	}

	// Token: 0x06004D8A RID: 19850 RVA: 0x00191FC4 File Offset: 0x001901C4
	public int GetActiveCount()
	{
		return this.activePool.Count;
	}

	// Token: 0x06004D8B RID: 19851 RVA: 0x00191FD1 File Offset: 0x001901D1
	public int GetInactiveCount()
	{
		return this.inactivePool.Count;
	}

	// Token: 0x04005CE6 RID: 23782
	public GameObject objectToPool;

	// Token: 0x04005CE7 RID: 23783
	public int initAmountToPool = 8;

	// Token: 0x04005CE8 RID: 23784
	private HashSet<int> pooledObjects;

	// Token: 0x04005CE9 RID: 23785
	private Stack<GameObject> inactivePool;

	// Token: 0x04005CEA RID: 23786
	private Dictionary<int, GameObject> activePool;

	// Token: 0x04005CEB RID: 23787
	private GameObject gameObject;

	// Token: 0x04005CEC RID: 23788
	private int amountAllocatedToPool;
}
