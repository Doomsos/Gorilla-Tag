using System;

// Token: 0x02000C85 RID: 3205
public static class StaticHashExt
{
	// Token: 0x06004E56 RID: 20054 RVA: 0x001964A9 File Offset: 0x001946A9
	public static int GetStaticHash(this int i)
	{
		return StaticHash.Compute(i);
	}

	// Token: 0x06004E57 RID: 20055 RVA: 0x001964B1 File Offset: 0x001946B1
	public static int GetStaticHash(this uint u)
	{
		return StaticHash.Compute(u);
	}

	// Token: 0x06004E58 RID: 20056 RVA: 0x001964B9 File Offset: 0x001946B9
	public static int GetStaticHash(this float f)
	{
		return StaticHash.Compute(f);
	}

	// Token: 0x06004E59 RID: 20057 RVA: 0x001964C1 File Offset: 0x001946C1
	public static int GetStaticHash(this long l)
	{
		return StaticHash.Compute(l);
	}

	// Token: 0x06004E5A RID: 20058 RVA: 0x001964C9 File Offset: 0x001946C9
	public static int GetStaticHash(this double d)
	{
		return StaticHash.Compute(d);
	}

	// Token: 0x06004E5B RID: 20059 RVA: 0x001964D1 File Offset: 0x001946D1
	public static int GetStaticHash(this bool b)
	{
		return StaticHash.Compute(b);
	}

	// Token: 0x06004E5C RID: 20060 RVA: 0x001964D9 File Offset: 0x001946D9
	public static int GetStaticHash(this DateTime dt)
	{
		return StaticHash.Compute(dt);
	}

	// Token: 0x06004E5D RID: 20061 RVA: 0x001964E1 File Offset: 0x001946E1
	public static int GetStaticHash(this string s)
	{
		return StaticHash.Compute(s);
	}

	// Token: 0x06004E5E RID: 20062 RVA: 0x001964E9 File Offset: 0x001946E9
	public static int GetStaticHash(this byte[] bytes)
	{
		return StaticHash.Compute(bytes);
	}
}
