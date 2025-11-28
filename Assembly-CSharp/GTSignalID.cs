using System;

// Token: 0x020007F4 RID: 2036
[Serializable]
public struct GTSignalID : IEquatable<GTSignalID>, IEquatable<int>
{
	// Token: 0x0600357F RID: 13695 RVA: 0x001224D4 File Offset: 0x001206D4
	public override bool Equals(object obj)
	{
		if (obj is GTSignalID)
		{
			GTSignalID other = (GTSignalID)obj;
			return this.Equals(other);
		}
		if (obj is int)
		{
			int other2 = (int)obj;
			return this.Equals(other2);
		}
		return false;
	}

	// Token: 0x06003580 RID: 13696 RVA: 0x00122510 File Offset: 0x00120710
	public bool Equals(GTSignalID other)
	{
		return this._id == other._id;
	}

	// Token: 0x06003581 RID: 13697 RVA: 0x00122520 File Offset: 0x00120720
	public bool Equals(int other)
	{
		return this._id == other;
	}

	// Token: 0x06003582 RID: 13698 RVA: 0x0012252B File Offset: 0x0012072B
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06003583 RID: 13699 RVA: 0x00122533 File Offset: 0x00120733
	public static bool operator ==(GTSignalID x, GTSignalID y)
	{
		return x.Equals(y);
	}

	// Token: 0x06003584 RID: 13700 RVA: 0x0012253D File Offset: 0x0012073D
	public static bool operator !=(GTSignalID x, GTSignalID y)
	{
		return !x.Equals(y);
	}

	// Token: 0x06003585 RID: 13701 RVA: 0x0012252B File Offset: 0x0012072B
	public static implicit operator int(GTSignalID sid)
	{
		return sid._id;
	}

	// Token: 0x06003586 RID: 13702 RVA: 0x0012254C File Offset: 0x0012074C
	public static implicit operator GTSignalID(string s)
	{
		return new GTSignalID
		{
			_id = GTSignal.ComputeID(s)
		};
	}

	// Token: 0x040044AE RID: 17582
	private int _id;
}
