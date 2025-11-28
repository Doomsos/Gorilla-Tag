using System;
using UnityEngine;
using UnityEngine.AI;

namespace GorillaTagScripts.AI.States
{
	// Token: 0x02000E98 RID: 3736
	public class Chase_State : IState
	{
		// Token: 0x170008A7 RID: 2215
		// (get) Token: 0x06005D6B RID: 23915 RVA: 0x001DFF10 File Offset: 0x001DE110
		// (set) Token: 0x06005D6C RID: 23916 RVA: 0x001DFF18 File Offset: 0x001DE118
		public Transform FollowTarget { get; set; }

		// Token: 0x06005D6D RID: 23917 RVA: 0x001DFF21 File Offset: 0x001DE121
		public Chase_State(AIEntity entity)
		{
			this.entity = entity;
			this.agent = this.entity.navMeshAgent;
		}

		// Token: 0x06005D6E RID: 23918 RVA: 0x001DFF41 File Offset: 0x001DE141
		public void Tick()
		{
			this.agent.SetDestination(this.FollowTarget.position);
			if (this.agent.remainingDistance < this.entity.attackDistance)
			{
				this.chaseOver = true;
			}
		}

		// Token: 0x06005D6F RID: 23919 RVA: 0x001DFF79 File Offset: 0x001DE179
		public void OnEnter()
		{
			this.chaseOver = false;
			string text = "Current State: ";
			Type typeFromHandle = typeof(Chase_State);
			Debug.Log(text + ((typeFromHandle != null) ? typeFromHandle.ToString() : null));
		}

		// Token: 0x06005D70 RID: 23920 RVA: 0x001DFFA7 File Offset: 0x001DE1A7
		public void OnExit()
		{
			this.chaseOver = true;
		}

		// Token: 0x04006B4C RID: 27468
		private AIEntity entity;

		// Token: 0x04006B4D RID: 27469
		private NavMeshAgent agent;

		// Token: 0x04006B4F RID: 27471
		public bool chaseOver;
	}
}
