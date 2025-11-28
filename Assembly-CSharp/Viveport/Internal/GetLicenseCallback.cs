using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D32 RID: 3378
	// (Invoke) Token: 0x06005237 RID: 21047
	[UnmanagedFunctionPointer(2)]
	internal delegate void GetLicenseCallback([MarshalAs(20)] string message, [MarshalAs(20)] string signature);
}
