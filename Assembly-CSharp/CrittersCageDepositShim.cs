using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class CrittersCageDepositShim : MonoBehaviour
{
	// Token: 0x0600019F RID: 415 RVA: 0x0000A3A0 File Offset: 0x000085A0
	[ContextMenu("Copy Deposit Data To Shim")]
	private CrittersCageDeposit CopySpawnerDataInPrefab()
	{
		CrittersCageDeposit component = base.gameObject.GetComponent<CrittersCageDeposit>();
		this.cageBoxCollider = (BoxCollider)component.gameObject.GetComponent<Collider>();
		this.type = component.actorType;
		this.disableGrabOnAttach = component.disableGrabOnAttach;
		this.allowMultiAttach = component.allowMultiAttach;
		this.snapOnAttach = component.snapOnAttach;
		this.startLocation = component.depositStartLocation;
		this.endLocation = component.depositEndLocation;
		this.submitDuration = component.submitDuration;
		this.returnDuration = component.returnDuration;
		this.depositAudio = component.depositAudio;
		this.depositStartSound = component.depositStartSound;
		this.depositEmptySound = component.depositEmptySound;
		this.depositCritterSound = component.depositCritterSound;
		this.attachPointTransform = component.GetComponentInChildren<CrittersActor>().transform;
		this.visiblePlatformTransform = this.attachPointTransform.transform.GetChild(0).transform;
		return component;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000A490 File Offset: 0x00008690
	[ContextMenu("Replace Deposit With Shim")]
	private void ReplaceSpawnerWithShim()
	{
		CrittersCageDeposit crittersCageDeposit = this.CopySpawnerDataInPrefab();
		if (crittersCageDeposit.attachPoint.GetComponent<Rigidbody>() != null)
		{
			Object.DestroyImmediate(crittersCageDeposit.attachPoint.GetComponent<Rigidbody>());
		}
		Object.DestroyImmediate(crittersCageDeposit.attachPoint);
		Object.DestroyImmediate(crittersCageDeposit);
	}

	// Token: 0x040001DE RID: 478
	public BoxCollider cageBoxCollider;

	// Token: 0x040001DF RID: 479
	public CrittersActor.CrittersActorType type;

	// Token: 0x040001E0 RID: 480
	public bool disableGrabOnAttach;

	// Token: 0x040001E1 RID: 481
	public bool allowMultiAttach;

	// Token: 0x040001E2 RID: 482
	public bool snapOnAttach;

	// Token: 0x040001E3 RID: 483
	public Vector3 startLocation;

	// Token: 0x040001E4 RID: 484
	public Vector3 endLocation;

	// Token: 0x040001E5 RID: 485
	public float submitDuration;

	// Token: 0x040001E6 RID: 486
	public float returnDuration;

	// Token: 0x040001E7 RID: 487
	public AudioSource depositAudio;

	// Token: 0x040001E8 RID: 488
	public AudioClip depositStartSound;

	// Token: 0x040001E9 RID: 489
	public AudioClip depositEmptySound;

	// Token: 0x040001EA RID: 490
	public AudioClip depositCritterSound;

	// Token: 0x040001EB RID: 491
	public Transform attachPointTransform;

	// Token: 0x040001EC RID: 492
	public Transform visiblePlatformTransform;
}
