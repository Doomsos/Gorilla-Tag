using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000DAE RID: 3502
	public class BuilderAttachEdge : MonoBehaviour
	{
		// Token: 0x06005620 RID: 22048 RVA: 0x001B0FA2 File Offset: 0x001AF1A2
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x06005621 RID: 22049 RVA: 0x001B0FC0 File Offset: 0x001AF1C0
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Transform transform = this.center;
			if (transform == null)
			{
				transform = base.transform;
			}
			Vector3 vector = transform.rotation * Vector3.right;
			Gizmos.DrawLine(transform.position - vector * this.length * 0.5f, transform.position + vector * this.length * 0.5f);
		}

		// Token: 0x0400633D RID: 25405
		public Transform center;

		// Token: 0x0400633E RID: 25406
		public float length;
	}
}
