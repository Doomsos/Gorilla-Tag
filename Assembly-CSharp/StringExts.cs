using System;

// Token: 0x020009F6 RID: 2550
public static class StringExts
{
	// Token: 0x06004133 RID: 16691 RVA: 0x0015B4ED File Offset: 0x001596ED
	public static string EscapeCsv(this string field)
	{
		if (StringExts._escapeChars == null)
		{
			StringExts._escapeChars = new char[]
			{
				',',
				'"',
				'\n',
				'\r'
			};
		}
		if (field.IndexOfAny(StringExts._escapeChars) != -1)
		{
			return field.Replace("\"", "\"\"");
		}
		return field;
	}

	// Token: 0x04005221 RID: 21025
	private static char[] _escapeChars;
}
