using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CjLib;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaNetworking;
using GorillaTag.Cosmetics;
using GorillaTag.GuidedRefs;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Voice.Unity;
using Steamworks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x020007D7 RID: 2007
public class GorillaTagger : MonoBehaviour, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x0600349C RID: 13468 RVA: 0x0011A584 File Offset: 0x00118784
	public static GorillaTagger Instance
	{
		get
		{
			return GorillaTagger._instance;
		}
	}

	// Token: 0x0600349D RID: 13469 RVA: 0x0011A58C File Offset: 0x0011878C
	public void SetExtraHandPosition(StiltID stiltID, Vector3 position, bool canTag, bool canStun)
	{
		this.stiltTagData[(int)stiltID].currentPositionForTag = position;
		this.stiltTagData[(int)stiltID].hasCurrentPosition = true;
		this.stiltTagData[(int)stiltID].canTag = canTag;
		this.stiltTagData[(int)stiltID].canStun = canStun;
	}

	// Token: 0x170004B2 RID: 1202
	// (get) Token: 0x0600349E RID: 13470 RVA: 0x0011A5E2 File Offset: 0x001187E2
	public NetworkView myVRRig
	{
		get
		{
			return this.offlineVRRig.netView;
		}
	}

	// Token: 0x170004B3 RID: 1203
	// (get) Token: 0x0600349F RID: 13471 RVA: 0x0011A5EF File Offset: 0x001187EF
	internal VRRigSerializer rigSerializer
	{
		get
		{
			return this.offlineVRRig.rigSerializer;
		}
	}

	// Token: 0x170004B4 RID: 1204
	// (get) Token: 0x060034A0 RID: 13472 RVA: 0x0011A5FC File Offset: 0x001187FC
	// (set) Token: 0x060034A1 RID: 13473 RVA: 0x0011A604 File Offset: 0x00118804
	public Rigidbody rigidbody { get; private set; }

	// Token: 0x170004B5 RID: 1205
	// (get) Token: 0x060034A2 RID: 13474 RVA: 0x0011A60D File Offset: 0x0011880D
	public float DefaultHandTapVolume
	{
		get
		{
			return this.cacheHandTapVolume;
		}
	}

	// Token: 0x170004B6 RID: 1206
	// (get) Token: 0x060034A3 RID: 13475 RVA: 0x0011A615 File Offset: 0x00118815
	// (set) Token: 0x060034A4 RID: 13476 RVA: 0x0011A61D File Offset: 0x0011881D
	public Recorder myRecorder { get; private set; }

	// Token: 0x170004B7 RID: 1207
	// (get) Token: 0x060034A5 RID: 13477 RVA: 0x0011A626 File Offset: 0x00118826
	public float sphereCastRadius
	{
		get
		{
			if (this.tagRadiusOverride == null)
			{
				return 0.03f;
			}
			return this.tagRadiusOverride.Value;
		}
	}

	// Token: 0x1400005F RID: 95
	// (add) Token: 0x060034A6 RID: 13478 RVA: 0x0011A648 File Offset: 0x00118848
	// (remove) Token: 0x060034A7 RID: 13479 RVA: 0x0011A680 File Offset: 0x00118880
	public event Action<bool, Vector3, Vector3> OnHandTap;

	// Token: 0x170004B8 RID: 1208
	// (get) Token: 0x060034A8 RID: 13480 RVA: 0x0011A6B5 File Offset: 0x001188B5
	// (set) Token: 0x060034A9 RID: 13481 RVA: 0x0011A6BD File Offset: 0x001188BD
	public bool hasTappedSurface { get; private set; }

	// Token: 0x060034AA RID: 13482 RVA: 0x0011A6C6 File Offset: 0x001188C6
	public void ResetTappedSurfaceCheck()
	{
		this.hasTappedSurface = false;
	}

	// Token: 0x060034AB RID: 13483 RVA: 0x0011A6CF File Offset: 0x001188CF
	public void SetTagRadiusOverrideThisFrame(float radius)
	{
		this.tagRadiusOverride = new float?(radius);
		this.tagRadiusOverrideFrame = Time.frameCount;
	}

	// Token: 0x060034AC RID: 13484 RVA: 0x0011A6E8 File Offset: 0x001188E8
	protected void Awake()
	{
		this.GuidedRefInitialize();
		this.RecoverMissingRefs();
		this.MirrorCameraCullingMask = new Watchable<int>(this.BaseMirrorCameraCullingMask);
		this.stiltTagData[0].isLeftHand = true;
		this.stiltTagData[4].isLeftHand = true;
		this.stiltTagData[5].isLeftHand = true;
		this.stiltTagData[2].isLeftHand = true;
		this.stiltTagData[6].isLeftHand = true;
		this.stiltTagData[7].isLeftHand = true;
		if (GorillaTagger._instance != null && GorillaTagger._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			GorillaTagger._instance = this;
			GorillaTagger.hasInstance = true;
			Action action = GorillaTagger.onPlayerSpawnedRootCallback;
			if (action != null)
			{
				action.Invoke();
			}
		}
		GRFirstTimeUserExperience grfirstTimeUserExperience = Object.FindAnyObjectByType<GRFirstTimeUserExperience>(1);
		GameObject gameObject = (grfirstTimeUserExperience != null) ? grfirstTimeUserExperience.gameObject : null;
		if (!this.disableTutorial && (this.testTutorial || (PlayerPrefs.GetString("tutorial") != "done" && PlayerPrefs.GetString("didTutorial") != "done" && NetworkSystemConfig.AppVersion != "dev")))
		{
			base.transform.parent.position = new Vector3(-140f, 28f, -102f);
			base.transform.parent.eulerAngles = new Vector3(0f, 180f, 0f);
			GTPlayer.Instance.InitializeValues();
			PlayerPrefs.SetFloat("redValue", Random.value);
			PlayerPrefs.SetFloat("greenValue", Random.value);
			PlayerPrefs.SetFloat("blueValue", Random.value);
			PlayerPrefs.Save();
		}
		else
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("didTutorial", true);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
			bool flag = true;
			if (gameObject != null && PlayerPrefs.GetString("spawnInWrongStump") == "flagged" && flag)
			{
				gameObject.SetActive(true);
				GRFirstTimeUserExperience grfirstTimeUserExperience2;
				if (gameObject.TryGetComponent<GRFirstTimeUserExperience>(ref grfirstTimeUserExperience2) && grfirstTimeUserExperience2.spawnPoint != null)
				{
					GTPlayer.Instance.TeleportTo(grfirstTimeUserExperience2.spawnPoint.position, grfirstTimeUserExperience2.spawnPoint.rotation, false, false);
					GTPlayer.Instance.InitializeValues();
					PlayerPrefs.DeleteKey("spawnInWrongStump");
					PlayerPrefs.Save();
				}
			}
		}
		this.thirdPersonCamera.SetActive(Application.platform != 11);
		this.inputDevice = InputDevices.GetDeviceAtXRNode(5);
		this.wasInOverlay = false;
		this.baseSlideControl = GTPlayer.Instance.slideControl;
		this.gorillaTagColliderLayerMask = UnityLayer.GorillaTagCollider.ToLayerMask();
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.cacheHandTapVolume = this.handTapVolume;
		OVRManager.foveatedRenderingLevel = 2;
		this._leftHandDown = new GorillaTagger.DebouncedBool(this._framesForHandTrigger, false);
		this._rightHandDown = new GorillaTagger.DebouncedBool(this._framesForHandTrigger, false);
	}

	// Token: 0x060034AD RID: 13485 RVA: 0x0011AA07 File Offset: 0x00118C07
	protected void OnDestroy()
	{
		if (GorillaTagger._instance == this)
		{
			GorillaTagger._instance = null;
			GorillaTagger.hasInstance = false;
		}
	}

	// Token: 0x060034AE RID: 13486 RVA: 0x0011AA24 File Offset: 0x00118C24
	private void IsXRSubsystemActive()
	{
		this.loadedDeviceName = XRSettings.loadedDeviceName;
		List<XRDisplaySubsystem> list = new List<XRDisplaySubsystem>();
		SubsystemManager.GetSubsystems<XRDisplaySubsystem>(list);
		using (List<XRDisplaySubsystem>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.running)
				{
					this.xrSubsystemIsActive = true;
					return;
				}
			}
		}
		this.xrSubsystemIsActive = false;
	}

	// Token: 0x060034AF RID: 13487 RVA: 0x0011AA98 File Offset: 0x00118C98
	protected void Start()
	{
		this.IsXRSubsystemActive();
		if (this.loadedDeviceName == "OpenVR Display")
		{
			Quaternion quaternion = Quaternion.Euler(new Vector3(-90f, 180f, -20f));
			Quaternion quaternion2 = Quaternion.Euler(new Vector3(-90f, 180f, 20f));
			Quaternion quaternion3 = Quaternion.Euler(new Vector3(-141f, 204f, -27f));
			Quaternion quaternion4 = Quaternion.Euler(new Vector3(-141f, 156f, 27f));
			GTPlayer.Instance.SetHandOffsets(true, new Vector3(-0.02f, 0f, -0.07f), quaternion3 * Quaternion.Inverse(quaternion));
			GTPlayer.Instance.SetHandOffsets(false, new Vector3(0.02f, 0f, -0.07f), quaternion4 * Quaternion.Inverse(quaternion2));
		}
		this.bodyVector = new Vector3(0f, this.bodyCollider.height / 2f - this.bodyCollider.radius, 0f);
		if (SteamManager.Initialized)
		{
			this.gameOverlayActivatedCb = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnGameOverlayActivated));
		}
	}

	// Token: 0x060034B0 RID: 13488 RVA: 0x0011ABCE File Offset: 0x00118DCE
	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		this.isGameOverlayActive = (pCallback.m_bActive > 0);
	}

	// Token: 0x060034B1 RID: 13489 RVA: 0x0011ABE0 File Offset: 0x00118DE0
	protected void LateUpdate()
	{
		GorillaTagger.<>c__DisplayClass133_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this.isGameOverlayActive)
		{
			if (this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(false);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = true;
		}
		else
		{
			if (!this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(true);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = false;
		}
		if (this.xrSubsystemIsActive && Application.platform != 11)
		{
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / XRDevice.refreshRate) > 0.0001f)
			{
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" refresh rate         :\t" + XRDevice.refreshRate.ToString());
				Time.fixedDeltaTime = 1f / XRDevice.refreshRate;
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" history size before  :\t" + GTPlayer.Instance.velocityHistorySize.ToString());
				GTPlayer.Instance.velocityHistorySize = Mathf.Max(Mathf.Min(Mathf.FloorToInt(XRDevice.refreshRate * 0.083333336f), 10), 6);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log("new history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GTPlayer.Instance.InitializeValues();
			}
		}
		else if (Application.platform != 11 && OVRManager.instance != null && OVRManager.OVRManagerinitialized && OVRManager.instance.gameObject != null && OVRManager.instance.gameObject.activeSelf)
		{
			Object.Destroy(OVRManager.instance.gameObject);
		}
		if (!this.frameRateUpdated && Application.platform == 11 && OVRManager.instance.gameObject.activeSelf)
		{
			InputSystem.settings.updateMode = 3;
			int num = OVRManager.display.displayFrequenciesAvailable.Length - 1;
			float num2 = OVRManager.display.displayFrequenciesAvailable[num];
			float systemDisplayFrequency = OVRPlugin.systemDisplayFrequency;
			if (systemDisplayFrequency != 60f)
			{
				if (systemDisplayFrequency == 71f)
				{
					num2 = 72f;
				}
			}
			else
			{
				num2 = 60f;
			}
			while (num2 > 90f)
			{
				num--;
				if (num < 0)
				{
					break;
				}
				num2 = OVRManager.display.displayFrequenciesAvailable[num];
			}
			float num3 = 1f;
			if (Mathf.Abs(Time.fixedDeltaTime - 1f / num2 * num3) > 0.0001f)
			{
				float num4 = Time.fixedDeltaTime - 1f / num2 * num3;
				Debug.Log(" =========== adjusting refresh size =========");
				Debug.Log("!!!!Time.fixedDeltaTime - (1f / newRefreshRate) * " + num3.ToString() + ")" + num4.ToString());
				Debug.Log("Old Refresh rate: " + systemDisplayFrequency.ToString());
				Debug.Log("New Refresh rate: " + num2.ToString());
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" fixedDeltaTime after :\t" + (1f / num2).ToString());
				Time.fixedDeltaTime = 1f / num2 * num3;
				OVRPlugin.systemDisplayFrequency = num2;
				GTPlayer.Instance.velocityHistorySize = Mathf.FloorToInt(num2 * 0.083333336f);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" history size before  :\t" + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log("new history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GTPlayer.Instance.InitializeValues();
				OVRManager.instance.gameObject.SetActive(false);
				this.frameRateUpdated = true;
			}
		}
		if (!this.xrSubsystemIsActive && Application.platform != 11 && Mathf.Abs(Time.fixedDeltaTime - 0.0069444445f) > 0.0001f)
		{
			Debug.Log("updating delta time. was: " + Time.fixedDeltaTime.ToString() + ". now it's " + 0.0069444445f.ToString());
			Application.targetFrameRate = 144;
			Time.fixedDeltaTime = 0.0069444445f;
			GTPlayer.Instance.velocityHistorySize = Mathf.Min(Mathf.FloorToInt(12f), 10);
			if (GTPlayer.Instance.velocityHistorySize > 9)
			{
				GTPlayer.Instance.velocityHistorySize--;
			}
			Debug.Log("new history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
			GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(144f);
			GTPlayer.Instance.InitializeValues();
		}
		this.otherPlayer = null;
		this.touchedPlayer = null;
		CS$<>8__locals1.otherTouchedPlayer = null;
		if (this.tagRadiusOverrideFrame < Time.frameCount)
		{
			this.tagRadiusOverride = default(float?);
		}
		Vector3 position = this.leftHandTransform.position;
		Vector3 position2 = this.rightHandTransform.position;
		Vector3 position3 = this.headCollider.transform.position;
		Vector3 position4 = this.bodyCollider.transform.position;
		float scale = GTPlayer.Instance.scale;
		float num5 = this.sphereCastRadius * scale;
		CS$<>8__locals1.bodyHit = false;
		CS$<>8__locals1.leftHandHit = false;
		CS$<>8__locals1.canTagHit = false;
		CS$<>8__locals1.canStunHit = false;
		if (!(GorillaGameManager.instance is CasualGameMode))
		{
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(this.lastLeftHandPositionForTag, position, num5, this.colliderOverlaps, this.gorillaTagColliderLayerMask, 2);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|133_0(true, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(position3, position, num5, this.colliderOverlaps, this.gorillaTagColliderLayerMask, 2);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|133_0(true, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(this.lastRightHandPositionForTag, position2, num5, this.colliderOverlaps, this.gorillaTagColliderLayerMask, 2);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|133_0(false, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(position3, position2, num5, this.colliderOverlaps, this.gorillaTagColliderLayerMask, 2);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|133_0(false, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			for (int i = 0; i < 12; i++)
			{
				GorillaTagger.StiltTagData stiltTagData = this.stiltTagData[i];
				if (stiltTagData.hasLastPosition && stiltTagData.hasCurrentPosition && (stiltTagData.canTag || stiltTagData.canStun))
				{
					this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(stiltTagData.currentPositionForTag, stiltTagData.lastPositionForTag, num5, this.colliderOverlaps, this.gorillaTagColliderLayerMask, 2);
					this.<LateUpdate>g__TryTaggingAllHitsOverlap|133_0(i == 0 || i == 2, this.maxStiltTagDistance, stiltTagData.canTag, stiltTagData.canStun, ref CS$<>8__locals1);
				}
			}
			this.topVector = this.lastHeadPositionForTag;
			this.bottomVector = this.lastBodyPositionForTag - this.bodyVector;
			this.nonAllocHits = Physics.CapsuleCastNonAlloc(this.topVector, this.bottomVector, this.bodyCollider.radius * 2f * GTPlayer.Instance.scale, this.bodyRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.bodyRaycastSweep.magnitude, num5), this.gorillaTagColliderLayerMask, 2);
			this.<LateUpdate>g__TryTaggingAllHitsCapsulecast|133_1(this.maxTagDistance, true, false, ref CS$<>8__locals1);
		}
		if (this.otherPlayer != null)
		{
			if (CS$<>8__locals1.canTagHit && (!CS$<>8__locals1.canStunHit || GorillaGameManager.instance.LocalCanTag(NetworkSystem.Instance.LocalPlayer, this.otherPlayer)))
			{
				GameMode.ActiveGameMode.LocalTag(this.otherPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.bodyHit, CS$<>8__locals1.leftHandHit);
				GameMode.ReportTag(this.otherPlayer);
			}
			if (CS$<>8__locals1.canStunHit)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, this.otherPlayer);
			}
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null && GorillaGameManager.instance != null)
		{
			CustomGameMode.TouchPlayer(CS$<>8__locals1.otherTouchedPlayer);
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null)
		{
			this.HitWithKnockBack(CS$<>8__locals1.otherTouchedPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.leftHandHit);
		}
		GTPlayer instance = GTPlayer.Instance;
		bool flag = true;
		StiltID stiltID = StiltID.None;
		this.ProcessHandTapping(flag, stiltID, ref this.lastLeftTap, ref this.lastLeftUpTap, ref this.leftHandWasTouching, this.leftHandSlideSource);
		flag = false;
		stiltID = StiltID.None;
		this.ProcessHandTapping(flag, stiltID, ref this.lastRightTap, ref this.lastRightUpTap, ref this.rightHandWasTouching, this.rightHandSlideSource);
		for (int j = 0; j < 12; j++)
		{
			GorillaTagger.StiltTagData stiltTagData2 = this.stiltTagData[j];
			if (stiltTagData2.hasLastPosition && stiltTagData2.hasCurrentPosition)
			{
				stiltID = (StiltID)j;
				this.ProcessHandTapping(stiltTagData2.isLeftHand, stiltID, ref stiltTagData2.lastTap, ref stiltTagData2.lastUpTap, ref stiltTagData2.wasTouching, this.leftHandSlideSource);
				this.stiltTagData[j] = stiltTagData2;
			}
		}
		this.CheckEndStatusEffect();
		this.lastLeftHandPositionForTag = position;
		this.lastRightHandPositionForTag = position2;
		this.lastBodyPositionForTag = position4;
		this.lastHeadPositionForTag = position3;
		for (int k = 0; k < 12; k++)
		{
			GorillaTagger.StiltTagData stiltTagData3 = this.stiltTagData[k];
			if (stiltTagData3.hasLastPosition || stiltTagData3.hasCurrentPosition)
			{
				stiltTagData3.lastPositionForTag = stiltTagData3.currentPositionForTag;
				stiltTagData3.hasLastPosition = stiltTagData3.hasCurrentPosition;
				stiltTagData3.hasCurrentPosition = false;
				this.stiltTagData[k] = stiltTagData3;
			}
		}
		if (GTPlayer.Instance.IsBodySliding && (double)GTPlayer.Instance.RigidbodyVelocity.magnitude >= 0.15)
		{
			if (!this.bodySlideSource.isPlaying)
			{
				this.bodySlideSource.Play();
			}
		}
		else
		{
			this.bodySlideSource.Stop();
		}
		if (GorillaComputer.instance == null || NetworkSystem.Instance.LocalRecorder == null)
		{
			return;
		}
		if (float.IsFinite(GorillaTagger.moderationMutedTime) && GorillaTagger.moderationMutedTime >= 0f)
		{
			GorillaTagger.moderationMutedTime -= Time.deltaTime;
		}
		if (GorillaComputer.instance.voiceChatOn == "TRUE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (GorillaTagger.moderationMutedTime > 0f)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "OPEN MIC")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(5);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(5);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(4);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(4);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						bool transmitEnabled = this.myRecorder.TransmitEnabled;
						this.myRecorder.TransmitEnabled = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
						{
							this.myRecorder.TransmitEnabled = true;
							return;
						}
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
					{
						this.myRecorder.TransmitEnabled = true;
						return;
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO TALK")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = false;
					bool transmitEnabled2 = this.myRecorder.TransmitEnabled;
					this.myRecorder.TransmitEnabled = false;
					return;
				}
			}
			else
			{
				if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
				{
					this.myRecorder.TransmitEnabled = true;
				}
				if (!this.offlineVRRig.shouldSendSpeakingLoudness)
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					return;
				}
			}
		}
		else if (GorillaComputer.instance.voiceChatOn == "FALSE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (!this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = true;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "OPEN MIC")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(5);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(5);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(4);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(4);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
				}
				else
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
				}
			}
			else if (!this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = true;
				return;
			}
		}
		else
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = false;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
		}
	}

	// Token: 0x060034B2 RID: 13490 RVA: 0x0011BAD4 File Offset: 0x00119CD4
	private bool TryToTag(VRRig rig, Vector3 hitObjectPos, bool isBodyTag, bool canStun, float maxTagDistance, out NetPlayer taggedPlayer, out NetPlayer touchedPlayer)
	{
		taggedPlayer = null;
		touchedPlayer = null;
		if (NetworkSystem.Instance.InRoom)
		{
			this.tempCreator = ((rig != null) ? rig.creator : null);
			if (this.tempCreator != null && NetworkSystem.Instance.LocalPlayer != this.tempCreator)
			{
				touchedPlayer = this.tempCreator;
				if (GorillaGameManager.instance != null && Time.time > this.taggedTime + this.tagCooldown && (canStun || GorillaGameManager.instance.LocalCanTag(NetworkSystem.Instance.LocalPlayer, this.tempCreator)) && (this.headCollider.transform.position - hitObjectPos).sqrMagnitude < maxTagDistance * maxTagDistance * GTPlayer.Instance.scale)
				{
					if (!isBodyTag)
					{
						this.StartVibration((this.leftHandTransform.position - hitObjectPos).magnitude < (this.rightHandTransform.position - hitObjectPos).magnitude, this.tagHapticStrength, this.tagHapticDuration);
					}
					else
					{
						this.StartVibration(true, this.tagHapticStrength, this.tagHapticDuration);
						this.StartVibration(false, this.tagHapticStrength, this.tagHapticDuration);
					}
					taggedPlayer = this.tempCreator;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060034B3 RID: 13491 RVA: 0x0011BC34 File Offset: 0x00119E34
	private bool TryToTag(Collider hitCollider, bool isBodyTag, bool canStun, float maxTagDistance, out NetPlayer taggedPlayer, out NetPlayer touchedNetPlayer)
	{
		VRRig vrrig;
		if (!this.tagRigDict.TryGetValue(hitCollider, ref vrrig))
		{
			vrrig = hitCollider.GetComponentInParent<VRRig>();
			this.tagRigDict.Add(hitCollider, vrrig);
		}
		if (vrrig == null)
		{
			PropHuntTaggableProp componentInParent = hitCollider.GetComponentInParent<PropHuntTaggableProp>();
			if (!(componentInParent != null))
			{
				taggedPlayer = null;
				touchedNetPlayer = null;
				return false;
			}
			vrrig = componentInParent.ownerRig;
		}
		else if (GorillaGameManager.instance != null && GorillaGameManager.instance.GameType() == GameModeType.PropHunt)
		{
			taggedPlayer = null;
			touchedNetPlayer = null;
			return false;
		}
		return this.TryToTag(vrrig, hitCollider.transform.position, isBodyTag, canStun, maxTagDistance, out taggedPlayer, out touchedNetPlayer);
	}

	// Token: 0x060034B4 RID: 13492 RVA: 0x0011BCD0 File Offset: 0x00119ED0
	private void HitWithKnockBack(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool leftHand)
	{
		Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(leftHand).GetAverageVelocity(true, 0.15f, false);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand;
		Vector3 vector = leftHand ? (-vrmap.rigTarget.right) : vrmap.rigTarget.right;
		RigContainer rigContainer2;
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer2) && rigContainer2.Rig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback, ref cosmeticEffect))
		{
			RoomSystem.HitPlayer(taggedPlayer, vector.normalized, averageVelocity.magnitude);
		}
	}

	// Token: 0x060034B5 RID: 13493 RVA: 0x0011BD7B File Offset: 0x00119F7B
	public void StartVibration(bool forLeftController, float amplitude, float duration)
	{
		base.StartCoroutine(this.HapticPulses(forLeftController, amplitude, duration));
	}

	// Token: 0x060034B6 RID: 13494 RVA: 0x0011BD8D File Offset: 0x00119F8D
	private IEnumerator HapticPulses(bool forLeftController, float amplitude, float duration)
	{
		float startTime = Time.time;
		uint channel = 0U;
		InputDevice device;
		if (forLeftController)
		{
			device = InputDevices.GetDeviceAtXRNode(4);
		}
		else
		{
			device = InputDevices.GetDeviceAtXRNode(5);
		}
		while (Time.time < startTime + duration)
		{
			device.SendHapticImpulse(channel, amplitude, this.hapticWaitSeconds);
			yield return new WaitForSeconds(this.hapticWaitSeconds * 0.9f);
		}
		yield break;
	}

	// Token: 0x060034B7 RID: 13495 RVA: 0x0011BDB4 File Offset: 0x00119FB4
	public void PlayHapticClip(bool forLeftController, AudioClip clip, float strength)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
			}
			this.leftHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
			return;
		}
		if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
		}
		this.rightHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
	}

	// Token: 0x060034B8 RID: 13496 RVA: 0x0011BE17 File Offset: 0x0011A017
	public void StopHapticClip(bool forLeftController)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
				this.leftHapticsRoutine = null;
				return;
			}
		}
		else if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
			this.rightHapticsRoutine = null;
		}
	}

	// Token: 0x060034B9 RID: 13497 RVA: 0x0011BE53 File Offset: 0x0011A053
	private IEnumerator AudioClipHapticPulses(bool forLeftController, AudioClip clip, float strength)
	{
		uint channel = 0U;
		int bufferSize = 8192;
		int sampleWindowSize = 256;
		float[] audioData;
		InputDevice device;
		if (forLeftController)
		{
			float[] array;
			if ((array = this.leftHapticsBuffer) == null)
			{
				array = (this.leftHapticsBuffer = new float[bufferSize]);
			}
			audioData = array;
			device = InputDevices.GetDeviceAtXRNode(4);
		}
		else
		{
			float[] array2;
			if ((array2 = this.rightHapticsBuffer) == null)
			{
				array2 = (this.rightHapticsBuffer = new float[bufferSize]);
			}
			audioData = array2;
			device = InputDevices.GetDeviceAtXRNode(5);
		}
		int sampleOffset = -bufferSize;
		float startTime = Time.time;
		float length = clip.length;
		float endTime = Time.time + length;
		float sampleRate = (float)clip.samples;
		while (Time.time <= endTime)
		{
			float num = (Time.time - startTime) / length;
			int num2 = (int)(sampleRate * num);
			if (Mathf.Max(num2 + sampleWindowSize - 1, audioData.Length - 1) >= sampleOffset + bufferSize)
			{
				clip.GetData(audioData, num2);
				sampleOffset = num2;
			}
			float num3 = 0f;
			int num4 = Mathf.Min(clip.samples - num2, sampleWindowSize);
			for (int i = 0; i < num4; i++)
			{
				float num5 = audioData[num2 - sampleOffset + i];
				num3 += num5 * num5;
			}
			float num6 = Mathf.Clamp01(((num4 > 0) ? Mathf.Sqrt(num3 / (float)num4) : 0f) * strength);
			device.SendHapticImpulse(channel, num6, Time.fixedDeltaTime);
			yield return null;
		}
		if (forLeftController)
		{
			this.leftHapticsRoutine = null;
		}
		else
		{
			this.rightHapticsRoutine = null;
		}
		yield break;
	}

	// Token: 0x060034BA RID: 13498 RVA: 0x0011BE78 File Offset: 0x0011A078
	public void DoVibration(XRNode node, float amplitude, float duration)
	{
		InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(node);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.SendHapticImpulse(0U, amplitude, duration);
		}
	}

	// Token: 0x060034BB RID: 13499 RVA: 0x0011BEA0 File Offset: 0x0011A0A0
	public void UpdateColor(float red, float green, float blue)
	{
		this.offlineVRRig.InitializeNoobMaterialLocal(red, green, blue);
		if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom)
		{
			this.offlineVRRig.bodyRenderer.ResetBodyMaterial();
		}
	}

	// Token: 0x060034BC RID: 13500 RVA: 0x0011BEDC File Offset: 0x0011A0DC
	protected void OnTriggerEnter(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(ref gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxTriggered();
		}
	}

	// Token: 0x060034BD RID: 13501 RVA: 0x0011BEFC File Offset: 0x0011A0FC
	protected void OnTriggerExit(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(ref gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxExited();
		}
	}

	// Token: 0x060034BE RID: 13502 RVA: 0x0011BF1C File Offset: 0x0011A11C
	public void ShowCosmeticParticles(bool showParticles)
	{
		if (showParticles)
		{
			this.mainCamera.GetComponent<Camera>().cullingMask |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			this.MirrorCameraCullingMask.value |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			return;
		}
		this.mainCamera.GetComponent<Camera>().cullingMask &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
		this.MirrorCameraCullingMask.value &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
	}

	// Token: 0x060034BF RID: 13503 RVA: 0x0011BF9D File Offset: 0x0011A19D
	public void ApplyStatusEffect(GorillaTagger.StatusEffect newStatus, float duration)
	{
		this.EndStatusEffect(this.currentStatus);
		this.currentStatus = newStatus;
		this.statusEndTime = Time.time + duration;
		switch (newStatus)
		{
		case GorillaTagger.StatusEffect.None:
		case GorillaTagger.StatusEffect.Slowed:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x060034C0 RID: 13504 RVA: 0x0011BFDD File Offset: 0x0011A1DD
	private void CheckEndStatusEffect()
	{
		if (Time.time > this.statusEndTime)
		{
			this.EndStatusEffect(this.currentStatus);
		}
	}

	// Token: 0x060034C1 RID: 13505 RVA: 0x0011BFF8 File Offset: 0x0011A1F8
	private void EndStatusEffect(GorillaTagger.StatusEffect effectToEnd)
	{
		switch (effectToEnd)
		{
		case GorillaTagger.StatusEffect.None:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = false;
			this.currentStatus = GorillaTagger.StatusEffect.None;
			return;
		case GorillaTagger.StatusEffect.Slowed:
			this.currentStatus = GorillaTagger.StatusEffect.None;
			break;
		default:
			return;
		}
	}

	// Token: 0x060034C2 RID: 13506 RVA: 0x0011C027 File Offset: 0x0011A227
	private float CalcSlideControl(float fps)
	{
		return Mathf.Pow(Mathf.Pow(1f - this.baseSlideControl, 120f), 1f / fps);
	}

	// Token: 0x060034C3 RID: 13507 RVA: 0x0011C04B File Offset: 0x0011A24B
	public static void OnPlayerSpawned(Action action)
	{
		if (GorillaTagger._instance)
		{
			action.Invoke();
			return;
		}
		GorillaTagger.onPlayerSpawnedRootCallback = (Action)Delegate.Combine(GorillaTagger.onPlayerSpawnedRootCallback, action);
	}

	// Token: 0x060034C4 RID: 13508 RVA: 0x0011C078 File Offset: 0x0011A278
	private void ProcessHandTapping(in bool isLeftHand, in StiltID stiltID, ref float lastTapTime, ref float lastTapUpTime, ref bool wasHandTouching, in AudioSource handSlideSource)
	{
		bool flag;
		bool flag2;
		int num;
		GorillaSurfaceOverride gorillaSurfaceOverride;
		RaycastHit raycastHit;
		Vector3 vector;
		GorillaVelocityTracker gorillaVelocityTracker;
		GTPlayer.Instance.GetHandTapData(isLeftHand, stiltID, out flag, out flag2, out num, out gorillaSurfaceOverride, out raycastHit, out vector, out gorillaVelocityTracker);
		GorillaTagger.DebouncedBool debouncedBool = isLeftHand ? this._leftHandDown : this._rightHandDown;
		if (GTPlayer.Instance.inOverlay)
		{
			handSlideSource.GTStop();
			return;
		}
		if (flag2)
		{
			this.StartVibration(isLeftHand, this.tapHapticStrength / 5f, Time.fixedDeltaTime);
			if (!handSlideSource.isPlaying)
			{
				handSlideSource.GTPlay();
			}
			return;
		}
		handSlideSource.GTStop();
		bool wasStablyEnabled = debouncedBool.WasStablyEnabled;
		debouncedBool.Set(flag);
		bool flag3 = !wasHandTouching && flag && debouncedBool.JustEnabled;
		bool flag4 = wasHandTouching && !flag && wasStablyEnabled;
		wasHandTouching = flag;
		if (!flag4 && !flag3)
		{
			return;
		}
		Tappable tappable = null;
		bool flag5 = gorillaSurfaceOverride != null && gorillaSurfaceOverride.TryGetComponent<Tappable>(ref tappable);
		HandEffectContext handEffect = this.offlineVRRig.GetHandEffect(isLeftHand, stiltID);
		if ((!flag5 || !tappable.overrideTapCooldown) && (!handEffect.SeparateUpTapCooldown || !flag4 || Time.time <= lastTapUpTime + this.tapCoolDown) && Time.time <= lastTapTime + this.tapCoolDown)
		{
			return;
		}
		float sqrMagnitude = (gorillaVelocityTracker.GetAverageVelocity(true, 0.03f, false) / GTPlayer.Instance.scale).sqrMagnitude;
		float sqrMagnitude2 = gorillaVelocityTracker.GetAverageVelocity(false, 0.03f, false).sqrMagnitude;
		this.handTapSpeed = Mathf.Sqrt(Mathf.Max(sqrMagnitude, sqrMagnitude2));
		if (handEffect.SeparateUpTapCooldown && flag4)
		{
			lastTapUpTime = Time.time;
		}
		else
		{
			lastTapTime = Time.time;
		}
		this.dirFromHitToHand = Vector3.Normalize(raycastHit.point - vector);
		GorillaAmbushManager gorillaAmbushManager = GameMode.ActiveGameMode as GorillaAmbushManager;
		if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(NetworkSystem.Instance.LocalPlayer))
		{
			this.handTapVolume = Mathf.Clamp(this.handTapSpeed, 0f, gorillaAmbushManager.crawlingSpeedForMaxVolume);
		}
		else
		{
			this.handTapVolume = this.cacheHandTapVolume;
		}
		GorillaFreezeTagManager gorillaFreezeTagManager = GameMode.ActiveGameMode as GorillaFreezeTagManager;
		if (gorillaFreezeTagManager != null && gorillaFreezeTagManager.IsFrozen(NetworkSystem.Instance.LocalPlayer))
		{
			this.audioClipIndex = gorillaFreezeTagManager.GetFrozenHandTapAudioIndex();
		}
		else if (gorillaSurfaceOverride != null)
		{
			this.audioClipIndex = gorillaSurfaceOverride.overrideIndex;
		}
		else
		{
			this.audioClipIndex = num;
		}
		if (gorillaSurfaceOverride != null)
		{
			if (gorillaSurfaceOverride.sendOnTapEvent)
			{
				IBuilderTappable builderTappable;
				if (flag5)
				{
					tappable.OnTap(this.handTapVolume);
				}
				else if (gorillaSurfaceOverride.TryGetComponent<IBuilderTappable>(ref builderTappable))
				{
					builderTappable.OnTapLocal(this.handTapVolume);
				}
			}
			PlayerGameEvents.TapObject(gorillaSurfaceOverride.name);
		}
		Vector3 averageVelocity = gorillaVelocityTracker.GetAverageVelocity(true, 0.03f, false);
		if (GameMode.ActiveGameMode != null)
		{
			GameMode.ActiveGameMode.HandleHandTap(NetworkSystem.Instance.LocalPlayer, tappable, isLeftHand, averageVelocity, raycastHit.normal);
		}
		this.StartVibration(isLeftHand, this.tapHapticStrength, this.tapHapticDuration);
		this.offlineVRRig.SetHandEffectData(handEffect, this.audioClipIndex, flag3, isLeftHand, stiltID, this.handTapVolume, this.handTapSpeed, this.dirFromHitToHand);
		FXSystem.PlayFX(handEffect);
		Action<bool, Vector3, Vector3> onHandTap = this.OnHandTap;
		if (onHandTap != null)
		{
			onHandTap.Invoke(isLeftHand, raycastHit.point, raycastHit.normal);
		}
		this.hasTappedSurface = true;
		if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority())
		{
			CrittersRigActorSetup crittersRigActorSetup = CrittersManager.instance.rigSetupByRig[this.offlineVRRig];
			if (crittersRigActorSetup.IsNotNull())
			{
				CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)crittersRigActorSetup.rigActors[isLeftHand ? 0 : 2].actorSet;
				if (crittersLoudNoise.IsNotNull())
				{
					crittersLoudNoise.PlayHandTapLocal(isLeftHand);
				}
			}
		}
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.offlineVRRig.zoneEntity.currentZone);
		if (managerForZone.IsNotNull() && managerForZone.ghostReactorManager.IsNotNull() && !UnityVectorExtensions.AlmostZero(averageVelocity))
		{
			Transform handFollower = GTPlayer.Instance.GetHandFollower(isLeftHand);
			RaycastHit raycastHit2;
			if (Physics.Raycast(new Ray(handFollower.position, averageVelocity.normalized), ref raycastHit2, 10f))
			{
				Vector3 vector2 = Vector3.ProjectOnPlane(-handFollower.forward, raycastHit2.normal);
				managerForZone.ghostReactorManager.OnTapLocal(isLeftHand, raycastHit2.point + raycastHit2.normal * 0.005f, Quaternion.LookRotation(vector2.normalized, isLeftHand ? (-raycastHit2.normal) : raycastHit2.normal), gorillaSurfaceOverride, averageVelocity);
			}
		}
		if (NetworkSystem.Instance.InRoom && this.myVRRig.IsNotNull() && this.myVRRig != null)
		{
			this.myVRRig.GetView.RPC("OnHandTapRPC", 1, new object[]
			{
				this.audioClipIndex,
				flag3,
				isLeftHand,
				stiltID,
				this.handTapSpeed,
				Utils.PackVector3ToLong(this.dirFromHitToHand)
			});
		}
	}

	// Token: 0x060034C5 RID: 13509 RVA: 0x0011C5AC File Offset: 0x0011A7AC
	public void DebugDrawTagCasts(Color color)
	{
		float num = this.sphereCastRadius * GTPlayer.Instance.scale;
		this.DrawSphereCast(this.lastLeftHandPositionForTag, this.leftRaycastSweep.normalized, num, Mathf.Max(this.leftRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.leftHeadRaycastSweep.normalized, num, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.lastRightHandPositionForTag, this.rightRaycastSweep.normalized, num, Mathf.Max(this.rightRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.rightHeadRaycastSweep.normalized, num, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num), color);
	}

	// Token: 0x060034C6 RID: 13510 RVA: 0x0011C687 File Offset: 0x0011A887
	private void DrawSphereCast(Vector3 start, Vector3 dir, float radius, float dist, Color color)
	{
		DebugUtil.DrawCapsule(start, start + dir * dist, radius, 16, 16, color, true, DebugUtil.Style.Wireframe);
	}

	// Token: 0x060034C7 RID: 13511 RVA: 0x0011C6A6 File Offset: 0x0011A8A6
	private void RecoverMissingRefs()
	{
		if (!this.offlineVRRig)
		{
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.leftHandSlideSource, "leftHandSlideSource", "./**/Left Arm IK/SlideAudio");
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.rightHandSlideSource, "rightHandSlideSource", "./**/Right Arm IK/SlideAudio");
		}
	}

	// Token: 0x060034C8 RID: 13512 RVA: 0x0011C6E4 File Offset: 0x0011A8E4
	private void RecoverMissingRefs_Asdf<T>(ref T objRef, string objFieldName, string recoveryPath) where T : Object
	{
		if (objRef)
		{
			return;
		}
		Transform transform;
		if (!this.offlineVRRig.transform.TryFindByPath(recoveryPath, out transform, false))
		{
			Debug.LogError(string.Concat(new string[]
			{
				"`",
				objFieldName,
				"` reference missing and could not find by path: \"",
				recoveryPath,
				"\""
			}), this);
		}
		objRef = transform.GetComponentInChildren<T>();
		if (!objRef)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"`",
				objFieldName,
				"` reference is missing. Found transform with recover path, but did not find the component. Recover path: \"",
				recoveryPath,
				"\""
			}), this);
		}
	}

	// Token: 0x060034C9 RID: 13513 RVA: 0x0011C79A File Offset: 0x0011A99A
	public void GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverField<GorillaTagger>(this, "offlineVRRig", ref this.offlineVRRig_gRef);
		GuidedRefHub.ReceiverFullyRegistered<GorillaTagger>(this);
	}

	// Token: 0x170004B9 RID: 1209
	// (get) Token: 0x060034CA RID: 13514 RVA: 0x0011C7B3 File Offset: 0x0011A9B3
	// (set) Token: 0x060034CB RID: 13515 RVA: 0x0011C7BB File Offset: 0x0011A9BB
	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	// Token: 0x060034CC RID: 13516 RVA: 0x0011C7C4 File Offset: 0x0011A9C4
	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		if (this.offlineVRRig_gRef.fieldId == target.fieldId && this.offlineVRRig == null)
		{
			this.offlineVRRig = (target.targetMono.GuidedRefTargetObject as VRRig);
			return this.offlineVRRig != null;
		}
		return false;
	}

	// Token: 0x060034CD RID: 13517 RVA: 0x00002789 File Offset: 0x00000989
	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
	}

	// Token: 0x060034CE RID: 13518 RVA: 0x00002789 File Offset: 0x00000989
	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
	}

	// Token: 0x060034D1 RID: 13521 RVA: 0x000743A9 File Offset: 0x000725A9
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x060034D2 RID: 13522 RVA: 0x000178ED File Offset: 0x00015AED
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x060034D3 RID: 13523 RVA: 0x0011C914 File Offset: 0x0011AB14
	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHitsOverlap|133_0(bool isLeftHand, float maxTagDistance, bool canTag = true, bool canStun = false, ref GorillaTagger.<>c__DisplayClass133_0 A_5)
	{
		for (int i = 0; i < this.nonAllocHits; i++)
		{
			VRRig vrrig;
			if (this.colliderOverlaps[i].gameObject.activeSelf && (!this.tagRigDict.TryGetValue(this.colliderOverlaps[i], ref vrrig) || !(vrrig == VRRig.LocalRig)))
			{
				if (this.TryToTag(this.colliderOverlaps[i], true, canStun, maxTagDistance, out this.tryPlayer, out this.touchedPlayer))
				{
					this.otherPlayer = this.tryPlayer;
					A_5.bodyHit = false;
					A_5.leftHandHit = isLeftHand;
					A_5.canTagHit = canTag;
					A_5.canStunHit = canStun;
					return;
				}
				if (this.touchedPlayer != null)
				{
					A_5.otherTouchedPlayer = this.touchedPlayer;
				}
			}
		}
	}

	// Token: 0x060034D4 RID: 13524 RVA: 0x0011C9D8 File Offset: 0x0011ABD8
	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHitsCapsulecast|133_1(float maxTagDistance, bool canTag = true, bool canStun = false, ref GorillaTagger.<>c__DisplayClass133_0 A_4)
	{
		for (int i = 0; i < this.nonAllocHits; i++)
		{
			VRRig vrrig;
			if (this.nonAllocRaycastHits[i].collider.gameObject.activeSelf && (!this.tagRigDict.TryGetValue(this.nonAllocRaycastHits[i].collider, ref vrrig) || !(vrrig == VRRig.LocalRig)))
			{
				if (this.TryToTag(this.nonAllocRaycastHits[i].collider, false, canStun, maxTagDistance, out this.tryPlayer, out this.touchedPlayer))
				{
					this.otherPlayer = this.tryPlayer;
					A_4.bodyHit = true;
					A_4.canTagHit = canTag;
					A_4.canStunHit = canStun;
					return;
				}
				if (this.touchedPlayer != null)
				{
					A_4.otherTouchedPlayer = this.touchedPlayer;
				}
			}
		}
	}

	// Token: 0x040042FD RID: 17149
	[OnEnterPlay_SetNull]
	private static GorillaTagger _instance;

	// Token: 0x040042FE RID: 17150
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x040042FF RID: 17151
	public static float moderationMutedTime = -1f;

	// Token: 0x04004300 RID: 17152
	public bool inCosmeticsRoom;

	// Token: 0x04004301 RID: 17153
	public SphereCollider headCollider;

	// Token: 0x04004302 RID: 17154
	public CapsuleCollider bodyCollider;

	// Token: 0x04004303 RID: 17155
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x04004304 RID: 17156
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x04004305 RID: 17157
	private Vector3 lastBodyPositionForTag;

	// Token: 0x04004306 RID: 17158
	private Vector3 lastHeadPositionForTag;

	// Token: 0x04004307 RID: 17159
	private GorillaTagger.StiltTagData[] stiltTagData = new GorillaTagger.StiltTagData[12];

	// Token: 0x04004308 RID: 17160
	public Transform rightHandTransform;

	// Token: 0x04004309 RID: 17161
	public Transform leftHandTransform;

	// Token: 0x0400430A RID: 17162
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x0400430B RID: 17163
	public float handTapVolume = 0.1f;

	// Token: 0x0400430C RID: 17164
	public float handTapSpeed;

	// Token: 0x0400430D RID: 17165
	public float tapCoolDown = 0.15f;

	// Token: 0x0400430E RID: 17166
	public float lastLeftTap;

	// Token: 0x0400430F RID: 17167
	public float lastLeftUpTap;

	// Token: 0x04004310 RID: 17168
	public float lastRightTap;

	// Token: 0x04004311 RID: 17169
	public float lastRightUpTap;

	// Token: 0x04004312 RID: 17170
	private bool leftHandWasTouching;

	// Token: 0x04004313 RID: 17171
	private bool rightHandWasTouching;

	// Token: 0x04004314 RID: 17172
	public float tapHapticDuration = 0.05f;

	// Token: 0x04004315 RID: 17173
	public float tapHapticStrength = 0.5f;

	// Token: 0x04004316 RID: 17174
	public float tagHapticDuration = 0.15f;

	// Token: 0x04004317 RID: 17175
	public float tagHapticStrength = 1f;

	// Token: 0x04004318 RID: 17176
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04004319 RID: 17177
	public float taggedHapticStrength = 1f;

	// Token: 0x0400431A RID: 17178
	public float taggedTime;

	// Token: 0x0400431B RID: 17179
	public float tagCooldown;

	// Token: 0x0400431C RID: 17180
	public float slowCooldown = 3f;

	// Token: 0x0400431D RID: 17181
	public float maxTagDistance = 2.2f;

	// Token: 0x0400431E RID: 17182
	public float maxStiltTagDistance = 3.2f;

	// Token: 0x0400431F RID: 17183
	public VRRig offlineVRRig;

	// Token: 0x04004320 RID: 17184
	[FormerlySerializedAs("offlineVRRig_guidedRef")]
	public GuidedRefReceiverFieldInfo offlineVRRig_gRef = new GuidedRefReceiverFieldInfo(false);

	// Token: 0x04004321 RID: 17185
	public GameObject thirdPersonCamera;

	// Token: 0x04004322 RID: 17186
	public GameObject mainCamera;

	// Token: 0x04004323 RID: 17187
	public bool testTutorial;

	// Token: 0x04004324 RID: 17188
	public bool disableTutorial;

	// Token: 0x04004325 RID: 17189
	public bool frameRateUpdated;

	// Token: 0x04004326 RID: 17190
	public GameObject leftHandTriggerCollider;

	// Token: 0x04004327 RID: 17191
	public GameObject rightHandTriggerCollider;

	// Token: 0x04004328 RID: 17192
	public AudioSource leftHandSlideSource;

	// Token: 0x04004329 RID: 17193
	public AudioSource rightHandSlideSource;

	// Token: 0x0400432A RID: 17194
	public AudioSource bodySlideSource;

	// Token: 0x0400432B RID: 17195
	public bool overrideNotInFocus;

	// Token: 0x0400432D RID: 17197
	private Vector3 leftRaycastSweep;

	// Token: 0x0400432E RID: 17198
	private Vector3 leftHeadRaycastSweep;

	// Token: 0x0400432F RID: 17199
	private Vector3 rightRaycastSweep;

	// Token: 0x04004330 RID: 17200
	private Vector3 rightHeadRaycastSweep;

	// Token: 0x04004331 RID: 17201
	private Vector3 headRaycastSweep;

	// Token: 0x04004332 RID: 17202
	private Vector3 bodyRaycastSweep;

	// Token: 0x04004333 RID: 17203
	private InputDevice rightDevice;

	// Token: 0x04004334 RID: 17204
	private InputDevice leftDevice;

	// Token: 0x04004335 RID: 17205
	private bool primaryButtonPressRight;

	// Token: 0x04004336 RID: 17206
	private bool secondaryButtonPressRight;

	// Token: 0x04004337 RID: 17207
	private bool primaryButtonPressLeft;

	// Token: 0x04004338 RID: 17208
	private bool secondaryButtonPressLeft;

	// Token: 0x04004339 RID: 17209
	private RaycastHit hitInfo;

	// Token: 0x0400433A RID: 17210
	public NetPlayer otherPlayer;

	// Token: 0x0400433B RID: 17211
	private NetPlayer tryPlayer;

	// Token: 0x0400433C RID: 17212
	private NetPlayer touchedPlayer;

	// Token: 0x0400433D RID: 17213
	private Vector3 topVector;

	// Token: 0x0400433E RID: 17214
	private Vector3 bottomVector;

	// Token: 0x0400433F RID: 17215
	private Vector3 bodyVector;

	// Token: 0x04004340 RID: 17216
	private Vector3 dirFromHitToHand;

	// Token: 0x04004341 RID: 17217
	private int audioClipIndex;

	// Token: 0x04004342 RID: 17218
	private InputDevice inputDevice;

	// Token: 0x04004343 RID: 17219
	private bool wasInOverlay;

	// Token: 0x04004344 RID: 17220
	private PhotonView tempView;

	// Token: 0x04004345 RID: 17221
	private NetPlayer tempCreator;

	// Token: 0x04004346 RID: 17222
	private float cacheHandTapVolume;

	// Token: 0x04004347 RID: 17223
	public GorillaTagger.StatusEffect currentStatus;

	// Token: 0x04004348 RID: 17224
	public float statusStartTime;

	// Token: 0x04004349 RID: 17225
	public float statusEndTime;

	// Token: 0x0400434A RID: 17226
	private float refreshRate;

	// Token: 0x0400434B RID: 17227
	private float baseSlideControl;

	// Token: 0x0400434C RID: 17228
	private int gorillaTagColliderLayerMask;

	// Token: 0x0400434D RID: 17229
	private RaycastHit[] nonAllocRaycastHits = new RaycastHit[30];

	// Token: 0x0400434E RID: 17230
	private Collider[] colliderOverlaps = new Collider[30];

	// Token: 0x0400434F RID: 17231
	private Dictionary<Collider, VRRig> tagRigDict = new Dictionary<Collider, VRRig>();

	// Token: 0x04004350 RID: 17232
	private int nonAllocHits;

	// Token: 0x04004352 RID: 17234
	private bool xrSubsystemIsActive;

	// Token: 0x04004353 RID: 17235
	public string loadedDeviceName = "";

	// Token: 0x04004354 RID: 17236
	[SerializeField]
	private int _framesForHandTrigger = 5;

	// Token: 0x04004355 RID: 17237
	private GorillaTagger.DebouncedBool _leftHandDown;

	// Token: 0x04004356 RID: 17238
	private GorillaTagger.DebouncedBool _rightHandDown;

	// Token: 0x04004357 RID: 17239
	[SerializeField]
	private LayerMask BaseMirrorCameraCullingMask;

	// Token: 0x04004358 RID: 17240
	public Watchable<int> MirrorCameraCullingMask;

	// Token: 0x04004359 RID: 17241
	private float[] leftHapticsBuffer;

	// Token: 0x0400435A RID: 17242
	private float[] rightHapticsBuffer;

	// Token: 0x0400435B RID: 17243
	private Coroutine leftHapticsRoutine;

	// Token: 0x0400435C RID: 17244
	private Coroutine rightHapticsRoutine;

	// Token: 0x0400435D RID: 17245
	private Callback<GameOverlayActivated_t> gameOverlayActivatedCb;

	// Token: 0x0400435E RID: 17246
	private bool isGameOverlayActive;

	// Token: 0x0400435F RID: 17247
	private float? tagRadiusOverride;

	// Token: 0x04004360 RID: 17248
	private int tagRadiusOverrideFrame = -1;

	// Token: 0x04004363 RID: 17251
	private static Action onPlayerSpawnedRootCallback;

	// Token: 0x020007D8 RID: 2008
	private struct StiltTagData
	{
		// Token: 0x04004365 RID: 17253
		public bool isLeftHand;

		// Token: 0x04004366 RID: 17254
		public bool hasCurrentPosition;

		// Token: 0x04004367 RID: 17255
		public bool hasLastPosition;

		// Token: 0x04004368 RID: 17256
		public Vector3 currentPositionForTag;

		// Token: 0x04004369 RID: 17257
		public Vector3 lastPositionForTag;

		// Token: 0x0400436A RID: 17258
		public bool wasTouching;

		// Token: 0x0400436B RID: 17259
		public float lastTap;

		// Token: 0x0400436C RID: 17260
		public float lastUpTap;

		// Token: 0x0400436D RID: 17261
		public bool canTag;

		// Token: 0x0400436E RID: 17262
		public bool canStun;
	}

	// Token: 0x020007D9 RID: 2009
	public enum StatusEffect
	{
		// Token: 0x04004370 RID: 17264
		None,
		// Token: 0x04004371 RID: 17265
		Frozen,
		// Token: 0x04004372 RID: 17266
		Slowed,
		// Token: 0x04004373 RID: 17267
		Dead,
		// Token: 0x04004374 RID: 17268
		Infected,
		// Token: 0x04004375 RID: 17269
		It
	}

	// Token: 0x020007DA RID: 2010
	private class DebouncedBool
	{
		// Token: 0x170004BA RID: 1210
		// (get) Token: 0x060034D5 RID: 13525 RVA: 0x0011CAAB File Offset: 0x0011ACAB
		// (set) Token: 0x060034D6 RID: 13526 RVA: 0x0011CAB3 File Offset: 0x0011ACB3
		public bool Value { get; private set; }

		// Token: 0x170004BB RID: 1211
		// (get) Token: 0x060034D7 RID: 13527 RVA: 0x0011CABC File Offset: 0x0011ACBC
		// (set) Token: 0x060034D8 RID: 13528 RVA: 0x0011CAC4 File Offset: 0x0011ACC4
		public bool JustEnabled { get; private set; }

		// Token: 0x170004BC RID: 1212
		// (get) Token: 0x060034D9 RID: 13529 RVA: 0x0011CACD File Offset: 0x0011ACCD
		// (set) Token: 0x060034DA RID: 13530 RVA: 0x0011CAD5 File Offset: 0x0011ACD5
		public bool WasStablyEnabled { get; private set; }

		// Token: 0x060034DB RID: 13531 RVA: 0x0011CADE File Offset: 0x0011ACDE
		public DebouncedBool(int callsUntilDisable, bool initialValue = false)
		{
			this._callsUntilStable = callsUntilDisable;
			this.Value = initialValue;
			this._lastValue = initialValue;
		}

		// Token: 0x060034DC RID: 13532 RVA: 0x0011CAFC File Offset: 0x0011ACFC
		public void Set(bool value)
		{
			this._lastValue = this.Value;
			if (!value)
			{
				this.WasStablyEnabled = false;
				this._callsSinceDisable++;
				if (this._callsSinceDisable == this._callsUntilStable)
				{
					this.Value = false;
				}
			}
			else
			{
				this.Value = true;
				this._callsSinceDisable = 0;
				this._callsSinceEnable++;
				if (this._callsSinceEnable >= this._callsUntilStable)
				{
					this.WasStablyEnabled = true;
				}
			}
			this.JustEnabled = (this.Value && !this._lastValue);
		}

		// Token: 0x04004376 RID: 17270
		private readonly int _callsUntilStable;

		// Token: 0x04004377 RID: 17271
		private int _callsSinceDisable;

		// Token: 0x04004378 RID: 17272
		private int _callsSinceEnable;

		// Token: 0x04004379 RID: 17273
		private bool _lastValue;
	}
}
