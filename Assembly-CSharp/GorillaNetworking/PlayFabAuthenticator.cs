using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GorillaExtensions;
using JetBrains.Annotations;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GorillaNetworking
{
	// Token: 0x02000F13 RID: 3859
	public class PlayFabAuthenticator : MonoBehaviour
	{
		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x060060C3 RID: 24771 RVA: 0x001F3676 File Offset: 0x001F1876
		public GorillaComputer gorillaComputer
		{
			get
			{
				return GorillaComputer.instance;
			}
		}

		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x060060C4 RID: 24772 RVA: 0x001F367F File Offset: 0x001F187F
		// (set) Token: 0x060060C5 RID: 24773 RVA: 0x001F3687 File Offset: 0x001F1887
		public bool IsReturningPlayer { get; private set; }

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x060060C6 RID: 24774 RVA: 0x001F3690 File Offset: 0x001F1890
		// (set) Token: 0x060060C7 RID: 24775 RVA: 0x001F3698 File Offset: 0x001F1898
		public bool postAuthSetSafety { get; private set; }

		// Token: 0x060060C8 RID: 24776 RVA: 0x001F36A4 File Offset: 0x001F18A4
		private void Awake()
		{
			if (PlayFabAuthenticator.instance == null)
			{
				PlayFabAuthenticator.instance = this;
			}
			else if (PlayFabAuthenticator.instance != this)
			{
				Object.Destroy(base.gameObject);
			}
			if (PlayFabAuthenticator.instance.photonAuthenticator == null)
			{
				PlayFabAuthenticator.instance.photonAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<PhotonAuthenticator>();
			}
			this.platform = ScriptableObject.CreateInstance<PlatformTagJoin>();
			PlayFabSettings.CompressApiData = false;
			new byte[1];
			if (this.screenDebugMode)
			{
				this.debugText.text = "";
			}
			Debug.Log("doing steam thing");
			if (PlayFabAuthenticator.instance.steamAuthenticator == null)
			{
				PlayFabAuthenticator.instance.steamAuthenticator = PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
			}
			this.platform.PlatformTag = "Steam";
			PlayFabSettings.TitleId = PlayFabAuthenticatorSettings.TitleId;
			PlayFabSettings.DisableFocusTimeCollection = true;
			this.BeginLoginFlow();
		}

		// Token: 0x060060C9 RID: 24777 RVA: 0x001F37A8 File Offset: 0x001F19A8
		public void BeginLoginFlow()
		{
			if (!MothershipClientApiUnity.IsEnabled())
			{
				this.AuthenticateWithPlayFab();
				return;
			}
			if (PlayFabAuthenticator.instance.mothershipAuthenticator == null)
			{
				PlayFabAuthenticator.instance.mothershipAuthenticator = (MothershipAuthenticator.Instance ?? PlayFabAuthenticator.instance.gameObject.GetOrAddComponent<MothershipAuthenticator>());
				MothershipAuthenticator mothershipAuthenticator = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator.OnLoginSuccess = (Action)Delegate.Combine(mothershipAuthenticator.OnLoginSuccess, delegate()
				{
					PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
				});
				MothershipAuthenticator mothershipAuthenticator2 = PlayFabAuthenticator.instance.mothershipAuthenticator;
				mothershipAuthenticator2.OnLoginFailure = (Action<string>)Delegate.Combine(mothershipAuthenticator2.OnLoginFailure, delegate(string errorMessage)
				{
					this.loginFailed = true;
					this.ShowMothershipAuthErrorMessage(errorMessage);
				});
				PlayFabAuthenticator.instance.mothershipAuthenticator.BeginLoginFlow();
			}
		}

		// Token: 0x060060CA RID: 24778 RVA: 0x00002789 File Offset: 0x00000989
		private void Start()
		{
		}

		// Token: 0x060060CB RID: 24779 RVA: 0x001F3880 File Offset: 0x001F1A80
		private void OnEnable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse += new Action<Dictionary<string, object>>(this.OnCustomAuthenticationResponse);
		}

		// Token: 0x060060CC RID: 24780 RVA: 0x001F3898 File Offset: 0x001F1A98
		private void OnDisable()
		{
			NetworkSystem.Instance.OnCustomAuthenticationResponse -= new Action<Dictionary<string, object>>(this.OnCustomAuthenticationResponse);
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			SteamAuthTicket steamAuthTicket2 = this.steamAuthTicketForPlayFab;
			if (steamAuthTicket2 == null)
			{
				return;
			}
			steamAuthTicket2.Dispose();
		}

		// Token: 0x060060CD RID: 24781 RVA: 0x001F38D1 File Offset: 0x001F1AD1
		public void RefreshSteamAuthTicketForPhoton(Action<string> successCallback, Action<EResult> failureCallback)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			this.steamAuthTicketForPhoton = this.steamAuthenticator.GetAuthTicketForWebApi(this.steamAuthIdForPhoton, successCallback, failureCallback);
		}

		// Token: 0x060060CE RID: 24782 RVA: 0x001F3904 File Offset: 0x001F1B04
		private void OnCustomAuthenticationResponse(Dictionary<string, object> response)
		{
			SteamAuthTicket steamAuthTicket = this.steamAuthTicketForPhoton;
			if (steamAuthTicket != null)
			{
				steamAuthTicket.Dispose();
			}
			object obj;
			if (response.TryGetValue("SteamAuthIdForPhoton", ref obj))
			{
				string text = obj as string;
				if (text != null)
				{
					this.steamAuthIdForPhoton = text;
					return;
				}
			}
			this.steamAuthIdForPhoton = null;
		}

		// Token: 0x060060CF RID: 24783 RVA: 0x001F394C File Offset: 0x001F1B4C
		public void AuthenticateWithPlayFab()
		{
			Debug.Log("authenticating with playFab!");
			GorillaServer gorillaServer = GorillaServer.Instance;
			if (gorillaServer != null && gorillaServer.FeatureFlagsReady)
			{
				if (KIDManager.KidEnabled)
				{
					Debug.Log("[KID] Is Enabled - Enabling safeties by platform and age category");
					this.DefaultSafetiesByAgeCategory();
				}
			}
			else
			{
				this.postAuthSetSafety = true;
			}
			if (SteamManager.Initialized)
			{
				this.userID = SteamUser.GetSteamID().ToString();
				Debug.Log("trying to auth with steam");
				this.steamAuthTicketForPlayFab = this.steamAuthenticator.GetAuthTicket(delegate(string ticket)
				{
					Debug.Log("Got steam auth session ticket!");
					PlayFabClientAPI.LoginWithSteam(new LoginWithSteamRequest
					{
						CreateAccount = new bool?(true),
						SteamTicket = ticket
					}, new Action<LoginResult>(this.OnLoginWithSteamResponse), new Action<PlayFabError>(this.OnPlayFabError), null, null);
				}, delegate(EResult result)
				{
					base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
				});
				return;
			}
			base.StartCoroutine(this.DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame());
		}

		// Token: 0x060060D0 RID: 24784 RVA: 0x001F39FE File Offset: 0x001F1BFE
		private IEnumerator VerifyKidAuthenticated(DateTime accountCreationDateTime)
		{
			Task<DateTime?> getNewPlayerDateTimeTask = KIDManager.CheckKIDNewPlayerDateTime();
			yield return new WaitUntil(() => getNewPlayerDateTimeTask.IsCompleted);
			DateTime? result = getNewPlayerDateTimeTask.Result;
			if (result != null && KIDManager.KidEnabled)
			{
				this.IsReturningPlayer = (accountCreationDateTime < result);
			}
			yield break;
		}

		// Token: 0x060060D1 RID: 24785 RVA: 0x001F3A14 File Offset: 0x001F1C14
		private IEnumerator DisplayGeneralFailureMessageOnGorillaComputerAfter1Frame()
		{
			yield return null;
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.");
				this.gorillaComputer.screenText.Text = "UNABLE TO AUTHENTICATE YOUR STEAM ACCOUNT! PLEASE MAKE SURE STEAM IS RUNNING AND YOU ARE LAUNCHING THE GAME DIRECTLY FROM STEAM.";
				Debug.Log("Couldn't authenticate steam account");
			}
			else
			{
				Debug.LogError("PlayFabAuthenticator: gorillaComputer is null, so could not set GeneralFailureMessage notifying user that the steam account could not be authenticated.", this);
			}
			yield break;
		}

		// Token: 0x060060D2 RID: 24786 RVA: 0x001F3A24 File Offset: 0x001F1C24
		private void OnLoginWithSteamResponse(LoginResult obj)
		{
			this._playFabPlayerIdCache = obj.PlayFabId;
			this._sessionTicket = obj.SessionTicket;
			base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
			{
				Platform = this.platform.ToString(),
				SessionTicket = this._sessionTicket,
				PlayFabId = this._playFabPlayerIdCache,
				TitleId = PlayFabSettings.TitleId,
				MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
				MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
				MothershipToken = MothershipClientContext.Token,
				MothershipId = MothershipClientContext.MothershipId
			}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
		}

		// Token: 0x060060D3 RID: 24787 RVA: 0x001F3AC8 File Offset: 0x001F1CC8
		private void OnCachePlayFabIdRequest([CanBeNull] PlayFabAuthenticator.CachePlayFabIdResponse response)
		{
			if (response != null)
			{
				this.steamAuthIdForPhoton = response.SteamAuthIdForPhoton;
				DateTime accountCreationDateTime;
				if (DateTime.TryParse(response.AccountCreationIsoTimestamp, CultureInfo.InvariantCulture, 128, ref accountCreationDateTime))
				{
					base.StartCoroutine(this.VerifyKidAuthenticated(accountCreationDateTime));
				}
				Debug.Log("Successfully cached PlayFab Id.  Continuing!");
				this.AdvanceLogin();
				return;
			}
			Debug.LogError("Could not cache PlayFab Id.  Cannot continue.");
		}

		// Token: 0x060060D4 RID: 24788 RVA: 0x001F3B26 File Offset: 0x001F1D26
		private void AdvanceLogin()
		{
			this.LogMessage("PlayFab authenticated ... Getting Nonce");
			this.RefreshSteamAuthTicketForPhoton(delegate(string ticket)
			{
				this._nonce = ticket;
				Debug.Log("Got nonce!  Authenticating...");
				this.AuthenticateWithPhoton();
			}, delegate(EResult result)
			{
				Debug.LogWarning("Failed to get nonce!");
				this.AuthenticateWithPhoton();
			});
		}

		// Token: 0x060060D5 RID: 24789 RVA: 0x001F3B54 File Offset: 0x001F1D54
		private void AuthenticateWithPhoton()
		{
			PhotonAuthenticator photonAuthenticator = this.photonAuthenticator;
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("AppId", PlayFabSettings.TitleId);
			dictionary.Add("AppVersion", NetworkSystemConfig.AppVersion ?? "-1");
			dictionary.Add("Ticket", this._sessionTicket);
			dictionary.Add("Nonce", this._nonce);
			dictionary.Add("MothershipEnvId", MothershipClientApiUnity.EnvironmentId);
			dictionary.Add("MothershipDeploymentId", MothershipClientApiUnity.DeploymentId);
			dictionary.Add("MothershipToken", MothershipClientContext.Token);
			photonAuthenticator.SetCustomAuthenticationParameters(dictionary);
			this.GetPlayerDisplayName(this._playFabPlayerIdCache);
			GorillaServer.Instance.AddOrRemoveDLCOwnership(delegate(ExecuteFunctionResult result)
			{
				Debug.Log("got results! updating!");
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			}, delegate(PlayFabError error)
			{
				Debug.Log("Got error retrieving user data:");
				Debug.Log(error.GenerateErrorReport());
				if (GorillaTagger.Instance != null)
				{
					GorillaTagger.Instance.offlineVRRig.GetCosmeticsPlayFabCatalogData();
				}
			});
			if (CosmeticsController.instance != null)
			{
				Debug.Log("initializing cosmetics");
				CosmeticsController.instance.Initialize();
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.OnConnectedToMasterStuff();
			}
			else
			{
				base.StartCoroutine(this.ComputerOnConnectedToMaster());
			}
			if (RankedProgressionManager.Instance != null)
			{
				RankedProgressionManager.Instance.LoadStats();
			}
			if (PhotonNetworkController.Instance != null)
			{
				Debug.Log("Finish authenticating");
				NetworkSystem.Instance.FinishAuthenticating();
			}
		}

		// Token: 0x060060D6 RID: 24790 RVA: 0x001F3CC9 File Offset: 0x001F1EC9
		private IEnumerator ComputerOnConnectedToMaster()
		{
			WaitForEndOfFrame frameYield = new WaitForEndOfFrame();
			while (this.gorillaComputer == null)
			{
				yield return frameYield;
			}
			this.gorillaComputer.OnConnectedToMasterStuff();
			yield break;
		}

		// Token: 0x060060D7 RID: 24791 RVA: 0x001F3CD8 File Offset: 0x001F1ED8
		private void OnPlayFabError(PlayFabError obj)
		{
			this.LogMessage(obj.ErrorMessage);
			Debug.Log("OnPlayFabError(): " + obj.ErrorMessage);
			this.loginFailed = true;
			if (obj.ErrorMessage == "The account making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
					if (keyValuePair.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair.Key);
					return;
				}
			}
			if (obj.ErrorMessage == "The IP making this request is currently banned")
			{
				using (Dictionary<string, List<string>>.Enumerator enumerator = obj.ErrorDetails.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						return;
					}
					KeyValuePair<string, List<string>> keyValuePair2 = enumerator.Current;
					if (keyValuePair2.Value[0] != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + keyValuePair2.Key + "\nHOURS LEFT: " + ((int)((DateTime.Parse(keyValuePair2.Value[0]) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
						return;
					}
					this.gorillaComputer.GeneralFailureMessage("THIS IP HAS BEEN BANNED INDEFINITELY.\nREASON: " + keyValuePair2.Key);
					return;
				}
			}
			if (this.gorillaComputer != null)
			{
				this.gorillaComputer.GeneralFailureMessage(this.gorillaComputer.unableToConnect);
			}
		}

		// Token: 0x060060D8 RID: 24792 RVA: 0x00002789 File Offset: 0x00000989
		private void LogMessage(string message)
		{
		}

		// Token: 0x060060D9 RID: 24793 RVA: 0x001F3F10 File Offset: 0x001F2110
		private void GetPlayerDisplayName(string playFabId)
		{
			GetPlayerProfileRequest getPlayerProfileRequest = new GetPlayerProfileRequest();
			getPlayerProfileRequest.PlayFabId = playFabId;
			getPlayerProfileRequest.ProfileConstraints = new PlayerProfileViewConstraints
			{
				ShowDisplayName = true
			};
			PlayFabClientAPI.GetPlayerProfile(getPlayerProfileRequest, delegate(GetPlayerProfileResult result)
			{
				this._displayName = result.PlayerProfile.DisplayName;
			}, delegate(PlayFabError error)
			{
				Debug.LogError(error.GenerateErrorReport());
			}, null, null);
		}

		// Token: 0x060060DA RID: 24794 RVA: 0x001F3F70 File Offset: 0x001F2170
		public void SetDisplayName(string playerName)
		{
			if (this._displayName == null || (this._displayName.Length > 4 && this._displayName.Substring(0, this._displayName.Length - 4) != playerName))
			{
				UpdateUserTitleDisplayNameRequest updateUserTitleDisplayNameRequest = new UpdateUserTitleDisplayNameRequest();
				updateUserTitleDisplayNameRequest.DisplayName = playerName;
				PlayFabClientAPI.UpdateUserTitleDisplayName(updateUserTitleDisplayNameRequest, delegate(UpdateUserTitleDisplayNameResult result)
				{
					this._displayName = playerName;
				}, delegate(PlayFabError error)
				{
					Debug.LogError(error.GenerateErrorReport());
				}, null, null);
			}
		}

		// Token: 0x060060DB RID: 24795 RVA: 0x001F4010 File Offset: 0x001F2210
		public void ScreenDebug(string debugString)
		{
			Debug.Log(debugString);
			if (this.screenDebugMode)
			{
				Text text = this.debugText;
				text.text = text.text + debugString + "\n";
			}
		}

		// Token: 0x060060DC RID: 24796 RVA: 0x001F403C File Offset: 0x001F223C
		public void ScreenDebugClear()
		{
			this.debugText.text = "";
		}

		// Token: 0x060060DD RID: 24797 RVA: 0x001F404E File Offset: 0x001F224E
		public IEnumerator PlayfabAuthenticate(PlayFabAuthenticator.PlayfabAuthRequestData data, Action<PlayFabAuthenticator.PlayfabAuthResponseData> callback)
		{
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/PlayFabAuthentication", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 30;
			yield return request.SendWebRequest();
			if (request.result != 2 && request.result != 3)
			{
				PlayFabAuthenticator.PlayfabAuthResponseData playfabAuthResponseData = JsonUtility.FromJson<PlayFabAuthenticator.PlayfabAuthResponseData>(request.downloadHandler.text);
				callback.Invoke(playfabAuthResponseData);
			}
			else
			{
				if (request.responseCode == 403L)
				{
					Debug.LogError(string.Format("HTTP {0}: {1}, with body: {2}", request.responseCode, request.error, request.downloadHandler.text));
					PlayFabAuthenticator.BanInfo banInfo = JsonUtility.FromJson<PlayFabAuthenticator.BanInfo>(request.downloadHandler.text);
					this.ShowBanMessage(banInfo);
					callback.Invoke(null);
				}
				if (request.result == 3 && request.responseCode != 400L)
				{
					retry = true;
					Debug.LogError(string.Format("HTTP {0} error: {1} message:{2}", request.responseCode, request.error, request.downloadHandler.text));
				}
				else if (request.result == 2)
				{
					retry = true;
					Debug.LogError("NETWORK ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
				}
				else
				{
					Debug.LogError("HTTP ERROR: " + request.error + "\nMessage: " + request.downloadHandler.text);
					retry = true;
				}
			}
			if (retry)
			{
				if (this.playFabAuthRetryCount < this.playFabMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabAuthRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabAuthRetryCount + 1, num));
					this.playFabAuthRetryCount++;
					yield return new WaitForSeconds((float)num);
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback.Invoke(null);
					this.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
				}
			}
			yield break;
		}

		// Token: 0x060060DE RID: 24798 RVA: 0x001F406C File Offset: 0x001F226C
		private void ShowMothershipAuthErrorMessage(string errorMessage)
		{
			try
			{
				this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE WITH MOTHERSHIP.\nREASON: " + errorMessage);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failed to show Mothership auth error message: {0}", ex));
			}
		}

		// Token: 0x060060DF RID: 24799 RVA: 0x001F40B4 File Offset: 0x001F22B4
		private void ShowPlayFabAuthErrorMessage(string errorJson)
		{
			try
			{
				PlayFabAuthenticator.ErrorInfo errorInfo = JsonUtility.FromJson<PlayFabAuthenticator.ErrorInfo>(errorJson);
				this.gorillaComputer.GeneralFailureMessage("UNABLE TO AUTHENTICATE WITH PLAYFAB.\nREASON: " + errorInfo.Message);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failed to show PlayFab auth error message: {0}", ex));
			}
		}

		// Token: 0x060060E0 RID: 24800 RVA: 0x001F4108 File Offset: 0x001F2308
		private void ShowBanMessage(PlayFabAuthenticator.BanInfo banInfo)
		{
			try
			{
				if (banInfo.BanExpirationTime != null && banInfo.BanMessage != null)
				{
					if (banInfo.BanExpirationTime != "Indefinite")
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED. YOU WILL NOT BE ABLE TO PLAY UNTIL THE BAN EXPIRES.\nREASON: " + banInfo.BanMessage + "\nHOURS LEFT: " + ((int)((DateTime.Parse(banInfo.BanExpirationTime) - DateTime.UtcNow).TotalHours + 1.0)).ToString());
					}
					else
					{
						this.gorillaComputer.GeneralFailureMessage("YOUR ACCOUNT HAS BEEN BANNED INDEFINITELY.\nREASON: " + banInfo.BanMessage);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Format("Failed to show ban message: {0}", ex));
			}
		}

		// Token: 0x060060E1 RID: 24801 RVA: 0x001F41CC File Offset: 0x001F23CC
		public IEnumerator CachePlayFabId(PlayFabAuthenticator.CachePlayFabIdRequest data, Action<PlayFabAuthenticator.CachePlayFabIdResponse> callback)
		{
			Debug.Log("Trying to cache playfab Id");
			UnityWebRequest request = new UnityWebRequest(PlayFabAuthenticatorSettings.AuthApiBaseUrl + "/api/CachePlayFabId", "POST");
			byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
			bool retry = false;
			request.uploadHandler = new UploadHandlerRaw(bytes);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.timeout = 30;
			yield return request.SendWebRequest();
			if (request.result != 2 && request.result != 3)
			{
				if (request.responseCode == 200L)
				{
					PlayFabAuthenticator.CachePlayFabIdResponse cachePlayFabIdResponse = JsonUtility.FromJson<PlayFabAuthenticator.CachePlayFabIdResponse>(request.downloadHandler.text);
					callback.Invoke(cachePlayFabIdResponse);
				}
			}
			else if (request.result == 3 && request.responseCode != 400L)
			{
				retry = true;
				Debug.LogError(string.Format("HTTP {0} error: {1}", request.responseCode, request.error));
			}
			else
			{
				retry = (request.result != 2 || true);
			}
			if (retry)
			{
				if (this.playFabCacheRetryCount < this.playFabCacheMaxRetries)
				{
					int num = (int)Mathf.Pow(2f, (float)(this.playFabCacheRetryCount + 1));
					Debug.LogWarning(string.Format("Retrying PlayFab auth... Retry attempt #{0}, waiting for {1} seconds", this.playFabCacheRetryCount + 1, num));
					this.playFabCacheRetryCount++;
					yield return new WaitForSeconds((float)num);
					base.StartCoroutine(this.CachePlayFabId(new PlayFabAuthenticator.CachePlayFabIdRequest
					{
						Platform = this.platform.ToString(),
						SessionTicket = this._sessionTicket,
						PlayFabId = this._playFabPlayerIdCache,
						TitleId = PlayFabSettings.TitleId,
						MothershipEnvId = MothershipClientApiUnity.EnvironmentId,
						MothershipDeploymentId = MothershipClientApiUnity.DeploymentId,
						MothershipToken = MothershipClientContext.Token,
						MothershipId = MothershipClientContext.MothershipId
					}, new Action<PlayFabAuthenticator.CachePlayFabIdResponse>(this.OnCachePlayFabIdRequest)));
				}
				else
				{
					Debug.LogError("Maximum retries attempted. Please check your network connection.");
					callback.Invoke(null);
					this.ShowPlayFabAuthErrorMessage(request.downloadHandler.text);
				}
			}
			yield break;
		}

		// Token: 0x060060E2 RID: 24802 RVA: 0x001F41E9 File Offset: 0x001F23E9
		public void DefaultSafetiesByAgeCategory()
		{
			Debug.Log("[KID::PLAYFAB_AUTHENTICATOR] Defaulting Safety Settings to Disabled because age category data unavailable on this platform");
			this.SetSafety(false, true, false);
		}

		// Token: 0x060060E3 RID: 24803 RVA: 0x001F4200 File Offset: 0x001F2400
		public void SetSafety(bool isSafety, bool isAutoSet, bool setPlayfab = false)
		{
			this.postAuthSetSafety = false;
			Action<bool> onSafetyUpdate = this.OnSafetyUpdate;
			if (onSafetyUpdate != null)
			{
				onSafetyUpdate.Invoke(isSafety);
			}
			Debug.Log("[KID] Setting safety to: [" + isSafety.ToString() + "]");
			this.isSafeAccount = isSafety;
			this.safetyType = PlayFabAuthenticator.SafetyType.None;
			if (!isSafety)
			{
				if (isAutoSet)
				{
					PlayerPrefs.SetInt("autoSafety", 0);
				}
				else
				{
					PlayerPrefs.SetInt("optSafety", 0);
				}
				PlayerPrefs.Save();
				return;
			}
			if (isAutoSet)
			{
				PlayerPrefs.SetInt("autoSafety", 1);
				this.safetyType = PlayFabAuthenticator.SafetyType.Auto;
				return;
			}
			PlayerPrefs.SetInt("optSafety", 1);
			this.safetyType = PlayFabAuthenticator.SafetyType.OptIn;
		}

		// Token: 0x060060E4 RID: 24804 RVA: 0x001F429B File Offset: 0x001F249B
		public string GetPlayFabSessionTicket()
		{
			return this._sessionTicket;
		}

		// Token: 0x060060E5 RID: 24805 RVA: 0x001F42A3 File Offset: 0x001F24A3
		public string GetPlayFabPlayerId()
		{
			return this._playFabPlayerIdCache;
		}

		// Token: 0x060060E6 RID: 24806 RVA: 0x001F42AB File Offset: 0x001F24AB
		public bool GetSafety()
		{
			return this.isSafeAccount;
		}

		// Token: 0x060060E7 RID: 24807 RVA: 0x001F42B3 File Offset: 0x001F24B3
		public PlayFabAuthenticator.SafetyType GetSafetyType()
		{
			return this.safetyType;
		}

		// Token: 0x060060E8 RID: 24808 RVA: 0x001F42BB File Offset: 0x001F24BB
		public string GetUserID()
		{
			return this.userID;
		}

		// Token: 0x04006F87 RID: 28551
		public static volatile PlayFabAuthenticator instance;

		// Token: 0x04006F88 RID: 28552
		private const int PlayFabAuthRequestTimeout = 30;

		// Token: 0x04006F89 RID: 28553
		private string _playFabPlayerIdCache;

		// Token: 0x04006F8A RID: 28554
		private string _sessionTicket;

		// Token: 0x04006F8B RID: 28555
		private string _displayName;

		// Token: 0x04006F8C RID: 28556
		private string _nonce;

		// Token: 0x04006F8D RID: 28557
		public string userID;

		// Token: 0x04006F8E RID: 28558
		private string userToken;

		// Token: 0x04006F8F RID: 28559
		public PlatformTagJoin platform;

		// Token: 0x04006F90 RID: 28560
		private bool isSafeAccount;

		// Token: 0x04006F91 RID: 28561
		public Action<bool> OnSafetyUpdate;

		// Token: 0x04006F92 RID: 28562
		private PlayFabAuthenticator.SafetyType safetyType;

		// Token: 0x04006F93 RID: 28563
		private byte[] m_Ticket;

		// Token: 0x04006F94 RID: 28564
		private uint m_pcbTicket;

		// Token: 0x04006F95 RID: 28565
		public Text debugText;

		// Token: 0x04006F96 RID: 28566
		public bool screenDebugMode;

		// Token: 0x04006F97 RID: 28567
		public bool loginFailed;

		// Token: 0x04006F98 RID: 28568
		[FormerlySerializedAs("loginDisplayID")]
		public GameObject emptyObject;

		// Token: 0x04006F99 RID: 28569
		private int playFabAuthRetryCount;

		// Token: 0x04006F9A RID: 28570
		private int playFabMaxRetries = 5;

		// Token: 0x04006F9B RID: 28571
		private int playFabCacheRetryCount;

		// Token: 0x04006F9C RID: 28572
		private int playFabCacheMaxRetries = 5;

		// Token: 0x04006F9D RID: 28573
		public MetaAuthenticator metaAuthenticator;

		// Token: 0x04006F9E RID: 28574
		public SteamAuthenticator steamAuthenticator;

		// Token: 0x04006F9F RID: 28575
		public MothershipAuthenticator mothershipAuthenticator;

		// Token: 0x04006FA0 RID: 28576
		public PhotonAuthenticator photonAuthenticator;

		// Token: 0x04006FA1 RID: 28577
		[SerializeField]
		private bool dbg_isReturningPlayer;

		// Token: 0x04006FA3 RID: 28579
		private SteamAuthTicket steamAuthTicketForPlayFab;

		// Token: 0x04006FA4 RID: 28580
		private SteamAuthTicket steamAuthTicketForPhoton;

		// Token: 0x04006FA5 RID: 28581
		private string steamAuthIdForPhoton;

		// Token: 0x02000F14 RID: 3860
		public enum SafetyType
		{
			// Token: 0x04006FA8 RID: 28584
			None,
			// Token: 0x04006FA9 RID: 28585
			Auto,
			// Token: 0x04006FAA RID: 28586
			OptIn
		}

		// Token: 0x02000F15 RID: 3861
		[Serializable]
		public class CachePlayFabIdRequest
		{
			// Token: 0x04006FAB RID: 28587
			public string Platform;

			// Token: 0x04006FAC RID: 28588
			public string SessionTicket;

			// Token: 0x04006FAD RID: 28589
			public string PlayFabId;

			// Token: 0x04006FAE RID: 28590
			public string TitleId;

			// Token: 0x04006FAF RID: 28591
			public string MothershipEnvId;

			// Token: 0x04006FB0 RID: 28592
			public string MothershipDeploymentId;

			// Token: 0x04006FB1 RID: 28593
			public string MothershipToken;

			// Token: 0x04006FB2 RID: 28594
			public string MothershipId;
		}

		// Token: 0x02000F16 RID: 3862
		[Serializable]
		public class PlayfabAuthRequestData
		{
			// Token: 0x04006FB3 RID: 28595
			public string AppId;

			// Token: 0x04006FB4 RID: 28596
			public string Nonce;

			// Token: 0x04006FB5 RID: 28597
			public string OculusId;

			// Token: 0x04006FB6 RID: 28598
			public string Platform;

			// Token: 0x04006FB7 RID: 28599
			public string AgeCategory;

			// Token: 0x04006FB8 RID: 28600
			public string MothershipEnvId;

			// Token: 0x04006FB9 RID: 28601
			public string MothershipDeploymentId;

			// Token: 0x04006FBA RID: 28602
			public string MothershipToken;

			// Token: 0x04006FBB RID: 28603
			public string MothershipId;
		}

		// Token: 0x02000F17 RID: 3863
		[Serializable]
		public class PlayfabAuthResponseData
		{
			// Token: 0x04006FBC RID: 28604
			public string SessionTicket;

			// Token: 0x04006FBD RID: 28605
			public string EntityToken;

			// Token: 0x04006FBE RID: 28606
			public string PlayFabId;

			// Token: 0x04006FBF RID: 28607
			public string EntityId;

			// Token: 0x04006FC0 RID: 28608
			public string EntityType;

			// Token: 0x04006FC1 RID: 28609
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x02000F18 RID: 3864
		[Serializable]
		public class CachePlayFabIdResponse
		{
			// Token: 0x04006FC2 RID: 28610
			public string PlayFabId;

			// Token: 0x04006FC3 RID: 28611
			public string SteamAuthIdForPhoton;

			// Token: 0x04006FC4 RID: 28612
			public string AccountCreationIsoTimestamp;
		}

		// Token: 0x02000F19 RID: 3865
		private class ErrorInfo
		{
			// Token: 0x04006FC5 RID: 28613
			public string Message;

			// Token: 0x04006FC6 RID: 28614
			public string Error;
		}

		// Token: 0x02000F1A RID: 3866
		private class BanInfo
		{
			// Token: 0x04006FC7 RID: 28615
			public string BanMessage;

			// Token: 0x04006FC8 RID: 28616
			public string BanExpirationTime;
		}
	}
}
