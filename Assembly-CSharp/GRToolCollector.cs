using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200071A RID: 1818
public class GRToolCollector : MonoBehaviour, IGameEntityDebugComponent, IGameEntityComponent
{
	// Token: 0x06002EA4 RID: 11940 RVA: 0x000FD32D File Offset: 0x000FB52D
	private void Awake()
	{
		this.state = GRToolCollector.State.Idle;
		this.stateTimeRemaining = -1f;
	}

	// Token: 0x06002EA5 RID: 11941 RVA: 0x000FD341 File Offset: 0x000FB541
	private void OnEnable()
	{
		this.SetState(GRToolCollector.State.Idle);
	}

	// Token: 0x06002EA6 RID: 11942 RVA: 0x000FD34A File Offset: 0x000FB54A
	public void OnEntityInit()
	{
		if (this.tool != null)
		{
			this.tool.onToolUpgraded += this.OnToolUpgraded;
			this.OnToolUpgraded(this.tool);
		}
		this.lastRechargeTime = (double)Time.time;
	}

	// Token: 0x06002EA7 RID: 11943 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002EA8 RID: 11944 RVA: 0x00002789 File Offset: 0x00000989
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002EA9 RID: 11945 RVA: 0x000FD38C File Offset: 0x000FB58C
	private void OnToolUpgraded(GRTool tool)
	{
		this.rechargeRate = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.RechargeRate);
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus1))
		{
			this.vacuumSound = this.upgrade1vacuumSound;
			this.vacuumParticleEffect = this.upgrade1VacuumParticleEffect;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus2))
		{
			this.vacuumSound = this.upgrade2vacuumSound;
			this.vacuumParticleEffect = this.upgrade2VacuumParticleEffect;
			return;
		}
		if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus3))
		{
			this.vacuumSound = this.upgrade3vacuumSound;
			this.vacuumParticleEffect = this.upgrade3VacuumParticleEffect;
		}
	}

	// Token: 0x06002EAA RID: 11946 RVA: 0x000FD414 File Offset: 0x000FB614
	private bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06002EAB RID: 11947 RVA: 0x000FD42D File Offset: 0x000FB62D
	public void OnUpdate(float dt)
	{
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002EAC RID: 11948 RVA: 0x000FD450 File Offset: 0x000FB650
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.IsHeldLocal() || this.activatedLocally)
		{
			this.OnUpdateAuthority(deltaTime);
			return;
		}
		this.OnUpdateRemote(deltaTime);
	}

	// Token: 0x06002EAD RID: 11949 RVA: 0x000FD484 File Offset: 0x000FB684
	private void OnUpdateAuthority(float dt)
	{
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
		{
			bool flag = this.IsButtonHeld();
			this.waitingForButtonRelease = (this.waitingForButtonRelease && flag);
			if (flag && !this.waitingForButtonRelease)
			{
				this.SetStateAuthority(GRToolCollector.State.Vacuuming);
				this.activatedLocally = true;
			}
			if (this.rechargeRate > 0f && Time.timeAsDouble > this.lastRechargeTime + (double)this.rechargeInterval)
			{
				this.gameEntity.manager.ghostReactorManager.RequestChargeTool(this.gameEntity.id, this.gameEntity.id, (int)(this.rechargeRate * this.rechargeInterval), false);
				this.lastRechargeTime = Time.timeAsDouble;
				if (this.passiveChargeParticleEffect != null)
				{
					this.passiveChargeParticleEffect.Play();
					return;
				}
			}
			break;
		}
		case GRToolCollector.State.Vacuuming:
		{
			bool flag2 = this.IsButtonHeld();
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Collect);
				return;
			}
			if (!flag2)
			{
				this.SetStateAuthority(GRToolCollector.State.Idle);
				this.activatedLocally = false;
				return;
			}
			break;
		}
		case GRToolCollector.State.Collect:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.SetStateAuthority(GRToolCollector.State.Cooldown);
				return;
			}
			break;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining -= dt;
			if (this.stateTimeRemaining <= 0f)
			{
				this.activatedLocally = false;
				this.waitingForButtonRelease = true;
				this.SetStateAuthority(GRToolCollector.State.Idle);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06002EAE RID: 11950 RVA: 0x000FD5F8 File Offset: 0x000FB7F8
	private void OnUpdateRemote(float dt)
	{
		GRToolCollector.State state = (GRToolCollector.State)this.gameEntity.GetState();
		if (state != this.state)
		{
			this.SetState(state);
		}
	}

	// Token: 0x06002EAF RID: 11951 RVA: 0x000FD622 File Offset: 0x000FB822
	private void SetStateAuthority(GRToolCollector.State newState)
	{
		this.SetState(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06002EB0 RID: 11952 RVA: 0x000FD644 File Offset: 0x000FB844
	private void SetState(GRToolCollector.State newState)
	{
		this.state = newState;
		switch (this.state)
		{
		case GRToolCollector.State.Idle:
			this.StopVacuum();
			this.stateTimeRemaining = -1f;
			this.lastRechargeTime = (double)Time.time;
			return;
		case GRToolCollector.State.Vacuuming:
			this.StartVacuum();
			this.stateTimeRemaining = this.chargeDuration;
			return;
		case GRToolCollector.State.Collect:
			this.TryCollect();
			this.stateTimeRemaining = this.collectDuration;
			return;
		case GRToolCollector.State.Cooldown:
			this.stateTimeRemaining = this.cooldownDuration;
			return;
		default:
			return;
		}
	}

	// Token: 0x06002EB1 RID: 11953 RVA: 0x000FD6C8 File Offset: 0x000FB8C8
	private void StartVacuum()
	{
		this.vacuumAudioSource.clip = this.vacuumSound;
		this.vacuumAudioSource.volume = this.vacuumSoundVolume;
		this.vacuumAudioSource.loop = true;
		this.vacuumAudioSource.Play();
		this.vacuumParticleEffect.Play();
		if (this.IsHeldLocal())
		{
			this.PlayVibration(GorillaTagger.Instance.tapHapticStrength, this.chargeDuration);
		}
	}

	// Token: 0x06002EB2 RID: 11954 RVA: 0x000FD737 File Offset: 0x000FB937
	private void StopVacuum()
	{
		this.vacuumAudioSource.loop = false;
		this.vacuumAudioSource.Stop();
		this.vacuumParticleEffect.Stop();
	}

	// Token: 0x06002EB3 RID: 11955 RVA: 0x000FD75C File Offset: 0x000FB95C
	private void TryCollect()
	{
		if (this.IsHeldLocal())
		{
			int num = Physics.SphereCastNonAlloc(this.shootFrom.position, 0.2f, this.shootFrom.rotation * Vector3.forward, this.tempHitResults, 1f, this.collectibleLayerMask);
			for (int i = 0; i < num; i++)
			{
				RaycastHit raycastHit = this.tempHitResults[i];
				GameObject gameObject = null;
				Rigidbody attachedRigidbody = raycastHit.collider.attachedRigidbody;
				if (attachedRigidbody != null)
				{
					gameObject = attachedRigidbody.gameObject;
				}
				else
				{
					GameEntity gameEntity = GameEntity.Get(raycastHit.collider);
					if (gameEntity != null)
					{
						gameObject = gameEntity.gameObject;
					}
				}
				if (gameObject != null)
				{
					GRCollectible component = gameObject.GetComponent<GRCollectible>();
					if (component != null && component.type != ProgressionManager.CoreType.ChaosSeed && this.tool.energy < this.tool.GetEnergyMax())
					{
						GhostReactorManager.Get(this.gameEntity).RequestCollectItem(component.entity.id, this.gameEntity.id);
						return;
					}
				}
			}
			for (int j = 0; j < num; j++)
			{
				RaycastHit raycastHit2 = this.tempHitResults[j];
				GameObject gameObject2 = null;
				Rigidbody attachedRigidbody2 = raycastHit2.collider.attachedRigidbody;
				if (attachedRigidbody2 != null)
				{
					gameObject2 = attachedRigidbody2.gameObject;
				}
				else
				{
					GameEntity gameEntity2 = GameEntity.Get(raycastHit2.collider);
					if (gameEntity2 != null)
					{
						gameObject2 = gameEntity2.gameObject;
					}
				}
				if (gameObject2 != null)
				{
					if (gameObject2.GetComponent<GRCurrencyDepositor>() != null)
					{
						if (this.tool.energy > 0)
						{
							GhostReactorManager.Get(this.gameEntity).RequestDepositCurrency(this.gameEntity.id);
						}
						return;
					}
					GRTool component2 = gameObject2.GetComponent<GRTool>();
					if (!(component2 == null) && !(component2 == this.tool))
					{
						GameEntity component3 = gameObject2.GetComponent<GameEntity>();
						if (component2 != null && component3 != null)
						{
							GhostReactorManager.Get(this.gameEntity).RequestChargeTool(this.gameEntity.id, component3.id, 0, true);
							if (this.tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.CollectorBonus3) && this.tool.energy > 50)
							{
								List<GRTool> list = new List<GRTool>();
								this.gameEntity.manager.GetEntitiesWithComponentInRadius<GRTool>(base.transform.position, this.level3ChargeRadius, true, list);
								for (int k = 0; k < list.Count; k++)
								{
									GRTool grtool = list[k];
									if (!(grtool.GetComponent<GRToolCollector>() != null) && !(grtool.gameEntity == this.gameEntity) && !(grtool.gameEntity == component3))
									{
										GhostReactorManager.Get(this.gameEntity).RequestChargeTool(this.gameEntity.id, grtool.gameEntity.id, 0, false);
									}
								}
							}
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x06002EB4 RID: 11956 RVA: 0x000FDA6C File Offset: 0x000FBC6C
	public void PerformCollection(GRCollectible collectible)
	{
		this.tool.RefillEnergy(collectible.energyValue + this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HarvestGain), collectible.entity.id);
		this.collectAudioSource.volume = this.collectSoundVolume;
		this.collectAudioSource.PlayOneShot(this.collectSound);
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x000FDAC4 File Offset: 0x000FBCC4
	public void PlayChargeEffect(GRTool targetTool)
	{
		if (targetTool == null)
		{
			return;
		}
		if (targetTool == this.tool)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		for (int i = 0; i < targetTool.energyMeters.Count; i++)
		{
			if (targetTool.energyMeters[i].chargePoint != null)
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].chargePoint.position);
			}
			else
			{
				this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetTool.energyMeters[i].transform.position);
			}
		}
	}

	// Token: 0x06002EB6 RID: 11958 RVA: 0x000FDBA8 File Offset: 0x000FBDA8
	public void PlayChargeEffect(GRCurrencyDepositor targetDepositor)
	{
		if (targetDepositor == null)
		{
			return;
		}
		this.collectAudioSource.volume = this.chargeBeamVolume;
		this.collectAudioSource.PlayOneShot(this.chargeBeamSound);
		this.lightningDispatcher.DispatchLightning(this.lightningDispatcher.transform.position, targetDepositor.depositingChargePoint.position);
	}

	// Token: 0x06002EB7 RID: 11959 RVA: 0x000FDC08 File Offset: 0x000FBE08
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
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

	// Token: 0x06002EB8 RID: 11960 RVA: 0x000FDC68 File Offset: 0x000FBE68
	private void PlayVibration(float strength, float duration)
	{
		if (!this.IsHeldLocal())
		{
			return;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		if (num == -1)
		{
			return;
		}
		GorillaTagger.Instance.StartVibration(GamePlayer.IsLeftHand(num), strength, duration);
	}

	// Token: 0x06002EB9 RID: 11961 RVA: 0x000FDCBC File Offset: 0x000FBEBC
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("Recharge Rate: <color=\"yellow\">{0}<color=\"white\">", this.rechargeRate));
	}

	// Token: 0x04003CCF RID: 15567
	public GameEntity gameEntity;

	// Token: 0x04003CD0 RID: 15568
	public GRTool tool;

	// Token: 0x04003CD1 RID: 15569
	public GRAttributes attributes;

	// Token: 0x04003CD2 RID: 15570
	public int energyDepositPerUse = 100;

	// Token: 0x04003CD3 RID: 15571
	public Transform shootFrom;

	// Token: 0x04003CD4 RID: 15572
	public LayerMask collectibleLayerMask;

	// Token: 0x04003CD5 RID: 15573
	public ParticleSystem vacuumParticleEffect;

	// Token: 0x04003CD6 RID: 15574
	public ParticleSystem upgrade1VacuumParticleEffect;

	// Token: 0x04003CD7 RID: 15575
	public ParticleSystem upgrade2VacuumParticleEffect;

	// Token: 0x04003CD8 RID: 15576
	public ParticleSystem upgrade3VacuumParticleEffect;

	// Token: 0x04003CD9 RID: 15577
	public ParticleSystem passiveChargeParticleEffect;

	// Token: 0x04003CDA RID: 15578
	public AudioSource vacuumAudioSource;

	// Token: 0x04003CDB RID: 15579
	public AudioClip vacuumSound;

	// Token: 0x04003CDC RID: 15580
	public AudioClip upgrade1vacuumSound;

	// Token: 0x04003CDD RID: 15581
	public AudioClip upgrade2vacuumSound;

	// Token: 0x04003CDE RID: 15582
	public AudioClip upgrade3vacuumSound;

	// Token: 0x04003CDF RID: 15583
	public float vacuumSoundVolume = 0.2f;

	// Token: 0x04003CE0 RID: 15584
	public AudioSource collectAudioSource;

	// Token: 0x04003CE1 RID: 15585
	[FormerlySerializedAs("flashSound")]
	public AudioClip collectSound;

	// Token: 0x04003CE2 RID: 15586
	[FormerlySerializedAs("flashSoundVolume")]
	public float collectSoundVolume = 1f;

	// Token: 0x04003CE3 RID: 15587
	public AudioClip chargeBeamSound;

	// Token: 0x04003CE4 RID: 15588
	public float chargeBeamVolume = 0.2f;

	// Token: 0x04003CE5 RID: 15589
	public LightningDispatcher lightningDispatcher;

	// Token: 0x04003CE6 RID: 15590
	public float chargeDuration = 0.75f;

	// Token: 0x04003CE7 RID: 15591
	[FormerlySerializedAs("flashDuration")]
	public float collectDuration = 0.1f;

	// Token: 0x04003CE8 RID: 15592
	public float cooldownDuration;

	// Token: 0x04003CE9 RID: 15593
	public AbilityHaptic collectHaptic;

	// Token: 0x04003CEA RID: 15594
	[NonSerialized]
	public GhostReactorManager grManager;

	// Token: 0x04003CEB RID: 15595
	private float rechargeRate;

	// Token: 0x04003CEC RID: 15596
	public float rechargeInterval = 1f;

	// Token: 0x04003CED RID: 15597
	private double lastRechargeTime;

	// Token: 0x04003CEE RID: 15598
	public float level3ChargeRadius = 4f;

	// Token: 0x04003CEF RID: 15599
	private GRToolCollector.State state;

	// Token: 0x04003CF0 RID: 15600
	private float stateTimeRemaining;

	// Token: 0x04003CF1 RID: 15601
	private bool activatedLocally;

	// Token: 0x04003CF2 RID: 15602
	private bool waitingForButtonRelease;

	// Token: 0x04003CF3 RID: 15603
	private RaycastHit[] tempHitResults = new RaycastHit[128];

	// Token: 0x0200071B RID: 1819
	private enum State
	{
		// Token: 0x04003CF5 RID: 15605
		Idle,
		// Token: 0x04003CF6 RID: 15606
		Vacuuming,
		// Token: 0x04003CF7 RID: 15607
		Collect,
		// Token: 0x04003CF8 RID: 15608
		Cooldown
	}
}
