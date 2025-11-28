using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Viveport.Internal
{
	// Token: 0x02000D46 RID: 3398
	internal class DLC
	{
		// Token: 0x060052B1 RID: 21169 RVA: 0x001A5063 File Offset: 0x001A3263
		static DLC()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x060052B2 RID: 21170
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDlc_IsReady")]
		internal static extern int IsReady(StatusCallback callback);

		// Token: 0x060052B3 RID: 21171
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDlc_IsReady")]
		internal static extern int IsReady_64(StatusCallback callback);

		// Token: 0x060052B4 RID: 21172
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDlc_GetCount")]
		internal static extern int GetCount();

		// Token: 0x060052B5 RID: 21173
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDlc_GetCount")]
		internal static extern int GetCount_64();

		// Token: 0x060052B6 RID: 21174
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDlc_GetIsAvailable")]
		internal static extern bool GetIsAvailable(int index, StringBuilder appId, out bool isAvailable);

		// Token: 0x060052B7 RID: 21175
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDlc_GetIsAvailable")]
		internal static extern bool GetIsAvailable_64(int index, StringBuilder appId, out bool isAvailable);

		// Token: 0x060052B8 RID: 21176
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDlc_IsSubscribed")]
		internal static extern int IsSubscribed();

		// Token: 0x060052B9 RID: 21177
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportDlc_IsSubscribed")]
		internal static extern int IsSubscribed_64();
	}
}
