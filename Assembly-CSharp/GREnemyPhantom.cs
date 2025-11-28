using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020006B6 RID: 1718
public class GREnemyPhantom : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x06002C09 RID: 11273 RVA: 0x000ED468 File Offset: 0x000EB668
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform);
	}

	// Token: 0x06002C0A RID: 11274 RVA: 0x000ED510 File Offset: 0x000EB710
	public void OnEntityInit()
	{
		this.abilityMine.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityRage.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAlert.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityReturn.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAttack.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		int num = (int)this.entity.createData;
		this.Setup((long)num);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
	}

	// Token: 0x06002C0B RID: 11275 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002C0C RID: 11276 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002C0D RID: 11277 RVA: 0x000ED7D8 File Offset: 0x000EB9D8
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002C0E RID: 11278 RVA: 0x000ED808 File Offset: 0x000EBA08
	private void Setup(long createData)
	{
		this.SetPatrolPath(createData);
		if (this.patrolPath != null && this.patrolPath.patrolNodes.Count > 0)
		{
			this.nextPatrolNode = 0;
			this.target = this.patrolPath.patrolNodes[0];
			this.idleLocation = this.target;
			this.SetBehavior(GREnemyPhantom.Behavior.Return, true);
		}
		else
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Mine, true);
		}
		this.SetBodyState(GREnemyPhantom.BodyState.Bones, true);
		if (this.attackLight != null)
		{
			this.attackLight.gameObject.SetActive(false);
		}
		if (this.negativeLight != null)
		{
			this.negativeLight.gameObject.SetActive(false);
		}
		GREnemy.HideRenderers(this.bones, false);
		GREnemy.HideRenderers(this.always, false);
	}

	// Token: 0x06002C0F RID: 11279 RVA: 0x000ED8D7 File Offset: 0x000EBAD7
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyPhantom.Behavior.Jump, false);
	}

	// Token: 0x06002C10 RID: 11280 RVA: 0x000ED8F1 File Offset: 0x000EBAF1
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 9)
		{
			return;
		}
		this.SetBehavior((GREnemyPhantom.Behavior)newState, false);
	}

	// Token: 0x06002C11 RID: 11281 RVA: 0x000ED905 File Offset: 0x000EBB05
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 2)
		{
			return;
		}
		this.SetBodyState((GREnemyPhantom.BodyState)newState, false);
	}

	// Token: 0x06002C12 RID: 11282 RVA: 0x000ED918 File Offset: 0x000EBB18
	public void SetPatrolPath(long createData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(createData);
		this.patrolPath = grpatrolPath;
	}

	// Token: 0x06002C13 RID: 11283 RVA: 0x000ED943 File Offset: 0x000EBB43
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x06002C14 RID: 11284 RVA: 0x000ED94C File Offset: 0x000EBB4C
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x000ED958 File Offset: 0x000EBB58
	public void SetBehavior(GREnemyPhantom.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		this.lastStateChange = PhotonNetwork.Time;
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Stop();
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyPhantom.Behavior.Alert:
			this.abilityAlert.Stop();
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Stop();
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Stop();
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.Stop();
			if (this.negativeLight != null)
			{
				this.negativeLight.gameObject.SetActive(false);
			}
			break;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Stop();
			if (this.attackLight != null)
			{
				this.attackLight.gameObject.SetActive(false);
			}
			break;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Start();
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemyPhantom.Behavior.Alert:
			this.abilityAlert.Start();
			this.soundAlert.Play(this.audioSource);
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Start();
			this.soundReturn.Play(this.audioSource);
			this.abilityReturn.SetTarget(this.idleLocation);
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Start();
			this.soundRage.Play(this.audioSource);
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.Start();
			this.soundChase.Play(this.audioSource);
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			this.investigateLocation = default(Vector3?);
			if (this.negativeLight != null)
			{
				this.negativeLight.gameObject.SetActive(true);
			}
			break;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Start();
			this.abilityAttack.SetTargetPlayer(this.agent.targetPlayer);
			this.investigateLocation = default(Vector3?);
			this.soundAttack.Play(this.audioSource);
			if (this.attackLight != null)
			{
				this.attackLight.gameObject.SetActive(true);
			}
			break;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x000EDC40 File Offset: 0x000EBE40
	public void SetBodyState(GREnemyPhantom.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		if (this.currBodyState == GREnemyPhantom.BodyState.Bones)
		{
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
		}
		this.currBodyState = newBodyState;
		if (this.currBodyState == GREnemyPhantom.BodyState.Bones)
		{
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06002C17 RID: 11287 RVA: 0x000EDCBC File Offset: 0x000EBEBC
	private void RefreshBody()
	{
		GREnemyPhantom.BodyState bodyState = this.currBodyState;
		if (bodyState == GREnemyPhantom.BodyState.Destroyed)
		{
			this.armor.SetHp(0);
			return;
		}
		if (bodyState != GREnemyPhantom.BodyState.Bones)
		{
			return;
		}
		this.armor.SetHp(0);
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x000EDCF1 File Offset: 0x000EBEF1
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x000EDD00 File Offset: 0x000EBF00
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			this.investigateLocation = default(Vector3?);
			this.SetBehavior(GREnemyPhantom.Behavior.Alert, false);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyPhantom.Behavior.Investigate, false);
			return;
		}
		if (this.currBehavior == GREnemyPhantom.Behavior.Investigate)
		{
			if (this.idleLocation != null)
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
				return;
			}
			this.SetBehavior(GREnemyPhantom.Behavior.Idle, false);
		}
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x000EDDAC File Offset: 0x000EBFAC
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyPhantom.tempRigs.Clear();
		GREnemyPhantom.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyPhantom.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyPhantom.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.ChooseNewBehavior();
			return;
		case GREnemyPhantom.Behavior.Idle:
			this.ChooseNewBehavior();
			return;
		case GREnemyPhantom.Behavior.Alert:
		case GREnemyPhantom.Behavior.Rage:
		case GREnemyPhantom.Behavior.Attack:
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.SetTarget(this.idleLocation);
			this.abilityReturn.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyPhantom.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			return;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Think(dt);
			this.ChooseNewBehavior();
			break;
		default:
			return;
		}
	}

	// Token: 0x06002C1B RID: 11291 RVA: 0x000EDED2 File Offset: 0x000EC0D2
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x000EDEF0 File Offset: 0x000EC0F0
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Update(dt);
			if (this.idleLocation != null)
			{
				GameAgent.UpdateFacingDir(base.transform, this.agent.navAgent, this.idleLocation.forward, 180f);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Update(dt);
			return;
		case GREnemyPhantom.Behavior.Alert:
			this.UpdateAlert(dt);
			return;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Update(dt);
			if (this.abilityReturn.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Mine, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Update(dt);
			if (this.abilityRage.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Chase:
		{
			this.abilityChase.Update(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyPhantom.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Update(dt);
			if (this.abilityAttack.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Update(dt);
			return;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.Update(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002C1D RID: 11293 RVA: 0x000EE0A0 File Offset: 0x000EC2A0
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Rage:
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002C1E RID: 11294 RVA: 0x000EE118 File Offset: 0x000EC318
	public void UpdateAlert(float dt)
	{
		this.abilityAlert.SetTargetPlayer(this.agent.targetPlayer);
		this.abilityAlert.Update(dt);
		double timeAsDouble = Time.timeAsDouble;
		if (!this.senseNearby.IsAnyoneNearby())
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
			return;
		}
		float num;
		if (this.abilityAlert.IsDone() && this.senseNearby.PickClosest(out num) != null)
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Rage, false);
		}
	}

	// Token: 0x06002C1F RID: 11295 RVA: 0x000EE190 File Offset: 0x000EC390
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyPhantom.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyPhantom.Behavior.Attack)
		{
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
			if (component != null && component.gamePlayer.IsLocal())
			{
				GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Phantom, this.entity.id, component, base.transform.position);
			}
			GRBreakable component2 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component3 = attachedRigidbody.GetComponent<GameHittable>();
			if (component2 != null && component3 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component3.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component2.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component2.transform.position
				};
				component3.RequestHit(hitData);
			}
		}
	}

	// Token: 0x06002C20 RID: 11296 RVA: 0x000EE298 File Offset: 0x000EC498
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
	}

	// Token: 0x06002C21 RID: 11297 RVA: 0x000EE2D0 File Offset: 0x000EC4D0
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		byte b2 = (byte)this.currBodyState;
		byte b3 = (byte)this.nextPatrolNode;
		writer.Write(b);
		writer.Write(b2);
		writer.Write(this.hp);
		writer.Write(b3);
	}

	// Token: 0x06002C22 RID: 11298 RVA: 0x000EE318 File Offset: 0x000EC518
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyPhantom.Behavior newBehavior = (GREnemyPhantom.Behavior)reader.ReadByte();
		GREnemyPhantom.BodyState newBodyState = (GREnemyPhantom.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte b = reader.ReadByte();
		this.SetPatrolPath(this.entity.createData);
		this.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
	}

	// Token: 0x040038D2 RID: 14546
	public GameEntity entity;

	// Token: 0x040038D3 RID: 14547
	public GameAgent agent;

	// Token: 0x040038D4 RID: 14548
	public GRArmorEnemy armor;

	// Token: 0x040038D5 RID: 14549
	public GRAttributes attributes;

	// Token: 0x040038D6 RID: 14550
	public Animation anim;

	// Token: 0x040038D7 RID: 14551
	public GRSenseNearby senseNearby;

	// Token: 0x040038D8 RID: 14552
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x040038D9 RID: 14553
	public GRAbilityIdle abilityMine;

	// Token: 0x040038DA RID: 14554
	public AbilitySound soundMine;

	// Token: 0x040038DB RID: 14555
	public GRAbilityIdle abilityIdle;

	// Token: 0x040038DC RID: 14556
	public GRAbilityWatch abilityRage;

	// Token: 0x040038DD RID: 14557
	public AbilitySound soundRage;

	// Token: 0x040038DE RID: 14558
	public GRAbilityWatch abilityAlert;

	// Token: 0x040038DF RID: 14559
	public AbilitySound soundAlert;

	// Token: 0x040038E0 RID: 14560
	public GRAbilityChase abilityChase;

	// Token: 0x040038E1 RID: 14561
	public AbilitySound soundChase;

	// Token: 0x040038E2 RID: 14562
	public GRAbilityMoveToTarget abilityReturn;

	// Token: 0x040038E3 RID: 14563
	public AbilitySound soundReturn;

	// Token: 0x040038E4 RID: 14564
	public GRAbilityAttackLatchOn abilityAttack;

	// Token: 0x040038E5 RID: 14565
	public AbilitySound soundAttack;

	// Token: 0x040038E6 RID: 14566
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x040038E7 RID: 14567
	public GRAbilityJump abilityJump;

	// Token: 0x040038E8 RID: 14568
	public List<Renderer> bones;

	// Token: 0x040038E9 RID: 14569
	public List<Renderer> always;

	// Token: 0x040038EA RID: 14570
	public Transform coreMarker;

	// Token: 0x040038EB RID: 14571
	public GRCollectible corePrefab;

	// Token: 0x040038EC RID: 14572
	public Transform headTransform;

	// Token: 0x040038ED RID: 14573
	public float attackRange = 2f;

	// Token: 0x040038EE RID: 14574
	public float hearingRadius = 7f;

	// Token: 0x040038EF RID: 14575
	public List<VRRig> rigsNearby;

	// Token: 0x040038F0 RID: 14576
	public GameLight attackLight;

	// Token: 0x040038F1 RID: 14577
	public GameLight negativeLight;

	// Token: 0x040038F2 RID: 14578
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x040038F3 RID: 14579
	private Transform idleLocation;

	// Token: 0x040038F4 RID: 14580
	public NavMeshAgent navAgent;

	// Token: 0x040038F5 RID: 14581
	public AudioSource audioSource;

	// Token: 0x040038F6 RID: 14582
	public double lastStateChange;

	// Token: 0x040038F7 RID: 14583
	private Vector3? investigateLocation;

	// Token: 0x040038F8 RID: 14584
	private Transform target;

	// Token: 0x040038F9 RID: 14585
	[ReadOnly]
	public int hp;

	// Token: 0x040038FA RID: 14586
	[ReadOnly]
	public GREnemyPhantom.Behavior currBehavior;

	// Token: 0x040038FB RID: 14587
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x040038FC RID: 14588
	[ReadOnly]
	public GREnemyPhantom.BodyState currBodyState;

	// Token: 0x040038FD RID: 14589
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x040038FE RID: 14590
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x040038FF RID: 14591
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x04003900 RID: 14592
	private Rigidbody rigidBody;

	// Token: 0x04003901 RID: 14593
	private List<Collider> colliders;

	// Token: 0x04003902 RID: 14594
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x020006B7 RID: 1719
	public enum Behavior
	{
		// Token: 0x04003904 RID: 14596
		Mine,
		// Token: 0x04003905 RID: 14597
		Idle,
		// Token: 0x04003906 RID: 14598
		Alert,
		// Token: 0x04003907 RID: 14599
		Return,
		// Token: 0x04003908 RID: 14600
		Rage,
		// Token: 0x04003909 RID: 14601
		Chase,
		// Token: 0x0400390A RID: 14602
		Attack,
		// Token: 0x0400390B RID: 14603
		Investigate,
		// Token: 0x0400390C RID: 14604
		Jump,
		// Token: 0x0400390D RID: 14605
		Count
	}

	// Token: 0x020006B8 RID: 1720
	public enum BodyState
	{
		// Token: 0x0400390F RID: 14607
		Destroyed,
		// Token: 0x04003910 RID: 14608
		Bones,
		// Token: 0x04003911 RID: 14609
		Count
	}
}
