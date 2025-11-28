using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000D19 RID: 3353
	public class UserStats
	{
		// Token: 0x0600514F RID: 20815 RVA: 0x001A2A28 File Offset: 0x001A0C28
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			UserStats.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x06005150 RID: 20816 RVA: 0x001A2A38 File Offset: 0x001A0C38
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.IsReady_64(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			}
			return UserStats.IsReady(new StatusCallback(UserStats.IsReadyIl2cppCallback));
		}

		// Token: 0x06005151 RID: 20817 RVA: 0x001A2AA5 File Offset: 0x001A0CA5
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadStatsIl2cppCallback(int errorCode)
		{
			UserStats.downloadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06005152 RID: 20818 RVA: 0x001A2AB4 File Offset: 0x001A0CB4
		public static int DownloadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadStats_64(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			}
			return UserStats.DownloadStats(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
		}

		// Token: 0x06005153 RID: 20819 RVA: 0x001A2B24 File Offset: 0x001A0D24
		public static int GetStat(string name, int defaultValue)
		{
			int result = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref result);
			}
			else
			{
				UserStats.GetStat(name, ref result);
			}
			return result;
		}

		// Token: 0x06005154 RID: 20820 RVA: 0x001A2B50 File Offset: 0x001A0D50
		public static float GetStat(string name, float defaultValue)
		{
			float result = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref result);
			}
			else
			{
				UserStats.GetStat(name, ref result);
			}
			return result;
		}

		// Token: 0x06005155 RID: 20821 RVA: 0x001A2B7C File Offset: 0x001A0D7C
		public static void SetStat(string name, int value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06005156 RID: 20822 RVA: 0x001A2B97 File Offset: 0x001A0D97
		public static void SetStat(string name, float value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06005157 RID: 20823 RVA: 0x001A2BB2 File Offset: 0x001A0DB2
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadStatsIl2cppCallback(int errorCode)
		{
			UserStats.uploadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06005158 RID: 20824 RVA: 0x001A2BC0 File Offset: 0x001A0DC0
		public static int UploadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadStats_64(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			}
			return UserStats.UploadStats(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
		}

		// Token: 0x06005159 RID: 20825 RVA: 0x001A2C30 File Offset: 0x001A0E30
		public static bool GetAchievement(string pchName)
		{
			int num = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievement_64(pchName, ref num);
			}
			else
			{
				UserStats.GetAchievement(pchName, ref num);
			}
			return num == 1;
		}

		// Token: 0x0600515A RID: 20826 RVA: 0x001A2C60 File Offset: 0x001A0E60
		public static int GetAchievementUnlockTime(string pchName)
		{
			int result = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievementUnlockTime_64(pchName, ref result);
			}
			else
			{
				UserStats.GetAchievementUnlockTime(pchName, ref result);
			}
			return result;
		}

		// Token: 0x0600515B RID: 20827 RVA: 0x00147A4B File Offset: 0x00145C4B
		public static string GetAchievementIcon(string pchName)
		{
			return "";
		}

		// Token: 0x0600515C RID: 20828 RVA: 0x00147A4B File Offset: 0x00145C4B
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr)
		{
			return "";
		}

		// Token: 0x0600515D RID: 20829 RVA: 0x00147A4B File Offset: 0x00145C4B
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr, Locale locale)
		{
			return "";
		}

		// Token: 0x0600515E RID: 20830 RVA: 0x001A2C8C File Offset: 0x001A0E8C
		public static int SetAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.SetAchievement_64(pchName);
			}
			return UserStats.SetAchievement(pchName);
		}

		// Token: 0x0600515F RID: 20831 RVA: 0x001A2CA3 File Offset: 0x001A0EA3
		public static int ClearAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.ClearAchievement_64(pchName);
			}
			return UserStats.ClearAchievement(pchName);
		}

		// Token: 0x06005160 RID: 20832 RVA: 0x001A2CBA File Offset: 0x001A0EBA
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadLeaderboardScoresIl2cppCallback(int errorCode)
		{
			UserStats.downloadLeaderboardScoresIl2cppCallback(errorCode);
		}

		// Token: 0x06005161 RID: 20833 RVA: 0x001A2CC8 File Offset: 0x001A0EC8
		public static int DownloadLeaderboardScores(StatusCallback callback, string pchLeaderboardName, UserStats.LeaderBoardRequestType eLeaderboardDataRequest, UserStats.LeaderBoardTimeRange eLeaderboardDataTimeRange, int nRangeStart, int nRangeEnd)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadLeaderboardScoresIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadLeaderboardScores_64(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
			}
			return UserStats.DownloadLeaderboardScores(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
		}

		// Token: 0x06005162 RID: 20834 RVA: 0x001A2D43 File Offset: 0x001A0F43
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadLeaderboardScoreIl2cppCallback(int errorCode)
		{
			UserStats.uploadLeaderboardScoreIl2cppCallback(errorCode);
		}

		// Token: 0x06005163 RID: 20835 RVA: 0x001A2D50 File Offset: 0x001A0F50
		public static int UploadLeaderboardScore(StatusCallback callback, string pchLeaderboardName, int nScore)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadLeaderboardScoreIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadLeaderboardScore_64(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
			}
			return UserStats.UploadLeaderboardScore(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
		}

		// Token: 0x06005164 RID: 20836 RVA: 0x001A2DC4 File Offset: 0x001A0FC4
		public static Leaderboard GetLeaderboardScore(int index)
		{
			LeaderboardEntry_t leaderboardEntry_t;
			leaderboardEntry_t.m_nGlobalRank = 0;
			leaderboardEntry_t.m_nScore = 0;
			leaderboardEntry_t.m_pUserName = "";
			if (IntPtr.Size == 8)
			{
				UserStats.GetLeaderboardScore_64(index, ref leaderboardEntry_t);
			}
			else
			{
				UserStats.GetLeaderboardScore(index, ref leaderboardEntry_t);
			}
			return new Leaderboard
			{
				Rank = leaderboardEntry_t.m_nGlobalRank,
				Score = leaderboardEntry_t.m_nScore,
				UserName = leaderboardEntry_t.m_pUserName
			};
		}

		// Token: 0x06005165 RID: 20837 RVA: 0x001A2E32 File Offset: 0x001A1032
		public static int GetLeaderboardScoreCount()
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.GetLeaderboardScoreCount_64();
			}
			return UserStats.GetLeaderboardScoreCount();
		}

		// Token: 0x06005166 RID: 20838 RVA: 0x001A2E47 File Offset: 0x001A1047
		public static UserStats.LeaderBoardSortMethod GetLeaderboardSortMethod()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod_64();
			}
			return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod();
		}

		// Token: 0x06005167 RID: 20839 RVA: 0x001A2E5C File Offset: 0x001A105C
		public static UserStats.LeaderBoardDiaplayType GetLeaderboardDisplayType()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType_64();
			}
			return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType();
		}

		// Token: 0x0400606B RID: 24683
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400606C RID: 24684
		private static StatusCallback downloadStatsIl2cppCallback;

		// Token: 0x0400606D RID: 24685
		private static StatusCallback uploadStatsIl2cppCallback;

		// Token: 0x0400606E RID: 24686
		private static StatusCallback downloadLeaderboardScoresIl2cppCallback;

		// Token: 0x0400606F RID: 24687
		private static StatusCallback uploadLeaderboardScoreIl2cppCallback;

		// Token: 0x02000D1A RID: 3354
		public enum LeaderBoardRequestType
		{
			// Token: 0x04006071 RID: 24689
			GlobalData,
			// Token: 0x04006072 RID: 24690
			GlobalDataAroundUser,
			// Token: 0x04006073 RID: 24691
			LocalData,
			// Token: 0x04006074 RID: 24692
			LocalDataAroundUser
		}

		// Token: 0x02000D1B RID: 3355
		public enum LeaderBoardTimeRange
		{
			// Token: 0x04006076 RID: 24694
			AllTime,
			// Token: 0x04006077 RID: 24695
			Daily,
			// Token: 0x04006078 RID: 24696
			Weekly,
			// Token: 0x04006079 RID: 24697
			Monthly
		}

		// Token: 0x02000D1C RID: 3356
		public enum LeaderBoardSortMethod
		{
			// Token: 0x0400607B RID: 24699
			None,
			// Token: 0x0400607C RID: 24700
			Ascending,
			// Token: 0x0400607D RID: 24701
			Descending
		}

		// Token: 0x02000D1D RID: 3357
		public enum LeaderBoardDiaplayType
		{
			// Token: 0x0400607F RID: 24703
			None,
			// Token: 0x04006080 RID: 24704
			Numeric,
			// Token: 0x04006081 RID: 24705
			TimeSeconds,
			// Token: 0x04006082 RID: 24706
			TimeMilliSeconds
		}

		// Token: 0x02000D1E RID: 3358
		public enum LeaderBoardScoreMethod
		{
			// Token: 0x04006084 RID: 24708
			None,
			// Token: 0x04006085 RID: 24709
			KeepBest,
			// Token: 0x04006086 RID: 24710
			ForceUpdate
		}

		// Token: 0x02000D1F RID: 3359
		public enum AchievementDisplayAttribute
		{
			// Token: 0x04006088 RID: 24712
			Name,
			// Token: 0x04006089 RID: 24713
			Desc,
			// Token: 0x0400608A RID: 24714
			Hidden
		}
	}
}
