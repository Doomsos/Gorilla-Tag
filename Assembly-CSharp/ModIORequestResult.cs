using System;
using Modio;

// Token: 0x02000965 RID: 2405
public struct ModIORequestResult
{
	// Token: 0x06003D9F RID: 15775 RVA: 0x00146744 File Offset: 0x00144944
	public static ModIORequestResult CreateFailureResult(string inMessage)
	{
		ModIORequestResult result;
		result.success = false;
		result.message = inMessage;
		return result;
	}

	// Token: 0x06003DA0 RID: 15776 RVA: 0x00146764 File Offset: 0x00144964
	public static ModIORequestResult CreateSuccessResult()
	{
		ModIORequestResult result;
		result.success = true;
		result.message = "";
		return result;
	}

	// Token: 0x06003DA1 RID: 15777 RVA: 0x00146788 File Offset: 0x00144988
	public static ModIORequestResult CreateFromError(Error error)
	{
		ModIORequestResult result;
		if (error)
		{
			result.success = false;
			result.message = error.GetMessage();
		}
		else
		{
			result.success = true;
			result.message = "";
		}
		return result;
	}

	// Token: 0x04004E34 RID: 20020
	public bool success;

	// Token: 0x04004E35 RID: 20021
	public string message;
}
