using System;
using UnityEngine;

// Token: 0x02000785 RID: 1925
public class GorillaHandNode : MonoBehaviour
{
	// Token: 0x17000478 RID: 1144
	// (get) Token: 0x06003274 RID: 12916 RVA: 0x00110348 File Offset: 0x0010E548
	public bool isGripping
	{
		get
		{
			return this.PollGrip();
		}
	}

	// Token: 0x17000479 RID: 1145
	// (get) Token: 0x06003275 RID: 12917 RVA: 0x00110350 File Offset: 0x0010E550
	public bool isLeftHand
	{
		get
		{
			return this._isLeftHand;
		}
	}

	// Token: 0x1700047A RID: 1146
	// (get) Token: 0x06003276 RID: 12918 RVA: 0x00110358 File Offset: 0x0010E558
	public bool isRightHand
	{
		get
		{
			return this._isRightHand;
		}
	}

	// Token: 0x06003277 RID: 12919 RVA: 0x00110360 File Offset: 0x0010E560
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06003278 RID: 12920 RVA: 0x00110368 File Offset: 0x0010E568
	private bool PollGrip()
	{
		if (this.rig == null)
		{
			return false;
		}
		bool flag = this.PollThumb() >= 0.25f;
		bool flag2 = this.PollIndex() >= 0.25f;
		bool flag3 = this.PollMiddle() >= 0.25f;
		return flag && flag2 && flag3;
	}

	// Token: 0x06003279 RID: 12921 RVA: 0x001103BC File Offset: 0x0010E5BC
	private void Setup()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		if (this.rigidbody == null)
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.rig)
		{
			this.vrIndex = (this._isLeftHand ? this.rig.leftIndex : this.rig.rightIndex);
			this.vrThumb = (this._isLeftHand ? this.rig.leftThumb : this.rig.rightThumb);
			this.vrMiddle = (this._isLeftHand ? this.rig.leftMiddle : this.rig.rightMiddle);
		}
		this._isLeftHand = base.name.Contains("left", 5);
		this._isRightHand = base.name.Contains("right", 5);
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(this._isLeftHand ? UnityTag.GorillaHandLeft : UnityTag.GorillaHandRight);
		base.gameObject.SetLayer(UnityLayer.GorillaHand);
		this.rigidbody.includeLayers = num;
		this.rigidbody.excludeLayers = ~num;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		this.rigidbody.constraints = 126;
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x00002789 File Offset: 0x00000989
	private void OnTriggerStay(Collider other)
	{
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x0011057B File Offset: 0x0010E77B
	private float PollIndex()
	{
		return Mathf.Clamp01(this.vrIndex.calcT / 0.88f);
	}

	// Token: 0x0600327C RID: 12924 RVA: 0x00110593 File Offset: 0x0010E793
	private float PollMiddle()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x00110593 File Offset: 0x0010E793
	private float PollThumb()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x040040D0 RID: 16592
	public VRRig rig;

	// Token: 0x040040D1 RID: 16593
	public Collider collider;

	// Token: 0x040040D2 RID: 16594
	public Rigidbody rigidbody;

	// Token: 0x040040D3 RID: 16595
	[Space]
	[NonSerialized]
	public VRMapIndex vrIndex;

	// Token: 0x040040D4 RID: 16596
	[NonSerialized]
	public VRMapThumb vrThumb;

	// Token: 0x040040D5 RID: 16597
	[NonSerialized]
	public VRMapMiddle vrMiddle;

	// Token: 0x040040D6 RID: 16598
	[Space]
	public GorillaHandSocket attachedToSocket;

	// Token: 0x040040D7 RID: 16599
	[Space]
	[SerializeField]
	private bool _isLeftHand;

	// Token: 0x040040D8 RID: 16600
	[SerializeField]
	private bool _isRightHand;

	// Token: 0x040040D9 RID: 16601
	public bool ignoreSockets;
}
