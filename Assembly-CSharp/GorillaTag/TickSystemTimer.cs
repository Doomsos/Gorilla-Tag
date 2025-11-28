using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x0200100F RID: 4111
	[Serializable]
	internal class TickSystemTimer : TickSystemTimerAbstract
	{
		// Token: 0x06006811 RID: 26641 RVA: 0x0021F01E File Offset: 0x0021D21E
		public TickSystemTimer()
		{
		}

		// Token: 0x06006812 RID: 26642 RVA: 0x0021F6BB File Offset: 0x0021D8BB
		public TickSystemTimer(float cd) : base(cd)
		{
		}

		// Token: 0x06006813 RID: 26643 RVA: 0x0021F6C4 File Offset: 0x0021D8C4
		public TickSystemTimer(float cd, Action cb) : base(cd)
		{
			this.callback = cb;
		}

		// Token: 0x06006814 RID: 26644 RVA: 0x0021F6D4 File Offset: 0x0021D8D4
		public TickSystemTimer(Action cb)
		{
			this.callback = cb;
		}

		// Token: 0x06006815 RID: 26645 RVA: 0x0021F6E3 File Offset: 0x0021D8E3
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
