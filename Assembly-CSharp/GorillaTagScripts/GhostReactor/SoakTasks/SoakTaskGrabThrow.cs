using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02000E42 RID: 3650
	public sealed class SoakTaskGrabThrow : IGhostReactorSoakTask
	{
		// Token: 0x1700087D RID: 2173
		// (get) Token: 0x06005AF2 RID: 23282 RVA: 0x001D204C File Offset: 0x001D024C
		// (set) Token: 0x06005AF3 RID: 23283 RVA: 0x001D2054 File Offset: 0x001D0254
		public bool Complete { get; private set; }

		// Token: 0x06005AF4 RID: 23284 RVA: 0x001D205D File Offset: 0x001D025D
		public SoakTaskGrabThrow(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x06005AF5 RID: 23285 RVA: 0x001D206C File Offset: 0x001D026C
		public bool Update()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._dropEntityTime == null || this._heldEntityId == null)
			{
				List<GameEntity> list = managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>();
				GameEntity gameEntity = null;
				foreach (GameEntity gameEntity2 in list)
				{
					if (!(gameEntity2 == null) && !gameEntity2.IsHeld() && gameEntity2.pickupable && !(gameEntity2.gameObject.GetComponent<GameAgent>() != null))
					{
						gameEntity = gameEntity2;
						break;
					}
				}
				if (gameEntity != null)
				{
					Debug.Log(string.Format("Soak grabbing entity {0}", gameEntity.id.index));
					managerForZone.RequestGrabEntity(gameEntity.id, true, Vector3.zero, Quaternion.identity);
					this._heldEntityId = new GameEntityId?(gameEntity.id);
					this._dropEntityTime = new float?(Time.time + 0.1f);
				}
			}
			else if (this._heldEntityId != null)
			{
				float? dropEntityTime = this._dropEntityTime;
				if (dropEntityTime != null)
				{
					float valueOrDefault = dropEntityTime.GetValueOrDefault();
					if (Time.time >= valueOrDefault)
					{
						Debug.Log(string.Format("Soak dropping entity {0}", this._heldEntityId.Value.index));
						managerForZone.RequestThrowEntity(this._heldEntityId.Value, true, Vector3.zero, Vector3.zero, Vector3.zero);
						this._heldEntityId = default(GameEntityId?);
						this._dropEntityTime = default(float?);
						this.Complete = true;
					}
				}
			}
			return true;
		}

		// Token: 0x06005AF6 RID: 23286 RVA: 0x001D2240 File Offset: 0x001D0440
		public void Reset()
		{
			this._heldEntityId = default(GameEntityId?);
			this._dropEntityTime = default(float?);
			this.Complete = false;
		}

		// Token: 0x04006819 RID: 26649
		public const float TIME_TO_HOLD_ENTITY = 0.1f;

		// Token: 0x0400681A RID: 26650
		private readonly GRPlayer _grPlayer;

		// Token: 0x0400681B RID: 26651
		private GameEntityId? _heldEntityId;

		// Token: 0x0400681C RID: 26652
		private float? _dropEntityTime;
	}
}
