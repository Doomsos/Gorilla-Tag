using System;
using UnityEngine;

// Token: 0x02000C38 RID: 3128
public class CollisionEventNotifier : MonoBehaviour
{
	// Token: 0x14000087 RID: 135
	// (add) Token: 0x06004CBF RID: 19647 RVA: 0x0018E938 File Offset: 0x0018CB38
	// (remove) Token: 0x06004CC0 RID: 19648 RVA: 0x0018E970 File Offset: 0x0018CB70
	public event CollisionEventNotifier.CollisionEvent CollisionEnterEvent;

	// Token: 0x14000088 RID: 136
	// (add) Token: 0x06004CC1 RID: 19649 RVA: 0x0018E9A8 File Offset: 0x0018CBA8
	// (remove) Token: 0x06004CC2 RID: 19650 RVA: 0x0018E9E0 File Offset: 0x0018CBE0
	public event CollisionEventNotifier.CollisionEvent CollisionExitEvent;

	// Token: 0x06004CC3 RID: 19651 RVA: 0x0018EA15 File Offset: 0x0018CC15
	private void OnCollisionEnter(Collision collision)
	{
		CollisionEventNotifier.CollisionEvent collisionEnterEvent = this.CollisionEnterEvent;
		if (collisionEnterEvent == null)
		{
			return;
		}
		collisionEnterEvent(this, collision);
	}

	// Token: 0x06004CC4 RID: 19652 RVA: 0x0018EA29 File Offset: 0x0018CC29
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
