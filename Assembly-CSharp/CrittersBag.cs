using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class CrittersBag : CrittersActor
{
	// Token: 0x06000172 RID: 370 RVA: 0x000096FB File Offset: 0x000078FB
	protected override void Awake()
	{
		base.Awake();
		this.overlapColliders = new Collider[20];
		this.attachedColliders = new Dictionary<int, GameObject>();
		this.isAttachedToPlayer = false;
	}

	// Token: 0x06000173 RID: 371 RVA: 0x00009722 File Offset: 0x00007922
	public override void OnHover(bool isLeft)
	{
		if (this.isAttachedToPlayer)
		{
			GorillaTagger.Instance.StartVibration(isLeft, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			return;
		}
		base.OnHover(isLeft);
	}

	// Token: 0x06000174 RID: 372 RVA: 0x00009760 File Offset: 0x00007960
	protected override void CleanupActor()
	{
		base.CleanupActor();
		for (int i = this.attachedColliders.Count - 1; i >= 0; i--)
		{
			this.attachedColliders[Enumerable.ElementAt<KeyValuePair<int, GameObject>>(this.attachedColliders, i).Key].gameObject.Destroy();
		}
		this.attachedColliders.Clear();
	}

	// Token: 0x06000175 RID: 373 RVA: 0x000097C0 File Offset: 0x000079C0
	protected override void GlobalGrabbedBy(CrittersActor grabbedBy)
	{
		base.GlobalGrabbedBy(grabbedBy);
		bool flag = this.attachedToLocalPlayer;
		if (grabbedBy.IsNotNull())
		{
			CrittersAttachPoint crittersAttachPoint = grabbedBy as CrittersAttachPoint;
			if (crittersAttachPoint != null)
			{
				this.isAttachedToPlayer = true;
				this.attachedToLocalPlayer = (crittersAttachPoint.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber);
				goto IL_4F;
			}
		}
		this.isAttachedToPlayer = false;
		this.attachedToLocalPlayer = false;
		IL_4F:
		if (this.attachedToLocalPlayer != flag)
		{
			bool flag2 = this.attachedToLocalPlayer || flag;
			this.audioSrc.transform.localPosition = Vector3.zero;
			this.audioSrc.GTPlayOneShot(this.attachedToLocalPlayer ? this.equipSound : this.unequipSound, flag2 ? 1f : 0.5f);
		}
	}

	// Token: 0x06000176 RID: 374 RVA: 0x00009873 File Offset: 0x00007A73
	public override void GrabbedBy(CrittersActor grabbedBy, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbedBy, positionOverride, localRotation, localOffset, disableGrabbing);
	}

	// Token: 0x06000177 RID: 375 RVA: 0x00009884 File Offset: 0x00007A84
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulse = default(Vector3), Vector3 impulseRotation = default(Vector3))
	{
		if (this.parentActorId >= 0)
		{
			base.AttemptRemoveStoredObjectCollider(this.parentActorId, true);
		}
		int num = Physics.OverlapBoxNonAlloc(this.dropCube.transform.position, this.dropCube.size / 2f, this.overlapColliders, this.dropCube.transform.rotation, CrittersManager.instance.objectLayers, 2);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				Rigidbody attachedRigidbody = this.overlapColliders[i].attachedRigidbody;
				if (!(attachedRigidbody == null))
				{
					CrittersAttachPoint component = attachedRigidbody.GetComponent<CrittersAttachPoint>();
					if (!(component == null) && component.anchorLocation == this.anchorLocation && !(component.GetComponentInChildren<CrittersBag>() != null))
					{
						CrittersActor crittersActor;
						if (this.lastGrabbedPlayer == PhotonNetwork.LocalPlayer.ActorNumber && CrittersManager.instance.actorById.TryGetValue(this.parentActorId, ref crittersActor))
						{
							CrittersGrabber crittersGrabber = crittersActor as CrittersGrabber;
							if (crittersGrabber != null)
							{
								GorillaTagger.Instance.StartVibration(crittersGrabber.isLeft, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
							}
						}
						this.GrabbedBy(component, true, default(Quaternion), default(Vector3), false);
						return;
					}
				}
			}
		}
		base.Released(keepWorldPosition, rotation, position, impulse, impulseRotation);
	}

	// Token: 0x06000178 RID: 376 RVA: 0x000099F4 File Offset: 0x00007BF4
	public void AddStoredObjectCollider(CrittersActor actor)
	{
		if (this.attachedColliders.ContainsKey(actor.actorId))
		{
			if (this.attachedColliders[actor.actorId].IsNull())
			{
				this.attachedColliders[actor.actorId] = CrittersManager.DuplicateCapsuleCollider(base.transform, actor.storeCollider).gameObject;
			}
		}
		else
		{
			this.attachedColliders.Add(actor.actorId, CrittersManager.DuplicateCapsuleCollider(base.transform, actor.storeCollider).gameObject);
		}
		this.audioSrc.transform.position = actor.transform.position;
		this.audioSrc.GTPlayOneShot(this.attachSound, 1f);
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00009AB0 File Offset: 0x00007CB0
	public void RemoveStoredObjectCollider(CrittersActor actor, bool playSound = true)
	{
		GameObject gameObject;
		if (this.attachedColliders.TryGetValue(actor.actorId, ref gameObject))
		{
			Object.Destroy(gameObject);
			this.attachedColliders.Remove(actor.actorId);
		}
		if (playSound)
		{
			this.audioSrc.transform.position = actor.transform.position;
			this.audioSrc.GTPlayOneShot(this.detachSound, 1f);
		}
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00009B1E File Offset: 0x00007D1E
	public bool IsActorValidStore(CrittersActor actor)
	{
		return this.blockAttachTypes == null || !this.blockAttachTypes.Contains(actor.crittersActorType);
	}

	// Token: 0x0400019D RID: 413
	public AudioSource audioSrc;

	// Token: 0x0400019E RID: 414
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x0400019F RID: 415
	public Collider attachableCollider;

	// Token: 0x040001A0 RID: 416
	public BoxCollider dropCube;

	// Token: 0x040001A1 RID: 417
	private Collider[] overlapColliders;

	// Token: 0x040001A2 RID: 418
	public List<Collider> attachDisableColliders;

	// Token: 0x040001A3 RID: 419
	public Dictionary<int, GameObject> attachedColliders;

	// Token: 0x040001A4 RID: 420
	[Header("Child object attachment sounds")]
	public AudioClip attachSound;

	// Token: 0x040001A5 RID: 421
	public AudioClip detachSound;

	// Token: 0x040001A6 RID: 422
	[Header("Monke equip sounds")]
	public AudioClip equipSound;

	// Token: 0x040001A7 RID: 423
	public AudioClip unequipSound;

	// Token: 0x040001A8 RID: 424
	[Header("Attachment Blocking")]
	public List<CrittersActor.CrittersActorType> blockAttachTypes;

	// Token: 0x040001A9 RID: 425
	private bool isAttachedToPlayer;

	// Token: 0x040001AA RID: 426
	private bool attachedToLocalPlayer;
}
