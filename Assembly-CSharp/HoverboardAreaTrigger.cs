using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200080C RID: 2060
public class HoverboardAreaTrigger : MonoBehaviour
{
	// Token: 0x0600363B RID: 13883 RVA: 0x0012632F File Offset: 0x0012452F
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(true, false);
		}
	}

	// Token: 0x0600363C RID: 13884 RVA: 0x0012634F File Offset: 0x0012454F
	private void OnTriggerExit(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(false, false);
		}
	}
}
