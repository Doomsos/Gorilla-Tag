using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio;
using Modio.Mods;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000996 RID: 2454
public class CustomMapsDisplayScreen : CustomMapsTerminalScreen
{
	// Token: 0x170005BF RID: 1471
	// (get) Token: 0x06003E86 RID: 16006 RVA: 0x0014E357 File Offset: 0x0014C557
	// (set) Token: 0x06003E85 RID: 16005 RVA: 0x0014E34E File Offset: 0x0014C54E
	public Mod currentMapMod { get; private set; }

	// Token: 0x06003E87 RID: 16007 RVA: 0x00002789 File Offset: 0x00000989
	public override void Initialize()
	{
	}

	// Token: 0x06003E88 RID: 16008 RVA: 0x0014E360 File Offset: 0x0014C560
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
		this.ResetToDefaultView();
	}

	// Token: 0x06003E89 RID: 16009 RVA: 0x0014E4DC File Offset: 0x0014C6DC
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

	// Token: 0x06003E8A RID: 16010 RVA: 0x0014E59F File Offset: 0x0014C79F
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

	// Token: 0x06003E8B RID: 16011 RVA: 0x0014E5D8 File Offset: 0x0014C7D8
	private void OnModIOLoggedOut()
	{
		if (this.currentMapMod.IsHidden())
		{
			this.UpdateMapDetails(true);
			return;
		}
		this.UpdateStatus(false);
	}

	// Token: 0x06003E8C RID: 16012 RVA: 0x0014E5F7 File Offset: 0x0014C7F7
	private void OnModIOUserChanged(User user)
	{
		this.UpdateStatus(false);
	}

	// Token: 0x06003E8D RID: 16013 RVA: 0x0014E604 File Offset: 0x0014C804
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

	// Token: 0x06003E8E RID: 16014 RVA: 0x0014E6A8 File Offset: 0x0014C8A8
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

	// Token: 0x06003E8F RID: 16015 RVA: 0x0014E6F9 File Offset: 0x0014C8F9
	public void SetModProfile(Mod mod)
	{
		if (mod.Id != ModId.Null)
		{
			this.pendingModId = 0L;
			this.currentMapMod = mod;
			this.hasModProfile = true;
			this.UpdateMapDetails(true);
		}
	}

	// Token: 0x06003E90 RID: 16016 RVA: 0x0014E72C File Offset: 0x0014C92C
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
			this.currentMapMod = null;
			this.ResetToDefaultView();
			this.RetrieveModFromModIO(id, true, null);
		}
	}

	// Token: 0x06003E91 RID: 16017 RVA: 0x0014E780 File Offset: 0x0014C980
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

	// Token: 0x06003E92 RID: 16018 RVA: 0x0014E7E0 File Offset: 0x0014C9E0
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
		this.mapScreenshotImage.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.outdatedText.gameObject.SetActive(false);
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

	// Token: 0x06003E93 RID: 16019 RVA: 0x0014E970 File Offset: 0x0014CB70
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
			this.modCreatorText.text = this.currentMapMod.Creator.Username;
			ModIOManager.GetModLogo(this.currentMapMod, new Action<Error, Texture2D>(this.OnGetModLogo));
		}
		this.UpdateStatus(false);
		if (refreshScreenState)
		{
			this.loadingText.gameObject.SetActive(false);
			this.loadingMapLabelText.gameObject.SetActive(false);
			this.loadingMapMessageText.gameObject.SetActive(false);
			this.loadRoomMapPromptText.gameObject.SetActive(false);
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.mapReadyText.gameObject.SetActive(false);
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

	// Token: 0x06003E94 RID: 16020 RVA: 0x0014EC64 File Offset: 0x0014CE64
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

	// Token: 0x06003E95 RID: 16021 RVA: 0x0014ECE0 File Offset: 0x0014CEE0
	private Task UpdateStatus(bool errorEncountered = false)
	{
		CustomMapsDisplayScreen.<UpdateStatus>d__49 <UpdateStatus>d__;
		<UpdateStatus>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<UpdateStatus>d__.<>4__this = this;
		<UpdateStatus>d__.errorEncountered = errorEncountered;
		<UpdateStatus>d__.<>1__state = -1;
		<UpdateStatus>d__.<>t__builder.Start<CustomMapsDisplayScreen.<UpdateStatus>d__49>(ref <UpdateStatus>d__);
		return <UpdateStatus>d__.<>t__builder.Task;
	}

	// Token: 0x06003E96 RID: 16022 RVA: 0x0014ED2B File Offset: 0x0014CF2B
	public void OnMapLoadComplete(bool success)
	{
		if (success)
		{
			this.OnMapLoadComplete_UIUpdate();
		}
	}

	// Token: 0x06003E97 RID: 16023 RVA: 0x0014ED38 File Offset: 0x0014CF38
	private void OnMapLoadComplete_UIUpdate()
	{
		this.modDescriptionText.gameObject.SetActive(false);
		this.loadingMapLabelText.gameObject.SetActive(false);
		this.loadingMapMessageText.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.errorText.gameObject.SetActive(false);
		this.mapReadyText.gameObject.SetActive(true);
	}

	// Token: 0x06003E98 RID: 16024 RVA: 0x0014EDBC File Offset: 0x0014CFBC
	private void OnMapUnloaded()
	{
		this.mapLoadError = false;
		this.loadingMapMessageText.fontSize = 80f;
		this.UpdateMapDetails(true);
	}

	// Token: 0x06003E99 RID: 16025 RVA: 0x0014EDDC File Offset: 0x0014CFDC
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

	// Token: 0x06003E9A RID: 16026 RVA: 0x0014EE2C File Offset: 0x0014D02C
	private void OnRoomMapRetrieved(Error error, Mod mod)
	{
		this.OnProfileReceived(error, mod);
		if (!error)
		{
			this.ShowLoadRoomMapPrompt();
		}
	}

	// Token: 0x06003E9B RID: 16027 RVA: 0x0014EE44 File Offset: 0x0014D044
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
		this.hiddenRoomMapText.gameObject.SetActive(false);
		this.loadRoomMapPromptText.gameObject.SetActive(false);
		if (this.IsCurrentModHidden())
		{
			this.hiddenRoomMapText.gameObject.SetActive(true);
			return;
		}
		this.loadRoomMapPromptText.gameObject.SetActive(true);
	}

	// Token: 0x06003E9C RID: 16028 RVA: 0x0014EF00 File Offset: 0x0014D100
	public void OnMapLoadProgress(MapLoadStatus loadStatus, int progress, string message)
	{
		if (loadStatus != MapLoadStatus.None)
		{
			this.mapLoadError = false;
			this.loadingMapMessageText.fontSize = 80f;
			this.hiddenRoomMapText.gameObject.SetActive(false);
			this.loadRoomMapPromptText.gameObject.SetActive(false);
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
				this.loadingMapMessageText.fontSize = 60f;
			}
			else
			{
				this.loadingMapMessageText.fontSize = 80f;
			}
			this.loadingMapMessageText.gameObject.SetActive(true);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003E9D RID: 16029 RVA: 0x0014F116 File Offset: 0x0014D316
	public ModId GetModId()
	{
		Mod currentMapMod = this.currentMapMod;
		if (currentMapMod == null)
		{
			return ModId.Null;
		}
		return currentMapMod.Id;
	}

	// Token: 0x06003E9E RID: 16030 RVA: 0x0014F12D File Offset: 0x0014D32D
	public bool IsCurrentModHidden()
	{
		return this.hasModProfile && (this.currentMapMod.Creator == null || (!ModIOManager.IsLoggedIn() && this.currentMapMod.IsHidden()));
	}

	// Token: 0x04004F7C RID: 20348
	[SerializeField]
	private SpriteRenderer mapScreenshotImage;

	// Token: 0x04004F7D RID: 20349
	[SerializeField]
	private Sprite hiddenMapLogo;

	// Token: 0x04004F7E RID: 20350
	[SerializeField]
	private TMP_Text loadingText;

	// Token: 0x04004F7F RID: 20351
	[SerializeField]
	private TMP_Text modNameText;

	// Token: 0x04004F80 RID: 20352
	[SerializeField]
	private TMP_Text modCreatorLabelText;

	// Token: 0x04004F81 RID: 20353
	[SerializeField]
	private TMP_Text modCreatorText;

	// Token: 0x04004F82 RID: 20354
	[SerializeField]
	private TMP_Text modDescriptionText;

	// Token: 0x04004F83 RID: 20355
	[SerializeField]
	private TMP_Text loadingMapLabelText;

	// Token: 0x04004F84 RID: 20356
	[SerializeField]
	private TMP_Text loadingMapMessageText;

	// Token: 0x04004F85 RID: 20357
	[SerializeField]
	private TMP_Text loadRoomMapPromptText;

	// Token: 0x04004F86 RID: 20358
	[SerializeField]
	private TMP_Text hiddenRoomMapText;

	// Token: 0x04004F87 RID: 20359
	[SerializeField]
	private TMP_Text mapReadyText;

	// Token: 0x04004F88 RID: 20360
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x04004F89 RID: 20361
	[SerializeField]
	private TMP_Text outdatedText;

	// Token: 0x04004F8A RID: 20362
	[SerializeField]
	private string mapAutoDownloadingString = "DOWNLOADING...";

	// Token: 0x04004F8B RID: 20363
	[SerializeField]
	private string mapLoadingString = "LOADING:";

	// Token: 0x04004F8C RID: 20364
	[SerializeField]
	private string mapUnloadingString = "UNLOADING...";

	// Token: 0x04004F8D RID: 20365
	[SerializeField]
	private string mapLoadingErrorString = "ERROR:";

	// Token: 0x04004F8E RID: 20366
	[SerializeField]
	private string mapLoadingErrorDriverString = "PRESS THE 'BACK' BUTTON TO TRY AGAIN";

	// Token: 0x04004F8F RID: 20367
	[SerializeField]
	private string mapLoadingErrorNonDriverString = "LEAVE AND REJOIN THE VIRTUAL STUMP TO TRY AGAIN";

	// Token: 0x04004F90 RID: 20368
	[SerializeField]
	private string mapLoadingErrorInvalidModFile = "INSTALL FAILED DUE TO INVALID MAP FILE";

	// Token: 0x04004F91 RID: 20369
	[SerializeField]
	private string mapNotDownloadedString = "NOT DOWNLOADED";

	// Token: 0x04004F92 RID: 20370
	[SerializeField]
	private string mapNeedsUpdateString = "NEEDS UPDATE";

	// Token: 0x04004F93 RID: 20371
	[SerializeField]
	private string hiddenMapTitle = "HIDDEN MAP";

	// Token: 0x04004F94 RID: 20372
	[SerializeField]
	private string hiddenMapDesc = "YOU DON'T CURRENTLY HAVE ACCESS TO THIS HIDDEN MAP.\nCHECK THAT YOU'RE LOGGED IN TO THE CORRECT MOD.IO ACCOUNT.";

	// Token: 0x04004F95 RID: 20373
	private const float LOGO_WIDTH = 320f;

	// Token: 0x04004F96 RID: 20374
	private const float LOGO_HEIGHT = 180f;

	// Token: 0x04004F97 RID: 20375
	public long pendingModId;

	// Token: 0x04004F99 RID: 20377
	private bool hasModProfile;

	// Token: 0x04004F9A RID: 20378
	private bool mapLoadError;

	// Token: 0x04004F9B RID: 20379
	private bool isFavorite;
}
