using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal.Arcade
{
	// Token: 0x02000D4B RID: 3403
	internal class Session
	{
		// Token: 0x060052DD RID: 21213
		[DllImport("viveport_api", CallingConvention = 2, EntryPoint = "IViveportArcadeSession_IsReady")]
		internal static extern void IsReady(SessionCallback callback);

		// Token: 0x060052DE RID: 21214
		[DllImport("viveport_api64", CallingConvention = 2, EntryPoint = "IViveportArcadeSession_IsReady")]
		internal static extern void IsReady_64(SessionCallback callback);

		// Token: 0x060052DF RID: 21215
		[DllImport("viveport_api", CallingConvention = 2, EntryPoint = "IViveportArcadeSession_Start")]
		internal static extern void Start(SessionCallback callback);

		// Token: 0x060052E0 RID: 21216
		[DllImport("viveport_api64", CallingConvention = 2, EntryPoint = "IViveportArcadeSession_Start")]
		internal static extern void Start_64(SessionCallback callback);

		// Token: 0x060052E1 RID: 21217
		[DllImport("viveport_api", CallingConvention = 2, EntryPoint = "IViveportArcadeSession_Stop")]
		internal static extern void Stop(SessionCallback callback);

		// Token: 0x060052E2 RID: 21218
		[DllImport("viveport_api64", CallingConvention = 2, EntryPoint = "IViveportArcadeSession_Stop")]
		internal static extern void Stop_64(SessionCallback callback);
	}
}
