using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GorillaNetworking;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020007C1 RID: 1985
public class GorillaTagCompetitiveServerApi : MonoBehaviour
{
	// Token: 0x06003443 RID: 13379 RVA: 0x00118A27 File Offset: 0x00116C27
	private void Awake()
	{
		if (GorillaTagCompetitiveServerApi.Instance)
		{
			GTDev.LogError<string>("Duplicate GorillaTagCompetitiveServerApi detected. Destroying self.", base.gameObject, null);
			Object.Destroy(this);
			return;
		}
		GorillaTagCompetitiveServerApi.Instance = this;
	}

	// Token: 0x06003444 RID: 13380 RVA: 0x00118A54 File Offset: 0x00116C54
	public void RequestGetRankInformation(List<string> playfabs, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestGetRankInformation Client Not Logged into Mothership", null);
			return;
		}
		if (this.GetRankInformationInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestGetRankInformation already in progress", null);
			return;
		}
		this.GetRankInformationInProgress = true;
		string platform = "PC";
		base.StartCoroutine(this.GetRankInformation(new GorillaTagCompetitiveServerApi.RankedModeProgressionRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			playfabIds = playfabs
		}, callback));
	}

	// Token: 0x06003445 RID: 13381 RVA: 0x00118ADD File Offset: 0x00116CDD
	private IEnumerator GetRankInformation(GorillaTagCompetitiveServerApi.RankedModeProgressionRequestData data, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/GetTier", "GET");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			GTDev.Log<string>("GetRankInformation Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteGetRankInformation(request.downloadHandler.text, callback);
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
					goto IL_156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_156;
			}
			bool flag = true;
			goto IL_159;
			IL_156:
			flag = false;
			IL_159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteGetRankInformation(null, callback);
			}
		}
		if (retry)
		{
			if (this.GetRankInformationRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.GetRankInformationRetryCount + 1)));
				this.GetRankInformationRetryCount++;
				yield return new WaitForSeconds(num);
				this.GetRankInformationInProgress = false;
				this.RequestGetRankInformation(data.playfabIds, callback);
			}
			else
			{
				this.GetRankInformationRetryCount = 0;
				this.OnCompleteGetRankInformation(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06003446 RID: 13382 RVA: 0x00118AFC File Offset: 0x00116CFC
	private void OnCompleteGetRankInformation([CanBeNull] string response, Action<GorillaTagCompetitiveServerApi.RankedModeProgressionData> callback)
	{
		this.GetRankInformationInProgress = false;
		this.GetRankInformationRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		string text = "{ \"playerData\": " + response + " }";
		GorillaTagCompetitiveServerApi.RankedModeProgressionData rankedModeProgressionData;
		try
		{
			rankedModeProgressionData = JsonUtility.FromJson<GorillaTagCompetitiveServerApi.RankedModeProgressionData>(text);
		}
		catch (ArgumentException ex)
		{
			Debug.LogException(ex);
			Debug.LogError("[GT/GorillaTagCompetitiveServerApi]  ERROR!!!  OnCompleteGetRankInformation: Encountered ArgumentException above while trying to parse json string:\n" + text);
			return;
		}
		catch (Exception ex2)
		{
			Debug.LogException(ex2);
			Debug.LogError("[GT/GorillaTagCompetitiveServerApi]  ERROR!!!  OnCompleteGetRankInformation: Encountered exception above while trying to parse json string:\n" + text);
			return;
		}
		if (callback != null)
		{
			callback.Invoke(rankedModeProgressionData);
		}
	}

	// Token: 0x06003447 RID: 13383 RVA: 0x00118B90 File Offset: 0x00116D90
	public void RequestCreateMatchId(Action<string> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestCreateMatchId Client Not Logged into Mothership", null);
			return;
		}
		if (this.CreateMatchIdInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestCreateMatchId already in progress", null);
			return;
		}
		string platform = "PC";
		this.CreateMatchIdInProgress = true;
		base.StartCoroutine(this.CreateMatchId(new GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform
		}, callback));
	}

	// Token: 0x06003448 RID: 13384 RVA: 0x00118C12 File Offset: 0x00116E12
	private IEnumerator CreateMatchId(GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed data, Action<string> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/CreateMatchId", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			GTDev.Log<string>("CreateMatchId Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteCreateMatchId(request.downloadHandler.text, callback);
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
					goto IL_156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_156;
			}
			bool flag = true;
			goto IL_159;
			IL_156:
			flag = false;
			IL_159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteCreateMatchId(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.CreateMatchIdRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.CreateMatchIdRetryCount + 1)));
				this.CreateMatchIdRetryCount++;
				yield return new WaitForSeconds(num);
				this.CreateMatchIdInProgress = false;
				this.RequestCreateMatchId(callback);
			}
			else
			{
				this.CreateMatchIdRetryCount = 0;
				this.OnCompleteCreateMatchId(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06003449 RID: 13385 RVA: 0x00118C2F File Offset: 0x00116E2F
	private void OnCompleteCreateMatchId([CanBeNull] string response, Action<string> callback)
	{
		this.CreateMatchIdInProgress = false;
		this.CreateMatchIdRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		if (callback != null)
		{
			callback.Invoke(response);
		}
	}

	// Token: 0x0600344A RID: 13386 RVA: 0x00118C54 File Offset: 0x00116E54
	public void RequestValidateMatchJoin(string matchId, Action<bool> callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestValidateMatchJoin Client Not Logged into Mothership", null);
			return;
		}
		if (this.ValidateMatchJoinInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestValidateMatchJoin already in progress", null);
			return;
		}
		string platform = "PC";
		this.ValidateMatchJoinInProgress = true;
		base.StartCoroutine(this.ValidateMatchJoin(new GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			matchId = matchId
		}, callback));
	}

	// Token: 0x0600344B RID: 13387 RVA: 0x00118CDD File Offset: 0x00116EDD
	private IEnumerator ValidateMatchJoin(GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/ValidateMatchJoin", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			GTDev.Log<string>("ValidateMatchJoin Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteValidateMatchJoin(request.downloadHandler.text, callback);
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
					goto IL_156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_156;
			}
			bool flag = true;
			goto IL_159;
			IL_156:
			flag = false;
			IL_159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteValidateMatchJoin(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.ValidateMatchJoinRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.ValidateMatchJoinRetryCount + 1)));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSeconds(num);
				this.ValidateMatchJoinInProgress = false;
				this.RequestValidateMatchJoin(data.matchId, callback);
			}
			else
			{
				this.ValidateMatchJoinRetryCount = 0;
				this.OnCompleteValidateMatchJoin(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x0600344C RID: 13388 RVA: 0x00118CFC File Offset: 0x00116EFC
	private void OnCompleteValidateMatchJoin([CanBeNull] string response, Action<bool> callback)
	{
		this.ValidateMatchJoinInProgress = false;
		this.ValidateMatchJoinRetryCount = 0;
		if (response.IsNullOrEmpty())
		{
			return;
		}
		GorillaTagCompetitiveServerApi.RankedModeValidateMatchJoinResponseData rankedModeValidateMatchJoinResponseData = JsonUtility.FromJson<GorillaTagCompetitiveServerApi.RankedModeValidateMatchJoinResponseData>(response);
		if (callback != null)
		{
			callback.Invoke(rankedModeValidateMatchJoinResponseData.validJoin);
		}
	}

	// Token: 0x0600344D RID: 13389 RVA: 0x00118D38 File Offset: 0x00116F38
	public void RequestSubmitMatchScores(string matchId, List<RankedMultiplayerScore.PlayerScore> finalScores)
	{
		List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> list = new List<GorillaTagCompetitiveServerApi.RankedModePlayerScore>();
		foreach (RankedMultiplayerScore.PlayerScore playerScore in finalScores)
		{
			NetPlayer player = NetworkSystem.Instance.GetPlayer(playerScore.PlayerId);
			list.Add(new GorillaTagCompetitiveServerApi.RankedModePlayerScore
			{
				playfabId = player.UserId,
				gameScore = playerScore.GameScore
			});
		}
		this.RequestSubmitMatchScores(matchId, list);
	}

	// Token: 0x0600344E RID: 13390 RVA: 0x00118DC4 File Offset: 0x00116FC4
	private void RequestSubmitMatchScores(string matchId, List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> playerScores)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSubmitMatchScores Client Not Logged into Mothership", null);
			return;
		}
		if (this.SubmitMatchScoresInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSubmitMatchScores already in progress", null);
			return;
		}
		this.SubmitMatchScoresInProgress = true;
		base.StartCoroutine(this.SubmitMatchScores(new GorillaTagCompetitiveServerApi.RankedModeSubmitMatchScoresRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			matchId = matchId,
			playfabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			playerScores = playerScores
		}));
	}

	// Token: 0x0600344F RID: 13391 RVA: 0x00118E52 File Offset: 0x00117052
	private IEnumerator SubmitMatchScores(GorillaTagCompetitiveServerApi.RankedModeSubmitMatchScoresRequestData data)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/SubmitMatchScores", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			GTDev.Log<string>("SubmitMatchScores Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteSubmitMatchScores(request.downloadHandler.text);
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
					goto IL_150;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_150;
			}
			bool flag = true;
			goto IL_153;
			IL_150:
			flag = false;
			IL_153:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteSubmitMatchScores(request.downloadHandler.text);
			}
		}
		if (retry)
		{
			if (this.SubmitMatchScoresRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.SubmitMatchScoresRetryCount + 1)));
				this.SubmitMatchScoresRetryCount++;
				yield return new WaitForSeconds(num);
				this.SubmitMatchScoresInProgress = false;
				this.RequestSubmitMatchScores(data.matchId, data.playerScores);
			}
			else
			{
				this.SubmitMatchScoresRetryCount = 0;
				this.OnCompleteSubmitMatchScores(null);
			}
		}
		yield break;
	}

	// Token: 0x06003450 RID: 13392 RVA: 0x00118E68 File Offset: 0x00117068
	private void OnCompleteSubmitMatchScores([CanBeNull] string response)
	{
		this.SubmitMatchScoresInProgress = false;
		this.SubmitMatchScoresRetryCount = 0;
	}

	// Token: 0x06003451 RID: 13393 RVA: 0x00118E78 File Offset: 0x00117078
	public void RequestSetEloValue(float desiredElo, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSetEloValue Client Not Logged into Mothership", null);
			return;
		}
		if (this.SetEloValueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestSetEloValue already in progress", null);
			return;
		}
		string platform = "PC";
		this.SetEloValueInProgress = true;
		base.StartCoroutine(this.SetEloValue(new GorillaTagCompetitiveServerApi.RankedModeSetEloValueRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			elo = desiredElo
		}, callback));
	}

	// Token: 0x06003452 RID: 13394 RVA: 0x00118F01 File Offset: 0x00117101
	private IEnumerator SetEloValue(GorillaTagCompetitiveServerApi.RankedModeSetEloValueRequestData data, Action callback)
	{
		GTDev.LogWarning<string>("SetEloValue is for internal use only (Is Beta)", null);
		yield break;
	}

	// Token: 0x06003453 RID: 13395 RVA: 0x00118F09 File Offset: 0x00117109
	private void OnCompleteSetEloValue([CanBeNull] string response, Action callback)
	{
		this.SetEloValueInProgress = false;
		this.SetEloValueRetryCount = 0;
		if (response != null && callback != null)
		{
			callback.Invoke();
		}
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x00118F28 File Offset: 0x00117128
	public void RequestPingRoom(string matchId, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestPingRoom Client Not Logged into Mothership", null);
			return;
		}
		if (this.SetEloValueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestPingRoom already in progress", null);
			return;
		}
		string platform = "PC";
		this.PingMatchInProgress = true;
		base.StartCoroutine(this.PingRoom(new GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			matchId = matchId
		}, callback));
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x00118FB1 File Offset: 0x001171B1
	private IEnumerator PingRoom(GorillaTagCompetitiveServerApi.RankedModeRequestDataWithMatchId data, Action callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/PingRoom", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			GTDev.Log<string>("PingRoom Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompletePingRoom(request.downloadHandler.text, callback);
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
					goto IL_156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_156;
			}
			bool flag = true;
			goto IL_159;
			IL_156:
			flag = false;
			IL_159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompletePingRoom(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.PingMatchRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.PingMatchRetryCount + 1)));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSeconds(num);
				this.PingMatchInProgress = false;
				this.RequestPingRoom(data.matchId, callback);
			}
			else
			{
				this.PingMatchRetryCount = 0;
				this.OnCompletePingRoom(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06003456 RID: 13398 RVA: 0x00118FCE File Offset: 0x001171CE
	private void OnCompletePingRoom([CanBeNull] string response, Action callback)
	{
		GTDev.Log<string>("PingRoom complete", null);
		this.PingMatchInProgress = false;
		this.PingMatchRetryCount = 0;
		if (response != null && callback != null)
		{
			callback.Invoke();
		}
	}

	// Token: 0x06003457 RID: 13399 RVA: 0x00118FF8 File Offset: 0x001171F8
	public void RequestUnlockCompetitiveQueue(bool unlocked, Action callback)
	{
		if (!MothershipClientContext.IsClientLoggedIn())
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestUnlockCompetitiveQueue Client Not Logged into Mothership", null);
			return;
		}
		if (this.UnlockCompetitiveQueueInProgress)
		{
			GTDev.LogWarning<string>("GorillaTagCompetitiveServerApi RequestUnlockCompetitiveQueue already in progress", null);
			return;
		}
		string platform = "PC";
		this.UnlockCompetitiveQueueInProgress = true;
		base.StartCoroutine(this.UnlockCompetitiveQueue(new GorillaTagCompetitiveServerApi.RankedModeUnlockCompetitiveQueueRequestData
		{
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId,
			platform = platform,
			unlocked = unlocked
		}, callback));
	}

	// Token: 0x06003458 RID: 13400 RVA: 0x00119081 File Offset: 0x00117281
	private IEnumerator UnlockCompetitiveQueue(GorillaTagCompetitiveServerApi.RankedModeUnlockCompetitiveQueueRequestData data, Action callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.MmrApiBaseUrl + "/api/UnlockCompetitiveQueue", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			GTDev.Log<string>("UnlockCompetitiveQueue Success: raw response: " + request.downloadHandler.text, null);
			this.OnCompleteUnlockCompetitiveQueue(request.downloadHandler.text, callback);
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
					goto IL_156;
				}
			}
			else if (responseCode != 408L && responseCode != 429L)
			{
				goto IL_156;
			}
			bool flag = true;
			goto IL_159;
			IL_156:
			flag = false;
			IL_159:
			if (flag)
			{
				retry = true;
			}
			else
			{
				this.OnCompleteUnlockCompetitiveQueue(request.downloadHandler.text, callback);
			}
		}
		if (retry)
		{
			if (this.UnlockCompetitiveQueueRetryCount < this.MAX_SERVER_RETRIES)
			{
				float num = Random.Range(0.5f, Mathf.Pow(2f, (float)(this.UnlockCompetitiveQueueRetryCount + 1)));
				this.ValidateMatchJoinRetryCount++;
				yield return new WaitForSeconds(num);
				this.UnlockCompetitiveQueueInProgress = false;
				this.RequestUnlockCompetitiveQueue(data.unlocked, callback);
			}
			else
			{
				this.UnlockCompetitiveQueueRetryCount = 0;
				this.OnCompleteUnlockCompetitiveQueue(null, callback);
			}
		}
		yield break;
	}

	// Token: 0x06003459 RID: 13401 RVA: 0x0011909E File Offset: 0x0011729E
	private void OnCompleteUnlockCompetitiveQueue([CanBeNull] string response, Action callback)
	{
		GTDev.Log<string>("UnlockCompetitiveQueue complete", null);
		this.UnlockCompetitiveQueueInProgress = false;
		this.UnlockCompetitiveQueueRetryCount = 0;
		if (response != null && callback != null)
		{
			callback.Invoke();
		}
	}

	// Token: 0x04004294 RID: 17044
	public static GorillaTagCompetitiveServerApi Instance;

	// Token: 0x04004295 RID: 17045
	public int MAX_SERVER_RETRIES = 3;

	// Token: 0x04004296 RID: 17046
	private bool GetRankInformationInProgress;

	// Token: 0x04004297 RID: 17047
	private int GetRankInformationRetryCount;

	// Token: 0x04004298 RID: 17048
	private bool CreateMatchIdInProgress;

	// Token: 0x04004299 RID: 17049
	private int CreateMatchIdRetryCount;

	// Token: 0x0400429A RID: 17050
	private bool ValidateMatchJoinInProgress;

	// Token: 0x0400429B RID: 17051
	private int ValidateMatchJoinRetryCount;

	// Token: 0x0400429C RID: 17052
	private bool SubmitMatchScoresInProgress;

	// Token: 0x0400429D RID: 17053
	private int SubmitMatchScoresRetryCount;

	// Token: 0x0400429E RID: 17054
	private bool SetEloValueInProgress;

	// Token: 0x0400429F RID: 17055
	private int SetEloValueRetryCount;

	// Token: 0x040042A0 RID: 17056
	private bool PingMatchInProgress;

	// Token: 0x040042A1 RID: 17057
	private int PingMatchRetryCount;

	// Token: 0x040042A2 RID: 17058
	private bool UnlockCompetitiveQueueInProgress;

	// Token: 0x040042A3 RID: 17059
	private int UnlockCompetitiveQueueRetryCount;

	// Token: 0x020007C2 RID: 1986
	public enum EPlatformType
	{
		// Token: 0x040042A5 RID: 17061
		PC,
		// Token: 0x040042A6 RID: 17062
		Quest,
		// Token: 0x040042A7 RID: 17063
		NumPlatforms
	}

	// Token: 0x020007C3 RID: 1987
	[Serializable]
	public class RankedModeRequestDataBase
	{
		// Token: 0x040042A8 RID: 17064
		public string mothershipId;

		// Token: 0x040042A9 RID: 17065
		public string mothershipToken;

		// Token: 0x040042AA RID: 17066
		public string mothershipEnvId;
	}

	// Token: 0x020007C4 RID: 1988
	[Serializable]
	public class RankedModeRequestDataPlatformed : GorillaTagCompetitiveServerApi.RankedModeRequestDataBase
	{
		// Token: 0x040042AB RID: 17067
		public string platform;
	}

	// Token: 0x020007C5 RID: 1989
	[Serializable]
	public class RankedModeProgressionRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040042AC RID: 17068
		public List<string> playfabIds;
	}

	// Token: 0x020007C6 RID: 1990
	[Serializable]
	public class RankedModeProgressionPlatformData
	{
		// Token: 0x040042AD RID: 17069
		public string platform;

		// Token: 0x040042AE RID: 17070
		public float elo;

		// Token: 0x040042AF RID: 17071
		public int majorTier;

		// Token: 0x040042B0 RID: 17072
		public int minorTier;

		// Token: 0x040042B1 RID: 17073
		public float rankProgress;
	}

	// Token: 0x020007C7 RID: 1991
	[Serializable]
	public class RankedModePlayerProgressionData
	{
		// Token: 0x040042B2 RID: 17074
		public string playfabID;

		// Token: 0x040042B3 RID: 17075
		public GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData[] platformData = new GorillaTagCompetitiveServerApi.RankedModeProgressionPlatformData[2];
	}

	// Token: 0x020007C8 RID: 1992
	[Serializable]
	public class RankedModeProgressionData
	{
		// Token: 0x040042B4 RID: 17076
		public List<GorillaTagCompetitiveServerApi.RankedModePlayerProgressionData> playerData;
	}

	// Token: 0x020007C9 RID: 1993
	[Serializable]
	public class RankedModeRequestDataWithMatchId : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040042B5 RID: 17077
		public string matchId;
	}

	// Token: 0x020007CA RID: 1994
	[Serializable]
	public class RankedModeValidateMatchJoinResponseData
	{
		// Token: 0x040042B6 RID: 17078
		public bool validJoin;
	}

	// Token: 0x020007CB RID: 1995
	[Serializable]
	public class RankedModePlayerScore
	{
		// Token: 0x040042B7 RID: 17079
		public string playfabId;

		// Token: 0x040042B8 RID: 17080
		public float gameScore;
	}

	// Token: 0x020007CC RID: 1996
	[Serializable]
	public class RankedModeSubmitMatchScoresRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataBase
	{
		// Token: 0x040042B9 RID: 17081
		public string matchId;

		// Token: 0x040042BA RID: 17082
		public string playfabId;

		// Token: 0x040042BB RID: 17083
		public List<GorillaTagCompetitiveServerApi.RankedModePlayerScore> playerScores;
	}

	// Token: 0x020007CD RID: 1997
	[Serializable]
	public class RankedModeSetEloValueRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040042BC RID: 17084
		public float elo;
	}

	// Token: 0x020007CE RID: 1998
	[Serializable]
	public class RankedModeUnlockCompetitiveQueueRequestData : GorillaTagCompetitiveServerApi.RankedModeRequestDataPlatformed
	{
		// Token: 0x040042BD RID: 17085
		public bool unlocked;
	}
}
