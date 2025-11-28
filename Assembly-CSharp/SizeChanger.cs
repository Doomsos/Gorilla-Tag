using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000829 RID: 2089
public class SizeChanger : GorillaTriggerBox
{
	// Token: 0x170004F0 RID: 1264
	// (get) Token: 0x060036EA RID: 14058 RVA: 0x00128A24 File Offset: 0x00126C24
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x170004F1 RID: 1265
	// (get) Token: 0x060036EB RID: 14059 RVA: 0x00128A64 File Offset: 0x00126C64
	public SizeChanger.ChangerType MyType
	{
		get
		{
			return this.myType;
		}
	}

	// Token: 0x170004F2 RID: 1266
	// (get) Token: 0x060036EC RID: 14060 RVA: 0x00128A6C File Offset: 0x00126C6C
	public float MaxScale
	{
		get
		{
			return this.maxScale;
		}
	}

	// Token: 0x170004F3 RID: 1267
	// (get) Token: 0x060036ED RID: 14061 RVA: 0x00128A74 File Offset: 0x00126C74
	public float MinScale
	{
		get
		{
			return this.minScale;
		}
	}

	// Token: 0x170004F4 RID: 1268
	// (get) Token: 0x060036EE RID: 14062 RVA: 0x00128A7C File Offset: 0x00126C7C
	public Transform StartPos
	{
		get
		{
			return this.startPos;
		}
	}

	// Token: 0x170004F5 RID: 1269
	// (get) Token: 0x060036EF RID: 14063 RVA: 0x00128A84 File Offset: 0x00126C84
	public Transform EndPos
	{
		get
		{
			return this.endPos;
		}
	}

	// Token: 0x170004F6 RID: 1270
	// (get) Token: 0x060036F0 RID: 14064 RVA: 0x00128A8C File Offset: 0x00126C8C
	public float StaticEasing
	{
		get
		{
			return this.staticEasing;
		}
	}

	// Token: 0x060036F1 RID: 14065 RVA: 0x00128A94 File Offset: 0x00126C94
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x060036F2 RID: 14066 RVA: 0x00128AB8 File Offset: 0x00126CB8
	public void OnEnable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter += this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit += this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x060036F3 RID: 14067 RVA: 0x00128B34 File Offset: 0x00126D34
	public void OnDisable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter -= this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit -= this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x060036F4 RID: 14068 RVA: 0x00128BAD File Offset: 0x00126DAD
	public void AddEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerEnter;
		}
	}

	// Token: 0x060036F5 RID: 14069 RVA: 0x00128BC9 File Offset: 0x00126DC9
	public void RemoveEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerEnter;
		}
	}

	// Token: 0x060036F6 RID: 14070 RVA: 0x00128BE5 File Offset: 0x00126DE5
	public void AddExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x060036F7 RID: 14071 RVA: 0x00128C01 File Offset: 0x00126E01
	public void RemoveExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x060036F8 RID: 14072 RVA: 0x00128C20 File Offset: 0x00126E20
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.acceptRig(component);
	}

	// Token: 0x060036F9 RID: 14073 RVA: 0x00128C5D File Offset: 0x00126E5D
	public void acceptRig(VRRig rig)
	{
		if (!rig.sizeManager.touchingChangers.Contains(this))
		{
			rig.sizeManager.touchingChangers.Add(this);
		}
		UnityAction onEnter = this.OnEnter;
		if (onEnter == null)
		{
			return;
		}
		onEnter.Invoke();
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x00128C94 File Offset: 0x00126E94
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.unacceptRig(component);
	}

	// Token: 0x060036FB RID: 14075 RVA: 0x00128CD1 File Offset: 0x00126ED1
	public void unacceptRig(VRRig rig)
	{
		rig.sizeManager.touchingChangers.Remove(this);
		UnityAction onExit = this.OnExit;
		if (onExit == null)
		{
			return;
		}
		onExit.Invoke();
	}

	// Token: 0x060036FC RID: 14076 RVA: 0x00128CF8 File Offset: 0x00126EF8
	public Vector3 ClosestPoint(Vector3 position)
	{
		if (this.enterTrigger && this.exitTrigger)
		{
			Vector3 vector = this.enterTrigger.ClosestPoint(position);
			Vector3 vector2 = this.exitTrigger.ClosestPoint(position);
			if (Vector3.Distance(position, vector) >= Vector3.Distance(position, vector2))
			{
				return vector2;
			}
			return vector;
		}
		else
		{
			if (this.myCollider)
			{
				return this.myCollider.ClosestPoint(position);
			}
			return position;
		}
	}

	// Token: 0x060036FD RID: 14077 RVA: 0x00128D68 File Offset: 0x00126F68
	public void SetScaleCenterPoint(Transform centerPoint)
	{
		this.scaleAwayFromPoint = centerPoint;
	}

	// Token: 0x060036FE RID: 14078 RVA: 0x00128D71 File Offset: 0x00126F71
	public bool TryGetScaleCenterPoint(out Vector3 centerPoint)
	{
		if (this.scaleAwayFromPoint != null)
		{
			centerPoint = this.scaleAwayFromPoint.position;
			return true;
		}
		centerPoint = Vector3.zero;
		return false;
	}

	// Token: 0x0400465E RID: 18014
	[SerializeField]
	private SizeChanger.ChangerType myType;

	// Token: 0x0400465F RID: 18015
	[SerializeField]
	private float staticEasing;

	// Token: 0x04004660 RID: 18016
	[SerializeField]
	private float maxScale;

	// Token: 0x04004661 RID: 18017
	[SerializeField]
	private float minScale;

	// Token: 0x04004662 RID: 18018
	private Collider myCollider;

	// Token: 0x04004663 RID: 18019
	[SerializeField]
	private Transform startPos;

	// Token: 0x04004664 RID: 18020
	[SerializeField]
	private Transform endPos;

	// Token: 0x04004665 RID: 18021
	[SerializeField]
	private SizeChangerTrigger enterTrigger;

	// Token: 0x04004666 RID: 18022
	[SerializeField]
	private SizeChangerTrigger exitTrigger;

	// Token: 0x04004667 RID: 18023
	[SerializeField]
	private Transform scaleAwayFromPoint;

	// Token: 0x04004668 RID: 18024
	[SerializeField]
	private SizeChangerTrigger exitOnEnterTrigger;

	// Token: 0x04004669 RID: 18025
	public bool alwaysControlWhenEntered;

	// Token: 0x0400466A RID: 18026
	public int priority;

	// Token: 0x0400466B RID: 18027
	public bool aprilFoolsEnabled;

	// Token: 0x0400466C RID: 18028
	public float startRadius;

	// Token: 0x0400466D RID: 18029
	public float endRadius;

	// Token: 0x0400466E RID: 18030
	public bool affectLayerA = true;

	// Token: 0x0400466F RID: 18031
	public bool affectLayerB = true;

	// Token: 0x04004670 RID: 18032
	public bool affectLayerC = true;

	// Token: 0x04004671 RID: 18033
	public bool affectLayerD = true;

	// Token: 0x04004672 RID: 18034
	public UnityAction OnExit;

	// Token: 0x04004673 RID: 18035
	public UnityAction OnEnter;

	// Token: 0x04004674 RID: 18036
	private HashSet<VRRig> unregisteredPresentRigs;

	// Token: 0x0200082A RID: 2090
	public enum ChangerType
	{
		// Token: 0x04004676 RID: 18038
		Static,
		// Token: 0x04004677 RID: 18039
		Continuous,
		// Token: 0x04004678 RID: 18040
		Radius
	}
}
