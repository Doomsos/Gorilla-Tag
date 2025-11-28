using System;
using UnityEngine;

// Token: 0x02000289 RID: 649
public class TasselPhysics : MonoBehaviour
{
	// Token: 0x060010B4 RID: 4276 RVA: 0x00057000 File Offset: 0x00055200
	private void Awake()
	{
		this.centerOfMassLength = this.localCenterOfMass.magnitude;
		if (this.LockXAxis)
		{
			this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(Vector3.right, this.localCenterOfMass));
			return;
		}
		this.rotCorrection = Quaternion.Inverse(Quaternion.LookRotation(this.localCenterOfMass));
	}

	// Token: 0x060010B5 RID: 4277 RVA: 0x00057058 File Offset: 0x00055258
	private void Update()
	{
		float y = base.transform.lossyScale.y;
		this.velocity *= this.drag;
		this.velocity.y = this.velocity.y - this.gravityStrength * y * Time.deltaTime;
		Vector3 position = base.transform.position;
		Vector3 vector = this.lastCenterPos + this.velocity * Time.deltaTime;
		Vector3 vector2 = position + (vector - position).normalized * this.centerOfMassLength * y;
		this.velocity = (vector2 - this.lastCenterPos) / Time.deltaTime;
		this.lastCenterPos = vector2;
		if (this.LockXAxis)
		{
			foreach (GameObject gameObject in this.tasselInstances)
			{
				gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.right, vector2 - position) * this.rotCorrection;
			}
			return;
		}
		foreach (GameObject gameObject2 in this.tasselInstances)
		{
			gameObject2.transform.rotation = Quaternion.LookRotation(vector2 - position, gameObject2.transform.position - position) * this.rotCorrection;
		}
	}

	// Token: 0x040014CB RID: 5323
	[SerializeField]
	private GameObject[] tasselInstances;

	// Token: 0x040014CC RID: 5324
	[SerializeField]
	private Vector3 localCenterOfMass;

	// Token: 0x040014CD RID: 5325
	[SerializeField]
	private float gravityStrength;

	// Token: 0x040014CE RID: 5326
	[SerializeField]
	private float drag;

	// Token: 0x040014CF RID: 5327
	[SerializeField]
	private bool LockXAxis;

	// Token: 0x040014D0 RID: 5328
	private Vector3 lastCenterPos;

	// Token: 0x040014D1 RID: 5329
	private Vector3 velocity;

	// Token: 0x040014D2 RID: 5330
	private float centerOfMassLength;

	// Token: 0x040014D3 RID: 5331
	private Quaternion rotCorrection;
}
