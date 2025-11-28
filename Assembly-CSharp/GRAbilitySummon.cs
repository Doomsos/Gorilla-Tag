using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200067A RID: 1658
[Serializable]
public class GRAbilitySummon : GRAbilityBase
{
	// Token: 0x06002A6A RID: 10858 RVA: 0x000E49CE File Offset: 0x000E2BCE
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002A6B RID: 10859 RVA: 0x000E49E0 File Offset: 0x000E2BE0
	public override void Start()
	{
		base.Start();
		this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
		this.duration = this.animData[this.lastAnimIndex].duration;
		this.chargeTime = this.animData[this.lastAnimIndex].eventTime;
		this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animSpeed);
		this.state = GRAbilitySummon.State.Charge;
		this.summonSound.Play(this.audioSource);
		this.spawnedCount = 0;
		this.agent.navAgent.isStopped = true;
		this.agent.navAgent.speed = 1f;
		if (this.fxStartSummon != null)
		{
			this.fxStartSummon.SetActive(false);
			this.fxStartSummon.SetActive(true);
		}
	}

	// Token: 0x06002A6C RID: 10860 RVA: 0x000E4ADA File Offset: 0x000E2CDA
	public override void Stop()
	{
		this.lookAtTarget = null;
		this.agent.navAgent.isStopped = false;
	}

	// Token: 0x06002A6D RID: 10861 RVA: 0x000E4AF4 File Offset: 0x000E2CF4
	public void SetLookAtTarget(Transform transform)
	{
		this.lookAtTarget = transform;
	}

	// Token: 0x06002A6E RID: 10862 RVA: 0x000E4AFD File Offset: 0x000E2CFD
	public override void Think(float dt)
	{
		this.UpdateState(dt);
	}

	// Token: 0x06002A6F RID: 10863 RVA: 0x000E4B06 File Offset: 0x000E2D06
	protected override void UpdateShared(float dt)
	{
		if (this.lookAtTarget != null)
		{
			GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.lookAtTarget, 360f);
		}
	}

	// Token: 0x06002A70 RID: 10864 RVA: 0x000E4B38 File Offset: 0x000E2D38
	private void UpdateState(float dt)
	{
		double num = Time.timeAsDouble - this.startTime;
		switch (this.state)
		{
		case GRAbilitySummon.State.Charge:
			if (num > (double)this.chargeTime)
			{
				this.SetState(GRAbilitySummon.State.Spawn);
				return;
			}
			break;
		case GRAbilitySummon.State.Spawn:
			if (!this.spawned)
			{
				this.spawned = this.DoSpawn();
			}
			if (this.spawned && num > (double)this.duration)
			{
				this.SetState(GRAbilitySummon.State.Done);
				this.spawned = false;
			}
			break;
		case GRAbilitySummon.State.Done:
			break;
		default:
			return;
		}
	}

	// Token: 0x06002A71 RID: 10865 RVA: 0x000E4BB2 File Offset: 0x000E2DB2
	private void SetState(GRAbilitySummon.State newState)
	{
		GRAbilitySummon.State state = this.state;
		this.state = newState;
		switch (newState)
		{
		default:
			return;
		}
	}

	// Token: 0x06002A72 RID: 10866 RVA: 0x000E4BD4 File Offset: 0x000E2DD4
	private Vector3? GetSpawnLocation()
	{
		Vector3 position = this.root.position;
		float num = Random.Range(-this.summonConeAngle / 2f, this.summonConeAngle / 2f);
		int i = 0;
		while (i < 5)
		{
			Vector3 vector = Quaternion.Euler(0f, num, 0f) * this.root.forward;
			Vector3 vector2 = position + vector * this.desiredSpawnDistance;
			NavMeshHit navMeshHit;
			if (NavMesh.Raycast(position, vector2, ref navMeshHit, this.walkableArea))
			{
				if (navMeshHit.distance < this.minSpawnDistance)
				{
					num += 15f;
					if (num > this.summonConeAngle / 2f)
					{
						this.summonConeAngle = -this.summonConeAngle / 2f;
					}
					i++;
					continue;
				}
				vector2 = navMeshHit.position + Vector3.up * this.spawnHeight;
			}
			return new Vector3?(vector2);
		}
		return default(Vector3?);
	}

	// Token: 0x06002A73 RID: 10867 RVA: 0x000E4CD0 File Offset: 0x000E2ED0
	private bool DoSpawn()
	{
		Vector3? spawnLocation = this.GetSpawnLocation();
		if (spawnLocation != null)
		{
			if (this.entity.IsAuthority())
			{
				Quaternion identity = Quaternion.identity;
				GhostReactorManager.Get(this.entity).gameEntityManager.RequestCreateItem(this.entityPrefabToSpawn.name.GetStaticHash(), spawnLocation.Value, identity, (long)this.entity.GetNetId());
				this.spawnedCount++;
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.summonSpawnAudioClip);
			}
			if (this.fxOnSpawn != null)
			{
				this.fxOnSpawn.SetActive(false);
				this.fxOnSpawn.SetActive(true);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06002A74 RID: 10868 RVA: 0x000E4D93 File Offset: 0x000E2F93
	public override bool IsDone()
	{
		return this.state == GRAbilitySummon.State.Done;
	}

	// Token: 0x040036AC RID: 13996
	private int lastAnimIndex = -1;

	// Token: 0x040036AD RID: 13997
	public GameEntity entityPrefabToSpawn;

	// Token: 0x040036AE RID: 13998
	public List<AnimationData> animData;

	// Token: 0x040036AF RID: 13999
	private float animSpeed = 1f;

	// Token: 0x040036B0 RID: 14000
	public float chargeTime = 3f;

	// Token: 0x040036B1 RID: 14001
	public float duration = 3f;

	// Token: 0x040036B2 RID: 14002
	public float desiredSpawnDistance = 3f;

	// Token: 0x040036B3 RID: 14003
	public float minSpawnDistance = 1f;

	// Token: 0x040036B4 RID: 14004
	public float spawnHeight = 1f;

	// Token: 0x040036B5 RID: 14005
	public float summonConeAngle = 120f;

	// Token: 0x040036B6 RID: 14006
	private bool spawned;

	// Token: 0x040036B7 RID: 14007
	public AudioClip summonSpawnAudioClip;

	// Token: 0x040036B8 RID: 14008
	public GameObject fxStartSummon;

	// Token: 0x040036B9 RID: 14009
	public GameObject fxOnSpawn;

	// Token: 0x040036BA RID: 14010
	public AbilitySound summonSound;

	// Token: 0x040036BB RID: 14011
	private int spawnedCount;

	// Token: 0x040036BC RID: 14012
	public Transform lookAtTarget;

	// Token: 0x040036BD RID: 14013
	private GRAbilitySummon.State state;

	// Token: 0x0200067B RID: 1659
	private enum State
	{
		// Token: 0x040036BF RID: 14015
		Charge,
		// Token: 0x040036C0 RID: 14016
		Spawn,
		// Token: 0x040036C1 RID: 14017
		Done
	}
}
