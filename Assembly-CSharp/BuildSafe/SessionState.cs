using System;

namespace BuildSafe
{
	// Token: 0x02000EB9 RID: 3769
	public class SessionState
	{
		// Token: 0x170008B8 RID: 2232
		public string this[string key]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x04006BD9 RID: 27609
		public static readonly SessionState Shared = new SessionState();
	}
}
