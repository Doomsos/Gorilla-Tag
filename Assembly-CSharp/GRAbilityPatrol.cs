using System;
using CjLib;
using Photon.Pun;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200067D RID: 1661
[Serializable]
public class GRAbilityPatrol : GRAbilityBase
{
	// Token: 0x06002A80 RID: 10880 RVA: 0x000E539C File Offset: 0x000E359C
	public bool HasValidPatrolPath()
	{
		return this.patrolPath != null && this.patrolPath.patrolNodes.Count > 1;
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x000E53C4 File Offset: 0x000E35C4
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
		this.moveAbility.Setup(agent, anim, audioSource, root, head, lineOfSight);
		if (this.attributes && this.moveAbility.moveSpeed == 0f)
		{
			this.moveAbility.moveSpeed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		}
		this.navMeshAgent = agent.GetComponent<NavMeshAgent>();
		this.InitializeRandoms();
		this.nextPatrolNode = 0;
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x000E5444 File Offset: 0x000E3644
	private void InitializeRandoms()
	{
		this.patrolGroanSoundDelayRandom = new Random((uint)this.entity.GetNetId());
		this.patrolGroanSoundRandom = new Random((uint)this.entity.GetNetId());
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x000E5474 File Offset: 0x000E3674
	public override void Start()
	{
		base.Start();
		this.moveAbility.Start();
		this.agent.SetIsPathing(true, true);
		if (this.patrolPath != null)
		{
			this.moveAbility.SetTarget(this.patrolPath.patrolNodes[this.nextPatrolNode]);
		}
		else
		{
			Debug.LogError("Starting patrol ability with no patrol path");
		}
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x000E54E0 File Offset: 0x000E36E0
	public override void Stop()
	{
		this.moveAbility.Stop();
	}

	// Token: 0x06002A85 RID: 10885 RVA: 0x00002076 File Offset: 0x00000276
	public override bool IsDone()
	{
		return false;
	}

	// Token: 0x06002A86 RID: 10886 RVA: 0x000E54ED File Offset: 0x000E36ED
	public void SetPatrolPath(GRPatrolPath patrolPath)
	{
		this.patrolPath = patrolPath;
	}

	// Token: 0x06002A87 RID: 10887 RVA: 0x000E54F6 File Offset: 0x000E36F6
	public GRPatrolPath GetPatrolPath()
	{
		return this.patrolPath;
	}

	// Token: 0x06002A88 RID: 10888 RVA: 0x000E54FE File Offset: 0x000E36FE
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x06002A89 RID: 10889 RVA: 0x000E5507 File Offset: 0x000E3707
	public void CalculateNextPatrolGroan()
	{
		this.nextPatrolGroanTime = this.patrolGroanSoundDelayRandom.NextDouble(this.ambientSoundDelayMin, this.ambientSoundDelayMax) + PhotonNetwork.Time;
	}

	// Token: 0x06002A8A RID: 10890 RVA: 0x000E552C File Offset: 0x000E372C
	private void PlayPatrolGroan()
	{
		this.audioSource.clip = this.ambientPatrolSounds[this.patrolGroanSoundRandom.NextInt(this.ambientPatrolSounds.Length - 1)];
		this.audioSource.volume = this.ambientSoundVolume;
		this.audioSource.Play();
		this.CalculateNextPatrolGroan();
	}

	// Token: 0x06002A8B RID: 10891 RVA: 0x000E5584 File Offset: 0x000E3784
	public override void Update(float dt)
	{
		this.moveAbility.Update(dt);
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.green, true);
		}
		if (this.moveAbility.IsDone())
		{
			this.nextPatrolNode = (this.nextPatrolNode + 1) % this.patrolPath.patrolNodes.Count;
			this.moveAbility.SetTarget(this.patrolPath.patrolNodes[this.nextPatrolNode]);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x06002A8C RID: 10892 RVA: 0x000E5628 File Offset: 0x000E3828
	public override void UpdateRemote(float dt)
	{
		this.moveAbility.SetTarget(null);
		this.moveAbility.SetTargetPos(this.agent.navAgent.destination);
		this.moveAbility.Update(dt);
		if (GhostReactorManager.entityDebugEnabled)
		{
			DebugUtil.DrawLine(this.root.position, this.moveAbility.GetTargetPos(), Color.green, true);
		}
		if (PhotonNetwork.Time >= this.nextPatrolGroanTime)
		{
			this.PlayPatrolGroan();
		}
	}

	// Token: 0x040036CB RID: 14027
	private NavMeshAgent navMeshAgent;

	// Token: 0x040036CC RID: 14028
	public GRAbilityMoveToTarget moveAbility;

	// Token: 0x040036CD RID: 14029
	private GRPatrolPath patrolPath;

	// Token: 0x040036CE RID: 14030
	public double lastStateChange;

	// Token: 0x040036CF RID: 14031
	public float ambientSoundVolume = 0.5f;

	// Token: 0x040036D0 RID: 14032
	public double ambientSoundDelayMin = 5.0;

	// Token: 0x040036D1 RID: 14033
	public double ambientSoundDelayMax = 10.0;

	// Token: 0x040036D2 RID: 14034
	public AudioClip[] ambientPatrolSounds;

	// Token: 0x040036D3 RID: 14035
	private double lastPartrolAmbientSoundTime;

	// Token: 0x040036D4 RID: 14036
	private double nextPatrolGroanTime;

	// Token: 0x040036D5 RID: 14037
	private Random patrolGroanSoundDelayRandom;

	// Token: 0x040036D6 RID: 14038
	private Random patrolGroanSoundRandom;

	// Token: 0x040036D7 RID: 14039
	[ReadOnly]
	public int nextPatrolNode;
}
