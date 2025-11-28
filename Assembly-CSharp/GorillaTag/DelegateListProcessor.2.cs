using System;

namespace GorillaTag
{
	// Token: 0x02001013 RID: 4115
	public class DelegateListProcessor<T> : DelegateListProcessorPlusMinus<DelegateListProcessor<T>, Action<T>>
	{
		// Token: 0x06006822 RID: 26658 RVA: 0x0021F895 File Offset: 0x0021DA95
		public DelegateListProcessor()
		{
		}

		// Token: 0x06006823 RID: 26659 RVA: 0x0021F89D File Offset: 0x0021DA9D
		public DelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x06006824 RID: 26660 RVA: 0x0021F8A6 File Offset: 0x0021DAA6
		public void InvokeSafe(in T data)
		{
			this.m_data = data;
			this.ProcessListSafe();
			this.m_data = default(T);
		}

		// Token: 0x06006825 RID: 26661 RVA: 0x0021F8C6 File Offset: 0x0021DAC6
		public void Invoke(in T data)
		{
			this.m_data = data;
			this.ProcessList();
			this.m_data = default(T);
		}

		// Token: 0x06006826 RID: 26662 RVA: 0x0021F8E6 File Offset: 0x0021DAE6
		protected override void ProcessItem(in Action<T> item)
		{
			item.Invoke(this.m_data);
		}

		// Token: 0x040076F2 RID: 30450
		private T m_data;
	}
}
