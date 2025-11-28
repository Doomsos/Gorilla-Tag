using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020006AD RID: 1709
public class GREnemyChaser : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x06002BB4 RID: 11188 RVA: 0x000EAB68 File Offset: 0x000E8D68
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.visibilityLayerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002BB5 RID: 11189 RVA: 0x000EAC1C File Offset: 0x000E8E1C
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilitySearch.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityAttackSwipe.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.senseNearby.Setup(this.headTransform);
		this.InitializeRandoms();
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
	}

	// Token: 0x06002BB6 RID: 11190 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002BB7 RID: 11191 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002BB8 RID: 11192 RVA: 0x00002789 File Offset: 0x00000989
	private void InitializeRandoms()
	{
	}

	// Token: 0x06002BB9 RID: 11193 RVA: 0x000EAEA8 File Offset: 0x000E90A8
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002BBA RID: 11194 RVA: 0x000EAED8 File Offset: 0x000E90D8
	public void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyChaser.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyChaser.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyChaser.BodyState.Bones, true);
	}

	// Token: 0x06002BBB RID: 11195 RVA: 0x000EAF2B File Offset: 0x000E912B
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyChaser.Behavior.Jump, false);
	}

	// Token: 0x06002BBC RID: 11196 RVA: 0x000EAF46 File Offset: 0x000E9146
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 10)
		{
			return;
		}
		this.SetBehavior((GREnemyChaser.Behavior)newState, false);
	}

	// Token: 0x06002BBD RID: 11197 RVA: 0x000EAF5A File Offset: 0x000E915A
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyChaser.BodyState)newState, false);
	}

	// Token: 0x06002BBE RID: 11198 RVA: 0x000EAF70 File Offset: 0x000E9170
	public void SetPatrolPath(long entityCreateData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData);
		this.abilityPatrol.SetPatrolPath(grpatrolPath);
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x000EAFA0 File Offset: 0x000E91A0
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.abilityPatrol.SetNextPatrolNode(nextPatrolNode);
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x000EAFAE File Offset: 0x000E91AE
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x000EAFB7 File Offset: 0x000E91B7
	public bool TrySetBehavior(GREnemyChaser.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyChaser.Behavior.Jump && newBehavior == GREnemyChaser.Behavior.Stagger)
		{
			return false;
		}
		if (newBehavior == GREnemyChaser.Behavior.Stagger && Time.time < this.lastStaggerTime + this.staggerImmuneTime)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x000EAFEC File Offset: 0x000E91EC
	public void SetBehavior(GREnemyChaser.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyChaser.Behavior.Dying:
			this.behaviorEndTime = 1.0;
			this.abilityDie.Stop();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Stop();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Stop();
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.Stop();
			this.lastJumpEndtime = Time.timeAsDouble;
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.Start();
			break;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Start();
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyChaser.Behavior.Dying:
			this.PlayAnim("GREnemyChaserIdle", 0.1f, 1f);
			this.behaviorEndTime = 1.0;
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			this.abilityDie.Start();
			break;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.Start();
			this.investigateLocation = default(Vector3?);
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Start();
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Start();
			this.investigateLocation = default(Vector3?);
			this.abilityAttackSwipe.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06002BC3 RID: 11203 RVA: 0x000EB298 File Offset: 0x000E9498
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06002BC4 RID: 11204 RVA: 0x000EB2C8 File Offset: 0x000E94C8
	public void SetBodyState(GREnemyChaser.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyChaser.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyChaser.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06002BC5 RID: 11205 RVA: 0x000EB3A0 File Offset: 0x000E95A0
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyChaser.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyChaser.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyChaser.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002BC6 RID: 11206 RVA: 0x000EB43A File Offset: 0x000E963A
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06002BC7 RID: 11207 RVA: 0x000EB448 File Offset: 0x000E9648
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyChaser.tempRigs.Clear();
		GREnemyChaser.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyChaser.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyChaser.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
		case GREnemyChaser.Behavior.Patrol:
		case GREnemyChaser.Behavior.Investigate:
			this.ChooseNewBehavior();
			return;
		case GREnemyChaser.Behavior.Stagger:
		case GREnemyChaser.Behavior.Dying:
		case GREnemyChaser.Behavior.Attack:
		case GREnemyChaser.Behavior.Flashed:
			break;
		case GREnemyChaser.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemyChaser.Behavior.Search:
			this.ChooseNewBehavior();
			return;
		default:
			return;
		}
	}

	// Token: 0x06002BC8 RID: 11208 RVA: 0x000EB544 File Offset: 0x000E9744
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			if (this.agent.targetPlayer != null)
			{
				Vector3 position = GRPlayer.Get(this.agent.targetPlayer).transform.position;
				Vector3 vector = position - base.transform.position;
				float magnitude = vector.magnitude;
				if (magnitude < this.attackRange)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
				}
				else if (this.canChaseJump && Time.timeAsDouble - this.lastJumpEndtime > (double)this.chaseJumpMinInterval && magnitude > this.attackRange + this.minChaseJumpDistance && GRSenseLineOfSight.HasNavmeshLineOfSight(base.transform.position, position, 10f))
				{
					Vector3 vector2 = vector / magnitude;
					float num = Mathf.Clamp(this.chaseJumpDistance, this.minChaseJumpDistance, magnitude - this.attackRange * 0.5f);
					NavMeshHit navMeshHit;
					if (NavMesh.SamplePosition(base.transform.position + vector2 * num, ref navMeshHit, 0.5f, AbilityHelperFunctions.GetNavMeshWalkableArea()))
					{
						this.agent.GetGameAgentManager().RequestJump(this.agent, base.transform.position, navMeshHit.position, 0.25f, 1.5f);
						return;
					}
				}
			}
			this.TrySetBehavior(GREnemyChaser.Behavior.Chase);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyChaser.Behavior.Investigate, false);
			return;
		}
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyChaser.Behavior.Patrol, false);
			return;
		}
		this.SetBehavior(GREnemyChaser.Behavior.Idle, false);
	}

	// Token: 0x06002BC9 RID: 11209 RVA: 0x000EB715 File Offset: 0x000E9915
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002BCA RID: 11210 RVA: 0x000EB734 File Offset: 0x000E9934
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.Update(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.Update(dt);
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.agent.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.Update(dt);
			return;
		case GREnemyChaser.Behavior.Chase:
		{
			this.abilityChase.Update(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Search, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.Update(dt);
			if (this.abilitySearch.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.Update(dt);
			if (this.abilityAttackSwipe.IsDone())
			{
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.Update(dt);
			if (this.abilityFlashed.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyChaser.Behavior.Search, false);
					return;
				}
				this.SetBehavior(GREnemyChaser.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.Update(dt);
			if (this.abilityInvestigate.IsDone())
			{
				this.investigateLocation = default(Vector3?);
			}
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
				return;
			}
			break;
		case GREnemyChaser.Behavior.Jump:
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

	// Token: 0x06002BCB RID: 11211 RVA: 0x000EB948 File Offset: 0x000E9B48
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyChaser.Behavior.Idle:
			this.abilityIdle.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Search:
			this.abilitySearch.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Attack:
			this.abilityAttackSwipe.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyChaser.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x000EBA0C File Offset: 0x000E9C0C
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState != GREnemyChaser.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyChaser.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
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
			this.SetBodyState(GREnemyChaser.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyChaser.Behavior.Dying, false);
			return;
		}
		this.lastSeenTargetPosition = tool.transform.position;
		this.lastSeenTargetTime = Time.timeAsDouble;
		Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
		vector.y = 0f;
		this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyChaser.Behavior.Stagger);
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x000EBB84 File Offset: 0x000E9D84
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Shell)
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
				this.SetBodyState(GREnemyChaser.BodyState.Bones, false);
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
		this.TrySetBehavior(GREnemyChaser.Behavior.Flashed);
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x000EBD3E File Offset: 0x000E9F3E
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		Debug.Log(string.Format("Chaser On Hit By Shield dmg:{0} impulse:{1} size:{2}", hit.hitAmount, hit.hitImpulse, hit.hitImpulse.magnitude));
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x000EBD80 File Offset: 0x000E9F80
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyChaser.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyChaser.Behavior.Attack)
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

	// Token: 0x06002BD0 RID: 11216 RVA: 0x000EBED8 File Offset: 0x000EA0D8
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (this.currBehavior == GREnemyChaser.Behavior.Attack && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position);
		}
		yield break;
	}

	// Token: 0x06002BD1 RID: 11217 RVA: 0x000EBEF0 File Offset: 0x000EA0F0
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
	}

	// Token: 0x06002BD2 RID: 11218 RVA: 0x000EBF94 File Offset: 0x000EA194
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		byte b2 = (byte)this.currBodyState;
		byte b3 = (byte)this.abilityPatrol.nextPatrolNode;
		int num = (this.targetPlayer == null) ? -1 : this.targetPlayer.ActorNumber;
		writer.Write(b);
		writer.Write(b2);
		writer.Write(this.hp);
		writer.Write(b3);
		writer.Write(num);
	}

	// Token: 0x06002BD3 RID: 11219 RVA: 0x000EC000 File Offset: 0x000EA200
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyChaser.Behavior newBehavior = (GREnemyChaser.Behavior)reader.ReadByte();
		GREnemyChaser.BodyState newBodyState = (GREnemyChaser.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte nextPatrolNode = reader.ReadByte();
		int playerID = reader.ReadInt32();
		this.SetPatrolPath(this.entity.createData);
		this.SetNextPatrolNode((int)nextPatrolNode);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x00027DED File Offset: 0x00025FED
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002BD5 RID: 11221 RVA: 0x000EC074 File Offset: 0x000EA274
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

	// Token: 0x0400383C RID: 14396
	public GameEntity entity;

	// Token: 0x0400383D RID: 14397
	public GameAgent agent;

	// Token: 0x0400383E RID: 14398
	public GRArmorEnemy armor;

	// Token: 0x0400383F RID: 14399
	public GameHittable hittable;

	// Token: 0x04003840 RID: 14400
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003841 RID: 14401
	public GRSenseNearby senseNearby;

	// Token: 0x04003842 RID: 14402
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003843 RID: 14403
	public Animation anim;

	// Token: 0x04003844 RID: 14404
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003845 RID: 14405
	public GRAbilityChase abilityChase;

	// Token: 0x04003846 RID: 14406
	public GRAbilityIdle abilitySearch;

	// Token: 0x04003847 RID: 14407
	public GRAbilityAttackSwipe abilityAttackSwipe;

	// Token: 0x04003848 RID: 14408
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003849 RID: 14409
	public GRAbilityDie abilityDie;

	// Token: 0x0400384A RID: 14410
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x0400384B RID: 14411
	public GRAbilityPatrol abilityPatrol;

	// Token: 0x0400384C RID: 14412
	public GRAbilityFlashed abilityFlashed;

	// Token: 0x0400384D RID: 14413
	public GRAbilityJump abilityJump;

	// Token: 0x0400384E RID: 14414
	public List<Renderer> bones;

	// Token: 0x0400384F RID: 14415
	public List<Renderer> always;

	// Token: 0x04003850 RID: 14416
	public Transform coreMarker;

	// Token: 0x04003851 RID: 14417
	public GRCollectible corePrefab;

	// Token: 0x04003852 RID: 14418
	public Transform headTransform;

	// Token: 0x04003853 RID: 14419
	public float turnSpeed = 540f;

	// Token: 0x04003854 RID: 14420
	public SoundBankPlayer chaseSoundBank;

	// Token: 0x04003855 RID: 14421
	public float attackRange = 1.5f;

	// Token: 0x04003856 RID: 14422
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04003857 RID: 14423
	public NavMeshAgent navAgent;

	// Token: 0x04003858 RID: 14424
	public AudioSource audioSource;

	// Token: 0x04003859 RID: 14425
	public AudioClip damagedSound;

	// Token: 0x0400385A RID: 14426
	public float damagedSoundVolume;

	// Token: 0x0400385B RID: 14427
	public List<AudioClip> damagedSounds;

	// Token: 0x0400385C RID: 14428
	private int damagedSoundIndex;

	// Token: 0x0400385D RID: 14429
	public GameObject fxDamaged;

	// Token: 0x0400385E RID: 14430
	private Vector3? investigateLocation;

	// Token: 0x0400385F RID: 14431
	private float lastStaggerTime;

	// Token: 0x04003860 RID: 14432
	public float staggerImmuneTime = 10f;

	// Token: 0x04003861 RID: 14433
	private Transform target;

	// Token: 0x04003862 RID: 14434
	[ReadOnly]
	public int hp;

	// Token: 0x04003863 RID: 14435
	[ReadOnly]
	public GREnemyChaser.Behavior currBehavior;

	// Token: 0x04003864 RID: 14436
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04003865 RID: 14437
	[ReadOnly]
	public GREnemyChaser.BodyState currBodyState;

	// Token: 0x04003866 RID: 14438
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003867 RID: 14439
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003868 RID: 14440
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003869 RID: 14441
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x0400386A RID: 14442
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x0400386B RID: 14443
	private double lastJumpEndtime;

	// Token: 0x0400386C RID: 14444
	public bool canChaseJump = true;

	// Token: 0x0400386D RID: 14445
	public float chaseJumpDistance = 5f;

	// Token: 0x0400386E RID: 14446
	public float chaseJumpMinInterval = 1f;

	// Token: 0x0400386F RID: 14447
	public float minChaseJumpDistance = 2f;

	// Token: 0x04003870 RID: 14448
	public static RaycastHit[] visibilityHits = new RaycastHit[16];

	// Token: 0x04003871 RID: 14449
	private LayerMask visibilityLayerMask;

	// Token: 0x04003872 RID: 14450
	private Rigidbody rigidBody;

	// Token: 0x04003873 RID: 14451
	private List<Collider> colliders;

	// Token: 0x04003874 RID: 14452
	private float lastHitPlayerTime;

	// Token: 0x04003875 RID: 14453
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003876 RID: 14454
	public float hearingRadius = 5f;

	// Token: 0x04003877 RID: 14455
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003878 RID: 14456
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x020006AE RID: 1710
	public enum Behavior
	{
		// Token: 0x0400387A RID: 14458
		Idle,
		// Token: 0x0400387B RID: 14459
		Patrol,
		// Token: 0x0400387C RID: 14460
		Stagger,
		// Token: 0x0400387D RID: 14461
		Dying,
		// Token: 0x0400387E RID: 14462
		Chase,
		// Token: 0x0400387F RID: 14463
		Search,
		// Token: 0x04003880 RID: 14464
		Attack,
		// Token: 0x04003881 RID: 14465
		Flashed,
		// Token: 0x04003882 RID: 14466
		Investigate,
		// Token: 0x04003883 RID: 14467
		Jump,
		// Token: 0x04003884 RID: 14468
		Count
	}

	// Token: 0x020006AF RID: 1711
	public enum BodyState
	{
		// Token: 0x04003886 RID: 14470
		Destroyed,
		// Token: 0x04003887 RID: 14471
		Bones,
		// Token: 0x04003888 RID: 14472
		Shell,
		// Token: 0x04003889 RID: 14473
		Count
	}
}
