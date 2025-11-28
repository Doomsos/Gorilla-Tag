using System;
using UnityEngine;

// Token: 0x020003CD RID: 973
[Serializable]
public struct NetworkSystemConfig
{
	// Token: 0x1700025F RID: 607
	// (get) Token: 0x06001774 RID: 6004 RVA: 0x00080CB5 File Offset: 0x0007EEB5
	public static string AppVersion
	{
		get
		{
			return NetworkSystemConfig.prependCode + "." + NetworkSystemConfig.AppVersionStripped;
		}
	}

	// Token: 0x17000260 RID: 608
	// (get) Token: 0x06001775 RID: 6005 RVA: 0x00080CCC File Offset: 0x0007EECC
	public static string AppVersionStripped
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.gameVersionType,
				".",
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x17000261 RID: 609
	// (get) Token: 0x06001776 RID: 6006 RVA: 0x00080D2C File Offset: 0x0007EF2C
	public static string BundleVersion
	{
		get
		{
			return string.Concat(new string[]
			{
				NetworkSystemConfig.majorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion.ToString(),
				".",
				NetworkSystemConfig.minorVersion2.ToString()
			});
		}
	}

	// Token: 0x17000262 RID: 610
	// (get) Token: 0x06001777 RID: 6007 RVA: 0x00080D7B File Offset: 0x0007EF7B
	public static string GameVersionType
	{
		get
		{
			return NetworkSystemConfig.gameVersionType;
		}
	}

	// Token: 0x17000263 RID: 611
	// (get) Token: 0x06001778 RID: 6008 RVA: 0x00080D82 File Offset: 0x0007EF82
	public static int GameMajorVersion
	{
		get
		{
			return NetworkSystemConfig.majorVersion;
		}
	}

	// Token: 0x17000264 RID: 612
	// (get) Token: 0x06001779 RID: 6009 RVA: 0x00080D89 File Offset: 0x0007EF89
	public static int GameMinorVersion
	{
		get
		{
			return NetworkSystemConfig.minorVersion;
		}
	}

	// Token: 0x17000265 RID: 613
	// (get) Token: 0x0600177A RID: 6010 RVA: 0x00080D90 File Offset: 0x0007EF90
	public static int GameMinorVersion2
	{
		get
		{
			return NetworkSystemConfig.minorVersion2;
		}
	}

	// Token: 0x04002152 RID: 8530
	[HideInInspector]
	public int MaxPlayerCount;

	// Token: 0x04002153 RID: 8531
	private static string gameVersionType = "live1";

	// Token: 0x04002154 RID: 8532
	public static string prependCode = "prependblackfriday";

	// Token: 0x04002155 RID: 8533
	public static int majorVersion = 1;

	// Token: 0x04002156 RID: 8534
	public static int minorVersion = 1;

	// Token: 0x04002157 RID: 8535
	public static int minorVersion2 = 125;
}
