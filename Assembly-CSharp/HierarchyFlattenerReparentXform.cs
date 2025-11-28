using System;
using UnityEngine;

// Token: 0x020002D5 RID: 725
[DefaultExecutionOrder(-1000)]
public class HierarchyFlattenerReparentXform : MonoBehaviour
{
	// Token: 0x060011DD RID: 4573 RVA: 0x0005E316 File Offset: 0x0005C516
	protected void Awake()
	{
		if (base.enabled)
		{
			this._DoIt();
		}
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x0005E326 File Offset: 0x0005C526
	protected void OnEnable()
	{
		this._DoIt();
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x0005E32E File Offset: 0x0005C52E
	private void _DoIt()
	{
		if (this._didIt)
		{
			return;
		}
		if (this.newParent != null)
		{
			base.transform.SetParent(this.newParent, true);
		}
		Object.Destroy(this);
	}

	// Token: 0x04001659 RID: 5721
	public Transform newParent;

	// Token: 0x0400165A RID: 5722
	private bool _didIt;
}
