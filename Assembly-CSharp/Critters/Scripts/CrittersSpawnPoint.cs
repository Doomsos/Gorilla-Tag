using System;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x0200114F RID: 4431
	public class CrittersSpawnPoint : MonoBehaviour
	{
		// Token: 0x06006FD3 RID: 28627 RVA: 0x00246788 File Offset: 0x00244988
		private void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(base.transform.position, 0.1f);
		}
	}
}
