using System;

namespace LitJson
{
	// Token: 0x02000D55 RID: 3413
	public class JsonException : ApplicationException
	{
		// Token: 0x0600537A RID: 21370 RVA: 0x001A6649 File Offset: 0x001A4849
		public JsonException()
		{
		}

		// Token: 0x0600537B RID: 21371 RVA: 0x001A6651 File Offset: 0x001A4851
		internal JsonException(ParserToken token) : base(string.Format("Invalid token '{0}' in input string", token))
		{
		}

		// Token: 0x0600537C RID: 21372 RVA: 0x001A6669 File Offset: 0x001A4869
		internal JsonException(ParserToken token, Exception inner_exception) : base(string.Format("Invalid token '{0}' in input string", token), inner_exception)
		{
		}

		// Token: 0x0600537D RID: 21373 RVA: 0x001A6682 File Offset: 0x001A4882
		internal JsonException(int c) : base(string.Format("Invalid character '{0}' in input string", (char)c))
		{
		}

		// Token: 0x0600537E RID: 21374 RVA: 0x001A669B File Offset: 0x001A489B
		internal JsonException(int c, Exception inner_exception) : base(string.Format("Invalid character '{0}' in input string", (char)c), inner_exception)
		{
		}

		// Token: 0x0600537F RID: 21375 RVA: 0x001A66B5 File Offset: 0x001A48B5
		public JsonException(string message) : base(message)
		{
		}

		// Token: 0x06005380 RID: 21376 RVA: 0x001A66BE File Offset: 0x001A48BE
		public JsonException(string message, Exception inner_exception) : base(message, inner_exception)
		{
		}
	}
}
