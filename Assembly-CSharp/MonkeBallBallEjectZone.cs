using System;
using UnityEngine;

// Token: 0x0200054A RID: 1354
public class MonkeBallBallEjectZone : MonoBehaviour
{
	// Token: 0x0600222A RID: 8746 RVA: 0x000B2BB4 File Offset: 0x000B0DB4
	private void OnCollisionEnter(Collision collision)
	{
		GameBall component = collision.gameObject.GetComponent<GameBall>();
		if (component != null && collision.contacts.Length != 0)
		{
			component.SetVelocity(collision.contacts[0].impulse.normalized * this.ejectVelocity);
		}
	}

	// Token: 0x04002CC9 RID: 11465
	public Transform target;

	// Token: 0x04002CCA RID: 11466
	public float ejectVelocity = 15f;
}
