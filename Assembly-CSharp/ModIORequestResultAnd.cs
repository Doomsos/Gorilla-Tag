using System;

// Token: 0x02000966 RID: 2406
public struct ModIORequestResultAnd<T>
{
	// Token: 0x06003DA2 RID: 15778 RVA: 0x001467EC File Offset: 0x001449EC
	public static ModIORequestResultAnd<T> CreateFailureResult(string inMessage)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateFailureResult(inMessage)
		};
	}

	// Token: 0x06003DA3 RID: 15779 RVA: 0x00146810 File Offset: 0x00144A10
	public static ModIORequestResultAnd<T> CreateSuccessResult(T payload)
	{
		return new ModIORequestResultAnd<T>
		{
			result = ModIORequestResult.CreateSuccessResult(),
			data = payload
		};
	}

	// Token: 0x04004E36 RID: 20022
	public ModIORequestResult result;

	// Token: 0x04004E37 RID: 20023
	public T data;
}
