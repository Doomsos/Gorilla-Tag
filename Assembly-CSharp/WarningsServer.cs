using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000AC7 RID: 2759
internal abstract class WarningsServer : MonoBehaviour
{
	// Token: 0x060044F7 RID: 17655
	public abstract Task<PlayerAgeGateWarningStatus?> FetchPlayerData(CancellationToken token);

	// Token: 0x060044F8 RID: 17656
	public abstract Task<PlayerAgeGateWarningStatus?> GetOptInFollowUpMessage(CancellationToken token);

	// Token: 0x040056CE RID: 22222
	public static volatile WarningsServer Instance;
}
