using System;
using UnityEngine;
using UnityEngine.Events;

namespace PerformanceSystems
{
	// Token: 0x02000D87 RID: 3463
	public class TimeSliceLodBehaviour : ATimeSliceBehaviour, ILod
	{
		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x060054DC RID: 21724 RVA: 0x001ABC0E File Offset: 0x001A9E0E
		public Vector3 Position
		{
			get
			{
				return this._transform.position;
			}
		}

		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x060054DD RID: 21725 RVA: 0x001ABC1B File Offset: 0x001A9E1B
		public float[] LodRanges
		{
			get
			{
				return this._lodRanges;
			}
		}

		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x060054DE RID: 21726 RVA: 0x001ABC23 File Offset: 0x001A9E23
		public UnityEvent[] OnLodRangeEvents
		{
			get
			{
				return this._onLodRangeEvents;
			}
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x060054DF RID: 21727 RVA: 0x001ABC2B File Offset: 0x001A9E2B
		public UnityEvent OnCulledEvent
		{
			get
			{
				return this._onCulledEvent;
			}
		}

		// Token: 0x17000810 RID: 2064
		// (get) Token: 0x060054E0 RID: 21728 RVA: 0x001ABC33 File Offset: 0x001A9E33
		public int CurrentLod
		{
			get
			{
				return this._currentLod;
			}
		}

		// Token: 0x060054E1 RID: 21729 RVA: 0x001ABC3B File Offset: 0x001A9E3B
		protected void Start()
		{
			this._updateIfDisabled = true;
			this._transform = base.transform;
		}

		// Token: 0x060054E2 RID: 21730 RVA: 0x001ABC50 File Offset: 0x001A9E50
		protected void SetLod(int newLod)
		{
			if (newLod == this._currentLod)
			{
				return;
			}
			this._currentLod = newLod;
			if (newLod < this._onLodRangeEvents.Length)
			{
				this._onLodRangeEvents[newLod].Invoke();
				return;
			}
			if (newLod == this._onLodRangeEvents.Length)
			{
				this._onCulledEvent.Invoke();
				return;
			}
			Debug.LogWarning(string.Format("No event for LOD [{0}]", newLod), this);
		}

		// Token: 0x060054E3 RID: 21731 RVA: 0x001ABCB4 File Offset: 0x001A9EB4
		public void UpdateLod(Vector3 refPos)
		{
			Vector3 position = this._transform.position;
			float num = Vector3.Distance(refPos, position);
			for (int i = 0; i < this._lodRanges.Length; i++)
			{
				float num2 = this._lodRanges[i];
				if (num <= num2)
				{
					this.SetLod(i);
					return;
				}
			}
			this.SetLod(this._lodRanges.Length);
		}

		// Token: 0x060054E4 RID: 21732 RVA: 0x00002789 File Offset: 0x00000989
		public override void SliceUpdate(float deltaTime)
		{
		}

		// Token: 0x060054E5 RID: 21733 RVA: 0x001ABD0B File Offset: 0x001A9F0B
		public override void SliceUpdateAlways(float deltaTime)
		{
			this.UpdateLod(this._timeSliceControllerAsset.ReferenceTransform.position);
		}

		// Token: 0x040061E3 RID: 25059
		[Space]
		[SerializeField]
		protected int _currentLod = -1;

		// Token: 0x040061E4 RID: 25060
		[SerializeField]
		protected float[] _lodRanges;

		// Token: 0x040061E5 RID: 25061
		[Space]
		[SerializeField]
		protected UnityEvent[] _onLodRangeEvents;

		// Token: 0x040061E6 RID: 25062
		[Space]
		[SerializeField]
		protected UnityEvent _onCulledEvent;

		// Token: 0x040061E7 RID: 25063
		protected Transform _transform;
	}
}
