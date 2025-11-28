using System;
using GorillaLocomotion;
using GorillaLocomotion.Gameplay;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000824 RID: 2084
public class GorillaGrabber : MonoBehaviour
{
	// Token: 0x170004E7 RID: 1255
	// (get) Token: 0x060036B5 RID: 14005 RVA: 0x00127D84 File Offset: 0x00125F84
	public bool isGrabbing
	{
		get
		{
			return this.currentGrabbable != null;
		}
	}

	// Token: 0x170004E8 RID: 1256
	// (get) Token: 0x060036B6 RID: 14006 RVA: 0x00127D8F File Offset: 0x00125F8F
	public XRNode XrNode
	{
		get
		{
			return this.xrNode;
		}
	}

	// Token: 0x170004E9 RID: 1257
	// (get) Token: 0x060036B7 RID: 14007 RVA: 0x00127D97 File Offset: 0x00125F97
	public bool IsLeftHand
	{
		get
		{
			return this.XrNode == 4;
		}
	}

	// Token: 0x170004EA RID: 1258
	// (get) Token: 0x060036B8 RID: 14008 RVA: 0x00127DA2 File Offset: 0x00125FA2
	public bool IsRightHand
	{
		get
		{
			return this.XrNode == 5;
		}
	}

	// Token: 0x170004EB RID: 1259
	// (get) Token: 0x060036B9 RID: 14009 RVA: 0x00127DAD File Offset: 0x00125FAD
	public GTPlayer Player
	{
		get
		{
			return this.player;
		}
	}

	// Token: 0x060036BA RID: 14010 RVA: 0x00127DB8 File Offset: 0x00125FB8
	private void Start()
	{
		this.hapticStrengthActual = this.hapticStrength;
		this.audioSource = base.GetComponent<AudioSource>();
		this.player = base.GetComponentInParent<GTPlayer>();
		if (!this.player)
		{
			Debug.LogWarning("Gorilla Grabber Component has no player in hierarchy. Disabling this Gorilla Grabber");
			base.GetComponent<GorillaGrabber>().enabled = false;
		}
	}

	// Token: 0x060036BB RID: 14011 RVA: 0x00127E0C File Offset: 0x0012600C
	public void CheckGrabber(bool initiateGrab)
	{
		bool grabMomentary = ControllerInputPoller.GetGrabMomentary(this.xrNode);
		bool grabRelease = ControllerInputPoller.GetGrabRelease(this.xrNode);
		if (this.currentGrabbable != null && (grabRelease || this.GrabDistanceOverCheck()))
		{
			this.Ungrab(null);
		}
		if (grabMomentary)
		{
			this.grabTimeStamp = Time.time;
		}
		if (initiateGrab && this.currentGrabbable == null)
		{
			this.currentGrabbable = this.TryGrab(Time.time - this.grabTimeStamp < this.coyoteTimeDuration);
		}
		if (this.currentGrabbable != null && this.hapticStrengthActual > 0f)
		{
			GorillaTagger.Instance.DoVibration(this.xrNode, this.hapticStrengthActual, Time.deltaTime);
			this.hapticStrengthActual -= this.hapticDecay * Time.deltaTime;
		}
	}

	// Token: 0x060036BC RID: 14012 RVA: 0x00127ECB File Offset: 0x001260CB
	private bool GrabDistanceOverCheck()
	{
		return this.currentGrabbedTransform == null || Vector3.Distance(base.transform.position, this.currentGrabbedTransform.TransformPoint(this.localGrabbedPosition)) > this.breakDistance;
	}

	// Token: 0x060036BD RID: 14013 RVA: 0x00127F08 File Offset: 0x00126108
	internal void Ungrab(IGorillaGrabable specificGrabbable = null)
	{
		if (specificGrabbable != null && specificGrabbable != this.currentGrabbable)
		{
			return;
		}
		this.currentGrabbable.OnGrabReleased(this);
		PlayerGameEvents.DroppedObject(this.currentGrabbable.name);
		this.currentGrabbable = null;
		this.gripEffects.Stop();
		this.hapticStrengthActual = this.hapticStrength;
	}

	// Token: 0x060036BE RID: 14014 RVA: 0x00127F5C File Offset: 0x0012615C
	private IGorillaGrabable TryGrab(bool momentary)
	{
		IGorillaGrabable gorillaGrabable = null;
		Debug.DrawRay(base.transform.position, base.transform.forward * (this.grabRadius * this.player.scale), Color.blue, 1f);
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.grabRadius * this.player.scale, this.grabCastResults);
		float num2 = float.MaxValue;
		for (int i = 0; i < num; i++)
		{
			IGorillaGrabable gorillaGrabable2;
			if (this.grabCastResults[i].TryGetComponent<IGorillaGrabable>(ref gorillaGrabable2))
			{
				float num3 = Vector3.Distance(base.transform.position, this.FindClosestPoint(this.grabCastResults[i], base.transform.position));
				if (num3 < num2)
				{
					num2 = num3;
					gorillaGrabable = gorillaGrabable2;
				}
			}
		}
		if (gorillaGrabable != null && (!gorillaGrabable.MomentaryGrabOnly() || momentary) && gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable.OnGrabbed(this, out this.currentGrabbedTransform, out this.localGrabbedPosition);
			PlayerGameEvents.GrabbedObject(gorillaGrabable.name);
		}
		if (gorillaGrabable != null && !gorillaGrabable.CanBeGrabbed(this))
		{
			gorillaGrabable = null;
		}
		return gorillaGrabable;
	}

	// Token: 0x060036BF RID: 14015 RVA: 0x0012806F File Offset: 0x0012626F
	private Vector3 FindClosestPoint(Collider collider, Vector3 position)
	{
		if (collider is MeshCollider && !(collider as MeshCollider).convex)
		{
			return position;
		}
		return collider.ClosestPoint(position);
	}

	// Token: 0x060036C0 RID: 14016 RVA: 0x00128090 File Offset: 0x00126290
	public void Inject(Transform currentGrabbableTransform, Vector3 localGrabbedPosition)
	{
		if (this.currentGrabbable != null)
		{
			this.Ungrab(null);
		}
		if (currentGrabbableTransform != null)
		{
			this.currentGrabbable = currentGrabbableTransform.GetComponent<IGorillaGrabable>();
			this.currentGrabbedTransform = currentGrabbableTransform;
			this.localGrabbedPosition = localGrabbedPosition;
			this.currentGrabbable.OnGrabbed(this, out this.currentGrabbedTransform, out localGrabbedPosition);
		}
	}

	// Token: 0x0400461A RID: 17946
	private GTPlayer player;

	// Token: 0x0400461B RID: 17947
	[SerializeField]
	private XRNode xrNode = 4;

	// Token: 0x0400461C RID: 17948
	private AudioSource audioSource;

	// Token: 0x0400461D RID: 17949
	private Transform currentGrabbedTransform;

	// Token: 0x0400461E RID: 17950
	private Vector3 localGrabbedPosition;

	// Token: 0x0400461F RID: 17951
	private IGorillaGrabable currentGrabbable;

	// Token: 0x04004620 RID: 17952
	[SerializeField]
	private float grabRadius = 0.015f;

	// Token: 0x04004621 RID: 17953
	[SerializeField]
	private float breakDistance = 0.3f;

	// Token: 0x04004622 RID: 17954
	[SerializeField]
	private float hapticStrength = 0.2f;

	// Token: 0x04004623 RID: 17955
	private float hapticStrengthActual = 0.2f;

	// Token: 0x04004624 RID: 17956
	[SerializeField]
	private float hapticDecay;

	// Token: 0x04004625 RID: 17957
	[SerializeField]
	private ParticleSystem gripEffects;

	// Token: 0x04004626 RID: 17958
	private Collider[] grabCastResults = new Collider[32];

	// Token: 0x04004627 RID: 17959
	private float grabTimeStamp;

	// Token: 0x04004628 RID: 17960
	[SerializeField]
	private float coyoteTimeDuration = 0.25f;
}
