using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class OrbitCamera : MonoBehaviour
{
	// Token: 0x06000065 RID: 101 RVA: 0x00002789 File Offset: 0x00000989
	public void Start()
	{
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003900 File Offset: 0x00001B00
	public void Update()
	{
		this.m_phase += OrbitCamera.kOrbitSpeed * MathUtil.TwoPi * Time.deltaTime;
		base.transform.position = new Vector3(-4f * Mathf.Cos(this.m_phase), 6f, 4f * Mathf.Sin(this.m_phase));
		base.transform.rotation = Quaternion.LookRotation((new Vector3(0f, 3f, 0f) - base.transform.position).normalized);
	}

	// Token: 0x0400005B RID: 91
	private static readonly float kOrbitSpeed = 0.01f;

	// Token: 0x0400005C RID: 92
	private float m_phase;
}
