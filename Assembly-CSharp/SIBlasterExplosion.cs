using System;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class SIBlasterExplosion : MonoBehaviour
{
	// Token: 0x0600049E RID: 1182 RVA: 0x0001A892 File Offset: 0x00018A92
	private void OnDisable()
	{
		SIGadgetBlasterProjectile.DespawnExplosion(base.gameObject);
	}
}
