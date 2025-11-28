using System;
using UnityEngine;

// Token: 0x02000C47 RID: 3143
public static class EchoUtils
{
	// Token: 0x06004D0E RID: 19726 RVA: 0x00190048 File Offset: 0x0018E248
	[HideInCallstack]
	public static T Echo<T>(this T message)
	{
		Debug.Log(message);
		return message;
	}

	// Token: 0x06004D0F RID: 19727 RVA: 0x00190056 File Offset: 0x0018E256
	[HideInCallstack]
	public static T Echo<T>(this T message, Object context)
	{
		Debug.Log(message, context);
		return message;
	}
}
