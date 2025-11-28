using System;
using UnityEngine;

// Token: 0x02000332 RID: 818
public class GorillaSetZoneTrigger : GorillaTriggerBox
{
	// Token: 0x060013C6 RID: 5062 RVA: 0x00072D03 File Offset: 0x00070F03
	public override void OnBoxTriggered()
	{
		ZoneManagement.SetActiveZones(this.zones);
	}

	// Token: 0x04001E36 RID: 7734
	[SerializeField]
	private GTZone[] zones;
}
