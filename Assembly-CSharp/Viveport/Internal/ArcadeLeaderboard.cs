using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D41 RID: 3393
	internal class ArcadeLeaderboard
	{
		// Token: 0x0600524A RID: 21066 RVA: 0x001A5083 File Offset: 0x001A3283
		static ArcadeLeaderboard()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x0600524B RID: 21067
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_IsReady")]
		internal static extern void IsReady(StatusCallback IsReadyCallback);

		// Token: 0x0600524C RID: 21068
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_IsReady")]
		internal static extern void IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x0600524D RID: 21069
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_DownloadLeaderboardScores")]
		internal static extern void DownloadLeaderboardScores(StatusCallback downloadLeaderboardScoresCB, string pchLeaderboardName, ELeaderboardDataTimeRange eLeaderboardDataTimeRange, int nCount);

		// Token: 0x0600524E RID: 21070
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_DownloadLeaderboardScores")]
		internal static extern void DownloadLeaderboardScores_64(StatusCallback downloadLeaderboardScoresCB, string pchLeaderboardName, ELeaderboardDataTimeRange eLeaderboardDataTimeRange, int nCount);

		// Token: 0x0600524F RID: 21071
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_UploadLeaderboardScore")]
		internal static extern void UploadLeaderboardScore(StatusCallback uploadLeaderboardScoreCB, string pchLeaderboardName, string pchUserName, int nScore);

		// Token: 0x06005250 RID: 21072
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_UploadLeaderboardScore")]
		internal static extern void UploadLeaderboardScore_64(StatusCallback uploadLeaderboardScoreCB, string pchLeaderboardName, string pchUserName, int nScore);

		// Token: 0x06005251 RID: 21073
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScore")]
		internal static extern void GetLeaderboardScore(int index, ref LeaderboardEntry_t pLeaderboardEntry);

		// Token: 0x06005252 RID: 21074
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScore")]
		internal static extern void GetLeaderboardScore_64(int index, ref LeaderboardEntry_t pLeaderboardEntry);

		// Token: 0x06005253 RID: 21075
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScoreCount")]
		internal static extern int GetLeaderboardScoreCount();

		// Token: 0x06005254 RID: 21076
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScoreCount")]
		internal static extern int GetLeaderboardScoreCount_64();

		// Token: 0x06005255 RID: 21077
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserRank")]
		internal static extern int GetLeaderboardUserRank();

		// Token: 0x06005256 RID: 21078
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserRank")]
		internal static extern int GetLeaderboardUserRank_64();

		// Token: 0x06005257 RID: 21079
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserScore")]
		internal static extern int GetLeaderboardUserScore();

		// Token: 0x06005258 RID: 21080
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserScore")]
		internal static extern int GetLeaderboardUserScore_64();
	}
}
