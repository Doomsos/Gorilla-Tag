using System;
using UnityEngine;

// Token: 0x02000046 RID: 70
public class CrittersActorSettings : MonoBehaviour
{
	// Token: 0x06000159 RID: 345 RVA: 0x00009296 File Offset: 0x00007496
	public virtual void OnEnable()
	{
		this.UpdateActorSettings();
	}

	// Token: 0x0600015A RID: 346 RVA: 0x000092A0 File Offset: 0x000074A0
	public virtual void UpdateActorSettings()
	{
		this.parentActor.usesRB = this.usesRB;
		this.parentActor.rb.isKinematic = !this.usesRB;
		this.parentActor.equipmentStorable = this.canBeStored;
		this.parentActor.storeCollider = this.storeCollider;
		this.parentActor.equipmentStoreTriggerCollider = this.equipmentStoreTriggerCollider;
	}

	// Token: 0x0400017F RID: 383
	public CrittersActor parentActor;

	// Token: 0x04000180 RID: 384
	public bool usesRB;

	// Token: 0x04000181 RID: 385
	public bool canBeStored;

	// Token: 0x04000182 RID: 386
	public CapsuleCollider storeCollider;

	// Token: 0x04000183 RID: 387
	public CapsuleCollider equipmentStoreTriggerCollider;
}
