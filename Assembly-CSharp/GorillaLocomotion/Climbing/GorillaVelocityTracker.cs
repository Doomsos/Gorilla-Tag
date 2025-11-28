using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x02000FB0 RID: 4016
	public class GorillaVelocityTracker : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x1700098E RID: 2446
		// (get) Token: 0x060064DA RID: 25818 RVA: 0x0020F0C8 File Offset: 0x0020D2C8
		// (set) Token: 0x060064DB RID: 25819 RVA: 0x0020F0D0 File Offset: 0x0020D2D0
		public bool TickRunning { get; set; }

		// Token: 0x060064DC RID: 25820 RVA: 0x0020F0DC File Offset: 0x0020D2DC
		public void ResetState()
		{
			this.trans = base.transform;
			this.localSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|20_0(this.localSpaceData);
			this.worldSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|20_0(this.worldSpaceData);
			this.isRelativeTo = (this.relativeTo != null);
			this.lastLocalSpacePos = this.GetPosition(false);
			this.lastWorldSpacePos = this.GetPosition(true);
			this.wasAboveThreshold = false;
		}

		// Token: 0x060064DD RID: 25821 RVA: 0x0020F162 File Offset: 0x0020D362
		private void Awake()
		{
			this.ResetState();
		}

		// Token: 0x060064DE RID: 25822 RVA: 0x0001877F File Offset: 0x0001697F
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x060064DF RID: 25823 RVA: 0x0020F16A File Offset: 0x0020D36A
		private void OnDisable()
		{
			this.ResetState();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x060064E0 RID: 25824 RVA: 0x0020F178 File Offset: 0x0020D378
		public void SetRelativeTo(Transform tf)
		{
			this.relativeTo = tf;
			this.isRelativeTo = (tf != null);
		}

		// Token: 0x060064E1 RID: 25825 RVA: 0x0020F18E File Offset: 0x0020D38E
		private Vector3 GetPosition(bool worldSpace)
		{
			if (worldSpace)
			{
				return this.trans.position;
			}
			if (this.isRelativeTo)
			{
				return this.relativeTo.InverseTransformPoint(this.trans.position);
			}
			return this.trans.localPosition;
		}

		// Token: 0x060064E2 RID: 25826 RVA: 0x0020F1CC File Offset: 0x0020D3CC
		public void Tick()
		{
			if (Time.frameCount <= this.lastTickedFrame)
			{
				return;
			}
			Vector3 position = this.GetPosition(false);
			Vector3 position2 = this.GetPosition(true);
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = this.localSpaceData[this.currentDataPointIndex];
			velocityDataPoint.delta = (position - this.lastLocalSpacePos) / Time.deltaTime;
			velocityDataPoint.time = Time.time;
			this.localSpaceData[this.currentDataPointIndex] = velocityDataPoint;
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint2 = this.worldSpaceData[this.currentDataPointIndex];
			velocityDataPoint2.delta = (position2 - this.lastWorldSpacePos) / Time.deltaTime;
			velocityDataPoint2.time = Time.time;
			this.worldSpaceData[this.currentDataPointIndex] = velocityDataPoint2;
			this.lastLocalSpacePos = position;
			this.lastWorldSpacePos = position2;
			this.currentDataPointIndex++;
			if (this.currentDataPointIndex >= this.maxDataPoints)
			{
				this.currentDataPointIndex = 0;
			}
			if (this.useVelocityEvents)
			{
				this.GetLatestVelocity(this.useWorldSpaceForEvents);
			}
			this.lastTickedFrame = Time.frameCount;
		}

		// Token: 0x060064E3 RID: 25827 RVA: 0x0020F2CE File Offset: 0x0020D4CE
		private void AddToQueue(ref List<GorillaVelocityTracker.VelocityDataPoint> dataPoints, GorillaVelocityTracker.VelocityDataPoint newData)
		{
			dataPoints.Add(newData);
			if (dataPoints.Count >= this.maxDataPoints)
			{
				dataPoints.RemoveAt(0);
			}
		}

		// Token: 0x060064E4 RID: 25828 RVA: 0x0020F2F0 File Offset: 0x0020D4F0
		public Vector3 GetAverageVelocity(bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
		{
			float num = maxTimeFromPast / 2f;
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return Vector3.zero;
			}
			GorillaVelocityTracker.<>c__DisplayClass28_0 CS$<>8__locals1;
			CS$<>8__locals1.total = Vector3.zero;
			CS$<>8__locals1.totalMag = 0f;
			CS$<>8__locals1.added = 0;
			float num2 = Time.time - maxTimeFromPast;
			float num3 = Time.time - num;
			int i = 0;
			int num4 = this.currentDataPointIndex;
			while (i < this.maxDataPoints)
			{
				GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = array[num4];
				if (doMagnitudeCheck && CS$<>8__locals1.added > 1 && velocityDataPoint.time >= num3)
				{
					if (velocityDataPoint.delta.magnitude >= CS$<>8__locals1.totalMag / (float)CS$<>8__locals1.added)
					{
						GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|28_0(velocityDataPoint, ref CS$<>8__locals1);
					}
				}
				else if (velocityDataPoint.time >= num2)
				{
					GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|28_0(velocityDataPoint, ref CS$<>8__locals1);
				}
				num4++;
				if (num4 >= this.maxDataPoints)
				{
					num4 = 0;
				}
				i++;
			}
			if (CS$<>8__locals1.added > 0)
			{
				return CS$<>8__locals1.total / (float)CS$<>8__locals1.added;
			}
			return Vector3.zero;
		}

		// Token: 0x060064E5 RID: 25829 RVA: 0x0020F400 File Offset: 0x0020D600
		public Vector3 GetLatestVelocity(bool worldSpace = false)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array[this.currentDataPointIndex].delta.magnitude >= this.latestVelocityThreshold && !this.wasAboveThreshold)
			{
				UnityEvent onLatestAboveThreshold = this.OnLatestAboveThreshold;
				if (onLatestAboveThreshold != null)
				{
					onLatestAboveThreshold.Invoke();
				}
				this.wasAboveThreshold = true;
			}
			else if (array[this.currentDataPointIndex].delta.magnitude < this.latestVelocityThreshold && this.wasAboveThreshold)
			{
				UnityEvent onLatestBelowThreshold = this.OnLatestBelowThreshold;
				if (onLatestBelowThreshold != null)
				{
					onLatestBelowThreshold.Invoke();
				}
				this.wasAboveThreshold = false;
			}
			return array[this.currentDataPointIndex].delta;
		}

		// Token: 0x060064E6 RID: 25830 RVA: 0x0020F4A4 File Offset: 0x0020D6A4
		public float GetAverageSpeedChangeMagnitudeInDirection(Vector3 dir, bool worldSpace = false, float maxTimeFromPast = 0.05f)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return 0f;
			}
			float num = 0f;
			int num2 = 0;
			float num3 = Time.time - maxTimeFromPast;
			bool flag = false;
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].time >= num3)
				{
					if (!flag)
					{
						vector = array[i].delta;
						flag = true;
					}
					else
					{
						num += Mathf.Abs(Vector3.Dot(array[i].delta - vector, dir));
						num2++;
					}
				}
			}
			if (num2 <= 0)
			{
				return 0f;
			}
			return num / (float)num2;
		}

		// Token: 0x060064E8 RID: 25832 RVA: 0x0020F564 File Offset: 0x0020D764
		[CompilerGenerated]
		private void <ResetState>g__PopulateArray|20_0(GorillaVelocityTracker.VelocityDataPoint[] array)
		{
			for (int i = 0; i < this.maxDataPoints; i++)
			{
				array[i] = new GorillaVelocityTracker.VelocityDataPoint();
			}
		}

		// Token: 0x060064E9 RID: 25833 RVA: 0x0020F58C File Offset: 0x0020D78C
		[CompilerGenerated]
		internal static void <GetAverageVelocity>g__AddPoint|28_0(GorillaVelocityTracker.VelocityDataPoint point, ref GorillaVelocityTracker.<>c__DisplayClass28_0 A_1)
		{
			A_1.total += point.delta;
			A_1.totalMag += point.delta.magnitude;
			int added = A_1.added;
			A_1.added = added + 1;
		}

		// Token: 0x0400749E RID: 29854
		[SerializeField]
		private int maxDataPoints = 20;

		// Token: 0x0400749F RID: 29855
		[SerializeField]
		private Transform relativeTo;

		// Token: 0x040074A0 RID: 29856
		[Tooltip("Use in Editor to trigger events when above or higher than a desired latest velocity.")]
		[SerializeField]
		private bool useVelocityEvents;

		// Token: 0x040074A1 RID: 29857
		[SerializeField]
		private float latestVelocityThreshold;

		// Token: 0x040074A2 RID: 29858
		public UnityEvent OnLatestBelowThreshold;

		// Token: 0x040074A3 RID: 29859
		public UnityEvent OnLatestAboveThreshold;

		// Token: 0x040074A4 RID: 29860
		[SerializeField]
		private bool useWorldSpaceForEvents;

		// Token: 0x040074A5 RID: 29861
		private bool wasAboveThreshold;

		// Token: 0x040074A6 RID: 29862
		private int currentDataPointIndex;

		// Token: 0x040074A7 RID: 29863
		private GorillaVelocityTracker.VelocityDataPoint[] localSpaceData;

		// Token: 0x040074A8 RID: 29864
		private GorillaVelocityTracker.VelocityDataPoint[] worldSpaceData;

		// Token: 0x040074A9 RID: 29865
		private Transform trans;

		// Token: 0x040074AA RID: 29866
		private Vector3 lastWorldSpacePos;

		// Token: 0x040074AB RID: 29867
		private Vector3 lastLocalSpacePos;

		// Token: 0x040074AC RID: 29868
		private bool isRelativeTo;

		// Token: 0x040074AD RID: 29869
		private int lastTickedFrame = -1;

		// Token: 0x02000FB1 RID: 4017
		public class VelocityDataPoint
		{
			// Token: 0x040074AF RID: 29871
			public Vector3 delta;

			// Token: 0x040074B0 RID: 29872
			public float time = -1f;
		}
	}
}
