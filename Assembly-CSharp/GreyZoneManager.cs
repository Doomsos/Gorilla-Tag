using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020001C7 RID: 455
public class GreyZoneManager : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000C37 RID: 3127 RVA: 0x000428A2 File Offset: 0x00040AA2
	public bool GreyZoneActive
	{
		get
		{
			return this.greyZoneActive;
		}
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x06000C38 RID: 3128 RVA: 0x000428AC File Offset: 0x00040AAC
	public bool GreyZoneAvailable
	{
		get
		{
			bool result = false;
			if (GorillaComputer.instance != null)
			{
				result = (GorillaComputer.instance.GetServerTime().DayOfYear >= this.greyZoneAvailableDayOfYear);
			}
			return result;
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x06000C39 RID: 3129 RVA: 0x000428EB File Offset: 0x00040AEB
	public int GravityFactorSelection
	{
		get
		{
			return this.gravityFactorOptionSelection;
		}
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x06000C3A RID: 3130 RVA: 0x000428F3 File Offset: 0x00040AF3
	// (set) Token: 0x06000C3B RID: 3131 RVA: 0x000428FB File Offset: 0x00040AFB
	public bool TickRunning
	{
		get
		{
			return this._tickRunning;
		}
		set
		{
			this._tickRunning = value;
		}
	}

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x06000C3C RID: 3132 RVA: 0x00042904 File Offset: 0x00040B04
	public bool HasAuthority
	{
		get
		{
			return !PhotonNetwork.InRoom || base.photonView.IsMine;
		}
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x06000C3D RID: 3133 RVA: 0x0004291A File Offset: 0x00040B1A
	public float SummoningProgress
	{
		get
		{
			return this.summoningProgress;
		}
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x00042922 File Offset: 0x00040B22
	public void RegisterSummoner(GreyZoneSummoner summoner)
	{
		if (!this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Add(summoner);
		}
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x0004293E File Offset: 0x00040B3E
	public void DeregisterSummoner(GreyZoneSummoner summoner)
	{
		if (this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Remove(summoner);
		}
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x0004295B File Offset: 0x00040B5B
	public void RegisterMoon(MoonController moon)
	{
		this.moonController = moon;
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x00042964 File Offset: 0x00040B64
	public void UnregisterMoon(MoonController moon)
	{
		if (this.moonController == moon)
		{
			this.moonController = null;
		}
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x0004297B File Offset: 0x00040B7B
	public void ActivateGreyZoneAuthority()
	{
		this.greyZoneActive = true;
		this.photonConnectedDuringActivation = PhotonNetwork.InRoom;
		this.greyZoneActivationTime = (this.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
		this.ActivateGreyZoneLocal();
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x000429B0 File Offset: 0x00040BB0
	private void ActivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 1);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
			this.gravityOverrideSet = true;
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeOutMusic(2f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioIn(this.greyZoneAmbience, this.greyZoneAmbienceVolume, this.ambienceFadeTime));
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.GTPlay();
		}
		this.greyZoneParticles.gameObject.SetActive(true);
		this.summoningProgress = 1f;
		this.UpdateSummonerVisuals();
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].OnGreyZoneActivated();
		}
		if (this.OnGreyZoneActivated != null)
		{
			this.OnGreyZoneActivated.Invoke();
		}
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x00042AC4 File Offset: 0x00040CC4
	public void DeactivateGreyZoneAuthority()
	{
		this.greyZoneActive = false;
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			this.summoningPlayerProgress[keyValuePair.Key] = 0f;
		}
		this.DeactivateGreyZoneLocal();
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x00042B34 File Offset: 0x00040D34
	private void DeactivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(4f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioOut(this.greyZoneAmbience, this.ambienceFadeTime));
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated.Invoke();
		}
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x00042BDC File Offset: 0x00040DDC
	public void ForceStopGreyZone()
	{
		this.greyZoneActive = false;
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.UnsetGravityOverride(this);
		}
		this.gravityOverrideSet = false;
		if (this.moonController != null)
		{
			this.moonController.UpdateDistance(1f);
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(0f);
		}
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.volume = 0f;
			this.greyZoneAmbience.GTStop();
		}
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated.Invoke();
		}
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x00042CBC File Offset: 0x00040EBC
	public void GravityOverrideFunction(GTPlayer player)
	{
		this.gravityReductionAmount = 0f;
		if (this.moonController != null)
		{
			this.gravityReductionAmount = Mathf.InverseLerp(1f - this.skyMonsterDistGravityRampBuffer, this.skyMonsterDistGravityRampBuffer, this.moonController.Distance);
		}
		float num = Mathf.Lerp(1f, this.gravityFactorOptions[this.gravityFactorOptionSelection], this.gravityReductionAmount);
		player.AddForce(Physics.gravity * num * player.scale, 5);
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x00042D45 File Offset: 0x00040F45
	private IEnumerator FadeAudioIn(AudioSource source, float maxVolume, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			source.GTPlay();
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, maxVolume, num);
				yield return null;
			}
			source.volume = maxVolume;
		}
		yield break;
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x00042D62 File Offset: 0x00040F62
	private IEnumerator FadeAudioOut(AudioSource source, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, 0f, num);
				yield return null;
			}
			source.volume = 0f;
			source.Stop();
		}
		yield break;
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x00042D78 File Offset: 0x00040F78
	public void VRRigEnteredSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (!this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Add(rig.Creator.ActorNumber, new ValueTuple<VRRig, GreyZoneSummoner>(rig, summoner));
			this.summoningPlayerProgress.Add(rig.Creator.ActorNumber, 0f);
		}
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x00042DD8 File Offset: 0x00040FD8
	public void VRRigExitedSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Remove(rig.Creator.ActorNumber);
			this.summoningPlayerProgress.Remove(rig.Creator.ActorNumber);
		}
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x00042E2C File Offset: 0x0004102C
	private void UpdateSummonerVisuals()
	{
		bool greyZoneAvailable = this.GreyZoneAvailable;
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].UpdateProgressFeedback(greyZoneAvailable);
		}
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x00042E68 File Offset: 0x00041068
	private void ValidateSummoningPlayers()
	{
		this.invalidSummoners.Clear();
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			VRRig item = keyValuePair.Value.Item1;
			GreyZoneSummoner item2 = keyValuePair.Value.Item2;
			if (item.Creator.ActorNumber != keyValuePair.Key || (item.head.rigTarget.position - item2.SummoningFocusPoint).sqrMagnitude > item2.SummonerMaxDistance * item2.SummonerMaxDistance)
			{
				this.invalidSummoners.Add(keyValuePair.Key);
			}
		}
		foreach (int num in this.invalidSummoners)
		{
			this.summoningPlayers.Remove(num);
			this.summoningPlayerProgress.Remove(num);
		}
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x00042F90 File Offset: 0x00041190
	private int DayNightOverrideFunction(int inputIndex)
	{
		int num = 0;
		int num2 = 8;
		int num3 = inputIndex - num;
		int num4 = num2 - inputIndex;
		if (num3 <= 0 || num4 <= 0)
		{
			return inputIndex;
		}
		if (num4 > num3)
		{
			return num2;
		}
		return num;
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x00042FBA File Offset: 0x000411BA
	private void Awake()
	{
		if (GreyZoneManager.Instance == null)
		{
			GreyZoneManager.Instance = this;
			this.greyZoneAmbienceVolume = this.greyZoneAmbience.volume;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x00042FF0 File Offset: 0x000411F0
	private void OnEnable()
	{
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.SetTimeIndexOverrideFunction(new Func<int, int>(this.DayNightOverrideFunction));
			}
		}
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x00043028 File Offset: 0x00041228
	private void OnDisable()
	{
		this.ForceStopGreyZone();
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.UnsetTimeIndexOverrideFunction();
			}
		}
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x0004305A File Offset: 0x0004125A
	private void Update()
	{
		if (this.HasAuthority)
		{
			this.AuthorityUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x00043070 File Offset: 0x00041270
	private void AuthorityUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (this.greyZoneActive)
		{
			this.summoningProgress = 1f;
			double num;
			if (this.photonConnectedDuringActivation && PhotonNetwork.InRoom)
			{
				num = PhotonNetwork.Time;
			}
			else if (!this.photonConnectedDuringActivation && !PhotonNetwork.InRoom)
			{
				num = (double)Time.time;
			}
			else
			{
				num = -100.0;
			}
			if (num > this.greyZoneActivationTime + (double)this.greyZoneActiveDuration || num < this.greyZoneActivationTime - 10.0)
			{
				this.DeactivateGreyZoneAuthority();
				return;
			}
		}
		else if (this.GreyZoneAvailable)
		{
			this.roomPlayerList = PhotonNetwork.PlayerList;
			int num2 = 1;
			if (this.roomPlayerList != null && this.roomPlayerList.Length != 0)
			{
				num2 = Mathf.Max((this.roomPlayerList.Length + 1) / 2, 1);
			}
			float num3 = 0f;
			float num4 = 1f / this.summoningActivationTime;
			foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
			{
				VRRig item = keyValuePair.Value.Item1;
				GreyZoneSummoner item2 = keyValuePair.Value.Item2;
				float num5 = this.summoningPlayerProgress[keyValuePair.Key];
				Vector3 vector = item2.SummoningFocusPoint - item.leftHand.rigTarget.position;
				Vector3 vector2 = -item.leftHand.rigTarget.right;
				bool flag = Vector3.Dot(vector, vector2) > 0f;
				Vector3 vector3 = item2.SummoningFocusPoint - item.rightHand.rigTarget.position;
				Vector3 right = item.rightHand.rigTarget.right;
				bool flag2 = Vector3.Dot(vector3, right) > 0f;
				if (flag && flag2)
				{
					num5 = Mathf.MoveTowards(num5, 1f, num4 * deltaTime);
				}
				else
				{
					num5 = Mathf.MoveTowards(num5, 0f, num4 * deltaTime);
				}
				num3 += num5;
				this.summoningPlayerProgress[keyValuePair.Key] = num5;
			}
			float num6 = 0.95f;
			this.summoningProgress = Mathf.Clamp01(num3 / num6 / (float)num2);
			this.UpdateSummonerVisuals();
			if (this.summoningProgress > 0.99f)
			{
				this.ActivateGreyZoneAuthority();
			}
		}
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x000432C8 File Offset: 0x000414C8
	private void SharedUpdate()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (this.greyZoneActive)
		{
			Vector3 vector = Vector3.ClampMagnitude(instance.InstantaneousVelocity * this.particlePredictiveSpawnVelocityFactor, this.particlePredictiveSpawnMaxDist);
			this.greyZoneParticles.transform.position = instance.HeadCenterPosition + Vector3.down * 0.5f + vector;
		}
		else if (this.gravityOverrideSet && this.gravityReductionAmount < 0.01f)
		{
			instance.UnsetGravityOverride(this);
			this.gravityOverrideSet = false;
		}
		float num = this.greyZoneActive ? 0f : 1f;
		float num2 = this.greyZoneActive ? this.skyMonsterMovementEnterTime : this.skyMonsterMovementExitTime;
		if (this.moonController != null && this.moonController.Distance != num)
		{
			float num3 = Mathf.SmoothDamp(this.moonController.Distance, num, ref this.skyMonsterMovementVelocity, num2);
			if ((double)Mathf.Abs(num3 - num) < 0.001)
			{
				num3 = num;
			}
			this.moonController.UpdateDistance(num3);
		}
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x000433DC File Offset: 0x000415DC
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.greyZoneActive);
			stream.SendNext(this.greyZoneActivationTime);
			stream.SendNext(this.photonConnectedDuringActivation);
			stream.SendNext(this.gravityFactorOptionSelection);
			stream.SendNext(this.summoningProgress);
			return;
		}
		if (stream.IsReading && info.Sender.IsMasterClient)
		{
			bool flag = this.greyZoneActive;
			this.greyZoneActive = (bool)stream.ReceiveNext();
			this.greyZoneActivationTime = ((double)stream.ReceiveNext()).GetFinite();
			this.photonConnectedDuringActivation = (bool)stream.ReceiveNext();
			this.gravityFactorOptionSelection = (int)stream.ReceiveNext();
			this.summoningProgress = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			this.UpdateSummonerVisuals();
			if (this.greyZoneActive && !flag)
			{
				this.ActivateGreyZoneLocal();
				return;
			}
			if (!this.greyZoneActive && flag)
			{
				this.DeactivateGreyZoneLocal();
			}
		}
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x000434FD File Offset: 0x000416FD
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x00002789 File Offset: 0x00000989
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x00002789 File Offset: 0x00000989
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x000434FD File Offset: 0x000416FD
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x04000F25 RID: 3877
	[OnEnterPlay_SetNull]
	public static volatile GreyZoneManager Instance;

	// Token: 0x04000F26 RID: 3878
	[SerializeField]
	private float greyZoneActiveDuration = 90f;

	// Token: 0x04000F27 RID: 3879
	[SerializeField]
	private float[] gravityFactorOptions = new float[]
	{
		0.25f,
		0.5f,
		0.75f
	};

	// Token: 0x04000F28 RID: 3880
	[SerializeField]
	private int gravityFactorOptionSelection = 1;

	// Token: 0x04000F29 RID: 3881
	[SerializeField]
	private float summoningActivationTime = 3f;

	// Token: 0x04000F2A RID: 3882
	[SerializeField]
	private AudioSource greyZoneAmbience;

	// Token: 0x04000F2B RID: 3883
	[SerializeField]
	private float ambienceFadeTime = 4f;

	// Token: 0x04000F2C RID: 3884
	[SerializeField]
	private bool forceTimeOfDayToNight;

	// Token: 0x04000F2D RID: 3885
	[SerializeField]
	private float skyMonsterMovementEnterTime = 4.5f;

	// Token: 0x04000F2E RID: 3886
	[SerializeField]
	private float skyMonsterMovementExitTime = 3.2f;

	// Token: 0x04000F2F RID: 3887
	[SerializeField]
	private float skyMonsterDistGravityRampBuffer = 0.15f;

	// Token: 0x04000F30 RID: 3888
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityReductionAmount = 1f;

	// Token: 0x04000F31 RID: 3889
	[SerializeField]
	private ParticleSystem greyZoneParticles;

	// Token: 0x04000F32 RID: 3890
	[SerializeField]
	private float particlePredictiveSpawnMaxDist = 4f;

	// Token: 0x04000F33 RID: 3891
	[SerializeField]
	private float particlePredictiveSpawnVelocityFactor = 0.5f;

	// Token: 0x04000F34 RID: 3892
	private bool photonConnectedDuringActivation;

	// Token: 0x04000F35 RID: 3893
	private double greyZoneActivationTime;

	// Token: 0x04000F36 RID: 3894
	private bool greyZoneActive;

	// Token: 0x04000F37 RID: 3895
	private bool _tickRunning;

	// Token: 0x04000F38 RID: 3896
	private float summoningProgress;

	// Token: 0x04000F39 RID: 3897
	private List<GreyZoneSummoner> activeSummoners = new List<GreyZoneSummoner>();

	// Token: 0x04000F3A RID: 3898
	private Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>> summoningPlayers = new Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>>();

	// Token: 0x04000F3B RID: 3899
	private Dictionary<int, float> summoningPlayerProgress = new Dictionary<int, float>();

	// Token: 0x04000F3C RID: 3900
	private HashSet<int> invalidSummoners = new HashSet<int>();

	// Token: 0x04000F3D RID: 3901
	private Coroutine audioFadeCoroutine;

	// Token: 0x04000F3E RID: 3902
	private Player[] roomPlayerList;

	// Token: 0x04000F3F RID: 3903
	private ShaderHashId _GreyZoneActive = new ShaderHashId("_GreyZoneActive");

	// Token: 0x04000F40 RID: 3904
	private MoonController moonController;

	// Token: 0x04000F41 RID: 3905
	private float skyMonsterMovementVelocity;

	// Token: 0x04000F42 RID: 3906
	private bool gravityOverrideSet;

	// Token: 0x04000F43 RID: 3907
	private float greyZoneAmbienceVolume = 0.15f;

	// Token: 0x04000F44 RID: 3908
	private int greyZoneAvailableDayOfYear = new DateTime(2024, 10, 25).DayOfYear;

	// Token: 0x04000F45 RID: 3909
	public Action OnGreyZoneActivated;

	// Token: 0x04000F46 RID: 3910
	public Action OnGreyZoneDeactivated;
}
