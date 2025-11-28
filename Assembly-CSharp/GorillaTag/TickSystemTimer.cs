using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x0200100F RID: 4111
	[Serializable]
	internal class TickSystemTimer : TickSystemTimerAbstract
	{
		// Token: 0x06006811 RID: 26641 RVA: 0x0021EFFE File Offset: 0x0021D1FE
		public TickSystemTimer()
		{
		}

		// Token: 0x06006812 RID: 26642 RVA: 0x0021F69B File Offset: 0x0021D89B
		public TickSystemTimer(float cd) : base(cd)
		{
		}

		// Token: 0x06006813 RID: 26643 RVA: 0x0021F6A4 File Offset: 0x0021D8A4
		public TickSystemTimer(float cd, Action cb) : base(cd)
		{
			this.callback = cb;
		}

		// Token: 0x06006814 RID: 26644 RVA: 0x0021F6B4 File Offset: 0x0021D8B4
		public TickSystemTimer(Action cb)
		{
			this.callback = cb;
		}

		// Token: 0x06006815 RID: 26645 RVA: 0x0021F6C3 File Offset: 0x0021D8C3
		[MethodImpl(256)]
		public override void OnTimedEvent()
		{
			Action action = this.callback;
			if (action == null)
			{
				return;
			}
			action.Invoke();
		}

		// Token: 0x040076EF RID: 30447
		public Action callback;
	}
}
