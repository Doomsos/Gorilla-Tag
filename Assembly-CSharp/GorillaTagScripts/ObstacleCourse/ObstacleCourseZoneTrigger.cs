using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000E34 RID: 3636
	public class ObstacleCourseZoneTrigger : MonoBehaviour
	{
		// Token: 0x14000096 RID: 150
		// (add) Token: 0x06005AC4 RID: 23236 RVA: 0x001D15C8 File Offset: 0x001CF7C8
		// (remove) Token: 0x06005AC5 RID: 23237 RVA: 0x001D1600 File Offset: 0x001CF800
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerEnter;

		// Token: 0x14000097 RID: 151
		// (add) Token: 0x06005AC6 RID: 23238 RVA: 0x001D1638 File Offset: 0x001CF838
		// (remove) Token: 0x06005AC7 RID: 23239 RVA: 0x001D1670 File Offset: 0x001CF870
		public event ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent OnPlayerTriggerExit;

		// Token: 0x06005AC8 RID: 23240 RVA: 0x001D16A5 File Offset: 0x001CF8A5
		private void OnTriggerEnter(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerEnter = this.OnPlayerTriggerEnter;
				if (onPlayerTriggerEnter == null)
				{
					return;
				}
				onPlayerTriggerEnter(other);
			}
		}

		// Token: 0x06005AC9 RID: 23241 RVA: 0x001D16DD File Offset: 0x001CF8DD
		private void OnTriggerExit(Collider other)
		{
			if (!other.GetComponent<SphereCollider>())
			{
				return;
			}
			if (other.attachedRigidbody.gameObject.CompareTag("GorillaPlayer"))
			{
				ObstacleCourseZoneTrigger.ObstacleCourseTriggerEvent onPlayerTriggerExit = this.OnPlayerTriggerExit;
				if (onPlayerTriggerExit == null)
				{
					return;
				}
				onPlayerTriggerExit(other);
			}
		}

		// Token: 0x040067FA RID: 26618
		public LayerMask bodyLayer;

		// Token: 0x02000E35 RID: 3637
		// (Invoke) Token: 0x06005ACC RID: 23244
		public delegate void ObstacleCourseTriggerEvent(Collider collider);
	}
}
