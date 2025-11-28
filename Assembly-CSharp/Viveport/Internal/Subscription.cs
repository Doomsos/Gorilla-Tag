using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D47 RID: 3399
	internal class Subscription
	{
		// Token: 0x060052BB RID: 21179 RVA: 0x001A5063 File Offset: 0x001A3263
		static Subscription()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x060052BC RID: 21180
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportSubscription_IsReady")]
		internal static extern void IsReady(StatusCallback2 IsReadyCallback);

		// Token: 0x060052BD RID: 21181
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportSubscription_IsReady")]
		internal static extern void IsReady_64(StatusCallback2 IsReadyCallback);

		// Token: 0x060052BE RID: 21182
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportSubscription_IsWindowsSubscriber")]
		internal static extern bool IsWindowsSubscriber();

		// Token: 0x060052BF RID: 21183
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportSubscription_IsWindowsSubscriber")]
		internal static extern bool IsWindowsSubscriber_64();

		// Token: 0x060052C0 RID: 21184
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportSubscription_IsAndroidSubscriber")]
		internal static extern bool IsAndroidSubscriber();

		// Token: 0x060052C1 RID: 21185
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportSubscription_IsAndroidSubscriber")]
		internal static extern bool IsAndroidSubscriber_64();

		// Token: 0x060052C2 RID: 21186
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportSubscription_GetTransactionType")]
		internal static extern ESubscriptionTransactionType GetTransactionType();

		// Token: 0x060052C3 RID: 21187
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportSubscription_GetTransactionType")]
		internal static extern ESubscriptionTransactionType GetTransactionType_64();
	}
}
