using System;
using UnityEngine;

// Token: 0x0200007A RID: 122
public class DebugTestGrabber : MonoBehaviour
{
	// Token: 0x060002FC RID: 764 RVA: 0x00012881 File Offset: 0x00010A81
	private void Awake()
	{
		if (this.grabber == null)
		{
			this.grabber = base.GetComponentInChildren<CrittersGrabber>();
		}
	}

	// Token: 0x060002FD RID: 765 RVA: 0x000128A0 File Offset: 0x00010AA0
	private void LateUpdate()
	{
		if (this.transformToFollow != null)
		{
			base.transform.rotation = this.transformToFollow.rotation;
			base.transform.position = this.transformToFollow.position;
		}
		if (this.grabber == null)
		{
			return;
		}
		if (!this.isGrabbing && this.setIsGrabbing)
		{
			this.setIsGrabbing = false;
			this.isGrabbing = true;
			this.remainingGrabDuration = this.grabDuration;
		}
		else if (this.isGrabbing && this.setRelease)
		{
			this.setRelease = false;
			this.isGrabbing = false;
			this.DoRelease();
		}
		if (this.isGrabbing && this.remainingGrabDuration > 0f)
		{
			this.remainingGrabDuration -= Time.deltaTime;
			this.DoGrab();
		}
	}

	// Token: 0x060002FE RID: 766 RVA: 0x00012974 File Offset: 0x00010B74
	private void DoGrab()
	{
		this.grabber.grabbing = true;
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius, this.colliders, LayerMask.GetMask(new string[]
		{
			"GorillaInteractable"
		}));
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				CrittersActor componentInParent = this.colliders[i].GetComponentInParent<CrittersActor>();
				if (!(componentInParent == null) && componentInParent.usesRB && componentInParent.CanBeGrabbed(this.grabber))
				{
					this.isHandGrabbingDisabled = true;
					if (componentInParent.equipmentStorable)
					{
						componentInParent.localCanStore = true;
					}
					componentInParent.GrabbedBy(this.grabber, false, default(Quaternion), default(Vector3), false);
					this.grabber.grabbedActors.Add(componentInParent);
					this.remainingGrabDuration = 0f;
					return;
				}
			}
		}
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00012A58 File Offset: 0x00010C58
	private void DoRelease()
	{
		this.grabber.grabbing = false;
		for (int i = this.grabber.grabbedActors.Count - 1; i >= 0; i--)
		{
			CrittersActor crittersActor = this.grabber.grabbedActors[i];
			crittersActor.Released(true, crittersActor.transform.rotation, crittersActor.transform.position, this.estimator.linearVelocity, default(Vector3));
			if (i < this.grabber.grabbedActors.Count)
			{
				this.grabber.grabbedActors.RemoveAt(i);
			}
		}
		if (this.isHandGrabbingDisabled)
		{
			this.isHandGrabbingDisabled = false;
		}
	}

	// Token: 0x0400039C RID: 924
	public bool isGrabbing;

	// Token: 0x0400039D RID: 925
	public bool setIsGrabbing;

	// Token: 0x0400039E RID: 926
	public bool setRelease;

	// Token: 0x0400039F RID: 927
	public Collider[] colliders = new Collider[50];

	// Token: 0x040003A0 RID: 928
	public bool isLeft;

	// Token: 0x040003A1 RID: 929
	public float grabRadius = 0.05f;

	// Token: 0x040003A2 RID: 930
	public Transform transformToFollow;

	// Token: 0x040003A3 RID: 931
	public GorillaVelocityEstimator estimator;

	// Token: 0x040003A4 RID: 932
	public CrittersGrabber grabber;

	// Token: 0x040003A5 RID: 933
	public CrittersActorGrabber otherHand;

	// Token: 0x040003A6 RID: 934
	private bool isHandGrabbingDisabled;

	// Token: 0x040003A7 RID: 935
	private float grabDuration = 0.3f;

	// Token: 0x040003A8 RID: 936
	private float remainingGrabDuration;
}
