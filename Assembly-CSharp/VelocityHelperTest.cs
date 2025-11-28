using System;
using UnityEngine;

// Token: 0x020009FB RID: 2555
public class VelocityHelperTest : MonoBehaviour
{
	// Token: 0x0600416F RID: 16751 RVA: 0x0015BBD9 File Offset: 0x00159DD9
	private void Setup()
	{
		this.lastPosition = base.transform.position;
		this.lastVelocity = Vector3.zero;
		this.velocity = Vector3.zero;
		this.speed = 0f;
	}

	// Token: 0x06004170 RID: 16752 RVA: 0x0015BC0D File Offset: 0x00159E0D
	private void Start()
	{
		this.Setup();
	}

	// Token: 0x06004171 RID: 16753 RVA: 0x0015BC18 File Offset: 0x00159E18
	private void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = (position - this.lastPosition) / deltaTime;
		this.velocity = Vector3.Lerp(this.lastVelocity, vector, deltaTime);
		this.speed = this.velocity.magnitude;
		this.lastPosition = position;
		this.lastVelocity = vector;
	}

	// Token: 0x06004172 RID: 16754 RVA: 0x00002789 File Offset: 0x00000989
	private void Update()
	{
	}

	// Token: 0x04005239 RID: 21049
	public Vector3 velocity;

	// Token: 0x0400523A RID: 21050
	public float speed;

	// Token: 0x0400523B RID: 21051
	[Space]
	public Vector3 lastVelocity;

	// Token: 0x0400523C RID: 21052
	public Vector3 lastPosition;

	// Token: 0x0400523D RID: 21053
	[Space]
	[SerializeField]
	private float[] _deltaTimes = new float[5];
}
