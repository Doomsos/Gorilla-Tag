using System;
using UnityEngine;

// Token: 0x02000678 RID: 1656
[Serializable]
public class GRAbilityGrabbed : GRAbilityBase
{
	// Token: 0x06002A5E RID: 10846 RVA: 0x000E48BA File Offset: 0x000E2ABA
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.idleAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002A5F RID: 10847 RVA: 0x000E48DF File Offset: 0x000E2ADF
	public override void Start()
	{
		base.Start();
		this.agent.SetIsPathing(false, true);
		this.idleAbility.Start();
	}

	// Token: 0x06002A60 RID: 10848 RVA: 0x000E48FF File Offset: 0x000E2AFF
	public override void Stop()
	{
		this.idleAbility.Stop();
		this.agent.SetIsPathing(true, true);
	}

	// Token: 0x06002A61 RID: 10849 RVA: 0x000E4919 File Offset: 0x000E2B19
	public override bool IsDone()
	{
		return this.idleAbility.IsDone();
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x000E4926 File Offset: 0x000E2B26
	public override void Update(float dt)
	{
		this.idleAbility.Update(dt);
	}

	// Token: 0x040036AA RID: 13994
	public GRAbilityIdle idleAbility;
}
