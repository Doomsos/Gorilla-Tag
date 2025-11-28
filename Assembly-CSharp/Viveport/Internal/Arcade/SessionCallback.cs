using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal.Arcade
{
	// Token: 0x02000D4A RID: 3402
	// (Invoke) Token: 0x060052DA RID: 21210
	[UnmanagedFunctionPointer(2)]
	internal delegate void SessionCallback(int code, [MarshalAs(20)] string message);
}
