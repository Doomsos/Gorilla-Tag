using System;
using System.Text;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000D18 RID: 3352
	public class User
	{
		// Token: 0x06005149 RID: 20809 RVA: 0x001A28E9 File Offset: 0x001A0AE9
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			User.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x0600514A RID: 20810 RVA: 0x001A28F8 File Offset: 0x001A0AF8
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			User.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(User.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return User.IsReady_64(new StatusCallback(User.IsReadyIl2cppCallback));
			}
			return User.IsReady(new StatusCallback(User.IsReadyIl2cppCallback));
		}

		// Token: 0x0600514B RID: 20811 RVA: 0x001A2968 File Offset: 0x001A0B68
		public static string GetUserId()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserID_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserID(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600514C RID: 20812 RVA: 0x001A29A8 File Offset: 0x001A0BA8
		public static string GetUserName()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (IntPtr.Size == 8)
			{
				User.GetUserName_64(stringBuilder, 256);
			}
			else
			{
				User.GetUserName(stringBuilder, 256);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600514D RID: 20813 RVA: 0x001A29E8 File Offset: 0x001A0BE8
		public static string GetUserAvatarUrl()
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			if (IntPtr.Size == 8)
			{
				User.GetUserAvatarUrl_64(stringBuilder, 512);
			}
			else
			{
				User.GetUserAvatarUrl(stringBuilder, 512);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04006067 RID: 24679
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x04006068 RID: 24680
		private const int MaxIdLength = 256;

		// Token: 0x04006069 RID: 24681
		private const int MaxNameLength = 256;

		// Token: 0x0400606A RID: 24682
		private const int MaxUrlLength = 512;
	}
}
