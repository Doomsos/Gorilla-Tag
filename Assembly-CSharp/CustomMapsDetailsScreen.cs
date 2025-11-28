using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio;
using Modio.Mods;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000994 RID: 2452
public class CustomMapsDetailsScreen : CustomMapsTerminalScreen
{
	// Token: 0x170005BE RID: 1470
	// (get) Token: 0x06003E5B RID: 15963 RVA: 0x0014C798 File Offset: 0x0014A998
	// (set) Token: 0x06003E5A RID: 15962 RVA: 0x0014C78F File Offset: 0x0014A98F
	public Mod currentMapMod { get; private set; }

	// Token: 0x06003E5C RID: 15964 RVA: 0x00002789 File Offset: 0x00000989
	public override void Initialize()
	{
	}

	// Token: 0x06003E5D RID: 15965 RVA: 0x0014C7A0 File Offset: 0x0014A9A0
	public override void Show()
	{
		base.Show();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
		ModIOManager.OnModManagementEvent.AddListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadStatusChanged.AddListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnMapLoadComplete.AddListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnRoomMapChanged.AddListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
		CustomMapManager.OnMapUnloadComplete.AddListener(new UnityAction(this.OnMapUnloaded));
		if (!ModIOManager.IsLoggedIn())
		{
			this.subscriptionToggleButton.gameObject.SetActive(false);
		}
		this.deleteButton.gameObject.SetActive(false);
		this.ResetToDefaultView();
	}

	// Token: 0x06003E5E RID: 15966 RVA: 0x0014C944 File Offset: 0x0014AB44
	public override void Hide()
	{
		base.Hide();
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
		ModIOManager.OnModManagementEvent.RemoveListener(new UnityAction<Mod, Modfile, ModInstallationManagement.OperationType, ModInstallationManagement.OperationPhase>(this.HandleModManagementEvent));
		CustomMapManager.OnMapLoadStatusChanged.RemoveListener(new UnityAction<MapLoadStatus, int, string>(this.OnMapLoadProgress));
		CustomMapManager.OnMapLoadComplete.RemoveListener(new UnityAction<bool>(this.OnMapLoadComplete));
		CustomMapManager.OnRoomMapChanged.RemoveListener(new UnityAction<ModId>(this.OnRoomMapChanged));
		CustomMapManager.OnMapUnloadComplete.RemoveListener(new UnityAction(this.OnMapUnloaded));
	}

	// Token: 0x06003E5F RID: 15967 RVA: 0x0014CA08 File Offset: 0x0014AC08
	private void OnModUpdated()
	{
		ModRating currentUserRating = this.currentMapMod.CurrentUserRating;
		this.rateUpButton.SetButtonActive(currentUserRating == 1);
		this.rateDownButton.SetButtonActive(currentUserRating == -1);
	}

	// Token: 0x06003E60 RID: 15968 RVA: 0x0014CA3F File Offset: 0x0014AC3F
	private void OnModIOLoggedIn()
	{
		if (this.currentMapMod.Creator == null)
		{
			this.RefreshCurrentMapMod();
			return;
		}
		if (this.currentMapMod.IsHidden())
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.UpdateStatus(false);
	}

	// Token: 0x06003E61 RID: 15969 RVA: 0x0014CA78 File Offset: 0x0014AC78
	private void OnModIOLoggedOut()
	{
		if (this.currentMapMod.IsHidden())
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.UpdateStatus(false);
	}

	// Token: 0x06003E62 RID: 15970 RVA: 0x0014CA97 File Offset: 0x0014AC97
	private void OnModIOUserChanged(User user)
	{
		this.UpdateStatus(false);
	}

	// Token: 0x06003E63 RID: 15971 RVA: 0x0014CAA4 File Offset: 0x0014ACA4
	private void HandleModManagementEvent(Mod mod, Modfile modfile, ModInstallationManagement.OperationType jobType, ModInstallationManagement.OperationPhase jobPhase)
	{
		if (base.isActiveAndEnabled && this.hasModProfile && this.GetModId() == mod.Id)
		{
			this.UpdateStatus(jobPhase == 3 || jobPhase == 4);
			if (jobPhase == 4)
			{
				this.modDescriptionText.gameObject.SetActive(false);
				this.loadingMapLabelText.text = this.mapLoadingErrorString;
				this.loadingMapLabelText.gameObject.SetActive(true);
				this.loadingMapMessageText.text = this.mapLoadingErrorInvalidModFile;
				this.loadingMapMessageText.gameObject.SetActive(true);
			}
		}
	}

	// Token: 0x06003E64 RID: 15972 RVA: 0x0014CB48 File Offset: 0x0014AD48
	private void Update()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		string text;
		if (this.GetModId().IsValid() && ModInstallationManagement.CurrentOperationOnMod != null && ModInstallationManagement.CurrentOperationOnMod.Id == this.GetModId() && ModInstallationManagement.CurrentOperationOnMod.File.State != 5 && CustomMapsDetailsScreen.modStatusStrings.TryGetValue(ModInstallationManagement.CurrentOperationOnMod.File.State, ref text))
		{
			float num = this.currentMapMod.File.FileStateProgress * 100f;
			this.modStatusText.text = text + string.Format(" {0}%", Mathf.RoundToInt(num));
		}
	}

	// Token: 0x06003E65 RID: 15973 RVA: 0x0014CC00 File Offset: 0x0014AE00
	public void RetrieveModFromModIO(long id, bool forceUpdate = false, Action<Error, Mod> callback = null)
	{
		if (this.hasModProfile && this.GetModId()._id == id)
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.pendingModId = id;
		ModIOManager.GetMod(new ModId(id), forceUpdate, (callback != null) ? callback : new Action<Error, Mod>(this.OnProfileReceived));
	}

	// Token: 0x06003E66 RID: 15974 RVA: 0x0014CC54 File Offset: 0x0014AE54
	public void SetModProfile(Mod mod)
	{
		if (mod.Id != ModId.Null)
		{
			this.pendingModId = 0L;
			this.currentMapMod = mod;
			this.hasModProfile = true;
			this.currentMapMod.OnModUpdated += new Action(this.OnModUpdated);
			this.isFavorite = ModIOManager.IsModFavorited(mod.Id);
			this.favoriteToggleButton.SetButtonActive(this.isFavorite);
			this.UpdateMapDetails(true);
		}
	}

	// Token: 0x06003E67 RID: 15975 RVA: 0x0014CCCC File Offset: 0x0014AECC
	public override void PressButton(CustomMapKeyboardBinding buttonPressed)
	{
		if (Time.time < this.showTime + this.activationTime)
		{
			return;
		}
		GTDev.Log<string>("[CustomMapsDetailsScreen::PressButton] Is Driver: " + CustomMapsTerminal.IsDriver.ToString() + ", Button Pressed: " + buttonPressed.ToString(), null);
		if (!base.isActiveAndEnabled || !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (buttonPressed == CustomMapKeyboardBinding.goback)
		{
			if (CustomMapManager.IsLoading())
			{
				return;
			}
			if (CustomMapManager.IsUnloading())
			{
				return;
			}
			if (this.mapLoadError)
			{
				this.mapLoadError = false;
				this.loadingMapMessageText.fontSize = 40f;
				CustomMapManager.ClearRoomMap();
				this.ResetToDefaultView();
				return;
			}
			if (CustomMapLoader.IsMapLoaded() || CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				string text;
				if (!this.CanChangeMapState(false, out text))
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.errorText.text = text;
					this.errorText.gameObject.SetActive(true);
					return;
				}
				this.UnloadMap();
				return;
			}
			else
			{
				if (ModInstallationManagement.CurrentOperationOnMod != null && ModInstallationManagement.CurrentOperationOnMod.Id == this.GetModId())
				{
					GTDev.Log<string>("[CustomMapsDetailsScreen::PressButton] Attempted to go back while this mod is " + ModInstallationManagement.CurrentOperationOnMod.File.State.ToString() + ", ignoring...", null);
					return;
				}
				CustomMapsTerminal.ReturnFromDetailsScreen();
				this.hasModProfile = false;
				this.currentMapMod.OnModUpdated -= new Action(this.OnModUpdated);
				this.currentMapMod = null;
				return;
			}
		}
		else
		{
			if (!this.hasModProfile || this.mapLoadError)
			{
				bool flag = this.mapLoadError;
				return;
			}
			if (buttonPressed == CustomMapKeyboardBinding.option3)
			{
				this.RefreshCurrentMapMod();
				return;
			}
			if (buttonPressed == CustomMapKeyboardBinding.map)
			{
				if (this.currentMapMod == null || CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.IsUnloading())
				{
					return;
				}
				this.errorText.gameObject.SetActive(false);
				this.errorText.text = "";
				this.loadingMapLabelText.gameObject.SetActive(false);
				this.loadingMapMessageText.gameObject.SetActive(false);
				this.modDescriptionText.gameObject.SetActive(true);
				ModIOManager.RefreshUserProfile(delegate(bool result)
				{
					if (this.currentMapMod.IsSubscribed)
					{
						ModIOManager.UnsubscribeFromMod(this.GetModId(), delegate(Error error)
						{
							if (!error)
							{
								this.UpdateMapDetails(false);
							}
						});
						return;
					}
					ModIOManager.SubscribeToMod(this.GetModId(), delegate(Error error)
					{
						if (!error)
						{
							this.UpdateMapDetails(false);
						}
					});
				}, false);
			}
			if (buttonPressed == CustomMapKeyboardBinding.enter && !CustomMapManager.IsLoading() && !CustomMapManager.IsUnloading() && !CustomMapLoader.IsMapLoaded() && this.currentMapMod != null && !this.IsCurrentModHidden())
			{
				if (this.currentMapMod.File.State == 5)
				{
					string text2;
					if (!this.CanChangeMapState(true, out text2))
					{
						this.modDescriptionText.gameObject.SetActive(false);
						this.errorText.text = text2;
						this.errorText.gameObject.SetActive(true);
					}
					else
					{
						this.LoadMap();
					}
				}
				else
				{
					ModFileState state = this.currentMapMod.File.State;
					if (state == 1 || state == null)
					{
						ModIOManager.DownloadMod(this.GetModId(), delegate(bool modDownloadStarted)
						{
							if (modDownloadStarted)
							{
								this.UpdateStatus(false);
							}
						});
					}
					else
					{
						Debug.Log(string.Format("[CustomMapsDetailsScreen::PressButton] mod has status: {0}, ", this.currentMapMod.File.State) + "cannot start download or attempt to load map...");
					}
				}
			}
			if (buttonPressed == CustomMapKeyboardBinding.fav && this.currentMapMod != null)
			{
				if (this.isFavorite)
				{
					ModIOManager.RemoveFavorite(this.currentMapMod.Id);
					this.isFavorite = ModIOManager.IsModFavorited(this.currentMapMod.Id);
					this.favoriteToggleButton.SetButtonActive(this.isFavorite);
					if (this.IsCurrentModHidden())
					{
						this.favoriteToggleButton.gameObject.SetActive(false);
					}
				}
				else if (!this.IsCurrentModHidden())
				{
					ModIOManager.AddFavorite(this.currentMapMod.Id, delegate(Error error)
					{
						this.isFavorite = ModIOManager.IsModFavorited(this.currentMapMod.Id);
						this.favoriteToggleButton.SetButtonActive(this.isFavorite);
					});
				}
			}
			if (buttonPressed == CustomMapKeyboardBinding.delete)
			{
				if (CustomMapManager.IsLoading() || CustomMapManager.IsUnloading() || CustomMapLoader.IsMapLoaded())
				{
					return;
				}
				Mod currentMapMod = this.currentMapMod;
				bool flag2;
				if (currentMapMod != null)
				{
					Modfile file = currentMapMod.File;
					if (file != null)
					{
						ModFileState state = file.State;
						if (state == 1 || state == 5)
						{
							flag2 = true;
							goto IL_3EC;
						}
					}
				}
				flag2 = false;
				IL_3EC:
				if (flag2)
				{
					this.currentMapMod.UninstallOtherUserMod(true);
					this.UpdateStatus(false);
				}
			}
			if (buttonPressed == CustomMapKeyboardBinding.rateUp)
			{
				this.currentMapMod.RateMod((this.currentMapMod.CurrentUserRating == 1) ? 0 : 1);
			}
			if (buttonPressed == CustomMapKeyboardBinding.rateDown)
			{
				this.currentMapMod.RateMod((this.currentMapMod.CurrentUserRating == -1) ? 0 : -1);
			}
			return;
		}
	}

	// Token: 0x06003E68 RID: 15976 RVA: 0x0014D124 File Offset: 0x0014B324
	private void RefreshCurrentMapMod()
	{
		if (CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.IsUnloading())
		{
			return;
		}
		if (this.hasModProfile)
		{
			long id = this.GetModId()._id;
			this.hasModProfile = false;
			this.currentMapMod.OnModUpdated -= new Action(this.OnModUpdated);
			this.currentMapMod = null;
			this.ResetToDefaultView();
			this.RetrieveModFromModIO(id, true, null);
		}
	}

	// Token: 0x06003E69 RID: 15977 RVA: 0x0014D190 File Offset: 0x0014B390
	private void OnProfileReceived(Error error, Mod mod)
	{
		if (error)
		{
			this.modDescriptionText.gameObject.SetActive(false);
			this.errorText.text = string.Format("FAILED TO RETRIEVE MOD DETAILS FOR MOD: {0}", this.GetModId());
			this.errorText.gameObject.SetActive(true);
			return;
		}
		this.SetModProfile(mod);
	}

	// Token: 0x06003E6A RID: 15978 RVA: 0x0014D1F0 File Offset: 0x0014B3F0
	private void ResetToDefaultView()
	{
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.modNameText.gameObject.SetActive(false);
		this.modCreatorLabelText.gameObject.SetActive(false);
		this.modCreatorText.gameObject.SetActive(false);
		this.modDescriptionText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.mapScreenshotImage.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.unloadPromptText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(true);
		if (CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.IsUnloading())
		{
			ModId modId;
			modId..ctor(CustomMapLoader.IsMapLoaded() ? CustomMapLoader.LoadedMapModId : (CustomMapManager.IsLoading() ? CustomMapManager.LoadingMapId : CustomMapManager.UnloadingMapId));
			if (this.hasModProfile && this.GetModId() == modId)
			{
				this.UpdateMapDetails(true);
				return;
			}
			this.RetrieveModFromModIO(modId, false, delegate(Error error, Mod mod)
			{
				this.OnProfileReceived(error, mod);
			});
			return;
		}
		else
		{
			if (CustomMapManager.GetRoomMapId() != ModId.Null)
			{
				this.OnRoomMapChanged(CustomMapManager.GetRoomMapId());
				return;
			}
			if (this.hasModProfile)
			{
				this.UpdateMapDetails(true);
			}
			return;
		}
	}

	// Token: 0x06003E6B RID: 15979 RVA: 0x0014D3A4 File Offset: 0x0014B5A4
	private void UpdateMapDetails(bool refreshScreenState = true)
	{
		if (!this.hasModProfile)
		{
			return;
		}
		if (this.IsCurrentModHidden())
		{
			this.modNameText.text = this.hiddenMapTitle;
			this.modDescriptionText.text = this.hiddenMapDesc;
			this.modCreatorLabelText.gameObject.SetActive(false);
			this.modCreatorText.text = "";
			this.mapScreenshotImage.sprite = this.hiddenMapLogo;
			this.mapScreenshotImage.gameObject.SetActive(true);
		}
		else
		{
			this.modNameText.text = this.currentMapMod.Name;
			this.modDescriptionText.text = this.currentMapMod.Description;
			this.modCreatorLabelText.gameObject.SetActive(true);
			this.modCreatorText.text = this.currentMapMod.Creator.Username;
			ModIOManager.GetModLogo(this.currentMapMod, new Action<Error, Texture2D>(this.OnGetModLogo));
		}
		this.UpdateStatus(false);
		if (refreshScreenState)
		{
			this.loadingText.gameObject.SetActive(false);
			this.loadingMapLabelText.gameObject.SetActive(false);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.errorText.gameObject.SetActive(false);
			this.modNameText.gameObject.SetActive(true);
			this.modDescriptionText.gameObject.SetActive(true);
			if (!this.IsCurrentModHidden())
			{
				this.modCreatorLabelText.gameObject.SetActive(true);
				this.modCreatorText.gameObject.SetActive(true);
			}
			if (CustomMapLoader.IsMapLoaded())
			{
				ModId modId;
				modId..ctor(CustomMapLoader.LoadedMapModId);
				if (this.GetModId() == modId)
				{
					this.OnMapLoadComplete_UIUpdate();
					return;
				}
				this.RetrieveModFromModIO(modId, false, delegate(Error error, Mod mod)
				{
					this.OnProfileReceived(error, mod);
				});
				return;
			}
			else
			{
				if (CustomMapManager.IsLoading() && !this.mapLoadError)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					if (!CustomMapManager.IsUnloading())
					{
						this.loadingMapLabelText.text = this.mapLoadingString + " 0%";
					}
					else
					{
						this.loadingMapLabelText.text = this.mapUnloadingString;
					}
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.IsUnloading())
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.text = this.mapUnloadingString;
					this.loadingMapLabelText.gameObject.SetActive(true);
					return;
				}
				if (CustomMapManager.GetRoomMapId() != ModId.Null)
				{
					this.ShowLoadRoomMapPrompt();
					return;
				}
				if (this.mapLoadError)
				{
					this.modDescriptionText.gameObject.SetActive(false);
					this.loadingMapLabelText.gameObject.SetActive(true);
					this.loadingMapMessageText.gameObject.SetActive(true);
				}
			}
		}
	}

	// Token: 0x06003E6C RID: 15980 RVA: 0x0014D6A8 File Offset: 0x0014B8A8
	private void OnGetModLogo(Error error, Texture2D modLogo)
	{
		if (error)
		{
			Debug.LogError(string.Format("[CustomMapsDetailsScreen::OnGetModLogo] Failed to retrieve logo for Mod {0}", this.GetModId()));
			return;
		}
		this.mapScreenshotImage.sprite = Sprite.Create(modLogo, new Rect(0f, 0f, 320f, 180f), new Vector2(0.5f, 0.5f));
		this.mapScreenshotImage.gameObject.SetActive(true);
	}

	// Token: 0x06003E6D RID: 15981 RVA: 0x0014D724 File Offset: 0x0014B924
	private Task UpdateStatus(bool errorEncountered = false)
	{
		CustomMapsDetailsScreen.<UpdateStatus>d__72 <UpdateStatus>d__;
		<UpdateStatus>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateStatus>d__.<>4__this = this;
		<UpdateStatus>d__.errorEncountered = errorEncountered;
		<UpdateStatus>d__.<>1__state = -1;
		<UpdateStatus>d__.<>t__builder.Start<CustomMapsDetailsScreen.<UpdateStatus>d__72>(ref <UpdateStatus>d__);
		return <UpdateStatus>d__.<>t__builder.Task;
	}

	// Token: 0x06003E6E RID: 15982 RVA: 0x0014D770 File Offset: 0x0014B970
	private bool CanChangeMapState(bool load, out string disallowedReason)
	{
		disallowedReason = "";
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.SessionIsPrivate)
		{
			if (!CustomMapManager.AreAllPlayersInVirtualStump())
			{
				disallowedReason = "ALL PLAYERS IN THE ROOM MUST BE INSIDE THE VIRTUAL STUMP BEFORE " + (load ? "" : "UN") + "LOADING A MAP.";
				return false;
			}
			return true;
		}
		else
		{
			if (!CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				disallowedReason = "YOU MUST BE INSIDE THE VIRTUAL STUMP TO " + (load ? "" : "UN") + "LOAD A MAP.";
				return false;
			}
			return true;
		}
	}

	// Token: 0x06003E6F RID: 15983 RVA: 0x0014D7F4 File Offset: 0x0014B9F4
	private void LoadMap()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.modStatusText.gameObject.SetActive(false);
		this.modSubscriptionStatusText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom && !NetworkSystem.Instance.SessionIsPrivate)
		{
			NetworkSystem.Instance.ReturnToSinglePlayer();
		}
		this.deleteButton.gameObject.SetActive(false);
		this.subscriptionToggleButton.gameObject.SetActive(false);
		this.networkObject.LoadMapSynced(this.GetModId());
	}

	// Token: 0x06003E70 RID: 15984 RVA: 0x0014D8B1 File Offset: 0x0014BAB1
	private void UnloadMap()
	{
		this.networkObject.UnloadMapSynced();
	}

	// Token: 0x06003E71 RID: 15985 RVA: 0x0014D8BE File Offset: 0x0014BABE
	public void OnMapLoadComplete(bool success)
	{
		if (success)
		{
			this.OnMapLoadComplete_UIUpdate();
		}
	}

	// Token: 0x06003E72 RID: 15986 RVA: 0x0014D8CC File Offset: 0x0014BACC
	private void OnMapLoadComplete_UIUpdate()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(true);
		this.unloadPromptText.gameObject.SetActive(true);
	}

	// Token: 0x06003E73 RID: 15987 RVA: 0x0014D950 File Offset: 0x0014BB50
	private void OnMapUnloaded()
	{
		this.mapLoadError = false;
		this.loadingMapMessageText.fontSize = 40f;
		this.UpdateMapDetails(true);
	}

	// Token: 0x06003E74 RID: 15988 RVA: 0x0014D970 File Offset: 0x0014BB70
	private void OnRoomMapChanged(ModId roomMapID)
	{
		if (roomMapID == ModId.Null)
		{
			this.UpdateMapDetails(true);
			return;
		}
		if (this.GetModId() != roomMapID)
		{
			this.RetrieveModFromModIO(roomMapID, false, new Action<Error, Mod>(this.OnRoomMapRetrieved));
			return;
		}
		this.ShowLoadRoomMapPrompt();
	}

	// Token: 0x06003E75 RID: 15989 RVA: 0x0014D9C0 File Offset: 0x0014BBC0
	private void OnRoomMapRetrieved(Error error, Mod mod)
	{
		this.OnProfileReceived(error, mod);
		if (!error)
		{
			this.ShowLoadRoomMapPrompt();
		}
	}

	// Token: 0x06003E76 RID: 15990 RVA: 0x0014D9D8 File Offset: 0x0014BBD8
	private void ShowLoadRoomMapPrompt()
	{
		if (CustomMapManager.IsUnloading() || CustomMapManager.IsLoading() || CustomMapLoader.IsMapLoaded(this.GetModId()))
		{
			return;
		}
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(false);
		this.unloadPromptText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		if (this.IsCurrentModHidden())
		{
			this.hiddenRoomMapText.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003E77 RID: 15991 RVA: 0x0014DA80 File Offset: 0x0014BC80
	public void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
	{
		if (loadStatus != MapLoadStatus.None)
		{
			this.mapLoadError = false;
			this.loadingMapMessageText.fontSize = 40f;
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.modDescriptionText.gameObject.SetActive(false);
		}
		switch (loadStatus)
		{
		case MapLoadStatus.Downloading:
			this.loadingMapLabelText.text = this.mapAutoDownloadingString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Loading:
			this.loadingMapLabelText.text = this.mapLoadingString + " " + progress.ToString() + "%";
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.text = message;
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		case MapLoadStatus.Unloading:
			this.mapReadyText.gameObject.SetActive(false);
			this.unloadPromptText.gameObject.SetActive(false);
			this.loadingMapLabelText.text = this.mapUnloadingString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadingMapMessageText.text = "";
			return;
		case MapLoadStatus.Error:
			this.mapLoadError = true;
			this.loadingMapLabelText.text = this.mapLoadingErrorString;
			this.loadingMapLabelText.gameObject.SetActive(true);
			if (CustomMapsTerminal.IsDriver)
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorDriverString;
			}
			else
			{
				this.loadingMapMessageText.text = message + "\n" + this.mapLoadingErrorNonDriverString;
			}
			if (this.loadingMapMessageText.text.Length > 150)
			{
				this.loadingMapMessageText.fontSize = 30f;
			}
			else
			{
				this.loadingMapMessageText.fontSize = 40f;
			}
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003E78 RID: 15992 RVA: 0x0014DC96 File Offset: 0x0014BE96
	public ModId GetModId()
	{
		Mod currentMapMod = this.currentMapMod;
		if (currentMapMod == null)
		{
			return ModId.Null;
		}
		return currentMapMod.Id;
	}

	// Token: 0x06003E79 RID: 15993 RVA: 0x0014DCAD File Offset: 0x0014BEAD
	public bool IsCurrentModHidden()
	{
		return this.hasModProfile && (this.currentMapMod.Creator == null || (!ModIOManager.IsLoggedIn() && this.currentMapMod.IsHidden()));
	}

	// Token: 0x06003E7B RID: 15995 RVA: 0x0014DDD4 File Offset: 0x0014BFD4
	// Note: this type is marked as 'beforefieldinit'.
	static CustomMapsDetailsScreen()
	{
		Dictionary<ModFileState, string> dictionary = new Dictionary<ModFileState, string>();
		dictionary.Add(5, "READY");
		dictionary.Add(1, "QUEUED");
		dictionary.Add(2, "DOWNLOADING");
		dictionary.Add(4, "INSTALLING");
		dictionary.Add(7, "UNINSTALLING");
		dictionary.Add(6, "UPDATING");
		dictionary.Add(8, "ERROR");
		dictionary.Add(0, "AVAILABLE");
		CustomMapsDetailsScreen.modStatusStrings = dictionary;
	}

	// Token: 0x04004F43 RID: 20291
	[SerializeField]
	private SpriteRenderer mapScreenshotImage;

	// Token: 0x04004F44 RID: 20292
	[SerializeField]
	private Sprite hiddenMapLogo;

	// Token: 0x04004F45 RID: 20293
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x04004F46 RID: 20294
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x04004F47 RID: 20295
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x04004F48 RID: 20296
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x04004F49 RID: 20297
	[SerializeField]
	private TMP_Text modDescriptionText;

	// Token: 0x04004F4A RID: 20298
	[SerializeField]
	private TMP_Text modStatusText;

	// Token: 0x04004F4B RID: 20299
	[SerializeField]
	private TMP_Text modStatusLabelText;

	// Token: 0x04004F4C RID: 20300
	[SerializeField]
	private TMP_Text modSubscriptionStatusText;

	// Token: 0x04004F4D RID: 20301
	[SerializeField]
	private TMP_Text loadingMapLabelText;

	// Token: 0x04004F4E RID: 20302
	[SerializeField]
	private TMP_Text loadingMapMessageText;

	// Token: 0x04004F4F RID: 20303
	[SerializeField]
	private TMP_Text hiddenRoomMapText;

	// Token: 0x04004F50 RID: 20304
	[SerializeField]
	private TMP_Text mapReadyText;

	// Token: 0x04004F51 RID: 20305
	[SerializeField]
	private TMP_Text unloadPromptText;

	// Token: 0x04004F52 RID: 20306
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x04004F53 RID: 20307
	[SerializeField]
	private TMP_Text outdatedText;

	// Token: 0x04004F54 RID: 20308
	[SerializeField]
	private CustomMapsScreenButton subscriptionToggleButton;

	// Token: 0x04004F55 RID: 20309
	[SerializeField]
	private CustomMapsScreenButton favoriteToggleButton;

	// Token: 0x04004F56 RID: 20310
	[SerializeField]
	private CustomMapsScreenButton rateUpButton;

	// Token: 0x04004F57 RID: 20311
	[SerializeField]
	private CustomMapsScreenButton rateDownButton;

	// Token: 0x04004F58 RID: 20312
	[SerializeField]
	private CustomMapsScreenButton loadButton;

	// Token: 0x04004F59 RID: 20313
	[SerializeField]
	private CustomMapsScreenButton deleteButton;

	// Token: 0x04004F5A RID: 20314
	[SerializeField]
	private string modAvailableString = "AVAILABLE";

	// Token: 0x04004F5B RID: 20315
	[SerializeField]
	private string mapAutoDownloadingString = "DOWNLOADING...";

	// Token: 0x04004F5C RID: 20316
	[SerializeField]
	private string mapDownloadQueuedString = "DOWNLOAD QUEUED";

	// Token: 0x04004F5D RID: 20317
	[SerializeField]
	private string mapLoadingString = "LOADING:";

	// Token: 0x04004F5E RID: 20318
	[SerializeField]
	private string mapUnloadingString = "UNLOADING...";

	// Token: 0x04004F5F RID: 20319
	[SerializeField]
	private string mapLoadingErrorString = "ERROR:";

	// Token: 0x04004F60 RID: 20320
	[SerializeField]
	private string mapLoadingErrorDriverString = "PRESS THE 'BACK' BUTTON TO TRY AGAIN";

	// Token: 0x04004F61 RID: 20321
	[SerializeField]
	private string mapLoadingErrorNonDriverString = "LEAVE AND REJOIN THE VIRTUAL STUMP TO TRY AGAIN";

	// Token: 0x04004F62 RID: 20322
	[SerializeField]
	private string mapLoadingErrorInvalidModFile = "INSTALL FAILED DUE TO INVALID MAP FILE";

	// Token: 0x04004F63 RID: 20323
	[SerializeField]
	private VirtualStumpSerializer networkObject;

	// Token: 0x04004F64 RID: 20324
	public static Dictionary<ModFileState, string> modStatusStrings;

	// Token: 0x04004F65 RID: 20325
	[SerializeField]
	private string mapNotDownloadedString = "NOT DOWNLOADED";

	// Token: 0x04004F66 RID: 20326
	[SerializeField]
	private string mapNeedsUpdateString = "NEEDS UPDATE";

	// Token: 0x04004F67 RID: 20327
	[SerializeField]
	private string subscribeString = "SUBSCRIBE";

	// Token: 0x04004F68 RID: 20328
	[SerializeField]
	private string unsubscribeString = "UNSUBSCRIBE";

	// Token: 0x04004F69 RID: 20329
	[SerializeField]
	private string subscribedStatusString = "SUBSCRIBED";

	// Token: 0x04004F6A RID: 20330
	[SerializeField]
	private string unsubscribedStatusString = "NOT SUBSCRIBED";

	// Token: 0x04004F6B RID: 20331
	[SerializeField]
	private string loadMapString = "LOAD";

	// Token: 0x04004F6C RID: 20332
	[SerializeField]
	private string downloadMapString = "DOWNLOAD";

	// Token: 0x04004F6D RID: 20333
	[SerializeField]
	private string updateMapString = "UPDATE";

	// Token: 0x04004F6E RID: 20334
	[SerializeField]
	private string hiddenMapTitle = "HIDDEN MAP";

	// Token: 0x04004F6F RID: 20335
	[SerializeField]
	private string hiddenMapDesc = "YOU DON'T CURRENTLY HAVE ACCESS TO THIS HIDDEN MAP.\nCHECK THAT YOU'RE LOGGED IN TO THE CORRECT MOD.IO ACCOUNT.";

	// Token: 0x04004F70 RID: 20336
	private const float LOGO_WIDTH = 320f;

	// Token: 0x04004F71 RID: 20337
	private const float LOGO_HEIGHT = 180f;

	// Token: 0x04004F72 RID: 20338
	public long pendingModId;

	// Token: 0x04004F74 RID: 20340
	private bool hasModProfile;

	// Token: 0x04004F75 RID: 20341
	private bool mapLoadError;

	// Token: 0x04004F76 RID: 20342
	private bool isFavorite;
}
