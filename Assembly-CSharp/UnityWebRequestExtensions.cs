using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000ABD RID: 2749
public static class UnityWebRequestExtensions
{
	// Token: 0x060044E0 RID: 17632 RVA: 0x0016CEA4 File Offset: 0x0016B0A4
	public static TaskAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
	{
		TaskCompletionSource<UnityWebRequest> tcs = new TaskCompletionSource<UnityWebRequest>();
		asyncOp.completed += delegate(AsyncOperation operation)
		{
			tcs.TrySetResult(asyncOp.webRequest);
		};
		return tcs.Task.GetAwaiter();
	}
}
