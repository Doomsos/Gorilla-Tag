using System;
using UnityEngine;

// Token: 0x02000673 RID: 1651
[Serializable]
public class GRAbilityAttackJump : GRAbilityBase
{
	// Token: 0x06002A39 RID: 10809 RVA: 0x000E3B31 File Offset: 0x000E1D31
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002A3A RID: 10810 RVA: 0x000E3B64 File Offset: 0x000E1D64
	public override void Start()
	{
		base.Start();
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.startTime = Time.timeAsDouble;
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.state = GRAbilityAttackJump.State.Tell;
	}

	// Token: 0x06002A3B RID: 10811 RVA: 0x000E3BD3 File Offset: 0x000E1DD3
	public override void Stop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
		if (this.damageTrigger != null)
		{
			this.damageTrigger.SetActive(false);
		}
	}

	// Token: 0x06002A3C RID: 10812 RVA: 0x000E3C08 File Offset: 0x000E1E08
	public override bool IsDone()
	{
		return Time.timeAsDouble - this.startTime >= (double)this.duration;
	}

	// Token: 0x06002A3D RID: 10813 RVA: 0x000E3C24 File Offset: 0x000E1E24
	protected override void UpdateShared(float dt)
	{
		double num = (double)((float)Time.timeAsDouble) - this.startTime;
		switch (this.state)
		{
		case GRAbilityAttackJump.State.Tell:
			if (num > (double)this.jumpTime)
			{
				this.targetPos = this.agent.transform.position + this.agent.transform.forward * 0.5f;
				if (this.target != null)
				{
					Vector3 vector = this.target.transform.position - this.agent.transform.position;
					this.targetPos = this.agent.transform.position + vector * this.jumpLengthScale;
					this.targetPos.y = this.target.transform.position.y;
				}
				float num2 = this.attackLandTime - this.jumpTime;
				num2 = Mathf.Max(0.1f, num2);
				this.initialPos = this.agent.transform.position;
				Vector3 vector2 = this.targetPos - this.initialPos;
				float y = vector2.y;
				vector2.y = 0f;
				float num3 = num2;
				float y2 = 0f;
				if (num3 > 0f)
				{
					Vector3 gravity = Physics.gravity;
					y2 = (y - 0.5f * gravity.y * num3 * num3) / num3;
				}
				this.initialVel = vector2 / num2;
				this.initialVel.y = y2;
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(true);
				}
				this.PlayAnim(this.jumpAnimName, 0.1f, this.animSpeed);
				this.jumpSound.Play(null);
				this.state = GRAbilityAttackJump.State.Jump;
			}
			break;
		case GRAbilityAttackJump.State.Jump:
		{
			float num4 = (float)(num - (double)this.jumpTime);
			Vector3 position = this.initialPos + this.initialVel * num4 + 0.5f * Physics.gravity * num4 * num4;
			this.root.position = position;
			if (num > (double)this.attackLandTime)
			{
				if (this.damageTrigger != null)
				{
					this.damageTrigger.SetActive(false);
				}
				if (this.doReturnPhase)
				{
					float num5 = this.attackReturnTime - this.attackLandTime;
					num5 = Mathf.Max(0.1f, num5);
					Vector3 vector3 = this.initialPos;
					this.initialPos = this.agent.transform.position;
					this.initialVel = (vector3 - this.initialPos) / num5;
					this.state = GRAbilityAttackJump.State.Return;
				}
				else
				{
					this.state = GRAbilityAttackJump.State.Done;
				}
			}
			break;
		}
		case GRAbilityAttackJump.State.Return:
		{
			float num6 = (float)(num - (double)this.attackLandTime);
			Vector3 position2 = this.initialPos + this.initialVel * num6;
			this.root.position = position2;
			if (num > (double)this.attackReturnTime)
			{
				this.state = GRAbilityAttackJump.State.Done;
			}
			break;
		}
		}
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002A3E RID: 10814 RVA: 0x000E3F5C File Offset: 0x000E215C
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

	// Token: 0x0400367B RID: 13947
	public float duration;

	// Token: 0x0400367C RID: 13948
	public float jumpTime;

	// Token: 0x0400367D RID: 13949
	public float attackLandTime;

	// Token: 0x0400367E RID: 13950
	public float attackReturnTime;

	// Token: 0x0400367F RID: 13951
	public bool doReturnPhase = true;

	// Token: 0x04003680 RID: 13952
	public float jumpLengthScale = 1f;

	// Token: 0x04003681 RID: 13953
	public string animName;

	// Token: 0x04003682 RID: 13954
	public float animSpeed;

	// Token: 0x04003683 RID: 13955
	public float maxTurnSpeed;

	// Token: 0x04003684 RID: 13956
	public string jumpAnimName;

	// Token: 0x04003685 RID: 13957
	public AbilitySound jumpSound;

	// Token: 0x04003686 RID: 13958
	public GameObject damageTrigger;

	// Token: 0x04003687 RID: 13959
	private Transform target;

	// Token: 0x04003688 RID: 13960
	private GRAbilityAttackJump.State state;

	// Token: 0x04003689 RID: 13961
	public Vector3 targetPos;

	// Token: 0x0400368A RID: 13962
	public Vector3 initialPos;

	// Token: 0x0400368B RID: 13963
	public Vector3 initialVel;

	// Token: 0x02000674 RID: 1652
	private enum State
	{
		// Token: 0x0400368D RID: 13965
		Tell,
		// Token: 0x0400368E RID: 13966
		Jump,
		// Token: 0x0400368F RID: 13967
		Return,
		// Token: 0x04003690 RID: 13968
		Done
	}
}
