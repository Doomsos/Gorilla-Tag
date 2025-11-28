using System;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag;
using GorillaTag.Audio;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200076C RID: 1900
[NetworkBehaviourWeaved(35)]
internal class VRRigSerializer : GorillaWrappedSerializer, IFXContextParems<HandTapArgs>, IFXContextParems<GeoSoundArg>
{
	// Token: 0x17000462 RID: 1122
	// (get) Token: 0x06003156 RID: 12630 RVA: 0x0010C2D5 File Offset: 0x0010A4D5
	// (set) Token: 0x06003157 RID: 12631 RVA: 0x0010C2FF File Offset: 0x0010A4FF
	[Networked]
	[NetworkedWeaved(0, 17)]
	public unsafe NetworkString<_16> nickName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x17000463 RID: 1123
	// (get) Token: 0x06003158 RID: 12632 RVA: 0x0010C32A File Offset: 0x0010A52A
	// (set) Token: 0x06003159 RID: 12633 RVA: 0x0010C358 File Offset: 0x0010A558
	[Networked]
	[NetworkedWeaved(17, 17)]
	public unsafe NetworkString<_16> defaultName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 17);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 17) = value;
		}
	}

	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x0600315A RID: 12634 RVA: 0x0010C387 File Offset: 0x0010A587
	// (set) Token: 0x0600315B RID: 12635 RVA: 0x0010C3B5 File Offset: 0x0010A5B5
	[Networked]
	[NetworkedWeaved(34, 1)]
	public bool tutorialComplete
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(this.Ptr + 34);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(this.Ptr + 34, value);
		}
	}

	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x0600315C RID: 12636 RVA: 0x0010C3E4 File Offset: 0x0010A5E4
	private PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
	}

	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x0600315D RID: 12637 RVA: 0x0010C3EC File Offset: 0x0010A5EC
	public VRRig VRRig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x0600315E RID: 12638 RVA: 0x0010C3F4 File Offset: 0x0010A5F4
	public FXSystemSettings settings
	{
		get
		{
			return this.vrrig.fxSettings;
		}
	}

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x0600315F RID: 12639 RVA: 0x0010C401 File Offset: 0x0010A601
	// (set) Token: 0x06003160 RID: 12640 RVA: 0x0010C409 File Offset: 0x0010A609
	public InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped> SuccesfullSpawnEvent { get; private set; } = new InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped>(2);

	// Token: 0x06003161 RID: 12641 RVA: 0x0010C414 File Offset: 0x0010A614
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (this.netView.IsRoomView)
		{
			if (player != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", player.UserId, player.NickName);
			}
			return false;
		}
		if (NetworkSystem.Instance.IsObjectRoomObject(base.gameObject))
		{
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
			if (player2 != null)
			{
				GorillaNot.instance.SendReport("creating rigs as room objects", player2.UserId, player2.NickName);
			}
			return false;
		}
		if (player != this.netView.Owner)
		{
			GorillaNot.instance.SendReport("creating rigs for someone else", player.UserId, player.NickName);
			return false;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out this.rigContainer))
		{
			outTargetObject = this.rigContainer.gameObject;
			outTargetType = typeof(VRRig);
			this.vrrig = this.rigContainer.Rig;
			return true;
		}
		return false;
	}

	// Token: 0x06003162 RID: 12642 RVA: 0x0010C51C File Offset: 0x0010A71C
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		bool initialized = this.rigContainer.Initialized;
		this.rigContainer.InitializeNetwork(this.netView, this.Voice, this);
		this.networkSpeaker.SetParent(this.rigContainer.SpeakerHead, false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, true);
		this.SetupLoudSpeakerNetwork(this.rigContainer);
		this.netView.GetView.AddCallbackTarget(this);
		if (!initialized)
		{
			object[] instantiationData = info.punInfo.photonView.InstantiationData;
			float red = 0f;
			float green = 0f;
			float blue = 0f;
			if (instantiationData != null && instantiationData.Length == 3)
			{
				object obj = instantiationData[0];
				if (obj is float)
				{
					float value = (float)obj;
					obj = instantiationData[1];
					if (obj is float)
					{
						float value2 = (float)obj;
						obj = instantiationData[2];
						if (obj is float)
						{
							float value3 = (float)obj;
							red = value.ClampSafe(0f, 1f);
							green = value2.ClampSafe(0f, 1f);
							blue = value3.ClampSafe(0f, 1f);
						}
					}
				}
			}
			this.vrrig.InitializeNoobMaterialLocal(red, green, blue);
		}
		this.SuccesfullSpawnEvent.InvokeSafe(this.rigContainer, info);
		NetworkSystem.Instance.IsObjectLocallyOwned(base.gameObject);
		if (VRRigCache.isInitialized)
		{
			VRRigCache.Instance.OnVrrigSerializerSuccesfullySpawned();
		}
	}

	// Token: 0x06003163 RID: 12643 RVA: 0x00002789 File Offset: 0x00000989
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06003164 RID: 12644 RVA: 0x0010C688 File Offset: 0x0010A888
	protected override void OnBeforeDespawn()
	{
		this.CleanUp(true);
	}

	// Token: 0x06003165 RID: 12645 RVA: 0x0010C694 File Offset: 0x0010A894
	private void CleanUp(bool netDestroy)
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		this.successfullInstantiate = false;
		if (this.vrrig != null)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.vrrig.isOfflineVRRig)
				{
					this.vrrig.ChangeMaterialLocal(0);
				}
			}
			else if (this.vrrig.isOfflineVRRig)
			{
				NetworkSystem.Instance.NetDestroy(base.gameObject);
			}
			if (this.vrrig.netView == this.netView)
			{
				this.vrrig.netView = null;
			}
			if (this.vrrig.rigSerializer == this)
			{
				this.vrrig.rigSerializer = null;
			}
		}
		if (this.networkSpeaker != null)
		{
			this.CleanupLoudSpeakerNetwork();
			if (netDestroy)
			{
				this.networkSpeaker.SetParent(base.transform, false);
			}
			else
			{
				this.networkSpeaker.SetParent(null);
			}
			this.networkSpeaker.gameObject.SetActive(false);
		}
		this.vrrig = null;
	}

	// Token: 0x06003166 RID: 12646 RVA: 0x0010C78B File Offset: 0x0010A98B
	private void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.CleanUp(false);
	}

	// Token: 0x06003167 RID: 12647 RVA: 0x0010C79A File Offset: 0x0010A99A
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.networkSpeaker != null && this.networkSpeaker.parent != base.transform)
		{
			Object.Destroy(this.networkSpeaker.gameObject);
		}
	}

	// Token: 0x06003168 RID: 12648 RVA: 0x0010C7D8 File Offset: 0x0010A9D8
	[PunRPC]
	public void RPC_InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfo info)
	{
		this.InitializeNoobMaterialShared(red, green, blue, info);
	}

	// Token: 0x06003169 RID: 12649 RVA: 0x0010C7EA File Offset: 0x0010A9EA
	[PunRPC]
	public void RPC_RequestCosmetics(PhotonMessageInfo info)
	{
		this.RequestCosmeticsShared(info);
	}

	// Token: 0x0600316A RID: 12650 RVA: 0x0010C7F8 File Offset: 0x0010A9F8
	[PunRPC]
	public void RPC_PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		this.PlayDrumShared(drumIndex, drumVolume, info);
	}

	// Token: 0x0600316B RID: 12651 RVA: 0x0010C808 File Offset: 0x0010AA08
	[PunRPC]
	public void RPC_PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		this.PlaySelfOnlyInstrumentShared(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x0600316C RID: 12652 RVA: 0x0010C81A File Offset: 0x0010AA1A
	[PunRPC]
	public void RPC_PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.PlayHandTapShared(soundIndex, isLeftHand, tapVolume, info);
	}

	// Token: 0x0600316D RID: 12653 RVA: 0x0010C82C File Offset: 0x0010AA2C
	public void RPC_UpdateNativeSize(float value, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.UpdateNativeSizeShared(value, info);
	}

	// Token: 0x0600316E RID: 12654 RVA: 0x00002789 File Offset: 0x00000989
	public void RPC_UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600316F RID: 12655 RVA: 0x00002789 File Offset: 0x00000989
	public void RPC_UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x06003170 RID: 12656 RVA: 0x0010C83B File Offset: 0x0010AA3B
	[PunRPC]
	public void RPC_UpdateCosmeticsWithTryonPacked(int[] currentItemsPacked, int[] tryOnItemsPacked, bool playfx, PhotonMessageInfo info)
	{
		this.UpdateCosmeticsWithTryonShared(currentItemsPacked, tryOnItemsPacked, playfx, info);
	}

	// Token: 0x06003171 RID: 12657 RVA: 0x0010C84D File Offset: 0x0010AA4D
	[PunRPC]
	public void RPC_HideAllCosmetics(PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.HideAllCosmetics(info);
	}

	// Token: 0x06003172 RID: 12658 RVA: 0x0010C860 File Offset: 0x0010AA60
	[PunRPC]
	public void RPC_PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		this.PlaySplashEffectShared(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x06003173 RID: 12659 RVA: 0x0010C878 File Offset: 0x0010AA78
	[PunRPC]
	public void RPC_PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		this.PlayGeodeEffectShared(hitPosition, info);
	}

	// Token: 0x06003174 RID: 12660 RVA: 0x0010C887 File Offset: 0x0010AA87
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfo info)
	{
		this.EnableNonCosmeticHandItemShared(enable, isLeftHand, info);
	}

	// Token: 0x06003175 RID: 12661 RVA: 0x0010C897 File Offset: 0x0010AA97
	[PunRPC]
	public void OnHandTapRPC(int audioClipIndex, bool isDownTap, bool isLeftHand, StiltID stiltID, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfo info)
	{
		this.OnHandTapRPCShared(audioClipIndex, isDownTap, isLeftHand, stiltID, handTapSpeed, packedDirFromHitToHand, info);
	}

	// Token: 0x06003176 RID: 12662 RVA: 0x0010C8AF File Offset: 0x0010AAAF
	[PunRPC]
	public void RPC_UpdateQuestScore(int score, PhotonMessageInfo info)
	{
		this.UpdateQuestScore(score, info);
	}

	// Token: 0x06003177 RID: 12663 RVA: 0x0010C8BE File Offset: 0x0010AABE
	[PunRPC]
	public void RPC_UpdateRankedInfo(float elo, int questRank, int PCRank, PhotonMessageInfo info)
	{
		this.UpdateRankedInfo(elo, questRank, PCRank, info);
	}

	// Token: 0x06003178 RID: 12664 RVA: 0x0010C8D0 File Offset: 0x0010AAD0
	private void SetupLoudSpeakerNetwork(RigContainer rigContainer)
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.AddSpeaker(component);
		}
	}

	// Token: 0x06003179 RID: 12665 RVA: 0x0010C948 File Offset: 0x0010AB48
	private void CleanupLoudSpeakerNetwork()
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in this.rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.RemoveSpeaker(component);
		}
	}

	// Token: 0x0600317A RID: 12666 RVA: 0x0010C9C4 File Offset: 0x0010ABC4
	public void BroadcastLoudSpeakerNetwork(bool toggleBroadcast, int actorNumber)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
		{
			return;
		}
		bool isLocal = actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		this.BroadcastLoudSpeakerNetworkShared(toggleBroadcast, rigContainer, actorNumber, isLocal);
	}

	// Token: 0x0600317B RID: 12667 RVA: 0x0010CA08 File Offset: 0x0010AC08
	private void BroadcastLoudSpeakerNetworkShared(bool toggleBroadcast, RigContainer rigContainer, int actorNumber, bool isLocal)
	{
		this.SetupLoudSpeakerNetwork(rigContainer);
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			if (toggleBroadcast)
			{
				loudSpeakerNetwork.BroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
			else
			{
				loudSpeakerNetwork.StopBroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
		}
	}

	// Token: 0x0600317C RID: 12668 RVA: 0x0010CA74 File Offset: 0x0010AC74
	[PunRPC]
	public void GrabbedByPlayer(bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand, PhotonMessageInfo info)
	{
		GorillaGuardianManager gorillaGuardianManager = GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager == null || !gorillaGuardianManager.IsPlayerGuardian(info.Sender))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.vrrig.GrabbedByPlayer(rigContainer.Rig, grabbedBody, grabbedLeftHand, grabbedWithLeftHand);
	}

	// Token: 0x0600317D RID: 12669 RVA: 0x0010CAD0 File Offset: 0x0010ACD0
	[PunRPC]
	public void DroppedByPlayer(Vector3 throwVelocity, PhotonMessageInfo info)
	{
		GorillaNot.IncrementRPCCall(info, "DroppedByPlayer");
		RigContainer rigContainer;
		if (this.vrrig.isOfflineVRRig && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			float num = 10000f;
			if (throwVelocity.IsValid(num))
			{
				this.vrrig.DroppedByPlayer(rigContainer.Rig, throwVelocity);
				return;
			}
		}
	}

	// Token: 0x0600317E RID: 12670 RVA: 0x0010CB2D File Offset: 0x0010AD2D
	void IFXContextParems<HandTapArgs>.OnPlayFX(HandTapArgs parems)
	{
		this.vrrig.PlayHandTapLocal(parems.soundIndex, parems.isLeftHand, parems.tapVolume);
	}

	// Token: 0x0600317F RID: 12671 RVA: 0x0010CB4C File Offset: 0x0010AD4C
	void IFXContextParems<GeoSoundArg>.OnPlayFX(GeoSoundArg parems)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayGeodeEffect(parems.position);
	}

	// Token: 0x06003180 RID: 12672 RVA: 0x0010CB64 File Offset: 0x0010AD64
	private void OnHandTapRPCShared(int audioClipIndex, bool isDownTap, bool isLeftHand, StiltID stiltID, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "OnHandTapRPCShared");
		if (info.Sender != this.netView.Owner)
		{
			return;
		}
		if (audioClipIndex < 0 || audioClipIndex >= GTPlayer.Instance.materialData.Count)
		{
			return;
		}
		HandLink handLink = isLeftHand ? this.vrrig.rightHandLink : this.vrrig.leftHandLink;
		NetPlayer grabbedPlayer = handLink.grabbedPlayer;
		if (grabbedPlayer != null && grabbedPlayer.IsLocal)
		{
			(handLink.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).PlayVicariousTapHaptic();
		}
		Vector3 tapDir = Utils.UnpackVector3FromLong(packedDirFromHitToHand);
		if (!Mathf.Approximately(tapDir.sqrMagnitude, 1f))
		{
			tapDir.Normalize();
		}
		float max = GorillaTagger.Instance.DefaultHandTapVolume;
		GorillaAmbushManager gorillaAmbushManager = GameMode.ActiveGameMode as GorillaAmbushManager;
		if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(this.rigContainer.Creator))
		{
			max = gorillaAmbushManager.crawlingSpeedForMaxVolume;
		}
		OnHandTapFX onHandTapFX = new OnHandTapFX
		{
			rig = this.vrrig,
			surfaceIndex = audioClipIndex,
			isDownTap = isDownTap,
			isLeftHand = isLeftHand,
			stiltID = stiltID,
			volume = handTapSpeed.ClampSafe(0f, max),
			speed = handTapSpeed,
			tapDir = tapDir
		};
		if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority() && CrittersManager.instance.rigSetupByRig[this.vrrig].IsNotNull())
		{
			CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.vrrig].rigActors[isLeftHand ? 0 : 2].actorSet;
			if (crittersLoudNoise.IsNotNull())
			{
				crittersLoudNoise.PlayHandTapRemote(info.SentServerTime, isLeftHand);
			}
		}
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(GTZone.ghostReactor);
		if (managerForZone != null && managerForZone.ghostReactorManager != null)
		{
			Vector3 tapPos = isLeftHand ? this.vrrig.leftHand.rigTarget.position : this.vrrig.rightHand.rigTarget.position;
			managerForZone.ghostReactorManager.OnSharedTap(this.vrrig, tapPos, handTapSpeed);
		}
		FXSystem.PlayFXForRig<HandEffectContext>(FXType.OnHandTap, onHandTapFX, info);
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x0010CDB0 File Offset: 0x0010AFB0
	private void PlayHandTapShared(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		GorillaNot.IncrementRPCCall(info, "PlayHandTapShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && float.IsFinite(tapVolume))
		{
			this.handTapArgs.soundIndex = soundIndex;
			this.handTapArgs.isLeftHand = isLeftHand;
			this.handTapArgs.tapVolume = Mathf.Clamp(tapVolume, 0f, 0.1f);
			FXSystem.PlayFX<HandTapArgs>(FXType.PlayHandTap, this, this.handTapArgs, info);
			return;
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent hand tap", sender.UserId, sender.NickName);
	}

	// Token: 0x06003182 RID: 12674 RVA: 0x0010CE50 File Offset: 0x0010B050
	private void UpdateNativeSizeShared(float value, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		GorillaNot.IncrementRPCCall(info, "UpdateNativeSizeShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && RPCUtil.SafeValue(value, 0.1f, 10f) && RPCUtil.NotSpam("UpdateNativeSizeShared", info, 1f))
		{
			if (this.vrrig != null)
			{
				this.vrrig.NativeScale = value;
				return;
			}
		}
		else
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent native size", sender.UserId, sender.NickName);
		}
	}

	// Token: 0x06003183 RID: 12675 RVA: 0x0010CEE0 File Offset: 0x0010B0E0
	private void PlayGeodeEffectShared(Vector3 hitPosition, PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "PlayGeodeEffectShared");
		if (info.Sender == this.netView.Owner)
		{
			float num = 10000f;
			if (hitPosition.IsValid(num))
			{
				this.geoSoundArg.position = hitPosition;
				FXSystem.PlayFX<GeoSoundArg>(FXType.PlayHandTap, this, this.geoSoundArg, info);
				return;
			}
		}
		GorillaNot.instance.SendReport("inappropriate tag data being sent geode effect", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06003184 RID: 12676 RVA: 0x0010CF5E File Offset: 0x0010B15E
	private void InitializeNoobMaterialShared(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.InitializeNoobMaterial(red, green, blue, info);
	}

	// Token: 0x06003185 RID: 12677 RVA: 0x0010CF75 File Offset: 0x0010B175
	private void RequestMaterialColorShared(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestMaterialColor(askingPlayerID, info);
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x0010CF8C File Offset: 0x0010B18C
	private void RequestCosmeticsShared(PhotonMessageInfoWrapped info)
	{
		GorillaNot.IncrementRPCCall(info, "RequestCosmetics");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[9].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestCosmetics(info);
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x0010CFEE File Offset: 0x0010B1EE
	private void PlayDrumShared(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayDrum(drumIndex, drumVolume, info);
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x0010D003 File Offset: 0x0010B203
	private void PlaySelfOnlyInstrumentShared(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySelfOnlyInstrument(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x06003189 RID: 12681 RVA: 0x0010D01A File Offset: 0x0010B21A
	private void UpdateCosmeticsWithTryonShared(int[] currentItems, int[] tryOnItems, bool playfx, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateCosmeticsWithTryon(currentItems, tryOnItems, playfx, info);
	}

	// Token: 0x0600318A RID: 12682 RVA: 0x0010D031 File Offset: 0x0010B231
	private void PlaySplashEffectShared(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySplashEffect(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x0600318B RID: 12683 RVA: 0x0010D04E File Offset: 0x0010B24E
	private void EnableNonCosmeticHandItemShared(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.EnableNonCosmeticHandItemRPC(enable, isLeftHand, info);
	}

	// Token: 0x0600318C RID: 12684 RVA: 0x0010D063 File Offset: 0x0010B263
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateQuestScore(score, info);
	}

	// Token: 0x0600318D RID: 12685 RVA: 0x0010D077 File Offset: 0x0010B277
	public void UpdateRankedInfo(float elo, int questRank, int PCRank, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateRankedInfo(elo, questRank, PCRank, info);
	}

	// Token: 0x0600318F RID: 12687 RVA: 0x0010D0B8 File Offset: 0x0010B2B8
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.nickName = this._nickName;
		this.defaultName = this._defaultName;
		this.tutorialComplete = this._tutorialComplete;
	}

	// Token: 0x06003190 RID: 12688 RVA: 0x0010D0E8 File Offset: 0x0010B2E8
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._nickName = this.nickName;
		this._defaultName = this.defaultName;
		this._tutorialComplete = this.tutorialComplete;
	}

	// Token: 0x04004003 RID: 16387
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("nickName", 0, 17)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private NetworkString<_16> _nickName;

	// Token: 0x04004004 RID: 16388
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("defaultName", 17, 17)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private NetworkString<_16> _defaultName;

	// Token: 0x04004005 RID: 16389
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("tutorialComplete", 34, 1)]
	[DrawIf("IsEditorWritable", true, 0, 0)]
	private bool _tutorialComplete;

	// Token: 0x04004006 RID: 16390
	[SerializeField]
	private PhotonVoiceView voiceView;

	// Token: 0x04004007 RID: 16391
	public Transform networkSpeaker;

	// Token: 0x04004008 RID: 16392
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x04004009 RID: 16393
	private RigContainer rigContainer;

	// Token: 0x0400400A RID: 16394
	private HandTapArgs handTapArgs = new HandTapArgs();

	// Token: 0x0400400B RID: 16395
	private GeoSoundArg geoSoundArg = new GeoSoundArg();
}
