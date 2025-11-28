using System;
using System.Runtime.CompilerServices;
using GorillaTagScripts.AI.States;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.AI.Entities
{
	// Token: 0x02000E9B RID: 3739
	public class TestShark : AIEntity
	{
		// Token: 0x06005D79 RID: 23929 RVA: 0x001E016C File Offset: 0x001DE36C
		private new void Awake()
		{
			base.Awake();
			this.chasingTimer = 0f;
			this._stateMachine = new StateMachine();
			this.circularPatrol = new CircularPatrol_State(this);
			this.patrol = new Patrol_State(this);
			this.chase = new Chase_State(this);
			this._stateMachine.AddTransition(this.patrol, this.chase, this.<Awake>g__ShouldChase|7_0());
			this._stateMachine.AddTransition(this.chase, this.patrol, this.<Awake>g__ShouldPatrol|7_1());
			this._stateMachine.SetState(this.patrol);
		}

		// Token: 0x06005D7A RID: 23930 RVA: 0x001E0204 File Offset: 0x001DE404
		private void Update()
		{
			this._stateMachine.Tick();
			this.shouldChase = false;
			this.chasingTimer += Time.deltaTime;
			if (this.chasingTimer >= this.nextTimeToChasePlayer)
			{
				base.ChooseClosestTarget();
				if (this.followTarget != null)
				{
					this.chase.FollowTarget = this.followTarget;
					this.shouldChase = true;
				}
				this.chasingTimer = 0f;
			}
		}

		// Token: 0x06005D7C RID: 23932 RVA: 0x001E028D File Offset: 0x001DE48D
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldChase|7_0()
		{
			return () => this.shouldChase && PhotonNetwork.InRoom;
		}

		// Token: 0x06005D7E RID: 23934 RVA: 0x001E02AC File Offset: 0x001DE4AC
		[CompilerGenerated]
		private Func<bool> <Awake>g__ShouldPatrol|7_1()
		{
			return () => this.chase.chaseOver;
		}

		// Token: 0x04006B54 RID: 27476
		public float nextTimeToChasePlayer = 30f;

		// Token: 0x04006B55 RID: 27477
		private float chasingTimer;

		// Token: 0x04006B56 RID: 27478
		private bool shouldChase;

		// Token: 0x04006B57 RID: 27479
		private StateMachine _stateMachine;

		// Token: 0x04006B58 RID: 27480
		private CircularPatrol_State circularPatrol;

		// Token: 0x04006B59 RID: 27481
		private Patrol_State patrol;

		// Token: 0x04006B5A RID: 27482
		private Chase_State chase;
	}
}
