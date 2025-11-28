using System;

namespace GorillaTag
{
	// Token: 0x02001018 RID: 4120
	public abstract class ListProcessorAbstract<T> : ListProcessor<T>
	{
		// Token: 0x06006845 RID: 26693 RVA: 0x0021FC61 File Offset: 0x0021DE61
		protected ListProcessorAbstract()
		{
			this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
		}

		// Token: 0x06006846 RID: 26694 RVA: 0x0021FC7C File Offset: 0x0021DE7C
		protected ListProcessorAbstract(int capacity) : base(capacity, null)
		{
			this.m_itemProcessorDelegate = new InAction<T>(this.ProcessItem);
		}

		// Token: 0x06006847 RID: 26695
		protected abstract void ProcessItem(in T item);
	}
}
