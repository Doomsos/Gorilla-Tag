using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020002E3 RID: 739
public class PerSceneRenderData : MonoBehaviour
{
	// Token: 0x06001215 RID: 4629 RVA: 0x0005F2B4 File Offset: 0x0005D4B4
	private void RefreshRenderer()
	{
		int sceneIndex = this.sceneIndex;
		new List<Renderer>();
		foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(0))
		{
			if (renderer.gameObject.scene.buildIndex == sceneIndex)
			{
				this.representativeRenderer = renderer;
				return;
			}
		}
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x06001216 RID: 4630 RVA: 0x0005F308 File Offset: 0x0005D508
	public string sceneName
	{
		get
		{
			return base.gameObject.scene.name;
		}
	}

	// Token: 0x170001BE RID: 446
	// (get) Token: 0x06001217 RID: 4631 RVA: 0x0005F328 File Offset: 0x0005D528
	public int sceneIndex
	{
		get
		{
			return base.gameObject.scene.buildIndex;
		}
	}

	// Token: 0x06001218 RID: 4632 RVA: 0x0005F348 File Offset: 0x0005D548
	private void Awake()
	{
		for (int i = 0; i < this.mRendererIndex; i++)
		{
			this.mRenderers[i] = this.gO[i].GetComponent<MeshRenderer>();
		}
	}

	// Token: 0x06001219 RID: 4633 RVA: 0x0005F37B File Offset: 0x0005D57B
	private void OnEnable()
	{
		BetterDayNightManager.Register(this);
	}

	// Token: 0x0600121A RID: 4634 RVA: 0x0005F383 File Offset: 0x0005D583
	private void OnDisable()
	{
		BetterDayNightManager.Unregister(this);
	}

	// Token: 0x0600121B RID: 4635 RVA: 0x0005F38C File Offset: 0x0005D58C
	public void AddMeshToList(GameObject _gO, MeshRenderer mR)
	{
		try
		{
			if (mR.lightmapIndex != -1)
			{
				this.gO[this.mRendererIndex] = _gO;
				this.mRendererIndex++;
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	// Token: 0x0600121C RID: 4636 RVA: 0x0005F3D8 File Offset: 0x0005D5D8
	public bool CheckShouldRepopulate()
	{
		return this.representativeRenderer.lightmapIndex != this.lastLightmapIndex;
	}

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x0600121D RID: 4637 RVA: 0x0005F3F0 File Offset: 0x0005D5F0
	public bool IsLoadingLightmaps
	{
		get
		{
			return this.resourceRequests.Count != 0;
		}
	}

	// Token: 0x170001C0 RID: 448
	// (get) Token: 0x0600121E RID: 4638 RVA: 0x0005F400 File Offset: 0x0005D600
	public int LoadingLightmapsCount
	{
		get
		{
			return this.resourceRequests.Count;
		}
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x0005F410 File Offset: 0x0005D610
	private Texture2D GetLightmap(string timeOfDay)
	{
		if (this.singleLightmap != null)
		{
			return this.singleLightmap;
		}
		Texture2D result;
		if (!this.lightmapsCache.TryGetValue(timeOfDay, ref result))
		{
			ResourceRequest request;
			if (this.resourceRequests.TryGetValue(timeOfDay, ref request))
			{
				return null;
			}
			request = Resources.LoadAsync<Texture2D>(Path.Combine(this.lightmapsResourcePath, timeOfDay));
			this.resourceRequests.Add(timeOfDay, request);
			request.completed += delegate(AsyncOperation ao)
			{
				if (this == null)
				{
					return;
				}
				this.lightmapsCache.Add(timeOfDay, (Texture2D)request.asset);
				this.resourceRequests.Remove(timeOfDay);
				if (BetterDayNightManager.instance != null)
				{
					BetterDayNightManager.instance.RequestRepopulateLightmaps();
				}
			};
		}
		return result;
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x0005F4C4 File Offset: 0x0005D6C4
	public void PopulateLightmaps(string fromTimeOfDay, string toTimeOfDay, LightmapData[] lightmaps)
	{
		LightmapData lightmapData = new LightmapData();
		lightmapData.lightmapColor = this.GetLightmap(fromTimeOfDay);
		lightmapData.lightmapDir = this.GetLightmap(toTimeOfDay);
		if (lightmapData.lightmapColor != null && lightmapData.lightmapDir != null && this.representativeRenderer.lightmapIndex < lightmaps.Length)
		{
			lightmaps[this.representativeRenderer.lightmapIndex] = lightmapData;
		}
		this.lastLightmapIndex = this.representativeRenderer.lightmapIndex;
		for (int i = 0; i < this.mRendererIndex; i++)
		{
			if (i < this.mRenderers.Length && this.mRenderers[i] != null)
			{
				this.mRenderers[i].lightmapIndex = this.lastLightmapIndex;
			}
		}
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x0005F57C File Offset: 0x0005D77C
	public void ReleaseLightmap(string oldTimeOfDay)
	{
		Texture2D texture2D;
		if (this.lightmapsCache.Remove(oldTimeOfDay, ref texture2D))
		{
			Resources.UnloadAsset(texture2D);
		}
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x0005F5A0 File Offset: 0x0005D7A0
	private void TryGetLightmapOrAsyncLoad(string momentName, Action<Texture2D> callback)
	{
		if (this.singleLightmap != null)
		{
			callback.Invoke(this.singleLightmap);
		}
		Texture2D texture2D;
		if (this.lightmapsCache.TryGetValue(momentName, ref texture2D))
		{
			callback.Invoke(texture2D);
		}
		List<Action<Texture2D>> callbacks;
		if (!this._momentName_to_callbacks.TryGetValue(momentName, ref callbacks))
		{
			callbacks = new List<Action<Texture2D>>(8);
			this._momentName_to_callbacks[momentName] = callbacks;
		}
		if (!callbacks.Contains(callback))
		{
			callbacks.Add(callback);
		}
		ResourceRequest request;
		if (this.resourceRequests.TryGetValue(momentName, ref request))
		{
			return;
		}
		request = Resources.LoadAsync<Texture2D>(Path.Combine(this.lightmapsResourcePath, momentName));
		this.resourceRequests.Add(momentName, request);
		request.completed += delegate(AsyncOperation ao)
		{
			if (this == null || ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			Texture2D texture2D2 = (Texture2D)request.asset;
			this.lightmapsCache.Add(momentName, texture2D2);
			this.resourceRequests.Remove(momentName);
			foreach (Action<Texture2D> action in callbacks)
			{
				if (action != null)
				{
					action.Invoke(texture2D2);
				}
			}
			callbacks.Clear();
		};
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x0005F6B4 File Offset: 0x0005D8B4
	public bool IsLightmapWithNameLoaded(string lightmapName)
	{
		if (this.singleLightmap != null)
		{
			return true;
		}
		string text;
		string text2;
		this.GetFromAndToLightmapNames(out text, out text2);
		return !string.IsNullOrEmpty(lightmapName) && ((!string.IsNullOrEmpty(text) && text == lightmapName) || (!string.IsNullOrEmpty(text2) && text2 == lightmapName));
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x0005F70C File Offset: 0x0005D90C
	public bool IsLightmapsWithNamesLoaded(string fromLightmapName, string toLightmapName)
	{
		if (this.singleLightmap != null)
		{
			return true;
		}
		string text;
		string text2;
		this.GetFromAndToLightmapNames(out text, out text2);
		return !string.IsNullOrEmpty(fromLightmapName) && !string.IsNullOrEmpty(toLightmapName) && !string.IsNullOrEmpty(text) && text == fromLightmapName && !string.IsNullOrEmpty(text2) && text2 == toLightmapName;
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x0005F768 File Offset: 0x0005D968
	public void GetFromAndToLightmapNames(out string fromLightmapName, out string toLightmapName)
	{
		if (this.singleLightmap != null)
		{
			fromLightmapName = null;
			toLightmapName = null;
			return;
		}
		LightmapData[] lightmaps = LightmapSettings.lightmaps;
		if (this.representativeRenderer.lightmapIndex < 0 || this.representativeRenderer.lightmapIndex >= lightmaps.Length)
		{
			fromLightmapName = null;
			toLightmapName = null;
			return;
		}
		Texture2D lightmapColor = lightmaps[this.representativeRenderer.lightmapIndex].lightmapColor;
		Texture2D lightmapDir = lightmaps[this.representativeRenderer.lightmapIndex].lightmapDir;
		fromLightmapName = ((lightmapColor != null) ? lightmapColor.name : null);
		toLightmapName = ((lightmapDir != null) ? lightmapDir.name : null);
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x0005F804 File Offset: 0x0005DA04
	public static void g_StartAllScenesPopulateLightmaps(string fromLightmapName, string toLightmapName)
	{
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Clear();
		PerSceneRenderData[] array = Object.FindObjectsByType<PerSceneRenderData>(0);
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.UnionWith(array);
		foreach (PerSceneRenderData perSceneRenderData in array)
		{
			perSceneRenderData.StartPopulateLightmaps(fromLightmapName, toLightmapName);
			perSceneRenderData.OnPopulateToAndFromLightmapsCompleted = (Action<PerSceneRenderData>)Delegate.Combine(perSceneRenderData.OnPopulateToAndFromLightmapsCompleted, new Action<PerSceneRenderData>(PerSceneRenderData._g_AllScenesPopulateLightmaps_OnOneCompleted));
		}
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x0005F86C File Offset: 0x0005DA6C
	private static void _g_AllScenesPopulateLightmaps_OnOneCompleted(PerSceneRenderData perSceneRenderData)
	{
		int count = PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Remove(perSceneRenderData);
		int count2 = PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		if (count2 == 0 && count2 != count)
		{
			Action action = PerSceneRenderData.g_OnAllScenesPopulateLightmapsCompleted;
			if (action == null)
			{
				return;
			}
			action.Invoke();
		}
	}

	// Token: 0x170001C1 RID: 449
	// (get) Token: 0x06001228 RID: 4648 RVA: 0x0005F8B1 File Offset: 0x0005DAB1
	public static int g_AllScenesPopulatingLightmapsLoadCount
	{
		get
		{
			return PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Count;
		}
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x0005F8C0 File Offset: 0x0005DAC0
	public void StartPopulateLightmaps(string fromMomentName, string toMomentName)
	{
		PerSceneRenderData._g_allScenesPopulateLightmaps_renderDatasHashSet.Clear();
		this._populateLightmaps_fromMomentLightmap = null;
		this._populateLightmaps_toMomentLightmap = null;
		this._populateLightmaps_fromMomentName = fromMomentName;
		this._populateLightmaps_toMomentName = toMomentName;
		this.TryGetLightmapOrAsyncLoad(fromMomentName, new Action<Texture2D>(this._PopulateLightmaps_OnLoadLightmap));
		this.TryGetLightmapOrAsyncLoad(toMomentName, new Action<Texture2D>(this._PopulateLightmaps_OnLoadLightmap));
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x0005F91C File Offset: 0x0005DB1C
	private void _PopulateLightmaps_OnLoadLightmap(Texture2D lightmapTex)
	{
		if (this == null || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this._populateLightmaps_fromMomentName != lightmapTex.name)
		{
			this._populateLightmaps_fromMomentLightmap = lightmapTex;
		}
		if (this._populateLightmaps_toMomentName != lightmapTex.name)
		{
			this._populateLightmaps_toMomentLightmap = lightmapTex;
		}
		if (this._populateLightmaps_fromMomentLightmap != null && this._populateLightmaps_toMomentLightmap != null)
		{
			LightmapData[] lightmaps = LightmapSettings.lightmaps;
			LightmapData lightmapData = new LightmapData
			{
				lightmapColor = this._populateLightmaps_fromMomentLightmap,
				lightmapDir = this._populateLightmaps_toMomentLightmap
			};
			if (this.representativeRenderer.lightmapIndex >= 0 && this.representativeRenderer.lightmapIndex < lightmaps.Length)
			{
				lightmaps[this.representativeRenderer.lightmapIndex] = lightmapData;
			}
			LightmapSettings.lightmaps = lightmaps;
			this.lastLightmapIndex = this.representativeRenderer.lightmapIndex;
			for (int i = 0; i < this.mRendererIndex; i++)
			{
				if (i < this.mRenderers.Length && this.mRenderers[i] != null)
				{
					this.mRenderers[i].lightmapIndex = this.lastLightmapIndex;
				}
			}
			Action<PerSceneRenderData> onPopulateToAndFromLightmapsCompleted = this.OnPopulateToAndFromLightmapsCompleted;
			if (onPopulateToAndFromLightmapsCompleted == null)
			{
				return;
			}
			onPopulateToAndFromLightmapsCompleted.Invoke(this);
		}
	}

	// Token: 0x040016B7 RID: 5815
	public Renderer representativeRenderer;

	// Token: 0x040016B8 RID: 5816
	public string lightmapsResourcePath;

	// Token: 0x040016B9 RID: 5817
	public Texture2D singleLightmap;

	// Token: 0x040016BA RID: 5818
	private int lastLightmapIndex = -1;

	// Token: 0x040016BB RID: 5819
	public GameObject[] gO = new GameObject[5000];

	// Token: 0x040016BC RID: 5820
	public MeshRenderer[] mRenderers = new MeshRenderer[5000];

	// Token: 0x040016BD RID: 5821
	public int mRendererIndex;

	// Token: 0x040016BE RID: 5822
	private readonly Dictionary<string, ResourceRequest> resourceRequests = new Dictionary<string, ResourceRequest>(8);

	// Token: 0x040016BF RID: 5823
	private readonly Dictionary<string, Texture2D> lightmapsCache = new Dictionary<string, Texture2D>(8);

	// Token: 0x040016C0 RID: 5824
	private Dictionary<string, List<Action<Texture2D>>> _momentName_to_callbacks = new Dictionary<string, List<Action<Texture2D>>>(8);

	// Token: 0x040016C1 RID: 5825
	private static readonly HashSet<PerSceneRenderData> _g_allScenesPopulateLightmaps_renderDatasHashSet = new HashSet<PerSceneRenderData>(32);

	// Token: 0x040016C2 RID: 5826
	public static Action g_OnAllScenesPopulateLightmapsCompleted;

	// Token: 0x040016C3 RID: 5827
	private string _populateLightmaps_fromMomentName;

	// Token: 0x040016C4 RID: 5828
	private string _populateLightmaps_toMomentName;

	// Token: 0x040016C5 RID: 5829
	private Texture2D _populateLightmaps_fromMomentLightmap;

	// Token: 0x040016C6 RID: 5830
	private Texture2D _populateLightmaps_toMomentLightmap;

	// Token: 0x040016C7 RID: 5831
	public Action<PerSceneRenderData> OnPopulateToAndFromLightmapsCompleted;
}
