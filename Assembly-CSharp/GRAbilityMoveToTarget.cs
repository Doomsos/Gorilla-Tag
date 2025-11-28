using System;
using UnityEngine;

// Token: 0x0200066D RID: 1645
[Serializable]
public class GRAbilityMoveToTarget : GRAbilityBase
{
	// Token: 0x06002A13 RID: 10771 RVA: 0x000E30D8 File Offset: 0x000E12D8
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
		this.targetPos = agent.transform.position;
	}

	// Token: 0x06002A14 RID: 10772 RVA: 0x000E3104 File Offset: 0x000E1304
	public override void Start()
	{
		base.Start();
		this.PlayAnim(this.animName, 0.3f, this.animSpeed);
		if (this.attributes && this.moveSpeed == 0f)
		{
			this.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		}
		this.agent.navAgent.speed = this.moveSpeed;
		this.targetPos = this.agent.transform.position;
		this.movementSound.Play(null);
	}

	// Token: 0x06002A15 RID: 10773 RVA: 0x000E3193 File Offset: 0x000E1393
	public override void Stop()
	{
		base.Stop();
		this.movementSound.Stop();
	}

	// Token: 0x06002A16 RID: 10774 RVA: 0x000E31A8 File Offset: 0x000E13A8
	public override bool IsDone()
	{
		return (this.targetPos - this.root.position).sqrMagnitude < 0.25f;
	}

	// Token: 0x06002A17 RID: 10775 RVA: 0x000E31DC File Offset: 0x000E13DC
	protected override void UpdateShared(float dt)
	{
		if (this.target != null)
		{
			this.targetPos = this.target.position;
			this.agent.RequestDestination(this.targetPos);
		}
		Transform transform = (this.lookAtTarget != null) ? this.lookAtTarget : this.target;
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, transform, this.maxTurnSpeed);
	}

	// Token: 0x06002A18 RID: 10776 RVA: 0x000E3253 File Offset: 0x000E1453
	public void SetTarget(Transform transform)
	{
		this.target = transform;
	}

	// Token: 0x06002A19 RID: 10777 RVA: 0x000E325C File Offset: 0x000E145C
	public void SetTargetPos(Vector3 targetPos)
	{
		this.targetPos = targetPos;
		this.agent.RequestDestination(targetPos);
	}

	// Token: 0x06002A1A RID: 10778 RVA: 0x000E3271 File Offset: 0x000E1471
	public Vector3 GetTargetPos()
	{
		return this.targetPos;
	}

	// Token: 0x06002A1B RID: 10779 RVA: 0x000E3279 File Offset: 0x000E1479
	public void SetLookAtTarget(Transform transform)
	{
		this.lookAtTarget = transform;
	}

	// Token: 0x0400364A RID: 13898
	public float moveSpeed;

	// Token: 0x0400364B RID: 13899
	public string animName;

	// Token: 0x0400364C RID: 13900
	public float animSpeed = 1f;

	// Token: 0x0400364D RID: 13901
	public float maxTurnSpeed = 360f;

	// Token: 0x0400364E RID: 13902
	public AbilitySound movementSound;

	// Token: 0x0400364F RID: 13903
	private Vector3 targetPos;

	// Token: 0x04003650 RID: 13904
	private Transform target;

	// Token: 0x04003651 RID: 13905
	private Transform lookAtTarget;
}
