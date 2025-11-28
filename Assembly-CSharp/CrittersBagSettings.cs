using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class CrittersBagSettings : CrittersActorSettings
{
	// Token: 0x0600017C RID: 380 RVA: 0x00009B48 File Offset: 0x00007D48
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersBag crittersBag = (CrittersBag)this.parentActor;
		crittersBag.attachableCollider = this.attachableCollider;
		crittersBag.dropCube = this.dropCube;
		crittersBag.anchorLocation = this.anchorLocation;
		crittersBag.attachDisableColliders = this.attachDisableColliders;
		crittersBag.attachSound = this.attachSound;
		crittersBag.detachSound = this.detachSound;
		crittersBag.blockAttachTypes = this.blockAttachTypes;
	}

	// Token: 0x040001AB RID: 427
	public Collider attachableCollider;

	// Token: 0x040001AC RID: 428
	public BoxCollider dropCube;

	// Token: 0x040001AD RID: 429
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x040001AE RID: 430
	public List<Collider> attachDisableColliders;

	// Token: 0x040001AF RID: 431
	public AudioClip attachSound;

	// Token: 0x040001B0 RID: 432
	public AudioClip detachSound;

	// Token: 0x040001B1 RID: 433
	public List<CrittersActor.CrittersActorType> blockAttachTypes;
}
