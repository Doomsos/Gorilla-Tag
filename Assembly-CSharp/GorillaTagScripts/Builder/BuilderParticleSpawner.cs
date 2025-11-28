using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000E4A RID: 3658
	public class BuilderParticleSpawner : MonoBehaviour
	{
		// Token: 0x06005B37 RID: 23351 RVA: 0x001D3B08 File Offset: 0x001D1D08
		private void Start()
		{
			this.spawnTrigger.onTriggerFirstEntered += new Action(this.OnEnter);
			this.spawnTrigger.onTriggerLastExited += new Action(this.OnExit);
		}

		// Token: 0x06005B38 RID: 23352 RVA: 0x001D3B38 File Offset: 0x001D1D38
		private void OnDestroy()
		{
			if (this.spawnTrigger != null)
			{
				this.spawnTrigger.onTriggerFirstEntered -= new Action(this.OnEnter);
				this.spawnTrigger.onTriggerLastExited -= new Action(this.OnExit);
			}
		}

		// Token: 0x06005B39 RID: 23353 RVA: 0x001D3B78 File Offset: 0x001D1D78
		public void TrySpawning()
		{
			if (Time.time > this.lastSpawnTime + this.cooldown)
			{
				this.lastSpawnTime = Time.time;
				ObjectPools.instance.Instantiate(this.prefab, this.spawnLocation.position, this.spawnLocation.rotation, this.myPiece.GetScale(), true);
			}
		}

		// Token: 0x06005B3A RID: 23354 RVA: 0x001D3BD7 File Offset: 0x001D1DD7
		private void OnEnter()
		{
			if (this.spawnOnEnter)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x06005B3B RID: 23355 RVA: 0x001D3BE7 File Offset: 0x001D1DE7
		private void OnExit()
		{
			if (this.spawnOnExit)
			{
				this.TrySpawning();
			}
		}

		// Token: 0x0400685B RID: 26715
		[SerializeField]
		private BuilderPiece myPiece;

		// Token: 0x0400685C RID: 26716
		public GameObject prefab;

		// Token: 0x0400685D RID: 26717
		public float cooldown = 0.1f;

		// Token: 0x0400685E RID: 26718
		private float lastSpawnTime;

		// Token: 0x0400685F RID: 26719
		[SerializeField]
		private BuilderSmallMonkeTrigger spawnTrigger;

		// Token: 0x04006860 RID: 26720
		[SerializeField]
		private bool spawnOnEnter = true;

		// Token: 0x04006861 RID: 26721
		[SerializeField]
		private bool spawnOnExit;

		// Token: 0x04006862 RID: 26722
		[SerializeField]
		private Transform spawnLocation;
	}
}
