using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020006BC RID: 1724
public class GREnemySummoner : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameEntityDebugComponent, IGameAgentComponent, IGRSummoningEntity
{
	// Token: 0x06002C55 RID: 11349 RVA: 0x000F0088 File Offset: 0x000EE288
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		this.trackedEntities = new List<int>();
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.agent = base.GetComponent<GameAgent>();
		this.entity = base.GetComponent<GameEntity>();
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform);
	}

	// Token: 0x06002C56 RID: 11350 RVA: 0x000F013C File Offset: 0x000EE33C
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, null, null, null);
		this.abilityWander.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilitySummon.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityKeepDistance.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityMoveToTarget.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetBehavior(GREnemySummoner.Behavior.Idle, true);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.SetHP(this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax));
		this.navAgent.speed = (float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.PatrolSpeed);
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemySummoner.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemySummoner.BodyState.Bones, true);
	}

	// Token: 0x06002C57 RID: 11351 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002C58 RID: 11352 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002C59 RID: 11353 RVA: 0x00002789 File Offset: 0x00000989
	private void OnDisable()
	{
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x00002789 File Offset: 0x00000989
	private void OnEnable()
	{
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x000F03F0 File Offset: 0x000EE5F0
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x000F0409 File Offset: 0x000EE609
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemySummoner.Behavior.Jump, false);
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x000F0423 File Offset: 0x000EE623
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 10)
		{
			return;
		}
		this.SetBehavior((GREnemySummoner.Behavior)newState, false);
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x000F0437 File Offset: 0x000EE637
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x000F0440 File Offset: 0x000EE640
	public bool TrySetBehavior(GREnemySummoner.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemySummoner.Behavior.Jump && newBehavior == GREnemySummoner.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x000F045C File Offset: 0x000EE65C
	public void SetBehavior(GREnemySummoner.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.Stop();
			break;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Stop();
			if (this.summonLight != null)
			{
				this.summonLight.gameObject.SetActive(false);
			}
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Stop();
			break;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Stop();
			break;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Start();
			this.soundWander.Play(this.audioSource);
			break;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemySummoner.Behavior.Destroyed:
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			this.abilityDie.Start();
			break;
		case GREnemySummoner.Behavior.Summon:
			if (this.summonLight != null)
			{
				this.summonLight.gameObject.SetActive(true);
			}
			this.lastSummonTime = Time.timeAsDouble;
			this.abilitySummon.SetLookAtTarget(this.GetPlayerTransform(this.agent.targetPlayer));
			this.abilitySummon.Start();
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.SetTargetPlayer(this.agent.targetPlayer);
			this.abilityKeepDistance.Start();
			break;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.SetTarget(this.GetPlayerTransform(this.agent.targetPlayer));
			this.abilityMoveToTarget.Start();
			break;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.Start();
			break;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		}
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x000F0720 File Offset: 0x000EE920
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06002C62 RID: 11362 RVA: 0x000F0730 File Offset: 0x000EE930
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.lastUpdateTime = Time.time;
		GREnemySummoner.tempRigs.Clear();
		GREnemySummoner.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemySummoner.tempRigs);
		this.senseNearby.UpdateNearby(GREnemySummoner.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemySummoner.Behavior.Stagger:
		case GREnemySummoner.Behavior.Destroyed:
			break;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Think(dt);
			if (this.abilitySummon.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Think(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Think(dt);
			this.ChooseNewBehavior();
			return;
		default:
			return;
		}
	}

	// Token: 0x06002C63 RID: 11363 RVA: 0x000F086C File Offset: 0x000EEA6C
	public bool CanSummon()
	{
		return !GhostReactorManager.AggroDisabled && (this.currBehavior != GREnemySummoner.Behavior.Summon || !this.abilitySummon.IsDone()) && Time.timeAsDouble - this.lastSummonTime >= (double)this.minSummonInterval && this.trackedEntities.Count < this.maxSimultaneousSummonedEntities;
	}

	// Token: 0x06002C64 RID: 11364 RVA: 0x000F08C4 File Offset: 0x000EEAC4
	public Transform GetPlayerTransform(NetPlayer targetPlayer)
	{
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				return grplayer.transform;
			}
		}
		return null;
	}

	// Token: 0x06002C65 RID: 11365 RVA: 0x000F08FC File Offset: 0x000EEAFC
	private void ChooseNewBehavior()
	{
		float num = 0f;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		if (!GhostReactorManager.AggroDisabled && vrrig != null)
		{
			this.investigateLocation = default(Vector3?);
			float num2 = (this.currBehavior == GREnemySummoner.Behavior.KeepDistance) ? (this.keepDistanceThreshold + 1f) : this.keepDistanceThreshold;
			if (num < num2 * num2)
			{
				this.SetBehavior(GREnemySummoner.Behavior.KeepDistance, false);
				return;
			}
			if (this.CanSummon())
			{
				this.SetBehavior(GREnemySummoner.Behavior.Summon, false);
				return;
			}
			float num3 = this.tooFarDistanceThreshold * this.tooFarDistanceThreshold;
			if (num > num3)
			{
				this.SetBehavior(GREnemySummoner.Behavior.MoveToTarget, false);
				return;
			}
			this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
			return;
		}
		else
		{
			this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
			if (this.investigateLocation != null)
			{
				this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
				this.SetBehavior(GREnemySummoner.Behavior.Investigate, false);
				return;
			}
			double num4 = Time.timeAsDouble - this.abilityIdle.startTime;
			if (this.currBehavior == GREnemySummoner.Behavior.Idle && num4 < (double)this.idleDuration)
			{
				this.SetBehavior(GREnemySummoner.Behavior.Idle, false);
				return;
			}
			this.SetBehavior(GREnemySummoner.Behavior.Wander, false);
			return;
		}
	}

	// Token: 0x06002C66 RID: 11366 RVA: 0x000F0A20 File Offset: 0x000EEC20
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002C67 RID: 11367 RVA: 0x000F0A40 File Offset: 0x000EEC40
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Idle:
			this.abilityIdle.Update(dt);
			return;
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.Update(dt);
			return;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				this.SetBehavior(GREnemySummoner.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.Update(dt);
			return;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.Update(dt);
			return;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.Update(dt);
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.Update(dt);
			return;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.Update(dt);
			return;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.Update(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.Update(dt);
			if (this.abilityFlashed.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002C68 RID: 11368 RVA: 0x000F0B44 File Offset: 0x000EED44
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemySummoner.Behavior.Wander:
			this.abilityWander.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Destroyed:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Summon:
			this.abilitySummon.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.KeepDistance:
			this.abilityKeepDistance.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.MoveToTarget:
			this.abilityMoveToTarget.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		case GREnemySummoner.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002C69 RID: 11369 RVA: 0x000F0BFC File Offset: 0x000EEDFC
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		byte b2 = (byte)this.currBodyState;
		writer.Write(b);
		writer.Write(this.hp);
		writer.Write(b2);
	}

	// Token: 0x06002C6A RID: 11370 RVA: 0x000F0C34 File Offset: 0x000EEE34
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemySummoner.Behavior newBehavior = (GREnemySummoner.Behavior)reader.ReadByte();
		int num = reader.ReadInt32();
		GREnemySummoner.BodyState newBodyState = (GREnemySummoner.BodyState)reader.ReadByte();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
	}

	// Token: 0x06002C6B RID: 11371 RVA: 0x00027DED File Offset: 0x00025FED
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002C6C RID: 11372 RVA: 0x000F0C70 File Offset: 0x000EEE70
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

	// Token: 0x06002C6D RID: 11373 RVA: 0x000F0CD4 File Offset: 0x000EEED4
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBehavior == GREnemySummoner.Behavior.Destroyed)
		{
			return;
		}
		if (this.currBodyState != GREnemySummoner.BodyState.Bones)
		{
			if (this.currBodyState == GREnemySummoner.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
		if (this.hp <= 0)
		{
			this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
			this.abilityDie.SetStaggerVelocity(hit.hitImpulse);
			this.SetBehavior(GREnemySummoner.Behavior.Destroyed, false);
			return;
		}
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemySummoner.Behavior.Stagger);
	}

	// Token: 0x06002C6E RID: 11374 RVA: 0x000F0D88 File Offset: 0x000EEF88
	public void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		this.abilityFlashed.SetStaggerVelocity(hit.hitImpulse);
		if (this.currBodyState == GREnemySummoner.BodyState.Shell)
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
				this.SetBodyState(GREnemySummoner.BodyState.Bones, false);
				if (tool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
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
		GRToolFlash component = tool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.TrySetBehavior(GREnemySummoner.Behavior.Flashed);
	}

	// Token: 0x06002C6F RID: 11375 RVA: 0x000F0EB0 File Offset: 0x000EF0B0
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x06002C70 RID: 11376 RVA: 0x000F0EBC File Offset: 0x000EF0BC
	private void OnTriggerEnter(Collider collider)
	{
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

	// Token: 0x06002C71 RID: 11377 RVA: 0x000F0FB4 File Offset: 0x000EF1B4
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemySummoner.BodyState.Destroyed:
			this.armor.SetHp(0);
			return;
		case GREnemySummoner.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, false);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		case GREnemySummoner.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, true);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002C72 RID: 11378 RVA: 0x000F1068 File Offset: 0x000EF268
	public void SetBodyState(GREnemySummoner.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemySummoner.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemySummoner.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemySummoner.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemySummoner.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemySummoner.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06002C73 RID: 11379 RVA: 0x000F1140 File Offset: 0x000EF340
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("Nearby rigs: <color=\"yellow\">{0}<color=\"white\">", this.senseNearby.rigsNearby.Count));
		strings.Add(string.Format("Spawned entities: <color=\"yellow\">{0}<color=\"white\">", this.trackedEntities.Count));
	}

	// Token: 0x06002C74 RID: 11380 RVA: 0x000F11C8 File Offset: 0x000EF3C8
	public void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
	}

	// Token: 0x06002C75 RID: 11381 RVA: 0x000F11E8 File Offset: 0x000EF3E8
	public void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
	}

	// Token: 0x06002C76 RID: 11382 RVA: 0x000F1217 File Offset: 0x000EF417
	public void OnSummonedEntityInit(GameEntity entity)
	{
		this.AddTrackedEntity(entity);
	}

	// Token: 0x06002C77 RID: 11383 RVA: 0x000F1220 File Offset: 0x000EF420
	public void OnSummonedEntityDestroy(GameEntity entity)
	{
		this.RemoveTrackedEntity(entity);
	}

	// Token: 0x0400397A RID: 14714
	private GameEntity entity;

	// Token: 0x0400397B RID: 14715
	private GameAgent agent;

	// Token: 0x0400397C RID: 14716
	public GRArmorEnemy armor;

	// Token: 0x0400397D RID: 14717
	public GRAttributes attributes;

	// Token: 0x0400397E RID: 14718
	public Animation anim;

	// Token: 0x0400397F RID: 14719
	public GRSenseNearby senseNearby;

	// Token: 0x04003980 RID: 14720
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003981 RID: 14721
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003982 RID: 14722
	public GRAbilityWander abilityWander;

	// Token: 0x04003983 RID: 14723
	public GRAbilityAttackJump abilityAttack;

	// Token: 0x04003984 RID: 14724
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003985 RID: 14725
	public GRAbilityDie abilityDie;

	// Token: 0x04003986 RID: 14726
	public GRAbilitySummon abilitySummon;

	// Token: 0x04003987 RID: 14727
	public GRAbilityKeepDistance abilityKeepDistance;

	// Token: 0x04003988 RID: 14728
	public GRAbilityMoveToTarget abilityMoveToTarget;

	// Token: 0x04003989 RID: 14729
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x0400398A RID: 14730
	public GRAbilityJump abilityJump;

	// Token: 0x0400398B RID: 14731
	public GRAbilityStagger abilityFlashed;

	// Token: 0x0400398C RID: 14732
	public AbilitySound soundWander;

	// Token: 0x0400398D RID: 14733
	public AbilitySound soundAttack;

	// Token: 0x0400398E RID: 14734
	public GameLight summonLight;

	// Token: 0x0400398F RID: 14735
	public List<Renderer> bones;

	// Token: 0x04003990 RID: 14736
	public List<Renderer> always;

	// Token: 0x04003991 RID: 14737
	public List<GameObject> bonesStateVisibleObjects;

	// Token: 0x04003992 RID: 14738
	public List<GameObject> alwaysVisibleObjects;

	// Token: 0x04003993 RID: 14739
	public Transform coreMarker;

	// Token: 0x04003994 RID: 14740
	public GRCollectible corePrefab;

	// Token: 0x04003995 RID: 14741
	public Transform headTransform;

	// Token: 0x04003996 RID: 14742
	public float attackRange = 2f;

	// Token: 0x04003997 RID: 14743
	public List<VRRig> rigsNearby;

	// Token: 0x04003998 RID: 14744
	public NavMeshAgent navAgent;

	// Token: 0x04003999 RID: 14745
	public AudioSource audioSource;

	// Token: 0x0400399A RID: 14746
	public float idleDuration = 2f;

	// Token: 0x0400399B RID: 14747
	public float keepDistanceThreshold = 3f;

	// Token: 0x0400399C RID: 14748
	public float tooFarDistanceThreshold = 5f;

	// Token: 0x0400399D RID: 14749
	public double lastSummonTime;

	// Token: 0x0400399E RID: 14750
	public float minSummonInterval = 4f;

	// Token: 0x0400399F RID: 14751
	public int maxSimultaneousSummonedEntities = 3;

	// Token: 0x040039A0 RID: 14752
	public float hearingRadius = 7f;

	// Token: 0x040039A1 RID: 14753
	[ReadOnly]
	public int hp;

	// Token: 0x040039A2 RID: 14754
	[ReadOnly]
	public GREnemySummoner.Behavior currBehavior;

	// Token: 0x040039A3 RID: 14755
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x040039A4 RID: 14756
	[ReadOnly]
	public GREnemySummoner.BodyState currBodyState;

	// Token: 0x040039A5 RID: 14757
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x040039A6 RID: 14758
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x040039A7 RID: 14759
	private Rigidbody rigidBody;

	// Token: 0x040039A8 RID: 14760
	private List<Collider> colliders;

	// Token: 0x040039A9 RID: 14761
	private List<int> trackedEntities;

	// Token: 0x040039AA RID: 14762
	private Vector3? investigateLocation;

	// Token: 0x040039AB RID: 14763
	private float lastUpdateTime;

	// Token: 0x040039AC RID: 14764
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x020006BD RID: 1725
	public enum Behavior
	{
		// Token: 0x040039AE RID: 14766
		Idle,
		// Token: 0x040039AF RID: 14767
		Wander,
		// Token: 0x040039B0 RID: 14768
		Stagger,
		// Token: 0x040039B1 RID: 14769
		Destroyed,
		// Token: 0x040039B2 RID: 14770
		Summon,
		// Token: 0x040039B3 RID: 14771
		KeepDistance,
		// Token: 0x040039B4 RID: 14772
		MoveToTarget,
		// Token: 0x040039B5 RID: 14773
		Investigate,
		// Token: 0x040039B6 RID: 14774
		Jump,
		// Token: 0x040039B7 RID: 14775
		Flashed,
		// Token: 0x040039B8 RID: 14776
		Count
	}

	// Token: 0x020006BE RID: 1726
	public enum BodyState
	{
		// Token: 0x040039BA RID: 14778
		Destroyed,
		// Token: 0x040039BB RID: 14779
		Bones,
		// Token: 0x040039BC RID: 14780
		Shell,
		// Token: 0x040039BD RID: 14781
		Count
	}
}
