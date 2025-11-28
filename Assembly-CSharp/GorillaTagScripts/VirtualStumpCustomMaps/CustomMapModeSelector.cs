using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000E20 RID: 3616
	public class CustomMapModeSelector : GameModeSelectorButtonLayout
	{
		// Token: 0x06005A52 RID: 23122 RVA: 0x001CEC62 File Offset: 0x001CCE62
		private void Awake()
		{
			CustomMapModeSelector.instances.AddIfNew(this);
		}

		// Token: 0x06005A53 RID: 23123 RVA: 0x001CEC70 File Offset: 0x001CCE70
		public void OnEnable()
		{
			if (GorillaComputer.instance != null)
			{
				this.SetupButtons();
				GorillaComputer.instance.SetGameModeWithoutButton(CustomMapModeSelector.defaultGamemodeForLoadedMap.ToString());
			}
			RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnMasterClientSwitchedEvent += new Action<NetPlayer>(this.OnRoomHostSwitched);
			NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnDisconnected);
			this.roomHostDescriptionText.SetActive(false);
			this.roomHostText.gameObject.SetActive(false);
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
			{
				this.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
			}
		}

		// Token: 0x06005A54 RID: 23124 RVA: 0x001CED50 File Offset: 0x001CCF50
		public void OnDisable()
		{
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnMasterClientSwitchedEvent -= new Action<NetPlayer>(this.OnRoomHostSwitched);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnDisconnected);
		}

		// Token: 0x06005A55 RID: 23125 RVA: 0x001CEDBA File Offset: 0x001CCFBA
		private void OnJoinedRoom()
		{
			this.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
		}

		// Token: 0x06005A56 RID: 23126 RVA: 0x001CEDCC File Offset: 0x001CCFCC
		private void OnRoomHostSwitched(NetPlayer newRoomHost)
		{
			if (!NetworkSystem.Instance.InRoom || !NetworkSystem.Instance.SessionIsPrivate)
			{
				return;
			}
			CustomMapModeSelector.reusableString = this.notInRoomHostString;
			if (!newRoomHost.IsNull)
			{
				this.roomHostDescriptionText.SetActive(true);
				CustomMapModeSelector.reusableString = newRoomHost.DefaultName;
				if (GorillaComputer.instance.NametagsEnabled && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags))
				{
					RigContainer rigContainer;
					if (newRoomHost.IsLocal)
					{
						CustomMapModeSelector.reusableString = newRoomHost.NickName;
					}
					else if (VRRigCache.Instance.TryGetVrrig(newRoomHost, out rigContainer))
					{
						CustomMapModeSelector.reusableString = rigContainer.Rig.playerNameVisible;
					}
				}
			}
			this.roomHostText.text = this.roomHostLabel + CustomMapModeSelector.reusableString;
			this.roomHostText.gameObject.SetActive(true);
		}

		// Token: 0x06005A57 RID: 23127 RVA: 0x001CEE92 File Offset: 0x001CD092
		private void OnDisconnected()
		{
			this.roomHostText.gameObject.SetActive(false);
			this.roomHostDescriptionText.SetActive(false);
		}

		// Token: 0x06005A58 RID: 23128 RVA: 0x001CEEB4 File Offset: 0x001CD0B4
		public static void ResetButtons()
		{
			List<GameModeType> list = new List<GameModeType>();
			list.Add(GameModeType.Casual);
			CustomMapModeSelector.gamemodes = list;
			CustomMapModeSelector.defaultGamemodeForLoadedMap = GameModeType.Casual;
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.SetupButtons();
			}
			GorillaComputer.instance.SetGameModeWithoutButton(CustomMapModeSelector.defaultGamemodeForLoadedMap.ToString());
		}

		// Token: 0x06005A59 RID: 23129 RVA: 0x001CEF38 File Offset: 0x001CD138
		public static void SetAvailableGameModes(int[] availableModes, int defaultMode)
		{
			CustomMapModeSelector.gamemodes.Clear();
			CustomMapModeSelector.gamemodes.Add(GameModeType.Casual);
			if (availableModes != null)
			{
				foreach (int num in availableModes)
				{
					CustomMapModeSelector.gamemodes.Add((GameModeType)num);
				}
			}
			CustomMapModeSelector.defaultGamemodeForLoadedMap = (GameModeType)defaultMode;
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.SetupButtons();
			}
			GorillaComputer.instance.SetGameModeWithoutButton(CustomMapModeSelector.defaultGamemodeForLoadedMap.ToString());
		}

		// Token: 0x06005A5A RID: 23130 RVA: 0x001CEFE0 File Offset: 0x001CD1E0
		public override void SetupButtons()
		{
			CustomMapModeSelector.<SetupButtons>d__16 <SetupButtons>d__;
			<SetupButtons>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SetupButtons>d__.<>4__this = this;
			<SetupButtons>d__.<>1__state = -1;
			<SetupButtons>d__.<>t__builder.Start<CustomMapModeSelector.<SetupButtons>d__16>(ref <SetupButtons>d__);
		}

		// Token: 0x06005A5B RID: 23131 RVA: 0x001CF018 File Offset: 0x001CD218
		public static void RefreshHostName()
		{
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
			}
		}

		// Token: 0x06005A5D RID: 23133 RVA: 0x001CF092 File Offset: 0x001CD292
		// Note: this type is marked as 'beforefieldinit'.
		static CustomMapModeSelector()
		{
			List<GameModeType> list = new List<GameModeType>();
			list.Add(GameModeType.Casual);
			CustomMapModeSelector.gamemodes = list;
			CustomMapModeSelector.defaultGamemodeForLoadedMap = GameModeType.Casual;
			CustomMapModeSelector.instances = new List<CustomMapModeSelector>();
			CustomMapModeSelector.reusableString = "";
		}

		// Token: 0x04006750 RID: 26448
		[SerializeField]
		private TMP_Text roomHostText;

		// Token: 0x04006751 RID: 26449
		[SerializeField]
		private GameObject roomHostDescriptionText;

		// Token: 0x04006752 RID: 26450
		[SerializeField]
		private string notInRoomHostString = "-NOT IN ROOM-";

		// Token: 0x04006753 RID: 26451
		[SerializeField]
		private string roomHostLabel = "ROOM HOST: ";

		// Token: 0x04006754 RID: 26452
		private static List<GameModeType> gamemodes;

		// Token: 0x04006755 RID: 26453
		private static GameModeType defaultGamemodeForLoadedMap;

		// Token: 0x04006756 RID: 26454
		private static List<CustomMapModeSelector> instances;

		// Token: 0x04006757 RID: 26455
		private static string reusableString;
	}
}
