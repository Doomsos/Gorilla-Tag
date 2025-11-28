using System;
using UnityEngine;

// Token: 0x020004F0 RID: 1264
public class KinematicWhenTargetInactive : MonoBehaviour
{
	// Token: 0x0600207F RID: 8319 RVA: 0x000AC834 File Offset: 0x000AAA34
	private void LateUpdate()
	{
		if (!this.target.activeSelf)
		{
			foreach (Rigidbody rigidbody in this.rigidBodies)
			{
				if (!rigidbody.isKinematic)
				{
					rigidbody.isKinematic = true;
				}
			}
			return;
		}
		foreach (Rigidbody rigidbody2 in this.rigidBodies)
		{
			if (rigidbody2.isKinematic)
			{
				rigidbody2.isKinematic = false;
			}
		}
	}

	// Token: 0x04002B13 RID: 11027
	public Rigidbody[] rigidBodies;

	// Token: 0x04002B14 RID: 11028
	public GameObject target;
}
