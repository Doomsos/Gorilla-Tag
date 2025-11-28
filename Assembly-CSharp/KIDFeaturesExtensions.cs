using System;

// Token: 0x02000A2E RID: 2606
public static class KIDFeaturesExtensions
{
	// Token: 0x0600420C RID: 16908 RVA: 0x0015D684 File Offset: 0x0015B884
	public static string ToStandardisedString(this EKIDFeatures feature)
	{
		switch (feature)
		{
		case EKIDFeatures.Multiplayer:
			return "multiplayer";
		case EKIDFeatures.Custom_Nametags:
			return "custom-username";
		case EKIDFeatures.Voice_Chat:
			return "voice-chat";
		case EKIDFeatures.Mods:
			return "mods";
		case EKIDFeatures.Groups:
			return "join-groups";
		default:
			return feature.ToString();
		}
	}

	// Token: 0x0600420D RID: 16909 RVA: 0x0015D6D8 File Offset: 0x0015B8D8
	public static EKIDFeatures? FromString(string name)
	{
		string text = name.ToLower();
		if (text == "voice-chat")
		{
			return new EKIDFeatures?(EKIDFeatures.Voice_Chat);
		}
		if (text == "custom-username")
		{
			return new EKIDFeatures?(EKIDFeatures.Custom_Nametags);
		}
		if (text == "multiplayer")
		{
			return new EKIDFeatures?(EKIDFeatures.Multiplayer);
		}
		if (text == "mods")
		{
			return new EKIDFeatures?(EKIDFeatures.Mods);
		}
		if (!(text == "join-groups"))
		{
			return default(EKIDFeatures?);
		}
		return new EKIDFeatures?(EKIDFeatures.Groups);
	}

	// Token: 0x0600420E RID: 16910 RVA: 0x0015D75C File Offset: 0x0015B95C
	public static bool TryGetFromString(string name, out EKIDFeatures result)
	{
		EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(name);
		if (ekidfeatures != null)
		{
			result = ekidfeatures.Value;
			return true;
		}
		result = EKIDFeatures.Voice_Chat;
		return false;
	}
}
