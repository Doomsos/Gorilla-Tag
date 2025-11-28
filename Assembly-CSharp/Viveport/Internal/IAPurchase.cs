using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D45 RID: 3397
	internal class IAPurchase
	{
		// Token: 0x06005296 RID: 21142
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_IsReady")]
		public static extern void IsReady(IAPurchaseCallback callback, string pchAppKey);

		// Token: 0x06005297 RID: 21143
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_IsReady")]
		public static extern void IsReady_64(IAPurchaseCallback callback, string pchAppKey);

		// Token: 0x06005298 RID: 21144
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_Request")]
		public static extern void Request(IAPurchaseCallback callback, string pchPrice);

		// Token: 0x06005299 RID: 21145
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_Request")]
		public static extern void Request_64(IAPurchaseCallback callback, string pchPrice);

		// Token: 0x0600529A RID: 21146
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_RequestWithUserData")]
		public static extern void Request(IAPurchaseCallback callback, string pchPrice, string pchUserData);

		// Token: 0x0600529B RID: 21147
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_RequestWithUserData")]
		public static extern void Request_64(IAPurchaseCallback callback, string pchPrice, string pchUserData);

		// Token: 0x0600529C RID: 21148
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_Purchase")]
		public static extern void Purchase(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600529D RID: 21149
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_Purchase")]
		public static extern void Purchase_64(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600529E RID: 21150
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_Query")]
		public static extern void Query(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x0600529F RID: 21151
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_Query")]
		public static extern void Query_64(IAPurchaseCallback callback, string pchPurchaseId);

		// Token: 0x060052A0 RID: 21152
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_QueryList")]
		public static extern void Query(IAPurchaseCallback callback);

		// Token: 0x060052A1 RID: 21153
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_QueryList")]
		public static extern void Query_64(IAPurchaseCallback callback);

		// Token: 0x060052A2 RID: 21154
		[DllImport("viveport_api", CallingConvention = 2, EntryPoint = "IViveportIAPurchase_GetBalance")]
		public static extern void GetBalance(IAPurchaseCallback callback);

		// Token: 0x060052A3 RID: 21155
		[DllImport("viveport_api64", CallingConvention = 2, EntryPoint = "IViveportIAPurchase_GetBalance")]
		public static extern void GetBalance_64(IAPurchaseCallback callback);

		// Token: 0x060052A4 RID: 21156
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_RequestSubscription")]
		public static extern void RequestSubscription(IAPurchaseCallback callback, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId);

		// Token: 0x060052A5 RID: 21157
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_RequestSubscription")]
		public static extern void RequestSubscription_64(IAPurchaseCallback callback, string pchPrice, string pchFreeTrialType, int nFreeTrialValue, string pchChargePeriodType, int nChargePeriodValue, int nNumberOfChargePeriod, string pchPlanId);

		// Token: 0x060052A6 RID: 21158
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_RequestSubscriptionWithPlanID")]
		public static extern void RequestSubscriptionWithPlanID(IAPurchaseCallback callback, string pchPlanId);

		// Token: 0x060052A7 RID: 21159
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_RequestSubscriptionWithPlanID")]
		public static extern void RequestSubscriptionWithPlanID_64(IAPurchaseCallback callback, string pchPlanId);

		// Token: 0x060052A8 RID: 21160
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_Subscribe")]
		public static extern void Subscribe(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x060052A9 RID: 21161
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_Subscribe")]
		public static extern void Subscribe_64(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x060052AA RID: 21162
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_QuerySubscription")]
		public static extern void QuerySubscription(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x060052AB RID: 21163
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_QuerySubscription")]
		public static extern void QuerySubscription_64(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x060052AC RID: 21164
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_QuerySubscriptionList")]
		public static extern void QuerySubscriptionList(IAPurchaseCallback callback);

		// Token: 0x060052AD RID: 21165
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_QuerySubscriptionList")]
		public static extern void QuerySubscriptionList_64(IAPurchaseCallback callback);

		// Token: 0x060052AE RID: 21166
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_CancelSubscription")]
		public static extern void CancelSubscription(IAPurchaseCallback callback, string pchSubscriptionId);

		// Token: 0x060052AF RID: 21167
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportIAPurchase_CancelSubscription")]
		public static extern void CancelSubscription_64(IAPurchaseCallback callback, string pchSubscriptionId);
	}
}
