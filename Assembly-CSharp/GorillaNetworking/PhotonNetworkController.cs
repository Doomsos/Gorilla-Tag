using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000F0D RID: 3853
	public class PhotonNetworkController : MonoBehaviour
	{
		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x0600608E RID: 24718 RVA: 0x001F1F95 File Offset: 0x001F0195
		// (set) Token: 0x0600608F RID: 24719 RVA: 0x001F1F9D File Offset: 0x001F019D
		public List<string> FriendIDList
		{
			get
			{
				return this.friendIDList;
			}
			set
			{
				this.friendIDList = value;
			}
		}

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x06006090 RID: 24720 RVA: 0x001F1FA6 File Offset: 0x001F01A6
		// (set) Token: 0x06006091 RID: 24721 RVA: 0x001F1FAE File Offset: 0x001F01AE
		public string StartLevel
		{
			get
			{
				return this.startLevel;
			}
			set
			{
				this.startLevel = value;
			}
		}

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x06006092 RID: 24722 RVA: 0x001F1FB7 File Offset: 0x001F01B7
		// (set) Token: 0x06006093 RID: 24723 RVA: 0x001F1FBF File Offset: 0x001F01BF
		public GTZone StartZone
		{
			get
			{
				return this.startZone;
			}
			set
			{
				this.startZone = value;
			}
		}

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x06006094 RID: 24724 RVA: 0x001F1FC8 File Offset: 0x001F01C8
		public GTZone CurrentRoomZone
		{
			get
			{
				if (!(this.currentJoinTrigger != null))
				{
					return GTZone.none;
				}
				return this.currentJoinTrigger.zone;
			}
		}

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x06006095 RID: 24725 RVA: 0x001F1FE6 File Offset: 0x001F01E6
		// (set) Token: 0x06006096 RID: 24726 RVA: 0x001F1FEE File Offset: 0x001F01EE
		public GorillaGeoHideShowTrigger StartGeoTrigger
		{
			get
			{
				return this.startGeoTrigger;
			}
			set
			{
				this.startGeoTrigger = value;
			}
		}

		// Token: 0x06006097 RID: 24727 RVA: 0x001F1FF8 File Offset: 0x001F01F8
		public void Awake()
		{
			if (PhotonNetworkController.Instance == null)
			{
				PhotonNetworkController.Instance = this;
			}
			else if (PhotonNetworkController.Instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			this.updatedName = false;
			this.playersInRegion = new int[this.serverRegions.Length];
			this.pingInRegion = new int[this.serverRegions.Length];
		}

		// Token: 0x06006098 RID: 24728 RVA: 0x001F2068 File Offset: 0x001F0268
		public void Start()
		{
			base.StartCoroutine(this.DisableOnStart());
			NetworkSystem.Instance.OnJoinedRoomEvent += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnDisconnected);
			PhotonNetwork.NetworkingClient.LoadBalancingPeer.ReuseEventInstance = true;
		}

		// Token: 0x06006099 RID: 24729 RVA: 0x001F20D4 File Offset: 0x001F02D4
		private IEnumerator DisableOnStart()
		{
			ZoneManagement.SetActiveZone(this.StartZone);
			yield break;
		}

		// Token: 0x0600609A RID: 24730 RVA: 0x001F20E4 File Offset: 0x001F02E4
		public void FixedUpdate()
		{
			this.headRightHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.GetControllerTransform(false).position).magnitude;
			this.headLeftHandDistance = (GTPlayer.Instance.headCollider.transform.position - GTPlayer.Instance.GetControllerTransform(true).position).magnitude;
			this.headQuat = GTPlayer.Instance.headCollider.transform.rotation;
			if (!this.disableAFKKick && Quaternion.Angle(this.headQuat, this.lastHeadQuat) <= 0.01f && Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) < 0.001f && Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) < 0.001f && this.pauseTime + this.disconnectTime < Time.realtimeSinceStartup)
			{
				this.pauseTime = Time.realtimeSinceStartup;
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			else if (Quaternion.Angle(this.headQuat, this.lastHeadQuat) > 0.01f || Mathf.Abs(this.headRightHandDistance - this.lastHeadRightHandDistance) >= 0.001f || Mathf.Abs(this.headLeftHandDistance - this.lastHeadLeftHandDistance) >= 0.001f)
			{
				this.pauseTime = Time.realtimeSinceStartup;
			}
			this.lastHeadRightHandDistance = this.headRightHandDistance;
			this.lastHeadLeftHandDistance = this.headLeftHandDistance;
			this.lastHeadQuat = this.headQuat;
			if (this.deferredJoin && Time.time >= this.partyJoinDeferredUntilTimestamp)
			{
				if ((this.partyJoinDeferredUntilTimestamp != 0f || NetworkSystem.Instance.netState == NetSystemState.Idle) && this.currentJoinTrigger != null)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
					if (this.currentJoinTrigger == this.privateTrigger)
					{
						this.AttemptToJoinSpecificRoom(this.customRoomID, FriendshipGroupDetection.Instance.IsInParty ? JoinType.JoinWithParty : JoinType.Solo);
						return;
					}
					this.AttemptToJoinPublicRoom(this.currentJoinTrigger, this.currentJoinType, null);
					return;
				}
				else if (NetworkSystem.Instance.netState != NetSystemState.PingRecon && NetworkSystem.Instance.netState != NetSystemState.Initialization)
				{
					this.deferredJoin = false;
					this.partyJoinDeferredUntilTimestamp = 0f;
				}
			}
		}

		// Token: 0x0600609B RID: 24731 RVA: 0x001F2334 File Offset: 0x001F0534
		public void DeferJoining(float duration)
		{
			this.partyJoinDeferredUntilTimestamp = Mathf.Max(this.partyJoinDeferredUntilTimestamp, Time.time + duration);
		}

		// Token: 0x0600609C RID: 24732 RVA: 0x001F234E File Offset: 0x001F054E
		public void ClearDeferredJoin()
		{
			this.partyJoinDeferredUntilTimestamp = 0f;
			this.deferredJoin = false;
		}

		// Token: 0x0600609D RID: 24733 RVA: 0x001F2362 File Offset: 0x001F0562
		public void AttemptToJoinPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo, List<ValueTuple<string, string>> additionalCustomProperties = null)
		{
			this.AttemptToJoinPublicRoomAsync(triggeredTrigger, roomJoinType, additionalCustomProperties);
		}

		// Token: 0x0600609E RID: 24734 RVA: 0x001F2370 File Offset: 0x001F0570
		private void AttemptToJoinPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType, List<ValueTuple<string, string>> additionalCustomProperties)
		{
			PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__68 <AttemptToJoinPublicRoomAsync>d__;
			<AttemptToJoinPublicRoomAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AttemptToJoinPublicRoomAsync>d__.<>4__this = this;
			<AttemptToJoinPublicRoomAsync>d__.triggeredTrigger = triggeredTrigger;
			<AttemptToJoinPublicRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinPublicRoomAsync>d__.additionalCustomProperties = additionalCustomProperties;
			<AttemptToJoinPublicRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinPublicRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinPublicRoomAsync>d__68>(ref <AttemptToJoinPublicRoomAsync>d__);
		}

		// Token: 0x0600609F RID: 24735 RVA: 0x001F23C0 File Offset: 0x001F05C0
		public void AttemptToJoinRankedPublicRoom(GorillaNetworkJoinTrigger triggeredTrigger, JoinType roomJoinType = JoinType.Solo)
		{
			string mmrTier = RankedProgressionManager.Instance.GetRankedMatchmakingTier().ToString();
			string platform = "PC";
			this.AttemptToJoinRankedPublicRoomAsync(triggeredTrigger, mmrTier, platform, roomJoinType);
		}

		// Token: 0x060060A0 RID: 24736 RVA: 0x001F23FC File Offset: 0x001F05FC
		private void AttemptToJoinRankedPublicRoomAsync(GorillaNetworkJoinTrigger triggeredTrigger, string mmrTier, string platform, JoinType roomJoinType)
		{
			PhotonNetworkController.<AttemptToJoinRankedPublicRoomAsync>d__70 <AttemptToJoinRankedPublicRoomAsync>d__;
			<AttemptToJoinRankedPublicRoomAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AttemptToJoinRankedPublicRoomAsync>d__.<>4__this = this;
			<AttemptToJoinRankedPublicRoomAsync>d__.triggeredTrigger = triggeredTrigger;
			<AttemptToJoinRankedPublicRoomAsync>d__.mmrTier = mmrTier;
			<AttemptToJoinRankedPublicRoomAsync>d__.platform = platform;
			<AttemptToJoinRankedPublicRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinRankedPublicRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinRankedPublicRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinRankedPublicRoomAsync>d__70>(ref <AttemptToJoinRankedPublicRoomAsync>d__);
		}

		// Token: 0x060060A1 RID: 24737 RVA: 0x001F2454 File Offset: 0x001F0654
		private Task SendPartyFollowCommands()
		{
			PhotonNetworkController.<SendPartyFollowCommands>d__71 <SendPartyFollowCommands>d__;
			<SendPartyFollowCommands>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendPartyFollowCommands>d__.<>1__state = -1;
			<SendPartyFollowCommands>d__.<>t__builder.Start<PhotonNetworkController.<SendPartyFollowCommands>d__71>(ref <SendPartyFollowCommands>d__);
			return <SendPartyFollowCommands>d__.<>t__builder.Task;
		}

		// Token: 0x060060A2 RID: 24738 RVA: 0x001F248F File Offset: 0x001F068F
		public void AttemptToJoinSpecificRoom(string roomID, JoinType roomJoinType)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, null);
		}

		// Token: 0x060060A3 RID: 24739 RVA: 0x001F249B File Offset: 0x001F069B
		public void AttemptToJoinSpecificRoomWithCallback(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			this.AttemptToJoinSpecificRoomAsync(roomID, roomJoinType, callback);
		}

		// Token: 0x060060A4 RID: 24740 RVA: 0x001F24A8 File Offset: 0x001F06A8
		public Task AttemptToJoinSpecificRoomAsync(string roomID, JoinType roomJoinType, Action<NetJoinResult> callback)
		{
			PhotonNetworkController.<AttemptToJoinSpecificRoomAsync>d__74 <AttemptToJoinSpecificRoomAsync>d__;
			<AttemptToJoinSpecificRoomAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AttemptToJoinSpecificRoomAsync>d__.<>4__this = this;
			<AttemptToJoinSpecificRoomAsync>d__.roomID = roomID;
			<AttemptToJoinSpecificRoomAsync>d__.roomJoinType = roomJoinType;
			<AttemptToJoinSpecificRoomAsync>d__.callback = callback;
			<AttemptToJoinSpecificRoomAsync>d__.<>1__state = -1;
			<AttemptToJoinSpecificRoomAsync>d__.<>t__builder.Start<PhotonNetworkController.<AttemptToJoinSpecificRoomAsync>d__74>(ref <AttemptToJoinSpecificRoomAsync>d__);
			return <AttemptToJoinSpecificRoomAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060060A5 RID: 24741 RVA: 0x001F2504 File Offset: 0x001F0704
		private void DisconnectCleanup()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			if (GorillaParent.instance != null)
			{
				GorillaScoreboardSpawner[] componentsInChildren = GorillaParent.instance.GetComponentsInChildren<GorillaScoreboardSpawner>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnLeftRoom();
				}
			}
			this.attemptingToConnect = true;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in this.offlineVRRig)
			{
				if (skinnedMeshRenderer != null)
				{
					skinnedMeshRenderer.enabled = true;
				}
			}
			if (GorillaComputer.instance != null && !ApplicationQuittingState.IsQuitting)
			{
				this.UpdateTriggerScreens();
			}
			GTPlayer.Instance.maxJumpSpeed = 6.5f;
			GTPlayer.Instance.jumpMultiplier = 1.1f;
			GorillaNot.instance.currentMasterClient = null;
			GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
			this.initialGameMode = "";
		}

		// Token: 0x060060A6 RID: 24742 RVA: 0x001F25E4 File Offset: 0x001F07E4
		public void OnJoinedRoom()
		{
			if (NetworkSystem.Instance.GameModeString.IsNullOrEmpty())
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
			this.initialGameMode = NetworkSystem.Instance.GameModeString;
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.currentJoinTrigger = this.privateTrigger;
				PhotonNetworkController.Instance.UpdateTriggerScreens();
			}
			else if (this.currentJoinType != JoinType.FollowingParty)
			{
				bool flag = false;
				for (int i = 0; i < GorillaComputer.instance.allowedMapsToJoin.Length; i++)
				{
					if (NetworkSystem.Instance.GameModeString.StartsWith(GorillaComputer.instance.allowedMapsToJoin[i]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					GorillaComputer.instance.roomNotAllowed = true;
					NetworkSystem.Instance.ReturnToSinglePlayer();
					return;
				}
			}
			NetworkSystem.Instance.SetMyTutorialComplete();
			VRRigCache.Instance.InstantiateNetworkObject();
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GameMode.LoadGameModeFromProperty(this.initialGameMode);
			}
			GorillaComputer.instance.roomFull = false;
			GorillaComputer.instance.roomNotAllowed = false;
			if (this.currentJoinType == JoinType.JoinWithParty || this.currentJoinType == JoinType.JoinWithNearby || this.currentJoinType == JoinType.ForceJoinWithParty || this.currentJoinType == JoinType.JoinWithElevator)
			{
				this.keyToFollow = NetworkSystem.Instance.LocalPlayer.UserId + this.keyStr;
				NetworkSystem.Instance.BroadcastMyRoom(true, this.keyToFollow, this.shuffler);
			}
			GorillaNot.instance.currentMasterClient = null;
			this.UpdateCurrentJoinTrigger();
			this.UpdateTriggerScreens();
			NetworkSystem.Instance.MultiplayerStarted();
		}

		// Token: 0x060060A7 RID: 24743 RVA: 0x001F276A File Offset: 0x001F096A
		public void RegisterJoinTrigger(GorillaNetworkJoinTrigger trigger)
		{
			this.allJoinTriggers.Add(trigger);
		}

		// Token: 0x060060A8 RID: 24744 RVA: 0x001F2778 File Offset: 0x001F0978
		private void UpdateCurrentJoinTrigger()
		{
			GorillaNetworkJoinTrigger joinTriggerFromFullGameModeString = GorillaComputer.instance.GetJoinTriggerFromFullGameModeString(NetworkSystem.Instance.GameModeString);
			if (joinTriggerFromFullGameModeString != null)
			{
				this.currentJoinTrigger = joinTriggerFromFullGameModeString;
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				if (this.currentJoinTrigger != this.privateTrigger)
				{
					Debug.LogError("IN a private game but private trigger isnt current");
					return;
				}
			}
			else
			{
				Debug.LogError("Not in private room and unabel tp update jointrigger.");
			}
		}

		// Token: 0x060060A9 RID: 24745 RVA: 0x001F27E4 File Offset: 0x001F09E4
		public void UpdateTriggerScreens()
		{
			foreach (GorillaNetworkJoinTrigger gorillaNetworkJoinTrigger in this.allJoinTriggers)
			{
				gorillaNetworkJoinTrigger.UpdateUI();
			}
		}

		// Token: 0x060060AA RID: 24746 RVA: 0x001F2834 File Offset: 0x001F0A34
		public void AttemptToFollowIntoPub(string userIDToFollow, int actorNumberToFollow, string newKeyStr, string shufflerStr, JoinType joinType)
		{
			this.friendToFollow = userIDToFollow;
			this.keyToFollow = userIDToFollow + newKeyStr;
			this.shuffler = shufflerStr;
			this.currentJoinType = joinType;
			this.ClearDeferredJoin();
			if (NetworkSystem.Instance.InRoom)
			{
				NetworkSystem.Instance.JoinFriendsRoom(this.friendToFollow, actorNumberToFollow, this.keyToFollow, this.shuffler);
			}
		}

		// Token: 0x060060AB RID: 24747 RVA: 0x001F2895 File Offset: 0x001F0A95
		public void OnDisconnected()
		{
			this.DisconnectCleanup();
		}

		// Token: 0x060060AC RID: 24748 RVA: 0x001F289D File Offset: 0x001F0A9D
		public void OnApplicationQuit()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion != "dev";
			}
		}

		// Token: 0x060060AD RID: 24749 RVA: 0x001F28C0 File Offset: 0x001F0AC0
		private string ReturnRoomName()
		{
			if (this.isPrivate)
			{
				return this.customRoomID;
			}
			return this.RandomRoomName();
		}

		// Token: 0x060060AE RID: 24750 RVA: 0x001F28D8 File Offset: 0x001F0AD8
		private string RandomRoomName()
		{
			string text = "";
			for (int i = 0; i < 4; i++)
			{
				text += "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Substring(Random.Range(0, "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789".Length), 1);
			}
			if (GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				return text;
			}
			return this.RandomRoomName();
		}

		// Token: 0x060060AF RID: 24751 RVA: 0x001F2930 File Offset: 0x001F0B30
		private string GetRegionWithLowestPing()
		{
			int num = 10000;
			int num2 = 0;
			for (int i = 0; i < this.serverRegions.Length; i++)
			{
				Debug.Log("ping in region " + this.serverRegions[i] + " is " + this.pingInRegion[i].ToString());
				if (this.pingInRegion[i] < num && this.pingInRegion[i] > 0)
				{
					num = this.pingInRegion[i];
					num2 = i;
				}
			}
			return this.serverRegions[num2];
		}

		// Token: 0x060060B0 RID: 24752 RVA: 0x001F29B0 File Offset: 0x001F0BB0
		public int TotalUsers()
		{
			int num = 0;
			foreach (int num2 in this.playersInRegion)
			{
				num += num2;
			}
			return num;
		}

		// Token: 0x060060B1 RID: 24753 RVA: 0x001F29E0 File Offset: 0x001F0BE0
		public string CurrentState()
		{
			if (NetworkSystem.Instance == null)
			{
				Debug.Log("Null netsys!!!");
			}
			return NetworkSystem.Instance.netState.ToString();
		}

		// Token: 0x060060B2 RID: 24754 RVA: 0x001F2A1C File Offset: 0x001F0C1C
		private void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				this.timeWhenApplicationPaused = new DateTime?(DateTime.Now);
				return;
			}
			if ((DateTime.Now - (this.timeWhenApplicationPaused ?? DateTime.Now)).TotalSeconds > (double)this.disconnectTime)
			{
				this.timeWhenApplicationPaused = default(DateTime?);
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance != null)
				{
					instance.ReturnToSinglePlayer();
				}
			}
			if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance2 = NetworkSystem.Instance;
				if (instance2 == null)
				{
					return;
				}
				instance2.ReturnToSinglePlayer();
			}
		}

		// Token: 0x060060B3 RID: 24755 RVA: 0x001F2AC9 File Offset: 0x001F0CC9
		private void OnApplicationFocus(bool focus)
		{
			if (!focus && NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom && NetworkSystem.Instance.netState == NetSystemState.InGame)
			{
				NetworkSystem instance = NetworkSystem.Instance;
				if (instance == null)
				{
					return;
				}
				instance.ReturnToSinglePlayer();
			}
		}

		// Token: 0x04006F36 RID: 28470
		[OnEnterPlay_SetNull]
		public static volatile PhotonNetworkController Instance;

		// Token: 0x04006F37 RID: 28471
		public int incrementCounter;

		// Token: 0x04006F38 RID: 28472
		public PlayFabAuthenticator playFabAuthenticator;

		// Token: 0x04006F39 RID: 28473
		public string[] serverRegions;

		// Token: 0x04006F3A RID: 28474
		public bool isPrivate;

		// Token: 0x04006F3B RID: 28475
		public string customRoomID;

		// Token: 0x04006F3C RID: 28476
		public GameObject playerOffset;

		// Token: 0x04006F3D RID: 28477
		public SkinnedMeshRenderer[] offlineVRRig;

		// Token: 0x04006F3E RID: 28478
		public bool attemptingToConnect;

		// Token: 0x04006F3F RID: 28479
		private int currentRegionIndex;

		// Token: 0x04006F40 RID: 28480
		public string currentGameType;

		// Token: 0x04006F41 RID: 28481
		public bool roomCosmeticsInitialized;

		// Token: 0x04006F42 RID: 28482
		public GameObject photonVoiceObjectPrefab;

		// Token: 0x04006F43 RID: 28483
		public Dictionary<string, bool> playerCosmeticsLookup = new Dictionary<string, bool>();

		// Token: 0x04006F44 RID: 28484
		private float lastHeadRightHandDistance;

		// Token: 0x04006F45 RID: 28485
		private float lastHeadLeftHandDistance;

		// Token: 0x04006F46 RID: 28486
		private float pauseTime;

		// Token: 0x04006F47 RID: 28487
		private float disconnectTime = 120f;

		// Token: 0x04006F48 RID: 28488
		public bool disableAFKKick;

		// Token: 0x04006F49 RID: 28489
		private float headRightHandDistance;

		// Token: 0x04006F4A RID: 28490
		private float headLeftHandDistance;

		// Token: 0x04006F4B RID: 28491
		private Quaternion headQuat;

		// Token: 0x04006F4C RID: 28492
		private Quaternion lastHeadQuat;

		// Token: 0x04006F4D RID: 28493
		public GameObject[] disableOnStartup;

		// Token: 0x04006F4E RID: 28494
		public GameObject[] enableOnStartup;

		// Token: 0x04006F4F RID: 28495
		public bool updatedName;

		// Token: 0x04006F50 RID: 28496
		private int[] playersInRegion;

		// Token: 0x04006F51 RID: 28497
		private int[] pingInRegion;

		// Token: 0x04006F52 RID: 28498
		private List<string> friendIDList = new List<string>();

		// Token: 0x04006F53 RID: 28499
		private JoinType currentJoinType;

		// Token: 0x04006F54 RID: 28500
		private string friendToFollow;

		// Token: 0x04006F55 RID: 28501
		private string keyToFollow;

		// Token: 0x04006F56 RID: 28502
		public string shuffler;

		// Token: 0x04006F57 RID: 28503
		public string keyStr;

		// Token: 0x04006F58 RID: 28504
		private string platformTag = "OTHER";

		// Token: 0x04006F59 RID: 28505
		private string startLevel;

		// Token: 0x04006F5A RID: 28506
		[SerializeField]
		private GTZone startZone;

		// Token: 0x04006F5B RID: 28507
		private GorillaGeoHideShowTrigger startGeoTrigger;

		// Token: 0x04006F5C RID: 28508
		public GorillaNetworkJoinTrigger privateTrigger;

		// Token: 0x04006F5D RID: 28509
		internal string initialGameMode = "";

		// Token: 0x04006F5E RID: 28510
		public GorillaNetworkJoinTrigger currentJoinTrigger;

		// Token: 0x04006F5F RID: 28511
		public string autoJoinRoom;

		// Token: 0x04006F60 RID: 28512
		public string autoJoinGameMode;

		// Token: 0x04006F61 RID: 28513
		private bool deferredJoin;

		// Token: 0x04006F62 RID: 28514
		private float partyJoinDeferredUntilTimestamp;

		// Token: 0x04006F63 RID: 28515
		private DateTime? timeWhenApplicationPaused;

		// Token: 0x04006F64 RID: 28516
		[NetworkPrefab]
		[SerializeField]
		private NetworkObject testPlayerPrefab;

		// Token: 0x04006F65 RID: 28517
		private List<GorillaNetworkJoinTrigger> allJoinTriggers = new List<GorillaNetworkJoinTrigger>();
	}
}
