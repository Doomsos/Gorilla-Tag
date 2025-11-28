using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02000E40 RID: 3648
	public sealed class SoakTaskBreakable : IGhostReactorSoakTask
	{
		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x06005AE8 RID: 23272 RVA: 0x001D1ACA File Offset: 0x001CFCCA
		// (set) Token: 0x06005AE9 RID: 23273 RVA: 0x001D1AD2 File Offset: 0x001CFCD2
		public bool Complete { get; private set; }

		// Token: 0x06005AEA RID: 23274 RVA: 0x001D1ADB File Offset: 0x001CFCDB
		public SoakTaskBreakable(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x06005AEB RID: 23275 RVA: 0x001D1AEC File Offset: 0x001CFCEC
		public bool Update()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._breakable != null && this._breakable.GetComponent<GRBreakable>().BrokenLocal)
			{
				Debug.Log(string.Format("soak breakable {0} is broken", this._breakable.id.index));
				this._breakable = null;
				this._nextHitTime = default(float?);
				this.Complete = true;
			}
			else
			{
				if (this._breakable == null)
				{
					using (List<GameEntity>.Enumerator enumerator = managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GameEntity gameEntity = enumerator.Current;
							if (!(gameEntity == null) && !gameEntity.IsHeld())
							{
								GRBreakable component = gameEntity.gameObject.GetComponent<GRBreakable>();
								if (component != null && !component.BrokenLocal)
								{
									this._breakable = gameEntity;
									this._nextHitTime = new float?(Time.time + 0.1f);
									break;
								}
							}
						}
						return true;
					}
				}
				if (this._breakable != null)
				{
					float? nextHitTime = this._nextHitTime;
					if (nextHitTime != null)
					{
						float valueOrDefault = nextHitTime.GetValueOrDefault();
						if (Time.time >= valueOrDefault)
						{
							Debug.Log(string.Format("soak hit breakable {0}", this._breakable.id.index));
							GameHitData hit = new GameHitData
							{
								hitEntityId = this._breakable.id,
								hitByEntityId = this._breakable.id,
								hitTypeId = 0,
								hitEntityPosition = Vector3.zero,
								hitPosition = Vector3.zero,
								hitImpulse = Vector3.zero,
								hitAmount = 1
							};
							managerForZone.RequestHit(hit);
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06005AEC RID: 23276 RVA: 0x001D1CF4 File Offset: 0x001CFEF4
		public void Reset()
		{
			this._breakable = null;
			this._nextHitTime = default(float?);
			this.Complete = false;
		}

		// Token: 0x0400680D RID: 26637
		public const float TIME_BETWEEN_HITS = 0.1f;

		// Token: 0x0400680E RID: 26638
		private readonly GRPlayer _grPlayer;

		// Token: 0x0400680F RID: 26639
		private GameEntity _breakable;

		// Token: 0x04006810 RID: 26640
		private float? _nextHitTime;
	}
}
