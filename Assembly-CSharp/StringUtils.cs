using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Cysharp.Text;
using UnityEngine;

// Token: 0x02000C86 RID: 3206
public static class StringUtils
{
	// Token: 0x06004E5F RID: 20063 RVA: 0x001964D1 File Offset: 0x001946D1
	[MethodImpl(256)]
	public static bool IsNullOrEmpty(this string s)
	{
		return string.IsNullOrEmpty(s);
	}

	// Token: 0x06004E60 RID: 20064 RVA: 0x001964D9 File Offset: 0x001946D9
	[MethodImpl(256)]
	public static bool IsNullOrWhiteSpace(this string s)
	{
		return string.IsNullOrWhiteSpace(s);
	}

	// Token: 0x06004E61 RID: 20065 RVA: 0x001964E4 File Offset: 0x001946E4
	public static string ToAlphaNumeric(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return string.Empty;
		}
		string result;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			for (int i = 0; i < s.Length; i++)
			{
				char c = s.get_Chars(i);
				if (char.IsLetterOrDigit(c))
				{
					utf16ValueStringBuilder.Append(c);
				}
			}
			result = utf16ValueStringBuilder.ToString();
		}
		return result;
	}

	// Token: 0x06004E62 RID: 20066 RVA: 0x00196560 File Offset: 0x00194760
	public static string Capitalize(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return s;
		}
		char[] array = s.ToCharArray();
		array[0] = char.ToUpperInvariant(array[0]);
		return new string(array);
	}

	// Token: 0x06004E63 RID: 20067 RVA: 0x0019658F File Offset: 0x0019478F
	public static string Concat(this IEnumerable<string> source)
	{
		return string.Concat(source);
	}

	// Token: 0x06004E64 RID: 20068 RVA: 0x00196597 File Offset: 0x00194797
	public static string Join(this IEnumerable<string> source, string separator)
	{
		return string.Join(separator, source);
	}

	// Token: 0x06004E65 RID: 20069 RVA: 0x001965A0 File Offset: 0x001947A0
	public static string Join(this IEnumerable<string> source, char separator)
	{
		return string.Join<string>(separator, source);
	}

	// Token: 0x06004E66 RID: 20070 RVA: 0x001965A9 File Offset: 0x001947A9
	public static string RemoveAll(this string s, string value, StringComparison mode = 5)
	{
		if (string.IsNullOrEmpty(s))
		{
			return s;
		}
		return s.Replace(value, string.Empty, mode);
	}

	// Token: 0x06004E67 RID: 20071 RVA: 0x001965C2 File Offset: 0x001947C2
	public static string RemoveAll(this string s, char value, StringComparison mode = 5)
	{
		return s.RemoveAll(value.ToString(), mode);
	}

	// Token: 0x06004E68 RID: 20072 RVA: 0x001965D2 File Offset: 0x001947D2
	public static byte[] ToBytesASCII(this string s)
	{
		return Encoding.ASCII.GetBytes(s);
	}

	// Token: 0x06004E69 RID: 20073 RVA: 0x001965DF File Offset: 0x001947DF
	public static byte[] ToBytesUTF8(this string s)
	{
		return Encoding.UTF8.GetBytes(s);
	}

	// Token: 0x06004E6A RID: 20074 RVA: 0x001965EC File Offset: 0x001947EC
	public static byte[] ToBytesUnicode(this string s)
	{
		return Encoding.Unicode.GetBytes(s);
	}

	// Token: 0x06004E6B RID: 20075 RVA: 0x001965FC File Offset: 0x001947FC
	public static string ComputeSHV2(this string s)
	{
		return Hash128.Compute(s).ToString();
	}

	// Token: 0x06004E6C RID: 20076 RVA: 0x0019661D File Offset: 0x0019481D
	public static string ToQueryString(this Dictionary<string, string> d)
	{
		if (d == null)
		{
			return null;
		}
		return "?" + string.Join("&", Enumerable.Select<KeyValuePair<string, string>, string>(d, (KeyValuePair<string, string> x) => x.Key + "=" + x.Value));
	}

	// Token: 0x06004E6D RID: 20077 RVA: 0x00196660 File Offset: 0x00194860
	public static string Combine(string separator, params string[] values)
	{
		if (values == null || values.Length == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = !string.IsNullOrEmpty(separator);
		for (int i = 0; i < values.Length; i++)
		{
			if (flag)
			{
				stringBuilder.Append(separator);
			}
			stringBuilder.Append(values);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06004E6E RID: 20078 RVA: 0x001966B0 File Offset: 0x001948B0
	public static string ToUpperCamelCase(this string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return string.Empty;
		}
		string[] array = Regex.Split(input, "[^A-Za-z0-9]+");
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Length > 0)
			{
				string[] array2 = array;
				int num = i;
				string text = char.ToUpper(array[i].get_Chars(0)).ToString();
				string text2;
				if (array[i].Length <= 1)
				{
					text2 = "";
				}
				else
				{
					string text3 = array[i];
					text2 = text3.Substring(1, text3.Length - 1).ToLower();
				}
				array2[num] = text + text2;
			}
		}
		return string.Join("", array);
	}

	// Token: 0x06004E6F RID: 20079 RVA: 0x00196744 File Offset: 0x00194944
	public static string ToUpperCaseFromCamelCase(this string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		input = input.Trim();
		string result;
		using (Utf16ValueStringBuilder utf16ValueStringBuilder = ZString.CreateStringBuilder())
		{
			bool flag = true;
			for (int i = 0; i < input.Length; i++)
			{
				char c = input.get_Chars(i);
				if (char.IsUpper(c) && !flag)
				{
					utf16ValueStringBuilder.Append(' ');
				}
				utf16ValueStringBuilder.Append(char.ToUpper(c));
				flag = char.IsUpper(c);
			}
			result = utf16ValueStringBuilder.ToString().Trim();
		}
		return result;
	}

	// Token: 0x06004E70 RID: 20080 RVA: 0x001967E4 File Offset: 0x001949E4
	public static string RemoveStart(this string s, string value, StringComparison comparison = 1)
	{
		if (string.IsNullOrEmpty(s) || !s.StartsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(value.Length);
	}

	// Token: 0x06004E71 RID: 20081 RVA: 0x00196806 File Offset: 0x00194A06
	public static string RemoveEnd(this string s, string value, StringComparison comparison = 1)
	{
		if (string.IsNullOrEmpty(s) || !s.EndsWith(value, comparison))
		{
			return s;
		}
		return s.Substring(0, s.Length - value.Length);
	}

	// Token: 0x06004E72 RID: 20082 RVA: 0x00196830 File Offset: 0x00194A30
	public static string RemoveBothEnds(this string s, string value, StringComparison comparison = 1)
	{
		return s.RemoveEnd(value, comparison).RemoveStart(value, comparison);
	}

	// Token: 0x06004E73 RID: 20083 RVA: 0x00196841 File Offset: 0x00194A41
	public static string TrailingSpace(this string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			Debug.LogError("[STRING::UTILS] Trying to add Space, but string is null or empty");
			return s;
		}
		if (s.get_Chars(s.Length - 1) == ' ')
		{
			return s;
		}
		return s + " ";
	}

	// Token: 0x04005D54 RID: 23892
	public const string kForwardSlash = "/";

	// Token: 0x04005D55 RID: 23893
	public const string kBackSlash = "/";

	// Token: 0x04005D56 RID: 23894
	public const string kBackTick = "`";

	// Token: 0x04005D57 RID: 23895
	public const string kMinusDash = "-";

	// Token: 0x04005D58 RID: 23896
	public const string kPeriod = ".";

	// Token: 0x04005D59 RID: 23897
	public const string kUnderScore = "_";

	// Token: 0x04005D5A RID: 23898
	public const string kColon = ":";
}
