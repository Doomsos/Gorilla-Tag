using System;
using UnityEngine;

// Token: 0x020000CB RID: 203
[RequireComponent(typeof(SIGadgetBlasterProjectile))]
public class SIGadgetProjectileStretchVisuals : MonoBehaviourTick
{
	// Token: 0x060004EB RID: 1259 RVA: 0x0001C594 File Offset: 0x0001A794
	public new void OnEnable()
	{
		base.OnEnable();
		this.projectile = base.GetComponent<SIGadgetBlasterProjectile>();
		this.totalLength = (this.frontStretch.position - this.rearStretch.position).magnitude;
		this.distancePerFrame = this.projectile.startingVelocity * Time.fixedDeltaTime;
		this.maxStretchRatio = this.distancePerFrame / this.totalLength * this.framesPerPosition;
		this.timeSpawned = Time.time;
		this.maxSizeReached = false;
		this.baseVisuals.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.baseVisuals.transform.localScale = new Vector3(1f, 1f, 1f);
		this.frontDistance = (this.frontStretch.position - base.transform.position).magnitude;
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x0001C690 File Offset: 0x0001A890
	public override void Tick()
	{
		if (this.maxSizeReached)
		{
			return;
		}
		float num = (Time.time - this.timeSpawned) * this.projectile.startingVelocity / this.totalLength / 2f + 1f;
		if (num >= this.maxStretchRatio)
		{
			num = this.maxStretchRatio;
			this.maxSizeReached = true;
		}
		this.baseVisuals.transform.localPosition = new Vector3(0f, 0f, -(num - 1f) * this.frontDistance);
		this.baseVisuals.transform.localScale = new Vector3(1f, 1f, num);
	}

	// Token: 0x040005CE RID: 1486
	private SIGadgetBlasterProjectile projectile;

	// Token: 0x040005CF RID: 1487
	public GameObject baseVisuals;

	// Token: 0x040005D0 RID: 1488
	public Transform frontStretch;

	// Token: 0x040005D1 RID: 1489
	public Transform rearStretch;

	// Token: 0x040005D2 RID: 1490
	public float framesPerPosition;

	// Token: 0x040005D3 RID: 1491
	private float totalLength;

	// Token: 0x040005D4 RID: 1492
	private float distancePerFrame;

	// Token: 0x040005D5 RID: 1493
	private float maxStretchRatio;

	// Token: 0x040005D6 RID: 1494
	private bool maxSizeReached;

	// Token: 0x040005D7 RID: 1495
	private float frontDistance;

	// Token: 0x040005D8 RID: 1496
	private float timeSpawned;
}
