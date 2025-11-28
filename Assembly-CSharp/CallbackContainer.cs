using System;
using GorillaTag;

// Token: 0x02000C36 RID: 3126
internal class CallbackContainer<T> : ListProcessorAbstract<T> where T : ICallBack
{
	// Token: 0x06004CB2 RID: 19634 RVA: 0x0018E7AC File Offset: 0x0018C9AC
	public CallbackContainer() : base(100)
	{
	}

	// Token: 0x06004CB3 RID: 19635 RVA: 0x0018E7B6 File Offset: 0x0018C9B6
	public CallbackContainer(int capacity) : base(capacity)
	{
	}

	// Token: 0x06004CB4 RID: 19636 RVA: 0x0018E7BF File Offset: 0x0018C9BF
	public void TryRunCallbacks()
	{
		this.ProcessListSafe();
	}

	// Token: 0x06004CB5 RID: 19637 RVA: 0x0018E7C8 File Offset: 0x0018C9C8
	protected override void ProcessItem(in T item)
	{
		T t = item;
		t.CallBack();
	}
}
