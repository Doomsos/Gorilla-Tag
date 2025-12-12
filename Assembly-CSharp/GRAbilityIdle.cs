using System;
using Unity.XR.CoreUtils;
using UnityEngine;

[Serializable]
public class GRAbilityIdle : GRAbilityBase
{
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.animLoops = 0;
	}

	protected override void OnStart()
	{
		this.agent.SetStopped(true);
		this.PlayAnim(this.animName, 0.3f, this.animSpeed);
		this.animLoops = 0;
		this.events.Reset();
	}

	protected override void OnStop()
	{
		this.agent.SetStopped(false);
	}

	public override bool IsDone()
	{
		return (double)this.duration > 0.0 && Time.timeAsDouble >= this.startTime + (double)this.duration;
	}

	protected override void OnUpdateShared(float dt)
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

	public float duration;

	public string animName;

	public float animSpeed;

	public GameAbilityEvents events;

	[ReadOnly]
	public int animLoops;
}
