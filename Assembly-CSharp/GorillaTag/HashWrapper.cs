using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000FE0 RID: 4064
	[Serializable]
	public struct HashWrapper : IEquatable<int>
	{
		// Token: 0x060066D5 RID: 26325 RVA: 0x00217503 File Offset: 0x00215703
		public HashWrapper(int hash = -1)
		{
			this.hashCode = hash;
		}

		// Token: 0x060066D6 RID: 26326 RVA: 0x0021750C File Offset: 0x0021570C
		public override int GetHashCode()
		{
			return this.hashCode;
		}

		// Token: 0x060066D7 RID: 26327 RVA: 0x00217514 File Offset: 0x00215714
		public override bool Equals(object obj)
		{
			return this.hashCode.Equals(obj);
		}

		// Token: 0x060066D8 RID: 26328 RVA: 0x00217522 File Offset: 0x00215722
		public bool Equals(int i)
		{
			return this.hashCode.Equals(i);
		}

		// Token: 0x060066D9 RID: 26329 RVA: 0x0021750C File Offset: 0x0021570C
		public static implicit operator int(in HashWrapper hash)
		{
			return hash.hashCode;
		}

		// Token: 0x04007561 RID: 30049
		[SerializeField]
		private int hashCode;

		// Token: 0x04007562 RID: 30050
		public const int NULL_HASH = -1;
	}
}
