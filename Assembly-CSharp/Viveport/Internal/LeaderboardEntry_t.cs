using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	internal struct LeaderboardEntry_t
	{
		internal int m_nGlobalRank;

		internal int m_nScore;

		[MarshalAs(23, SizeConst = 64)]
		internal string m_pUserName;
	}
}
