using System;
using UnityEngine;

// Token: 0x02000B9C RID: 2972
[CreateAssetMenu(fileName = "PhotonAuthenticatorSettings", menuName = "ScriptableObjects/PhotonAuthenticatorSettings")]
public class PhotonAuthenticatorSettingsScriptableObject : ScriptableObject
{
	// Token: 0x040059E9 RID: 23017
	public string PunAppId;

	// Token: 0x040059EA RID: 23018
	public string FusionAppId;

	// Token: 0x040059EB RID: 23019
	public string VoiceAppId;
}
