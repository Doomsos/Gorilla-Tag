using System;
using UnityEngine;

// Token: 0x020006EB RID: 1771
public class GRRangedEnemyProjectile : MonoBehaviour, IGameEntityComponent, IGameHittable, IGameHitter
{
	// Token: 0x06002D55 RID: 11605 RVA: 0x000F53B8 File Offset: 0x000F35B8
	private void Awake()
	{
		this.particleSystem = base.GetComponentInChildren<ParticleSystem>();
		this.audioSource = base.GetComponentInChildren<AudioSource>();
		this.meshRenderer = base.GetComponentInChildren<MeshRenderer>();
		this.hittable = base.GetComponentInChildren<GameHittable>();
		this.projectileRigidbody = base.GetComponent<Rigidbody>();
		this.entity = base.GetComponent<GameEntity>();
	}

	// Token: 0x06002D56 RID: 11606 RVA: 0x000F5410 File Offset: 0x000F3610
	private void Start()
	{
		if (this.projectileRigidbody != null)
		{
			this.projectileRigidbody.linearVelocity = base.transform.forward * this.projectileSpeed;
		}
		this.projectileHasImpacted = false;
		if (this.owningEntity != null)
		{
			Collider componentInChildren = base.GetComponentInChildren<Collider>();
			if (componentInChildren != null)
			{
				Collider[] componentsInChildren = this.owningEntity.gameObject.GetComponentsInChildren<Collider>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Physics.IgnoreCollision(componentInChildren, componentsInChildren[i]);
				}
			}
		}
	}

	// Token: 0x06002D57 RID: 11607 RVA: 0x000F549C File Offset: 0x000F369C
	private void Update()
	{
		if (this.entity.IsAuthority() && this.projectileHasImpacted && Time.timeAsDouble > this.projectileImpactTime + (double)this.postImpactLifetime)
		{
			this.entity.manager.RequestDestroyItem(this.entity.id);
		}
	}

	// Token: 0x06002D58 RID: 11608 RVA: 0x000F54F0 File Offset: 0x000F36F0
	public void OnEntityInit()
	{
		this.owningEntityNetID = (int)this.entity.createData;
		if (this.owningEntityNetID != 0)
		{
			this.owningEntity = this.FindOwningEntity();
			this.projectileLauncher = this.owningEntity.GetComponent<IGameProjectileLauncher>();
			if (this.projectileLauncher != null)
			{
				this.projectileLauncher.OnProjectileInit(this);
			}
		}
	}

	// Token: 0x06002D59 RID: 11609 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002D5A RID: 11610 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002D5B RID: 11611 RVA: 0x000F5548 File Offset: 0x000F3748
	private GameEntity FindOwningEntity()
	{
		if (this.owningEntityNetID != 0)
		{
			GameEntityManager gameEntityManager = GhostReactorManager.Get(this.entity).gameEntityManager;
			GameEntityId entityIdFromNetId = gameEntityManager.GetEntityIdFromNetId(this.owningEntityNetID);
			return gameEntityManager.GetGameEntity(entityIdFromNetId);
		}
		return null;
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x000F5584 File Offset: 0x000F3784
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.projectileHasImpacted)
		{
			if (this.canHitPlayer)
			{
				Vector3 position = base.transform.position;
				if ((VRRig.LocalRig.GetMouthPosition() - position).sqrMagnitude < this.projectileHitRadius * this.projectileHitRadius && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
				{
					this.lastHitPlayerTime = Time.time;
					GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Ranged, this.entity.id, VRRig.LocalRig.GetComponent<GRPlayer>(), position);
				}
				if (this.projectileLauncher != null)
				{
					this.projectileLauncher.OnProjectileHit(this, collision);
				}
			}
			this.projectileHasImpacted = true;
			this.projectileImpactTime = Time.timeAsDouble;
		}
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x000F5648 File Offset: 0x000F3848
	private void OnTriggerEnter(Collider collider)
	{
		if (!this.projectileHasImpacted)
		{
			GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
			if (component != null)
			{
				component.BlockHittable(this.projectileRigidbody.transform.position, this.projectileRigidbody.linearVelocity.normalized, this.hittable);
			}
		}
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x00027DED File Offset: 0x00025FED
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x000F569C File Offset: 0x000F389C
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

	// Token: 0x06002D60 RID: 11616 RVA: 0x000F5700 File Offset: 0x000F3900
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		this.projectileHasImpacted = true;
		this.projectileImpactTime = Time.timeAsDouble;
		if (this.projectileRigidbody != null)
		{
			this.PlayImpactFX();
			this.projectileRigidbody.linearVelocity = hit.hitImpulse * (this.projectileRigidbody.linearVelocity.magnitude * 0.7f);
		}
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x00002789 File Offset: 0x00000989
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
	}

	// Token: 0x06002D62 RID: 11618 RVA: 0x000F5762 File Offset: 0x000F3962
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.projectileHasImpacted = true;
		this.projectileImpactTime = Time.timeAsDouble;
		if (this.projectileRigidbody != null)
		{
			this.PlayImpactFX();
			this.projectileRigidbody.linearVelocity = hit.hitImpulse;
		}
	}

	// Token: 0x06002D63 RID: 11619 RVA: 0x000F579B File Offset: 0x000F399B
	private void PlayImpactFX()
	{
		if (this.particleSystem != null)
		{
			this.particleSystem.Play();
		}
		if (this.meshRenderer != null)
		{
			this.meshRenderer.enabled = false;
		}
	}

	// Token: 0x06002D64 RID: 11620 RVA: 0x000F57D0 File Offset: 0x000F39D0
	public void OnSuccessfulHit(GameHitData hit)
	{
		this.PlayImpactFX();
	}

	// Token: 0x06002D65 RID: 11621 RVA: 0x000F57D8 File Offset: 0x000F39D8
	public void OnSuccessfulHitPlayer(GRPlayer player, Vector3 hitPosition)
	{
		this.PlayImpactFX();
		this.hitSFX.Play(null);
		if (this.applyFreezeEffect)
		{
			player.SetAsFrozen(4f);
		}
	}

	// Token: 0x04003AF7 RID: 15095
	private int owningEntityNetID;

	// Token: 0x04003AF8 RID: 15096
	private GameEntity entity;

	// Token: 0x04003AF9 RID: 15097
	public GameEntity owningEntity;

	// Token: 0x04003AFA RID: 15098
	private IGameProjectileLauncher projectileLauncher;

	// Token: 0x04003AFB RID: 15099
	public Rigidbody projectileRigidbody;

	// Token: 0x04003AFC RID: 15100
	private ParticleSystem particleSystem;

	// Token: 0x04003AFD RID: 15101
	private AudioSource audioSource;

	// Token: 0x04003AFE RID: 15102
	private MeshRenderer meshRenderer;

	// Token: 0x04003AFF RID: 15103
	private GameHittable hittable;

	// Token: 0x04003B00 RID: 15104
	public float projectileSpeed = 5f;

	// Token: 0x04003B01 RID: 15105
	public float projectileHitRadius = 1f;

	// Token: 0x04003B02 RID: 15106
	public float postImpactLifetime = 2f;

	// Token: 0x04003B03 RID: 15107
	private bool projectileHasImpacted;

	// Token: 0x04003B04 RID: 15108
	private double projectileImpactTime;

	// Token: 0x04003B05 RID: 15109
	private float lastHitPlayerTime;

	// Token: 0x04003B06 RID: 15110
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003B07 RID: 15111
	public bool applyFreezeEffect;

	// Token: 0x04003B08 RID: 15112
	public bool canHitPlayer = true;

	// Token: 0x04003B09 RID: 15113
	public AbilitySound hitSFX;
}
