using System;
using UnityEngine;

// Token: 0x02000679 RID: 1657
[Serializable]
public class GRAbilityThrown : GRAbilityBase
{
	// Token: 0x06002A64 RID: 10852 RVA: 0x000E4934 File Offset: 0x000E2B34
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.idleAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002A65 RID: 10853 RVA: 0x000E4959 File Offset: 0x000E2B59
	public override void Start()
	{
		base.Start();
		this.agent.SetIsPathing(false, false);
		this.idleAbility.Start();
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x000E4979 File Offset: 0x000E2B79
	public override void Stop()
	{
		this.idleAbility.Stop();
		this.agent.SetIsPathing(true, false);
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x000E4993 File Offset: 0x000E2B93
	public override bool IsDone()
	{
		return this.idleAbility.IsDone();
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x000E49A0 File Offset: 0x000E2BA0
	public override void Update(float dt)
	{
		this.idleAbility.Update(dt);
	}

	// Token: 0x040036AB RID: 13995
	public GRAbilityIdle idleAbility;
}
