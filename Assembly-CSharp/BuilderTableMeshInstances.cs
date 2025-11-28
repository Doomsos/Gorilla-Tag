using System;
using Unity.Collections;
using UnityEngine.Jobs;

// Token: 0x02000590 RID: 1424
public struct BuilderTableMeshInstances
{
	// Token: 0x04002F1A RID: 12058
	public TransformAccessArray transforms;

	// Token: 0x04002F1B RID: 12059
	public NativeList<int> texIndex;

	// Token: 0x04002F1C RID: 12060
	public NativeList<float> tint;
}
