using System;

namespace PerformanceSystems
{
	// Token: 0x02000D84 RID: 3460
	public interface ITimeSlice
	{
		// Token: 0x060054CA RID: 21706
		void SliceUpdate();

		// Token: 0x060054CB RID: 21707
		void SliceUpdateAlways(float deltaTime);

		// Token: 0x060054CC RID: 21708
		void SliceUpdate(float deltaTime);
	}
}
