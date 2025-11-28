using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D42 RID: 3394
	internal class Api
	{
		// Token: 0x0600525A RID: 21082 RVA: 0x001A5083 File Offset: 0x001A3283
		static Api()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x0600525B RID: 21083
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_GetLicense")]
		internal static extern void GetLicense(GetLicenseCallback callback, string appId, string appKey);

		// Token: 0x0600525C RID: 21084
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_GetLicense")]
		internal static extern void GetLicense_64(GetLicenseCallback callback, string appId, string appKey);

		// Token: 0x0600525D RID: 21085
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_Init")]
		internal static extern int Init(StatusCallback initCallback, string appId);

		// Token: 0x0600525E RID: 21086
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_Init")]
		internal static extern int Init_64(StatusCallback initCallback, string appId);

		// Token: 0x0600525F RID: 21087
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_Shutdown")]
		internal static extern int Shutdown(StatusCallback initCallback);

		// Token: 0x06005260 RID: 21088
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_Shutdown")]
		internal static extern int Shutdown_64(StatusCallback initCallback);

		// Token: 0x06005261 RID: 21089
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_Version")]
		internal static extern IntPtr Version();

		// Token: 0x06005262 RID: 21090
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_Version")]
		internal static extern IntPtr Version_64();

		// Token: 0x06005263 RID: 21091
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_QueryRuntimeMode")]
		internal static extern void QueryRuntimeMode(QueryRuntimeModeCallback queryRunTimeCallback);

		// Token: 0x06005264 RID: 21092
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportAPI_QueryRuntimeMode")]
		internal static extern void QueryRuntimeMode_64(QueryRuntimeModeCallback queryRunTimeCallback);

		// Token: 0x06005265 RID: 21093
		[DllImport("kernel32.dll")]
		internal static extern IntPtr LoadLibrary(string dllToLoad);

		// Token: 0x06005266 RID: 21094 RVA: 0x001A5090 File Offset: 0x001A3290
		internal static void LoadLibraryManually(string dllName)
		{
			if (string.IsNullOrEmpty(dllName))
			{
				return;
			}
			if (IntPtr.Size == 8)
			{
				Api.LoadLibrary("x64/" + dllName + "64.dll");
				return;
			}
			Api.LoadLibrary("x86/" + dllName + ".dll");
		}
	}
}
