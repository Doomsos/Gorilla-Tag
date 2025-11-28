using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000F0B RID: 3851
	public class TitleDataFeatureFlags
	{
		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06006087 RID: 24711 RVA: 0x001F1CAC File Offset: 0x001EFEAC
		// (set) Token: 0x06006088 RID: 24712 RVA: 0x001F1CB4 File Offset: 0x001EFEB4
		public bool ready { get; private set; }

		// Token: 0x06006089 RID: 24713 RVA: 0x001F1CBD File Offset: 0x001EFEBD
		public void FetchFeatureFlags()
		{
			PlayFabTitleDataCache.Instance.GetTitleData(this.TitleDataKey, delegate(string json)
			{
				FeatureFlagListData featureFlagListData = JsonUtility.FromJson<FeatureFlagListData>(json);
				foreach (FeatureFlagData featureFlagData in featureFlagListData.flags)
				{
					if (featureFlagData.valueType == "percent")
					{
						this.flagValueByName.AddOrUpdate(featureFlagData.name, featureFlagData.value);
					}
					List<string> alwaysOnForUsers = featureFlagData.alwaysOnForUsers;
					if (alwaysOnForUsers != null && alwaysOnForUsers.Count > 0)
					{
						this.flagValueByUser.AddOrUpdate(featureFlagData.name, featureFlagData.alwaysOnForUsers);
					}
				}
				Debug.Log(string.Format("GorillaServer: Fetched flags ({0})", featureFlagListData));
				this.ready = true;
			}, delegate(PlayFabError e)
			{
				Debug.LogError("Error fetching rollout feature flags: " + e.ErrorMessage);
				this.ready = true;
			}, false);
		}

		// Token: 0x0600608A RID: 24714 RVA: 0x001F1CE8 File Offset: 0x001EFEE8
		public bool IsEnabledForUser(string flagName)
		{
			bool flag;
			this.logSent.TryGetValue(flagName, ref flag);
			this.logSent[flagName] = true;
			string playFabPlayerId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			if (!flag)
			{
				Debug.Log(string.Concat(new string[]
				{
					"GorillaServer: Checking flag ",
					flagName,
					" for ",
					playFabPlayerId,
					"\nFlag values:\n",
					JsonConvert.SerializeObject(this.flagValueByName),
					"\n\nDefaults:\n",
					JsonConvert.SerializeObject(this.defaults)
				}));
			}
			List<string> list;
			if (this.flagValueByUser.TryGetValue(flagName, ref list) && list != null && list.Contains(playFabPlayerId))
			{
				return true;
			}
			int num;
			if (!this.flagValueByName.TryGetValue(flagName, ref num))
			{
				if (!flag)
				{
					Debug.Log("GorillaServer: Returning default");
				}
				bool flag2;
				return this.defaults.TryGetValue(flagName, ref flag2) && flag2;
			}
			if (!flag)
			{
				Debug.Log(string.Format("GorillaServer: Rollout % is {0}", num));
			}
			if (num <= 0)
			{
				if (!flag)
				{
					Debug.Log("GorillaServer: " + flagName + " is off (<=0%).");
				}
				return false;
			}
			if (num >= 100)
			{
				if (!flag)
				{
					Debug.Log("GorillaServer: " + flagName + " is on (>=100%).");
				}
				return true;
			}
			uint num2 = XXHash32.Compute(Encoding.UTF8.GetBytes(playFabPlayerId), 0U) % 100U;
			if (!flag)
			{
				Debug.Log(string.Format("GorillaServer: Partial rollout, seed = {0} flag value = {1}", num2, (ulong)num2 < (ulong)((long)num)));
			}
			return (ulong)num2 < (ulong)((long)num);
		}

		// Token: 0x0600608B RID: 24715 RVA: 0x001F1E60 File Offset: 0x001F0060
		public TitleDataFeatureFlags()
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			dictionary.Add("2024-06-CosmeticsAuthenticationV2", true);
			dictionary.Add("2025-04-CosmeticsAuthenticationV2-SetData", false);
			dictionary.Add("2025-04-CosmeticsAuthenticationV2-ReadData", false);
			dictionary.Add("2025-04-CosmeticsAuthenticationV2-Compat", true);
			this.defaults = dictionary;
			this.flagValueByName = new Dictionary<string, int>();
			this.flagValueByUser = new Dictionary<string, List<string>>();
			this.logSent = new Dictionary<string, bool>();
			base..ctor();
		}

		// Token: 0x04006F26 RID: 28454
		public string TitleDataKey = "DeployFeatureFlags";

		// Token: 0x04006F28 RID: 28456
		public Dictionary<string, bool> defaults;

		// Token: 0x04006F29 RID: 28457
		private Dictionary<string, int> flagValueByName;

		// Token: 0x04006F2A RID: 28458
		private Dictionary<string, List<string>> flagValueByUser;

		// Token: 0x04006F2B RID: 28459
		private Dictionary<string, bool> logSent;
	}
}
