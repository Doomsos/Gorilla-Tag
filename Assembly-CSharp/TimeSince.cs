using System;

// Token: 0x020009F9 RID: 2553
public struct TimeSince
{
	// Token: 0x1700060B RID: 1547
	// (get) Token: 0x06004141 RID: 16705 RVA: 0x0015B7A4 File Offset: 0x001599A4
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
	// (get) Token: 0x06004142 RID: 16706 RVA: 0x0015B7E1 File Offset: 0x001599E1
	public float secondsElapsedFloat
	{
		get
		{
			return (float)this.secondsElapsed;
		}
	}

	// Token: 0x1700060D RID: 1549
	// (get) Token: 0x06004143 RID: 16707 RVA: 0x0015B7EA File Offset: 0x001599EA
	public int secondsElapsedInt
	{
		get
		{
			return (int)this.secondsElapsed;
		}
	}

	// Token: 0x1700060E RID: 1550
	// (get) Token: 0x06004144 RID: 16708 RVA: 0x0015B7F3 File Offset: 0x001599F3
	public uint secondsElapsedUint
	{
		get
		{
			return (uint)this.secondsElapsed;
		}
	}

	// Token: 0x1700060F RID: 1551
	// (get) Token: 0x06004145 RID: 16709 RVA: 0x0015B7FC File Offset: 0x001599FC
	public long secondsElapsedLong
	{
		get
		{
			return (long)this.secondsElapsed;
		}
	}

	// Token: 0x17000610 RID: 1552
	// (get) Token: 0x06004146 RID: 16710 RVA: 0x0015B805 File Offset: 0x00159A05
	public TimeSpan secondsElapsedSpan
	{
		get
		{
			return TimeSpan.FromSeconds(this.secondsElapsed);
		}
	}

	// Token: 0x06004147 RID: 16711 RVA: 0x0015B812 File Offset: 0x00159A12
	public TimeSince(DateTime dt)
	{
		this._dt = dt;
	}

	// Token: 0x06004148 RID: 16712 RVA: 0x0015B81C File Offset: 0x00159A1C
	public TimeSince(int elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x06004149 RID: 16713 RVA: 0x0015B840 File Offset: 0x00159A40
	public TimeSince(uint elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-1.0 * elapsed);
	}

	// Token: 0x0600414A RID: 16714 RVA: 0x0015B870 File Offset: 0x00159A70
	public TimeSince(float elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x0600414B RID: 16715 RVA: 0x0015B894 File Offset: 0x00159A94
	public TimeSince(double elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds(-elapsed);
	}

	// Token: 0x0600414C RID: 16716 RVA: 0x0015B8B8 File Offset: 0x00159AB8
	public TimeSince(long elapsed)
	{
		this._dt = DateTime.UtcNow.AddSeconds((double)(-(double)elapsed));
	}

	// Token: 0x0600414D RID: 16717 RVA: 0x0015B8DC File Offset: 0x00159ADC
	public TimeSince(TimeSpan elapsed)
	{
		this._dt = DateTime.UtcNow.Add(-elapsed);
	}

	// Token: 0x0600414E RID: 16718 RVA: 0x0015B902 File Offset: 0x00159B02
	public bool HasElapsed(int seconds)
	{
		return this.secondsElapsedInt >= seconds;
	}

	// Token: 0x0600414F RID: 16719 RVA: 0x0015B910 File Offset: 0x00159B10
	public bool HasElapsed(uint seconds)
	{
		return this.secondsElapsedUint >= seconds;
	}

	// Token: 0x06004150 RID: 16720 RVA: 0x0015B91E File Offset: 0x00159B1E
	public bool HasElapsed(float seconds)
	{
		return this.secondsElapsedFloat >= seconds;
	}

	// Token: 0x06004151 RID: 16721 RVA: 0x0015B92C File Offset: 0x00159B2C
	public bool HasElapsed(double seconds)
	{
		return this.secondsElapsed >= seconds;
	}

	// Token: 0x06004152 RID: 16722 RVA: 0x0015B93A File Offset: 0x00159B3A
	public bool HasElapsed(long seconds)
	{
		return this.secondsElapsedLong >= seconds;
	}

	// Token: 0x06004153 RID: 16723 RVA: 0x0015B948 File Offset: 0x00159B48
	public bool HasElapsed(TimeSpan seconds)
	{
		return this.secondsElapsedSpan >= seconds;
	}

	// Token: 0x06004154 RID: 16724 RVA: 0x0015B956 File Offset: 0x00159B56
	public void Reset()
	{
		this._dt = DateTime.UtcNow;
	}

	// Token: 0x06004155 RID: 16725 RVA: 0x0015B963 File Offset: 0x00159B63
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

	// Token: 0x06004156 RID: 16726 RVA: 0x0015B987 File Offset: 0x00159B87
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

	// Token: 0x06004157 RID: 16727 RVA: 0x0015B9AB File Offset: 0x00159BAB
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

	// Token: 0x06004158 RID: 16728 RVA: 0x0015B9CF File Offset: 0x00159BCF
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

	// Token: 0x06004159 RID: 16729 RVA: 0x0015B9F3 File Offset: 0x00159BF3
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

	// Token: 0x0600415A RID: 16730 RVA: 0x0015BA17 File Offset: 0x00159C17
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

	// Token: 0x0600415B RID: 16731 RVA: 0x0015BA40 File Offset: 0x00159C40
	public override string ToString()
	{
		return string.Format("{0:F3} seconds since {{{1:s}", this.secondsElapsed, this._dt);
	}

	// Token: 0x0600415C RID: 16732 RVA: 0x0015BA62 File Offset: 0x00159C62
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._dt);
	}

	// Token: 0x0600415D RID: 16733 RVA: 0x0015BA6F File Offset: 0x00159C6F
	public static TimeSince Now()
	{
		return new TimeSince(DateTime.UtcNow);
	}

	// Token: 0x0600415E RID: 16734 RVA: 0x0015BA7B File Offset: 0x00159C7B
	public static implicit operator long(TimeSince ts)
	{
		return ts.secondsElapsedLong;
	}

	// Token: 0x0600415F RID: 16735 RVA: 0x0015BA84 File Offset: 0x00159C84
	public static implicit operator double(TimeSince ts)
	{
		return ts.secondsElapsed;
	}

	// Token: 0x06004160 RID: 16736 RVA: 0x0015BA8D File Offset: 0x00159C8D
	public static implicit operator float(TimeSince ts)
	{
		return ts.secondsElapsedFloat;
	}

	// Token: 0x06004161 RID: 16737 RVA: 0x0015BA96 File Offset: 0x00159C96
	public static implicit operator int(TimeSince ts)
	{
		return ts.secondsElapsedInt;
	}

	// Token: 0x06004162 RID: 16738 RVA: 0x0015BA9F File Offset: 0x00159C9F
	public static implicit operator uint(TimeSince ts)
	{
		return ts.secondsElapsedUint;
	}

	// Token: 0x06004163 RID: 16739 RVA: 0x0015BAA8 File Offset: 0x00159CA8
	public static implicit operator TimeSpan(TimeSince ts)
	{
		return ts.secondsElapsedSpan;
	}

	// Token: 0x06004164 RID: 16740 RVA: 0x0015BAB1 File Offset: 0x00159CB1
	public static implicit operator TimeSince(int elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004165 RID: 16741 RVA: 0x0015BAB9 File Offset: 0x00159CB9
	public static implicit operator TimeSince(uint elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004166 RID: 16742 RVA: 0x0015BAC1 File Offset: 0x00159CC1
	public static implicit operator TimeSince(float elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004167 RID: 16743 RVA: 0x0015BAC9 File Offset: 0x00159CC9
	public static implicit operator TimeSince(double elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004168 RID: 16744 RVA: 0x0015BAD1 File Offset: 0x00159CD1
	public static implicit operator TimeSince(long elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x06004169 RID: 16745 RVA: 0x0015BAD9 File Offset: 0x00159CD9
	public static implicit operator TimeSince(TimeSpan elapsed)
	{
		return new TimeSince(elapsed);
	}

	// Token: 0x0600416A RID: 16746 RVA: 0x0015BAE1 File Offset: 0x00159CE1
	public static implicit operator TimeSince(DateTime dt)
	{
		return new TimeSince(dt);
	}

	// Token: 0x04005233 RID: 21043
	private DateTime _dt;

	// Token: 0x04005234 RID: 21044
	private const double INT32_MAX = 2147483647.0;
}
