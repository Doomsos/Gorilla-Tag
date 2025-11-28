using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020009DE RID: 2526
[Serializable]
public struct Id32
{
	// Token: 0x0600406E RID: 16494 RVA: 0x00159418 File Offset: 0x00157618
	public Id32(string idString)
	{
		if (idString == null)
		{
			throw new ArgumentNullException("idString");
		}
		if (string.IsNullOrWhiteSpace(idString.Trim()))
		{
			throw new ArgumentNullException("idString");
		}
		this._id = XXHash32.Compute(idString, 0U);
	}

	// Token: 0x0600406F RID: 16495 RVA: 0x0015944D File Offset: 0x0015764D
	public unsafe static implicit operator int(Id32 i32)
	{
		return *Unsafe.As<Id32, int>(ref i32);
	}

	// Token: 0x06004070 RID: 16496 RVA: 0x00159457 File Offset: 0x00157657
	public static implicit operator Id32(string s)
	{
		return Id32.ComputeID(s);
	}

	// Token: 0x06004071 RID: 16497 RVA: 0x00159460 File Offset: 0x00157660
	[MethodImpl(256)]
	public unsafe static Id32 ComputeID(string s)
	{
		int num = Id32.ComputeHash(s);
		return *Unsafe.As<int, Id32>(ref num);
	}

	// Token: 0x06004072 RID: 16498 RVA: 0x00159480 File Offset: 0x00157680
	[MethodImpl(256)]
	public static int ComputeHash(string s)
	{
		if (s == null)
		{
			return 0;
		}
		s = s.Trim();
		if (string.IsNullOrWhiteSpace(s))
		{
			return 0;
		}
		return XXHash32.Compute(s, 0U);
	}

	// Token: 0x06004073 RID: 16499 RVA: 0x001594A0 File Offset: 0x001576A0
	public override int GetHashCode()
	{
		return this._id;
	}

	// Token: 0x06004074 RID: 16500 RVA: 0x001594A8 File Offset: 0x001576A8
	public override string ToString()
	{
		return string.Format("{{ {0} : {1} }}", "Id32", this._id);
	}

	// Token: 0x0400518E RID: 20878
	[SerializeField]
	private int _id;
}
