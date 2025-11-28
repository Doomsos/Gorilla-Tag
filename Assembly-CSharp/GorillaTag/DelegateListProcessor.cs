using System;

namespace GorillaTag
{
	// Token: 0x02001012 RID: 4114
	public class DelegateListProcessor : DelegateListProcessorPlusMinus<DelegateListProcessor, Action>
	{
		// Token: 0x0600681D RID: 26653 RVA: 0x0021F84B File Offset: 0x0021DA4B
		public DelegateListProcessor()
		{
		}

		// Token: 0x0600681E RID: 26654 RVA: 0x0021F853 File Offset: 0x0021DA53
		public DelegateListProcessor(int capacity) : base(capacity)
		{
		}

		// Token: 0x0600681F RID: 26655 RVA: 0x0021F85C File Offset: 0x0021DA5C
		public void Invoke()
		{
			this.ProcessList();
		}

		// Token: 0x06006820 RID: 26656 RVA: 0x0021F864 File Offset: 0x0021DA64
		public void InvokeSafe()
		{
			this.ProcessListSafe();
		}

		// Token: 0x06006821 RID: 26657 RVA: 0x0021F86C File Offset: 0x0021DA6C
		protected override void ProcessItem(in Action del)
		{
			del.Invoke();
		}
	}
}
