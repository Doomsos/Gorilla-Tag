using System;

// Token: 0x02000A2A RID: 2602
public class UpgradeSessionData
{
	// Token: 0x0600420A RID: 16906 RVA: 0x0015D5D0 File Offset: 0x0015B7D0
	public UpgradeSessionData(UpgradeSessionResponse response)
	{
		this.status = response.status;
		this.session = new TMPSession(response.session, null, default(int?), this.status);
	}

	// Token: 0x0400530E RID: 21262
	public readonly SessionStatus status;

	// Token: 0x0400530F RID: 21263
	public readonly TMPSession session;
}
