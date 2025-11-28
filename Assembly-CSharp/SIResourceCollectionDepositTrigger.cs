using System;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class SIResourceCollectionDepositTrigger : MonoBehaviour
{
	// Token: 0x06000849 RID: 2121 RVA: 0x0002D02E File Offset: 0x0002B22E
	private void Awake()
	{
		this.resourceDeposit = this.parentCollection.GetComponent<ISIResourceDeposit>();
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0002D044 File Offset: 0x0002B244
	private void OnTriggerEnter(Collider other)
	{
		SIResource componentInParent = other.GetComponentInParent<SIResource>();
		if (componentInParent == null)
		{
			return;
		}
		if (componentInParent.CanDeposit())
		{
			this.resourceDeposit.ResourceDeposited(componentInParent);
		}
	}

	// Token: 0x04000A27 RID: 2599
	public GameObject parentCollection;

	// Token: 0x04000A28 RID: 2600
	private ISIResourceDeposit resourceDeposit;
}
