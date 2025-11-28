using System;
using GorillaLocomotion.Gameplay;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200036B RID: 875
public class LckDirectGrabbable : MonoBehaviour, IGorillaGrabable
{
	// Token: 0x1400002C RID: 44
	// (add) Token: 0x060014D1 RID: 5329 RVA: 0x00076CDC File Offset: 0x00074EDC
	// (remove) Token: 0x060014D2 RID: 5330 RVA: 0x00076D14 File Offset: 0x00074F14
	public event Action onGrabbed;

	// Token: 0x1400002D RID: 45
	// (add) Token: 0x060014D3 RID: 5331 RVA: 0x00076D4C File Offset: 0x00074F4C
	// (remove) Token: 0x060014D4 RID: 5332 RVA: 0x00076D84 File Offset: 0x00074F84
	public event Action onReleased;

	// Token: 0x170001FF RID: 511
	// (get) Token: 0x060014D5 RID: 5333 RVA: 0x00076DB9 File Offset: 0x00074FB9
	public GorillaGrabber grabber
	{
		get
		{
			return this._grabber;
		}
	}

	// Token: 0x17000200 RID: 512
	// (get) Token: 0x060014D6 RID: 5334 RVA: 0x00076DC1 File Offset: 0x00074FC1
	public bool isGrabbed
	{
		get
		{
			return this._grabber != null;
		}
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x00076DCF File Offset: 0x00074FCF
	public Vector3 GetLocalGrabbedPosition(GorillaGrabber grabber)
	{
		if (grabber == null)
		{
			return Vector3.zero;
		}
		return base.transform.InverseTransformPoint(grabber.transform.position);
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x00076DF6 File Offset: 0x00074FF6
	public bool CanBeGrabbed(GorillaGrabber grabber)
	{
		return this._grabber == null || grabber == this._grabber;
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x00076E14 File Offset: 0x00075014
	public void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition)
	{
		if (!base.isActiveAndEnabled)
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		if (this._grabber != null && this._grabber != grabber)
		{
			this.ForceRelease();
		}
		bool flag;
		bool flag2;
		if (this._precise && this.IsSlingshotHeldInHand(out flag, out flag2) && ((grabber.XrNode == 4 && flag) || (grabber.XrNode == 5 && flag2)))
		{
			this._grabber = null;
			grabbedTransform = grabber.transform;
			localGrabbedPosition = Vector3.zero;
			return;
		}
		this._grabber = grabber;
		GtColliderTriggerProcessor.CurrentGrabbedHand = grabber.XrNode;
		GtColliderTriggerProcessor.IsGrabbingTablet = true;
		grabbedTransform = base.transform;
		localGrabbedPosition = this.GetLocalGrabbedPosition(this._grabber);
		this.target.SetParent(grabber.transform, true);
		Action action = this.onGrabbed;
		if (action != null)
		{
			action.Invoke();
		}
		UnityEvent onTabletGrabbed = this.OnTabletGrabbed;
		if (onTabletGrabbed == null)
		{
			return;
		}
		onTabletGrabbed.Invoke();
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x00076F14 File Offset: 0x00075114
	public void OnGrabReleased(GorillaGrabber grabber)
	{
		this.target.transform.SetParent(this._originalTargetParent, true);
		this._grabber = null;
		GtColliderTriggerProcessor.IsGrabbingTablet = false;
		Action action = this.onReleased;
		if (action != null)
		{
			action.Invoke();
		}
		UnityEvent onTabletReleased = this.OnTabletReleased;
		if (onTabletReleased == null)
		{
			return;
		}
		onTabletReleased.Invoke();
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x00076F66 File Offset: 0x00075166
	public void ForceGrab(GorillaGrabber grabber)
	{
		grabber.Inject(base.transform, this.GetLocalGrabbedPosition(grabber));
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x00076F7B File Offset: 0x0007517B
	public void ForceRelease()
	{
		if (this._grabber == null)
		{
			return;
		}
		this._grabber.Inject(null, Vector3.zero);
	}

	// Token: 0x060014DD RID: 5341 RVA: 0x00076FA0 File Offset: 0x000751A0
	private bool IsSlingshotHeldInHand(out bool leftHand, out bool rightHand)
	{
		VRRig rig = VRRigCache.Instance.localRig.Rig;
		if (rig == null || rig.projectileWeapon == null)
		{
			leftHand = false;
			rightHand = false;
			return false;
		}
		leftHand = rig.projectileWeapon.InLeftHand();
		rightHand = rig.projectileWeapon.InRightHand();
		return rig.projectileWeapon.InHand();
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x00077001 File Offset: 0x00075201
	public void SetOriginalTargetParent(Transform parent)
	{
		this._originalTargetParent = parent;
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x00027DED File Offset: 0x00025FED
	public bool MomentaryGrabOnly()
	{
		return true;
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x00013E3B File Offset: 0x0001203B
	string IGorillaGrabable.get_name()
	{
		return base.name;
	}

	// Token: 0x04001F69 RID: 8041
	public UnityEvent OnTabletGrabbed = new UnityEvent();

	// Token: 0x04001F6A RID: 8042
	public UnityEvent OnTabletReleased = new UnityEvent();

	// Token: 0x04001F6B RID: 8043
	[SerializeField]
	private Transform _originalTargetParent;

	// Token: 0x04001F6C RID: 8044
	public Transform target;

	// Token: 0x04001F6D RID: 8045
	[SerializeField]
	private bool _precise;

	// Token: 0x04001F6E RID: 8046
	private GorillaGrabber _grabber;
}
