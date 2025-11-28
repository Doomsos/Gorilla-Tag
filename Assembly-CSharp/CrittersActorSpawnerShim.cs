using System;
using Critters.Scripts;
using UnityEngine;

// Token: 0x02000048 RID: 72
public class CrittersActorSpawnerShim : MonoBehaviour
{
	// Token: 0x06000168 RID: 360 RVA: 0x00009564 File Offset: 0x00007764
	[ContextMenu("Copy Spawner Data To Shim")]
	private CrittersActorSpawner CopySpawnerDataInPrefab()
	{
		CrittersActorSpawner component = base.gameObject.GetComponent<CrittersActorSpawner>();
		this.spawnerPointTransform = component.spawnPoint.transform;
		this.actorType = component.actorType;
		this.subActorIndex = component.subActorIndex;
		this.insideSpawnerBounds = (BoxCollider)component.insideSpawnerCheck;
		this.spawnDelay = component.spawnDelay;
		this.applyImpulseOnSpawn = component.applyImpulseOnSpawn;
		this.attachSpawnedObjectToSpawnLocation = component.attachSpawnedObjectToSpawnLocation;
		this.colliderTrigger = base.gameObject.GetComponent<BoxCollider>();
		return component;
	}

	// Token: 0x06000169 RID: 361 RVA: 0x000095F0 File Offset: 0x000077F0
	[ContextMenu("Replace Spawner With Shim")]
	private void ReplaceSpawnerWithShim()
	{
		CrittersActorSpawner crittersActorSpawner = this.CopySpawnerDataInPrefab();
		if (crittersActorSpawner.spawnPoint.GetComponent<Rigidbody>() != null)
		{
			Object.DestroyImmediate(crittersActorSpawner.spawnPoint.GetComponent<Rigidbody>());
		}
		Object.DestroyImmediate(crittersActorSpawner.spawnPoint);
		Object.DestroyImmediate(crittersActorSpawner);
	}

	// Token: 0x04000187 RID: 391
	public Transform spawnerPointTransform;

	// Token: 0x04000188 RID: 392
	public CrittersActor.CrittersActorType actorType;

	// Token: 0x04000189 RID: 393
	public int subActorIndex;

	// Token: 0x0400018A RID: 394
	public BoxCollider insideSpawnerBounds;

	// Token: 0x0400018B RID: 395
	public int spawnDelay;

	// Token: 0x0400018C RID: 396
	public bool applyImpulseOnSpawn;

	// Token: 0x0400018D RID: 397
	public bool attachSpawnedObjectToSpawnLocation;

	// Token: 0x0400018E RID: 398
	public BoxCollider colliderTrigger;
}
