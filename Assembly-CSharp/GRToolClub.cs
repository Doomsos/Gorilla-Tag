using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000718 RID: 1816
public class GRToolClub : MonoBehaviourTick, IGameHitter, IGameEntityDebugComponent, IGameEntityComponent
{
	// Token: 0x06002E93 RID: 11923 RVA: 0x000FCD4D File Offset: 0x000FAF4D
	private void Awake()
	{
		this.retractableSection.localPosition = new Vector3(0f, 0f, 0f);
	}

	// Token: 0x06002E94 RID: 11924 RVA: 0x000FCD6E File Offset: 0x000FAF6E
	public new void OnEnable()
	{
		base.OnEnable();
		this.SetExtendedAmount(0f);
		this.gameHitter.hitFx = this.noPowerFx;
		this.gameHitter.damageAttribute = this.noPowerAttribute;
		this.SetState(GRToolClub.State.Idle);
	}

	// Token: 0x06002E95 RID: 11925 RVA: 0x000FCDAA File Offset: 0x000FAFAA
	public void OnEntityInit()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
	}

	// Token: 0x06002E96 RID: 11926 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x00002789 File Offset: 0x00000989
	private void OnToolUpgraded(GRTool tool)
	{
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x000FCDE0 File Offset: 0x000FAFE0
	private void EnableImpactVFXForCurrentUpgradeLevel()
	{
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.BatonDamage1))
		{
			this.gameHitter.hitFx = this.upgrade1ImpactVFX;
			return;
		}
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.BatonDamage2))
		{
			this.gameHitter.hitFx = this.upgrade2ImpactVFX;
			return;
		}
		if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.BatonDamage3))
		{
			this.gameHitter.hitFx = this.upgrade3ImpactVFX;
			return;
		}
		this.gameHitter.hitFx = this.poweredImpactFx;
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x000FCE60 File Offset: 0x000FB060
	public override void Tick()
	{
		float deltaTime = Time.deltaTime;
		if (this.gameEntity.IsHeld())
		{
			if (this.gameEntity.IsHeldByLocalPlayer())
			{
				this.OnUpdateAuthority(deltaTime);
			}
			else
			{
				this.OnUpdateRemote(deltaTime);
			}
		}
		else
		{
			this.SetState(GRToolClub.State.Idle);
		}
		this.OnUpdateShared(deltaTime);
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x000FCEB0 File Offset: 0x000FB0B0
	private void OnUpdateAuthority(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (!this.IsButtonHeld() || !this.tool.HasEnoughEnergy())
			{
				this.SetState(GRToolClub.State.Idle);
			}
		}
		else if (this.IsButtonHeld() && this.tool.HasEnoughEnergy())
		{
			this.SetState(GRToolClub.State.Extended);
			return;
		}
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x000FCF10 File Offset: 0x000FB110
	private void OnUpdateRemote(float dt)
	{
		GRToolClub.State state = (GRToolClub.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x000FCF3C File Offset: 0x000FB13C
	private void OnUpdateShared(float dt)
	{
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state != GRToolClub.State.Extended)
			{
				return;
			}
			if (this.extendedAmount < 1f)
			{
				float num = Mathf.MoveTowards(this.extendedAmount, 1f, 1f / this.extensionTime * Time.deltaTime);
				this.SetExtendedAmount(num);
			}
		}
		else if (this.extendedAmount > 0f)
		{
			float num2 = Mathf.MoveTowards(this.extendedAmount, 0f, 1f / this.extensionTime * Time.deltaTime);
			this.SetExtendedAmount(num2);
			return;
		}
	}

	// Token: 0x06002E9E RID: 11934 RVA: 0x000FCFC8 File Offset: 0x000FB1C8
	private void SetExtendedAmount(float newExtendedAmount)
	{
		this.extendedAmount = newExtendedAmount;
		float num = Mathf.Lerp(this.retractableSectionMin, this.retractableSectionMax, this.extendedAmount);
		this.retractableSection.localPosition = new Vector3(0f, num, 0f);
	}

	// Token: 0x06002E9F RID: 11935 RVA: 0x000FD010 File Offset: 0x000FB210
	private void SetState(GRToolClub.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		GRToolClub.State state = this.state;
		if (state != GRToolClub.State.Idle)
		{
		}
		this.state = newState;
		state = this.state;
		if (state != GRToolClub.State.Idle)
		{
			if (state == GRToolClub.State.Extended)
			{
				this.idleCollider.enabled = false;
				this.extendedCollider.enabled = true;
				for (int i = 0; i < this.meshAndMaterials.Count; i++)
				{
					MaterialUtils.SwapMaterial(this.meshAndMaterials[i], false);
				}
				this.humAudioSource.Play();
				this.dullLight.SetActive(true);
				this.audioSource.PlayOneShot(this.extendAudio, this.extendVolume);
				for (int j = 0; j < this.humParticleEffects.Count; j++)
				{
					this.humParticleEffects[j].gameObject.SetActive(true);
				}
				this.EnableImpactVFXForCurrentUpgradeLevel();
				this.gameHitter.damageAttribute = this.poweredAttribute;
				this.openHaptic.PlayIfHeldLocal(this.gameEntity);
			}
		}
		else
		{
			this.extendedCollider.enabled = false;
			this.idleCollider.enabled = true;
			for (int k = 0; k < this.meshAndMaterials.Count; k++)
			{
				MaterialUtils.SwapMaterial(this.meshAndMaterials[k], true);
			}
			this.humAudioSource.Stop();
			this.dullLight.SetActive(false);
			this.audioSource.PlayOneShot(this.retractAudio, this.retractVolume);
			for (int l = 0; l < this.humParticleEffects.Count; l++)
			{
				this.humParticleEffects[l].gameObject.SetActive(false);
			}
			this.gameHitter.hitFx = this.noPowerFx;
			this.gameHitter.damageAttribute = this.noPowerAttribute;
			this.closeHaptic.PlayIfHeldLocal(this.gameEntity);
		}
		if (this.gameEntity.IsHeldByLocalPlayer())
		{
			this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
		}
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x000FD214 File Offset: 0x000FB414
	private bool IsButtonHeld()
	{
		if (!this.gameEntity.IsHeldByLocalPlayer())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? 4 : 5) > 0.25f;
	}

	// Token: 0x06002EA1 RID: 11937 RVA: 0x000FD276 File Offset: 0x000FB476
	public void OnSuccessfulHit(GameHitData hitData)
	{
		if (this.state == GRToolClub.State.Extended)
		{
			this.tool.UseEnergy();
		}
	}

	// Token: 0x06002EA2 RID: 11938 RVA: 0x000FD28C File Offset: 0x000FB48C
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Knockback: <color=\"yellow\">x{0}<color=\"white\">", this.gameHitter.knockbackMultiplier));
	}

	// Token: 0x04003CAC RID: 15532
	public GameEntity gameEntity;

	// Token: 0x04003CAD RID: 15533
	public GameHitter gameHitter;

	// Token: 0x04003CAE RID: 15534
	public GRTool tool;

	// Token: 0x04003CAF RID: 15535
	public Rigidbody rigidBody;

	// Token: 0x04003CB0 RID: 15536
	public AudioSource audioSource;

	// Token: 0x04003CB1 RID: 15537
	public AudioSource humAudioSource;

	// Token: 0x04003CB2 RID: 15538
	public List<ParticleSystem> humParticleEffects = new List<ParticleSystem>();

	// Token: 0x04003CB3 RID: 15539
	public GRAttributes attributes;

	// Token: 0x04003CB4 RID: 15540
	public AudioClip extendAudio;

	// Token: 0x04003CB5 RID: 15541
	public float extendVolume = 0.5f;

	// Token: 0x04003CB6 RID: 15542
	public AudioClip retractAudio;

	// Token: 0x04003CB7 RID: 15543
	public float retractVolume = 0.5f;

	// Token: 0x04003CB8 RID: 15544
	public GameHitFx noPowerFx;

	// Token: 0x04003CB9 RID: 15545
	public GameHitFx poweredImpactFx;

	// Token: 0x04003CBA RID: 15546
	public GameHitFx upgrade1ImpactVFX;

	// Token: 0x04003CBB RID: 15547
	public GameHitFx upgrade2ImpactVFX;

	// Token: 0x04003CBC RID: 15548
	public GameHitFx upgrade3ImpactVFX;

	// Token: 0x04003CBD RID: 15549
	public GRAttributeType noPowerAttribute;

	// Token: 0x04003CBE RID: 15550
	public GRAttributeType poweredAttribute;

	// Token: 0x04003CBF RID: 15551
	public float minHitSpeed = 2.25f;

	// Token: 0x04003CC0 RID: 15552
	public GameObject dullLight;

	// Token: 0x04003CC1 RID: 15553
	public List<MeshAndMaterials> meshAndMaterials;

	// Token: 0x04003CC2 RID: 15554
	public Transform retractableSection;

	// Token: 0x04003CC3 RID: 15555
	public Collider idleCollider;

	// Token: 0x04003CC4 RID: 15556
	public Collider extendedCollider;

	// Token: 0x04003CC5 RID: 15557
	public float retractableSectionMin = -0.31f;

	// Token: 0x04003CC6 RID: 15558
	public float retractableSectionMax;

	// Token: 0x04003CC7 RID: 15559
	public float extensionTime = 0.15f;

	// Token: 0x04003CC8 RID: 15560
	[Header("Haptic")]
	public AbilityHaptic openHaptic;

	// Token: 0x04003CC9 RID: 15561
	public AbilityHaptic closeHaptic;

	// Token: 0x04003CCA RID: 15562
	private float extendedAmount;

	// Token: 0x04003CCB RID: 15563
	private GRToolClub.State state;

	// Token: 0x02000719 RID: 1817
	private enum State
	{
		// Token: 0x04003CCD RID: 15565
		Idle,
		// Token: 0x04003CCE RID: 15566
		Extended
	}
}
