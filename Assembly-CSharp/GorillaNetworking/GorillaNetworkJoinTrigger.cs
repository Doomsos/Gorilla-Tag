using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking
{
	// Token: 0x02000EFA RID: 3834
	public class GorillaNetworkJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x06006039 RID: 24633 RVA: 0x001F09C4 File Offset: 0x001EEBC4
		public GroupJoinZoneAB groupJoinRequiredZonesAB
		{
			get
			{
				return new GroupJoinZoneAB
				{
					a = this.groupJoinRequiredZones,
					b = this.groupJoinRequiredZonesB
				};
			}
		}

		// Token: 0x0600603A RID: 24634 RVA: 0x001F09F4 File Offset: 0x001EEBF4
		private void Start()
		{
			if (this.primaryTriggerForMyZone == null)
			{
				this.primaryTriggerForMyZone = this;
			}
			if (this.primaryTriggerForMyZone == this)
			{
				GorillaComputer.instance.RegisterPrimaryJoinTrigger(this);
			}
			PhotonNetworkController.Instance.RegisterJoinTrigger(this);
			if (!this.didRegisterForCallbacks && this.ui != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
		}

		// Token: 0x0600603B RID: 24635 RVA: 0x001F0A74 File Offset: 0x001EEC74
		public void RegisterUI(JoinTriggerUI ui)
		{
			this.ui = ui;
			if (!this.didRegisterForCallbacks && FriendshipGroupDetection.Instance != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
			this.UpdateUI();
		}

		// Token: 0x0600603C RID: 24636 RVA: 0x001F0AC0 File Offset: 0x001EECC0
		public void UnregisterUI(JoinTriggerUI ui)
		{
			this.ui = null;
		}

		// Token: 0x0600603D RID: 24637 RVA: 0x001F0AC9 File Offset: 0x001EECC9
		private void OnDestroy()
		{
			if (this.didRegisterForCallbacks)
			{
				FriendshipGroupDetection.Instance.RemoveGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
		}

		// Token: 0x0600603E RID: 24638 RVA: 0x001F0AE9 File Offset: 0x001EECE9
		private void OnGroupPositionsChanged(GroupJoinZoneAB groupZone)
		{
			this.UpdateUI();
		}

		// Token: 0x0600603F RID: 24639 RVA: 0x001F0AF4 File Offset: 0x001EECF4
		public void UpdateUI()
		{
			if (this.ui == null || NetworkSystem.Instance == null)
			{
				return;
			}
			if (GorillaScoreboardTotalUpdater.instance.offlineTextErrorString != null)
			{
				this.ui.SetState(JoinTriggerVisualState.ConnectionError, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.ui.SetState(JoinTriggerVisualState.InPrivateRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
			{
				this.ui.SetState(JoinTriggerVisualState.AlreadyInRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				this.ui.SetState(this.CanPartyJoin() ? JoinTriggerVisualState.LeaveRoomAndPartyJoin : JoinTriggerVisualState.AbandonPartyAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				this.ui.SetState(JoinTriggerVisualState.NotConnectedSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (PhotonNetworkController.Instance.currentJoinTrigger == this.primaryTriggerForMyZone)
			{
				this.ui.SetState(JoinTriggerVisualState.ChangingGameModeSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			this.ui.SetState(JoinTriggerVisualState.LeaveRoomAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
		}

		// Token: 0x06006040 RID: 24640 RVA: 0x001F0D3E File Offset: 0x001EEF3E
		private string GetActiveNetworkZone()
		{
			return PhotonNetworkController.Instance.currentJoinTrigger.networkZone.ToUpper();
		}

		// Token: 0x06006041 RID: 24641 RVA: 0x001F0D56 File Offset: 0x001EEF56
		private string GetDesiredNetworkZone()
		{
			return this.networkZone.ToUpper();
		}

		// Token: 0x06006042 RID: 24642 RVA: 0x001F0D63 File Offset: 0x001EEF63
		public static string GetActiveGameType()
		{
			GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
			return ((activeGameMode != null) ? activeGameMode.GameModeName() : null) ?? "";
		}

		// Token: 0x06006043 RID: 24643 RVA: 0x001F0D80 File Offset: 0x001EEF80
		public string GetDesiredGameType()
		{
			return GameMode.GameModeZoneMapping.VerifyModeForZone(this.zone, Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true), NetworkSystem.Instance.SessionIsPrivate).ToString();
		}

		// Token: 0x06006044 RID: 24644 RVA: 0x001F0DCC File Offset: 0x001EEFCC
		public string GetDesiredGameTypeLocalized()
		{
			return GorillaGameManager.GameModeEnumToName(GameMode.GameModeZoneMapping.VerifyModeForZone(this.zone, Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true), NetworkSystem.Instance.SessionIsPrivate));
		}

		// Token: 0x06006045 RID: 24645 RVA: 0x001F0E04 File Offset: 0x001EF004
		public virtual string GetFullDesiredGameModeString()
		{
			return this.networkZone + GorillaComputer.instance.currentQueue + this.GetDesiredGameType();
		}

		// Token: 0x06006046 RID: 24646 RVA: 0x001F0E23 File Offset: 0x001EF023
		public virtual byte GetRoomSize()
		{
			return RoomSystem.GetRoomSizeForCreate(this.networkZone);
		}

		// Token: 0x06006047 RID: 24647 RVA: 0x001F0E30 File Offset: 0x001EF030
		public bool CanPartyJoin()
		{
			return this.CanPartyJoin(FriendshipGroupDetection.Instance.partyZone);
		}

		// Token: 0x06006048 RID: 24648 RVA: 0x001F0E42 File Offset: 0x001EF042
		public bool CanPartyJoin(GroupJoinZoneAB zone)
		{
			return (this.groupJoinRequiredZonesAB & zone) == zone;
		}

		// Token: 0x06006049 RID: 24649 RVA: 0x001F0E58 File Offset: 0x001EF058
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (GorillaNetworkJoinTrigger.triggerJoinsDisabled)
			{
				Debug.Log("GorillaNetworkJoinTrigger::OnBoxTriggered - blocking join call");
				return;
			}
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			if (NetworkSystem.Instance.groupJoinInProgress)
			{
				return;
			}
			List<ValueTuple<string, string>> list = new List<ValueTuple<string, string>>();
			foreach (AdditionalCustomProperty additionalCustomProperty in this.additionalJoinCustomProperties)
			{
				list.Add(new ValueTuple<string, string>(additionalCustomProperty.key, additionalCustomProperty.value));
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (this.ignoredIfInParty)
				{
					return;
				}
				if (NetworkSystem.Instance.netState == NetSystemState.Connecting || NetworkSystem.Instance.netState == NetSystemState.Disconnecting || NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
				{
					return;
				}
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
					{
						Debug.Log("JoinTrigger: Ignoring party join/leave because " + this.networkZone + " is already the game mode");
						return;
					}
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						Debug.Log("JoinTrigger: Ignoring party join/leave because we're in a private room");
						return;
					}
				}
				if (this.CanPartyJoin())
				{
					Debug.Log(string.Format("JoinTrigger: Attempting party join in 1 second! <{0}> accepts <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
					PhotonNetworkController.Instance.DeferJoining(1f);
					FriendshipGroupDetection.Instance.SendAboutToGroupJoin();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.JoinWithParty, list);
					return;
				}
				Debug.Log(string.Format("JoinTrigger: LeaveGroup: Leaving party and will solo join, wanted <{0}> but got <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
				FriendshipGroupDetection.Instance.LeaveParty();
				PhotonNetworkController.Instance.DeferJoining(1f);
			}
			else
			{
				Debug.Log("JoinTrigger: Solo join (not in a group)");
				PhotonNetworkController.Instance.ClearDeferredJoin();
			}
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.Solo, list);
		}

		// Token: 0x0600604A RID: 24650 RVA: 0x001F1043 File Offset: 0x001EF243
		public static void DisableTriggerJoins()
		{
			Debug.Log("[GorillaNetworkJoinTrigger::DisableTriggerJoins] Disabling Trigger-based Room Joins...");
			GorillaNetworkJoinTrigger.triggerJoinsDisabled = true;
		}

		// Token: 0x0600604B RID: 24651 RVA: 0x001F1055 File Offset: 0x001EF255
		public static void EnableTriggerJoins()
		{
			Debug.Log("[GorillaNetworkJoinTrigger::EnableTriggerJoins] Enabling Trigger-based Room Joins...");
			GorillaNetworkJoinTrigger.triggerJoinsDisabled = false;
		}

		// Token: 0x04006EE8 RID: 28392
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04006EE9 RID: 28393
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04006EEA RID: 28394
		public GTZone zone;

		// Token: 0x04006EEB RID: 28395
		public GroupJoinZoneA groupJoinRequiredZones;

		// Token: 0x04006EEC RID: 28396
		public GroupJoinZoneB groupJoinRequiredZonesB;

		// Token: 0x04006EED RID: 28397
		[FormerlySerializedAs("gameModeName")]
		public string networkZone;

		// Token: 0x04006EEE RID: 28398
		public string componentTypeToAdd;

		// Token: 0x04006EEF RID: 28399
		public GameObject componentTarget;

		// Token: 0x04006EF0 RID: 28400
		public GorillaFriendCollider myCollider;

		// Token: 0x04006EF1 RID: 28401
		public GorillaNetworkJoinTrigger primaryTriggerForMyZone;

		// Token: 0x04006EF2 RID: 28402
		public bool ignoredIfInParty;

		// Token: 0x04006EF3 RID: 28403
		private JoinTriggerUI ui;

		// Token: 0x04006EF4 RID: 28404
		private bool didRegisterForCallbacks;

		// Token: 0x04006EF5 RID: 28405
		public AdditionalCustomProperty[] additionalJoinCustomProperties;

		// Token: 0x04006EF6 RID: 28406
		private static bool triggerJoinsDisabled;
	}
}
