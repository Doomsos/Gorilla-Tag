using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.Reactions
{
	// Token: 0x02001030 RID: 4144
	public class SpawnWorldEffects : MonoBehaviour
	{
		// Token: 0x060068AF RID: 26799 RVA: 0x00221AFC File Offset: 0x0021FCFC
		protected void OnEnable()
		{
			if (GorillaComputer.instance == null)
			{
				Debug.LogError("SpawnWorldEffects: Disabling because GorillaComputer not found! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			if (this._prefabToSpawn != null && !this._isPrefabInPool)
			{
				if (this._prefabToSpawn.CompareTag("Untagged"))
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab has no tag! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._isPrefabInPool = ObjectPools.instance.DoesPoolExist(this._prefabToSpawn);
				if (!this._isPrefabInPool)
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab not in pool! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._pool = ObjectPools.instance.GetPoolByObjectType(this._prefabToSpawn);
			}
			this._hasPrefabToSpawn = (this._prefabToSpawn != null && this._isPrefabInPool);
		}

		// Token: 0x060068B0 RID: 26800 RVA: 0x00221C00 File Offset: 0x0021FE00
		public void RequestSpawn(Vector3 worldPosition)
		{
			this.RequestSpawn(worldPosition, Vector3.up);
		}

		// Token: 0x060068B1 RID: 26801 RVA: 0x00221C10 File Offset: 0x0021FE10
		public void RequestSpawn(Vector3 worldPosition, Vector3 normal)
		{
			if (this._maxParticleHitReactionRate < 1E-05f || !FireManager.hasInstance)
			{
				return;
			}
			double num = GTTime.TimeAsDouble();
			if ((float)(num - this._lastCollisionTime) < 1f / this._maxParticleHitReactionRate)
			{
				return;
			}
			if (this._hasPrefabToSpawn && this._isPrefabInPool && this._pool.GetInactiveCount() > 0)
			{
				FireManager.SpawnFire(this._pool, worldPosition, normal, base.transform.lossyScale.x);
			}
			this._lastCollisionTime = num;
		}

		// Token: 0x04007771 RID: 30577
		[Tooltip("The defaults are numbers for the flamethrower hair dryer.")]
		private readonly float _maxParticleHitReactionRate = 2f;

		// Token: 0x04007772 RID: 30578
		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject _prefabToSpawn;

		// Token: 0x04007773 RID: 30579
		private bool _hasPrefabToSpawn;

		// Token: 0x04007774 RID: 30580
		private bool _isPrefabInPool;

		// Token: 0x04007775 RID: 30581
		private double _lastCollisionTime;

		// Token: 0x04007776 RID: 30582
		private SinglePool _pool;
	}
}
