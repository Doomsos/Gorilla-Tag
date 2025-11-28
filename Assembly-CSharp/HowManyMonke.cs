using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlayFab;
using UnityEngine;

// Token: 0x02000355 RID: 853
public class HowManyMonke : MonoBehaviour
{
	// Token: 0x170001F1 RID: 497
	// (get) Token: 0x06001461 RID: 5217 RVA: 0x00074BFB File Offset: 0x00072DFB
	public static float RecheckDelay
	{
		get
		{
			return Mathf.Max((float)HowManyMonke.recheckDelay / 1000f, 1f);
		}
	}

	// Token: 0x06001462 RID: 5218 RVA: 0x00074C14 File Offset: 0x00072E14
	public void Start()
	{
		HowManyMonke.<Start>d__9 <Start>d__;
		<Start>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<Start>d__.<>4__this = this;
		<Start>d__.<>1__state = -1;
		<Start>d__.<>t__builder.Start<HowManyMonke.<Start>d__9>(ref <Start>d__);
	}

	// Token: 0x06001463 RID: 5219 RVA: 0x00074C4C File Offset: 0x00072E4C
	private Task FetchRecheckDelay()
	{
		HowManyMonke.<FetchRecheckDelay>d__10 <FetchRecheckDelay>d__;
		<FetchRecheckDelay>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<FetchRecheckDelay>d__.<>4__this = this;
		<FetchRecheckDelay>d__.<>1__state = -1;
		<FetchRecheckDelay>d__.<>t__builder.Start<HowManyMonke.<FetchRecheckDelay>d__10>(ref <FetchRecheckDelay>d__);
		return <FetchRecheckDelay>d__.<>t__builder.Task;
	}

	// Token: 0x06001464 RID: 5220 RVA: 0x00074C8F File Offset: 0x00072E8F
	private void onTDError(PlayFabError error)
	{
		this.state = HowManyMonke.State.READY;
		HowManyMonke.recheckDelay = 0;
	}

	// Token: 0x06001465 RID: 5221 RVA: 0x00074C9E File Offset: 0x00072E9E
	private void onTD(string obj)
	{
		this.state = HowManyMonke.State.READY;
		if (int.TryParse(obj, ref HowManyMonke.recheckDelay))
		{
			HowManyMonke.recheckDelay *= 1000;
			return;
		}
		HowManyMonke.recheckDelay = 0;
	}

	// Token: 0x06001466 RID: 5222 RVA: 0x00074CCC File Offset: 0x00072ECC
	private Task<int> FetchThisMany()
	{
		HowManyMonke.<FetchThisMany>d__13 <FetchThisMany>d__;
		<FetchThisMany>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
		<FetchThisMany>d__.<>4__this = this;
		<FetchThisMany>d__.<>1__state = -1;
		<FetchThisMany>d__.<>t__builder.Start<HowManyMonke.<FetchThisMany>d__13>(ref <FetchThisMany>d__);
		return <FetchThisMany>d__.<>t__builder.Task;
	}

	// Token: 0x04001EF6 RID: 7926
	public static int ThisMany = 12549;

	// Token: 0x04001EF7 RID: 7927
	public static Action<int> OnCheck;

	// Token: 0x04001EF8 RID: 7928
	[SerializeField]
	private string titleDataKey;

	// Token: 0x04001EF9 RID: 7929
	private HowManyMonke.State state;

	// Token: 0x04001EFA RID: 7930
	private static int recheckDelay;

	// Token: 0x04001EFB RID: 7931
	[SerializeField]
	private string CCUEndpoint;

	// Token: 0x02000356 RID: 854
	private enum State
	{
		// Token: 0x04001EFD RID: 7933
		READY,
		// Token: 0x04001EFE RID: 7934
		TD_LOOKUP,
		// Token: 0x04001EFF RID: 7935
		HMM_LOOKUP
	}

	// Token: 0x02000357 RID: 855
	private class CCUResponse
	{
		// Token: 0x04001F00 RID: 7936
		public int CCUTotal;

		// Token: 0x04001F01 RID: 7937
		public string ErrorMessage;
	}
}
