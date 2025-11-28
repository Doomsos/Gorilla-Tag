using System;
using UnityEngine;

// Token: 0x02000400 RID: 1024
public class PlayerPrefFlags
{
	// Token: 0x060018FE RID: 6398 RVA: 0x00085B3B File Offset: 0x00083D3B
	internal static bool Check(PlayerPrefFlags.Flag flag)
	{
		return (PlayerPrefs.GetInt("PlayerPrefFlags0", 1) & (int)flag) == (int)flag;
	}

	// Token: 0x060018FF RID: 6399 RVA: 0x00085B50 File Offset: 0x00083D50
	internal static void Touch(PlayerPrefFlags.Flag flag)
	{
		bool flag2 = (PlayerPrefs.GetInt("PlayerPrefFlags0", 1) & (int)flag) == (int)flag;
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange.Invoke(flag, flag2);
		}
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x00085B84 File Offset: 0x00083D84
	internal static void TouchIf(PlayerPrefFlags.Flag flag, bool value)
	{
		int @int = PlayerPrefs.GetInt("PlayerPrefFlags0", 1);
		if (value == ((@int & (int)flag) == (int)flag) && PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange.Invoke(flag, value);
		}
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x00085BBC File Offset: 0x00083DBC
	internal static void Set(PlayerPrefFlags.Flag flag, bool value)
	{
		int num = PlayerPrefs.GetInt("PlayerPrefFlags0", 1);
		if (value)
		{
			num |= (int)flag;
		}
		else
		{
			num &= (int)(~(int)flag);
		}
		PlayerPrefs.SetInt("PlayerPrefFlags0", num);
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange.Invoke(flag, value);
		}
	}

	// Token: 0x06001902 RID: 6402 RVA: 0x00085C04 File Offset: 0x00083E04
	internal static bool Flip(PlayerPrefFlags.Flag flag)
	{
		int num = PlayerPrefs.GetInt("PlayerPrefFlags0", 1);
		bool flag2 = (num & (int)flag) != (int)flag;
		if (flag2)
		{
			num |= (int)flag;
		}
		else
		{
			num &= (int)(~(int)flag);
		}
		PlayerPrefs.SetInt("PlayerPrefFlags0", num);
		if (PlayerPrefFlags.OnFlagChange != null)
		{
			PlayerPrefFlags.OnFlagChange.Invoke(flag, flag2);
		}
		return flag2;
	}

	// Token: 0x04002255 RID: 8789
	public static Action<PlayerPrefFlags.Flag, bool> OnFlagChange;

	// Token: 0x04002256 RID: 8790
	private const int defaultValue = 1;

	// Token: 0x02000401 RID: 1025
	public enum Flag
	{
		// Token: 0x04002258 RID: 8792
		SHOW_1P_COSMETICS = 1,
		// Token: 0x04002259 RID: 8793
		SWAP_HELD_COSMETICS
	}
}
