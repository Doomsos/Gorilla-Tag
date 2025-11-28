using System;

// Token: 0x020005F4 RID: 1524
[Serializable]
public struct GroupJoinZoneAB
{
	// Token: 0x06002651 RID: 9809 RVA: 0x000CC924 File Offset: 0x000CAB24
	public static GroupJoinZoneAB operator &(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a & two.a),
			b = (one.b & two.b)
		};
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000CC964 File Offset: 0x000CAB64
	public static GroupJoinZoneAB operator |(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return new GroupJoinZoneAB
		{
			a = (one.a | two.a),
			b = (one.b | two.b)
		};
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000CC9A4 File Offset: 0x000CABA4
	public static GroupJoinZoneAB operator ~(GroupJoinZoneAB z)
	{
		return new GroupJoinZoneAB
		{
			a = ~z.a,
			b = ~z.b
		};
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x000CC9D6 File Offset: 0x000CABD6
	public static bool operator ==(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a == two.a && one.b == two.b;
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x000CC9F6 File Offset: 0x000CABF6
	public static bool operator !=(GroupJoinZoneAB one, GroupJoinZoneAB two)
	{
		return one.a != two.a || one.b != two.b;
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x000CCA19 File Offset: 0x000CAC19
	public override bool Equals(object other)
	{
		return this == (GroupJoinZoneAB)other;
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x000CCA2C File Offset: 0x000CAC2C
	public override int GetHashCode()
	{
		return this.a.GetHashCode() ^ this.b.GetHashCode();
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x000CCA54 File Offset: 0x000CAC54
	public static implicit operator GroupJoinZoneAB(int d)
	{
		return new GroupJoinZoneAB
		{
			a = (GroupJoinZoneA)d
		};
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000CCA74 File Offset: 0x000CAC74
	public override string ToString()
	{
		if (this.b == (GroupJoinZoneB)0)
		{
			return this.a.ToString();
		}
		if (this.a != (GroupJoinZoneA)0)
		{
			return this.a.ToString() + "," + this.b.ToString();
		}
		return this.b.ToString();
	}

	// Token: 0x04003259 RID: 12889
	public GroupJoinZoneA a;

	// Token: 0x0400325A RID: 12890
	public GroupJoinZoneB b;
}
