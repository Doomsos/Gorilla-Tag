using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020008DE RID: 2270
public class ScienceExperimentSceneElements : MonoBehaviour
{
	// Token: 0x06003A2E RID: 14894 RVA: 0x001338FB File Offset: 0x00131AFB
	private void Awake()
	{
		ScienceExperimentManager.instance.InitElements(this);
	}

	// Token: 0x06003A2F RID: 14895 RVA: 0x0013390A File Offset: 0x00131B0A
	private void OnDestroy()
	{
		ScienceExperimentManager.instance.DeInitElements();
	}

	// Token: 0x0400496E RID: 18798
	public List<ScienceExperimentSceneElements.DisableByLiquidData> disableByLiquidList = new List<ScienceExperimentSceneElements.DisableByLiquidData>();

	// Token: 0x0400496F RID: 18799
	public ParticleSystem sodaFizzParticles;

	// Token: 0x04004970 RID: 18800
	public ParticleSystem sodaEruptionParticles;

	// Token: 0x020008DF RID: 2271
	[Serializable]
	public struct DisableByLiquidData
	{
		// Token: 0x04004971 RID: 18801
		public Transform target;

		// Token: 0x04004972 RID: 18802
		public float heightOffset;
	}
}
