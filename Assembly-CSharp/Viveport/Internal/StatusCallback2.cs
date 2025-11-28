using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	[UnmanagedFunctionPointer(2)]
	internal delegate void StatusCallback2(int nResult, [MarshalAs(20)] string message);
}
