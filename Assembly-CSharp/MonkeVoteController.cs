using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020001CC RID: 460
public class MonkeVoteController : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x17000129 RID: 297
	// (get) Token: 0x06000C78 RID: 3192 RVA: 0x00043BF2 File Offset: 0x00041DF2
	// (set) Token: 0x06000C79 RID: 3193 RVA: 0x00043BF9 File Offset: 0x00041DF9
	public static MonkeVoteController instance { get; private set; }

	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06000C7A RID: 3194 RVA: 0x00043C04 File Offset: 0x00041E04
	// (remove) Token: 0x06000C7B RID: 3195 RVA: 0x00043C3C File Offset: 0x00041E3C
	public event Action OnPollsUpdated;

	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06000C7C RID: 3196 RVA: 0x00043C74 File Offset: 0x00041E74
	// (remove) Token: 0x06000C7D RID: 3197 RVA: 0x00043CAC File Offset: 0x00041EAC
	public event Action OnVoteAccepted;

	// Token: 0x14000014 RID: 20
	// (add) Token: 0x06000C7E RID: 3198 RVA: 0x00043CE4 File Offset: 0x00041EE4
	// (remove) Token: 0x06000C7F RID: 3199 RVA: 0x00043D1C File Offset: 0x00041F1C
	public event Action OnVoteFailed;

	// Token: 0x14000015 RID: 21
	// (add) Token: 0x06000C80 RID: 3200 RVA: 0x00043D54 File Offset: 0x00041F54
	// (remove) Token: 0x06000C81 RID: 3201 RVA: 0x00043D8C File Offset: 0x00041F8C
	public event Action OnCurrentPollEnded;

	// Token: 0x06000C82 RID: 3202 RVA: 0x00043DC1 File Offset: 0x00041FC1
	public void Awake()
	{
		if (MonkeVoteController.instance == null)
		{
			MonkeVoteController.instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x00043DE0 File Offset: 0x00041FE0
	public void SliceUpdate()
	{
		if (this.isCurrentPollActive && !this.hasCurrentPollCompleted && this.currentPollCompletionTime < DateTime.UtcNow)
		{
			GTDev.Log<string>("Active vote poll completed.", null);
			this.hasCurrentPollCompleted = true;
			Action onCurrentPollEnded = this.OnCurrentPollEnded;
			if (onCurrentPollEnded == null)
			{
				return;
			}
			onCurrentPollEnded.Invoke();
		}
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x0001773D File Offset: 0x0001593D
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x00017746 File Offset: 0x00015946
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x00043E34 File Offset: 0x00042034
	public void RequestPolls()
	{
		MonkeVoteController.<RequestPolls>d__34 <RequestPolls>d__;
		<RequestPolls>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RequestPolls>d__.<>4__this = this;
		<RequestPolls>d__.<>1__state = -1;
		<RequestPolls>d__.<>t__builder.Start<MonkeVoteController.<RequestPolls>d__34>(ref <RequestPolls>d__);
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x00043E6C File Offset: 0x0004206C
	private Task WaitForSessionToken()
	{
		MonkeVoteController.<WaitForSessionToken>d__35 <WaitForSessionToken>d__;
		<WaitForSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<WaitForSessionToken>d__.<>1__state = -1;
		<WaitForSessionToken>d__.<>t__builder.Start<MonkeVoteController.<WaitForSessionToken>d__35>(ref <WaitForSessionToken>d__);
		return <WaitForSessionToken>d__.<>t__builder.Task;
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x00043EA8 File Offset: 0x000420A8
	private void FetchPolls()
	{
		base.StartCoroutine(this.DoFetchPolls(new MonkeVoteController.FetchPollsRequest
		{
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			IncludeInactive = this.includeInactive
		}, new Action<List<MonkeVoteController.FetchPollsResponse>>(this.OnFetchPollsResponse)));
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x00043F0E File Offset: 0x0004210E
	private IEnumerator DoFetchPolls(MonkeVoteController.FetchPollsRequest data, Action<List<MonkeVoteController.FetchPollsResponse>> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/FetchPoll", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			List<MonkeVoteController.FetchPollsResponse> list = JsonConvert.DeserializeObject<List<MonkeVoteController.FetchPollsResponse>>(request.downloadHandler.text);
			callback.Invoke(list);
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
			if (this.fetchPollsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.fetchPollsRetryCount + 1));
				this.fetchPollsRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.FetchPolls();
			}
			else
			{
				GTDev.LogError<string>("Maximum FetchPolls retries attempted. Please check your network connection.", null);
				this.fetchPollsRetryCount = 0;
				callback.Invoke(null);
			}
		}
		yield break;
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x00043F2C File Offset: 0x0004212C
	private void OnFetchPollsResponse([CanBeNull] List<MonkeVoteController.FetchPollsResponse> response)
	{
		this.isFetchingPoll = false;
		this.hasPoll = false;
		this.lastPollData = null;
		this.currentPollData = null;
		this.isCurrentPollActive = false;
		this.hasCurrentPollCompleted = false;
		if (response != null)
		{
			DateTime minValue = DateTime.MinValue;
			using (List<MonkeVoteController.FetchPollsResponse>.Enumerator enumerator = response.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MonkeVoteController.FetchPollsResponse fetchPollsResponse = enumerator.Current;
					if (fetchPollsResponse.isActive)
					{
						this.hasPoll = true;
						this.currentPollData = fetchPollsResponse;
						if (this.currentPollData.EndTime > DateTime.UtcNow)
						{
							this.isCurrentPollActive = true;
							this.hasCurrentPollCompleted = false;
							this.currentPollCompletionTime = this.currentPollData.EndTime;
							this.currentPollCompletionTime = this.currentPollCompletionTime.AddMinutes(1.0);
						}
					}
					if (!fetchPollsResponse.isActive && fetchPollsResponse.EndTime > minValue && fetchPollsResponse.EndTime < DateTime.UtcNow)
					{
						this.lastPollData = fetchPollsResponse;
					}
				}
				goto IL_106;
			}
		}
		GTDev.LogError<string>("Error: Could not fetch polls!", null);
		IL_106:
		Action onPollsUpdated = this.OnPollsUpdated;
		if (onPollsUpdated == null)
		{
			return;
		}
		onPollsUpdated.Invoke();
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00044060 File Offset: 0x00042260
	public void Vote(int pollId, int option, bool isPrediction)
	{
		if (!this.hasPoll)
		{
			return;
		}
		if (this.isSendingVote)
		{
			return;
		}
		this.isSendingVote = true;
		this.pollId = pollId;
		this.option = option;
		this.isPrediction = isPrediction;
		this.SendVote();
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x00044096 File Offset: 0x00042296
	private void SendVote()
	{
		this.GetNonceForVotingCallback(null);
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x000440A0 File Offset: 0x000422A0
	private void GetNonceForVotingCallback([CanBeNull] Message<UserProof> message)
	{
		if (message != null)
		{
			UserProof data = message.Data;
			this.Nonce = ((data != null) ? data.Value : null);
		}
		base.StartCoroutine(this.DoVote(new MonkeVoteController.VoteRequest
		{
			PollId = this.pollId,
			TitleId = PlayFabAuthenticatorSettings.TitleId,
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			OculusId = PlayFabAuthenticator.instance.userID,
			UserPlatform = PlayFabAuthenticator.instance.platform.ToString(),
			UserNonce = this.Nonce,
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			OptionIndex = this.option,
			IsPrediction = this.isPrediction
		}, new Action<MonkeVoteController.VoteResponse>(this.OnVoteSuccess)));
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0004416E File Offset: 0x0004236E
	private IEnumerator DoVote(MonkeVoteController.VoteRequest data, Action<MonkeVoteController.VoteResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.VotingApiBaseUrl + "/api/Vote", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
		bool retry = false;
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		if (request.result == 1)
		{
			MonkeVoteController.VoteResponse voteResponse = JsonConvert.DeserializeObject<MonkeVoteController.VoteResponse>(request.downloadHandler.text);
			callback.Invoke(voteResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				retry = true;
			}
			else if (request.responseCode == 429L)
			{
				GTDev.LogWarning<string>("User already voted on this poll!", null);
				callback.Invoke(null);
			}
			else if (request.result == 2)
			{
				retry = true;
			}
		}
		if (retry)
		{
			if (this.voteRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.voteRetryCount + 1));
				this.voteRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.SendVote();
			}
			else
			{
				GTDev.LogError<string>("Maximum Vote retries attempted. Please check your network connection.", null);
				this.voteRetryCount = 0;
				callback.Invoke(null);
			}
		}
		else
		{
			this.isSendingVote = false;
		}
		yield break;
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0004418B File Offset: 0x0004238B
	private void OnVoteSuccess([CanBeNull] MonkeVoteController.VoteResponse response)
	{
		this.isSendingVote = false;
		if (response != null)
		{
			this.lastVoteData = response;
			Action onVoteAccepted = this.OnVoteAccepted;
			if (onVoteAccepted == null)
			{
				return;
			}
			onVoteAccepted.Invoke();
			return;
		}
		else
		{
			Action onVoteFailed = this.OnVoteFailed;
			if (onVoteFailed == null)
			{
				return;
			}
			onVoteFailed.Invoke();
			return;
		}
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x000441BF File Offset: 0x000423BF
	public MonkeVoteController.FetchPollsResponse GetLastPollData()
	{
		return this.lastPollData;
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x000441C7 File Offset: 0x000423C7
	public MonkeVoteController.FetchPollsResponse GetCurrentPollData()
	{
		return this.currentPollData;
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x000441CF File Offset: 0x000423CF
	public MonkeVoteController.VoteResponse GetVoteData()
	{
		return this.lastVoteData;
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x000441D7 File Offset: 0x000423D7
	public int GetLastVotePollId()
	{
		return this.pollId;
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x000441DF File Offset: 0x000423DF
	public int GetLastVoteSelectedOption()
	{
		return this.option;
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x000441E7 File Offset: 0x000423E7
	public bool GetLastVoteWasPrediction()
	{
		return this.isPrediction;
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x000441EF File Offset: 0x000423EF
	public DateTime GetCurrentPollCompletionTime()
	{
		return this.currentPollCompletionTime;
	}

	// Token: 0x04000F66 RID: 3942
	private string Nonce = "";

	// Token: 0x04000F67 RID: 3943
	private bool includeInactive = true;

	// Token: 0x04000F68 RID: 3944
	private int fetchPollsRetryCount;

	// Token: 0x04000F69 RID: 3945
	private int maxRetriesOnFail = 3;

	// Token: 0x04000F6A RID: 3946
	private int voteRetryCount;

	// Token: 0x04000F6F RID: 3951
	private MonkeVoteController.FetchPollsResponse lastPollData;

	// Token: 0x04000F70 RID: 3952
	private MonkeVoteController.FetchPollsResponse currentPollData;

	// Token: 0x04000F71 RID: 3953
	private MonkeVoteController.VoteResponse lastVoteData;

	// Token: 0x04000F72 RID: 3954
	private bool isFetchingPoll;

	// Token: 0x04000F73 RID: 3955
	private bool hasPoll;

	// Token: 0x04000F74 RID: 3956
	private bool isCurrentPollActive;

	// Token: 0x04000F75 RID: 3957
	private bool hasCurrentPollCompleted;

	// Token: 0x04000F76 RID: 3958
	private DateTime currentPollCompletionTime;

	// Token: 0x04000F77 RID: 3959
	private bool isSendingVote;

	// Token: 0x04000F78 RID: 3960
	private int pollId = -1;

	// Token: 0x04000F79 RID: 3961
	private int option;

	// Token: 0x04000F7A RID: 3962
	private bool isPrediction;

	// Token: 0x020001CD RID: 461
	[Serializable]
	private class FetchPollsRequest
	{
		// Token: 0x04000F7B RID: 3963
		public string TitleId;

		// Token: 0x04000F7C RID: 3964
		public string PlayFabId;

		// Token: 0x04000F7D RID: 3965
		public string PlayFabTicket;

		// Token: 0x04000F7E RID: 3966
		public bool IncludeInactive;
	}

	// Token: 0x020001CE RID: 462
	[Serializable]
	public class FetchPollsResponse
	{
		// Token: 0x04000F7F RID: 3967
		public int PollId;

		// Token: 0x04000F80 RID: 3968
		public string Question;

		// Token: 0x04000F81 RID: 3969
		public List<string> VoteOptions;

		// Token: 0x04000F82 RID: 3970
		public List<int> VoteCount;

		// Token: 0x04000F83 RID: 3971
		public List<int> PredictionCount;

		// Token: 0x04000F84 RID: 3972
		public DateTime StartTime;

		// Token: 0x04000F85 RID: 3973
		public DateTime EndTime;

		// Token: 0x04000F86 RID: 3974
		public bool isActive;
	}

	// Token: 0x020001CF RID: 463
	[Serializable]
	private class VoteRequest
	{
		// Token: 0x04000F87 RID: 3975
		public int PollId;

		// Token: 0x04000F88 RID: 3976
		public string TitleId;

		// Token: 0x04000F89 RID: 3977
		public string PlayFabId;

		// Token: 0x04000F8A RID: 3978
		public string OculusId;

		// Token: 0x04000F8B RID: 3979
		public string UserNonce;

		// Token: 0x04000F8C RID: 3980
		public string UserPlatform;

		// Token: 0x04000F8D RID: 3981
		public int OptionIndex;

		// Token: 0x04000F8E RID: 3982
		public bool IsPrediction;

		// Token: 0x04000F8F RID: 3983
		public string PlayFabTicket;
	}

	// Token: 0x020001D0 RID: 464
	[Serializable]
	public class VoteResponse
	{
		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000C9B RID: 3227 RVA: 0x0004421F File Offset: 0x0004241F
		// (set) Token: 0x06000C9C RID: 3228 RVA: 0x00044227 File Offset: 0x00042427
		public int PollId { get; set; }

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x06000C9D RID: 3229 RVA: 0x00044230 File Offset: 0x00042430
		// (set) Token: 0x06000C9E RID: 3230 RVA: 0x00044238 File Offset: 0x00042438
		public string TitleId { get; set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x06000C9F RID: 3231 RVA: 0x00044241 File Offset: 0x00042441
		// (set) Token: 0x06000CA0 RID: 3232 RVA: 0x00044249 File Offset: 0x00042449
		public List<string> VoteOptions { get; set; }

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x06000CA1 RID: 3233 RVA: 0x00044252 File Offset: 0x00042452
		// (set) Token: 0x06000CA2 RID: 3234 RVA: 0x0004425A File Offset: 0x0004245A
		public List<int> VoteCount { get; set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x06000CA3 RID: 3235 RVA: 0x00044263 File Offset: 0x00042463
		// (set) Token: 0x06000CA4 RID: 3236 RVA: 0x0004426B File Offset: 0x0004246B
		public List<int> PredictionCount { get; set; }
	}
}
