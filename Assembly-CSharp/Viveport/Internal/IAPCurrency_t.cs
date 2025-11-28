using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	internal struct IAPCurrency_t
	{
		[MarshalAs(23, SizeConst = 16)]
		internal string m_pName;

		[MarshalAs(23, SizeConst = 16)]
		internal string m_pSymbol;
	}
}
