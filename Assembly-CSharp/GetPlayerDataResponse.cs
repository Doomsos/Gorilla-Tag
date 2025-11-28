using System;
using System.Runtime.CompilerServices;
using KID.Model;

// Token: 0x02000A1F RID: 2591
[Serializable]
public class GetPlayerDataResponse
{
	// Token: 0x040052E3 RID: 21219
	public SessionStatus? Status;

	// Token: 0x040052E4 RID: 21220
	public Session Session;

	// Token: 0x040052E5 RID: 21221
	public int? Age;

	// Token: 0x040052E6 RID: 21222
	public AgeStatusType? AgeStatus;

	// Token: 0x040052E7 RID: 21223
	public KIDDefaultSession DefaultSession;

	// Token: 0x040052E8 RID: 21224
	[Nullable(new byte[]
	{
		2,
		0
	})]
	public string[] Permissions;

	// Token: 0x040052E9 RID: 21225
	public bool HasConfirmedSetup;
}
