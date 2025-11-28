using System;

namespace GorillaTag
{
	// Token: 0x02001014 RID: 4116
	public class DelegateListProcessor<T1, T2> : DelegateListProcessorPlusMinus<DelegateListProcessor<T1, T2>, Action<T1, T2>>
	{
		// Token: 0x06006827 RID: 26663 RVA: 0x0021F8D5 File Offset: 0x0021DAD5
		public DelegateListProcessor()
		{
		}

		// Token: 0x06006828 RID: 26664 RVA: 0x0021F8DD File Offset: 0x0021DADD
		public DelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x06006829 RID: 26665 RVA: 0x0021F8E6 File Offset: 0x0021DAE6
		public void InvokeSafe(in T1 data1, in T2 data2)
		{
			this.SetData(data1, data2);
			this.ProcessListSafe();
			this.ResetData();
		}

		// Token: 0x0600682A RID: 26666 RVA: 0x0021F8FC File Offset: 0x0021DAFC
		public void Invoke(in T1 data1, in T2 data2)
		{
			this.SetData(data1, data2);
			this.ProcessList();
			this.ResetData();
		}

		// Token: 0x0600682B RID: 26667 RVA: 0x0021F912 File Offset: 0x0021DB12
		protected override void ProcessItem(in Action<T1, T2> item)
		{
			item.Invoke(this.m_data1, this.m_data2);
		}

		// Token: 0x0600682C RID: 26668 RVA: 0x0021F927 File Offset: 0x0021DB27
		private void SetData(in T1 data1, in T2 data2)
		{
			this.m_data1 = data1;
			this.m_data2 = data2;
		}

		// Token: 0x0600682D RID: 26669 RVA: 0x0021F941 File Offset: 0x0021DB41
		private void ResetData()
		{
			this.m_data1 = default(T1);
			this.m_data2 = default(T2);
		}

		// Token: 0x040076F3 RID: 30451
		private T1 m_data1;

		// Token: 0x040076F4 RID: 30452
		private T2 m_data2;
	}
}
