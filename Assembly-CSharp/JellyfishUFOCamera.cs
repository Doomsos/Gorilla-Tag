using System;
using BoingKit;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class JellyfishUFOCamera : MonoBehaviour
{
	// Token: 0x06000048 RID: 72 RVA: 0x00002BA6 File Offset: 0x00000DA6
	private void Start()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_spring.Reset(this.Target.transform.position);
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00002BD4 File Offset: 0x00000DD4
	private void FixedUpdate()
	{
		if (this.Target == null)
		{
			return;
		}
		this.m_spring.TrackExponential(this.Target.transform.position, 0.5f, Time.fixedDeltaTime);
		Vector3 normalized = (this.m_spring.Value - base.transform.position).normalized;
		base.transform.rotation = Quaternion.LookRotation(normalized);
	}

	// Token: 0x0400002D RID: 45
	public Transform Target;

	// Token: 0x0400002E RID: 46
	private Vector3Spring m_spring;
}
