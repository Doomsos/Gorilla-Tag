using System;
using UnityEngine;

// Token: 0x0200013B RID: 315
public class SIScannableHand : MonoBehaviour
{
	// Token: 0x0600085D RID: 2141 RVA: 0x0002D4C5 File Offset: 0x0002B6C5
	private void Awake()
	{
		this.parentPlayer = base.GetComponentInParent<SIPlayer>();
	}

	// Token: 0x04000A3C RID: 2620
	public SIPlayer parentPlayer;
}
