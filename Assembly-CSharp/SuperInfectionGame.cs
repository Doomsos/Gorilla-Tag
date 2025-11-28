using System;
using GorillaGameModes;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000157 RID: 343
public sealed class SuperInfectionGame : GorillaTagManager
{
	// Token: 0x170000C1 RID: 193
	// (get) Token: 0x06000928 RID: 2344 RVA: 0x000318E4 File Offset: 0x0002FAE4
	// (set) Token: 0x06000929 RID: 2345 RVA: 0x000318EB File Offset: 0x0002FAEB
	public new static SuperInfectionGame instance { get; private set; }

	// Token: 0x0600092A RID: 2346 RVA: 0x000318F3 File Offset: 0x0002FAF3
	public override GameModeType GameType()
	{
		return GameModeType.SuperInfect;
	}

	// Token: 0x170000C2 RID: 194
	// (get) Token: 0x0600092B RID: 2347 RVA: 0x000318F7 File Offset: 0x0002FAF7
	// (set) Token: 0x0600092C RID: 2348 RVA: 0x000318FF File Offset: 0x0002FAFF
	[DebugReadout]
	public ESuperInfectionGameState gameState { get; private set; }

	// Token: 0x0600092D RID: 2349 RVA: 0x00031908 File Offset: 0x0002FB08
	public override void Awake()
	{
		SuperInfectionGame.instance = this;
		this.gameState = ESuperInfectionGameState.Stopped;
		base.Awake();
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x0003191D File Offset: 0x0002FB1D
	public override void OnEnable()
	{
		base.OnEnable();
		SIProgression instance = SIProgression.Instance;
		if (instance == null)
		{
			return;
		}
		instance.ResetTelemetryIntervalData();
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x00031934 File Offset: 0x0002FB34
	public override void OnDisable()
	{
		base.OnDisable();
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0003193C File Offset: 0x0002FB3C
	public override void Tick()
	{
		base.Tick();
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00031944 File Offset: 0x0002FB44
	public override void StartPlaying()
	{
		this.gameState = ESuperInfectionGameState.Starting;
		base.StartPlaying();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			SIProgression.Instance.AddRoundTelemetry();
		}
		VRRig.LocalRig.EnableSuperInfectionHands(true);
		for (int i = 0; i < this.currentNetPlayerArray.Length; i++)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.currentNetPlayerArray[i], out rigContainer))
			{
				rigContainer.Rig.EnableSuperInfectionHands(true);
			}
		}
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x000319B4 File Offset: 0x0002FBB4
	public override void StopPlaying()
	{
		base.StopPlaying();
		this.gameState = ESuperInfectionGameState.Stopped;
		VRRig.LocalRig.EnableSuperInfectionHands(false);
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x000319D0 File Offset: 0x0002FBD0
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableSuperInfectionHands(true);
		}
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x000319FF File Offset: 0x0002FBFF
	public override string GameModeName()
	{
		return "SUPER INFECTION";
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00031A08 File Offset: 0x0002FC08
	public override string GameModeNameRoomLabel()
	{
		string result;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_SUPER_INFECTION_ROOM_LABEL", out result, "(SUPER INFECTION GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_SUPER_INFECTION_ROOM_LABEL]");
		}
		return result;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00031A33 File Offset: 0x0002FC33
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00031A3B File Offset: 0x0002FC3B
	protected override void InfectionRoundStart()
	{
		base.InfectionRoundStart();
		this.gameState = ESuperInfectionGameState.Playing;
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00031A4A File Offset: 0x0002FC4A
	protected override void InfectionRoundEnd()
	{
		base.InfectionRoundEnd();
		this.gameState = ESuperInfectionGameState.RoundRestarting;
		SuperInfectionManager.activeSuperInfectionManager.zoneSuperInfection.ResetPerRoundResources();
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00031A68 File Offset: 0x0002FC68
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return base.LocalCanTag(myPlayer, otherPlayer);
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00031A72 File Offset: 0x0002FC72
	public override void UpdatePlayerAppearance(VRRig rig)
	{
		base.UpdatePlayerAppearance(rig);
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00031A7B File Offset: 0x0002FC7B
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		return base.MyMatIndex(forPlayer);
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x00031A84 File Offset: 0x0002FC84
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeWrite(stream, info);
		stream.SendNext(this.gameState);
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x00031AA0 File Offset: 0x0002FCA0
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		base.OnSerializeRead(stream, info);
		ESuperInfectionGameState gameState = (ESuperInfectionGameState)stream.ReceiveNext();
		if (!Enum.IsDefined(typeof(ESuperInfectionGameState), this.gameState))
		{
			return;
		}
		this.gameState = gameState;
		if (this.gameState != this._gameState_previous)
		{
			this._OnGameStateChanged();
			this._gameState_previous = this.gameState;
		}
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x00031B05 File Offset: 0x0002FD05
	public void _OnGameStateChanged()
	{
		if (this.gameState == ESuperInfectionGameState.Starting)
		{
			SIProgression.Instance.AddRoundTelemetry();
		}
		GTDev.Log<string>(string.Format("Game state changed to {0} ...\n(was {1}).", this.gameState, this._gameState_previous), null);
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00031B40 File Offset: 0x0002FD40
	public override void HandleTagBroadcast(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		try
		{
			SIProgression.Instance.HandleTagTelemetry(taggedPlayer, taggingPlayer);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex, this);
		}
		RigContainer rigContainer;
		RigContainer rigContainer2;
		if (!VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer) || !VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer2))
		{
			return;
		}
		if (taggingPlayer.ActorNumber != SIPlayer.LocalPlayer.ActorNr)
		{
			return;
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Dash] > 0)
		{
			PlayerGameEvents.MiscEvent("SIDashTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Thruster] > 0)
		{
			PlayerGameEvents.MiscEvent("SIThrusterTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Stilt] > 0)
		{
			PlayerGameEvents.MiscEvent("SIStiltTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Platform] > 0)
		{
			PlayerGameEvents.MiscEvent("SIPlatformTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedByGadgetPageType[SITechTreePageId.Blaster] > 0)
		{
			PlayerGameEvents.MiscEvent("SIBlasterTag", 1);
		}
		if (SIProgression.Instance.heldOrSnappedOthersGadgets > 0)
		{
			PlayerGameEvents.MiscEvent("SIBorrowedGadgetTag", 1);
		}
		PlayerGameEvents.MiscEvent("SIGameModeTag", 1);
	}

	// Token: 0x04000B3A RID: 2874
	[SerializeField]
	private int _mySuperExampleSerializedField = 123;

	// Token: 0x04000B3C RID: 2876
	private ESuperInfectionGameState _gameState_previous;
}
