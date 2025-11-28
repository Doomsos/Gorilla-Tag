using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000668 RID: 1640
[Serializable]
public class GRAbilityIdle : GRAbilityBase
{
	// Token: 0x060029F5 RID: 10741 RVA: 0x000E2946 File Offset: 0x000E0B46
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.animLoops = 0;
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x000E2960 File Offset: 0x000E0B60
	public override void Start()
	{
		base.Start();
		this.agent.navAgent.isStopped = true;
		this.PlayAnim(this.animName, 0.3f, this.animSpeed);
		this.animLoops = 0;
		this.events.Reset();
	}

	// Token: 0x060029F7 RID: 10743 RVA: 0x000E29AD File Offset: 0x000E0BAD
	public override void Stop()
	{
		base.Stop();
		this.agent.navAgent.isStopped = false;
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x000E29C6 File Offset: 0x000E0BC6
	public override bool IsDone()
	{
		return (double)this.duration > 0.0 && Time.timeAsDouble >= this.startTime + (double)this.duration;
	}

	// Token: 0x060029F9 RID: 10745 RVA: 0x000E29F4 File Offset: 0x000E0BF4
	protected override void UpdateShared(float dt)
	{
		float abilityTime = (float)(Time.timeAsDouble - this.startTime);
		if (this.anim != null && this.anim[this.animName] != null)
		{
			if ((int)this.anim[this.animName].normalizedTime > this.animLoops)
			{
				this.events.Reset();
				this.animLoops = (int)this.anim[this.animName].normalizedTime;
			}
			abilityTime = this.anim[this.animName].time - this.anim[this.animName].length * (float)this.animLoops;
		}
		this.events.TryPlay(abilityTime, this.audioSource);
	}

	// Token: 0x04003629 RID: 13865
	public float duration;

	// Token: 0x0400362A RID: 13866
	public string animName;

	// Token: 0x0400362B RID: 13867
	public float animSpeed;

	// Token: 0x0400362C RID: 13868
	public GameAbilityEvents events;

	// Token: 0x0400362D RID: 13869
	[ReadOnly]
	public int animLoops;
}
