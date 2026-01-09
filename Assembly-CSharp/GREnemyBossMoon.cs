using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

public class GREnemyBossMoon : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		if (this.navAgent != null)
		{
			this.navAgent.updateRotation = false;
		}
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.abilities = new GRAbilityBase[15];
	}

	public void OnEntityInit()
	{
		this.currAbility = null;
		this.SetupAbility(GREnemyBossMoon.Behavior.Idle, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Chase, this.abilityChase, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.SetupAbility(GREnemyBossMoon.Behavior.Search, this.abilitySearch, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle00, this.abilityAttackTentacle00, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle01, this.abilityAttackTentacle01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle02, this.abilityAttackTentacle02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle03, this.abilityAttackTentacle03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle04, this.abilityAttackTentacle04, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Attack, this.abilityAttackLaser, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackDisco, this.abilityAttackDiscoWander, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackSlamdown, this.abilityAttackSlamdown, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Patrol, this.abilityPatrol, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Stagger, this.abilityStagger, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Dying, this.abilityDie, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Flashed, this.abilityFlashed, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.senseNearby.Setup(this.headTransform);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		if (this.agent.navAgent != null)
		{
			this.agent.navAgent.autoTraverseOffMeshLink = false;
		}
		this.SetBehavior(GREnemyBossMoon.Behavior.Idle, true);
	}

	private void SetupAbility(GREnemyBossMoon.Behavior behavior, GRAbilityBase ability, GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.abilities[(int)behavior] = ability;
		ability.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	public void OnEntityDestroy()
	{
	}

	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	public void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyBossMoon.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyBossMoon.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyBossMoon.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyBossMoon.BodyState.Bones, true);
	}

	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 15)
		{
			return;
		}
		this.SetBehavior((GREnemyBossMoon.Behavior)newState, false);
	}

	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyBossMoon.BodyState)newState, false);
	}

	public void SetPatrolPath(long entityCreateData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData);
		this.abilityPatrol.SetPatrolPath(grpatrolPath);
	}

	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	public bool TrySetBehavior(GREnemyBossMoon.Behavior newBehavior)
	{
		if (newBehavior == GREnemyBossMoon.Behavior.Stagger)
		{
			return false;
		}
		if (newBehavior == GREnemyBossMoon.Behavior.Stagger && Time.time < this.lastStaggerTime + this.staggerImmuneTime)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	public void SetBehavior(GREnemyBossMoon.Behavior newBehavior, bool force = false)
	{
		if (newBehavior < GREnemyBossMoon.Behavior.Idle || newBehavior >= (GREnemyBossMoon.Behavior)this.abilities.Length)
		{
			Debug.LogErrorFormat("New Behavior Index is invalid {0} {1} {2}", new object[]
			{
				(int)newBehavior,
				newBehavior,
				base.gameObject.name
			});
			return;
		}
		GRAbilityBase grabilityBase = this.abilities[(int)newBehavior];
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		if (this.currAbility != null)
		{
			this.currAbility.Stop();
		}
		this.currBehavior = newBehavior;
		this.currAbility = grabilityBase;
		if (this.currAbility != null)
		{
			this.currAbility.Start();
		}
		GREnemyBossMoon.Behavior behavior = this.currBehavior;
		if (behavior != GREnemyBossMoon.Behavior.Stagger)
		{
			if (behavior != GREnemyBossMoon.Behavior.Chase)
			{
				if (behavior == GREnemyBossMoon.Behavior.Attack)
				{
					this.abilityAttackLaser.SetTargetPlayer(this.agent.targetPlayer);
				}
			}
			else
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
		}
		else
		{
			this.lastStaggerTime = Time.time;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	private int CalcMaxHP()
	{
		float difficultyScalingForCurrentFloor = this.entity.manager.ghostReactorManager.reactor.difficultyScalingForCurrentFloor;
		return (int)((float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax) * difficultyScalingForCurrentFloor);
	}

	public void SetBodyState(GREnemyBossMoon.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyBossMoon.BodyState.Bones:
			this.hp = this.CalcMaxHP();
			break;
		case GREnemyBossMoon.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyBossMoon.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyBossMoon.BodyState.Bones:
			this.hp = this.CalcMaxHP();
			break;
		case GREnemyBossMoon.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyBossMoon.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyBossMoon.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyBossMoon.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyBossMoon.tempRigs.Clear();
		GREnemyBossMoon.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyBossMoon.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyBossMoon.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		if (this.currAbility != null)
		{
			this.currAbility.Think(dt);
		}
		switch (this.currBehavior)
		{
		case GREnemyBossMoon.Behavior.Idle:
		case GREnemyBossMoon.Behavior.Patrol:
		case GREnemyBossMoon.Behavior.Search:
			this.ChooseNewBehavior();
			return;
		case GREnemyBossMoon.Behavior.Stagger:
		case GREnemyBossMoon.Behavior.Dying:
			break;
		case GREnemyBossMoon.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			this.ChooseNewBehavior();
			break;
		default:
			return;
		}
	}

	private bool TryChooseAttackBehavior(float toPlayerDistSq)
	{
		bool flag = this.senseNearby.IsAnyoneNearby(this.abilityAttackTentacle00.GetRange());
		if (flag && this.abilityAttackTentacle00.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyBossMoon.Behavior.AttackTentacle00, false);
			return true;
		}
		if (flag && this.abilityAttackTentacle01.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyBossMoon.Behavior.AttackTentacle01, false);
			return true;
		}
		if (flag && this.abilityAttackTentacle02.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyBossMoon.Behavior.AttackTentacle02, false);
			return true;
		}
		if (flag && this.abilityAttackTentacle03.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyBossMoon.Behavior.AttackTentacle03, false);
			return true;
		}
		if (flag && this.abilityAttackTentacle04.IsCoolDownOver())
		{
			this.SetBehavior(GREnemyBossMoon.Behavior.AttackTentacle04, false);
			return true;
		}
		return false;
	}

	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			if (this.agent.targetPlayer != null)
			{
				float magnitude = (GRPlayer.Get(this.agent.targetPlayer).transform.position - base.transform.position).magnitude;
				if (this.TryChooseAttackBehavior(magnitude * magnitude))
				{
					return;
				}
			}
			if (!this.abilityAttackLaser.IsCoolDownOver())
			{
				this.TrySetBehavior(GREnemyBossMoon.Behavior.Idle);
				return;
			}
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Chase);
			return;
		}
		else
		{
			if (this.abilityPatrol.HasValidPatrolPath())
			{
				this.SetBehavior(GREnemyBossMoon.Behavior.Patrol, false);
				return;
			}
			this.SetBehavior(GREnemyBossMoon.Behavior.Idle, false);
			return;
		}
	}

	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	public void OnUpdateAuthority(float dt)
	{
		if (this.currAbility != null)
		{
			this.currAbility.UpdateAuthority(dt);
			if (this.currAbility.IsDone() && this.currAbility.IsDone())
			{
				this.ChooseNewBehavior();
			}
		}
		if (this.currBehavior == GREnemyBossMoon.Behavior.Chase && !this.abilityChase.IsDone())
		{
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float sqrMagnitude = (grplayer.transform.position - base.transform.position).sqrMagnitude;
				this.TryChooseAttackBehavior(sqrMagnitude);
			}
		}
	}

	public void OnUpdateRemote(float dt)
	{
		if (this.currAbility != null)
		{
			this.currAbility.UpdateRemote(dt);
		}
	}

	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Bones)
		{
			this.hp -= hit.hitAmount;
			if (this.damagedSounds.Count > 0)
			{
				this.damagedSoundIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.damagedSounds.Count, this.damagedSoundIndex);
				this.audioSource.PlayOneShot(this.damagedSounds[this.damagedSoundIndex], this.damagedSoundVolume);
			}
			if (this.fxDamaged != null)
			{
				this.fxDamaged.SetActive(false);
				this.fxDamaged.SetActive(true);
			}
			if (this.hp <= 0)
			{
				this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
				this.SetBodyState(GREnemyBossMoon.BodyState.Destroyed, false);
				this.SetBehavior(GREnemyBossMoon.Behavior.Dying, false);
				return;
			}
			this.lastSeenTargetPosition = tool.transform.position;
			this.lastSeenTargetTime = Time.timeAsDouble;
			Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
			vector.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
			if (this.allowStagger)
			{
				this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
				this.TrySetBehavior(GREnemyBossMoon.Behavior.Stagger);
				return;
			}
		}
		else if (this.currBodyState == GREnemyBossMoon.BodyState.Shell && this.armor != null)
		{
			this.armor.PlayBlockFx(hit.hitEntityPosition);
		}
	}

	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Shell)
		{
			this.hp -= hit.hitAmount;
			if (this.armor != null)
			{
				this.armor.SetHp(this.hp);
			}
			if (this.hp <= 0)
			{
				if (this.armor != null)
				{
					this.armor.PlayDestroyFx(this.armor.transform.position);
				}
				this.SetBodyState(GREnemyBossMoon.BodyState.Bones, false);
				if (grTool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (grTool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
			}
			else if (grTool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = grTool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.RefreshBody();
			}
			else
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.RefreshBody();
			}
		}
		GRToolFlash component = grTool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.TrySetBehavior(GREnemyBossMoon.Behavior.Flashed);
	}

	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyBossMoon.Behavior.Attack && this.currBehavior != GREnemyBossMoon.Behavior.AttackDisco && this.currBehavior != GREnemyBossMoon.Behavior.AttackSlamdown)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(this.headTransform.position, base.transform.forward, component2);
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component3 = attachedRigidbody.GetComponent<GRPlayer>();
			if (component3 != null && component3.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				if (this.tryHitPlayerCoroutine != null)
				{
					base.StopCoroutine(this.tryHitPlayerCoroutine);
				}
				this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(component3));
			}
			GRBreakable component4 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component5 = attachedRigidbody.GetComponent<GameHittable>();
			if (component4 != null && component5 != null)
			{
				GameHitData hitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component5.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component4.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component4.transform.position
				};
				component5.RequestHit(hitData);
			}
		}
	}

	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if ((this.currBehavior == GREnemyBossMoon.Behavior.Attack || this.currBehavior == GREnemyBossMoon.Behavior.AttackDisco || this.currBehavior == GREnemyBossMoon.Behavior.AttackSlamdown) && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			Vector3 hitImpulse = player.transform.position - base.transform.position;
			hitImpulse.y = 0f;
			hitImpulse = hitImpulse.normalized * 6f;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position, hitImpulse);
		}
		yield break;
	}

	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		float num = (this.navAgent == null) ? 0f : this.navAgent.speed;
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", num, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
	}

	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte value = (byte)this.currBehavior;
		byte value2 = (byte)this.currBodyState;
		byte value3 = (byte)this.abilityPatrol.nextPatrolNode;
		int value4 = (this.targetPlayer == null) ? -1 : this.targetPlayer.ActorNumber;
		writer.Write(value);
		writer.Write(value2);
		writer.Write(this.hp);
		writer.Write(value3);
		writer.Write(value4);
	}

	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyBossMoon.Behavior newBehavior = (GREnemyBossMoon.Behavior)reader.ReadByte();
		GREnemyBossMoon.BodyState newBodyState = (GREnemyBossMoon.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte nextPatrolNode = reader.ReadByte();
		int playerID = reader.ReadInt32();
		this.SetPatrolPath(this.entity.createData);
		this.abilityPatrol.SetNextPatrolNode((int)nextPatrolNode);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
	}

	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(gameComponent, hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			default:
				return;
			}
		}
	}

	public GameEntity entity;

	public GameAgent agent;

	public GREnemy enemy;

	public GRArmorEnemy armor;

	public GameHittable hittable;

	[SerializeField]
	private GRAttributes attributes;

	public GRSenseNearby senseNearby;

	public GRSenseLineOfSight senseLineOfSight;

	public Animation anim;

	public GRAbilityAttackSimple abilityAttackTentacle00;

	public GRAbilityAttackSimple abilityAttackTentacle01;

	public GRAbilityAttackSimple abilityAttackTentacle02;

	public GRAbilityAttackSimple abilityAttackTentacle03;

	public GRAbilityAttackSimple abilityAttackTentacle04;

	public GRAbilityAttackSimple abilityAttackTongue01;

	private GRAbilityBase[] abilities;

	private GRAbilityBase currAbility;

	public List<GREnemyBossMoon.Eye> eyes;

	public GRAbilityAgent abilityAgent;

	public GRAbilityIdle abilityIdle;

	public GRAbilityChase abilityChase;

	public GRAbilityIdle abilitySearch;

	public GRAbilityAttackLaser abilityAttackLaser;

	public GRAbilityAttackSimpleWander abilityAttackDiscoWander;

	public GRAbilityAttackSimple abilityAttackSlamdown;

	public bool allowStagger;

	public GRAbilityStagger abilityStagger;

	public GRAbilityDie abilityDie;

	public GRAbilityPatrol abilityPatrol;

	public GRAbilityFlashed abilityFlashed;

	public List<Renderer> bones;

	public List<Renderer> always;

	public Transform coreMarker;

	public GRCollectible corePrefab;

	public Transform headTransform;

	public float turnSpeed = 540f;

	public SoundBankPlayer chaseSoundBank;

	public float attackRange = 1.5f;

	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	public NavMeshAgent navAgent;

	public AudioSource audioSource;

	public AudioClip damagedSound;

	public float damagedSoundVolume;

	public List<AudioClip> damagedSounds;

	private int damagedSoundIndex;

	public GameObject fxDamaged;

	private float lastStaggerTime;

	public float staggerImmuneTime = 10f;

	private Transform target;

	[ReadOnly]
	public int hp;

	[ReadOnly]
	public GREnemyBossMoon.Behavior currBehavior;

	[ReadOnly]
	public GREnemyBossMoon.BodyState currBodyState;

	[ReadOnly]
	public NetPlayer targetPlayer;

	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	[ReadOnly]
	public double lastSeenTargetTime;

	[ReadOnly]
	public Vector3 searchPosition;

	private double lastJumpEndtime;

	public bool canChaseJump = true;

	public float chaseJumpDistance = 5f;

	public float chaseJumpMinInterval = 1f;

	public float minChaseJumpDistance = 2f;

	private Rigidbody rigidBody;

	private List<Collider> colliders;

	private float lastHitPlayerTime;

	private float minTimeBetweenHits = 0.5f;

	public float hearingRadius = 5f;

	private static List<VRRig> tempRigs = new List<VRRig>(16);

	private Coroutine tryHitPlayerCoroutine;

	[Serializable]
	public class Eye
	{
	}

	public enum Behavior
	{
		Idle,
		Patrol,
		Stagger,
		Dying,
		Chase,
		Search,
		AttackTentacle00,
		AttackTentacle01,
		AttackTentacle02,
		AttackTentacle03,
		AttackTentacle04,
		Attack,
		AttackDisco,
		AttackSlamdown,
		Flashed,
		Count
	}

	public enum BodyState
	{
		Destroyed,
		Bones,
		Shell,
		Count
	}
}
