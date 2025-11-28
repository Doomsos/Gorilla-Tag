using System;
using System.Collections.Generic;

namespace Viveport
{
	// Token: 0x02000D13 RID: 3347
	public class SubscriptionStatus
	{
		// Token: 0x17000799 RID: 1945
		// (get) Token: 0x06005135 RID: 20789 RVA: 0x001A22E6 File Offset: 0x001A04E6
		// (set) Token: 0x06005136 RID: 20790 RVA: 0x001A22EE File Offset: 0x001A04EE
		public List<SubscriptionStatus.Platform> Platforms { get; set; }

		// Token: 0x1700079A RID: 1946
		// (get) Token: 0x06005137 RID: 20791 RVA: 0x001A22F7 File Offset: 0x001A04F7
		// (set) Token: 0x06005138 RID: 20792 RVA: 0x001A22FF File Offset: 0x001A04FF
		public SubscriptionStatus.TransactionType Type { get; set; }

		// Token: 0x06005139 RID: 20793 RVA: 0x001A2308 File Offset: 0x001A0508
		public SubscriptionStatus()
		{
			this.Platforms = new List<SubscriptionStatus.Platform>();
			this.Type = SubscriptionStatus.TransactionType.Unknown;
		}

		// Token: 0x02000D14 RID: 3348
		public enum Platform
		{
			// Token: 0x04006055 RID: 24661
			Windows,
			// Token: 0x04006056 RID: 24662
			Android
		}

		// Token: 0x02000D15 RID: 3349
		public enum TransactionType
		{
			// Token: 0x04006058 RID: 24664
			Unknown,
			// Token: 0x04006059 RID: 24665
			Paid,
			// Token: 0x0400605A RID: 24666
			Redeem,
			// Token: 0x0400605B RID: 24667
			FreeTrial
		}
	}
}
