using System;
using UnityEngine;

// Token: 0x02000672 RID: 1650
[Serializable]
public class GRAbilityAttackLatchOn : GRAbilityBase
{
	// Token: 0x06002A30 RID: 10800 RVA: 0x000E38FB File Offset: 0x000E1AFB
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002A31 RID: 10801 RVA: 0x000E3930 File Offset: 0x000E1B30
	public override void Start()
	{
		base.Start();
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.navAgent.speed = this.tellMoveSpeed;
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002A32 RID: 10802 RVA: 0x000E3995 File Offset: 0x000E1B95
	public override void Stop()
	{
		this.agent.transform.SetParent(null);
		this.agent.SetIsPathing(true, true);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002A33 RID: 10803 RVA: 0x000E39CF File Offset: 0x000E1BCF
	public override bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	// Token: 0x06002A34 RID: 10804 RVA: 0x000E39E9 File Offset: 0x000E1BE9
	public override void Update(float dt)
	{
		this.UpdateNavSpeed();
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002A35 RID: 10805 RVA: 0x000E3A13 File Offset: 0x000E1C13
	public override void UpdateRemote(float dt)
	{
		this.UpdateNavSpeed();
	}

	// Token: 0x06002A36 RID: 10806 RVA: 0x000E3A1C File Offset: 0x000E1C1C
	private void UpdateNavSpeed()
	{
		if (Time.timeAsDouble - this.startTime > (double)this.tellDuration)
		{
			this.agent.navAgent.velocity = this.agent.navAgent.velocity.normalized * this.attackMoveSpeed;
			this.agent.navAgent.speed = this.attackMoveSpeed;
			if (this.damageTrigger != null)
			{
				this.damageTrigger.SetActive(true);
			}
		}
	}

	// Token: 0x06002A37 RID: 10807 RVA: 0x000E3AA4 File Offset: 0x000E1CA4
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
				this.agent.transform.SetParent(grplayer.attachEnemy);
				this.agent.transform.localPosition = Vector3.zero;
				this.agent.transform.localRotation = Quaternion.identity;
				this.agent.SetIsPathing(false, true);
			}
		}
	}

	// Token: 0x04003672 RID: 13938
	public float duration;

	// Token: 0x04003673 RID: 13939
	public float attackMoveSpeed;

	// Token: 0x04003674 RID: 13940
	public float tellDuration;

	// Token: 0x04003675 RID: 13941
	public float tellMoveSpeed;

	// Token: 0x04003676 RID: 13942
	public string animName;

	// Token: 0x04003677 RID: 13943
	public float animSpeed;

	// Token: 0x04003678 RID: 13944
	public float maxTurnSpeed;

	// Token: 0x04003679 RID: 13945
	public Transform target;

	// Token: 0x0400367A RID: 13946
	public GameObject damageTrigger;
}
