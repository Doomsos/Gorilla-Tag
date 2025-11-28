using System;
using UnityEngine;

// Token: 0x0200070A RID: 1802
public class GRSpotlight : MonoBehaviourTick
{
	// Token: 0x06002E4C RID: 11852 RVA: 0x000FBBEC File Offset: 0x000F9DEC
	private void Awake()
	{
		this.yStart = base.transform.rotation.eulerAngles.y;
		this.xStart = base.transform.rotation.eulerAngles.x;
		this.timeOffset = Random.value * 360f;
		this.yFrequency += Random.value / 100f;
		this.xFrequency += Random.value / 100f;
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x000FBC78 File Offset: 0x000F9E78
	public override void Tick()
	{
		base.transform.eulerAngles = new Vector3(this.xStart + this.xAmplitude * Mathf.Sin(Time.time * this.xFrequency), this.yStart + this.yAmplitude * Mathf.Cos(Time.time * this.yFrequency), 0f);
	}

	// Token: 0x04003C6D RID: 15469
	public float yAmplitude = 75f;

	// Token: 0x04003C6E RID: 15470
	public float xAmplitude = 40f;

	// Token: 0x04003C6F RID: 15471
	public float yFrequency = 0.2f;

	// Token: 0x04003C70 RID: 15472
	public float xFrequency = 0.3f;

	// Token: 0x04003C71 RID: 15473
	private float yStart;

	// Token: 0x04003C72 RID: 15474
	private float xStart;

	// Token: 0x04003C73 RID: 15475
	private float timeOffset;
}
