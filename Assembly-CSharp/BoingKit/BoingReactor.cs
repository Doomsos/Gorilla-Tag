using System;

namespace BoingKit
{
	// Token: 0x0200119C RID: 4508
	public class BoingReactor : BoingBehavior
	{
		// Token: 0x060071BB RID: 29115 RVA: 0x00252A62 File Offset: 0x00250C62
		protected override void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060071BC RID: 29116 RVA: 0x00252A6A File Offset: 0x00250C6A
		protected override void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060071BD RID: 29117 RVA: 0x00252A72 File Offset: 0x00250C72
		public override void PrepareExecute()
		{
			base.PrepareExecute(true);
		}
	}
}
