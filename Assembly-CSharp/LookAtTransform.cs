using System;
using UnityEngine;

// Token: 0x02000C56 RID: 3158
public class LookAtTransform : MonoBehaviour
{
	// Token: 0x06004D45 RID: 19781 RVA: 0x001908D7 File Offset: 0x0018EAD7
	private void Update()
	{
		base.transform.rotation = Quaternion.LookRotation(this.lookAt.position - base.transform.position);
	}

	// Token: 0x04005CD9 RID: 23769
	[SerializeField]
	private Transform lookAt;
}
