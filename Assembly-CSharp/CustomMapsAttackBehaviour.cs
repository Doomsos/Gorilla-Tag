using System;
using GorillaExtensions;
using GorillaGameModes;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000957 RID: 2391
public class CustomMapsAttackBehaviour : CustomMapsBehaviourBase
{
	// Token: 0x06003D29 RID: 15657 RVA: 0x00144B60 File Offset: 0x00142D60
	public CustomMapsAttackBehaviour(CustomMapsAIBehaviourController AIController, AIAgent agentSettings)
	{
		this.attackType = agentSettings.attackType;
		this.attackDist = agentSettings.attackDist;
		this.attackDistSq = this.attackDist * this.attackDist;
		this.stopMovingToAttack = agentSettings.stopMovingToAttack;
		this.useColliders = agentSettings.useColliders;
		this.damageDelayAfterPlayingAnimation = agentSettings.damageDelayAfterPlayingAnim;
		this.damageAmount = agentSettings.damageAmount;
		this.attackAnimName = agentSettings.attackAnimName;
		this.sightOffset = agentSettings.sightOffset;
		this.sightFOV = agentSettings.sightFOV;
		this.sightMinDot = Mathf.Cos(this.sightFOV / 2f * 0.017453292f);
		this.controller = AIController;
		this.animBlendTime = agentSettings.animBlendTime;
		this.turnSpeed = agentSettings.turnSpeed * 10f;
		this.timeBetweenAttacks = agentSettings.timeBetweenAttacks;
		this.controller.attributes.AddAttribute(GRAttributeType.PlayerDamage, this.damageAmount);
		this.state = CustomMapsAttackBehaviour.State.Idle;
	}

	// Token: 0x06003D2A RID: 15658 RVA: 0x00144C5F File Offset: 0x00142E5F
	public override bool CanExecute()
	{
		return !this.controller.IsNull() && !this.controller.TargetPlayer.IsNull() && this.IsTargetInAttackRange(null) && this.IsTargetVisible();
	}

	// Token: 0x06003D2B RID: 15659 RVA: 0x00144C98 File Offset: 0x00142E98
	private bool IsTargetVisible()
	{
		Vector3 startPos = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
		return this.controller.IsTargetVisible(startPos, this.controller.TargetPlayer, this.attackDist);
	}

	// Token: 0x06003D2C RID: 15660 RVA: 0x00144CF0 File Offset: 0x00142EF0
	private bool IsTargetInAttackRange(GRPlayer target = null)
	{
		if (target.IsNull() && this.controller.TargetPlayer.IsNull())
		{
			return false;
		}
		if (target.IsNotNull())
		{
			Vector3 vector;
			return this.controller.IsTargetInRange(this.controller.transform.position, target, this.attackDistSq, out vector);
		}
		Vector3 vector2;
		return this.controller.IsTargetInRange(this.controller.transform.position, this.controller.TargetPlayer, this.attackDistSq, out vector2);
	}

	// Token: 0x06003D2D RID: 15661 RVA: 0x00144D74 File Offset: 0x00142F74
	public override bool CanContinueExecuting()
	{
		if (this.state != CustomMapsAttackBehaviour.State.Idle && this.controller.IsAnimationPlaying(this.attackAnimName))
		{
			return true;
		}
		if (this.controller.IsNull() || this.controller.TargetPlayer.IsNull())
		{
			return false;
		}
		if (!this.controller.IsTargetable(this.controller.TargetPlayer))
		{
			this.controller.ClearTarget();
			return false;
		}
		return this.CanExecute();
	}

	// Token: 0x06003D2E RID: 15662 RVA: 0x00144DEA File Offset: 0x00142FEA
	public override void Execute()
	{
		if (this.controller.IsNull())
		{
			return;
		}
		if (this.stopMovingToAttack)
		{
			this.controller.StopMoving();
		}
		this.FaceTarget();
		this.controller.agent.RequestBehaviorChange(2);
	}

	// Token: 0x06003D2F RID: 15663 RVA: 0x00144E24 File Offset: 0x00143024
	public override void NetExecute()
	{
		if (this.controller.IsNull())
		{
			return;
		}
		if (this.state == CustomMapsAttackBehaviour.State.Attacking && !this.useColliders && this.startTime > this.lastAttackTime && Time.time > this.startTime + this.damageDelayAfterPlayingAnimation)
		{
			this.TriggerAttack(null);
		}
		if (this.controller.IsAnimationPlaying(this.attackAnimName))
		{
			return;
		}
		CustomMapsAttackBehaviour.State state = this.state;
		if (state != CustomMapsAttackBehaviour.State.Idle)
		{
			if (state != CustomMapsAttackBehaviour.State.Attacking)
			{
				return;
			}
			if (Time.time < this.startTime + this.timeBetweenAttacks)
			{
				this.state = CustomMapsAttackBehaviour.State.Idle;
				return;
			}
			this.startTime = Time.time;
			this.controller.PlayAnimation(this.attackAnimName, this.animBlendTime);
			return;
		}
		else
		{
			if (Time.time < this.startTime + this.timeBetweenAttacks)
			{
				return;
			}
			this.startTime = Time.time;
			this.state = CustomMapsAttackBehaviour.State.Attacking;
			this.controller.PlayAnimation(this.attackAnimName, this.animBlendTime);
			return;
		}
	}

	// Token: 0x06003D30 RID: 15664 RVA: 0x00144F19 File Offset: 0x00143119
	public override void ResetBehavior()
	{
		this.state = CustomMapsAttackBehaviour.State.Idle;
	}

	// Token: 0x06003D31 RID: 15665 RVA: 0x00144F24 File Offset: 0x00143124
	private void FaceTarget()
	{
		if (this.controller.TargetPlayer.IsNull())
		{
			return;
		}
		GameAgent.UpdateFacingTarget(this.controller.transform, this.controller.agent.navAgent, this.controller.TargetPlayer.transform, this.turnSpeed);
	}

	// Token: 0x06003D32 RID: 15666 RVA: 0x00144F7C File Offset: 0x0014317C
	public override void OnTriggerEnter(Collider otherCollider)
	{
		if (!this.useColliders)
		{
			return;
		}
		if (Time.time < this.lastAttackTime + this.timeBetweenAttacks || this.state != CustomMapsAttackBehaviour.State.Attacking)
		{
			return;
		}
		GRPlayer componentInParent = otherCollider.GetComponentInParent<GRPlayer>();
		if (componentInParent.IsNull())
		{
			return;
		}
		if (componentInParent.MyRig.IsNotNull() && !componentInParent.MyRig.isLocal)
		{
			return;
		}
		if (componentInParent.State == GRPlayer.GRPlayerState.Ghost)
		{
			return;
		}
		this.TriggerAttack(componentInParent);
	}

	// Token: 0x06003D33 RID: 15667 RVA: 0x00144FEC File Offset: 0x001431EC
	private void TriggerAttack(GRPlayer targetPlayer = null)
	{
		this.lastAttackTime = Time.time;
		GRPlayer grplayer = (targetPlayer != null) ? targetPlayer : (this.controller.entity.IsAuthority() ? this.controller.TargetPlayer : null);
		if (!this.controller.entity.IsAuthority() && grplayer == null)
		{
			Vector3 sourcePos = this.controller.transform.position + this.controller.transform.TransformVector(this.sightOffset);
			grplayer = this.controller.FindBestTarget(sourcePos, this.attackDist, this.attackDistSq, this.sightMinDot);
		}
		if (grplayer == null)
		{
			return;
		}
		if (!grplayer.MyRig.isLocal)
		{
			return;
		}
		if (this.controller.entity.IsAuthority() && !this.IsTargetInAttackRange(grplayer))
		{
			return;
		}
		switch (this.attackType)
		{
		case 0:
			if (GameMode.ActiveGameMode.GameType() != GameModeType.Custom)
			{
				GameMode.ReportHit();
				return;
			}
			CustomGameMode.TaggedByAI(this.controller.entity, grplayer.MyRig.OwningNetPlayer.ActorNumber);
			return;
		case 1:
			CustomMapsGameManager.instance.OnPlayerHit(this.controller.entity.id, grplayer, this.controller.transform.position);
			return;
		case 2:
			CustomGameMode.OnPlayerHit(this.controller.entity, grplayer.MyRig.OwningNetPlayer.ActorNumber, this.damageAmount);
			return;
		default:
			return;
		}
	}

	// Token: 0x04004DE6 RID: 19942
	private CustomMapsAIBehaviourController controller;

	// Token: 0x04004DE7 RID: 19943
	private CustomMapsAttackBehaviour.State state;

	// Token: 0x04004DE8 RID: 19944
	private AttackType attackType;

	// Token: 0x04004DE9 RID: 19945
	private float attackDist;

	// Token: 0x04004DEA RID: 19946
	private float attackDistSq;

	// Token: 0x04004DEB RID: 19947
	private bool stopMovingToAttack;

	// Token: 0x04004DEC RID: 19948
	private bool useColliders;

	// Token: 0x04004DED RID: 19949
	private float damageAmount;

	// Token: 0x04004DEE RID: 19950
	private Vector3 sightOffset;

	// Token: 0x04004DEF RID: 19951
	private float sightFOV;

	// Token: 0x04004DF0 RID: 19952
	private float sightMinDot;

	// Token: 0x04004DF1 RID: 19953
	private string attackAnimName;

	// Token: 0x04004DF2 RID: 19954
	private float timeBetweenAttacks;

	// Token: 0x04004DF3 RID: 19955
	private float damageDelayAfterPlayingAnimation;

	// Token: 0x04004DF4 RID: 19956
	private float animBlendTime;

	// Token: 0x04004DF5 RID: 19957
	private float startTime;

	// Token: 0x04004DF6 RID: 19958
	private float turnSpeed;

	// Token: 0x04004DF7 RID: 19959
	private float lastAttackTime;

	// Token: 0x02000958 RID: 2392
	private enum State
	{
		// Token: 0x04004DF9 RID: 19961
		Idle,
		// Token: 0x04004DFA RID: 19962
		Attacking
	}
}
