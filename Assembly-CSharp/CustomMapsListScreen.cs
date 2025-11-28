using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio.Mods;
using Modio.Users;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200099A RID: 2458
public class CustomMapsListScreen : CustomMapsTerminalScreen
{
	// Token: 0x170005C0 RID: 1472
	// (get) Token: 0x06003EAD RID: 16045 RVA: 0x0014F4EF File Offset: 0x0014D6EF
	public bool OfficialMapsOnly
	{
		get
		{
			return this.officialMapsOnly;
		}
	}

	// Token: 0x170005C1 RID: 1473
	// (get) Token: 0x06003EAE RID: 16046 RVA: 0x0014F4F7 File Offset: 0x0014D6F7
	public int CurrentModPage
	{
		get
		{
			return this.currentModPage;
		}
	}

	// Token: 0x170005C2 RID: 1474
	// (get) Token: 0x06003EAF RID: 16047 RVA: 0x0014F4FF File Offset: 0x0014D6FF
	public int ModsPerPage
	{
		get
		{
			return this.modsPerPage;
		}
	}

	// Token: 0x170005C3 RID: 1475
	// (get) Token: 0x06003EB0 RID: 16048 RVA: 0x0014F507 File Offset: 0x0014D707
	// (set) Token: 0x06003EB1 RID: 16049 RVA: 0x0014F510 File Offset: 0x0014D710
	public SortModsBy SortType
	{
		get
		{
			return this.sortType;
		}
		set
		{
			if (this.sortType != value)
			{
				this.currentAvailableModsRequestPage = 0;
			}
			this.sortType = value;
			switch (this.sortType)
			{
			case 0:
				this.isAscendingOrder = true;
				return;
			case 1:
				break;
			case 2:
				this.isAscendingOrder = false;
				return;
			case 3:
				this.isAscendingOrder = false;
				return;
			case 4:
				this.isAscendingOrder = false;
				return;
			case 5:
				this.isAscendingOrder = false;
				return;
			case 6:
				this.isAscendingOrder = false;
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06003EB2 RID: 16050 RVA: 0x0014F58D File Offset: 0x0014D78D
	private void Awake()
	{
		this.subscribedBttnPosition = this.subscribedMapsButton.transform.position;
		this.searchBttnPosition = this.searchButton.transform.position;
	}

	// Token: 0x06003EB3 RID: 16051 RVA: 0x00002789 File Offset: 0x00000989
	public override void Initialize()
	{
	}

	// Token: 0x06003EB4 RID: 16052 RVA: 0x0014F5BC File Offset: 0x0014D7BC
	public override void Show()
	{
		base.Show();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOCacheRefreshing.RemoveListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshing.AddListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshed.RemoveListener(new UnityAction(this.OnModCacheRefreshed));
		ModIOManager.OnModIOCacheRefreshed.AddListener(new UnityAction(this.OnModCacheRefreshed));
		if (this.featuredMods.IsNullOrEmpty<Mod>())
		{
			this.RetrieveFeaturedMods();
		}
		if (this.availableMods.IsNullOrEmpty<Mod>())
		{
			this.RetrieveAvailableMods();
		}
		this.RetrieveInstalledMods(false);
		this.RetrieveFavoriteMods(false);
		this.RetrieveSubscribedMods();
		this.RefreshScreenState();
	}

	// Token: 0x06003EB5 RID: 16053 RVA: 0x0014F6F0 File Offset: 0x0014D8F0
	public override void Hide()
	{
		base.Hide();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOCacheRefreshing.RemoveListener(new UnityAction(this.OnModCacheRefreshing));
		ModIOManager.OnModIOCacheRefreshed.RemoveListener(new UnityAction(this.OnModCacheRefreshed));
	}

	// Token: 0x06003EB6 RID: 16054 RVA: 0x0014F771 File Offset: 0x0014D971
	private void OnModIOLoggedIn()
	{
		if (CustomMapsTerminal.IsDriver)
		{
			this.subscribedMapsButton.gameObject.SetActive(true);
		}
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
		this.RetrieveSubscribedMods();
	}

	// Token: 0x06003EB7 RID: 16055 RVA: 0x0014F7AB File Offset: 0x0014D9AB
	private void OnModIOLoggedOut()
	{
		this.subscribedMapsButton.gameObject.SetActive(false);
		this.subscribedMods = null;
		this.filteredSubscribedMods.Clear();
		this.totalSubscribedMods = 0;
	}

	// Token: 0x06003EB8 RID: 16056 RVA: 0x00002789 File Offset: 0x00000989
	private void OnModIOUserChanged(User user)
	{
	}

	// Token: 0x06003EB9 RID: 16057 RVA: 0x0014F7D7 File Offset: 0x0014D9D7
	private void OnModCacheRefreshing()
	{
		this.RefreshScreenState();
	}

	// Token: 0x06003EBA RID: 16058 RVA: 0x0014F7DF File Offset: 0x0014D9DF
	private void OnModCacheRefreshed()
	{
		this.RetrieveFavoriteMods(false);
		this.RetrieveInstalledMods(false);
		if (ModIOManager.IsLoggedIn())
		{
			this.RetrieveSubscribedMods();
		}
	}

	// Token: 0x06003EBB RID: 16059 RVA: 0x0014F800 File Offset: 0x0014DA00
	public override void PressButton(CustomMapKeyboardBinding buttonPressed)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		GTDev.Log<string>("[CustomMapsListScreen::PressButton] Is Driver: " + CustomMapsTerminal.IsDriver.ToString() + ", Button Pressed: " + buttonPressed.ToString(), null);
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.goback)
		{
			return;
		}
		if (this.loadingText.gameObject.activeSelf)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.option3)
		{
			ModIOManager.RefreshUserProfile(delegate(bool result)
			{
				if (result)
				{
					this.Refresh();
				}
			}, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.option4)
		{
			CustomMapsTerminal.ShowSearchScreen();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.up)
		{
			this.currentModPage--;
			this.RefreshScreenState();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.down)
		{
			this.currentModPage++;
			this.RefreshScreenState();
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.all)
		{
			bool flag = this.officialMapsOnly;
			this.officialMapsOnly = false;
			this.displayFeaturedMods = (this.sortType == 3);
			if (flag)
			{
				this.RefreshModSearch();
			}
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, flag);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.mustplay)
		{
			bool flag2 = !this.officialMapsOnly;
			this.officialMapsOnly = true;
			this.displayFeaturedMods = false;
			if (flag2)
			{
				this.RefreshModSearch();
			}
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, flag2);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.sub)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.SubscribedMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.fav)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.FavoriteMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.inst)
		{
			this.SwapListDisplay(CustomMapsListScreen.ListScreenState.InstalledMods, false);
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.sort)
		{
			this.SetSortType();
			this.RefreshModSearch();
			return;
		}
		if (CustomMapKeyboardBinding.one <= buttonPressed && buttonPressed <= CustomMapKeyboardBinding.nine && !this.customMapsGalleryView.IsNull())
		{
			this.customMapsGalleryView.ShowDetailsForEntry(buttonPressed - CustomMapKeyboardBinding.one);
		}
	}

	// Token: 0x06003EBC RID: 16060 RVA: 0x0014F990 File Offset: 0x0014DB90
	private void SetSortType()
	{
		this.currentAvailableModsRequestPage = 0;
		this.sortTypeIndex++;
		if (this.sortTypeIndex >= 6)
		{
			this.sortTypeIndex = 0;
		}
		switch (this.sortTypeIndex)
		{
		case 0:
			this.SortType = 3;
			this.useMapName = true;
			this.displayFeaturedMods = !this.officialMapsOnly;
			return;
		case 1:
			this.SortType = 6;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 2:
			this.SortType = 2;
			this.useMapName = false;
			this.displayFeaturedMods = false;
			return;
		case 3:
			this.SortType = 4;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 4:
			this.SortType = 5;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		case 5:
			this.SortType = 0;
			this.useMapName = true;
			this.displayFeaturedMods = false;
			return;
		default:
			this.sortTypeIndex = 0;
			this.SortType = 3;
			this.useMapName = true;
			this.displayFeaturedMods = !this.officialMapsOnly;
			return;
		}
	}

	// Token: 0x06003EBD RID: 16061 RVA: 0x0014FA9C File Offset: 0x0014DC9C
	public void SwapListDisplay(CustomMapsListScreen.ListScreenState newState, bool force = false)
	{
		if (this.currentState == newState && !force)
		{
			return;
		}
		if (newState == CustomMapsListScreen.ListScreenState.SubscribedMods && !ModIOManager.IsLoggedIn())
		{
			return;
		}
		this.currentState = newState;
		this.currentModPage = 0;
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.allMapsButton.SetButtonActive(!this.officialMapsOnly);
			this.officialMapsButton.SetButtonActive(this.officialMapsOnly);
			this.favoriteMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			break;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.allMapsButton.SetButtonActive(false);
			this.officialMapsButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(true);
			break;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.allMapsButton.SetButtonActive(false);
			this.officialMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(true);
			break;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.allMapsButton.SetButtonActive(false);
			this.officialMapsButton.SetButtonActive(false);
			this.installedMapsButton.SetButtonActive(false);
			this.favoriteMapsButton.SetButtonActive(false);
			this.searchButton.SetButtonActive(false);
			this.subscribedMapsButton.SetButtonActive(true);
			break;
		}
		this.RefreshScreenState();
	}

	// Token: 0x06003EBE RID: 16062 RVA: 0x0014FC34 File Offset: 0x0014DE34
	public void RefreshModSearch()
	{
		if (this.loadingAvailableMods || this.loadingFavoriteMods || this.loadingInstalledMods || this.loadingSubscribedMods)
		{
			return;
		}
		this.currentModPage = 0;
		this.availableMods.Clear();
		this.filteredAvailableMods.Clear();
		this.currentAvailableModsRequestPage = 0;
		this.errorLoadingAvailableMods = false;
		this.totalAvailableMods = 0;
		this.RetrieveAvailableMods();
	}

	// Token: 0x06003EBF RID: 16063 RVA: 0x0014FC9C File Offset: 0x0014DE9C
	public void Refresh()
	{
		if (this.loadingAvailableMods || this.loadingFavoriteMods || this.loadingFeaturedMods || this.loadingInstalledMods || this.loadingSubscribedMods)
		{
			return;
		}
		this.currentModPage = 0;
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.featuredMods.Clear();
			this.availableMods.Clear();
			this.filteredAvailableMods.Clear();
			this.currentAvailableModsRequestPage = 0;
			this.errorLoadingAvailableMods = false;
			this.totalAvailableMods = 0;
			this.RetrieveFeaturedMods();
			this.RetrieveAvailableMods();
			return;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.RetrieveInstalledMods(true);
			return;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.RetrieveFavoriteMods(true);
			return;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.RetrieveSubscribedMods();
			return;
		default:
			return;
		}
	}

	// Token: 0x06003EC0 RID: 16064 RVA: 0x0014FD54 File Offset: 0x0014DF54
	private void RetrieveFeaturedMods()
	{
		if (this.loadingFeaturedMods || this.featuredMods.Count > 0)
		{
			return;
		}
		this.loadingFeaturedMods = true;
		PlayFabTitleDataCache.Instance.GetTitleData(this.featuredModsPlayFabKey, new Action<string>(this.OnGetFeaturedModsTitleData), delegate(PlayFabError error)
		{
			this.loadingFeaturedMods = false;
			this.RefreshScreenState();
		}, false);
	}

	// Token: 0x06003EC1 RID: 16065 RVA: 0x0014FDA8 File Offset: 0x0014DFA8
	private void OnGetFeaturedModsTitleData(string data)
	{
		CustomMapsListScreen.<OnGetFeaturedModsTitleData>d__102 <OnGetFeaturedModsTitleData>d__;
		<OnGetFeaturedModsTitleData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnGetFeaturedModsTitleData>d__.<>4__this = this;
		<OnGetFeaturedModsTitleData>d__.data = data;
		<OnGetFeaturedModsTitleData>d__.<>1__state = -1;
		<OnGetFeaturedModsTitleData>d__.<>t__builder.Start<CustomMapsListScreen.<OnGetFeaturedModsTitleData>d__102>(ref <OnGetFeaturedModsTitleData>d__);
	}

	// Token: 0x06003EC2 RID: 16066 RVA: 0x0014FDE8 File Offset: 0x0014DFE8
	private void RetrieveAvailableMods()
	{
		CustomMapsListScreen.<RetrieveAvailableMods>d__103 <RetrieveAvailableMods>d__;
		<RetrieveAvailableMods>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RetrieveAvailableMods>d__.<>4__this = this;
		<RetrieveAvailableMods>d__.<>1__state = -1;
		<RetrieveAvailableMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveAvailableMods>d__103>(ref <RetrieveAvailableMods>d__);
	}

	// Token: 0x06003EC3 RID: 16067 RVA: 0x0014FE20 File Offset: 0x0014E020
	private void FilterAvailableMods()
	{
		this.filteredAvailableMods.Clear();
		if (this.availableMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		this.totalAvailableMods = Mathf.Max(0, this.totalAvailableMods - 1);
		foreach (Mod mod in this.availableMods)
		{
			ModId modId;
			ModIOManager.TryGetNewMapsModId(out modId);
			if (!(mod.Id == modId) && (!this.displayFeaturedMods || this.featuredModIds.IsNullOrEmpty<long>() || !this.featuredModIds.Contains(mod.Id)))
			{
				this.filteredAvailableMods.Add(mod);
			}
		}
		if (this.displayFeaturedMods && !this.featuredMods.IsNullOrEmpty<Mod>())
		{
			this.filteredAvailableMods.InsertRange(0, this.featuredMods);
		}
	}

	// Token: 0x06003EC4 RID: 16068 RVA: 0x0014FF10 File Offset: 0x0014E110
	private Task RetrieveSubscribedMods()
	{
		CustomMapsListScreen.<RetrieveSubscribedMods>d__105 <RetrieveSubscribedMods>d__;
		<RetrieveSubscribedMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveSubscribedMods>d__.<>4__this = this;
		<RetrieveSubscribedMods>d__.<>1__state = -1;
		<RetrieveSubscribedMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveSubscribedMods>d__105>(ref <RetrieveSubscribedMods>d__);
		return <RetrieveSubscribedMods>d__.<>t__builder.Task;
	}

	// Token: 0x06003EC5 RID: 16069 RVA: 0x0014FF54 File Offset: 0x0014E154
	private void FilterSubscribedMods()
	{
		this.filteredSubscribedMods.Clear();
		if (this.subscribedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.subscribedMods)
		{
			ModId modId;
			ModIOManager.TryGetNewMapsModId(out modId);
			if (!(mod.Id == modId))
			{
				this.filteredSubscribedMods.Add(mod);
			}
		}
	}

	// Token: 0x06003EC6 RID: 16070 RVA: 0x0014FFB8 File Offset: 0x0014E1B8
	private Task RetrieveInstalledMods(bool forceRefresh = false)
	{
		CustomMapsListScreen.<RetrieveInstalledMods>d__107 <RetrieveInstalledMods>d__;
		<RetrieveInstalledMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveInstalledMods>d__.<>4__this = this;
		<RetrieveInstalledMods>d__.forceRefresh = forceRefresh;
		<RetrieveInstalledMods>d__.<>1__state = -1;
		<RetrieveInstalledMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveInstalledMods>d__107>(ref <RetrieveInstalledMods>d__);
		return <RetrieveInstalledMods>d__.<>t__builder.Task;
	}

	// Token: 0x06003EC7 RID: 16071 RVA: 0x00150004 File Offset: 0x0014E204
	private void FilterInstalledMods()
	{
		this.filteredInstalledMods.Clear();
		if (this.installedMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.installedMods)
		{
			ModId modId;
			if (!ModIOManager.TryGetNewMapsModId(out modId) || !(mod.Id == modId))
			{
				this.filteredInstalledMods.Add(mod);
			}
		}
	}

	// Token: 0x06003EC8 RID: 16072 RVA: 0x00150068 File Offset: 0x0014E268
	private Task RetrieveFavoriteMods(bool forceRefresh = false)
	{
		CustomMapsListScreen.<RetrieveFavoriteMods>d__109 <RetrieveFavoriteMods>d__;
		<RetrieveFavoriteMods>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<RetrieveFavoriteMods>d__.<>4__this = this;
		<RetrieveFavoriteMods>d__.forceRefresh = forceRefresh;
		<RetrieveFavoriteMods>d__.<>1__state = -1;
		<RetrieveFavoriteMods>d__.<>t__builder.Start<CustomMapsListScreen.<RetrieveFavoriteMods>d__109>(ref <RetrieveFavoriteMods>d__);
		return <RetrieveFavoriteMods>d__.<>t__builder.Task;
	}

	// Token: 0x06003EC9 RID: 16073 RVA: 0x001500B4 File Offset: 0x0014E2B4
	private void FilterFavoriteMods()
	{
		this.filteredFavoriteMods.Clear();
		if (this.favoriteMods.IsNullOrEmpty<Mod>())
		{
			return;
		}
		foreach (Mod mod in this.favoriteMods)
		{
			ModId modId;
			if (!ModIOManager.TryGetNewMapsModId(out modId) || !(mod.Id == modId))
			{
				this.filteredFavoriteMods.Add(mod);
			}
		}
	}

	// Token: 0x06003ECA RID: 16074 RVA: 0x0015013C File Offset: 0x0014E33C
	public void GetDisplayedModList(out long[] modList)
	{
		if (this.displayedModProfiles.IsNullOrEmpty<Mod>())
		{
			modList = Array.Empty<long>();
			return;
		}
		modList = new long[this.displayedModProfiles.Count];
		for (int i = 0; i < this.displayedModProfiles.Count; i++)
		{
			modList[i] = this.displayedModProfiles[i].Id;
		}
	}

	// Token: 0x06003ECB RID: 16075 RVA: 0x001501A0 File Offset: 0x0014E3A0
	private void RefreshScreenState()
	{
		this.displayedModProfiles.Clear();
		this.errorText.gameObject.SetActive(false);
		this.sortTypeText.gameObject.SetActive(false);
		this.modPageText.gameObject.SetActive(false);
		this.titleText.text = this.GetTitleForCurrentState();
		this.loadingText.gameObject.SetActive(true);
		if (CustomMapsTerminal.IsDriver && ModIOManager.IsLoggedIn())
		{
			this.subscribedMapsButton.gameObject.SetActive(true);
			this.subscribedMapsButton.transform.position = this.subscribedBttnPosition;
			this.searchButton.transform.position = this.searchBttnPosition;
		}
		else
		{
			this.subscribedMapsButton.gameObject.SetActive(false);
			this.subscribedMapsButton.transform.position = this.searchBttnPosition;
			this.searchButton.transform.position = this.subscribedBttnPosition;
		}
		if (this.currentState == CustomMapsListScreen.ListScreenState.AvailableMods)
		{
			this.RefreshScreenForAvailableMods();
			return;
		}
		this.sortByButton.SetActive(false);
		this.RefreshScreenForCurrentState();
	}

	// Token: 0x06003ECC RID: 16076 RVA: 0x001502B8 File Offset: 0x0014E4B8
	private void RefreshScreenForAvailableMods()
	{
		string text = (this.sortType == 6) ? "NEWEST" : this.sortType.ToString().ToUpper();
		this.sortByButton.SetActive(true);
		this.sortTypeText.gameObject.SetActive(true);
		this.sortTypeText.text = text;
		this.customMapsGalleryView.ResetGallery();
		if (this.loadingAvailableMods)
		{
			return;
		}
		if (this.errorLoadingAvailableMods)
		{
			this.errorText.text = this.failedToRetrieveModsString;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.UpdatePageCount(this.totalAvailableMods);
		int num = 0;
		int num2 = this.modsPerPage - 1;
		if (!this.IsOnFirstPage())
		{
			num = this.currentModPage * this.modsPerPage;
			num2 = num + this.modsPerPage - 1;
			this.pageUpButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageUpButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.pageDownButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageDownButton.gameObject.SetActive(false);
		}
		if (this.filteredAvailableMods.Count <= num2 && this.totalAvailableMods > this.availableMods.Count)
		{
			this.displayedModProfiles.Clear();
			this.RetrieveAvailableMods();
			return;
		}
		int num3 = num;
		while (num3 <= num2 && this.filteredAvailableMods.Count > num3)
		{
			this.displayedModProfiles.Add(this.filteredAvailableMods[num3]);
			num3++;
		}
		string text2;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedModProfiles, this.useMapName, out text2))
		{
			this.errorText.text = text2;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		if (this.displayFeaturedMods && !this.featuredModIds.IsNullOrEmpty<long>())
		{
			for (int i = 0; i < this.displayedModProfiles.Count; i++)
			{
				if (this.featuredModIds.Contains(this.displayedModProfiles[i].Id))
				{
					this.customMapsGalleryView.HighlightTileAtIndex(num + i);
				}
			}
		}
		this.loadingText.gameObject.SetActive(false);
	}

	// Token: 0x06003ECD RID: 16077 RVA: 0x00150514 File Offset: 0x0014E714
	private void RefreshScreenForCurrentState()
	{
		this.customMapsGalleryView.ResetGallery();
		if (this.GetLoadingStatusForCurrentState())
		{
			return;
		}
		if (this.HasModLoadingErrorForCurrentState())
		{
			this.modPageText.gameObject.SetActive(false);
			if (CustomMapsTerminal.IsDriver)
			{
				this.currentModPage = -1;
			}
			this.errorText.text = this.failedToRetrieveModsString;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.UpdatePageCount(this.GetTotalModsForCurrentState());
		if (!this.IsOnFirstPage())
		{
			this.pageUpButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageUpButton.gameObject.SetActive(false);
		}
		if (!this.IsOnLastPage())
		{
			this.pageDownButton.gameObject.SetActive(true);
		}
		else
		{
			this.pageDownButton.gameObject.SetActive(false);
		}
		List<Mod> modListForCurrentState = this.GetModListForCurrentState();
		if (modListForCurrentState != null)
		{
			if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
			{
				this.displayedModProfiles.AddRange(modListForCurrentState);
			}
			else
			{
				int num = this.currentModPage * this.modsPerPage;
				int num2 = num;
				while (num2 < num + this.modsPerPage && modListForCurrentState.Count > num2)
				{
					this.displayedModProfiles.Add(modListForCurrentState[num2]);
					num2++;
				}
			}
		}
		string text;
		if (!this.customMapsGalleryView.DisplayGallery(this.displayedModProfiles, true, out text))
		{
			this.errorText.text = text;
			this.loadingText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.loadingText.gameObject.SetActive(false);
	}

	// Token: 0x06003ECE RID: 16078 RVA: 0x001506A8 File Offset: 0x0014E8A8
	private bool GetLoadingStatusForCurrentState()
	{
		if (ModIOManager.IsRefreshing())
		{
			return true;
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.loadingAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.loadingInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.loadingFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.loadingSubscribedMods;
		default:
			return false;
		}
	}

	// Token: 0x06003ECF RID: 16079 RVA: 0x001506FC File Offset: 0x0014E8FC
	private bool HasModLoadingErrorForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.errorLoadingAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.errorLoadingInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.errorLoadingFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.errorLoadingSubscribedMods;
		default:
			return false;
		}
	}

	// Token: 0x06003ED0 RID: 16080 RVA: 0x00150748 File Offset: 0x0014E948
	private List<Mod> GetModListForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.filteredAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.filteredInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.filteredFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.filteredSubscribedMods;
		default:
			return null;
		}
	}

	// Token: 0x06003ED1 RID: 16081 RVA: 0x00150794 File Offset: 0x0014E994
	private int GetTotalModsForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			return this.totalAvailableMods;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.totalInstalledMods;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.totalFavoriteMods;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.totalSubscribedMods;
		default:
			return 0;
		}
	}

	// Token: 0x06003ED2 RID: 16082 RVA: 0x001507E0 File Offset: 0x0014E9E0
	private string GetTitleForCurrentState()
	{
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			if (this.officialMapsOnly)
			{
				return this.officialModsTitle;
			}
			return this.browseModsTitle;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			return this.installedModsTitle;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			return this.favoriteModsTitle;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			return this.subscribedModsTitle;
		default:
			return "";
		}
	}

	// Token: 0x06003ED3 RID: 16083 RVA: 0x0015083C File Offset: 0x0014EA3C
	private void UpdatePageCount(int totalMods)
	{
		this.totalModCount = totalMods;
		this.modPageText.gameObject.SetActive(false);
		if (this.totalModCount != 0)
		{
			int numPages = this.GetNumPages();
			if (numPages > 1)
			{
				this.modPageText.text = string.Format("{0} / {1}", this.currentModPage + 1, numPages);
				this.modPageText.gameObject.SetActive(true);
			}
			return;
		}
		switch (this.currentState)
		{
		case CustomMapsListScreen.ListScreenState.AvailableMods:
			this.errorText.text = this.noModsAvailableString;
			return;
		case CustomMapsListScreen.ListScreenState.InstalledMods:
			this.errorText.text = this.noInstalledModsString;
			return;
		case CustomMapsListScreen.ListScreenState.FavoriteMods:
			this.errorText.text = this.noFavoriteModsString;
			return;
		case CustomMapsListScreen.ListScreenState.SubscribedMods:
			this.errorText.text = this.noSubscribedModsString;
			return;
		case CustomMapsListScreen.ListScreenState.CustomModList:
			this.errorText.text = this.noModsFoundGenericString;
			return;
		default:
			return;
		}
	}

	// Token: 0x06003ED4 RID: 16084 RVA: 0x0015092C File Offset: 0x0014EB2C
	public int GetNumPages()
	{
		int num = this.totalModCount % this.modsPerPage;
		int num2 = this.totalModCount / this.modsPerPage;
		if (num > 0)
		{
			num2++;
		}
		return num2;
	}

	// Token: 0x06003ED5 RID: 16085 RVA: 0x0015095C File Offset: 0x0014EB5C
	private bool IsOnFirstPage()
	{
		return this.currentModPage == 0;
	}

	// Token: 0x06003ED6 RID: 16086 RVA: 0x00150968 File Offset: 0x0014EB68
	private bool IsOnLastPage()
	{
		long num = (long)this.GetNumPages();
		return (long)(this.currentModPage + 1) == num;
	}

	// Token: 0x06003ED7 RID: 16087 RVA: 0x0015098C File Offset: 0x0014EB8C
	public void RefreshDriverNickname(string driverNickname)
	{
		if (this.currentState == CustomMapsListScreen.ListScreenState.CustomModList)
		{
			this.titleText.text = driverNickname;
		}
	}

	// Token: 0x04004FA3 RID: 20387
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x04004FA4 RID: 20388
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x04004FA5 RID: 20389
	[SerializeField]
	private TMP_Text modPageText;

	// Token: 0x04004FA6 RID: 20390
	[SerializeField]
	private TMP_Text titleText;

	// Token: 0x04004FA7 RID: 20391
	[SerializeField]
	private TMP_Text sortTypeText;

	// Token: 0x04004FA8 RID: 20392
	[SerializeField]
	private GameObject sortByButton;

	// Token: 0x04004FA9 RID: 20393
	[SerializeField]
	private CustomMapsScreenButton allMapsButton;

	// Token: 0x04004FAA RID: 20394
	[SerializeField]
	private CustomMapsScreenButton officialMapsButton;

	// Token: 0x04004FAB RID: 20395
	[SerializeField]
	private CustomMapsScreenButton favoriteMapsButton;

	// Token: 0x04004FAC RID: 20396
	[SerializeField]
	private CustomMapsScreenButton installedMapsButton;

	// Token: 0x04004FAD RID: 20397
	[SerializeField]
	private CustomMapsScreenButton subscribedMapsButton;

	// Token: 0x04004FAE RID: 20398
	[SerializeField]
	private CustomMapsScreenButton searchButton;

	// Token: 0x04004FAF RID: 20399
	[SerializeField]
	private CustomMapsScreenButton pageUpButton;

	// Token: 0x04004FB0 RID: 20400
	[SerializeField]
	private CustomMapsScreenButton pageDownButton;

	// Token: 0x04004FB1 RID: 20401
	[SerializeField]
	private CustomMapsGalleryView customMapsGalleryView;

	// Token: 0x04004FB2 RID: 20402
	[SerializeField]
	private string browseModsTitle = "AVAILABLE MODS";

	// Token: 0x04004FB3 RID: 20403
	[SerializeField]
	private string officialModsTitle = "OFFICIAL MODS";

	// Token: 0x04004FB4 RID: 20404
	[SerializeField]
	private string installedModsTitle = "INSTALLED MODS";

	// Token: 0x04004FB5 RID: 20405
	[SerializeField]
	private string favoriteModsTitle = "FAVORITE MODS";

	// Token: 0x04004FB6 RID: 20406
	[SerializeField]
	private string subscribedModsTitle = "SUBSCRIBED MODS";

	// Token: 0x04004FB7 RID: 20407
	[SerializeField]
	private string noModsAvailableString = "NO MODS AVAILABLE";

	// Token: 0x04004FB8 RID: 20408
	[SerializeField]
	private string noModsFoundGenericString = "NO MODS FOUND";

	// Token: 0x04004FB9 RID: 20409
	[SerializeField]
	private string noSubscribedModsString = "NOT SUBSCRIBED TO ANY MODS";

	// Token: 0x04004FBA RID: 20410
	[SerializeField]
	private string noInstalledModsString = "NO MODS INSTALLED";

	// Token: 0x04004FBB RID: 20411
	[SerializeField]
	private string noFavoriteModsString = "NO FAVORITE MODS FOUND";

	// Token: 0x04004FBC RID: 20412
	[SerializeField]
	private string failedToRetrieveModsString = "FAILED TO RETRIEVE MODS FROM MOD.IO \nPRESS THE 'REFRESH' BUTTON TO RETRY";

	// Token: 0x04004FBD RID: 20413
	[SerializeField]
	private int modsPerPage = 12;

	// Token: 0x04004FBE RID: 20414
	[SerializeField]
	private int numModsPerRequest = 24;

	// Token: 0x04004FBF RID: 20415
	[SerializeField]
	private int maxModListItemLength = 25;

	// Token: 0x04004FC0 RID: 20416
	[SerializeField]
	private string officialMapsTag = "Official Maps";

	// Token: 0x04004FC1 RID: 20417
	[SerializeField]
	private string featuredModsPlayFabKey = "VStumpFeaturedMaps";

	// Token: 0x04004FC2 RID: 20418
	private bool loadingFeaturedMods;

	// Token: 0x04004FC3 RID: 20419
	private bool displayFeaturedMods = true;

	// Token: 0x04004FC4 RID: 20420
	private int totalFeaturedMods;

	// Token: 0x04004FC5 RID: 20421
	private List<long> featuredModIds = new List<long>();

	// Token: 0x04004FC6 RID: 20422
	private List<Mod> featuredMods = new List<Mod>();

	// Token: 0x04004FC7 RID: 20423
	private int currentAvailableModsRequestPage;

	// Token: 0x04004FC8 RID: 20424
	private bool loadingAvailableMods;

	// Token: 0x04004FC9 RID: 20425
	private int totalAvailableMods;

	// Token: 0x04004FCA RID: 20426
	private bool errorLoadingAvailableMods;

	// Token: 0x04004FCB RID: 20427
	private List<Mod> availableMods = new List<Mod>();

	// Token: 0x04004FCC RID: 20428
	private List<Mod> filteredAvailableMods = new List<Mod>();

	// Token: 0x04004FCD RID: 20429
	private bool loadingInstalledMods;

	// Token: 0x04004FCE RID: 20430
	private bool errorLoadingInstalledMods;

	// Token: 0x04004FCF RID: 20431
	private int totalInstalledMods;

	// Token: 0x04004FD0 RID: 20432
	private Mod[] installedMods;

	// Token: 0x04004FD1 RID: 20433
	private List<Mod> filteredInstalledMods = new List<Mod>();

	// Token: 0x04004FD2 RID: 20434
	private bool loadingFavoriteMods;

	// Token: 0x04004FD3 RID: 20435
	private bool errorLoadingFavoriteMods;

	// Token: 0x04004FD4 RID: 20436
	private int totalFavoriteMods;

	// Token: 0x04004FD5 RID: 20437
	private List<Mod> favoriteMods = new List<Mod>();

	// Token: 0x04004FD6 RID: 20438
	private List<Mod> filteredFavoriteMods = new List<Mod>();

	// Token: 0x04004FD7 RID: 20439
	private bool loadingSubscribedMods;

	// Token: 0x04004FD8 RID: 20440
	private bool errorLoadingSubscribedMods;

	// Token: 0x04004FD9 RID: 20441
	private int totalSubscribedMods;

	// Token: 0x04004FDA RID: 20442
	private Mod[] subscribedMods;

	// Token: 0x04004FDB RID: 20443
	private List<Mod> filteredSubscribedMods = new List<Mod>();

	// Token: 0x04004FDC RID: 20444
	private int currentModPage;

	// Token: 0x04004FDD RID: 20445
	private int totalModCount;

	// Token: 0x04004FDE RID: 20446
	private List<Mod> displayedModProfiles = new List<Mod>();

	// Token: 0x04004FDF RID: 20447
	private int sortTypeIndex;

	// Token: 0x04004FE0 RID: 20448
	private SortModsBy sortType = 3;

	// Token: 0x04004FE1 RID: 20449
	private const int MAX_SORT_TYPES = 6;

	// Token: 0x04004FE2 RID: 20450
	private List<string> searchTags = new List<string>();

	// Token: 0x04004FE3 RID: 20451
	private bool isAscendingOrder;

	// Token: 0x04004FE4 RID: 20452
	private bool officialMapsOnly;

	// Token: 0x04004FE5 RID: 20453
	private bool useMapName = true;

	// Token: 0x04004FE6 RID: 20454
	private Vector3 subscribedBttnPosition;

	// Token: 0x04004FE7 RID: 20455
	private Vector3 searchBttnPosition;

	// Token: 0x04004FE8 RID: 20456
	private bool restartCustomModListRetrieval;

	// Token: 0x04004FE9 RID: 20457
	private bool restartCustomModListRetrievalForceRefresh;

	// Token: 0x04004FEA RID: 20458
	private bool restartInstalledModsRetrieval;

	// Token: 0x04004FEB RID: 20459
	private bool restartInstalledModsRetrievalForceRefresh;

	// Token: 0x04004FEC RID: 20460
	private bool restartFavoriteModsRetrieval;

	// Token: 0x04004FED RID: 20461
	private bool restartFavoriteModsRetrievalForceRefresh;

	// Token: 0x04004FEE RID: 20462
	private bool restartSubscribedModsRetrieval;

	// Token: 0x04004FEF RID: 20463
	public CustomMapsListScreen.ListScreenState currentState;

	// Token: 0x0200099B RID: 2459
	public enum ListScreenState
	{
		// Token: 0x04004FF1 RID: 20465
		AvailableMods,
		// Token: 0x04004FF2 RID: 20466
		InstalledMods,
		// Token: 0x04004FF3 RID: 20467
		FavoriteMods,
		// Token: 0x04004FF4 RID: 20468
		SubscribedMods,
		// Token: 0x04004FF5 RID: 20469
		CustomModList
	}
}
