using System;

// Token: 0x02000800 RID: 2048
public struct HandLinkAuthorityStatus
{
	// Token: 0x060035D3 RID: 13779 RVA: 0x00123F30 File Offset: 0x00122130
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority)
	{
		this.type = authority;
		this.timestamp = -1f;
		this.tiebreak = -1;
	}

	// Token: 0x060035D4 RID: 13780 RVA: 0x00123F4B File Offset: 0x0012214B
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority, float timestamp, int tiebreak)
	{
		this.type = authority;
		this.timestamp = timestamp;
		this.tiebreak = tiebreak;
	}

	// Token: 0x060035D5 RID: 13781 RVA: 0x00123F64 File Offset: 0x00122164
	public static bool operator >(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type > b.type || (b.type <= a.type && (a.timestamp > b.timestamp || (b.timestamp <= a.timestamp && a.tiebreak > b.tiebreak)));
	}

	// Token: 0x060035D6 RID: 13782 RVA: 0x00123FC0 File Offset: 0x001221C0
	public static bool operator <(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type < b.type || (b.type >= a.type && (a.timestamp < b.timestamp || (b.timestamp >= a.timestamp && a.tiebreak < b.tiebreak)));
	}

	// Token: 0x060035D7 RID: 13783 RVA: 0x0012401C File Offset: 0x0012221C
	public int CompareTo(HandLinkAuthorityStatus b)
	{
		int num = this.type.CompareTo(b.type);
		if (num != 0)
		{
			return num;
		}
		int num2 = this.timestamp.CompareTo(b.timestamp);
		if (num2 != 0)
		{
			return num2;
		}
		return this.tiebreak.CompareTo(b.tiebreak);
	}

	// Token: 0x060035D8 RID: 13784 RVA: 0x00124073 File Offset: 0x00122273
	public static bool operator ==(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type == b.type && a.timestamp == b.timestamp && a.tiebreak == b.tiebreak;
	}

	// Token: 0x060035D9 RID: 13785 RVA: 0x001240A1 File Offset: 0x001222A1
	public static bool operator !=(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.timestamp != b.timestamp || a.tiebreak != b.tiebreak;
	}

	// Token: 0x060035DA RID: 13786 RVA: 0x001240C4 File Offset: 0x001222C4
	public override string ToString()
	{
		return string.Format("{0}/{1}", this.timestamp.ToString("0.0000"), this.tiebreak);
	}

	// Token: 0x0400452A RID: 17706
	public HandLinkAuthorityType type;

	// Token: 0x0400452B RID: 17707
	public float timestamp;

	// Token: 0x0400452C RID: 17708
	public int tiebreak;
}
