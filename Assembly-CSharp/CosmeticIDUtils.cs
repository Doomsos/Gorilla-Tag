using System;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Mathematics;

// Token: 0x0200028D RID: 653
public static class CosmeticIDUtils
{
	// Token: 0x060010C9 RID: 4297 RVA: 0x0005763D File Offset: 0x0005583D
	public static int PlayFabIdToIndexInCategory(string playFabIdString)
	{
		return CosmeticIDUtils._PlayFabIdToInt(playFabIdString, 2);
	}

	// Token: 0x060010CA RID: 4298 RVA: 0x00057646 File Offset: 0x00055846
	public static int PlayFabIdToInt(string playFabIdString)
	{
		return CosmeticIDUtils._PlayFabIdToInt(playFabIdString, 1);
	}

	// Token: 0x060010CB RID: 4299 RVA: 0x00057650 File Offset: 0x00055850
	[MethodImpl(256)]
	private static int _PlayFabIdToInt(string playFabIdString, int start)
	{
		if (playFabIdString == null)
		{
			throw new ArgumentException("_PlayFabIdToInt: playFabId cannot be null.");
		}
		if (playFabIdString.Length < 6)
		{
			throw new ArgumentException("_PlayFabIdToInt: playFabId \"" + playFabIdString + "\" cannot be less than 6 chars.");
		}
		if (playFabIdString.Length > 8)
		{
			throw new ArgumentException("_PlayFabIdToInt: playFabId \"" + playFabIdString + "\" cannot be greater than 8 chars.");
		}
		if (playFabIdString.get_Chars(0) != 'L' || playFabIdString.get_Chars(playFabIdString.Length - 1) != '.')
		{
			throw new ArgumentException("PlayFabIdToIndexInCategory: playFabId must start with 'L' and end with '.', instead got " + playFabIdString + ".");
		}
		int num = playFabIdString.Length - 2;
		int num2 = 0;
		for (int i = start; i <= num; i++)
		{
			char c = playFabIdString.get_Chars(i);
			if (c < 'A' || c > 'Z')
			{
				throw new ArgumentException("String must contain only uppercase letters A-Z.");
			}
			int num3 = (int)(playFabIdString.get_Chars(i) - 'A');
			num2 += num3 * (int)math.pow(26f, (float)(num - i));
		}
		return num2;
	}

	// Token: 0x060010CC RID: 4300 RVA: 0x00057734 File Offset: 0x00055934
	[MethodImpl(256)]
	public static string IntToPlayFabId(int id)
	{
		if (id < 0)
		{
			throw new ArgumentException("Input integer cannot be negative.", "id");
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (id == 0)
		{
			stringBuilder.Append('A');
		}
		else
		{
			for (int i = id; i > 0; i /= 26)
			{
				int num = i % 26;
				char c = (char)(65 + num);
				stringBuilder.Insert(0, c);
			}
		}
		stringBuilder.Insert(0, 'L');
		stringBuilder.Append('.');
		return stringBuilder.ToString();
	}
}
