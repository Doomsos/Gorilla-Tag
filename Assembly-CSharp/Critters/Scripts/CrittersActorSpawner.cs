using System;
using GorillaExtensions;
using UnityEngine;

namespace Critters.Scripts
{
	// Token: 0x0200114A RID: 4426
	public class CrittersActorSpawner : MonoBehaviour
	{
		// Token: 0x06006FBE RID: 28606 RVA: 0x002463EC File Offset: 0x002445EC
		private void Awake()
		{
			this.spawnPoint.OnSpawnChanged += new Action<CrittersActor>(this.HandleSpawnedActor);
		}

		// Token: 0x06006FBF RID: 28607 RVA: 0x00246405 File Offset: 0x00244605
		private void OnEnable()
		{
			if (!CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Add(this);
			}
		}

		// Token: 0x06006FC0 RID: 28608 RVA: 0x0024642D File Offset: 0x0024462D
		private void OnDisable()
		{
			if (CrittersManager.instance.actorSpawners.Contains(this))
			{
				CrittersManager.instance.actorSpawners.Remove(this);
			}
		}

		// Token: 0x06006FC1 RID: 28609 RVA: 0x00246458 File Offset: 0x00244658
		public void ProcessLocal()
		{
			if (!CrittersManager.instance.LocalAuthority())
			{
				return;
			}
			if (this.nextSpawnTime <= (double)Time.time)
			{
				this.nextSpawnTime = (double)(Time.time + (float)this.spawnDelay);
				if (this.currentSpawnedObject == null || !this.currentSpawnedObject.isEnabled)
				{
					this.SpawnActor();
				}
			}
			if (this.currentSpawnedObject.IsNotNull())
			{
				if (!this.currentSpawnedObject.isEnabled)
				{
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.insideSpawnerCheck.bounds.Contains(this.currentSpawnedObject.transform.position))
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
					return;
				}
				if (!this.VerifySpawnAttached())
				{
					this.currentSpawnedObject.RemoveDespawnBlock();
					this.currentSpawnedObject = null;
					this.spawnPoint.SetSpawnedActor(null);
				}
			}
		}

		// Token: 0x06006FC2 RID: 28610 RVA: 0x00246552 File Offset: 0x00244752
		public void DoReset()
		{
			this.currentSpawnedObject = null;
		}

		// Token: 0x06006FC3 RID: 28611 RVA: 0x0024655B File Offset: 0x0024475B
		private void HandleSpawnedActor(CrittersActor spawnedActor)
		{
			this.currentSpawnedObject = spawnedActor;
		}

		// Token: 0x06006FC4 RID: 28612 RVA: 0x00246564 File Offset: 0x00244764
		private void SpawnActor()
		{
			CrittersActor crittersActor = CrittersManager.instance.SpawnActor(this.actorType, this.subActorIndex);
			this.spawnPoint.SetSpawnedActor(crittersActor);
			if (crittersActor.IsNull())
			{
				return;
			}
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				crittersActor.GrabbedBy(this.spawnPoint, true, default(Quaternion), default(Vector3), false);
				return;
			}
			crittersActor.MoveActor(this.spawnPoint.transform.position, this.spawnPoint.transform.rotation, false, true, true);
			crittersActor.rb.linearVelocity = Vector3.zero;
			if (this.applyImpulseOnSpawn)
			{
				crittersActor.SetImpulse();
			}
		}

		// Token: 0x06006FC5 RID: 28613 RVA: 0x00246610 File Offset: 0x00244810
		private bool VerifySpawnAttached()
		{
			if (this.attachSpawnedObjectToSpawnLocation)
			{
				CrittersActor crittersActor;
				CrittersManager.instance.actorById.TryGetValue(this.currentSpawnedObject.parentActorId, ref crittersActor);
				if (crittersActor.IsNull() || crittersActor != this.spawnPoint)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04008029 RID: 32809
		public CrittersActorSpawnerPoint spawnPoint;

		// Token: 0x0400802A RID: 32810
		public CrittersActor currentSpawnedObject;

		// Token: 0x0400802B RID: 32811
		public CrittersActor.CrittersActorType actorType;

		// Token: 0x0400802C RID: 32812
		public int subActorIndex = -1;

		// Token: 0x0400802D RID: 32813
		public Collider insideSpawnerCheck;

		// Token: 0x0400802E RID: 32814
		public int spawnDelay = 5;

		// Token: 0x0400802F RID: 32815
		public bool applyImpulseOnSpawn = true;

		// Token: 0x04008030 RID: 32816
		public bool attachSpawnedObjectToSpawnLocation;

		// Token: 0x04008031 RID: 32817
		private double nextSpawnTime;
	}
}
