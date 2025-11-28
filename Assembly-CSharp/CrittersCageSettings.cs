using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class CrittersCageSettings : CrittersActorSettings
{
	// Token: 0x060001A2 RID: 418 RVA: 0x0000A4D8 File Offset: 0x000086D8
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersCage crittersCage = (CrittersCage)this.parentActor;
		crittersCage.cagePosition = this.cagePoint;
		crittersCage.grabPosition = this.grabPoint;
	}

	// Token: 0x040001ED RID: 493
	public Transform cagePoint;

	// Token: 0x040001EE RID: 494
	public Transform grabPoint;
}
