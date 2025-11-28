using System;
using UnityEngine;

// Token: 0x02000300 RID: 768
[Serializable]
public struct MaterialCombinerPerRendererInfo
{
	// Token: 0x04001754 RID: 5972
	public Renderer renderer;

	// Token: 0x04001755 RID: 5973
	public int slotIndex;

	// Token: 0x04001756 RID: 5974
	public int sliceIndex;

	// Token: 0x04001757 RID: 5975
	public Color baseColor;

	// Token: 0x04001758 RID: 5976
	public Material oldMat;

	// Token: 0x04001759 RID: 5977
	public bool wasMeshCombined;
}
