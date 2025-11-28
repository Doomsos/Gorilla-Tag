using System;
using UnityEngine;

// Token: 0x020006C2 RID: 1730
public class GREntityDestroyTrigger : MonoBehaviour
{
	// Token: 0x06002C86 RID: 11398 RVA: 0x000F1490 File Offset: 0x000EF690
	private void OnTriggerEnter(Collider other)
	{
		GameEntity component = other.attachedRigidbody.GetComponent<GameEntity>();
		if (component != null && component.IsAuthority())
		{
			component.manager.RequestDestroyItem(component.id);
		}
	}
}
