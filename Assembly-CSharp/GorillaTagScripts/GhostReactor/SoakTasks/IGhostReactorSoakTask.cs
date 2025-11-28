using System;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02000E3F RID: 3647
	public interface IGhostReactorSoakTask
	{
		// Token: 0x1700087A RID: 2170
		// (get) Token: 0x06005AE5 RID: 23269
		bool Complete { get; }

		// Token: 0x06005AE6 RID: 23270
		bool Update();

		// Token: 0x06005AE7 RID: 23271
		void Reset();
	}
}
