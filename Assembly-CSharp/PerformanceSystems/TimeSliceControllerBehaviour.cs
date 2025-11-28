using System;
using UnityEngine;

namespace PerformanceSystems
{
	// Token: 0x02000D86 RID: 3462
	public class TimeSliceControllerBehaviour : MonoBehaviour
	{
		// Token: 0x060054D9 RID: 21721 RVA: 0x001ABBD4 File Offset: 0x001A9DD4
		private void Awake()
		{
			this._timeSliceControllerAsset.InitializeReferenceTransformWithMainCam();
		}

		// Token: 0x060054DA RID: 21722 RVA: 0x001ABBE1 File Offset: 0x001A9DE1
		private void Update()
		{
			this._timeSliceControllerAsset.Update();
		}

		// Token: 0x040061E2 RID: 25058
		[SerializeField]
		private TimeSliceControllerAsset _timeSliceControllerAsset;
	}
}
