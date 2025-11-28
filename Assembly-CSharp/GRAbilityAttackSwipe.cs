using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000670 RID: 1648
[Serializable]
public class GRAbilityAttackSwipe : GRAbilityBase
{
	// Token: 0x06002A28 RID: 10792 RVA: 0x000E3568 File Offset: 0x000E1768
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002A29 RID: 10793 RVA: 0x000E359C File Offset: 0x000E179C
	public override void Start()
	{
		base.Start();
		if (this.animData.Count > 0)
		{
			this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
			this.duration = this.animData[this.lastAnimIndex].duration;
			this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animData[this.lastAnimIndex].speed);
			this.animNameString = this.animData[this.lastAnimIndex].animName;
		}
		else
		{
			this.duration = 0.5f;
		}
		this.soundAttack.soundSelectMode = AbilitySound.SoundSelectMode.Random;
		this.soundAttack.Play(null);
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.state = GRAbilityAttackSwipe.State.Tell;
	}

	// Token: 0x06002A2A RID: 10794 RVA: 0x000E36AB File Offset: 0x000E18AB
	public override void Stop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002A2B RID: 10795 RVA: 0x000E36E0 File Offset: 0x000E18E0
	public override bool IsDone()
	{
		return this.state == GRAbilityAttackSwipe.State.Done;
	}

	// Token: 0x06002A2C RID: 10796 RVA: 0x000E36EC File Offset: 0x000E18EC
	protected override void UpdateShared(float dt)
	{
		float num = (float)(Time.timeAsDouble - this.startTime);
		switch (this.state)
		{
		case GRAbilityAttackSwipe.State.Tell:
			this.targetPos = this.root.position + this.root.transform.forward;
			if (this.target != null)
			{
				this.targetPos = this.target.position;
			}
			GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
			if (num > this.tellDuration)
			{
				this.state = GRAbilityAttackSwipe.State.Attack;
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				this.initialPos = this.root.position;
				this.initialVel = (this.targetPos - this.initialPos).normalized * this.attackMoveSpeed;
				return;
			}
			break;
		case GRAbilityAttackSwipe.State.Attack:
		{
			float num2 = num - this.tellDuration;
			Vector3 vector = this.initialPos + this.initialVel * num2;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector, ref navMeshHit, 0.5f, this.walkableArea))
			{
				vector = navMeshHit.position;
				if (NavMesh.Raycast(this.initialPos, vector, ref navMeshHit, this.walkableArea))
				{
					vector = navMeshHit.position;
				}
				this.root.position = vector;
			}
			if (num > this.tellDuration + this.attackDuration)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				this.state = GRAbilityAttackSwipe.State.FollowThrough;
				return;
			}
			break;
		}
		case GRAbilityAttackSwipe.State.FollowThrough:
			if (num >= this.duration)
			{
				this.state = GRAbilityAttackSwipe.State.Done;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002A2D RID: 10797 RVA: 0x000E38A4 File Offset: 0x000E1AA4
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
			}
		}
	}

	// Token: 0x06002A2E RID: 10798 RVA: 0x000E38E4 File Offset: 0x000E1AE4
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x0400365E RID: 13918
	public float duration;

	// Token: 0x0400365F RID: 13919
	public float tellDuration;

	// Token: 0x04003660 RID: 13920
	public float attackDuration;

	// Token: 0x04003661 RID: 13921
	public float attackMoveSpeed;

	// Token: 0x04003662 RID: 13922
	public List<AnimationData> animData;

	// Token: 0x04003663 RID: 13923
	public AbilitySound soundAttack;

	// Token: 0x04003664 RID: 13924
	private GRAbilityAttackSwipe.State state;

	// Token: 0x04003665 RID: 13925
	public float maxTurnSpeed;

	// Token: 0x04003666 RID: 13926
	public GameObject damageTrigger;

	// Token: 0x04003667 RID: 13927
	private Transform target;

	// Token: 0x04003668 RID: 13928
	private string animNameString;

	// Token: 0x04003669 RID: 13929
	private int lastAnimIndex = -1;

	// Token: 0x0400366A RID: 13930
	public Vector3 targetPos;

	// Token: 0x0400366B RID: 13931
	public Vector3 initialPos;

	// Token: 0x0400366C RID: 13932
	public Vector3 initialVel;

	// Token: 0x02000671 RID: 1649
	private enum State
	{
		// Token: 0x0400366E RID: 13934
		Tell,
		// Token: 0x0400366F RID: 13935
		Attack,
		// Token: 0x04003670 RID: 13936
		FollowThrough,
		// Token: 0x04003671 RID: 13937
		Done
	}
}
