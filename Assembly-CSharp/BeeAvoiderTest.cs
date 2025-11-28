using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020001B6 RID: 438
public class BeeAvoiderTest : MonoBehaviour
{
	// Token: 0x06000BAA RID: 2986 RVA: 0x0003FF00 File Offset: 0x0003E100
	public void Update()
	{
		Vector3 position = this.patrolPoints[this.nextPatrolPoint].transform.position;
		Vector3 position2 = base.transform.position;
		Vector3 vector = (position - position2).normalized * this.speed;
		this.velocity = Vector3.MoveTowards(this.velocity * this.drag, vector, this.acceleration);
		if ((position2 - position).IsLongerThan(this.instabilityOffRadius))
		{
			this.velocity += Random.insideUnitSphere * this.instability * Time.deltaTime;
		}
		Vector3 vector2 = position2 + this.velocity * Time.deltaTime;
		GameObject[] array = this.avoidancePoints;
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 position3 = array[i].transform.position;
			if ((vector2 - position3).IsShorterThan(this.avoidRadius))
			{
				Vector3 normalized = Vector3.Cross(position3 - vector2, position - vector2).normalized;
				Vector3 normalized2 = (position - position3).normalized;
				float num = Vector3.Dot(vector2 - position3, normalized);
				Vector3 vector3 = (this.avoidRadius - num) * normalized;
				vector2 += vector3;
				this.velocity += vector3;
			}
		}
		base.transform.position = vector2;
		base.transform.rotation = Quaternion.LookRotation(position - vector2);
		if ((vector2 - position).IsShorterThan(this.patrolArrivedRadius))
		{
			this.nextPatrolPoint = (this.nextPatrolPoint + 1) % this.patrolPoints.Length;
		}
	}

	// Token: 0x04000E6C RID: 3692
	public GameObject[] patrolPoints;

	// Token: 0x04000E6D RID: 3693
	public GameObject[] avoidancePoints;

	// Token: 0x04000E6E RID: 3694
	public float speed;

	// Token: 0x04000E6F RID: 3695
	public float acceleration;

	// Token: 0x04000E70 RID: 3696
	public float instability;

	// Token: 0x04000E71 RID: 3697
	public float instabilityOffRadius;

	// Token: 0x04000E72 RID: 3698
	public float drag;

	// Token: 0x04000E73 RID: 3699
	public float avoidRadius;

	// Token: 0x04000E74 RID: 3700
	public float patrolArrivedRadius;

	// Token: 0x04000E75 RID: 3701
	private int nextPatrolPoint;

	// Token: 0x04000E76 RID: 3702
	private Vector3 velocity;
}
