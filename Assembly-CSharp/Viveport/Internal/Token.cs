using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D48 RID: 3400
	internal class Token
	{
		// Token: 0x060052C5 RID: 21189 RVA: 0x001A5063 File Offset: 0x001A3263
		static Token()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x060052C6 RID: 21190
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportToken_IsReady")]
		internal static extern int IsReady(StatusCallback IsReadyCallback);

		// Token: 0x060052C7 RID: 21191
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportToken_IsReady")]
		internal static extern int IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x060052C8 RID: 21192
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportToken_GetSessionToken")]
		internal static extern int GetSessionToken(StatusCallback2 GetSessionTokenCallback);

		// Token: 0x060052C9 RID: 21193
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportToken_GetSessionToken")]
		internal static extern int GetSessionToken_64(StatusCallback2 GetSessionTokenCallback);
	}
}
