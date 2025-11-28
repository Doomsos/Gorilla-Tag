using System;

namespace Utilities
{
	// Token: 0x02000D80 RID: 3456
	public static class PathUtils
	{
		// Token: 0x060054BA RID: 21690 RVA: 0x001AB7B4 File Offset: 0x001A99B4
		public static string Resolve(params string[] subPaths)
		{
			if (subPaths == null || subPaths.Length == 0)
			{
				return null;
			}
			string[] array = string.Concat(subPaths).Split(PathUtils.kPathSeps, 1);
			return Uri.UnescapeDataString(new Uri(string.Join("/", array)).AbsolutePath);
		}

		// Token: 0x040061D5 RID: 25045
		private static readonly char[] kPathSeps = new char[]
		{
			'\\',
			'/'
		};

		// Token: 0x040061D6 RID: 25046
		private const string kFwdSlash = "/";
	}
}
