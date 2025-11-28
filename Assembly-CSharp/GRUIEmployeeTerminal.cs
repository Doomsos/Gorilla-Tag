using System;
using System.Collections.Generic;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000741 RID: 1857
public class GRUIEmployeeTerminal : MonoBehaviour
{
	// Token: 0x06002FF3 RID: 12275 RVA: 0x0010608C File Offset: 0x0010428C
	public void Setup()
	{
		this.signupButton.onPressButton.AddListener(new UnityAction(this.OnSignup));
		GetUserDataRequest getUserDataRequest = new GetUserDataRequest();
		getUserDataRequest.PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		List<string> list = new List<string>();
		list.Add("GRData");
		getUserDataRequest.Keys = list;
		this.isSigningUp = true;
		PlayFabClientAPI.GetUserData(getUserDataRequest, new Action<GetUserDataResult>(this.OnGetUserDataInitialState), new Action<PlayFabError>(this.OnGetUserDataInitialStateFail), null, null);
		this.Refresh();
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x00106110 File Offset: 0x00104310
	public void OnSignup()
	{
		if (this.isSigningUp || this.isEmployee)
		{
			return;
		}
		UpdateUserDataRequest updateUserDataRequest = new UpdateUserDataRequest();
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("GRData", "Now we have data");
		updateUserDataRequest.Data = dictionary;
		UpdateUserDataRequest updateUserDataRequest2 = updateUserDataRequest;
		if (!PlayFabClientAPI.IsClientLoggedIn())
		{
			if (PlayFabAuthenticator.instance != null)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			return;
		}
		this.isSigningUp = true;
		PlayFabClientAPI.UpdateUserData(updateUserDataRequest2, new Action<UpdateUserDataResult>(this.OnSaveTableSuccess), new Action<PlayFabError>(this.OnSaveTableFailure), null, null);
	}

	// Token: 0x06002FF5 RID: 12277 RVA: 0x00106199 File Offset: 0x00104399
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x001061A4 File Offset: 0x001043A4
	public void Refresh()
	{
		if (this.isSigningUp)
		{
			this.signupButtonText.text = "APPLYING";
			return;
		}
		if (this.isEmployee)
		{
			this.signupButtonText.text = "HIRED";
			return;
		}
		this.signupButtonText.text = "APPLY";
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x001061F4 File Offset: 0x001043F4
	private void OnGetUserDataInitialState(GetUserDataResult result)
	{
		UserDataRecord userDataRecord;
		if (result.Data.TryGetValue("GRData", ref userDataRecord))
		{
			string value = userDataRecord.Value;
			this.isEmployee = true;
		}
		else
		{
			this.isEmployee = false;
		}
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x00106239 File Offset: 0x00104439
	private void OnGetUserDataInitialStateFail(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x0010624F File Offset: 0x0010444F
	private void OnSaveTableSuccess(UpdateUserDataResult result)
	{
		this.isEmployee = true;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x00106239 File Offset: 0x00104439
	private void OnSaveTableFailure(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x04003EE7 RID: 16103
	[SerializeField]
	private GorillaPressableButton signupButton;

	// Token: 0x04003EE8 RID: 16104
	[SerializeField]
	private TMP_Text signupButtonText;

	// Token: 0x04003EE9 RID: 16105
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x04003EEA RID: 16106
	[SerializeField]
	private GRUIStationEmployeeBadges badgeStation;

	// Token: 0x04003EEB RID: 16107
	private int entityTypeId;

	// Token: 0x04003EEC RID: 16108
	private bool isEmployee;

	// Token: 0x04003EED RID: 16109
	private bool isSigningUp;

	// Token: 0x04003EEE RID: 16110
	private const string GR_DATA_KEY = "GRData";
}
