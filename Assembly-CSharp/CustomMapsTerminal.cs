using System;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009A9 RID: 2473
public class CustomMapsTerminal : MonoBehaviour
{
	// Token: 0x170005C7 RID: 1479
	// (get) Token: 0x06003F18 RID: 16152 RVA: 0x00152536 File Offset: 0x00150736
	public static int LocalPlayerID
	{
		get
		{
			return NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x170005C8 RID: 1480
	// (get) Token: 0x06003F19 RID: 16153 RVA: 0x00152547 File Offset: 0x00150747
	public static long LocalModDetailsID
	{
		get
		{
			return CustomMapsTerminal.localModDetailsID;
		}
	}

	// Token: 0x170005C9 RID: 1481
	// (get) Token: 0x06003F1A RID: 16154 RVA: 0x0015254E File Offset: 0x0015074E
	public static int CurrentScreen
	{
		get
		{
			return (int)CustomMapsTerminal.localCurrentScreen;
		}
	}

	// Token: 0x170005CA RID: 1482
	// (get) Token: 0x06003F1B RID: 16155 RVA: 0x00152555 File Offset: 0x00150755
	public static bool IsDriver
	{
		get
		{
			return CustomMapsTerminal.localDriverID == CustomMapsTerminal.LocalPlayerID;
		}
	}

	// Token: 0x06003F1C RID: 16156 RVA: 0x00152563 File Offset: 0x00150763
	private void Awake()
	{
		CustomMapsTerminal.instance = this;
		CustomMapsTerminal.hasInstance = true;
	}

	// Token: 0x06003F1D RID: 16157 RVA: 0x00152574 File Offset: 0x00150774
	private void Start()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		this.controlAccessScreen.Show();
		this.detailsAccessScreen.Show();
		this.modListScreen.Hide();
		this.modDetailsScreen.Hide();
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.OnJoinedRoom);
		NetworkSystem.Instance.OnReturnedToSinglePlayer += new Action(this.OnReturnedToSinglePlayer);
	}

	// Token: 0x06003F1E RID: 16158 RVA: 0x00152630 File Offset: 0x00150830
	private void OnDestroy()
	{
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted -= new Action(this.OnJoinedRoom);
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= new Action(this.OnReturnedToSinglePlayer);
	}

	// Token: 0x06003F1F RID: 16159 RVA: 0x001526AC File Offset: 0x001508AC
	public static void ShowDetailsScreen(Mod mod)
	{
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.localCurrentScreen;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		CustomMapsTerminal.localModDetailsID = mod.Id;
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.controlAccessScreen.Hide();
		CustomMapsTerminal.instance.detailsAccessScreen.Hide();
		CustomMapsTerminal.instance.modDetailsScreen.Show();
		CustomMapsTerminal.instance.modDetailsScreen.SetModProfile(mod);
		CustomMapsTerminal.instance.modDisplayScreen.Show();
		CustomMapsTerminal.instance.modDisplayScreen.SetModProfile(mod);
		CustomMapsTerminal.instance.modSearchScreen.Hide();
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x06003F20 RID: 16160 RVA: 0x00152758 File Offset: 0x00150958
	public static void ReturnFromDetailsScreen()
	{
		CustomMapsTerminal.ScreenType screenType = CustomMapsTerminal.previousScreen;
		if (screenType == CustomMapsTerminal.ScreenType.ModDetails || screenType == CustomMapsTerminal.ScreenType.Invalid || screenType == CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		else
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.previousScreen;
		}
		switch (CustomMapsTerminal.localCurrentScreen)
		{
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Show();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.AvailableMods:
		case CustomMapsTerminal.ScreenType.InstalledMods:
		case CustomMapsTerminal.ScreenType.FavoriteMods:
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Hide();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.SearchMods:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modSearchScreen.ReturnFromDetailsScreen();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Hide();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		}
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x06003F21 RID: 16161 RVA: 0x001528D4 File Offset: 0x00150AD4
	public static void ShowSearchScreen()
	{
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.localCurrentScreen;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.SearchMods;
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.controlAccessScreen.Hide();
		CustomMapsTerminal.instance.detailsAccessScreen.SetDetailsScreenForDriver();
		CustomMapsTerminal.instance.detailsAccessScreen.Show();
		CustomMapsTerminal.instance.modDetailsScreen.Hide();
		CustomMapsTerminal.instance.modDisplayScreen.Hide();
		CustomMapsTerminal.instance.modSearchScreen.Show();
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x06003F22 RID: 16162 RVA: 0x00152960 File Offset: 0x00150B60
	public static void ReturnFromSearchScreen()
	{
		CustomMapsTerminal.ScreenType screenType = CustomMapsTerminal.previousScreen;
		if (screenType == CustomMapsTerminal.ScreenType.ModDetails || screenType == CustomMapsTerminal.ScreenType.Invalid || screenType == CustomMapsTerminal.ScreenType.TerminalControlPrompt || screenType == CustomMapsTerminal.ScreenType.SearchMods)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		else
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.previousScreen;
		}
		switch (CustomMapsTerminal.localCurrentScreen)
		{
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Show();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.AvailableMods:
		case CustomMapsTerminal.ScreenType.InstalledMods:
		case CustomMapsTerminal.ScreenType.FavoriteMods:
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Hide();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		}
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x06003F23 RID: 16163 RVA: 0x00152A7E File Offset: 0x00150C7E
	public static void SendTerminalStatus()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.instance.mapTerminalNetworkObject.SendTerminalStatus();
	}

	// Token: 0x06003F24 RID: 16164 RVA: 0x00152A97 File Offset: 0x00150C97
	public static void ResetTerminalControl()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
		CustomMapsTerminal.ShowTerminalControlScreen();
	}

	// Token: 0x06003F25 RID: 16165 RVA: 0x00152AB4 File Offset: 0x00150CB4
	public static void HandleTerminalControlStatusChangeRequest(bool lockedStatus, int playerID)
	{
		if (lockedStatus && playerID == -2)
		{
			return;
		}
		if (CustomMapsTerminal.localDriverID == -2)
		{
			if (!lockedStatus)
			{
				return;
			}
		}
		else if (CustomMapsTerminal.localDriverID != playerID)
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(lockedStatus, playerID, true);
	}

	// Token: 0x06003F26 RID: 16166 RVA: 0x00152AE0 File Offset: 0x00150CE0
	public static void SetTerminalControlStatus(bool isLocked, int driverID = -2, bool sendRPC = false)
	{
		GTDev.Log<string>(string.Format("[CustomMapsTerminal::SetTerminalControlStatus] isLocked: {0} | driverID: {1} | playerId {2} | sendRPC: {3}", new object[]
		{
			isLocked,
			driverID,
			CustomMapsTerminal.LocalPlayerID,
			sendRPC
		}), null);
		if (isLocked)
		{
			CustomMapsTerminal.localDriverID = driverID;
			CustomMapsTerminal.instance.terminalControlButton.LockTerminalControl();
			if (CustomMapsTerminal.IsDriver)
			{
				CustomMapsTerminal.HideTerminalControlScreens();
			}
			else
			{
				CustomMapsTerminal.ShowTerminalControlScreen();
			}
		}
		else
		{
			CustomMapsTerminal.localDriverID = -2;
			CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
			CustomMapsTerminal.ShowTerminalControlScreen();
		}
		if (sendRPC && NetworkSystem.Instance.IsMasterClient)
		{
			CustomMapsTerminal.instance.mapTerminalNetworkObject.SetTerminalControlStatus(isLocked, CustomMapsTerminal.localDriverID);
		}
	}

	// Token: 0x06003F27 RID: 16167 RVA: 0x00152B98 File Offset: 0x00150D98
	public static void UpdateFromDriver(int currentScreen, long modDetailsID, int driverID)
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.localDriverID = driverID;
		CustomMapsTerminal.cachedModDetailsID = modDetailsID;
		CustomMapsTerminal.localModDetailsID = modDetailsID;
		CustomMapsTerminal.cachedCurrentScreen = (CustomMapsTerminal.ScreenType)currentScreen;
		CustomMapsTerminal.localCurrentScreen = (CustomMapsTerminal.ScreenType)currentScreen;
		Debug.Log(string.Format("[CustomMapsTerminal::UpdateFromDriver] currentScreen {0} modDetailsID {1}", CustomMapsTerminal.localCurrentScreen, CustomMapsTerminal.localModDetailsID));
		if (CustomMapsTerminal.localDriverID != -2)
		{
			CustomMapsTerminal.RefreshDriverNickName();
		}
		CustomMapsTerminal.ScreenType screenType = CustomMapsTerminal.localCurrentScreen;
		if (screenType <= CustomMapsTerminal.ScreenType.SearchMods)
		{
			CustomMapsTerminal.ShowTerminalControlScreen();
			return;
		}
		if (screenType != CustomMapsTerminal.ScreenType.ModDetails)
		{
			return;
		}
		CustomMapsTerminal.ShowTerminalControlScreen();
		if (CustomMapsTerminal.localModDetailsID <= 0L)
		{
			return;
		}
		CustomMapsTerminal.instance.detailsAccessScreen.Hide();
		CustomMapsTerminal.instance.modDisplayScreen.Show();
		CustomMapsTerminal.instance.modDisplayScreen.RetrieveModFromModIO(CustomMapsTerminal.localModDetailsID, false, null);
	}

	// Token: 0x06003F28 RID: 16168 RVA: 0x00152C54 File Offset: 0x00150E54
	private void UpdateControlScreenForDriver()
	{
		GTDev.Log<string>(string.Format("[CustomMapsTerminal::UpdateScreenToMatchStatus] driverID: {0} ", CustomMapsTerminal.localDriverID) + string.Format("| currentScreen: {0} ", CustomMapsTerminal.localCurrentScreen) + string.Format("| previousScreen: {0} ", CustomMapsTerminal.previousScreen), null);
		switch (CustomMapsTerminal.localCurrentScreen)
		{
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			return;
		case CustomMapsTerminal.ScreenType.AvailableMods:
		case CustomMapsTerminal.ScreenType.InstalledMods:
		case CustomMapsTerminal.ScreenType.FavoriteMods:
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			this.controlAccessScreen.Hide();
			this.modSearchScreen.Hide();
			this.detailsAccessScreen.SetDetailsScreenForDriver();
			this.detailsAccessScreen.Show();
			this.modListScreen.Show();
			this.modDetailsScreen.Hide();
			this.modDisplayScreen.Hide();
			return;
		case CustomMapsTerminal.ScreenType.SearchMods:
			this.controlAccessScreen.Hide();
			this.modSearchScreen.Show();
			this.detailsAccessScreen.SetDetailsScreenForDriver();
			this.detailsAccessScreen.Show();
			this.modListScreen.Hide();
			this.modDetailsScreen.Hide();
			this.modDisplayScreen.Hide();
			return;
		case CustomMapsTerminal.ScreenType.ModDetails:
			this.controlAccessScreen.Hide();
			this.modSearchScreen.Hide();
			this.detailsAccessScreen.Hide();
			this.modListScreen.Hide();
			this.modDetailsScreen.Show();
			this.modDetailsScreen.RetrieveModFromModIO(CustomMapsTerminal.localModDetailsID, false, null);
			this.modDisplayScreen.Show();
			this.modDisplayScreen.RetrieveModFromModIO(CustomMapsTerminal.localModDetailsID, false, null);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003F29 RID: 16169 RVA: 0x00152DD4 File Offset: 0x00150FD4
	private void ValidateLocalStatus()
	{
		if (CustomMapsTerminal.localDriverID == -2)
		{
			return;
		}
		if (CustomMapLoader.IsMapLoaded())
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localModDetailsID = CustomMapLoader.LoadedMapModId;
			CustomMapsTerminal.SendTerminalStatus();
			return;
		}
		if (CustomMapManager.IsLoading())
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localModDetailsID = CustomMapManager.LoadingMapId;
			CustomMapsTerminal.SendTerminalStatus();
			return;
		}
		if (CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localModDetailsID = CustomMapManager.GetRoomMapId()._id;
			CustomMapsTerminal.SendTerminalStatus();
		}
	}

	// Token: 0x06003F2A RID: 16170 RVA: 0x00002789 File Offset: 0x00000989
	private void OnModIOLoggedIn()
	{
	}

	// Token: 0x06003F2B RID: 16171 RVA: 0x00152E55 File Offset: 0x00151055
	private void OnModIOLoggedOut()
	{
		if (CustomMapsTerminal.localCurrentScreen == CustomMapsTerminal.ScreenType.SubscribedMods)
		{
			if (this.modListScreen.isActiveAndEnabled)
			{
				this.modListScreen.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, false);
			}
			else
			{
				CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			}
		}
		if (CustomMapsTerminal.previousScreen == CustomMapsTerminal.ScreenType.SubscribedMods)
		{
			CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
	}

	// Token: 0x06003F2C RID: 16172 RVA: 0x00152E90 File Offset: 0x00151090
	public void HandleTerminalControlButtonPressed()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			CustomMapsTerminal.SetTerminalControlStatus(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID, false);
			return;
		}
		if (CustomMapsTerminal.localDriverID != -2 && !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (this.mapTerminalNetworkObject.HasAuthority)
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID);
			return;
		}
		this.mapTerminalNetworkObject.RequestTerminalControlStatusChange(!this.terminalControlButton.IsLocked);
	}

	// Token: 0x06003F2D RID: 16173 RVA: 0x00152F14 File Offset: 0x00151114
	private static void ShowTerminalControlScreen()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (CustomMapsTerminal.localDriverID == -2)
		{
			CustomMapsTerminal.instance.controlAccessScreen.Reset();
			CustomMapsTerminal.instance.detailsAccessScreen.Reset();
		}
		else
		{
			CustomMapsTerminal.instance.controlAccessScreen.SetDriverName();
			CustomMapsTerminal.instance.detailsAccessScreen.SetDriverName();
		}
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.modDetailsScreen.Hide();
		CustomMapsTerminal.instance.modDisplayScreen.Hide();
		CustomMapsTerminal.instance.controlAccessScreen.Show();
		CustomMapsTerminal.instance.detailsAccessScreen.Show();
		CustomMapsTerminal.instance.modSearchScreen.Hide();
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.localCurrentScreen;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
	}

	// Token: 0x06003F2E RID: 16174 RVA: 0x00152FDC File Offset: 0x001511DC
	private static void HideTerminalControlScreens()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (CustomMapsTerminal.localCurrentScreen != CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			return;
		}
		if (CustomMapsTerminal.previousScreen > CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.previousScreen;
			if ((CustomMapsTerminal.localCurrentScreen == CustomMapsTerminal.ScreenType.SubscribedMods || CustomMapsTerminal.localCurrentScreen == CustomMapsTerminal.ScreenType.FavoriteMods) && !ModIOManager.IsLoggedIn())
			{
				CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			}
		}
		else if (CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		}
		else
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		CustomMapsTerminal.instance.UpdateControlScreenForDriver();
	}

	// Token: 0x06003F2F RID: 16175 RVA: 0x00153061 File Offset: 0x00151261
	public static void RequestDriverNickNameRefresh()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
		CustomMapsTerminal.instance.mapTerminalNetworkObject.RefreshDriverNickName();
	}

	// Token: 0x06003F30 RID: 16176 RVA: 0x00153088 File Offset: 0x00151288
	public static void RefreshDriverNickName()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		CustomMapsTerminal.instance.terminalControllerLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.localDriverID);
			CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.DefaultName;
			if (GorillaComputer.instance.NametagsEnabled && flag)
			{
				RigContainer rigContainer;
				if (netPlayerByID.IsLocal)
				{
					CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.NickName;
				}
				else if (VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					CustomMapsTerminal.instance.terminalControllerText.text = rigContainer.Rig.playerNameVisible;
				}
			}
		}
		else
		{
			CustomMapsTerminal.instance.terminalControllerText.text = ((GorillaComputer.instance.NametagsEnabled && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		CustomMapsTerminal.instance.terminalControllerText.gameObject.SetActive(true);
		CustomMapsTerminal.instance.modListScreen.RefreshDriverNickname(CustomMapsTerminal.instance.terminalControllerText.text);
	}

	// Token: 0x06003F31 RID: 16177 RVA: 0x001531BC File Offset: 0x001513BC
	private void OnReturnedToSinglePlayer()
	{
		if (CustomMapsTerminal.localDriverID != CustomMapsTerminal.cachedLocalPlayerID)
		{
			CustomMapsTerminal.ResetTerminalControl();
		}
		else
		{
			CustomMapsTerminal.localDriverID = CustomMapsTerminal.LocalPlayerID;
		}
		CustomMapsTerminal.cachedLocalPlayerID = -1;
	}

	// Token: 0x06003F32 RID: 16178 RVA: 0x001531E1 File Offset: 0x001513E1
	private void OnJoinedRoom()
	{
		CustomMapsTerminal.cachedLocalPlayerID = CustomMapsTerminal.LocalPlayerID;
		CustomMapsTerminal.ResetTerminalControl();
	}

	// Token: 0x06003F33 RID: 16179 RVA: 0x001531F2 File Offset: 0x001513F2
	public static bool IsLocked()
	{
		return CustomMapsTerminal.localDriverID != -2;
	}

	// Token: 0x06003F34 RID: 16180 RVA: 0x00153200 File Offset: 0x00151400
	public static int GetDriverID()
	{
		return CustomMapsTerminal.localDriverID;
	}

	// Token: 0x06003F35 RID: 16181 RVA: 0x00153207 File Offset: 0x00151407
	public static string GetDriverNickname()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return "";
		}
		return CustomMapsTerminal.instance.terminalControllerText.text;
	}

	// Token: 0x04005050 RID: 20560
	[SerializeField]
	private CustomMapsAccessScreen controlAccessScreen;

	// Token: 0x04005051 RID: 20561
	[SerializeField]
	private CustomMapsAccessScreen detailsAccessScreen;

	// Token: 0x04005052 RID: 20562
	[SerializeField]
	private CustomMapsListScreen modListScreen;

	// Token: 0x04005053 RID: 20563
	[SerializeField]
	private CustomMapsDetailsScreen modDetailsScreen;

	// Token: 0x04005054 RID: 20564
	[SerializeField]
	private CustomMapsDisplayScreen modDisplayScreen;

	// Token: 0x04005055 RID: 20565
	[SerializeField]
	private CustomMapsSearchScreen modSearchScreen;

	// Token: 0x04005056 RID: 20566
	[SerializeField]
	private VirtualStumpSerializer mapTerminalNetworkObject;

	// Token: 0x04005057 RID: 20567
	[SerializeField]
	private CustomMapsTerminalControlButton terminalControlButton;

	// Token: 0x04005058 RID: 20568
	[SerializeField]
	private TMP_Text terminalControllerLabelText;

	// Token: 0x04005059 RID: 20569
	[SerializeField]
	private TMP_Text terminalControllerText;

	// Token: 0x0400505A RID: 20570
	public const int NO_DRIVER_ID = -2;

	// Token: 0x0400505B RID: 20571
	private static CustomMapsTerminal instance;

	// Token: 0x0400505C RID: 20572
	private static bool hasInstance;

	// Token: 0x0400505D RID: 20573
	private static long localModDetailsID = -1L;

	// Token: 0x0400505E RID: 20574
	private static long cachedModDetailsID = -1L;

	// Token: 0x0400505F RID: 20575
	private static int localDriverID = -1;

	// Token: 0x04005060 RID: 20576
	private static int cachedLocalPlayerID = -1;

	// Token: 0x04005061 RID: 20577
	private static CustomMapsTerminal.ScreenType localCurrentScreen = CustomMapsTerminal.ScreenType.Invalid;

	// Token: 0x04005062 RID: 20578
	private static CustomMapsTerminal.ScreenType cachedCurrentScreen = CustomMapsTerminal.ScreenType.Invalid;

	// Token: 0x04005063 RID: 20579
	private static CustomMapsTerminal.ScreenType previousScreen = CustomMapsTerminal.ScreenType.Invalid;

	// Token: 0x020009AA RID: 2474
	public enum ScreenType
	{
		// Token: 0x04005065 RID: 20581
		Invalid = -1,
		// Token: 0x04005066 RID: 20582
		TerminalControlPrompt,
		// Token: 0x04005067 RID: 20583
		AvailableMods,
		// Token: 0x04005068 RID: 20584
		InstalledMods,
		// Token: 0x04005069 RID: 20585
		FavoriteMods,
		// Token: 0x0400506A RID: 20586
		SubscribedMods,
		// Token: 0x0400506B RID: 20587
		SearchMods,
		// Token: 0x0400506C RID: 20588
		ModDetails
	}
}
