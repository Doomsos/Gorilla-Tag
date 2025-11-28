using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000D31 RID: 3377
	internal class Token
	{
		// Token: 0x06005231 RID: 21041 RVA: 0x001A4F84 File Offset: 0x001A3184
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			Token.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06005232 RID: 21042 RVA: 0x001A4F94 File Offset: 0x001A3194
		public static void IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Token.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(Token.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Token.IsReady_64(new StatusCallback(Token.IsReadyIl2cppCallback));
				return;
			}
			Token.IsReady(new StatusCallback(Token.IsReadyIl2cppCallback));
		}

		// Token: 0x06005233 RID: 21043 RVA: 0x001A5003 File Offset: 0x001A3203
		[MonoPInvokeCallback(typeof(StatusCallback2))]
		private static void GetSessionTokenIl2cppCallback(int errorCode, string message)
		{
			Token.getSessionTokenIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005234 RID: 21044 RVA: 0x001A5014 File Offset: 0x001A3214
		public static void GetSessionToken(StatusCallback2 callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			Token.getSessionTokenIl2cppCallback = new StatusCallback2(callback.Invoke);
			Api.InternalStatusCallback2s.Add(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				Token.GetSessionToken_64(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
				return;
			}
			Token.GetSessionToken(new StatusCallback2(Token.GetSessionTokenIl2cppCallback));
		}

		// Token: 0x040060D0 RID: 24784
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x040060D1 RID: 24785
		private static StatusCallback2 getSessionTokenIl2cppCallback;
	}
}
