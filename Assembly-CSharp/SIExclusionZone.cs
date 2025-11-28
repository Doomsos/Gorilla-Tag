using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class SIExclusionZone : MonoBehaviour
{
	// Token: 0x0600070C RID: 1804 RVA: 0x00026AC4 File Offset: 0x00024CC4
	private void OnDisable()
	{
		foreach (SIGadget sigadget in this.gadgetsInZone)
		{
			if (sigadget != null)
			{
				sigadget.LeaveExclusionZone(this);
			}
		}
		this.gadgetsInZone.Clear();
		foreach (SIPlayer siplayer in this.playersInZone)
		{
			if (siplayer != null)
			{
				siplayer.exclusionZoneCount--;
			}
		}
		this.playersInZone.Clear();
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x00026B88 File Offset: 0x00024D88
	private void OnTriggerEnter(Collider other)
	{
		SIGadget componentInParent = other.GetComponentInParent<SIGadget>();
		if (componentInParent != null)
		{
			if (!this.gadgetsInZone.Contains(componentInParent))
			{
				this.gadgetsInZone.Add(componentInParent);
			}
			componentInParent.ApplyExclusionZone(this);
		}
		SIPlayer componentInParent2 = other.GetComponentInParent<SIPlayer>();
		if (componentInParent2 != null && !this.playersInZone.Contains(componentInParent2))
		{
			this.playersInZone.Add(componentInParent2);
			componentInParent2.exclusionZoneCount++;
		}
	}

	// Token: 0x0600070E RID: 1806 RVA: 0x00026C00 File Offset: 0x00024E00
	private void OnTriggerExit(Collider other)
	{
		SIGadget componentInParent = other.GetComponentInParent<SIGadget>();
		if (componentInParent != null && this.gadgetsInZone.Contains(componentInParent))
		{
			componentInParent.LeaveExclusionZone(this);
			this.gadgetsInZone.Remove(componentInParent);
		}
		SIPlayer componentInParent2 = other.GetComponentInParent<SIPlayer>();
		if (componentInParent2 != null && this.playersInZone.Contains(componentInParent2))
		{
			this.playersInZone.Remove(componentInParent2);
			componentInParent2.exclusionZoneCount--;
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00026C78 File Offset: 0x00024E78
	public void ClearGadget(SIGadget gadget)
	{
		this.gadgetsInZone.Remove(gadget);
	}

	// Token: 0x040008E7 RID: 2279
	private List<SIGadget> gadgetsInZone = new List<SIGadget>();

	// Token: 0x040008E8 RID: 2280
	private List<SIPlayer> playersInZone = new List<SIPlayer>();
}
