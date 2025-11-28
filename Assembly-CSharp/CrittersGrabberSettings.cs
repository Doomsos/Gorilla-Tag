using System;
using UnityEngine;

// Token: 0x0200005B RID: 91
public class CrittersGrabberSettings : CrittersActorSettings
{
	// Token: 0x060001B9 RID: 441 RVA: 0x0000AA3E File Offset: 0x00008C3E
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersGrabber crittersGrabber = (CrittersGrabber)this.parentActor;
		crittersGrabber.grabPosition = this._grabPosition;
		crittersGrabber.grabDistance = this._grabDistance;
	}

	// Token: 0x04000206 RID: 518
	public Transform _grabPosition;

	// Token: 0x04000207 RID: 519
	public float _grabDistance;
}
