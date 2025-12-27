using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal.Arcade
{
	[UnmanagedFunctionPointer(2)]
	internal delegate void SessionCallback(int code, [MarshalAs(20)] string message);
}
