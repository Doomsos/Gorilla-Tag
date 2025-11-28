using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// Token: 0x020009DC RID: 2524
[Serializable]
[StructLayout(2)]
public struct Id128 : IEquatable<Id128>, IComparable<Id128>, IEquatable<Guid>, IEquatable<Hash128>
{
	// Token: 0x06004045 RID: 16453 RVA: 0x00158ED0 File Offset: 0x001570D0
	public Id128(int a, int b, int c, int d)
	{
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = (this.y = 0L);
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}

	// Token: 0x06004046 RID: 16454 RVA: 0x00158F24 File Offset: 0x00157124
	public Id128(long x, long y)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = default(Hash128);
		this.x = x;
		this.y = y;
	}

	// Token: 0x06004047 RID: 16455 RVA: 0x00158F78 File Offset: 0x00157178
	public Id128(Hash128 hash)
	{
		this.x = (this.y = 0L);
		this.a = (this.b = (this.c = (this.d = 0)));
		this.guid = Guid.Empty;
		this.h128 = hash;
	}

	// Token: 0x06004048 RID: 16456 RVA: 0x00158FCC File Offset: 0x001571CC
	public Id128(Guid guid)
	{
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = guid;
	}

	// Token: 0x06004049 RID: 16457 RVA: 0x00159020 File Offset: 0x00157220
	public Id128(string guid)
	{
		if (string.IsNullOrWhiteSpace(guid))
		{
			throw new ArgumentNullException("guid");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = Guid.Parse(guid);
	}

	// Token: 0x0600404A RID: 16458 RVA: 0x0015908C File Offset: 0x0015728C
	public Id128(byte[] bytes)
	{
		if (bytes == null)
		{
			throw new ArgumentNullException("bytes");
		}
		if (bytes.Length != 16)
		{
			throw new ArgumentException("Input buffer must be exactly 16 bytes", "bytes");
		}
		this.a = (this.b = (this.c = (this.d = 0)));
		this.x = (this.y = 0L);
		this.h128 = default(Hash128);
		this.guid = new Guid(bytes);
	}

	// Token: 0x0600404B RID: 16459 RVA: 0x00159109 File Offset: 0x00157309
	[return: TupleElementNames(new string[]
	{
		"l1",
		"l2"
	})]
	public ValueTuple<long, long> ToLongs()
	{
		return new ValueTuple<long, long>(this.x, this.y);
	}

	// Token: 0x0600404C RID: 16460 RVA: 0x0015911C File Offset: 0x0015731C
	[return: TupleElementNames(new string[]
	{
		"i1",
		"i2",
		"i3",
		"i4"
	})]
	public ValueTuple<int, int, int, int> ToInts()
	{
		return new ValueTuple<int, int, int, int>(this.a, this.b, this.c, this.d);
	}

	// Token: 0x0600404D RID: 16461 RVA: 0x0015913B File Offset: 0x0015733B
	public byte[] ToByteArray()
	{
		return this.guid.ToByteArray();
	}

	// Token: 0x0600404E RID: 16462 RVA: 0x00159148 File Offset: 0x00157348
	public bool Equals(Id128 id)
	{
		return this.x == id.x && this.y == id.y;
	}

	// Token: 0x0600404F RID: 16463 RVA: 0x00159168 File Offset: 0x00157368
	public bool Equals(Guid g)
	{
		return this.guid == g;
	}

	// Token: 0x06004050 RID: 16464 RVA: 0x00159176 File Offset: 0x00157376
	public bool Equals(Hash128 h)
	{
		return this.h128 == h;
	}

	// Token: 0x06004051 RID: 16465 RVA: 0x00159184 File Offset: 0x00157384
	public override bool Equals(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.Equals(id);
		}
		if (obj is Guid)
		{
			Guid g = (Guid)obj;
			return this.Equals(g);
		}
		if (obj is Hash128)
		{
			Hash128 h = (Hash128)obj;
			return this.Equals(h);
		}
		return false;
	}

	// Token: 0x06004052 RID: 16466 RVA: 0x001591D7 File Offset: 0x001573D7
	public override string ToString()
	{
		return this.guid.ToString();
	}

	// Token: 0x06004053 RID: 16467 RVA: 0x001591EA File Offset: 0x001573EA
	public override int GetHashCode()
	{
		return StaticHash.Compute(this.a, this.b, this.c, this.d);
	}

	// Token: 0x06004054 RID: 16468 RVA: 0x0015920C File Offset: 0x0015740C
	public int CompareTo(Id128 id)
	{
		int num = this.x.CompareTo(id.x);
		if (num == 0)
		{
			num = this.y.CompareTo(id.y);
		}
		return num;
	}

	// Token: 0x06004055 RID: 16469 RVA: 0x00159244 File Offset: 0x00157444
	public int CompareTo(object obj)
	{
		if (obj is Id128)
		{
			Id128 id = (Id128)obj;
			return this.CompareTo(id);
		}
		if (obj is Guid)
		{
			Guid guid = (Guid)obj;
			return this.guid.CompareTo(guid);
		}
		if (obj is Hash128)
		{
			Hash128 hash = (Hash128)obj;
			return this.h128.CompareTo(hash);
		}
		throw new ArgumentException("Object must be of type Id128 or Guid");
	}

	// Token: 0x06004056 RID: 16470 RVA: 0x001592AA File Offset: 0x001574AA
	public static Id128 NewId()
	{
		return new Id128(Guid.NewGuid());
	}

	// Token: 0x06004057 RID: 16471 RVA: 0x001592B8 File Offset: 0x001574B8
	public static Id128 ComputeMD5(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		Id128 result;
		using (MD5 md = MD5.Create())
		{
			result = new Guid(md.ComputeHash(Encoding.UTF8.GetBytes(s)));
		}
		return result;
	}

	// Token: 0x06004058 RID: 16472 RVA: 0x00159314 File Offset: 0x00157514
	public static Id128 ComputeSHV2(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return Id128.Empty;
		}
		return Hash128.Compute(s);
	}

	// Token: 0x06004059 RID: 16473 RVA: 0x0015932F File Offset: 0x0015752F
	public static bool operator ==(Id128 j, Id128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x0600405A RID: 16474 RVA: 0x00159339 File Offset: 0x00157539
	public static bool operator !=(Id128 j, Id128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x0600405B RID: 16475 RVA: 0x00159346 File Offset: 0x00157546
	public static bool operator ==(Id128 j, Guid k)
	{
		return j.Equals(k);
	}

	// Token: 0x0600405C RID: 16476 RVA: 0x00159350 File Offset: 0x00157550
	public static bool operator !=(Id128 j, Guid k)
	{
		return !j.Equals(k);
	}

	// Token: 0x0600405D RID: 16477 RVA: 0x0015935D File Offset: 0x0015755D
	public static bool operator ==(Guid j, Id128 k)
	{
		return j.Equals(k.guid);
	}

	// Token: 0x0600405E RID: 16478 RVA: 0x0015936C File Offset: 0x0015756C
	public static bool operator !=(Guid j, Id128 k)
	{
		return !j.Equals(k.guid);
	}

	// Token: 0x0600405F RID: 16479 RVA: 0x0015937E File Offset: 0x0015757E
	public static bool operator ==(Id128 j, Hash128 k)
	{
		return j.Equals(k);
	}

	// Token: 0x06004060 RID: 16480 RVA: 0x00159388 File Offset: 0x00157588
	public static bool operator !=(Id128 j, Hash128 k)
	{
		return !j.Equals(k);
	}

	// Token: 0x06004061 RID: 16481 RVA: 0x00159395 File Offset: 0x00157595
	public static bool operator ==(Hash128 j, Id128 k)
	{
		return j.Equals(k.h128);
	}

	// Token: 0x06004062 RID: 16482 RVA: 0x001593A4 File Offset: 0x001575A4
	public static bool operator !=(Hash128 j, Id128 k)
	{
		return !j.Equals(k.h128);
	}

	// Token: 0x06004063 RID: 16483 RVA: 0x001593B6 File Offset: 0x001575B6
	public static bool operator <(Id128 j, Id128 k)
	{
		return j.CompareTo(k) < 0;
	}

	// Token: 0x06004064 RID: 16484 RVA: 0x001593C3 File Offset: 0x001575C3
	public static bool operator >(Id128 j, Id128 k)
	{
		return j.CompareTo(k) > 0;
	}

	// Token: 0x06004065 RID: 16485 RVA: 0x001593D0 File Offset: 0x001575D0
	public static bool operator <=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) <= 0;
	}

	// Token: 0x06004066 RID: 16486 RVA: 0x001593E0 File Offset: 0x001575E0
	public static bool operator >=(Id128 j, Id128 k)
	{
		return j.CompareTo(k) >= 0;
	}

	// Token: 0x06004067 RID: 16487 RVA: 0x001593F0 File Offset: 0x001575F0
	public static implicit operator Guid(Id128 id)
	{
		return id.guid;
	}

	// Token: 0x06004068 RID: 16488 RVA: 0x001593F8 File Offset: 0x001575F8
	public static implicit operator Id128(Guid guid)
	{
		return new Id128(guid);
	}

	// Token: 0x06004069 RID: 16489 RVA: 0x00159400 File Offset: 0x00157600
	public static implicit operator Id128(Hash128 h)
	{
		return new Id128(h);
	}

	// Token: 0x0600406A RID: 16490 RVA: 0x00159408 File Offset: 0x00157608
	public static implicit operator Hash128(Id128 id)
	{
		return id.h128;
	}

	// Token: 0x0600406B RID: 16491 RVA: 0x00159410 File Offset: 0x00157610
	public static explicit operator Id128(string s)
	{
		return Id128.ComputeMD5(s);
	}

	// Token: 0x04005185 RID: 20869
	[SerializeField]
	[FieldOffset(0)]
	public long x;

	// Token: 0x04005186 RID: 20870
	[SerializeField]
	[FieldOffset(8)]
	public long y;

	// Token: 0x04005187 RID: 20871
	[NonSerialized]
	[FieldOffset(0)]
	public int a;

	// Token: 0x04005188 RID: 20872
	[NonSerialized]
	[FieldOffset(4)]
	public int b;

	// Token: 0x04005189 RID: 20873
	[NonSerialized]
	[FieldOffset(8)]
	public int c;

	// Token: 0x0400518A RID: 20874
	[NonSerialized]
	[FieldOffset(12)]
	public int d;

	// Token: 0x0400518B RID: 20875
	[NonSerialized]
	[FieldOffset(0)]
	public Guid guid;

	// Token: 0x0400518C RID: 20876
	[NonSerialized]
	[FieldOffset(0)]
	public Hash128 h128;

	// Token: 0x0400518D RID: 20877
	public static readonly Id128 Empty;
}
