using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020006C6 RID: 1734
public class GRFirstTimeUserExperience : MonoBehaviour
{
	// Token: 0x06002C92 RID: 11410 RVA: 0x000E763B File Offset: 0x000E583B
	[ContextMenu("Set Player Pref")]
	private void RemovePlayerPref()
	{
		PlayerPrefs.SetString("spawnInWrongStump", "flagged");
		PlayerPrefs.Save();
	}

	// Token: 0x06002C93 RID: 11411 RVA: 0x000F1620 File Offset: 0x000EF820
	private void OnEnable()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.flickerSphere.SetActive(false);
		this.logoQuad.SetActive(false);
		this.flickerSphereOrigParent = this.flickerSphere.transform.parent;
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
		this.playerLight = GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true);
		this.playerLight.gameObject.SetActive(true);
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Waiting);
	}

	// Token: 0x06002C94 RID: 11412 RVA: 0x000F16A4 File Offset: 0x000EF8A4
	public void ChangeState(GRFirstTimeUserExperience.TransitionState state)
	{
		this.transitionState = state;
		switch (state)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
			this.transitionState = GRFirstTimeUserExperience.TransitionState.Flicker;
			this.flickerSphere.transform.SetParent(GTPlayer.Instance.headCollider.transform, false);
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(false);
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Logo:
			this.stateStartTime = Time.time;
			this.flickerSphere.SetActive(true);
			this.logoQuad.SetActive(true);
			return;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.OnSceneLoadsCompleted = (Action)Delegate.Combine(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
			ZoneManagement.SetActiveZone(this.teleportZone);
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this.joinRoomTrigger, JoinType.Solo, null);
			GTPlayer.Instance.TeleportTo(this.teleportLocation.position, this.teleportLocation.rotation, false, false);
			GTPlayer.Instance.InitializeValues();
			this.stateStartTime = Time.time;
			return;
		case GRFirstTimeUserExperience.TransitionState.Exit:
			this.flickerSphere.transform.SetParent(this.flickerSphereOrigParent, false);
			this.flickerSphere.SetActive(false);
			this.logoQuad.SetActive(false);
			this.rootObject.SetActive(false);
			GorillaTagger.Instance.mainCamera.GetComponentInChildren<GameLight>(true).gameObject.SetActive(false);
			return;
		default:
			return;
		}
	}

	// Token: 0x06002C95 RID: 11413 RVA: 0x000F1826 File Offset: 0x000EFA26
	private void OnZoneLoadComplete()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.OnSceneLoadsCompleted = (Action)Delegate.Remove(instance.OnSceneLoadsCompleted, new Action(this.OnZoneLoadComplete));
		this.ChangeState(GRFirstTimeUserExperience.TransitionState.Teleport);
	}

	// Token: 0x06002C96 RID: 11414 RVA: 0x000F1858 File Offset: 0x000EFA58
	public void InterruptWaitingTimer()
	{
		this.stateStartTime = -1f;
		for (int i = 0; i < this.delayObjects.Count; i++)
		{
			this.delayObjects[i].enabledTime = this.stateStartTime;
		}
	}

	// Token: 0x06002C97 RID: 11415 RVA: 0x000F18A0 File Offset: 0x000EFAA0
	private void Update()
	{
		switch (this.transitionState)
		{
		case GRFirstTimeUserExperience.TransitionState.Waiting:
			if (PrivateUIRoom.GetInOverlay())
			{
				if (this.stateStartTime >= 0f)
				{
					this.InterruptWaitingTimer();
				}
			}
			else if (this.stateStartTime < 0f)
			{
				this.stateStartTime = Time.time;
			}
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.transitionDelay)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Flicker);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.Flicker:
		{
			float num = Time.time - this.stateStartTime;
			if (this.stateStartTime >= 0f && num >= this.flickerDuration)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Logo);
				return;
			}
			bool flag = this.flickerTimeline.Evaluate(num / this.flickerDuration) < 0f;
			this.flickerSphere.SetActive(flag);
			if (flag && !this.flickerLightWasOff)
			{
				if (this.audioSource != null && this.flickerAudioCount < this.flickerAudio.Count && this.flickerAudio[this.flickerAudioCount] != null)
				{
					this.audioSource.PlayOneShot(this.flickerAudio[this.flickerAudioCount]);
				}
				this.flickerAudioCount++;
			}
			this.flickerLightWasOff = flag;
			return;
		}
		case GRFirstTimeUserExperience.TransitionState.Logo:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.logoDisplayTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.ZoneLoad);
				return;
			}
			break;
		case GRFirstTimeUserExperience.TransitionState.ZoneLoad:
			break;
		case GRFirstTimeUserExperience.TransitionState.Teleport:
			if (this.stateStartTime >= 0f && Time.time - this.stateStartTime >= this.teleportSettleTime)
			{
				this.ChangeState(GRFirstTimeUserExperience.TransitionState.Exit);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x040039CE RID: 14798
	public Transform spawnPoint;

	// Token: 0x040039CF RID: 14799
	public GameObject rootObject;

	// Token: 0x040039D0 RID: 14800
	public GameObject flickerSphere;

	// Token: 0x040039D1 RID: 14801
	public GameObject logoQuad;

	// Token: 0x040039D2 RID: 14802
	public AnimationCurve flickerTimeline;

	// Token: 0x040039D3 RID: 14803
	public float flickerDuration = 3f;

	// Token: 0x040039D4 RID: 14804
	public GTZone teleportZone = GTZone.none;

	// Token: 0x040039D5 RID: 14805
	public Transform teleportLocation;

	// Token: 0x040039D6 RID: 14806
	public float transitionDelay = 60f;

	// Token: 0x040039D7 RID: 14807
	public float logoDisplayTime = 4f;

	// Token: 0x040039D8 RID: 14808
	public float teleportSettleTime = 1f;

	// Token: 0x040039D9 RID: 14809
	public GorillaNetworkJoinTrigger joinRoomTrigger;

	// Token: 0x040039DA RID: 14810
	public List<AudioClip> flickerAudio = new List<AudioClip>();

	// Token: 0x040039DB RID: 14811
	public List<DisableGameObjectDelayed> delayObjects;

	// Token: 0x040039DC RID: 14812
	private Transform flickerSphereOrigParent;

	// Token: 0x040039DD RID: 14813
	private float stateStartTime = -1f;

	// Token: 0x040039DE RID: 14814
	private bool flickerLightWasOff;

	// Token: 0x040039DF RID: 14815
	private int flickerAudioCount;

	// Token: 0x040039E0 RID: 14816
	private AudioSource audioSource;

	// Token: 0x040039E1 RID: 14817
	private GRFirstTimeUserExperience.TransitionState transitionState;

	// Token: 0x040039E2 RID: 14818
	public GameLight playerLight;

	// Token: 0x020006C7 RID: 1735
	public enum TransitionState
	{
		// Token: 0x040039E4 RID: 14820
		Waiting,
		// Token: 0x040039E5 RID: 14821
		Flicker,
		// Token: 0x040039E6 RID: 14822
		Logo,
		// Token: 0x040039E7 RID: 14823
		ZoneLoad,
		// Token: 0x040039E8 RID: 14824
		Teleport,
		// Token: 0x040039E9 RID: 14825
		Exit
	}
}
