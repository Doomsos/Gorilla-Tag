using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CosmeticRoom;
using CustomMapSupport;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion.Swimming;
using GorillaNetworking;
using GorillaNetworking.Store;
using GorillaTag.Rendering;
using GorillaTagScripts;
using GorillaTagScripts.CustomMapSupport;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using Modio;
using Modio.Mods;
using Newtonsoft.Json;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x0200093F RID: 2367
public class CustomMapLoader : MonoBehaviour, IBuildValidation
{
	// Token: 0x06003C73 RID: 15475 RVA: 0x0013F294 File Offset: 0x0013D494
	[RuntimeInitializeOnLoadMethod(1)]
	private static void InitOnLoad()
	{
		GTDev.Log<string>("CML::InitOnLoad", null);
		CustomMapLoader.instance = null;
		CustomMapLoader.hasInstance = false;
		CustomMapLoader.isLoading = false;
		CustomMapLoader.isUnloading = false;
		CustomMapLoader.runningAsyncLoad = false;
		CustomMapLoader.attemptedLoadID = 0L;
		CustomMapLoader.attemptedSceneToLoad = null;
		CustomMapLoader.shouldAbortMapLoading = false;
		CustomMapLoader.shouldAbortSceneLoad = false;
		CustomMapLoader.errorEncounteredDuringLoad = false;
		CustomMapLoader.unloadMapCallback = null;
		CustomMapLoader.cachedExceptionMessage = "";
		CustomMapLoader.mapBundle = null;
		CustomMapLoader.initialSceneNames = new List<string>();
		CustomMapLoader.initialSceneIndexes = new List<int>();
		CustomMapLoader.maxPlayersForMap = 10;
		CustomMapLoader.loadedMapModId = ModId.Null;
		CustomMapLoader.loadedMapModFileId = -1L;
		CustomMapLoader.loadedMapPackageInfo = null;
		CustomMapLoader.cachedLuauScript = null;
		CustomMapLoader.devModeEnabled = false;
		CustomMapLoader.disableHoldingHandsAllModes = false;
		CustomMapLoader.disableHoldingHandsCustomMode = false;
		CustomMapLoader.mapLoadProgressCallback = null;
		CustomMapLoader.mapLoadFinishedCallback = null;
		CustomMapLoader.zoneLoadingCoroutine = null;
		CustomMapLoader.sceneLoadedCallback = null;
		CustomMapLoader.sceneUnloadedCallback = null;
		CustomMapLoader.queuedLoadZoneRequests = new List<CustomMapLoader.LoadZoneRequest>();
		CustomMapLoader.assetBundleSceneFilePaths = null;
		CustomMapLoader.loadedSceneFilePaths = new List<string>();
		CustomMapLoader.loadedSceneNames = new List<string>();
		CustomMapLoader.loadedSceneIndexes = new List<int>();
		CustomMapLoader.leafGliderIndex = 0;
		CustomMapLoader.usingDynamicLighting = false;
		CustomMapLoader.totalObjectsInLoadingScene = 0;
		CustomMapLoader.objectsProcessedForLoadingScene = 0;
		CustomMapLoader.objectsProcessedThisFrame = 0;
		CustomMapLoader.initializePhaseTwoComponents = new List<Component>();
		CustomMapLoader.entitiesToCreate = new List<MapEntity>(Constants.aiAgentLimit);
		CustomMapLoader.lightmaps = null;
		CustomMapLoader.lightmapsToKeep = new List<Texture2D>();
		CustomMapLoader.placeholderReplacements = new List<GameObject>();
		CustomMapLoader.customMapATM = null;
		CustomMapLoader.storeCheckouts = new List<GameObject>();
		CustomMapLoader.storeDisplayStands = new List<GameObject>();
		CustomMapLoader.storeTryOnConsoles = new List<GameObject>();
		CustomMapLoader.storeTryOnAreas = new List<GameObject>();
	}

	// Token: 0x06003C74 RID: 15476 RVA: 0x0013F416 File Offset: 0x0013D616
	private void Awake()
	{
		if (CustomMapLoader.instance == null)
		{
			CustomMapLoader.instance = this;
			CustomMapLoader.hasInstance = true;
			return;
		}
		if (CustomMapLoader.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06003C75 RID: 15477 RVA: 0x0013F450 File Offset: 0x0013D650
	private void Start()
	{
		byte[] array = new byte[]
		{
			Convert.ToByte(68),
			Convert.ToByte(111),
			Convert.ToByte(110),
			Convert.ToByte(116),
			Convert.ToByte(68),
			Convert.ToByte(101),
			Convert.ToByte(115),
			Convert.ToByte(116),
			Convert.ToByte(114),
			Convert.ToByte(111),
			Convert.ToByte(121),
			Convert.ToByte(79),
			Convert.ToByte(110),
			Convert.ToByte(76),
			Convert.ToByte(111),
			Convert.ToByte(97),
			Convert.ToByte(100)
		};
		this.dontDestroyOnLoadSceneName = Encoding.ASCII.GetString(array);
		if (this.publicJoinTrigger != null)
		{
			this.publicJoinTrigger.SetActive(false);
		}
	}

	// Token: 0x06003C76 RID: 15478 RVA: 0x0013F542 File Offset: 0x0013D742
	public static void Initialize(Action<MapLoadStatus, int, string> onLoadProgress, Action<bool> onLoadFinished, Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		CustomMapLoader.mapLoadProgressCallback = onLoadProgress;
		CustomMapLoader.mapLoadFinishedCallback = onLoadFinished;
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
	}

	// Token: 0x06003C77 RID: 15479 RVA: 0x0013F55C File Offset: 0x0013D75C
	public static void LoadMap(long mapModId, string mapFilePath)
	{
		if (!CustomMapLoader.hasInstance)
		{
			return;
		}
		if (CustomMapLoader.isLoading)
		{
			return;
		}
		if (CustomMapLoader.isUnloading)
		{
			Action<bool> action = CustomMapLoader.mapLoadFinishedCallback;
			if (action == null)
			{
				return;
			}
			action.Invoke(false);
			return;
		}
		else
		{
			if (!CustomMapLoader.IsMapLoaded(mapModId))
			{
				GorillaNetworkJoinTrigger.DisableTriggerJoins();
				CustomMapLoader.CanLoadEntities = false;
				CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadAssetBundle(mapModId, mapFilePath, new Action<bool, bool>(CustomMapLoader.OnAssetBundleLoaded)));
				return;
			}
			Action<bool> action2 = CustomMapLoader.mapLoadFinishedCallback;
			if (action2 == null)
			{
				return;
			}
			action2.Invoke(true);
			return;
		}
	}

	// Token: 0x06003C78 RID: 15480 RVA: 0x0013F5DA File Offset: 0x0013D7DA
	public static bool OpenDoorToMap()
	{
		if (!CustomMapLoader.hasInstance)
		{
			return false;
		}
		if (CustomMapLoader.instance.accessDoor != null)
		{
			CustomMapLoader.instance.accessDoor.OpenDoor();
			return true;
		}
		return false;
	}

	// Token: 0x06003C79 RID: 15481 RVA: 0x0013F60D File Offset: 0x0013D80D
	private static IEnumerator LoadAssetBundle(long mapModID, string packageInfoFilePath, Action<bool, bool> OnLoadComplete)
	{
		CustomMapLoader.isLoading = true;
		CustomMapLoader.errorEncounteredDuringLoad = false;
		CustomMapLoader.attemptedLoadID = mapModID;
		CustomMapLoader.refreshReviveStations = false;
		CustomMapLoader.instance.ghostReactorManager.reactor.RefreshReviveStations(false);
		Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
		if (action != null)
		{
			action.Invoke(MapLoadStatus.Loading, 1, "CACHING LIGHTMAP DATA");
		}
		CustomMapLoader.CacheLightmaps();
		Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
		if (action2 != null)
		{
			action2.Invoke(MapLoadStatus.Loading, 2, "LOADING PACKAGE INFO");
		}
		try
		{
			CustomMapLoader.loadedMapPackageInfo = CustomMapLoader.GetPackageInfo(packageInfoFilePath);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("[CML.LoadAssetBundle] GetPackageInfo Exception: {0}", ex));
			Action<MapLoadStatus, int, string> action3 = CustomMapLoader.mapLoadProgressCallback;
			if (action3 != null)
			{
				action3.Invoke(MapLoadStatus.Error, 0, ex.ToString());
			}
			OnLoadComplete.Invoke(false, false);
			yield break;
		}
		if (CustomMapLoader.loadedMapPackageInfo == null)
		{
			Action<MapLoadStatus, int, string> action4 = CustomMapLoader.mapLoadProgressCallback;
			if (action4 != null)
			{
				action4.Invoke(MapLoadStatus.Error, 0, "FAILED TO READ FILE AT " + packageInfoFilePath);
			}
			OnLoadComplete.Invoke(false, false);
			yield break;
		}
		CustomMapLoader.LoadInitialSceneNames();
		Action<MapLoadStatus, int, string> action5 = CustomMapLoader.mapLoadProgressCallback;
		if (action5 != null)
		{
			action5.Invoke(MapLoadStatus.Loading, 3, "PACKAGE INFO LOADED");
		}
		string text = Path.GetDirectoryName(packageInfoFilePath) + "/" + CustomMapLoader.loadedMapPackageInfo.pcFileName;
		Action<MapLoadStatus, int, string> action6 = CustomMapLoader.mapLoadProgressCallback;
		if (action6 != null)
		{
			action6.Invoke(MapLoadStatus.Loading, 4, "LOADING MAP ASSET BUNDLE");
		}
		AssetBundleCreateRequest loadBundleRequest = AssetBundle.LoadFromFileAsync(text);
		yield return loadBundleRequest;
		CustomMapLoader.mapBundle = loadBundleRequest.assetBundle;
		if (CustomMapLoader.shouldAbortMapLoading || CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(-1);
			OnLoadComplete.Invoke(false, true);
			yield break;
		}
		if (CustomMapLoader.mapBundle == null)
		{
			Action<MapLoadStatus, int, string> action7 = CustomMapLoader.mapLoadProgressCallback;
			if (action7 != null)
			{
				action7.Invoke(MapLoadStatus.Error, 0, "CUSTOM MAP ASSET BUNDLE FAILED TO LOAD");
			}
			OnLoadComplete.Invoke(false, false);
			yield break;
		}
		if (!CustomMapLoader.mapBundle.isStreamedSceneAssetBundle)
		{
			CustomMapLoader.mapBundle.Unload(true);
			Action<MapLoadStatus, int, string> action8 = CustomMapLoader.mapLoadProgressCallback;
			if (action8 != null)
			{
				action8.Invoke(MapLoadStatus.Error, 0, "AssetBundle does not contain a Unity Scene file");
			}
			OnLoadComplete.Invoke(false, false);
			yield break;
		}
		Action<MapLoadStatus, int, string> action9 = CustomMapLoader.mapLoadProgressCallback;
		if (action9 != null)
		{
			action9.Invoke(MapLoadStatus.Loading, 10, "MAP ASSET BUNDLE LOADED");
		}
		CustomMapLoader.assetBundleSceneFilePaths = CustomMapLoader.mapBundle.GetAllScenePaths();
		if (CustomMapLoader.assetBundleSceneFilePaths.Length == 0)
		{
			CustomMapLoader.mapBundle.Unload(true);
			Action<MapLoadStatus, int, string> action10 = CustomMapLoader.mapLoadProgressCallback;
			if (action10 != null)
			{
				action10.Invoke(MapLoadStatus.Error, 0, "AssetBundle does not contain a Unity Scene file");
			}
			OnLoadComplete.Invoke(false, false);
			yield break;
		}
		foreach (string text2 in CustomMapLoader.assetBundleSceneFilePaths)
		{
			if (text2.Equals(CustomMapLoader.instance.dontDestroyOnLoadSceneName, 5))
			{
				CustomMapLoader.mapBundle.Unload(true);
				Action<MapLoadStatus, int, string> action11 = CustomMapLoader.mapLoadProgressCallback;
				if (action11 != null)
				{
					action11.Invoke(MapLoadStatus.Error, 0, "Map name is " + text2 + " this is an invalid name");
				}
				OnLoadComplete.Invoke(false, false);
				yield break;
			}
		}
		OnLoadComplete.Invoke(true, false);
		yield break;
	}

	// Token: 0x06003C7A RID: 15482 RVA: 0x0013F62C File Offset: 0x0013D82C
	private static void LoadInitialSceneNames()
	{
		CustomMapLoader.initialSceneNames.Clear();
		if (CustomMapLoader.loadedMapPackageInfo != null)
		{
			if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion <= 2)
			{
				CustomMapLoader.initialSceneNames.Add(CustomMapLoader.loadedMapPackageInfo.initialScene);
				return;
			}
			if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion > 2)
			{
				CustomMapLoader.initialSceneNames.AddRange(CustomMapLoader.loadedMapPackageInfo.initialScenes);
			}
		}
	}

	// Token: 0x06003C7B RID: 15483 RVA: 0x0013F690 File Offset: 0x0013D890
	private static void OnAssetBundleLoaded(bool loadSucceeded, bool loadAborted)
	{
		if (loadAborted)
		{
			return;
		}
		if (loadSucceeded)
		{
			CustomMapLoader.loadedMapModId = CustomMapLoader.attemptedLoadID;
			CustomMapLoader.loadedMapModFileId = 0L;
			ModIOManager.GetMod(new ModId(CustomMapLoader.loadedMapModId), false, delegate(Error error, Mod mod)
			{
				if (!error && mod != null && mod.File != null)
				{
					CustomMapLoader.loadedMapModFileId = mod.File.Id;
				}
			});
			foreach (string text in CustomMapLoader.initialSceneNames)
			{
				int num = -1;
				if (text != string.Empty)
				{
					num = CustomMapLoader.GetSceneIndex(text);
				}
				if (num == -1)
				{
					GTDev.LogError<string>("[CustomMapLoader::OnAssetBundleLoaded] Encountered invalid initial scene, could not get scene index for: \"" + text + "\"", null);
				}
				else
				{
					CustomMapLoader.initialSceneIndexes.Add(num);
				}
			}
			if (CustomMapLoader.initialSceneIndexes.Count == 0)
			{
				if (CustomMapLoader.assetBundleSceneFilePaths.Length == 1)
				{
					GTDev.LogWarning<string>("[CustomMapLoader::OnAssetBundleLoaded] Asset Bundle only contains 1 Scene, but it isn't marked as an initial scene. Treating it as an initial scene...", null);
					CustomMapLoader.initialSceneIndexes.Add(0);
				}
				else if (CustomMapLoader.mapBundle != null)
				{
					string text2 = "";
					if (CustomMapLoader.assetBundleSceneFilePaths.Length == 0)
					{
						text2 = "MAP ASSET BUNDLE CONTAINS NO VALID SCENES.";
					}
					else if (CustomMapLoader.assetBundleSceneFilePaths.Length > 1)
					{
						text2 = "MAP ASSET BUNDLE CONTAINS MULTIPLE SCENES, BUT NONE ARE SET AS INITIAL SCENE.";
					}
					Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
					if (action != null)
					{
						action.Invoke(MapLoadStatus.Error, 0, text2);
					}
					CustomMapLoader.OnInitialLoadComplete(false, true);
				}
			}
			CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadInitialScenesCoroutine(CustomMapLoader.initialSceneIndexes.ToArray()));
		}
	}

	// Token: 0x06003C7C RID: 15484 RVA: 0x0013F804 File Offset: 0x0013DA04
	private static IEnumerator LoadInitialScenesCoroutine(int[] sceneIndexes)
	{
		CustomMapLoader.<>c__DisplayClass99_0 CS$<>8__locals1 = new CustomMapLoader.<>c__DisplayClass99_0();
		CS$<>8__locals1.sceneIndexes = sceneIndexes;
		if (!CustomMapLoader.loadedSceneIndexes.IsNullOrEmpty<int>())
		{
			GTDev.LogError<string>("[CustomMapLoader::LoadInitialScenesCoroutine] loadedSceneIndexes is not empty, LoadInitialScenes should not be called in this case!", null);
			yield break;
		}
		int progressAmountPerScene = 89 / CS$<>8__locals1.sceneIndexes.Length;
		GTDev.Log<string>(string.Format("[CustomMapLoader::LoadInitialScenesCoroutine] loading {0} scenes...", CS$<>8__locals1.sceneIndexes.Length), null);
		CS$<>8__locals1.i = 0;
		while (CS$<>8__locals1.i < CS$<>8__locals1.sceneIndexes.Length)
		{
			CustomMapLoader.<>c__DisplayClass99_1 CS$<>8__locals2 = new CustomMapLoader.<>c__DisplayClass99_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			int num = 10 + CS$<>8__locals2.CS$<>8__locals1.i * progressAmountPerScene;
			int endingProgress = num + progressAmountPerScene;
			CS$<>8__locals2.isLastScene = (CS$<>8__locals2.CS$<>8__locals1.i == CS$<>8__locals2.CS$<>8__locals1.sceneIndexes.Length - 1);
			CS$<>8__locals2.stopLoading = false;
			CS$<>8__locals2.initialLoadAborted = false;
			yield return CustomMapLoader.LoadSceneFromAssetBundle(CS$<>8__locals2.CS$<>8__locals1.sceneIndexes[CS$<>8__locals2.CS$<>8__locals1.i], delegate(bool loadSucceeded, bool loadAborted, string loadedSceneName)
			{
				if (!loadSucceeded || loadAborted)
				{
					GTDev.Log<string>("[CustomMapLoader::LoadInitialScenesCoroutine] failed to load scene at index " + string.Format("\"{0}\", aborting initial load...", CS$<>8__locals2.CS$<>8__locals1.sceneIndexes[CS$<>8__locals2.CS$<>8__locals1.i]), null);
					CS$<>8__locals2.stopLoading = true;
					CS$<>8__locals2.initialLoadAborted = loadAborted;
					return;
				}
				if (CS$<>8__locals2.isLastScene)
				{
					CustomMapLoader.OnInitialLoadComplete(true, false);
				}
			}, true, num, endingProgress);
			if (CS$<>8__locals2.stopLoading || CustomMapLoader.shouldAbortMapLoading)
			{
				CustomMapLoader.OnInitialLoadComplete(false, CS$<>8__locals2.initialLoadAborted);
				break;
			}
			CS$<>8__locals2 = null;
			int i = CS$<>8__locals1.i;
			CS$<>8__locals1.i = i + 1;
		}
		yield break;
	}

	// Token: 0x06003C7D RID: 15485 RVA: 0x0013F814 File Offset: 0x0013DA14
	private static void OnInitialLoadComplete(bool loadSucceeded, bool loadAborted)
	{
		if (loadAborted || !loadSucceeded)
		{
			if (!loadAborted)
			{
				CustomMapLoader.instance.StartCoroutine(CustomMapLoader.AbortMapLoad());
				return;
			}
			Action<bool> action = CustomMapLoader.mapLoadFinishedCallback;
			if (action == null)
			{
				return;
			}
			action.Invoke(false);
			return;
		}
		else
		{
			if (CustomMapLoader.loadedMapPackageInfo != null && CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion >= 3)
			{
				CustomMapLoader.maxPlayersForMap = (byte)Math.Clamp(CustomMapLoader.loadedMapPackageInfo.maxPlayers, 1, 10);
				if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion >= 5)
				{
					CustomMapModeSelector.SetAvailableGameModes(CustomMapLoader.loadedMapPackageInfo.availableGameModes, CustomMapLoader.loadedMapPackageInfo.defaultGameMode);
					if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
					{
						if (GameMode.ActiveGameMode.IsNull())
						{
							GameModeType defaultGameMode = (GameModeType)CustomMapLoader.loadedMapPackageInfo.defaultGameMode;
							GameMode.ChangeGameMode(defaultGameMode.ToString());
						}
						else if (GameMode.ActiveGameMode.GameType() != (GameModeType)CustomMapLoader.loadedMapPackageInfo.defaultGameMode)
						{
							GameModeType defaultGameMode = (GameModeType)CustomMapLoader.loadedMapPackageInfo.defaultGameMode;
							GameMode.ChangeGameMode(defaultGameMode.ToString());
						}
					}
				}
				else
				{
					List<int> list = new List<int>();
					foreach (GameModeType gameModeType in CustomMapLoader.instance.availableModesForOldMaps)
					{
						list.Add((int)gameModeType);
					}
					GameModeType gameModeType2 = CustomMapLoader.instance.defaultGameModeForNonCustomOldMaps;
					if (!CustomMapLoader.loadedMapPackageInfo.customGamemodeScript.IsNullOrEmpty())
					{
						gameModeType2 = GameModeType.Custom;
						list.Add(7);
					}
					CustomMapModeSelector.SetAvailableGameModes(list.ToArray(), (int)gameModeType2);
					if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
					{
						if (GameMode.ActiveGameMode.IsNull())
						{
							GameMode.ChangeGameMode(gameModeType2.ToString());
						}
						else if (GameMode.ActiveGameMode.GameType() != gameModeType2)
						{
							GameMode.ChangeGameMode(gameModeType2.ToString());
						}
					}
				}
				CustomMapLoader.cachedLuauScript = CustomMapLoader.loadedMapPackageInfo.customGamemodeScript;
				CustomMapLoader.devModeEnabled = CustomMapLoader.loadedMapPackageInfo.devMode;
				CustomMapLoader.disableHoldingHandsAllModes = CustomMapLoader.loadedMapPackageInfo.disableHoldingHandsAllModes;
				CustomMapLoader.disableHoldingHandsCustomMode = CustomMapLoader.loadedMapPackageInfo.disableHoldingHandsCustomMode;
				Color ambientLightDynamic;
				ambientLightDynamic..ctor(CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_R, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_G, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_B, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_A);
				if (CustomMapLoader.loadedMapPackageInfo.useUberShaderDynamicLighting)
				{
					GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
					GameLightingManager.instance.SetAmbientLightDynamic(ambientLightDynamic);
					CustomMapLoader.usingDynamicLighting = true;
				}
				VirtualStumpReturnWatch.SetWatchProperties(CustomMapLoader.loadedMapPackageInfo.GetReturnToVStumpWatchProps());
			}
			CustomMapLoader.isLoading = false;
			CustomMapLoader.CanLoadEntities = true;
			GorillaNetworkJoinTrigger.EnableTriggerJoins();
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
			if (action2 != null)
			{
				action2.Invoke(MapLoadStatus.Loading, 100, "LOAD COMPLETE");
			}
			if (CustomMapLoader.instance.publicJoinTrigger != null)
			{
				CustomMapLoader.instance.publicJoinTrigger.SetActive(true);
			}
			foreach (string text in CustomMapLoader.loadedSceneNames)
			{
				Action<string> action3 = CustomMapLoader.sceneLoadedCallback;
				if (action3 != null)
				{
					action3.Invoke(text);
				}
			}
			Action<bool> action4 = CustomMapLoader.mapLoadFinishedCallback;
			if (action4 == null)
			{
				return;
			}
			action4.Invoke(true);
			return;
		}
	}

	// Token: 0x06003C7E RID: 15486 RVA: 0x0013FB84 File Offset: 0x0013DD84
	private static IEnumerator LoadScenesCoroutine(int[] sceneIndexes, Action<bool, bool, List<string>> loadCompleteCallback = null)
	{
		CustomMapLoader.<>c__DisplayClass101_0 CS$<>8__locals1 = new CustomMapLoader.<>c__DisplayClass101_0();
		CS$<>8__locals1.loadCompleteCallback = loadCompleteCallback;
		if (sceneIndexes.IsNullOrEmpty<int>())
		{
			Action<bool, bool, List<string>> loadCompleteCallback2 = CS$<>8__locals1.loadCompleteCallback;
			if (loadCompleteCallback2 != null)
			{
				loadCompleteCallback2.Invoke(false, false, null);
			}
			yield break;
		}
		CustomMapLoader.isLoading = true;
		CS$<>8__locals1.successfullyLoadedSceneNames = new List<string>();
		CS$<>8__locals1.successfullyLoadedAllScenes = true;
		int num;
		for (int i = 0; i < sceneIndexes.Length; i = num + 1)
		{
			CustomMapLoader.<>c__DisplayClass101_1 CS$<>8__locals2 = new CustomMapLoader.<>c__DisplayClass101_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			if (CustomMapLoader.loadedSceneIndexes.Contains(sceneIndexes[i]))
			{
				GTDev.LogWarning<string>("[CustomMapLoader::LoadScenesCoroutine] Cannot load scene " + string.Format("{0}:\"{1}\" because it's already loaded!", sceneIndexes[i], CustomMapLoader.assetBundleSceneFilePaths[sceneIndexes[i]]), null);
			}
			else
			{
				CS$<>8__locals2.shouldAbortLoad = false;
				CS$<>8__locals2.isLastScene = (i == sceneIndexes.Length - 1);
				yield return CustomMapLoader.LoadSceneFromAssetBundle(sceneIndexes[i], delegate(bool loadSucceeded, bool loadAborted, string loadedSceneName)
				{
					if (!loadSucceeded || loadAborted)
					{
						CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedAllScenes = false;
					}
					else
					{
						Action<string> action = CustomMapLoader.sceneLoadedCallback;
						if (action != null)
						{
							action.Invoke(loadedSceneName);
						}
						CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedSceneNames.Add(loadedSceneName);
					}
					if (loadAborted)
					{
						CS$<>8__locals2.shouldAbortLoad = true;
						return;
					}
					if (CS$<>8__locals2.isLastScene)
					{
						Action<bool, bool, List<string>> loadCompleteCallback4 = CS$<>8__locals2.CS$<>8__locals1.loadCompleteCallback;
						if (loadCompleteCallback4 == null)
						{
							return;
						}
						loadCompleteCallback4.Invoke(CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedAllScenes, false, CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedSceneNames);
					}
				}, false, 10, 90);
				if (CS$<>8__locals2.shouldAbortLoad)
				{
					CustomMapLoader.isLoading = false;
					Action<bool, bool, List<string>> loadCompleteCallback3 = CS$<>8__locals2.CS$<>8__locals1.loadCompleteCallback;
					if (loadCompleteCallback3 == null)
					{
						break;
					}
					loadCompleteCallback3.Invoke(false, true, CS$<>8__locals2.CS$<>8__locals1.successfullyLoadedSceneNames);
					break;
				}
				else
				{
					CS$<>8__locals2 = null;
				}
			}
			num = i;
		}
		CustomMapLoader.isLoading = false;
		yield break;
	}

	// Token: 0x06003C7F RID: 15487 RVA: 0x0013FB9A File Offset: 0x0013DD9A
	private static IEnumerator LoadSceneFromAssetBundle(int sceneIndex, Action<bool, bool, string> OnLoadComplete, bool useProgressCallback = false, int startingProgress = 10, int endingProgress = 90)
	{
		int progressAmount = endingProgress - startingProgress;
		int currentProgress = startingProgress;
		CustomMapLoader.refreshReviveStations = false;
		LoadSceneParameters loadSceneParameters = default(LoadSceneParameters);
		loadSceneParameters.loadSceneMode = 1;
		loadSceneParameters.localPhysicsMode = 0;
		LoadSceneParameters loadSceneParameters2 = loadSceneParameters;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete.Invoke(false, true, "");
			yield break;
		}
		CustomMapLoader.runningAsyncLoad = true;
		if (useProgressCallback)
		{
			int num = startingProgress + Mathf.RoundToInt((float)progressAmount * 0.02f);
			Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
			if (action != null)
			{
				action.Invoke(MapLoadStatus.Loading, num, "LOADING MAP SCENE");
			}
		}
		CustomMapLoader.attemptedSceneToLoad = CustomMapLoader.assetBundleSceneFilePaths[sceneIndex];
		string sceneName = CustomMapLoader.GetSceneNameFromFilePath(CustomMapLoader.attemptedSceneToLoad);
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(CustomMapLoader.attemptedSceneToLoad, loadSceneParameters2);
		yield return asyncOperation;
		CustomMapLoader.runningAsyncLoad = false;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete.Invoke(false, true, "");
			yield break;
		}
		if (useProgressCallback)
		{
			currentProgress += Mathf.RoundToInt((float)progressAmount * 0.28f);
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
			if (action2 != null)
			{
				action2.Invoke(MapLoadStatus.Loading, currentProgress, "SANITIZING MAP");
			}
		}
		GameObject[] rootGameObjects = SceneManager.GetSceneByName(sceneName).GetRootGameObjects();
		List<MapDescriptor> list = new List<MapDescriptor>();
		for (int i = 0; i < rootGameObjects.Length; i++)
		{
			MapDescriptor component = rootGameObjects[i].GetComponent<MapDescriptor>();
			if (component.IsNotNull())
			{
				list.Add(component);
			}
		}
		MapDescriptor mapDescriptor = null;
		bool flag = false;
		foreach (MapDescriptor mapDescriptor2 in list)
		{
			if (!mapDescriptor.IsNull())
			{
				flag = true;
				break;
			}
			mapDescriptor = mapDescriptor2;
		}
		if (flag)
		{
			GTDev.LogWarning<string>("[CustomMapLoader::LoadSceneFromAssetBundle] Found multiple MapDescriptor components in Scene \"" + sceneName + "\". Only the first one found will be used...", null);
		}
		if (mapDescriptor.IsNull())
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			if (useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action3 = CustomMapLoader.mapLoadProgressCallback;
				if (action3 != null)
				{
					action3.Invoke(MapLoadStatus.Error, 0, "SCENE \"" + sceneName + "\" DOES NOT CONTAIN A MAP DESCRIPTOR ON ONE OF ITS ROOT GAME OBJECTS.");
				}
			}
			OnLoadComplete.Invoke(false, false, "");
			yield break;
		}
		GameObject gameObject = mapDescriptor.gameObject;
		if (!CustomMapLoader.SanitizeObject(gameObject, gameObject))
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			if (useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action4 = CustomMapLoader.mapLoadProgressCallback;
				if (action4 != null)
				{
					action4.Invoke(MapLoadStatus.Error, 0, "MAP DESCRIPTOR GAME OBJECT ON SCENE \"" + sceneName + "\" HAS UNAPPROVED COMPONENTS ON IT");
				}
			}
			OnLoadComplete.Invoke(false, false, "");
			yield break;
		}
		if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion < 4)
		{
			foreach (TextMeshPro textMeshPro in gameObject.transform.GetComponentsInChildren<TextMeshPro>(true))
			{
				if (textMeshPro.font == null || textMeshPro.font.material == null)
				{
					textMeshPro.font = CustomMapLoader.instance.DefaultFont;
				}
			}
			foreach (TextMeshProUGUI textMeshProUGUI in gameObject.transform.GetComponentsInChildren<TextMeshProUGUI>(true))
			{
				if (textMeshProUGUI.font == null || textMeshProUGUI.font.material == null)
				{
					textMeshProUGUI.font = CustomMapLoader.instance.DefaultFont;
				}
			}
		}
		CustomMapLoader.totalObjectsInLoadingScene = 0;
		for (int l = 0; l < rootGameObjects.Length; l++)
		{
			CustomMapLoader.SanitizeObjectRecursive(rootGameObjects[l], gameObject);
		}
		CustomMapLoader.ResolveVirtualStumpColliderOverlaps(sceneName);
		if (useProgressCallback)
		{
			currentProgress += Mathf.RoundToInt((float)progressAmount * 0.2f);
			Action<MapLoadStatus, int, string> action5 = CustomMapLoader.mapLoadProgressCallback;
			if (action5 != null)
			{
				action5.Invoke(MapLoadStatus.Loading, currentProgress, "MAP SCENE LOADED");
			}
		}
		CustomMapLoader.leafGliderIndex = 0;
		yield return CustomMapLoader.FinalizeSceneLoad(mapDescriptor, useProgressCallback, currentProgress, endingProgress);
		yield return null;
		if (CustomMapLoader.shouldAbortSceneLoad)
		{
			yield return CustomMapLoader.AbortSceneLoad(sceneIndex);
			OnLoadComplete.Invoke(false, true, "");
			if (CustomMapLoader.cachedExceptionMessage.Length > 0 && useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action6 = CustomMapLoader.mapLoadProgressCallback;
				if (action6 != null)
				{
					action6.Invoke(MapLoadStatus.Error, 0, CustomMapLoader.cachedExceptionMessage);
				}
			}
			yield break;
		}
		if (CustomMapLoader.errorEncounteredDuringLoad)
		{
			OnLoadComplete.Invoke(false, false, "");
			if (CustomMapLoader.cachedExceptionMessage.Length > 0 && useProgressCallback)
			{
				Action<MapLoadStatus, int, string> action7 = CustomMapLoader.mapLoadProgressCallback;
				if (action7 != null)
				{
					action7.Invoke(MapLoadStatus.Error, 0, CustomMapLoader.cachedExceptionMessage);
				}
			}
			yield break;
		}
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action8 = CustomMapLoader.mapLoadProgressCallback;
			if (action8 != null)
			{
				action8.Invoke(MapLoadStatus.Loading, endingProgress, "FINALIZING MAP");
			}
		}
		CustomMapLoader.loadedSceneFilePaths.AddIfNew(CustomMapLoader.attemptedSceneToLoad);
		CustomMapLoader.loadedSceneNames.AddIfNew(sceneName);
		CustomMapLoader.loadedSceneIndexes.AddIfNew(sceneIndex);
		if (CustomMapLoader.refreshReviveStations)
		{
			CustomMapLoader.instance.ghostReactorManager.reactor.RefreshReviveStations(true);
		}
		OnLoadComplete.Invoke(true, false, sceneName);
		yield break;
	}

	// Token: 0x06003C80 RID: 15488 RVA: 0x0013FBC8 File Offset: 0x0013DDC8
	private static void SanitizeObjectRecursive(GameObject rootObject, GameObject mapRoot)
	{
		if (!CustomMapLoader.SanitizeObject(rootObject, mapRoot))
		{
			return;
		}
		CustomMapLoader.totalObjectsInLoadingScene++;
		for (int i = 0; i < rootObject.transform.childCount; i++)
		{
			GameObject gameObject = rootObject.transform.GetChild(i).gameObject;
			if (gameObject.IsNotNull())
			{
				CustomMapLoader.SanitizeObjectRecursive(gameObject, mapRoot);
			}
		}
	}

	// Token: 0x06003C81 RID: 15489 RVA: 0x0013FC24 File Offset: 0x0013DE24
	private static bool SanitizeObject(GameObject gameObject, GameObject mapRoot)
	{
		if (gameObject == null)
		{
			Debug.LogError("CustomMapLoader::SanitizeObject gameobject null");
			return false;
		}
		if (!CustomMapLoader.APPROVED_LAYERS.Contains(gameObject.layer))
		{
			gameObject.layer = 0;
		}
		foreach (Component component in gameObject.GetComponents<Component>())
		{
			if (component == null)
			{
				Object.DestroyImmediate(gameObject, true);
				return false;
			}
			bool flag = true;
			foreach (Type type in CustomMapLoader.componentAllowlist)
			{
				if (component.GetType() == type)
				{
					if (type == typeof(Camera))
					{
						Camera camera = (Camera)component;
						if (camera.IsNotNull() && camera.targetTexture.IsNull())
						{
							break;
						}
					}
					flag = false;
					break;
				}
			}
			if (flag)
			{
				foreach (string text in CustomMapLoader.componentTypeStringAllowList)
				{
					if (component.GetType().ToString().Contains(text))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				Object.DestroyImmediate(gameObject, true);
				return false;
			}
		}
		if (gameObject.transform.parent.IsNull() && gameObject.transform != mapRoot.transform)
		{
			gameObject.transform.SetParent(mapRoot.transform);
		}
		return true;
	}

	// Token: 0x06003C82 RID: 15490 RVA: 0x0013FDB4 File Offset: 0x0013DFB4
	private static void ResolveVirtualStumpColliderOverlaps(string sceneName)
	{
		Vector3 vector;
		vector..ctor(5.15f, 0.72f, 5.15f);
		Vector3 vector2;
		vector2..ctor(0f, 0.73f, 0f);
		float num = vector.x * 0.5f + 2f;
		GameObject gameObject = GameObject.CreatePrimitive(2);
		gameObject.transform.position = CustomMapLoader.instance.virtualStumpMesh.transform.position + vector2;
		gameObject.transform.localScale = vector;
		Collider[] array = Physics.OverlapSphere(gameObject.transform.position, num);
		if (array == null || array.Length == 0)
		{
			Object.DestroyImmediate(gameObject);
			return;
		}
		MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
		meshCollider.convex = true;
		foreach (Collider collider in array)
		{
			Vector3 vector3;
			float num2;
			if (!(collider == null) && !(collider.gameObject == gameObject) && !(collider.gameObject.scene.name != sceneName) && Physics.ComputePenetration(meshCollider, gameObject.transform.position, gameObject.transform.rotation, collider, collider.transform.position, collider.transform.rotation, ref vector3, ref num2) && !collider.isTrigger)
			{
				GTDev.Log<string>("[CustomMapLoader::ResolveVirtualStumpColliderOverlaps] Gameobject " + collider.name + " has a collider overlapping with the virtual stump. Collider will be removed", null);
				Object.DestroyImmediate(collider);
			}
		}
		Object.DestroyImmediate(gameObject);
	}

	// Token: 0x06003C83 RID: 15491 RVA: 0x0013FF3C File Offset: 0x0013E13C
	private static IEnumerator FinalizeSceneLoad(MapDescriptor sceneDescriptor, bool useProgressCallback = false, int startingProgress = 50, int endingProgress = 90)
	{
		int num = endingProgress - startingProgress;
		int num2 = startingProgress;
		if (useProgressCallback)
		{
			num2 += Mathf.RoundToInt((float)num * 0.02f);
			Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
			if (action != null)
			{
				action.Invoke(MapLoadStatus.Loading, num2, "PROCESSING ROOT MAP OBJECT");
			}
		}
		CustomMapLoader.objectsProcessedForLoadingScene = 0;
		CustomMapLoader.objectsProcessedThisFrame = 0;
		if (useProgressCallback)
		{
			num2 += Mathf.RoundToInt((float)num * 0.03f);
			Action<MapLoadStatus, int, string> action2 = CustomMapLoader.mapLoadProgressCallback;
			if (action2 != null)
			{
				action2.Invoke(MapLoadStatus.Loading, num2, "PROCESSING CHILD OBJECTS");
			}
		}
		int processChildrenEndingProgress = endingProgress - Mathf.RoundToInt((float)num * 0.02f);
		CustomMapLoader.initializePhaseTwoComponents.Clear();
		CustomMapLoader.entitiesToCreate.Clear();
		yield return CustomMapLoader.ProcessChildObjects(sceneDescriptor.gameObject, useProgressCallback, num2, processChildrenEndingProgress);
		if (CustomMapLoader.shouldAbortSceneLoad || CustomMapLoader.errorEncounteredDuringLoad)
		{
			yield break;
		}
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action3 = CustomMapLoader.mapLoadProgressCallback;
			if (action3 != null)
			{
				action3.Invoke(MapLoadStatus.Loading, processChildrenEndingProgress, "PROCESSING COMPLETE");
			}
		}
		yield return null;
		CustomMapLoader.InitializeComponentsPhaseTwo();
		CustomMapLoader.placeholderReplacements.Clear();
		if (useProgressCallback)
		{
			Action<MapLoadStatus, int, string> action4 = CustomMapLoader.mapLoadProgressCallback;
			if (action4 != null)
			{
				action4.Invoke(MapLoadStatus.Loading, endingProgress, "PROCESSING COMPLETE");
			}
		}
		if (CustomMapLoader.loadedMapPackageInfo != null && CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion < 3 && sceneDescriptor.IsInitialScene)
		{
			CustomMapLoader.maxPlayersForMap = (byte)Math.Clamp(sceneDescriptor.MaxPlayers, 1, 10);
			CustomMapLoader.cachedLuauScript = ((sceneDescriptor.CustomGamemode != null) ? sceneDescriptor.CustomGamemode.text : "");
			CustomMapLoader.devModeEnabled = sceneDescriptor.DevMode;
			CustomMapLoader.disableHoldingHandsAllModes = sceneDescriptor.DisableHoldingHandsAllGameModes;
			CustomMapLoader.disableHoldingHandsCustomMode = sceneDescriptor.DisableHoldingHandsCustomOnly;
			if (sceneDescriptor.UseUberShaderDynamicLighting)
			{
				GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
				GameLightingManager.instance.SetAmbientLightDynamic(sceneDescriptor.UberShaderAmbientDynamicLight);
				CustomMapLoader.usingDynamicLighting = true;
			}
			List<int> list = new List<int>();
			foreach (GameModeType gameModeType in CustomMapLoader.instance.availableModesForOldMaps)
			{
				list.Add((int)gameModeType);
			}
			GameModeType gameModeType2 = CustomMapLoader.instance.defaultGameModeForNonCustomOldMaps;
			if (!CustomMapLoader.cachedLuauScript.IsNullOrEmpty())
			{
				gameModeType2 = GameModeType.Custom;
				list.Add(7);
			}
			CustomMapModeSelector.SetAvailableGameModes(list.ToArray(), (int)gameModeType2);
			if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
			{
				if (GameMode.ActiveGameMode.IsNull())
				{
					GameMode.ChangeGameMode(gameModeType2.ToString());
				}
				else if (GameMode.ActiveGameMode.GameType() != gameModeType2)
				{
					GameMode.ChangeGameMode(gameModeType2.ToString());
				}
			}
			VirtualStumpReturnWatch.SetWatchProperties(sceneDescriptor.GetReturnToVStumpWatchProps());
		}
		yield break;
	}

	// Token: 0x06003C84 RID: 15492 RVA: 0x0013FF60 File Offset: 0x0013E160
	private static IEnumerator ProcessChildObjects(GameObject parent, bool useProgressCallback = false, int startingProgress = 75, int endingProgress = 90)
	{
		if (parent == null || CustomMapLoader.placeholderReplacements.Contains(parent))
		{
			yield break;
		}
		int progressAmount = endingProgress - startingProgress;
		int num3;
		for (int i = 0; i < parent.transform.childCount; i = num3 + 1)
		{
			Transform child = parent.transform.GetChild(i);
			if (!(child == null))
			{
				GameObject gameObject = child.gameObject;
				if (!(gameObject == null) && !CustomMapLoader.placeholderReplacements.Contains(gameObject))
				{
					try
					{
						CustomMapLoader.InitializeComponentsPhaseOne(gameObject);
					}
					catch (Exception ex)
					{
						CustomMapLoader.errorEncounteredDuringLoad = true;
						CustomMapLoader.cachedExceptionMessage = ex.ToString();
						Debug.LogError("[CML.LoadMap] Exception: " + ex.ToString());
						yield break;
					}
					if (gameObject.transform.childCount > 0)
					{
						yield return CustomMapLoader.ProcessChildObjects(gameObject, useProgressCallback, startingProgress, endingProgress);
						if (CustomMapLoader.shouldAbortSceneLoad || CustomMapLoader.errorEncounteredDuringLoad)
						{
							yield break;
						}
					}
					if (CustomMapLoader.shouldAbortSceneLoad)
					{
						yield break;
					}
					CustomMapLoader.objectsProcessedForLoadingScene++;
					CustomMapLoader.objectsProcessedThisFrame++;
					if (CustomMapLoader.objectsProcessedThisFrame >= CustomMapLoader.numObjectsToProcessPerFrame)
					{
						CustomMapLoader.objectsProcessedThisFrame = 0;
						if (useProgressCallback)
						{
							float num = (float)CustomMapLoader.objectsProcessedForLoadingScene / (float)CustomMapLoader.totalObjectsInLoadingScene;
							int num2 = startingProgress + Mathf.FloorToInt((float)progressAmount * num);
							Action<MapLoadStatus, int, string> action = CustomMapLoader.mapLoadProgressCallback;
							if (action != null)
							{
								action.Invoke(MapLoadStatus.Loading, num2, "PROCESSING CHILD OBJECTS");
							}
						}
						yield return null;
					}
				}
			}
			num3 = i;
		}
		yield break;
	}

	// Token: 0x06003C85 RID: 15493 RVA: 0x0013FF84 File Offset: 0x0013E184
	private static void InitializeComponentsPhaseOne(GameObject childGameObject)
	{
		CustomMapLoader.SetupCollisions(childGameObject);
		CustomMapLoader.ReplaceDataOnlyScripts(childGameObject);
		CustomMapLoader.ReplacePlaceholders(childGameObject);
		CustomMapLoader.SetupDynamicLight(childGameObject);
		CustomMapLoader.StoreMapEntity(childGameObject);
		CustomMapLoader.SetupReviveStation(childGameObject);
	}

	// Token: 0x06003C86 RID: 15494 RVA: 0x0013FFAC File Offset: 0x0013E1AC
	private static void InitializeComponentsPhaseTwo()
	{
		for (int i = 0; i < CustomMapLoader.initializePhaseTwoComponents.Count; i++)
		{
		}
		CustomMapLoader.initializePhaseTwoComponents.Clear();
		if (CustomMapLoader.entitiesToCreate.Count > 0)
		{
			for (int j = 0; j < CustomMapLoader.entitiesToCreate.Count; j++)
			{
				CustomMapLoader.entitiesToCreate[j].gameObject.SetActive(false);
			}
			CustomMapsGameManager.AddAgentsToCreate(CustomMapLoader.entitiesToCreate);
		}
	}

	// Token: 0x06003C87 RID: 15495 RVA: 0x0014001C File Offset: 0x0013E21C
	private static void SetupReviveStation(GameObject gameObject)
	{
		if (gameObject == null)
		{
			return;
		}
		CustomMapReviveStation component = gameObject.GetComponent<CustomMapReviveStation>();
		if (component == null)
		{
			return;
		}
		GameObject gameObject2 = Object.Instantiate<GameObject>(CustomMapLoader.instance.reviveStationPrefab, gameObject.transform.parent);
		if (gameObject2 == null)
		{
			return;
		}
		gameObject2.transform.position = gameObject.transform.position;
		gameObject2.transform.rotation = gameObject.transform.rotation;
		gameObject.transform.SetParent(gameObject2.transform);
		GRReviveStation component2 = gameObject2.GetComponent<GRReviveStation>();
		if (component2 == null)
		{
			return;
		}
		component2.audioSource = component.audioSource;
		if (!component.particleEffects.IsNullOrEmpty<ParticleSystem>())
		{
			component2.particleEffects = new ParticleSystem[component.particleEffects.Length];
			for (int i = 0; i < component.particleEffects.Length; i++)
			{
				component2.particleEffects[i] = component.particleEffects[i];
			}
		}
		component2.SetReviveCooldownSeconds(component.reviveCooldownSeconds);
		CustomMapLoader.refreshReviveStations = true;
	}

	// Token: 0x06003C88 RID: 15496 RVA: 0x0014011C File Offset: 0x0013E31C
	private static void SetupCollisions(GameObject gameObject)
	{
		if (gameObject == null || CustomMapLoader.placeholderReplacements.Contains(gameObject))
		{
			return;
		}
		Collider[] components = gameObject.GetComponents<Collider>();
		if (components == null)
		{
			return;
		}
		bool flag = true;
		foreach (Collider collider in components)
		{
			if (!(collider == null))
			{
				if (collider.isTrigger)
				{
					if (gameObject.layer != UnityLayer.GorillaInteractable.ToLayerIndex())
					{
						gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
						break;
					}
				}
				else
				{
					if (gameObject.layer == UnityLayer.GorillaTrigger.ToLayerIndex())
					{
						collider.isTrigger = true;
					}
					flag = false;
					if (gameObject.GetComponent<GrabbableEntity>().IsNotNull())
					{
						gameObject.layer = UnityLayer.Default.ToLayerIndex();
						return;
					}
				}
			}
		}
		if (!flag)
		{
			SurfaceOverrideSettings component = gameObject.GetComponent<SurfaceOverrideSettings>();
			GorillaSurfaceOverride gorillaSurfaceOverride = gameObject.AddComponent<GorillaSurfaceOverride>();
			if (component == null)
			{
				gorillaSurfaceOverride.overrideIndex = 0;
				return;
			}
			gorillaSurfaceOverride.overrideIndex = component.soundOverride;
			gorillaSurfaceOverride.extraVelMultiplier = component.extraVelMultiplier;
			gorillaSurfaceOverride.extraVelMaxMultiplier = component.extraVelMaxMultiplier;
			gorillaSurfaceOverride.slidePercentageOverride = component.slidePercentage;
			gorillaSurfaceOverride.disablePushBackEffect = component.disablePushBackEffect;
			Object.Destroy(component);
		}
	}

	// Token: 0x06003C89 RID: 15497 RVA: 0x0014023C File Offset: 0x0013E43C
	private static bool ValidateTeleporterDestination(Transform teleportTarget)
	{
		using (List<GameObject>.Enumerator enumerator = CustomMapLoader.storeCheckouts.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.transform.position, teleportTarget.position) < Constants.minTeleportDistFromStorePlaceholder)
				{
					return false;
				}
			}
		}
		using (List<GameObject>.Enumerator enumerator = CustomMapLoader.storeDisplayStands.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (Vector3.Distance(enumerator.Current.transform.position, teleportTarget.position) < Constants.minTeleportDistFromStorePlaceholder)
				{
					return false;
				}
			}
		}
		return !CustomMapLoader.customMapATM.IsNotNull() || Vector3.Distance(CustomMapLoader.customMapATM.transform.position, teleportTarget.position) >= Constants.minTeleportDistFromStorePlaceholder;
	}

	// Token: 0x06003C8A RID: 15498 RVA: 0x00140334 File Offset: 0x0013E534
	private static bool ValidateStorePlaceholderPosition(GameObject storePlaceholder)
	{
		foreach (Component component in CustomMapLoader.teleporters)
		{
			if (!(component == null))
			{
				List<Transform> list = null;
				if (component.GetType() == typeof(CMSMapBoundary))
				{
					CMSMapBoundary cmsmapBoundary = (CMSMapBoundary)component;
					if (cmsmapBoundary != null)
					{
						list = cmsmapBoundary.TeleportPoints;
					}
				}
				else if (component.GetType() == typeof(CMSTeleporter))
				{
					CMSTeleporter cmsteleporter = (CMSTeleporter)component;
					if (cmsteleporter != null)
					{
						list = cmsteleporter.TeleportPoints;
					}
				}
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						Transform transform = list[i];
						if (Vector3.Distance(storePlaceholder.transform.position, transform.position) < Constants.minTeleportDistFromStorePlaceholder)
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06003C8B RID: 15499 RVA: 0x00140440 File Offset: 0x0013E640
	private static void ReplaceDataOnlyScripts(GameObject gameObject)
	{
		MapBoundarySettings[] components = gameObject.GetComponents<MapBoundarySettings>();
		if (components != null)
		{
			foreach (MapBoundarySettings mapBoundarySettings in components)
			{
				bool flag = false;
				for (int j = 0; j < mapBoundarySettings.TeleportPoints.Count; j++)
				{
					if (!mapBoundarySettings.TeleportPoints[j].IsNull() && !CustomMapLoader.ValidateTeleporterDestination(mapBoundarySettings.TeleportPoints[j]))
					{
						flag = true;
						Object.Destroy(mapBoundarySettings);
						break;
					}
				}
				if (!flag)
				{
					CMSMapBoundary cmsmapBoundary = gameObject.AddComponent<CMSMapBoundary>();
					if (cmsmapBoundary != null)
					{
						cmsmapBoundary.CopyTriggerSettings(mapBoundarySettings);
						CustomMapLoader.teleporters.Add(cmsmapBoundary);
					}
					Object.Destroy(mapBoundarySettings);
				}
			}
		}
		TagZoneSettings[] components2 = gameObject.GetComponents<TagZoneSettings>();
		if (components2 != null)
		{
			foreach (TagZoneSettings tagZoneSettings in components2)
			{
				CMSTagZone cmstagZone = gameObject.AddComponent<CMSTagZone>();
				if (cmstagZone != null)
				{
					cmstagZone.CopyTriggerSettings(tagZoneSettings);
				}
				Object.Destroy(tagZoneSettings);
			}
		}
		TeleporterSettings[] components3 = gameObject.GetComponents<TeleporterSettings>();
		if (components3 != null)
		{
			foreach (TeleporterSettings teleporterSettings in components3)
			{
				bool flag2 = false;
				for (int k = 0; k < teleporterSettings.TeleportPoints.Count; k++)
				{
					if (!teleporterSettings.TeleportPoints[k].IsNull() && !CustomMapLoader.ValidateTeleporterDestination(teleporterSettings.TeleportPoints[k]))
					{
						flag2 = true;
						Object.Destroy(teleporterSettings);
						break;
					}
				}
				if (!flag2)
				{
					CMSTeleporter cmsteleporter = gameObject.AddComponent<CMSTeleporter>();
					if (cmsteleporter != null)
					{
						cmsteleporter.CopyTriggerSettings(teleporterSettings);
					}
					Object.Destroy(teleporterSettings);
				}
			}
		}
		ObjectActivationTriggerSettings[] components4 = gameObject.GetComponents<ObjectActivationTriggerSettings>();
		if (components4 != null)
		{
			foreach (ObjectActivationTriggerSettings objectActivationTriggerSettings in components4)
			{
				CMSObjectActivationTrigger cmsobjectActivationTrigger = gameObject.AddComponent<CMSObjectActivationTrigger>();
				if (cmsobjectActivationTrigger != null)
				{
					cmsobjectActivationTrigger.CopyTriggerSettings(objectActivationTriggerSettings);
				}
				Object.Destroy(objectActivationTriggerSettings);
			}
		}
		LuauTriggerSettings[] components5 = gameObject.GetComponents<LuauTriggerSettings>();
		if (components5 != null)
		{
			foreach (LuauTriggerSettings luauTriggerSettings in components5)
			{
				CMSLuau cmsluau = gameObject.AddComponent<CMSLuau>();
				if (cmsluau != null)
				{
					cmsluau.CopyTriggerSettings(luauTriggerSettings);
				}
				Object.Destroy(luauTriggerSettings);
			}
		}
		PlayAnimationTriggerSettings[] components6 = gameObject.GetComponents<PlayAnimationTriggerSettings>();
		if (components6 != null)
		{
			foreach (PlayAnimationTriggerSettings playAnimationTriggerSettings in components6)
			{
				CMSPlayAnimationTrigger cmsplayAnimationTrigger = gameObject.AddComponent<CMSPlayAnimationTrigger>();
				if (cmsplayAnimationTrigger != null)
				{
					cmsplayAnimationTrigger.CopyTriggerSettings(playAnimationTriggerSettings);
				}
				Object.Destroy(playAnimationTriggerSettings);
			}
		}
		LoadZoneSettings[] components7 = gameObject.GetComponents<LoadZoneSettings>();
		if (components7 != null)
		{
			foreach (LoadZoneSettings loadZoneSettings in components7)
			{
				CMSLoadingZone cmsloadingZone = gameObject.AddComponent<CMSLoadingZone>();
				if (cmsloadingZone != null)
				{
					cmsloadingZone.SetupLoadingZone(loadZoneSettings, CustomMapLoader.assetBundleSceneFilePaths);
				}
				Object.Destroy(loadZoneSettings);
			}
		}
		ZoneShaderTriggerSettings[] components8 = gameObject.GetComponents<ZoneShaderTriggerSettings>();
		if (components8 != null)
		{
			foreach (ZoneShaderTriggerSettings zoneShaderTriggerSettings in components8)
			{
				gameObject.AddComponent<CMSZoneShaderSettingsTrigger>().CopySettings(zoneShaderTriggerSettings);
				Object.Destroy(zoneShaderTriggerSettings);
			}
		}
		CMSZoneShaderSettings component = gameObject.GetComponent<CMSZoneShaderSettings>();
		if (component.IsNotNull())
		{
			ZoneShaderSettings zoneShaderSettings = gameObject.AddComponent<ZoneShaderSettings>();
			zoneShaderSettings.CopySettings(component, false);
			if (component.isDefaultValues)
			{
				CustomMapManager.SetDefaultZoneShaderSettings(zoneShaderSettings, component.GetProperties());
			}
			CustomMapManager.AddZoneShaderSettings(zoneShaderSettings);
			Object.Destroy(component);
		}
		HandHoldSettings component2 = gameObject.GetComponent<HandHoldSettings>();
		if (component2.IsNotNull())
		{
			gameObject.AddComponent<HandHold>().CopyProperties(component2);
			Object.Destroy(component2);
		}
		CustomMapEjectButtonSettings component3 = gameObject.GetComponent<CustomMapEjectButtonSettings>();
		if (component3.IsNotNull())
		{
			CustomMapEjectButton customMapEjectButton = gameObject.AddComponent<CustomMapEjectButton>();
			customMapEjectButton.gameObject.layer = UnityLayer.GorillaInteractable.ToLayerIndex();
			customMapEjectButton.CopySettings(component3);
			Object.Destroy(component3);
		}
		MovingSurfaceSettings component4 = gameObject.GetComponent<MovingSurfaceSettings>();
		if (component4.IsNotNull())
		{
			MovingSurface movingSurface = gameObject.AddComponent<MovingSurface>();
			if (movingSurface.IsNotNull())
			{
				movingSurface.CopySettings(component4);
				Object.Destroy(component4);
			}
		}
		SurfaceMoverSettings component5 = gameObject.GetComponent<SurfaceMoverSettings>();
		if (component5.IsNotNull())
		{
			gameObject.AddComponent<SurfaceMover>().CopySettings(component5);
			Object.Destroy(component5);
		}
	}

	// Token: 0x06003C8C RID: 15500 RVA: 0x00140864 File Offset: 0x0013EA64
	private static void ReplacePlaceholders(GameObject placeholderGameObject)
	{
		if (placeholderGameObject.IsNull())
		{
			return;
		}
		GTObjectPlaceholder component = placeholderGameObject.GetComponent<GTObjectPlaceholder>();
		if (component.IsNull())
		{
			return;
		}
		switch (component.PlaceholderObject)
		{
		case 0:
			if (CustomMapLoader.leafGliderIndex < CustomMapLoader.instance.leafGliders.Length)
			{
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].enabled = true;
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].CustomMapLoad(component.transform, component.maxDistanceBeforeRespawn);
				CustomMapLoader.instance.leafGliders[CustomMapLoader.leafGliderIndex].transform.GetChild(0).gameObject.SetActive(true);
				CustomMapLoader.leafGliderIndex++;
				return;
			}
			break;
		case 1:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(CustomMapLoader.instance.gliderWindVolume, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject != null)
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject);
					gameObject.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject.transform.SetParent(placeholderGameObject.transform);
					GliderWindVolume component2 = gameObject.GetComponent<GliderWindVolume>();
					if (component2 == null)
					{
						return;
					}
					component2.SetProperties(component.maxSpeed, component.maxAccel, component.SpeedVSAccelCurve, component.localWindDirection);
					return;
				}
			}
			else
			{
				placeholderGameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
				GliderWindVolume gliderWindVolume = placeholderGameObject.AddComponent<GliderWindVolume>();
				if (gliderWindVolume.IsNotNull())
				{
					gliderWindVolume.SetProperties(component.maxSpeed, component.maxAccel, component.SpeedVSAccelCurve, component.localWindDirection);
					return;
				}
			}
			break;
		}
		case 2:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(CustomMapLoader.instance.waterVolumePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject2 != null)
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject2);
					gameObject2.layer = UnityLayer.Water.ToLayerIndex();
					gameObject2.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject2.transform.SetParent(placeholderGameObject.transform);
					MeshRenderer component3 = gameObject2.GetComponent<MeshRenderer>();
					if (component3.IsNull())
					{
						return;
					}
					if (!component.useWaterMesh)
					{
						component3.enabled = false;
						return;
					}
					component3.enabled = true;
					WaterSurfaceMaterialController component4 = gameObject2.GetComponent<WaterSurfaceMaterialController>();
					if (component4.IsNull())
					{
						return;
					}
					component4.ScrollX = component.scrollTextureX;
					component4.ScrollY = component.scrollTextureY;
					component4.Scale = component.scaleTexture;
					return;
				}
			}
			else
			{
				placeholderGameObject.layer = UnityLayer.Water.ToLayerIndex();
				WaterVolume waterVolume = placeholderGameObject.AddComponent<WaterVolume>();
				if (waterVolume.IsNotNull())
				{
					WaterParameters parameters = null;
					CMSZoneShaderSettings.EZoneLiquidType liquidType = component.liquidType;
					if (liquidType != 1)
					{
						if (liquidType == 2)
						{
							parameters = CustomMapLoader.instance.defaultLavaParameters;
						}
					}
					else
					{
						parameters = CustomMapLoader.instance.defaultWaterParameters;
					}
					waterVolume.SetPropertiesFromPlaceholder(component.GetWaterVolumeProperties(), list, parameters);
					waterVolume.RefreshColliders();
					return;
				}
			}
			break;
		}
		case 3:
		{
			List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
			if (component.useDefaultPlaceholder || list.Count == 0)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(CustomMapLoader.instance.forceVolumePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
				if (gameObject3.IsNotNull())
				{
					CustomMapLoader.placeholderReplacements.Add(gameObject3);
					gameObject3.transform.localScale = placeholderGameObject.transform.localScale;
					placeholderGameObject.transform.localScale = Vector3.one;
					gameObject3.transform.SetParent(placeholderGameObject.transform);
					ForceVolume component5 = gameObject3.GetComponent<ForceVolume>();
					if (component5.IsNull())
					{
						return;
					}
					component5.SetPropertiesFromPlaceholder(component.GetForceVolumeProperties(), null, null);
					return;
				}
			}
			else
			{
				ForceVolume forceVolume = placeholderGameObject.AddComponent<ForceVolume>();
				if (forceVolume.IsNotNull())
				{
					AudioSource audioSource = placeholderGameObject.GetComponent<AudioSource>();
					if (audioSource.IsNull())
					{
						audioSource = placeholderGameObject.AddComponent<AudioSource>();
						audioSource.spatialize = true;
						audioSource.playOnAwake = false;
						audioSource.priority = 128;
						audioSource.volume = 0.522f;
						audioSource.pitch = 1f;
						audioSource.panStereo = 0f;
						audioSource.spatialBlend = 1f;
						audioSource.reverbZoneMix = 1f;
						audioSource.dopplerLevel = 1f;
						audioSource.spread = 0f;
						audioSource.rolloffMode = 0;
						audioSource.minDistance = 8.2f;
						audioSource.maxDistance = 43.94f;
						audioSource.enabled = true;
					}
					audioSource.outputAudioMixerGroup = CustomMapLoader.instance.masterAudioMixer;
					for (int i = list.Count - 1; i >= 0; i--)
					{
						if (i == 0)
						{
							list[i].isTrigger = true;
						}
						else
						{
							Object.Destroy(list[i]);
						}
					}
					placeholderGameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
					forceVolume.SetPropertiesFromPlaceholder(component.GetForceVolumeProperties(), audioSource, component.GetComponent<Collider>());
					return;
				}
				Debug.LogError("[CustomMapLoader::ReplacePlaceholders] Failed to add ForceVolume component to Placeholder!");
				return;
			}
			break;
		}
		case 4:
		{
			if (CustomMapLoader.customMapATM.IsNotNull())
			{
				Object.Destroy(component);
				return;
			}
			if (!CustomMapLoader.ValidateStorePlaceholderPosition(placeholderGameObject))
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject4 = CustomMapLoader.instance.atmPrefab;
			if (component.useCustomMesh)
			{
				gameObject4 = CustomMapLoader.instance.atmNoShellPrefab;
			}
			if (gameObject4.IsNull())
			{
				return;
			}
			GameObject gameObject5 = Object.Instantiate<GameObject>(gameObject4, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject5.IsNotNull())
			{
				gameObject5.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform, true);
				gameObject5.transform.localScale = Vector3.one;
				ATM_UI componentInChildren = gameObject5.GetComponentInChildren<ATM_UI>();
				if (componentInChildren.IsNotNull() && ATM_Manager.instance.IsNotNull())
				{
					componentInChildren.SetCustomMapScene(placeholderGameObject.scene);
					CustomMapLoader.customMapATM = gameObject5;
					ATM_Manager.instance.AddATM(componentInChildren);
					if (!component.defaultCreatorCode.IsNullOrEmpty())
					{
						ATM_Manager.instance.SetTemporaryCreatorCode(component.defaultCreatorCode);
						return;
					}
				}
			}
			break;
		}
		case 5:
			if (component.AddComponent<HoverboardAreaTrigger>().IsNotNull())
			{
				component.gameObject.layer = UnityLayer.GorillaBoundary.ToLayerIndex();
				List<Collider> list = new List<Collider>(component.GetComponents<Collider>());
				if (list.Count != 0)
				{
					for (int j = list.Count - 1; j >= 0; j--)
					{
						if (j == 0)
						{
							list[j].isTrigger = true;
						}
						else
						{
							Object.Destroy(list[j]);
						}
					}
					return;
				}
				BoxCollider boxCollider = component.AddComponent<BoxCollider>();
				if (boxCollider.IsNotNull())
				{
					boxCollider.isTrigger = true;
					return;
				}
			}
			break;
		case 6:
		{
			if (CustomMapLoader.instance.hoverboardDispenserPrefab.IsNull())
			{
				Debug.LogError("[CustomMapLoader::ReplacePlaceholders] hoverboardDispenserPrefab is NULL!");
				return;
			}
			GameObject gameObject6 = Object.Instantiate<GameObject>(CustomMapLoader.instance.hoverboardDispenserPrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject6.IsNotNull())
			{
				CustomMapLoader.placeholderReplacements.Add(gameObject6);
				gameObject6.transform.SetParent(placeholderGameObject.transform);
				return;
			}
			break;
		}
		case 7:
		{
			GameObject gameObject7 = Object.Instantiate<GameObject>(CustomMapLoader.instance.ropeSwingPrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject7.IsNull())
			{
				return;
			}
			gameObject7.transform.SetParent(placeholderGameObject.transform);
			CustomMapsGorillaRopeSwing component6 = gameObject7.GetComponent<CustomMapsGorillaRopeSwing>();
			if (component6.IsNull())
			{
				Object.DestroyImmediate(gameObject7);
				return;
			}
			component.ropeLength = Math.Clamp(component.ropeLength, 3, 31);
			if (component.useDefaultPlaceholder)
			{
				component6.SetRopeLength(component.ropeLength);
			}
			else
			{
				component6.SetRopeProperties(component);
			}
			CustomMapLoader.placeholderReplacements.Add(gameObject7);
			return;
		}
		case 8:
		{
			GameObject gameObject8 = Object.Instantiate<GameObject>(CustomMapLoader.instance.ziplinePrefab, placeholderGameObject.transform.position, placeholderGameObject.transform.rotation);
			if (gameObject8.IsNull())
			{
				return;
			}
			gameObject8.transform.SetParent(placeholderGameObject.transform);
			CustomMapsGorillaZipline component7 = gameObject8.GetComponent<CustomMapsGorillaZipline>();
			if (component7.IsNull())
			{
				Object.DestroyImmediate(gameObject8);
				return;
			}
			if (component.useDefaultPlaceholder)
			{
				if (!component7.GenerateZipline(component.spline))
				{
					Object.DestroyImmediate(gameObject8);
					return;
				}
			}
			else
			{
				component7.Init(component);
			}
			CustomMapLoader.placeholderReplacements.Add(gameObject8);
			return;
		}
		case 9:
		{
			if (CustomMapLoader.instance.storeDisplayStandPrefab.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeDisplayStands.Count >= Constants.storeDisplayStandLimit)
			{
				Object.Destroy(component);
				return;
			}
			if (placeholderGameObject.transform.lossyScale != Vector3.one)
			{
				Object.Destroy(component);
				return;
			}
			if (!CustomMapLoader.ValidateStorePlaceholderPosition(placeholderGameObject))
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject9 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeDisplayStandPrefab, placeholderGameObject.transform);
			if (gameObject9.IsNull())
			{
				return;
			}
			gameObject9.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform, true);
			gameObject9.transform.localScale = Vector3.one;
			DynamicCosmeticStand component8 = gameObject9.GetComponent<DynamicCosmeticStand>();
			if (component8.IsNull())
			{
				Object.DestroyImmediate(gameObject9);
				return;
			}
			component8.InitializeForCustomMapCosmeticItem(component.CosmeticItem, placeholderGameObject.scene);
			CustomMapLoader.storeDisplayStands.Add(gameObject9);
			CustomMapLoader.placeholderReplacements.Add(gameObject9);
			return;
		}
		case 10:
		{
			if (CustomMapLoader.instance.storeTryOnAreaPrefab.IsNull() || CustomMapLoader.instance.compositeTryOnArea.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeTryOnAreas.Count >= Constants.storeTryOnAreaLimit)
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject10 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeTryOnAreaPrefab, placeholderGameObject.transform);
			gameObject10.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform);
			CMSTryOnArea component9 = gameObject10.GetComponent<CMSTryOnArea>();
			if (component9.IsNull() || component9.tryOnAreaCollider.IsNull())
			{
				Object.DestroyImmediate(gameObject10);
				return;
			}
			BoxCollider tryOnAreaCollider = component9.tryOnAreaCollider;
			Vector3 zero = Vector3.zero;
			zero.x = tryOnAreaCollider.size.x * tryOnAreaCollider.transform.lossyScale.x;
			zero.y = tryOnAreaCollider.size.y * tryOnAreaCollider.transform.lossyScale.y;
			zero.z = tryOnAreaCollider.size.z * tryOnAreaCollider.transform.lossyScale.z;
			if (Math.Abs(zero.x * zero.y * zero.z) > Constants.storeTryOnAreaVolumeLimit)
			{
				Object.DestroyImmediate(gameObject10);
				return;
			}
			component9.InitializeForCustomMap(CustomMapLoader.instance.compositeTryOnArea, placeholderGameObject.scene);
			CustomMapLoader.storeTryOnAreas.Add(gameObject10);
			CustomMapLoader.placeholderReplacements.Add(gameObject10);
			break;
		}
		case 11:
		{
			if (CustomMapLoader.instance.storeCheckoutCounterPrefab.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeCheckouts.Count >= Constants.storeCheckoutCounterLimit)
			{
				Object.Destroy(component);
				return;
			}
			if (placeholderGameObject.transform.lossyScale != Vector3.one)
			{
				Object.Destroy(component);
				return;
			}
			if (!CustomMapLoader.ValidateStorePlaceholderPosition(placeholderGameObject))
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject11 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeCheckoutCounterPrefab, placeholderGameObject.transform);
			if (gameObject11.IsNull())
			{
				return;
			}
			gameObject11.transform.SetParent(CustomMapLoader.instance.compositeTryOnArea.transform);
			gameObject11.transform.localScale = Vector3.one;
			ItemCheckout componentInChildren2 = gameObject11.GetComponentInChildren<ItemCheckout>();
			if (componentInChildren2.IsNull())
			{
				Object.DestroyImmediate(gameObject11);
				return;
			}
			componentInChildren2.InitializeForCustomMap(CustomMapLoader.instance.compositeTryOnArea, placeholderGameObject.scene, component.useCustomMesh);
			CustomMapLoader.storeCheckouts.Add(gameObject11);
			CustomMapLoader.placeholderReplacements.Add(gameObject11);
			return;
		}
		case 12:
		{
			if (CustomMapLoader.instance.storeTryOnConsolePrefab.IsNull())
			{
				return;
			}
			if (CustomMapLoader.storeTryOnConsoles.Count >= Constants.storeTryOnConsoleLimit)
			{
				Object.Destroy(component);
				return;
			}
			GameObject gameObject12 = Object.Instantiate<GameObject>(CustomMapLoader.instance.storeTryOnConsolePrefab, placeholderGameObject.transform);
			if (gameObject12.IsNull())
			{
				return;
			}
			FittingRoom componentInChildren3 = gameObject12.GetComponentInChildren<FittingRoom>();
			if (componentInChildren3.IsNull())
			{
				Object.DestroyImmediate(gameObject12);
				return;
			}
			componentInChildren3.InitializeForCustomMap(component.useCustomMesh);
			CustomMapLoader.storeTryOnConsoles.Add(gameObject12);
			CustomMapLoader.placeholderReplacements.Add(gameObject12);
			return;
		}
		default:
			return;
		}
	}

	// Token: 0x06003C8D RID: 15501 RVA: 0x00141510 File Offset: 0x0013F710
	private static void SetupDynamicLight(GameObject dynamicLightGameObject)
	{
		if (dynamicLightGameObject.IsNull())
		{
			return;
		}
		UberShaderDynamicLight component = dynamicLightGameObject.GetComponent<UberShaderDynamicLight>();
		if (component.IsNull())
		{
			return;
		}
		if (component.dynamicLight.IsNull())
		{
			return;
		}
		GameObject gameObject = new GameObject(dynamicLightGameObject.name + "GameLight");
		GameLight gameLight = gameObject.AddComponent<GameLight>();
		gameLight.light = component.dynamicLight;
		GameLightingManager.instance.AddGameLight(gameLight, false);
		gameObject.transform.SetParent(dynamicLightGameObject.transform.parent);
		gameObject.transform.position = component.transform.position;
	}

	// Token: 0x06003C8E RID: 15502 RVA: 0x001415A8 File Offset: 0x0013F7A8
	private static void StoreMapEntity(GameObject entityGameObject)
	{
		if (entityGameObject.IsNull() || CustomMapsGameManager.instance.IsNull())
		{
			return;
		}
		MapEntity component = entityGameObject.GetComponent<MapEntity>();
		if (component.IsNull())
		{
			return;
		}
		if (component is AIAgent)
		{
			AIAgent aiagent = (AIAgent)component;
			if (!aiagent.IsNull())
			{
				string.Format(" | AgentID: {0}", aiagent.enemyTypeId);
			}
		}
		if (component.isTemplate)
		{
			return;
		}
		CustomMapLoader.entitiesToCreate.Add(component);
	}

	// Token: 0x06003C8F RID: 15503 RVA: 0x0014161C File Offset: 0x0013F81C
	private static void CacheLightmaps()
	{
		CustomMapLoader.lightmaps = new LightmapData[LightmapSettings.lightmaps.Length];
		if (CustomMapLoader.lightmapsToKeep.Count > 0)
		{
			CustomMapLoader.lightmapsToKeep.Clear();
		}
		CustomMapLoader.lightmapsToKeep = new List<Texture2D>(LightmapSettings.lightmaps.Length * 2);
		for (int i = 0; i < LightmapSettings.lightmaps.Length; i++)
		{
			CustomMapLoader.lightmaps[i] = LightmapSettings.lightmaps[i];
			if (LightmapSettings.lightmaps[i].lightmapColor != null)
			{
				CustomMapLoader.lightmapsToKeep.Add(LightmapSettings.lightmaps[i].lightmapColor);
			}
			if (LightmapSettings.lightmaps[i].lightmapDir != null)
			{
				CustomMapLoader.lightmapsToKeep.Add(LightmapSettings.lightmaps[i].lightmapDir);
			}
		}
	}

	// Token: 0x06003C90 RID: 15504 RVA: 0x001416D8 File Offset: 0x0013F8D8
	private static void LoadLightmaps(Texture2D[] colorMaps, Texture2D[] dirMaps)
	{
		if (colorMaps.Length == 0)
		{
			return;
		}
		CustomMapLoader.UnloadLightmaps();
		List<LightmapData> list = new List<LightmapData>(LightmapSettings.lightmaps);
		for (int i = 0; i < colorMaps.Length; i++)
		{
			bool flag = false;
			LightmapData lightmapData = new LightmapData();
			if (colorMaps[i] != null)
			{
				lightmapData.lightmapColor = colorMaps[i];
				flag = true;
				if (i < dirMaps.Length && dirMaps[i] != null)
				{
					lightmapData.lightmapDir = dirMaps[i];
				}
			}
			if (flag)
			{
				list.Add(lightmapData);
			}
		}
		LightmapSettings.lightmaps = list.ToArray();
	}

	// Token: 0x06003C91 RID: 15505 RVA: 0x00141758 File Offset: 0x0013F958
	public static void ResetToInitialZone(Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		List<int> list = new List<int>(CustomMapLoader.initialSceneIndexes);
		List<int> list2 = new List<int>(CustomMapLoader.loadedSceneIndexes);
		foreach (int num in CustomMapLoader.loadedSceneIndexes)
		{
			if (CustomMapLoader.initialSceneIndexes.Contains(num))
			{
				list2.Remove(num);
				list.Remove(num);
			}
		}
		if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion <= 2 && CustomMapLoader.loadedSceneIndexes.Contains(CustomMapLoader.initialSceneIndexes[0]))
		{
			MapDescriptor[] array = Object.FindObjectsByType<MapDescriptor>(0);
			bool flag = false;
			int i;
			for (i = 0; i < array.Length; i++)
			{
				if (array[i].IsInitialScene && array[i].UseUberShaderDynamicLighting)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
				GameLightingManager.instance.SetAmbientLightDynamic(array[i].UberShaderAmbientDynamicLight);
				CustomMapLoader.usingDynamicLighting = true;
			}
			else
			{
				GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
				GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
				CustomMapLoader.usingDynamicLighting = false;
			}
		}
		else if (CustomMapLoader.loadedMapPackageInfo.customMapSupportVersion > 2)
		{
			if (CustomMapLoader.loadedMapPackageInfo.useUberShaderDynamicLighting)
			{
				Color ambientLightDynamic;
				ambientLightDynamic..ctor(CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_R, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_G, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_B, CustomMapLoader.loadedMapPackageInfo.uberShaderAmbientDynamicLight_A);
				GameLightingManager.instance.SetCustomDynamicLightingEnabled(true);
				GameLightingManager.instance.SetAmbientLightDynamic(ambientLightDynamic);
				CustomMapLoader.usingDynamicLighting = true;
			}
			else
			{
				GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
				GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
				CustomMapLoader.usingDynamicLighting = false;
			}
		}
		if (list.IsNullOrEmpty<int>() && list2.IsNullOrEmpty<int>())
		{
			return;
		}
		if (CustomMapLoader.zoneLoadingCoroutine != null)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = new CustomMapLoader.LoadZoneRequest
			{
				sceneIndexesToLoad = list.ToArray(),
				sceneIndexesToUnload = list2.ToArray(),
				onSceneLoadedCallback = onSceneLoaded,
				onSceneUnloadedCallback = onSceneUnloaded
			};
			CustomMapLoader.queuedLoadZoneRequests.Add(loadZoneRequest);
			return;
		}
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
		CustomMapLoader.zoneLoadingCoroutine = CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadZoneCoroutine(list.ToArray(), list2.ToArray()));
	}

	// Token: 0x06003C92 RID: 15506 RVA: 0x001419B4 File Offset: 0x0013FBB4
	public static void LoadZoneTriggered(int[] loadSceneIndexes, int[] unloadSceneIndexes, Action<string> onSceneLoaded, Action<string> onSceneUnloaded)
	{
		string text = "";
		for (int i = 0; i < loadSceneIndexes.Length; i++)
		{
			text += loadSceneIndexes[i].ToString();
			if (i != loadSceneIndexes.Length - 1)
			{
				text += ", ";
			}
		}
		string text2 = "";
		for (int j = 0; j < unloadSceneIndexes.Length; j++)
		{
			text2 += unloadSceneIndexes[j].ToString();
			if (j != unloadSceneIndexes.Length - 1)
			{
				text2 += ", ";
			}
		}
		if (CustomMapLoader.zoneLoadingCoroutine != null)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = new CustomMapLoader.LoadZoneRequest
			{
				sceneIndexesToLoad = loadSceneIndexes,
				sceneIndexesToUnload = unloadSceneIndexes,
				onSceneLoadedCallback = onSceneLoaded,
				onSceneUnloadedCallback = onSceneUnloaded
			};
			CustomMapLoader.queuedLoadZoneRequests.Add(loadZoneRequest);
			return;
		}
		CustomMapLoader.sceneLoadedCallback = onSceneLoaded;
		CustomMapLoader.sceneUnloadedCallback = onSceneUnloaded;
		CustomMapLoader.zoneLoadingCoroutine = CustomMapLoader.instance.StartCoroutine(CustomMapLoader.LoadZoneCoroutine(loadSceneIndexes, unloadSceneIndexes));
	}

	// Token: 0x06003C93 RID: 15507 RVA: 0x00141A9B File Offset: 0x0013FC9B
	private static IEnumerator LoadZoneCoroutine(int[] loadScenes, int[] unloadScenes)
	{
		if (!unloadScenes.IsNullOrEmpty<int>())
		{
			yield return CustomMapLoader.UnloadScenesCoroutine(unloadScenes);
		}
		if (!loadScenes.IsNullOrEmpty<int>())
		{
			yield return CustomMapLoader.LoadScenesCoroutine(loadScenes, delegate(bool successfullyLoadedAllScenes, bool loadAborted, List<string> successfullyLoadedSceneNames)
			{
				if (loadAborted)
				{
					CustomMapLoader.queuedLoadZoneRequests.Clear();
				}
			});
		}
		CustomMapLoader.zoneLoadingCoroutine = null;
		if (CustomMapLoader.queuedLoadZoneRequests.Count > 0)
		{
			CustomMapLoader.LoadZoneRequest loadZoneRequest = CustomMapLoader.queuedLoadZoneRequests[0];
			CustomMapLoader.queuedLoadZoneRequests.RemoveAt(0);
			CustomMapLoader.LoadZoneTriggered(loadZoneRequest.sceneIndexesToLoad, loadZoneRequest.sceneIndexesToUnload, loadZoneRequest.onSceneLoadedCallback, loadZoneRequest.onSceneUnloadedCallback);
		}
		yield break;
	}

	// Token: 0x06003C94 RID: 15508 RVA: 0x00141AB1 File Offset: 0x0013FCB1
	public static void CloseDoorAndUnloadMap(Action unloadCompleted = null)
	{
		if (!CustomMapLoader.IsMapLoaded() && !CustomMapLoader.isLoading)
		{
			return;
		}
		if (unloadCompleted != null)
		{
			CustomMapLoader.unloadMapCallback = unloadCompleted;
		}
		if (CustomMapLoader.isLoading)
		{
			CustomMapLoader.RequestAbortMapLoad();
			return;
		}
		CustomMapLoader.instance.StartCoroutine(CustomMapLoader.CloseDoorAndUnloadMapCoroutine());
	}

	// Token: 0x06003C95 RID: 15509 RVA: 0x00141AEA File Offset: 0x0013FCEA
	private static IEnumerator CloseDoorAndUnloadMapCoroutine()
	{
		if (!CustomMapLoader.IsMapLoaded())
		{
			yield break;
		}
		if (CustomMapLoader.instance.accessDoor != null)
		{
			CustomMapLoader.instance.accessDoor.CloseDoor();
		}
		if (CustomMapLoader.instance.publicJoinTrigger != null)
		{
			CustomMapLoader.instance.publicJoinTrigger.SetActive(false);
		}
		CustomMapLoader.shouldAbortMapLoading = true;
		if (CustomMapLoader.IsLoading())
		{
			yield break;
		}
		yield return CustomMapLoader.UnloadMapCoroutine();
		yield break;
	}

	// Token: 0x06003C96 RID: 15510 RVA: 0x00141AF2 File Offset: 0x0013FCF2
	private static void RequestAbortMapLoad()
	{
		CustomMapLoader.shouldAbortSceneLoad = true;
		CustomMapLoader.shouldAbortMapLoading = true;
	}

	// Token: 0x06003C97 RID: 15511 RVA: 0x00141B00 File Offset: 0x0013FD00
	private static IEnumerator AbortMapLoad()
	{
		GTDev.Log<string>("[CML.AbortMapLoad] Aborting map load...", null);
		CustomMapLoader.shouldAbortSceneLoad = true;
		CustomMapLoader.shouldAbortMapLoading = true;
		yield return CustomMapLoader.AbortSceneLoad(-1);
		Action<bool> action = CustomMapLoader.mapLoadFinishedCallback;
		if (action != null)
		{
			action.Invoke(false);
		}
		yield break;
	}

	// Token: 0x06003C98 RID: 15512 RVA: 0x00141B08 File Offset: 0x0013FD08
	private static IEnumerator UnloadMapCoroutine()
	{
		GTDev.Log<string>("[CML.UnloadMap_Co] Unloading Custom Map...", null);
		if (CustomMapLoader.zoneLoadingCoroutine != null)
		{
			CustomMapLoader.queuedLoadZoneRequests.Clear();
			CustomMapLoader.instance.StopCoroutine(CustomMapLoader.zoneLoadingCoroutine);
			CustomMapLoader.zoneLoadingCoroutine = null;
		}
		CustomMapLoader.isUnloading = true;
		CustomMapLoader.CanLoadEntities = false;
		CustomMapTelemetry.EndMapTracking();
		ZoneShaderSettings.ActivateDefaultSettings();
		CustomMapLoader.CleanupPlaceholders();
		CMSSerializer.ResetSyncedMapObjects();
		CustomMapLoader.instance.ghostReactorManager.reactor.RefreshReviveStations(false);
		if (!CustomMapLoader.assetBundleSceneFilePaths.IsNullOrEmpty<string>())
		{
			int num;
			for (int sceneIndex = 0; sceneIndex < CustomMapLoader.assetBundleSceneFilePaths.Length; sceneIndex = num + 1)
			{
				yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndex, null);
				num = sceneIndex;
			}
		}
		GorillaNetworkJoinTrigger.EnableTriggerJoins();
		LightmapSettings.lightmaps = CustomMapLoader.lightmaps;
		CustomMapLoader.UnloadLightmaps();
		yield return CustomMapLoader.ResetLightmaps();
		CustomMapLoader.usingDynamicLighting = false;
		GameLightingManager.instance.SetCustomDynamicLightingEnabled(false);
		GameLightingManager.instance.SetAmbientLightDynamic(Color.black);
		if (CustomMapLoader.mapBundle != null)
		{
			CustomMapLoader.mapBundle.Unload(true);
		}
		CustomMapLoader.mapBundle = null;
		Resources.UnloadUnusedAssets();
		CustomMapLoader.cachedLuauScript = "";
		CustomMapLoader.devModeEnabled = false;
		CustomMapLoader.disableHoldingHandsAllModes = false;
		CustomMapLoader.disableHoldingHandsCustomMode = false;
		CustomMapLoader.queuedLoadZoneRequests.Clear();
		CustomMapLoader.assetBundleSceneFilePaths = new string[]
		{
			""
		};
		CustomMapLoader.loadedMapPackageInfo = null;
		CustomMapLoader.loadedMapModId = 0L;
		CustomMapLoader.loadedSceneFilePaths.Clear();
		CustomMapLoader.loadedSceneNames.Clear();
		CustomMapLoader.loadedSceneIndexes.Clear();
		CustomMapLoader.initialSceneIndexes.Clear();
		CustomMapLoader.initialSceneNames.Clear();
		CustomMapLoader.maxPlayersForMap = 10;
		CustomMapModeSelector.ResetButtons();
		if (RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
		{
			if (GameMode.ActiveGameMode.IsNull())
			{
				GameMode.ChangeGameMode(GameModeType.Casual.ToString());
			}
			else if (GameMode.ActiveGameMode.GameType() != GameModeType.Casual)
			{
				GameMode.ChangeGameMode(GameModeType.Casual.ToString());
			}
		}
		CustomMapLoader.shouldAbortMapLoading = false;
		CustomMapLoader.shouldAbortSceneLoad = false;
		CustomMapLoader.isUnloading = false;
		if (CustomMapLoader.unloadMapCallback != null)
		{
			Action action = CustomMapLoader.unloadMapCallback;
			if (action != null)
			{
				action.Invoke();
			}
			CustomMapLoader.unloadMapCallback = null;
		}
		yield break;
	}

	// Token: 0x06003C99 RID: 15513 RVA: 0x00141B10 File Offset: 0x0013FD10
	private static IEnumerator AbortSceneLoad(int sceneIndex)
	{
		if (sceneIndex == -1)
		{
			CustomMapLoader.shouldAbortMapLoading = true;
		}
		CustomMapLoader.isLoading = false;
		if (CustomMapLoader.shouldAbortMapLoading)
		{
			yield return CustomMapLoader.UnloadMapCoroutine();
		}
		else
		{
			yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndex, null);
		}
		CustomMapLoader.shouldAbortSceneLoad = false;
		yield break;
	}

	// Token: 0x06003C9A RID: 15514 RVA: 0x00141B1F File Offset: 0x0013FD1F
	private static IEnumerator UnloadScenesCoroutine(int[] sceneIndexes)
	{
		int num;
		for (int i = 0; i < sceneIndexes.Length; i = num + 1)
		{
			yield return CustomMapLoader.UnloadSceneCoroutine(sceneIndexes[i], null);
			num = i;
		}
		yield break;
	}

	// Token: 0x06003C9B RID: 15515 RVA: 0x00141B2E File Offset: 0x0013FD2E
	private static IEnumerator UnloadSceneCoroutine(int sceneIndex, Action OnUnloadComplete = null)
	{
		if (!CustomMapLoader.hasInstance)
		{
			yield break;
		}
		if (sceneIndex < 0 || sceneIndex >= CustomMapLoader.assetBundleSceneFilePaths.Length)
		{
			Debug.LogError(string.Format("[CustomMapLoader::UnloadSceneCoroutine] SceneIndex of {0} is invalid! ", sceneIndex) + string.Format("The currently loaded AssetBundle contains {0} scenes.", CustomMapLoader.assetBundleSceneFilePaths.Length));
			yield break;
		}
		while (CustomMapLoader.runningAsyncLoad)
		{
			yield return null;
		}
		UnloadSceneOptions unloadSceneOptions = 1;
		string scenePathWithExtension = CustomMapLoader.assetBundleSceneFilePaths[sceneIndex];
		string[] array = scenePathWithExtension.Split(".", 0);
		string text = "";
		string sceneName = "";
		if (!array.IsNullOrEmpty<string>())
		{
			text = array[0];
			if (text.Length > 0)
			{
				sceneName = Path.GetFileName(text);
			}
		}
		Scene sceneByName = SceneManager.GetSceneByName(text);
		if (sceneByName.IsValid())
		{
			CustomMapLoader.RemoveUnloadingStorePrefabs(sceneByName);
			for (int i = CustomMapLoader.teleporters.Count - 1; i >= 0; i--)
			{
				if (CustomMapLoader.teleporters[i].gameObject.scene == sceneByName)
				{
					CustomMapLoader.teleporters.RemoveAt(i);
				}
			}
			AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scenePathWithExtension, unloadSceneOptions);
			yield return asyncOperation;
			CustomMapLoader.loadedSceneFilePaths.Remove(scenePathWithExtension);
			CustomMapLoader.loadedSceneNames.Remove(sceneName);
			CustomMapLoader.loadedSceneIndexes.Remove(sceneIndex);
			Action<string> action = CustomMapLoader.sceneUnloadedCallback;
			if (action != null)
			{
				action.Invoke(sceneName);
			}
			if (OnUnloadComplete != null)
			{
				OnUnloadComplete.Invoke();
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x06003C9C RID: 15516 RVA: 0x00141B44 File Offset: 0x0013FD44
	private static void RemoveUnloadingStorePrefabs(Scene unloadingScene)
	{
		if (CustomMapLoader.customMapATM.IsNotNull())
		{
			ATM_UI componentInChildren = CustomMapLoader.customMapATM.GetComponentInChildren<ATM_UI>();
			if (componentInChildren.IsNotNull() && componentInChildren.IsFromCustomMapScene(unloadingScene) && ATM_Manager.instance.IsNotNull())
			{
				ATM_Manager.instance.RemoveATM(componentInChildren);
				ATM_Manager.instance.SetTemporaryCreatorCode(null);
			}
			Object.Destroy(CustomMapLoader.customMapATM);
			CustomMapLoader.customMapATM = null;
		}
		for (int i = CustomMapLoader.storeDisplayStands.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeDisplayStands[i].IsNull())
			{
				CustomMapLoader.storeDisplayStands.RemoveAt(i);
			}
			else
			{
				DynamicCosmeticStand componentInChildren2 = CustomMapLoader.storeDisplayStands[i].GetComponentInChildren<DynamicCosmeticStand>();
				if (componentInChildren2.IsNotNull() && componentInChildren2.IsFromCustomMapScene(unloadingScene))
				{
					if (componentInChildren2.IsNotNull())
					{
						StoreController.instance.RemoveStandFromPlayFabIDDictionary(componentInChildren2);
					}
					Object.Destroy(CustomMapLoader.storeDisplayStands[i]);
					CustomMapLoader.storeDisplayStands.RemoveAt(i);
				}
			}
		}
		for (int i = CustomMapLoader.storeCheckouts.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeCheckouts[i].IsNull())
			{
				CustomMapLoader.storeCheckouts.RemoveAt(i);
			}
			else
			{
				ItemCheckout componentInChildren3 = CustomMapLoader.storeCheckouts[i].GetComponentInChildren<ItemCheckout>();
				if (componentInChildren3.IsNotNull() && componentInChildren3.IsFromScene(unloadingScene))
				{
					componentInChildren3.RemoveFromCustomMap(CustomMapLoader.instance.compositeTryOnArea);
					CosmeticsController.instance.RemoveItemCheckout(componentInChildren3);
					Object.Destroy(CustomMapLoader.storeCheckouts[i]);
					CustomMapLoader.storeCheckouts.RemoveAt(i);
				}
			}
		}
		for (int i = CustomMapLoader.storeTryOnConsoles.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeTryOnConsoles[i].IsNull())
			{
				CustomMapLoader.storeTryOnConsoles.RemoveAt(i);
			}
			else if (CustomMapLoader.storeTryOnConsoles[i].scene.Equals(unloadingScene))
			{
				FittingRoom componentInChildren4 = CustomMapLoader.storeTryOnConsoles[i].GetComponentInChildren<FittingRoom>();
				if (componentInChildren4.IsNotNull())
				{
					CosmeticsController.instance.RemoveFittingRoom(componentInChildren4);
				}
				CustomMapLoader.storeTryOnConsoles.RemoveAt(i);
			}
		}
		for (int i = CustomMapLoader.storeTryOnAreas.Count - 1; i >= 0; i--)
		{
			if (CustomMapLoader.storeTryOnAreas[i].IsNull())
			{
				CustomMapLoader.storeTryOnAreas.RemoveAt(i);
			}
			else
			{
				CMSTryOnArea component = CustomMapLoader.storeTryOnAreas[i].GetComponent<CMSTryOnArea>();
				if (component.IsNotNull() && component.IsFromScene(unloadingScene))
				{
					component.RemoveFromCustomMap(CustomMapLoader.instance.compositeTryOnArea);
					Object.Destroy(CustomMapLoader.storeTryOnAreas[i]);
					CustomMapLoader.storeTryOnAreas.RemoveAt(i);
				}
			}
		}
	}

	// Token: 0x06003C9D RID: 15517 RVA: 0x00141DF0 File Offset: 0x0013FFF0
	private static void CleanupPlaceholders()
	{
		for (int i = 0; i < CustomMapLoader.instance.leafGliders.Length; i++)
		{
			CustomMapLoader.instance.leafGliders[i].CustomMapUnload();
			CustomMapLoader.instance.leafGliders[i].enabled = false;
			CustomMapLoader.instance.leafGliders[i].transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	// Token: 0x06003C9E RID: 15518 RVA: 0x00141E61 File Offset: 0x00140061
	private static IEnumerator ResetLightmaps()
	{
		CustomMapLoader.instance.dayNightManager.RequestRepopulateLightmaps();
		LoadSceneParameters loadSceneParameters = default(LoadSceneParameters);
		loadSceneParameters.loadSceneMode = 1;
		loadSceneParameters.localPhysicsMode = 0;
		LoadSceneParameters loadSceneParameters2 = loadSceneParameters;
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(10, loadSceneParameters2);
		yield return asyncOperation;
		asyncOperation = SceneManager.UnloadSceneAsync(10);
		yield return asyncOperation;
		yield break;
	}

	// Token: 0x06003C9F RID: 15519 RVA: 0x00141E6C File Offset: 0x0014006C
	private static void UnloadLightmaps()
	{
		foreach (LightmapData lightmapData in LightmapSettings.lightmaps)
		{
			if (lightmapData.lightmapColor != null && !CustomMapLoader.lightmapsToKeep.Contains(lightmapData.lightmapColor))
			{
				Resources.UnloadAsset(lightmapData.lightmapColor);
			}
			if (lightmapData.lightmapDir != null && !CustomMapLoader.lightmapsToKeep.Contains(lightmapData.lightmapDir))
			{
				Resources.UnloadAsset(lightmapData.lightmapDir);
			}
		}
	}

	// Token: 0x06003CA0 RID: 15520 RVA: 0x00141EE8 File Offset: 0x001400E8
	private static int GetSceneIndex(string sceneName)
	{
		int result = -1;
		if (CustomMapLoader.assetBundleSceneFilePaths.Length == 1)
		{
			return 0;
		}
		for (int i = 0; i < CustomMapLoader.assetBundleSceneFilePaths.Length; i++)
		{
			string sceneNameFromFilePath = CustomMapLoader.GetSceneNameFromFilePath(CustomMapLoader.assetBundleSceneFilePaths[i]);
			if (sceneNameFromFilePath != null && sceneNameFromFilePath.Equals(sceneName))
			{
				result = i;
				break;
			}
		}
		return result;
	}

	// Token: 0x06003CA1 RID: 15521 RVA: 0x00141F37 File Offset: 0x00140137
	private static string GetSceneNameFromFilePath(string filePath)
	{
		string[] array = filePath.Split("/", 0);
		return array[array.Length - 1].Split(".", 0)[0];
	}

	// Token: 0x06003CA2 RID: 15522 RVA: 0x00141F58 File Offset: 0x00140158
	public static MapPackageInfo GetPackageInfo(string packageInfoFilePath)
	{
		MapPackageInfo result;
		using (StreamReader streamReader = new StreamReader(File.OpenRead(packageInfoFilePath), Encoding.Default))
		{
			result = JsonConvert.DeserializeObject<MapPackageInfo>(streamReader.ReadToEnd());
		}
		return result;
	}

	// Token: 0x17000595 RID: 1429
	// (get) Token: 0x06003CA3 RID: 15523 RVA: 0x00141FA0 File Offset: 0x001401A0
	public static ModId LoadedMapModId
	{
		get
		{
			return CustomMapLoader.loadedMapModId;
		}
	}

	// Token: 0x17000596 RID: 1430
	// (get) Token: 0x06003CA4 RID: 15524 RVA: 0x00141FA7 File Offset: 0x001401A7
	public static long LoadedMapModFileId
	{
		get
		{
			return CustomMapLoader.loadedMapModFileId;
		}
	}

	// Token: 0x17000597 RID: 1431
	// (get) Token: 0x06003CA6 RID: 15526 RVA: 0x00141FB6 File Offset: 0x001401B6
	// (set) Token: 0x06003CA5 RID: 15525 RVA: 0x00141FAE File Offset: 0x001401AE
	public static bool CanLoadEntities { get; private set; }

	// Token: 0x06003CA7 RID: 15527 RVA: 0x00141FBD File Offset: 0x001401BD
	public static bool IsMapLoaded()
	{
		return CustomMapLoader.IsMapLoaded(ModId.Null);
	}

	// Token: 0x06003CA8 RID: 15528 RVA: 0x00141FCC File Offset: 0x001401CC
	public static bool IsMapLoaded(ModId mapModId)
	{
		if (mapModId.IsValid())
		{
			return !CustomMapLoader.IsLoading() && CustomMapLoader.LoadedMapModId == mapModId;
		}
		return !CustomMapLoader.IsLoading() && CustomMapLoader.LoadedMapModId.IsValid();
	}

	// Token: 0x06003CA9 RID: 15529 RVA: 0x0014200D File Offset: 0x0014020D
	public static bool IsLoading()
	{
		return CustomMapLoader.isLoading;
	}

	// Token: 0x06003CAA RID: 15530 RVA: 0x00142014 File Offset: 0x00140214
	public static long GetLoadingMapModId()
	{
		return CustomMapLoader.attemptedLoadID;
	}

	// Token: 0x06003CAB RID: 15531 RVA: 0x0014201B File Offset: 0x0014021B
	public static byte GetRoomSizeForCurrentlyLoadedMap()
	{
		if (!CustomMapLoader.IsMapLoaded())
		{
			return 10;
		}
		return CustomMapLoader.maxPlayersForMap;
	}

	// Token: 0x06003CAC RID: 15532 RVA: 0x0014202C File Offset: 0x0014022C
	public static bool IsCustomScene(string sceneName)
	{
		return CustomMapLoader.loadedSceneNames.Contains(sceneName);
	}

	// Token: 0x06003CAD RID: 15533 RVA: 0x00142039 File Offset: 0x00140239
	public static string GetLuauGamemodeScript()
	{
		if (!CustomMapLoader.IsMapLoaded())
		{
			return "";
		}
		return CustomMapLoader.cachedLuauScript;
	}

	// Token: 0x06003CAE RID: 15534 RVA: 0x0014204D File Offset: 0x0014024D
	public static bool IsDevModeEnabled()
	{
		return CustomMapLoader.IsMapLoaded() && CustomMapLoader.devModeEnabled;
	}

	// Token: 0x06003CAF RID: 15535 RVA: 0x0014205D File Offset: 0x0014025D
	public static Transform GetCustomMapsDefaultSpawnLocation()
	{
		if (CustomMapLoader.hasInstance)
		{
			return CustomMapLoader.instance.CustomMapsDefaultSpawnLocation;
		}
		return null;
	}

	// Token: 0x06003CB0 RID: 15536 RVA: 0x00142074 File Offset: 0x00140274
	public static bool LoadedMapWantsHoldingHandsDisabled()
	{
		return CustomMapLoader.IsMapLoaded() && (CustomMapLoader.disableHoldingHandsAllModes || (CustomMapLoader.disableHoldingHandsCustomMode && GorillaGameManager.instance.IsNotNull() && GorillaGameManager.instance.GameType() == GameModeType.Custom));
	}

	// Token: 0x06003CB1 RID: 15537 RVA: 0x001420AB File Offset: 0x001402AB
	bool IBuildValidation.BuildValidationCheck()
	{
		if (this.defaultNexusGroupId == null)
		{
			Debug.LogError("You have to set defaultNexusGroupId in " + base.name + " or things will not work!");
			return false;
		}
		return true;
	}

	// Token: 0x06003CB2 RID: 15538 RVA: 0x001420D8 File Offset: 0x001402D8
	public CustomMapLoader()
	{
		List<GameModeType> list = new List<GameModeType>();
		list.Add(GameModeType.Infection);
		list.Add(GameModeType.FreezeTag);
		list.Add(GameModeType.Paintbrawl);
		this.availableModesForOldMaps = list;
		this.defaultGameModeForNonCustomOldMaps = GameModeType.Infection;
		this.dontDestroyOnLoadSceneName = "";
		base..ctor();
	}

	// Token: 0x06003CB3 RID: 15539 RVA: 0x00142114 File Offset: 0x00140314
	// Note: this type is marked as 'beforefieldinit'.
	static CustomMapLoader()
	{
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(1);
		list.Add(2);
		list.Add(4);
		list.Add(5);
		list.Add(9);
		list.Add(11);
		list.Add(18);
		list.Add(20);
		list.Add(22);
		list.Add(27);
		list.Add(30);
		CustomMapLoader.APPROVED_LAYERS = list;
		CustomMapLoader.runningAsyncLoad = false;
		CustomMapLoader.attemptedLoadID = 0L;
		CustomMapLoader.shouldAbortMapLoading = false;
		CustomMapLoader.shouldAbortSceneLoad = false;
		CustomMapLoader.errorEncounteredDuringLoad = false;
		CustomMapLoader.cachedExceptionMessage = "";
		CustomMapLoader.initialSceneNames = new List<string>();
		CustomMapLoader.initialSceneIndexes = new List<int>();
		CustomMapLoader.maxPlayersForMap = 10;
		CustomMapLoader.queuedLoadZoneRequests = new List<CustomMapLoader.LoadZoneRequest>();
		CustomMapLoader.loadedSceneFilePaths = new List<string>();
		CustomMapLoader.loadedSceneNames = new List<string>();
		CustomMapLoader.loadedSceneIndexes = new List<int>();
		CustomMapLoader.usingDynamicLighting = false;
		CustomMapLoader.refreshReviveStations = false;
		CustomMapLoader.totalObjectsInLoadingScene = 0;
		CustomMapLoader.objectsProcessedForLoadingScene = 0;
		CustomMapLoader.objectsProcessedThisFrame = 0;
		CustomMapLoader.initializePhaseTwoComponents = new List<Component>();
		CustomMapLoader.entitiesToCreate = new List<MapEntity>(Constants.aiAgentLimit);
		CustomMapLoader.lightmapsToKeep = new List<Texture2D>();
		CustomMapLoader.placeholderReplacements = new List<GameObject>();
		CustomMapLoader.customMapATM = null;
		CustomMapLoader.storeCheckouts = new List<GameObject>();
		CustomMapLoader.storeDisplayStands = new List<GameObject>();
		CustomMapLoader.storeTryOnConsoles = new List<GameObject>();
		CustomMapLoader.storeTryOnAreas = new List<GameObject>();
		CustomMapLoader.teleporters = new List<Component>();
		List<Type> list2 = new List<Type>();
		list2.Add(typeof(MeshRenderer));
		list2.Add(typeof(Transform));
		list2.Add(typeof(MeshFilter));
		list2.Add(typeof(MeshRenderer));
		list2.Add(typeof(Collider));
		list2.Add(typeof(BoxCollider));
		list2.Add(typeof(SphereCollider));
		list2.Add(typeof(CapsuleCollider));
		list2.Add(typeof(MeshCollider));
		list2.Add(typeof(Light));
		list2.Add(typeof(ReflectionProbe));
		list2.Add(typeof(AudioSource));
		list2.Add(typeof(Animator));
		list2.Add(typeof(SkinnedMeshRenderer));
		list2.Add(typeof(TextMesh));
		list2.Add(typeof(ParticleSystem));
		list2.Add(typeof(ParticleSystemRenderer));
		list2.Add(typeof(RectTransform));
		list2.Add(typeof(SpriteRenderer));
		list2.Add(typeof(BillboardRenderer));
		list2.Add(typeof(Canvas));
		list2.Add(typeof(CanvasRenderer));
		list2.Add(typeof(CanvasScaler));
		list2.Add(typeof(GraphicRaycaster));
		list2.Add(typeof(Rigidbody));
		list2.Add(typeof(TrailRenderer));
		list2.Add(typeof(LineRenderer));
		list2.Add(typeof(LensFlareComponentSRP));
		list2.Add(typeof(Camera));
		list2.Add(typeof(UniversalAdditionalCameraData));
		list2.Add(typeof(NavMeshAgent));
		list2.Add(typeof(NavMesh));
		list2.Add(typeof(NavMeshObstacle));
		list2.Add(typeof(NavMeshLink));
		list2.Add(typeof(NavMeshModifierVolume));
		list2.Add(typeof(NavMeshModifier));
		list2.Add(typeof(NavMeshSurface));
		list2.Add(typeof(HingeJoint));
		list2.Add(typeof(ConstantForce));
		list2.Add(typeof(LODGroup));
		list2.Add(typeof(MapDescriptor));
		list2.Add(typeof(AccessDoorPlaceholder));
		list2.Add(typeof(MapOrientationPoint));
		list2.Add(typeof(SurfaceOverrideSettings));
		list2.Add(typeof(TeleporterSettings));
		list2.Add(typeof(TagZoneSettings));
		list2.Add(typeof(LuauTriggerSettings));
		list2.Add(typeof(MapBoundarySettings));
		list2.Add(typeof(ObjectActivationTriggerSettings));
		list2.Add(typeof(LoadZoneSettings));
		list2.Add(typeof(GTObjectPlaceholder));
		list2.Add(typeof(CMSZoneShaderSettings));
		list2.Add(typeof(ZoneShaderTriggerSettings));
		list2.Add(typeof(MultiPartFire));
		list2.Add(typeof(HandHoldSettings));
		list2.Add(typeof(CustomMapEjectButtonSettings));
		list2.Add(typeof(BezierSpline));
		list2.Add(typeof(UberShaderDynamicLight));
		list2.Add(typeof(MapEntity));
		list2.Add(typeof(GrabbableEntity));
		list2.Add(typeof(AIAgent));
		list2.Add(typeof(AISpawnManager));
		list2.Add(typeof(AISpawnPoint));
		list2.Add(typeof(MapSpawnPoint));
		list2.Add(typeof(MapSpawnManager));
		list2.Add(typeof(RopeSwingSegment));
		list2.Add(typeof(ZiplineSegment));
		list2.Add(typeof(PlayAnimationTriggerSettings));
		list2.Add(typeof(SurfaceMoverSettings));
		list2.Add(typeof(MovingSurfaceSettings));
		list2.Add(typeof(CustomMapReviveStation));
		list2.Add(typeof(ProBuilderMesh));
		list2.Add(typeof(TMP_Text));
		list2.Add(typeof(TextMeshPro));
		list2.Add(typeof(TextMeshProUGUI));
		list2.Add(typeof(UniversalAdditionalLightData));
		list2.Add(typeof(BakerySkyLight));
		list2.Add(typeof(BakeryDirectLight));
		list2.Add(typeof(BakeryPointLight));
		list2.Add(typeof(ftLightmapsStorage));
		list2.Add(typeof(BakeryAlwaysRender));
		list2.Add(typeof(BakeryLightMesh));
		list2.Add(typeof(BakeryLightmapGroupSelector));
		list2.Add(typeof(BakeryPackAsSingleSquare));
		list2.Add(typeof(BakerySector));
		list2.Add(typeof(BakeryVolume));
		list2.Add(typeof(BakeryLightmapGroup));
		CustomMapLoader.componentAllowlist = list2;
		List<string> list3 = new List<string>();
		list3.Add("UnityEngine.Halo");
		CustomMapLoader.componentTypeStringAllowList = list3;
		CustomMapLoader.badComponents = new Type[]
		{
			typeof(EventTrigger),
			typeof(UIBehaviour),
			typeof(GorillaPressableButton),
			typeof(GorillaPressableDelayButton),
			typeof(Camera),
			typeof(AudioListener),
			typeof(VideoPlayer)
		};
	}

	// Token: 0x04004D16 RID: 19734
	[SerializeField]
	private NexusGroupId defaultNexusGroupId;

	// Token: 0x04004D17 RID: 19735
	[OnEnterPlay_SetNull]
	private static volatile CustomMapLoader instance;

	// Token: 0x04004D18 RID: 19736
	[OnEnterPlay_Set(false)]
	private static bool hasInstance;

	// Token: 0x04004D19 RID: 19737
	public Transform CustomMapsDefaultSpawnLocation;

	// Token: 0x04004D1A RID: 19738
	public CustomMapAccessDoor accessDoor;

	// Token: 0x04004D1B RID: 19739
	[FormerlySerializedAs("networkTrigger")]
	public GameObject publicJoinTrigger;

	// Token: 0x04004D1C RID: 19740
	[SerializeField]
	private BetterDayNightManager dayNightManager;

	// Token: 0x04004D1D RID: 19741
	[SerializeField]
	private GhostReactorManager ghostReactorManager;

	// Token: 0x04004D1E RID: 19742
	[SerializeField]
	private GameObject placeholderParent;

	// Token: 0x04004D1F RID: 19743
	[SerializeField]
	private GliderHoldable[] leafGliders;

	// Token: 0x04004D20 RID: 19744
	[SerializeField]
	private GameObject leafGlider;

	// Token: 0x04004D21 RID: 19745
	[SerializeField]
	private GameObject gliderWindVolume;

	// Token: 0x04004D22 RID: 19746
	[FormerlySerializedAs("waterVolume")]
	[SerializeField]
	private GameObject waterVolumePrefab;

	// Token: 0x04004D23 RID: 19747
	[SerializeField]
	private WaterParameters defaultWaterParameters;

	// Token: 0x04004D24 RID: 19748
	[SerializeField]
	private WaterParameters defaultLavaParameters;

	// Token: 0x04004D25 RID: 19749
	[FormerlySerializedAs("forceVolume")]
	[SerializeField]
	private GameObject forceVolumePrefab;

	// Token: 0x04004D26 RID: 19750
	[SerializeField]
	private GameObject atmPrefab;

	// Token: 0x04004D27 RID: 19751
	[SerializeField]
	private GameObject atmNoShellPrefab;

	// Token: 0x04004D28 RID: 19752
	[SerializeField]
	private GameObject storeDisplayStandPrefab;

	// Token: 0x04004D29 RID: 19753
	[SerializeField]
	private GameObject storeCheckoutCounterPrefab;

	// Token: 0x04004D2A RID: 19754
	[SerializeField]
	private GameObject storeTryOnConsolePrefab;

	// Token: 0x04004D2B RID: 19755
	[SerializeField]
	private GameObject storeTryOnAreaPrefab;

	// Token: 0x04004D2C RID: 19756
	[SerializeField]
	private GameObject hoverboardDispenserPrefab;

	// Token: 0x04004D2D RID: 19757
	[SerializeField]
	private GameObject ropeSwingPrefab;

	// Token: 0x04004D2E RID: 19758
	[SerializeField]
	private GameObject ziplinePrefab;

	// Token: 0x04004D2F RID: 19759
	[SerializeField]
	private GameObject reviveStationPrefab;

	// Token: 0x04004D30 RID: 19760
	[SerializeField]
	private GameObject zoneShaderSettingsTrigger;

	// Token: 0x04004D31 RID: 19761
	[SerializeField]
	private AudioMixerGroup masterAudioMixer;

	// Token: 0x04004D32 RID: 19762
	[SerializeField]
	private ZoneShaderSettings customMapZoneShaderSettings;

	// Token: 0x04004D33 RID: 19763
	[SerializeField]
	private CompositeTriggerEvents compositeTryOnArea;

	// Token: 0x04004D34 RID: 19764
	[SerializeField]
	private GameObject virtualStumpMesh;

	// Token: 0x04004D35 RID: 19765
	[SerializeField]
	private List<GameModeType> availableModesForOldMaps;

	// Token: 0x04004D36 RID: 19766
	[SerializeField]
	private GameModeType defaultGameModeForNonCustomOldMaps;

	// Token: 0x04004D37 RID: 19767
	public TMP_FontAsset DefaultFont;

	// Token: 0x04004D38 RID: 19768
	private static readonly int numObjectsToProcessPerFrame = 5;

	// Token: 0x04004D39 RID: 19769
	private static readonly List<int> APPROVED_LAYERS;

	// Token: 0x04004D3A RID: 19770
	private static bool isLoading;

	// Token: 0x04004D3B RID: 19771
	private static bool isUnloading;

	// Token: 0x04004D3C RID: 19772
	private static bool runningAsyncLoad;

	// Token: 0x04004D3D RID: 19773
	private static long attemptedLoadID;

	// Token: 0x04004D3E RID: 19774
	private static string attemptedSceneToLoad;

	// Token: 0x04004D3F RID: 19775
	private static bool shouldAbortMapLoading;

	// Token: 0x04004D40 RID: 19776
	private static bool shouldAbortSceneLoad;

	// Token: 0x04004D41 RID: 19777
	private static bool errorEncounteredDuringLoad;

	// Token: 0x04004D42 RID: 19778
	private static Action unloadMapCallback;

	// Token: 0x04004D43 RID: 19779
	private static string cachedExceptionMessage;

	// Token: 0x04004D44 RID: 19780
	private static AssetBundle mapBundle;

	// Token: 0x04004D45 RID: 19781
	private static List<string> initialSceneNames;

	// Token: 0x04004D46 RID: 19782
	private static List<int> initialSceneIndexes;

	// Token: 0x04004D47 RID: 19783
	private static byte maxPlayersForMap;

	// Token: 0x04004D48 RID: 19784
	private static ModId loadedMapModId;

	// Token: 0x04004D49 RID: 19785
	private static long loadedMapModFileId;

	// Token: 0x04004D4A RID: 19786
	private static MapPackageInfo loadedMapPackageInfo;

	// Token: 0x04004D4B RID: 19787
	private static string cachedLuauScript;

	// Token: 0x04004D4C RID: 19788
	private static bool devModeEnabled;

	// Token: 0x04004D4D RID: 19789
	private static bool disableHoldingHandsAllModes;

	// Token: 0x04004D4E RID: 19790
	private static bool disableHoldingHandsCustomMode;

	// Token: 0x04004D4F RID: 19791
	private static Action<MapLoadStatus, int, string> mapLoadProgressCallback;

	// Token: 0x04004D50 RID: 19792
	private static Action<bool> mapLoadFinishedCallback;

	// Token: 0x04004D51 RID: 19793
	private static Coroutine zoneLoadingCoroutine;

	// Token: 0x04004D52 RID: 19794
	private static Action<string> sceneLoadedCallback;

	// Token: 0x04004D53 RID: 19795
	private static Action<string> sceneUnloadedCallback;

	// Token: 0x04004D54 RID: 19796
	private static List<CustomMapLoader.LoadZoneRequest> queuedLoadZoneRequests;

	// Token: 0x04004D55 RID: 19797
	private static string[] assetBundleSceneFilePaths;

	// Token: 0x04004D56 RID: 19798
	private static List<string> loadedSceneFilePaths;

	// Token: 0x04004D57 RID: 19799
	private static List<string> loadedSceneNames;

	// Token: 0x04004D58 RID: 19800
	private static List<int> loadedSceneIndexes;

	// Token: 0x04004D59 RID: 19801
	private Coroutine loadScenesCoroutine;

	// Token: 0x04004D5A RID: 19802
	private static int leafGliderIndex;

	// Token: 0x04004D5B RID: 19803
	private static bool usingDynamicLighting;

	// Token: 0x04004D5C RID: 19804
	private static bool refreshReviveStations;

	// Token: 0x04004D5D RID: 19805
	private static int totalObjectsInLoadingScene;

	// Token: 0x04004D5E RID: 19806
	private static int objectsProcessedForLoadingScene;

	// Token: 0x04004D5F RID: 19807
	private static int objectsProcessedThisFrame;

	// Token: 0x04004D60 RID: 19808
	private static List<Component> initializePhaseTwoComponents;

	// Token: 0x04004D61 RID: 19809
	private static List<MapEntity> entitiesToCreate;

	// Token: 0x04004D62 RID: 19810
	private static LightmapData[] lightmaps;

	// Token: 0x04004D63 RID: 19811
	private static List<Texture2D> lightmapsToKeep;

	// Token: 0x04004D64 RID: 19812
	private static List<GameObject> placeholderReplacements;

	// Token: 0x04004D65 RID: 19813
	private static GameObject customMapATM;

	// Token: 0x04004D66 RID: 19814
	private static List<GameObject> storeCheckouts;

	// Token: 0x04004D67 RID: 19815
	private static List<GameObject> storeDisplayStands;

	// Token: 0x04004D68 RID: 19816
	private static List<GameObject> storeTryOnConsoles;

	// Token: 0x04004D69 RID: 19817
	private static List<GameObject> storeTryOnAreas;

	// Token: 0x04004D6A RID: 19818
	private static List<Component> teleporters;

	// Token: 0x04004D6B RID: 19819
	private string dontDestroyOnLoadSceneName;

	// Token: 0x04004D6C RID: 19820
	private static readonly List<Type> componentAllowlist;

	// Token: 0x04004D6D RID: 19821
	private static readonly List<string> componentTypeStringAllowList;

	// Token: 0x04004D6E RID: 19822
	private static readonly Type[] badComponents;

	// Token: 0x02000940 RID: 2368
	private struct LoadZoneRequest
	{
		// Token: 0x04004D70 RID: 19824
		public int[] sceneIndexesToLoad;

		// Token: 0x04004D71 RID: 19825
		public int[] sceneIndexesToUnload;

		// Token: 0x04004D72 RID: 19826
		public Action<string> onSceneLoadedCallback;

		// Token: 0x04004D73 RID: 19827
		public Action<string> onSceneUnloadedCallback;
	}
}
