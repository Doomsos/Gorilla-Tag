using System;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal.Arcade;

namespace Viveport.Arcade
{
	// Token: 0x02000D4C RID: 3404
	internal class Session
	{
		// Token: 0x060052E4 RID: 21220 RVA: 0x001A50DD File Offset: 0x001A32DD
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			Session.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x060052E5 RID: 21221 RVA: 0x001A50EB File Offset: 0x001A32EB
		public static void IsReady(Session.SessionListener listener)
		{
			Session.isReadyIl2cppCallback = new Session.SessionHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				Session.IsReady_64(new SessionCallback(Session.IsReadyIl2cppCallback));
				return;
			}
			Session.IsReady(new SessionCallback(Session.IsReadyIl2cppCallback));
		}

		// Token: 0x060052E6 RID: 21222 RVA: 0x001A5128 File Offset: 0x001A3328
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StartIl2cppCallback(int errorCode, string message)
		{
			Session.startIl2cppCallback(errorCode, message);
		}

		// Token: 0x060052E7 RID: 21223 RVA: 0x001A5136 File Offset: 0x001A3336
		public static void Start(Session.SessionListener listener)
		{
			Session.startIl2cppCallback = new Session.SessionHandler(listener).getStartHandler();
			if (IntPtr.Size == 8)
			{
				Session.Start_64(new SessionCallback(Session.StartIl2cppCallback));
				return;
			}
			Session.Start(new SessionCallback(Session.StartIl2cppCallback));
		}

		// Token: 0x060052E8 RID: 21224 RVA: 0x001A5173 File Offset: 0x001A3373
		[MonoPInvokeCallback(typeof(SessionCallback))]
		private static void StopIl2cppCallback(int errorCode, string message)
		{
			Session.stopIl2cppCallback(errorCode, message);
		}

		// Token: 0x060052E9 RID: 21225 RVA: 0x001A5181 File Offset: 0x001A3381
		public static void Stop(Session.SessionListener listener)
		{
			Session.stopIl2cppCallback = new Session.SessionHandler(listener).getStopHandler();
			if (IntPtr.Size == 8)
			{
				Session.Stop_64(new SessionCallback(Session.StopIl2cppCallback));
				return;
			}
			Session.Stop(new SessionCallback(Session.StopIl2cppCallback));
		}

		// Token: 0x04006100 RID: 24832
		private static SessionCallback isReadyIl2cppCallback;

		// Token: 0x04006101 RID: 24833
		private static SessionCallback startIl2cppCallback;

		// Token: 0x04006102 RID: 24834
		private static SessionCallback stopIl2cppCallback;

		// Token: 0x02000D4D RID: 3405
		private class SessionHandler : Session.BaseHandler
		{
			// Token: 0x060052EB RID: 21227 RVA: 0x001A51BE File Offset: 0x001A33BE
			public SessionHandler(Session.SessionListener cb)
			{
				Session.SessionHandler.listener = cb;
			}

			// Token: 0x060052EC RID: 21228 RVA: 0x001A51CC File Offset: 0x001A33CC
			public SessionCallback getIsReadyHandler()
			{
				return new SessionCallback(this.IsReadyHandler);
			}

			// Token: 0x060052ED RID: 21229 RVA: 0x001A51DC File Offset: 0x001A33DC
			protected override void IsReadyHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[Session IsReadyHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string text = "[Session IsReadyHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text2 = "";
				string text3 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string text4 = "[IsReadyHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text3 = (string)jsonData["appID"];
						}
						catch (Exception ex5)
						{
							string text5 = "[IsReadyHandler] appID ex=";
							Exception ex6 = ex5;
							Logger.Log(text5 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] appID=" + text3);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnSuccess(text3);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060052EE RID: 21230 RVA: 0x001A5350 File Offset: 0x001A3550
			public SessionCallback getStartHandler()
			{
				return new SessionCallback(this.StartHandler);
			}

			// Token: 0x060052EF RID: 21231 RVA: 0x001A5360 File Offset: 0x001A3560
			protected override void StartHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[Session StartHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string text = "[Session StartHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text2 = "";
				string text3 = "";
				string text4 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string text5 = "[StartHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StartHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text3 = (string)jsonData["appID"];
							text4 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string text6 = "[StartHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(text6 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StartHandler] appID=" + text3 + ",Guid=" + text4);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStartSuccess(text3, text4);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060052F0 RID: 21232 RVA: 0x001A54F4 File Offset: 0x001A36F4
			public SessionCallback getStopHandler()
			{
				return new SessionCallback(this.StopHandler);
			}

			// Token: 0x060052F1 RID: 21233 RVA: 0x001A5504 File Offset: 0x001A3704
			protected override void StopHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[Session StopHandler] message=" + message + ",code=" + code.ToString());
				JsonData jsonData = null;
				try
				{
					jsonData = JsonMapper.ToObject(message);
				}
				catch (Exception ex)
				{
					string text = "[Session StopHandler] exception=";
					Exception ex2 = ex;
					Logger.Log(text + ((ex2 != null) ? ex2.ToString() : null));
				}
				int num = -1;
				string text2 = "";
				string text3 = "";
				string text4 = "";
				if (code == 0 && jsonData != null)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex3)
					{
						string text5 = "[StopHandler] statusCode, message ex=";
						Exception ex4 = ex3;
						Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[StopHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text3 = (string)jsonData["appID"];
							text4 = (string)jsonData["Guid"];
						}
						catch (Exception ex5)
						{
							string text6 = "[StopHandler] appID, Guid ex=";
							Exception ex6 = ex5;
							Logger.Log(text6 + ((ex6 != null) ? ex6.ToString() : null));
						}
						Logger.Log("[StopHandler] appID=" + text3 + ",Guid=" + text4);
					}
				}
				if (Session.SessionHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							Session.SessionHandler.listener.OnStopSuccess(text3, text4);
							return;
						}
						Session.SessionHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						Session.SessionHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x04006103 RID: 24835
			private static Session.SessionListener listener;
		}

		// Token: 0x02000D4E RID: 3406
		private abstract class BaseHandler
		{
			// Token: 0x060052F2 RID: 21234
			protected abstract void IsReadyHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060052F3 RID: 21235
			protected abstract void StartHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060052F4 RID: 21236
			protected abstract void StopHandler(int code, [MarshalAs(20)] string message);
		}

		// Token: 0x02000D4F RID: 3407
		public class SessionListener
		{
			// Token: 0x060052F6 RID: 21238 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnSuccess(string pchAppID)
			{
			}

			// Token: 0x060052F7 RID: 21239 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnStartSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x060052F8 RID: 21240 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnStopSuccess(string pchAppID, string pchGuid)
			{
			}

			// Token: 0x060052F9 RID: 21241 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}
		}
	}
}
