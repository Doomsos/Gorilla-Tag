using System;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class SpawnOnEnter : MonoBehaviour
{
	// Token: 0x06000ACA RID: 2762 RVA: 0x0003A908 File Offset: 0x00038B08
	public void OnTriggerEnter(Collider other)
	{
		if (Time.time > this.lastSpawnTime + this.cooldown)
		{
			this.lastSpawnTime = Time.time;
			ObjectPools.instance.Instantiate(this.prefab, other.transform.position, true);
		}
	}

	// Token: 0x04000D3D RID: 3389
	public GameObject prefab;

	// Token: 0x04000D3E RID: 3390
	public float cooldown = 0.1f;

	// Token: 0x04000D3F RID: 3391
	private float lastSpawnTime;
}
