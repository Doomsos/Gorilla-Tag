using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200067E RID: 1662
[Serializable]
public class GRAbilityFlashed : GRAbilityBase
{
	// Token: 0x06002A8E RID: 10894 RVA: 0x000E49CE File Offset: 0x000E2BCE
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x000E56F4 File Offset: 0x000E38F4
	public void SetStunTime(float time)
	{
		this.stunTime = time;
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x000E5700 File Offset: 0x000E3900
	public override void Start()
	{
		base.Start();
		if (this.flashAnimations.Count > 0)
		{
			this.flashAnimationIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.flashAnimations.Count, this.flashAnimationIndex);
			this.PlayAnim(this.flashAnimations[this.flashAnimationIndex].animName, 0.1f, this.flashAnimations[this.flashAnimationIndex].speed);
			this.behaviorEndTime = Time.timeAsDouble + (double)this.flashAnimations[this.flashAnimationIndex].duration + (double)this.stunTime;
		}
		else
		{
			this.PlayAnim("GREnemyFlashReaction01", 0.1f, 1f);
			this.behaviorEndTime = Time.timeAsDouble + 0.5 + (double)this.stunTime;
		}
		this.agent.SetIsPathing(false, true);
		this.agent.SetDisableNetworkSync(true);
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x000E4143 File Offset: 0x000E2343
	public override void Stop()
	{
		this.agent.SetIsPathing(true, true);
		this.agent.SetDisableNetworkSync(false);
	}

	// Token: 0x06002A92 RID: 10898 RVA: 0x000E57F0 File Offset: 0x000E39F0
	public override bool IsDone()
	{
		return Time.timeAsDouble >= this.behaviorEndTime;
	}

	// Token: 0x040036D8 RID: 14040
	public List<AnimationData> flashAnimations;

	// Token: 0x040036D9 RID: 14041
	private int flashAnimationIndex;

	// Token: 0x040036DA RID: 14042
	private double behaviorEndTime;

	// Token: 0x040036DB RID: 14043
	private float stunTime;
}
