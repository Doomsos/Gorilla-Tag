using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200053C RID: 1340
public class AutoSyncTransforms : MonoBehaviour
{
	// Token: 0x17000390 RID: 912
	// (get) Token: 0x060021B5 RID: 8629 RVA: 0x000B0362 File Offset: 0x000AE562
	public Transform TargetTransform
	{
		get
		{
			return this.m_transform;
		}
	}

	// Token: 0x17000391 RID: 913
	// (get) Token: 0x060021B6 RID: 8630 RVA: 0x000B036A File Offset: 0x000AE56A
	public Rigidbody TargetRigidbody
	{
		get
		{
			return this.m_rigidbody;
		}
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x000B0374 File Offset: 0x000AE574
	private void Awake()
	{
		if (this.m_transform.IsNull())
		{
			this.m_transform = base.transform;
		}
		if (this.m_rigidbody.IsNull())
		{
			this.m_rigidbody = base.GetComponent<Rigidbody>();
		}
		if (this.m_transform.IsNull() || this.m_rigidbody.IsNull())
		{
			base.enabled = false;
			Debug.LogError("AutoSyncTransforms: Rigidbody or Transform is null, disabling!! Please add the missing reference or component", this);
			return;
		}
		this.clean = true;
	}

	// Token: 0x060021B8 RID: 8632 RVA: 0x000B03E7 File Offset: 0x000AE5E7
	private void OnEnable()
	{
		if (this.clean)
		{
			PostVRRigPhysicsSynch.AddSyncTarget(this);
		}
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x000B03F7 File Offset: 0x000AE5F7
	private void OnDisable()
	{
		if (this.clean)
		{
			PostVRRigPhysicsSynch.RemoveSyncTarget(this);
		}
	}

	// Token: 0x04002C66 RID: 11366
	[SerializeField]
	private Transform m_transform;

	// Token: 0x04002C67 RID: 11367
	[SerializeField]
	private Rigidbody m_rigidbody;

	// Token: 0x04002C68 RID: 11368
	private bool clean;
}
