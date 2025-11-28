using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009F0 RID: 2544
public class ParticleCollisionListener : MonoBehaviour
{
	// Token: 0x060040CF RID: 16591 RVA: 0x0015A6E4 File Offset: 0x001588E4
	private void Awake()
	{
		this._events = new List<ParticleCollisionEvent>();
	}

	// Token: 0x060040D0 RID: 16592 RVA: 0x00002789 File Offset: 0x00000989
	protected virtual void OnCollisionEvent(ParticleCollisionEvent ev)
	{
	}

	// Token: 0x060040D1 RID: 16593 RVA: 0x0015A6F4 File Offset: 0x001588F4
	public void OnParticleCollision(GameObject other)
	{
		int collisionEvents = ParticlePhysicsExtensions.GetCollisionEvents(this.target, other, this._events);
		for (int i = 0; i < collisionEvents; i++)
		{
			this.OnCollisionEvent(this._events[i]);
		}
	}

	// Token: 0x04005207 RID: 20999
	public ParticleSystem target;

	// Token: 0x04005208 RID: 21000
	[SerializeReference]
	private List<ParticleCollisionEvent> _events = new List<ParticleCollisionEvent>();
}
