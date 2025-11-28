using System;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200066B RID: 1643
[Serializable]
public class GRAbilityJump : GRAbilityBase
{
	// Token: 0x06002A02 RID: 10754 RVA: 0x000E2CA6 File Offset: 0x000E0EA6
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.isActive = false;
	}

	// Token: 0x06002A03 RID: 10755 RVA: 0x000E2CC0 File Offset: 0x000E0EC0
	public void SetupJump(Vector3 start, Vector3 end, float heightScale = 1f, float speedScale = 1f)
	{
		this.elapsedTime = 0f;
		this.startPos = start;
		this.endPos = end;
		float magnitude = (this.endPos - this.startPos).magnitude;
		this.controlPoint = (this.startPos + this.endPos) / 2f + new Vector3(0f, magnitude * heightScale, 0f);
		this.jumpTime = magnitude / (this.jumpSpeed * speedScale);
	}

	// Token: 0x06002A04 RID: 10756 RVA: 0x000E2D4C File Offset: 0x000E0F4C
	public void SetupJumpFromLinkData(OffMeshLinkData linkData)
	{
		if ((this.root.position - linkData.startPos).sqrMagnitude < (this.root.position - linkData.endPos).sqrMagnitude)
		{
			this.SetupJump(linkData.startPos, linkData.endPos, 1f, 1f);
			return;
		}
		this.SetupJump(linkData.endPos, linkData.startPos, 1f, 1f);
	}

	// Token: 0x06002A05 RID: 10757 RVA: 0x000E2DD8 File Offset: 0x000E0FD8
	public override void Start()
	{
		base.Start();
		this.elapsedTime = 0f;
		this.isActive = true;
		this.PlayAnim(this.animationData.animName, 0.05f, this.animationData.speed);
		this.agent.navAgent.isStopped = true;
		this.agent.SetDisableNetworkSync(true);
		this.agent.pauseEntityThink = true;
		this.soundJump.Play(this.audioSource);
	}

	// Token: 0x06002A06 RID: 10758 RVA: 0x000E2E58 File Offset: 0x000E1058
	public override void Stop()
	{
		base.Stop();
		this.agent.navAgent.Warp(this.endPos);
		this.agent.navAgent.CompleteOffMeshLink();
		this.agent.navAgent.isStopped = false;
		this.isActive = false;
		this.agent.SetDisableNetworkSync(false);
		this.agent.pauseEntityThink = false;
	}

	// Token: 0x06002A07 RID: 10759 RVA: 0x000E2EC2 File Offset: 0x000E10C2
	public override bool IsDone()
	{
		return this.elapsedTime >= this.jumpTime;
	}

	// Token: 0x06002A08 RID: 10760 RVA: 0x000E2ED5 File Offset: 0x000E10D5
	public bool IsActive()
	{
		return this.isActive;
	}

	// Token: 0x06002A09 RID: 10761 RVA: 0x000E2EE0 File Offset: 0x000E10E0
	protected override void UpdateShared(float dt)
	{
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.startPos, this.controlPoint, Color.green, true);
			DebugUtil.DrawLine(this.endPos, this.controlPoint, Color.green, true);
		}
		float t = (this.jumpTime > 0f) ? Math.Clamp(this.elapsedTime / this.jumpTime, 0f, 1f) : 1f;
		Vector3 position = GRAbilityJump.EvaluateQuadratic(this.startPos, this.controlPoint, this.endPos, t);
		this.root.position = position;
		if (this.rb != null)
		{
			this.rb.position = position;
		}
		this.elapsedTime += dt;
	}

	// Token: 0x06002A0A RID: 10762 RVA: 0x000E2FA0 File Offset: 0x000E11A0
	public static Vector3 EvaluateQuadratic(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		Vector3 vector = Vector3.Lerp(p0, p1, t);
		Vector3 vector2 = Vector3.Lerp(p1, p2, t);
		return Vector3.Lerp(vector, vector2, t);
	}

	// Token: 0x0400363B RID: 13883
	private Vector3 startPos;

	// Token: 0x0400363C RID: 13884
	private Vector3 endPos;

	// Token: 0x0400363D RID: 13885
	private Vector3 controlPoint;

	// Token: 0x0400363E RID: 13886
	[ReadOnly]
	public float jumpTime;

	// Token: 0x0400363F RID: 13887
	[ReadOnly]
	public float elapsedTime;

	// Token: 0x04003640 RID: 13888
	private bool isActive;

	// Token: 0x04003641 RID: 13889
	public AnimationData animationData;

	// Token: 0x04003642 RID: 13890
	public float jumpSpeed = 3f;

	// Token: 0x04003643 RID: 13891
	public AbilitySound soundJump;
}
