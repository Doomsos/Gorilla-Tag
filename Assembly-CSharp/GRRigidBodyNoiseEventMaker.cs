using System;
using UnityEngine;

// Token: 0x020006F2 RID: 1778
public class GRRigidBodyNoiseEventMaker : MonoBehaviour
{
	// Token: 0x06002D98 RID: 11672 RVA: 0x000F674C File Offset: 0x000F494C
	public void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > this.velocityThreshold && base.GetComponent<GameEntity>() != null)
		{
			GRNoiseEventManager.instance.AddNoiseEvent(collision.GetContact(0).point, 1f, 1f);
		}
	}

	// Token: 0x04003B46 RID: 15174
	public float velocityThreshold = 5f;
}
