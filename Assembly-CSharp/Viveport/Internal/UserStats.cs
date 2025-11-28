using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000D44 RID: 3396
	internal class UserStats
	{
		// Token: 0x06005272 RID: 21106 RVA: 0x001A5063 File Offset: 0x001A3263
		static UserStats()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06005273 RID: 21107
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_IsReady")]
		internal static extern int IsReady(StatusCallback IsReadyCallback);

		// Token: 0x06005274 RID: 21108
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_IsReady")]
		internal static extern int IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x06005275 RID: 21109
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_DownloadStats")]
		internal static extern int DownloadStats(StatusCallback downloadStatsCallback);

		// Token: 0x06005276 RID: 21110
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_DownloadStats")]
		internal static extern int DownloadStats_64(StatusCallback downloadStatsCallback);

		// Token: 0x06005277 RID: 21111
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetStat0")]
		internal static extern int GetStat(string pchName, ref int pnData);

		// Token: 0x06005278 RID: 21112
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetStat0")]
		internal static extern int GetStat_64(string pchName, ref int pnData);

		// Token: 0x06005279 RID: 21113
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetStat")]
		internal static extern int GetStat(string pchName, ref float pfData);

		// Token: 0x0600527A RID: 21114
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetStat")]
		internal static extern int GetStat_64(string pchName, ref float pfData);

		// Token: 0x0600527B RID: 21115
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_SetStat0")]
		internal static extern int SetStat(string pchName, int nData);

		// Token: 0x0600527C RID: 21116
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_SetStat0")]
		internal static extern int SetStat_64(string pchName, int nData);

		// Token: 0x0600527D RID: 21117
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_SetStat")]
		internal static extern int SetStat(string pchName, float fData);

		// Token: 0x0600527E RID: 21118
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_SetStat")]
		internal static extern int SetStat_64(string pchName, float fData);

		// Token: 0x0600527F RID: 21119
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_UploadStats")]
		internal static extern int UploadStats(StatusCallback uploadStatsCallback);

		// Token: 0x06005280 RID: 21120
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_UploadStats")]
		internal static extern int UploadStats_64(StatusCallback uploadStatsCallback);

		// Token: 0x06005281 RID: 21121
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetAchievement")]
		internal static extern int GetAchievement(string pchName, ref int pbAchieved);

		// Token: 0x06005282 RID: 21122
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetAchievement")]
		internal static extern int GetAchievement_64(string pchName, ref int pbAchieved);

		// Token: 0x06005283 RID: 21123
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetAchievementUnlockTime")]
		internal static extern int GetAchievementUnlockTime(string pchName, ref int punUnlockTime);

		// Token: 0x06005284 RID: 21124
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetAchievementUnlockTime")]
		internal static extern int GetAchievementUnlockTime_64(string pchName, ref int punUnlockTime);

		// Token: 0x06005285 RID: 21125
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_SetAchievement")]
		internal static extern int SetAchievement(string pchName);

		// Token: 0x06005286 RID: 21126
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_SetAchievement")]
		internal static extern int SetAchievement_64(string pchName);

		// Token: 0x06005287 RID: 21127
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_ClearAchievement")]
		internal static extern int ClearAchievement(string pchName);

		// Token: 0x06005288 RID: 21128
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_ClearAchievement")]
		internal static extern int ClearAchievement_64(string pchName);

		// Token: 0x06005289 RID: 21129
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_DownloadLeaderboardScores")]
		internal static extern int DownloadLeaderboardScores(StatusCallback downloadLeaderboardScoresCB, string pchLeaderboardName, ELeaderboardDataRequest eLeaderboardDataRequest, ELeaderboardDataTimeRange eLeaderboardDataTimeRange, int nRangeStart, int nRangeEnd);

		// Token: 0x0600528A RID: 21130
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_DownloadLeaderboardScores")]
		internal static extern int DownloadLeaderboardScores_64(StatusCallback downloadLeaderboardScoresCB, string pchLeaderboardName, ELeaderboardDataRequest eLeaderboardDataRequest, ELeaderboardDataTimeRange eLeaderboardDataTimeRange, int nRangeStart, int nRangeEnd);

		// Token: 0x0600528B RID: 21131
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_UploadLeaderboardScore")]
		internal static extern int UploadLeaderboardScore(StatusCallback uploadLeaderboardScoreCB, string pchLeaderboardName, int nScore);

		// Token: 0x0600528C RID: 21132
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_UploadLeaderboardScore")]
		internal static extern int UploadLeaderboardScore_64(StatusCallback uploadLeaderboardScoreCB, string pchLeaderboardName, int nScore);

		// Token: 0x0600528D RID: 21133
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetLeaderboardScore")]
		internal static extern int GetLeaderboardScore(int index, ref LeaderboardEntry_t pLeaderboardEntry);

		// Token: 0x0600528E RID: 21134
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetLeaderboardScore")]
		internal static extern int GetLeaderboardScore_64(int index, ref LeaderboardEntry_t pLeaderboardEntry);

		// Token: 0x0600528F RID: 21135
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetLeaderboardScoreCount")]
		internal static extern int GetLeaderboardScoreCount();

		// Token: 0x06005290 RID: 21136
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetLeaderboardScoreCount")]
		internal static extern int GetLeaderboardScoreCount_64();

		// Token: 0x06005291 RID: 21137
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetLeaderboardSortMethod")]
		internal static extern ELeaderboardSortMethod GetLeaderboardSortMethod();

		// Token: 0x06005292 RID: 21138
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetLeaderboardSortMethod")]
		internal static extern ELeaderboardSortMethod GetLeaderboardSortMethod_64();

		// Token: 0x06005293 RID: 21139
		[DllImport("viveport_api", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetLeaderboardDisplayType")]
		internal static extern ELeaderboardDisplayType GetLeaderboardDisplayType();

		// Token: 0x06005294 RID: 21140
		[DllImport("viveport_api64", CallingConvention = 2, CharSet = 2, EntryPoint = "IViveportUserStats_GetLeaderboardDisplayType")]
		internal static extern ELeaderboardDisplayType GetLeaderboardDisplayType_64();
	}
}
