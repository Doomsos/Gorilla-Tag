using System;

namespace Cosmetics
{
	// Token: 0x02000FC7 RID: 4039
	public interface ICreatorCodeProvider
	{
		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x0600666B RID: 26219
		string TerminalId { get; }

		// Token: 0x0600666C RID: 26220
		void GetCreatorCode(out string code, out NexusGroupId[] groups);
	}
}
