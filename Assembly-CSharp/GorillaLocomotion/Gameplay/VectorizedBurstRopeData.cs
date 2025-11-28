using System;
using Unity.Collections;
using Unity.Mathematics;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000FAA RID: 4010
	public struct VectorizedBurstRopeData
	{
		// Token: 0x04007472 RID: 29810
		public NativeArray<float4> posX;

		// Token: 0x04007473 RID: 29811
		public NativeArray<float4> posY;

		// Token: 0x04007474 RID: 29812
		public NativeArray<float4> posZ;

		// Token: 0x04007475 RID: 29813
		public NativeArray<int4> validNodes;

		// Token: 0x04007476 RID: 29814
		public NativeArray<float4> lastPosX;

		// Token: 0x04007477 RID: 29815
		public NativeArray<float4> lastPosY;

		// Token: 0x04007478 RID: 29816
		public NativeArray<float4> lastPosZ;

		// Token: 0x04007479 RID: 29817
		public NativeArray<float3> ropeRoots;

		// Token: 0x0400747A RID: 29818
		public NativeArray<float4> nodeMass;
	}
}
