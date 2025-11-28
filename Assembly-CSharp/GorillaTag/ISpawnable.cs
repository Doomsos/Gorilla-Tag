using System;
using GorillaTag.CosmeticSystem;

namespace GorillaTag
{
	// Token: 0x02000FD0 RID: 4048
	public interface ISpawnable
	{
		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x0600669E RID: 26270
		// (set) Token: 0x0600669F RID: 26271
		bool IsSpawned { get; set; }

		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x060066A0 RID: 26272
		// (set) Token: 0x060066A1 RID: 26273
		ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x060066A2 RID: 26274
		void OnSpawn(VRRig rig);

		// Token: 0x060066A3 RID: 26275
		void OnDespawn();
	}
}
