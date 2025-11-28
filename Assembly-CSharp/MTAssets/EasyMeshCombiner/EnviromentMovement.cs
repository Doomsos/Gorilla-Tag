using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x02000F5A RID: 3930
	public class EnviromentMovement : MonoBehaviour
	{
		// Token: 0x0600627F RID: 25215 RVA: 0x001FB46F File Offset: 0x001F966F
		private void Start()
		{
			this.thisTransform = base.gameObject.GetComponent<Transform>();
			this.nextPosition = this.pos1;
		}

		// Token: 0x06006280 RID: 25216 RVA: 0x001FB490 File Offset: 0x001F9690
		private void Update()
		{
			if (Vector3.Distance(this.thisTransform.position, this.nextPosition) > 0.5f)
			{
				base.transform.position = Vector3.Lerp(this.thisTransform.position, this.nextPosition, 2f * Time.deltaTime);
				return;
			}
			if (this.nextPosition == this.pos1)
			{
				this.nextPosition = this.pos2;
				return;
			}
			if (this.nextPosition == this.pos2)
			{
				this.nextPosition = this.pos1;
				return;
			}
		}

		// Token: 0x0400710A RID: 28938
		private Vector3 nextPosition = Vector3.zero;

		// Token: 0x0400710B RID: 28939
		private Transform thisTransform;

		// Token: 0x0400710C RID: 28940
		public Vector3 pos1;

		// Token: 0x0400710D RID: 28941
		public Vector3 pos2;
	}
}
