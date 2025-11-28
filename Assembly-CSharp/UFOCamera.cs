using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class UFOCamera : MonoBehaviour
{
	// Token: 0x06000057 RID: 87 RVA: 0x000032F8 File Offset: 0x000014F8
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_targetOffset = base.transform.position - this.Target.position;
		this.m_spring.Reset(base.transform.position);
	}

	// Token: 0x06000058 RID: 88 RVA: 0x0000334C File Offset: 0x0000154C
	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		Vector3 targetValue = this.Target.position + this.m_targetOffset;
		base.transform.position = this.m_spring.TrackExponential(targetValue, 0.02f, Time.fixedDeltaTime);
	}

	// Token: 0x04000048 RID: 72
	public Transform Target;

	// Token: 0x04000049 RID: 73
	private Vector3 m_targetOffset;

	// Token: 0x0400004A RID: 74
	private Vector3Spring m_spring;
}
