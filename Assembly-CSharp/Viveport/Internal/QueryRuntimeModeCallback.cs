using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D35 RID: 3381
	// (Invoke) Token: 0x06005243 RID: 21059
	[UnmanagedFunctionPointer(2)]
	internal delegate void QueryRuntimeModeCallback(int nResult, int nMode);
}
