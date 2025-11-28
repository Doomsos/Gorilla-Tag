using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001E RID: 30
public class CurveBall : MonoBehaviour
{
	// Token: 0x06000073 RID: 115 RVA: 0x00003F94 File Offset: 0x00002194
	public void Reset()
	{
		float num = Random.Range(0f, MathUtil.TwoPi);
		float num2 = Mathf.Cos(num);
		float num3 = Mathf.Sin(num);
		this.m_speedX = 40f * num2;
		this.m_speedZ = 40f * num3;
		this.m_timer = 0f;
		Vector3 position = base.transform.position;
		position.x = -10f * num2;
		position.z = -10f * num3;
		base.transform.position = position;
	}

	// Token: 0x06000074 RID: 116 RVA: 0x00004016 File Offset: 0x00002216
	public void Start()
	{
		this.Reset();
	}

	// Token: 0x06000075 RID: 117 RVA: 0x00004020 File Offset: 0x00002220
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		if (this.m_timer > this.Interval)
		{
			this.Reset();
		}
		Vector3 position = base.transform.position;
		position.x += this.m_speedX * deltaTime;
		position.z += this.m_speedZ * deltaTime;
		base.transform.position = position;
		this.m_timer += deltaTime;
	}

	// Token: 0x04000071 RID: 113
	public float Interval = 2f;

	// Token: 0x04000072 RID: 114
	private float m_speedX;

	// Token: 0x04000073 RID: 115
	private float m_speedZ;

	// Token: 0x04000074 RID: 116
	private float m_timer;
}
