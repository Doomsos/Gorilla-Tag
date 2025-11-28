using System;
using UnityEngine;

// Token: 0x02000AAC RID: 2732
public class KIDUI_DebugScreen : MonoBehaviour
{
	// Token: 0x0600447B RID: 17531 RVA: 0x0016AC5E File Offset: 0x00168E5E
	private void Awake()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x0600447C RID: 17532 RVA: 0x00002789 File Offset: 0x00000989
	public void OnResetUserAndQuit()
	{
	}

	// Token: 0x0600447D RID: 17533 RVA: 0x00002789 File Offset: 0x00000989
	public void OnClose()
	{
	}

	// Token: 0x0600447E RID: 17534 RVA: 0x000743B1 File Offset: 0x000725B1
	public static string GetOrCreateUsername()
	{
		return null;
	}

	// Token: 0x0600447F RID: 17535 RVA: 0x00002789 File Offset: 0x00000989
	public void ResetAll()
	{
	}

	// Token: 0x04005628 RID: 22056
	public const string KID_ENABLED_KEY = "dbg-kid-enabled";
}
