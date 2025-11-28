using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000E36 RID: 3638
	public class ObstacleEndLineTrigger : MonoBehaviour
	{
		// Token: 0x14000098 RID: 152
		// (add) Token: 0x06005ACF RID: 23247 RVA: 0x001D1718 File Offset: 0x001CF918
		// (remove) Token: 0x06005AD0 RID: 23248 RVA: 0x001D1750 File Offset: 0x001CF950
		public event ObstacleEndLineTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x06005AD1 RID: 23249 RVA: 0x001D1788 File Offset: 0x001CF988
		private void OnTriggerEnter(Collider other)
		{
			VRRig vrrig;
			if (other.attachedRigidbody.gameObject.TryGetComponent<VRRig>(ref vrrig))
			{
				ObstacleEndLineTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(vrrig);
			}
		}

		// Token: 0x02000E37 RID: 3639
		// (Invoke) Token: 0x06005AD4 RID: 23252
		public delegate void ObstacleCourseTriggerEvent(VRRig vrrig);
	}
}
