using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000336 RID: 822
public class ZoneManagement : MonoBehaviour
{
	// Token: 0x14000027 RID: 39
	// (add) Token: 0x060013CB RID: 5067 RVA: 0x00072D98 File Offset: 0x00070F98
	// (remove) Token: 0x060013CC RID: 5068 RVA: 0x00072DCC File Offset: 0x00070FCC
	public static event ZoneManagement.ZoneChangeEvent OnZoneChange;

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x060013CD RID: 5069 RVA: 0x00072DFF File Offset: 0x00070FFF
	// (set) Token: 0x060013CE RID: 5070 RVA: 0x00072E07 File Offset: 0x00071007
	public bool hasInstance { get; private set; }

	// Token: 0x060013CF RID: 5071 RVA: 0x00072E10 File Offset: 0x00071010
	private void Awake()
	{
		if (ZoneManagement.instance == null)
		{
			this.Initialize();
			return;
		}
		if (ZoneManagement.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060013D0 RID: 5072 RVA: 0x00072E3E File Offset: 0x0007103E
	public static void SetActiveZone(GTZone zone)
	{
		ZoneManagement.SetActiveZones(new GTZone[]
		{
			zone
		});
	}

	// Token: 0x060013D1 RID: 5073 RVA: 0x00072E50 File Offset: 0x00071050
	public static void SetActiveZones(GTZone[] zones)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		if (zones == null || zones.Length == 0)
		{
			return;
		}
		ZoneManagement.instance.SetZones(zones);
		Action action = ZoneManagement.instance.onZoneChanged;
		if (action != null)
		{
			action.Invoke();
		}
		if (ZoneManagement.OnZoneChange != null)
		{
			ZoneManagement.OnZoneChange(ZoneManagement.instance.zones);
		}
	}

	// Token: 0x060013D2 RID: 5074 RVA: 0x00072EB4 File Offset: 0x000710B4
	public static bool IsInZone(GTZone zone)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneData zoneData = ZoneManagement.instance.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x060013D3 RID: 5075 RVA: 0x00072EEA File Offset: 0x000710EA
	public GameObject GetPrimaryGameObject(GTZone zone)
	{
		return this.GetZoneData(zone).rootGameObjects[0];
	}

	// Token: 0x060013D4 RID: 5076 RVA: 0x00072EFA File Offset: 0x000710FA
	public static void AddSceneToForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Add(sceneName);
	}

	// Token: 0x060013D5 RID: 5077 RVA: 0x00072F1F File Offset: 0x0007111F
	public static void RemoveSceneFromForceStayLoaded(string sceneName)
	{
		if (ZoneManagement.instance == null)
		{
			ZoneManagement.FindInstance();
		}
		ZoneManagement.instance.sceneForceStayLoaded.Remove(sceneName);
	}

	// Token: 0x060013D6 RID: 5078 RVA: 0x00072F44 File Offset: 0x00071144
	public static void FindInstance()
	{
		ZoneManagement zoneManagement = Object.FindAnyObjectByType<ZoneManagement>();
		if (zoneManagement == null)
		{
			throw new NullReferenceException("Unable to find ZoneManagement object in scene.");
		}
		Debug.LogWarning("ZoneManagement accessed before MonoBehaviour awake function called; consider delaying zone management functions to avoid FindObject lookup.");
		zoneManagement.Initialize();
	}

	// Token: 0x060013D7 RID: 5079 RVA: 0x00072F70 File Offset: 0x00071170
	public bool IsSceneLoaded(GTZone gtZone)
	{
		foreach (ZoneData zoneData in this.zones)
		{
			if (zoneData.zone == gtZone && this.scenesLoaded.Contains(zoneData.sceneName))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060013D8 RID: 5080 RVA: 0x00072FB8 File Offset: 0x000711B8
	public bool IsZoneActive(GTZone zone)
	{
		ZoneData zoneData = this.GetZoneData(zone);
		return zoneData != null && zoneData.active;
	}

	// Token: 0x060013D9 RID: 5081 RVA: 0x00072FD8 File Offset: 0x000711D8
	public HashSet<string> GetAllLoadedScenes()
	{
		return this.scenesLoaded;
	}

	// Token: 0x060013DA RID: 5082 RVA: 0x00072FE0 File Offset: 0x000711E0
	public bool IsSceneLoaded(string sceneName)
	{
		return this.scenesLoaded.Contains(sceneName);
	}

	// Token: 0x060013DB RID: 5083 RVA: 0x00072FF0 File Offset: 0x000711F0
	private void Initialize()
	{
		ZoneManagement.instance = this;
		this.hasInstance = true;
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		List<GameObject> list = new List<GameObject>(8);
		for (int i = 0; i < this.zones.Length; i++)
		{
			list.Clear();
			ZoneData zoneData = this.zones[i];
			if (zoneData != null && zoneData.rootGameObjects != null)
			{
				hashSet.UnionWith(zoneData.rootGameObjects);
				for (int j = 0; j < zoneData.rootGameObjects.Length; j++)
				{
					GameObject gameObject = zoneData.rootGameObjects[j];
					if (!(gameObject == null))
					{
						list.Add(gameObject);
					}
				}
				hashSet.UnionWith(list);
			}
		}
		this.allObjects = Enumerable.ToArray<GameObject>(hashSet);
		this.objectActivationState = new bool[this.allObjects.Length];
	}

	// Token: 0x060013DC RID: 5084 RVA: 0x000730AC File Offset: 0x000712AC
	private void SetZones(GTZone[] newActiveZones)
	{
		for (int i = 0; i < this.objectActivationState.Length; i++)
		{
			this.objectActivationState[i] = false;
		}
		this.activeZones.Clear();
		for (int j = 0; j < newActiveZones.Length; j++)
		{
			this.activeZones.Add(newActiveZones[j]);
		}
		this.scenesRequested.Clear();
		this.scenesRequested.Add("GorillaTag");
		float num = 0f;
		for (int k = 0; k < this.zones.Length; k++)
		{
			ZoneData zoneData = this.zones[k];
			if (zoneData == null || zoneData.rootGameObjects == null || !Enumerable.Contains<GTZone>(newActiveZones, zoneData.zone))
			{
				zoneData.active = false;
			}
			else
			{
				zoneData.active = true;
				num = Mathf.Max(num, zoneData.CameraFarClipPlane);
				if (!string.IsNullOrEmpty(zoneData.sceneName))
				{
					this.scenesRequested.Add(zoneData.sceneName);
				}
				foreach (GameObject gameObject in zoneData.rootGameObjects)
				{
					if (!(gameObject == null))
					{
						for (int m = 0; m < this.allObjects.Length; m++)
						{
							if (gameObject == this.allObjects[m])
							{
								this.objectActivationState[m] = true;
								break;
							}
						}
					}
				}
			}
		}
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		this.mainCamera.farClipPlane = num;
		int loadedSceneCount = SceneManager.loadedSceneCount;
		for (int n = 0; n < loadedSceneCount; n++)
		{
			this.scenesLoaded.Add(SceneManager.GetSceneAt(n).name);
		}
		foreach (string text in this.scenesRequested)
		{
			if (this.scenesLoaded.Add(text))
			{
				AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(text, 1);
				this._scenes_to_loadOps[text] = asyncOperation;
				asyncOperation.completed += new Action<AsyncOperation>(this.HandleOnSceneLoadCompleted);
			}
		}
		this.scenesToUnload.Clear();
		foreach (string text2 in this.scenesLoaded)
		{
			if (!this.scenesRequested.Contains(text2) && !this.sceneForceStayLoaded.Contains(text2))
			{
				this.scenesToUnload.Add(text2);
			}
		}
		foreach (string text3 in this.scenesToUnload)
		{
			this.scenesLoaded.Remove(text3);
			AsyncOperation asyncOperation2 = SceneManager.UnloadSceneAsync(text3);
			this._scenes_to_unloadOps[text3] = asyncOperation2;
		}
		for (int num2 = 0; num2 < this.objectActivationState.Length; num2++)
		{
			if (!(this.allObjects[num2] == null))
			{
				this.allObjects[num2].SetActive(this.objectActivationState[num2]);
			}
		}
	}

	// Token: 0x060013DD RID: 5085 RVA: 0x000733F0 File Offset: 0x000715F0
	private void HandleOnSceneLoadCompleted(AsyncOperation thisLoadOp)
	{
		using (Dictionary<string, AsyncOperation>.ValueCollection.Enumerator enumerator = this._scenes_to_loadOps.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.isDone)
				{
					return;
				}
			}
		}
		using (Dictionary<string, AsyncOperation>.ValueCollection.Enumerator enumerator = this._scenes_to_unloadOps.Values.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.isDone)
				{
					return;
				}
			}
		}
		Action onSceneLoadsCompleted = this.OnSceneLoadsCompleted;
		if (onSceneLoadsCompleted == null)
		{
			return;
		}
		onSceneLoadsCompleted.Invoke();
	}

	// Token: 0x060013DE RID: 5086 RVA: 0x000734A4 File Offset: 0x000716A4
	private ZoneData GetZoneData(GTZone zone)
	{
		for (int i = 0; i < this.zones.Length; i++)
		{
			if (this.zones[i].zone == zone)
			{
				return this.zones[i];
			}
		}
		return null;
	}

	// Token: 0x060013DF RID: 5087 RVA: 0x000734DE File Offset: 0x000716DE
	public static bool IsValidZoneInt(int zoneInt)
	{
		return zoneInt >= 11 && zoneInt <= 24;
	}

	// Token: 0x04001E64 RID: 7780
	public static ZoneManagement instance;

	// Token: 0x04001E66 RID: 7782
	[SerializeField]
	private ZoneData[] zones;

	// Token: 0x04001E67 RID: 7783
	private GameObject[] allObjects;

	// Token: 0x04001E68 RID: 7784
	private bool[] objectActivationState;

	// Token: 0x04001E69 RID: 7785
	public Action onZoneChanged;

	// Token: 0x04001E6A RID: 7786
	public Action OnSceneLoadsCompleted;

	// Token: 0x04001E6B RID: 7787
	public List<GTZone> activeZones = new List<GTZone>(20);

	// Token: 0x04001E6C RID: 7788
	private HashSet<string> scenesLoaded = new HashSet<string>();

	// Token: 0x04001E6D RID: 7789
	private HashSet<string> scenesRequested = new HashSet<string>();

	// Token: 0x04001E6E RID: 7790
	private HashSet<string> sceneForceStayLoaded = new HashSet<string>(8);

	// Token: 0x04001E6F RID: 7791
	private List<string> scenesToUnload = new List<string>();

	// Token: 0x04001E70 RID: 7792
	private Dictionary<string, AsyncOperation> _scenes_to_loadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x04001E71 RID: 7793
	private Dictionary<string, AsyncOperation> _scenes_to_unloadOps = new Dictionary<string, AsyncOperation>(32);

	// Token: 0x04001E72 RID: 7794
	private Camera mainCamera;

	// Token: 0x02000337 RID: 823
	// (Invoke) Token: 0x060013E2 RID: 5090
	public delegate void ZoneChangeEvent(ZoneData[] zones);
}
