using System;
using UnityEngine;

// Token: 0x02000B9B RID: 2971
public class PhotonAuthenticatorSettings
{
	// Token: 0x06004963 RID: 18787 RVA: 0x001814DC File Offset: 0x0017F6DC
	static PhotonAuthenticatorSettings()
	{
		PhotonAuthenticatorSettings.Load("PhotonAuthenticatorSettings");
	}

	// Token: 0x06004964 RID: 18788 RVA: 0x001814E8 File Offset: 0x0017F6E8
	public static void Load(string path)
	{
		PhotonAuthenticatorSettingsScriptableObject photonAuthenticatorSettingsScriptableObject = Resources.Load<PhotonAuthenticatorSettingsScriptableObject>(path);
		PhotonAuthenticatorSettings.PunAppId = photonAuthenticatorSettingsScriptableObject.PunAppId;
		PhotonAuthenticatorSettings.FusionAppId = photonAuthenticatorSettingsScriptableObject.FusionAppId;
		PhotonAuthenticatorSettings.VoiceAppId = photonAuthenticatorSettingsScriptableObject.VoiceAppId;
	}

	// Token: 0x040059E6 RID: 23014
	public static string PunAppId;

	// Token: 0x040059E7 RID: 23015
	public static string FusionAppId;

	// Token: 0x040059E8 RID: 23016
	public static string VoiceAppId;
}
