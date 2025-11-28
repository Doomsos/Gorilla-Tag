using System;

// Token: 0x02000BE1 RID: 3041
[Serializable]
public class CallLimitType<T> where T : CallLimiter
{
	// Token: 0x06004B20 RID: 19232 RVA: 0x0018867F File Offset: 0x0018687F
	public static implicit operator CallLimitType<CallLimiter>(CallLimitType<T> clt)
	{
		return new CallLimitType<CallLimiter>
		{
			Key = clt.Key,
			UseNetWorkTime = clt.UseNetWorkTime,
			CallLimitSettings = clt.CallLimitSettings
		};
	}

	// Token: 0x04005B45 RID: 23365
	public FXType Key;

	// Token: 0x04005B46 RID: 23366
	public bool UseNetWorkTime;

	// Token: 0x04005B47 RID: 23367
	public T CallLimitSettings;
}
