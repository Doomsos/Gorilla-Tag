using System;

// Token: 0x02000A2B RID: 2603
public class VerifyAgeData
{
	// Token: 0x0600420B RID: 16907 RVA: 0x0015D610 File Offset: 0x0015B810
	public VerifyAgeData(VerifyAgeResponse response, int? age)
	{
		if (response == null)
		{
			return;
		}
		this.Status = response.Status;
		if (response.Session == null && response.DefaultSession == null)
		{
			return;
		}
		this.Session = new TMPSession(response.Session, response.DefaultSession, age, this.Status);
	}

	// Token: 0x04005310 RID: 21264
	public readonly SessionStatus Status;

	// Token: 0x04005311 RID: 21265
	public readonly TMPSession Session;
}
