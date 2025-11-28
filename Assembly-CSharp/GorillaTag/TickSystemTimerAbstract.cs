using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x0200100E RID: 4110
	[Serializable]
	internal abstract class TickSystemTimerAbstract : CoolDownHelper, ITickSystemPre
	{
		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x06006807 RID: 26631 RVA: 0x0021F64C File Offset: 0x0021D84C
		// (set) Token: 0x06006808 RID: 26632 RVA: 0x0021F654 File Offset: 0x0021D854
		bool ITickSystemPre.PreTickRunning
		{
			get
			{
				return this.registered;
			}
			set
			{
				this.registered = value;
			}
		}

		// Token: 0x170009C8 RID: 2504
		// (get) Token: 0x06006809 RID: 26633 RVA: 0x0021F64C File Offset: 0x0021D84C
		public bool Running
		{
			get
			{
				return this.registered;
			}
		}

		// Token: 0x0600680A RID: 26634 RVA: 0x0021F65D File Offset: 0x0021D85D
		protected TickSystemTimerAbstract()
		{
		}

		// Token: 0x0600680B RID: 26635 RVA: 0x0021F665 File Offset: 0x0021D865
		protected TickSystemTimerAbstract(float cd) : base(cd)
		{
		}

		// Token: 0x0600680C RID: 26636 RVA: 0x0021F66E File Offset: 0x0021D86E
		public override void Start()
		{
			base.Start();
			TickSystem<object>.AddPreTickCallback(this);
		}

		// Token: 0x0600680D RID: 26637 RVA: 0x0021F67C File Offset: 0x0021D87C
		public override void Stop()
		{
			base.Stop();
			TickSystem<object>.RemovePreTickCallback(this);
		}

		// Token: 0x0600680E RID: 26638 RVA: 0x0021F68A File Offset: 0x0021D88A
		public override void OnCheckPass()
		{
			this.OnTimedEvent();
		}

		// Token: 0x0600680F RID: 26639
		public abstract void OnTimedEvent();

		// Token: 0x06006810 RID: 26640 RVA: 0x0021F692 File Offset: 0x0021D892
		[MethodImpl(256)]
		void ITickSystemPre.PreTick()
		{
			base.CheckCooldown();
		}

		// Token: 0x040076EE RID: 30446
		[NonSerialized]
		internal bool registered;
	}
}
