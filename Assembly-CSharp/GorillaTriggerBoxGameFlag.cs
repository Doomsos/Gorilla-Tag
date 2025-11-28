using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000529 RID: 1321
public class GorillaTriggerBoxGameFlag : GorillaTriggerBox
{
	// Token: 0x0600216D RID: 8557 RVA: 0x000AF6BE File Offset: 0x000AD8BE
	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		PhotonView.Get(Object.FindAnyObjectByType<GorillaGameManager>()).RPC(this.functionName, 2, null);
	}

	// Token: 0x04002C26 RID: 11302
	public string functionName;
}
