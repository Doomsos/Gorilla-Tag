using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D34 RID: 3380
	// (Invoke) Token: 0x0600523F RID: 21055
	[UnmanagedFunctionPointer(2)]
	internal delegate void StatusCallback2(int nResult, [MarshalAs(20)] string message);
}
