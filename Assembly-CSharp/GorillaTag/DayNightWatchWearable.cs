using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000FF2 RID: 4082
	public class DayNightWatchWearable : MonoBehaviour
	{
		// Token: 0x06006720 RID: 26400 RVA: 0x00218984 File Offset: 0x00216B84
		private void Start()
		{
			if (!this.dayNightManager)
			{
				this.dayNightManager = BetterDayNightManager.instance;
			}
			this.rotationDegree = 0f;
			if (this.clockNeedle)
			{
				this.initialRotation = this.clockNeedle.localRotation;
			}
		}

		// Token: 0x06006721 RID: 26401 RVA: 0x002189D4 File Offset: 0x00216BD4
		private void Update()
		{
			this.currentTimeOfDay = this.dayNightManager.currentTimeOfDay;
			double currentTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).currentTimeInSeconds;
			double totalTimeInSeconds = ((ITimeOfDaySystem)this.dayNightManager).totalTimeInSeconds;
			this.rotationDegree = (float)(360.0 * currentTimeInSeconds / totalTimeInSeconds);
			this.rotationDegree = Mathf.Floor(this.rotationDegree);
			if (this.clockNeedle)
			{
				this.clockNeedle.localRotation = this.initialRotation * Quaternion.AngleAxis(this.rotationDegree, this.needleRotationAxis);
			}
		}

		// Token: 0x040075AA RID: 30122
		[Tooltip("The transform that will be rotated to indicate the current time.")]
		public Transform clockNeedle;

		// Token: 0x040075AB RID: 30123
		[FormerlySerializedAs("dialRotationAxis")]
		[Tooltip("The axis that the needle will rotate around.")]
		public Vector3 needleRotationAxis = Vector3.right;

		// Token: 0x040075AC RID: 30124
		private BetterDayNightManager dayNightManager;

		// Token: 0x040075AD RID: 30125
		[DebugOption]
		private float rotationDegree;

		// Token: 0x040075AE RID: 30126
		private string currentTimeOfDay;

		// Token: 0x040075AF RID: 30127
		private Quaternion initialRotation;
	}
}
