using System;

namespace GorillaTag
{
	// Token: 0x02001016 RID: 4118
	public class InDelegateListProcessor<T1, T2> : DelegateListProcessorPlusMinus<InDelegateListProcessor<T1, T2>, InAction<T1, T2>>
	{
		// Token: 0x06006833 RID: 26675 RVA: 0x0021F9BB File Offset: 0x0021DBBB
		public InDelegateListProcessor()
		{
		}

		// Token: 0x06006834 RID: 26676 RVA: 0x0021F9C3 File Offset: 0x0021DBC3
		public InDelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x06006835 RID: 26677 RVA: 0x0021F9CC File Offset: 0x0021DBCC
		public void InvokeSafe(in T1 data1, in T2 data2)
		{
			this.SetData(data1, data2);
			this.ProcessListSafe();
			this.ResetData();
		}

		// Token: 0x06006836 RID: 26678 RVA: 0x0021F9E2 File Offset: 0x0021DBE2
		public void Invoke(in T1 data1, in T2 data2)
		{
			this.SetData(data1, data2);
			this.ProcessList();
			this.ResetData();
		}

		// Token: 0x06006837 RID: 26679 RVA: 0x0021F9F8 File Offset: 0x0021DBF8
		protected override void ProcessItem(in InAction<T1, T2> item)
		{
			item(this.m_data1, this.m_data2);
		}

		// Token: 0x06006838 RID: 26680 RVA: 0x0021FA0D File Offset: 0x0021DC0D
		private void SetData(in T1 data1, in T2 data2)
		{
			this.m_data1 = data1;
			this.m_data2 = data2;
		}

		// Token: 0x06006839 RID: 26681 RVA: 0x0021FA27 File Offset: 0x0021DC27
		private void ResetData()
		{
			this.m_data1 = default(T1);
			this.m_data2 = default(T2);
		}

		// Token: 0x040076F6 RID: 30454
		private T1 m_data1;

		// Token: 0x040076F7 RID: 30455
		private T2 m_data2;
	}
}
