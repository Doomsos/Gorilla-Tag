using System;
using System.Collections.Generic;
using UnityEngine;

namespace PerformanceSystems
{
	// Token: 0x02000D85 RID: 3461
	[CreateAssetMenu(menuName = "PerformanceTools/TimeSlicer/TimeSliceController", fileName = "TimeSliceController")]
	public class TimeSliceControllerAsset : ScriptableObject
	{
		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x060054CD RID: 21709 RVA: 0x001AB925 File Offset: 0x001A9B25
		public Transform ReferenceTransform
		{
			get
			{
				return this._referenceTransform;
			}
		}

		// Token: 0x060054CE RID: 21710 RVA: 0x001AB92D File Offset: 0x001A9B2D
		private void RemovePendingObjects()
		{
			this._currentTimeSliceBehaviours.FastRemove(this._timeSliceBehavioursToRemove);
			this._timeSliceBehavioursToRemove.Clear();
		}

		// Token: 0x060054CF RID: 21711 RVA: 0x001AB94C File Offset: 0x001A9B4C
		private void AddPendingObjects()
		{
			foreach (ITimeSlice timeSlice in this._timeSliceBehavioursToAdd)
			{
				if (!this._currentTimeSliceBehaviours.Contains(timeSlice))
				{
					this._currentTimeSliceBehaviours.Add(timeSlice);
				}
			}
			this._timeSliceBehavioursToAdd.Clear();
		}

		// Token: 0x060054D0 RID: 21712 RVA: 0x001AB9C0 File Offset: 0x001A9BC0
		private void UpdateCurrentSliceObjects()
		{
			int count = this._currentTimeSliceBehaviours.Count;
			if (count == 0)
			{
				return;
			}
			int num = Mathf.Max(1, this._timeSlices);
			this._sliceSize = Mathf.CeilToInt((float)count / (float)num);
			if (this._sliceSize <= 0)
			{
				this._sliceSize = 1;
			}
			int num2 = this._sliceSize * this._currentSlice;
			if (num2 >= count)
			{
				num2 = Mathf.Max(0, count - this._sliceSize);
			}
			int num3 = Mathf.Min(this._sliceSize, count - num2);
			if (num3 <= 0)
			{
				return;
			}
			for (int i = 0; i < num3; i++)
			{
				int num4 = num2 + i;
				if (num4 < 0 || num4 >= this._currentTimeSliceBehaviours.Count)
				{
					break;
				}
				ITimeSlice timeSlice = this._currentTimeSliceBehaviours[num4];
				if (timeSlice != null)
				{
					timeSlice.SliceUpdate();
				}
			}
		}

		// Token: 0x060054D1 RID: 21713 RVA: 0x001ABA81 File Offset: 0x001A9C81
		public void SetRefTransform(Transform refTransform)
		{
			this._referenceTransform = refTransform;
			this._isActive = (this._referenceTransform != null);
		}

		// Token: 0x060054D2 RID: 21714 RVA: 0x001ABA9C File Offset: 0x001A9C9C
		public void AddTimeSliceBehaviour(ITimeSlice timeSlice)
		{
			if (this._currentTimeSliceBehaviours.Contains(timeSlice))
			{
				return;
			}
			this._timeSliceBehavioursToAdd.Add(timeSlice);
		}

		// Token: 0x060054D3 RID: 21715 RVA: 0x001ABABA File Offset: 0x001A9CBA
		public void RemoveTimeSliceBehaviour(ITimeSlice timeSlice)
		{
			if (!this._currentTimeSliceBehaviours.Contains(timeSlice))
			{
				this._timeSliceBehavioursToRemove.Remove(timeSlice);
				return;
			}
			this._timeSliceBehavioursToRemove.Add(timeSlice);
		}

		// Token: 0x060054D4 RID: 21716 RVA: 0x001ABAE8 File Offset: 0x001A9CE8
		public void Update()
		{
			this.InitializeReferenceTransformWithMainCam();
			if (!this._isActive)
			{
				return;
			}
			if (this._currentSlice == 0)
			{
				this.RemovePendingObjects();
				this.AddPendingObjects();
			}
			this.UpdateCurrentSliceObjects();
			this._currentSlice = (this._currentSlice + 1) % Mathf.Max(1, this._timeSlices);
		}

		// Token: 0x060054D5 RID: 21717 RVA: 0x001ABB39 File Offset: 0x001A9D39
		public void InitializeReferenceTransformWithMainCam()
		{
			if (this._referenceTransform == null)
			{
				Camera main = Camera.main;
				this._referenceTransform = ((main != null) ? main.transform : null);
			}
			this._isActive = (this._referenceTransform != null);
		}

		// Token: 0x060054D6 RID: 21718 RVA: 0x001ABB72 File Offset: 0x001A9D72
		private void OnDisable()
		{
			this.ClearAsset();
		}

		// Token: 0x060054D7 RID: 21719 RVA: 0x001ABB7A File Offset: 0x001A9D7A
		public void ClearAsset()
		{
			this._currentTimeSliceBehaviours.Clear();
			this._timeSliceBehavioursToAdd.Clear();
			this._timeSliceBehavioursToRemove.Clear();
			this._referenceTransform = null;
		}

		// Token: 0x040061DA RID: 25050
		private readonly List<ITimeSlice> _currentTimeSliceBehaviours = new List<ITimeSlice>();

		// Token: 0x040061DB RID: 25051
		private readonly HashSet<ITimeSlice> _timeSliceBehavioursToAdd = new HashSet<ITimeSlice>();

		// Token: 0x040061DC RID: 25052
		private readonly HashSet<ITimeSlice> _timeSliceBehavioursToRemove = new HashSet<ITimeSlice>();

		// Token: 0x040061DD RID: 25053
		private Transform _referenceTransform;

		// Token: 0x040061DE RID: 25054
		[Range(1f, 150f)]
		[SerializeField]
		private int _timeSlices = 1;

		// Token: 0x040061DF RID: 25055
		private int _currentSlice;

		// Token: 0x040061E0 RID: 25056
		private bool _isActive;

		// Token: 0x040061E1 RID: 25057
		private int _sliceSize;
	}
}
