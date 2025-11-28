using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020006DC RID: 1756
public class GRPlayer : MonoBehaviourTick
{
	// Token: 0x17000416 RID: 1046
	// (get) Token: 0x06002CEA RID: 11498 RVA: 0x000F347A File Offset: 0x000F167A
	public GRPlayer.GRPlayerState State
	{
		get
		{
			return this.state;
		}
	}

	// Token: 0x17000417 RID: 1047
	// (get) Token: 0x06002CEB RID: 11499 RVA: 0x000F3482 File Offset: 0x000F1682
	public int Juice
	{
		get
		{
			return this.playerJuice;
		}
	}

	// Token: 0x17000418 RID: 1048
	// (get) Token: 0x06002CEC RID: 11500 RVA: 0x000F348A File Offset: 0x000F168A
	// (set) Token: 0x06002CED RID: 11501 RVA: 0x000F3492 File Offset: 0x000F1692
	public int ShiftCreditCapIncreases { get; set; }

	// Token: 0x17000419 RID: 1049
	// (get) Token: 0x06002CEE RID: 11502 RVA: 0x000F349B File Offset: 0x000F169B
	// (set) Token: 0x06002CEF RID: 11503 RVA: 0x000F34A3 File Offset: 0x000F16A3
	public int ShiftCreditCapIncreasesMax { get; set; }

	// Token: 0x1700041A RID: 1050
	// (get) Token: 0x06002CF0 RID: 11504 RVA: 0x000F34AC File Offset: 0x000F16AC
	public int ShiftCredits
	{
		get
		{
			return this.shiftCreditCache;
		}
	}

	// Token: 0x06002CF1 RID: 11505 RVA: 0x000F34B4 File Offset: 0x000F16B4
	public bool HasXRayVision()
	{
		return this.xRayVisionRefCount > 0;
	}

	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x06002CF2 RID: 11506 RVA: 0x000F34BF File Offset: 0x000F16BF
	public int MaxHp
	{
		get
		{
			return this.maxHp;
		}
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x06002CF3 RID: 11507 RVA: 0x000F34C7 File Offset: 0x000F16C7
	public int MaxShieldHp
	{
		get
		{
			return this.maxShieldHp;
		}
	}

	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x06002CF4 RID: 11508 RVA: 0x000F34CF File Offset: 0x000F16CF
	public int Hp
	{
		get
		{
			return this.hp;
		}
	}

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x06002CF5 RID: 11509 RVA: 0x000F34D7 File Offset: 0x000F16D7
	public int ShieldHp
	{
		get
		{
			return this.shieldHp;
		}
	}

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x06002CF6 RID: 11510 RVA: 0x000F34DF File Offset: 0x000F16DF
	public int ShieldFlags
	{
		get
		{
			return this.shieldFlags;
		}
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x06002CF7 RID: 11511 RVA: 0x000F34E7 File Offset: 0x000F16E7
	public bool InStealthMode
	{
		get
		{
			return this.inStealthMode;
		}
	}

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x06002CF8 RID: 11512 RVA: 0x000F34EF File Offset: 0x000F16EF
	public VRRig MyRig
	{
		get
		{
			return this.vrRig;
		}
	}

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x06002CF9 RID: 11513 RVA: 0x000F34F7 File Offset: 0x000F16F7
	// (set) Token: 0x06002CFA RID: 11514 RVA: 0x000F34FF File Offset: 0x000F16FF
	public float ShiftPlayTime
	{
		get
		{
			return this.shiftPlayTime;
		}
		set
		{
			this.shiftPlayTime = value;
		}
	}

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x06002CFB RID: 11515 RVA: 0x000F3508 File Offset: 0x000F1708
	// (set) Token: 0x06002CFC RID: 11516 RVA: 0x000F3510 File Offset: 0x000F1710
	public int LastShiftCut
	{
		get
		{
			return this.lastShiftCut;
		}
		set
		{
			this.lastShiftCut = value;
		}
	}

	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x06002CFD RID: 11517 RVA: 0x000F3519 File Offset: 0x000F1719
	// (set) Token: 0x06002CFE RID: 11518 RVA: 0x000F3521 File Offset: 0x000F1721
	public GRPlayer.ProgressionData CurrentProgression
	{
		get
		{
			return this.currentProgression;
		}
		set
		{
			this.currentProgression = value;
		}
	}

	// Token: 0x06002CFF RID: 11519 RVA: 0x000F352C File Offset: 0x000F172C
	private void Awake()
	{
		this.vrRig = base.GetComponent<VRRig>();
		this.lowHealthVisualPropertyBlock = new MaterialPropertyBlock();
		this.damageEffects = GTPlayer.Instance.mainCamera.GetComponent<GRPlayerDamageEffects>();
		this.lowHealthTintPropertyId = Shader.PropertyToID("_TintColor");
		this.isEmployee = false;
		this.SetHp(this.maxHp);
		this.SetShieldHp(0);
		this.state = GRPlayer.GRPlayerState.Alive;
		this.RefreshDamageVignetteVisual();
		this.shieldHeadVisual.gameObject.SetActive(false);
		this.shieldBodyVisual.gameObject.SetActive(false);
		this.shieldGameLight = this.shieldBodyVisual.gameObject.GetComponentInChildren<GameLight>(true);
		this.requestCollectItemLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestChargeToolLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestDepositCurrencyLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestShiftStartLimiter = new CallLimiter(25, 1f, 0.5f);
		this.requestToolPurchaseStationLimiter = new CallLimiter(25, 1f, 0.5f);
		this.applyEnemyHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportLocalHitLimiter = new CallLimiter(25, 1f, 0.5f);
		this.reportBreakableBrokenLimiter = new CallLimiter(25, 1f, 0.5f);
		this.playerStateChangeLimiter = new CallLimiter(25, 1f, 0.5f);
		this.promotionBotLimiter = new CallLimiter(25, 1f, 0.5f);
		this.progressionBroadcastLimiter = new CallLimiter(25, 1f, 0.5f);
		this.scoreboardPageLimiter = new CallLimiter(25, 1f, 0.5f);
		this.fireShieldLimiter = new CallLimiter(25, 1f, 0.5f);
		this.shuttleData = new GRPlayer.ShuttleData();
		this.lastLeftWithBadgeAttachedTime = -10000.0;
	}

	// Token: 0x06002D00 RID: 11520 RVA: 0x000F371C File Offset: 0x000F191C
	private void Start()
	{
		if (this.gamePlayer != null && this.gamePlayer.IsLocal())
		{
			this.LoadMyProgression();
			ProgressionManager.Instance.OnGetShiftCredit += new Action<string, int>(this.OnShiftCreditChanged);
			ProgressionManager.Instance.OnGetShiftCreditCapData += new Action<string, int, int>(this.OnShiftCreditCapChanged);
			this.soak = new GhostReactorSoak();
			this.soak.Setup(this);
		}
		else
		{
			this.currentProgression = new GRPlayer.ProgressionData
			{
				points = 0,
				redeemedPoints = 0
			};
		}
		if (ProgressionManager.Instance != null)
		{
			ProgressionManager.Instance.OnGetShiftCredit += new Action<string, int>(this.OnShiftCreditChanged);
			ProgressionManager.Instance.OnGetShiftCreditCapData += new Action<string, int, int>(this.OnShiftCreditCapChanged);
		}
	}

	// Token: 0x06002D01 RID: 11521 RVA: 0x000F37E7 File Offset: 0x000F19E7
	private new void OnDisable()
	{
		this.Reset();
	}

	// Token: 0x06002D02 RID: 11522 RVA: 0x000F37F0 File Offset: 0x000F19F0
	public void Reset()
	{
		this.SetHp(this.maxHp);
		this.SetShieldHp(0);
		this.state = GRPlayer.GRPlayerState.Alive;
		this.RefreshDamageVignetteVisual();
		this.RefreshPlayerVisuals();
		for (int i = 0; i < 8; i++)
		{
			this.synchronizedSessionStats[i] = 0f;
		}
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x000F383C File Offset: 0x000F1A3C
	private void SetHp(int newHp)
	{
		this.hp = Mathf.Max(newHp, 0);
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x000F384B File Offset: 0x000F1A4B
	private void SetShieldHp(int newShieldHp)
	{
		this.shieldHp = Mathf.Max(newShieldHp, 0);
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x000F385C File Offset: 0x000F1A5C
	public void OnShiftCreditCapChanged(string targetMothershipId, int newCap, int newCapMax)
	{
		if (this.mothershipId != null && targetMothershipId == this.mothershipId)
		{
			if (this.gamePlayer.IsLocal() && (newCap != this.ShiftCreditCapIncreases || newCapMax != this.ShiftCreditCapIncreasesMax) && GhostReactor.instance != null)
			{
				GhostReactor.instance.grManager.RefreshShiftCredit();
			}
			this.ShiftCreditCapIncreases = newCap;
			this.ShiftCreditCapIncreasesMax = newCapMax;
		}
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x000F38C8 File Offset: 0x000F1AC8
	public void OnShiftCreditChanged(string targetMothershipId, int newShiftCredits)
	{
		if (this.mothershipId != null && targetMothershipId == this.mothershipId)
		{
			int num = this.shiftCreditCache;
			this.shiftCreditCache = newShiftCredits;
			if (GhostReactor.instance != null && this.gamePlayer.IsLocal() && num != newShiftCredits && GhostReactor.instance != null)
			{
				if (GhostReactor.instance.promotionBot != null)
				{
					GhostReactor.instance.promotionBot.Refresh();
				}
				if (GhostReactor.instance.grManager != null)
				{
					GhostReactor.instance.grManager.RefreshShiftCredit();
				}
			}
		}
		if (GhostReactor.instance != null)
		{
			GhostReactor.instance.RefreshScoreboards();
		}
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x000F3980 File Offset: 0x000F1B80
	public void OnShiftCreditCapData(string targetMothershipId, int shiftCreditCapNumberOfIncreases, int shiftCreditMaxNumberOfIncreases)
	{
		if (this.mothershipId != null)
		{
			targetMothershipId == this.mothershipId;
		}
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x000F3997 File Offset: 0x000F1B97
	public void SubtractShiftCredit(int shiftCreditDelta)
	{
		if (this.gamePlayer.IsLocal())
		{
			ProgressionManager.Instance.SubtractShiftCredit(shiftCreditDelta);
		}
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x000F39B4 File Offset: 0x000F1BB4
	public void OnPlayerHit(Vector3 hitPosition, GhostReactorManager manager, GameEntityId hitByEntityId)
	{
		GameEntity gameEntity = manager.gameEntityManager.GetGameEntity(hitByEntityId);
		int num = 1;
		if (this.State == GRPlayer.GRPlayerState.Alive)
		{
			if (this.shieldHp > 0)
			{
				if (gameEntity != null)
				{
					GRAttributes component = gameEntity.GetComponent<GRAttributes>();
					if (component != null)
					{
						num = component.CalculateFinalValueForAttribute(GRAttributeType.PlayerShieldDamage);
					}
				}
				this.SetShieldHp(this.shieldHp - num);
				if (this.shieldHp > 0)
				{
					if (this.shieldDamagedSound != null)
					{
						this.audioSource.PlayOneShot(this.shieldDamagedSound, this.shieldDamagedVolume);
					}
					this.shieldDamagedEffect.Play();
				}
				else
				{
					if (this.shieldDestroyedSound != null)
					{
						this.audioSource.PlayOneShot(this.shieldDestroyedSound, this.shieldDestroyedVolume);
					}
					this.shieldDestroyedEffect.Play();
				}
				this.RefreshPlayerVisuals();
				return;
			}
			if (gameEntity != null)
			{
				GRAttributes component2 = gameEntity.GetComponent<GRAttributes>();
				if (component2 != null)
				{
					num = component2.CalculateFinalValueForAttribute(GRAttributeType.PlayerDamage);
				}
			}
			this.PlayHitFx(hitPosition);
			this.SetHp(this.hp - num);
			this.RefreshDamageVignetteVisual();
			if (this.hp <= 0)
			{
				this.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, manager);
			}
		}
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x000F3AD7 File Offset: 0x000F1CD7
	public void OnPlayerRevive(GhostReactorManager manager)
	{
		this.SetHp(this.maxHp);
		this.RefreshDamageVignetteVisual();
		this.ChangePlayerState(GRPlayer.GRPlayerState.Alive, manager);
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x000F3AF4 File Offset: 0x000F1CF4
	public void ChangePlayerState(GRPlayer.GRPlayerState newState, GhostReactorManager manager)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			newState = GRPlayer.GRPlayerState.Alive;
		}
		if (this.state == newState)
		{
			return;
		}
		this.state = newState;
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState != GRPlayer.GRPlayerState.Alive)
		{
			if (grplayerState == GRPlayer.GRPlayerState.Ghost)
			{
				this.SetHp(0);
				this.SetShieldHp(0);
				this.RefreshDamageVignetteVisual();
				if (this.playerTurnedGhostEffect != null)
				{
					this.playerTurnedGhostEffect.Play();
				}
				this.playerTurnedGhostSoundBank.Play();
				manager.ReportPlayerDeath(this);
				this.IncrementDeaths(1);
			}
		}
		else
		{
			this.SetHp(this.maxHp);
			this.RefreshDamageVignetteVisual();
			this.IncrementRevives(1);
			if (this.playerRevivedEffect != null)
			{
				this.playerRevivedEffect.Play();
			}
			if (this.audioSource != null && this.playerRevivedSound != null)
			{
				this.audioSource.PlayOneShot(this.playerRevivedSound, this.playerRevivedVolume);
			}
		}
		this.RefreshPlayerVisuals();
		if (this.vrRig.isLocal)
		{
			this.vrRigs.Clear();
			VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
			for (int i = 0; i < this.vrRigs.Count; i++)
			{
				this.vrRigs[i].GetComponent<GRPlayer>().RefreshPlayerVisuals();
			}
		}
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x000F3C3C File Offset: 0x000F1E3C
	public void RefreshPlayerVisuals()
	{
		this.RefreshDamageVignetteVisual();
		GRPlayer.GRPlayerState grplayerState = this.state;
		if (grplayerState == GRPlayer.GRPlayerState.Alive)
		{
			this.gamePlayer.DisableGrabbing(false);
			if (this.badge != null)
			{
				this.badge.UnHide();
			}
			this.vrRig.ChangeMaterialLocal(0);
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			if (this.vrRig.isLocal)
			{
				CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
				GameLightingManager.instance.SetDesaturateAndTintEnabled(false, Color.black);
				Color ambientLightDynamic = Color.black;
				GhostReactor instance = GhostReactor.instance;
				if (instance != null && instance.zone != GTZone.customMaps)
				{
					ambientLightDynamic = instance.GetCurrLevelGenConfig().ambientLight;
				}
				GameLightingManager.instance.SetAmbientLightDynamic(ambientLightDynamic);
			}
			if (this.shieldHp > 0)
			{
				this.shieldHeadVisual.gameObject.SetActive(true);
				this.shieldBodyVisual.gameObject.SetActive(true);
				Color color = this.shieldColorNormal;
				if ((this.shieldFlags & 1) != 0)
				{
					color = this.shieldColorLight;
				}
				else if ((this.shieldFlags & 2) != 0)
				{
					color = this.shieldColorStealth;
				}
				else if ((this.shieldFlags & 4) != 0)
				{
					color = this.shieldColorHeal;
				}
				Renderer component = this.shieldBodyVisual.GetComponent<Renderer>();
				if (component != null)
				{
					component.material.SetColor("_BaseColor", color);
				}
				Renderer component2 = this.shieldHeadVisual.GetComponent<Renderer>();
				if (component2 != null)
				{
					component2.material.SetColor("_BaseColor", color);
				}
			}
			else
			{
				this.shieldHeadVisual.gameObject.SetActive(false);
				this.shieldBodyVisual.gameObject.SetActive(false);
			}
			this.shieldGameLight.gameObject.SetActive((this.shieldFlags & 1) != 0);
			return;
		}
		if (grplayerState != GRPlayer.GRPlayerState.Ghost)
		{
			return;
		}
		if (this.vrRig.isLocal)
		{
			this.gamePlayer.RequestDropAllSnapped();
		}
		this.gamePlayer.DisableGrabbing(true);
		this.shieldHeadVisual.gameObject.SetActive(false);
		this.shieldBodyVisual.gameObject.SetActive(false);
		this.shieldGameLight.gameObject.SetActive(false);
		if (this.badge != null)
		{
			this.badge.Hide();
		}
		if (this.vrRig.isLocal)
		{
			GamePlayerLocal.instance.OnUpdateInteract();
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
			this.vrRig.ChangeMaterialLocal(13);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(true);
			GameLightingManager.instance.SetDesaturateAndTintEnabled(true, this.deathTintColor);
			GameLightingManager.instance.SetAmbientLightDynamic(this.deathAmbientLightColor);
			return;
		}
		if (VRRigCache.Instance.localRig.GetComponent<GRPlayer>().State == GRPlayer.GRPlayerState.Ghost)
		{
			this.vrRig.ChangeMaterialLocal(13);
			this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Skeleton);
			this.vrRig.SetInvisibleToLocalPlayer(false);
			return;
		}
		this.vrRig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Invisible);
		this.vrRig.SetInvisibleToLocalPlayer(true);
	}

	// Token: 0x06002D0D RID: 11533 RVA: 0x000F3F58 File Offset: 0x000F2158
	public static GRPlayer Get(int actorNumber)
	{
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(actorNumber, out gamePlayer))
		{
			return null;
		}
		return gamePlayer.GetComponent<GRPlayer>();
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x000F3F77 File Offset: 0x000F2177
	public static GRPlayer Get(NetPlayer player)
	{
		if (player == null)
		{
			return null;
		}
		return GRPlayer.Get(player.ActorNumber);
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x000F3F89 File Offset: 0x000F2189
	public static GRPlayer Get(VRRig vrRig)
	{
		if (!(vrRig != null))
		{
			return null;
		}
		return vrRig.GetComponent<GRPlayer>();
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x000F3F9C File Offset: 0x000F219C
	public static GRPlayer GetLocal()
	{
		return GRPlayer.Get(VRRig.LocalRig);
	}

	// Token: 0x06002D11 RID: 11537 RVA: 0x000F3FA8 File Offset: 0x000F21A8
	public void AttachBadge(GRBadge grBadge)
	{
		this.badge = grBadge;
		this.badge.transform.SetParent(this.badgeBodyAnchor);
		this.badge.GetComponent<Rigidbody>().isKinematic = true;
		this.badge.StartRetracting();
	}

	// Token: 0x06002D12 RID: 11538 RVA: 0x000F3FE3 File Offset: 0x000F21E3
	public bool CanActivateShield(int shieldHitPoints)
	{
		return this.state == GRPlayer.GRPlayerState.Alive && shieldHitPoints > 0;
	}

	// Token: 0x06002D13 RID: 11539 RVA: 0x000F3FF4 File Offset: 0x000F21F4
	public bool TryActivateShield(int shieldHitpoints, int shieldFlags)
	{
		if (this.state == GRPlayer.GRPlayerState.Alive)
		{
			if (this.shieldHp <= 0 && this.shieldActivatedSound != null)
			{
				this.audioSource.PlayOneShot(this.shieldActivatedSound, this.shieldActivatedVolume);
			}
			this.SetShieldHp(Mathf.Min(shieldHitpoints, this.maxShieldHp));
			this.shieldFlags = shieldFlags;
			this.inStealthMode = ((shieldFlags & 2) != 0);
			if (this.inStealthMode)
			{
				if (this.damageEffects.stealthModeVisualRenderer != null)
				{
					this.damageEffects.stealthModeVisualRenderer.gameObject.SetActive(true);
				}
				this.shieldStealthModeEndTime = Time.timeAsDouble + (double)this.shieldStealthModeDuration;
			}
			if ((shieldFlags & 4) != 0)
			{
				this.SetHp(this.maxHp);
			}
			this.RefreshPlayerVisuals();
			return true;
		}
		return false;
	}

	// Token: 0x06002D14 RID: 11540 RVA: 0x000F40BD File Offset: 0x000F22BD
	public void ClearStealthMode()
	{
		this.inStealthMode = false;
		if (this.damageEffects.stealthModeVisualRenderer != null)
		{
			this.damageEffects.stealthModeVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002D15 RID: 11541 RVA: 0x000F40F0 File Offset: 0x000F22F0
	public void SerializeNetworkState(BinaryWriter writer, NetPlayer player)
	{
		writer.Write((byte)this.state);
		writer.Write(this.hp);
		writer.Write(this.shieldHp);
		writer.Write(this.shiftJoinTime);
		writer.Write(this.isEmployee ? 1 : 0);
		writer.Write(this.CurrentProgression.points);
		writer.Write(this.CurrentProgression.redeemedPoints);
		writer.Write(this.dropPodLevel);
		writer.Write(this.dropPodChasisLevel);
		for (int i = 0; i < 8; i++)
		{
			writer.Write(this.synchronizedSessionStats[i]);
		}
	}

	// Token: 0x06002D16 RID: 11542 RVA: 0x000F4198 File Offset: 0x000F2398
	public static void DeserializeNetworkStateAndBurn(BinaryReader reader, GRPlayer player, GhostReactorManager grManager)
	{
		GRPlayer.GRPlayerState newState = (GRPlayer.GRPlayerState)reader.ReadByte();
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		double num3 = reader.ReadDouble();
		bool flag = reader.ReadByte() > 0;
		int points = reader.ReadInt32();
		int redeemedPoints = reader.ReadInt32();
		int num4 = reader.ReadInt32();
		int num5 = reader.ReadInt32();
		for (int i = 0; i < 8; i++)
		{
			player.synchronizedSessionStats[i] = reader.ReadSingle();
		}
		if (player != null)
		{
			player.SetHp(num);
			player.SetShieldHp(num2);
			player.isEmployee = flag;
			player.ChangePlayerState(newState, grManager);
			player.RefreshPlayerVisuals();
			if (!player.gamePlayer.IsLocal())
			{
				player.SetProgressionData(points, redeemedPoints, false);
				player.dropPodLevel = num4;
				player.dropPodChasisLevel = num5;
			}
			if (double.IsNaN(num3) || double.IsInfinity(num3))
			{
				player.shiftJoinTime = PhotonNetwork.Time;
			}
			else
			{
				player.shiftJoinTime = Math.Min(num3, PhotonNetwork.Time);
			}
		}
		if (grManager != null)
		{
			grManager.SendMothershipId();
		}
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x000F42A0 File Offset: 0x000F24A0
	public void PlayHitFx(Vector3 attackLocation)
	{
		if (this.playerDamageAudioSource != null)
		{
			this.playerDamageAudioSource.PlayOneShot(this.playerDamageSound, this.playerDamageVolume);
		}
		if (this.bodyCenter != null)
		{
			Vector3 vector = attackLocation - this.bodyCenter.position;
			vector.y = 0f;
			Vector3 vector2 = vector.normalized * this.playerDamageOffsetDist;
			if (this.playerDamageEffect != null)
			{
				this.playerDamageEffect.transform.position = this.bodyCenter.position + vector2;
				this.playerDamageEffect.Play();
			}
			if (this.vrRig.isLocal)
			{
				Vector3 normalized = Vector3.ProjectOnPlane(GTPlayer.Instance.mainCamera.transform.forward, Vector3.up).normalized;
				vector = Vector3.ProjectOnPlane(vector, Vector3.up).normalized;
				float num = Vector3.SignedAngle(normalized, vector, Vector3.up);
				this.damageEffects.radialDamageEffect.transform.localRotation = Quaternion.Euler(0f, 0f, -num);
				this.damageEffects.radialDamageEffect.Play();
			}
		}
		if (this.gamePlayer == GamePlayerLocal.instance.gamePlayer)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength, 0.5f);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength, 0.5f);
		}
	}

	// Token: 0x06002D18 RID: 11544 RVA: 0x000F4424 File Offset: 0x000F2624
	public void SendGameStartedTelemetry(float timeIntoShift, bool wasPlayerInAtStart, int currentFloor)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(GhostReactorProgression.GetTitleLevel(this.CurrentProgression.redeemedPoints));
		GorillaTelemetry.GhostReactorShiftStart(this.gameId, this.ShiftCredits, timeIntoShift, wasPlayerInAtStart, this.vrRigs.Count + 1, currentFloor, titleNameFromLevel);
		this.wasPlayerInAtShiftStart = wasPlayerInAtStart;
		this.ResetGameTelemetryTracking();
	}

	// Token: 0x06002D19 RID: 11545 RVA: 0x000F4494 File Offset: 0x000F2694
	public void SendGameEndedTelemetry(bool isShiftActuallyEnding, ZoneClearReason zoneClearReason)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		GorillaTelemetry.GhostReactorGameEnd(this.gameId, this.ShiftCredits, this.totalCoresCollectedByPlayer, this.totalCoresCollectedByGroup, this.totalCoresSpentByPlayer, this.totalCoresSpentByGroup, this.totalGatesUnlocked, this.totalDeaths, this.totalItemsPurchased, this.lastShiftCut, isShiftActuallyEnding, this.timeIntoShiftAtJoin, (float)(PhotonNetwork.Time - (double)this.gameStartTime), this.wasPlayerInAtShiftStart, zoneClearReason, this.maxNumberOfPlayersInShift, this.vrRigs.Count + 1, this.totalItemTypesHeldThisShift, this.totalRevives, this.numShiftsPlayed);
		this.isFirstShift = true;
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x000F4548 File Offset: 0x000F2748
	public void SendFloorStartedTelemetry(float timeIntoShift, bool wasPlayerInAtStart, int currentFloor, string floorPreset, string floorModifier)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		string titleNameFromLevel = GhostReactorProgression.GetTitleNameFromLevel(GhostReactorProgression.GetTitleLevel(this.CurrentProgression.redeemedPoints));
		GorillaTelemetry.GhostReactorFloorStart(this.gameId, this.ShiftCredits, timeIntoShift, wasPlayerInAtStart, this.vrRigs.Count + 1, titleNameFromLevel, currentFloor, floorPreset, floorModifier);
		this.wasPlayerInAtShiftStart = wasPlayerInAtStart;
	}

	// Token: 0x06002D1B RID: 11547 RVA: 0x000F45B4 File Offset: 0x000F27B4
	public void SendFloorEndedTelemetry(bool isShiftActuallyEnding, float shiftStartTime, ZoneClearReason zoneClearReason, int currentFloor, string floorPreset, string floorModifier, bool objectivesCompleted, string section, int xpGained)
	{
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		GorillaTelemetry.GhostReactorFloorComplete(this.gameId, this.ShiftCredits, this.coresCollectedByPlayer, this.coresCollectedByGroup, this.coresSpentByPlayer, this.coresSpentByGroup, this.gatesUnlocked, this.deaths, this.itemsPurchased, this.lastShiftCut, isShiftActuallyEnding, this.timeIntoShiftAtJoin, (float)(PhotonNetwork.Time - (double)(this.timeIntoShiftAtJoin + shiftStartTime)), this.wasPlayerInAtShiftStart, zoneClearReason, this.maxNumberOfPlayersInShift, this.vrRigs.Count + 1, this.itemTypesHeldThisShift, this.revives, currentFloor, floorPreset, floorModifier, this.sentientCoresCollected, objectivesCompleted, section, xpGained);
	}

	// Token: 0x06002D1C RID: 11548 RVA: 0x000F466C File Offset: 0x000F286C
	public void SendToolPurchasedTelemetry(string toolName, int toolLevel, int coresSpent, int shinyRocksSpent)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorToolPurchased(this.gameId, toolName, toolLevel, coresSpent, shinyRocksSpent, floor, preset);
	}

	// Token: 0x06002D1D RID: 11549 RVA: 0x000F46C0 File Offset: 0x000F28C0
	public void SendRankUpTelemetry(string newRank)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorRankUp(this.gameId, newRank, floor, preset);
	}

	// Token: 0x06002D1E RID: 11550 RVA: 0x000F4710 File Offset: 0x000F2910
	public void SendToolUpgradeTelemetry(string upgradeType, string toolName, int newLevel, int juiceSpent, int griftSpent, int coresSpent)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorToolUpgrade(this.gameId, upgradeType, toolName, newLevel, juiceSpent, griftSpent, coresSpent, floor, preset);
	}

	// Token: 0x06002D1F RID: 11551 RVA: 0x000F4768 File Offset: 0x000F2968
	public void SendSeedDepositedTelemetry(string unlockTime, int seedsInQueue)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorChaosSeedStart(this.gameId, unlockTime, seedsInQueue, floor, preset);
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x000F47B8 File Offset: 0x000F29B8
	public void SendJuiceCollectedTelemetry(int juiceCollected, int coresProcessedByOverdrive)
	{
		GorillaTelemetry.GhostReactorChaosJuiceCollected(this.gameId, juiceCollected, coresProcessedByOverdrive);
	}

	// Token: 0x06002D21 RID: 11553 RVA: 0x000F47C8 File Offset: 0x000F29C8
	public void SendOverdrivePurchasedTelemetry(int shinyRocksUsed, int seedsInQueue)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorOverdrivePurchased(this.gameId, shinyRocksUsed, seedsInQueue, floor, preset);
	}

	// Token: 0x06002D22 RID: 11554 RVA: 0x000F4818 File Offset: 0x000F2A18
	public void SendPodUpgradeTelemetry(string toolName, int level, int shinyRocksSpent, int juiceSpent)
	{
		GorillaTelemetry.GhostReactorPodUpgradePurchased(this.gameId, toolName, level, shinyRocksSpent, juiceSpent);
	}

	// Token: 0x06002D23 RID: 11555 RVA: 0x000F482C File Offset: 0x000F2A2C
	public void SendCreditsRefilledTelemetry(int shinyRocksSpent, int finalCredits)
	{
		int floor = -1;
		string preset = "";
		GhostReactor instance = GhostReactor.instance;
		if (instance != null && instance.zone != GTZone.customMaps)
		{
			floor = instance.GetDepthLevel();
			preset = instance.GetCurrLevelGenConfig().name;
		}
		GorillaTelemetry.GhostReactorCreditsRefillPurchased(this.gameId, shinyRocksSpent, finalCredits, floor, preset);
	}

	// Token: 0x06002D24 RID: 11556 RVA: 0x000F487C File Offset: 0x000F2A7C
	public void ResetTelemetryTracking(string newGameId, float timeSinceShiftStart)
	{
		this.gameId = newGameId;
		this.coresCollectedByPlayer = 0;
		this.coresCollectedByGroup = 0;
		this.gatesUnlocked = 0;
		this.deaths = 0;
		this.caughtByAnomaly = false;
		this.itemsPurchased = new List<string>();
		this.levelsUnlocked = new List<string>();
		this.sentientCoresCollected = 0;
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.maxNumberOfPlayersInShift = this.vrRigs.Count + 1;
		this.timeIntoShiftAtJoin = timeSinceShiftStart;
		this.itemsHeldThisShift.Clear();
		this.itemTypesHeldThisShift.Clear();
	}

	// Token: 0x06002D25 RID: 11557 RVA: 0x000F491C File Offset: 0x000F2B1C
	public void ResetGameTelemetryTracking()
	{
		this.totalCoresCollectedByPlayer = 0;
		this.totalCoresCollectedByGroup = 0;
		this.totalGatesUnlocked = 0;
		this.totalDeaths = 0;
		this.totalItemsPurchased = new List<string>();
		this.vrRigs.Clear();
		VRRigCache.Instance.GetAllUsedRigs(this.vrRigs);
		this.maxNumberOfPlayersIngame = this.vrRigs.Count + 1;
		this.totalItemsHeldThisShift.Clear();
		this.totalItemTypesHeldThisShift.Clear();
		this.numShiftsPlayed = 0;
		this.isFirstShift = false;
	}

	// Token: 0x06002D26 RID: 11558 RVA: 0x000F49A2 File Offset: 0x000F2BA2
	public void IncrementCoresCollectedPlayer(int coreValue)
	{
		this.totalCoresCollectedByPlayer += coreValue;
		this.coresCollectedByPlayer += coreValue;
	}

	// Token: 0x06002D27 RID: 11559 RVA: 0x000F49C0 File Offset: 0x000F2BC0
	public void IncrementCoresCollectedGroup(int coreValue)
	{
		this.totalCoresCollectedByGroup += coreValue;
		this.coresCollectedByGroup += coreValue;
	}

	// Token: 0x06002D28 RID: 11560 RVA: 0x000F49DE File Offset: 0x000F2BDE
	public void IncrementCoresSpentPlayer(int coreValue)
	{
		this.totalCoresSpentByPlayer += coreValue;
		this.coresSpentByPlayer += coreValue;
	}

	// Token: 0x06002D29 RID: 11561 RVA: 0x000F49FC File Offset: 0x000F2BFC
	public void IncrementCoresSpentGroup(int coreValue)
	{
		this.totalCoresSpentByGroup += coreValue;
		this.coresSpentByGroup += coreValue;
	}

	// Token: 0x06002D2A RID: 11562 RVA: 0x000F4A1A File Offset: 0x000F2C1A
	public void IncrementChaosSeedsCollected(int numSeeds)
	{
		this.sentientCoresCollected += numSeeds;
	}

	// Token: 0x06002D2B RID: 11563 RVA: 0x000F4A2A File Offset: 0x000F2C2A
	public void IncrementGatesUnlocked(int numGatesUnlocked)
	{
		this.gatesUnlocked += numGatesUnlocked;
		this.totalGatesUnlocked += numGatesUnlocked;
	}

	// Token: 0x06002D2C RID: 11564 RVA: 0x000F4A48 File Offset: 0x000F2C48
	public void IncrementDeaths(int numDeaths)
	{
		this.deaths += numDeaths;
		this.totalDeaths += numDeaths;
	}

	// Token: 0x06002D2D RID: 11565 RVA: 0x000F4A66 File Offset: 0x000F2C66
	public void IncrementRevives(int numRevives)
	{
		this.revives += numRevives;
		this.totalRevives += numRevives;
	}

	// Token: 0x06002D2E RID: 11566 RVA: 0x000F4A84 File Offset: 0x000F2C84
	public void IncrementShiftsPlayed(int numShifts)
	{
		this.numShiftsPlayed += numShifts;
	}

	// Token: 0x06002D2F RID: 11567 RVA: 0x000F4A94 File Offset: 0x000F2C94
	public void AddItemPurchased(string newItemPurchased)
	{
		this.itemsPurchased.Add(newItemPurchased);
		this.totalItemsPurchased.Add(newItemPurchased);
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x000F4AB0 File Offset: 0x000F2CB0
	public void GrabbedItem(GameEntityId id, string itemName)
	{
		if (this.itemsHeldThisShift.Contains(id))
		{
			return;
		}
		this.itemsHeldThisShift.Add(id);
		if (this.itemTypesHeldThisShift.ContainsKey(itemName))
		{
			this.itemTypesHeldThisShift[itemName] = this.itemTypesHeldThisShift[itemName] + 1;
		}
		else
		{
			this.itemTypesHeldThisShift[itemName] = 1;
		}
		if (this.totalItemsHeldThisShift.Contains(id))
		{
			return;
		}
		this.totalItemsHeldThisShift.Add(id);
		if (this.totalItemTypesHeldThisShift.ContainsKey(itemName))
		{
			this.totalItemTypesHeldThisShift[itemName] = this.totalItemTypesHeldThisShift[itemName] + 1;
			return;
		}
		this.totalItemTypesHeldThisShift[itemName] = 1;
	}

	// Token: 0x06002D31 RID: 11569 RVA: 0x000F4B64 File Offset: 0x000F2D64
	public GRShuttle GetAssignedShuttle(bool isOnDrillovator)
	{
		GhostReactor instance = GhostReactor.instance;
		GRShuttle drillShuttleForPlayer = GRElevatorManager._instance.GetDrillShuttleForPlayer(this.gamePlayer.rig.OwningNetPlayer.ActorNumber);
		GRShuttle stagingShuttleForPlayer = GRElevatorManager._instance.GetStagingShuttleForPlayer(this.gamePlayer.rig.OwningNetPlayer.ActorNumber);
		if (!isOnDrillovator)
		{
			return stagingShuttleForPlayer;
		}
		return drillShuttleForPlayer;
	}

	// Token: 0x06002D32 RID: 11570 RVA: 0x000F4BC0 File Offset: 0x000F2DC0
	public void RefreshShuttles()
	{
		GRShuttle assignedShuttle = this.GetAssignedShuttle(true);
		if (assignedShuttle != null)
		{
			assignedShuttle.Refresh();
		}
		assignedShuttle = this.GetAssignedShuttle(false);
		if (assignedShuttle != null)
		{
			assignedShuttle.Refresh();
		}
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x000F4BFC File Offset: 0x000F2DFC
	public static GRPlayer GetFromUserId(string userId)
	{
		GRPlayer.tempRigs.Clear();
		GRPlayer.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GRPlayer.tempRigs);
		for (int i = 0; i < GRPlayer.tempRigs.Count; i++)
		{
			if (GRPlayer.tempRigs[i].OwningNetPlayer != null && GRPlayer.tempRigs[i].OwningNetPlayer.UserId == userId)
			{
				return GRPlayer.Get(GRPlayer.tempRigs[i].OwningNetPlayer);
			}
		}
		return null;
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x000F4C8C File Offset: 0x000F2E8C
	[ContextMenu("Refresh Damage Vignette Visual")]
	public void RefreshDamageVignetteVisual()
	{
		if (this.vrRig.isLocal && this.currentHealthVisualValue != this.hp)
		{
			this.currentHealthVisualValue = this.hp;
			if (this.hp <= this.damageOverlayMaxHp && this.hp > 0)
			{
				if (this.lowHeathVisualCoroutine != null)
				{
					base.StopCoroutine(this.lowHeathVisualCoroutine);
				}
				this.damageEffects.lowHealthVisualRenderer.gameObject.SetActive(true);
				this.lowHeathVisualCoroutine = base.StartCoroutine(this.LowHeathVisualCoroutine());
				return;
			}
			this.damageEffects.lowHealthVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x000F4D2D File Offset: 0x000F2F2D
	private IEnumerator LowHeathVisualCoroutine()
	{
		int index = this.hp - 1;
		if (index >= 0 && index < this.damageOverlayValues.Count)
		{
			float startTime = Time.time;
			while (Time.time - startTime < this.damageOverlayValues[index].effectDuration)
			{
				float num = Mathf.Clamp01((Time.time - startTime) / this.damageOverlayValues[index].effectDuration);
				float num2 = this.damageOverlayValues[index].effectCurve.Evaluate(num);
				Color tint = this.damageOverlayValues[index].tint;
				tint.a *= num2;
				this.damageEffects.lowHealthVisualRenderer.GetPropertyBlock(this.lowHealthVisualPropertyBlock);
				this.lowHealthVisualPropertyBlock.SetColor(this.lowHealthTintPropertyId, tint);
				this.damageEffects.lowHealthVisualRenderer.SetPropertyBlock(this.lowHealthVisualPropertyBlock);
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x000F4D3C File Offset: 0x000F2F3C
	public void SetGooParticleSystemEnabled(bool bIsLeftHand, bool newEnableState)
	{
		if (this.vrRig != null)
		{
			this.vrRig.SetGooParticleSystemStatus(bIsLeftHand, newEnableState);
		}
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x000F4D5C File Offset: 0x000F2F5C
	public void SetAsFrozen(float duration)
	{
		if (GorillaTagger.Instance.currentStatus != GorillaTagger.StatusEffect.Frozen)
		{
			this.freezeDuration = duration;
			if (this.gamePlayer.rig.OwningNetPlayer.IsLocal)
			{
				GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, duration);
				GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.taggedHapticStrength, GorillaTagger.Instance.taggedHapticDuration);
				GorillaTagger.Instance.offlineVRRig.PlayTaggedEffect();
				if (this.damageEffects.frozenVisualRenderer != null)
				{
					this.damageEffects.frozenVisualRenderer.gameObject.SetActive(true);
				}
				this.playerDamageAudioSource.PlayOneShot(this.playerFrozenSound, 1f);
			}
			this.gamePlayer.rig.UpdateFrozenEffect(true);
			base.Invoke("RemoveFrozen", duration);
		}
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x000F4E50 File Offset: 0x000F3050
	public void RemoveFrozen()
	{
		this.gamePlayer.rig.UpdateFrozenEffect(false);
		this.freezeDuration = 0f;
		if (this.damageEffects.frozenVisualRenderer != null)
		{
			this.damageEffects.frozenVisualRenderer.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x000F4EA4 File Offset: 0x000F30A4
	public override void Tick()
	{
		if (this.lastPlayerPosition != Vector3.zero)
		{
			Vector3 position = this.vrRig.transform.position;
			float magnitude = (this.lastPlayerPosition - position).magnitude;
			this.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.DistanceTraveled, magnitude);
		}
		this.lastPlayerPosition = this.vrRig.transform.position;
		if (this.freezeDuration > 0f)
		{
			this.gamePlayer.rig.UpdateFrozen(Time.deltaTime, this.freezeDuration);
		}
		if (this.inStealthMode && Time.timeAsDouble > this.shieldStealthModeEndTime)
		{
			this.ClearStealthMode();
		}
		GRShuttle.UpdateGRPlayerShuttle(this);
		if (this.soak != null && this.soak.IsSoaking())
		{
			this.soak.OnUpdate();
		}
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x000F4F70 File Offset: 0x000F3170
	public void SetSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat stat, float amt)
	{
		this.synchronizedSessionStats[(int)stat] = amt;
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x000F4F7B File Offset: 0x000F317B
	public void IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat stat, float amt)
	{
		this.synchronizedSessionStats[(int)stat] += amt;
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x000F4F90 File Offset: 0x000F3190
	public void ResetSynchronizedSessionStats()
	{
		for (int i = 0; i < 8; i++)
		{
			this.synchronizedSessionStats[i] = 0f;
		}
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x000F4FB6 File Offset: 0x000F31B6
	public bool IsDropPodUnlocked()
	{
		return this.dropPodLevel > 0;
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x000F4FC4 File Offset: 0x000F31C4
	public int GetMaxDropFloor()
	{
		switch (this.dropPodChasisLevel + this.dropPodLevel)
		{
		case 0:
			return 1;
		case 1:
			return 5;
		case 2:
			return 10;
		case 3:
			return 15;
		case 4:
			return 20;
		default:
			return 0;
		}
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x000F5009 File Offset: 0x000F3209
	public void CollectShiftCut()
	{
		this.SetProgressionData(this.currentProgression.points + this.LastShiftCut, this.currentProgression.redeemedPoints, true);
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x000F5030 File Offset: 0x000F3230
	public bool AttemptPromotion()
	{
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(this.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item3;
		int item2 = gradePointDetails.Item4;
		if (item - item2 < this.CurrentProgression.points - this.CurrentProgression.redeemedPoints)
		{
			this.SetProgressionData(this.currentProgression.points, this.currentProgression.points, false);
			return true;
		}
		return false;
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x000F5098 File Offset: 0x000F3298
	public void SetProgressionData(int _points, int _redeemedPoints, bool saveProgression = false)
	{
		if (_points < 0 || _redeemedPoints < 0)
		{
			return;
		}
		this.currentProgression = new GRPlayer.ProgressionData
		{
			points = _points,
			redeemedPoints = _redeemedPoints
		};
		if (this.gamePlayer.IsLocal() && saveProgression)
		{
			this.SaveMyProgression();
		}
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x000F50E2 File Offset: 0x000F32E2
	public void LoadMyProgression()
	{
		GhostReactorProgression.instance.GetStartingProgression(this);
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x000F50EF File Offset: 0x000F32EF
	public void SaveMyProgression()
	{
		GhostReactorProgression.instance.SetProgression(this.LastShiftCut, this);
	}

	// Token: 0x04003A4A RID: 14922
	public const int MAX_CURRENCY = 500;

	// Token: 0x04003A4B RID: 14923
	public GamePlayer gamePlayer;

	// Token: 0x04003A4C RID: 14924
	private GRPlayer.GRPlayerState state;

	// Token: 0x04003A4D RID: 14925
	private int shiftCreditCache;

	// Token: 0x04003A4E RID: 14926
	public int startingShiftCreditCache;

	// Token: 0x04003A4F RID: 14927
	public int playerJuice;

	// Token: 0x04003A52 RID: 14930
	public double shiftJoinTime;

	// Token: 0x04003A53 RID: 14931
	public bool isEmployee;

	// Token: 0x04003A54 RID: 14932
	public AudioSource audioSource;

	// Token: 0x04003A55 RID: 14933
	[Header("Hit / Revive Effects")]
	public ParticleSystem playerTurnedGhostEffect;

	// Token: 0x04003A56 RID: 14934
	public SoundBankPlayer playerTurnedGhostSoundBank;

	// Token: 0x04003A57 RID: 14935
	public ParticleSystem playerRevivedEffect;

	// Token: 0x04003A58 RID: 14936
	public AudioClip playerRevivedSound;

	// Token: 0x04003A59 RID: 14937
	public float playerRevivedVolume = 1f;

	// Token: 0x04003A5A RID: 14938
	public AudioSource playerDamageAudioSource;

	// Token: 0x04003A5B RID: 14939
	public Transform bodyCenter;

	// Token: 0x04003A5C RID: 14940
	public ParticleSystem playerDamageEffect;

	// Token: 0x04003A5D RID: 14941
	public float playerDamageVolume = 1f;

	// Token: 0x04003A5E RID: 14942
	public AudioClip playerDamageSound;

	// Token: 0x04003A5F RID: 14943
	public float playerDamageOffsetDist = 0.25f;

	// Token: 0x04003A60 RID: 14944
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathTintColor;

	// Token: 0x04003A61 RID: 14945
	[ColorUsage(true, true)]
	[SerializeField]
	private Color deathAmbientLightColor;

	// Token: 0x04003A62 RID: 14946
	public GameLight shieldGameLight;

	// Token: 0x04003A63 RID: 14947
	[Header("Attach")]
	public Transform attachEnemy;

	// Token: 0x04003A64 RID: 14948
	[Header("Shield")]
	public Transform shieldHeadVisual;

	// Token: 0x04003A65 RID: 14949
	public Transform shieldBodyVisual;

	// Token: 0x04003A66 RID: 14950
	public AudioClip shieldActivatedSound;

	// Token: 0x04003A67 RID: 14951
	public float shieldActivatedVolume = 0.5f;

	// Token: 0x04003A68 RID: 14952
	public ParticleSystem shieldDamagedEffect;

	// Token: 0x04003A69 RID: 14953
	public AudioClip shieldDamagedSound;

	// Token: 0x04003A6A RID: 14954
	public float shieldDamagedVolume = 0.5f;

	// Token: 0x04003A6B RID: 14955
	public ParticleSystem shieldDestroyedEffect;

	// Token: 0x04003A6C RID: 14956
	public AudioClip shieldDestroyedSound;

	// Token: 0x04003A6D RID: 14957
	public float shieldDestroyedVolume = 0.5f;

	// Token: 0x04003A6E RID: 14958
	public float shieldStealthModeDuration = 20f;

	// Token: 0x04003A6F RID: 14959
	private double shieldStealthModeEndTime;

	// Token: 0x04003A70 RID: 14960
	public Color shieldColorNormal = new Color(0.42352942f, 0.25490198f, 1f, 0.45490196f);

	// Token: 0x04003A71 RID: 14961
	public Color shieldColorLight = new Color(1f, 1f, 1f, 0.5f);

	// Token: 0x04003A72 RID: 14962
	public Color shieldColorStealth = new Color(1f, 0.2f, 0f, 0.5f);

	// Token: 0x04003A73 RID: 14963
	public Color shieldColorHeal = new Color(0f, 1f, 1f, 0.5f);

	// Token: 0x04003A74 RID: 14964
	public int xRayVisionRefCount;

	// Token: 0x04003A75 RID: 14965
	[Header("Badge")]
	public Transform badgeBodyAnchor;

	// Token: 0x04003A76 RID: 14966
	[SerializeField]
	private Transform badgeBodyStringAttach;

	// Token: 0x04003A77 RID: 14967
	[NonSerialized]
	public double lastLeftWithBadgeAttachedTime;

	// Token: 0x04003A78 RID: 14968
	[Header("Health")]
	[SerializeField]
	private int maxHp = 1;

	// Token: 0x04003A79 RID: 14969
	[SerializeField]
	private int maxShieldHp = 1;

	// Token: 0x04003A7A RID: 14970
	public string mothershipId;

	// Token: 0x04003A7B RID: 14971
	private int hp;

	// Token: 0x04003A7C RID: 14972
	private int shieldHp;

	// Token: 0x04003A7D RID: 14973
	private int shieldFlags;

	// Token: 0x04003A7E RID: 14974
	private bool inStealthMode;

	// Token: 0x04003A7F RID: 14975
	[Header("Damage Vignette")]
	[SerializeField]
	[Tooltip("First entry is 1 hp, second entry is 2 hp, etc.")]
	private List<GRPlayer.DamageOverlayValues> damageOverlayValues = new List<GRPlayer.DamageOverlayValues>();

	// Token: 0x04003A80 RID: 14976
	[SerializeField]
	private int damageOverlayMaxHp = 1;

	// Token: 0x04003A81 RID: 14977
	[HideInInspector]
	public GRBadge badge;

	// Token: 0x04003A82 RID: 14978
	public CallLimiter requestCollectItemLimiter;

	// Token: 0x04003A83 RID: 14979
	public CallLimiter requestChargeToolLimiter;

	// Token: 0x04003A84 RID: 14980
	public CallLimiter requestDepositCurrencyLimiter;

	// Token: 0x04003A85 RID: 14981
	public CallLimiter requestShiftStartLimiter;

	// Token: 0x04003A86 RID: 14982
	public CallLimiter requestToolPurchaseStationLimiter;

	// Token: 0x04003A87 RID: 14983
	public CallLimiter applyEnemyHitLimiter;

	// Token: 0x04003A88 RID: 14984
	public CallLimiter reportLocalHitLimiter;

	// Token: 0x04003A89 RID: 14985
	public CallLimiter reportBreakableBrokenLimiter;

	// Token: 0x04003A8A RID: 14986
	public CallLimiter playerStateChangeLimiter;

	// Token: 0x04003A8B RID: 14987
	public CallLimiter promotionBotLimiter;

	// Token: 0x04003A8C RID: 14988
	public CallLimiter progressionBroadcastLimiter;

	// Token: 0x04003A8D RID: 14989
	public CallLimiter scoreboardPageLimiter;

	// Token: 0x04003A8E RID: 14990
	public CallLimiter fireShieldLimiter;

	// Token: 0x04003A8F RID: 14991
	private VRRig vrRig;

	// Token: 0x04003A90 RID: 14992
	private List<VRRig> vrRigs = new List<VRRig>();

	// Token: 0x04003A91 RID: 14993
	private string gameId;

	// Token: 0x04003A92 RID: 14994
	public int coresCollectedByPlayer;

	// Token: 0x04003A93 RID: 14995
	public int coresCollectedByGroup;

	// Token: 0x04003A94 RID: 14996
	public int coresSpentByPlayer;

	// Token: 0x04003A95 RID: 14997
	public int coresSpentByGroup;

	// Token: 0x04003A96 RID: 14998
	public int gatesUnlocked;

	// Token: 0x04003A97 RID: 14999
	public int deaths;

	// Token: 0x04003A98 RID: 15000
	public bool caughtByAnomaly;

	// Token: 0x04003A99 RID: 15001
	public List<string> itemsPurchased;

	// Token: 0x04003A9A RID: 15002
	public List<string> levelsUnlocked;

	// Token: 0x04003A9B RID: 15003
	public float timeIntoShiftAtJoin;

	// Token: 0x04003A9C RID: 15004
	public bool wasPlayerInAtShiftStart;

	// Token: 0x04003A9D RID: 15005
	public int sentientCoresCollected;

	// Token: 0x04003A9E RID: 15006
	public int maxNumberOfPlayersInShift;

	// Token: 0x04003A9F RID: 15007
	public int revives;

	// Token: 0x04003AA0 RID: 15008
	public float[] synchronizedSessionStats = new float[8];

	// Token: 0x04003AA1 RID: 15009
	private HashSet<GameEntityId> itemsHeldThisShift = new HashSet<GameEntityId>();

	// Token: 0x04003AA2 RID: 15010
	private Dictionary<string, int> itemTypesHeldThisShift = new Dictionary<string, int>();

	// Token: 0x04003AA3 RID: 15011
	public int totalCoresCollectedByPlayer;

	// Token: 0x04003AA4 RID: 15012
	public int totalCoresCollectedByGroup;

	// Token: 0x04003AA5 RID: 15013
	public int totalCoresSpentByPlayer;

	// Token: 0x04003AA6 RID: 15014
	public int totalCoresSpentByGroup;

	// Token: 0x04003AA7 RID: 15015
	public int totalGatesUnlocked;

	// Token: 0x04003AA8 RID: 15016
	public int totalDeaths;

	// Token: 0x04003AA9 RID: 15017
	public List<string> totalItemsPurchased;

	// Token: 0x04003AAA RID: 15018
	public float timeIntoGameAtJoin;

	// Token: 0x04003AAB RID: 15019
	public bool wasPlayerInAtGameStart;

	// Token: 0x04003AAC RID: 15020
	public int maxNumberOfPlayersIngame;

	// Token: 0x04003AAD RID: 15021
	public int totalRevives;

	// Token: 0x04003AAE RID: 15022
	public int numShiftsPlayed;

	// Token: 0x04003AAF RID: 15023
	public float gameStartTime;

	// Token: 0x04003AB0 RID: 15024
	public bool isFirstShift = true;

	// Token: 0x04003AB1 RID: 15025
	private HashSet<GameEntityId> totalItemsHeldThisShift = new HashSet<GameEntityId>();

	// Token: 0x04003AB2 RID: 15026
	private Dictionary<string, int> totalItemTypesHeldThisShift = new Dictionary<string, int>();

	// Token: 0x04003AB3 RID: 15027
	private GRPlayerDamageEffects damageEffects;

	// Token: 0x04003AB4 RID: 15028
	private MaterialPropertyBlock lowHealthVisualPropertyBlock;

	// Token: 0x04003AB5 RID: 15029
	private int lowHealthTintPropertyId;

	// Token: 0x04003AB6 RID: 15030
	private int currentHealthVisualValue;

	// Token: 0x04003AB7 RID: 15031
	private Coroutine lowHeathVisualCoroutine;

	// Token: 0x04003AB8 RID: 15032
	public AudioClip playerFrozenSound;

	// Token: 0x04003AB9 RID: 15033
	public GRPlayer.ShuttleData shuttleData;

	// Token: 0x04003ABA RID: 15034
	private GRPlayer.ProgressionData currentProgression;

	// Token: 0x04003ABB RID: 15035
	private float shiftPlayTime;

	// Token: 0x04003ABC RID: 15036
	private int lastShiftCut;

	// Token: 0x04003ABD RID: 15037
	private GhostReactorSoak soak;

	// Token: 0x04003ABE RID: 15038
	private static List<VRRig> tempRigs = new List<VRRig>(32);

	// Token: 0x04003ABF RID: 15039
	private float freezeDuration;

	// Token: 0x04003AC0 RID: 15040
	private Vector3 lastPlayerPosition = Vector3.zero;

	// Token: 0x04003AC1 RID: 15041
	public int dropPodLevel;

	// Token: 0x04003AC2 RID: 15042
	public int dropPodChasisLevel;

	// Token: 0x020006DD RID: 1757
	public enum GRPlayerState
	{
		// Token: 0x04003AC4 RID: 15044
		Alive,
		// Token: 0x04003AC5 RID: 15045
		Ghost,
		// Token: 0x04003AC6 RID: 15046
		Shielded
	}

	// Token: 0x020006DE RID: 1758
	public enum GRPlayerShieldFlags
	{
		// Token: 0x04003AC8 RID: 15048
		Light = 1,
		// Token: 0x04003AC9 RID: 15049
		Stealth,
		// Token: 0x04003ACA RID: 15050
		Heal = 4
	}

	// Token: 0x020006DF RID: 1759
	public enum SynchronizedSessionStat
	{
		// Token: 0x04003ACC RID: 15052
		CoresDeposited,
		// Token: 0x04003ACD RID: 15053
		EarnedCredits,
		// Token: 0x04003ACE RID: 15054
		SpentCredits,
		// Token: 0x04003ACF RID: 15055
		DistanceTraveled,
		// Token: 0x04003AD0 RID: 15056
		Deaths,
		// Token: 0x04003AD1 RID: 15057
		Kills,
		// Token: 0x04003AD2 RID: 15058
		Assists,
		// Token: 0x04003AD3 RID: 15059
		TimeChaosExposure,
		// Token: 0x04003AD4 RID: 15060
		Count
	}

	// Token: 0x020006E0 RID: 1760
	[Serializable]
	private struct DamageOverlayValues
	{
		// Token: 0x04003AD5 RID: 15061
		public Color tint;

		// Token: 0x04003AD6 RID: 15062
		public float effectDuration;

		// Token: 0x04003AD7 RID: 15063
		public AnimationCurve effectCurve;
	}

	// Token: 0x020006E1 RID: 1761
	public enum ShuttleState
	{
		// Token: 0x04003AD9 RID: 15065
		Idle,
		// Token: 0x04003ADA RID: 15066
		Moving,
		// Token: 0x04003ADB RID: 15067
		WaitForLeaveRoom,
		// Token: 0x04003ADC RID: 15068
		JoinRoom,
		// Token: 0x04003ADD RID: 15069
		WaitForLeadPlayer,
		// Token: 0x04003ADE RID: 15070
		Teleport,
		// Token: 0x04003ADF RID: 15071
		TeleportToMyShuttleSafety,
		// Token: 0x04003AE0 RID: 15072
		PostTeleport
	}

	// Token: 0x020006E2 RID: 1762
	public class ShuttleData
	{
		// Token: 0x04003AE1 RID: 15073
		public string ownerUserId;

		// Token: 0x04003AE2 RID: 15074
		public int currShuttleId;

		// Token: 0x04003AE3 RID: 15075
		public int targetShuttleId;

		// Token: 0x04003AE4 RID: 15076
		public int targetLevel;

		// Token: 0x04003AE5 RID: 15077
		public GRPlayer.ShuttleState state;

		// Token: 0x04003AE6 RID: 15078
		public double stateStartTime;
	}

	// Token: 0x020006E3 RID: 1763
	[Serializable]
	public struct ProgressionData
	{
		// Token: 0x04003AE7 RID: 15079
		public int points;

		// Token: 0x04003AE8 RID: 15080
		public int redeemedPoints;
	}

	// Token: 0x020006E4 RID: 1764
	[Serializable]
	public struct ProgressionLevels
	{
		// Token: 0x04003AE9 RID: 15081
		public int tierId;

		// Token: 0x04003AEA RID: 15082
		public string tierName;

		// Token: 0x04003AEB RID: 15083
		public int grades;

		// Token: 0x04003AEC RID: 15084
		public int pointsPerGrade;
	}
}
