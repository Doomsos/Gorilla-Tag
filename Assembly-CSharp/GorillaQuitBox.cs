using System;
using UnityEngine;

// Token: 0x02000523 RID: 1315
public class GorillaQuitBox : GorillaTriggerBox
{
	// Token: 0x06002161 RID: 8545 RVA: 0x00002789 File Offset: 0x00000989
	private void Start()
	{
	}

	// Token: 0x06002162 RID: 8546 RVA: 0x000AF63D File Offset: 0x000AD83D
	public override void OnBoxTriggered()
	{
		Debug.Log("quitbox hit! hopefully you expected this to happen!");
		Application.Quit();
	}
}
