using System;
using UnityEngine;

// Token: 0x020006C3 RID: 1731
public class GREntityLifetime : MonoBehaviour
{
	// Token: 0x06002C88 RID: 11400 RVA: 0x000F14CB File Offset: 0x000EF6CB
	private void Start()
	{
		this.entity = base.GetComponent<GameEntity>();
		base.Invoke("DestroySelf", this.Lifetime);
	}

	// Token: 0x06002C89 RID: 11401 RVA: 0x00002789 File Offset: 0x00000989
	private void Update()
	{
	}

	// Token: 0x06002C8A RID: 11402 RVA: 0x000F14EA File Offset: 0x000EF6EA
	private void DestroySelf()
	{
		if (this.entity != null)
		{
			this.entity.manager.RequestDestroyItem(this.entity.id);
		}
	}

	// Token: 0x040039C3 RID: 14787
	public float Lifetime = 3f;

	// Token: 0x040039C4 RID: 14788
	private GameEntity entity;
}
