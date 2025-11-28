using System;
using System.ComponentModel;
using UnityEngine;

// Token: 0x0200032A RID: 810
public static class UnityTagsExt
{
	// Token: 0x06001398 RID: 5016 RVA: 0x000713C4 File Offset: 0x0006F5C4
	public static UnityTag ToTag(this string s)
	{
		if (string.IsNullOrWhiteSpace(s))
		{
			return UnityTag.Invalid;
		}
		UnityTag result;
		if (!UnityTags.StringToTag.TryGetValue(s, ref result))
		{
			return UnityTag.Invalid;
		}
		return result;
	}

	// Token: 0x06001399 RID: 5017 RVA: 0x000713ED File Offset: 0x0006F5ED
	public static void SetTag(this Component c, UnityTag tag)
	{
		if (c == null)
		{
			return;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		c.tag = UnityTags.StringValues[(int)tag];
	}

	// Token: 0x0600139A RID: 5018 RVA: 0x00071415 File Offset: 0x0006F615
	public static void SetTag(this GameObject g, UnityTag tag)
	{
		if (g == null)
		{
			return;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		g.tag = UnityTags.StringValues[(int)tag];
	}

	// Token: 0x0600139B RID: 5019 RVA: 0x0007143D File Offset: 0x0006F63D
	public static bool TryGetTag(this GameObject g, out UnityTag tag)
	{
		tag = UnityTag.Invalid;
		return !(g == null) && UnityTags.StringToTag.TryGetValue(g.tag, ref tag);
	}

	// Token: 0x0600139C RID: 5020 RVA: 0x0007145E File Offset: 0x0006F65E
	public static bool TryGetTag(this Component c, out UnityTag tag)
	{
		tag = UnityTag.Invalid;
		return !(c == null) && UnityTags.StringToTag.TryGetValue(c.tag, ref tag);
	}

	// Token: 0x0600139D RID: 5021 RVA: 0x0007147F File Offset: 0x0006F67F
	public static bool CompareTag(this GameObject g, UnityTag tag)
	{
		if (g == null)
		{
			return false;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		return g.CompareTag(UnityTags.StringValues[(int)tag]);
	}

	// Token: 0x0600139E RID: 5022 RVA: 0x000714A8 File Offset: 0x0006F6A8
	public static bool CompareTag(this Component c, UnityTag tag)
	{
		if (c == null)
		{
			return false;
		}
		if (tag == UnityTag.Invalid)
		{
			throw new InvalidEnumArgumentException("tag");
		}
		return c.CompareTag(UnityTags.StringValues[(int)tag]);
	}
}
