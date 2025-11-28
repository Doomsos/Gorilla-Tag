using System;
using UnityEngine;

// Token: 0x020000B3 RID: 179
public class CosmeticCritterShadeHidden : CosmeticCritter
{
	// Token: 0x06000477 RID: 1143 RVA: 0x00019AF6 File Offset: 0x00017CF6
	public void SetCenterAndRadius(Vector3 center, float radius)
	{
		this.orbitCenter = center;
		this.orbitRadius = radius;
	}

	// Token: 0x06000478 RID: 1144 RVA: 0x00019B06 File Offset: 0x00017D06
	public override void SetRandomVariables()
	{
		this.initialAngle = Random.Range(0f, 6.2831855f);
		this.orbitDirection = ((Random.value > 0.5f) ? 1f : -1f);
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00019B3C File Offset: 0x00017D3C
	public override void Tick()
	{
		float num = (float)base.GetAliveTime();
		float num2 = this.initialAngle + this.orbitDegreesPerSecond * num * this.orbitDirection;
		float num3 = this.verticalBobMagnitude * Mathf.Sin(num * this.verticalBobFrequency);
		base.transform.position = this.orbitCenter + new Vector3(this.orbitRadius * Mathf.Cos(num2), num3, this.orbitRadius * Mathf.Sin(num2));
	}

	// Token: 0x0400051B RID: 1307
	[Space]
	[Tooltip("How quickly the Shade orbits around the point where it spawned (the spawner's position).")]
	[SerializeField]
	private float orbitDegreesPerSecond;

	// Token: 0x0400051C RID: 1308
	[Tooltip("The strength of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobMagnitude;

	// Token: 0x0400051D RID: 1309
	[Tooltip("The frequency of additional up-and-down motion while orbiting.")]
	[SerializeField]
	private float verticalBobFrequency;

	// Token: 0x0400051E RID: 1310
	private Vector3 orbitCenter;

	// Token: 0x0400051F RID: 1311
	private float initialAngle;

	// Token: 0x04000520 RID: 1312
	private float orbitRadius;

	// Token: 0x04000521 RID: 1313
	private float orbitDirection;
}
