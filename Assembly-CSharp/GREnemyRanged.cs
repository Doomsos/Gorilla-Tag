using System;
using System.Collections.Generic;
using System.IO;
using CjLib;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// Token: 0x020006B9 RID: 1721
public class GREnemyRanged : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameProjectileLauncher, IGameEntityDebugComponent
{
	// Token: 0x06002C25 RID: 11301 RVA: 0x000EE39C File Offset: 0x000EC59C
	private bool IsMoving()
	{
		return this.navAgent.velocity.sqrMagnitude > 0f;
	}

	// Token: 0x06002C26 RID: 11302 RVA: 0x000EE3C4 File Offset: 0x000EC5C4
	private void SoftResetThrowableHead()
	{
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
		this.headLightReset = true;
		this.spitterLightTurnOffTime = Time.timeAsDouble + this.spitterLightTurnOffDelay;
	}

	// Token: 0x06002C27 RID: 11303 RVA: 0x000EE430 File Offset: 0x000EC630
	private void ForceResetThrowableHead()
	{
		this.headRemoved = false;
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06002C28 RID: 11304 RVA: 0x000EE494 File Offset: 0x000EC694
	private void ForceHeadToDeadState()
	{
		this.headRemoved = false;
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06002C29 RID: 11305 RVA: 0x000EE4F8 File Offset: 0x000EC6F8
	private void EnableVFXForShoulderHead()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(true);
		this.spitterHeadOnShouldersLight.SetActive(true);
		this.spitterHeadOnShouldersVFX.SetActive(true);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06002C2A RID: 11306 RVA: 0x000EE554 File Offset: 0x000EC754
	private void EnableVFXForHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(true);
		this.spitterHeadInHandLight.SetActive(true);
		this.spitterHeadInHandVFX.SetActive(true);
	}

	// Token: 0x06002C2B RID: 11307 RVA: 0x000EE5B0 File Offset: 0x000EC7B0
	private void DisableHeadInHand()
	{
		this.headLightReset = false;
		this.spitterHeadInHand.SetActive(false);
	}

	// Token: 0x06002C2C RID: 11308 RVA: 0x000EE5C8 File Offset: 0x000EC7C8
	private void DisableHeadOnShoulderAndHeadInHand()
	{
		this.headLightReset = false;
		this.headRemoved = false;
		this.spitterHeadOnShoulders.SetActive(false);
		this.spitterHeadOnShouldersLight.SetActive(false);
		this.spitterHeadOnShouldersVFX.SetActive(false);
		this.spitterHeadInHand.SetActive(false);
		this.spitterHeadInHandLight.SetActive(false);
		this.spitterHeadInHandVFX.SetActive(false);
	}

	// Token: 0x06002C2D RID: 11309 RVA: 0x000EE62C File Offset: 0x000EC82C
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.visibilityLayerMask = LayerMask.GetMask(new string[]
		{
			"Default"
		});
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06002C2E RID: 11310 RVA: 0x000EE6D0 File Offset: 0x000EC8D0
	public void OnEntityInit()
	{
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityPatrol.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityKeepDistance.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
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

	// Token: 0x06002C2F RID: 11311 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002C30 RID: 11312 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002C31 RID: 11313 RVA: 0x000EE8D0 File Offset: 0x000ECAD0
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
		this.DestroyProjectile();
	}

	// Token: 0x06002C32 RID: 11314 RVA: 0x000EE908 File Offset: 0x000ECB08
	public void Setup(long entityCreateData)
	{
		this.SetPatrolPath(entityCreateData);
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, true);
		}
		else
		{
			this.SetBehavior(GREnemyRanged.Behavior.Idle, true);
		}
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyRanged.BodyState.Shell, true);
		}
		else
		{
			this.SetBodyState(GREnemyRanged.BodyState.Bones, true);
		}
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
	}

	// Token: 0x06002C33 RID: 11315 RVA: 0x000EE986 File Offset: 0x000ECB86
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyRanged.Behavior.Jump, false);
	}

	// Token: 0x06002C34 RID: 11316 RVA: 0x000EE9A1 File Offset: 0x000ECBA1
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyRanged.Behavior)newState, false);
	}

	// Token: 0x06002C35 RID: 11317 RVA: 0x000EE9B5 File Offset: 0x000ECBB5
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyRanged.BodyState)newState, false);
	}

	// Token: 0x06002C36 RID: 11318 RVA: 0x000EE9C8 File Offset: 0x000ECBC8
	public void SetPatrolPath(long entityCreateData)
	{
		this.abilityPatrol.SetPatrolPath(GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(entityCreateData));
	}

	// Token: 0x06002C37 RID: 11319 RVA: 0x000EE9EB File Offset: 0x000ECBEB
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06002C38 RID: 11320 RVA: 0x000EE9F4 File Offset: 0x000ECBF4
	public bool TrySetBehavior(GREnemyRanged.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyRanged.Behavior.Jump && newBehavior == GREnemyRanged.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06002C39 RID: 11321 RVA: 0x000EEA10 File Offset: 0x000ECC10
	public void SetBehavior(GREnemyRanged.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Patrol:
			this.abilityPatrol.Stop();
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Stop();
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			if (newBehavior != GREnemyRanged.Behavior.RangedAttack)
			{
				this.SoftResetThrowableHead();
			}
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			if (newBehavior != GREnemyRanged.Behavior.RangedAttackCooldown)
			{
				this.ForceResetThrowableHead();
			}
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.ForceResetThrowableHead();
			this.abilityKeepDistance.Stop();
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Idle:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedIdleSearch", 0.1f, 1f);
			break;
		case GREnemyRanged.Behavior.Patrol:
			this.targetPlayer = null;
			this.abilityPatrol.Start();
			break;
		case GREnemyRanged.Behavior.Search:
			this.targetPlayer = null;
			this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
			this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
			this.lastMoving = false;
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Start();
			if (this.entity.IsAuthority())
			{
				this.entity.manager.RequestCreateItem(this.corePrefab.gameObject.name.GetStaticHash(), this.coreMarker.position, this.coreMarker.rotation, 0L);
			}
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
			this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.ChaseSpeed);
			this.EnableVFXForShoulderHead();
			this.chaseAbilitySound.Play(this.audioSecondarySource);
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			this.PlayAnim("GREnemyRangedAttack01", 0.1f, 1f);
			this.navAgent.speed = 0f;
			this.navAgent.velocity = Vector3.zero;
			this.headRemovaltime = PhotonNetwork.Time + (double)this.headRemovalFrame;
			this.attackAbilitySound.Play(this.audioSource);
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.lastMoving = true;
			this.abilityKeepDistance.SetTargetPlayer(this.targetPlayer);
			this.abilityKeepDistance.Start();
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06002C3A RID: 11322 RVA: 0x000EED18 File Offset: 0x000ECF18
	private void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null)
		{
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06002C3B RID: 11323 RVA: 0x000EED48 File Offset: 0x000ECF48
	public void SetBodyState(GREnemyRanged.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.ForceResetThrowableHead();
			for (int i = 0; i < this.colliders.Count; i++)
			{
				this.colliders[i].enabled = true;
			}
			break;
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.DisableHeadOnShoulderAndHeadInHand();
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyRanged.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyRanged.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06002C3C RID: 11324 RVA: 0x000EEE58 File Offset: 0x000ED058
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyRanged.BodyState.Destroyed:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, true);
			this.DisableHeadOnShoulderAndHeadInHand();
			return;
		case GREnemyRanged.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyRanged.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002C3D RID: 11325 RVA: 0x000EEEF8 File Offset: 0x000ED0F8
	private void Update()
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(Time.deltaTime);
		}
		else
		{
			this.OnUpdateRemote(Time.deltaTime);
		}
		this.UpdateShared();
	}

	// Token: 0x06002C3E RID: 11326 RVA: 0x000EEF28 File Offset: 0x000ED128
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (!GhostReactorManager.AggroDisabled)
		{
			GREnemyRanged.Behavior behavior = this.currBehavior;
			if (behavior > GREnemyRanged.Behavior.Search)
			{
				if (behavior == GREnemyRanged.Behavior.RangedAttackCooldown)
				{
					this.abilityKeepDistance.Think(dt);
					this.UpdateTarget();
					return;
				}
				if (behavior != GREnemyRanged.Behavior.Investigate)
				{
					return;
				}
			}
			this.UpdateTarget();
		}
	}

	// Token: 0x06002C3F RID: 11327 RVA: 0x000EEF78 File Offset: 0x000ED178
	private void UpdateTarget()
	{
		float num = float.MaxValue;
		this.bestTargetPlayer = null;
		this.bestTargetNetPlayer = null;
		GREnemyRanged.tempRigs.Clear();
		GREnemyRanged.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyRanged.tempRigs);
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.rotation * Vector3.forward;
		float num2 = this.sightDist * this.sightDist;
		float num3 = Mathf.Cos(this.sightFOV * 0.017453292f);
		for (int i = 0; i < GREnemyRanged.tempRigs.Count; i++)
		{
			VRRig vrrig = GREnemyRanged.tempRigs[i];
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if (component.State != GRPlayer.GRPlayerState.Ghost)
			{
				Vector3 position2 = vrrig.transform.position;
				Vector3 vector2 = position2 - position;
				float sqrMagnitude = vector2.sqrMagnitude;
				if (sqrMagnitude <= num2)
				{
					float num4 = 0f;
					if (sqrMagnitude > 0f)
					{
						num4 = Mathf.Sqrt(sqrMagnitude);
						if (Vector3.Dot(vector2 / num4, vector) < num3)
						{
							goto IL_16F;
						}
					}
					if (num4 < num && Physics.RaycastNonAlloc(new Ray(this.headTransform.position, position2 - this.headTransform.position), GREnemyChaser.visibilityHits, num4, this.visibilityLayerMask.value, 1) < 1)
					{
						num = num4;
						this.bestTargetPlayer = component;
						this.bestTargetNetPlayer = vrrig.OwningNetPlayer;
						this.lastSeenTargetTime = Time.timeAsDouble;
						this.lastSeenTargetPosition = position2;
					}
				}
			}
			IL_16F:;
		}
	}

	// Token: 0x06002C40 RID: 11328 RVA: 0x000EF10C File Offset: 0x000ED30C
	private void ChooseNewBehavior()
	{
		if (this.bestTargetPlayer != null && Time.timeAsDouble - this.lastSeenTargetTime < (double)this.sightLostFollowStopTime)
		{
			this.targetPlayer = this.bestTargetNetPlayer;
			this.lastSeenTargetTime = Time.timeAsDouble;
			this.investigateLocation = default(Vector3?);
			this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
			return;
		}
		if (Time.timeAsDouble - this.lastSeenTargetTime < (double)this.searchTime)
		{
			this.SetBehavior(GREnemyRanged.Behavior.Search, false);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyRanged.Behavior.Investigate, false);
			return;
		}
		if (this.abilityPatrol.HasValidPatrolPath())
		{
			this.SetBehavior(GREnemyRanged.Behavior.Patrol, false);
			return;
		}
		this.SetBehavior(GREnemyRanged.Behavior.Idle, false);
	}

	// Token: 0x06002C41 RID: 11329 RVA: 0x000EF1F4 File Offset: 0x000ED3F4
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Idle:
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Patrol:
			this.abilityPatrol.Update(dt);
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Search:
			this.UpdateSearch();
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.Update(dt);
			if (this.abilityStagger.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.Update(dt);
			break;
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
			if (this.targetPlayer != null)
			{
				GRPlayer grplayer = GRPlayer.Get(this.targetPlayer.ActorNumber);
				if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
				{
					Vector3 position = grplayer.transform.position;
					Vector3 position2 = base.transform.position;
					float magnitude = (position - position2).magnitude;
					if (magnitude > this.loseSightDist)
					{
						this.ChooseNewBehavior();
					}
					else
					{
						float num = Vector3.Distance(position, this.headTransform.position);
						bool flag = false;
						if (num < this.sightDist)
						{
							flag = (Physics.RaycastNonAlloc(new Ray(this.headTransform.position, position - this.headTransform.position), GREnemyChaser.visibilityHits, num, this.visibilityLayerMask.value, 1) < 1);
						}
						if (flag)
						{
							this.lastSeenTargetPosition = position;
							this.lastSeenTargetTime = Time.timeAsDouble;
						}
						if (Time.timeAsDouble - this.lastSeenTargetTime < (double)this.sightLostFollowStopTime)
						{
							this.searchPosition = position;
							this.agent.RequestDestination(this.lastSeenTargetPosition);
							if (flag)
							{
								this.rangedTargetPosition = position;
								Vector3 vector = Vector3.up * 0.4f;
								this.rangedTargetPosition += vector;
								if (magnitude < this.rangedAttackDistMax)
								{
									this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackChargeTime;
									this.SetBehavior(GREnemyRanged.Behavior.RangedAttack, false);
									GhostReactorManager.Get(this.entity).RequestFireProjectile(this.entity.id, this.rangedProjectileFirePoint.position, this.rangedTargetPosition, PhotonNetwork.Time + (double)this.rangedAttackChargeTime);
								}
							}
						}
						else
						{
							this.ChooseNewBehavior();
						}
					}
				}
			}
			break;
		case GREnemyRanged.Behavior.RangedAttack:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				if (this.targetPlayer != null)
				{
					GRPlayer grplayer2 = GRPlayer.Get(this.targetPlayer.ActorNumber);
					if (grplayer2 != null && grplayer2.State == GRPlayer.GRPlayerState.Alive)
					{
						this.rangedTargetPosition = grplayer2.transform.position;
					}
				}
				this.SetBehavior(GREnemyRanged.Behavior.RangedAttackCooldown, false);
				this.behaviorEndTime = Time.timeAsDouble + (double)this.rangedAttackRecoverTime;
			}
			break;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			if (Time.timeAsDouble > this.behaviorEndTime)
			{
				this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				this.behaviorEndTime = Time.timeAsDouble;
			}
			else
			{
				this.abilityKeepDistance.Update(dt);
			}
			break;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.Update(dt);
			if (this.abilityFlashed.IsDone())
			{
				if (this.targetPlayer == null)
				{
					this.SetBehavior(GREnemyRanged.Behavior.Search, false);
				}
				else
				{
					this.SetBehavior(GREnemyRanged.Behavior.SeekRangedAttackPosition, false);
				}
			}
			break;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.Update(dt);
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
			}
			this.ChooseNewBehavior();
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.Update(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		}
		GameAgent.UpdateFacing(base.transform, this.navAgent, this.targetPlayer, this.turnSpeed);
	}

	// Token: 0x06002C42 RID: 11330 RVA: 0x000EF5D0 File Offset: 0x000ED7D0
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyRanged.Behavior.Patrol:
			this.abilityPatrol.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Search:
		case GREnemyRanged.Behavior.SeekRangedAttackPosition:
		case GREnemyRanged.Behavior.RangedAttack:
			break;
		case GREnemyRanged.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Dying:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.RangedAttackCooldown:
			this.abilityKeepDistance.Update(dt);
			return;
		case GREnemyRanged.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			return;
		case GREnemyRanged.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			if (GhostReactorManager.noiseDebugEnabled)
			{
				DebugUtil.DrawLine(base.transform.position, this.abilityInvestigate.GetTargetPos(), Color.green, true);
				return;
			}
			break;
		case GREnemyRanged.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002C43 RID: 11331 RVA: 0x000EF698 File Offset: 0x000ED898
	public void UpdateShared()
	{
		if (this.rangedAttackQueued)
		{
			if (!this.headRemoved && this.currBehavior == GREnemyRanged.Behavior.RangedAttack && PhotonNetwork.Time >= this.headRemovaltime)
			{
				this.headRemoved = true;
				this.EnableVFXForHeadInHand();
			}
			if (PhotonNetwork.Time > this.queuedFiringTime)
			{
				this.rangedAttackQueued = false;
				this.FireRangedAttack(this.queuedFiringPosition, this.queuedTargetPosition);
			}
		}
		if (this.headLightReset && Time.timeAsDouble > this.spitterLightTurnOffTime)
		{
			this.spitterHeadOnShouldersLight.SetActive(false);
			this.headLightReset = false;
		}
	}

	// Token: 0x06002C44 RID: 11332 RVA: 0x000EF728 File Offset: 0x000ED928
	public void UpdateSearch()
	{
		Vector3 vector = this.searchPosition - base.transform.position;
		Vector3 vector2;
		vector2..ctor(vector.x, 0f, vector.z);
		if (vector2.sqrMagnitude < 0.15f)
		{
			Vector3 vector3 = this.lastSeenTargetPosition - this.searchPosition;
			vector3.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector3;
		}
		if (this.IsMoving())
		{
			if (!this.lastMoving)
			{
				this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
				this.lastMoving = true;
			}
		}
		else if (this.lastMoving)
		{
			this.PlayAnim("GREnemyRangedWalk", 0.1f, 1f);
			this.lastMoving = false;
		}
		this.agent.RequestDestination(this.searchPosition);
		if (Time.timeAsDouble - this.lastSeenTargetTime > (double)this.searchTime)
		{
			this.ChooseNewBehavior();
		}
	}

	// Token: 0x06002C45 RID: 11333 RVA: 0x000EF820 File Offset: 0x000EDA20
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState != GREnemyRanged.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyRanged.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		this.hp -= hit.hitAmount;
		this.audioSource.PlayOneShot(this.damagedSound, this.damagedSoundVolume);
		if (this.fxDamaged != null)
		{
			this.fxDamaged.SetActive(false);
			this.fxDamaged.SetActive(true);
		}
		if (this.hp <= 0)
		{
			this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
			this.SetBodyState(GREnemyRanged.BodyState.Destroyed, false);
			this.SetBehavior(GREnemyRanged.Behavior.Dying, false);
			return;
		}
		this.lastSeenTargetPosition = tool.transform.position;
		this.lastSeenTargetTime = Time.timeAsDouble;
		Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
		vector.y = 0f;
		this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyRanged.Behavior.Stagger);
	}

	// Token: 0x06002C46 RID: 11334 RVA: 0x000EF964 File Offset: 0x000EDB64
	public void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		if (this.currBodyState == GREnemyRanged.BodyState.Shell)
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
				this.SetBodyState(GREnemyRanged.BodyState.Bones, false);
				if (tool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
			}
			else if (tool != null)
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.lastSeenTargetPosition = tool.transform.position;
				this.lastSeenTargetTime = Time.timeAsDouble;
				Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
				vector.y = 0f;
				this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
				this.SetBehavior(GREnemyRanged.Behavior.Search, false);
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
		GRToolFlash component = tool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.SetBehavior(GREnemyRanged.Behavior.Flashed, false);
	}

	// Token: 0x06002C47 RID: 11335 RVA: 0x000EFB26 File Offset: 0x000EDD26
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x06002C48 RID: 11336 RVA: 0x000EFB30 File Offset: 0x000EDD30
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

	// Token: 0x06002C49 RID: 11337 RVA: 0x000EFB9C File Offset: 0x000EDD9C
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyRanged.Behavior newBehavior = (GREnemyRanged.Behavior)reader.ReadByte();
		GREnemyRanged.BodyState newBodyState = (GREnemyRanged.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte b = reader.ReadByte();
		int playerID = reader.ReadInt32();
		this.SetPatrolPath((long)((int)this.entity.createData));
		this.abilityPatrol.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(newBehavior, true);
		this.SetBodyState(newBodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(playerID);
	}

	// Token: 0x06002C4A RID: 11338 RVA: 0x00027DED File Offset: 0x00025FED
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002C4B RID: 11339 RVA: 0x000EFC18 File Offset: 0x000EDE18
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

	// Token: 0x06002C4C RID: 11340 RVA: 0x000EFC79 File Offset: 0x000EDE79
	public void RequestRangedAttack(Vector3 firingPosition, Vector3 targetPosition, double fireTime)
	{
		this.rangedAttackQueued = true;
		this.queuedFiringTime = fireTime;
		this.queuedFiringPosition = firingPosition;
		this.queuedTargetPosition = targetPosition;
	}

	// Token: 0x06002C4D RID: 11341 RVA: 0x000EFC98 File Offset: 0x000EDE98
	private void DestroyProjectile()
	{
		if (this.entity.IsAuthority() && this.rangedProjectileInstance != null)
		{
			GameEntity component = this.rangedProjectileInstance.GetComponent<GameEntity>();
			if (component != null)
			{
				component.manager.RequestDestroyItem(component.id);
			}
		}
	}

	// Token: 0x06002C4E RID: 11342 RVA: 0x000EFCE8 File Offset: 0x000EDEE8
	private void FireRangedAttack(Vector3 launchPosition, Vector3 targetPosition)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.DisableHeadInHand();
		this.DestroyProjectile();
		Vector3 vector;
		if (GREnemyRanged.CalculateLaunchDirection(launchPosition, targetPosition, this.projectileSpeed, out vector))
		{
			this.entity.manager.RequestCreateItem(this.rangedProjectilePrefab.name.GetStaticHash(), launchPosition, Quaternion.LookRotation(vector, Vector3.up), (long)this.entity.GetNetId());
		}
	}

	// Token: 0x06002C4F RID: 11343 RVA: 0x000EFD5C File Offset: 0x000EDF5C
	public static bool CalculateLaunchDirection(Vector3 startPos, Vector3 targetPos, float speed, out Vector3 direction)
	{
		direction = Vector3.zero;
		Vector3 vector = targetPos - startPos;
		Vector3 vector2;
		vector2..ctor(vector.x, 0f, vector.z);
		float magnitude = vector2.magnitude;
		Vector3 normalized = vector2.normalized;
		float y = vector.y;
		float num = 9.8f;
		float num2 = speed * speed;
		float num3 = num2 * num2 - num * (num * magnitude * magnitude + 2f * y * num2);
		if (num3 < 0f)
		{
			return false;
		}
		int num4 = 0;
		float num5 = Mathf.Sqrt(num3);
		float num6 = (num2 + num5) / (num * magnitude);
		float num7 = (num2 - num5) / (num * magnitude);
		float num8 = num2 / (num6 * num6 + 1f);
		float num9 = num2 / (num7 * num7 + 1f);
		float num10 = (num4 != 0) ? Mathf.Min(num8, num9) : Mathf.Max(num8, num9);
		float num11 = (num4 != 0) ? ((num8 < num9) ? Mathf.Sign(num6) : Mathf.Sign(num7)) : ((num8 > num9) ? Mathf.Sign(num6) : Mathf.Sign(num7));
		float num12 = Mathf.Sqrt(num10);
		float num13 = Mathf.Sqrt(Mathf.Abs(num2 - num10));
		direction = (normalized * num12 + new Vector3(0f, num13 * num11, 0f)).normalized;
		return true;
	}

	// Token: 0x06002C50 RID: 11344 RVA: 0x000EFEB8 File Offset: 0x000EE0B8
	public void OnProjectileInit(GRRangedEnemyProjectile projectile)
	{
		this.rangedProjectileInstance = projectile.gameObject;
	}

	// Token: 0x06002C51 RID: 11345 RVA: 0x00002789 File Offset: 0x00000989
	public void OnProjectileHit(GRRangedEnemyProjectile projectile, Collision collision)
	{
	}

	// Token: 0x06002C52 RID: 11346 RVA: 0x000EFEC8 File Offset: 0x000EE0C8
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		strings.Add(string.Format("speed: <color=\"yellow\">{0}<color=\"white\"> patrol node:<color=\"yellow\">{1}/{2}<color=\"white\">", this.navAgent.speed, this.abilityPatrol.nextPatrolNode, (this.abilityPatrol.GetPatrolPath() != null) ? this.abilityPatrol.GetPatrolPath().patrolNodes.Count : 0));
		if (this.targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(this.targetPlayer.ActorNumber);
			if (grplayer != null)
			{
				float magnitude = (grplayer.transform.position - base.transform.position).magnitude;
				strings.Add(string.Format("TargetDis: <color=\"yellow\">{0}<color=\"white\"> ", magnitude));
			}
		}
	}

	// Token: 0x04003912 RID: 14610
	public GameEntity entity;

	// Token: 0x04003913 RID: 14611
	public GameAgent agent;

	// Token: 0x04003914 RID: 14612
	public GRArmorEnemy armor;

	// Token: 0x04003915 RID: 14613
	public GameHittable hittable;

	// Token: 0x04003916 RID: 14614
	public GRAttributes attributes;

	// Token: 0x04003917 RID: 14615
	public Animation anim;

	// Token: 0x04003918 RID: 14616
	public GRAbilityStagger abilityStagger;

	// Token: 0x04003919 RID: 14617
	public GRAbilityDie abilityDie;

	// Token: 0x0400391A RID: 14618
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x0400391B RID: 14619
	public GRAbilityPatrol abilityPatrol;

	// Token: 0x0400391C RID: 14620
	public GRAbilityFlashed abilityFlashed;

	// Token: 0x0400391D RID: 14621
	public GRAbilityKeepDistance abilityKeepDistance;

	// Token: 0x0400391E RID: 14622
	public GRAbilityJump abilityJump;

	// Token: 0x0400391F RID: 14623
	public List<Renderer> bones;

	// Token: 0x04003920 RID: 14624
	public List<Renderer> always;

	// Token: 0x04003921 RID: 14625
	public Transform coreMarker;

	// Token: 0x04003922 RID: 14626
	public GRCollectible corePrefab;

	// Token: 0x04003923 RID: 14627
	public Transform headTransform;

	// Token: 0x04003924 RID: 14628
	public float sightDist;

	// Token: 0x04003925 RID: 14629
	public float loseSightDist;

	// Token: 0x04003926 RID: 14630
	public float sightFOV;

	// Token: 0x04003927 RID: 14631
	public float sightLostFollowStopTime = 0.5f;

	// Token: 0x04003928 RID: 14632
	public float searchTime = 5f;

	// Token: 0x04003929 RID: 14633
	public float hearingRadius = 5f;

	// Token: 0x0400392A RID: 14634
	public float turnSpeed = 540f;

	// Token: 0x0400392B RID: 14635
	public Color chaseColor = Color.red;

	// Token: 0x0400392C RID: 14636
	public AbilitySound attackAbilitySound;

	// Token: 0x0400392D RID: 14637
	public AbilitySound chaseAbilitySound;

	// Token: 0x0400392E RID: 14638
	public float rangedAttackDistMin = 6f;

	// Token: 0x0400392F RID: 14639
	public float rangedAttackDistMax = 8f;

	// Token: 0x04003930 RID: 14640
	public float rangedAttackChargeTime = 0.5f;

	// Token: 0x04003931 RID: 14641
	public float rangedAttackRecoverTime = 2f;

	// Token: 0x04003932 RID: 14642
	public float projectileSpeed = 5f;

	// Token: 0x04003933 RID: 14643
	public float projectileHitRadius = 1f;

	// Token: 0x04003934 RID: 14644
	public GameObject rangedProjectilePrefab;

	// Token: 0x04003935 RID: 14645
	public Transform rangedProjectileFirePoint;

	// Token: 0x04003936 RID: 14646
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04003937 RID: 14647
	public NavMeshAgent navAgent;

	// Token: 0x04003938 RID: 14648
	public AudioSource audioSource;

	// Token: 0x04003939 RID: 14649
	public AudioSource audioSecondarySource;

	// Token: 0x0400393A RID: 14650
	public AudioClip damagedSound;

	// Token: 0x0400393B RID: 14651
	public float damagedSoundVolume;

	// Token: 0x0400393C RID: 14652
	public GameObject fxDamaged;

	// Token: 0x0400393D RID: 14653
	public bool lastMoving;

	// Token: 0x0400393E RID: 14654
	private Vector3? investigateLocation;

	// Token: 0x0400393F RID: 14655
	public bool debugLog;

	// Token: 0x04003940 RID: 14656
	public GameObject spitterHeadOnShoulders;

	// Token: 0x04003941 RID: 14657
	public GameObject spitterHeadOnShouldersLight;

	// Token: 0x04003942 RID: 14658
	public GameObject spitterHeadOnShouldersVFX;

	// Token: 0x04003943 RID: 14659
	public GameObject spitterHeadInHand;

	// Token: 0x04003944 RID: 14660
	public GameObject spitterHeadInHandLight;

	// Token: 0x04003945 RID: 14661
	public GameObject spitterHeadInHandVFX;

	// Token: 0x04003946 RID: 14662
	public double spitterLightTurnOffDelay = 0.75;

	// Token: 0x04003947 RID: 14663
	private bool headLightReset;

	// Token: 0x04003948 RID: 14664
	private double spitterLightTurnOffTime;

	// Token: 0x04003949 RID: 14665
	[FormerlySerializedAs("headRemovalInterval")]
	public float headRemovalFrame = 0.23333333f;

	// Token: 0x0400394A RID: 14666
	private double headRemovaltime;

	// Token: 0x0400394B RID: 14667
	private bool headRemoved;

	// Token: 0x0400394C RID: 14668
	private Transform target;

	// Token: 0x0400394D RID: 14669
	[ReadOnly]
	public int hp;

	// Token: 0x0400394E RID: 14670
	[ReadOnly]
	public GREnemyRanged.Behavior currBehavior;

	// Token: 0x0400394F RID: 14671
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04003950 RID: 14672
	[ReadOnly]
	public GREnemyRanged.BodyState currBodyState;

	// Token: 0x04003951 RID: 14673
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x04003952 RID: 14674
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003953 RID: 14675
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003954 RID: 14676
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003955 RID: 14677
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003956 RID: 14678
	[ReadOnly]
	public Vector3 rangedFiringPosition;

	// Token: 0x04003957 RID: 14679
	[ReadOnly]
	public Vector3 rangedTargetPosition;

	// Token: 0x04003958 RID: 14680
	[ReadOnly]
	private GRPlayer bestTargetPlayer;

	// Token: 0x04003959 RID: 14681
	[ReadOnly]
	private NetPlayer bestTargetNetPlayer;

	// Token: 0x0400395A RID: 14682
	private bool rangedAttackQueued;

	// Token: 0x0400395B RID: 14683
	private double queuedFiringTime;

	// Token: 0x0400395C RID: 14684
	private Vector3 queuedFiringPosition;

	// Token: 0x0400395D RID: 14685
	private Vector3 queuedTargetPosition;

	// Token: 0x0400395E RID: 14686
	private GameObject rangedProjectileInstance;

	// Token: 0x0400395F RID: 14687
	private bool projectileHasImpacted;

	// Token: 0x04003960 RID: 14688
	private double projectileImpactTime;

	// Token: 0x04003961 RID: 14689
	private Rigidbody rigidBody;

	// Token: 0x04003962 RID: 14690
	private List<Collider> colliders;

	// Token: 0x04003963 RID: 14691
	private LayerMask visibilityLayerMask;

	// Token: 0x04003964 RID: 14692
	private Color defaultColor;

	// Token: 0x04003965 RID: 14693
	private float lastHitPlayerTime;

	// Token: 0x04003966 RID: 14694
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003967 RID: 14695
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x020006BA RID: 1722
	public enum Behavior
	{
		// Token: 0x04003969 RID: 14697
		Idle,
		// Token: 0x0400396A RID: 14698
		Patrol,
		// Token: 0x0400396B RID: 14699
		Search,
		// Token: 0x0400396C RID: 14700
		Stagger,
		// Token: 0x0400396D RID: 14701
		Dying,
		// Token: 0x0400396E RID: 14702
		SeekRangedAttackPosition,
		// Token: 0x0400396F RID: 14703
		RangedAttack,
		// Token: 0x04003970 RID: 14704
		RangedAttackCooldown,
		// Token: 0x04003971 RID: 14705
		Flashed,
		// Token: 0x04003972 RID: 14706
		Investigate,
		// Token: 0x04003973 RID: 14707
		Jump,
		// Token: 0x04003974 RID: 14708
		Count
	}

	// Token: 0x020006BB RID: 1723
	public enum BodyState
	{
		// Token: 0x04003976 RID: 14710
		Destroyed,
		// Token: 0x04003977 RID: 14711
		Bones,
		// Token: 0x04003978 RID: 14712
		Shell,
		// Token: 0x04003979 RID: 14713
		Count
	}
}
