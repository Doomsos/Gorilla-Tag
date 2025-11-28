using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class ColliderSpinner : MonoBehaviour
{
	// Token: 0x06000054 RID: 84 RVA: 0x00003254 File Offset: 0x00001454
	private void Start()
	{
		this.m_targetOffset = ((this.Target != null) ? (base.transform.position - this.Target.position) : Vector3.zero);
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x06000055 RID: 85 RVA: 0x000032B0 File Offset: 0x000014B0
	private void FixedUpdate()
	{
		Vector3 targetValue = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(targetValue, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x04000045 RID: 69
	public Transform Target;

	// Token: 0x04000046 RID: 70
	private Vector3 m_targetOffset;

	// Token: 0x04000047 RID: 71
	private Vector3Spring m_spring;
}
