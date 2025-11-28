using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020006B2 RID: 1714
public class GREnemyPest : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent, ITickSystemTick
{
	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x06002BDE RID: 11230 RVA: 0x000EC238 File Offset: 0x000EA438
	// (set) Token: 0x06002BDF RID: 11231 RVA: 0x000EC240 File Offset: 0x000EA440
	public bool TickRunning { get; set; }

	// Token: 0x06002BE0 RID: 11232 RVA: 0x000EC24C File Offset: 0x000EA44C
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
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform);
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		base.Invoke("PlaySpawnAudio", 0.1f);
	}

	// Token: 0x06002BE1 RID: 11233 RVA: 0x0001877F File Offset: 0x0001697F
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06002BE2 RID: 11234 RVA: 0x00018787 File Offset: 0x00016987
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06002BE3 RID: 11235 RVA: 0x000EC338 File Offset: 0x000EA538
	private void PlaySpawnAudio()
	{
		this.spawnSound.Play(null);
	}

	// Token: 0x06002BE4 RID: 11236 RVA: 0x000EC348 File Offset: 0x000EA548
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAttack.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityWander.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityGrabbed.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityThrown.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.SetBehavior(GREnemyPest.Behavior.Wander, false);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry entry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(entry);
			}
		}
		this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		this.SetHP(this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax));
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyPest.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyPest.BodyState.Bones, true);
	}

	// Token: 0x06002BE5 RID: 11237 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002BE6 RID: 11238 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002BE7 RID: 11239 RVA: 0x000EC694 File Offset: 0x000EA894
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002BE8 RID: 11240 RVA: 0x000EC6AD File Offset: 0x000EA8AD
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyPest.Behavior.Jump, false);
	}

	// Token: 0x06002BE9 RID: 11241 RVA: 0x000EC6C8 File Offset: 0x000EA8C8
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyPest.Behavior)newState, false);
	}

	// Token: 0x06002BEA RID: 11242 RVA: 0x000EC6DC File Offset: 0x000EA8DC
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06002BEB RID: 11243 RVA: 0x000EC6E5 File Offset: 0x000EA8E5
	public bool TrySetBehavior(GREnemyPest.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyPest.Behavior.Jump && newBehavior == GREnemyPest.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06002BEC RID: 11244 RVA: 0x000EC700 File Offset: 0x000EA900
	public void SetBehavior(GREnemyPest.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Stop();
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyPest.Behavior.Grabbed:
			this.abilityGrabbed.Stop();
			break;
		case GREnemyPest.Behavior.Thrown:
			this.abilityThrown.Stop();
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Stop();
			break;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Start();
			break;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.Start();
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Start();
			this.abilityAttack.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyPest.Behavior.Grabbed:
			this.abilityGrabbed.Start();
			break;
		case GREnemyPest.Behavior.Thrown:
			this.abilityThrown.Start();
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Start();
			break;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.Start();
			break;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		}
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06002BED RID: 11245 RVA: 0x000EC916 File Offset: 0x000EAB16
	private void OnGrabbed()
	{
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Grabbed, false);
	}

	// Token: 0x06002BEE RID: 11246 RVA: 0x000EC92A File Offset: 0x000EAB2A
	private void OnReleased()
	{
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Thrown, false);
	}

	// Token: 0x06002BEF RID: 11247 RVA: 0x000EC93E File Offset: 0x000EAB3E
	public void Tick()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06002BF0 RID: 11248 RVA: 0x000EC94C File Offset: 0x000EAB4C
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyPest.tempRigs.Clear();
		GREnemyPest.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyPest.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyPest.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		GREnemyPest.Behavior behavior = this.currBehavior;
		switch (behavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.ChooseNewBehavior();
			return;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyPest.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			return;
		default:
			if (behavior != GREnemyPest.Behavior.Investigate)
			{
				return;
			}
			this.abilityInvestigate.Think(dt);
			this.ChooseNewBehavior();
			return;
		}
	}

	// Token: 0x06002BF1 RID: 11249 RVA: 0x000ECA4C File Offset: 0x000EAC4C
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			this.investigateLocation = default(Vector3?);
			this.SetBehavior(GREnemyPest.Behavior.Chase, false);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyPest.Behavior.Investigate, false);
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Wander, false);
	}

	// Token: 0x06002BF2 RID: 11250 RVA: 0x000ECAD8 File Offset: 0x000EACD8
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002BF3 RID: 11251 RVA: 0x000ECAF8 File Offset: 0x000EACF8
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Update(dt);
			return;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Update(dt);
			return;
		case GREnemyPest.Behavior.Chase:
		{
			this.abilityChase.Update(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyPest.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Update(dt);
			if (this.abilityAttack.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Grabbed:
			break;
		case GREnemyPest.Behavior.Thrown:
			if (this.abilityThrown.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Update(dt);
			return;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.Update(dt);
			return;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.Update(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyPest.Behavior.Flashed:
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

	// Token: 0x06002BF4 RID: 11252 RVA: 0x000ECC94 File Offset: 0x000EAE94
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Grabbed:
		case GREnemyPest.Behavior.Thrown:
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002BF5 RID: 11253 RVA: 0x000ECD40 File Offset: 0x000EAF40
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		byte b2 = (byte)this.currBodyState;
		writer.Write(b);
		writer.Write(this.hp);
		writer.Write(b2);
	}

	// Token: 0x06002BF6 RID: 11254 RVA: 0x000ECD78 File Offset: 0x000EAF78
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyPest.Behavior newBehavior = (GREnemyPest.Behavior)reader.ReadByte();
		int num = reader.ReadInt32();
		GREnemyPest.BodyState newBodyState = (GREnemyPest.BodyState)reader.ReadByte();
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
	}

	// Token: 0x06002BF7 RID: 11255 RVA: 0x00027DED File Offset: 0x00025FED
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x000ECDB4 File Offset: 0x000EAFB4
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(hit);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06002BF9 RID: 11257 RVA: 0x000ECE14 File Offset: 0x000EB014
	public void OnHitByClub(GameHitData hit)
	{
		if (this.currBodyState != GREnemyPest.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyPest.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.hp -= hit.hitAmount;
		if (this.hp <= 0)
		{
			this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
			this.SetBehavior(GREnemyPest.Behavior.Destroyed, false);
			return;
		}
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyPest.Behavior.Stagger);
	}

	// Token: 0x06002BFA RID: 11258 RVA: 0x000ECEB8 File Offset: 0x000EB0B8
	public void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		this.abilityFlashed.SetStaggerVelocity(hit.hitImpulse);
		if (this.currBodyState == GREnemyPest.BodyState.Shell)
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
				this.SetBodyState(GREnemyPest.BodyState.Bones, false);
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
		this.TrySetBehavior(GREnemyPest.Behavior.Flashed);
	}

	// Token: 0x06002BFB RID: 11259 RVA: 0x000ECFE0 File Offset: 0x000EB1E0
	public void OnHitByShield(GameHitData hit)
	{
		this.OnHitByClub(hit);
	}

	// Token: 0x06002BFC RID: 11260 RVA: 0x000ECFEC File Offset: 0x000EB1EC
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBehavior != GREnemyPest.Behavior.Attack)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			Vector3 enemyAttackDirection = this.abilityAttack.targetPos - this.abilityAttack.initialPos;
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(base.transform.position, enemyAttackDirection, component2);
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

	// Token: 0x06002BFD RID: 11261 RVA: 0x000ED151 File Offset: 0x000EB351
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (this.currBehavior == GREnemyPest.Behavior.Attack && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position);
		}
		yield break;
	}

	// Token: 0x06002BFE RID: 11262 RVA: 0x000ED168 File Offset: 0x000EB368
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyPest.BodyState.Destroyed:
			this.armor.SetHp(0);
			return;
		case GREnemyPest.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, false);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		case GREnemyPest.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, true);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002BFF RID: 11263 RVA: 0x000ED1EC File Offset: 0x000EB3EC
	public void SetBodyState(GREnemyPest.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyPest.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyPest.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyPest.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyPest.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyPest.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06002C00 RID: 11264 RVA: 0x000ED2C4 File Offset: 0x000EB4C4
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		float magnitude = (GRSenseNearby.GetRigTestLocation(VRRig.LocalRig) - base.transform.position).magnitude;
		bool flag = GRSenseLineOfSight.HasGeoLineOfSight(this.headTransform.position, GRSenseNearby.GetRigTestLocation(VRRig.LocalRig), this.senseLineOfSight.sightDist, this.senseLineOfSight.visibilityMask);
		strings.Add(string.Format("player rig dis: {0} has los: {1}", magnitude, flag));
	}

	// Token: 0x04003890 RID: 14480
	public GameEntity entity;

	// Token: 0x04003891 RID: 14481
	public GameAgent agent;

	// Token: 0x04003892 RID: 14482
	public GRArmorEnemy armor;

	// Token: 0x04003893 RID: 14483
	public GRAttributes attributes;

	// Token: 0x04003894 RID: 14484
	public Animation anim;

	// Token: 0x04003895 RID: 14485
	public GRSenseNearby senseNearby;

	// Token: 0x04003896 RID: 14486
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003897 RID: 14487
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003898 RID: 14488
	public GRAbilityChase abilityChase;

	// Token: 0x04003899 RID: 14489
	public GRAbilityWander abilityWander;

	// Token: 0x0400389A RID: 14490
	public GRAbilityAttackJump abilityAttack;

	// Token: 0x0400389B RID: 14491
	public GRAbilityStagger abilityStagger;

	// Token: 0x0400389C RID: 14492
	public GRAbilityStagger abilityFlashed;

	// Token: 0x0400389D RID: 14493
	public GRAbilityDie abilityDie;

	// Token: 0x0400389E RID: 14494
	public GRAbilityGrabbed abilityGrabbed;

	// Token: 0x0400389F RID: 14495
	public GRAbilityThrown abilityThrown;

	// Token: 0x040038A0 RID: 14496
	public AbilitySound spawnSound;

	// Token: 0x040038A1 RID: 14497
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x040038A2 RID: 14498
	public GRAbilityJump abilityJump;

	// Token: 0x040038A3 RID: 14499
	public List<GameObject> bonesStateVisibleObjects;

	// Token: 0x040038A4 RID: 14500
	public List<GameObject> alwaysVisibleObjects;

	// Token: 0x040038A5 RID: 14501
	public Transform coreMarker;

	// Token: 0x040038A6 RID: 14502
	public GRCollectible corePrefab;

	// Token: 0x040038A7 RID: 14503
	public Transform headTransform;

	// Token: 0x040038A8 RID: 14504
	public float attackRange = 2f;

	// Token: 0x040038A9 RID: 14505
	public List<VRRig> rigsNearby;

	// Token: 0x040038AA RID: 14506
	public NavMeshAgent navAgent;

	// Token: 0x040038AB RID: 14507
	public AudioSource audioSource;

	// Token: 0x040038AC RID: 14508
	public float hearingRadius = 5f;

	// Token: 0x040038AD RID: 14509
	private Vector3? investigateLocation;

	// Token: 0x040038AF RID: 14511
	[ReadOnly]
	public int hp;

	// Token: 0x040038B0 RID: 14512
	[ReadOnly]
	public GREnemyPest.Behavior currBehavior;

	// Token: 0x040038B1 RID: 14513
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x040038B2 RID: 14514
	[ReadOnly]
	public GREnemyPest.BodyState currBodyState;

	// Token: 0x040038B3 RID: 14515
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x040038B4 RID: 14516
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x040038B5 RID: 14517
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x040038B6 RID: 14518
	private Rigidbody rigidBody;

	// Token: 0x040038B7 RID: 14519
	private List<Collider> colliders;

	// Token: 0x040038B8 RID: 14520
	private float lastHitPlayerTime;

	// Token: 0x040038B9 RID: 14521
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x040038BA RID: 14522
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x040038BB RID: 14523
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x020006B3 RID: 1715
	public enum Behavior
	{
		// Token: 0x040038BD RID: 14525
		Idle,
		// Token: 0x040038BE RID: 14526
		Wander,
		// Token: 0x040038BF RID: 14527
		Chase,
		// Token: 0x040038C0 RID: 14528
		Attack,
		// Token: 0x040038C1 RID: 14529
		Stagger,
		// Token: 0x040038C2 RID: 14530
		Grabbed,
		// Token: 0x040038C3 RID: 14531
		Thrown,
		// Token: 0x040038C4 RID: 14532
		Destroyed,
		// Token: 0x040038C5 RID: 14533
		Investigate,
		// Token: 0x040038C6 RID: 14534
		Jump,
		// Token: 0x040038C7 RID: 14535
		Flashed,
		// Token: 0x040038C8 RID: 14536
		Count
	}

	// Token: 0x020006B4 RID: 1716
	public enum BodyState
	{
		// Token: 0x040038CA RID: 14538
		Destroyed,
		// Token: 0x040038CB RID: 14539
		Bones,
		// Token: 0x040038CC RID: 14540
		Shell,
		// Token: 0x040038CD RID: 14541
		Count
	}
}
