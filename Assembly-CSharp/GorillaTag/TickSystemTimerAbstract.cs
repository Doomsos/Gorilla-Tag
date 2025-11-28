using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x0200100E RID: 4110
	[Serializable]
	internal abstract class TickSystemTimerAbstract : CoolDownHelper, ITickSystemPre
	{
		// Token: 0x170009C7 RID: 2503
		// (get) Token: 0x06006807 RID: 26631 RVA: 0x0021F66C File Offset: 0x0021D86C
		// (set) Token: 0x06006808 RID: 26632 RVA: 0x0021F674 File Offset: 0x0021D874
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
		// (get) Token: 0x06006809 RID: 26633 RVA: 0x0021F66C File Offset: 0x0021D86C
		public bool Running
		{
			get
			{
				return this.registered;
			}
		}

		// Token: 0x0600680A RID: 26634 RVA: 0x0021F67D File Offset: 0x0021D87D
		protected TickSystemTimerAbstract()
		{
		}

		// Token: 0x0600680B RID: 26635 RVA: 0x0021F685 File Offset: 0x0021D885
		protected TickSystemTimerAbstract(float cd) : base(cd)
		{
		}

		// Token: 0x0600680C RID: 26636 RVA: 0x0021F68E File Offset: 0x0021D88E
		public override void Start()
		{
			base.Start();
			TickSystem<object>.AddPreTickCallback(this);
		}

		// Token: 0x0600680D RID: 26637 RVA: 0x0021F69C File Offset: 0x0021D89C
		public override void Stop()
		{
			base.Stop();
			TickSystem<object>.RemovePreTickCallback(this);
		}

		// Token: 0x0600680E RID: 26638 RVA: 0x0021F6AA File Offset: 0x0021D8AA
		public override void OnCheckPass()
		{
			this.OnTimedEvent();
		}

		// Token: 0x0600680F RID: 26639
		public abstract void OnTimedEvent();

		// Token: 0x06006810 RID: 26640 RVA: 0x0021F6B2 File Offset: 0x0021D8B2
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
