using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200071C RID: 1820
public class GRToolDirectionalShield : MonoBehaviour, IGameHitter
{
	// Token: 0x06002EBB RID: 11963 RVA: 0x000FDD5C File Offset: 0x000FBF5C
	private void Awake()
	{
		this.hitter = base.GetComponent<GameHitter>();
		this.attributes = base.GetComponent<GRAttributes>();
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x06002EBC RID: 11964 RVA: 0x000FDDB4 File Offset: 0x000FBFB4
	private void OnToolUpgraded(GRTool tool)
	{
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.DirectionalShieldSize1))
		{
			this.deflectAudio = this.upgrade1DeflectAudio;
			this.shieldDeflectVFX = this.upgrade1ShieldDeflectVFX;
			this.reflectsProjectiles = true;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.DirectionalShieldSize2))
		{
			this.deflectAudio = this.upgrade2DeflectAudio;
			this.shieldDeflectVFX = this.upgrade2ShieldDeflectVFX;
			this.reflectsProjectiles = false;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.DirectionalShieldSize3))
		{
			this.deflectAudio = this.upgrade3DeflectAudio;
			this.shieldDeflectVFX = this.upgrade3ShieldDeflectVFX;
			this.reflectsProjectiles = true;
			return;
		}
		this.reflectsProjectiles = false;
	}

	// Token: 0x06002EBD RID: 11965 RVA: 0x000FDE46 File Offset: 0x000FC046
	public void OnEnable()
	{
		this.SetState(GRToolDirectionalShield.State.Closed);
	}

	// Token: 0x06002EBE RID: 11966 RVA: 0x000FDE4F File Offset: 0x000FC04F
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002EBF RID: 11967 RVA: 0x000FDE68 File Offset: 0x000FC068
	private bool IsHeld()
	{
		return this.gameEntity.heldByActorNumber != -1;
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x000FDE7C File Offset: 0x000FC07C
	public void BlockHittable(Vector3 enemyPosition, Vector3 enemyAttackDirection, GameHittable hittable, GRShieldCollider shieldCollider)
	{
		if (this.IsHeldLocal())
		{
			float num = 1f;
			if (this.attributes != null && this.attributes.HasValueForAttribute(GRAttributeType.KnockbackMultiplier))
			{
				num = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.KnockbackMultiplier);
			}
			Vector3 hitImpulse = -enemyAttackDirection * shieldCollider.KnockbackVelocity * num;
			if (this.reflectsProjectiles)
			{
				GRRangedEnemyProjectile component = hittable.GetComponent<GRRangedEnemyProjectile>();
				Vector3 vector;
				if (component != null && component.owningEntity != null && GREnemyRanged.CalculateLaunchDirection(enemyPosition, component.owningEntity.transform.position + new Vector3(0f, 0.5f, 0f), component.projectileSpeed, out vector))
				{
					hitImpulse = vector * component.projectileSpeed;
				}
			}
			GameHitData hitData = new GameHitData
			{
				hitTypeId = 2,
				hitEntityId = hittable.gameEntity.id,
				hitByEntityId = this.gameEntity.id,
				hitEntityPosition = enemyPosition,
				hitImpulse = hitImpulse,
				hitPosition = enemyPosition,
				hitAmount = this.hitter.CalcHitAmount(GameHitType.Shield, hittable, this.gameEntity)
			};
			if (hittable.IsHitValid(hitData))
			{
				hittable.RequestHit(hitData);
			}
		}
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x000FDFC4 File Offset: 0x000FC1C4
	public void OnEnemyBlocked(Vector3 enemyPosition)
	{
		this.tool.UseEnergy();
		this.PlayBlockEffects(enemyPosition);
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x000FDFD8 File Offset: 0x000FC1D8
	private void PlayBlockEffects(Vector3 enemyPosition)
	{
		this.audioSource.PlayOneShot(this.deflectAudio, this.deflectVolume);
		this.shieldDeflectVFX.Play();
		Vector3 vector = Vector3.ClampMagnitude(enemyPosition - this.shieldArcCenterReferencePoint.position, this.shieldArcCenterRadius);
		Vector3 position = this.shieldArcCenterReferencePoint.position + vector;
		this.shieldDeflectImpactPointVFX.transform.position = position;
		this.shieldDeflectImpactPointVFX.Play();
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x000FE052 File Offset: 0x000FC252
	public void OnSuccessfulHit(GameHitData hitData)
	{
		this.tool.UseEnergy();
		this.PlayBlockEffects(hitData.hitEntityPosition);
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x000FE06C File Offset: 0x000FC26C
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (!this.IsHeld())
		{
			this.SetState(GRToolDirectionalShield.State.Closed);
			return;
		}
		if (this.IsHeldLocal())
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06002EC5 RID: 11973 RVA: 0x000FE0A8 File Offset: 0x000FC2A8
	private void OnUpdateAuthority(float dt)
	{
		GRToolDirectionalShield.State state = this.state;
		if (state != GRToolDirectionalShield.State.Closed)
		{
			if (state != GRToolDirectionalShield.State.Open)
			{
				return;
			}
			if (!this.IsButtonHeld() || !this.tool.HasEnoughEnergy())
			{
				this.SetStateAuthority(GRToolDirectionalShield.State.Closed);
			}
		}
		else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
		{
			this.SetStateAuthority(GRToolDirectionalShield.State.Open);
			return;
		}
	}

	// Token: 0x06002EC6 RID: 11974 RVA: 0x000FE108 File Offset: 0x000FC308
	private void OnUpdateRemote(float dt)
	{
		GRToolDirectionalShield.State state = (GRToolDirectionalShield.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06002EC7 RID: 11975 RVA: 0x000FE132 File Offset: 0x000FC332
	private void SetStateAuthority(GRToolDirectionalShield.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x000FE154 File Offset: 0x000FC354
	private void SetState(GRToolDirectionalShield.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		GRToolDirectionalShield.State state = this.state;
		if (state != GRToolDirectionalShield.State.Closed)
		{
		}
		this.state = newState;
		state = this.state;
		if (state == GRToolDirectionalShield.State.Closed)
		{
			this.openCollidersParent.gameObject.SetActive(false);
			for (int i = 0; i < this.shieldAnimators.Count; i++)
			{
				this.shieldAnimators[i].SetBool("Activated", false);
			}
			this.audioSource.PlayOneShot(this.closeAudio, this.closeVolume);
			this.closeHaptic.PlayIfHeldLocal(this.gameEntity);
			this.hitter != null;
			return;
		}
		if (state != GRToolDirectionalShield.State.Open)
		{
			return;
		}
		this.openCollidersParent.gameObject.SetActive(true);
		for (int j = 0; j < this.shieldAnimators.Count; j++)
		{
			this.shieldAnimators[j].SetBool("Activated", true);
		}
		this.audioSource.PlayOneShot(this.openAudio, this.openVolume);
		this.openHaptic.PlayIfHeldLocal(this.gameEntity);
		this.hitter != null;
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x000FE278 File Offset: 0x000FC478
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer = GamePlayer.GetGamePlayer(this.gameEntity.heldByActorNumber);
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? 4 : 5) > 0.25f;
	}

	// Token: 0x04003CF9 RID: 15609
	[Header("References")]
	public GameEntity gameEntity;

	// Token: 0x04003CFA RID: 15610
	public GRTool tool;

	// Token: 0x04003CFB RID: 15611
	public Rigidbody rigidBody;

	// Token: 0x04003CFC RID: 15612
	public AudioSource audioSource;

	// Token: 0x04003CFD RID: 15613
	public List<Animator> shieldAnimators;

	// Token: 0x04003CFE RID: 15614
	public Transform openCollidersParent;

	// Token: 0x04003CFF RID: 15615
	private GameHitter hitter;

	// Token: 0x04003D00 RID: 15616
	private GRAttributes attributes;

	// Token: 0x04003D01 RID: 15617
	[Header("Audio")]
	public AudioClip openAudio;

	// Token: 0x04003D02 RID: 15618
	public float openVolume = 0.5f;

	// Token: 0x04003D03 RID: 15619
	public AudioClip closeAudio;

	// Token: 0x04003D04 RID: 15620
	public float closeVolume = 0.5f;

	// Token: 0x04003D05 RID: 15621
	public AudioClip deflectAudio;

	// Token: 0x04003D06 RID: 15622
	public AudioClip upgrade1DeflectAudio;

	// Token: 0x04003D07 RID: 15623
	public AudioClip upgrade2DeflectAudio;

	// Token: 0x04003D08 RID: 15624
	public AudioClip upgrade3DeflectAudio;

	// Token: 0x04003D09 RID: 15625
	public float deflectVolume = 0.5f;

	// Token: 0x04003D0A RID: 15626
	[Header("VFX")]
	public ParticleSystem shieldDeflectVFX;

	// Token: 0x04003D0B RID: 15627
	public ParticleSystem upgrade1ShieldDeflectVFX;

	// Token: 0x04003D0C RID: 15628
	public ParticleSystem upgrade2ShieldDeflectVFX;

	// Token: 0x04003D0D RID: 15629
	public ParticleSystem upgrade3ShieldDeflectVFX;

	// Token: 0x04003D0E RID: 15630
	public ParticleSystem shieldDeflectImpactPointVFX;

	// Token: 0x04003D0F RID: 15631
	public Transform shieldArcCenterReferencePoint;

	// Token: 0x04003D10 RID: 15632
	public float shieldArcCenterRadius = 1f;

	// Token: 0x04003D11 RID: 15633
	[Header("Haptic")]
	public AbilityHaptic openHaptic;

	// Token: 0x04003D12 RID: 15634
	public AbilityHaptic closeHaptic;

	// Token: 0x04003D13 RID: 15635
	public bool reflectsProjectiles;

	// Token: 0x04003D14 RID: 15636
	private GRToolDirectionalShield.State state;

	// Token: 0x0200071D RID: 1821
	private enum State
	{
		// Token: 0x04003D16 RID: 15638
		Closed,
		// Token: 0x04003D17 RID: 15639
		Open
	}
}
