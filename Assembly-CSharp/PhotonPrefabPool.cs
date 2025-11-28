using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200075F RID: 1887
public class PhotonPrefabPool : MonoBehaviour, IPunPrefabPoolVerify, IPunPrefabPool, ITickSystemPre
{
	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x060030CA RID: 12490 RVA: 0x0010A232 File Offset: 0x00108432
	// (set) Token: 0x060030CB RID: 12491 RVA: 0x0010A23A File Offset: 0x0010843A
	bool ITickSystemPre.PreTickRunning { get; set; }

	// Token: 0x060030CC RID: 12492 RVA: 0x0010A243 File Offset: 0x00108443
	private void Awake()
	{
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
	}

	// Token: 0x060030CD RID: 12493 RVA: 0x0010A260 File Offset: 0x00108460
	private void Start()
	{
		PhotonNetwork.PrefabPool = this;
		for (int i = 0; i < this.networkPrefabsData.Length; i++)
		{
			ref PrefabType ptr = ref this.networkPrefabsData[i];
			if (ptr.prefab)
			{
				if (string.IsNullOrEmpty(ptr.prefabName))
				{
					ptr.prefabName = ptr.prefab.name;
				}
				int photonViewCount = ptr.prefab.GetComponentsInChildren<PhotonView>().Length;
				ptr.photonViewCount = photonViewCount;
				this.networkPrefabs.Add(ptr.prefabName, ptr);
			}
		}
	}

	// Token: 0x060030CE RID: 12494 RVA: 0x0010A2EC File Offset: 0x001084EC
	bool IPunPrefabPoolVerify.VerifyInstantiation(Player sender, string prefabName, Vector3 position, Quaternion rotation, int[] viewIDs, out GameObject prefab)
	{
		prefab = null;
		if (viewIDs != null)
		{
			float num = 10000f;
			PrefabType prefabType;
			if (position.IsValid(num) && rotation.IsValid() && this.networkPrefabs.TryGetValue(prefabName, ref prefabType) && viewIDs.Length == prefabType.photonViewCount)
			{
				int num2 = (sender != null) ? sender.ActorNumber : 0;
				int num3 = viewIDs[0] / PhotonNetwork.MAX_VIEW_IDS;
				for (int i = 0; i < viewIDs.Length; i++)
				{
					int num4 = viewIDs[i];
					if (PhotonNetwork.ViewIDExists(num4))
					{
						return false;
					}
					for (int j = 0; j < viewIDs.Length; j++)
					{
						if (j != i && viewIDs[j] == num4)
						{
							return false;
						}
					}
					int num5 = num4 / PhotonNetwork.MAX_VIEW_IDS;
					if (num5 != num3)
					{
						return false;
					}
					if (num5 == 0)
					{
						if (!prefabType.roomObject)
						{
							return false;
						}
					}
					else if (num5 != num2)
					{
						return false;
					}
				}
				prefab = prefabType.prefab;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060030CF RID: 12495 RVA: 0x0010A3CC File Offset: 0x001085CC
	GameObject IPunPrefabPoolVerify.Instantiate(GameObject prefabInstance, Vector3 position, Quaternion rotation)
	{
		bool activeSelf = prefabInstance.activeSelf;
		if (activeSelf)
		{
			prefabInstance.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(prefabInstance, position, rotation);
		this.netInstantiedObjects.Add(gameObject);
		if (activeSelf)
		{
			prefabInstance.SetActive(true);
		}
		return gameObject;
	}

	// Token: 0x060030D0 RID: 12496 RVA: 0x0010A40C File Offset: 0x0010860C
	GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation)
	{
		PrefabType prefabType;
		if (!this.networkPrefabs.TryGetValue(prefabId, ref prefabType))
		{
			return null;
		}
		return this.Instantiate(prefabType.prefab, position, rotation);
	}

	// Token: 0x060030D1 RID: 12497 RVA: 0x0010A43C File Offset: 0x0010863C
	void IPunPrefabPool.Destroy(GameObject netObj)
	{
		if (netObj.IsNull())
		{
			return;
		}
		if (this.netInstantiedObjects.Remove(netObj))
		{
			PhotonViewCache photonViewCache;
			if (this.m_invalidCreatePool.Count < 200 && netObj.TryGetComponent<PhotonViewCache>(ref photonViewCache) && !photonViewCache.Initialized)
			{
				if (this.m_m_invalidCreatePoolLookup.Add(netObj))
				{
					this.m_invalidCreatePool.Add(netObj);
				}
				return;
			}
			Object.Destroy(netObj);
			return;
		}
		else
		{
			PhotonView photonView;
			if (!netObj.TryGetComponent<PhotonView>(ref photonView) || photonView.isRuntimeInstantiated)
			{
				Object.Destroy(netObj);
				return;
			}
			if (!this.objectsQueued.Contains(netObj))
			{
				this.objectsWaiting.Enqueue(netObj);
				this.objectsQueued.Add(netObj);
			}
			if (!this.waiting)
			{
				this.waiting = true;
				TickSystem<object>.AddPreTickCallback(this);
			}
			return;
		}
	}

	// Token: 0x060030D2 RID: 12498 RVA: 0x0010A4FC File Offset: 0x001086FC
	void ITickSystemPre.PreTick()
	{
		if (this.waiting)
		{
			this.waiting = false;
			return;
		}
		Queue<GameObject> queue = this.queueBeingProcssed;
		Queue<GameObject> queue2 = this.objectsWaiting;
		this.objectsWaiting = queue;
		this.queueBeingProcssed = queue2;
		while (this.queueBeingProcssed.Count > 0)
		{
			GameObject gameObject = this.queueBeingProcssed.Dequeue();
			this.objectsQueued.Remove(gameObject);
			if (!gameObject.IsNull())
			{
				gameObject.SetActive(true);
				gameObject.GetComponents<PhotonView>(this.tempViews);
				for (int i = 0; i < this.tempViews.Count; i++)
				{
					PhotonNetwork.RegisterPhotonView(this.tempViews[i]);
				}
			}
		}
		if (this.objectsQueued.Count < 1)
		{
			TickSystem<object>.RemovePreTickCallback(this);
			return;
		}
		this.waiting = true;
	}

	// Token: 0x060030D3 RID: 12499 RVA: 0x0010A5C0 File Offset: 0x001087C0
	private void OnLeftRoom()
	{
		foreach (GameObject gameObject in this.m_invalidCreatePool)
		{
			if (!gameObject.IsNull())
			{
				Object.Destroy(gameObject);
			}
		}
		this.m_invalidCreatePool.Clear();
		this.m_m_invalidCreatePoolLookup.Clear();
	}

	// Token: 0x060030D4 RID: 12500 RVA: 0x0010A630 File Offset: 0x00108830
	private void CheckVOIPSettings(RemoteVoiceLink voiceLink)
	{
		try
		{
			NetPlayer netPlayer = null;
			if (voiceLink.Info.UserData != null)
			{
				int num;
				if (int.TryParse(voiceLink.Info.UserData.ToString(), ref num))
				{
					netPlayer = NetworkSystem.Instance.GetPlayer(num / PhotonNetwork.MAX_VIEW_IDS);
				}
			}
			else
			{
				netPlayer = NetworkSystem.Instance.GetPlayer(voiceLink.PlayerId);
			}
			if (netPlayer != null)
			{
				RigContainer rigContainer;
				if ((voiceLink.Info.Bitrate > 20000 || voiceLink.Info.SamplingRate > 16000) && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
				{
					rigContainer.ForceMute = true;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	// Token: 0x04003FB2 RID: 16306
	[SerializeField]
	private PrefabType[] networkPrefabsData;

	// Token: 0x04003FB3 RID: 16307
	public Dictionary<string, PrefabType> networkPrefabs = new Dictionary<string, PrefabType>();

	// Token: 0x04003FB4 RID: 16308
	private Queue<GameObject> objectsWaiting = new Queue<GameObject>(20);

	// Token: 0x04003FB5 RID: 16309
	private Queue<GameObject> queueBeingProcssed = new Queue<GameObject>(20);

	// Token: 0x04003FB6 RID: 16310
	private HashSet<GameObject> objectsQueued = new HashSet<GameObject>();

	// Token: 0x04003FB7 RID: 16311
	private HashSet<GameObject> netInstantiedObjects = new HashSet<GameObject>();

	// Token: 0x04003FB8 RID: 16312
	private List<PhotonView> tempViews = new List<PhotonView>(5);

	// Token: 0x04003FB9 RID: 16313
	private List<GameObject> m_invalidCreatePool = new List<GameObject>(100);

	// Token: 0x04003FBA RID: 16314
	private HashSet<GameObject> m_m_invalidCreatePoolLookup = new HashSet<GameObject>(100);

	// Token: 0x04003FBB RID: 16315
	private bool waiting;
}
