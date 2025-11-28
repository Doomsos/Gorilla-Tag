using System;
using UnityEngine;

// Token: 0x02000AE6 RID: 2790
public static class LocalizationTelemetry
{
	// Token: 0x1700068D RID: 1677
	// (get) Token: 0x0600458E RID: 17806 RVA: 0x00033213 File Offset: 0x00031413
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x0400578D RID: 22413
	public const string LANGUAGE_CHANGED_EVENT_NAME = "language_changed";

	// Token: 0x0400578E RID: 22414
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x0400578F RID: 22415
	public const string STARTING_LANGUAGE_BODY_DATA = "starting_language";

	// Token: 0x04005790 RID: 22416
	public const string NEW_LANGUAGE_BODY_DATA = "new_language";
}
