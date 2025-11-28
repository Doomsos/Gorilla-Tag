using System;

namespace GorillaTag
{
	// Token: 0x02001015 RID: 4117
	public class InDelegateListProcessor<T> : DelegateListProcessorPlusMinus<InDelegateListProcessor<T>, InAction<T>>
	{
		// Token: 0x0600682E RID: 26670 RVA: 0x0021F95B File Offset: 0x0021DB5B
		public InDelegateListProcessor()
		{
		}

		// Token: 0x0600682F RID: 26671 RVA: 0x0021F963 File Offset: 0x0021DB63
		public InDelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x06006830 RID: 26672 RVA: 0x0021F96C File Offset: 0x0021DB6C
		public void InvokeSafe(in T data)
		{
			this.m_data = data;
			this.ProcessListSafe();
			this.m_data = default(T);
		}

		// Token: 0x06006831 RID: 26673 RVA: 0x0021F98C File Offset: 0x0021DB8C
		public void Invoke(in T data)
		{
			this.m_data = data;
			this.ProcessList();
			this.m_data = default(T);
		}

		// Token: 0x06006832 RID: 26674 RVA: 0x0021F9AC File Offset: 0x0021DBAC
		protected override void ProcessItem(in InAction<T> item)
		{
			item(this.m_data);
		}

		// Token: 0x040076F5 RID: 30453
		private T m_data;
	}
}
