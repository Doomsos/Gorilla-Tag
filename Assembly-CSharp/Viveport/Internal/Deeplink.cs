using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Viveport.Internal
{
	// Token: 0x02000D49 RID: 3401
	internal class Deeplink
	{
		// Token: 0x060052CB RID: 21195 RVA: 0x001A5083 File Offset: 0x001A3283
		static Deeplink()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x060052CC RID: 21196
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_IsReady")]
		internal static extern void IsReady(StatusCallback IsReadyCallback);

		// Token: 0x060052CD RID: 21197
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_IsReady")]
		internal static extern void IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x060052CE RID: 21198
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GoToApp")]
		internal static extern void GoToApp(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData);

		// Token: 0x060052CF RID: 21199
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GoToApp")]
		internal static extern void GoToApp_64(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData);

		// Token: 0x060052D0 RID: 21200
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GoToAppWithBranchName")]
		internal static extern void GoToApp(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData, string branchName);

		// Token: 0x060052D1 RID: 21201
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GoToAppWithBranchName")]
		internal static extern void GoToApp_64(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData, string branchName);

		// Token: 0x060052D2 RID: 21202
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GoToStore")]
		internal static extern void GoToStore(StatusCallback2 GetSessionTokenCallback, string ViveportId);

		// Token: 0x060052D3 RID: 21203
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GoToStore")]
		internal static extern void GoToStore_64(StatusCallback2 GetSessionTokenCallback, string ViveportId);

		// Token: 0x060052D4 RID: 21204
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GoToAppOrGoToStore")]
		internal static extern void GoToAppOrGoToStore(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData);

		// Token: 0x060052D5 RID: 21205
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GoToAppOrGoToStore")]
		internal static extern void GoToAppOrGoToStore_64(StatusCallback2 GoToAppCallback, string ViveportId, string LaunchData);

		// Token: 0x060052D6 RID: 21206
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GetAppLaunchData")]
		internal static extern int GetAppLaunchData(StringBuilder userId, int size);

		// Token: 0x060052D7 RID: 21207
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDeeplink_GetAppLaunchData")]
		internal static extern int GetAppLaunchData_64(StringBuilder userId, int size);
	}
}
