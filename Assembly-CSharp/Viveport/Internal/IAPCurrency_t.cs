using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D40 RID: 3392
	internal struct IAPCurrency_t
	{
		// Token: 0x040060FE RID: 24830
		[MarshalAs(23, SizeConst = 16)]
		internal string m_pName;

		// Token: 0x040060FF RID: 24831
		[MarshalAs(23, SizeConst = 16)]
		internal string m_pSymbol;
	}
}
