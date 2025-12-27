using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using PlayFab;
using UnityEngine;

namespace GorillaNetworking
{
	public class TitleDataFeatureFlags
	{
		public bool ready { get; private set; }

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

		public string TitleDataKey = "DeployFeatureFlags";

		public Dictionary<string, bool> defaults;

		private Dictionary<string, int> flagValueByName;

		private Dictionary<string, List<string>> flagValueByUser;

		private Dictionary<string, bool> logSent;
	}
}
