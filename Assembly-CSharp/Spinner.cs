using System;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class Spinner : MonoBehaviour
{
	// Token: 0x0600008C RID: 140 RVA: 0x00004D6F File Offset: 0x00002F6F
	public void OnEnable()
	{
		this.m_angle = Random.Range(0f, 360f);
	}

	// Token: 0x0600008D RID: 141 RVA: 0x00004D88 File Offset: 0x00002F88
	public void Update()
	{
		this.m_angle += this.Speed * 360f * Time.deltaTime;
		base.transform.rotation = Quaternion.Euler(0f, -this.m_angle, 0f);
	}

	// Token: 0x040000AE RID: 174
	public float Speed;

	// Token: 0x040000AF RID: 175
	private float m_angle;
}
