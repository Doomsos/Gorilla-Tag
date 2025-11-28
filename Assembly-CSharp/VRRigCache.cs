using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000767 RID: 1895
internal class VRRigCache : MonoBehaviour
{
	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x06003121 RID: 12577 RVA: 0x0010B2B2 File Offset: 0x001094B2
	// (set) Token: 0x06003122 RID: 12578 RVA: 0x0010B2B9 File Offset: 0x001094B9
	public static VRRigCache Instance { get; private set; }

	// Token: 0x1700045E RID: 1118
	// (get) Token: 0x06003123 RID: 12579 RVA: 0x0010B2C1 File Offset: 0x001094C1
	public Transform NetworkParent
	{
		get
		{
			return this.networkParent;
		}
	}

	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x06003124 RID: 12580 RVA: 0x0010B2C9 File Offset: 0x001094C9
	// (set) Token: 0x06003125 RID: 12581 RVA: 0x0010B2D0 File Offset: 0x001094D0
	public static bool isInitialized { get; private set; }

	// Token: 0x14000051 RID: 81
	// (add) Token: 0x06003126 RID: 12582 RVA: 0x0010B2D8 File Offset: 0x001094D8
	// (remove) Token: 0x06003127 RID: 12583 RVA: 0x0010B30C File Offset: 0x0010950C
	public static event Action OnActiveRigsChanged;

	// Token: 0x14000052 RID: 82
	// (add) Token: 0x06003128 RID: 12584 RVA: 0x0010B340 File Offset: 0x00109540
	// (remove) Token: 0x06003129 RID: 12585 RVA: 0x0010B374 File Offset: 0x00109574
	public static event Action OnPostInitialize;

	// Token: 0x14000053 RID: 83
	// (add) Token: 0x0600312A RID: 12586 RVA: 0x0010B3A8 File Offset: 0x001095A8
	// (remove) Token: 0x0600312B RID: 12587 RVA: 0x0010B3DC File Offset: 0x001095DC
	public static event Action OnPostSpawnRig;

	// Token: 0x14000054 RID: 84
	// (add) Token: 0x0600312C RID: 12588 RVA: 0x0010B410 File Offset: 0x00109610
	// (remove) Token: 0x0600312D RID: 12589 RVA: 0x0010B444 File Offset: 0x00109644
	public static event Action<RigContainer> OnRigActivated;

	// Token: 0x14000055 RID: 85
	// (add) Token: 0x0600312E RID: 12590 RVA: 0x0010B478 File Offset: 0x00109678
	// (remove) Token: 0x0600312F RID: 12591 RVA: 0x0010B4AC File Offset: 0x001096AC
	public static event Action<RigContainer> OnRigDeactivated;

	// Token: 0x14000056 RID: 86
	// (add) Token: 0x06003130 RID: 12592 RVA: 0x0010B4E0 File Offset: 0x001096E0
	// (remove) Token: 0x06003131 RID: 12593 RVA: 0x0010B514 File Offset: 0x00109714
	public static event Action<RigContainer> OnRigNameChanged;

	// Token: 0x06003132 RID: 12594 RVA: 0x0010B548 File Offset: 0x00109748
	private void Awake()
	{
		this.InitializeVRRigCache();
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			if (this.localRig.Rig.bodyRenderer != null)
			{
				this.localRig.Rig.bodyRenderer.SetupAsLocalPlayerBody();
			}
		}
		TickSystemTimer ensureNetworkObjectTimer = this.m_ensureNetworkObjectTimer;
		ensureNetworkObjectTimer.callback = (Action)Delegate.Combine(ensureNetworkObjectTimer.callback, new Action(this.InstantiateNetworkObject));
	}

	// Token: 0x06003133 RID: 12595 RVA: 0x0010B5F8 File Offset: 0x001097F8
	private void OnDestroy()
	{
		if (VRRigCache.Instance == this)
		{
			VRRigCache.Instance = null;
		}
		VRRigCache.isInitialized = false;
		if (this.localRig != null && this.localRig.Rig != null)
		{
			VRRig rig = this.localRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		}
	}

	// Token: 0x06003134 RID: 12596 RVA: 0x0010B664 File Offset: 0x00109864
	public void InitializeVRRigCache()
	{
		if (VRRigCache.isInitialized || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (VRRigCache.Instance != null && VRRigCache.Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		VRRigCache.Instance = this;
		if (this.rigParent == null)
		{
			this.rigParent = base.transform;
		}
		if (this.networkParent == null)
		{
			this.networkParent = base.transform;
		}
		for (int i = 0; i < this.rigAmount; i++)
		{
			RigContainer rigContainer = this.SpawnRig();
			VRRigCache.freeRigs.Enqueue(rigContainer);
			rigContainer.Rig.BuildInitialize();
			rigContainer.Rig.transform.parent = null;
		}
		VRRigCache.isInitialized = true;
		Action onPostInitialize = VRRigCache.OnPostInitialize;
		if (onPostInitialize != null)
		{
			onPostInitialize.Invoke();
		}
		Action onPostSpawnRig = VRRigCache.OnPostSpawnRig;
		if (onPostSpawnRig == null)
		{
			return;
		}
		onPostSpawnRig.Invoke();
	}

	// Token: 0x06003135 RID: 12597 RVA: 0x0010B73F File Offset: 0x0010993F
	private RigContainer SpawnRig()
	{
		if (this.rigTemplate.activeSelf)
		{
			this.rigTemplate.SetActive(false);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.rigTemplate, this.rigParent, false);
		if (gameObject == null)
		{
			return null;
		}
		return gameObject.GetComponent<RigContainer>();
	}

	// Token: 0x06003136 RID: 12598 RVA: 0x0010B777 File Offset: 0x00109977
	internal bool TryGetVrrig(Player targetPlayer, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayer.ActorNumber), out playerRig);
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x0010B790 File Offset: 0x00109990
	internal bool TryGetVrrig(int targetPlayerId, out RigContainer playerRig)
	{
		return this.TryGetVrrig(NetworkSystem.Instance.GetPlayer(targetPlayerId), out playerRig);
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x0010B7A4 File Offset: 0x001099A4
	internal bool TryGetVrrig(NetPlayer targetPlayer, out RigContainer playerRig)
	{
		playerRig = null;
		if (ApplicationQuittingState.IsQuitting)
		{
			return false;
		}
		if (targetPlayer == null || targetPlayer.IsNull)
		{
			GTDev.LogError<string>("[GT/VRRigCache]  ERROR!!!  TryGetVrrig: Supplied targetPlayer cannot be null!", null);
			return false;
		}
		if (targetPlayer.IsLocal)
		{
			playerRig = this.localRig;
			return true;
		}
		if (!targetPlayer.InRoom)
		{
			return false;
		}
		if (!VRRigCache.rigsInUse.TryGetValue(targetPlayer, ref playerRig))
		{
			if (VRRigCache.freeRigs.Count <= 0)
			{
				return false;
			}
			playerRig = VRRigCache.freeRigs.Dequeue();
			playerRig.Creator = targetPlayer;
			VRRigCache.rigsInUse.Add(targetPlayer, playerRig);
			VRRig rig = playerRig.Rig;
			rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
			VRRig rig2 = playerRig.Rig;
			rig2.OnNameChanged = (Action<RigContainer>)Delegate.Combine(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
			playerRig.gameObject.SetActive(true);
			playerRig.RigEvents.SendPostEnableEvent();
			GamePlayer.UpdateStaticLookupCaches();
			Action<RigContainer> onRigActivated = VRRigCache.OnRigActivated;
			if (onRigActivated != null)
			{
				onRigActivated.Invoke(playerRig);
			}
			Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
			if (onActiveRigsChanged != null)
			{
				onActiveRigsChanged.Invoke();
			}
		}
		return true;
	}

	// Token: 0x06003139 RID: 12601 RVA: 0x0010B8BC File Offset: 0x00109ABC
	private void AddRigToGorillaParent(NetPlayer player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (instance == null)
		{
			return;
		}
		if (!instance.vrrigs.Contains(vrrig))
		{
			instance.vrrigs.Add(vrrig);
		}
		if (!instance.vrrigDict.ContainsKey(player))
		{
			instance.vrrigDict.Add(player, vrrig);
			return;
		}
		instance.vrrigDict[player] = vrrig;
	}

	// Token: 0x0600313A RID: 12602 RVA: 0x0010B920 File Offset: 0x00109B20
	public void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		if (newPlayer.ActorNumber == -1)
		{
			Debug.LogError("LocalPlayer returned, vrrig no correctly initialised");
		}
		RigContainer rigContainer;
		if (this.TryGetVrrig(newPlayer, out rigContainer))
		{
			this.AddRigToGorillaParent(newPlayer, rigContainer.Rig);
			GamePlayer.UpdateStaticLookupCaches();
			Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
			if (onActiveRigsChanged == null)
			{
				return;
			}
			onActiveRigsChanged.Invoke();
		}
	}

	// Token: 0x0600313B RID: 12603 RVA: 0x0010B96C File Offset: 0x00109B6C
	public void OnJoinedRoom()
	{
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			RigContainer rigContainer;
			if (this.TryGetVrrig(netPlayer, out rigContainer))
			{
				this.AddRigToGorillaParent(netPlayer, rigContainer.Rig);
			}
		}
		this.m_ensureNetworkObjectTimer.Start();
		GamePlayer.UpdateStaticLookupCaches();
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged.Invoke();
	}

	// Token: 0x0600313C RID: 12604 RVA: 0x0010B9D0 File Offset: 0x00109BD0
	private void RemoveRigFromGorillaParent(NetPlayer player, VRRig vrrig)
	{
		GorillaParent instance = GorillaParent.instance;
		if (instance == null)
		{
			return;
		}
		if (instance.vrrigs.Contains(vrrig))
		{
			instance.vrrigs.Remove(vrrig);
		}
		if (instance.vrrigDict.ContainsKey(player))
		{
			instance.vrrigDict.Remove(player);
		}
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x0010BA28 File Offset: 0x00109C28
	public void OnPlayerLeftRoom(NetPlayer leavingPlayer)
	{
		if (leavingPlayer.IsNull)
		{
			Debug.LogError("Leaving players NetPlayer is Null");
			this.CheckForMissingPlayer();
		}
		RigContainer rigContainer;
		if (!VRRigCache.rigsInUse.TryGetValue(leavingPlayer, ref rigContainer))
		{
			this.LogError("failed to find player's vrrig who left " + leavingPlayer.UserId);
			return;
		}
		rigContainer.gameObject.Disable();
		VRRig rig = rigContainer.Rig;
		rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
		VRRigCache.freeRigs.Enqueue(rigContainer);
		VRRigCache.rigsInUse.Remove(leavingPlayer);
		this.RemoveRigFromGorillaParent(leavingPlayer, rigContainer.Rig);
		GamePlayer.UpdateStaticLookupCaches();
		Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
		if (onRigDeactivated != null)
		{
			onRigDeactivated.Invoke(rigContainer);
		}
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged.Invoke();
	}

	// Token: 0x0600313E RID: 12606 RVA: 0x0010BAE8 File Offset: 0x00109CE8
	private void CheckForMissingPlayer()
	{
		foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
		{
			if (keyValuePair.Key == null || keyValuePair.Value == null)
			{
				Debug.LogError("Somehow null reference in rigsInUse");
			}
			else if (!keyValuePair.Key.InRoom)
			{
				keyValuePair.Value.gameObject.Disable();
				VRRig rig = keyValuePair.Value.Rig;
				rig.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig.OnNameChanged, VRRigCache.OnRigNameChanged);
				VRRigCache.freeRigs.Enqueue(keyValuePair.Value);
				VRRigCache.rigsInUse.Remove(keyValuePair.Key);
				this.RemoveRigFromGorillaParent(keyValuePair.Key, keyValuePair.Value.Rig);
				GamePlayer.UpdateStaticLookupCaches();
				Action<RigContainer> onRigDeactivated = VRRigCache.OnRigDeactivated;
				if (onRigDeactivated != null)
				{
					onRigDeactivated.Invoke(keyValuePair.Value);
				}
				Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
				if (onActiveRigsChanged != null)
				{
					onActiveRigsChanged.Invoke();
				}
			}
		}
	}

	// Token: 0x0600313F RID: 12607 RVA: 0x0010BC10 File Offset: 0x00109E10
	public void OnLeftRoom()
	{
		this.m_ensureNetworkObjectTimer.Stop();
		Dictionary<NetPlayer, RigContainer> dictionary;
		using (DictionaryPool<NetPlayer, RigContainer>.Get(ref dictionary))
		{
			dictionary.EnsureCapacity(VRRigCache.rigsInUse.Count);
			dictionary.Clear();
			NetPlayer netPlayer;
			RigContainer rigContainer;
			foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in VRRigCache.rigsInUse)
			{
				keyValuePair.Deconstruct(ref netPlayer, ref rigContainer);
				NetPlayer netPlayer2 = netPlayer;
				RigContainer rigContainer2 = rigContainer;
				dictionary.Add(netPlayer2, rigContainer2);
			}
			foreach (KeyValuePair<NetPlayer, RigContainer> keyValuePair in dictionary)
			{
				keyValuePair.Deconstruct(ref netPlayer, ref rigContainer);
				NetPlayer netPlayer3 = netPlayer;
				RigContainer rigContainer3 = rigContainer;
				if (!(rigContainer3 == null))
				{
					VRRig rig = VRRigCache.rigsInUse[netPlayer3].Rig;
					VRRig rig2 = rigContainer3.Rig;
					rig2.OnNameChanged = (Action<RigContainer>)Delegate.Remove(rig2.OnNameChanged, VRRigCache.OnRigNameChanged);
					rigContainer3.gameObject.Disable();
					VRRigCache.rigsInUse.Remove(netPlayer3);
					this.RemoveRigFromGorillaParent(netPlayer3, rig);
					VRRigCache.freeRigs.Enqueue(rigContainer3);
				}
			}
			GamePlayer.UpdateStaticLookupCaches();
			if (VRRigCache.OnRigDeactivated != null)
			{
				foreach (RigContainer rigContainer4 in dictionary.Values)
				{
					VRRigCache.OnRigDeactivated.Invoke(rigContainer4);
				}
			}
			Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
			if (onActiveRigsChanged != null)
			{
				onActiveRigsChanged.Invoke();
			}
		}
	}

	// Token: 0x06003140 RID: 12608 RVA: 0x0010BE10 File Offset: 0x0010A010
	[MethodImpl(256)]
	internal VRRig[] GetAllRigs()
	{
		VRRig[] array = new VRRig[VRRigCache.rigsInUse.Count + VRRigCache.freeRigs.Count];
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			array[num] = rigContainer.Rig;
			num++;
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			array[num] = rigContainer2.Rig;
			num++;
		}
		return array;
	}

	// Token: 0x06003141 RID: 12609 RVA: 0x0010BED8 File Offset: 0x0010A0D8
	[MethodImpl(256)]
	internal void GetAllUsedRigs(List<VRRig> rigs)
	{
		if (rigs == null)
		{
			return;
		}
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigs.Add(rigContainer.Rig);
		}
	}

	// Token: 0x06003142 RID: 12610 RVA: 0x0010BF38 File Offset: 0x0010A138
	[MethodImpl(256)]
	internal void GetActiveRigs(List<VRRig> rigsListToUpdate)
	{
		if (rigsListToUpdate == null)
		{
			return;
		}
		rigsListToUpdate.Clear();
		if (!VRRigCache.isInitialized)
		{
			return;
		}
		rigsListToUpdate.Add(VRRigCache.Instance.localRig.Rig);
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			rigsListToUpdate.Add(rigContainer.Rig);
		}
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x0010BFBC File Offset: 0x0010A1BC
	[MethodImpl(256)]
	internal int GetAllRigsHash()
	{
		int num = 0;
		foreach (RigContainer rigContainer in VRRigCache.rigsInUse.Values)
		{
			num += rigContainer.GetInstanceID();
		}
		foreach (RigContainer rigContainer2 in VRRigCache.freeRigs)
		{
			num += rigContainer2.GetInstanceID();
		}
		return num;
	}

	// Token: 0x06003144 RID: 12612 RVA: 0x0010C060 File Offset: 0x0010A260
	internal void InstantiateNetworkObject()
	{
		if (this.localRig.netView.IsNotNull() || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		PrefabType prefabType;
		if (!VRRigCache.Instance.GetComponent<PhotonPrefabPool>().networkPrefabs.TryGetValue("Player Network Controller", ref prefabType) || prefabType.prefab == null)
		{
			Debug.LogError("OnJoinedRoom: Unable to find player prefab to spawn");
			return;
		}
		GameObject gameObject = GTPlayer.Instance.gameObject;
		Color playerColor = this.localRig.Rig.playerColor;
		VRRigCache.rigRGBData[0] = playerColor.r;
		VRRigCache.rigRGBData[1] = playerColor.g;
		VRRigCache.rigRGBData[2] = playerColor.b;
		NetworkSystem.Instance.NetInstantiate(prefabType.prefab, gameObject.transform.position, gameObject.transform.rotation, false, 0, VRRigCache.rigRGBData, null);
	}

	// Token: 0x06003145 RID: 12613 RVA: 0x0010C143 File Offset: 0x0010A343
	internal void OnVrrigSerializerSuccesfullySpawned()
	{
		GamePlayer.UpdateStaticLookupCaches();
		Action onActiveRigsChanged = VRRigCache.OnActiveRigsChanged;
		if (onActiveRigsChanged == null)
		{
			return;
		}
		onActiveRigsChanged.Invoke();
	}

	// Token: 0x06003146 RID: 12614 RVA: 0x00002789 File Offset: 0x00000989
	private void LogInfo(string log)
	{
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x00002789 File Offset: 0x00000989
	private void LogWarning(string log)
	{
	}

	// Token: 0x06003148 RID: 12616 RVA: 0x00002789 File Offset: 0x00000989
	private void LogError(string log)
	{
	}

	// Token: 0x04003FE2 RID: 16354
	private const string preLog = "[GT/VRRigCache] ";

	// Token: 0x04003FE3 RID: 16355
	private const string preErr = "[GT/VRRigCache]  ERROR!!!  ";

	// Token: 0x04003FE4 RID: 16356
	private const string preErrBeta = "[GT/VRRigCache]  ERROR!!!  (beta only log) ";

	// Token: 0x04003FE5 RID: 16357
	private const string preErrEd = "[GT/VRRigCache]  ERROR!!!  (editor only log) ";

	// Token: 0x04003FE7 RID: 16359
	public RigContainer localRig;

	// Token: 0x04003FE8 RID: 16360
	[SerializeField]
	private Transform rigParent;

	// Token: 0x04003FE9 RID: 16361
	[SerializeField]
	private Transform networkParent;

	// Token: 0x04003FEA RID: 16362
	[SerializeField]
	private GameObject rigTemplate;

	// Token: 0x04003FEB RID: 16363
	private int rigAmount = 9;

	// Token: 0x04003FEC RID: 16364
	[SerializeField]
	private TickSystemTimer m_ensureNetworkObjectTimer = new TickSystemTimer(0.1f);

	// Token: 0x04003FED RID: 16365
	[OnEnterPlay_Clear]
	private static Queue<RigContainer> freeRigs = new Queue<RigContainer>(10);

	// Token: 0x04003FEE RID: 16366
	[OnEnterPlay_Clear]
	private static Dictionary<NetPlayer, RigContainer> rigsInUse = new Dictionary<NetPlayer, RigContainer>(10);

	// Token: 0x04003FF6 RID: 16374
	private static object[] rigRGBData = new object[]
	{
		0f,
		0f,
		0f
	};
}
