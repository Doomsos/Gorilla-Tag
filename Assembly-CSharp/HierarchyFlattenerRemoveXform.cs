using System;
using UnityEngine;

// Token: 0x020002D4 RID: 724
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerRemoveXform : MonoBehaviour
{
	// Token: 0x060011DA RID: 4570 RVA: 0x0005E292 File Offset: 0x0005C492
	protected void Awake()
	{
		this._DoIt();
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x0005E29C File Offset: 0x0005C49C
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (base.GetComponentInChildren<HierarchyFlattenerRemoveXform>(true) != null)
		{
			return;
		}
		HierarchyFlattenerRemoveXform componentInParent = base.GetComponentInParent<HierarchyFlattenerRemoveXform>(true);
		this._didIt = true;
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).SetParent(transform.parent, true);
		}
		Object.Destroy(base.gameObject);
		if (componentInParent != null)
		{
			componentInParent._DoIt();
		}
	}

	// Token: 0x04001658 RID: 5720
	private bool _didIt;
}
