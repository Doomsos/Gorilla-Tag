using System;
using UnityEngine;

// Token: 0x02000B9E RID: 2974
public class PlayFabAuthenticatorSettings
{
	// Token: 0x06004975 RID: 18805 RVA: 0x00181530 File Offset: 0x0017F730
	static PlayFabAuthenticatorSettings()
	{
		PlayFabAuthenticatorSettings.Load("PlayFabAuthenticatorSettings");
	}

	// Token: 0x06004976 RID: 18806 RVA: 0x0018153C File Offset: 0x0017F73C
	public static void Load(string path)
	{
		PlayFabAuthenticatorSettingsScriptableObject playFabAuthenticatorSettingsScriptableObject = Resources.Load<PlayFabAuthenticatorSettingsScriptableObject>(path);
		PlayFabAuthenticatorSettings.TitleId = playFabAuthenticatorSettingsScriptableObject.TitleId;
		PlayFabAuthenticatorSettings.AuthApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.AuthApiBaseUrl;
		PlayFabAuthenticatorSettings.DailyQuestsApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.DailyQuestsApiBaseUrl;
		PlayFabAuthenticatorSettings.FriendApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.FriendApiBaseUrl;
		PlayFabAuthenticatorSettings.HpPromoApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.HpPromoApiBaseUrl;
		PlayFabAuthenticatorSettings.IapApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.IapApiBaseUrl;
		PlayFabAuthenticatorSettings.KidApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.KidApiBaseUrl;
		PlayFabAuthenticatorSettings.MmrApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.MmrApiBaseUrl;
		PlayFabAuthenticatorSettings.ModerationApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.ModerationApiBaseUrl;
		PlayFabAuthenticatorSettings.ProgressionApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.ProgressionApiBaseUrl;
		PlayFabAuthenticatorSettings.TitleDataApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.TitleDataApiBaseUrl;
		PlayFabAuthenticatorSettings.VotingApiBaseUrl = playFabAuthenticatorSettingsScriptableObject.VotingApiBaseUrl;
	}

	// Token: 0x040059EC RID: 23020
	public static string TitleId;

	// Token: 0x040059ED RID: 23021
	public static string AuthApiBaseUrl;

	// Token: 0x040059EE RID: 23022
	public static string DailyQuestsApiBaseUrl;

	// Token: 0x040059EF RID: 23023
	public static string FriendApiBaseUrl;

	// Token: 0x040059F0 RID: 23024
	public static string HpPromoApiBaseUrl;

	// Token: 0x040059F1 RID: 23025
	public static string IapApiBaseUrl;

	// Token: 0x040059F2 RID: 23026
	public static string KidApiBaseUrl;

	// Token: 0x040059F3 RID: 23027
	public static string MmrApiBaseUrl;

	// Token: 0x040059F4 RID: 23028
	public static string ModerationApiBaseUrl;

	// Token: 0x040059F5 RID: 23029
	public static string ProgressionApiBaseUrl;

	// Token: 0x040059F6 RID: 23030
	public static string TitleDataApiBaseUrl;

	// Token: 0x040059F7 RID: 23031
	public static string VotingApiBaseUrl;
}
