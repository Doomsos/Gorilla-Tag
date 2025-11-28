using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000D30 RID: 3376
	public class Deeplink
	{
		// Token: 0x06005225 RID: 21029 RVA: 0x001A4CA9 File Offset: 0x001A2EA9
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			Deeplink.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06005226 RID: 21030 RVA: 0x001A4CB8 File Offset: 0x001A2EB8
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Deeplink.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.IsReady_64(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
				return;
			}
			Deeplink.IsReady(new StatusCallback(Deeplink.IsReadyIl2cppCallback));
		}

		// Token: 0x06005227 RID: 21031 RVA: 0x001A4D25 File Offset: 0x001A2F25
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005228 RID: 21032 RVA: 0x001A4D34 File Offset: 0x001A2F34
		public static void GoToApp(StatusCallback2 callback, string viveportId, string launchData)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToApp_64(new StatusCallback2(Deeplink.GoToAppIl2cppCallback), viveportId, launchData);
				return;
			}
			Deeplink.GoToApp(new StatusCallback2(Deeplink.GoToAppIl2cppCallback), viveportId, launchData);
		}

		// Token: 0x06005229 RID: 21033 RVA: 0x001A4DAD File Offset: 0x001A2FAD
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppWithBranchNameIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppWithBranchNameIl2cppCallback(errorCode, message);
		}

		// Token: 0x0600522A RID: 21034 RVA: 0x001A4DBC File Offset: 0x001A2FBC
		public static void GoToApp(StatusCallback2 callback, string viveportId, string launchData, string branchName)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppWithBranchNameIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToApp_64(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback), viveportId, launchData, branchName);
				return;
			}
			Deeplink.GoToApp(new StatusCallback2(Deeplink.GoToAppWithBranchNameIl2cppCallback), viveportId, launchData, branchName);
		}

		// Token: 0x0600522B RID: 21035 RVA: 0x001A4E37 File Offset: 0x001A3037
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToStoreIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToStoreIl2cppCallback(errorCode, message);
		}

		// Token: 0x0600522C RID: 21036 RVA: 0x001A4E48 File Offset: 0x001A3048
		public static void GoToStore(StatusCallback2 callback, string viveportId = "")
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToStoreIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToStore_64(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback), viveportId);
				return;
			}
			Deeplink.GoToStore(new StatusCallback2(Deeplink.GoToStoreIl2cppCallback), viveportId);
		}

		// Token: 0x0600522D RID: 21037 RVA: 0x001A4EB7 File Offset: 0x001A30B7
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GoToAppOrGoToStoreIl2cppCallback(int errorCode, string message)
		{
			Deeplink.goToAppOrGoToStoreIl2cppCallback(errorCode, message);
		}

		// Token: 0x0600522E RID: 21038 RVA: 0x001A4EC8 File Offset: 0x001A30C8
		public static void GoToAppOrGoToStore(StatusCallback2 callback, string viveportId, string launchData)
		{
			if (callback == null || string.IsNullOrEmpty(viveportId))
			{
				throw new InvalidOperationException("callback == null || string.IsNullOrEmpty(viveportId)");
			}
			Deeplink.goToAppOrGoToStoreIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Deeplink.GoToAppOrGoToStore_64(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback), viveportId, launchData);
				return;
			}
			Deeplink.GoToAppOrGoToStore(new StatusCallback2(Deeplink.GoToAppOrGoToStoreIl2cppCallback), viveportId, launchData);
		}

		// Token: 0x0600522F RID: 21039 RVA: 0x001A4F44 File Offset: 0x001A3144
		public static string GetAppLaunchData()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				Deeplink.GetAppLaunchData_64(stringBuilder, 256);
			}
			else
			{
				Deeplink.GetAppLaunchData(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040060CA RID: 24778
		private const int MaxIdLength = 256;

		// Token: 0x040060CB RID: 24779
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x040060CC RID: 24780
		private static StatusCallback2 goToAppIl2cppCallback;

		// Token: 0x040060CD RID: 24781
		private static StatusCallback2 goToAppWithBranchNameIl2cppCallback;

		// Token: 0x040060CE RID: 24782
		private static StatusCallback2 goToStoreIl2cppCallback;

		// Token: 0x040060CF RID: 24783
		private static StatusCallback2 goToAppOrGoToStoreIl2cppCallback;
	}
}
