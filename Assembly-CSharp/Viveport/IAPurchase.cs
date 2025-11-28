using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using LitJson;
using Viveport.Core;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000D22 RID: 3362
	public class IAPurchase
	{
		// Token: 0x06005174 RID: 20852 RVA: 0x001A309F File Offset: 0x001A129F
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void IsReadyIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.isReadyIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005175 RID: 20853 RVA: 0x001A30AD File Offset: 0x001A12AD
		public static void IsReady(IAPurchase.IAPurchaseListener listener, string pchAppKey)
		{
			IAPurchase.isReadyIl2cppCallback = new IAPurchase.IAPHandler(listener).getIsReadyHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.IsReady_64(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
				return;
			}
			IAPurchase.IsReady(new IAPurchaseCallback(IAPurchase.IsReadyIl2cppCallback), pchAppKey);
		}

		// Token: 0x06005176 RID: 20854 RVA: 0x001A30EC File Offset: 0x001A12EC
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request01Il2cppCallback(errorCode, message);
		}

		// Token: 0x06005177 RID: 20855 RVA: 0x001A30FA File Offset: 0x001A12FA
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice)
		{
			IAPurchase.request01Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request01Il2cppCallback), pchPrice);
		}

		// Token: 0x06005178 RID: 20856 RVA: 0x001A3139 File Offset: 0x001A1339
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Request02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.request02Il2cppCallback(errorCode, message);
		}

		// Token: 0x06005179 RID: 20857 RVA: 0x001A3148 File Offset: 0x001A1348
		public static void Request(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchUserData)
		{
			IAPurchase.request02Il2cppCallback = new IAPurchase.IAPHandler(listener).getRequestHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Request_64(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
				return;
			}
			IAPurchase.Request(new IAPurchaseCallback(IAPurchase.Request02Il2cppCallback), pchPrice, pchUserData);
		}

		// Token: 0x0600517A RID: 20858 RVA: 0x001A3194 File Offset: 0x001A1394
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void PurchaseIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.purchaseIl2cppCallback(errorCode, message);
		}

		// Token: 0x0600517B RID: 20859 RVA: 0x001A31A2 File Offset: 0x001A13A2
		public static void Purchase(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.purchaseIl2cppCallback = new IAPurchase.IAPHandler(listener).getPurchaseHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Purchase_64(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Purchase(new IAPurchaseCallback(IAPurchase.PurchaseIl2cppCallback), pchPurchaseId);
		}

		// Token: 0x0600517C RID: 20860 RVA: 0x001A31E1 File Offset: 0x001A13E1
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query01Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query01Il2cppCallback(errorCode, message);
		}

		// Token: 0x0600517D RID: 20861 RVA: 0x001A31EF File Offset: 0x001A13EF
		public static void Query(IAPurchase.IAPurchaseListener listener, string pchPurchaseId)
		{
			IAPurchase.query01Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query01Il2cppCallback), pchPurchaseId);
		}

		// Token: 0x0600517E RID: 20862 RVA: 0x001A322E File Offset: 0x001A142E
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void Query02Il2cppCallback(int errorCode, string message)
		{
			IAPurchase.query02Il2cppCallback(errorCode, message);
		}

		// Token: 0x0600517F RID: 20863 RVA: 0x001A323C File Offset: 0x001A143C
		public static void Query(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.query02Il2cppCallback = new IAPurchase.IAPHandler(listener).getQueryListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Query_64(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
				return;
			}
			IAPurchase.Query(new IAPurchaseCallback(IAPurchase.Query02Il2cppCallback));
		}

		// Token: 0x06005180 RID: 20864 RVA: 0x001A3279 File Offset: 0x001A1479
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void GetBalanceIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.getBalanceIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005181 RID: 20865 RVA: 0x001A3287 File Offset: 0x001A1487
		public static void GetBalance(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.getBalanceIl2cppCallback = new IAPurchase.IAPHandler(listener).getBalanceHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.GetBalance_64(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
				return;
			}
			IAPurchase.GetBalance(new IAPurchaseCallback(IAPurchase.GetBalanceIl2cppCallback));
		}

		// Token: 0x06005182 RID: 20866 RVA: 0x001A32C4 File Offset: 0x001A14C4
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005183 RID: 20867 RVA: 0x001A32D4 File Offset: 0x001A14D4
		public static void RequestSubscription(IAPurchase.IAPurchaseListener listener, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId)
		{
			IAPurchase.requestSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscription_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
				return;
			}
			IAPurchase.RequestSubscription(new IAPurchaseCallback(IAPurchase.RequestSubscriptionIl2cppCallback), pchPrice, pchFreeTrialType, nFreeTrialValue, pchChargePeriodType, nChargePeriodValue, nNumberOfChargePeriod, pchPlanId);
		}

		// Token: 0x06005184 RID: 20868 RVA: 0x001A3332 File Offset: 0x001A1532
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void RequestSubscriptionWithPlanIDIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005185 RID: 20869 RVA: 0x001A3340 File Offset: 0x001A1540
		public static void RequestSubscriptionWithPlanID(IAPurchase.IAPurchaseListener listener, string pchPlanId)
		{
			IAPurchase.requestSubscriptionWithPlanIDIl2cppCallback = new IAPurchase.IAPHandler(listener).getRequestSubscriptionWithPlanIDHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.RequestSubscriptionWithPlanID_64(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
				return;
			}
			IAPurchase.RequestSubscriptionWithPlanID(new IAPurchaseCallback(IAPurchase.RequestSubscriptionWithPlanIDIl2cppCallback), pchPlanId);
		}

		// Token: 0x06005186 RID: 20870 RVA: 0x001A337F File Offset: 0x001A157F
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void SubscribeIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.subscribeIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005187 RID: 20871 RVA: 0x001A338D File Offset: 0x001A158D
		public static void Subscribe(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.subscribeIl2cppCallback = new IAPurchase.IAPHandler(listener).getSubscribeHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.Subscribe_64(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.Subscribe(new IAPurchaseCallback(IAPurchase.SubscribeIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x06005188 RID: 20872 RVA: 0x001A33CC File Offset: 0x001A15CC
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x06005189 RID: 20873 RVA: 0x001A33DA File Offset: 0x001A15DA
		public static void QuerySubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.querySubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscription_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.QuerySubscription(new IAPurchaseCallback(IAPurchase.QuerySubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x0600518A RID: 20874 RVA: 0x001A3419 File Offset: 0x001A1619
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void QuerySubscriptionListIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.querySubscriptionListIl2cppCallback(errorCode, message);
		}

		// Token: 0x0600518B RID: 20875 RVA: 0x001A3427 File Offset: 0x001A1627
		public static void QuerySubscriptionList(IAPurchase.IAPurchaseListener listener)
		{
			IAPurchase.querySubscriptionListIl2cppCallback = new IAPurchase.IAPHandler(listener).getQuerySubscriptionListHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.QuerySubscriptionList_64(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
				return;
			}
			IAPurchase.QuerySubscriptionList(new IAPurchaseCallback(IAPurchase.QuerySubscriptionListIl2cppCallback));
		}

		// Token: 0x0600518C RID: 20876 RVA: 0x001A3464 File Offset: 0x001A1664
		[MonoPInvokeCallback(typeof(IAPurchaseCallback))]
		private static void CancelSubscriptionIl2cppCallback(int errorCode, string message)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback(errorCode, message);
		}

		// Token: 0x0600518D RID: 20877 RVA: 0x001A3472 File Offset: 0x001A1672
		public static void CancelSubscription(IAPurchase.IAPurchaseListener listener, string pchSubscriptionId)
		{
			IAPurchase.cancelSubscriptionIl2cppCallback = new IAPurchase.IAPHandler(listener).getCancelSubscriptionHandler();
			if (IntPtr.Size == 8)
			{
				IAPurchase.CancelSubscription_64(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
				return;
			}
			IAPurchase.CancelSubscription(new IAPurchaseCallback(IAPurchase.CancelSubscriptionIl2cppCallback), pchSubscriptionId);
		}

		// Token: 0x04006090 RID: 24720
		private static IAPurchaseCallback isReadyIl2cppCallback;

		// Token: 0x04006091 RID: 24721
		private static IAPurchaseCallback request01Il2cppCallback;

		// Token: 0x04006092 RID: 24722
		private static IAPurchaseCallback request02Il2cppCallback;

		// Token: 0x04006093 RID: 24723
		private static IAPurchaseCallback purchaseIl2cppCallback;

		// Token: 0x04006094 RID: 24724
		private static IAPurchaseCallback query01Il2cppCallback;

		// Token: 0x04006095 RID: 24725
		private static IAPurchaseCallback query02Il2cppCallback;

		// Token: 0x04006096 RID: 24726
		private static IAPurchaseCallback getBalanceIl2cppCallback;

		// Token: 0x04006097 RID: 24727
		private static IAPurchaseCallback requestSubscriptionIl2cppCallback;

		// Token: 0x04006098 RID: 24728
		private static IAPurchaseCallback requestSubscriptionWithPlanIDIl2cppCallback;

		// Token: 0x04006099 RID: 24729
		private static IAPurchaseCallback subscribeIl2cppCallback;

		// Token: 0x0400609A RID: 24730
		private static IAPurchaseCallback querySubscriptionIl2cppCallback;

		// Token: 0x0400609B RID: 24731
		private static IAPurchaseCallback querySubscriptionListIl2cppCallback;

		// Token: 0x0400609C RID: 24732
		private static IAPurchaseCallback cancelSubscriptionIl2cppCallback;

		// Token: 0x02000D23 RID: 3363
		private class IAPHandler : IAPurchase.BaseHandler
		{
			// Token: 0x0600518F RID: 20879 RVA: 0x001A34B1 File Offset: 0x001A16B1
			public IAPHandler(IAPurchase.IAPurchaseListener cb)
			{
				IAPurchase.IAPHandler.listener = cb;
			}

			// Token: 0x06005190 RID: 20880 RVA: 0x001A34BF File Offset: 0x001A16BF
			public IAPurchaseCallback getIsReadyHandler()
			{
				return new IAPurchaseCallback(this.IsReadyHandler);
			}

			// Token: 0x06005191 RID: 20881 RVA: 0x001A34D0 File Offset: 0x001A16D0
			protected override void IsReadyHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[IsReadyHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text3 = "[IsReadyHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[IsReadyHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["currencyName"];
						}
						catch (Exception ex3)
						{
							string text4 = "[IsReadyHandler] currencyName ex=";
							Exception ex4 = ex3;
							Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[IsReadyHandler] currencyName=" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005192 RID: 20882 RVA: 0x001A3600 File Offset: 0x001A1800
			public IAPurchaseCallback getRequestHandler()
			{
				return new IAPurchaseCallback(this.RequestHandler);
			}

			// Token: 0x06005193 RID: 20883 RVA: 0x001A3610 File Offset: 0x001A1810
			protected override void RequestHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[RequestHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text3 = "[RequestHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[RequestHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
						}
						catch (Exception ex3)
						{
							string text4 = "[RequestHandler] purchase_id ex=";
							Exception ex4 = ex3;
							Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[RequestHandler] purchaseId =" + text);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005194 RID: 20884 RVA: 0x001A3740 File Offset: 0x001A1940
			public IAPurchaseCallback getPurchaseHandler()
			{
				return new IAPurchaseCallback(this.PurchaseHandler);
			}

			// Token: 0x06005195 RID: 20885 RVA: 0x001A3750 File Offset: 0x001A1950
			protected override void PurchaseHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[PurchaseHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				long num2 = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text3 = "[PurchaseHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[PurchaseHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							num2 = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string text4 = "[PurchaseHandler] purchase_id,paid_timestamp ex=";
							Exception ex4 = ex3;
							Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[PurchaseHandler] purchaseId =" + text + ",paid_timestamp=" + num2.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnPurchaseSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005196 RID: 20886 RVA: 0x001A38A0 File Offset: 0x001A1AA0
			public IAPurchaseCallback getQueryHandler()
			{
				return new IAPurchaseCallback(this.QueryHandler);
			}

			// Token: 0x06005197 RID: 20887 RVA: 0x001A38B0 File Offset: 0x001A1AB0
			protected override void QueryHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[QueryHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				string text4 = "";
				string text5 = "";
				string text6 = "";
				long paid_timestamp = 0L;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text2 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text7 = "[QueryHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text7 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["purchase_id"];
							text3 = (string)jsonData["order_id"];
							text4 = (string)jsonData["status"];
							text5 = (string)jsonData["price"];
							text6 = (string)jsonData["currency"];
							paid_timestamp = (long)jsonData["paid_timestamp"];
						}
						catch (Exception ex3)
						{
							string text8 = "[QueryHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(text8 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] status =",
							text4,
							",price=",
							text5,
							",currency=",
							text6
						}));
						Logger.Log(string.Concat(new string[]
						{
							"[QueryHandler] purchaseId =",
							text,
							",order_id=",
							text3,
							",paid_timestamp=",
							paid_timestamp.ToString()
						}));
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryResponse queryResponse = new IAPurchase.QueryResponse();
							queryResponse.purchase_id = text;
							queryResponse.order_id = text3;
							queryResponse.price = text5;
							queryResponse.currency = text6;
							queryResponse.paid_timestamp = paid_timestamp;
							queryResponse.status = text4;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x06005198 RID: 20888 RVA: 0x001A3AFC File Offset: 0x001A1CFC
			public IAPurchaseCallback getQueryListHandler()
			{
				return new IAPurchaseCallback(this.QueryListHandler);
			}

			// Token: 0x06005199 RID: 20889 RVA: 0x001A3B0C File Offset: 0x001A1D0C
			protected override void QueryListHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[QueryListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				int total = 0;
				int from = 0;
				int to = 0;
				List<IAPurchase.QueryResponse2> list = new List<IAPurchase.QueryResponse2>();
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[QueryListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QueryListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							JsonData jsonData2 = JsonMapper.ToObject(text);
							total = (int)jsonData2["total"];
							from = (int)jsonData2["from"];
							to = (int)jsonData2["to"];
							JsonData jsonData3 = jsonData2["purchases"];
							bool isArray = jsonData3.IsArray;
							foreach (object obj in jsonData3)
							{
								JsonData jsonData4 = (JsonData)obj;
								IAPurchase.QueryResponse2 queryResponse = new IAPurchase.QueryResponse2();
								IDictionary dictionary = jsonData4;
								queryResponse.app_id = (dictionary.Contains("app_id") ? ((string)jsonData4["app_id"]) : "");
								queryResponse.currency = (dictionary.Contains("currency") ? ((string)jsonData4["currency"]) : "");
								queryResponse.purchase_id = (dictionary.Contains("purchase_id") ? ((string)jsonData4["purchase_id"]) : "");
								queryResponse.order_id = (dictionary.Contains("order_id") ? ((string)jsonData4["order_id"]) : "");
								queryResponse.price = (dictionary.Contains("price") ? ((string)jsonData4["price"]) : "");
								queryResponse.user_data = (dictionary.Contains("user_data") ? ((string)jsonData4["user_data"]) : "");
								if (dictionary.Contains("paid_timestamp"))
								{
									if (jsonData4["paid_timestamp"].IsLong)
									{
										queryResponse.paid_timestamp = (long)jsonData4["paid_timestamp"];
									}
									else if (jsonData4["paid_timestamp"].IsInt)
									{
										queryResponse.paid_timestamp = (long)((int)jsonData4["paid_timestamp"]);
									}
								}
								list.Add(queryResponse);
							}
						}
						catch (Exception ex3)
						{
							string text3 = "[QueryListHandler] purchase_id, order_id ex=";
							Exception ex4 = ex3;
							Logger.Log(text3 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.QueryListResponse queryListResponse = new IAPurchase.QueryListResponse();
							queryListResponse.total = total;
							queryListResponse.from = from;
							queryListResponse.to = to;
							queryListResponse.purchaseList = list;
							IAPurchase.IAPHandler.listener.OnQuerySuccess(queryListResponse);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0600519A RID: 20890 RVA: 0x001A3E94 File Offset: 0x001A2094
			public IAPurchaseCallback getBalanceHandler()
			{
				return new IAPurchaseCallback(this.BalanceHandler);
			}

			// Token: 0x0600519B RID: 20891 RVA: 0x001A3EA4 File Offset: 0x001A20A4
			protected override void BalanceHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[BalanceHandler] code=" + code.ToString() + ",message= " + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text3 = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text4 = "[BalanceHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text4 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[BalanceHandler] statusCode =" + num.ToString() + ",errMessage=" + text3);
					if (num == 0)
					{
						try
						{
							text = (string)jsonData["currencyName"];
							text2 = (string)jsonData["balance"];
						}
						catch (Exception ex3)
						{
							string text5 = "[BalanceHandler] currencyName, balance ex=";
							Exception ex4 = ex3;
							Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
						}
						Logger.Log("[BalanceHandler] currencyName=" + text + ",balance=" + text2);
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnBalanceSuccess(text2);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text3);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0600519C RID: 20892 RVA: 0x001A4000 File Offset: 0x001A2200
			public IAPurchaseCallback getRequestSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionHandler);
			}

			// Token: 0x0600519D RID: 20893 RVA: 0x001A4010 File Offset: 0x001A2210
			protected override void RequestSubscriptionHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[RequestSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string text3 = "[RequestSubscriptionHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string text4 = "[RequestSubscriptionHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0600519E RID: 20894 RVA: 0x001A4138 File Offset: 0x001A2338
			public IAPurchaseCallback getRequestSubscriptionWithPlanIDHandler()
			{
				return new IAPurchaseCallback(this.RequestSubscriptionWithPlanIDHandler);
			}

			// Token: 0x0600519F RID: 20895 RVA: 0x001A4148 File Offset: 0x001A2348
			protected override void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string text3 = "[RequestSubscriptionWithPlanIDHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(text3 + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[RequestSubscriptionWithPlanIDHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
					}
					catch (Exception ex3)
					{
						string text4 = "[RequestSubscriptionWithPlanIDHandler] subscription_id ex=";
						Exception ex4 = ex3;
						Logger.Log(text4 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log("[RequestSubscriptionWithPlanIDHandler] subscription_id =" + text);
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnRequestSubscriptionWithPlanIDSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060051A0 RID: 20896 RVA: 0x001A4270 File Offset: 0x001A2470
			public IAPurchaseCallback getSubscribeHandler()
			{
				return new IAPurchaseCallback(this.SubscribeHandler);
			}

			// Token: 0x060051A1 RID: 20897 RVA: 0x001A4280 File Offset: 0x001A2480
			protected override void SubscribeHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[SubscribeHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				string text2 = "";
				string text3 = "";
				long num2 = 0L;
				try
				{
					num = (int)jsonData["statusCode"];
					text2 = (string)jsonData["message"];
				}
				catch (Exception ex)
				{
					string text4 = "[SubscribeHandler] statusCode, message ex=";
					Exception ex2 = ex;
					Logger.Log(text4 + ((ex2 != null) ? ex2.ToString() : null));
				}
				Logger.Log("[SubscribeHandler] statusCode =" + num.ToString() + ",errMessage=" + text2);
				if (num == 0)
				{
					try
					{
						text = (string)jsonData["subscription_id"];
						text3 = (string)jsonData["plan_id"];
						num2 = (long)jsonData["subscribed_timestamp"];
					}
					catch (Exception ex3)
					{
						string text5 = "[SubscribeHandler] subscription_id, plan_id ex=";
						Exception ex4 = ex3;
						Logger.Log(text5 + ((ex4 != null) ? ex4.ToString() : null));
					}
					Logger.Log(string.Concat(new string[]
					{
						"[SubscribeHandler] subscription_id =",
						text,
						", plan_id=",
						text3,
						", timestamp=",
						num2.ToString()
					}));
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnSubscribeSuccess(text);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text2);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060051A2 RID: 20898 RVA: 0x001A4408 File Offset: 0x001A2608
			public IAPurchaseCallback getQuerySubscriptionHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionHandler);
			}

			// Token: 0x060051A3 RID: 20899 RVA: 0x001A4418 File Offset: 0x001A2618
			protected override void QuerySubscriptionHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[QuerySubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[QuerySubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string text3 = "[QuerySubscriptionHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(text3 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060051A4 RID: 20900 RVA: 0x001A4540 File Offset: 0x001A2740
			public IAPurchaseCallback getQuerySubscriptionListHandler()
			{
				return new IAPurchaseCallback(this.QuerySubscriptionListHandler);
			}

			// Token: 0x060051A5 RID: 20901 RVA: 0x001A4550 File Offset: 0x001A2750
			protected override void QuerySubscriptionListHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[QuerySubscriptionListHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				string text = "";
				List<IAPurchase.Subscription> list = null;
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[QuerySubscriptionListHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[QuerySubscriptionListHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						try
						{
							list = JsonMapper.ToObject<IAPurchase.QuerySubscritionResponse>(message).subscriptions;
						}
						catch (Exception ex3)
						{
							string text3 = "[QuerySubscriptionListHandler] ex =";
							Exception ex4 = ex3;
							Logger.Log(text3 + ((ex4 != null) ? ex4.ToString() : null));
						}
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0 && list != null && list.Count > 0)
						{
							IAPurchase.IAPHandler.listener.OnQuerySubscriptionListSuccess(list.ToArray());
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x060051A6 RID: 20902 RVA: 0x001A4678 File Offset: 0x001A2878
			public IAPurchaseCallback getCancelSubscriptionHandler()
			{
				return new IAPurchaseCallback(this.CancelSubscriptionHandler);
			}

			// Token: 0x060051A7 RID: 20903 RVA: 0x001A4688 File Offset: 0x001A2888
			protected override void CancelSubscriptionHandler(int code, [MarshalAs(20)] string message)
			{
				Logger.Log("[CancelSubscriptionHandler] message=" + message);
				JsonData jsonData = JsonMapper.ToObject(message);
				int num = -1;
				bool bCanceled = false;
				string text = "";
				if (code == 0)
				{
					try
					{
						num = (int)jsonData["statusCode"];
						text = (string)jsonData["message"];
					}
					catch (Exception ex)
					{
						string text2 = "[CancelSubscriptionHandler] statusCode, message ex=";
						Exception ex2 = ex;
						Logger.Log(text2 + ((ex2 != null) ? ex2.ToString() : null));
					}
					Logger.Log("[CancelSubscriptionHandler] statusCode =" + num.ToString() + ",errMessage=" + text);
					if (num == 0)
					{
						bCanceled = true;
						Logger.Log("[CancelSubscriptionHandler] isCanceled = " + bCanceled.ToString());
					}
				}
				if (IAPurchase.IAPHandler.listener != null)
				{
					if (code == 0)
					{
						if (num == 0)
						{
							IAPurchase.IAPHandler.listener.OnCancelSubscriptionSuccess(bCanceled);
							return;
						}
						IAPurchase.IAPHandler.listener.OnFailure(num, text);
						return;
					}
					else
					{
						IAPurchase.IAPHandler.listener.OnFailure(code, message);
					}
				}
			}

			// Token: 0x0400609D RID: 24733
			private static IAPurchase.IAPurchaseListener listener;
		}

		// Token: 0x02000D24 RID: 3364
		private abstract class BaseHandler
		{
			// Token: 0x060051A8 RID: 20904
			protected abstract void IsReadyHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051A9 RID: 20905
			protected abstract void RequestHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051AA RID: 20906
			protected abstract void PurchaseHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051AB RID: 20907
			protected abstract void QueryHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051AC RID: 20908
			protected abstract void QueryListHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051AD RID: 20909
			protected abstract void BalanceHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051AE RID: 20910
			protected abstract void RequestSubscriptionHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051AF RID: 20911
			protected abstract void RequestSubscriptionWithPlanIDHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051B0 RID: 20912
			protected abstract void SubscribeHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051B1 RID: 20913
			protected abstract void QuerySubscriptionHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051B2 RID: 20914
			protected abstract void QuerySubscriptionListHandler(int code, [MarshalAs(20)] string message);

			// Token: 0x060051B3 RID: 20915
			protected abstract void CancelSubscriptionHandler(int code, [MarshalAs(20)] string message);
		}

		// Token: 0x02000D25 RID: 3365
		public class IAPurchaseListener
		{
			// Token: 0x060051B5 RID: 20917 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnSuccess(string pchCurrencyName)
			{
			}

			// Token: 0x060051B6 RID: 20918 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnRequestSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x060051B7 RID: 20919 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnPurchaseSuccess(string pchPurchaseId)
			{
			}

			// Token: 0x060051B8 RID: 20920 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnQuerySuccess(IAPurchase.QueryResponse response)
			{
			}

			// Token: 0x060051B9 RID: 20921 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnQuerySuccess(IAPurchase.QueryListResponse response)
			{
			}

			// Token: 0x060051BA RID: 20922 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnBalanceSuccess(string pchBalance)
			{
			}

			// Token: 0x060051BB RID: 20923 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnFailure(int nCode, string pchMessage)
			{
			}

			// Token: 0x060051BC RID: 20924 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnRequestSubscriptionSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x060051BD RID: 20925 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnRequestSubscriptionWithPlanIDSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x060051BE RID: 20926 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnSubscribeSuccess(string pchSubscriptionId)
			{
			}

			// Token: 0x060051BF RID: 20927 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnQuerySubscriptionSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x060051C0 RID: 20928 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnQuerySubscriptionListSuccess(IAPurchase.Subscription[] subscriptionlist)
			{
			}

			// Token: 0x060051C1 RID: 20929 RVA: 0x00002789 File Offset: 0x00000989
			public virtual void OnCancelSubscriptionSuccess(bool bCanceled)
			{
			}
		}

		// Token: 0x02000D26 RID: 3366
		public class QueryResponse
		{
			// Token: 0x1700079B RID: 1947
			// (get) Token: 0x060051C3 RID: 20931 RVA: 0x001A4778 File Offset: 0x001A2978
			// (set) Token: 0x060051C4 RID: 20932 RVA: 0x001A4780 File Offset: 0x001A2980
			public string order_id { get; set; }

			// Token: 0x1700079C RID: 1948
			// (get) Token: 0x060051C5 RID: 20933 RVA: 0x001A4789 File Offset: 0x001A2989
			// (set) Token: 0x060051C6 RID: 20934 RVA: 0x001A4791 File Offset: 0x001A2991
			public string purchase_id { get; set; }

			// Token: 0x1700079D RID: 1949
			// (get) Token: 0x060051C7 RID: 20935 RVA: 0x001A479A File Offset: 0x001A299A
			// (set) Token: 0x060051C8 RID: 20936 RVA: 0x001A47A2 File Offset: 0x001A29A2
			public string status { get; set; }

			// Token: 0x1700079E RID: 1950
			// (get) Token: 0x060051C9 RID: 20937 RVA: 0x001A47AB File Offset: 0x001A29AB
			// (set) Token: 0x060051CA RID: 20938 RVA: 0x001A47B3 File Offset: 0x001A29B3
			public string price { get; set; }

			// Token: 0x1700079F RID: 1951
			// (get) Token: 0x060051CB RID: 20939 RVA: 0x001A47BC File Offset: 0x001A29BC
			// (set) Token: 0x060051CC RID: 20940 RVA: 0x001A47C4 File Offset: 0x001A29C4
			public string currency { get; set; }

			// Token: 0x170007A0 RID: 1952
			// (get) Token: 0x060051CD RID: 20941 RVA: 0x001A47CD File Offset: 0x001A29CD
			// (set) Token: 0x060051CE RID: 20942 RVA: 0x001A47D5 File Offset: 0x001A29D5
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000D27 RID: 3367
		public class QueryResponse2
		{
			// Token: 0x170007A1 RID: 1953
			// (get) Token: 0x060051D0 RID: 20944 RVA: 0x001A47DE File Offset: 0x001A29DE
			// (set) Token: 0x060051D1 RID: 20945 RVA: 0x001A47E6 File Offset: 0x001A29E6
			public string order_id { get; set; }

			// Token: 0x170007A2 RID: 1954
			// (get) Token: 0x060051D2 RID: 20946 RVA: 0x001A47EF File Offset: 0x001A29EF
			// (set) Token: 0x060051D3 RID: 20947 RVA: 0x001A47F7 File Offset: 0x001A29F7
			public string app_id { get; set; }

			// Token: 0x170007A3 RID: 1955
			// (get) Token: 0x060051D4 RID: 20948 RVA: 0x001A4800 File Offset: 0x001A2A00
			// (set) Token: 0x060051D5 RID: 20949 RVA: 0x001A4808 File Offset: 0x001A2A08
			public string purchase_id { get; set; }

			// Token: 0x170007A4 RID: 1956
			// (get) Token: 0x060051D6 RID: 20950 RVA: 0x001A4811 File Offset: 0x001A2A11
			// (set) Token: 0x060051D7 RID: 20951 RVA: 0x001A4819 File Offset: 0x001A2A19
			public string user_data { get; set; }

			// Token: 0x170007A5 RID: 1957
			// (get) Token: 0x060051D8 RID: 20952 RVA: 0x001A4822 File Offset: 0x001A2A22
			// (set) Token: 0x060051D9 RID: 20953 RVA: 0x001A482A File Offset: 0x001A2A2A
			public string price { get; set; }

			// Token: 0x170007A6 RID: 1958
			// (get) Token: 0x060051DA RID: 20954 RVA: 0x001A4833 File Offset: 0x001A2A33
			// (set) Token: 0x060051DB RID: 20955 RVA: 0x001A483B File Offset: 0x001A2A3B
			public string currency { get; set; }

			// Token: 0x170007A7 RID: 1959
			// (get) Token: 0x060051DC RID: 20956 RVA: 0x001A4844 File Offset: 0x001A2A44
			// (set) Token: 0x060051DD RID: 20957 RVA: 0x001A484C File Offset: 0x001A2A4C
			public long paid_timestamp { get; set; }
		}

		// Token: 0x02000D28 RID: 3368
		public class QueryListResponse
		{
			// Token: 0x170007A8 RID: 1960
			// (get) Token: 0x060051DF RID: 20959 RVA: 0x001A4855 File Offset: 0x001A2A55
			// (set) Token: 0x060051E0 RID: 20960 RVA: 0x001A485D File Offset: 0x001A2A5D
			public int total { get; set; }

			// Token: 0x170007A9 RID: 1961
			// (get) Token: 0x060051E1 RID: 20961 RVA: 0x001A4866 File Offset: 0x001A2A66
			// (set) Token: 0x060051E2 RID: 20962 RVA: 0x001A486E File Offset: 0x001A2A6E
			public int from { get; set; }

			// Token: 0x170007AA RID: 1962
			// (get) Token: 0x060051E3 RID: 20963 RVA: 0x001A4877 File Offset: 0x001A2A77
			// (set) Token: 0x060051E4 RID: 20964 RVA: 0x001A487F File Offset: 0x001A2A7F
			public int to { get; set; }

			// Token: 0x040060AE RID: 24750
			public List<IAPurchase.QueryResponse2> purchaseList;
		}

		// Token: 0x02000D29 RID: 3369
		public class StatusDetailTransaction
		{
			// Token: 0x170007AB RID: 1963
			// (get) Token: 0x060051E6 RID: 20966 RVA: 0x001A4888 File Offset: 0x001A2A88
			// (set) Token: 0x060051E7 RID: 20967 RVA: 0x001A4890 File Offset: 0x001A2A90
			public long create_time { get; set; }

			// Token: 0x170007AC RID: 1964
			// (get) Token: 0x060051E8 RID: 20968 RVA: 0x001A4899 File Offset: 0x001A2A99
			// (set) Token: 0x060051E9 RID: 20969 RVA: 0x001A48A1 File Offset: 0x001A2AA1
			public string payment_method { get; set; }

			// Token: 0x170007AD RID: 1965
			// (get) Token: 0x060051EA RID: 20970 RVA: 0x001A48AA File Offset: 0x001A2AAA
			// (set) Token: 0x060051EB RID: 20971 RVA: 0x001A48B2 File Offset: 0x001A2AB2
			public string status { get; set; }
		}

		// Token: 0x02000D2A RID: 3370
		public class StatusDetail
		{
			// Token: 0x170007AE RID: 1966
			// (get) Token: 0x060051ED RID: 20973 RVA: 0x001A48BB File Offset: 0x001A2ABB
			// (set) Token: 0x060051EE RID: 20974 RVA: 0x001A48C3 File Offset: 0x001A2AC3
			public long date_next_charge { get; set; }

			// Token: 0x170007AF RID: 1967
			// (get) Token: 0x060051EF RID: 20975 RVA: 0x001A48CC File Offset: 0x001A2ACC
			// (set) Token: 0x060051F0 RID: 20976 RVA: 0x001A48D4 File Offset: 0x001A2AD4
			public IAPurchase.StatusDetailTransaction[] transactions { get; set; }

			// Token: 0x170007B0 RID: 1968
			// (get) Token: 0x060051F1 RID: 20977 RVA: 0x001A48DD File Offset: 0x001A2ADD
			// (set) Token: 0x060051F2 RID: 20978 RVA: 0x001A48E5 File Offset: 0x001A2AE5
			public string cancel_reason { get; set; }
		}

		// Token: 0x02000D2B RID: 3371
		public class TimePeriod
		{
			// Token: 0x170007B1 RID: 1969
			// (get) Token: 0x060051F4 RID: 20980 RVA: 0x001A48EE File Offset: 0x001A2AEE
			// (set) Token: 0x060051F5 RID: 20981 RVA: 0x001A48F6 File Offset: 0x001A2AF6
			public string time_type { get; set; }

			// Token: 0x170007B2 RID: 1970
			// (get) Token: 0x060051F6 RID: 20982 RVA: 0x001A48FF File Offset: 0x001A2AFF
			// (set) Token: 0x060051F7 RID: 20983 RVA: 0x001A4907 File Offset: 0x001A2B07
			public int value { get; set; }
		}

		// Token: 0x02000D2C RID: 3372
		public class Subscription
		{
			// Token: 0x170007B3 RID: 1971
			// (get) Token: 0x060051F9 RID: 20985 RVA: 0x001A4910 File Offset: 0x001A2B10
			// (set) Token: 0x060051FA RID: 20986 RVA: 0x001A4918 File Offset: 0x001A2B18
			public string app_id { get; set; }

			// Token: 0x170007B4 RID: 1972
			// (get) Token: 0x060051FB RID: 20987 RVA: 0x001A4921 File Offset: 0x001A2B21
			// (set) Token: 0x060051FC RID: 20988 RVA: 0x001A4929 File Offset: 0x001A2B29
			public string order_id { get; set; }

			// Token: 0x170007B5 RID: 1973
			// (get) Token: 0x060051FD RID: 20989 RVA: 0x001A4932 File Offset: 0x001A2B32
			// (set) Token: 0x060051FE RID: 20990 RVA: 0x001A493A File Offset: 0x001A2B3A
			public string subscription_id { get; set; }

			// Token: 0x170007B6 RID: 1974
			// (get) Token: 0x060051FF RID: 20991 RVA: 0x001A4943 File Offset: 0x001A2B43
			// (set) Token: 0x06005200 RID: 20992 RVA: 0x001A494B File Offset: 0x001A2B4B
			public string price { get; set; }

			// Token: 0x170007B7 RID: 1975
			// (get) Token: 0x06005201 RID: 20993 RVA: 0x001A4954 File Offset: 0x001A2B54
			// (set) Token: 0x06005202 RID: 20994 RVA: 0x001A495C File Offset: 0x001A2B5C
			public string currency { get; set; }

			// Token: 0x170007B8 RID: 1976
			// (get) Token: 0x06005203 RID: 20995 RVA: 0x001A4965 File Offset: 0x001A2B65
			// (set) Token: 0x06005204 RID: 20996 RVA: 0x001A496D File Offset: 0x001A2B6D
			public long subscribed_timestamp { get; set; }

			// Token: 0x170007B9 RID: 1977
			// (get) Token: 0x06005205 RID: 20997 RVA: 0x001A4976 File Offset: 0x001A2B76
			// (set) Token: 0x06005206 RID: 20998 RVA: 0x001A497E File Offset: 0x001A2B7E
			public IAPurchase.TimePeriod free_trial_period { get; set; }

			// Token: 0x170007BA RID: 1978
			// (get) Token: 0x06005207 RID: 20999 RVA: 0x001A4987 File Offset: 0x001A2B87
			// (set) Token: 0x06005208 RID: 21000 RVA: 0x001A498F File Offset: 0x001A2B8F
			public IAPurchase.TimePeriod charge_period { get; set; }

			// Token: 0x170007BB RID: 1979
			// (get) Token: 0x06005209 RID: 21001 RVA: 0x001A4998 File Offset: 0x001A2B98
			// (set) Token: 0x0600520A RID: 21002 RVA: 0x001A49A0 File Offset: 0x001A2BA0
			public int number_of_charge_period { get; set; }

			// Token: 0x170007BC RID: 1980
			// (get) Token: 0x0600520B RID: 21003 RVA: 0x001A49A9 File Offset: 0x001A2BA9
			// (set) Token: 0x0600520C RID: 21004 RVA: 0x001A49B1 File Offset: 0x001A2BB1
			public string plan_id { get; set; }

			// Token: 0x170007BD RID: 1981
			// (get) Token: 0x0600520D RID: 21005 RVA: 0x001A49BA File Offset: 0x001A2BBA
			// (set) Token: 0x0600520E RID: 21006 RVA: 0x001A49C2 File Offset: 0x001A2BC2
			public string plan_name { get; set; }

			// Token: 0x170007BE RID: 1982
			// (get) Token: 0x0600520F RID: 21007 RVA: 0x001A49CB File Offset: 0x001A2BCB
			// (set) Token: 0x06005210 RID: 21008 RVA: 0x001A49D3 File Offset: 0x001A2BD3
			public string status { get; set; }

			// Token: 0x170007BF RID: 1983
			// (get) Token: 0x06005211 RID: 21009 RVA: 0x001A49DC File Offset: 0x001A2BDC
			// (set) Token: 0x06005212 RID: 21010 RVA: 0x001A49E4 File Offset: 0x001A2BE4
			public IAPurchase.StatusDetail status_detail { get; set; }
		}

		// Token: 0x02000D2D RID: 3373
		public class QuerySubscritionResponse
		{
			// Token: 0x170007C0 RID: 1984
			// (get) Token: 0x06005214 RID: 21012 RVA: 0x001A49ED File Offset: 0x001A2BED
			// (set) Token: 0x06005215 RID: 21013 RVA: 0x001A49F5 File Offset: 0x001A2BF5
			public int statusCode { get; set; }

			// Token: 0x170007C1 RID: 1985
			// (get) Token: 0x06005216 RID: 21014 RVA: 0x001A49FE File Offset: 0x001A2BFE
			// (set) Token: 0x06005217 RID: 21015 RVA: 0x001A4A06 File Offset: 0x001A2C06
			public string message { get; set; }

			// Token: 0x170007C2 RID: 1986
			// (get) Token: 0x06005218 RID: 21016 RVA: 0x001A4A0F File Offset: 0x001A2C0F
			// (set) Token: 0x06005219 RID: 21017 RVA: 0x001A4A17 File Offset: 0x001A2C17
			public List<IAPurchase.Subscription> subscriptions { get; set; }
		}
	}
}
