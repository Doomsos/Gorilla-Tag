using System;
using UnityEngine;

// Token: 0x020009C4 RID: 2500
[Serializable]
public struct AnimStateHash
{
	// Token: 0x06003FF8 RID: 16376 RVA: 0x00157D40 File Offset: 0x00155F40
	public static implicit operator AnimStateHash(string s)
	{
		return new AnimStateHash
		{
			_hash = Animator.StringToHash(s)
		};
	}

	// Token: 0x06003FF9 RID: 16377 RVA: 0x00157D63 File Offset: 0x00155F63
	public static implicit operator int(AnimStateHash ash)
	{
		return ash._hash;
	}

	// Token: 0x0400512C RID: 20780
	[SerializeField]
	private int _hash;
}
