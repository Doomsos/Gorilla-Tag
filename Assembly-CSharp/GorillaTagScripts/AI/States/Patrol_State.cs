using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000E9A RID: 3738
	public class Patrol_State : IState
	{
		// Token: 0x06005D75 RID: 23925 RVA: 0x001E0057 File Offset: 0x001DE257
		public Patrol_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x06005D76 RID: 23926 RVA: 0x001E0078 File Offset: 0x001DE278
		public void Tick()
		{
			if (this.agent.remainingDistance <= this.agent.stoppingDistance)
			{
				Vector3 position = this.entity.waypoints[Random.Range(0, this.entity.waypoints.Count - 1)].transform.position;
				this.agent.SetDestination(position);
			}
		}

		// Token: 0x06005D77 RID: 23927 RVA: 0x001E00E0 File Offset: 0x001DE2E0
		public void OnEnter()
		{
			string text = "Current State: ";
			Type typeFromHandle = typeof(Patrol_State);
			Debug.Log(text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
			if (this.entity.waypoints.Count > 0)
			{
				this.agent.SetDestination(this.entity.waypoints[0].transform.position);
			}
		}

		// Token: 0x06005D78 RID: 23928 RVA: 0x00002789 File Offset: 0x00000989
		public void OnExit()
		{
		}

		// Token: 0x04006B52 RID: 27474
		private AIEntity entity;

		// Token: 0x04006B53 RID: 27475
		private NavMeshAgent agent;
	}
}
