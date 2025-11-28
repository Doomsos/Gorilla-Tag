using System;
using UnityEngine;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000E99 RID: 3737
	public class CircularPatrol_State : IState
	{
		// Token: 0x06005D71 RID: 23921 RVA: 0x001DFFB0 File Offset: 0x001DE1B0
		public CircularPatrol_State(AIEntity entity)
		{
			this.entity = entity;
		}

		// Token: 0x06005D72 RID: 23922 RVA: 0x001DFFC0 File Offset: 0x001DE1C0
		public void Tick()
		{
			Vector3 position = this.entity.circleCenter.position;
			float num = position.x + Mathf.Cos(this.angle) * this.entity.angularSpeed;
			float y = position.y;
			float num2 = position.z + Mathf.Sin(this.angle) * this.entity.angularSpeed;
			this.entity.transform.position = new Vector3(num, y, num2);
			this.angle += this.entity.angularSpeed * Time.deltaTime;
		}

		// Token: 0x06005D73 RID: 23923 RVA: 0x00002789 File Offset: 0x00000989
		public void OnEnter()
		{
		}

		// Token: 0x06005D74 RID: 23924 RVA: 0x00002789 File Offset: 0x00000989
		public void OnExit()
		{
		}

		// Token: 0x04006B50 RID: 27472
		private AIEntity entity;

		// Token: 0x04006B51 RID: 27473
		private float angle;
	}
}
