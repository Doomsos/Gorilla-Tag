using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GorillaExtensions;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

// Token: 0x02000BEB RID: 3051
internal class PlayerCosmeticsSystem : MonoBehaviour, ITickSystemPre
{
	// Token: 0x17000702 RID: 1794
	// (get) Token: 0x06004B43 RID: 19267 RVA: 0x00188EE6 File Offset: 0x001870E6
	// (set) Token: 0x06004B44 RID: 19268 RVA: 0x00188EEE File Offset: 0x001870EE
	bool ITickSystemPre.PreTickRunning { get; set; }

	// Token: 0x06004B45 RID: 19269 RVA: 0x00188EF8 File Offset: 0x001870F8
	private void Awake()
	{
		if (PlayerCosmeticsSystem.instance == null)
		{
			PlayerCosmeticsSystem.instance = this;
			base.transform.SetParent(null, true);
			Object.DontDestroyOnLoad(this);
			this.inventory = new List<string>();
			this.inventory.Add("Inventory");
			NetworkSystem.Instance.OnRaiseEvent += new Action<byte, object, int>(this.OnNetEvent);
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06004B46 RID: 19270 RVA: 0x00188F68 File Offset: 0x00187168
	private void Start()
	{
		this.playerLookUpCooldown = Mathf.Max(this.playerLookUpCooldown, 3f);
		PlayFabTitleDataCache.Instance.GetTitleData("EnableTempCosmeticUnlocks", delegate(string data)
		{
			bool tempUnlocksEnabled;
			if (bool.TryParse(data, ref tempUnlocksEnabled))
			{
				PlayerCosmeticsSystem.TempUnlocksEnabled = tempUnlocksEnabled;
				return;
			}
			Debug.LogError("PlayerCosmeticsSystem: error parsing EnableTempCosmeticUnlocks data");
		}, delegate(PlayFabError error)
		{
		}, false);
	}

	// Token: 0x06004B47 RID: 19271 RVA: 0x00188FD9 File Offset: 0x001871D9
	private void OnDestroy()
	{
		if (PlayerCosmeticsSystem.instance == this)
		{
			PlayerCosmeticsSystem.instance = null;
		}
	}

	// Token: 0x06004B48 RID: 19272 RVA: 0x00188FEE File Offset: 0x001871EE
	private void LookUpPlayerCosmetics(bool wait = false)
	{
		if (!this.isLookingUp)
		{
			TickSystem<object>.AddPreTickCallback(this);
			if (wait)
			{
				this.startSearchingTime = Time.time;
				return;
			}
			this.startSearchingTime = float.MinValue;
		}
	}

	// Token: 0x06004B49 RID: 19273 RVA: 0x00189018 File Offset: 0x00187218
	public void PreTick()
	{
		if (PlayerCosmeticsSystem.playersToLookUp.Count < 1)
		{
			TickSystem<object>.RemovePreTickCallback(this);
			this.startSearchingTime = float.MinValue;
			this.isLookingUp = false;
			return;
		}
		this.isLookingUp = true;
		if (this.startSearchingTime + this.playerLookUpCooldown > Time.time)
		{
			return;
		}
		if (GorillaServer.Instance.NewCosmeticsPathShouldReadSharedGroupData())
		{
			this.NewCosmeticsPath();
			return;
		}
		PlayerCosmeticsSystem.playerIDsList.Clear();
		while (PlayerCosmeticsSystem.playersToLookUp.Count > 0)
		{
			NetPlayer netPlayer = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
			string text = netPlayer.ActorNumber.ToString();
			if (netPlayer.InRoom() && !PlayerCosmeticsSystem.playerIDsList.Contains(text))
			{
				if (PlayerCosmeticsSystem.playerIDsList.Count == 0)
				{
					int actorNumber = netPlayer.ActorNumber;
				}
				PlayerCosmeticsSystem.playerIDsList.Add(text);
				PlayerCosmeticsSystem.playersWaiting.AddSortedUnique(netPlayer.ActorNumber);
			}
		}
		if (PlayerCosmeticsSystem.playerIDsList.Count > 0)
		{
			GetSharedGroupDataRequest getSharedGroupDataRequest = new GetSharedGroupDataRequest();
			getSharedGroupDataRequest.Keys = PlayerCosmeticsSystem.playerIDsList;
			getSharedGroupDataRequest.SharedGroupId = NetworkSystem.Instance.RoomName + Regex.Replace(NetworkSystem.Instance.CurrentRegion, "[^a-zA-Z0-9]", "").ToUpper();
			PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, new Action<GetSharedGroupDataResult>(this.OnGetsharedGroupData), delegate(PlayFabError error)
			{
				Debug.Log(error.GenerateErrorReport());
				if (error.Error == 1074)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == 1002)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			}, null, null);
		}
		this.isLookingUp = false;
	}

	// Token: 0x06004B4A RID: 19274 RVA: 0x0018917D File Offset: 0x0018737D
	private void NewCosmeticsPath()
	{
		if (this.isLookingUpNew)
		{
			return;
		}
		base.StartCoroutine(this.NewCosmeticsPathCoroutine());
	}

	// Token: 0x06004B4B RID: 19275 RVA: 0x00189195 File Offset: 0x00187395
	private IEnumerator NewCosmeticsPathCoroutine()
	{
		this.isLookingUpNew = true;
		NetPlayer player = null;
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playerActorNumberList.Clear();
		while (PlayerCosmeticsSystem.playersToLookUp.Count > 0)
		{
			player = PlayerCosmeticsSystem.playersToLookUp.Dequeue();
			string text = player.ActorNumber.ToString();
			if (player.InRoom() && !PlayerCosmeticsSystem.playerIDsList.Contains(text))
			{
				PlayerCosmeticsSystem.playerIDsList.Add(player.UserId);
				PlayerCosmeticsSystem.playerActorNumberList.Add(player.ActorNumber);
			}
		}
		int num;
		for (int i = 0; i < PlayerCosmeticsSystem.playerIDsList.Count; i = num + 1)
		{
			int j = i;
			GetSharedGroupDataRequest getSharedGroupDataRequest = new GetSharedGroupDataRequest();
			getSharedGroupDataRequest.Keys = this.inventory;
			getSharedGroupDataRequest.SharedGroupId = PlayerCosmeticsSystem.playerIDsList[j] + "Inventory";
			PlayFabClientAPI.GetSharedGroupData(getSharedGroupDataRequest, delegate(GetSharedGroupDataResult result)
			{
				if (!NetworkSystem.Instance.InRoom)
				{
					PlayerCosmeticsSystem.playersWaiting.Clear();
					return;
				}
				foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
				{
					if (!(keyValuePair.Key != "Inventory") && Utils.PlayerInRoom(PlayerCosmeticsSystem.playerActorNumberList[j]))
					{
						this.tempCosmetics = keyValuePair.Value.Value;
						IUserCosmeticsCallback userCosmeticsCallback;
						if (!PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(PlayerCosmeticsSystem.playerActorNumberList[j], ref userCosmeticsCallback))
						{
							PlayerCosmeticsSystem.userCosmeticsWaiting[PlayerCosmeticsSystem.playerActorNumberList[j]] = this.tempCosmetics;
						}
						else
						{
							userCosmeticsCallback.PendingUpdate = false;
							if (!userCosmeticsCallback.OnGetUserCosmetics(this.tempCosmetics))
							{
								PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
								userCosmeticsCallback.PendingUpdate = true;
							}
						}
					}
				}
			}, delegate(PlayFabError error)
			{
				if (error.Error == 1074)
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
					return;
				}
				if (error.Error == 1002)
				{
					GorillaGameManager.ForceStopGame_DisconnectAndDestroy();
				}
			}, null, null);
			yield return new WaitForSeconds(this.getSharedGroupDataCooldown);
			num = i;
		}
		this.isLookingUpNew = false;
		yield break;
	}

	// Token: 0x06004B4C RID: 19276 RVA: 0x001891A4 File Offset: 0x001873A4
	private void UpdatePlayersWaitingAndDoLookup(bool retrying)
	{
		if (PlayerCosmeticsSystem.playersWaiting.Count > 0)
		{
			for (int i = PlayerCosmeticsSystem.playersWaiting.Count - 1; i >= 0; i--)
			{
				int num = PlayerCosmeticsSystem.playersWaiting[i];
				if (!Utils.PlayerInRoom(num))
				{
					PlayerCosmeticsSystem.playersWaiting.RemoveAt(i);
				}
				else
				{
					PlayerCosmeticsSystem.playersToLookUp.Enqueue(NetworkSystem.Instance.GetPlayer(num));
					retrying = true;
				}
			}
		}
		if (retrying)
		{
			this.LookUpPlayerCosmetics(true);
		}
	}

	// Token: 0x06004B4D RID: 19277 RVA: 0x00189218 File Offset: 0x00187418
	private void OnGetsharedGroupData(GetSharedGroupDataResult result)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			PlayerCosmeticsSystem.playersWaiting.Clear();
			return;
		}
		bool retrying = false;
		foreach (KeyValuePair<string, SharedGroupDataRecord> keyValuePair in result.Data)
		{
			this.playerTemp = null;
			int num;
			if (int.TryParse(keyValuePair.Key, ref num))
			{
				if (!Utils.PlayerInRoom(num))
				{
					PlayerCosmeticsSystem.playersWaiting.Remove(num);
				}
				else
				{
					PlayerCosmeticsSystem.playersWaiting.Remove(num);
					this.playerTemp = NetworkSystem.Instance.GetPlayer(num);
					this.tempCosmetics = keyValuePair.Value.Value;
					IUserCosmeticsCallback userCosmeticsCallback;
					if (!PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(num, ref userCosmeticsCallback))
					{
						PlayerCosmeticsSystem.userCosmeticsWaiting[num] = this.tempCosmetics;
					}
					else
					{
						userCosmeticsCallback.PendingUpdate = false;
						if (!userCosmeticsCallback.OnGetUserCosmetics(this.tempCosmetics))
						{
							Debug.Log("retrying cosmetics for " + this.playerTemp.ToStringFull());
							PlayerCosmeticsSystem.playersToLookUp.Enqueue(this.playerTemp);
							retrying = true;
							userCosmeticsCallback.PendingUpdate = true;
						}
					}
				}
			}
		}
		this.UpdatePlayersWaitingAndDoLookup(retrying);
	}

	// Token: 0x06004B4E RID: 19278 RVA: 0x0018935C File Offset: 0x0018755C
	private void OnNetEvent(byte code, object data, int source)
	{
		if (code != 199 || source < 0)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(source);
		GorillaNot.IncrementRPCCall(new PhotonMessageInfoWrapped(source, NetworkSystem.Instance.ServerTimestamp), "UpdatePlayerCosmetics");
		PlayerCosmeticsSystem.UpdatePlayerCosmetics(player);
	}

	// Token: 0x17000703 RID: 1795
	// (get) Token: 0x06004B4F RID: 19279 RVA: 0x00189395 File Offset: 0x00187595
	private static bool nullInstance
	{
		get
		{
			return PlayerCosmeticsSystem.instance == null || !PlayerCosmeticsSystem.instance;
		}
	}

	// Token: 0x17000704 RID: 1796
	// (get) Token: 0x06004B50 RID: 19280 RVA: 0x001893AD File Offset: 0x001875AD
	// (set) Token: 0x06004B51 RID: 19281 RVA: 0x001893B4 File Offset: 0x001875B4
	public static bool TempUnlocksEnabled { get; private set; } = false;

	// Token: 0x17000705 RID: 1797
	// (get) Token: 0x06004B52 RID: 19282 RVA: 0x001893BC File Offset: 0x001875BC
	// (set) Token: 0x06004B53 RID: 19283 RVA: 0x001893C3 File Offset: 0x001875C3
	public static string[] TempUnlockCosmeticString { get; private set; } = Array.Empty<string>();

	// Token: 0x06004B54 RID: 19284 RVA: 0x001893CC File Offset: 0x001875CC
	public static void RegisterCosmeticCallback(int playerID, IUserCosmeticsCallback callback)
	{
		PlayerCosmeticsSystem.userCosmeticCallback[playerID] = callback;
		string cosmetics;
		if (PlayerCosmeticsSystem.userCosmeticsWaiting.TryGetValue(playerID, ref cosmetics))
		{
			callback.PendingUpdate = false;
			callback.OnGetUserCosmetics(cosmetics);
			PlayerCosmeticsSystem.userCosmeticsWaiting.Remove(playerID);
		}
	}

	// Token: 0x06004B55 RID: 19285 RVA: 0x0018940F File Offset: 0x0018760F
	public static void RemoveCosmeticCallback(int playerID)
	{
		if (PlayerCosmeticsSystem.userCosmeticCallback.ContainsKey(playerID))
		{
			PlayerCosmeticsSystem.userCosmeticCallback.Remove(playerID);
		}
	}

	// Token: 0x06004B56 RID: 19286 RVA: 0x0018942C File Offset: 0x0018762C
	public static void UpdatePlayerCosmetics(NetPlayer player)
	{
		if (player == null || player.IsLocal)
		{
			return;
		}
		PlayerCosmeticsSystem.playersToLookUp.Enqueue(player);
		IUserCosmeticsCallback userCosmeticsCallback;
		if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(player.ActorNumber, ref userCosmeticsCallback))
		{
			userCosmeticsCallback.PendingUpdate = true;
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(true);
		}
	}

	// Token: 0x06004B57 RID: 19287 RVA: 0x00189480 File Offset: 0x00187680
	public static void UpdatePlayerCosmetics(List<NetPlayer> players)
	{
		foreach (NetPlayer netPlayer in players)
		{
			if (netPlayer != null && !netPlayer.IsLocal)
			{
				PlayerCosmeticsSystem.playersToLookUp.Enqueue(netPlayer);
				IUserCosmeticsCallback userCosmeticsCallback;
				if (PlayerCosmeticsSystem.userCosmeticCallback.TryGetValue(netPlayer.ActorNumber, ref userCosmeticsCallback))
				{
					userCosmeticsCallback.PendingUpdate = true;
				}
			}
		}
		if (!PlayerCosmeticsSystem.nullInstance)
		{
			PlayerCosmeticsSystem.instance.LookUpPlayerCosmetics(false);
		}
	}

	// Token: 0x06004B58 RID: 19288 RVA: 0x0018950C File Offset: 0x0018770C
	public static void SetRigTryOn(bool inTryon, RigContainer rigRefg)
	{
		VRRig rig = rigRefg.Rig;
		rig.inTryOnRoom = inTryon;
		if (inTryon)
		{
			if (PlayerCosmeticsSystem.sinceLastTryOnEvent.HasElapsed(0.5f, true))
			{
				GorillaTelemetry.PostShopEvent(rig, GTShopEventType.item_try_on, rig.tryOnSet.items);
			}
		}
		else if (rig.isOfflineVRRig)
		{
			rig.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
			CosmeticsController.instance.ClearCheckout(false);
			CosmeticsController.instance.UpdateShoppingCart();
			CosmeticsController.instance.UpdateWornCosmetics(true);
			rig.myBodyDockPositions.RefreshTransferrableItems();
			return;
		}
		rig.LocalUpdateCosmeticsWithTryon(rig.cosmeticSet, rig.tryOnSet, false);
		rig.myBodyDockPositions.RefreshTransferrableItems();
	}

	// Token: 0x06004B59 RID: 19289 RVA: 0x001895BF File Offset: 0x001877BF
	public static void UnlockTemporaryCosmeticsForPlayer(RigContainer rigRef)
	{
		PlayerCosmeticsSystem.UnlockTemporaryCosmeticsForPlayer(rigRef, PlayerCosmeticsSystem.TempUnlockCosmeticString);
	}

	// Token: 0x06004B5A RID: 19290 RVA: 0x001895CC File Offset: 0x001877CC
	public static void UnlockTemporaryCosmeticsForPlayer(RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		if (cosmeticIds == null)
		{
			Debug.LogError("PlayerCosmeticsSystem failed to unlock temporary cosmetics, cosmetic IDs are null");
			return;
		}
		VRRig rig = rigRef.Rig;
		foreach (string text in cosmeticIds)
		{
			if (!rig.concatStringOfCosmeticsAllowed.Contains(text) && rig.TemporaryCosmetics.Add(text) && rig.isOfflineVRRig)
			{
				CosmeticsController.instance.AddTempUnlockToWardrobe(text);
			}
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.UpdateWornCosmetics(true);
			return;
		}
		rig.RefreshCosmetics();
	}

	// Token: 0x06004B5B RID: 19291 RVA: 0x00189670 File Offset: 0x00187870
	public static void LockTemporaryCosmeticsForPlayer(RigContainer rigRef)
	{
		PlayerCosmeticsSystem.LockTemporaryCosmeticsForPlayer(rigRef, PlayerCosmeticsSystem.TempUnlockCosmeticString);
	}

	// Token: 0x06004B5C RID: 19292 RVA: 0x00189680 File Offset: 0x00187880
	public static void LockTemporaryCosmeticsForPlayer(RigContainer rigRef, IReadOnlyList<string> cosmeticIds)
	{
		if (cosmeticIds == null)
		{
			Debug.LogError("PlayerCosmeticsSystem failed to unlock temporary cosmetics, cosmetic IDs are null");
			return;
		}
		VRRig rig = rigRef.Rig;
		foreach (string text in cosmeticIds)
		{
			if (rig.TemporaryCosmetics.Remove(text) && rig.isOfflineVRRig && !rig.concatStringOfCosmeticsAllowed.Contains(text))
			{
				CosmeticsController.instance.RemoveTempUnlockFromWardrobe(text);
			}
		}
		if (rig.isOfflineVRRig)
		{
			CosmeticsController.instance.UpdateWornCosmetics(true);
			return;
		}
		rig.RefreshCosmetics();
	}

	// Token: 0x06004B5D RID: 19293 RVA: 0x00189724 File Offset: 0x00187924
	internal static void UnlockTemporaryCosmeticsGlobal(IReadOnlyList<string> cosmeticIds)
	{
		int count = cosmeticIds.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerCosmeticsSystem.UnlockTemporaryCosmeticGlobal(cosmeticIds[i]);
		}
	}

	// Token: 0x06004B5E RID: 19294 RVA: 0x00189750 File Offset: 0x00187950
	internal static void UnlockTemporaryCosmeticGlobal(string cosmeticId)
	{
		int num = 0;
		if (PlayerCosmeticsSystem.k_tempUnlockedCosmetics.ContainsKey(cosmeticId))
		{
			num = PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId];
		}
		num++;
		PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId] = num;
	}

	// Token: 0x06004B5F RID: 19295 RVA: 0x00189788 File Offset: 0x00187988
	internal static void LockTemporaryCosmeticsGlobal(IReadOnlyList<string> cosmeticIds)
	{
		int count = cosmeticIds.Count;
		for (int i = 0; i < count; i++)
		{
			PlayerCosmeticsSystem.LockTemporaryCosmeticGlobal(cosmeticIds[i]);
		}
	}

	// Token: 0x06004B60 RID: 19296 RVA: 0x001897B4 File Offset: 0x001879B4
	internal static void LockTemporaryCosmeticGlobal(string cosmeticId)
	{
		if (!PlayerCosmeticsSystem.k_tempUnlockedCosmetics.ContainsKey(cosmeticId))
		{
			Debug.LogError("PlayerCosmeticsSystem: Unable to lock cosmetic, ID:-" + cosmeticId + " not found!");
			return;
		}
		int num = PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId];
		num--;
		PlayerCosmeticsSystem.k_tempUnlockedCosmetics[cosmeticId] = num;
	}

	// Token: 0x06004B61 RID: 19297 RVA: 0x00189800 File Offset: 0x00187A00
	public static bool IsTemporaryCosmeticAllowed(VRRig rigRef, string cosmeticId)
	{
		int num;
		return rigRef.TemporaryCosmetics.Contains(cosmeticId) || (PlayerCosmeticsSystem.k_tempUnlockedCosmetics.TryGetValue(cosmeticId, ref num) && num > 0);
	}

	// Token: 0x06004B62 RID: 19298 RVA: 0x00189832 File Offset: 0x00187A32
	public static void StaticReset()
	{
		PlayerCosmeticsSystem.playersToLookUp.Clear();
		PlayerCosmeticsSystem.userCosmeticCallback.Clear();
		PlayerCosmeticsSystem.userCosmeticsWaiting.Clear();
		PlayerCosmeticsSystem.playerIDsList.Clear();
		PlayerCosmeticsSystem.playersWaiting.Clear();
	}

	// Token: 0x04005B61 RID: 23393
	public float playerLookUpCooldown = 3f;

	// Token: 0x04005B62 RID: 23394
	public float getSharedGroupDataCooldown = 0.1f;

	// Token: 0x04005B63 RID: 23395
	private float startSearchingTime = float.MinValue;

	// Token: 0x04005B64 RID: 23396
	private bool isLookingUp;

	// Token: 0x04005B65 RID: 23397
	private bool isLookingUpNew;

	// Token: 0x04005B66 RID: 23398
	private string tempCosmetics;

	// Token: 0x04005B67 RID: 23399
	private NetPlayer playerTemp;

	// Token: 0x04005B68 RID: 23400
	private RigContainer tempRC;

	// Token: 0x04005B69 RID: 23401
	private List<string> inventory;

	// Token: 0x04005B6A RID: 23402
	private static PlayerCosmeticsSystem instance;

	// Token: 0x04005B6B RID: 23403
	private static Queue<NetPlayer> playersToLookUp = new Queue<NetPlayer>(10);

	// Token: 0x04005B6C RID: 23404
	private static Dictionary<int, IUserCosmeticsCallback> userCosmeticCallback = new Dictionary<int, IUserCosmeticsCallback>(10);

	// Token: 0x04005B6D RID: 23405
	private static Dictionary<int, string> userCosmeticsWaiting = new Dictionary<int, string>(5);

	// Token: 0x04005B6E RID: 23406
	private static List<string> playerIDsList = new List<string>(10);

	// Token: 0x04005B6F RID: 23407
	private static List<int> playerActorNumberList = new List<int>(10);

	// Token: 0x04005B70 RID: 23408
	private static List<int> playersWaiting = new List<int>();

	// Token: 0x04005B71 RID: 23409
	private static TimeSince sinceLastTryOnEvent = 0f;

	// Token: 0x04005B72 RID: 23410
	private static readonly Dictionary<string, int> k_tempUnlockedCosmetics = new Dictionary<string, int>(20);
}
