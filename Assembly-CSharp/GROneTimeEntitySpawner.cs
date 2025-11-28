using System;
using UnityEngine;

// Token: 0x020006D9 RID: 1753
public class GROneTimeEntitySpawner : MonoBehaviour
{
	// Token: 0x06002CE1 RID: 11489 RVA: 0x000F319F File Offset: 0x000F139F
	private void Start()
	{
		if (this.EntityPrefab == null)
		{
			Debug.Log("Can't  spawn null entity", this);
		}
		base.Invoke("TrySpawn", this.SpawnDelay);
	}

	// Token: 0x06002CE2 RID: 11490 RVA: 0x00002789 File Offset: 0x00000989
	private void Update()
	{
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x000F31CC File Offset: 0x000F13CC
	private void TrySpawn()
	{
		if (!this.bHasSpawned && this.EntityPrefab != null)
		{
			Debug.Log("trying to spawn entity" + this.EntityPrefab.name, this);
			GameEntityManager gameEntityManager = this.reactor.grManager.gameEntityManager;
			if (gameEntityManager.IsAuthority())
			{
				if (!gameEntityManager.IsZoneActive())
				{
					Debug.Log("delaying spawn attempt because zone not active", this);
					base.Invoke("TrySpawn", 0.2f);
					return;
				}
				Debug.Log("trying to spawn entity", this);
				gameEntityManager.RequestCreateItem(this.EntityPrefab.name.GetStaticHash(), base.transform.position + new Vector3(0f, 0f, 0f), base.transform.rotation, 0L);
				this.bHasSpawned = true;
			}
		}
	}

	// Token: 0x04003A44 RID: 14916
	public GhostReactor reactor;

	// Token: 0x04003A45 RID: 14917
	public GameEntity EntityPrefab;

	// Token: 0x04003A46 RID: 14918
	private bool bHasSpawned;

	// Token: 0x04003A47 RID: 14919
	private float SpawnDelay = 3f;
}
