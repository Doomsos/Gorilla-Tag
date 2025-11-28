using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Text;
using UnityEngine;

namespace GorillaExtensions
{
	// Token: 0x02000FB6 RID: 4022
	public static class GTStringBuilderExtensions
	{
		// Token: 0x060064F7 RID: 25847 RVA: 0x0020F874 File Offset: 0x0020DA74
		public unsafe static IEnumerable<ReadOnlyMemory<char>> GetSegmentsOfMem(this Utf16ValueStringBuilder sb, int maxCharsPerSegment = 16300)
		{
			int i = 0;
			List<ReadOnlyMemory<char>> list = new List<ReadOnlyMemory<char>>(64);
			ReadOnlyMemory<char> readOnlyMemory = sb.AsMemory();
			while (i < readOnlyMemory.Length)
			{
				int num = Mathf.Min(i + maxCharsPerSegment, readOnlyMemory.Length);
				if (num < readOnlyMemory.Length)
				{
					int num2 = -1;
					for (int j = num - 1; j >= i; j--)
					{
						if (*readOnlyMemory.Span[j] == 10)
						{
							num2 = j;
							break;
						}
					}
					if (num2 != -1)
					{
						num = num2;
					}
				}
				list.Add(readOnlyMemory.Slice(i, num - i));
				i = num + 1;
			}
			return list;
		}

		// Token: 0x060064F8 RID: 25848 RVA: 0x0020F909 File Offset: 0x0020DB09
		[MethodImpl(256)]
		public static void GTAddPath(this Utf16ValueStringBuilder stringBuilderToAddTo, GameObject gameObject)
		{
			gameObject.transform.GetPathQ(ref stringBuilderToAddTo);
		}

		// Token: 0x060064F9 RID: 25849 RVA: 0x0020F918 File Offset: 0x0020DB18
		[MethodImpl(256)]
		public static void GTAddPath(this Utf16ValueStringBuilder stringBuilderToAddTo, Transform transform)
		{
			transform.GetPathQ(ref stringBuilderToAddTo);
		}

		// Token: 0x060064FA RID: 25850 RVA: 0x0020F922 File Offset: 0x0020DB22
		[MethodImpl(256)]
		public static void Q(this Utf16ValueStringBuilder sb, string value)
		{
			sb.Append('"');
			sb.Append(value);
			sb.Append('"');
		}

		// Token: 0x060064FB RID: 25851 RVA: 0x0020F93E File Offset: 0x0020DB3E
		[MethodImpl(256)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b)
		{
			sb.Append(a);
			sb.Append(b);
		}

		// Token: 0x060064FC RID: 25852 RVA: 0x0020F950 File Offset: 0x0020DB50
		[MethodImpl(256)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
		}

		// Token: 0x060064FD RID: 25853 RVA: 0x0020F96A File Offset: 0x0020DB6A
		[MethodImpl(256)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
		}

		// Token: 0x060064FE RID: 25854 RVA: 0x0020F98D File Offset: 0x0020DB8D
		[MethodImpl(256)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
		}

		// Token: 0x060064FF RID: 25855 RVA: 0x0020F9B9 File Offset: 0x0020DBB9
		[MethodImpl(256)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
		}

		// Token: 0x06006500 RID: 25856 RVA: 0x0020F9EE File Offset: 0x0020DBEE
		[MethodImpl(256)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
		}

		// Token: 0x06006501 RID: 25857 RVA: 0x0020FA2C File Offset: 0x0020DC2C
		[MethodImpl(256)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
		}

		// Token: 0x06006502 RID: 25858 RVA: 0x0020FA80 File Offset: 0x0020DC80
		[MethodImpl(256)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
			sb.Append(i);
		}

		// Token: 0x06006503 RID: 25859 RVA: 0x0020FADC File Offset: 0x0020DCDC
		[MethodImpl(256)]
		public static void GTMany(this Utf16ValueStringBuilder sb, string a, string b, string c, string d, string e, string f, string g, string h, string i, string j)
		{
			sb.Append(a);
			sb.Append(b);
			sb.Append(c);
			sb.Append(d);
			sb.Append(e);
			sb.Append(f);
			sb.Append(g);
			sb.Append(h);
			sb.Append(i);
			sb.Append(j);
		}
	}
}
