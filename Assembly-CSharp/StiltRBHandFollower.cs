using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000155 RID: 341
public class StiltRBHandFollower : MonoBehaviour
{
	// Token: 0x06000909 RID: 2313 RVA: 0x00030C19 File Offset: 0x0002EE19
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.rb.maxAngularVelocity = this.angularSpeedLimit;
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00030C38 File Offset: 0x0002EE38
	private void FixedUpdate()
	{
		Vector3 vector = this.targetHand.TransformPoint(this.handOffset);
		float num;
		Vector3 vector2;
		(this.targetHand.TransformRotation(this.handRotOffset) * Quaternion.Inverse(this.rb.transform.rotation)).ToAngleAxis(ref num, ref vector2);
		this.rb.linearVelocity = (vector - this.rb.transform.position) / Time.fixedDeltaTime;
		this.rb.angularVelocity = vector2 * num * 0.017453292f / Time.fixedDeltaTime;
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00030CDF File Offset: 0x0002EEDF
	private void OnCollisionEnter(Collision collision)
	{
		this.collisions[collision.collider] = collision.contacts[0].point;
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00030CDF File Offset: 0x0002EEDF
	private void OnCollisionStay(Collision collision)
	{
		this.collisions[collision.collider] = collision.contacts[0].point;
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00030D03 File Offset: 0x0002EF03
	private void OnCollisionExit(Collision collision)
	{
		this.collisions.Remove(collision.collider);
	}

	// Token: 0x04000B1A RID: 2842
	private Rigidbody rb;

	// Token: 0x04000B1B RID: 2843
	[SerializeField]
	private Transform targetHand;

	// Token: 0x04000B1C RID: 2844
	[SerializeField]
	private Vector3 handOffset;

	// Token: 0x04000B1D RID: 2845
	[SerializeField]
	private Quaternion handRotOffset = Quaternion.identity;

	// Token: 0x04000B1E RID: 2846
	[SerializeField]
	private float angularSpeedLimit;

	// Token: 0x04000B1F RID: 2847
	private Dictionary<Collider, Vector3> collisions = new Dictionary<Collider, Vector3>();
}
