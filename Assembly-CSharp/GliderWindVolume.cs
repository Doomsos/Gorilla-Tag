using System;
using UnityEngine;

// Token: 0x02000BAC RID: 2988
public class GliderWindVolume : MonoBehaviour
{
	// Token: 0x060049D1 RID: 18897 RVA: 0x00184DBF File Offset: 0x00182FBF
	public void SetProperties(float speed, float accel, AnimationCurve svaCurve, Vector3 windDirection)
	{
		this.maxSpeed = speed;
		this.maxAccel = accel;
		this.speedVsAccelCurve.CopyFrom(svaCurve);
		this.localWindDirection = windDirection;
	}

	// Token: 0x170006DF RID: 1759
	// (get) Token: 0x060049D2 RID: 18898 RVA: 0x00184DE3 File Offset: 0x00182FE3
	public Vector3 WindDirection
	{
		get
		{
			return base.transform.TransformDirection(this.localWindDirection);
		}
	}

	// Token: 0x060049D3 RID: 18899 RVA: 0x00184DF8 File Offset: 0x00182FF8
	public Vector3 GetAccelFromVelocity(Vector3 velocity)
	{
		Vector3 windDirection = this.WindDirection;
		float num = Mathf.Clamp(Vector3.Dot(velocity, windDirection), -this.maxSpeed, this.maxSpeed) / this.maxSpeed;
		float num2 = this.speedVsAccelCurve.Evaluate(num) * this.maxAccel;
		return windDirection * num2;
	}

	// Token: 0x04005AAB RID: 23211
	[SerializeField]
	private float maxSpeed = 30f;

	// Token: 0x04005AAC RID: 23212
	[SerializeField]
	private float maxAccel = 15f;

	// Token: 0x04005AAD RID: 23213
	[SerializeField]
	private AnimationCurve speedVsAccelCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x04005AAE RID: 23214
	[SerializeField]
	private Vector3 localWindDirection = Vector3.up;
}
