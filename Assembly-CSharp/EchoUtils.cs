using System;
using UnityEngine;

// Token: 0x02000C47 RID: 3143
public static class EchoUtils
{
	// Token: 0x06004D0E RID: 19726 RVA: 0x00190028 File Offset: 0x0018E228
	[HideInCallstack]
	public static T Echo<T>(this T message)
	{
		Debug.Log(message);
		return message;
	}

	// Token: 0x06004D0F RID: 19727 RVA: 0x00190036 File Offset: 0x0018E236
	[HideInCallstack]
	public static T Echo<T>(this T message, Object context)
	{
		Debug.Log(message, context);
		return message;
	}
}
