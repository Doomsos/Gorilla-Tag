using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x02000203 RID: 515
public sealed class GorillaPropHuntGameManager : GorillaTagManager
{
	// Token: 0x1700014E RID: 334
	// (get) Token: 0x06000DFE RID: 3582 RVA: 0x0004A12E File Offset: 0x0004832E
	// (set) Token: 0x06000DFF RID: 3583 RVA: 0x0004A135 File Offset: 0x00048335
	public new static GorillaPropHuntGameManager instance { get; private set; }

	// Token: 0x06000E00 RID: 3584 RVA: 0x00006AE5 File Offset: 0x00004CE5
	public override GameModeType GameType()
	{
		return GameModeType.PropHunt;
	}

	// Token: 0x06000E01 RID: 3585 RVA: 0x0004A13D File Offset: 0x0004833D
	public override string GameModeName()
	{
		return "PROP HUNT";
	}

	// Token: 0x06000E02 RID: 3586 RVA: 0x0004A144 File Offset: 0x00048344
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_PROP_HUNT_ROOM_LABEL", out result, "(PROP HUNT GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_PROP_HUNT_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x1700014F RID: 335
	// (get) Token: 0x06000E03 RID: 3587 RVA: 0x0004A16F File Offset: 0x0004836F
	public PropPlacementRB PropDecoyPrefab
	{
		get
		{
			return this.m_ph_propDecoyPrefab;
		}
	}

	// Token: 0x17000150 RID: 336
	// (get) Token: 0x06000E04 RID: 3588 RVA: 0x0004A177 File Offset: 0x00048377
	public float HandFollowDistance
	{
		get
		{
			return this.m_ph_hand_follow_distance;
		}
	}

	// Token: 0x17000151 RID: 337
	// (get) Token: 0x06000E05 RID: 3589 RVA: 0x0004A17F File Offset: 0x0004837F
	public bool RoundIsPlaying
	{
		get
		{
			return this._roundIsPlaying;
		}
	}

	// Token: 0x17000152 RID: 338
	// (get) Token: 0x06000E06 RID: 3590 RVA: 0x0004A187 File Offset: 0x00048387
	public string[] AllPropIDs_NoPool
	{
		get
		{
			return PropHuntPools.AllPropCosmeticIds;
		}
	}

	// Token: 0x17000153 RID: 339
	// (get) Token: 0x06000E07 RID: 3591 RVA: 0x0004A18E File Offset: 0x0004838E
	// (set) Token: 0x06000E08 RID: 3592 RVA: 0x0004A196 File Offset: 0x00048396
	[DebugReadout]
	private long _ph_timeRoundStartedMillis
	{
		get
		{
			return this.__ph_timeRoundStartedMillis__;
		}
		set
		{
			this.__ph_timeRoundStartedMillis__ = value;
		}
	}

	// Token: 0x06000E09 RID: 3593 RVA: 0x0004A19F File Offset: 0x0004839F
	public int GetSeed()
	{
		return this._ph_randomSeed;
	}

	// Token: 0x06000E0A RID: 3594 RVA: 0x0004A1A7 File Offset: 0x000483A7
	public override void Awake()
	{
		GorillaPropHuntGameManager.instance = this;
		PhotonNetwork.AddCallbackTarget(this);
		base.Awake();
	}

	// Token: 0x06000E0B RID: 3595 RVA: 0x0004A1BB File Offset: 0x000483BB
	private void Start()
	{
		PropHuntPools.StartInitializingPropsList(this.m_ph_allCosmetics, this.m_ph_fallbackPropCosmeticSO);
		if (this._ph_gorillaGhostBodyMaterialIndex == -1)
		{
			this._Initialize_gorillaGhostBodyMaterialIndex();
		}
		this._Initialize_defaultStencilRefOfSkeletonMat();
	}

	// Token: 0x17000154 RID: 340
	// (get) Token: 0x06000E0C RID: 3596 RVA: 0x0004A1E3 File Offset: 0x000483E3
	public bool IsReadyToSpawnProps_NoPool
	{
		get
		{
			return PropHuntPools.IsReady;
		}
	}

	// Token: 0x06000E0D RID: 3597 RVA: 0x0004A1EA File Offset: 0x000483EA
	private void _ProcessPropsList_NoPool(string titleDataPropsLines)
	{
		this._ph_allPropIDs_noPool = titleDataPropsLines.Split(GorillaPropHuntGameManager._g_ph_titleDataSeparators, 1);
	}

	// Token: 0x06000E0E RID: 3598 RVA: 0x0004A200 File Offset: 0x00048400
	public override void StartPlaying()
	{
		base.StartPlaying();
		bool isMasterClient = PhotonNetwork.IsMasterClient;
		this._ResolveXSceneRefs();
		GameMode.ParticipatingPlayersChanged += new Action<List<NetPlayer>, List<NetPlayer>>(this._OnParticipatingPlayersChanged);
		this._UpdateParticipatingPlayers();
		if (this.m_ph_soundNearBorder_audioSource != null)
		{
			this.m_ph_soundNearBorder_audioSource.volume = 0f;
		}
	}

	// Token: 0x06000E0F RID: 3599 RVA: 0x0004A254 File Offset: 0x00048454
	public override void StopPlaying()
	{
		base.StopPlaying();
		this._ph_gameState = GorillaPropHuntGameManager.EPropHuntGameState.StoppedGameMode;
		GameMode.ParticipatingPlayersChanged -= new Action<List<NetPlayer>, List<NetPlayer>>(this._OnParticipatingPlayersChanged);
		foreach (VRRig rig in GorillaParent.instance.vrrigs)
		{
			GorillaSkin.ApplyToRig(rig, null, GorillaSkin.SkinType.gameMode);
			this._ResetRigAppearance(rig);
		}
		CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
		EquipmentInteractor.instance.ForceDropAnyEquipment();
		if (this.m_ph_soundNearBorder_audioSource != null)
		{
			this.m_ph_soundNearBorder_audioSource.volume = 0f;
		}
		if (this._ph_playBoundary_isResolved)
		{
			this._ph_playBoundary.enabled = false;
			if (this._ph_playBoundary_initialPosition_isInitialized)
			{
				this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
			}
		}
		this._ph_playBoundary_hasTargetPositionForRound = false;
	}

	// Token: 0x06000E10 RID: 3600 RVA: 0x0004A344 File Offset: 0x00048544
	public override bool CanPlayerParticipate(NetPlayer player)
	{
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			VRRig rig = rigContainer.Rig;
			return rig.zoneEntity.currentZone == GTZone.bayou && rig.zoneEntity.currentSubZone != GTSubZone.entrance_tunnel;
		}
		return true;
	}

	// Token: 0x06000E11 RID: 3601 RVA: 0x0004A38C File Offset: 0x0004858C
	private void _OnParticipatingPlayersChanged(List<NetPlayer> addedPlayers, List<NetPlayer> removedPlayers)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			for (int i = 0; i < addedPlayers.Count; i++)
			{
				NetPlayer infectedPlayer = addedPlayers[i];
				this.AddInfectedPlayer(infectedPlayer, true);
			}
		}
		for (int j = 0; j < removedPlayers.Count; j++)
		{
			NetPlayer netPlayer = removedPlayers[j];
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				if (PhotonNetwork.IsMasterClient)
				{
					while (this.currentInfected.Contains(netPlayer))
					{
						this.currentInfected.Remove(netPlayer);
					}
				}
				VRRig rig = rigContainer.Rig;
				this._ResetRigAppearance(rig);
			}
		}
		if (PhotonNetwork.IsMasterClient)
		{
			this.UpdateInfectionState();
		}
	}

	// Token: 0x06000E12 RID: 3602 RVA: 0x0004A42B File Offset: 0x0004862B
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			bool isCurrentlyTag = this.isCurrentlyTag;
			this.UpdateState();
			if (!isCurrentlyTag && !this.isCurrentlyTag)
			{
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x06000E13 RID: 3603 RVA: 0x0004A458 File Offset: 0x00048658
	public override void Tick()
	{
		base.Tick();
		this._UpdateParticipatingPlayers();
		this._UpdateGameState();
		if (this._ph_playBoundary_isResolved)
		{
			this._ph_playBoundary.enabled = this._ph_isLocalPlayerParticipating;
			float num = (this._ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Playing) ? 0f : Mathf.Clamp01(this._ph_roundTime / this.m_ph_playBoundary_radiusScaleOverRoundTime_maxTime);
			this._ph_playBoundary.radiusScale = this.m_ph_playBoundary_radiusScaleOverRoundTime_curve.Evaluate(num);
			if (this._ph_playBoundary_hasTargetPositionForRound)
			{
				Vector3 position = Vector3.Lerp(this._ph_playBoundary_initialPosition, this._ph_playBoundary_currentTargetPosition, num);
				this._ph_playBoundary.transform.position = position;
			}
			if (this._ph_isLocalPlayerParticipating || (PhotonNetwork.IsMasterClient && GameMode.ParticipatingPlayers.Count > 0))
			{
				this._ph_playBoundary.UpdateSim();
			}
		}
	}

	// Token: 0x06000E14 RID: 3604 RVA: 0x0004A520 File Offset: 0x00048720
	public void _UpdateParticipatingPlayers()
	{
		VRRigCache.Instance.GetActiveRigs(GorillaPropHuntGameManager._g_ph_activePlayerRigs);
		for (int i = 0; i < GorillaPropHuntGameManager._g_ph_activePlayerRigs.Count; i++)
		{
			VRRig vrrig = GorillaPropHuntGameManager._g_ph_activePlayerRigs[i];
			bool flag = vrrig.zoneEntity.currentZone == GTZone.bayou && vrrig.zoneEntity.currentSubZone != GTSubZone.entrance_tunnel;
			bool flag2 = GameMode.ParticipatingPlayers.Contains(vrrig.OwningNetPlayer);
			if (flag && !flag2)
			{
				GameMode.OptIn(vrrig.OwningNetPlayer.ActorNumber);
			}
			else if (!flag && flag2)
			{
				GameMode.OptOut(vrrig.OwningNetPlayer.ActorNumber);
				this._SetPlayerBlindfoldVisibility(vrrig, vrrig.OwningNetPlayer, false);
			}
		}
		this._ph_isLocalPlayerParticipating = GameMode.ParticipatingPlayers.Contains(VRRig.LocalRig.OwningNetPlayer);
		this.m_ph_soundNearBorder_audioSource.gameObject.SetActive(this._ph_isLocalPlayerParticipating);
	}

	// Token: 0x06000E15 RID: 3605 RVA: 0x0004A608 File Offset: 0x00048808
	private void _UpdateGameState()
	{
		this._ph_gameState_lastUpdate = this._ph_gameState;
		long num = GTTime.TimeAsMilliseconds();
		if (GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
		{
			this._ph_gameState = GorillaPropHuntGameManager.EPropHuntGameState.WaitingForMorePlayers;
			this._ph_roundTime = 0f;
		}
		else if (this._ph_timeRoundStartedMillis <= 0L || num < this._ph_timeRoundStartedMillis)
		{
			this._ph_gameState = GorillaPropHuntGameManager.EPropHuntGameState.WaitingForRoundToStart;
			this._ph_roundTime = 0f;
		}
		else
		{
			this._ph_roundTime = (float)(num - this._ph_timeRoundStartedMillis) / 1000f;
			this._ph_gameState = ((this._ph_roundTime < this.m_ph_hideState_duration) ? GorillaPropHuntGameManager.EPropHuntGameState.Hiding : GorillaPropHuntGameManager.EPropHuntGameState.Playing);
		}
		if (this._ph_gameState != this._ph_gameState_lastUpdate)
		{
			foreach (PlayableBoundaryTracker playableBoundaryTracker in GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers.Values)
			{
				playableBoundaryTracker.ResetValues();
			}
		}
		PlayableBoundaryTracker playableBoundaryTracker2;
		if (!this._ph_isLocalPlayerParticipating && GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers.TryGetValue(VRRig.LocalRig.GetInstanceID(), ref playableBoundaryTracker2))
		{
			playableBoundaryTracker2.ResetValues();
		}
		switch (this._ph_gameState)
		{
		case GorillaPropHuntGameManager.EPropHuntGameState.Invalid:
			Debug.LogError("ERROR!!!  GorillaPropHuntGameManager: " + string.Format("Game state was `{0}` but should only be that when the app ", GorillaPropHuntGameManager.EPropHuntGameState.Invalid) + "starts and then assigned during `StartPlaying` call.");
			return;
		case GorillaPropHuntGameManager.EPropHuntGameState.StoppedGameMode:
		case GorillaPropHuntGameManager.EPropHuntGameState.StartingGameMode:
		case GorillaPropHuntGameManager.EPropHuntGameState.WaitingForMorePlayers:
			if (this._ph_gameState != this._ph_gameState_lastUpdate)
			{
				this._ph_hideState_warnSounds_timesPlayed = 0;
				VRRig rig = VRRigCache.Instance.localRig.Rig;
				this._ph_timeRoundStartedMillis = -1000L;
				this._ResetRigAppearance(rig);
				return;
			}
			break;
		case GorillaPropHuntGameManager.EPropHuntGameState.WaitingForRoundToStart:
			this._ph_hideState_warnSounds_timesPlayed = 0;
			if (PhotonNetwork.IsMasterClient && !this.waitingToStartNextInfectionGame)
			{
				base.ClearInfectionState();
				this.InfectionRoundEnd();
				return;
			}
			break;
		case GorillaPropHuntGameManager.EPropHuntGameState.Hiding:
		{
			if (this._ph_gameState != this._ph_gameState_lastUpdate && this.m_ph_hideState_startSoundBank != null && ZoneManagement.IsInZone(GTZone.bayou))
			{
				this.m_ph_hideState_startSoundBank.Play();
				if (!this._ph_isLocalPlayerSkeleton)
				{
					this.m_ph_soundNearBorder_audioSource.volume = 0f;
				}
			}
			for (int i = 0; i < GameMode.ParticipatingPlayers.Count; i++)
			{
				NetPlayer netPlayer = GameMode.ParticipatingPlayers[i];
				if (this.currentInfected.Contains(netPlayer))
				{
					this._SetPlayerBlindfoldVisibility(netPlayer, true);
				}
			}
			int num2 = this.m_ph_hideState_warnSoundBank_playCount - this._ph_hideState_warnSounds_timesPlayed;
			if (num2 > 0)
			{
				float num3 = this.m_ph_hideState_duration - (float)num2;
				if (this._ph_roundTime > num3 && ZoneManagement.IsInZone(GTZone.bayou))
				{
					if (this.m_ph_hideState_warnSoundBank != null)
					{
						this.m_ph_hideState_warnSoundBank.Play();
					}
					this._ph_hideState_warnSounds_timesPlayed++;
					return;
				}
			}
			break;
		}
		case GorillaPropHuntGameManager.EPropHuntGameState.Playing:
		{
			if (this._ph_gameState_lastUpdate != GorillaPropHuntGameManager.EPropHuntGameState.Playing)
			{
				this._ph_hideState_warnSounds_timesPlayed = 0;
				this._ph_playState_startLightning_strikeTimes_index = 0;
				if (this.m_ph_playState_startSoundBank != null && ZoneManagement.IsInZone(GTZone.bayou))
				{
					this.m_ph_playState_startSoundBank.Play();
				}
				for (int j = 0; j < GorillaPropHuntGameManager._g_ph_activePlayerRigs.Count; j++)
				{
					VRRig vrrig = GorillaPropHuntGameManager._g_ph_activePlayerRigs[j];
					this._SetPlayerBlindfoldVisibility(vrrig, vrrig.OwningNetPlayer, false);
				}
			}
			int num4 = this.m_ph_playState_startLightning_strikeTimes.Length;
			int num5 = math.min(this._ph_playState_startLightning_strikeTimes_index, num4 - 1);
			if (num5 < num4 && this._ph_playState_startLightning_manager_isResolved)
			{
				float num6 = this._ph_roundTime - this.m_ph_hideState_duration;
				if (this.m_ph_playState_startLightning_strikeTimes[num5] <= num6)
				{
					this._ph_playState_startLightning_strikeTimes_index++;
					this._ph_playState_startLightning_manager.DoLightningStrike();
				}
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x06000E16 RID: 3606 RVA: 0x0004A98C File Offset: 0x00048B8C
	public override void UpdatePlayerAppearance(VRRig rig)
	{
		if (rig.zoneEntity.currentZone != GTZone.bayou || (rig.zoneEntity.currentZone == GTZone.bayou && rig.zoneEntity.currentSubZone == GTSubZone.entrance_tunnel))
		{
			return;
		}
		List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
		bool flag = this._GetRigShouldBeSkeleton(rig, participatingPlayers);
		this._ph_isLocalPlayerSkeleton = (this._ph_isLocalPlayerParticipating && !base.IsInfected(NetworkSystem.Instance.LocalPlayer));
		GorillaBodyType gorillaBodyType = flag ? GorillaBodyType.Skeleton : GorillaBodyType.Default;
		int num = flag ? this._ph_gorillaGhostBodyMaterialIndex : 0;
		if (gorillaBodyType != rig.bodyRenderer.gameModeBodyType)
		{
			rig.bodyRenderer.SetGameModeBodyType(gorillaBodyType);
			if (rig.setMatIndex != num)
			{
				rig.ChangeMaterialLocal(num);
			}
		}
		if (PropHuntPools.IsReady)
		{
			bool flag2 = flag;
			if (rig.propHuntHandFollower.hasProp != flag2)
			{
				if (flag2)
				{
					rig.propHuntHandFollower.CreateProp();
				}
				else
				{
					rig.propHuntHandFollower.DestroyProp();
				}
			}
		}
		float signedDistToBoundary = this._UpdateBoundaryProximityState(rig, flag);
		bool flag3 = this._ShouldRigBeVisible(rig, flag, signedDistToBoundary);
		if (!rig.isOfflineVRRig)
		{
			rig.SetInvisibleToLocalPlayer(!flag3);
			if (flag || GorillaBodyRenderer.ForceSkeleton)
			{
				rig.bodyRenderer.SetSkeletonBodyActive(flag3);
			}
		}
	}

	// Token: 0x06000E17 RID: 3607 RVA: 0x0004AAB3 File Offset: 0x00048CB3
	private bool _GetRigShouldBeSkeleton(VRRig rig, List<NetPlayer> participatingPlayers)
	{
		return rig.zoneEntity.currentZone == GTZone.bayou && participatingPlayers.Count >= 2 && participatingPlayers.Contains(rig.OwningNetPlayer) && !base.IsInfected(rig.Creator);
	}

	// Token: 0x06000E18 RID: 3608 RVA: 0x0004AAEC File Offset: 0x00048CEC
	private bool _ShouldRigBeVisible(VRRig rig, bool shouldBeSkeleton, float signedDistToBoundary)
	{
		return this._ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Hiding && (rig.isOfflineVRRig || !shouldBeSkeleton || signedDistToBoundary > 0f || this._ph_isLocalPlayerSkeleton);
	}

	// Token: 0x06000E19 RID: 3609 RVA: 0x0004AB14 File Offset: 0x00048D14
	private float _UpdateBoundaryProximityState(VRRig rig, bool isSkeleton)
	{
		float num = float.MinValue;
		float num2 = float.MinValue;
		if (isSkeleton)
		{
			PlayableBoundaryTracker playableBoundaryTracker;
			if (!GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers.TryGetValue(rig.GetInstanceID(), ref playableBoundaryTracker))
			{
				rig.bodyTransform.GetOrAddComponent(out playableBoundaryTracker);
				GorillaPropHuntGameManager._g_ph_rig_to_propHuntZoneTrackers[rig.GetInstanceID()] = playableBoundaryTracker;
				if (this._ph_playBoundary_isResolved)
				{
					this._ph_playBoundary.tracked.AddIfNew(playableBoundaryTracker);
				}
			}
			num = playableBoundaryTracker.signedDistanceToBoundary;
			num2 = playableBoundaryTracker.prevSignedDistanceToBoundary;
			if (PhotonNetwork.IsMasterClient && !playableBoundaryTracker.IsInsideZone() && playableBoundaryTracker.timeSinceCrossingBorder > this.m_ph_playBoundary_timeLimit)
			{
				this.AddInfectedPlayer(rig.OwningNetPlayer, true);
			}
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(isSkeleton);
			if (isSkeleton)
			{
				float num3 = 1f - math.saturate(-num / this.m_ph_soundNearBorder_maxDistance);
				AudioSource ph_soundNearBorder_audioSource = this.m_ph_soundNearBorder_audioSource;
				GorillaPropHuntGameManager.EPropHuntGameState ph_gameState = this._ph_gameState;
				ph_soundNearBorder_audioSource.volume = ((ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Hiding || ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Playing) ? (this.m_ph_soundNearBorder_baseVolume * this.m_ph_soundNearBorder_volumeCurve.Evaluate(num3)) : 0f);
				if (num >= 0f && num2 < 0f && !this.m_ph_planeCrossingSoundBank.isPlaying)
				{
					this.m_ph_planeCrossingSoundBank.Play();
				}
				this._UpdateControllerHaptics(num);
			}
			else
			{
				this.m_ph_soundNearBorder_audioSource.volume = 0f;
			}
		}
		return num;
	}

	// Token: 0x06000E1A RID: 3610 RVA: 0x0004AC60 File Offset: 0x00048E60
	private void _UpdateControllerHaptics(float signedDistToBoundary)
	{
		if (Time.unscaledTime < GorillaPropHuntGameManager._g_ph_hapticsLastImpulseEndTime || math.abs(signedDistToBoundary) > this.m_ph_hapticsNearBorder_borderProximity)
		{
			return;
		}
		float num = 1f - math.saturate(-signedDistToBoundary / this.m_ph_hapticsNearBorder_borderProximity);
		float num2 = this.m_ph_hapticsNearBorder_ampCurve.Evaluate(num);
		float num3 = math.saturate(this.m_ph_hapticsNearBorder_baseAmp * num2 * (GorillaTagger.Instance.tapHapticStrength * 2f));
		GorillaPropHuntGameManager._g_ph_hapticsLastImpulseEndTime = Time.unscaledTime + 0.1f;
		InputDevices.GetDeviceAtXRNode(4).SendHapticImpulse(0U, num3, 0.1f);
		InputDevices.GetDeviceAtXRNode(5).SendHapticImpulse(0U, num3, 0.1f);
	}

	// Token: 0x06000E1B RID: 3611 RVA: 0x0004AD08 File Offset: 0x00048F08
	private void _Initialize_defaultStencilRefOfSkeletonMat()
	{
		if (GorillaPropHuntGameManager._g_ph_defaultStencilRefOfSkeletonMat == -1 && this._ph_gorillaGhostBodyMaterialIndex != -1)
		{
			Material[] materialsToChangeTo = VRRig.LocalRig.materialsToChangeTo;
			if (materialsToChangeTo != null && materialsToChangeTo.Length >= 1 && VRRig.LocalRig.materialsToChangeTo[0] != null)
			{
				GorillaPropHuntGameManager._g_ph_defaultStencilRefOfSkeletonMat = (int)VRRig.LocalRig.materialsToChangeTo[this._ph_gorillaGhostBodyMaterialIndex].GetFloat(ShaderProps._StencilReference);
				return;
			}
		}
		else
		{
			GorillaPropHuntGameManager._g_ph_defaultStencilRefOfSkeletonMat = 7;
		}
	}

	// Token: 0x06000E1C RID: 3612 RVA: 0x0004AD78 File Offset: 0x00048F78
	private void _Initialize_gorillaGhostBodyMaterialIndex()
	{
		this._ph_gorillaGhostBodyMaterialIndex = -1;
		Material[] materialsToChangeTo = VRRig.LocalRig.materialsToChangeTo;
		for (int i = 0; i < materialsToChangeTo.Length; i++)
		{
			if (materialsToChangeTo[i].name.StartsWith(this.m_ph_gorillaGhostBodyMaterial.name))
			{
				this._ph_gorillaGhostBodyMaterialIndex = i;
				break;
			}
		}
		if (this._ph_gorillaGhostBodyMaterialIndex == -1)
		{
			this._ph_gorillaGhostBodyMaterialIndex = 15;
		}
	}

	// Token: 0x06000E1D RID: 3613 RVA: 0x0004ADDC File Offset: 0x00048FDC
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		GorillaPropHuntGameManager.EPropHuntGameState ph_gameState = this._ph_gameState;
		if ((ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Playing && ph_gameState != GorillaPropHuntGameManager.EPropHuntGameState.Hiding) || !GameMode.ParticipatingPlayers.Contains(forPlayer) || base.IsInfected(forPlayer))
		{
			return 0;
		}
		return this._ph_gorillaGhostBodyMaterialIndex;
	}

	// Token: 0x06000E1E RID: 3614 RVA: 0x0004AE1C File Offset: 0x0004901C
	protected override void InfectionRoundEnd()
	{
		base.InfectionRoundEnd();
		this.InfectionRoundEndCheck();
	}

	// Token: 0x06000E1F RID: 3615 RVA: 0x0004AE2A File Offset: 0x0004902A
	private void InfectionRoundEndCheck()
	{
		this._roundIsPlaying = false;
		if (PhotonNetwork.IsMasterClient)
		{
			this.PH_OnRoundEnd();
		}
	}

	// Token: 0x06000E20 RID: 3616 RVA: 0x0004AE40 File Offset: 0x00049040
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return this._ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Playing && base.LocalCanTag(myPlayer, otherPlayer);
	}

	// Token: 0x06000E21 RID: 3617 RVA: 0x0004AE55 File Offset: 0x00049055
	public override bool LocalIsTagged(NetPlayer player)
	{
		return this._ph_gameState == GorillaPropHuntGameManager.EPropHuntGameState.Playing && base.LocalIsTagged(player);
	}

	// Token: 0x06000E22 RID: 3618 RVA: 0x0004AE6C File Offset: 0x0004906C
	private void _ResetRigAppearance(VRRig rig)
	{
		rig.bodyRenderer.SetSkeletonBodyActive(true);
		rig.bodyRenderer.SetGameModeBodyType(GorillaBodyType.Default);
		this._SetPlayerBlindfoldVisibility(rig, rig.OwningNetPlayer, false);
		rig.ChangeMaterialLocal(0);
		rig.SetInvisibleToLocalPlayer(false);
		if (rig == VRRig.LocalRig)
		{
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
		}
		for (int i = 0; i < GorillaPropHuntGameManager._g_ph_allHandFollowers.Count; i++)
		{
			PropHuntHandFollower propHuntHandFollower = GorillaPropHuntGameManager._g_ph_allHandFollowers[i];
			if (propHuntHandFollower.attachedToRig == rig && propHuntHandFollower.hasProp)
			{
				propHuntHandFollower.DestroyProp();
			}
		}
	}

	// Token: 0x06000E23 RID: 3619 RVA: 0x0004AF04 File Offset: 0x00049104
	protected override void InfectionRoundStart()
	{
		base.InfectionRoundStart();
		this.InfectionRoundStartCheck();
	}

	// Token: 0x06000E24 RID: 3620 RVA: 0x0004AF12 File Offset: 0x00049112
	private void InfectionRoundStartCheck()
	{
		this._roundIsPlaying = true;
		if (PhotonNetwork.IsMasterClient)
		{
			this._ph_randomSeed = Random.Range(1, int.MaxValue);
			this.PH_OnRoundStartRPC(GTTime.TimeAsMilliseconds(), this._ph_randomSeed);
		}
	}

	// Token: 0x06000E25 RID: 3621 RVA: 0x0004AF44 File Offset: 0x00049144
	public override void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
	{
		base.AddInfectedPlayer(infectedPlayer, withTagStop);
		if (infectedPlayer.IsLocal)
		{
			this.m_ph_playState_taggedSoundBank.Play();
		}
	}

	// Token: 0x06000E26 RID: 3622 RVA: 0x0004AF64 File Offset: 0x00049164
	private void _ResolveXSceneRefs()
	{
		if (!this._isListeningForXSceneRefLoadCallbacks)
		{
			this.m_ph_playBoundary_xSceneRef.AddCallbackOnLoad(new Action(this._OnXSceneRefLoaded_PlayBoundary));
			this.m_ph_playBoundary_xSceneRef.AddCallbackOnUnload(new Action(this._OnXSceneRefUnloaded_PlayBoundary));
			this.m_ph_playState_startLightning_manager_ref.AddCallbackOnLoad(new Action(this._OnXSceneRefLoaded_LightningManager));
			this.m_ph_playState_startLightning_manager_ref.AddCallbackOnUnload(new Action(this._OnXSceneRefUnloaded_LightningManager));
		}
		this._OnXSceneRefLoaded_PlayBoundary();
		if (VRRig.LocalRig.zoneEntity.currentZone == GTZone.bayou)
		{
			this._OnXSceneRefLoaded_LightningManager();
		}
	}

	// Token: 0x06000E27 RID: 3623 RVA: 0x0004AFF4 File Offset: 0x000491F4
	private void _OnXSceneRefLoaded_PlayBoundary()
	{
		if (!this._ph_playBoundary_isResolved)
		{
			this._ph_playBoundary_isResolved = (this.m_ph_playBoundary_xSceneRef.TryResolve<PlayableBoundaryManager>(out this._ph_playBoundary) && this._ph_playBoundary != null);
			if (this._ph_playBoundary_isResolved)
			{
				PlayableBoundaryManager ph_playBoundary = this._ph_playBoundary;
				if (ph_playBoundary.tracked == null)
				{
					ph_playBoundary.tracked = new List<PlayableBoundaryTracker>(10);
				}
				this._ph_playBoundary.tracked.Clear();
				if (!this._ph_playBoundary_initialPosition_isInitialized)
				{
					this._ph_playBoundary_initialPosition_isInitialized = true;
					this._ph_playBoundary_initialPosition = this._ph_playBoundary.transform.position;
					this._ph_playBoundary_hasTargetPositionForRound = false;
				}
			}
		}
	}

	// Token: 0x06000E28 RID: 3624 RVA: 0x0004B094 File Offset: 0x00049294
	private void _OnXSceneRefUnloaded_PlayBoundary()
	{
		this._ph_playBoundary_isResolved = false;
		this._ph_playBoundary = null;
		this._ph_playBoundary_hasTargetPositionForRound = false;
	}

	// Token: 0x06000E29 RID: 3625 RVA: 0x0004B0AB File Offset: 0x000492AB
	private void _OnXSceneRefLoaded_LightningManager()
	{
		this._ph_playState_startLightning_manager_isResolved = (this.m_ph_playState_startLightning_manager_ref.TryResolve<LightningManager>(out this._ph_playState_startLightning_manager) && this._ph_playState_startLightning_manager != null);
	}

	// Token: 0x06000E2A RID: 3626 RVA: 0x0004B0D5 File Offset: 0x000492D5
	private void _OnXSceneRefUnloaded_LightningManager()
	{
		this._ph_playState_startLightning_manager_isResolved = false;
		this._ph_playState_startLightning_manager = null;
	}

	// Token: 0x06000E2B RID: 3627 RVA: 0x0004B0E8 File Offset: 0x000492E8
	public void PH_OnRoundEnd()
	{
		VRRigCache.Instance.GetActiveRigs(GorillaPropHuntGameManager._g_ph_activePlayerRigs);
		for (int i = 0; i < GorillaPropHuntGameManager._g_ph_activePlayerRigs.Count; i++)
		{
			this._ResetRigAppearance(GorillaPropHuntGameManager._g_ph_activePlayerRigs[i]);
		}
		CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
		EquipmentInteractor.instance.ForceDropAnyEquipment();
		if (LckSocialCameraManager.Instance != null)
		{
			LckSocialCameraManager.Instance.SetForceHidden(false);
		}
		this._ph_timeRoundStartedMillis = -1000L;
		if (this.m_ph_soundNearBorder_audioSource != null)
		{
			this.m_ph_soundNearBorder_audioSource.volume = 0f;
		}
		if (this._ph_playBoundary_isResolved && this._ph_playBoundary_initialPosition_isInitialized)
		{
			this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
		}
		this._ph_playBoundary_hasTargetPositionForRound = false;
	}

	// Token: 0x06000E2C RID: 3628 RVA: 0x0004B1B2 File Offset: 0x000493B2
	public void PH_OnRoundStartRPC(long timeRoundStartedMillis, int seed)
	{
		this._ph_isLocalPlayerParticipating = GameMode.ParticipatingPlayers.Contains(VRRig.LocalRig.OwningNetPlayer);
		this._ph_timeRoundStartedMillis = timeRoundStartedMillis;
		this._ph_randomSeed = seed;
		this._PH_OnRoundStart();
	}

	// Token: 0x06000E2D RID: 3629 RVA: 0x0004B1E4 File Offset: 0x000493E4
	private void _PH_OnRoundStart()
	{
		if (this._ph_playBoundary_isResolved)
		{
			SRand srand = new SRand(this._ph_randomSeed);
			int num = srand.NextInt(this.m_ph_playBoundary_endPointTransforms.Count);
			Transform transform = this.m_ph_playBoundary_endPointTransforms[num];
			if (transform != null)
			{
				this._ph_playBoundary_currentTargetPosition = transform.position;
				this._ph_playBoundary_hasTargetPositionForRound = true;
				this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
			}
		}
		else if (this._ph_playBoundary_isResolved && this._ph_playBoundary_initialPosition_isInitialized)
		{
			this._ph_playBoundary.transform.position = this._ph_playBoundary_initialPosition;
		}
		if (PropHuntPools.IsReady)
		{
			this.SpawnProps();
		}
		else if (!this._isListeningTo_Pools_OnReady)
		{
			PropHuntPools.OnReady = (Action)Delegate.Combine(PropHuntPools.OnReady, new Action(this._Pools_OnReady));
		}
		if (this._ph_isLocalPlayerParticipating)
		{
			CosmeticsController.instance.SetHideCosmeticsFromRemotePlayers(false);
			if (LckSocialCameraManager.Instance != null)
			{
				LckSocialCameraManager.Instance.SetForceHidden(true);
			}
		}
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0004B2E4 File Offset: 0x000494E4
	private void _Pools_OnReady()
	{
		if (PhotonNetwork.IsMasterClient || this._ph_isLocalPlayerParticipating)
		{
			this.SpawnProps();
		}
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x0004B2FB File Offset: 0x000494FB
	public static void RegisterPropZone(PropHuntPropZone propZone)
	{
		GorillaPropHuntGameManager._g_ph_allPropZones.Add(propZone);
		if (GorillaPropHuntGameManager.instance != null && PropHuntPools.IsReady)
		{
			propZone.OnRoundStart();
		}
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x0004B31C File Offset: 0x0004951C
	public static void UnregisterPropZone(PropHuntPropZone propZone)
	{
		GorillaPropHuntGameManager._g_ph_allPropZones.Remove(propZone);
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x0004B32A File Offset: 0x0004952A
	public static void RegisterPropHandFollower(PropHuntHandFollower hand)
	{
		GorillaPropHuntGameManager._g_ph_allHandFollowers.Add(hand);
		if (GorillaPropHuntGameManager.instance != null)
		{
			hand.OnRoundStart();
		}
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x0004B344 File Offset: 0x00049544
	public static void UnregisterPropHandFollower(PropHuntHandFollower hand)
	{
		GorillaPropHuntGameManager._g_ph_allHandFollowers.Remove(hand);
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x0004B354 File Offset: 0x00049554
	public void SpawnProps()
	{
		if (!PropHuntPools.IsReady)
		{
			if (!this._isListeningTo_Pools_OnReady)
			{
				PropHuntPools.OnReady = (Action)Delegate.Combine(PropHuntPools.OnReady, new Action(this._Pools_OnReady));
			}
			return;
		}
		foreach (PropHuntPropZone propHuntPropZone in GorillaPropHuntGameManager._g_ph_allPropZones)
		{
			propHuntPropZone.OnRoundStart();
		}
		foreach (PropHuntHandFollower propHuntHandFollower in GorillaPropHuntGameManager._g_ph_allHandFollowers)
		{
			if (GameMode.ParticipatingPlayers.Contains(propHuntHandFollower.attachedToRig.OwningNetPlayer))
			{
				propHuntHandFollower.OnRoundStart();
			}
		}
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x0004B42C File Offset: 0x0004962C
	public string GetCosmeticId(uint randomUInt)
	{
		if (PropHuntPools.AllPropCosmeticIds == null)
		{
			return this.m_ph_fallbackPropCosmeticSO.info.playFabID;
		}
		return PropHuntPools.AllPropCosmeticIds[(int)(checked((IntPtr)(unchecked((ulong)randomUInt % (ulong)((long)PropHuntPools.AllPropCosmeticIds.Length)))))];
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x0004B458 File Offset: 0x00049658
	public GTAssetRef<GameObject> GetPropRef_NoPool(uint randomUInt, out CosmeticSO out_debugCosmeticSO)
	{
		if (this.AllPropIDs_NoPool == null)
		{
			out_debugCosmeticSO = this.m_ph_fallbackPropCosmeticSO;
			return this.m_ph_fallbackPropCosmeticSO.info.wardrobeParts[0].prefabAssetRef;
		}
		string cosmeticID = this.AllPropIDs_NoPool[(int)(checked((IntPtr)(unchecked((ulong)randomUInt % (ulong)((long)this.AllPropIDs_NoPool.Length)))))];
		return this.GetPropRefByCosmeticID_NoPool(cosmeticID, out out_debugCosmeticSO);
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x0004B4B0 File Offset: 0x000496B0
	public GTAssetRef<GameObject> GetPropRefByCosmeticID_NoPool(string cosmeticID, out CosmeticSO out_debugCosmeticSO)
	{
		CosmeticSO cosmeticSO = this.m_ph_allCosmetics.SearchForCosmeticSO(cosmeticID);
		if (cosmeticSO == null)
		{
			GTDev.LogError<string>("ERROR!!!  GorillaPropHuntGameManager.GetPropRefByCosmeticID_NoPool: Got cosmetic id from title data, but could not find \"" + cosmeticID + "\".", null);
			out_debugCosmeticSO = this.m_ph_fallbackPropCosmeticSO;
			return this.m_ph_fallbackPropCosmeticSO.info.wardrobeParts[0].prefabAssetRef;
		}
		if (cosmeticSO.info.wardrobeParts.Length == 0)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Invalid prop ",
				cosmeticID,
				" ",
				cosmeticSO.info.displayName,
				" has no wardrobeParts"
			}));
			out_debugCosmeticSO = this.m_ph_fallbackPropCosmeticSO;
			return this.m_ph_fallbackPropCosmeticSO.info.wardrobeParts[0].prefabAssetRef;
		}
		out_debugCosmeticSO = cosmeticSO;
		return cosmeticSO.info.wardrobeParts[0].prefabAssetRef;
	}

	// Token: 0x06000E37 RID: 3639 RVA: 0x0004B594 File Offset: 0x00049794
	private void _SetPlayerBlindfoldVisibility(NetPlayer netPlayer, bool shouldEnable)
	{
		VRRig vrrig = this.FindPlayerVRRig(netPlayer);
		if (vrrig == null && netPlayer.InRoom)
		{
			return;
		}
		this._SetPlayerBlindfoldVisibility(vrrig, netPlayer, shouldEnable);
	}

	// Token: 0x06000E38 RID: 3640 RVA: 0x0004B5C4 File Offset: 0x000497C4
	private void _SetPlayerBlindfoldVisibility(VRRig vrRig, NetPlayer netPlayer, bool shouldEnable)
	{
		if (netPlayer == VRRig.LocalRig.OwningNetPlayer)
		{
			if (!this._ph_blindfold_forCamera_isInitialized)
			{
				this._InitializeBlindfoldForCamera();
			}
			if (this._ph_blindfold_forCamera_isInitialized)
			{
				this._ph_blindfold_forCamera_1p.SetActive(shouldEnable);
				this._ph_blindfold_forCamera_3p.SetActive(shouldEnable);
				return;
			}
		}
		else
		{
			GameObject gameObject;
			if (!this._ph_vrRig_to_blindfolds.TryGetValue(vrRig.GetInstanceID(), ref gameObject))
			{
				Transform[] boneXforms;
				string text;
				if (!GTHardCodedBones.TryGetBoneXforms(vrRig, out boneXforms, out text))
				{
					return;
				}
				Transform transform;
				if (!GTHardCodedBones.TryGetBoneXform(boneXforms, GTHardCodedBones.EBone.head, out transform))
				{
					return;
				}
				if (this.m_ph_blindfold_forAvatarPrefab == null)
				{
					return;
				}
				gameObject = Object.Instantiate<GameObject>(this.m_ph_blindfold_forAvatarPrefab, transform);
				this._ph_vrRig_to_blindfolds[vrRig.GetInstanceID()] = gameObject;
			}
			gameObject.SetActive(shouldEnable);
		}
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x0004B670 File Offset: 0x00049870
	private void _InitializeBlindfoldForCamera()
	{
		if (GorillaTagger.Instance == null)
		{
			return;
		}
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		if (mainCamera == null)
		{
			return;
		}
		if (this.m_ph_blindfold_forCameraPrefab == null)
		{
			return;
		}
		this._ph_blindfold_forCamera_1p = Object.Instantiate<GameObject>(this.m_ph_blindfold_forCameraPrefab, mainCamera.transform);
		Camera camera = null;
		if (GorillaTagger.Instance.thirdPersonCamera != null)
		{
			camera = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>(true);
		}
		if (camera == null)
		{
			return;
		}
		this._ph_blindfold_forCamera_3p = Object.Instantiate<GameObject>(this.m_ph_blindfold_forCameraPrefab, camera.transform);
		this._ph_blindfold_forCamera_isInitialized = (this._ph_blindfold_forCamera_1p != null);
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x0004B720 File Offset: 0x00049920
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeRead(stream, info);
		this._ph_randomSeed = (int)stream.ReceiveNext();
		long ph_timeRoundStartedMillis = this._ph_timeRoundStartedMillis;
		this._ph_timeRoundStartedMillis = (long)stream.ReceiveNext();
		if (ph_timeRoundStartedMillis != this._ph_timeRoundStartedMillis)
		{
			if (this._ph_timeRoundStartedMillis > 0L)
			{
				this._PH_OnRoundStart();
				return;
			}
			this.PH_OnRoundEnd();
		}
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x0004B77C File Offset: 0x0004997C
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this._ph_randomSeed);
		stream.SendNext(this._ph_timeRoundStartedMillis);
	}

	// Token: 0x040010FA RID: 4346
	private const string preLog = "GorillaPropHuntGameManager: ";

	// Token: 0x040010FB RID: 4347
	private const string preLogEd = "(editor only log) GorillaPropHuntGameManager: ";

	// Token: 0x040010FC RID: 4348
	private const string preLogBeta = "(beta only log) GorillaPropHuntGameManager: ";

	// Token: 0x040010FD RID: 4349
	private const string preErr = "ERROR!!!  GorillaPropHuntGameManager: ";

	// Token: 0x040010FE RID: 4350
	private const string preErrEd = "ERROR!!!  (editor only log) GorillaPropHuntGameManager: ";

	// Token: 0x040010FF RID: 4351
	private const string preErrBeta = "ERROR!!!  (beta only log) GorillaPropHuntGameManager: ";

	// Token: 0x04001100 RID: 4352
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x04001102 RID: 4354
	[FormerlySerializedAs("allCosmetics")]
	[SerializeField]
	private AllCosmeticsArraySO m_ph_allCosmetics;

	// Token: 0x04001103 RID: 4355
	[FormerlySerializedAs("backupCosmetic")]
	[FormerlySerializedAs("m_ph_backupCosmetic")]
	[SerializeField]
	private CosmeticSO m_ph_fallbackPropCosmeticSO;

	// Token: 0x04001104 RID: 4356
	[Tooltip("This us used by PropHuntPools as the parent gameobject that the cosmetic prefab instance will be parented to.")]
	[FormerlySerializedAs("m_ph_propPlacementPrefab")]
	[SerializeField]
	private PropPlacementRB m_ph_propDecoyPrefab;

	// Token: 0x04001105 RID: 4357
	[Tooltip("The time that players have to hide before their props can be seen by the tagger monke.")]
	[FormerlySerializedAs("m_propHunt_hideState_duration")]
	[SerializeField]
	private float m_ph_hideState_duration = 10f;

	// Token: 0x04001106 RID: 4358
	[Tooltip("Prefab that will be parented to the camera if the current player is not a ghost during hiding state.")]
	[FormerlySerializedAs("m_propHunt_blindfold_1stPersonPrefab")]
	[SerializeField]
	private GameObject m_ph_blindfold_forCameraPrefab;

	// Token: 0x04001107 RID: 4359
	private GameObject _ph_blindfold_forCamera_1p;

	// Token: 0x04001108 RID: 4360
	private GameObject _ph_blindfold_forCamera_3p;

	// Token: 0x04001109 RID: 4361
	private bool _ph_blindfold_forCamera_isInitialized;

	// Token: 0x0400110A RID: 4362
	[Tooltip("Prefab to cover the eyes of the non-ghost gorilla's avatar during the hiding state.")]
	[FormerlySerializedAs("m_propHunt_blindfold_3rdPersonPrefab")]
	[SerializeField]
	private GameObject m_ph_blindfold_forAvatarPrefab;

	// Token: 0x0400110B RID: 4363
	private readonly Dictionary<int, GameObject> _ph_vrRig_to_blindfolds = new Dictionary<int, GameObject>(10);

	// Token: 0x0400110C RID: 4364
	[Tooltip("A randomly picked sound in this soundbank will be played when the hide state starts.")]
	[FormerlySerializedAs("m_propHunt_hideState_startSoundBank")]
	[SerializeField]
	private SoundBankPlayer m_ph_hideState_startSoundBank;

	// Token: 0x0400110D RID: 4365
	[FormerlySerializedAs("m_propHunt_hideState_warnSoundBank")]
	[Tooltip("A randomly picked Sound in this Sound Bank will be played to warn players that the hiding period is ending.")]
	[FormerlySerializedAs("m_propHunt_hideState_startSoundBank")]
	[SerializeField]
	private SoundBankPlayer m_ph_hideState_warnSoundBank;

	// Token: 0x0400110E RID: 4366
	[FormerlySerializedAs("m_propHunt_hideState_warnSoundBank_playCount")]
	[Tooltip("How many times should the warning sound play before the hiding period ends? Will play every 1 second.")]
	[SerializeField]
	private int m_ph_hideState_warnSoundBank_playCount = 3;

	// Token: 0x0400110F RID: 4367
	private int _ph_hideState_warnSounds_timesPlayed;

	// Token: 0x04001110 RID: 4368
	[FormerlySerializedAs("m_propHunt_playState_startSoundBank")]
	[Tooltip("A randomly picked sound in this Sound Bank will be played when the hiding state ends and the playing state has started.")]
	[SerializeField]
	private SoundBankPlayer m_ph_playState_startSoundBank;

	// Token: 0x04001111 RID: 4369
	[FormerlySerializedAs("m_propHunt_playState_startLightning_manager_ref")]
	[Tooltip("Lightning manager for doing lightning strike strikes when playing starts.")]
	[SerializeField]
	private XSceneRef m_ph_playState_startLightning_manager_ref;

	// Token: 0x04001112 RID: 4370
	private LightningManager _ph_playState_startLightning_manager;

	// Token: 0x04001113 RID: 4371
	private bool _ph_playState_startLightning_manager_isResolved;

	// Token: 0x04001114 RID: 4372
	[Tooltip("How long after the playing starts should the lightning strikes happen?")]
	private float[] m_ph_playState_startLightning_strikeTimes = new float[]
	{
		1f,
		1.5f,
		1.8f
	};

	// Token: 0x04001115 RID: 4373
	private int _ph_playState_startLightning_strikeTimes_index;

	// Token: 0x04001116 RID: 4374
	[Tooltip("A randomly picked sound in this Sound Bank will be played when the ghost is tagged by the hunter.")]
	[SerializeField]
	private SoundBankPlayer m_ph_playState_taggedSoundBank;

	// Token: 0x04001117 RID: 4375
	[Tooltip("Maximum distance prop can be from the center of the player's hand")]
	[SerializeField]
	private float m_ph_hand_follow_distance = 0.35f;

	// Token: 0x04001118 RID: 4376
	[FormerlySerializedAs("_playBoundary_xSceneRef")]
	[FormerlySerializedAs("_playZone_xSceneRef")]
	[SerializeField]
	private XSceneRef m_ph_playBoundary_xSceneRef;

	// Token: 0x04001119 RID: 4377
	[Tooltip("A list of Transforms representing potential end positions for the playable boundary each round.")]
	[SerializeField]
	private List<Transform> m_ph_playBoundary_endPointTransforms = new List<Transform>();

	// Token: 0x0400111A RID: 4378
	private PlayableBoundaryManager _ph_playBoundary;

	// Token: 0x0400111B RID: 4379
	private bool _ph_playBoundary_isResolved;

	// Token: 0x0400111C RID: 4380
	private Vector3 _ph_playBoundary_initialPosition;

	// Token: 0x0400111D RID: 4381
	private bool _ph_playBoundary_initialPosition_isInitialized;

	// Token: 0x0400111E RID: 4382
	private Vector3 _ph_playBoundary_currentTargetPosition;

	// Token: 0x0400111F RID: 4383
	private bool _ph_playBoundary_hasTargetPositionForRound;

	// Token: 0x04001120 RID: 4384
	[Tooltip("The maximum time a player can be outside of the boundary before being tagged.")]
	[SerializeField]
	private float m_ph_playBoundary_timeLimit = 15f;

	// Token: 0x04001121 RID: 4385
	[Tooltip("On the What does 1.0 on the X axis")]
	[FormerlySerializedAs("_playBoundary_radiusScaleOverRoundTime_maxTime")]
	[SerializeField]
	private float m_ph_playBoundary_radiusScaleOverRoundTime_maxTime = 180f;

	// Token: 0x04001122 RID: 4386
	[FormerlySerializedAs("_playBoundary_radiusScaleOverRoundTime_curve")]
	[FormerlySerializedAs("_playZoneRadiusOverRoundTime")]
	[SerializeField]
	private AnimationCurve m_ph_playBoundary_radiusScaleOverRoundTime_curve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f, 1f, 1f, 0f, 0f),
		new Keyframe(0.9f, 0.01f, 1f, 0f, 0f, 0f),
		new Keyframe(1f, 0.01f, 1f, 0f, 0f, 0f)
	});

	// Token: 0x04001123 RID: 4387
	[FormerlySerializedAs("_ph_gorillaGhostBodyMaterial")]
	[FormerlySerializedAs("gorillaGhostBodyMaterial")]
	[SerializeField]
	private Material m_ph_gorillaGhostBodyMaterial;

	// Token: 0x04001124 RID: 4388
	private int _ph_gorillaGhostBodyMaterialIndex = -1;

	// Token: 0x04001125 RID: 4389
	[Tooltip("A randomly picked sound in this Sound Bank will be played when the spectral plane border is crossed.")]
	[SerializeField]
	private SoundBankPlayer m_ph_planeCrossingSoundBank;

	// Token: 0x04001126 RID: 4390
	[Tooltip("This AudioSource will only be heard by the local player and is non directional.")]
	[FormerlySerializedAs("m_soundNearBorder_audioSource")]
	[FormerlySerializedAs("soundNearBorderAudioSource")]
	[FormerlySerializedAs("soundNearBoundaryAudioSource")]
	[SerializeField]
	private AudioSource m_ph_soundNearBorder_audioSource;

	// Token: 0x04001127 RID: 4391
	[FormerlySerializedAs("m_soundNearBorder_maxDistance")]
	[FormerlySerializedAs("soundNearBorderMaxDistance")]
	[FormerlySerializedAs("soundNearBoundaryMaxDistance")]
	[SerializeField]
	private float m_ph_soundNearBorder_maxDistance = 2f;

	// Token: 0x04001128 RID: 4392
	[FormerlySerializedAs("m_soundNearBorder_volumeCurve")]
	[FormerlySerializedAs("soundNearBorderVolumeCurve")]
	[FormerlySerializedAs("soundNearBoundaryVolumeCurve")]
	[SerializeField]
	private AnimationCurve m_ph_soundNearBorder_volumeCurve = AnimationCurves.Linear;

	// Token: 0x04001129 RID: 4393
	[Tooltip("The resulting volume curve value is multiplied by this.")]
	[FormerlySerializedAs("m_soundNearBorder_baseVolume")]
	[SerializeField]
	private float m_ph_soundNearBorder_baseVolume = 0.5f;

	// Token: 0x0400112A RID: 4394
	[FormerlySerializedAs("m_hapticsNearBorder_borderProximity")]
	[SerializeField]
	private float m_ph_hapticsNearBorder_borderProximity = 2f;

	// Token: 0x0400112B RID: 4395
	[FormerlySerializedAs("m_hapticsNearBorder_ampCurve")]
	[SerializeField]
	private AnimationCurve m_ph_hapticsNearBorder_ampCurve = AnimationCurves.Linear;

	// Token: 0x0400112C RID: 4396
	[FormerlySerializedAs("m_hapticsNearBorder_baseAmp")]
	[SerializeField]
	private float m_ph_hapticsNearBorder_baseAmp = 1f;

	// Token: 0x0400112D RID: 4397
	private bool _ph_isLocalPlayerSkeleton;

	// Token: 0x0400112E RID: 4398
	[OnEnterPlay_Clear]
	private static readonly Dictionary<int, PlayableBoundaryTracker> _g_ph_rig_to_propHuntZoneTrackers = new Dictionary<int, PlayableBoundaryTracker>(10);

	// Token: 0x0400112F RID: 4399
	[OnEnterPlay_Set(0f)]
	private static float _g_ph_hapticsLastImpulseEndTime;

	// Token: 0x04001130 RID: 4400
	[OnEnterPlay_Clear]
	private static readonly List<VRRig> _g_ph_activePlayerRigs = new List<VRRig>(10);

	// Token: 0x04001131 RID: 4401
	[OnEnterPlay_Clear]
	private static readonly List<PropHuntPropZone> _g_ph_allPropZones = new List<PropHuntPropZone>();

	// Token: 0x04001132 RID: 4402
	[OnEnterPlay_Clear]
	private static readonly List<PropHuntHandFollower> _g_ph_allHandFollowers = new List<PropHuntHandFollower>();

	// Token: 0x04001133 RID: 4403
	private static readonly string[] _g_ph_titleDataSeparators = new string[]
	{
		"\"",
		" ",
		"\\n"
	};

	// Token: 0x04001134 RID: 4404
	[OnEnterPlay_Set(-1)]
	private static int _g_ph_defaultStencilRefOfSkeletonMat = -1;

	// Token: 0x04001135 RID: 4405
	[DebugReadout]
	private GorillaPropHuntGameManager.EPropHuntGameState _ph_gameState;

	// Token: 0x04001136 RID: 4406
	private GorillaPropHuntGameManager.EPropHuntGameState _ph_gameState_lastUpdate;

	// Token: 0x04001137 RID: 4407
	private bool _roundIsPlaying;

	// Token: 0x04001138 RID: 4408
	private string[] _ph_allPropIDs_noPool;

	// Token: 0x04001139 RID: 4409
	[DebugReadout]
	private float _ph_roundTime;

	// Token: 0x0400113A RID: 4410
	private long __ph_timeRoundStartedMillis__;

	// Token: 0x0400113B RID: 4411
	private int _ph_randomSeed;

	// Token: 0x0400113C RID: 4412
	private bool _ph_isLocalPlayerParticipating;

	// Token: 0x0400113D RID: 4413
	private bool _isListeningTo_Pools_OnReady;

	// Token: 0x0400113E RID: 4414
	private bool _isListeningForXSceneRefLoadCallbacks;

	// Token: 0x02000204 RID: 516
	private enum EPropHuntGameState
	{
		// Token: 0x04001140 RID: 4416
		Invalid,
		// Token: 0x04001141 RID: 4417
		StoppedGameMode,
		// Token: 0x04001142 RID: 4418
		StartingGameMode,
		// Token: 0x04001143 RID: 4419
		WaitingForMorePlayers,
		// Token: 0x04001144 RID: 4420
		WaitingForRoundToStart,
		// Token: 0x04001145 RID: 4421
		Hiding,
		// Token: 0x04001146 RID: 4422
		Playing
	}
}
