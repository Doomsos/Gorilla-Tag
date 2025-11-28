using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000EE RID: 238
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
public class SIGadgetHolster : SIGadget, I_SIDisruptable
{
	// Token: 0x060005CA RID: 1482 RVA: 0x00020C8F File Offset: 0x0001EE8F
	private void Start()
	{
		this.gtPlayer = GTPlayer.Instance;
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x00002789 File Offset: 0x00000989
	public void Disrupt(float disruptTime)
	{
	}

	// Token: 0x04000724 RID: 1828
	[SerializeField]
	private Image imageMask;

	// Token: 0x04000725 RID: 1829
	public List<SuperInfectionSnapPoint> snapPoints;

	// Token: 0x04000726 RID: 1830
	private SIGadgetHolster.State state;

	// Token: 0x04000727 RID: 1831
	private GTPlayer gtPlayer;

	// Token: 0x020000EF RID: 239
	private enum State
	{
		// Token: 0x04000729 RID: 1833
		Unequipped,
		// Token: 0x0400072A RID: 1834
		Equipped
	}
}
