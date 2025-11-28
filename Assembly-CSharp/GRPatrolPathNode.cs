using System;
using UnityEngine;

// Token: 0x020006DB RID: 1755
public class GRPatrolPathNode : MonoBehaviour
{
	// Token: 0x06002CE8 RID: 11496 RVA: 0x000F3438 File Offset: 0x000F1638
	public void OnDrawGizmosSelected()
	{
		if (base.transform.parent == null)
		{
			return;
		}
		GRPatrolPath component = base.transform.parent.GetComponent<GRPatrolPath>();
		if (component == null)
		{
			return;
		}
		component.OnDrawGizmosSelected();
	}
}
