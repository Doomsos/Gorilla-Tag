using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion.Gameplay;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007FB RID: 2043
[RequireComponent(typeof(Collider))]
public class HandHold : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x14000060 RID: 96
	// (add) Token: 0x060035BD RID: 13757 RVA: 0x00123AE0 File Offset: 0x00121CE0
	// (remove) Token: 0x060035BE RID: 13758 RVA: 0x00123B14 File Offset: 0x00121D14
	public static event HandHold.HandHoldPositionEvent HandPositionRequestOverride;

	// Token: 0x14000061 RID: 97
	// (add) Token: 0x060035BF RID: 13759 RVA: 0x00123B48 File Offset: 0x00121D48
	// (remove) Token: 0x060035C0 RID: 13760 RVA: 0x00123B7C File Offset: 0x00121D7C
	public static event HandHold.HandHoldEvent HandPositionReleaseOverride;

	// Token: 0x060035C1 RID: 13761 RVA: 0x00123BB0 File Offset: 0x00121DB0
	public void OnDisable()
	{
		for (int i = 0; i < this.currentGrabbers.Count; i++)
		{
			if (this.currentGrabbers[i].IsNotNull())
			{
				this.currentGrabbers[i].Ungrab(this);
			}
		}
	}

	// Token: 0x060035C2 RID: 13762 RVA: 0x00123BF8 File Offset: 0x00121DF8
	private void Initialize()
	{
		if (this.initialized)
		{
			return;
		}
		this.myTappable = base.GetComponent<Tappable>();
		this.myCollider = base.GetComponent<Collider>();
		this.initialized = true;
	}

	// Token: 0x060035C3 RID: 13763 RVA: 0x00027DED File Offset: 0x00025FED
	public virtual bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return true;
	}

	// Token: 0x060035C4 RID: 13764 RVA: 0x00123C24 File Offset: 0x00121E24
	void IGorillaGrabable.OnGrabbed(GorillaGrabber g, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		this.Initialize();
		grabbedTransform = base.transform;
		Vector3 position = g.transform.position;
		localGrabbedPosition = base.transform.InverseTransformPoint(position);
		Vector3 vector;
		g.Player.AddHandHold(base.transform, localGrabbedPosition, g, g.IsLeftHand, this.rotatePlayerWhenHeld, out vector);
		this.currentGrabbers.AddIfNew(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionRequestOverride != null)
		{
			HandHold.HandPositionRequestOverride(this, g.IsLeftHand, this.CalculateOffset(position));
		}
		UnityEvent<Vector3> onGrab = this.OnGrab;
		if (onGrab != null)
		{
			onGrab.Invoke(vector);
		}
		UnityEvent<HandHold> onGrabHandHold = this.OnGrabHandHold;
		if (onGrabHandHold != null)
		{
			onGrabHandHold.Invoke(this);
		}
		UnityEvent<bool> onGrabHanded = this.OnGrabHanded;
		if (onGrabHanded != null)
		{
			onGrabHanded.Invoke(g.IsLeftHand);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnGrab();
		}
	}

	// Token: 0x060035C5 RID: 13765 RVA: 0x00123D0C File Offset: 0x00121F0C
	void IGorillaGrabable.OnGrabReleased(GorillaGrabber g)
	{
		this.Initialize();
		g.Player.RemoveHandHold(g, g.IsLeftHand);
		this.currentGrabbers.Remove(g);
		if (this.handSnapMethod != HandHold.HandSnapMethod.None && HandHold.HandPositionReleaseOverride != null)
		{
			HandHold.HandPositionReleaseOverride(this, g.IsLeftHand);
		}
		UnityEvent onRelease = this.OnRelease;
		if (onRelease != null)
		{
			onRelease.Invoke();
		}
		UnityEvent<HandHold> onReleaseHandHold = this.OnReleaseHandHold;
		if (onReleaseHandHold != null)
		{
			onReleaseHandHold.Invoke(this);
		}
		if (this.myTappable != null)
		{
			this.myTappable.OnRelease();
		}
	}

	// Token: 0x060035C6 RID: 13766 RVA: 0x00123D9C File Offset: 0x00121F9C
	private Vector3 CalculateOffset(Vector3 position)
	{
		switch (this.handSnapMethod)
		{
		case HandHold.HandSnapMethod.SnapToNearestEdge:
			if (this.myCollider == null)
			{
				this.myCollider = base.GetComponent<Collider>();
				if (this.myCollider is MeshCollider && !(this.myCollider as MeshCollider).convex)
				{
					this.handSnapMethod = HandHold.HandSnapMethod.None;
					return Vector3.zero;
				}
			}
			return base.transform.position - this.myCollider.ClosestPoint(position);
		case HandHold.HandSnapMethod.SnapToXAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.right * base.transform.InverseTransformPoint(position).x);
		case HandHold.HandSnapMethod.SnapToYAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.up * base.transform.InverseTransformPoint(position).y);
		case HandHold.HandSnapMethod.SnapToZAxisPoint:
			return base.transform.position - base.transform.TransformPoint(Vector3.forward * base.transform.InverseTransformPoint(position).z);
		default:
			return Vector3.zero;
		}
	}

	// Token: 0x060035C7 RID: 13767 RVA: 0x00123EDA File Offset: 0x001220DA
	public bool MomentaryGrabOnly()
	{
		return this.forceMomentary;
	}

	// Token: 0x060035C8 RID: 13768 RVA: 0x00123EE2 File Offset: 0x001220E2
	public void CopyProperties(HandHoldSettings handHoldSettings)
	{
		this.handSnapMethod = handHoldSettings.handSnapMethod;
		this.rotatePlayerWhenHeld = handHoldSettings.rotatePlayerWhenHeld;
		this.forceMomentary = !handHoldSettings.allowPreGrab;
	}

	// Token: 0x060035CA RID: 13770 RVA: 0x00013E3B File Offset: 0x0001203B
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04004511 RID: 17681
	private Dictionary<Transform, Transform> attached = new Dictionary<Transform, Transform>();

	// Token: 0x04004512 RID: 17682
	[SerializeField]
	private HandHold.HandSnapMethod handSnapMethod;

	// Token: 0x04004513 RID: 17683
	[SerializeField]
	private bool rotatePlayerWhenHeld;

	// Token: 0x04004514 RID: 17684
	[SerializeField]
	private UnityEvent<Vector3> OnGrab;

	// Token: 0x04004515 RID: 17685
	[SerializeField]
	private UnityEvent<HandHold> OnGrabHandHold;

	// Token: 0x04004516 RID: 17686
	[SerializeField]
	private UnityEvent<bool> OnGrabHanded;

	// Token: 0x04004517 RID: 17687
	[SerializeField]
	private UnityEvent OnRelease;

	// Token: 0x04004518 RID: 17688
	[SerializeField]
	private UnityEvent<HandHold> OnReleaseHandHold;

	// Token: 0x04004519 RID: 17689
	private bool initialized;

	// Token: 0x0400451A RID: 17690
	private Collider myCollider;

	// Token: 0x0400451B RID: 17691
	private Tappable myTappable;

	// Token: 0x0400451C RID: 17692
	[Tooltip("Turning this on disables \"pregrabbing\". Use pregrabbing to allow players to catch a handhold even if they have squeezed the trigger too soon. Useful if you're anticipating jumping players needed to grab while airborne")]
	[SerializeField]
	private bool forceMomentary = true;

	// Token: 0x0400451D RID: 17693
	private List<GorillaGrabber> currentGrabbers = new List<GorillaGrabber>();

	// Token: 0x020007FC RID: 2044
	private enum HandSnapMethod
	{
		// Token: 0x0400451F RID: 17695
		None,
		// Token: 0x04004520 RID: 17696
		SnapToCenterPoint,
		// Token: 0x04004521 RID: 17697
		SnapToNearestEdge,
		// Token: 0x04004522 RID: 17698
		SnapToXAxisPoint,
		// Token: 0x04004523 RID: 17699
		SnapToYAxisPoint,
		// Token: 0x04004524 RID: 17700
		SnapToZAxisPoint
	}

	// Token: 0x020007FD RID: 2045
	// (Invoke) Token: 0x060035CC RID: 13772
	public delegate void HandHoldPositionEvent(HandHold hh, bool lh, Vector3 pos);

	// Token: 0x020007FE RID: 2046
	// (Invoke) Token: 0x060035D0 RID: 13776
	public delegate void HandHoldEvent(HandHold hh, bool lh);
}
