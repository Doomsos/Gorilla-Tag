using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class LongScarfSim : MonoBehaviour
{
	// Token: 0x0600100E RID: 4110 RVA: 0x000547D4 File Offset: 0x000529D4
	private void Start()
	{
		this.clampToPlane.Normalize();
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.baseLocalRotations = new Quaternion[this.gameObjects.Length];
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			this.baseLocalRotations[i] = this.gameObjects[i].transform.localRotation;
		}
	}

	// Token: 0x0600100F RID: 4111 RVA: 0x0005483C File Offset: 0x00052A3C
	private void LateUpdate()
	{
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector2 = position + (vector - position).normalized * this.centerOfMassLength;
		Vector3 vector3 = base.transform.InverseTransformPoint(vector2);
		float num = Vector3.Dot(vector3, this.clampToPlane);
		if (num < 0f)
		{
			vector3 -= this.clampToPlane * num;
			vector2 = base.transform.TransformPoint(vector3);
		}
		Vector3 vector4 = vector2;
		this.velocity = (vector4 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = vector4;
		float num2 = (float)(this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold) ? 1 : 0);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, num2, this.blendAmountPerSecond * Time.deltaTime);
		Quaternion quaternion = Quaternion.LookRotation(vector4 - position);
		for (int i = 0; i < this.gameObjects.Length; i++)
		{
			Quaternion quaternion2 = this.gameObjects[i].transform.parent.rotation * this.baseLocalRotations[i];
			this.gameObjects[i].transform.rotation = Quaternion.Lerp(quaternion2, quaternion, this.currentBlend);
		}
	}

	// Token: 0x040013FF RID: 5119
	[SerializeField]
	private GameObject[] gameObjects;

	// Token: 0x04001400 RID: 5120
	[SerializeField]
	private float speedThreshold = 1f;

	// Token: 0x04001401 RID: 5121
	[SerializeField]
	private float blendAmountPerSecond = 1f;

	// Token: 0x04001402 RID: 5122
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001403 RID: 5123
	private Quaternion[] baseLocalRotations;

	// Token: 0x04001404 RID: 5124
	private float currentBlend;

	// Token: 0x04001405 RID: 5125
	[SerializeField]
	private float centerOfMassLength;

	// Token: 0x04001406 RID: 5126
	[SerializeField]
	private float gravityStrength;

	// Token: 0x04001407 RID: 5127
	[SerializeField]
	private float drag;

	// Token: 0x04001408 RID: 5128
	[SerializeField]
	private Vector3 clampToPlane;

	// Token: 0x04001409 RID: 5129
	private Vector3 lastCenterPos;

	// Token: 0x0400140A RID: 5130
	private Vector3 velocity;
}
