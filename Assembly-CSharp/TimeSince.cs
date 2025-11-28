using System;

// Token: 0x020009F9 RID: 2553
public struct TimeSince
{
	// Token: 0x1700060B RID: 1547
	// (get) Token: 0x06004141 RID: 16705 RVA: 0x0015B7C4 File Offset: 0x001599C4
	public double secondsElapsed
	{
		get
		{
			double totalSeconds = (DateTime.UtcNow - this._dt).TotalSeconds;
			if (totalSeconds <= 2147483647.0)
			{
				return totalSeconds;
			}
			return 2147483647.0;
		}
	}

	// Token: 0x1700060C RID: 1548
	// (get) Token: 0x06004142 RID: 16706 RVA: 0x0015B801 File Offset: 0x00159A01
	public float secondsElapsedFloat
	{
		get
		{
			return (float)this.secondsElapsed;
		}
	}

	// Token: 0x1700060D RID: 1549
	// (get) Token: 0x06004143 RID: 16707 RVA: 0x0015B80A File Offset: 0x00159A0A
	public int secondsElapsedInt
	{
		get
		{
			return (int)this.secondsElapsed;
		}
	}

	// Token: 0x1700060E RID: 1550
	// (get) Token: 0x06004144 RID: 16708 RVA: 0x0015B813 File Offset: 0x00159A13
	public uint secondsElapsedUint
	{
		get
		{
			return (uint)this.secondsElapsed;
		}
	}

	// Token: 0x1700060F RID: 1551
	// (get) Token: 0x06004145 RID: 16709 RVA: 0x0015B81C File Offset: 0x00159A1C
	public long secondsElapsedLong
	{
		get
		{
			return (long)this.secondsElapsed;
		}
	}

	// Token: 0x17000610 RID: 1552
	// (get) Token: 0x06004146 RID: 16710 RVA: 0x0015B825 File Offset: 0x00159A25
	public TimeSpan secondsElapsedSpan
	{
		get
		{
			return TimeSpan.FromSeconds(this.secondsElapsed);
		}
	}

	// Token: 0x06004147 RID: 16711 RVA: 0x0015B832 File Offset: 0x00159A32
	public TimeSince(DateTime dt)
	{
		this._dt = dt;
	}

	// Token: 0x06004148 RID: 16712 RVA: 0x0015B83C File Offset: 0x00159A3C
	public TimeSince(int elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x06004149 RID: 16713 RVA: 0x0015B860 File Offset: 0x00159A60
	public TimeSince(uint elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-1.0 * elapsed);
	}

	// Token: 0x0600414A RID: 16714 RVA: 0x0015B890 File Offset: 0x00159A90
	public TimeSince(float elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x0600414B RID: 16715 RVA: 0x0015B8B4 File Offset: 0x00159AB4
	public TimeSince(double elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-elapsed);
	}

	// Token: 0x0600414C RID: 16716 RVA: 0x0015B8D8 File Offset: 0x00159AD8
	public TimeSince(long elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x0600414D RID: 16717 RVA: 0x0015B8FC File Offset: 0x00159AFC
	public TimeSince(TimeSpan elapsed)
	{
		this._dt = DateTime.UtcNow.Add(-elapsed);
	}

	// Token: 0x0600414E RID: 16718 RVA: 0x0015B922 File Offset: 0x00159B22
	public bool HasElapsed(int seconds)
	{
		return this.secondsElapsedInt >= seconds;
	}

	// Token: 0x0600414F RID: 16719 RVA: 0x0015B930 File Offset: 0x00159B30
	public bool HasElapsed(uint seconds)
	{
		return this.secondsElapsedUint >= seconds;
	}

	// Token: 0x06004150 RID: 16720 RVA: 0x0015B93E File Offset: 0x00159B3E
	public bool HasElapsed(float seconds)
	{
		return this.secondsElapsedFloat >= seconds;
	}

	// Token: 0x06004151 RID: 16721 RVA: 0x0015B94C File Offset: 0x00159B4C
	public bool HasElapsed(double seconds)
	{
		return this.secondsElapsed >= seconds;
	}

	// Token: 0x06004152 RID: 16722 RVA: 0x0015B95A File Offset: 0x00159B5A
	public bool HasElapsed(long seconds)
	{
		return this.secondsElapsedLong >= seconds;
	}

	// Token: 0x06004153 RID: 16723 RVA: 0x0015B968 File Offset: 0x00159B68
	public bool HasElapsed(TimeSpan seconds)
	{
		return this.secondsElapsedSpan >= seconds;
	}

	// Token: 0x06004154 RID: 16724 RVA: 0x0015B976 File Offset: 0x00159B76
	public void Reset()
	{
		this._dt = DateTime.UtcNow;
	}

	// Token: 0x06004155 RID: 16725 RVA: 0x0015B983 File Offset: 0x00159B83
	public bool HasElapsed(int seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedInt >= seconds;
		}
		if (this.secondsElapsedInt < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06004156 RID: 16726 RVA: 0x0015B9A7 File Offset: 0x00159BA7
	public bool HasElapsed(uint seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedUint >= seconds;
		}
		if (this.secondsElapsedUint < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06004157 RID: 16727 RVA: 0x0015B9CB File Offset: 0x00159BCB
	public bool HasElapsed(float seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedFloat >= seconds;
		}
		if (this.secondsElapsedFloat < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06004158 RID: 16728 RVA: 0x0015B9EF File Offset: 0x00159BEF
	public bool HasElapsed(double seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsed >= seconds;
		}
		if (this.secondsElapsed < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x06004159 RID: 16729 RVA: 0x0015BA13 File Offset: 0x00159C13
	public bool HasElapsed(long seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedLong >= seconds;
		}
		if (this.secondsElapsedLong < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600415A RID: 16730 RVA: 0x0015BA37 File Offset: 0x00159C37
	public bool HasElapsed(TimeSpan seconds, bool resetOnElapsed)
	{
		if (!resetOnElapsed)
		{
			return this.secondsElapsedSpan >= seconds;
		}
		if (this.secondsElapsedSpan < seconds)
		{
			return false;
		}
		this.Reset();
		return true;
	}

	// Token: 0x0600415B RID: 16731 RVA: 0x0015BA60 File Offset: 0x00159C60
	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:s}", this.secondsElapsed, this._dt);
	}

	// Token: 0x0600415C RID: 16732 RVA: 0x0015BA82 File Offset: 0x00159C82
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._dt);
	}

	// Token: 0x0600415D RID: 16733 RVA: 0x0015BA8F File Offset: 0x00159C8F
	public static TimeSince Now()
	{
		return new TimeSince(DateTime.UtcNow);
	}

	// Token: 0x0600415E RID: 16734 RVA: 0x0015BA9B File Offset: 0x00159C9B
	public static implicit operator long(TimeSince ts)
	{
		return ts.secondsElapsedLong;
	}

	// Token: 0x0600415F RID: 16735 RVA: 0x0015BAA4 File Offset: 0x00159CA4
	public static implicit operator double(TimeSince ts)
	{
		return ts.secondsElapsed;
	}

	// Token: 0x06004160 RID: 16736 RVA: 0x0015BAAD File Offset: 0x00159CAD
	public static implicit operator float(TimeSince ts)
	{
		return ts.secondsElapsedFloat;
	}

	// Token: 0x06004161 RID: 16737 RVA: 0x0015BAB6 File Offset: 0x00159CB6
	public static implicit operator int(TimeSince ts)
	{
		return ts.secondsElapsedInt;
	}

	// Token: 0x06004162 RID: 16738 RVA: 0x0015BABF File Offset: 0x00159CBF
	public static implicit operator uint(TimeSince ts)
	{
		return ts.secondsElapsedUint;
	}

	// Token: 0x06004163 RID: 16739 RVA: 0x0015BAC8 File Offset: 0x00159CC8
	public static implicit operator TimeSpan(TimeSince ts)
	{
		return ts.secondsElapsedSpan;
	}

	// Token: 0x06004164 RID: 16740 RVA: 0x0015BAD1 File Offset: 0x00159CD1
	public static implicit operator TimeSince(int elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004165 RID: 16741 RVA: 0x0015BAD9 File Offset: 0x00159CD9
	public static implicit operator TimeSince(uint elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004166 RID: 16742 RVA: 0x0015BAE1 File Offset: 0x00159CE1
	public static implicit operator TimeSince(float elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004167 RID: 16743 RVA: 0x0015BAE9 File Offset: 0x00159CE9
	public static implicit operator TimeSince(double elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004168 RID: 16744 RVA: 0x0015BAF1 File Offset: 0x00159CF1
	public static implicit operator TimeSince(long elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004169 RID: 16745 RVA: 0x0015BAF9 File Offset: 0x00159CF9
	public static implicit operator TimeSince(TimeSpan elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600416A RID: 16746 RVA: 0x0015BB01 File Offset: 0x00159D01
	public static implicit operator TimeSince(DateTime dt)
	{
		return new TimeSince(dt);
	}

	// Token: 0x04005233 RID: 21043
	private DateTime _dt;

	// Token: 0x04005234 RID: 21044
	private const double INT32_MAX = 2147483647.0;
}
