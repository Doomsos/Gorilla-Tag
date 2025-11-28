using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D3E RID: 3390
	internal struct LeaderboardEntry_t
	{
		// Token: 0x040060FB RID: 24827
		internal int m_nGlobalRank;

		// Token: 0x040060FC RID: 24828
		internal int m_nScore;

		// Token: 0x040060FD RID: 24829
		[MarshalAs(23, SizeConst = 64)]
		internal string m_pUserName;
	}
}
