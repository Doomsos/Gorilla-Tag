using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E6E RID: 3694
	public class SharedBlocksManager : MonoBehaviour
	{
		// Token: 0x1400009E RID: 158
		// (add) Token: 0x06005C54 RID: 23636 RVA: 0x001DAAC4 File Offset: 0x001D8CC4
		// (remove) Token: 0x06005C55 RID: 23637 RVA: 0x001DAAFC File Offset: 0x001D8CFC
		public event Action<string> OnGetTableConfiguration;

		// Token: 0x1400009F RID: 159
		// (add) Token: 0x06005C56 RID: 23638 RVA: 0x001DAB34 File Offset: 0x001D8D34
		// (remove) Token: 0x06005C57 RID: 23639 RVA: 0x001DAB6C File Offset: 0x001D8D6C
		public event Action<string> OnGetTitleDataBuildComplete;

		// Token: 0x140000A0 RID: 160
		// (add) Token: 0x06005C58 RID: 23640 RVA: 0x001DABA4 File Offset: 0x001D8DA4
		// (remove) Token: 0x06005C59 RID: 23641 RVA: 0x001DABDC File Offset: 0x001D8DDC
		public event Action<int> OnSavePrivateScanSuccess;

		// Token: 0x140000A1 RID: 161
		// (add) Token: 0x06005C5A RID: 23642 RVA: 0x001DAC14 File Offset: 0x001D8E14
		// (remove) Token: 0x06005C5B RID: 23643 RVA: 0x001DAC4C File Offset: 0x001D8E4C
		public event Action<int, string> OnSavePrivateScanFailed;

		// Token: 0x140000A2 RID: 162
		// (add) Token: 0x06005C5C RID: 23644 RVA: 0x001DAC84 File Offset: 0x001D8E84
		// (remove) Token: 0x06005C5D RID: 23645 RVA: 0x001DACBC File Offset: 0x001D8EBC
		public event Action<int, bool> OnFetchPrivateScanComplete;

		// Token: 0x140000A3 RID: 163
		// (add) Token: 0x06005C5E RID: 23646 RVA: 0x001DACF4 File Offset: 0x001D8EF4
		// (remove) Token: 0x06005C5F RID: 23647 RVA: 0x001DAD2C File Offset: 0x001D8F2C
		public event Action<bool, SharedBlocksManager.SharedBlocksMap> OnFoundDefaultSharedBlocksMap;

		// Token: 0x140000A4 RID: 164
		// (add) Token: 0x06005C60 RID: 23648 RVA: 0x001DAD64 File Offset: 0x001D8F64
		// (remove) Token: 0x06005C61 RID: 23649 RVA: 0x001DAD9C File Offset: 0x001D8F9C
		public event Action<bool> OnGetPopularMapsComplete;

		// Token: 0x140000A5 RID: 165
		// (add) Token: 0x06005C62 RID: 23650 RVA: 0x001DADD4 File Offset: 0x001D8FD4
		// (remove) Token: 0x06005C63 RID: 23651 RVA: 0x001DAE08 File Offset: 0x001D9008
		public static event Action OnRecentMapIdsUpdated;

		// Token: 0x140000A6 RID: 166
		// (add) Token: 0x06005C64 RID: 23652 RVA: 0x001DAE3C File Offset: 0x001D903C
		// (remove) Token: 0x06005C65 RID: 23653 RVA: 0x001DAE70 File Offset: 0x001D9070
		public static event Action OnSaveTimeUpdated;

		// Token: 0x17000888 RID: 2184
		// (get) Token: 0x06005C66 RID: 23654 RVA: 0x001DAEA3 File Offset: 0x001D90A3
		public List<SharedBlocksManager.SharedBlocksMap> LatestPopularMaps
		{
			get
			{
				return this.latestPopularMaps;
			}
		}

		// Token: 0x17000889 RID: 2185
		// (get) Token: 0x06005C67 RID: 23655 RVA: 0x001DAEAB File Offset: 0x001D90AB
		public string[] BuildData
		{
			get
			{
				return this.privateScanDataCache;
			}
		}

		// Token: 0x06005C68 RID: 23656 RVA: 0x001DAEB3 File Offset: 0x001D90B3
		public bool IsWaitingOnRequest()
		{
			return this.saveScanInProgress || this.getScanInProgress;
		}

		// Token: 0x06005C69 RID: 23657 RVA: 0x001DAEC8 File Offset: 0x001D90C8
		private void Awake()
		{
			if (SharedBlocksManager.instance == null)
			{
				SharedBlocksManager.instance = this;
				for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
				{
					this.privateScanDataCache[i] = string.Empty;
					this.hasPulledPrivateScanMothership[i] = false;
				}
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06005C6A RID: 23658 RVA: 0x001DAF18 File Offset: 0x001D9118
		public void Start()
		{
			SharedBlocksManager.<Start>d__100 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<SharedBlocksManager.<Start>d__100>(ref <Start>d__);
		}

		// Token: 0x06005C6B RID: 23659 RVA: 0x001DAF50 File Offset: 0x001D9150
		private bool TryGetCachedSharedBlocksMapByMapID(string mapID, out SharedBlocksManager.SharedBlocksMap result)
		{
			foreach (SharedBlocksManager.SharedBlocksMap sharedBlocksMap in this.mapResponseCache)
			{
				if (sharedBlocksMap.MapID.Equals(mapID))
				{
					result = sharedBlocksMap;
					return true;
				}
			}
			result = null;
			return false;
		}

		// Token: 0x06005C6C RID: 23660 RVA: 0x001DAFB8 File Offset: 0x001D91B8
		private void AddMapToResponseCache(SharedBlocksManager.SharedBlocksMap map)
		{
			if (map == null)
			{
				return;
			}
			try
			{
				int num = this.mapResponseCache.FindIndex((SharedBlocksManager.SharedBlocksMap x) => x.MapID.Equals(map.MapID));
				if (num < 0)
				{
					this.mapResponseCache.Add(map);
				}
				else
				{
					this.mapResponseCache[num] = map;
				}
			}
			catch (Exception ex)
			{
				GTDev.LogError<string>("SharedBlocksManager AddMapToResponseCache Exception " + ex.ToString(), null);
			}
			if (this.mapResponseCache.Count >= 5)
			{
				this.mapResponseCache.RemoveAt(0);
			}
		}

		// Token: 0x06005C6D RID: 23661 RVA: 0x001DB064 File Offset: 0x001D9264
		public static bool IsMapIDValid(string mapID)
		{
			if (mapID.IsNullOrEmpty())
			{
				return false;
			}
			if (mapID.Length != 8)
			{
				return false;
			}
			if (!Regex.IsMatch(mapID, "^[CFGHKMNPRTWXZ256789]+$"))
			{
				GTDev.LogError<string>("Invalid Characters in SharedBlocksManager IsMapIDValid map " + mapID, null);
				return false;
			}
			return true;
		}

		// Token: 0x06005C6E RID: 23662 RVA: 0x001DB09C File Offset: 0x001D929C
		public static LinkedList<string> GetRecentUpVotes()
		{
			return SharedBlocksManager.recentUpVotes;
		}

		// Token: 0x06005C6F RID: 23663 RVA: 0x001DB0A3 File Offset: 0x001D92A3
		public static List<string> GetLocalMapIDs()
		{
			return SharedBlocksManager.localMapIds;
		}

		// Token: 0x06005C70 RID: 23664 RVA: 0x001DB0AC File Offset: 0x001D92AC
		private static void SetPublishTimeForSlot(int slotID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo localPublishInfo;
			if (SharedBlocksManager.localPublishData.TryGetValue(slotID, ref localPublishInfo))
			{
				localPublishInfo.publishTime = time.ToBinary();
				SharedBlocksManager.localPublishData[slotID] = localPublishInfo;
				return;
			}
			SharedBlocksManager.LocalPublishInfo localPublishInfo2 = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.Add(slotID, localPublishInfo2);
		}

		// Token: 0x06005C71 RID: 23665 RVA: 0x001DB110 File Offset: 0x001D9310
		private static void SetMapIDAndPublishTimeForSlot(int slotID, string mapID, DateTime time)
		{
			SharedBlocksManager.LocalPublishInfo value = new SharedBlocksManager.LocalPublishInfo
			{
				mapID = mapID,
				publishTime = time.ToBinary()
			};
			SharedBlocksManager.localPublishData.AddOrUpdate(slotID, value);
		}

		// Token: 0x06005C72 RID: 23666 RVA: 0x001DB14C File Offset: 0x001D934C
		public static SharedBlocksManager.LocalPublishInfo GetPublishInfoForSlot(int slot)
		{
			SharedBlocksManager.LocalPublishInfo result;
			if (SharedBlocksManager.localPublishData.TryGetValue(slot, ref result))
			{
				return result;
			}
			return new SharedBlocksManager.LocalPublishInfo
			{
				mapID = null,
				publishTime = DateTime.MinValue.ToBinary()
			};
		}

		// Token: 0x06005C73 RID: 23667 RVA: 0x001DB18C File Offset: 0x001D938C
		private void LoadPlayerPrefs()
		{
			string recentVotesPrefsKey = this.serializationConfig.recentVotesPrefsKey;
			string localMapsPrefsKey = this.serializationConfig.localMapsPrefsKey;
			string @string = PlayerPrefs.GetString(recentVotesPrefsKey, null);
			string string2 = PlayerPrefs.GetString(localMapsPrefsKey, null);
			if (!@string.IsNullOrEmpty())
			{
				try
				{
					SharedBlocksManager.recentUpVotes = JsonConvert.DeserializeObject<LinkedList<string>>(@string);
					while (SharedBlocksManager.recentUpVotes.Count > 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					goto IL_82;
				}
				catch (Exception ex)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize Recent Up Votes " + ex.Message, null);
					SharedBlocksManager.recentUpVotes.Clear();
					goto IL_82;
				}
			}
			SharedBlocksManager.recentUpVotes.Clear();
			IL_82:
			if (!string2.IsNullOrEmpty())
			{
				SharedBlocksManager.localPublishData.Clear();
				SharedBlocksManager.localMapIds.Clear();
				try
				{
					SharedBlocksManager.localPublishData = JsonConvert.DeserializeObject<Dictionary<int, SharedBlocksManager.LocalPublishInfo>>(string2);
				}
				catch (Exception ex2)
				{
					GTDev.LogWarning<string>("SharedBlocksManager failed to deserialize localMapIDs " + ex2.Message, null);
					this.GetPlayfabLastSaveTime();
				}
				foreach (KeyValuePair<int, SharedBlocksManager.LocalPublishInfo> keyValuePair in SharedBlocksManager.localPublishData)
				{
					if (!keyValuePair.Value.mapID.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(keyValuePair.Value.mapID))
					{
						SharedBlocksManager.localMapIds.Add(keyValuePair.Value.mapID);
					}
				}
				Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
				if (onSaveTimeUpdated != null)
				{
					onSaveTimeUpdated.Invoke();
				}
			}
			else
			{
				SharedBlocksManager.localMapIds.Clear();
				this.GetPlayfabLastSaveTime();
			}
			Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
			if (onRecentMapIdsUpdated == null)
			{
				return;
			}
			onRecentMapIdsUpdated.Invoke();
		}

		// Token: 0x06005C74 RID: 23668 RVA: 0x001DB330 File Offset: 0x001D9530
		private void SaveRecentVotesToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.recentVotesPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.recentUpVotes));
			PlayerPrefs.Save();
		}

		// Token: 0x06005C75 RID: 23669 RVA: 0x001DB351 File Offset: 0x001D9551
		private void SaveLocalMapIdsToPlayerPrefs()
		{
			PlayerPrefs.SetString(this.serializationConfig.localMapsPrefsKey, JsonConvert.SerializeObject(SharedBlocksManager.localPublishData));
			PlayerPrefs.Save();
		}

		// Token: 0x06005C76 RID: 23670 RVA: 0x001DB374 File Offset: 0x001D9574
		public void RequestVote(string mapID, bool up, Action<bool, string> callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback.Invoke(false, 1.ToString());
				}
				return;
			}
			if (this.voteInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestVote already in progress", null);
				return;
			}
			this.voteInProgress = true;
			base.StartCoroutine(this.PostVote(new SharedBlocksManager.VoteRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				mapId = mapID,
				vote = (up ? 1 : -1)
			}, callback));
		}

		// Token: 0x06005C77 RID: 23671 RVA: 0x001DB40A File Offset: 0x001D960A
		private IEnumerator PostVote(SharedBlocksManager.VoteRequest data, Action<bool, string> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/MapVote", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == 1)
			{
				string mapId = data.mapId;
				if (data.vote == -1)
				{
					if (SharedBlocksManager.recentUpVotes.Remove(mapId))
					{
						this.SaveRecentVotesToPlayerPrefs();
						Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
						if (onRecentMapIdsUpdated != null)
						{
							onRecentMapIdsUpdated.Invoke();
						}
					}
				}
				else if (!SharedBlocksManager.recentUpVotes.Contains(mapId))
				{
					if (SharedBlocksManager.recentUpVotes.Count >= 10)
					{
						SharedBlocksManager.recentUpVotes.RemoveLast();
					}
					SharedBlocksManager.recentUpVotes.AddFirst(mapId);
					this.SaveRecentVotesToPlayerPrefs();
					Action onRecentMapIdsUpdated2 = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated2 != null)
					{
						onRecentMapIdsUpdated2.Invoke();
					}
				}
				this.voteInProgress = false;
				if (callback != null)
				{
					callback.Invoke(true, "");
				}
			}
			else
			{
				GTDev.LogError<string>(string.Format("PostVote Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text, null);
				if (request.result != 3)
				{
					retry = true;
				}
				else
				{
					long responseCode = request.responseCode;
					if (responseCode >= 500L)
					{
						if (responseCode >= 600L)
						{
							goto IL_207;
						}
					}
					else if (responseCode != 408L && responseCode != 429L)
					{
						goto IL_207;
					}
					bool flag = true;
					goto IL_20A;
					IL_207:
					flag = false;
					IL_20A:
					if (flag)
					{
						retry = true;
					}
					else
					{
						this.voteInProgress = false;
						if (callback != null)
						{
							callback.Invoke(false, "REQUEST ERROR");
						}
					}
				}
			}
			if (retry)
			{
				if (this.voteRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.voteRetryCount + 1)));
					this.voteRetryCount++;
					yield return new WaitForSeconds(num);
					this.voteInProgress = false;
					this.RequestVote(data.mapId, data.vote == 1, callback);
				}
				else
				{
					this.voteRetryCount = 0;
					this.voteInProgress = false;
					if (callback != null)
					{
						callback.Invoke(false, "CONNECTION ERROR");
					}
				}
			}
			yield break;
		}

		// Token: 0x06005C78 RID: 23672 RVA: 0x001DB428 File Offset: 0x001D9628
		private void RequestPublishMap(string userMetadataKey)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Client Not Logged into Mothership", null);
				this.PublishMapComplete(false, userMetadataKey, string.Empty, 0L);
				return;
			}
			if (this.publishRequestInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestPublishMap Publish Request in progress", null);
				return;
			}
			this.publishRequestInProgress = true;
			base.StartCoroutine(this.PostPublishMapRequest(new SharedBlocksManager.PublishMapRequestData
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				userdataMetadataKey = userMetadataKey,
				playerNickname = GorillaTagger.Instance.offlineVRRig.playerNameVisible
			}, new SharedBlocksManager.PublishMapRequestCallback(this.PublishMapComplete)));
		}

		// Token: 0x06005C79 RID: 23673 RVA: 0x001DB4D0 File Offset: 0x001D96D0
		private void PublishMapComplete(bool success, string key, [CanBeNull] string mapID, long response)
		{
			this.publishRequestInProgress = false;
			if (success)
			{
				int num = this.serializationConfig.scanSlotMothershipKeys.IndexOf(key);
				if (num >= 0)
				{
					SharedBlocksManager.LocalPublishInfo localPublishInfo;
					if (SharedBlocksManager.localPublishData.TryGetValue(num, ref localPublishInfo))
					{
						SharedBlocksManager.localMapIds.Remove(localPublishInfo.mapID);
					}
					SharedBlocksManager.SetMapIDAndPublishTimeForSlot(num, mapID, DateTime.Now);
					this.SaveLocalMapIdsToPlayerPrefs();
				}
				if (!SharedBlocksManager.localMapIds.Contains(mapID))
				{
					SharedBlocksManager.localMapIds.Add(mapID);
					Action onRecentMapIdsUpdated = SharedBlocksManager.OnRecentMapIdsUpdated;
					if (onRecentMapIdsUpdated != null)
					{
						onRecentMapIdsUpdated.Invoke();
					}
				}
				SharedBlocksManager.SharedBlocksMap map = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = this.privateScanDataCache[num],
					CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
					UpdateTime = DateTime.Now
				};
				this.AddMapToResponseCache(map);
				Action<int> onSavePrivateScanSuccess = this.OnSavePrivateScanSuccess;
				if (onSavePrivateScanSuccess != null)
				{
					onSavePrivateScanSuccess.Invoke(this.currentSaveScanIndex);
				}
			}
			else
			{
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed.Invoke(this.currentSaveScanIndex, "ERROR PUBLISHING: " + response.ToString());
				}
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		// Token: 0x06005C7A RID: 23674 RVA: 0x001DB5EF File Offset: 0x001D97EF
		private IEnumerator PostPublishMapRequest(SharedBlocksManager.PublishMapRequestData data, SharedBlocksManager.PublishMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/Publish", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == 1)
			{
				GTDev.Log<string>("PostPublishMapRequest Success: raw response: " + request.downloadHandler.text, null);
				try
				{
					string text = request.downloadHandler.text;
					bool success = !text.IsNullOrEmpty() && SharedBlocksManager.IsMapIDValid(text);
					if (callback != null)
					{
						callback(success, data.userdataMetadataKey, text, request.responseCode);
					}
					goto IL_21D;
				}
				catch (Exception ex)
				{
					GTDev.LogError<string>("SharedBlocksManager PostPublishMapRequest " + ex.Message, null);
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, null, request.responseCode);
					}
					goto IL_21D;
				}
			}
			if (request.result != 3)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_1E0;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_1E0;
				}
				bool flag = true;
				goto IL_1E3;
				IL_1E0:
				flag = false;
				IL_1E3:
				if (flag)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
				}
			}
			IL_21D:
			if (retry)
			{
				if (this.postPublishMapRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.postPublishMapRetryCount + 1)));
					this.postPublishMapRetryCount++;
					yield return new WaitForSeconds(num);
					this.publishRequestInProgress = false;
					this.RequestPublishMap(data.userdataMetadataKey);
				}
				else
				{
					this.postPublishMapRetryCount = 0;
					if (callback != null)
					{
						callback(false, data.userdataMetadataKey, string.Empty, request.responseCode);
					}
				}
			}
			yield break;
		}

		// Token: 0x06005C7B RID: 23675 RVA: 0x001DB60C File Offset: 0x001D980C
		public void RequestMapDataFromID(string mapID, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Client Not Logged into Mothership", null);
				if (callback != null)
				{
					callback(null);
				}
				return;
			}
			SharedBlocksManager.SharedBlocksMap response;
			if (this.TryGetCachedSharedBlocksMapByMapID(mapID, out response))
			{
				if (callback != null)
				{
					callback(response);
				}
				return;
			}
			if (this.getMapDataFromIDInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestMapDataFromID Fetch already in progress", null);
				return;
			}
			this.getMapDataFromIDInProgress = true;
			base.StartCoroutine(this.GetMapDataFromID(new SharedBlocksManager.GetMapDataFromIDRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				mapId = mapID
			}, callback));
		}

		// Token: 0x06005C7C RID: 23676 RVA: 0x001DB6A2 File Offset: 0x001D98A2
		private IEnumerator GetMapDataFromID(SharedBlocksManager.GetMapDataFromIDRequest data, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMapData", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == 1)
			{
				string text = request.downloadHandler.text;
				this.GetMapDataFromIDComplete(data.mapId, text, callback);
			}
			else if (request.result != 3)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_14E;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_14E;
				}
				bool flag = true;
				goto IL_151;
				IL_14E:
				flag = false;
				IL_151:
				if (flag)
				{
					retry = true;
				}
				else
				{
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			if (retry)
			{
				if (this.getMapDataFromIDRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.getMapDataFromIDRetryCount + 1)));
					this.getMapDataFromIDRetryCount++;
					yield return new WaitForSeconds(num);
					this.getMapDataFromIDInProgress = false;
					this.RequestMapDataFromID(data.mapId, callback);
				}
				else
				{
					this.getMapDataFromIDRetryCount = 0;
					this.GetMapDataFromIDComplete(data.mapId, null, callback);
				}
			}
			yield break;
		}

		// Token: 0x06005C7D RID: 23677 RVA: 0x001DB6C0 File Offset: 0x001D98C0
		private void GetMapDataFromIDComplete(string mapID, [CanBeNull] string response, SharedBlocksManager.BlocksMapRequestCallback callback)
		{
			this.getMapDataFromIDInProgress = false;
			if (response == null)
			{
				if (callback != null)
				{
					callback(null);
					return;
				}
			}
			else
			{
				SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
				{
					MapID = mapID,
					MapData = response
				};
				this.AddMapToResponseCache(sharedBlocksMap);
				if (callback != null)
				{
					callback(sharedBlocksMap);
				}
			}
		}

		// Token: 0x06005C7E RID: 23678 RVA: 0x001DB708 File Offset: 0x001D9908
		public bool RequestGetTopMaps(int pageNum, int pageSize, string sort)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps Client Not Logged into Mothership", null);
				return false;
			}
			if (this.getTopMapsInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestFetchPopularBlocksMaps already in progress", null);
				return false;
			}
			this.getTopMapsInProgress = true;
			this.lastGetTopMapsTime = Time.timeAsDouble;
			base.StartCoroutine(this.GetTopMaps(new SharedBlocksManager.GetMapsRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				page = pageNum,
				pageSize = pageSize,
				sort = sort,
				ShowInactive = false
			}, new Action<List<SharedBlocksManager.SharedBlocksMapMetaData>>(this.GetTopMapsComplete)));
			return true;
		}

		// Token: 0x06005C7F RID: 23679 RVA: 0x001DB7AC File Offset: 0x001D99AC
		private IEnumerator GetTopMaps(SharedBlocksManager.GetMapsRequest data, Action<List<SharedBlocksManager.SharedBlocksMapMetaData>> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/GetMaps", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == 1)
			{
				try
				{
					List<SharedBlocksManager.SharedBlocksMapMetaData> list = JsonConvert.DeserializeObject<List<SharedBlocksManager.SharedBlocksMapMetaData>>(request.downloadHandler.text);
					if (callback != null)
					{
						callback.Invoke(list);
					}
					goto IL_187;
				}
				catch (Exception)
				{
					if (callback != null)
					{
						callback.Invoke(null);
					}
					goto IL_187;
				}
			}
			if (request.result != 3)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_165;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_165;
				}
				bool flag = true;
				goto IL_168;
				IL_165:
				flag = false;
				IL_168:
				if (flag)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback.Invoke(null);
				}
			}
			IL_187:
			if (retry)
			{
				if (this.getTopMapsRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.getTopMapsRetryCount + 1)));
					this.getTopMapsRetryCount++;
					yield return new WaitForSeconds(num);
					this.getTopMapsInProgress = false;
					this.RequestGetTopMaps(data.page, data.pageSize, data.sort);
				}
				else
				{
					this.getTopMapsRetryCount = 0;
					if (callback != null)
					{
						callback.Invoke(null);
					}
				}
			}
			yield break;
		}

		// Token: 0x06005C80 RID: 23680 RVA: 0x001DB7CC File Offset: 0x001D99CC
		private void GetTopMapsComplete([CanBeNull] List<SharedBlocksManager.SharedBlocksMapMetaData> maps)
		{
			this.getTopMapsInProgress = false;
			if (maps != null)
			{
				this.latestPopularMaps.Clear();
				foreach (SharedBlocksManager.SharedBlocksMapMetaData sharedBlocksMapMetaData in maps)
				{
					if (sharedBlocksMapMetaData != null && SharedBlocksManager.IsMapIDValid(sharedBlocksMapMetaData.mapId))
					{
						DateTime createTime = DateTime.MinValue;
						DateTime updateTime = DateTime.MinValue;
						try
						{
							createTime = DateTime.Parse(sharedBlocksMapMetaData.createdTime);
							updateTime = DateTime.Parse(sharedBlocksMapMetaData.updatedTime);
						}
						catch (Exception ex)
						{
							GTDev.LogWarning<string>("SharedBlocksManager GetTopMaps bad update or create time" + ex.Message, null);
						}
						SharedBlocksManager.SharedBlocksMap sharedBlocksMap = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = sharedBlocksMapMetaData.mapId,
							CreatorID = null,
							CreatorNickName = sharedBlocksMapMetaData.nickname,
							CreateTime = createTime,
							UpdateTime = updateTime,
							MapData = null
						};
						this.latestPopularMaps.Add(sharedBlocksMap);
					}
				}
				this.hasCachedTopMaps = true;
				Action<bool> onGetPopularMapsComplete = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete == null)
				{
					return;
				}
				onGetPopularMapsComplete.Invoke(true);
				return;
			}
			else
			{
				Action<bool> onGetPopularMapsComplete2 = this.OnGetPopularMapsComplete;
				if (onGetPopularMapsComplete2 == null)
				{
					return;
				}
				onGetPopularMapsComplete2.Invoke(false);
				return;
			}
		}

		// Token: 0x06005C81 RID: 23681 RVA: 0x001DB908 File Offset: 0x001D9B08
		private void RequestUpdateMapActive(string userMetadataKey, bool active)
		{
			if (!MothershipClientContext.IsClientLoggedIn())
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive Client Not Logged into Mothership", null);
				return;
			}
			if (this.updateMapActiveInProgress)
			{
				GTDev.LogWarning<string>("SharedBlocksManager RequestUpdateMapActive already in progress", null);
				return;
			}
			this.updateMapActiveInProgress = true;
			base.StartCoroutine(this.PostUpdateMapActive(new SharedBlocksManager.UpdateMapActiveRequest
			{
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				userdataMetadataKey = userMetadataKey,
				setActive = active
			}, new Action<bool>(this.OnUpdatedMapActiveComplete)));
		}

		// Token: 0x06005C82 RID: 23682 RVA: 0x001DB990 File Offset: 0x001D9B90
		private IEnumerator PostUpdateMapActive(SharedBlocksManager.UpdateMapActiveRequest data, Action<bool> callback)
		{
			UnityWebRequest request = new UnityWebRequest(this.serializationConfig.sharedBlocksApiBaseURL + "/api/UpdateMapActive", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			yield return request.SendWebRequest();
			if (request.result == 1)
			{
				if (callback != null)
				{
					callback.Invoke(true);
				}
			}
			else if (request.result != 3)
			{
				retry = true;
			}
			else
			{
				long responseCode = request.responseCode;
				if (responseCode >= 500L)
				{
					if (responseCode >= 600L)
					{
						goto IL_132;
					}
				}
				else if (responseCode != 408L && responseCode != 429L)
				{
					goto IL_132;
				}
				bool flag = true;
				goto IL_135;
				IL_132:
				flag = false;
				IL_135:
				if (flag)
				{
					retry = true;
				}
				else if (callback != null)
				{
					callback.Invoke(false);
				}
			}
			if (retry)
			{
				if (this.updateMapActiveRetryCount < this.maxRetriesOnFail)
				{
					float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.updateMapActiveRetryCount + 1)));
					this.updateMapActiveRetryCount++;
					yield return new WaitForSeconds(num);
					this.updateMapActiveInProgress = false;
					this.RequestUpdateMapActive(data.userdataMetadataKey, data.setActive);
				}
				else
				{
					this.updateMapActiveRetryCount = 0;
					if (callback != null)
					{
						callback.Invoke(false);
					}
				}
			}
			yield break;
		}

		// Token: 0x06005C83 RID: 23683 RVA: 0x001DB9AD File Offset: 0x001D9BAD
		private void OnUpdatedMapActiveComplete(bool success)
		{
			this.updateMapActiveInProgress = false;
		}

		// Token: 0x06005C84 RID: 23684 RVA: 0x001DB9B8 File Offset: 0x001D9BB8
		private Task WaitForPlayfabSessionToken()
		{
			SharedBlocksManager.<WaitForPlayfabSessionToken>d__126 <WaitForPlayfabSessionToken>d__;
			<WaitForPlayfabSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForPlayfabSessionToken>d__.<>1__state = -1;
			<WaitForPlayfabSessionToken>d__.<>t__builder.Start<SharedBlocksManager.<WaitForPlayfabSessionToken>d__126>(ref <WaitForPlayfabSessionToken>d__);
			return <WaitForPlayfabSessionToken>d__.<>t__builder.Task;
		}

		// Token: 0x06005C85 RID: 23685 RVA: 0x001DB9F3 File Offset: 0x001D9BF3
		public void RequestTableConfiguration()
		{
			if (this.fetchedTableConfig)
			{
				Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
				if (onGetTableConfiguration == null)
				{
					return;
				}
				onGetTableConfiguration.Invoke(this.tableConfigResponse);
			}
		}

		// Token: 0x06005C86 RID: 23686 RVA: 0x001DBA14 File Offset: 0x001D9C14
		private void FetchConfigurationFromTitleData()
		{
			GetTitleDataRequest getTitleDataRequest = new GetTitleDataRequest();
			List<string> list = new List<string>();
			list.Add(this.serializationConfig.tableConfigurationKey);
			getTitleDataRequest.Keys = list;
			PlayFabClientAPI.GetTitleData(getTitleDataRequest, new Action<GetTitleDataResult>(this.OnGetConfigurationSuccess), new Action<PlayFabError>(this.OnGetConfigurationFail), null, null);
		}

		// Token: 0x06005C87 RID: 23687 RVA: 0x001DBA64 File Offset: 0x001D9C64
		private void OnGetConfigurationSuccess(GetTitleDataResult result)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetConfigurationSuccess", null);
			string text;
			if (result.Data.TryGetValue(this.serializationConfig.tableConfigurationKey, ref text))
			{
				this.tableConfigResponse = text;
				this.fetchedTableConfig = true;
				Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
				if (onGetTableConfiguration == null)
				{
					return;
				}
				onGetTableConfiguration.Invoke(this.tableConfigResponse);
			}
		}

		// Token: 0x06005C88 RID: 23688 RVA: 0x001DBABC File Offset: 0x001D9CBC
		private void OnGetConfigurationFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnGetConfigurationFail " + error.Error.ToString(), null);
			if (error.Error == 2 && this.fetchTableConfigRetryCount < this.maxRetriesOnFail)
			{
				float waitTime = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.fetchTableConfigRetryCount + 1)));
				this.fetchTableConfigRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(waitTime, new Action(this.FetchConfigurationFromTitleData)));
				return;
			}
			this.tableConfigResponse = string.Empty;
			this.fetchedTableConfig = true;
			Action<string> onGetTableConfiguration = this.OnGetTableConfiguration;
			if (onGetTableConfiguration == null)
			{
				return;
			}
			onGetTableConfiguration.Invoke(this.tableConfigResponse);
		}

		// Token: 0x06005C89 RID: 23689 RVA: 0x001DBB70 File Offset: 0x001D9D70
		private IEnumerator RetryAfterWaitTime(float waitTime, Action function)
		{
			yield return new WaitForSeconds(waitTime);
			if (function != null)
			{
				function.Invoke();
			}
			yield break;
		}

		// Token: 0x06005C8A RID: 23690 RVA: 0x001DBB88 File Offset: 0x001D9D88
		public void FetchTitleDataBuild()
		{
			if (!this.fetchTitleDataBuildComplete)
			{
				if (!this.fetchTitleDataBuildInProgress)
				{
					this.fetchTitleDataBuildInProgress = true;
					GetTitleDataRequest getTitleDataRequest = new GetTitleDataRequest();
					List<string> list = new List<string>();
					list.Add(this.serializationConfig.titleDataKey);
					getTitleDataRequest.Keys = list;
					base.StartCoroutine(this.SendTitleDataRequest(getTitleDataRequest, new Action<GetTitleDataResult>(this.OnGetTitleDataBuildSuccess), new Action<PlayFabError>(this.OnGetTitleDataBuildFail)));
				}
				return;
			}
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete.Invoke(this.titleDataBuildCache);
		}

		// Token: 0x06005C8B RID: 23691 RVA: 0x001DBC09 File Offset: 0x001D9E09
		private IEnumerator SendTitleDataRequest(GetTitleDataRequest request, Action<GetTitleDataResult> successCallback, Action<PlayFabError> failCallback)
		{
			while (!PlayFabSettings.staticPlayer.IsClientLoggedIn())
			{
				yield return new WaitForSeconds(5f);
			}
			PlayFabClientAPI.GetTitleData(request, successCallback, failCallback, null, null);
			yield break;
		}

		// Token: 0x06005C8C RID: 23692 RVA: 0x001DBC28 File Offset: 0x001D9E28
		private void OnGetTitleDataBuildSuccess(GetTitleDataResult result)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnGetTitleDataBuildSuccess", null);
			string s;
			if (result.Data.TryGetValue(this.serializationConfig.titleDataKey, ref s) && !s.IsNullOrEmpty())
			{
				this.titleDataBuildCache = s;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete == null)
				{
					return;
				}
				onGetTitleDataBuildComplete.Invoke(this.titleDataBuildCache);
				return;
			}
			else
			{
				this.titleDataBuildCache = string.Empty;
				this.fetchTitleDataBuildComplete = true;
				Action<string> onGetTitleDataBuildComplete2 = this.OnGetTitleDataBuildComplete;
				if (onGetTitleDataBuildComplete2 == null)
				{
					return;
				}
				onGetTitleDataBuildComplete2.Invoke(this.titleDataBuildCache);
				return;
			}
		}

		// Token: 0x06005C8D RID: 23693 RVA: 0x001DBCB8 File Offset: 0x001D9EB8
		private void OnGetTitleDataBuildFail(PlayFabError error)
		{
			this.fetchTitleDataBuildInProgress = false;
			GTDev.LogWarning<string>("SharedBlocksManager FetchTitleDataBuildFail " + error.Error.ToString(), null);
			if (error.Error == 2 && this.fetchTitleDataRetryCount < this.maxRetriesOnFail)
			{
				float waitTime = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.fetchTitleDataRetryCount + 1)));
				this.fetchTitleDataRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(waitTime, new Action(this.FetchTitleDataBuild)));
				return;
			}
			this.titleDataBuildCache = string.Empty;
			this.fetchTitleDataBuildComplete = true;
			Action<string> onGetTitleDataBuildComplete = this.OnGetTitleDataBuildComplete;
			if (onGetTitleDataBuildComplete == null)
			{
				return;
			}
			onGetTitleDataBuildComplete.Invoke(this.titleDataBuildCache);
		}

		// Token: 0x06005C8E RID: 23694 RVA: 0x001DBD73 File Offset: 0x001D9F73
		private string GetPlayfabKeyForSlot(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2");
		}

		// Token: 0x06005C8F RID: 23695 RVA: 0x001DBD91 File Offset: 0x001D9F91
		private string GetPlayfabSlotTimeKey(int slot)
		{
			return this.serializationConfig.playfabScanKey + slot.ToString("D2") + this.serializationConfig.timeAppend;
		}

		// Token: 0x06005C90 RID: 23696 RVA: 0x001DBDBC File Offset: 0x001D9FBC
		private void GetPlayfabLastSaveTime()
		{
			if (!this.hasQueriedSaveTime)
			{
				GetUserDataRequest getUserDataRequest = new GetUserDataRequest
				{
					PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
					Keys = SharedBlocksManager.saveDateKeys
				};
				try
				{
					PlayFabClientAPI.GetUserData(getUserDataRequest, new Action<GetUserDataResult>(this.OnGetLastSaveTimeSuccess), new Action<PlayFabError>(this.OnGetLastSaveTimeFailure), null, null);
				}
				catch (PlayFabException ex)
				{
					this.OnGetLastSaveTimeFailure(new PlayFabError
					{
						Error = 1,
						ErrorMessage = ex.Message
					});
				}
				this.hasQueriedSaveTime = true;
				return;
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated.Invoke();
		}

		// Token: 0x06005C91 RID: 23697 RVA: 0x001DBE60 File Offset: 0x001DA060
		private void OnGetLastSaveTimeSuccess(GetUserDataResult result)
		{
			bool flag = false;
			for (int i = 0; i < BuilderScanKiosk.NUM_SAVE_SLOTS; i++)
			{
				UserDataRecord userDataRecord;
				if (result.Data.TryGetValue(this.GetPlayfabSlotTimeKey(i), ref userDataRecord))
				{
					flag = true;
					DateTime lastUpdated = userDataRecord.LastUpdated;
					SharedBlocksManager.SetPublishTimeForSlot(i, lastUpdated + DateTimeOffset.Now.Offset);
				}
			}
			if (flag)
			{
				this.SaveLocalMapIdsToPlayerPrefs();
			}
			Action onSaveTimeUpdated = SharedBlocksManager.OnSaveTimeUpdated;
			if (onSaveTimeUpdated == null)
			{
				return;
			}
			onSaveTimeUpdated.Invoke();
		}

		// Token: 0x06005C92 RID: 23698 RVA: 0x001DBED0 File Offset: 0x001DA0D0
		private void OnGetLastSaveTimeFailure(PlayFabError error)
		{
			string text = ((error != null) ? error.ErrorMessage : null) ?? "Null";
			GTDev.LogError<string>("SharedBlocksManager GetLastSaveTimeFailure " + text, null);
		}

		// Token: 0x06005C93 RID: 23699 RVA: 0x001DBF04 File Offset: 0x001DA104
		private void FetchBuildFromPlayfab()
		{
			if (this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex])
			{
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete != null)
				{
					onFetchPrivateScanComplete.Invoke(this.currentGetScanIndex, true);
				}
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				return;
			}
			GetUserDataRequest getUserDataRequest = new GetUserDataRequest();
			getUserDataRequest.PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			List<string> list = new List<string>();
			list.Add(this.GetPlayfabKeyForSlot(this.currentGetScanIndex));
			getUserDataRequest.Keys = list;
			GetUserDataRequest request = getUserDataRequest;
			base.StartCoroutine(this.SendPlayfabUserDataRequest(request, new Action<GetUserDataResult>(this.OnFetchBuildFromPlayfabSuccess), new Action<PlayFabError>(this.OnFetchBuildFromPlayfabFail)));
		}

		// Token: 0x06005C94 RID: 23700 RVA: 0x001DBFA2 File Offset: 0x001DA1A2
		private IEnumerator SendPlayfabUserDataRequest(GetUserDataRequest request, Action<GetUserDataResult> resultCallback, Action<PlayFabError> errorCallback)
		{
			while (!PlayFabSettings.staticPlayer.IsClientLoggedIn())
			{
				yield return new WaitForSeconds(5f);
			}
			try
			{
				PlayFabClientAPI.GetUserData(request, resultCallback, errorCallback, null, null);
				yield break;
			}
			catch (PlayFabException ex)
			{
				if (errorCallback != null)
				{
					errorCallback.Invoke(new PlayFabError
					{
						Error = 1,
						ErrorMessage = ex.Message
					});
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x06005C95 RID: 23701 RVA: 0x001DBFC0 File Offset: 0x001DA1C0
		private void OnFetchBuildFromPlayfabSuccess(GetUserDataResult result)
		{
			this.getScanInProgress = false;
			GTDev.Log<string>("SharedBlocksManager OnFetchBuildsFromPlayfabSuccess", null);
			UserDataRecord userDataRecord;
			if (result != null && result.Data != null && result.Data.TryGetValue(this.GetPlayfabKeyForSlot(this.currentGetScanIndex), ref userDataRecord))
			{
				this.privateScanDataCache[this.currentGetScanIndex] = userDataRecord.Value;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
				if (!userDataRecord.Value.IsNullOrEmpty())
				{
					this.RequestSavePrivateScan(this.currentGetScanIndex, userDataRecord.Value);
				}
			}
			else
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			}
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete.Invoke(this.currentGetScanIndex, true);
			}
			this.currentGetScanIndex = -1;
		}

		// Token: 0x06005C96 RID: 23702 RVA: 0x001DC088 File Offset: 0x001DA288
		private void OnFetchBuildFromPlayfabFail(PlayFabError error)
		{
			GTDev.LogWarning<string>("SharedBlocksManager OnFetchBuildsFromPlayfabFail " + (((error != null) ? error.ErrorMessage : null) ?? "Null"), null);
			if (error != null && error.Error == 2 && this.fetchPlayfabBuildsRetryCount < this.maxRetriesOnFail)
			{
				float waitTime = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.fetchPlayfabBuildsRetryCount + 1)));
				this.fetchPlayfabBuildsRetryCount++;
				base.StartCoroutine(this.RetryAfterWaitTime(waitTime, new Action(this.FetchBuildFromPlayfab)));
				return;
			}
			this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
			this.hasPulledPrivateScanPlayfab[this.currentGetScanIndex] = true;
			this.getScanInProgress = false;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete.Invoke(this.currentGetScanIndex, false);
			}
			this.currentGetScanIndex = -1;
		}

		// Token: 0x06005C97 RID: 23703 RVA: 0x001DC164 File Offset: 0x001DA364
		private Task WaitForMothership()
		{
			SharedBlocksManager.<WaitForMothership>d__145 <WaitForMothership>d__;
			<WaitForMothership>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForMothership>d__.<>1__state = -1;
			<WaitForMothership>d__.<>t__builder.Start<SharedBlocksManager.<WaitForMothership>d__145>(ref <WaitForMothership>d__);
			return <WaitForMothership>d__.<>t__builder.Task;
		}

		// Token: 0x06005C98 RID: 23704 RVA: 0x001DC1A0 File Offset: 0x001DA3A0
		public void RequestSavePrivateScan(int scanIndex, string scanData)
		{
			if (scanIndex < 0 || scanIndex >= this.serializationConfig.scanSlotMothershipKeys.Count)
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScanToMothership: scan index {0} out of bounds", scanIndex), null);
				return;
			}
			this.currentSaveScanIndex = scanIndex;
			this.currentSaveScanData = scanData;
			if (!this.hasPulledPrivateScanMothership[scanIndex])
			{
				this.PullMothershipPrivateScanThenPush(scanIndex);
				return;
			}
			this.privateScanDataCache[scanIndex] = scanData;
			this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[scanIndex], scanData);
		}

		// Token: 0x06005C99 RID: 23705 RVA: 0x001DC21C File Offset: 0x001DA41C
		private void PullMothershipPrivateScanThenPush(int scanIndex)
		{
			if (this.getScanInProgress && this.currentGetScanIndex != scanIndex)
			{
				GTDev.LogWarning<string>("SharedBLocksManager PullMothershipPrivateScanThenPush GetScan in progress", null);
				Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
				if (onSavePrivateScanFailed != null)
				{
					onSavePrivateScanFailed.Invoke(scanIndex, "ERROR SAVING: BUSY");
				}
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			this.OnFetchPrivateScanComplete += new Action<int, bool>(this.PushMothershipPrivateScan);
			this.RequestFetchPrivateScan(scanIndex);
		}

		// Token: 0x06005C9A RID: 23706 RVA: 0x001DC288 File Offset: 0x001DA488
		private void PushMothershipPrivateScan(int scan, bool success)
		{
			if (scan == this.currentSaveScanIndex)
			{
				this.OnFetchPrivateScanComplete -= new Action<int, bool>(this.PushMothershipPrivateScan);
				this.privateScanDataCache[this.currentSaveScanIndex] = this.currentSaveScanData;
				this.RequestSetMothershipUserData(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex], this.currentSaveScanData);
			}
		}

		// Token: 0x06005C9B RID: 23707 RVA: 0x001DC2E8 File Offset: 0x001DA4E8
		private void RequestSetMothershipUserData(string keyName, string value)
		{
			if (this.saveScanInProgress)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: request already in progress");
				return;
			}
			this.saveScanInProgress = true;
			try
			{
				if (!MothershipClientApiUnity.SetUserDataValue(keyName, value, new Action<SetUserDataResponse>(this.OnSetMothershipUserDataSuccess), new Action<MothershipError, int>(this.OnSetMothershipUserDataFail), ""))
				{
					Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: SetUserDataValue Fail");
					this.OnSetMothershipDataComplete(false);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("SharedBlocksManager RequestSetMothershipUserData: exception " + ex.Message);
				this.OnSetMothershipDataComplete(false);
			}
		}

		// Token: 0x06005C9C RID: 23708 RVA: 0x001DC378 File Offset: 0x001DA578
		private void OnSetMothershipUserDataSuccess(SetUserDataResponse response)
		{
			GTDev.Log<string>("SharedBlocksManager OnSetMothershipUserDataSuccess", null);
			this.OnSetMothershipDataComplete(true);
			response.Dispose();
		}

		// Token: 0x06005C9D RID: 23709 RVA: 0x001DC394 File Offset: 0x001DA594
		private void OnSetMothershipUserDataFail(MothershipError error, int status)
		{
			string text = (error == null) ? status.ToString() : error.Message;
			GTDev.LogError<string>("SharedBlocksManager OnSetMothershipUserDataFail: " + text, null);
			this.OnSetMothershipDataComplete(false);
			if (error != null)
			{
				error.Dispose();
			}
		}

		// Token: 0x06005C9E RID: 23710 RVA: 0x001DC3D8 File Offset: 0x001DA5D8
		private void OnSetMothershipDataComplete(bool success)
		{
			this.saveScanInProgress = false;
			if (!BuilderScanKiosk.IsSaveSlotValid(this.currentSaveScanIndex))
			{
				this.currentSaveScanIndex = -1;
				this.currentSaveScanData = string.Empty;
				return;
			}
			if (success)
			{
				this.RequestPublishMap(this.serializationConfig.scanSlotMothershipKeys[this.currentSaveScanIndex]);
				return;
			}
			Action<int, string> onSavePrivateScanFailed = this.OnSavePrivateScanFailed;
			if (onSavePrivateScanFailed != null)
			{
				onSavePrivateScanFailed.Invoke(this.currentSaveScanIndex, "ERROR SAVING");
			}
			this.currentSaveScanIndex = -1;
			this.currentSaveScanData = string.Empty;
		}

		// Token: 0x06005C9F RID: 23711 RVA: 0x001DC45A File Offset: 0x001DA65A
		public bool TryGetPrivateScanResponse(int scanSlot, out string scanData)
		{
			if (scanSlot < 0 || scanSlot >= this.privateScanDataCache.Length || !this.hasPulledPrivateScanMothership[scanSlot])
			{
				scanData = string.Empty;
				return false;
			}
			scanData = this.privateScanDataCache[scanSlot];
			return true;
		}

		// Token: 0x06005CA0 RID: 23712 RVA: 0x001DC48C File Offset: 0x001DA68C
		public void RequestFetchPrivateScan(int slot)
		{
			if (!BuilderScanKiosk.IsSaveSlotValid(slot))
			{
				GTDev.LogError<string>(string.Format("SharedBlocksManager RequestSaveScan: slot {0} OOB", slot), null);
				slot = Mathf.Clamp(slot, 0, BuilderScanKiosk.NUM_SAVE_SLOTS - 1);
			}
			if (this.hasPulledPrivateScanMothership[slot])
			{
				bool flag = this.privateScanDataCache[slot].Length > 0;
				Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete == null)
				{
					return;
				}
				onFetchPrivateScanComplete.Invoke(slot, flag);
				return;
			}
			else
			{
				if (this.getScanInProgress)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan: request already in progress");
					if (slot != this.currentGetScanIndex)
					{
						Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete2 == null)
						{
							return;
						}
						onFetchPrivateScanComplete2.Invoke(slot, false);
					}
					return;
				}
				this.currentGetScanIndex = slot;
				this.getScanInProgress = true;
				try
				{
					if (!MothershipClientApiUnity.GetUserDataValue(this.serializationConfig.scanSlotMothershipKeys[slot], new Action<MothershipUserData>(this.OnGetMothershipPrivateScanSuccess), new Action<MothershipError, int>(this.OnGetMothershipPrivateScanFail), ""))
					{
						Debug.LogError("SharedBlocksManager RequestFetchPrivateScan failed ");
						this.currentGetScanIndex = -1;
						this.getScanInProgress = false;
						Action<int, bool> onFetchPrivateScanComplete3 = this.OnFetchPrivateScanComplete;
						if (onFetchPrivateScanComplete3 != null)
						{
							onFetchPrivateScanComplete3.Invoke(slot, false);
						}
					}
				}
				catch (Exception ex)
				{
					Debug.LogError("SharedBlocksManager RequestFetchPrivateScan exception " + ex.Message);
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete4 = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete4 != null)
					{
						onFetchPrivateScanComplete4.Invoke(slot, false);
					}
				}
				return;
			}
		}

		// Token: 0x06005CA1 RID: 23713 RVA: 0x001DC5DC File Offset: 0x001DA7DC
		private void OnGetMothershipPrivateScanSuccess(MothershipUserData response)
		{
			GTDev.Log<string>("SharedBlocksManager OnGetMothershipPrivateScanSuccess", null);
			bool flag = response != null && response.value != null && response.value.Length > 0;
			int num = this.currentGetScanIndex;
			if (response != null)
			{
				this.privateScanDataCache[this.currentGetScanIndex] = response.value;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
				if (flag)
				{
					SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(this.currentGetScanIndex);
					if (publishInfoForSlot.mapID != null)
					{
						SharedBlocksManager.SharedBlocksMap map = new SharedBlocksManager.SharedBlocksMap
						{
							MapID = publishInfoForSlot.mapID,
							MapData = this.privateScanDataCache[this.currentGetScanIndex],
							CreatorNickName = GorillaTagger.Instance.offlineVRRig.playerNameVisible,
							UpdateTime = DateTime.Now
						};
						this.AddMapToResponseCache(map);
					}
					this.currentGetScanIndex = -1;
					this.getScanInProgress = false;
					Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
					if (onFetchPrivateScanComplete != null)
					{
						onFetchPrivateScanComplete.Invoke(num, true);
					}
				}
				else
				{
					this.FetchBuildFromPlayfab();
				}
			}
			else
			{
				this.currentGetScanIndex = -1;
				this.getScanInProgress = false;
				Action<int, bool> onFetchPrivateScanComplete2 = this.OnFetchPrivateScanComplete;
				if (onFetchPrivateScanComplete2 != null)
				{
					onFetchPrivateScanComplete2.Invoke(num, false);
				}
			}
			if (response != null)
			{
				response.Dispose();
			}
		}

		// Token: 0x06005CA2 RID: 23714 RVA: 0x001DC6FC File Offset: 0x001DA8FC
		private void OnGetMothershipPrivateScanFail(MothershipError error, int status)
		{
			string text = (error == null) ? status.ToString() : error.Message;
			GTDev.LogError<string>("SharedBlocksManager OnGetMothershipPrivateScanFail: " + text, null);
			int num = this.currentGetScanIndex;
			if (BuilderScanKiosk.IsSaveSlotValid(this.currentGetScanIndex))
			{
				this.privateScanDataCache[this.currentGetScanIndex] = string.Empty;
				this.hasPulledPrivateScanMothership[this.currentGetScanIndex] = true;
			}
			this.getScanInProgress = false;
			this.currentGetScanIndex = -1;
			Action<int, bool> onFetchPrivateScanComplete = this.OnFetchPrivateScanComplete;
			if (onFetchPrivateScanComplete != null)
			{
				onFetchPrivateScanComplete.Invoke(num, false);
			}
			if (error != null)
			{
				error.Dispose();
			}
		}

		// Token: 0x04006A10 RID: 27152
		public static SharedBlocksManager instance;

		// Token: 0x04006A1A RID: 27162
		[SerializeField]
		private BuilderTableSerializationConfig serializationConfig;

		// Token: 0x04006A1B RID: 27163
		private int maxRetriesOnFail = 3;

		// Token: 0x04006A1C RID: 27164
		public const int MAP_ID_LENGTH = 8;

		// Token: 0x04006A1D RID: 27165
		private const string MAP_ID_PATTERN = "^[CFGHKMNPRTWXZ256789]+$";

		// Token: 0x04006A1E RID: 27166
		public const float MINIMUM_REFRESH_DELAY = 60f;

		// Token: 0x04006A1F RID: 27167
		public const int VOTE_HISTORY_LENGTH = 10;

		// Token: 0x04006A20 RID: 27168
		private const int NUM_CACHED_MAP_RESULTS = 5;

		// Token: 0x04006A21 RID: 27169
		private SharedBlocksManager.StartingMapConfig startingMapConfig = new SharedBlocksManager.StartingMapConfig
		{
			pageNumber = 0,
			pageSize = 10,
			sortMethod = SharedBlocksManager.MapSortMethod.Top.ToString(),
			useMapID = false,
			mapID = null
		};

		// Token: 0x04006A22 RID: 27170
		private bool hasQueriedSaveTime;

		// Token: 0x04006A23 RID: 27171
		private static List<string> saveDateKeys = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x04006A24 RID: 27172
		private bool fetchedTableConfig;

		// Token: 0x04006A25 RID: 27173
		private int fetchTableConfigRetryCount;

		// Token: 0x04006A26 RID: 27174
		private string tableConfigResponse;

		// Token: 0x04006A27 RID: 27175
		private bool fetchTitleDataBuildInProgress;

		// Token: 0x04006A28 RID: 27176
		private bool fetchTitleDataBuildComplete;

		// Token: 0x04006A29 RID: 27177
		private int fetchTitleDataRetryCount;

		// Token: 0x04006A2A RID: 27178
		private string titleDataBuildCache = string.Empty;

		// Token: 0x04006A2B RID: 27179
		private bool[] hasPulledPrivateScanPlayfab = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x04006A2C RID: 27180
		private int fetchPlayfabBuildsRetryCount;

		// Token: 0x04006A2D RID: 27181
		private readonly int publicSlotIndex = BuilderScanKiosk.NUM_SAVE_SLOTS;

		// Token: 0x04006A2E RID: 27182
		private string[] privateScanDataCache = new string[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x04006A2F RID: 27183
		private bool[] hasPulledPrivateScanMothership = new bool[BuilderScanKiosk.NUM_SAVE_SLOTS];

		// Token: 0x04006A30 RID: 27184
		private bool hasPulledDevScan;

		// Token: 0x04006A31 RID: 27185
		private string devScanDataCache;

		// Token: 0x04006A32 RID: 27186
		private bool saveScanInProgress;

		// Token: 0x04006A33 RID: 27187
		private int currentSaveScanIndex = -1;

		// Token: 0x04006A34 RID: 27188
		private string currentSaveScanData = string.Empty;

		// Token: 0x04006A35 RID: 27189
		private bool getScanInProgress;

		// Token: 0x04006A36 RID: 27190
		private int currentGetScanIndex = -1;

		// Token: 0x04006A37 RID: 27191
		private int voteRetryCount;

		// Token: 0x04006A38 RID: 27192
		private bool voteInProgress;

		// Token: 0x04006A39 RID: 27193
		private bool publishRequestInProgress;

		// Token: 0x04006A3A RID: 27194
		private int postPublishMapRetryCount;

		// Token: 0x04006A3B RID: 27195
		private bool getMapDataFromIDInProgress;

		// Token: 0x04006A3C RID: 27196
		private int getMapDataFromIDRetryCount;

		// Token: 0x04006A3D RID: 27197
		private bool getTopMapsInProgress;

		// Token: 0x04006A3E RID: 27198
		private int getTopMapsRetryCount;

		// Token: 0x04006A3F RID: 27199
		private bool hasCachedTopMaps;

		// Token: 0x04006A40 RID: 27200
		private double lastGetTopMapsTime = double.MinValue;

		// Token: 0x04006A41 RID: 27201
		private bool updateMapActiveInProgress;

		// Token: 0x04006A42 RID: 27202
		private int updateMapActiveRetryCount;

		// Token: 0x04006A43 RID: 27203
		private List<SharedBlocksManager.SharedBlocksMap> latestPopularMaps = new List<SharedBlocksManager.SharedBlocksMap>();

		// Token: 0x04006A44 RID: 27204
		private static LinkedList<string> recentUpVotes = new LinkedList<string>();

		// Token: 0x04006A45 RID: 27205
		private static Dictionary<int, SharedBlocksManager.LocalPublishInfo> localPublishData = new Dictionary<int, SharedBlocksManager.LocalPublishInfo>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x04006A46 RID: 27206
		private static List<string> localMapIds = new List<string>(BuilderScanKiosk.NUM_SAVE_SLOTS);

		// Token: 0x04006A47 RID: 27207
		private List<SharedBlocksManager.SharedBlocksMap> mapResponseCache = new List<SharedBlocksManager.SharedBlocksMap>(5);

		// Token: 0x04006A48 RID: 27208
		private SharedBlocksManager.SharedBlocksMap defaultMap;

		// Token: 0x04006A49 RID: 27209
		private bool hasDefaultMap;

		// Token: 0x04006A4A RID: 27210
		private double defaultMapCacheTime = double.MinValue;

		// Token: 0x04006A4B RID: 27211
		private bool getDefaultMapInProgress;

		// Token: 0x02000E6F RID: 3695
		[Serializable]
		public class SharedBlocksMap
		{
			// Token: 0x1700088A RID: 2186
			// (get) Token: 0x06005CA5 RID: 23717 RVA: 0x001DC8B9 File Offset: 0x001DAAB9
			// (set) Token: 0x06005CA6 RID: 23718 RVA: 0x001DC8C1 File Offset: 0x001DAAC1
			public string MapID { get; set; }

			// Token: 0x1700088B RID: 2187
			// (get) Token: 0x06005CA7 RID: 23719 RVA: 0x001DC8CA File Offset: 0x001DAACA
			// (set) Token: 0x06005CA8 RID: 23720 RVA: 0x001DC8D2 File Offset: 0x001DAAD2
			public string CreatorID { get; set; }

			// Token: 0x1700088C RID: 2188
			// (get) Token: 0x06005CA9 RID: 23721 RVA: 0x001DC8DB File Offset: 0x001DAADB
			// (set) Token: 0x06005CAA RID: 23722 RVA: 0x001DC8E3 File Offset: 0x001DAAE3
			public string CreatorNickName { get; set; }

			// Token: 0x1700088D RID: 2189
			// (get) Token: 0x06005CAB RID: 23723 RVA: 0x001DC8EC File Offset: 0x001DAAEC
			// (set) Token: 0x06005CAC RID: 23724 RVA: 0x001DC8F4 File Offset: 0x001DAAF4
			public DateTime CreateTime { get; set; }

			// Token: 0x1700088E RID: 2190
			// (get) Token: 0x06005CAD RID: 23725 RVA: 0x001DC8FD File Offset: 0x001DAAFD
			// (set) Token: 0x06005CAE RID: 23726 RVA: 0x001DC905 File Offset: 0x001DAB05
			public DateTime UpdateTime { get; set; }

			// Token: 0x1700088F RID: 2191
			// (get) Token: 0x06005CAF RID: 23727 RVA: 0x001DC90E File Offset: 0x001DAB0E
			// (set) Token: 0x06005CB0 RID: 23728 RVA: 0x001DC916 File Offset: 0x001DAB16
			public string MapData { get; set; }
		}

		// Token: 0x02000E70 RID: 3696
		[Serializable]
		public struct LocalPublishInfo
		{
			// Token: 0x04006A52 RID: 27218
			public string mapID;

			// Token: 0x04006A53 RID: 27219
			public long publishTime;
		}

		// Token: 0x02000E71 RID: 3697
		[Serializable]
		private class SharedBlocksRequestBase
		{
			// Token: 0x04006A54 RID: 27220
			public string mothershipId;

			// Token: 0x04006A55 RID: 27221
			public string mothershipToken;

			// Token: 0x04006A56 RID: 27222
			public string mothershipEnvId;
		}

		// Token: 0x02000E72 RID: 3698
		[Serializable]
		private class VoteRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04006A57 RID: 27223
			public string mapId;

			// Token: 0x04006A58 RID: 27224
			public int vote;
		}

		// Token: 0x02000E73 RID: 3699
		[Serializable]
		private class PublishMapRequestData : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04006A59 RID: 27225
			public string userdataMetadataKey;

			// Token: 0x04006A5A RID: 27226
			public string playerNickname;
		}

		// Token: 0x02000E74 RID: 3700
		public enum MapSortMethod
		{
			// Token: 0x04006A5C RID: 27228
			Top,
			// Token: 0x04006A5D RID: 27229
			NewlyCreated,
			// Token: 0x04006A5E RID: 27230
			RecentlyUpdated
		}

		// Token: 0x02000E75 RID: 3701
		public struct StartingMapConfig
		{
			// Token: 0x04006A5F RID: 27231
			public int pageNumber;

			// Token: 0x04006A60 RID: 27232
			public int pageSize;

			// Token: 0x04006A61 RID: 27233
			public string sortMethod;

			// Token: 0x04006A62 RID: 27234
			public bool useMapID;

			// Token: 0x04006A63 RID: 27235
			public string mapID;
		}

		// Token: 0x02000E76 RID: 3702
		[Serializable]
		private class GetMapsRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04006A64 RID: 27236
			public int page;

			// Token: 0x04006A65 RID: 27237
			public int pageSize;

			// Token: 0x04006A66 RID: 27238
			public string sort;

			// Token: 0x04006A67 RID: 27239
			public bool ShowInactive;
		}

		// Token: 0x02000E77 RID: 3703
		[Serializable]
		private class GetMapDataFromIDRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04006A68 RID: 27240
			public string mapId;
		}

		// Token: 0x02000E78 RID: 3704
		[Serializable]
		private class GetMapIDFromPlayerRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04006A69 RID: 27241
			public string requestId;

			// Token: 0x04006A6A RID: 27242
			public string requestUserDataMetaKey;
		}

		// Token: 0x02000E79 RID: 3705
		[Serializable]
		private class GetMapIDFromPlayerResponse
		{
			// Token: 0x04006A6B RID: 27243
			public SharedBlocksManager.SharedBlocksMapMetaData result;

			// Token: 0x04006A6C RID: 27244
			public int statusCode;

			// Token: 0x04006A6D RID: 27245
			public string error;
		}

		// Token: 0x02000E7A RID: 3706
		[Serializable]
		private class SharedBlocksMapMetaData
		{
			// Token: 0x04006A6E RID: 27246
			public string mapId;

			// Token: 0x04006A6F RID: 27247
			public string mothershipId;

			// Token: 0x04006A70 RID: 27248
			public string userDataMetadataKey;

			// Token: 0x04006A71 RID: 27249
			public string nickname;

			// Token: 0x04006A72 RID: 27250
			public string createdTime;

			// Token: 0x04006A73 RID: 27251
			public string updatedTime;

			// Token: 0x04006A74 RID: 27252
			public int voteCount;

			// Token: 0x04006A75 RID: 27253
			public bool isActive;
		}

		// Token: 0x02000E7B RID: 3707
		[Serializable]
		private struct GetMapDataFromPlayerRequestData
		{
			// Token: 0x04006A76 RID: 27254
			public string CreatorID;

			// Token: 0x04006A77 RID: 27255
			public string MapScan;

			// Token: 0x04006A78 RID: 27256
			public SharedBlocksManager.BlocksMapRequestCallback Callback;
		}

		// Token: 0x02000E7C RID: 3708
		[Serializable]
		private class UpdateMapActiveRequest : SharedBlocksManager.SharedBlocksRequestBase
		{
			// Token: 0x04006A79 RID: 27257
			public string userdataMetadataKey;

			// Token: 0x04006A7A RID: 27258
			public bool setActive;
		}

		// Token: 0x02000E7D RID: 3709
		// (Invoke) Token: 0x06005CBC RID: 23740
		public delegate void PublishMapRequestCallback(bool success, string key, string mapID, long responseCode);

		// Token: 0x02000E7E RID: 3710
		// (Invoke) Token: 0x06005CC0 RID: 23744
		public delegate void BlocksMapRequestCallback(SharedBlocksManager.SharedBlocksMap response);
	}
}
