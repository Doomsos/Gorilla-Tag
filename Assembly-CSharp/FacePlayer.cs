using System;
using UnityEngine;

// Token: 0x02000509 RID: 1289
public class FacePlayer : MonoBehaviour
{
	// Token: 0x060020F3 RID: 8435 RVA: 0x000AE6C8 File Offset: 0x000AC8C8
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - GorillaTagger.Instance.headCollider.transform.position) * Quaternion.AngleAxis(-90f, Vector3.up);
	}
}
