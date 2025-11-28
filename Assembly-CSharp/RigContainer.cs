using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using GorillaTag.Audio;
using Newtonsoft.Json;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

// Token: 0x02000761 RID: 1889
[RequireComponent(typeof(VRRig), typeof(VRRigReliableState))]
public class RigContainer : MonoBehaviour
{
	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x060030DA RID: 12506 RVA: 0x0010A76D File Offset: 0x0010896D
	// (set) Token: 0x060030DB RID: 12507 RVA: 0x0010A775 File Offset: 0x00108975
	public bool Initialized { get; private set; }

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x060030DC RID: 12508 RVA: 0x0010A77E File Offset: 0x0010897E
	public VRRig Rig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x060030DD RID: 12509 RVA: 0x0010A786 File Offset: 0x00108986
	public VRRigReliableState ReliableState
	{
		get
		{
			return this.reliableState;
		}
	}

	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x060030DE RID: 12510 RVA: 0x0010A78E File Offset: 0x0010898E
	public Transform SpeakerHead
	{
		get
		{
			return this.speakerHead;
		}
	}

	// Token: 0x1700044C RID: 1100
	// (get) Token: 0x060030DF RID: 12511 RVA: 0x0010A796 File Offset: 0x00108996
	public AudioSource ReplacementVoiceSource
	{
		get
		{
			return this.replacementVoiceSource;
		}
	}

	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x060030E0 RID: 12512 RVA: 0x0010A79E File Offset: 0x0010899E
	public List<LoudSpeakerNetwork> LoudSpeakerNetworks
	{
		get
		{
			return this.loudSpeakerNetworks;
		}
	}

	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x060030E1 RID: 12513 RVA: 0x0010A7A6 File Offset: 0x001089A6
	public LCKSocialCameraFollower LckCococamFollower
	{
		get
		{
			return this.m_lckCococamFollower;
		}
	}

	// Token: 0x1700044F RID: 1103
	// (get) Token: 0x060030E2 RID: 12514 RVA: 0x0010A7AE File Offset: 0x001089AE
	public LCKSocialCameraFollower LCKTabletFollower
	{
		get
		{
			return this.m_lckTablet;
		}
	}

	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x060030E3 RID: 12515 RVA: 0x0010A7B6 File Offset: 0x001089B6
	// (set) Token: 0x060030E4 RID: 12516 RVA: 0x0010A7BE File Offset: 0x001089BE
	public PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
		set
		{
			if (value == this.voiceView)
			{
				return;
			}
			if (this.voiceView != null)
			{
				this.voiceView.SpeakerInUse.enabled = false;
			}
			this.voiceView = value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x060030E5 RID: 12517 RVA: 0x0010A7FB File Offset: 0x001089FB
	public NetworkView netView
	{
		get
		{
			return this.vrrig.netView;
		}
	}

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x060030E6 RID: 12518 RVA: 0x0010A808 File Offset: 0x00108A08
	public int CachedNetViewID
	{
		get
		{
			return this.m_cachedNetViewID;
		}
	}

	// Token: 0x17000453 RID: 1107
	// (get) Token: 0x060030E7 RID: 12519 RVA: 0x0010A810 File Offset: 0x00108A10
	// (set) Token: 0x060030E8 RID: 12520 RVA: 0x0010A81B File Offset: 0x00108A1B
	public bool Muted
	{
		get
		{
			return !this.enableVoice;
		}
		set
		{
			this.enableVoice = !value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x17000454 RID: 1108
	// (get) Token: 0x060030E9 RID: 12521 RVA: 0x0010A82D File Offset: 0x00108A2D
	// (set) Token: 0x060030EA RID: 12522 RVA: 0x0010A83A File Offset: 0x00108A3A
	public NetPlayer Creator
	{
		get
		{
			return this.vrrig.creator;
		}
		set
		{
			if (this.vrrig.isOfflineVRRig || (this.vrrig.creator != null && this.vrrig.creator.InRoom))
			{
				return;
			}
			this.vrrig.creator = value;
		}
	}

	// Token: 0x17000455 RID: 1109
	// (get) Token: 0x060030EB RID: 12523 RVA: 0x0010A875 File Offset: 0x00108A75
	// (set) Token: 0x060030EC RID: 12524 RVA: 0x0010A87D File Offset: 0x00108A7D
	public bool ForceMute
	{
		get
		{
			return this.forceMute;
		}
		set
		{
			this.forceMute = value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x17000456 RID: 1110
	// (get) Token: 0x060030ED RID: 12525 RVA: 0x0010A88C File Offset: 0x00108A8C
	public SphereCollider HeadCollider
	{
		get
		{
			return this.headCollider;
		}
	}

	// Token: 0x17000457 RID: 1111
	// (get) Token: 0x060030EE RID: 12526 RVA: 0x0010A894 File Offset: 0x00108A94
	public CapsuleCollider BodyCollider
	{
		get
		{
			return this.bodyCollider;
		}
	}

	// Token: 0x17000458 RID: 1112
	// (get) Token: 0x060030EF RID: 12527 RVA: 0x0010A89C File Offset: 0x00108A9C
	public VRRigEvents RigEvents
	{
		get
		{
			return this.rigEvents;
		}
	}

	// Token: 0x060030F0 RID: 12528 RVA: 0x0010A8A4 File Offset: 0x00108AA4
	public bool GetIsPlayerAutoMuted()
	{
		return this.bPlayerAutoMuted;
	}

	// Token: 0x060030F1 RID: 12529 RVA: 0x0010A8AC File Offset: 0x00108AAC
	public void UpdateAutomuteLevel(string autoMuteLevel)
	{
		if (autoMuteLevel.Equals("LOW", 5))
		{
			this.playerChatQuality = 1;
		}
		else if (autoMuteLevel.Equals("HIGH", 5))
		{
			this.playerChatQuality = 0;
		}
		else if (autoMuteLevel.Equals("ERROR", 5))
		{
			this.playerChatQuality = 2;
		}
		else
		{
			this.playerChatQuality = 2;
		}
		this.RefreshVoiceChat();
	}

	// Token: 0x060030F2 RID: 12530 RVA: 0x0010A90B File Offset: 0x00108B0B
	private void Awake()
	{
		this.loudSpeakerNetworks = new List<LoudSpeakerNetwork>();
	}

	// Token: 0x060030F3 RID: 12531 RVA: 0x0010A918 File Offset: 0x00108B18
	private void Start()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.LocalPlayer;
			RoomSystem.JoinedRoomEvent += new Action(this.OnMultiPlayerStarted);
			RoomSystem.LeftRoomEvent += new Action(this.OnReturnedToSinglePlayer);
		}
		else
		{
			this.rigEvents.enableEvent += new Action<RigContainer>(this.RigPostEnable);
		}
		this.Rig.rigContainer = this;
	}

	// Token: 0x060030F4 RID: 12532 RVA: 0x0010A9AD File Offset: 0x00108BAD
	private void RigPostEnable(RigContainer _)
	{
		this.vrrig.UpdateName();
	}

	// Token: 0x060030F5 RID: 12533 RVA: 0x0010A9BA File Offset: 0x00108BBA
	private void OnMultiPlayerStarted()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.GetLocalPlayer();
		}
	}

	// Token: 0x060030F6 RID: 12534 RVA: 0x0010A9DE File Offset: 0x00108BDE
	private void OnReturnedToSinglePlayer()
	{
		if (this.Rig.isOfflineVRRig)
		{
			RigContainer.CancelAutomuteRequest();
		}
	}

	// Token: 0x060030F7 RID: 12535 RVA: 0x0010A9F4 File Offset: 0x00108BF4
	private void OnDisable()
	{
		this.Initialized = false;
		this.enableVoice = true;
		this.voiceView = null;
		base.gameObject.transform.localPosition = Vector3.zero;
		base.gameObject.transform.localRotation = Quaternion.identity;
		this.vrrig.syncPos = base.gameObject.transform.position;
		this.vrrig.syncRotation = base.gameObject.transform.rotation;
		this.forceMute = false;
	}

	// Token: 0x060030F8 RID: 12536 RVA: 0x0010AA7D File Offset: 0x00108C7D
	internal void InitializeNetwork(NetworkView netView, PhotonVoiceView voiceView, VRRigSerializer vrRigSerializer)
	{
		if (!netView || !voiceView)
		{
			return;
		}
		this.InitializeNetwork_Shared(netView, vrRigSerializer);
		this.Voice = voiceView;
		this.vrrig.voiceAudio = voiceView.SpeakerInUse.GetComponent<AudioSource>();
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x0010AAB8 File Offset: 0x00108CB8
	private void InitializeNetwork_Shared(NetworkView netView, VRRigSerializer vrRigSerializer)
	{
		if (this.vrrig.netView)
		{
			GorillaNot.instance.SendReport("inappropriate tag data being sent creating multiple vrrigs", this.Creator.UserId, this.Creator.NickName);
			if (this.vrrig.netView.IsMine)
			{
				NetworkSystem.Instance.NetDestroy(this.vrrig.gameObject);
			}
			else
			{
				this.vrrig.netView.gameObject.SetActive(false);
			}
		}
		this.vrrig.netView = netView;
		this.vrrig.rigSerializer = vrRigSerializer;
		this.vrrig.OwningNetPlayer = NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject));
		this.m_cachedNetViewID = netView.ViewID;
		if (!this.Initialized)
		{
			this.vrrig.NetInitialize();
			if (GorillaGameManager.instance != null && NetworkSystem.Instance.IsMasterClient)
			{
				int owningPlayerID = NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject);
				bool playerTutorialCompletion = NetworkSystem.Instance.GetPlayerTutorialCompletion(owningPlayerID);
				GorillaGameManager.instance.NewVRRig(netView.Owner, netView.ViewID, playerTutorialCompletion);
			}
			bool isLocal = this.vrrig.OwningNetPlayer.IsLocal;
			if (this.vrrig.InitializedCosmetics)
			{
				netView.SendRPC("RPC_RequestCosmetics", netView.Owner, Array.Empty<object>());
			}
		}
		this.Initialized = true;
		if (!this.vrrig.isOfflineVRRig)
		{
			base.StartCoroutine(RigContainer.QueueAutomute(this.Creator));
		}
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x0010AC43 File Offset: 0x00108E43
	private static IEnumerator QueueAutomute(NetPlayer player)
	{
		RigContainer.playersToCheckAutomute.Add(player);
		if (!RigContainer.automuteQueued)
		{
			RigContainer.automuteQueued = true;
			yield return new WaitForSecondsRealtime(1f);
			while (RigContainer.waitingForAutomuteCallback)
			{
				yield return null;
			}
			RigContainer.automuteQueued = false;
			RigContainer.RequestAutomuteSettings();
		}
		yield break;
	}

	// Token: 0x060030FB RID: 12539 RVA: 0x0010AC54 File Offset: 0x00108E54
	private static void RequestAutomuteSettings()
	{
		if (RigContainer.playersToCheckAutomute.Count == 0)
		{
			return;
		}
		RigContainer.waitingForAutomuteCallback = true;
		RigContainer.playersToCheckAutomute.RemoveAll((NetPlayer player) => player == null);
		RigContainer.requestedAutomutePlayers = new List<NetPlayer>(RigContainer.playersToCheckAutomute);
		RigContainer.playersToCheckAutomute.Clear();
		string[] array = Enumerable.ToArray<string>(Enumerable.Select<NetPlayer, string>(RigContainer.requestedAutomutePlayers, (NetPlayer x) => x.UserId));
		foreach (NetPlayer netPlayer in RigContainer.requestedAutomutePlayers)
		{
		}
		ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
		executeFunctionRequest.Entity = new EntityKey
		{
			Id = PlayFabSettings.staticPlayer.EntityId,
			Type = PlayFabSettings.staticPlayer.EntityType
		};
		executeFunctionRequest.FunctionName = "ShouldUserAutomutePlayer";
		executeFunctionRequest.FunctionParameter = string.Join(",", array);
		PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
			if (dictionary == null)
			{
				using (List<NetPlayer>.Enumerator enumerator2 = RigContainer.requestedAutomutePlayers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NetPlayer netPlayer2 = enumerator2.Current;
						if (netPlayer2 != null)
						{
							RigContainer.ReceiveAutomuteSettings(netPlayer2, "none");
						}
					}
					goto IL_A6;
				}
			}
			foreach (NetPlayer netPlayer3 in RigContainer.requestedAutomutePlayers)
			{
				if (netPlayer3 != null)
				{
					string score;
					if (dictionary.TryGetValue(netPlayer3.UserId, ref score))
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, score);
					}
					else
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, "none");
					}
				}
			}
			IL_A6:
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, delegate(PlayFabError error)
		{
			foreach (NetPlayer player in RigContainer.requestedAutomutePlayers)
			{
				RigContainer.ReceiveAutomuteSettings(player, "ERROR");
			}
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, null, null);
	}

	// Token: 0x060030FC RID: 12540 RVA: 0x0010ADB8 File Offset: 0x00108FB8
	private static void CancelAutomuteRequest()
	{
		RigContainer.playersToCheckAutomute.Clear();
		RigContainer.automuteQueued = false;
		if (RigContainer.requestedAutomutePlayers != null)
		{
			RigContainer.requestedAutomutePlayers.Clear();
		}
		RigContainer.waitingForAutomuteCallback = false;
	}

	// Token: 0x060030FD RID: 12541 RVA: 0x0010ADE4 File Offset: 0x00108FE4
	private static void ReceiveAutomuteSettings(NetPlayer player, string score)
	{
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer != null)
		{
			rigContainer.UpdateAutomuteLevel(score);
		}
	}

	// Token: 0x060030FE RID: 12542 RVA: 0x0010AE10 File Offset: 0x00109010
	private void ProcessAutomute()
	{
		int @int = PlayerPrefs.GetInt("autoMute", 1);
		this.bPlayerAutoMuted = (!this.hasManualMute && this.playerChatQuality < @int);
	}

	// Token: 0x060030FF RID: 12543 RVA: 0x0010AE44 File Offset: 0x00109044
	public void RefreshVoiceChat()
	{
		if (this.Voice == null)
		{
			return;
		}
		this.ProcessAutomute();
		this.Voice.SpeakerInUse.enabled = (!this.forceMute && this.enableVoice && !this.bPlayerAutoMuted && GorillaComputer.instance.voiceChatOn == "TRUE");
		this.replacementVoiceSource.mute = (this.forceMute || !this.enableVoice || this.bPlayerAutoMuted || GorillaComputer.instance.voiceChatOn == "OFF");
	}

	// Token: 0x06003100 RID: 12544 RVA: 0x0010AEE3 File Offset: 0x001090E3
	public void AddLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		if (this.loudSpeakerNetworks.Contains(network))
		{
			return;
		}
		this.loudSpeakerNetworks.Add(network);
	}

	// Token: 0x06003101 RID: 12545 RVA: 0x0010AF00 File Offset: 0x00109100
	public void RemoveLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		this.loudSpeakerNetworks.Remove(network);
	}

	// Token: 0x06003102 RID: 12546 RVA: 0x0010AF10 File Offset: 0x00109110
	public static void RefreshAllRigVoices()
	{
		RigContainer.staticTempRC = null;
		if (!NetworkSystem.Instance.InRoom || VRRigCache.Instance == null)
		{
			return;
		}
		foreach (NetPlayer targetPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			if (VRRigCache.Instance.TryGetVrrig(targetPlayer, out RigContainer.staticTempRC))
			{
				RigContainer.staticTempRC.RefreshVoiceChat();
			}
		}
	}

	// Token: 0x04003FC0 RID: 16320
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x04003FC1 RID: 16321
	[SerializeField]
	private VRRigReliableState reliableState;

	// Token: 0x04003FC2 RID: 16322
	[SerializeField]
	private Transform speakerHead;

	// Token: 0x04003FC3 RID: 16323
	[SerializeField]
	private AudioSource replacementVoiceSource;

	// Token: 0x04003FC4 RID: 16324
	private List<LoudSpeakerNetwork> loudSpeakerNetworks;

	// Token: 0x04003FC5 RID: 16325
	[SerializeField]
	private LCKSocialCameraFollower m_lckCococamFollower;

	// Token: 0x04003FC6 RID: 16326
	[SerializeField]
	private LCKSocialCameraFollower m_lckTablet;

	// Token: 0x04003FC7 RID: 16327
	private PhotonVoiceView voiceView;

	// Token: 0x04003FC8 RID: 16328
	private int m_cachedNetViewID;

	// Token: 0x04003FC9 RID: 16329
	private bool enableVoice = true;

	// Token: 0x04003FCA RID: 16330
	private bool forceMute;

	// Token: 0x04003FCB RID: 16331
	[SerializeField]
	private SphereCollider headCollider;

	// Token: 0x04003FCC RID: 16332
	[SerializeField]
	private CapsuleCollider bodyCollider;

	// Token: 0x04003FCD RID: 16333
	[SerializeField]
	private VRRigEvents rigEvents;

	// Token: 0x04003FCE RID: 16334
	public bool hasManualMute;

	// Token: 0x04003FCF RID: 16335
	private bool bPlayerAutoMuted;

	// Token: 0x04003FD0 RID: 16336
	public int playerChatQuality = 2;

	// Token: 0x04003FD1 RID: 16337
	private static List<NetPlayer> playersToCheckAutomute = new List<NetPlayer>();

	// Token: 0x04003FD2 RID: 16338
	private static bool automuteQueued = false;

	// Token: 0x04003FD3 RID: 16339
	private static List<NetPlayer> requestedAutomutePlayers;

	// Token: 0x04003FD4 RID: 16340
	private static bool waitingForAutomuteCallback = false;

	// Token: 0x04003FD5 RID: 16341
	private static RigContainer staticTempRC;
}
