using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000042 RID: 66
public class CrittersActorDeposit : MonoBehaviour
{
	// Token: 0x06000132 RID: 306 RVA: 0x000081F0 File Offset: 0x000063F0
	public void OnTriggerEnter(Collider other)
	{
		if (other.attachedRigidbody.IsNotNull())
		{
			CrittersActor component = other.attachedRigidbody.GetComponent<CrittersActor>();
			if (CrittersManager.instance.LocalAuthority() && component.IsNotNull() && this.CanDeposit(component) && this.IsAttachAvailable())
			{
				this.HandleDeposit(component);
			}
		}
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00008244 File Offset: 0x00006444
	protected virtual bool CanDeposit(CrittersActor depositActor)
	{
		if (depositActor.crittersActorType != this.actorType)
		{
			return false;
		}
		CrittersActor crittersActor;
		if (CrittersManager.instance.actorById.TryGetValue(depositActor.parentActorId, ref crittersActor))
		{
			return crittersActor.crittersActorType == CrittersActor.CrittersActorType.Grabber;
		}
		return depositActor.parentActorId == -1;
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00008290 File Offset: 0x00006490
	private bool IsAttachAvailable()
	{
		return this.allowMultiAttach || this.currentAttach == null;
	}

	// Token: 0x06000135 RID: 309 RVA: 0x000082A8 File Offset: 0x000064A8
	protected virtual void HandleDeposit(CrittersActor depositedActor)
	{
		this.currentAttach = depositedActor;
		depositedActor.ReleasedEvent.AddListener(new UnityAction<CrittersActor>(this.HandleDetach));
		CrittersActor grabbingActor = this.attachPoint;
		bool positionOverride = this.snapOnAttach;
		bool disableGrabbing = this.disableGrabOnAttach;
		depositedActor.GrabbedBy(grabbingActor, positionOverride, default(Quaternion), default(Vector3), disableGrabbing);
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00008300 File Offset: 0x00006500
	protected virtual void HandleDetach(CrittersActor detachingActor)
	{
		this.currentAttach = null;
	}

	// Token: 0x04000154 RID: 340
	public CrittersActor attachPoint;

	// Token: 0x04000155 RID: 341
	public CrittersActor.CrittersActorType actorType;

	// Token: 0x04000156 RID: 342
	public bool disableGrabOnAttach;

	// Token: 0x04000157 RID: 343
	public bool allowMultiAttach;

	// Token: 0x04000158 RID: 344
	public bool snapOnAttach;

	// Token: 0x04000159 RID: 345
	private CrittersActor currentAttach;
}
