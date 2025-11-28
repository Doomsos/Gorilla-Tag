using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000677 RID: 1655
[Serializable]
public class GRAbilityWander : GRAbilityBase
{
	// Token: 0x06002A55 RID: 10837 RVA: 0x000E462F File Offset: 0x000E282F
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002A56 RID: 10838 RVA: 0x000E4654 File Offset: 0x000E2854
	public override void Start()
	{
		base.Start();
		this.moveAbility.Start();
		Vector3 targetPos = this.PickRandomDestination();
		this.moveAbility.SetTargetPos(targetPos);
	}

	// Token: 0x06002A57 RID: 10839 RVA: 0x000E4685 File Offset: 0x000E2885
	public override void Stop()
	{
		this.moveAbility.Stop();
	}

	// Token: 0x06002A58 RID: 10840 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002A59 RID: 10841 RVA: 0x000E4694 File Offset: 0x000E2894
	public override void Think(float dt)
	{
		if (this.moveAbility.IsDone())
		{
			Vector3 targetPos = this.PickRandomDestination();
			this.moveAbility.SetTargetPos(targetPos);
		}
	}

	// Token: 0x06002A5A RID: 10842 RVA: 0x000E46C4 File Offset: 0x000E28C4
	private Vector3 PickRandomDestination()
	{
		Vector3 position = this.agent.transform.position;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(position, ref navMeshHit, 1f, this.walkableArea))
		{
			Vector3 position2 = navMeshHit.position;
			Vector3 forward = this.agent.transform.forward;
			float num = 0f;
			Vector3 vector = position2;
			for (int i = 0; i < GRAbilityWander.rotations.Length; i++)
			{
				Vector3 vector2 = GRAbilityWander.rotations[i] * forward;
				float num2 = 8f;
				if (NavMesh.Raycast(position2, position2 + vector2 * num2, ref navMeshHit, this.walkableArea))
				{
					num2 = navMeshHit.distance * 0.95f;
				}
				float num3 = num2 * GRAbilityWander.rotationWeight[i];
				if (num3 > num)
				{
					num = num3;
					vector = position2 + vector2 * num2;
				}
			}
			if (NavMesh.SamplePosition(vector, ref navMeshHit, 1f, this.walkableArea))
			{
				position = navMeshHit.position;
			}
		}
		return position;
	}

	// Token: 0x06002A5B RID: 10843 RVA: 0x000E47C6 File Offset: 0x000E29C6
	protected override void UpdateShared(float dt)
	{
		this.moveAbility.Update(dt);
	}

	// Token: 0x040036A7 RID: 13991
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x040036A8 RID: 13992
	private static Quaternion[] rotations = new Quaternion[]
	{
		Quaternion.Euler(0f, 0f, 0f),
		Quaternion.Euler(0f, 45f, 0f),
		Quaternion.Euler(0f, -45f, 0f),
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, -90f, 0f),
		Quaternion.Euler(0f, 135f, 0f),
		Quaternion.Euler(0f, -135f, 0f),
		Quaternion.Euler(0f, 180f, 0f)
	};

	// Token: 0x040036A9 RID: 13993
	private static float[] rotationWeight = new float[]
	{
		1f,
		0.75f,
		0.75f,
		0.5f,
		0.5f,
		0.2f,
		0.2f,
		0.2f
	};
}
