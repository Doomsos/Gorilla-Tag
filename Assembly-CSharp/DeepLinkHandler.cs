using System;
using System.Collections;
using GorillaNetworking;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000504 RID: 1284
public class DeepLinkHandler : MonoBehaviour
{
	// Token: 0x060020DC RID: 8412 RVA: 0x000AE08A File Offset: 0x000AC28A
	public void Awake()
	{
		if (DeepLinkHandler.instance == null)
		{
			DeepLinkHandler.instance = this;
			return;
		}
		if (DeepLinkHandler.instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x000AE0BC File Offset: 0x000AC2BC
	public static void Initialize(GameObject parent)
	{
		if (DeepLinkHandler.instance == null && parent != null)
		{
			parent.AddComponent<DeepLinkHandler>();
		}
		if (DeepLinkHandler.instance == null)
		{
			return;
		}
		DeepLinkHandler.instance.RefreshLaunchDetails();
		if (DeepLinkHandler.instance.cachedLaunchDetails != null && DeepLinkHandler.instance.cachedLaunchDetails.LaunchType == 4)
		{
			DeepLinkHandler.instance.HandleDeepLink();
			return;
		}
		Object.Destroy(DeepLinkHandler.instance);
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000AE140 File Offset: 0x000AC340
	private void RefreshLaunchDetails()
	{
		if (Application.platform != 11)
		{
			GTDev.Log<string>("[DeepLinkHandler::RefreshLaunchDetails] Not on Android Platform!", null);
			return;
		}
		this.cachedLaunchDetails = ApplicationLifecycle.GetLaunchDetails();
		GTDev.Log<string>(string.Concat(new string[]
		{
			"[DeepLinkHandler::RefreshLaunchDetails] LaunchType: ",
			this.cachedLaunchDetails.LaunchType.ToString(),
			"\n[DeepLinkHandler::RefreshLaunchDetails] LaunchSource: ",
			this.cachedLaunchDetails.LaunchSource,
			"\n[DeepLinkHandler::RefreshLaunchDetails] DeepLinkMessage: ",
			this.cachedLaunchDetails.DeeplinkMessage
		}), null);
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x000AE1CB File Offset: 0x000AC3CB
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback.Invoke(request);
		yield break;
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x000AE1F0 File Offset: 0x000AC3F0
	private void HandleDeepLink()
	{
		GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Handling deep link...", null);
		if (this.cachedLaunchDetails.LaunchSource.Contains("7221491444554579"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Witchblood, processing...", null);
			string text = JsonUtility.ToJson(new DeepLinkHandler.CollabRequest
			{
				itemGUID = this.cachedLaunchDetails.DeeplinkMessage,
				launchSource = this.cachedLaunchDetails.LaunchSource,
				oculusUserID = PlayFabAuthenticator.instance.userID,
				playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text, "application/json", new Action<UnityWebRequest>(this.OnWitchbloodCollabResponse)));
			return;
		}
		if (this.cachedLaunchDetails.LaunchSource.Contains("1903584373052985"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Racoon Lagoon, processing...", null);
			string text2 = JsonUtility.ToJson(new DeepLinkHandler.CollabRequest
			{
				itemGUID = this.cachedLaunchDetails.DeeplinkMessage,
				launchSource = this.cachedLaunchDetails.LaunchSource,
				oculusUserID = PlayFabAuthenticator.instance.userID,
				playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text2, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text2, "application/json", new Action<UnityWebRequest>(this.OnRaccoonLagoonCollabResponse)));
			return;
		}
		GTDev.LogError<string>("[DeepLinkHandler::HandleDeepLink] App launched via DeepLink, but from an unknown app. App ID: " + this.cachedLaunchDetails.LaunchSource, null);
		Object.Destroy(this);
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000AE3FC File Offset: 0x000AC5FC
	private void OnWitchbloodCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != 1)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", 5))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.WitchbloodCollabCosmeticID, true, true, true));
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000AE48C File Offset: 0x000AC68C
	private void OnRaccoonLagoonCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != 1)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", 5))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.RaccoonLagoonCosmeticIDs, true, true, true));
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000AE51A File Offset: 0x000AC71A
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		GTDev.Log<string>("[DeepLinkHandler::CheckProcessExternalUnlock] Checking if we can process external cosmetic unlock...", null);
		while (!CosmeticsController.instance.allCosmeticsDict_isInitialized || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			yield return null;
		}
		GTDev.Log<string>("[DeepLinkHandler::CheckProcessExternalUnlock] Cosmetics initialized, proceeding to process external unlock...", null);
		foreach (string itemID in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(itemID, autoEquip, isLeftHand);
		}
		if (destroyOnFinish)
		{
			Object.Destroy(this);
		}
		yield break;
	}

	// Token: 0x04002B81 RID: 11137
	public static volatile DeepLinkHandler instance;

	// Token: 0x04002B82 RID: 11138
	private LaunchDetails cachedLaunchDetails;

	// Token: 0x04002B83 RID: 11139
	private const string WitchbloodAppID = "7221491444554579";

	// Token: 0x04002B84 RID: 11140
	private readonly string[] WitchbloodCollabCosmeticID = new string[]
	{
		"LMAKT."
	};

	// Token: 0x04002B85 RID: 11141
	private const string RaccoonLagoonAppID = "1903584373052985";

	// Token: 0x04002B86 RID: 11142
	private readonly string[] RaccoonLagoonCosmeticIDs = new string[]
	{
		"LMALI.",
		"LHAGS."
	};

	// Token: 0x04002B87 RID: 11143
	private const string HiddenPathCollabEndpoint = "/api/ConsumeItem";

	// Token: 0x02000505 RID: 1285
	[Serializable]
	private class CollabRequest
	{
		// Token: 0x04002B88 RID: 11144
		public string itemGUID;

		// Token: 0x04002B89 RID: 11145
		public string launchSource;

		// Token: 0x04002B8A RID: 11146
		public string oculusUserID;

		// Token: 0x04002B8B RID: 11147
		public string playFabID;

		// Token: 0x04002B8C RID: 11148
		public string playFabSessionTicket;

		// Token: 0x04002B8D RID: 11149
		public string mothershipId;

		// Token: 0x04002B8E RID: 11150
		public string mothershipToken;

		// Token: 0x04002B8F RID: 11151
		public string mothershipEnvId;
	}
}
