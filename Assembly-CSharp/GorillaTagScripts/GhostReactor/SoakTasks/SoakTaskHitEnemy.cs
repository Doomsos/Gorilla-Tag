using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02000E43 RID: 3651
	public sealed class SoakTaskHitEnemy : IGhostReactorSoakTask
	{
		// Token: 0x1700087E RID: 2174
		// (get) Token: 0x06005AF7 RID: 23287 RVA: 0x001D2261 File Offset: 0x001D0461
		// (set) Token: 0x06005AF8 RID: 23288 RVA: 0x001D2269 File Offset: 0x001D0469
		public bool Complete { get; private set; }

		// Token: 0x06005AF9 RID: 23289 RVA: 0x001D2272 File Offset: 0x001D0472
		public SoakTaskHitEnemy(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x06005AFA RID: 23290 RVA: 0x001D2284 File Offset: 0x001D0484
		public bool Update()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._enemy != null && !SoakTaskHitEnemy.IsLivingEnemy(this._enemy))
			{
				Debug.Log(string.Format("soak enemy {0} is dead", this._enemy.id.index));
				this.Complete = true;
				return true;
			}
			if (this._enemy == null)
			{
				foreach (GameEntity gameEntity in managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>())
				{
					if (!(gameEntity == null) && !gameEntity.IsHeld() && !(gameEntity.GetComponent<GameAgent>() == null) && !(gameEntity.GetComponent<GameHittable>() == null) && SoakTaskHitEnemy.IsEnemy(gameEntity))
					{
						this._enemy = gameEntity;
						this._nextHitTime = new float?(Time.time + 0.1f);
						break;
					}
				}
				return this._enemy != null;
			}
			if (this._nextHitTime == null)
			{
				throw new Exception("Invalid state in HitEnemySoakTask.");
			}
			if (Time.time < this._nextHitTime.Value)
			{
				return true;
			}
			Debug.Log(string.Format("soak hitting enemy {0}", this._enemy.id.index));
			GameEntity randomTool = this.GetRandomTool();
			if (randomTool == null)
			{
				Debug.LogError("No club found for soak task hit enemy.");
				return false;
			}
			GameHitData hit = new GameHitData
			{
				hitEntityId = this._enemy.id,
				hitByEntityId = randomTool.id,
				hitTypeId = 0,
				hitEntityPosition = Vector3.zero,
				hitPosition = Vector3.zero,
				hitImpulse = Vector3.zero,
				hitAmount = 1
			};
			managerForZone.RequestHit(hit);
			this._nextHitTime = new float?(Time.time + 0.1f);
			return true;
		}

		// Token: 0x06005AFB RID: 23291 RVA: 0x001D24A4 File Offset: 0x001D06A4
		public void Reset()
		{
			this._enemy = null;
			this._nextHitTime = default(float?);
			this.Complete = false;
		}

		// Token: 0x06005AFC RID: 23292 RVA: 0x001D24C0 File Offset: 0x001D06C0
		private GameEntity GetRandomTool()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return null;
			}
			foreach (GameEntity gameEntity in managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>())
			{
				if (!(gameEntity == null))
				{
					GRTool component = gameEntity.GetComponent<GRTool>();
					if (component != null)
					{
						GRTool.GRToolType toolType = component.toolType;
						if (toolType == GRTool.GRToolType.Club || toolType == GRTool.GRToolType.HockeyStick)
						{
							return gameEntity;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06005AFD RID: 23293 RVA: 0x001D256C File Offset: 0x001D076C
		private static bool IsEnemy(GameEntity entity)
		{
			return entity.GetComponent<GREnemyChaser>() != null || entity.GetComponent<GREnemyPest>() != null || entity.GetComponent<GREnemyRanged>() != null || entity.GetComponent<GREnemySummoner>() != null;
		}

		// Token: 0x06005AFE RID: 23294 RVA: 0x001D25A8 File Offset: 0x001D07A8
		private static bool IsLivingEnemy(GameEntity entity)
		{
			if (SoakTaskHitEnemy.IsEnemy(entity))
			{
				GREnemyChaser component = entity.GetComponent<GREnemyChaser>();
				if (component == null || component.hp <= 0)
				{
					GREnemyPest component2 = entity.GetComponent<GREnemyPest>();
					if (component2 == null || component2.hp <= 0)
					{
						GREnemyRanged component3 = entity.GetComponent<GREnemyRanged>();
						if (component3 == null || component3.hp <= 0)
						{
							GREnemySummoner component4 = entity.GetComponent<GREnemySummoner>();
							return component4 != null && component4.hp > 0;
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0400681E RID: 26654
		public const float TIME_BETWEEN_HITS = 0.1f;

		// Token: 0x0400681F RID: 26655
		private readonly GRPlayer _grPlayer;

		// Token: 0x04006820 RID: 26656
		private GameEntity _enemy;

		// Token: 0x04006821 RID: 26657
		private float? _nextHitTime;
	}
}
