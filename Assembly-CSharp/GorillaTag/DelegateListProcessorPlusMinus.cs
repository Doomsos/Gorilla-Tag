using System;

namespace GorillaTag
{
	// Token: 0x02001011 RID: 4113
	public abstract class DelegateListProcessorPlusMinus<T1, T2> : ListProcessorAbstract<T2> where T1 : DelegateListProcessorPlusMinus<T1, T2>, new() where T2 : Delegate
	{
		// Token: 0x06006819 RID: 26649 RVA: 0x0021F7D2 File Offset: 0x0021D9D2
		protected DelegateListProcessorPlusMinus()
		{
		}

		// Token: 0x0600681A RID: 26650 RVA: 0x0021F7DA File Offset: 0x0021D9DA
		protected DelegateListProcessorPlusMinus(int capacity) : base(capacity)
		{
		}

		// Token: 0x0600681B RID: 26651 RVA: 0x0021F7E3 File Offset: 0x0021D9E3
		public static T1 operator +(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
		{
			if (left == null)
			{
				left = Activator.CreateInstance<T1>();
			}
			if (right == null)
			{
				return (T1)((object)left);
			}
			left.Add(right);
			return (T1)((object)left);
		}

		// Token: 0x0600681C RID: 26652 RVA: 0x0021F814 File Offset: 0x0021DA14
		public static T1 operator -(DelegateListProcessorPlusMinus<T1, T2> left, T2 right)
		{
			if (left == null)
			{
				return default(T1);
			}
			if (right == null)
			{
				return (T1)((object)left);
			}
			left.Remove(right);
			return (T1)((object)left);
		}
	}
}
