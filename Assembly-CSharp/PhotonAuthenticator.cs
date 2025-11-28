using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000B9A RID: 2970
public class PhotonAuthenticator : MonoBehaviour
{
	// Token: 0x06004960 RID: 18784 RVA: 0x00181486 File Offset: 0x0017F686
	private void Awake()
	{
		Debug.Log("Environment is *************** PRODUCTION PUN *******************");
		PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime = PhotonAuthenticatorSettings.PunAppId;
		PhotonNetwork.PhotonServerSettings.AppSettings.AppIdVoice = PhotonAuthenticatorSettings.VoiceAppId;
	}

	// Token: 0x06004961 RID: 18785 RVA: 0x001814BC File Offset: 0x0017F6BC
	public void SetCustomAuthenticationParameters(Dictionary<string, object> customAuthData)
	{
		AuthenticationValues authenticationValues = new AuthenticationValues();
		authenticationValues.AuthType = 0;
		authenticationValues.SetAuthPostData(customAuthData);
		NetworkSystem.Instance.SetAuthenticationValues(authenticationValues);
		Debug.Log("Set Photon auth data. AppVersion is: " + NetworkSystemConfig.AppVersion);
	}
}
