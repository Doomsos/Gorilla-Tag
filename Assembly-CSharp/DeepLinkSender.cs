using System;
using UnityEngine;

// Token: 0x02000508 RID: 1288
public static class DeepLinkSender
{
	// Token: 0x060020F2 RID: 8434 RVA: 0x000AE6B8 File Offset: 0x000AC8B8
	public static bool SendDeepLink(ulong deepLinkAppID, string deepLinkMessage, Action<string> onSent)
	{
		Debug.LogError("[DeepLinkSender::SendDeepLink] Called on non-oculus platform!");
		return false;
	}

	// Token: 0x04002B9E RID: 11166
	private static Action<string> currentDeepLinkSentCallback;
}
