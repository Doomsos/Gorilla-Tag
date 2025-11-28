using System;
using GorillaExtensions;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x02000253 RID: 595
public class FakeWheelDriver : MonoBehaviour
{
	// Token: 0x1700016F RID: 367
	// (get) Token: 0x06000F82 RID: 3970 RVA: 0x00052310 File Offset: 0x00050510
	// (set) Token: 0x06000F83 RID: 3971 RVA: 0x00052318 File Offset: 0x00050518
	public bool hasCollision { get; private set; }

	// Token: 0x06000F84 RID: 3972 RVA: 0x00052321 File Offset: 0x00050521
	public void SetThrust(Vector3 thrust)
	{
		this.thrust = thrust;
	}

	// Token: 0x06000F85 RID: 3973 RVA: 0x0005232C File Offset: 0x0005052C
	private void OnCollisionStay(Collision collision)
	{
		int num = 0;
		Vector3 vector = Vector3.zero;
		foreach (ContactPoint contactPoint in collision.contacts)
		{
			if (contactPoint.thisCollider == this.wheelCollider)
			{
				vector += contactPoint.point;
				num++;
			}
		}
		if (num > 0)
		{
			this.collisionNormal = collision.contacts[0].normal;
			this.collisionPoint = vector / (float)num;
			this.hasCollision = true;
		}
	}

	// Token: 0x06000F86 RID: 3974 RVA: 0x000523B8 File Offset: 0x000505B8
	private void FixedUpdate()
	{
		if (this.hasCollision)
		{
			Vector3 vector = base.transform.rotation * this.thrust;
			if (this.myRigidBody.linearVelocity.IsShorterThan(this.maxSpeed))
			{
				vector = UnityVectorExtensions.ProjectOntoPlane(vector, this.collisionNormal).normalized * this.thrust.magnitude;
				this.myRigidBody.AddForceAtPosition(vector, this.collisionPoint);
			}
			Vector3 vector2 = UnityVectorExtensions.ProjectOntoPlane(UnityVectorExtensions.ProjectOntoPlane(this.myRigidBody.linearVelocity, this.collisionNormal), vector.normalized);
			if (vector2.IsLongerThan(this.lateralFrictionForce))
			{
				this.myRigidBody.AddForceAtPosition(-vector2.normalized * this.lateralFrictionForce, this.collisionPoint);
			}
			else
			{
				this.myRigidBody.AddForceAtPosition(-vector2, this.collisionPoint);
			}
		}
		this.hasCollision = false;
	}

	// Token: 0x04001325 RID: 4901
	[SerializeField]
	private Rigidbody myRigidBody;

	// Token: 0x04001326 RID: 4902
	[SerializeField]
	private Vector3 thrust;

	// Token: 0x04001327 RID: 4903
	[SerializeField]
	private Collider wheelCollider;

	// Token: 0x04001328 RID: 4904
	[SerializeField]
	private float maxSpeed;

	// Token: 0x04001329 RID: 4905
	[SerializeField]
	private float lateralFrictionForce;

	// Token: 0x0400132B RID: 4907
	private Vector3 collisionPoint;

	// Token: 0x0400132C RID: 4908
	private Vector3 collisionNormal;
}
