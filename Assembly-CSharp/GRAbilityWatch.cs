using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x0200066C RID: 1644
[Serializable]
public class GRAbilityWatch : GRAbilityBase
{
	// Token: 0x06002A0C RID: 10764 RVA: 0x000E2FB8 File Offset: 0x000E11B8
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.target = null;
	}

	// Token: 0x06002A0D RID: 10765 RVA: 0x000E2FD0 File Offset: 0x000E11D0
	public override void Start()
	{
		base.Start();
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.endTime = -1.0;
		if (this.duration > 0f)
		{
			this.endTime = Time.timeAsDouble + (double)this.duration;
		}
		this.agent.navAgent.isStopped = true;
	}

	// Token: 0x06002A0E RID: 10766 RVA: 0x000E303A File Offset: 0x000E123A
	public override void Stop()
	{
		this.agent.navAgent.isStopped = false;
	}

	// Token: 0x06002A0F RID: 10767 RVA: 0x000E304D File Offset: 0x000E124D
	public override bool IsDone()
	{
		return this.endTime > 0.0 && Time.timeAsDouble >= this.endTime;
	}

	// Token: 0x06002A10 RID: 10768 RVA: 0x000E3072 File Offset: 0x000E1272
	protected override void UpdateShared(float dt)
	{
		GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.target, this.maxTurnSpeed);
	}

	// Token: 0x06002A11 RID: 10769 RVA: 0x000E3098 File Offset: 0x000E1298
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

	// Token: 0x04003644 RID: 13892
	public float duration;

	// Token: 0x04003645 RID: 13893
	public string animName;

	// Token: 0x04003646 RID: 13894
	public float animSpeed;

	// Token: 0x04003647 RID: 13895
	public float maxTurnSpeed;

	// Token: 0x04003648 RID: 13896
	private Transform target;

	// Token: 0x04003649 RID: 13897
	[ReadOnly]
	public double endTime;
}
