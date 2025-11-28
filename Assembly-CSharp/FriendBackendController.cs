using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GorillaNetworking;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000B66 RID: 2918
public class FriendBackendController : MonoBehaviour
{
	// Token: 0x14000079 RID: 121
	// (add) Token: 0x060047D9 RID: 18393 RVA: 0x0017A694 File Offset: 0x00178894
	// (remove) Token: 0x060047DA RID: 18394 RVA: 0x0017A6CC File Offset: 0x001788CC
	public event Action<bool> OnGetFriendsComplete;

	// Token: 0x1400007A RID: 122
	// (add) Token: 0x060047DB RID: 18395 RVA: 0x0017A704 File Offset: 0x00178904
	// (remove) Token: 0x060047DC RID: 18396 RVA: 0x0017A73C File Offset: 0x0017893C
	public event Action<bool> OnSetPrivacyStateComplete;

	// Token: 0x1400007B RID: 123
	// (add) Token: 0x060047DD RID: 18397 RVA: 0x0017A774 File Offset: 0x00178974
	// (remove) Token: 0x060047DE RID: 18398 RVA: 0x0017A7AC File Offset: 0x001789AC
	public event Action<NetPlayer, bool> OnAddFriendComplete;

	// Token: 0x1400007C RID: 124
	// (add) Token: 0x060047DF RID: 18399 RVA: 0x0017A7E4 File Offset: 0x001789E4
	// (remove) Token: 0x060047E0 RID: 18400 RVA: 0x0017A81C File Offset: 0x00178A1C
	public event Action<FriendBackendController.Friend, bool> OnRemoveFriendComplete;

	// Token: 0x17000693 RID: 1683
	// (get) Token: 0x060047E1 RID: 18401 RVA: 0x0017A851 File Offset: 0x00178A51
	public List<FriendBackendController.Friend> FriendsList
	{
		get
		{
			return this.lastFriendsList;
		}
	}

	// Token: 0x17000694 RID: 1684
	// (get) Token: 0x060047E2 RID: 18402 RVA: 0x0017A859 File Offset: 0x00178A59
	public FriendBackendController.PrivacyState MyPrivacyState
	{
		get
		{
			return this.lastPrivacyState;
		}
	}

	// Token: 0x060047E3 RID: 18403 RVA: 0x0017A861 File Offset: 0x00178A61
	public void GetFriends()
	{
		if (!this.getFriendsInProgress)
		{
			this.getFriendsInProgress = true;
			this.GetFriendsInternal();
		}
	}

	// Token: 0x060047E4 RID: 18404 RVA: 0x0017A878 File Offset: 0x00178A78
	public void SetPrivacyState(FriendBackendController.PrivacyState state)
	{
		if (!this.setPrivacyStateInProgress)
		{
			this.setPrivacyStateInProgress = true;
			this.setPrivacyStateState = state;
			this.SetPrivacyStateInternal();
			return;
		}
		this.setPrivacyStateQueue.Enqueue(state);
	}

	// Token: 0x060047E5 RID: 18405 RVA: 0x0017A8A4 File Offset: 0x00178AA4
	public void AddFriend(NetPlayer target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.UserId.GetHashCode();
		if (!this.addFriendInProgress)
		{
			this.addFriendInProgress = true;
			this.addFriendTargetIdHash = hashCode;
			this.addFriendTargetPlayer = target;
			this.AddFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.addFriendRequestQueue.Contains(new ValueTuple<int, NetPlayer>(hashCode, target)))
		{
			this.addFriendRequestQueue.Enqueue(new ValueTuple<int, NetPlayer>(hashCode, target));
		}
	}

	// Token: 0x060047E6 RID: 18406 RVA: 0x0017A914 File Offset: 0x00178B14
	public void RemoveFriend(FriendBackendController.Friend target)
	{
		if (target == null)
		{
			return;
		}
		int hashCode = target.Presence.FriendLinkId.GetHashCode();
		if (!this.removeFriendInProgress)
		{
			this.removeFriendInProgress = true;
			this.removeFriendTargetIdHash = hashCode;
			this.removeFriendTarget = target;
			this.RemoveFriendInternal();
			return;
		}
		if (hashCode != this.addFriendTargetIdHash && !this.removeFriendRequestQueue.Contains(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target)))
		{
			this.removeFriendRequestQueue.Enqueue(new ValueTuple<int, FriendBackendController.Friend>(hashCode, target));
		}
	}

	// Token: 0x060047E7 RID: 18407 RVA: 0x0017A989 File Offset: 0x00178B89
	private void Awake()
	{
		if (FriendBackendController.Instance == null)
		{
			FriendBackendController.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060047E8 RID: 18408 RVA: 0x0017A9AC File Offset: 0x00178BAC
	private void GetFriendsInternal()
	{
		base.StartCoroutine(this.SendGetFriendsRequest(new FriendBackendController.GetFriendsRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipId = ""
		}, new Action<FriendBackendController.GetFriendsResponse>(this.GetFriendsComplete)));
	}

	// Token: 0x060047E9 RID: 18409 RVA: 0x0017AA06 File Offset: 0x00178C06
	private IEnumerator SendGetFriendsRequest(FriendBackendController.GetFriendsRequest data, Action<FriendBackendController.GetFriendsResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/GetFriendsV2", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == 1)
		{
			FriendBackendController.GetFriendsResponse getFriendsResponse = JsonConvert.DeserializeObject<FriendBackendController.GetFriendsResponse>(request.downloadHandler.text);
			callback.Invoke(getFriendsResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == 2)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.getFriendsRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.getFriendsRetryCount + 1));
				this.getFriendsRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.GetFriendsInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum GetFriends retries attempted. Please check your network connection.", null);
				this.getFriendsRetryCount = 0;
				callback.Invoke(null);
			}
		}
		else
		{
			this.getFriendsInProgress = false;
		}
		yield break;
	}

	// Token: 0x060047EA RID: 18410 RVA: 0x0017AA24 File Offset: 0x00178C24
	private void GetFriendsComplete([CanBeNull] FriendBackendController.GetFriendsResponse response)
	{
		this.getFriendsInProgress = false;
		if (response != null)
		{
			this.lastGetFriendsResponse = response;
			if (this.lastGetFriendsResponse.Result != null)
			{
				this.lastPrivacyState = this.lastGetFriendsResponse.Result.MyPrivacyState;
				if (this.lastGetFriendsResponse.Result.Friends != null)
				{
					this.lastFriendsList.Clear();
					foreach (FriendBackendController.Friend friend in this.lastGetFriendsResponse.Result.Friends)
					{
						this.lastFriendsList.Add(friend);
					}
				}
			}
			Action<bool> onGetFriendsComplete = this.OnGetFriendsComplete;
			if (onGetFriendsComplete == null)
			{
				return;
			}
			onGetFriendsComplete.Invoke(true);
			return;
		}
		else
		{
			Action<bool> onGetFriendsComplete2 = this.OnGetFriendsComplete;
			if (onGetFriendsComplete2 == null)
			{
				return;
			}
			onGetFriendsComplete2.Invoke(false);
			return;
		}
	}

	// Token: 0x060047EB RID: 18411 RVA: 0x0017AB00 File Offset: 0x00178D00
	public void CreateTestFriends()
	{
		Debug.Log("Adding test friends");
		for (int i = 0; i < 15; i++)
		{
			FriendBackendController.FriendPresence friendPresence = new FriendBackendController.FriendPresence();
			friendPresence.FriendLinkId = i.ToString();
			friendPresence.UserName = i.ToString();
			friendPresence.RoomId = i.ToString();
			friendPresence.Zone = "TreeHouse";
			friendPresence.Region = "Jungle";
			friendPresence.IsPublic = new bool?(true);
			FriendBackendController.Friend friend = new FriendBackendController.Friend();
			friend.Presence = friendPresence;
			friend.Created = DateTime.Now;
			this.FriendsList.Add(friend);
		}
	}

	// Token: 0x060047EC RID: 18412 RVA: 0x0017AB98 File Offset: 0x00178D98
	private void SetPrivacyStateInternal()
	{
		base.StartCoroutine(this.SendSetPrivacyStateRequest(new FriendBackendController.SetPrivacyStateRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			PrivacyState = this.setPrivacyStateState.ToString()
		}, new Action<FriendBackendController.SetPrivacyStateResponse>(this.SetPrivacyStateComplete)));
	}

	// Token: 0x060047ED RID: 18413 RVA: 0x0017ABFE File Offset: 0x00178DFE
	private IEnumerator SendSetPrivacyStateRequest(FriendBackendController.SetPrivacyStateRequest data, Action<FriendBackendController.SetPrivacyStateResponse> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/SetPrivacyState", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == 1)
		{
			FriendBackendController.SetPrivacyStateResponse setPrivacyStateResponse = JsonConvert.DeserializeObject<FriendBackendController.SetPrivacyStateResponse>(request.downloadHandler.text);
			callback.Invoke(setPrivacyStateResponse);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == 2)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.setPrivacyStateRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.setPrivacyStateRetryCount + 1));
				this.setPrivacyStateRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.SetPrivacyStateInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum SetPrivacyState retries attempted. Please check your network connection.", null);
				this.setPrivacyStateRetryCount = 0;
				callback.Invoke(null);
			}
		}
		else
		{
			this.setPrivacyStateInProgress = false;
		}
		yield break;
	}

	// Token: 0x060047EE RID: 18414 RVA: 0x0017AC1C File Offset: 0x00178E1C
	private void SetPrivacyStateComplete([CanBeNull] FriendBackendController.SetPrivacyStateResponse response)
	{
		this.setPrivacyStateInProgress = false;
		if (response != null)
		{
			this.lastPrivacyStateResponse = response;
			Action<bool> onSetPrivacyStateComplete = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete != null)
			{
				onSetPrivacyStateComplete.Invoke(true);
			}
		}
		else
		{
			Action<bool> onSetPrivacyStateComplete2 = this.OnSetPrivacyStateComplete;
			if (onSetPrivacyStateComplete2 != null)
			{
				onSetPrivacyStateComplete2.Invoke(false);
			}
		}
		if (this.setPrivacyStateQueue.Count > 0)
		{
			FriendBackendController.PrivacyState privacyState = this.setPrivacyStateQueue.Dequeue();
			this.SetPrivacyState(privacyState);
		}
	}

	// Token: 0x060047EF RID: 18415 RVA: 0x0017AC84 File Offset: 0x00178E84
	private void AddFriendInternal()
	{
		base.StartCoroutine(this.SendAddFriendRequest(new FriendBackendController.FriendRequestRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MothershipToken = "",
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.addFriendTargetPlayer.UserId
		}, new Action<bool>(this.AddFriendComplete)));
	}

	// Token: 0x060047F0 RID: 18416 RVA: 0x0017AD0F File Offset: 0x00178F0F
	private IEnumerator SendAddFriendRequest(FriendBackendController.FriendRequestRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RequestFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == 1)
		{
			callback.Invoke(true);
		}
		else
		{
			if (request.responseCode == 409L)
			{
				flag = false;
			}
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == 2)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.addFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.addFriendRetryCount + 1));
				this.addFriendRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.addFriendRetryCount = 0;
				callback.Invoke(false);
			}
		}
		else
		{
			this.addFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x060047F1 RID: 18417 RVA: 0x0017AD2C File Offset: 0x00178F2C
	private void AddFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<NetPlayer, bool> onAddFriendComplete = this.OnAddFriendComplete;
			if (onAddFriendComplete != null)
			{
				onAddFriendComplete.Invoke(this.addFriendTargetPlayer, true);
			}
		}
		else
		{
			Action<NetPlayer, bool> onAddFriendComplete2 = this.OnAddFriendComplete;
			if (onAddFriendComplete2 != null)
			{
				onAddFriendComplete2.Invoke(this.addFriendTargetPlayer, false);
			}
		}
		this.addFriendInProgress = false;
		this.addFriendTargetIdHash = 0;
		this.addFriendTargetPlayer = null;
		if (this.addFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, NetPlayer> valueTuple = this.addFriendRequestQueue.Dequeue();
			this.AddFriend(valueTuple.Item2);
		}
	}

	// Token: 0x060047F2 RID: 18418 RVA: 0x0017ADAC File Offset: 0x00178FAC
	private void RemoveFriendInternal()
	{
		base.StartCoroutine(this.SendRemoveFriendRequest(new FriendBackendController.RemoveFriendRequest
		{
			PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			MothershipId = "",
			PlayFabTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			MyFriendLinkId = NetworkSystem.Instance.LocalPlayer.UserId,
			FriendFriendLinkId = this.removeFriendTarget.Presence.FriendLinkId
		}, new Action<bool>(this.RemoveFriendComplete)));
	}

	// Token: 0x060047F3 RID: 18419 RVA: 0x0017AE31 File Offset: 0x00179031
	private IEnumerator SendRemoveFriendRequest(FriendBackendController.RemoveFriendRequest data, Action<bool> callback)
	{
		UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.FriendApiBaseUrl + "/api/RemoveFriend", "POST");
		byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
		request.uploadHandler = new UploadHandlerRaw(bytes);
		request.downloadHandler = new DownloadHandlerBuffer();
		request.SetRequestHeader("Content-Type", "application/json");
		yield return request.SendWebRequest();
		bool flag = false;
		if (request.result == 1)
		{
			callback.Invoke(true);
		}
		else
		{
			long responseCode = request.responseCode;
			if (responseCode >= 500L && responseCode < 600L)
			{
				flag = true;
			}
			else if (request.result == 2)
			{
				flag = true;
			}
		}
		if (flag)
		{
			if (this.removeFriendRetryCount < this.maxRetriesOnFail)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.removeFriendRetryCount + 1));
				this.removeFriendRetryCount++;
				yield return new WaitForSeconds((float)num);
				this.AddFriendInternal();
			}
			else
			{
				GTDev.LogError<string>("Maximum AddFriend retries attempted. Please check your network connection.", null);
				this.removeFriendRetryCount = 0;
				callback.Invoke(false);
			}
		}
		else
		{
			this.removeFriendInProgress = false;
		}
		yield break;
	}

	// Token: 0x060047F4 RID: 18420 RVA: 0x0017AE50 File Offset: 0x00179050
	private void RemoveFriendComplete([CanBeNull] bool success)
	{
		if (success)
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete != null)
			{
				onRemoveFriendComplete.Invoke(this.removeFriendTarget, true);
			}
		}
		else
		{
			Action<FriendBackendController.Friend, bool> onRemoveFriendComplete2 = this.OnRemoveFriendComplete;
			if (onRemoveFriendComplete2 != null)
			{
				onRemoveFriendComplete2.Invoke(this.removeFriendTarget, false);
			}
		}
		this.removeFriendInProgress = false;
		this.removeFriendTargetIdHash = 0;
		this.removeFriendTarget = null;
		if (this.removeFriendRequestQueue.Count > 0)
		{
			ValueTuple<int, FriendBackendController.Friend> valueTuple = this.removeFriendRequestQueue.Dequeue();
			this.RemoveFriend(valueTuple.Item2);
		}
	}

	// Token: 0x060047F5 RID: 18421 RVA: 0x0017AED0 File Offset: 0x001790D0
	private void LogNetPlayersInRoom()
	{
		Debug.Log("Local Player PlayfabId: " + PlayFabAuthenticator.instance.GetPlayFabPlayerId());
		int num = 0;
		foreach (NetPlayer netPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			Debug.Log(string.Format("[{0}] Player: {1}, ActorNumber: {2}, UserID: {3}, IsMasterClient: {4}", new object[]
			{
				num,
				netPlayer.NickName,
				netPlayer.ActorNumber,
				netPlayer.UserId,
				netPlayer.IsMasterClient
			}));
			num++;
		}
	}

	// Token: 0x060047F6 RID: 18422 RVA: 0x0017AF68 File Offset: 0x00179168
	private void TestAddFriend()
	{
		this.OnAddFriendComplete -= new Action<NetPlayer, bool>(this.TestAddFriendCompleteCallback);
		this.OnAddFriendComplete += new Action<NetPlayer, bool>(this.TestAddFriendCompleteCallback);
		NetPlayer target = null;
		if (this.netPlayerIndexToAddFriend >= 0 && this.netPlayerIndexToAddFriend < NetworkSystem.Instance.AllNetPlayers.Length)
		{
			target = NetworkSystem.Instance.AllNetPlayers[this.netPlayerIndexToAddFriend];
		}
		this.AddFriend(target);
	}

	// Token: 0x060047F7 RID: 18423 RVA: 0x0017AFD1 File Offset: 0x001791D1
	private void TestAddFriendCompleteCallback(NetPlayer player, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestAddFriendCompleteCallback returned with success = false");
	}

	// Token: 0x060047F8 RID: 18424 RVA: 0x0017AFEC File Offset: 0x001791EC
	private void TestRemoveFriend()
	{
		this.OnRemoveFriendComplete -= new Action<FriendBackendController.Friend, bool>(this.TestRemoveFriendCompleteCallback);
		this.OnRemoveFriendComplete += new Action<FriendBackendController.Friend, bool>(this.TestRemoveFriendCompleteCallback);
		FriendBackendController.Friend target = null;
		if (this.friendListIndexToRemoveFriend >= 0 && this.friendListIndexToRemoveFriend < this.FriendsList.Count)
		{
			target = this.FriendsList[this.friendListIndexToRemoveFriend];
		}
		this.RemoveFriend(target);
	}

	// Token: 0x060047F9 RID: 18425 RVA: 0x0017B054 File Offset: 0x00179254
	private void TestRemoveFriendCompleteCallback(FriendBackendController.Friend friend, bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = true");
			return;
		}
		Debug.Log("FriendBackend: TestRemoveFriendCompleteCallback returned with success = false");
	}

	// Token: 0x060047FA RID: 18426 RVA: 0x0017B06E File Offset: 0x0017926E
	private void TestGetFriends()
	{
		this.OnGetFriendsComplete -= new Action<bool>(this.TestGetFriendsCompleteCallback);
		this.OnGetFriendsComplete += new Action<bool>(this.TestGetFriendsCompleteCallback);
		this.GetFriends();
	}

	// Token: 0x060047FB RID: 18427 RVA: 0x0017B09C File Offset: 0x0017929C
	private void TestGetFriendsCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = true");
			if (this.FriendsList != null)
			{
				string text = string.Format("Friend Count: {0} Friends: \n", this.FriendsList.Count);
				for (int i = 0; i < this.FriendsList.Count; i++)
				{
					if (this.FriendsList[i] != null && this.FriendsList[i].Presence != null)
					{
						text = string.Concat(new string[]
						{
							text,
							this.FriendsList[i].Presence.UserName,
							", ",
							this.FriendsList[i].Presence.FriendLinkId,
							", ",
							this.FriendsList[i].Presence.RoomId,
							", ",
							this.FriendsList[i].Presence.Region,
							", ",
							this.FriendsList[i].Presence.Zone,
							"\n"
						});
					}
					else
					{
						text += "null friend\n";
					}
				}
				Debug.Log(text);
				return;
			}
		}
		else
		{
			Debug.Log("FriendBackend: TestGetFriendsCompleteCallback returned with success = false");
		}
	}

	// Token: 0x060047FC RID: 18428 RVA: 0x0017B1F9 File Offset: 0x001793F9
	private void TestSetPrivacyState()
	{
		this.OnSetPrivacyStateComplete -= new Action<bool>(this.TestSetPrivacyStateCompleteCallback);
		this.OnSetPrivacyStateComplete += new Action<bool>(this.TestSetPrivacyStateCompleteCallback);
		this.SetPrivacyState(this.privacyStateToSet);
	}

	// Token: 0x060047FD RID: 18429 RVA: 0x0017B22C File Offset: 0x0017942C
	private void TestSetPrivacyStateCompleteCallback(bool success)
	{
		if (success)
		{
			Debug.Log(string.Format("SetPrivacyState Success: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
			return;
		}
		Debug.Log(string.Format("SetPrivacyState Failed: Status: {0} Error: {1}", this.lastPrivacyStateResponse.StatusCode, this.lastPrivacyStateResponse.Error));
	}

	// Token: 0x0400589F RID: 22687
	[OnEnterPlay_SetNull]
	public static volatile FriendBackendController Instance;

	// Token: 0x040058A4 RID: 22692
	private int maxRetriesOnFail = 3;

	// Token: 0x040058A5 RID: 22693
	private int getFriendsRetryCount;

	// Token: 0x040058A6 RID: 22694
	private int setPrivacyStateRetryCount;

	// Token: 0x040058A7 RID: 22695
	private int addFriendRetryCount;

	// Token: 0x040058A8 RID: 22696
	private int removeFriendRetryCount;

	// Token: 0x040058A9 RID: 22697
	private bool getFriendsInProgress;

	// Token: 0x040058AA RID: 22698
	private FriendBackendController.GetFriendsResponse lastGetFriendsResponse;

	// Token: 0x040058AB RID: 22699
	private List<FriendBackendController.Friend> lastFriendsList = new List<FriendBackendController.Friend>();

	// Token: 0x040058AC RID: 22700
	private bool setPrivacyStateInProgress;

	// Token: 0x040058AD RID: 22701
	private FriendBackendController.PrivacyState setPrivacyStateState;

	// Token: 0x040058AE RID: 22702
	private FriendBackendController.SetPrivacyStateResponse lastPrivacyStateResponse;

	// Token: 0x040058AF RID: 22703
	private Queue<FriendBackendController.PrivacyState> setPrivacyStateQueue = new Queue<FriendBackendController.PrivacyState>();

	// Token: 0x040058B0 RID: 22704
	private FriendBackendController.PrivacyState lastPrivacyState;

	// Token: 0x040058B1 RID: 22705
	private bool addFriendInProgress;

	// Token: 0x040058B2 RID: 22706
	private int addFriendTargetIdHash;

	// Token: 0x040058B3 RID: 22707
	private NetPlayer addFriendTargetPlayer;

	// Token: 0x040058B4 RID: 22708
	private Queue<ValueTuple<int, NetPlayer>> addFriendRequestQueue = new Queue<ValueTuple<int, NetPlayer>>();

	// Token: 0x040058B5 RID: 22709
	private bool removeFriendInProgress;

	// Token: 0x040058B6 RID: 22710
	private int removeFriendTargetIdHash;

	// Token: 0x040058B7 RID: 22711
	private FriendBackendController.Friend removeFriendTarget;

	// Token: 0x040058B8 RID: 22712
	private Queue<ValueTuple<int, FriendBackendController.Friend>> removeFriendRequestQueue = new Queue<ValueTuple<int, FriendBackendController.Friend>>();

	// Token: 0x040058B9 RID: 22713
	[SerializeField]
	private int netPlayerIndexToAddFriend;

	// Token: 0x040058BA RID: 22714
	[SerializeField]
	private int friendListIndexToRemoveFriend;

	// Token: 0x040058BB RID: 22715
	[SerializeField]
	private FriendBackendController.PrivacyState privacyStateToSet;

	// Token: 0x02000B67 RID: 2919
	public class Friend
	{
		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x060047FF RID: 18431 RVA: 0x0017B2CC File Offset: 0x001794CC
		// (set) Token: 0x06004800 RID: 18432 RVA: 0x0017B2D4 File Offset: 0x001794D4
		public FriendBackendController.FriendPresence Presence { get; set; }

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06004801 RID: 18433 RVA: 0x0017B2DD File Offset: 0x001794DD
		// (set) Token: 0x06004802 RID: 18434 RVA: 0x0017B2E5 File Offset: 0x001794E5
		public DateTime Created { get; set; }
	}

	// Token: 0x02000B68 RID: 2920
	public class FriendPresence
	{
		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x06004804 RID: 18436 RVA: 0x0017B2EE File Offset: 0x001794EE
		// (set) Token: 0x06004805 RID: 18437 RVA: 0x0017B2F6 File Offset: 0x001794F6
		public string FriendLinkId { get; set; }

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x06004806 RID: 18438 RVA: 0x0017B2FF File Offset: 0x001794FF
		// (set) Token: 0x06004807 RID: 18439 RVA: 0x0017B307 File Offset: 0x00179507
		public string UserName { get; set; }

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06004808 RID: 18440 RVA: 0x0017B310 File Offset: 0x00179510
		// (set) Token: 0x06004809 RID: 18441 RVA: 0x0017B318 File Offset: 0x00179518
		public string RoomId { get; set; }

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x0600480A RID: 18442 RVA: 0x0017B321 File Offset: 0x00179521
		// (set) Token: 0x0600480B RID: 18443 RVA: 0x0017B329 File Offset: 0x00179529
		public string Zone { get; set; }

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x0600480C RID: 18444 RVA: 0x0017B332 File Offset: 0x00179532
		// (set) Token: 0x0600480D RID: 18445 RVA: 0x0017B33A File Offset: 0x0017953A
		public string Region { get; set; }

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x0600480E RID: 18446 RVA: 0x0017B343 File Offset: 0x00179543
		// (set) Token: 0x0600480F RID: 18447 RVA: 0x0017B34B File Offset: 0x0017954B
		public bool? IsPublic { get; set; }
	}

	// Token: 0x02000B69 RID: 2921
	public class FriendLink
	{
		// Token: 0x1700069D RID: 1693
		// (get) Token: 0x06004811 RID: 18449 RVA: 0x0017B354 File Offset: 0x00179554
		// (set) Token: 0x06004812 RID: 18450 RVA: 0x0017B35C File Offset: 0x0017955C
		public string my_playfab_id { get; set; }

		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06004813 RID: 18451 RVA: 0x0017B365 File Offset: 0x00179565
		// (set) Token: 0x06004814 RID: 18452 RVA: 0x0017B36D File Offset: 0x0017956D
		public string my_mothership_id { get; set; }

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06004815 RID: 18453 RVA: 0x0017B376 File Offset: 0x00179576
		// (set) Token: 0x06004816 RID: 18454 RVA: 0x0017B37E File Offset: 0x0017957E
		public string my_friendlink_id { get; set; }

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x06004817 RID: 18455 RVA: 0x0017B387 File Offset: 0x00179587
		// (set) Token: 0x06004818 RID: 18456 RVA: 0x0017B38F File Offset: 0x0017958F
		public string friend_playfab_id { get; set; }

		// Token: 0x170006A1 RID: 1697
		// (get) Token: 0x06004819 RID: 18457 RVA: 0x0017B398 File Offset: 0x00179598
		// (set) Token: 0x0600481A RID: 18458 RVA: 0x0017B3A0 File Offset: 0x001795A0
		public string friend_mothership_id { get; set; }

		// Token: 0x170006A2 RID: 1698
		// (get) Token: 0x0600481B RID: 18459 RVA: 0x0017B3A9 File Offset: 0x001795A9
		// (set) Token: 0x0600481C RID: 18460 RVA: 0x0017B3B1 File Offset: 0x001795B1
		public string friend_friendlink_id { get; set; }

		// Token: 0x170006A3 RID: 1699
		// (get) Token: 0x0600481D RID: 18461 RVA: 0x0017B3BA File Offset: 0x001795BA
		// (set) Token: 0x0600481E RID: 18462 RVA: 0x0017B3C2 File Offset: 0x001795C2
		public DateTime created { get; set; }
	}

	// Token: 0x02000B6A RID: 2922
	[NullableContext(2)]
	[Nullable(0)]
	public class FriendIdResponse
	{
		// Token: 0x170006A4 RID: 1700
		// (get) Token: 0x06004820 RID: 18464 RVA: 0x0017B3CB File Offset: 0x001795CB
		// (set) Token: 0x06004821 RID: 18465 RVA: 0x0017B3D3 File Offset: 0x001795D3
		public string PlayFabId { get; set; }

		// Token: 0x170006A5 RID: 1701
		// (get) Token: 0x06004822 RID: 18466 RVA: 0x0017B3DC File Offset: 0x001795DC
		// (set) Token: 0x06004823 RID: 18467 RVA: 0x0017B3E4 File Offset: 0x001795E4
		public string MothershipId { get; set; } = "";
	}

	// Token: 0x02000B6B RID: 2923
	public class FriendRequestRequest
	{
		// Token: 0x170006A6 RID: 1702
		// (get) Token: 0x06004825 RID: 18469 RVA: 0x0017B400 File Offset: 0x00179600
		// (set) Token: 0x06004826 RID: 18470 RVA: 0x0017B408 File Offset: 0x00179608
		public string PlayFabId { get; set; }

		// Token: 0x170006A7 RID: 1703
		// (get) Token: 0x06004827 RID: 18471 RVA: 0x0017B411 File Offset: 0x00179611
		// (set) Token: 0x06004828 RID: 18472 RVA: 0x0017B419 File Offset: 0x00179619
		public string MothershipId { get; set; } = "";

		// Token: 0x170006A8 RID: 1704
		// (get) Token: 0x06004829 RID: 18473 RVA: 0x0017B422 File Offset: 0x00179622
		// (set) Token: 0x0600482A RID: 18474 RVA: 0x0017B42A File Offset: 0x0017962A
		public string PlayFabTicket { get; set; }

		// Token: 0x170006A9 RID: 1705
		// (get) Token: 0x0600482B RID: 18475 RVA: 0x0017B433 File Offset: 0x00179633
		// (set) Token: 0x0600482C RID: 18476 RVA: 0x0017B43B File Offset: 0x0017963B
		public string MothershipToken { get; set; }

		// Token: 0x170006AA RID: 1706
		// (get) Token: 0x0600482D RID: 18477 RVA: 0x0017B444 File Offset: 0x00179644
		// (set) Token: 0x0600482E RID: 18478 RVA: 0x0017B44C File Offset: 0x0017964C
		public string MyFriendLinkId { get; set; }

		// Token: 0x170006AB RID: 1707
		// (get) Token: 0x0600482F RID: 18479 RVA: 0x0017B455 File Offset: 0x00179655
		// (set) Token: 0x06004830 RID: 18480 RVA: 0x0017B45D File Offset: 0x0017965D
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x02000B6C RID: 2924
	public class GetFriendsRequest
	{
		// Token: 0x170006AC RID: 1708
		// (get) Token: 0x06004832 RID: 18482 RVA: 0x0017B479 File Offset: 0x00179679
		// (set) Token: 0x06004833 RID: 18483 RVA: 0x0017B481 File Offset: 0x00179681
		public string PlayFabId { get; set; }

		// Token: 0x170006AD RID: 1709
		// (get) Token: 0x06004834 RID: 18484 RVA: 0x0017B48A File Offset: 0x0017968A
		// (set) Token: 0x06004835 RID: 18485 RVA: 0x0017B492 File Offset: 0x00179692
		public string MothershipId { get; set; } = "";

		// Token: 0x170006AE RID: 1710
		// (get) Token: 0x06004836 RID: 18486 RVA: 0x0017B49B File Offset: 0x0017969B
		// (set) Token: 0x06004837 RID: 18487 RVA: 0x0017B4A3 File Offset: 0x001796A3
		public string MothershipToken { get; set; }

		// Token: 0x170006AF RID: 1711
		// (get) Token: 0x06004838 RID: 18488 RVA: 0x0017B4AC File Offset: 0x001796AC
		// (set) Token: 0x06004839 RID: 18489 RVA: 0x0017B4B4 File Offset: 0x001796B4
		public string PlayFabTicket { get; set; }
	}

	// Token: 0x02000B6D RID: 2925
	public class GetFriendsResponse
	{
		// Token: 0x170006B0 RID: 1712
		// (get) Token: 0x0600483B RID: 18491 RVA: 0x0017B4D0 File Offset: 0x001796D0
		// (set) Token: 0x0600483C RID: 18492 RVA: 0x0017B4D8 File Offset: 0x001796D8
		[CanBeNull]
		public FriendBackendController.GetFriendsResult Result { get; set; }

		// Token: 0x170006B1 RID: 1713
		// (get) Token: 0x0600483D RID: 18493 RVA: 0x0017B4E1 File Offset: 0x001796E1
		// (set) Token: 0x0600483E RID: 18494 RVA: 0x0017B4E9 File Offset: 0x001796E9
		public int StatusCode { get; set; }

		// Token: 0x170006B2 RID: 1714
		// (get) Token: 0x0600483F RID: 18495 RVA: 0x0017B4F2 File Offset: 0x001796F2
		// (set) Token: 0x06004840 RID: 18496 RVA: 0x0017B4FA File Offset: 0x001796FA
		[Nullable(2)]
		public string Error { [NullableContext(2)] get; [NullableContext(2)] set; }
	}

	// Token: 0x02000B6E RID: 2926
	public class GetFriendsResult
	{
		// Token: 0x170006B3 RID: 1715
		// (get) Token: 0x06004842 RID: 18498 RVA: 0x0017B503 File Offset: 0x00179703
		// (set) Token: 0x06004843 RID: 18499 RVA: 0x0017B50B File Offset: 0x0017970B
		public List<FriendBackendController.Friend> Friends { get; set; }

		// Token: 0x170006B4 RID: 1716
		// (get) Token: 0x06004844 RID: 18500 RVA: 0x0017B514 File Offset: 0x00179714
		// (set) Token: 0x06004845 RID: 18501 RVA: 0x0017B51C File Offset: 0x0017971C
		public FriendBackendController.PrivacyState MyPrivacyState { get; set; }
	}

	// Token: 0x02000B6F RID: 2927
	public class SetPrivacyStateRequest
	{
		// Token: 0x170006B5 RID: 1717
		// (get) Token: 0x06004847 RID: 18503 RVA: 0x0017B525 File Offset: 0x00179725
		// (set) Token: 0x06004848 RID: 18504 RVA: 0x0017B52D File Offset: 0x0017972D
		public string PlayFabId { get; set; }

		// Token: 0x170006B6 RID: 1718
		// (get) Token: 0x06004849 RID: 18505 RVA: 0x0017B536 File Offset: 0x00179736
		// (set) Token: 0x0600484A RID: 18506 RVA: 0x0017B53E File Offset: 0x0017973E
		public string PlayFabTicket { get; set; }

		// Token: 0x170006B7 RID: 1719
		// (get) Token: 0x0600484B RID: 18507 RVA: 0x0017B547 File Offset: 0x00179747
		// (set) Token: 0x0600484C RID: 18508 RVA: 0x0017B54F File Offset: 0x0017974F
		public string PrivacyState { get; set; }
	}

	// Token: 0x02000B70 RID: 2928
	[NullableContext(2)]
	[Nullable(0)]
	public class SetPrivacyStateResponse
	{
		// Token: 0x170006B8 RID: 1720
		// (get) Token: 0x0600484E RID: 18510 RVA: 0x0017B558 File Offset: 0x00179758
		// (set) Token: 0x0600484F RID: 18511 RVA: 0x0017B560 File Offset: 0x00179760
		public int StatusCode { get; set; }

		// Token: 0x170006B9 RID: 1721
		// (get) Token: 0x06004850 RID: 18512 RVA: 0x0017B569 File Offset: 0x00179769
		// (set) Token: 0x06004851 RID: 18513 RVA: 0x0017B571 File Offset: 0x00179771
		public string Error { get; set; }
	}

	// Token: 0x02000B71 RID: 2929
	public class RemoveFriendRequest
	{
		// Token: 0x170006BA RID: 1722
		// (get) Token: 0x06004853 RID: 18515 RVA: 0x0017B57A File Offset: 0x0017977A
		// (set) Token: 0x06004854 RID: 18516 RVA: 0x0017B582 File Offset: 0x00179782
		public string PlayFabId { get; set; }

		// Token: 0x170006BB RID: 1723
		// (get) Token: 0x06004855 RID: 18517 RVA: 0x0017B58B File Offset: 0x0017978B
		// (set) Token: 0x06004856 RID: 18518 RVA: 0x0017B593 File Offset: 0x00179793
		public string MothershipId { get; set; } = "";

		// Token: 0x170006BC RID: 1724
		// (get) Token: 0x06004857 RID: 18519 RVA: 0x0017B59C File Offset: 0x0017979C
		// (set) Token: 0x06004858 RID: 18520 RVA: 0x0017B5A4 File Offset: 0x001797A4
		public string PlayFabTicket { get; set; }

		// Token: 0x170006BD RID: 1725
		// (get) Token: 0x06004859 RID: 18521 RVA: 0x0017B5AD File Offset: 0x001797AD
		// (set) Token: 0x0600485A RID: 18522 RVA: 0x0017B5B5 File Offset: 0x001797B5
		public string MothershipToken { get; set; }

		// Token: 0x170006BE RID: 1726
		// (get) Token: 0x0600485B RID: 18523 RVA: 0x0017B5BE File Offset: 0x001797BE
		// (set) Token: 0x0600485C RID: 18524 RVA: 0x0017B5C6 File Offset: 0x001797C6
		public string MyFriendLinkId { get; set; }

		// Token: 0x170006BF RID: 1727
		// (get) Token: 0x0600485D RID: 18525 RVA: 0x0017B5CF File Offset: 0x001797CF
		// (set) Token: 0x0600485E RID: 18526 RVA: 0x0017B5D7 File Offset: 0x001797D7
		public string FriendFriendLinkId { get; set; }
	}

	// Token: 0x02000B72 RID: 2930
	public enum PendingRequestStatus
	{
		// Token: 0x040058E8 RID: 22760
		I_REQUESTED,
		// Token: 0x040058E9 RID: 22761
		THEY_REQUESTED,
		// Token: 0x040058EA RID: 22762
		CONFIRMED,
		// Token: 0x040058EB RID: 22763
		NOT_FOUND
	}

	// Token: 0x02000B73 RID: 2931
	public enum PrivacyState
	{
		// Token: 0x040058ED RID: 22765
		VISIBLE,
		// Token: 0x040058EE RID: 22766
		PUBLIC_ONLY,
		// Token: 0x040058EF RID: 22767
		HIDDEN
	}
}
