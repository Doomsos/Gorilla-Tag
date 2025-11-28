using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020001E8 RID: 488
public class ProgressionController : MonoBehaviour
{
	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06000D61 RID: 3425 RVA: 0x00047470 File Offset: 0x00045670
	// (remove) Token: 0x06000D62 RID: 3426 RVA: 0x000474A4 File Offset: 0x000456A4
	public static event Action OnQuestSelectionChanged;

	// Token: 0x14000026 RID: 38
	// (add) Token: 0x06000D63 RID: 3427 RVA: 0x000474D8 File Offset: 0x000456D8
	// (remove) Token: 0x06000D64 RID: 3428 RVA: 0x0004750C File Offset: 0x0004570C
	public static event Action OnProgressEvent;

	// Token: 0x1700013E RID: 318
	// (get) Token: 0x06000D65 RID: 3429 RVA: 0x0004753F File Offset: 0x0004573F
	// (set) Token: 0x06000D66 RID: 3430 RVA: 0x00047546 File Offset: 0x00045746
	public static int WeeklyCap { get; private set; } = 25;

	// Token: 0x1700013F RID: 319
	// (get) Token: 0x06000D67 RID: 3431 RVA: 0x0004754E File Offset: 0x0004574E
	public static int TotalPoints
	{
		get
		{
			return ProgressionController._gInstance.totalPointsRaw - ProgressionController._gInstance.unclaimedPoints;
		}
	}

	// Token: 0x06000D68 RID: 3432 RVA: 0x00047565 File Offset: 0x00045765
	public static void ReportQuestChanged(bool initialLoad)
	{
		ProgressionController._gInstance.OnQuestProgressChanged(initialLoad);
	}

	// Token: 0x06000D69 RID: 3433 RVA: 0x00047572 File Offset: 0x00045772
	public static void ReportQuestSelectionChanged()
	{
		ProgressionController._gInstance.LoadCompletedQuestQueue();
		Action onQuestSelectionChanged = ProgressionController.OnQuestSelectionChanged;
		if (onQuestSelectionChanged == null)
		{
			return;
		}
		onQuestSelectionChanged.Invoke();
	}

	// Token: 0x06000D6A RID: 3434 RVA: 0x0004758D File Offset: 0x0004578D
	public static void ReportQuestComplete(int questId, bool isDaily)
	{
		ProgressionController._gInstance.OnQuestComplete(questId, isDaily);
	}

	// Token: 0x06000D6B RID: 3435 RVA: 0x0004759B File Offset: 0x0004579B
	public static void RedeemProgress()
	{
		ProgressionController._gInstance.RequestProgressRedemption(new Action(ProgressionController._gInstance.OnProgressRedeemed));
	}

	// Token: 0x06000D6C RID: 3436 RVA: 0x000475B7 File Offset: 0x000457B7
	[return: TupleElementNames(new string[]
	{
		"weekly",
		"unclaimed",
		"total"
	})]
	public static ValueTuple<int, int, int> GetProgressionData()
	{
		return ProgressionController._gInstance.GetProgress();
	}

	// Token: 0x06000D6D RID: 3437 RVA: 0x000475C3 File Offset: 0x000457C3
	public static void RequestProgressUpdate()
	{
		ProgressionController gInstance = ProgressionController._gInstance;
		if (gInstance == null)
		{
			return;
		}
		gInstance.ReportProgress();
	}

	// Token: 0x06000D6E RID: 3438 RVA: 0x000475D4 File Offset: 0x000457D4
	private void Awake()
	{
		if (ProgressionController._gInstance)
		{
			Debug.LogError("Duplicate ProgressionController detected. Destroying self.", base.gameObject);
			Object.Destroy(this);
			return;
		}
		ProgressionController._gInstance = this;
		this.unclaimedPoints = PlayerPrefs.GetInt("Claimed_Points_Key", 0);
		this.RequestStatus();
		this.LoadCompletedQuestQueue();
	}

	// Token: 0x06000D6F RID: 3439 RVA: 0x00047628 File Offset: 0x00045828
	private void RequestStatus()
	{
		ProgressionController.<RequestStatus>d__36 <RequestStatus>d__;
		<RequestStatus>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestStatus>d__.<>4__this = this;
		<RequestStatus>d__.<>1__state = -1;
		<RequestStatus>d__.<>t__builder.Start<ProgressionController.<RequestStatus>d__36>(ref <RequestStatus>d__);
	}

	// Token: 0x06000D70 RID: 3440 RVA: 0x00047660 File Offset: 0x00045860
	private Task WaitForSessionToken()
	{
		ProgressionController.<WaitForSessionToken>d__37 <WaitForSessionToken>d__;
		<WaitForSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForSessionToken>d__.<>1__state = -1;
		<WaitForSessionToken>d__.<>t__builder.Start<ProgressionController.<WaitForSessionToken>d__37>(ref <WaitForSessionToken>d__);
		return <WaitForSessionToken>d__.<>t__builder.Task;
	}

	// Token: 0x06000D71 RID: 3441 RVA: 0x0004769C File Offset: 0x0004589C
	private void FetchStatus()
	{
		base.StartCoroutine(this.DoFetchStatus(new ProgressionController.GetQuestsStatusRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = "",
			MothershipToken = ""
		}, new Action<ProgressionController.GetQuestStatusResponse>(this.OnFetchStatusResponse)));
	}

	// Token: 0x06000D72 RID: 3442 RVA: 0x00047701 File Offset: 0x00045901
	private IEnumerator DoFetchStatus(ProgressionController.GetQuestsStatusRequest data, Action<ProgressionController.GetQuestStatusResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl + "/api/GetQuestStatus", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionController.GetQuestStatusResponse getQuestStatusResponse = JsonConvert.DeserializeObject<ProgressionController.GetQuestStatusResponse>(request.downloadHandler.text);
			callback.Invoke(getQuestStatusResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.result == 2)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this._fetchStatusRetryCount < this._maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this._fetchStatusRetryCount + 1));
				this._fetchStatusRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.FetchStatus();
			}
			else
			{
				GTDev.LogError<string>("Maximum FetchStatus retries attempted. Please check your network connection.", null);
				this._fetchStatusRetryCount = 0;
				callback.Invoke(null);
			}
		}
		yield break;
	}

	// Token: 0x06000D73 RID: 3443 RVA: 0x00047720 File Offset: 0x00045920
	private void OnFetchStatusResponse([CanBeNull] ProgressionController.GetQuestStatusResponse response)
	{
		this._isFetchingStatus = false;
		this._statusReceived = false;
		if (response != null)
		{
			this.SetProgressionValues(response.result.GetWeeklyPoints(), this.unclaimedPoints, response.result.userPointsTotal);
			this.ReportProgress();
			return;
		}
		GTDev.LogError<string>("Error: Could not fetch status!", null);
	}

	// Token: 0x06000D74 RID: 3444 RVA: 0x00047772 File Offset: 0x00045972
	private void SendQuestCompleted(int questId)
	{
		if (this._isSendingQuestComplete)
		{
			return;
		}
		this._isSendingQuestComplete = true;
		this.StartSendQuestComplete(questId);
	}

	// Token: 0x06000D75 RID: 3445 RVA: 0x0004778C File Offset: 0x0004598C
	private void StartSendQuestComplete(int questId)
	{
		base.StartCoroutine(this.DoSendQuestComplete(new ProgressionController.SetQuestCompleteRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = "",
			MothershipToken = "",
			QuestId = questId,
			ClientVersion = MothershipClientApiUnity.DeploymentId
		}, new Action<ProgressionController.SetQuestCompleteResponse>(this.OnSendQuestCompleteSuccess)));
	}

	// Token: 0x06000D76 RID: 3446 RVA: 0x00047803 File Offset: 0x00045A03
	private IEnumerator DoSendQuestComplete(ProgressionController.SetQuestCompleteRequest data, Action<ProgressionController.SetQuestCompleteResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl + "/api/SetQuestComplete", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			ProgressionController.SetQuestCompleteResponse setQuestCompleteResponse = JsonConvert.DeserializeObject<ProgressionController.SetQuestCompleteResponse>(request.downloadHandler.text);
			callback.Invoke(setQuestCompleteResponse);
			this.ProcessQuestSubmittedSuccess();
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.responseCode == 403L)
			{
				GTDev.LogWarning<string>("User already reached the max number of completion points for this time period!", null);
				callback.Invoke(null);
				this.ClearQuestQueue();
			}
			else if (request.result == 2)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this._sendQuestCompleteRetryCount < this._maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this._sendQuestCompleteRetryCount + 1));
				this._sendQuestCompleteRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.StartSendQuestComplete(data.QuestId);
			}
			else
			{
				GTDev.LogError<string>("Maximum SendQuestComplete retries attempted. Please check your network connection.", null);
				this._sendQuestCompleteRetryCount = 0;
				callback.Invoke(null);
				this.ProcessQuestSubmittedFail();
			}
		}
		else
		{
			this._isSendingQuestComplete = false;
		}
		yield break;
	}

	// Token: 0x06000D77 RID: 3447 RVA: 0x00047820 File Offset: 0x00045A20
	private void OnSendQuestCompleteSuccess([CanBeNull] ProgressionController.SetQuestCompleteResponse response)
	{
		this._isSendingQuestComplete = false;
		if (response != null)
		{
			this.UpdateProgressionValues(response.result.GetWeeklyPoints(), response.result.userPointsTotal);
			this.ReportProgress();
		}
	}

	// Token: 0x06000D78 RID: 3448 RVA: 0x0004784E File Offset: 0x00045A4E
	private void OnQuestProgressChanged(bool initialLoad)
	{
		this.ReportProgress();
	}

	// Token: 0x06000D79 RID: 3449 RVA: 0x00047856 File Offset: 0x00045A56
	private void OnQuestComplete(int questId, bool isDaily)
	{
		this.QueueQuestCompletion(questId, isDaily);
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x00047860 File Offset: 0x00045A60
	private void QueueQuestCompletion(int questId, bool isDaily)
	{
		if (isDaily)
		{
			this._queuedDailyCompletedQuests.Add(questId);
		}
		else
		{
			this._queuedWeeklyCompletedQuests.Add(questId);
		}
		this.SaveCompletedQuestQueue();
		this.SubmitNextQuestInQueue();
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x0004788C File Offset: 0x00045A8C
	private void SubmitNextQuestInQueue()
	{
		if (this._currentlyProcessingQuest == -1 && this.AreCompletedQuestsQueued())
		{
			int num = -1;
			if (this._queuedWeeklyCompletedQuests.Count > 0)
			{
				num = this._queuedWeeklyCompletedQuests[0];
			}
			else if (this._queuedDailyCompletedQuests.Count > 0)
			{
				num = this._queuedDailyCompletedQuests[0];
			}
			this._currentlyProcessingQuest = num;
			this.SendQuestCompleted(num);
		}
	}

	// Token: 0x06000D7C RID: 3452 RVA: 0x000478F2 File Offset: 0x00045AF2
	private void ClearQuestQueue()
	{
		this._currentlyProcessingQuest = -1;
		this._queuedDailyCompletedQuests.Clear();
		this._queuedWeeklyCompletedQuests.Clear();
		this.SaveCompletedQuestQueue();
	}

	// Token: 0x06000D7D RID: 3453 RVA: 0x00047918 File Offset: 0x00045B18
	private void ProcessQuestSubmittedSuccess()
	{
		if (this._currentlyProcessingQuest != -1)
		{
			if (this.AreCompletedQuestsQueued())
			{
				if (this._queuedWeeklyCompletedQuests.Remove(this._currentlyProcessingQuest))
				{
					this.SaveCompletedQuestQueue();
				}
				else if (this._queuedDailyCompletedQuests.Remove(this._currentlyProcessingQuest))
				{
					this.SaveCompletedQuestQueue();
				}
			}
			this._currentlyProcessingQuest = -1;
			this.SubmitNextQuestInQueue();
		}
	}

	// Token: 0x06000D7E RID: 3454 RVA: 0x00047977 File Offset: 0x00045B77
	private void ProcessQuestSubmittedFail()
	{
		this._currentlyProcessingQuest = -1;
	}

	// Token: 0x06000D7F RID: 3455 RVA: 0x00047980 File Offset: 0x00045B80
	private bool AreCompletedQuestsQueued()
	{
		return this._queuedDailyCompletedQuests.Count > 0 || this._queuedWeeklyCompletedQuests.Count > 0;
	}

	// Token: 0x06000D80 RID: 3456 RVA: 0x000479A0 File Offset: 0x00045BA0
	private void SaveCompletedQuestQueue()
	{
		int num = 0;
		for (int i = 0; i < this._queuedDailyCompletedQuests.Count; i++)
		{
			PlayerPrefs.SetInt(string.Format("{0}{1}", "Queued_Quest_Daily_ID_Key", num), this._queuedDailyCompletedQuests[i]);
			num++;
		}
		int dailyQuestSetID = this._questManager.dailyQuestSetID;
		PlayerPrefs.SetInt("Queued_Quest_Daily_SetID_Key", dailyQuestSetID);
		PlayerPrefs.SetInt("Queued_Quest_Daily_SaveCount_Key", num);
		int num2 = 0;
		for (int j = 0; j < this._queuedWeeklyCompletedQuests.Count; j++)
		{
			PlayerPrefs.SetInt(string.Format("{0}{1}", "Queued_Quest_Weekly_ID_Key", num2), this._queuedWeeklyCompletedQuests[j]);
			num2++;
		}
		int weeklyQuestSetID = this._questManager.weeklyQuestSetID;
		PlayerPrefs.SetInt("Queued_Quest_Weekly_SetID_Key", weeklyQuestSetID);
		PlayerPrefs.SetInt("Queued_Quest_Weekly_SaveCount_Key", num2);
	}

	// Token: 0x06000D81 RID: 3457 RVA: 0x00047A80 File Offset: 0x00045C80
	private void LoadCompletedQuestQueue()
	{
		this._queuedDailyCompletedQuests.Clear();
		int @int = PlayerPrefs.GetInt("Queued_Quest_Daily_SetID_Key", -1);
		int int2 = PlayerPrefs.GetInt("Queued_Quest_Daily_SaveCount_Key", -1);
		int dailyQuestSetID = this._questManager.dailyQuestSetID;
		if (@int == dailyQuestSetID)
		{
			for (int i = 0; i < int2; i++)
			{
				int int3 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Queued_Quest_Daily_ID_Key", i), -1);
				if (int3 != -1)
				{
					this._queuedDailyCompletedQuests.Add(int3);
				}
			}
		}
		this._queuedWeeklyCompletedQuests.Clear();
		int int4 = PlayerPrefs.GetInt("Queued_Quest_Weekly_SetID_Key", -1);
		int int5 = PlayerPrefs.GetInt("Queued_Quest_Weekly_SaveCount_Key", -1);
		int weeklyQuestSetID = this._questManager.weeklyQuestSetID;
		if (int4 == weeklyQuestSetID)
		{
			for (int j = 0; j < int5; j++)
			{
				int int6 = PlayerPrefs.GetInt(string.Format("{0}{1}", "Queued_Quest_Weekly_ID_Key", j), -1);
				if (int6 != -1)
				{
					this._queuedWeeklyCompletedQuests.Add(int6);
				}
			}
		}
		this.SubmitNextQuestInQueue();
	}

	// Token: 0x06000D82 RID: 3458 RVA: 0x00047B78 File Offset: 0x00045D78
	private void RequestProgressRedemption(Action onComplete)
	{
		ProgressionController.<RequestProgressRedemption>d__66 <RequestProgressRedemption>d__;
		<RequestProgressRedemption>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestProgressRedemption>d__.onComplete = onComplete;
		<RequestProgressRedemption>d__.<>1__state = -1;
		<RequestProgressRedemption>d__.<>t__builder.Start<ProgressionController.<RequestProgressRedemption>d__66>(ref <RequestProgressRedemption>d__);
	}

	// Token: 0x06000D83 RID: 3459 RVA: 0x00047BAF File Offset: 0x00045DAF
	private void OnProgressRedeemed()
	{
		this.unclaimedPoints = 0;
		PlayerPrefs.SetInt("Claimed_Points_Key", this.unclaimedPoints);
		this.ReportProgress();
	}

	// Token: 0x06000D84 RID: 3460 RVA: 0x00047BD0 File Offset: 0x00045DD0
	private void AddPoints(int points)
	{
		if (this.weeklyPoints >= ProgressionController.WeeklyCap)
		{
			return;
		}
		int num = Mathf.Clamp(points, 0, ProgressionController.WeeklyCap - this.weeklyPoints);
		this.SetProgressionValues(this.weeklyPoints + num, this.unclaimedPoints + num, this.totalPointsRaw + num);
	}

	// Token: 0x06000D85 RID: 3461 RVA: 0x00047C20 File Offset: 0x00045E20
	private void UpdateProgressionValues(int weekly, int totalRaw)
	{
		int num = totalRaw - this.totalPointsRaw;
		this.unclaimedPoints += num;
		this.SetProgressionValues(weekly, this.unclaimedPoints, totalRaw);
	}

	// Token: 0x06000D86 RID: 3462 RVA: 0x00047C52 File Offset: 0x00045E52
	private void SetProgressionValues(int weekly, int unclaimed, int totalRaw)
	{
		this.weeklyPoints = weekly;
		this.unclaimedPoints = unclaimed;
		this.totalPointsRaw = totalRaw;
		this.ReportScoreChange();
		PlayerPrefs.SetInt("Claimed_Points_Key", unclaimed);
	}

	// Token: 0x06000D87 RID: 3463 RVA: 0x00047C7C File Offset: 0x00045E7C
	private void ReportProgress()
	{
		ProgressionController.<ReportProgress>d__71 <ReportProgress>d__;
		<ReportProgress>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<ReportProgress>d__.<>4__this = this;
		<ReportProgress>d__.<>1__state = -1;
		<ReportProgress>d__.<>t__builder.Start<ProgressionController.<ReportProgress>d__71>(ref <ReportProgress>d__);
	}

	// Token: 0x06000D88 RID: 3464 RVA: 0x00047CB4 File Offset: 0x00045EB4
	private void ReportScoreChange()
	{
		ValueTuple<int, int, int> valueTuple = new ValueTuple<int, int, int>(this.weeklyPoints, this.unclaimedPoints, this.totalPointsRaw);
		ValueTuple<int, int, int> lastProgressReport = this._lastProgressReport;
		ValueTuple<int, int, int> valueTuple2 = valueTuple;
		if (lastProgressReport.Item1 == valueTuple2.Item1 && lastProgressReport.Item2 == valueTuple2.Item2 && lastProgressReport.Item3 == valueTuple2.Item3)
		{
			return;
		}
		if (VRRig.LocalRig)
		{
			VRRig.LocalRig.SetQuestScore(ProgressionController.TotalPoints);
		}
		this._lastProgressReport = valueTuple;
	}

	// Token: 0x06000D89 RID: 3465 RVA: 0x00047D30 File Offset: 0x00045F30
	[return: TupleElementNames(new string[]
	{
		"weekly",
		"unclaimed",
		"total"
	})]
	private ValueTuple<int, int, int> GetProgress()
	{
		return new ValueTuple<int, int, int>(this.weeklyPoints, this.unclaimedPoints, this.totalPointsRaw - this.unclaimedPoints);
	}

	// Token: 0x06000D8C RID: 3468 RVA: 0x00047D85 File Offset: 0x00045F85
	[CompilerGenerated]
	private bool <RequestStatus>g__ShouldFetchStatus|36_0()
	{
		return !this._isFetchingStatus && !this._statusReceived;
	}

	// Token: 0x04001053 RID: 4179
	private static ProgressionController _gInstance;

	// Token: 0x04001056 RID: 4182
	[SerializeField]
	private RotatingQuestsManager _questManager;

	// Token: 0x04001057 RID: 4183
	private int weeklyPoints;

	// Token: 0x04001058 RID: 4184
	private int totalPointsRaw;

	// Token: 0x04001059 RID: 4185
	private int unclaimedPoints;

	// Token: 0x0400105A RID: 4186
	private bool _progressReportPending;

	// Token: 0x0400105B RID: 4187
	[TupleElementNames(new string[]
	{
		"weeklyPoints",
		"unclaimedPoints",
		"totalPointsRaw"
	})]
	private ValueTuple<int, int, int> _lastProgressReport;

	// Token: 0x0400105C RID: 4188
	private bool _isFetchingStatus;

	// Token: 0x0400105D RID: 4189
	private bool _statusReceived;

	// Token: 0x0400105E RID: 4190
	private bool _isSendingQuestComplete;

	// Token: 0x0400105F RID: 4191
	private int _fetchStatusRetryCount;

	// Token: 0x04001060 RID: 4192
	private int _sendQuestCompleteRetryCount;

	// Token: 0x04001061 RID: 4193
	private int _maxRetriesOnFail = 3;

	// Token: 0x04001062 RID: 4194
	private List<int> _queuedDailyCompletedQuests = new List<int>();

	// Token: 0x04001063 RID: 4195
	private List<int> _queuedWeeklyCompletedQuests = new List<int>();

	// Token: 0x04001064 RID: 4196
	private int _currentlyProcessingQuest = -1;

	// Token: 0x04001065 RID: 4197
	private const string kUnclaimedPointKey = "Claimed_Points_Key";

	// Token: 0x04001067 RID: 4199
	private const string kQueuedDailyQuestSetIDKey = "Queued_Quest_Daily_SetID_Key";

	// Token: 0x04001068 RID: 4200
	private const string kQueuedDailyQuestSaveCountKey = "Queued_Quest_Daily_SaveCount_Key";

	// Token: 0x04001069 RID: 4201
	private const string kQueuedDailyQuestIDKey = "Queued_Quest_Daily_ID_Key";

	// Token: 0x0400106A RID: 4202
	private const string kQueuedWeeklyQuestSetIDKey = "Queued_Quest_Weekly_SetID_Key";

	// Token: 0x0400106B RID: 4203
	private const string kQueuedWeeklyQuestSaveCountKey = "Queued_Quest_Weekly_SaveCount_Key";

	// Token: 0x0400106C RID: 4204
	private const string kQueuedWeeklyQuestIDKey = "Queued_Quest_Weekly_ID_Key";

	// Token: 0x020001E9 RID: 489
	[Serializable]
	private class GetQuestsStatusRequest
	{
		// Token: 0x0400106D RID: 4205
		public string PlayFabId;

		// Token: 0x0400106E RID: 4206
		public string PlayFabTicket;

		// Token: 0x0400106F RID: 4207
		public string MothershipId;

		// Token: 0x04001070 RID: 4208
		public string MothershipToken;
	}

	// Token: 0x020001EA RID: 490
	[Serializable]
	public class GetQuestStatusResponse
	{
		// Token: 0x04001071 RID: 4209
		public ProgressionController.UserQuestsStatus result;
	}

	// Token: 0x020001EB RID: 491
	public class UserQuestsStatus
	{
		// Token: 0x06000D8F RID: 3471 RVA: 0x00047D9C File Offset: 0x00045F9C
		public int GetWeeklyPoints()
		{
			int num = 0;
			if (this.dailyPoints != null)
			{
				foreach (KeyValuePair<string, int> keyValuePair in this.dailyPoints)
				{
					num += keyValuePair.Value;
				}
			}
			if (this.weeklyPoints != null)
			{
				foreach (KeyValuePair<int, int> keyValuePair2 in this.weeklyPoints)
				{
					num += keyValuePair2.Value;
				}
			}
			return Mathf.Min(num, ProgressionController.WeeklyCap);
		}

		// Token: 0x04001072 RID: 4210
		public Dictionary<string, int> dailyPoints;

		// Token: 0x04001073 RID: 4211
		public Dictionary<int, int> weeklyPoints;

		// Token: 0x04001074 RID: 4212
		public int userPointsTotal;
	}

	// Token: 0x020001EC RID: 492
	[Serializable]
	private class SetQuestCompleteRequest
	{
		// Token: 0x04001075 RID: 4213
		public string PlayFabId;

		// Token: 0x04001076 RID: 4214
		public string PlayFabTicket;

		// Token: 0x04001077 RID: 4215
		public string MothershipId;

		// Token: 0x04001078 RID: 4216
		public string MothershipToken;

		// Token: 0x04001079 RID: 4217
		public int QuestId;

		// Token: 0x0400107A RID: 4218
		public string ClientVersion;
	}

	// Token: 0x020001ED RID: 493
	[Serializable]
	public class SetQuestCompleteResponse
	{
		// Token: 0x0400107B RID: 4219
		public ProgressionController.UserQuestsStatus result;
	}
}
