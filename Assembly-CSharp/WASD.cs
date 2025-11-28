using System;
using UnityEngine;

// Token: 0x02000028 RID: 40
public class WASD : MonoBehaviour
{
	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600008F RID: 143 RVA: 0x00004DD5 File Offset: 0x00002FD5
	public Vector3 Velocity
	{
		get
		{
			return this.m_velocity;
		}
	}

	// Token: 0x06000090 RID: 144 RVA: 0x00004DE0 File Offset: 0x00002FE0
	public void Update()
	{
		Vector3 zero = Vector3.zero;
		float num = 0f;
		if (Input.GetKey(119))
		{
			zero.z += 1f;
		}
		if (Input.GetKey(97))
		{
			zero.x -= 1f;
		}
		if (Input.GetKey(115))
		{
			zero.z -= 1f;
		}
		if (Input.GetKey(100))
		{
			zero.x += 1f;
		}
		Vector3 vector = (zero.sqrMagnitude > 0f) ? (zero.normalized * this.Speed * Time.deltaTime) : Vector3.zero;
		Quaternion quaternion = Quaternion.AngleAxis(num * this.Omega * 57.29578f * Time.deltaTime, Vector3.up);
		this.m_velocity = vector / Time.deltaTime;
		base.transform.position += vector;
		base.transform.rotation = quaternion * base.transform.rotation;
	}

	// Token: 0x040000B0 RID: 176
	public float Speed = 1f;

	// Token: 0x040000B1 RID: 177
	public float Omega = 1f;

	// Token: 0x040000B2 RID: 178
	public Vector3 m_velocity;
}
