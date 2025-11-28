using System;

// Token: 0x02000800 RID: 2048
public struct HandLinkAuthorityStatus
{
	// Token: 0x060035D3 RID: 13779 RVA: 0x00123F10 File Offset: 0x00122110
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority)
	{
		this.type = authority;
		this.timestamp = -1f;
		this.tiebreak = -1;
	}

	// Token: 0x060035D4 RID: 13780 RVA: 0x00123F2B File Offset: 0x0012212B
	public HandLinkAuthorityStatus(HandLinkAuthorityType authority, float timestamp, int tiebreak)
	{
		this.type = authority;
		this.timestamp = timestamp;
		this.tiebreak = tiebreak;
	}

	// Token: 0x060035D5 RID: 13781 RVA: 0x00123F44 File Offset: 0x00122144
	public static bool operator >(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type > b.type || (b.type <= a.type && (a.timestamp > b.timestamp || (b.timestamp <= a.timestamp && a.tiebreak > b.tiebreak)));
	}

	// Token: 0x060035D6 RID: 13782 RVA: 0x00123FA0 File Offset: 0x001221A0
	public static bool operator <(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type < b.type || (b.type >= a.type && (a.timestamp < b.timestamp || (b.timestamp >= a.timestamp && a.tiebreak < b.tiebreak)));
	}

	// Token: 0x060035D7 RID: 13783 RVA: 0x00123FFC File Offset: 0x001221FC
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

	// Token: 0x060035D8 RID: 13784 RVA: 0x00124053 File Offset: 0x00122253
	public static bool operator ==(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.type == b.type && a.timestamp == b.timestamp && a.tiebreak == b.tiebreak;
	}

	// Token: 0x060035D9 RID: 13785 RVA: 0x00124081 File Offset: 0x00122281
	public static bool operator !=(HandLinkAuthorityStatus a, HandLinkAuthorityStatus b)
	{
		return a.timestamp != b.timestamp || a.tiebreak != b.tiebreak;
	}

	// Token: 0x060035DA RID: 13786 RVA: 0x001240A4 File Offset: 0x001222A4
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
