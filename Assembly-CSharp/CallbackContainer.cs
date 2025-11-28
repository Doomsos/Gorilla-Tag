using System;
using GorillaTag;

// Token: 0x02000C36 RID: 3126
internal class CallbackContainer<T> : ListProcessorAbstract<T> where T : ICallBack
{
	// Token: 0x06004CB2 RID: 19634 RVA: 0x0018E7CC File Offset: 0x0018C9CC
	public CallbackContainer() : base(100)
	{
	}

	// Token: 0x06004CB3 RID: 19635 RVA: 0x0018E7D6 File Offset: 0x0018C9D6
	public CallbackContainer(int capacity) : base(capacity)
	{
	}

	// Token: 0x06004CB4 RID: 19636 RVA: 0x0018E7DF File Offset: 0x0018C9DF
	public void TryRunCallbacks()
	{
		this.ProcessListSafe();
	}

	// Token: 0x06004CB5 RID: 19637 RVA: 0x0018E7E8 File Offset: 0x0018C9E8
	protected override void ProcessItem(in T item)
	{
		T t = item;
		t.CallBack();
	}
}
