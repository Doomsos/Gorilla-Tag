using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000962 RID: 2402
public class ForceDisableHoverboardTrigger : MonoBehaviour
{
	// Token: 0x06003D9A RID: 15770 RVA: 0x001466A5 File Offset: 0x001448A5
	public void OnTriggerEnter(Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			this.wasEnabled = GTPlayer.Instance.isHoverAllowed;
			GTPlayer.Instance.SetHoverAllowed(false, true);
		}
	}

	// Token: 0x06003D9B RID: 15771 RVA: 0x001466D8 File Offset: 0x001448D8
	public void OnTriggerExit(Collider other)
	{
		if (!this.reEnableOnExit || !this.wasEnabled)
		{
			return;
		}
		if (this.reEnableOnlyInVStump && !GorillaComputer.instance.IsPlayerInVirtualStump())
		{
			return;
		}
		if (other == GTPlayer.Instance.headCollider)
		{
			GTPlayer.Instance.SetHoverAllowed(true, false);
		}
	}

	// Token: 0x04004E31 RID: 20017
	[Tooltip("If TRUE and the Hoverboard was enabled when the player entered this trigger, it will be re-enabled when they exit.")]
	public bool reEnableOnExit = true;

	// Token: 0x04004E32 RID: 20018
	public bool reEnableOnlyInVStump = true;

	// Token: 0x04004E33 RID: 20019
	private bool wasEnabled;
}
