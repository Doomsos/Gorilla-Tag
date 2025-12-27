using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	public class CustomMapModeSelector : GameModeSelectorButtonLayout
	{
		private void Awake()
		{
			CustomMapModeSelector.instances.AddIfNew(this);
		}

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

		public void OnDisable()
		{
			RoomSystem.JoinedRoomEvent -= new Action(this.OnJoinedRoom);
			NetworkSystem.Instance.OnMasterClientSwitchedEvent -= new Action<NetPlayer>(this.OnRoomHostSwitched);
			NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnDisconnected);
		}

		private void OnJoinedRoom()
		{
			this.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
		}

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

		private void OnDisconnected()
		{
			this.roomHostText.gameObject.SetActive(false);
			this.roomHostDescriptionText.SetActive(false);
		}

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

		public override void SetupButtons()
		{
			CustomMapModeSelector.<SetupButtons>d__16 <SetupButtons>d__;
			<SetupButtons>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SetupButtons>d__.<>4__this = this;
			<SetupButtons>d__.<>1__state = -1;
			<SetupButtons>d__.<>t__builder.Start<CustomMapModeSelector.<SetupButtons>d__16>(ref <SetupButtons>d__);
		}

		public static void RefreshHostName()
		{
			foreach (CustomMapModeSelector customMapModeSelector in CustomMapModeSelector.instances)
			{
				customMapModeSelector.OnRoomHostSwitched(NetworkSystem.Instance.MasterClient);
			}
		}

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

		[SerializeField]
		private TMP_Text roomHostText;

		[SerializeField]
		private GameObject roomHostDescriptionText;

		[SerializeField]
		private string notInRoomHostString = "-NOT IN ROOM-";

		[SerializeField]
		private string roomHostLabel = "ROOM HOST: ";

		private static List<GameModeType> gamemodes;

		private static GameModeType defaultGamemodeForLoadedMap;

		private static List<CustomMapModeSelector> instances;

		private static string reusableString;
	}
}
