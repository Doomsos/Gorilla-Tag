using System;
using UnityEngine;

// Token: 0x02000C20 RID: 3104
public static class AnimatorUtils
{
	// Token: 0x06004C58 RID: 19544 RVA: 0x0018D2B8 File Offset: 0x0018B4B8
	public static void ResetToEntryState(this Animator a)
	{
		if (a == null)
		{
			return;
		}
		a.Rebind();
		a.Update(0f);
	}
}
