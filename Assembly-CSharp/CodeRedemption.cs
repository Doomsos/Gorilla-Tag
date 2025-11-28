using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000469 RID: 1129
public class CodeRedemption : MonoBehaviour
{
	// Token: 0x06001C97 RID: 7319 RVA: 0x000978B2 File Offset: 0x00095AB2
	public void Awake()
	{
		if (CodeRedemption.Instance == null)
		{
			CodeRedemption.Instance = this;
			return;
		}
		if (CodeRedemption.Instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06001C98 RID: 7320 RVA: 0x000978E4 File Offset: 0x00095AE4
	public void HandleCodeRedemption(string code)
	{
		string text = JsonUtility.ToJson(new CodeRedemption.CodeRedemptionRequest
		{
			itemGUID = code,
			playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId
		});
		Debug.Log("[CodeRedemption] Web Request body: \n" + text);
		base.StartCoroutine(CodeRedemption.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeCodeItem", text, "application/json", new Action<UnityWebRequest>(this.OnCodeRedemptionResponse)));
	}

	// Token: 0x06001C99 RID: 7321 RVA: 0x00097988 File Offset: 0x00095B88
	private void OnCodeRedemptionResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != 1)
		{
			Debug.LogError("[CodeRedemption] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text);
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		string text = string.Empty;
		try
		{
			CodeRedemption.CodeRedemptionResponse codeRedemptionResponse = JsonUtility.FromJson<CodeRedemption.CodeRedemptionResponse>(completedRequest.downloadHandler.text);
			if (codeRedemptionResponse.result.Contains("AlreadyRedeemed", 5))
			{
				Debug.Log("[CodeRedemption] Item has already been redeemed!");
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.AlreadyUsed;
				return;
			}
			text = codeRedemptionResponse.playFabItemName;
		}
		catch (Exception ex)
		{
			string text2 = "[CodeRedemption] Error parsing JSON response: ";
			Exception ex2 = ex;
			Debug.LogError(text2 + ((ex2 != null) ? ex2.ToString() : null));
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		Debug.Log("[CodeRedemption] Item successfully granted, processing external unlock...");
		GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Success;
		GorillaComputer.instance.RedemptionCode = "";
		base.StartCoroutine(this.CheckProcessExternalUnlock(new string[]
		{
			text
		}, true, true, true));
	}

	// Token: 0x06001C9A RID: 7322 RVA: 0x00097A9C File Offset: 0x00095C9C
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		Debug.Log("[CodeRedemption] Checking if we can process external cosmetic unlock...");
		while (!CosmeticsController.instance.allCosmeticsDict_isInitialized || !CosmeticsV2Spawner_Dirty.allPartsInstantiated)
		{
			yield return null;
		}
		Debug.Log("[CodeRedemption] Cosmetics initialized, proceeding to process external unlock...");
		foreach (string itemID in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(itemID, autoEquip, isLeftHand);
		}
		yield break;
	}

	// Token: 0x06001C9B RID: 7323 RVA: 0x00097AB9 File Offset: 0x00095CB9
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback.Invoke(request);
		yield break;
	}

	// Token: 0x040026A2 RID: 9890
	public static volatile CodeRedemption Instance;

	// Token: 0x040026A3 RID: 9891
	private const string HiddenPathCollabEndpoint = "/api/ConsumeCodeItem";

	// Token: 0x0200046A RID: 1130
	[Serializable]
	private class CodeRedemptionRequest
	{
		// Token: 0x040026A4 RID: 9892
		public string itemGUID;

		// Token: 0x040026A5 RID: 9893
		public string playFabID;

		// Token: 0x040026A6 RID: 9894
		public string playFabSessionTicket;

		// Token: 0x040026A7 RID: 9895
		public string mothershipId;

		// Token: 0x040026A8 RID: 9896
		public string mothershipToken;

		// Token: 0x040026A9 RID: 9897
		public string mothershipEnvId;
	}

	// Token: 0x0200046B RID: 1131
	[Serializable]
	private class CodeRedemptionResponse
	{
		// Token: 0x040026AA RID: 9898
		public string result;

		// Token: 0x040026AB RID: 9899
		public string itemID;

		// Token: 0x040026AC RID: 9900
		public string playFabItemName;
	}
}
