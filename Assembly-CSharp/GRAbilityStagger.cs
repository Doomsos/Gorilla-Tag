using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000675 RID: 1653
[Serializable]
public class GRAbilityStagger : GRAbilityBase
{
	// Token: 0x06002A40 RID: 10816 RVA: 0x000E3F96 File Offset: 0x000E2196
	public void SetStunTime(float time)
	{
		this.stunTime = time;
	}

	// Token: 0x06002A41 RID: 10817 RVA: 0x000E3FA0 File Offset: 0x000E21A0
	public void SetStaggerVelocity(Vector3 vel)
	{
		float magnitude = vel.magnitude;
		if (magnitude > 0f)
		{
			Vector3 vector = vel / magnitude;
			vector.y = 0f;
			vel = vector * magnitude;
		}
		this.staggerMovement.InitFromVelocityAndDuration(vel, this.duration);
	}

	// Token: 0x06002A42 RID: 10818 RVA: 0x000E3FEC File Offset: 0x000E21EC
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.staggerMovement.Setup(root);
		this.staggerMovement.interpolationType = GRAbilityInterpolatedMovement.InterpType.EaseOut;
	}

	// Token: 0x06002A43 RID: 10819 RVA: 0x000E4018 File Offset: 0x000E2218
	public override void Start()
	{
		base.Start();
		if (this.animData.Count > 0)
		{
			this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
			this.duration = this.animData[this.lastAnimIndex].duration + this.stunTime;
			this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animData[this.lastAnimIndex].speed);
			this.animNameString = this.animData[this.lastAnimIndex].animName;
		}
		else
		{
			this.duration = 0.5f + this.stunTime;
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
		this.staggerMovement.InitFromVelocityAndDuration(this.staggerMovement.velocity, this.duration);
		this.staggerMovement.Start();
	}

	// Token: 0x06002A44 RID: 10820 RVA: 0x000E4123 File Offset: 0x000E2323
	public override void Stop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
	}

	// Token: 0x06002A45 RID: 10821 RVA: 0x000E413E File Offset: 0x000E233E
	public override bool IsDone()
	{
		return this.staggerMovement.IsDone();
	}

	// Token: 0x06002A46 RID: 10822 RVA: 0x000E414B File Offset: 0x000E234B
	protected override void UpdateShared(float dt)
	{
		this.staggerMovement.Update(dt);
	}

	// Token: 0x06002A47 RID: 10823 RVA: 0x000E4159 File Offset: 0x000E2359
	public string GetAnimName()
	{
		return this.animNameString;
	}

	// Token: 0x04003691 RID: 13969
	private float duration;

	// Token: 0x04003692 RID: 13970
	public List<AnimationData> animData;

	// Token: 0x04003693 RID: 13971
	private int lastAnimIndex = -1;

	// Token: 0x04003694 RID: 13972
	private string animNameString;

	// Token: 0x04003695 RID: 13973
	public GRAbilityInterpolatedMovement staggerMovement;

	// Token: 0x04003696 RID: 13974
	private float stunTime;
}
