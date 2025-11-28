using System;
using UnityEngine;

// Token: 0x02000CA1 RID: 3233
public class SpawnManager : MonoBehaviour
{
	// Token: 0x06004EF3 RID: 20211 RVA: 0x001984C6 File Offset: 0x001966C6
	public Transform[] ChildrenXfs()
	{
		return base.transform.GetComponentsInChildren<Transform>();
	}
}
