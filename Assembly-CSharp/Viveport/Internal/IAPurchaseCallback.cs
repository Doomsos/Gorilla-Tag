using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D3F RID: 3391
	// (Invoke) Token: 0x06005247 RID: 21063
	[UnmanagedFunctionPointer(2)]
	internal delegate void IAPurchaseCallback(int code, [MarshalAs(20)] string message);
}
