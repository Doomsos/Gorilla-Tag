using System;
using UnityEngine;

// Token: 0x02000270 RID: 624
public class IgnoreLocalRotation : MonoBehaviour
{
	// Token: 0x06000FFE RID: 4094 RVA: 0x0005442F File Offset: 0x0005262F
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.identity;
	}
}
