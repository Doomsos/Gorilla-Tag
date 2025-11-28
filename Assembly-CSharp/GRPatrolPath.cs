using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006DA RID: 1754
public class GRPatrolPath : MonoBehaviour
{
	// Token: 0x06002CE5 RID: 11493 RVA: 0x000F32DC File Offset: 0x000F14DC
	private void Awake()
	{
		this.patrolNodes = new List<Transform>(base.transform.childCount);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			this.patrolNodes.Add(base.transform.GetChild(i));
		}
	}

	// Token: 0x06002CE6 RID: 11494 RVA: 0x000F332C File Offset: 0x000F152C
	public void OnDrawGizmosSelected()
	{
		if (this.patrolNodes == null || base.transform.childCount != this.patrolNodes.Count)
		{
			this.patrolNodes = new List<Transform>(base.transform.childCount);
			for (int i = 0; i < base.transform.childCount; i++)
			{
				this.patrolNodes.Add(base.transform.GetChild(i));
			}
		}
		if (this.patrolNodes != null)
		{
			for (int j = 0; j < this.patrolNodes.Count; j++)
			{
				Gizmos.color = Color.magenta;
				Gizmos.DrawCube(this.patrolNodes[j].transform.position, Vector3.one * 0.5f);
				if (j < this.patrolNodes.Count - 1)
				{
					Gizmos.DrawLine(this.patrolNodes[j].transform.position, this.patrolNodes[j + 1].transform.position);
				}
			}
		}
	}

	// Token: 0x04003A48 RID: 14920
	[NonSerialized]
	public List<Transform> patrolNodes;

	// Token: 0x04003A49 RID: 14921
	public int index;
}
