using System;
using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;

namespace GorillaTag.Dev.Benchmarks
{
	// Token: 0x0200104E RID: 4174
	public class VisualBenchmark : MonoBehaviour
	{
		// Token: 0x06006936 RID: 26934 RVA: 0x002239CC File Offset: 0x00221BCC
		protected void Awake()
		{
			Application.quitting += delegate()
			{
				VisualBenchmark.isQuitting = true;
			};
			List<ProfilerRecorderHandle> list = new List<ProfilerRecorderHandle>(5500);
			ProfilerRecorderHandle.GetAvailable(list);
			Debug.Log(string.Format("poop Available stats: {0}", list.Count), this);
			List<VisualBenchmark.StatInfo> list2 = new List<VisualBenchmark.StatInfo>(600);
			foreach (ProfilerRecorderHandle profilerRecorderHandle in list)
			{
				ProfilerRecorderDescription description = ProfilerRecorderHandle.GetDescription(profilerRecorderHandle);
				if (description.Category == ProfilerCategory.Render)
				{
					list2.Add(new VisualBenchmark.StatInfo
					{
						name = description.Name,
						unit = description.UnitType
					});
				}
			}
			this.availableRenderStats = list2.ToArray();
			Debug.Log(string.Format("poop availableRenderStats: {0}", list2.Count), this);
			List<Transform> list3 = new List<Transform>(this.benchmarkLocations.Length);
			foreach (Transform transform in this.benchmarkLocations)
			{
				if (transform != null)
				{
					list3.Add(transform);
				}
			}
			this.benchmarkLocations = list3.ToArray();
		}

		// Token: 0x06006937 RID: 26935 RVA: 0x00223B30 File Offset: 0x00221D30
		protected void OnEnable()
		{
			this.renderStatsRecorders = new ProfilerRecorder[this.availableRenderStats.Length];
			for (int i = 0; i < this.availableRenderStats.Length; i++)
			{
				this.renderStatsRecorders[i] = ProfilerRecorder.StartNew(ProfilerCategory.Render, this.availableRenderStats[i].name, 1, 24);
			}
			this.state = VisualBenchmark.EState.Setup;
		}

		// Token: 0x06006938 RID: 26936 RVA: 0x00223B94 File Offset: 0x00221D94
		protected void OnDisable()
		{
			foreach (ProfilerRecorder profilerRecorder in this.renderStatsRecorders)
			{
				profilerRecorder.Dispose();
			}
		}

		// Token: 0x06006939 RID: 26937 RVA: 0x00223BC8 File Offset: 0x00221DC8
		protected void LateUpdate()
		{
			if (VisualBenchmark.isQuitting)
			{
				return;
			}
			switch (this.state)
			{
			case VisualBenchmark.EState.Setup:
				Debug.Log("poop start");
				this.sb.Clear();
				this.currentLocationIndex = 0;
				this.lastTime = Time.realtimeSinceStartup;
				this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
				return;
			case VisualBenchmark.EState.WaitingBeforeCollectingGarbage:
				Debug.Log("poop wait 1");
				if (Time.realtimeSinceStartup - this.lastTime >= this.collectGarbageDelay)
				{
					this.lastTime = Time.time;
					GC.Collect();
					this.state = VisualBenchmark.EState.WaitingBeforeRecordingStats;
					return;
				}
				break;
			case VisualBenchmark.EState.WaitingBeforeRecordingStats:
				Debug.Log("poop wait 2");
				if (Time.time - this.lastTime >= this.recordStatsDelay)
				{
					this.lastTime = Time.time;
					this.RecordLocationStats(this.benchmarkLocations[this.currentLocationIndex]);
					if (this.currentLocationIndex < this.benchmarkLocations.Length - 1)
					{
						this.currentLocationIndex++;
						this.state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;
						return;
					}
					this.state = VisualBenchmark.EState.TearDown;
					return;
				}
				break;
			case VisualBenchmark.EState.TearDown:
				Debug.Log("poop teardown");
				Debug.Log(this.sb.ToString());
				this.state = VisualBenchmark.EState.Setup;
				if (this.sb.Length > this.sb.Capacity)
				{
					Debug.Log("Capacity exceeded on string builder, increase string builder's capacity. " + string.Format("capacity={0}, length={1}", this.sb.Capacity, this.sb.Length), this);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x0600693A RID: 26938 RVA: 0x00223D48 File Offset: 0x00221F48
		private void RecordLocationStats(Transform xform)
		{
			this.sb.Append("Location: ");
			this.sb.Append(xform.name);
			this.sb.Append("\n");
			this.sb.Append("pos=");
			this.sb.Append(xform.position.ToString("F3"));
			this.sb.Append(" rot=");
			this.sb.Append(xform.rotation.ToString("F3"));
			this.sb.Append(" scale=");
			this.sb.Append(xform.lossyScale.ToString("F3"));
			this.sb.Append("\n");
			for (int i = 0; i < this.renderStatsRecorders.Length; i++)
			{
				this.sb.Append(this.availableRenderStats[i].name);
				this.sb.Append(": ");
				ProfilerMarkerDataUnit unit = this.availableRenderStats[i].unit;
				if (unit != 1)
				{
					if (unit == 2)
					{
						this.sb.Append((double)this.renderStatsRecorders[i].LastValue / 1024.0);
						this.sb.Append("kb");
					}
					else
					{
						this.sb.Append(this.renderStatsRecorders[i].LastValue);
						this.sb.Append(' ');
						this.sb.Append(this.availableRenderStats[i].unit.ToString());
					}
				}
				else
				{
					this.sb.Append((double)this.renderStatsRecorders[i].LastValue / 1000000.0);
					this.sb.Append("ms");
				}
				this.sb.Append('\n');
			}
		}

		// Token: 0x040077DF RID: 30687
		[Tooltip("the camera will be moved and rotated to these spots and record stats.")]
		public Transform[] benchmarkLocations;

		// Token: 0x040077E0 RID: 30688
		[Tooltip("How long to wait before calling GC.Collect() to clean up memory.")]
		public float collectGarbageDelay = 2f;

		// Token: 0x040077E1 RID: 30689
		[Tooltip("How long to wait before recording stats after the camera was moved to a new location.\nThis + collectGarbageDelay is the total time spent at each location.")]
		private float recordStatsDelay = 2f;

		// Token: 0x040077E2 RID: 30690
		[Tooltip("The camera to use for profiling. If null, a new camera will be created.")]
		private Camera cam;

		// Token: 0x040077E3 RID: 30691
		private VisualBenchmark.StatInfo[] availableRenderStats;

		// Token: 0x040077E4 RID: 30692
		private ProfilerRecorder[] renderStatsRecorders;

		// Token: 0x040077E5 RID: 30693
		private static bool isQuitting = true;

		// Token: 0x040077E6 RID: 30694
		private int currentLocationIndex;

		// Token: 0x040077E7 RID: 30695
		private VisualBenchmark.EState state = VisualBenchmark.EState.WaitingBeforeCollectingGarbage;

		// Token: 0x040077E8 RID: 30696
		private float lastTime;

		// Token: 0x040077E9 RID: 30697
		private readonly StringBuilder sb = new StringBuilder(1024);

		// Token: 0x0200104F RID: 4175
		private struct StatInfo
		{
			// Token: 0x040077EA RID: 30698
			public string name;

			// Token: 0x040077EB RID: 30699
			public ProfilerMarkerDataUnit unit;
		}

		// Token: 0x02001050 RID: 4176
		private enum EState
		{
			// Token: 0x040077ED RID: 30701
			Setup,
			// Token: 0x040077EE RID: 30702
			WaitingBeforeCollectingGarbage,
			// Token: 0x040077EF RID: 30703
			WaitingBeforeRecordingStats,
			// Token: 0x040077F0 RID: 30704
			TearDown
		}
	}
}
