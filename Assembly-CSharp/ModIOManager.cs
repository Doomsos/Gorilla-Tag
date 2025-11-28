using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using Modio;
using Modio.API;
using Modio.Authentication;
using Modio.Customizations;
using Modio.FileIO;
using Modio.Mods;
using Modio.Users;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

// Token: 0x02000967 RID: 2407
public class ModIOManager : MonoBehaviour, ISteamCredentialProvider, IOculusCredentialProvider
{
	// Token: 0x06003DA4 RID: 15780 RVA: 0x0014681C File Offset: 0x00144A1C
	private void Awake()
	{
		if (ModIOManager.instance == null)
		{
			ModIOManager.instance = this;
			ModIOManager.hasInstance = true;
			UGCPermissionManager.SubscribeToUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.SubscribeToUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
			ModioServices.Bind<IModioAuthService>().FromInstance(ModIOManager.accountLinkingAuthService, 41, null);
			ModioServices.Bind<IModioAuthService>().FromInstance(ModIOManager.steamAuthService, 40, null);
			long gameId = ModioServices.Resolve<ModioSettings>().GameId;
			ModIOManager.ModIODirectory = Path.Combine(ModioServices.Resolve<IModioRootPathProvider>().Path, "mod.io", gameId.ToString()) + Path.DirectorySeparatorChar.ToString();
			return;
		}
		if (ModIOManager.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06003DA5 RID: 15781 RVA: 0x001468E6 File Offset: 0x00144AE6
	private void Start()
	{
		NetworkSystem.Instance.OnMultiplayerStarted += new Action(this.OnJoinedRoom);
	}

	// Token: 0x06003DA6 RID: 15782 RVA: 0x0014690C File Offset: 0x00144B0C
	private void OnDestroy()
	{
		if (ModIOManager.instance == this)
		{
			ModIOManager.instance = null;
			ModIOManager.hasInstance = false;
			UGCPermissionManager.UnsubscribeFromUGCEnabled(new Action(ModIOManager.OnUGCEnabled));
			UGCPermissionManager.UnsubscribeFromUGCDisabled(new Action(ModIOManager.OnUGCDisabled));
		}
		NetworkSystem.Instance.OnMultiplayerStarted -= new Action(this.OnJoinedRoom);
	}

	// Token: 0x06003DA7 RID: 15783 RVA: 0x00146979 File Offset: 0x00144B79
	private void Update()
	{
		bool flag = ModIOManager.hasInstance;
	}

	// Token: 0x06003DA8 RID: 15784 RVA: 0x00002789 File Offset: 0x00000989
	private static void OnUGCEnabled()
	{
	}

	// Token: 0x06003DA9 RID: 15785 RVA: 0x00002789 File Offset: 0x00000989
	private static void OnUGCDisabled()
	{
	}

	// Token: 0x06003DAA RID: 15786 RVA: 0x00146981 File Offset: 0x00144B81
	public static bool IsInitialized()
	{
		return ModIOManager.initialized;
	}

	// Token: 0x06003DAB RID: 15787 RVA: 0x00146988 File Offset: 0x00144B88
	public static Task<Error> Initialize()
	{
		ModIOManager.<Initialize>d__47 <Initialize>d__;
		<Initialize>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<Initialize>d__.<>1__state = -1;
		<Initialize>d__.<>t__builder.Start<ModIOManager.<Initialize>d__47>(ref <Initialize>d__);
		return <Initialize>d__.<>t__builder.Task;
	}

	// Token: 0x06003DAC RID: 15788 RVA: 0x001469C4 File Offset: 0x00144BC4
	private static Task<Error> InitInternal()
	{
		ModIOManager.<InitInternal>d__48 <InitInternal>d__;
		<InitInternal>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<InitInternal>d__.<>1__state = -1;
		<InitInternal>d__.<>t__builder.Start<ModIOManager.<InitInternal>d__48>(ref <InitInternal>d__);
		return <InitInternal>d__.<>t__builder.Task;
	}

	// Token: 0x06003DAD RID: 15789 RVA: 0x00146A00 File Offset: 0x00144C00
	private Task<ValueTuple<Error, bool, bool>> HasAcceptedLatestTerms()
	{
		ModIOManager.<HasAcceptedLatestTerms>d__49 <HasAcceptedLatestTerms>d__;
		<HasAcceptedLatestTerms>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, bool, bool>>.Create();
		<HasAcceptedLatestTerms>d__.<>1__state = -1;
		<HasAcceptedLatestTerms>d__.<>t__builder.Start<ModIOManager.<HasAcceptedLatestTerms>d__49>(ref <HasAcceptedLatestTerms>d__);
		return <HasAcceptedLatestTerms>d__.<>t__builder.Task;
	}

	// Token: 0x06003DAE RID: 15790 RVA: 0x00146A3C File Offset: 0x00144C3C
	private Task<Error> ShowModIOTermsOfUse()
	{
		ModIOManager.<ShowModIOTermsOfUse>d__50 <ShowModIOTermsOfUse>d__;
		<ShowModIOTermsOfUse>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<ShowModIOTermsOfUse>d__.<>4__this = this;
		<ShowModIOTermsOfUse>d__.<>1__state = -1;
		<ShowModIOTermsOfUse>d__.<>t__builder.Start<ModIOManager.<ShowModIOTermsOfUse>d__50>(ref <ShowModIOTermsOfUse>d__);
		return <ShowModIOTermsOfUse>d__.<>t__builder.Task;
	}

	// Token: 0x06003DAF RID: 15791 RVA: 0x00146A80 File Offset: 0x00144C80
	private void OnModIOTermsOfUseAcknowledged(bool accepted)
	{
		if (accepted)
		{
			CustomMapManager.RequestEnableTeleportHUD(true);
			Action<ModIORequestResultAnd<bool>> action = ModIOManager.modIOTermsAcknowledgedCallback;
			if (action != null)
			{
				action.Invoke(ModIORequestResultAnd<bool>.CreateSuccessResult(true));
			}
		}
		else
		{
			Action<ModIORequestResultAnd<bool>> action2 = ModIOManager.modIOTermsAcknowledgedCallback;
			if (action2 != null)
			{
				action2.Invoke(ModIORequestResultAnd<bool>.CreateFailureResult("MOD.IO TERMS OF USE HAVE NOT BEEN ACCEPTED. YOU MUST ACCEPT THE MOD.IO TERMS OF USE TO LOGIN WITH YOUR PLATFORM CREDENTIALS OR YOU CAN LOGIN WITH AN EXISTING MOD.IO ACCOUNT BY PRESSING THE 'LINK MOD.IO ACCOUNT' BUTTON AND FOLLOWING THE INSTRUCTIONS."));
			}
		}
		ModIOManager.modIOTermsAcknowledgedCallback = null;
	}

	// Token: 0x06003DB0 RID: 15792 RVA: 0x00146ACE File Offset: 0x00144CCE
	private static void EnableModManagement()
	{
		if (!ModIOManager.modManagementEnabled)
		{
			ModInstallationManagement.ManagementEvents += new ModInstallationManagement.InstallationManagementEventDelegate(ModIOManager.HandleModManagementEvent);
			ModInstallationManagement.Activate();
			ModIOManager.modManagementEnabled = true;
			ModioLog verbose = ModioLog.Verbose;
			if (verbose == null)
			{
				return;
			}
			verbose.Log("[ModIOManager::EnableModManagement] Mod Management enabled.");
		}
	}

	// Token: 0x06003DB1 RID: 15793 RVA: 0x00146B07 File Offset: 0x00144D07
	private static void DisableModManagement()
	{
		if (ModIOManager.modManagementEnabled)
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::EnableModManagement] Mod Management disabled!");
			}
			ModInstallationManagement.ManagementEvents -= new ModInstallationManagement.InstallationManagementEventDelegate(ModIOManager.HandleModManagementEvent);
			ModInstallationManagement.Deactivate(false);
			ModIOManager.modManagementEnabled = false;
		}
	}

	// Token: 0x06003DB2 RID: 15794 RVA: 0x00146B44 File Offset: 0x00144D44
	private static void HandleModManagementEvent(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
	{
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::HandleModManagementEvent] Mod " + mod.Id.ToString() + " | FileState: " + string.Format("{0} | JobType: {1} | JobPhase: {2}", modfile.State.ToString(), jobType, jobPhase));
		}
		try
		{
			if ((jobType == 1 || jobType == null) && jobPhase == 2 && modfile.State == 5)
			{
				ModIOManager.outdatedModCMSVersions.Remove(mod.Id);
				ModIOManager.IsModOutdated(mod);
			}
			if (jobPhase == 1 && (jobType == null || jobType == 2 || jobType == 3))
			{
				ModIOManager.outdatedModCMSVersions.Remove(mod.Id);
			}
		}
		catch (Exception ex)
		{
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log(string.Format("[ModIOManager::HandleModManagementEvent] Exception: {0}", ex));
			}
		}
		UnityEvent<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase> onModManagementEvent = ModIOManager.OnModManagementEvent;
		if (onModManagementEvent == null)
		{
			return;
		}
		onModManagementEvent.Invoke(mod, modfile, jobType, jobPhase);
	}

	// Token: 0x06003DB3 RID: 15795 RVA: 0x00146C40 File Offset: 0x00144E40
	public static Task RefreshModCache()
	{
		ModIOManager.<RefreshModCache>d__55 <RefreshModCache>d__;
		<RefreshModCache>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RefreshModCache>d__.<>1__state = -1;
		<RefreshModCache>d__.<>t__builder.Start<ModIOManager.<RefreshModCache>d__55>(ref <RefreshModCache>d__);
		return <RefreshModCache>d__.<>t__builder.Task;
	}

	// Token: 0x06003DB4 RID: 15796 RVA: 0x00146C7B File Offset: 0x00144E7B
	public static bool IsRefreshing()
	{
		return ModIOManager.refreshingModCache;
	}

	// Token: 0x06003DB5 RID: 15797 RVA: 0x00146C84 File Offset: 0x00144E84
	public static Task<ValueTuple<bool, int>> IsModOutdated(ModId modId)
	{
		ModIOManager.<IsModOutdated>d__57 <IsModOutdated>d__;
		<IsModOutdated>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, int>>.Create();
		<IsModOutdated>d__.modId = modId;
		<IsModOutdated>d__.<>1__state = -1;
		<IsModOutdated>d__.<>t__builder.Start<ModIOManager.<IsModOutdated>d__57>(ref <IsModOutdated>d__);
		return <IsModOutdated>d__.<>t__builder.Task;
	}

	// Token: 0x06003DB6 RID: 15798 RVA: 0x00146CC8 File Offset: 0x00144EC8
	public static ValueTuple<bool, int> IsModOutdated(Mod mod)
	{
		int num;
		if (ModIOManager.outdatedModCMSVersions.TryGetValue(mod.Id, ref num))
		{
			return new ValueTuple<bool, int>(true, num);
		}
		if (mod.File != null)
		{
			if (mod.File.State == 5)
			{
				ValueTuple<bool, int> valueTuple = ModIOManager.IsInstalledModOutdated(mod);
				bool item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				return new ValueTuple<bool, int>(item, item2);
			}
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("[ModIOManager::IsModOutdated] Mod File for " + mod.Name + " is not installed. " + string.Format("State: {0}.", mod.File.State));
			}
		}
		else
		{
			ModioLog error2 = ModioLog.Error;
			if (error2 != null)
			{
				error2.Log("[ModIOManager::IsModOutdated] Mod File for " + mod.Name + " is null.");
			}
		}
		return new ValueTuple<bool, int>(false, -1);
	}

	// Token: 0x06003DB7 RID: 15799 RVA: 0x00146D90 File Offset: 0x00144F90
	public static void SaveFavoriteMods()
	{
		if (!ModIOManager.initialized || !ModIOManager.modManagementEnabled)
		{
			return;
		}
		try
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(ModIOManager.ModIODirectory);
			if (!directoryInfo.Exists)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log("[ModIOManager::SaveFavoriteMods] ModIO Directory for GorillaTag does not exist!");
				}
			}
			else
			{
				long[] array = new long[ModIOManager.favoriteMods.Count];
				int num = 0;
				foreach (KeyValuePair<ModId, Mod> keyValuePair in ModIOManager.favoriteMods)
				{
					array[num++] = keyValuePair.Key;
				}
				string text = JsonConvert.SerializeObject(array);
				File.WriteAllText(Path.Join(directoryInfo.FullName, "favoriteMods.json"), text);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06003DB8 RID: 15800 RVA: 0x00146E78 File Offset: 0x00145078
	[return: TupleElementNames(new string[]
	{
		"error",
		"favoriteMods"
	})]
	public static Task<ValueTuple<Error, List<Mod>>> GetFavoriteMods(bool forceRefresh = false)
	{
		ModIOManager.<GetFavoriteMods>d__60 <GetFavoriteMods>d__;
		<GetFavoriteMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, List<Mod>>>.Create();
		<GetFavoriteMods>d__.forceRefresh = forceRefresh;
		<GetFavoriteMods>d__.<>1__state = -1;
		<GetFavoriteMods>d__.<>t__builder.Start<ModIOManager.<GetFavoriteMods>d__60>(ref <GetFavoriteMods>d__);
		return <GetFavoriteMods>d__.<>t__builder.Task;
	}

	// Token: 0x06003DB9 RID: 15801 RVA: 0x00146EBC File Offset: 0x001450BC
	public static Task<Error> AddFavorite(ModId modId, Action<Error> callback = null)
	{
		ModIOManager.<AddFavorite>d__61 <AddFavorite>d__;
		<AddFavorite>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<AddFavorite>d__.modId = modId;
		<AddFavorite>d__.callback = callback;
		<AddFavorite>d__.<>1__state = -1;
		<AddFavorite>d__.<>t__builder.Start<ModIOManager.<AddFavorite>d__61>(ref <AddFavorite>d__);
		return <AddFavorite>d__.<>t__builder.Task;
	}

	// Token: 0x06003DBA RID: 15802 RVA: 0x00146F07 File Offset: 0x00145107
	public static Error RemoveFavorite(ModId modId)
	{
		if (!ModIOManager.favoriteMods.ContainsKey(modId))
		{
			return new Error(-2147483648L, "MOD NOT FAVORITED");
		}
		ModIOManager.favoriteMods.Remove(modId);
		ModIOManager.SaveFavoriteMods();
		return Error.None;
	}

	// Token: 0x06003DBB RID: 15803 RVA: 0x00146F3D File Offset: 0x0014513D
	public static bool IsModFavorited(ModId modId)
	{
		return ModIOManager.favoriteMods.ContainsKey(modId);
	}

	// Token: 0x06003DBC RID: 15804 RVA: 0x00146F4C File Offset: 0x0014514C
	[return: TupleElementNames(new string[]
	{
		"error",
		"installedMods"
	})]
	public static Task<ValueTuple<Error, Mod[]>> GetInstalledMods(bool forceRefresh = false)
	{
		ModIOManager.<GetInstalledMods>d__64 <GetInstalledMods>d__;
		<GetInstalledMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod[]>>.Create();
		<GetInstalledMods>d__.forceRefresh = forceRefresh;
		<GetInstalledMods>d__.<>1__state = -1;
		<GetInstalledMods>d__.<>t__builder.Start<ModIOManager.<GetInstalledMods>d__64>(ref <GetInstalledMods>d__);
		return <GetInstalledMods>d__.<>t__builder.Task;
	}

	// Token: 0x06003DBD RID: 15805 RVA: 0x00146F8F File Offset: 0x0014518F
	public static bool ValidateInstalledMod(Mod mod)
	{
		return ModIOManager.initialized && ModInstallationManagement.ValidateInstalledMod(mod);
	}

	// Token: 0x06003DBE RID: 15806 RVA: 0x00146FA0 File Offset: 0x001451A0
	private static ValueTuple<bool, int> IsInstalledModOutdated(Mod mod)
	{
		int num = -1;
		if (!ModIOManager.hasInstance)
		{
			return new ValueTuple<bool, int>(false, num);
		}
		if (mod.File == null || mod.File.State != 5)
		{
			ModioLog message = ModioLog.Message;
			if (message != null)
			{
				message.Log("[ModIOManager::IsInstalledModOutdated] Mod " + mod.Id.ToString() + " is not currently installed.");
			}
			return new ValueTuple<bool, int>(false, num);
		}
		try
		{
			FileInfo[] files = new DirectoryInfo(mod.File.InstallLocation).GetFiles("package.json");
			if (files.Length == 0)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log(string.Concat(new string[]
					{
						"[ModIOManager::IsInstalledModOutdated] Directory (",
						mod.File.InstallLocation,
						") for mod ",
						mod.Name,
						" does not contain a package.json file!"
					}));
				}
			}
			if (files.Length > 1)
			{
				ModioLog warning = ModioLog.Warning;
				if (warning != null)
				{
					warning.Log(string.Concat(new string[]
					{
						"[ModIOManager::IsInstalledModOutdated] Directory (",
						mod.File.InstallLocation,
						") for mod ",
						mod.Name,
						" contains more than one package.json file! Only the first one found will be used!"
					}));
				}
			}
			MapPackageInfo packageInfo = CustomMapLoader.GetPackageInfo(files[0].FullName);
			if (packageInfo.customMapSupportVersion != Constants.customMapSupportVersion)
			{
				ModIOManager.outdatedModCMSVersions.Add(mod.Id, packageInfo.customMapSupportVersion);
				return new ValueTuple<bool, int>(true, packageInfo.customMapSupportVersion);
			}
		}
		catch (Exception ex)
		{
			ModioLog error2 = ModioLog.Error;
			if (error2 != null)
			{
				error2.Log(string.Format("[ModIOManager::IsInstalledModOutdated] Exception while reading package.json: {0}", ex));
			}
			ModInstallationManagement.RefreshMod(mod);
			return new ValueTuple<bool, int>(false, num);
		}
		return new ValueTuple<bool, int>(false, num);
	}

	// Token: 0x06003DBF RID: 15807 RVA: 0x00147158 File Offset: 0x00145358
	public static Task RefreshUserProfile(Action<bool> callback = null, bool force = false)
	{
		ModIOManager.<RefreshUserProfile>d__67 <RefreshUserProfile>d__;
		<RefreshUserProfile>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RefreshUserProfile>d__.callback = callback;
		<RefreshUserProfile>d__.force = force;
		<RefreshUserProfile>d__.<>1__state = -1;
		<RefreshUserProfile>d__.<>t__builder.Start<ModIOManager.<RefreshUserProfile>d__67>(ref <RefreshUserProfile>d__);
		return <RefreshUserProfile>d__.<>t__builder.Task;
	}

	// Token: 0x06003DC0 RID: 15808 RVA: 0x001471A4 File Offset: 0x001453A4
	[return: TupleElementNames(new string[]
	{
		"error",
		"mods"
	})]
	public static Task<ValueTuple<Error, ICollection<Mod>>> GetMods(ICollection<long> modIds, bool forceRefresh = false, Action<Error, ICollection<Mod>> callback = null)
	{
		ModIOManager.<GetMods>d__68 <GetMods>d__;
		<GetMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ICollection<Mod>>>.Create();
		<GetMods>d__.modIds = modIds;
		<GetMods>d__.forceRefresh = forceRefresh;
		<GetMods>d__.callback = callback;
		<GetMods>d__.<>1__state = -1;
		<GetMods>d__.<>t__builder.Start<ModIOManager.<GetMods>d__68>(ref <GetMods>d__);
		return <GetMods>d__.<>t__builder.Task;
	}

	// Token: 0x06003DC1 RID: 15809 RVA: 0x001471F8 File Offset: 0x001453F8
	[return: TupleElementNames(new string[]
	{
		"error",
		"result"
	})]
	public static Task<ValueTuple<Error, Mod>> GetMod(ModId modId, bool forceUpdate = false, Action<Error, Mod> callback = null)
	{
		ModIOManager.<GetMod>d__69 <GetMod>d__;
		<GetMod>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod>>.Create();
		<GetMod>d__.modId = modId;
		<GetMod>d__.forceUpdate = forceUpdate;
		<GetMod>d__.callback = callback;
		<GetMod>d__.<>1__state = -1;
		<GetMod>d__.<>t__builder.Start<ModIOManager.<GetMod>d__69>(ref <GetMod>d__);
		return <GetMod>d__.<>t__builder.Task;
	}

	// Token: 0x06003DC2 RID: 15810 RVA: 0x0014724C File Offset: 0x0014544C
	[return: TupleElementNames(new string[]
	{
		"error",
		"logo"
	})]
	public static Task<ValueTuple<Error, Texture2D>> GetModLogo(Mod mod, Action<Error, Texture2D> callback)
	{
		ModIOManager.<GetModLogo>d__70 <GetModLogo>d__;
		<GetModLogo>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Texture2D>>.Create();
		<GetModLogo>d__.mod = mod;
		<GetModLogo>d__.callback = callback;
		<GetModLogo>d__.<>1__state = -1;
		<GetModLogo>d__.<>t__builder.Start<ModIOManager.<GetModLogo>d__70>(ref <GetModLogo>d__);
		return <GetModLogo>d__.<>t__builder.Task;
	}

	// Token: 0x06003DC3 RID: 15811 RVA: 0x00147298 File Offset: 0x00145498
	[return: TupleElementNames(new string[]
	{
		"error",
		"modsPage"
	})]
	public static Task<ValueTuple<Error, ModioPage<Mod>>> GetMods(ModioAPI.Mods.GetModsFilter searchFilter)
	{
		ModIOManager.<GetMods>d__71 <GetMods>d__;
		<GetMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, ModioPage<Mod>>>.Create();
		<GetMods>d__.searchFilter = searchFilter;
		<GetMods>d__.<>1__state = -1;
		<GetMods>d__.<>t__builder.Start<ModIOManager.<GetMods>d__71>(ref <GetMods>d__);
		return <GetMods>d__.<>t__builder.Task;
	}

	// Token: 0x06003DC4 RID: 15812 RVA: 0x001472DC File Offset: 0x001454DC
	private static void ModIOUserChanged(User currentUser)
	{
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::ModIOUserChanged] CurrentUser: " + ((currentUser == null) ? "NULL" : currentUser.Profile.Username));
		}
		UnityEvent<User> onModIOUserChanged = ModIOManager.OnModIOUserChanged;
		if (onModIOUserChanged == null)
		{
			return;
		}
		onModIOUserChanged.Invoke(currentUser);
	}

	// Token: 0x06003DC5 RID: 15813 RVA: 0x00147328 File Offset: 0x00145528
	private static void ModIOUserSyncComplete()
	{
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::ModIOUserSyncComplete] Refreshing mod cache...");
		}
		ModIOManager.RefreshModCache();
	}

	// Token: 0x06003DC6 RID: 15814 RVA: 0x00147345 File Offset: 0x00145545
	public static bool IsLoggedIn()
	{
		return User.Current != null && User.Current.IsAuthenticated;
	}

	// Token: 0x06003DC7 RID: 15815 RVA: 0x0014735A File Offset: 0x0014555A
	public static bool IsLoggingIn()
	{
		return ModIOManager.loggingIn;
	}

	// Token: 0x06003DC8 RID: 15816 RVA: 0x00147361 File Offset: 0x00145561
	public static bool IsLoggingOut()
	{
		return ModIOManager.loggingOut;
	}

	// Token: 0x06003DC9 RID: 15817 RVA: 0x00147368 File Offset: 0x00145568
	public static string GetCurrentUsername()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::GetCurrentUsername] Username: " + User.Current.Profile.Username);
		}
		return User.Current.Profile.Username;
	}

	// Token: 0x06003DCA RID: 15818 RVA: 0x001473BC File Offset: 0x001455BC
	public static string GetCurrentUserId()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log(string.Format("[ModIOManager::GetCurrentUserId] User ID: {0}", User.Current.Profile.UserId));
		}
		return User.Current.Profile.UserId.ToString();
	}

	// Token: 0x06003DCB RID: 15819 RVA: 0x0014741B File Offset: 0x0014561B
	public static string GetCurrentAuthToken()
	{
		if (!ModIOManager.IsLoggedIn())
		{
			return "";
		}
		return User.Current.Token;
	}

	// Token: 0x06003DCC RID: 15820 RVA: 0x00147434 File Offset: 0x00145634
	public static bool IsAuthenticated(bool sendEvents = false)
	{
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		bool isAuthenticated = User.Current.IsAuthenticated;
		if (isAuthenticated)
		{
			ModIOManager.loggingIn = false;
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::IsAuthenticated] User already authenticated...");
			}
			if (sendEvents)
			{
				UnityEvent onModIOLoggedIn = ModIOManager.OnModIOLoggedIn;
				if (onModIOLoggedIn != null)
				{
					onModIOLoggedIn.Invoke();
				}
			}
		}
		else
		{
			try
			{
				ModioLog verbose2 = ModioLog.Verbose;
				if (verbose2 != null)
				{
					verbose2.Log("[ModIOManager::IsAuthenticated] User not authenticated");
				}
				if (sendEvents)
				{
					UnityEvent onModIOLoggedOut = ModIOManager.OnModIOLoggedOut;
					if (onModIOLoggedOut != null)
					{
						onModIOLoggedOut.Invoke();
					}
				}
			}
			catch (Exception ex)
			{
				ModioLog verbose3 = ModioLog.Verbose;
				if (verbose3 != null)
				{
					verbose3.Log(string.Format("[ModIOManager::IsAuthenticated] error {0}", ex));
				}
			}
		}
		ModioLog verbose4 = ModioLog.Verbose;
		if (verbose4 != null)
		{
			verbose4.Log(string.Format("[ModIOManager::IsAuthenticated] returning {0}", isAuthenticated));
		}
		return isAuthenticated;
	}

	// Token: 0x06003DCD RID: 15821 RVA: 0x00147504 File Offset: 0x00145704
	public static void LogoutFromModIO()
	{
		if (!ModIOManager.hasInstance || ModIOManager.loggingIn || !ModIOManager.IsLoggedIn())
		{
			return;
		}
		ModIOManager.loggingOut = true;
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::LogoutFromModIO] Logging out of mod.io...");
		}
		ModIOManager.CancelExternalAuthentication();
		ModIOManager.loggingIn = false;
		User.DeleteUserData();
		ModioLog verbose2 = ModioLog.Verbose;
		if (verbose2 != null)
		{
			verbose2.Log("[ModIOManager::LogoutFromModIO] User data deleted...");
		}
		PlayerPrefs.SetInt("modIOLassSuccessfulAuthMethod", ModIOManager.ModIOAuthMethod.Invalid.GetIndex<ModIOManager.ModIOAuthMethod>());
		ModioLog verbose3 = ModioLog.Verbose;
		if (verbose3 != null)
		{
			verbose3.Log("[ModIOManager::LogoutFromModIO] User fully logged out.");
		}
		ModIOManager.loggingOut = false;
		UnityEvent onModIOLoggedOut = ModIOManager.OnModIOLoggedOut;
		if (onModIOLoggedOut != null)
		{
			onModIOLoggedOut.Invoke();
		}
		ModIOManager.RefreshModCache();
	}

	// Token: 0x06003DCE RID: 15822 RVA: 0x001475A8 File Offset: 0x001457A8
	public static void SetAccountLinkPrompter(IWssAuthPrompter prompter)
	{
		if (ModIOManager.accountLinkingAuthService != null)
		{
			ModIOManager.accountLinkingAuthService.SetPrompter(prompter);
		}
	}

	// Token: 0x06003DCF RID: 15823 RVA: 0x001475BC File Offset: 0x001457BC
	public static Task<Error> RequestAccountLinkCode()
	{
		ModIOManager.<RequestAccountLinkCode>d__83 <RequestAccountLinkCode>d__;
		<RequestAccountLinkCode>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<RequestAccountLinkCode>d__.<>1__state = -1;
		<RequestAccountLinkCode>d__.<>t__builder.Start<ModIOManager.<RequestAccountLinkCode>d__83>(ref <RequestAccountLinkCode>d__);
		return <RequestAccountLinkCode>d__.<>t__builder.Task;
	}

	// Token: 0x06003DD0 RID: 15824 RVA: 0x001475F7 File Offset: 0x001457F7
	public static void CancelExternalAuthentication()
	{
		if (!ModIOManager.hasInstance)
		{
			return;
		}
		if (ModIOManager.accountLinkingAuthService != null && ModIOManager.accountLinkingAuthService.InProgress())
		{
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log("[ModIOManager::CancelExternalAuthentication] Cancelling Mod.io Account Linking process...");
			}
			ModIOManager.accountLinkingAuthService.Cancel();
		}
	}

	// Token: 0x06003DD1 RID: 15825 RVA: 0x00147634 File Offset: 0x00145834
	public static Task<Error> RequestPlatformLogin()
	{
		ModIOManager.<RequestPlatformLogin>d__85 <RequestPlatformLogin>d__;
		<RequestPlatformLogin>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<RequestPlatformLogin>d__.<>1__state = -1;
		<RequestPlatformLogin>d__.<>t__builder.Start<ModIOManager.<RequestPlatformLogin>d__85>(ref <RequestPlatformLogin>d__);
		return <RequestPlatformLogin>d__.<>t__builder.Task;
	}

	// Token: 0x06003DD2 RID: 15826 RVA: 0x00147670 File Offset: 0x00145870
	private Task<Error> InitiatePlatformLogin()
	{
		ModIOManager.<InitiatePlatformLogin>d__86 <InitiatePlatformLogin>d__;
		<InitiatePlatformLogin>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<InitiatePlatformLogin>d__.<>4__this = this;
		<InitiatePlatformLogin>d__.<>1__state = -1;
		<InitiatePlatformLogin>d__.<>t__builder.Start<ModIOManager.<InitiatePlatformLogin>d__86>(ref <InitiatePlatformLogin>d__);
		return <InitiatePlatformLogin>d__.<>t__builder.Task;
	}

	// Token: 0x06003DD3 RID: 15827 RVA: 0x001476B4 File Offset: 0x001458B4
	private Task<Error> ContinuePlatformLogin()
	{
		ModIOManager.<ContinuePlatformLogin>d__87 <ContinuePlatformLogin>d__;
		<ContinuePlatformLogin>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<ContinuePlatformLogin>d__.<>4__this = this;
		<ContinuePlatformLogin>d__.<>1__state = -1;
		<ContinuePlatformLogin>d__.<>t__builder.Start<ModIOManager.<ContinuePlatformLogin>d__87>(ref <ContinuePlatformLogin>d__);
		return <ContinuePlatformLogin>d__.<>t__builder.Task;
	}

	// Token: 0x06003DD4 RID: 15828 RVA: 0x001476F8 File Offset: 0x001458F8
	public void RequestEncryptedAppTicket(Action<bool, string> callback)
	{
		if (this.requestEncryptedAppTicketCallback != null)
		{
			ModioLog warning = ModioLog.Warning;
			if (warning != null)
			{
				warning.Log("[ModIOManager::RequestEncryptedAppTicket] Callback already set, Encrypted App Ticket request already in progress!");
			}
			if (callback != null)
			{
				callback.Invoke(false, "AN ENCRYPTED APP TICKET REQUEST IS ALREADY IN PROGRESS");
			}
			return;
		}
		this.requestEncryptedAppTicketCallback = callback;
		if (ModIOManager.requestEncryptedAppTicketResponse == null)
		{
			ModIOManager.requestEncryptedAppTicketResponse = CallResult<EncryptedAppTicketResponse_t>.Create(new CallResult<EncryptedAppTicketResponse_t>.APIDispatchDelegate(this.OnRequestEncryptedAppTicketFinished));
		}
		ModioLog verbose = ModioLog.Verbose;
		if (verbose != null)
		{
			verbose.Log("[ModIOManager::RequestEncryptedAppTicket] Requesting Steam Encrypted App Ticket...");
		}
		SteamAPICall_t steamAPICall_t = SteamUser.RequestEncryptedAppTicket(null, 0);
		ModIOManager.requestEncryptedAppTicketResponse.Set(steamAPICall_t, null);
	}

	// Token: 0x06003DD5 RID: 15829 RVA: 0x00147780 File Offset: 0x00145980
	private void OnRequestEncryptedAppTicketFinished(EncryptedAppTicketResponse_t response, bool bIOFailure)
	{
		if (bIOFailure)
		{
			ModioLog error = ModioLog.Error;
			if (error != null)
			{
				error.Log("Failed to retrieve EncryptedAppTicket due to a Steam API IO failure...");
			}
			Action<bool, string> action = this.requestEncryptedAppTicketCallback;
			if (action != null)
			{
				action.Invoke(false, "FAILED TO RETRIEVE 'EncryptedAppTicket' DUE TO A STEAM API IO FAILURE.");
			}
			this.requestEncryptedAppTicketCallback = null;
			return;
		}
		EResult eResult = response.m_eResult;
		if (eResult <= 3)
		{
			if (eResult != 1)
			{
				if (eResult == 3)
				{
					ModioLog error2 = ModioLog.Error;
					if (error2 != null)
					{
						error2.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Not connected to steam.");
					}
					Action<bool, string> action2 = this.requestEncryptedAppTicketCallback;
					if (action2 != null)
					{
						action2.Invoke(false, "NOT CONNECTED TO STEAM.");
					}
					this.requestEncryptedAppTicketCallback = null;
					return;
				}
			}
			else
			{
				if (!SteamUser.GetEncryptedAppTicket(ModIOManager.ticketBlob, ModIOManager.ticketBlob.Length, ref ModIOManager.ticketSize))
				{
					ModioLog error3 = ModioLog.Error;
					if (error3 != null)
					{
						error3.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Failed to retrieve " + string.Format("EncryptedAppTicket! Needed size: {0}", ModIOManager.ticketSize));
					}
					Action<bool, string> action3 = this.requestEncryptedAppTicketCallback;
					if (action3 != null)
					{
						action3.Invoke(false, "FAILED TO RETRIEVE 'EncryptedAppTicket'.");
					}
					this.requestEncryptedAppTicketCallback = null;
					return;
				}
				Array.Resize<byte>(ref ModIOManager.ticketBlob, (int)ModIOManager.ticketSize);
				string text = Convert.ToBase64String(ModIOManager.ticketBlob);
				ModioLog verbose = ModioLog.Verbose;
				if (verbose != null)
				{
					verbose.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Successfully retrieved Steam Encrypted App Ticket: " + text);
				}
				Action<bool, string> action4 = this.requestEncryptedAppTicketCallback;
				if (action4 != null)
				{
					action4.Invoke(true, text);
				}
				this.requestEncryptedAppTicketCallback = null;
				return;
			}
		}
		else
		{
			if (eResult == 25)
			{
				ModioLog error4 = ModioLog.Error;
				if (error4 != null)
				{
					error4.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] Rate Limit exceeded, this function should not be called more than once per minute.");
				}
				Action<bool, string> action5 = this.requestEncryptedAppTicketCallback;
				if (action5 != null)
				{
					action5.Invoke(false, "RATE LIMIT EXCEEDED, CAN ONLY REQUEST ONE 'EncryptedAppTicket' PER MINUTE.");
				}
				this.requestEncryptedAppTicketCallback = null;
				return;
			}
			if (eResult == 29)
			{
				ModioLog error5 = ModioLog.Error;
				if (error5 != null)
				{
					error5.Log("[ModIOManager::OnRequestEncryptedAppTicketFinished] There is already a pending EncryptedAppTicket request.");
				}
				Action<bool, string> action6 = this.requestEncryptedAppTicketCallback;
				if (action6 != null)
				{
					action6.Invoke(false, "THERE IS ALREADY AN 'EncryptedAppTicket' REQUEST IN PROGRESS.");
				}
				this.requestEncryptedAppTicketCallback = null;
				return;
			}
		}
		ModioLog error6 = ModioLog.Error;
		if (error6 != null)
		{
			error6.Log(string.Format("[ModIOManager::OnRequestEncryptedAppTicketFinished] Unknown Error: {0}", response.m_eResult));
		}
		Action<bool, string> action7 = this.requestEncryptedAppTicketCallback;
		if (action7 != null)
		{
			action7.Invoke(false, string.Format("{0}", response.m_eResult));
		}
		this.requestEncryptedAppTicketCallback = null;
	}

	// Token: 0x06003DD6 RID: 15830 RVA: 0x00147998 File Offset: 0x00145B98
	public Task<ValueTuple<Error, string>> GetOculusUserId()
	{
		ModIOManager.<GetOculusUserId>d__90 <GetOculusUserId>d__;
		<GetOculusUserId>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, string>>.Create();
		<GetOculusUserId>d__.<>1__state = -1;
		<GetOculusUserId>d__.<>t__builder.Start<ModIOManager.<GetOculusUserId>d__90>(ref <GetOculusUserId>d__);
		return <GetOculusUserId>d__.<>t__builder.Task;
	}

	// Token: 0x06003DD7 RID: 15831 RVA: 0x001479D4 File Offset: 0x00145BD4
	public Task<string> GetOculusAccessToken()
	{
		ModIOManager.<GetOculusAccessToken>d__91 <GetOculusAccessToken>d__;
		<GetOculusAccessToken>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetOculusAccessToken>d__.<>1__state = -1;
		<GetOculusAccessToken>d__.<>t__builder.Start<ModIOManager.<GetOculusAccessToken>d__91>(ref <GetOculusAccessToken>d__);
		return <GetOculusAccessToken>d__.<>t__builder.Task;
	}

	// Token: 0x06003DD8 RID: 15832 RVA: 0x00147A10 File Offset: 0x00145C10
	public Task<string> GetOculusUserProof()
	{
		ModIOManager.<GetOculusUserProof>d__92 <GetOculusUserProof>d__;
		<GetOculusUserProof>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetOculusUserProof>d__.<>1__state = -1;
		<GetOculusUserProof>d__.<>t__builder.Start<ModIOManager.<GetOculusUserProof>d__92>(ref <GetOculusUserProof>d__);
		return <GetOculusUserProof>d__.<>t__builder.Task;
	}

	// Token: 0x06003DD9 RID: 15833 RVA: 0x00147A4B File Offset: 0x00145C4B
	public string GetOculusDevice()
	{
		return "";
	}

	// Token: 0x06003DDA RID: 15834 RVA: 0x00147A52 File Offset: 0x00145C52
	private static void OnAuthenticationComplete(Error error)
	{
		ModIOManager.loggingIn = false;
		if (error)
		{
			UnityEvent<string> onModIOLoginFailed = ModIOManager.OnModIOLoginFailed;
			if (onModIOLoginFailed == null)
			{
				return;
			}
			onModIOLoginFailed.Invoke(string.Format("FAILED TO LOGIN TO MOD.IO: {0}", error));
			return;
		}
		else
		{
			UnityEvent onModIOLoggedIn = ModIOManager.OnModIOLoggedIn;
			if (onModIOLoggedIn == null)
			{
				return;
			}
			onModIOLoggedIn.Invoke();
			return;
		}
	}

	// Token: 0x06003DDB RID: 15835 RVA: 0x00147A8C File Offset: 0x00145C8C
	public static ModIOManager.ModIOAuthMethod GetLastAuthMethod()
	{
		int @int = PlayerPrefs.GetInt("modIOLassSuccessfulAuthMethod", -1);
		if (@int == -1)
		{
			return ModIOManager.ModIOAuthMethod.Invalid;
		}
		return (ModIOManager.ModIOAuthMethod)@int;
	}

	// Token: 0x06003DDC RID: 15836 RVA: 0x00147AAC File Offset: 0x00145CAC
	public static Task<ValueTuple<Error, Mod[]>> GetSubscribedMods()
	{
		ModIOManager.<GetSubscribedMods>d__96 <GetSubscribedMods>d__;
		<GetSubscribedMods>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Mod[]>>.Create();
		<GetSubscribedMods>d__.<>1__state = -1;
		<GetSubscribedMods>d__.<>t__builder.Start<ModIOManager.<GetSubscribedMods>d__96>(ref <GetSubscribedMods>d__);
		return <GetSubscribedMods>d__.<>t__builder.Task;
	}

	// Token: 0x06003DDD RID: 15837 RVA: 0x00147AE8 File Offset: 0x00145CE8
	public static Task<Error> SubscribeToMod(ModId modId, Action<Error> callback)
	{
		ModIOManager.<SubscribeToMod>d__97 <SubscribeToMod>d__;
		<SubscribeToMod>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<SubscribeToMod>d__.modId = modId;
		<SubscribeToMod>d__.callback = callback;
		<SubscribeToMod>d__.<>1__state = -1;
		<SubscribeToMod>d__.<>t__builder.Start<ModIOManager.<SubscribeToMod>d__97>(ref <SubscribeToMod>d__);
		return <SubscribeToMod>d__.<>t__builder.Task;
	}

	// Token: 0x06003DDE RID: 15838 RVA: 0x00147B34 File Offset: 0x00145D34
	public static Task<Error> UnsubscribeFromMod(ModId modId, Action<Error> callback)
	{
		ModIOManager.<UnsubscribeFromMod>d__98 <UnsubscribeFromMod>d__;
		<UnsubscribeFromMod>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
		<UnsubscribeFromMod>d__.modId = modId;
		<UnsubscribeFromMod>d__.callback = callback;
		<UnsubscribeFromMod>d__.<>1__state = -1;
		<UnsubscribeFromMod>d__.<>t__builder.Start<ModIOManager.<UnsubscribeFromMod>d__98>(ref <UnsubscribeFromMod>d__);
		return <UnsubscribeFromMod>d__.<>t__builder.Task;
	}

	// Token: 0x06003DDF RID: 15839 RVA: 0x00147B80 File Offset: 0x00145D80
	public static Task<ValueTuple<bool, ModFileState>> GetSubscribedModStatus(ModId modId)
	{
		ModIOManager.<GetSubscribedModStatus>d__99 <GetSubscribedModStatus>d__;
		<GetSubscribedModStatus>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, ModFileState>>.Create();
		<GetSubscribedModStatus>d__.modId = modId;
		<GetSubscribedModStatus>d__.<>1__state = -1;
		<GetSubscribedModStatus>d__.<>t__builder.Start<ModIOManager.<GetSubscribedModStatus>d__99>(ref <GetSubscribedModStatus>d__);
		return <GetSubscribedModStatus>d__.<>t__builder.Task;
	}

	// Token: 0x06003DE0 RID: 15840 RVA: 0x00147BC4 File Offset: 0x00145DC4
	public static Task<ValueTuple<bool, Mod>> GetSubscribedModProfile(ModId modId, Action<bool, Mod> callback = null)
	{
		ModIOManager.<GetSubscribedModProfile>d__100 <GetSubscribedModProfile>d__;
		<GetSubscribedModProfile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<bool, Mod>>.Create();
		<GetSubscribedModProfile>d__.modId = modId;
		<GetSubscribedModProfile>d__.callback = callback;
		<GetSubscribedModProfile>d__.<>1__state = -1;
		<GetSubscribedModProfile>d__.<>t__builder.Start<ModIOManager.<GetSubscribedModProfile>d__100>(ref <GetSubscribedModProfile>d__);
		return <GetSubscribedModProfile>d__.<>t__builder.Task;
	}

	// Token: 0x06003DE1 RID: 15841 RVA: 0x00147C10 File Offset: 0x00145E10
	public static Task<ModFileState> GetModStatus(ModId modId)
	{
		ModIOManager.<GetModStatus>d__101 <GetModStatus>d__;
		<GetModStatus>d__.<>t__builder = AsyncTaskMethodBuilder<ModFileState>.Create();
		<GetModStatus>d__.modId = modId;
		<GetModStatus>d__.<>1__state = -1;
		<GetModStatus>d__.<>t__builder.Start<ModIOManager.<GetModStatus>d__101>(ref <GetModStatus>d__);
		return <GetModStatus>d__.<>t__builder.Task;
	}

	// Token: 0x06003DE2 RID: 15842 RVA: 0x00147C54 File Offset: 0x00145E54
	public static Task<bool> DownloadMod(ModId modId, Action<bool> callback = null)
	{
		ModIOManager.<DownloadMod>d__102 <DownloadMod>d__;
		<DownloadMod>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<DownloadMod>d__.modId = modId;
		<DownloadMod>d__.callback = callback;
		<DownloadMod>d__.<>1__state = -1;
		<DownloadMod>d__.<>t__builder.Start<ModIOManager.<DownloadMod>d__102>(ref <DownloadMod>d__);
		return <DownloadMod>d__.<>t__builder.Task;
	}

	// Token: 0x06003DE3 RID: 15843 RVA: 0x00147CA0 File Offset: 0x00145EA0
	private void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.RoomName.Contains(GorillaComputer.instance.VStumpRoomPrepend) && !GorillaComputer.instance.IsPlayerInVirtualStump() && !CustomMapManager.IsLocalPlayerInVirtualStump())
		{
			Debug.LogError("[ModIOManager::OnJoinedRoom] Player joined @ room while not in the VStump! Leaving the room...");
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
	}

	// Token: 0x06003DE4 RID: 15844 RVA: 0x00147CF4 File Offset: 0x00145EF4
	public static bool TryGetNewMapsModId(out ModId newMapsModId)
	{
		newMapsModId = ModId.Null;
		if (!ModIOManager.hasInstance)
		{
			return false;
		}
		newMapsModId = new ModId(ModIOManager.instance.newMapsModId);
		return true;
	}

	// Token: 0x06003DE5 RID: 15845 RVA: 0x00147D22 File Offset: 0x00145F22
	public static IEnumerator AssociateMothershipAndModIOAccounts(AssociateMotherhsipAndModIOAccountsRequest data, Action<AssociateMotherhsipAndModIOAccountsResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/AssociatePlayFabAndModIO", "POST");
		string text = JsonUtility.ToJson(data);
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		request.timeout = 15;
		yield return request.SendWebRequest();
		if (request.result != 2 && request.result != 3)
		{
			AssociateMotherhsipAndModIOAccountsResponse associateMotherhsipAndModIOAccountsResponse = JsonUtility.FromJson<AssociateMotherhsipAndModIOAccountsResponse>(request.downloadHandler.text);
			callback.Invoke(associateMotherhsipAndModIOAccountsResponse);
		}
		else if (request.result == 3 && request.responseCode != 400L)
		{
			retry = true;
			Debug.LogError(string.Format("HTTP {0} error: {1} message:{2}", request.responseCode, request.error, request.downloadHandler.text));
		}
		else if (request.result == 2)
		{
			retry = true;
			Debug.LogError("NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
		}
		else
		{
			Debug.LogError("HTTP ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
			retry = true;
		}
		if (retry)
		{
			if (ModIOManager.currentAssociationRetries < ModIOManager.associationMaxRetries)
			{
				int num = (int)Mathf.Pow(2f, (float)(ModIOManager.currentAssociationRetries + 1));
				Debug.LogWarning(string.Format("Retrying Account Association... Retry attempt #{0}, waiting for {1} seconds", ModIOManager.currentAssociationRetries + 1, num));
				ModIOManager.currentAssociationRetries++;
				yield return new WaitForSeconds((float)num);
				ModIOManager.AssociateMothershipAndModIOAccounts(data, callback);
			}
			else
			{
				Debug.LogError("Maximum retries attempted. Please check your network connection.");
				callback.Invoke(null);
			}
		}
		yield break;
	}

	// Token: 0x04004E38 RID: 20024
	private const string MODIO_ACCEPTED_TERMS_KEY = "modIOAcceptedTermsHash";

	// Token: 0x04004E39 RID: 20025
	private const string MODIO_ACCEPTED_TERMS_OF_USE_ID_KEY = "modIOAcceptedTermsOfUseId";

	// Token: 0x04004E3A RID: 20026
	private const string MODIO_ACCEPTED_PRIVACY_POLICY_ID_KEY = "modIOAcceptedPrivacyPolicyId";

	// Token: 0x04004E3B RID: 20027
	private const string MODIO_LAST_AUTH_METHOD_KEY = "modIOLassSuccessfulAuthMethod";

	// Token: 0x04004E3C RID: 20028
	private const string FAVORITES_FILE_NAME = "favoriteMods.json";

	// Token: 0x04004E3D RID: 20029
	private const float REFRESH_RATE_LIMIT = 5f;

	// Token: 0x04004E3E RID: 20030
	[OnEnterPlay_SetNull]
	private static volatile ModIOManager instance;

	// Token: 0x04004E3F RID: 20031
	[OnEnterPlay_Set(false)]
	private static bool hasInstance;

	// Token: 0x04004E40 RID: 20032
	private static string ModIODirectory;

	// Token: 0x04004E41 RID: 20033
	private static ModioWssAuthService accountLinkingAuthService = new ModioWssAuthService();

	// Token: 0x04004E42 RID: 20034
	private static bool initialized;

	// Token: 0x04004E43 RID: 20035
	private static bool refreshing;

	// Token: 0x04004E44 RID: 20036
	private static bool modManagementEnabled;

	// Token: 0x04004E45 RID: 20037
	private static bool loggingIn;

	// Token: 0x04004E46 RID: 20038
	private static bool loggingOut;

	// Token: 0x04004E47 RID: 20039
	private static bool refreshingModCache;

	// Token: 0x04004E48 RID: 20040
	private static bool favoriteModsLoaded;

	// Token: 0x04004E49 RID: 20041
	private static bool restartRefreshModCache;

	// Token: 0x04004E4A RID: 20042
	private static Coroutine refreshDisabledCoroutine;

	// Token: 0x04004E4B RID: 20043
	private static float lastRefreshTime;

	// Token: 0x04004E4C RID: 20044
	private static List<Action<bool>> currentRefreshCallbacks = new List<Action<bool>>();

	// Token: 0x04004E4D RID: 20045
	private static Action<ModIORequestResultAnd<bool>> modIOTermsAcknowledgedCallback;

	// Token: 0x04004E4E RID: 20046
	private static Dictionary<ModId, Mod> favoriteMods = new Dictionary<ModId, Mod>();

	// Token: 0x04004E4F RID: 20047
	private static Dictionary<ModId, int> outdatedModCMSVersions = new Dictionary<ModId, int>();

	// Token: 0x04004E50 RID: 20048
	private static byte[] ticketBlob = new byte[1024];

	// Token: 0x04004E51 RID: 20049
	private static uint ticketSize;

	// Token: 0x04004E52 RID: 20050
	protected static CallResult<EncryptedAppTicketResponse_t> requestEncryptedAppTicketResponse = null;

	// Token: 0x04004E53 RID: 20051
	private Action<bool, string> requestEncryptedAppTicketCallback;

	// Token: 0x04004E54 RID: 20052
	private static ModioSteamAuthService steamAuthService = new ModioSteamAuthService();

	// Token: 0x04004E55 RID: 20053
	[SerializeField]
	private GameObject modIOTermsOfUsePrefab;

	// Token: 0x04004E56 RID: 20054
	[SerializeField]
	private long newMapsModId;

	// Token: 0x04004E57 RID: 20055
	public static UnityEvent OnModIOLoginStarted = new UnityEvent();

	// Token: 0x04004E58 RID: 20056
	public static UnityEvent OnModIOLoggedIn = new UnityEvent();

	// Token: 0x04004E59 RID: 20057
	public static UnityEvent<string> OnModIOLoginFailed = new UnityEvent<string>();

	// Token: 0x04004E5A RID: 20058
	public static UnityEvent OnModIOLoggedOut = new UnityEvent();

	// Token: 0x04004E5B RID: 20059
	public static UnityEvent<User> OnModIOUserChanged = new UnityEvent<User>();

	// Token: 0x04004E5C RID: 20060
	public static UnityEvent<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase> OnModManagementEvent = new UnityEvent<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>();

	// Token: 0x04004E5D RID: 20061
	public static UnityEvent OnModIOCacheRefreshing = new UnityEvent();

	// Token: 0x04004E5E RID: 20062
	public static UnityEvent OnModIOCacheRefreshed = new UnityEvent();

	// Token: 0x04004E5F RID: 20063
	private static int associationMaxRetries = 5;

	// Token: 0x04004E60 RID: 20064
	private static int currentAssociationRetries = 0;

	// Token: 0x02000968 RID: 2408
	public enum ModIOAuthMethod
	{
		// Token: 0x04004E62 RID: 20066
		Invalid,
		// Token: 0x04004E63 RID: 20067
		LinkedAccount,
		// Token: 0x04004E64 RID: 20068
		Steam,
		// Token: 0x04004E65 RID: 20069
		Oculus
	}
}
