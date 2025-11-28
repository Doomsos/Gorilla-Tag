using System;
using GorillaTag.Reactions;
using UnityEngine;

// Token: 0x02000CA3 RID: 3235
[RequireComponent(typeof(SpawnWorldEffects))]
public class SpawnWorldEffectsTrigger : MonoBehaviour
{
	// Token: 0x06004EF6 RID: 20214 RVA: 0x001984C6 File Offset: 0x001966C6
	private void OnEnable()
	{
		if (this.swe == null)
		{
			this.swe = base.GetComponent<SpawnWorldEffects>();
		}
	}

	// Token: 0x06004EF7 RID: 20215 RVA: 0x001984E2 File Offset: 0x001966E2
	private void OnTriggerEnter(Collider other)
	{
		this.spawnTime = Time.time;
		this.swe.RequestSpawn(base.transform.position);
	}

	// Token: 0x06004EF8 RID: 20216 RVA: 0x00198505 File Offset: 0x00196705
	private void OnTriggerStay(Collider other)
	{
		if (Time.time - this.spawnTime < this.spawnCooldown)
		{
			return;
		}
		this.swe.RequestSpawn(base.transform.position);
		this.spawnTime = Time.time;
	}

	// Token: 0x04005DA6 RID: 23974
	private SpawnWorldEffects swe;

	// Token: 0x04005DA7 RID: 23975
	private float spawnTime;

	// Token: 0x04005DA8 RID: 23976
	[SerializeField]
	private float spawnCooldown = 1f;
}
