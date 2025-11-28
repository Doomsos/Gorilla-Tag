using System;
using UnityEngine;

// Token: 0x02000C38 RID: 3128
public class CollisionEventNotifier : MonoBehaviour
{
	// Token: 0x14000087 RID: 135
	// (add) Token: 0x06004CBF RID: 19647 RVA: 0x0018E918 File Offset: 0x0018CB18
	// (remove) Token: 0x06004CC0 RID: 19648 RVA: 0x0018E950 File Offset: 0x0018CB50
	public event CollisionEventNotifier.CollisionEvent CollisionEnterEvent;

	// Token: 0x14000088 RID: 136
	// (add) Token: 0x06004CC1 RID: 19649 RVA: 0x0018E988 File Offset: 0x0018CB88
	// (remove) Token: 0x06004CC2 RID: 19650 RVA: 0x0018E9C0 File Offset: 0x0018CBC0
	public event CollisionEventNotifier.CollisionEvent CollisionExitEvent;

	// Token: 0x06004CC3 RID: 19651 RVA: 0x0018E9F5 File Offset: 0x0018CBF5
	private void OnCollisionEnter(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionEnterEvent = this.CollisionEnterEvent;
		if (collisionEnterEvent == null)
		{
			return;
		}
		collisionEnterEvent(this, collision);
	}

	// Token: 0x06004CC4 RID: 19652 RVA: 0x0018EA09 File Offset: 0x0018CC09
	private void OnCollisionExit(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionExitEvent = this.CollisionExitEvent;
		if (collisionExitEvent == null)
		{
			return;
		}
		collisionExitEvent(this, collision);
	}

	// Token: 0x02000C39 RID: 3129
	// (Invoke) Token: 0x06004CC7 RID: 19655
	public delegate void CollisionEvent(CollisionEventNotifier notifier, Collision collision);
}
