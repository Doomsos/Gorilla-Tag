using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace GorillaNetworking
{
	// Token: 0x02000F2B RID: 3883
	public class TitleVotingExample : MonoBehaviour
	{
		// Token: 0x0600614B RID: 24907 RVA: 0x001F574C File Offset: 0x001F394C
		public void Start()
		{
			TitleVotingExample.<Start>d__8 <Start>d__;
			<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<TitleVotingExample.<Start>d__8>(ref <Start>d__);
		}

		// Token: 0x0600614C RID: 24908 RVA: 0x00002789 File Offset: 0x00000989
		public void Update()
		{
		}

		// Token: 0x0600614D RID: 24909 RVA: 0x001F5784 File Offset: 0x001F3984
		private Task WaitForSessionToken()
		{
			TitleVotingExample.<WaitForSessionToken>d__10 <WaitForSessionToken>d__;
			<WaitForSessionToken>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForSessionToken>d__.<>1__state = -1;
			<WaitForSessionToken>d__.<>t__builder.Start<TitleVotingExample.<WaitForSessionToken>d__10>(ref <WaitForSessionToken>d__);
			return <WaitForSessionToken>d__.<>t__builder.Task;
		}

		// Token: 0x0600614E RID: 24910 RVA: 0x001F57C0 File Offset: 0x001F39C0
		public void FetchPollsAndVote()
		{
			base.StartCoroutine(this.DoFetchPolls(new TitleVotingExample.FetchPollsRequest
			{
				TitleId = PlayFabAuthenticatorSettings.TitleId,
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				IncludeInactive = this.includeInactive
			}, new Action<List<TitleVotingExample.FetchPollsResponse>>(this.OnFetchPollsResponse)));
		}

		// Token: 0x0600614F RID: 24911 RVA: 0x001F5828 File Offset: 0x001F3A28
		private void GetNonceForVotingCallback([CanBeNull] Message<UserProof> message)
		{
			if (message != null)
			{
				UserProof data = message.Data;
				this.Nonce = ((data != null) ? data.ToString() : null);
			}
			base.StartCoroutine(this.DoVote(new TitleVotingExample.VoteRequest
			{
				PollId = this.PollId,
				TitleId = PlayFabAuthenticatorSettings.TitleId,
				PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				OculusId = PlayFabAuthenticator.instance.userID,
				UserPlatform = PlayFabAuthenticator.instance.platform.ToString(),
				UserNonce = this.Nonce,
				PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				OptionIndex = this.Option,
				IsPrediction = this.isPrediction
			}, new Action<TitleVotingExample.VoteResponse>(this.OnVoteSuccess)));
		}

		// Token: 0x06006150 RID: 24912 RVA: 0x001F58F6 File Offset: 0x001F3AF6
		public void Vote()
		{
			this.GetNonceForVotingCallback(null);
		}

		// Token: 0x06006151 RID: 24913 RVA: 0x001F58FF File Offset: 0x001F3AFF
		private IEnumerator DoFetchPolls(TitleVotingExample.FetchPollsRequest data, Action<List<TitleVotingExample.FetchPollsResponse>> callback)
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
				List<TitleVotingExample.FetchPollsResponse> list = JsonConvert.DeserializeObject<List<TitleVotingExample.FetchPollsResponse>>(request.downloadHandler.text);
				callback.Invoke(list);
			}
			else
			{
				Debug.LogError(string.Format("FetchPolls Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
				long responseCode = request.responseCode;
				if (responseCode >= 500L && responseCode < 600L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
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
					Debug.LogWarning(string.Format("Retrying Title Voting FetchPolls... Retry attempt #{0}, waiting for {1} seconds", this.fetchPollsRetryCount + 1, num));
					this.fetchPollsRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.FetchPollsAndVote();
				}
				else
				{
					Debug.LogError("Maximum FetchPolls retries attempted. Please check your network connection.");
					this.fetchPollsRetryCount = 0;
					callback.Invoke(null);
				}
			}
			yield break;
		}

		// Token: 0x06006152 RID: 24914 RVA: 0x001F591C File Offset: 0x001F3B1C
		private IEnumerator DoVote(TitleVotingExample.VoteRequest data, Action<TitleVotingExample.VoteResponse> callback)
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
				TitleVotingExample.VoteResponse voteResponse = JsonConvert.DeserializeObject<TitleVotingExample.VoteResponse>(request.downloadHandler.text);
				callback.Invoke(voteResponse);
			}
			else
			{
				Debug.LogError(string.Format("Vote Error: {0} -- raw response: ", request.responseCode) + request.downloadHandler.text);
				long responseCode = request.responseCode;
				if (responseCode >= 500L && responseCode < 600L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
				}
				else if (request.responseCode == 409L)
				{
					Debug.LogWarning("User already voted on this poll!");
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
					Debug.LogWarning(string.Format("Retrying Voting... Retry attempt #{0}, waiting for {1} seconds", this.voteRetryCount + 1, num));
					this.voteRetryCount++;
					yield return new WaitForSeconds((float)num);
					this.Vote();
				}
				else
				{
					Debug.LogError("Maximum Vote retries attempted. Please check your network connection.");
					this.voteRetryCount = 0;
					callback.Invoke(null);
				}
			}
			yield break;
		}

		// Token: 0x06006153 RID: 24915 RVA: 0x001F5939 File Offset: 0x001F3B39
		private void OnFetchPollsResponse([CanBeNull] List<TitleVotingExample.FetchPollsResponse> response)
		{
			if (response != null)
			{
				Debug.Log("Got polls: " + JsonConvert.SerializeObject(response));
				this.Vote();
				return;
			}
			Debug.LogError("Error: Could not fetch polls!");
		}

		// Token: 0x06006154 RID: 24916 RVA: 0x001F5964 File Offset: 0x001F3B64
		private void OnVoteSuccess([CanBeNull] TitleVotingExample.VoteResponse response)
		{
			if (response != null)
			{
				Debug.Log("Voted! " + JsonConvert.SerializeObject(response));
				return;
			}
			Debug.LogError("Error: Could not vote!");
		}

		// Token: 0x04007008 RID: 28680
		private string Nonce = "";

		// Token: 0x04007009 RID: 28681
		private int PollId = 5;

		// Token: 0x0400700A RID: 28682
		private bool includeInactive = true;

		// Token: 0x0400700B RID: 28683
		private int Option;

		// Token: 0x0400700C RID: 28684
		private bool isPrediction;

		// Token: 0x0400700D RID: 28685
		private int fetchPollsRetryCount;

		// Token: 0x0400700E RID: 28686
		private int voteRetryCount;

		// Token: 0x0400700F RID: 28687
		private int maxRetriesOnFail = 3;

		// Token: 0x02000F2C RID: 3884
		[Serializable]
		private class FetchPollsRequest
		{
			// Token: 0x04007010 RID: 28688
			public string TitleId;

			// Token: 0x04007011 RID: 28689
			public string PlayFabId;

			// Token: 0x04007012 RID: 28690
			public string PlayFabTicket;

			// Token: 0x04007013 RID: 28691
			public bool IncludeInactive;
		}

		// Token: 0x02000F2D RID: 3885
		[Serializable]
		private class FetchPollsResponse
		{
			// Token: 0x04007014 RID: 28692
			public int PollId;

			// Token: 0x04007015 RID: 28693
			public string Question;

			// Token: 0x04007016 RID: 28694
			public List<string> VoteOptions;

			// Token: 0x04007017 RID: 28695
			public List<int> VoteCount;

			// Token: 0x04007018 RID: 28696
			public List<int> PredictionCount;

			// Token: 0x04007019 RID: 28697
			public DateTime StartTime;

			// Token: 0x0400701A RID: 28698
			public DateTime EndTime;
		}

		// Token: 0x02000F2E RID: 3886
		[Serializable]
		private class VoteRequest
		{
			// Token: 0x0400701B RID: 28699
			public int PollId;

			// Token: 0x0400701C RID: 28700
			public string TitleId;

			// Token: 0x0400701D RID: 28701
			public string PlayFabId;

			// Token: 0x0400701E RID: 28702
			public string OculusId;

			// Token: 0x0400701F RID: 28703
			public string UserNonce;

			// Token: 0x04007020 RID: 28704
			public string UserPlatform;

			// Token: 0x04007021 RID: 28705
			public int OptionIndex;

			// Token: 0x04007022 RID: 28706
			public bool IsPrediction;

			// Token: 0x04007023 RID: 28707
			public string PlayFabTicket;
		}

		// Token: 0x02000F2F RID: 3887
		[Serializable]
		private class VoteResponse
		{
			// Token: 0x17000904 RID: 2308
			// (get) Token: 0x06006159 RID: 24921 RVA: 0x001F59B1 File Offset: 0x001F3BB1
			// (set) Token: 0x0600615A RID: 24922 RVA: 0x001F59B9 File Offset: 0x001F3BB9
			public int PollId { get; set; }

			// Token: 0x17000905 RID: 2309
			// (get) Token: 0x0600615B RID: 24923 RVA: 0x001F59C2 File Offset: 0x001F3BC2
			// (set) Token: 0x0600615C RID: 24924 RVA: 0x001F59CA File Offset: 0x001F3BCA
			public string TitleId { get; set; }

			// Token: 0x17000906 RID: 2310
			// (get) Token: 0x0600615D RID: 24925 RVA: 0x001F59D3 File Offset: 0x001F3BD3
			// (set) Token: 0x0600615E RID: 24926 RVA: 0x001F59DB File Offset: 0x001F3BDB
			public List<string> VoteOptions { get; set; }

			// Token: 0x17000907 RID: 2311
			// (get) Token: 0x0600615F RID: 24927 RVA: 0x001F59E4 File Offset: 0x001F3BE4
			// (set) Token: 0x06006160 RID: 24928 RVA: 0x001F59EC File Offset: 0x001F3BEC
			public List<int> VoteCount { get; set; }

			// Token: 0x17000908 RID: 2312
			// (get) Token: 0x06006161 RID: 24929 RVA: 0x001F59F5 File Offset: 0x001F3BF5
			// (set) Token: 0x06006162 RID: 24930 RVA: 0x001F59FD File Offset: 0x001F3BFD
			public List<int> PredictionCount { get; set; }
		}
	}
}
