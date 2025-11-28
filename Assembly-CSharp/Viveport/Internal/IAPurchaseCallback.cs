using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	[UnmanagedFunctionPointer(2)]
	internal delegate void IAPurchaseCallback(int code, [MarshalAs(20)] string message);
}
