using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200066E RID: 1646
[Serializable]
public class GRAbilityChase : GRAbilityBase
{
	// Token: 0x06002A1D RID: 10781 RVA: 0x000E32A0 File Offset: 0x000E14A0
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.targetPlayer = null;
		this.lastSeenTargetTime = 0.0;
		this.lastSeenTargetPosition = Vector3.zero;
		if (GRAbilityChase.targetOffsets == null)
		{
			int num = 8;
			GRAbilityChase.targetOffsets = new List<Vector3>(num);
			float num2 = 1f;
			for (int i = 0; i < num; i++)
			{
				Vector3 vector;
				vector..ctor(num2, 0f, 0f);
				vector = Quaternion.Euler(0f, (float)i / (float)num * 360f, 0f) * vector;
				GRAbilityChase.targetOffsets.Add(vector);
			}
			Random random = new Random();
			List<Vector3> list = Enumerable.ToList<Vector3>(Enumerable.OrderBy<Vector3, int>(GRAbilityChase.targetOffsets, (Vector3 x) => random.Next()));
			GRAbilityChase.targetOffsets.Clear();
			GRAbilityChase.targetOffsets.AddRange(list);
		}
		if (this.attributes && this.chaseSpeed == 0f)
		{
			this.chaseSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.ChaseSpeed);
		}
	}

	// Token: 0x06002A1E RID: 10782 RVA: 0x000E33BC File Offset: 0x000E15BC
	public override void Start()
	{
		base.Start();
		this.PlayAnim(this.animName, 0.1f, this.animSpeed);
		this.agent.navAgent.speed = this.chaseSpeed;
		this.lastSeenTargetTime = Time.timeAsDouble;
		this.movementSound.Play(null);
	}

	// Token: 0x06002A1F RID: 10783 RVA: 0x000E3413 File Offset: 0x000E1613
	public override void Stop()
	{
		base.Stop();
	}

	// Token: 0x06002A20 RID: 10784 RVA: 0x000E341B File Offset: 0x000E161B
	public override bool IsDone()
	{
		return this.targetPlayer == null || Time.timeAsDouble - this.lastSeenTargetTime >= (double)this.giveUpDelay;
	}

	// Token: 0x06002A21 RID: 10785 RVA: 0x000E3440 File Offset: 0x000E1640
	public override void Think(float dt)
	{
		GRPlayer grplayer = GRPlayer.Get(this.targetPlayer);
		if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
		{
			Vector3 vector = grplayer.transform.position;
			vector += GRAbilityChase.GetMoveTargetOffset(vector, this.entity);
			if (this.lineOfSight.HasLineOfSight(this.head.position, vector))
			{
				this.lastSeenTargetTime = Time.timeAsDouble;
			}
			if ((float)(Time.timeAsDouble - this.lastSeenTargetTime) <= this.loseVisibilityDelay)
			{
				this.lastSeenTargetPosition = vector;
			}
		}
		this.agent.RequestDestination(this.lastSeenTargetPosition);
	}

	// Token: 0x06002A22 RID: 10786 RVA: 0x000E34DC File Offset: 0x000E16DC
	protected override void UpdateShared(float dt)
	{
		GameAgent.UpdateFacing(this.root, this.agent.navAgent, this.targetPlayer, this.maxTurnSpeed);
	}

	// Token: 0x06002A23 RID: 10787 RVA: 0x000E3500 File Offset: 0x000E1700
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.targetPlayer = targetPlayer;
	}

	// Token: 0x06002A24 RID: 10788 RVA: 0x000E350C File Offset: 0x000E170C
	public static Vector3 GetMoveTargetOffset(Vector3 targetPos, GameEntity attackingEntity)
	{
		int num = attackingEntity.id.index % GRAbilityChase.targetOffsets.Count;
		return GRAbilityChase.targetOffsets[num];
	}

	// Token: 0x04003652 RID: 13906
	public float chaseSpeed;

	// Token: 0x04003653 RID: 13907
	public string animName;

	// Token: 0x04003654 RID: 13908
	public float animSpeed;

	// Token: 0x04003655 RID: 13909
	public float maxTurnSpeed;

	// Token: 0x04003656 RID: 13910
	public float loseVisibilityDelay;

	// Token: 0x04003657 RID: 13911
	public float giveUpDelay;

	// Token: 0x04003658 RID: 13912
	public AbilitySound movementSound;

	// Token: 0x04003659 RID: 13913
	private NetPlayer targetPlayer;

	// Token: 0x0400365A RID: 13914
	private double lastSeenTargetTime;

	// Token: 0x0400365B RID: 13915
	private Vector3 lastSeenTargetPosition;

	// Token: 0x0400365C RID: 13916
	private static List<Vector3> targetOffsets;
}
