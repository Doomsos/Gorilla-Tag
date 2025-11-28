using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class ScaleSpring : MonoBehaviour
{
	// Token: 0x06000069 RID: 105 RVA: 0x000039AC File Offset: 0x00001BAC
	public void Tick()
	{
		this.m_targetScale = ((this.m_targetScale == ScaleSpring.kSmallScale) ? ScaleSpring.kLargeScale : ScaleSpring.kSmallScale);
		this.m_lastTickTime = Time.time;
		base.GetComponent<BoingEffector>().MoveDistance = ScaleSpring.kMoveDistance * ((this.m_targetScale == ScaleSpring.kSmallScale) ? -1f : 1f);
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00003A0D File Offset: 0x00001C0D
	public void Start()
	{
		this.Tick();
		this.m_spring.Reset(this.m_targetScale * Vector3.one);
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00003A30 File Offset: 0x00001C30
	public void FixedUpdate()
	{
		if (Time.time - this.m_lastTickTime > ScaleSpring.kInterval)
		{
			this.Tick();
		}
		this.m_spring.TrackHalfLife(this.m_targetScale * Vector3.one, 6f, 0.05f, Time.fixedDeltaTime);
		base.transform.localScale = this.m_spring.Value;
		base.GetComponent<BoingEffector>().MoveDistance *= Mathf.Min(0.99f, 35f * Time.fixedDeltaTime);
	}

	// Token: 0x0400005D RID: 93
	private static readonly float kInterval = 2f;

	// Token: 0x0400005E RID: 94
	private static readonly float kSmallScale = 0.6f;

	// Token: 0x0400005F RID: 95
	private static readonly float kLargeScale = 2f;

	// Token: 0x04000060 RID: 96
	private static readonly float kMoveDistance = 30f;

	// Token: 0x04000061 RID: 97
	private Vector3Spring m_spring;

	// Token: 0x04000062 RID: 98
	private float m_targetScale;

	// Token: 0x04000063 RID: 99
	private float m_lastTickTime;
}
