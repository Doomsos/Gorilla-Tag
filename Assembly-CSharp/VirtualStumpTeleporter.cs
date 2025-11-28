using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using TMPro;
using UnityEngine;

// Token: 0x020009B5 RID: 2485
public class VirtualStumpTeleporter : MonoBehaviour, IBuildValidation, IGorillaSliceableSimple
{
	// Token: 0x06003F7C RID: 16252 RVA: 0x001547ED File Offset: 0x001529ED
	public bool BuildValidationCheck()
	{
		if (this.netSerializer.IsNull())
		{
			Debug.LogError("VStump Teleporter \"" + base.gameObject.GetPath() + "\" needs a reference to a VirtualStumpTeleporterSerializer for networked FX to function. Check out the teleporter prefabs in arcade or the stump", this);
			return false;
		}
		return true;
	}

	// Token: 0x06003F7D RID: 16253 RVA: 0x00154820 File Offset: 0x00152A20
	public void SliceUpdate()
	{
		if (!this.accessDenied && NetworkSystem.Instance.netState != NetSystemState.Idle && NetworkSystem.Instance.netState != NetSystemState.InGame)
		{
			this.DenyAccess();
		}
		if (this.accessDenied && (NetworkSystem.Instance.netState == NetSystemState.Idle || NetworkSystem.Instance.netState == NetSystemState.InGame) && !UGCPermissionManager.IsUGCDisabled)
		{
			this.AllowAccess();
		}
	}

	// Token: 0x06003F7E RID: 16254 RVA: 0x00154884 File Offset: 0x00152A84
	public void OnEnable()
	{
		if (this.netSerializer.IsNull())
		{
			Debug.LogWarning("[VStumpTeleporter.OnEnable] Net Serializer is null for \"" + base.gameObject.GetPath() + "\", networked teleport FX will not function.");
		}
		if (UGCPermissionManager.IsUGCDisabled || (NetworkSystem.Instance.netState != NetSystemState.Idle && NetworkSystem.Instance.netState != NetSystemState.InGame))
		{
			ushort num = VirtualStumpTeleporter.lastLoggingHandsMsgId;
			VirtualStumpTeleporter.lastLoggingHandsMsgId = 1;
			this.DenyAccess();
		}
		else
		{
			ushort num2 = VirtualStumpTeleporter.lastLoggingHandsMsgId;
			VirtualStumpTeleporter.lastLoggingHandsMsgId = 2;
			this.AllowAccess();
		}
		UGCPermissionManager.SubscribeToUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.SubscribeToUGCDisabled(new Action(this.OnUGCDisabled));
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003F7F RID: 16255 RVA: 0x00154931 File Offset: 0x00152B31
	public void OnDisable()
	{
		this.AllowAccess();
		UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(this.OnUGCEnabled));
		UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(this.OnUGCDisabled));
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003F80 RID: 16256 RVA: 0x00154962 File Offset: 0x00152B62
	private void OnUGCEnabled()
	{
		this.AllowAccess();
		ushort num = VirtualStumpTeleporter.lastLoggingHandsMsgId;
		VirtualStumpTeleporter.lastLoggingHandsMsgId = 3;
	}

	// Token: 0x06003F81 RID: 16257 RVA: 0x00154978 File Offset: 0x00152B78
	private void OnUGCDisabled()
	{
		this.DenyAccess();
		ushort num = VirtualStumpTeleporter.lastLoggingHandsMsgId;
		VirtualStumpTeleporter.lastLoggingHandsMsgId = 4;
	}

	// Token: 0x06003F82 RID: 16258 RVA: 0x00154990 File Offset: 0x00152B90
	public void OnTriggerEnter(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied || this.teleporting || CustomMapManager.WaitingForRoomJoin || CustomMapManager.WaitingForDisconnect)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = Time.time;
			this.ShowCountdownText();
		}
	}

	// Token: 0x06003F83 RID: 16259 RVA: 0x001549F0 File Offset: 0x00152BF0
	public void OnTriggerStay(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject && this.triggerEntryTime >= 0f)
		{
			this.UpdateCountdownText();
			if (!this.teleporting && this.triggerEntryTime + this.stayInTriggerDuration <= Time.time)
			{
				this.TeleportPlayer();
				this.HideCountdownText();
			}
		}
	}

	// Token: 0x06003F84 RID: 16260 RVA: 0x00154A64 File Offset: 0x00152C64
	public void OnTriggerExit(Collider other)
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (other.gameObject == GorillaTagger.Instance.headCollider.gameObject)
		{
			this.triggerEntryTime = -1f;
			this.HideCountdownText();
		}
	}

	// Token: 0x06003F85 RID: 16261 RVA: 0x00154AA4 File Offset: 0x00152CA4
	private void ShowCountdownText()
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			int num = 1 + Mathf.FloorToInt(this.stayInTriggerDuration);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num.ToString();
					this.countdownTexts[i].gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06003F86 RID: 16262 RVA: 0x00154B28 File Offset: 0x00152D28
	private void HideCountdownText()
	{
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = "";
					this.countdownTexts[i].gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06003F87 RID: 16263 RVA: 0x00154B8C File Offset: 0x00152D8C
	private void UpdateCountdownText()
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (!this.countdownTexts.IsNullOrEmpty<TMP_Text>())
		{
			float num = this.stayInTriggerDuration - (Time.time - this.triggerEntryTime);
			int num2 = 1 + Mathf.FloorToInt(num);
			for (int i = 0; i < this.countdownTexts.Length; i++)
			{
				if (!this.countdownTexts[i].IsNull())
				{
					this.countdownTexts[i].text = num2.ToString();
				}
			}
		}
	}

	// Token: 0x06003F88 RID: 16264 RVA: 0x00154C09 File Offset: 0x00152E09
	public void TeleportPlayer()
	{
		if (UGCPermissionManager.IsUGCDisabled || this.accessDenied)
		{
			return;
		}
		if (!this.teleporting)
		{
			this.teleporting = true;
			CustomMapManager.TeleportToVirtualStump(this, new Action<bool>(this.FinishTeleport));
		}
	}

	// Token: 0x06003F89 RID: 16265 RVA: 0x00154C3C File Offset: 0x00152E3C
	private void FinishTeleport(bool success = true)
	{
		if (this.teleporting)
		{
			this.teleporting = false;
			this.triggerEntryTime = -1f;
		}
	}

	// Token: 0x06003F8A RID: 16266 RVA: 0x00154C58 File Offset: 0x00152E58
	private void DenyAccess()
	{
		this.accessDenied = true;
		foreach (GameObject gameObject in this.accessDeniedEnabledObjects)
		{
			gameObject.SetActive(true);
		}
		foreach (GameObject gameObject2 in this.accessDeniedDisabledObjects)
		{
			gameObject2.SetActive(false);
		}
	}

	// Token: 0x06003F8B RID: 16267 RVA: 0x00154CF0 File Offset: 0x00152EF0
	private void AllowAccess()
	{
		if (UGCPermissionManager.IsUGCDisabled)
		{
			return;
		}
		this.accessDenied = false;
		foreach (GameObject gameObject in this.accessDeniedEnabledObjects)
		{
			gameObject.SetActive(false);
		}
		foreach (GameObject gameObject2 in this.accessDeniedDisabledObjects)
		{
			gameObject2.SetActive(true);
		}
	}

	// Token: 0x06003F8C RID: 16268 RVA: 0x00154D90 File Offset: 0x00152F90
	private short GetIndex()
	{
		if (!this.netSerializer.IsNotNull())
		{
			return -1;
		}
		return this.netSerializer.GetTeleporterIndex(this);
	}

	// Token: 0x06003F8D RID: 16269 RVA: 0x00154DAD File Offset: 0x00152FAD
	public GTZone GetZone()
	{
		return this.entranceZone;
	}

	// Token: 0x06003F8E RID: 16270 RVA: 0x00154DB5 File Offset: 0x00152FB5
	public GorillaNetworkJoinTrigger GetExitVStumpJoinTrigger()
	{
		return this.exitVStumpJoinTrigger;
	}

	// Token: 0x06003F8F RID: 16271 RVA: 0x00154DBD File Offset: 0x00152FBD
	public Transform GetReturnTransform()
	{
		return this.returnLocation;
	}

	// Token: 0x06003F90 RID: 16272 RVA: 0x00154DC5 File Offset: 0x00152FC5
	public long GetAutoLoadMapModId()
	{
		return this.autoLoadMapModId;
	}

	// Token: 0x06003F91 RID: 16273 RVA: 0x00154DCD File Offset: 0x00152FCD
	public GameModeType GetAutoLoadGamemode()
	{
		return this.autoLoadGamemode;
	}

	// Token: 0x06003F92 RID: 16274 RVA: 0x00154DD5 File Offset: 0x00152FD5
	public GameModeType GetReturnGamemode()
	{
		return this.forcedGamemodeUponReturn;
	}

	// Token: 0x06003F93 RID: 16275 RVA: 0x00154DE0 File Offset: 0x00152FE0
	public void PlayTeleportEffects(bool forLocalPlayer, bool toVStump, AudioSource vStumpSFXAudioSource = null, bool sendRPC = false)
	{
		if (sendRPC && this.netSerializer.IsNotNull())
		{
			this.netSerializer.NotifyPlayerTeleporting(this.GetIndex(), vStumpSFXAudioSource);
		}
		ParticleSystem particleSystem;
		if (toVStump)
		{
			particleSystem = this.teleportToVStumpVFX;
			if (forLocalPlayer && vStumpSFXAudioSource.IsNotNull() && !this.teleportingPlayerSoundClips.IsNullOrEmpty<AudioClip>())
			{
				vStumpSFXAudioSource.clip = this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)];
				vStumpSFXAudioSource.Play();
			}
			if (!forLocalPlayer && this.teleporterSFXAudioSource.IsNotNull() && !this.observerSoundClips.IsNullOrEmpty<AudioClip>())
			{
				this.teleporterSFXAudioSource.clip = this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)];
				this.teleporterSFXAudioSource.Play();
			}
		}
		else
		{
			particleSystem = this.returnFromVStumpVFX;
			if (this.teleporterSFXAudioSource.IsNotNull())
			{
				if (forLocalPlayer && !this.teleportingPlayerSoundClips.IsNullOrEmpty<AudioClip>())
				{
					this.teleporterSFXAudioSource.clip = this.teleportingPlayerSoundClips[Random.Range(0, this.teleportingPlayerSoundClips.Count)];
				}
				else if (!forLocalPlayer && !this.observerSoundClips.IsNullOrEmpty<AudioClip>())
				{
					this.teleporterSFXAudioSource.clip = this.observerSoundClips[Random.Range(0, this.observerSoundClips.Count)];
				}
				this.teleporterSFXAudioSource.Play();
			}
		}
		if (particleSystem.IsNotNull())
		{
			particleSystem.Play();
		}
	}

	// Token: 0x040050A5 RID: 20645
	[SerializeField]
	private float stayInTriggerDuration = 3f;

	// Token: 0x040050A6 RID: 20646
	[SerializeField]
	private TMP_Text[] countdownTexts;

	// Token: 0x040050A7 RID: 20647
	[SerializeField]
	private GameObject[] handHoldObjects;

	// Token: 0x040050A8 RID: 20648
	[SerializeField]
	private List<GameObject> accessDeniedDisabledObjects = new List<GameObject>();

	// Token: 0x040050A9 RID: 20649
	[SerializeField]
	private List<GameObject> accessDeniedEnabledObjects = new List<GameObject>();

	// Token: 0x040050AA RID: 20650
	[SerializeField]
	private Transform returnLocation;

	// Token: 0x040050AB RID: 20651
	[SerializeField]
	private GTZone entranceZone = GTZone.arcade;

	// Token: 0x040050AC RID: 20652
	[SerializeField]
	private GorillaNetworkJoinTrigger exitVStumpJoinTrigger;

	// Token: 0x040050AD RID: 20653
	[SerializeField]
	private long autoLoadMapModId = ModId.Null;

	// Token: 0x040050AE RID: 20654
	[SerializeField]
	private GameModeType autoLoadGamemode = GameModeType.None;

	// Token: 0x040050AF RID: 20655
	[SerializeField]
	private GameModeType forcedGamemodeUponReturn = GameModeType.None;

	// Token: 0x040050B0 RID: 20656
	[SerializeField]
	private ParticleSystem teleportToVStumpVFX;

	// Token: 0x040050B1 RID: 20657
	[SerializeField]
	private ParticleSystem returnFromVStumpVFX;

	// Token: 0x040050B2 RID: 20658
	[SerializeField]
	private AudioSource teleporterSFXAudioSource;

	// Token: 0x040050B3 RID: 20659
	[SerializeField]
	private List<AudioClip> teleportingPlayerSoundClips = new List<AudioClip>();

	// Token: 0x040050B4 RID: 20660
	[SerializeField]
	private List<AudioClip> observerSoundClips = new List<AudioClip>();

	// Token: 0x040050B5 RID: 20661
	[SerializeField]
	private VirtualStumpTeleporterSerializer netSerializer;

	// Token: 0x040050B6 RID: 20662
	private VirtualStumpTeleporterSerializer mySerializer;

	// Token: 0x040050B7 RID: 20663
	private bool accessDenied;

	// Token: 0x040050B8 RID: 20664
	private bool teleporting;

	// Token: 0x040050B9 RID: 20665
	private float triggerEntryTime = -1f;

	// Token: 0x040050BA RID: 20666
	[OnEnterPlay_Set(0)]
	private static ushort lastLoggingHandsMsgId;
}
