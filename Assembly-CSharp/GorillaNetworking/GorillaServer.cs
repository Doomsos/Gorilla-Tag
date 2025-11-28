using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000F05 RID: 3845
	public class GorillaServer : MonoBehaviour, ISerializationCallbackReceiver
	{
		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x0600605C RID: 24668 RVA: 0x001F1254 File Offset: 0x001EF454
		public bool FeatureFlagsReady
		{
			get
			{
				return this.featureFlags.ready;
			}
		}

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x0600605D RID: 24669 RVA: 0x001F1261 File Offset: 0x001EF461
		private EntityKey playerEntity
		{
			get
			{
				return new EntityKey
				{
					Id = PlayFabSettings.staticPlayer.EntityId,
					Type = PlayFabSettings.staticPlayer.EntityType
				};
			}
		}

		// Token: 0x0600605E RID: 24670 RVA: 0x001F1288 File Offset: 0x001EF488
		public void Start()
		{
			this.featureFlags.FetchFeatureFlags();
		}

		// Token: 0x0600605F RID: 24671 RVA: 0x001F1295 File Offset: 0x001EF495
		private void Awake()
		{
			if (GorillaServer.Instance == null)
			{
				GorillaServer.Instance = this;
				return;
			}
			Object.Destroy(this);
		}

		// Token: 0x06006060 RID: 24672 RVA: 0x001F12B8 File Offset: 0x001EF4B8
		public void ReturnCurrentVersion(ReturnCurrentVersionRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnCurrentVersion result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnCurrentVersion error");
			Debug.Log("GorillaServer: ReturnCurrentVersion V2 call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnCurrentVersionV2",
				FunctionParameter = request
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006061 RID: 24673 RVA: 0x001F1318 File Offset: 0x001EF518
		public void ReturnMyOculusHash(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnMyOculusHash result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnMyOculusHash error");
			Debug.Log("GorillaServer: ReturnMyOculusHash V2 call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnMyOculusHashV2",
				FunctionParameter = new
				{

				}
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006062 RID: 24674 RVA: 0x001F137C File Offset: 0x001EF57C
		public void TryDistributeCurrency(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "TryDistributeCurrency result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "TryDistributeCurrency error");
			Debug.Log("GorillaServer: TryDistributeCurrency V2 call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "TryDistributeCurrencyV2",
				FunctionParameter = new
				{

				}
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006063 RID: 24675 RVA: 0x001F13E0 File Offset: 0x001EF5E0
		public void AddOrRemoveDLCOwnership(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "AddOrRemoveDLCOwnership result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "AddOrRemoveDLCOwnership error");
			Debug.Log("GorillaServer: AddOrRemoveDLCOwnership V2 call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "AddOrRemoveDLCOwnershipV2",
				FunctionParameter = new
				{

				}
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006064 RID: 24676 RVA: 0x001F1444 File Offset: 0x001EF644
		public void BroadcastMyRoom(BroadcastMyRoomRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "BroadcastMyRoom result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "BroadcastMyRoom error");
			Debug.Log(string.Format("GorillaServer: BroadcastMyRoom V2 call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "BroadcastMyRoomV2",
				FunctionParameter = request
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006065 RID: 24677 RVA: 0x001F14A9 File Offset: 0x001EF6A9
		public bool NewCosmeticsPath()
		{
			return this.featureFlags.IsEnabledForUser("2024-06-CosmeticsAuthenticationV2");
		}

		// Token: 0x06006066 RID: 24678 RVA: 0x001F14BB File Offset: 0x001EF6BB
		public bool NewCosmeticsPathShouldSetSharedGroupData()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-SetData");
		}

		// Token: 0x06006067 RID: 24679 RVA: 0x001F14CD File Offset: 0x001EF6CD
		public bool NewCosmeticsPathShouldReadSharedGroupData()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-ReadData");
		}

		// Token: 0x06006068 RID: 24680 RVA: 0x001F14DF File Offset: 0x001EF6DF
		public bool NewCosmeticsPathShouldSetRoomData()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-CosmeticsAuthenticationV2-Compat");
		}

		// Token: 0x06006069 RID: 24681 RVA: 0x001F14F4 File Offset: 0x001EF6F4
		public void UpdateUserCosmetics()
		{
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "UpdatePersonalCosmeticsList";
			executeFunctionRequest.FunctionParameter = new
			{

			};
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
				if (CosmeticsController.instance != null)
				{
					CosmeticsController.instance.CheckCosmeticsSharedGroup();
				}
			}, delegate(PlayFabError error)
			{
			}, null, null);
		}

		// Token: 0x0600606A RID: 24682 RVA: 0x001F157C File Offset: 0x001EF77C
		public void GetAcceptedAgreements(GetAcceptedAgreementsRequest request, Action<Dictionary<string, string>> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<Dictionary<string, string>>(successCallback, "GetAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetAcceptedAgreements json error");
			Debug.Log(string.Format("GorillaServer: GetAcceptedAgreements call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetAcceptedAgreements",
				FunctionParameter = string.Join(",", request.AgreementKeys),
				GeneratePlayStreamEvent = new bool?(false)
			}, delegate(ExecuteFunctionResult result)
			{
				try
				{
					string text = Convert.ToString(result.FunctionResult);
					successCallback.Invoke(JsonConvert.DeserializeObject<Dictionary<string, string>>(text));
				}
				catch (Exception ex)
				{
					errorCallback.Invoke(new PlayFabError
					{
						ErrorMessage = string.Format("Invalid format for GetAcceptedAgreements ({0})", ex),
						Error = 3
					});
				}
			}, errorCallback, null, null);
		}

		// Token: 0x0600606B RID: 24683 RVA: 0x001F1634 File Offset: 0x001EF834
		public void SubmitAcceptedAgreements(SubmitAcceptedAgreementsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "SubmitAcceptedAgreements result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "SubmitAcceptedAgreements error");
			Debug.Log(string.Format("GorillaServer: SubmitAcceptedAgreements call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "SubmitAcceptedAgreements",
				FunctionParameter = request.Agreements,
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x0600606C RID: 24684 RVA: 0x001F16AC File Offset: 0x001EF8AC
		public void UploadGorillanalytics(object uploadData)
		{
			Debug.Log(string.Format("GorillaServer: UploadGorillanalytics call ({0})", uploadData));
			ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
			executeFunctionRequest.Entity = this.playerEntity;
			executeFunctionRequest.FunctionName = "Gorillanalytics";
			executeFunctionRequest.FunctionParameter = uploadData;
			executeFunctionRequest.GeneratePlayStreamEvent = new bool?(false);
			PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
			{
				Debug.Log(string.Format("The {0} function took {1} to complete", result.FunctionName, result.ExecutionTimeMilliseconds));
			}, delegate(PlayFabError error)
			{
				Debug.Log("Error uploading Gorillanalytics: " + error.GenerateErrorReport());
			}, null, null);
		}

		// Token: 0x0600606D RID: 24685 RVA: 0x001F1740 File Offset: 0x001EF940
		public void CheckForBadName(CheckForBadNameRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "CheckForBadName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "CheckForBadName error");
			Debug.Log(string.Format("GorillaServer: CheckForBadName call ({0})", request));
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "CheckForBadName",
				FunctionParameter = new
				{
					name = request.name,
					forRoom = request.forRoom.ToString(),
					forTroop = request.forTroop.ToString()
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x0600606E RID: 24686 RVA: 0x001F17D4 File Offset: 0x001EF9D4
		public void GetRandomName(Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "GetRandomName result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "GetRandomName error");
			Debug.Log("GorillaServer: GetRandomName call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "GetRandomName",
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x0600606F RID: 24687 RVA: 0x001F1838 File Offset: 0x001EFA38
		public void ReturnQueueStats(ReturnQueueStatsRequest request, Action<ExecuteFunctionResult> successCallback, Action<PlayFabError> errorCallback)
		{
			successCallback = this.DebugWrapCb<ExecuteFunctionResult>(successCallback, "ReturnQueueStats result");
			errorCallback = this.DebugWrapCb<PlayFabError>(errorCallback, "ReturnQueueStats error");
			Debug.Log("GorillaServer: ReturnQueueStats call");
			PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest
			{
				Entity = this.playerEntity,
				FunctionName = "ReturnQueueStats",
				FunctionParameter = new
				{
					QueueName = request.queueName
				},
				GeneratePlayStreamEvent = new bool?(false)
			}, successCallback, errorCallback, null, null);
		}

		// Token: 0x06006070 RID: 24688 RVA: 0x001F18AD File Offset: 0x001EFAAD
		private Action<T> DebugWrapCb<T>(Action<T> cb, string label)
		{
			return delegate(T arg)
			{
				if (this.debug)
				{
					try
					{
						Debug.Log(string.Concat(new string[]
						{
							"GorillaServer: ",
							label,
							" (",
							JsonConvert.SerializeObject(arg, this.serializationSettings),
							")"
						}));
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("GorillaServer: {0} Error printing failure log: {1}", label, ex));
					}
				}
				cb.Invoke(arg);
			};
		}

		// Token: 0x06006071 RID: 24689 RVA: 0x001F18D4 File Offset: 0x001EFAD4
		private ExecuteFunctionResult toFunctionResult(ExecuteCloudScriptResult csResult)
		{
			FunctionExecutionError error = null;
			if (csResult.Error != null)
			{
				error = new FunctionExecutionError
				{
					Error = csResult.Error.Error,
					Message = csResult.Error.Message,
					StackTrace = csResult.Error.StackTrace
				};
			}
			return new ExecuteFunctionResult
			{
				CustomData = csResult.CustomData,
				Error = error,
				ExecutionTimeMilliseconds = Convert.ToInt32(Math.Round(csResult.ExecutionTimeSeconds * 1000.0)),
				FunctionName = csResult.FunctionName,
				FunctionResult = csResult.FunctionResult,
				FunctionResultTooLarge = csResult.FunctionResultTooLarge
			};
		}

		// Token: 0x06006072 RID: 24690 RVA: 0x001F1980 File Offset: 0x001EFB80
		public void OnBeforeSerialize()
		{
			this.FeatureFlagsTitleDataKey = this.featureFlags.TitleDataKey;
			this.DefaultDeployFeatureFlagsEnabled.Clear();
			foreach (KeyValuePair<string, bool> keyValuePair in this.featureFlags.defaults)
			{
				if (keyValuePair.Value)
				{
					this.DefaultDeployFeatureFlagsEnabled.Add(keyValuePair.Key);
				}
			}
		}

		// Token: 0x06006073 RID: 24691 RVA: 0x001F1A08 File Offset: 0x001EFC08
		public void OnAfterDeserialize()
		{
			this.featureFlags.TitleDataKey = this.FeatureFlagsTitleDataKey;
			foreach (string key in this.DefaultDeployFeatureFlagsEnabled)
			{
				this.featureFlags.defaults.AddOrUpdate(key, true);
			}
		}

		// Token: 0x06006074 RID: 24692 RVA: 0x001F1A78 File Offset: 0x001EFC78
		public bool CheckIsInKIDOptInCohort()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-KIDOptIn");
		}

		// Token: 0x06006075 RID: 24693 RVA: 0x001F1A8A File Offset: 0x001EFC8A
		public bool CheckIsInKIDRequiredCohort()
		{
			return this.featureFlags.IsEnabledForUser("2025-04-KIDRequired");
		}

		// Token: 0x06006076 RID: 24694 RVA: 0x001F1A9C File Offset: 0x001EFC9C
		public bool CheckOptedInKID()
		{
			return KIDManager.HasOptedInToKID;
		}

		// Token: 0x06006077 RID: 24695 RVA: 0x001F1AA3 File Offset: 0x001EFCA3
		public bool CheckIsTZE_Enabled()
		{
			return this.featureFlags.IsEnabledForUser("2025-10-TelemetryZoneEventSampling");
		}

		// Token: 0x06006078 RID: 24696 RVA: 0x001F1AB5 File Offset: 0x001EFCB5
		public bool CheckIsMothershipTelemetryEnabled()
		{
			return this.featureFlags.IsEnabledForUser("2025-09-MothershipAnalyticsSampleRate");
		}

		// Token: 0x06006079 RID: 24697 RVA: 0x001F1AC7 File Offset: 0x001EFCC7
		public bool CheckIsPlayFabTelemetryEnabled()
		{
			return this.featureFlags.IsEnabledForUser("2025-09-PlayFabAnalyticsSampleRate");
		}

		// Token: 0x04006F11 RID: 28433
		public static volatile GorillaServer Instance;

		// Token: 0x04006F12 RID: 28434
		public string FeatureFlagsTitleDataKey = "DeployFeatureFlags";

		// Token: 0x04006F13 RID: 28435
		public List<string> DefaultDeployFeatureFlagsEnabled = new List<string>();

		// Token: 0x04006F14 RID: 28436
		private TitleDataFeatureFlags featureFlags = new TitleDataFeatureFlags();

		// Token: 0x04006F15 RID: 28437
		private bool debug;

		// Token: 0x04006F16 RID: 28438
		private JsonSerializerSettings serializationSettings = new JsonSerializerSettings
		{
			NullValueHandling = 1,
			DefaultValueHandling = 1,
			MissingMemberHandling = 0,
			ObjectCreationHandling = 2,
			ReferenceLoopHandling = 1,
			TypeNameHandling = 4
		};
	}
}
