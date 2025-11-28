using System;
using UnityEngine;

namespace PerformanceSystems
{
	// Token: 0x02000D81 RID: 3457
	public abstract class ATimeSliceBehaviour : MonoBehaviour, ITimeSlice
	{
		// Token: 0x060054BC RID: 21692 RVA: 0x001AB80D File Offset: 0x001A9A0D
		protected void Awake()
		{
			this._timeSliceControllerAsset.AddTimeSliceBehaviour(this);
		}

		// Token: 0x060054BD RID: 21693 RVA: 0x001AB81B File Offset: 0x001A9A1B
		protected void OnDestroy()
		{
			this._timeSliceControllerAsset.RemoveTimeSliceBehaviour(this);
		}

		// Token: 0x060054BE RID: 21694 RVA: 0x001AB82C File Offset: 0x001A9A2C
		public void SliceUpdate()
		{
			float deltaTime = Time.realtimeSinceStartup - this._lastUpdateTime;
			this._lastUpdateTime = Time.realtimeSinceStartup;
			this.SliceUpdateAlways(deltaTime);
			if (this._updateIfDisabled || base.gameObject.activeSelf)
			{
				this.SliceUpdate(deltaTime);
			}
		}

		// Token: 0x060054BF RID: 21695
		public abstract void SliceUpdate(float deltaTime);

		// Token: 0x060054C0 RID: 21696
		public abstract void SliceUpdateAlways(float deltaTime);

		// Token: 0x040061D7 RID: 25047
		[SerializeField]
		protected TimeSliceControllerAsset _timeSliceControllerAsset;

		// Token: 0x040061D8 RID: 25048
		[SerializeField]
		protected bool _updateIfDisabled = true;

		// Token: 0x040061D9 RID: 25049
		protected float _lastUpdateTime;
	}
}
