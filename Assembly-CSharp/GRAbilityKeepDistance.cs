using System;
using CjLib;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200067C RID: 1660
[Serializable]
public class GRAbilityKeepDistance : GRAbilityBase
{
	// Token: 0x06002A76 RID: 10870 RVA: 0x000E4DE8 File Offset: 0x000E2FE8
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.navMeshAgent = agent.GetComponent<NavMeshAgent>();
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.attributes && this.moveAbility.moveSpeed == 0f)
		{
			this.moveAbility.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.BackupSpeed);
		}
	}

	// Token: 0x06002A77 RID: 10871 RVA: 0x000E4E5C File Offset: 0x000E305C
	public override void Start()
	{
		base.Start();
		if (this.target != null)
		{
			Vector3 vector = this.agent.transform.position - this.target.position;
			if (this.maxDistanceFromTarget > 0f && vector.magnitude > this.maxDistanceFromTarget)
			{
				this.navMeshAgent.isStopped = true;
				this.PlayAnim(this.idleAnimName, 0.5f, 1f);
				this.idleSound.Play(null);
			}
			else
			{
				this.moveAbility.Start();
			}
		}
		else
		{
			this.moveAbility.Start();
		}
		this.agent.SetIsPathing(true, true);
		Vector3 targetPos = this.PickBackupDestination();
		this.moveAbility.SetTargetPos(targetPos);
		this.defaultUpdateRotation = this.navMeshAgent.updateRotation;
		this.navMeshAgent.updateRotation = false;
	}

	// Token: 0x06002A78 RID: 10872 RVA: 0x000E4F3F File Offset: 0x000E313F
	public override void Stop()
	{
		this.moveAbility.Stop();
		this.idleSound.Stop();
		this.navMeshAgent.updateRotation = this.defaultUpdateRotation;
		this.navMeshAgent.isStopped = false;
	}

	// Token: 0x06002A79 RID: 10873 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002A7A RID: 10874 RVA: 0x000E4F74 File Offset: 0x000E3174
	public void SetTargetPlayer(NetPlayer targetPlayer)
	{
		this.target = null;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				this.target = grplayer.transform;
				this.moveAbility.SetLookAtTarget(this.target);
			}
		}
	}

	// Token: 0x06002A7B RID: 10875 RVA: 0x000E4FC8 File Offset: 0x000E31C8
	public override void Think(float dt)
	{
		Vector3 vector = this.agent.transform.position - this.target.position;
		if (this.moveAbility.IsDone())
		{
			if (this.maxDistanceFromTarget < 0f || vector.magnitude < this.maxDistanceFromTarget)
			{
				if (this.navMeshAgent.isOnNavMesh && this.navMeshAgent.isStopped)
				{
					this.idleSound.Stop();
					this.moveAbility.Start();
				}
				Vector3 targetPos = this.PickBackupDestination();
				this.moveAbility.SetTargetPos(targetPos);
				return;
			}
		}
		else if (this.maxDistanceFromTarget > 0f && vector.magnitude > this.maxDistanceFromTarget)
		{
			this.moveAbility.SetTargetPos(this.root.position);
			this.moveAbility.Stop();
			this.navMeshAgent.isStopped = true;
			this.PlayAnim(this.idleAnimName, 0.5f, 1f);
			this.idleSound.Play(null);
		}
	}

	// Token: 0x06002A7C RID: 10876 RVA: 0x000E50D4 File Offset: 0x000E32D4
	private Vector3 PickBackupDestination()
	{
		Vector3 position = this.agent.transform.position;
		if (this.target == null)
		{
			return position;
		}
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(position, ref navMeshHit, 1f, this.walkableArea))
		{
			Vector3 position2 = navMeshHit.position;
			Vector3 vector = this.agent.transform.position - this.target.position;
			vector.y = 0f;
			Vector3 normalized = vector.normalized;
			int i = 0;
			while (i < GRAbilityKeepDistance.rotations.Length)
			{
				Vector3 vector2 = GRAbilityKeepDistance.rotations[i] * normalized;
				float num = 2f;
				Vector3 vector3 = position2 + vector2 * num;
				NavMeshHit navMeshHit2;
				if (!NavMesh.Raycast(position2, vector3, ref navMeshHit2, this.walkableArea))
				{
					goto IL_D6;
				}
				if (navMeshHit2.distance >= this.minBackupSpaceRequired)
				{
					vector3 = navMeshHit2.position;
					goto IL_D6;
				}
				IL_128:
				i++;
				continue;
				IL_D6:
				NavMeshHit navMeshHit3;
				if (!NavMesh.SamplePosition(vector3, ref navMeshHit3, 1f, this.walkableArea))
				{
					goto IL_128;
				}
				Vector3 position3 = navMeshHit3.position;
				Vector3 vector4 = position3 - this.target.position;
				vector4.y = 0f;
				if (vector4.sqrMagnitude > vector.sqrMagnitude)
				{
					return position3;
				}
				goto IL_128;
			}
		}
		return position;
	}

	// Token: 0x06002A7D RID: 10877 RVA: 0x000E521E File Offset: 0x000E341E
	protected override void UpdateShared(float dt)
	{
		this.moveAbility.Update(dt);
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.magenta, true);
		}
	}

	// Token: 0x040036C2 RID: 14018
	private NavMeshAgent navMeshAgent;

	// Token: 0x040036C3 RID: 14019
	private Transform target;

	// Token: 0x040036C4 RID: 14020
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x040036C5 RID: 14021
	public string idleAnimName;

	// Token: 0x040036C6 RID: 14022
	public AbilitySound idleSound;

	// Token: 0x040036C7 RID: 14023
	public float minBackupSpaceRequired = 0.5f;

	// Token: 0x040036C8 RID: 14024
	public float maxDistanceFromTarget = -1f;

	// Token: 0x040036C9 RID: 14025
	private bool defaultUpdateRotation;

	// Token: 0x040036CA RID: 14026
	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 30f, 0f),
		Quaternion.Euler(0f, -30f, 0f),
		Quaternion.Euler(0f, 60f, 0f),
		Quaternion.Euler(0f, -60f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};
}
