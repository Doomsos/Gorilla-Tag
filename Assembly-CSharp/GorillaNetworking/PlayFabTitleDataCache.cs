using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using PlayFab;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaNetworking
{
	// Token: 0x02000F24 RID: 3876
	public class PlayFabTitleDataCache : MonoBehaviour
	{
		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x06006121 RID: 24865 RVA: 0x001F4CAC File Offset: 0x001F2EAC
		// (set) Token: 0x06006122 RID: 24866 RVA: 0x001F4CB3 File Offset: 0x001F2EB3
		public static PlayFabTitleDataCache Instance { get; private set; }

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x06006123 RID: 24867 RVA: 0x001F4CBB File Offset: 0x001F2EBB
		private static string FilePath
		{
			get
			{
				return Path.Combine(Application.persistentDataPath, "TitleDataCache.json");
			}
		}

		// Token: 0x06006124 RID: 24868 RVA: 0x001F4CCC File Offset: 0x001F2ECC
		public void GetTitleData(string name, Action<string> callback, Action<PlayFabError> errorCallback, bool ignoreCache = false)
		{
			Dictionary<string, string> dictionary;
			string data;
			if (!ignoreCache && !this.isFirstLoad && this.localizedTitleData.TryGetValue(LocalisationManager.CurrentLanguage.Identifier.Code, ref dictionary) && dictionary.TryGetValue(name, ref data))
			{
				callback.SafeInvoke(data);
				return;
			}
			PlayFabTitleDataCache.DataRequest dataRequest = new PlayFabTitleDataCache.DataRequest
			{
				Name = name,
				Callback = callback,
				ErrorCallback = errorCallback
			};
			this.requests.Add(dataRequest);
			this.TryUpdateData();
		}

		// Token: 0x06006125 RID: 24869 RVA: 0x001F4D46 File Offset: 0x001F2F46
		private void Awake()
		{
			if (PlayFabTitleDataCache.Instance != null)
			{
				Object.Destroy(this);
				return;
			}
			PlayFabTitleDataCache.Instance = this;
		}

		// Token: 0x06006126 RID: 24870 RVA: 0x001F4D62 File Offset: 0x001F2F62
		private void Start()
		{
			this.UpdateData();
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.TryUpdateData));
		}

		// Token: 0x06006127 RID: 24871 RVA: 0x001F4D7B File Offset: 0x001F2F7B
		private void OnDestroy()
		{
			LocalisationManager.UnregisterOnLanguageChanged(new Action(this.TryUpdateData));
		}

		// Token: 0x06006128 RID: 24872 RVA: 0x001F4D8E File Offset: 0x001F2F8E
		private void TryUpdateData()
		{
			if (!this.isFirstLoad && this.updateDataCoroutine == null)
			{
				this.UpdateData();
			}
		}

		// Token: 0x06006129 RID: 24873 RVA: 0x001F4DA8 File Offset: 0x001F2FA8
		public CacheImport LoadDataFromFile()
		{
			CacheImport result;
			try
			{
				if (!File.Exists(PlayFabTitleDataCache.FilePath))
				{
					Debug.LogWarning("[PlayFabTitleDataCache::LoadDataFromFile] Title data file " + PlayFabTitleDataCache.FilePath + " does not exist!");
					result = null;
				}
				else
				{
					result = (JsonMapper.ToObject<CacheImport>(File.ReadAllText(PlayFabTitleDataCache.FilePath)) ?? new CacheImport());
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[PlayFabTitleDataCache::LoadDataFromFile] Error reading PlayFab title data from file: {0}", ex));
				result = null;
			}
			return result;
		}

		// Token: 0x0600612A RID: 24874 RVA: 0x001F4E20 File Offset: 0x001F3020
		private static void SaveDataToFile(string filepath, Dictionary<string, Dictionary<string, string>> titleData)
		{
			try
			{
				string text = JsonMapper.ToJson(new CacheImport
				{
					DeploymentId = MothershipClientApiUnity.DeploymentId,
					TitleData = titleData
				});
				File.WriteAllText(filepath, text);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("[PlayFabTitleDataCache::SaveDataToFile] Error writing PlayFab title data to file: {0}", ex));
			}
		}

		// Token: 0x0600612B RID: 24875 RVA: 0x001F4E78 File Offset: 0x001F3078
		public void UpdateData()
		{
			this.updateDataCoroutine = base.StartCoroutine(this.UpdateDataCo());
		}

		// Token: 0x0600612C RID: 24876 RVA: 0x001F4E8C File Offset: 0x001F308C
		private IEnumerator UpdateDataCo()
		{
			try
			{
				PlayFabTitleDataCache.<>c__DisplayClass23_0 CS$<>8__locals1 = new PlayFabTitleDataCache.<>c__DisplayClass23_0();
				CacheImport oldCache = this.LoadDataFromFile();
				string currentLocale = LocalisationManager.CurrentLanguage.Identifier.Code;
				Dictionary<string, string> titleData;
				if (!this.localizedTitleData.TryGetValue(currentLocale, ref titleData))
				{
					this.localizedTitleData[currentLocale] = new Dictionary<string, string>();
					titleData = this.localizedTitleData[currentLocale];
				}
				Dictionary<string, string> oldLocalizedCache;
				if (oldCache == null || oldCache.TitleData == null || !oldCache.TitleData.TryGetValue(currentLocale, ref oldLocalizedCache))
				{
					oldLocalizedCache = new Dictionary<string, string>();
				}
				yield return new WaitUntil(() => MothershipClientApiUnity.IsClientLoggedIn());
				bool wipeOldData = oldCache == null || oldCache.DeploymentId != MothershipClientApiUnity.DeploymentId;
				CS$<>8__locals1.newTitleData = null;
				CS$<>8__locals1.mothershipError = null;
				Stopwatch sw = Stopwatch.StartNew();
				Debug.Log("[PlayFabTitleDataCache::UpdateDataCo] Starting Mothership API call");
				StringVector stringVector = new StringVector();
				foreach (PlayFabTitleDataCache.DataRequest dataRequest in this.requests)
				{
					stringVector.Add(dataRequest.Name);
				}
				CS$<>8__locals1.finished = false;
				Debug.Log("[PlayFabTitleDataCache::UpdateDataCo] Keys to fetch: " + string.Join(", ", stringVector));
				Debug.Log(string.Format("[PlayFabTitleDataCache::UpdateDataCo] Calling MothershipClientApiUnity.ListMothershipTitleData with TitleId={0}, EnvironmentId={1}, DeploymentId={2}, keys count={3}", new object[]
				{
					MothershipClientApiUnity.TitleId,
					MothershipClientApiUnity.EnvironmentId,
					MothershipClientApiUnity.DeploymentId,
					stringVector.Count
				}));
				if (!MothershipClientApiUnity.ListMothershipTitleData(MothershipClientApiUnity.TitleId, MothershipClientApiUnity.EnvironmentId, MothershipClientApiUnity.DeploymentId, stringVector, delegate(ListClientMothershipTitleDataResponse response)
				{
					string text6 = "[PlayFabTitleDataCache::UpdateDataCo] Mothership API success callback - Response: {0}, Results: {1}";
					object obj = response != null;
					int? num;
					if (response == null)
					{
						num = default(int?);
					}
					else
					{
						TitleDataShortVector results = response.Results;
						num = ((results != null) ? new int?(results.Count) : default(int?));
					}
					int? num2 = num;
					Debug.Log(string.Format(text6, obj, num2.GetValueOrDefault()));
					if (response != null && response.Results != null)
					{
						CS$<>8__locals1.newTitleData = new Dictionary<string, string>();
						for (int j = 0; j < response.Results.Count; j++)
						{
							MothershipTitleDataShort mothershipTitleDataShort = response.Results[j];
							string text7 = "[PlayFabTitleDataCache::UpdateDataCo] Processing title data item {0}: key='{1}', data length={2}";
							object obj2 = j;
							object key = mothershipTitleDataShort.key;
							string data = mothershipTitleDataShort.data;
							Debug.Log(string.Format(text7, obj2, key, (data != null) ? data.Length : 0));
							if (!string.IsNullOrEmpty(mothershipTitleDataShort.key))
							{
								CS$<>8__locals1.newTitleData[mothershipTitleDataShort.key] = mothershipTitleDataShort.data;
							}
						}
						CS$<>8__locals1.mothershipError = null;
						Debug.Log(string.Format("[PlayFabTitleDataCache::UpdateDataCo] Successfully processed {0} title data items", CS$<>8__locals1.newTitleData.Count));
					}
					else
					{
						CS$<>8__locals1.mothershipError = "Failed to fetch title data - response or results were null";
						Debug.LogError("[PlayFabTitleDataCache::UpdateDataCo] " + CS$<>8__locals1.mothershipError);
					}
					CS$<>8__locals1.finished = true;
				}, delegate(MothershipError error, int statusCode)
				{
					CS$<>8__locals1.mothershipError = string.Format("Error fetching title data: {0} (Status: {1})", ((error != null) ? error.Message : null) ?? "Unknown error", statusCode);
					Debug.LogError("[PlayFabTitleDataCache::UpdateDataCo] Mothership API error callback - " + CS$<>8__locals1.mothershipError);
					CS$<>8__locals1.finished = true;
				}))
				{
					CS$<>8__locals1.mothershipError = "Mothership API call was not sent.";
					Debug.LogError("[PlayFabTitleDataCache::UpdateDataCo] " + CS$<>8__locals1.mothershipError);
				}
				Debug.Log("[PlayFabTitleDataCache::UpdateDataCo] Waiting for Mothership API response");
				yield return new WaitUntil(() => CS$<>8__locals1.finished);
				Debug.Log(string.Format("[PlayFabTitleDataCache::UpdateDataCo] {0:N5}s", sw.Elapsed.TotalSeconds));
				if (CS$<>8__locals1.newTitleData != null)
				{
					Debug.Log(string.Format("[PlayFabTitleDataCache::UpdateDataCo] Processing {0} new title data items", CS$<>8__locals1.newTitleData.Count));
					if (wipeOldData)
					{
						this.localizedTitleData.Clear();
						this.localizedTitleData[currentLocale] = new Dictionary<string, string>();
						titleData = this.localizedTitleData[currentLocale];
					}
					if (!this.localesUpdated.ContainsKey(currentLocale))
					{
						titleData.Clear();
					}
					foreach (KeyValuePair<string, string> keyValuePair in CS$<>8__locals1.newTitleData)
					{
						string text;
						string text2;
						keyValuePair.Deconstruct(ref text, ref text2);
						string text3 = text;
						string text4 = text2;
						Debug.Log("[PlayFabTitleDataCache::UpdateDataCo] Updating title data key: " + text3);
						titleData[text3] = text4;
						for (int i = this.requests.Count - 1; i >= 0; i--)
						{
							PlayFabTitleDataCache.DataRequest dataRequest2 = this.requests[i];
							if (dataRequest2.Name == text3)
							{
								Action<string> callback = dataRequest2.Callback;
								if (callback != null)
								{
									callback.Invoke(text4);
								}
								this.requests.RemoveAt(i);
								break;
							}
						}
						string text5;
						if (oldLocalizedCache.TryGetValue(text3, ref text5) && text5 != text4)
						{
							PlayFabTitleDataCache.DataUpdate onTitleDataUpdate = this.OnTitleDataUpdate;
							if (onTitleDataUpdate != null)
							{
								onTitleDataUpdate.Invoke(text3);
							}
						}
					}
					this.localesUpdated[currentLocale] = true;
					PlayFabTitleDataCache.SaveDataToFile(PlayFabTitleDataCache.FilePath, this.localizedTitleData);
				}
				CS$<>8__locals1 = null;
				oldCache = null;
				currentLocale = null;
				titleData = null;
				oldLocalizedCache = null;
				sw = null;
			}
			finally
			{
				this.ClearRequestWithError(null);
				this.isFirstLoad = false;
				this.updateDataCoroutine = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x0600612D RID: 24877 RVA: 0x001F4E9C File Offset: 0x001F309C
		private static string MD5(string value)
		{
			HashAlgorithm hashAlgorithm = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.Default.GetBytes(value);
			byte[] array = hashAlgorithm.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600612E RID: 24878 RVA: 0x001F4EF4 File Offset: 0x001F30F4
		private void ClearRequestWithError(PlayFabError e = null)
		{
			if (e == null)
			{
				e = new PlayFabError();
			}
			foreach (PlayFabTitleDataCache.DataRequest dataRequest in this.requests)
			{
				dataRequest.ErrorCallback.SafeInvoke(e);
			}
			this.requests.Clear();
		}

		// Token: 0x04006FED RID: 28653
		public PlayFabTitleDataCache.DataUpdate OnTitleDataUpdate;

		// Token: 0x04006FEE RID: 28654
		private const string FileName = "TitleDataCache.json";

		// Token: 0x04006FEF RID: 28655
		private readonly List<PlayFabTitleDataCache.DataRequest> requests = new List<PlayFabTitleDataCache.DataRequest>();

		// Token: 0x04006FF0 RID: 28656
		private Dictionary<string, Dictionary<string, string>> localizedTitleData = new Dictionary<string, Dictionary<string, string>>();

		// Token: 0x04006FF1 RID: 28657
		private Dictionary<string, bool> localesUpdated = new Dictionary<string, bool>();

		// Token: 0x04006FF2 RID: 28658
		private bool isFirstLoad = true;

		// Token: 0x04006FF3 RID: 28659
		private Coroutine updateDataCoroutine;

		// Token: 0x02000F25 RID: 3877
		[Serializable]
		public sealed class DataUpdate : UnityEvent<string>
		{
		}

		// Token: 0x02000F26 RID: 3878
		private class DataRequest
		{
			// Token: 0x170008FD RID: 2301
			// (get) Token: 0x06006131 RID: 24881 RVA: 0x001F4F98 File Offset: 0x001F3198
			// (set) Token: 0x06006132 RID: 24882 RVA: 0x001F4FA0 File Offset: 0x001F31A0
			public string Name { get; set; }

			// Token: 0x170008FE RID: 2302
			// (get) Token: 0x06006133 RID: 24883 RVA: 0x001F4FA9 File Offset: 0x001F31A9
			// (set) Token: 0x06006134 RID: 24884 RVA: 0x001F4FB1 File Offset: 0x001F31B1
			public Action<string> Callback { get; set; }

			// Token: 0x170008FF RID: 2303
			// (get) Token: 0x06006135 RID: 24885 RVA: 0x001F4FBA File Offset: 0x001F31BA
			// (set) Token: 0x06006136 RID: 24886 RVA: 0x001F4FC2 File Offset: 0x001F31C2
			public Action<PlayFabError> ErrorCallback { get; set; }
		}
	}
}
