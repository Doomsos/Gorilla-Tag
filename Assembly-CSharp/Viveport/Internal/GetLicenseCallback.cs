using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	[UnmanagedFunctionPointer(2)]
	internal delegate void GetLicenseCallback([MarshalAs(20)] string message, [MarshalAs(20)] string signature);
}
