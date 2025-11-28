using System;
using Cysharp.Text;
using TMPro;

namespace GorillaExtensions
{
	// Token: 0x02000FB7 RID: 4023
	public static class GTTextMeshProExtensions
	{
		// Token: 0x06006504 RID: 25860 RVA: 0x0020FB60 File Offset: 0x0020DD60
		public static void SetTextToZString(this TMP_Text textMono, Utf16ValueStringBuilder zStringBuilder)
		{
			ArraySegment<char> arraySegment = zStringBuilder.AsArraySegment();
			textMono.SetCharArray(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}
	}
}
