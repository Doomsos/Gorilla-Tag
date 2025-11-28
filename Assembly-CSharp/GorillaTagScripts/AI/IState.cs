using System;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000E95 RID: 3733
	public interface IState
	{
		// Token: 0x06005D5D RID: 23901
		void Tick();

		// Token: 0x06005D5E RID: 23902
		void OnEnter();

		// Token: 0x06005D5F RID: 23903
		void OnExit();
	}
}
