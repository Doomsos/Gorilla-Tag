using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000345 RID: 837
[GTStripGameObjectFromBuild("!GT_AUTOMATED_PERF_TEST")]
public class PerfTestFPSCaptureController : MonoBehaviour
{
	// Token: 0x04001EB1 RID: 7857
	[SerializeField]
	private SerializablePerformanceReport<ScenePerformanceData> performanceSummary;
}
