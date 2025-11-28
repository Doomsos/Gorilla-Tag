using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000308 RID: 776
public static class GTVertexDataStreams_Descriptors
{
	// Token: 0x060012DB RID: 4827 RVA: 0x0006AAB0 File Offset: 0x00068CB0
	public static void DoSetVertexBufferParams(ref Mesh.MeshData writeData, int totalVertexCount)
	{
		NativeArray<VertexAttributeDescriptor> nativeArray = new NativeArray<VertexAttributeDescriptor>(6, 2, 0);
		int num = 0;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.position;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.color;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.uv1;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.lightmapUv;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.normal;
		nativeArray[num++] = GTVertexDataStreams_Descriptors.tangent;
		writeData.SetVertexBufferParams(totalVertexCount, nativeArray);
		nativeArray.Dispose();
	}

	// Token: 0x04001900 RID: 6400
	public static readonly VertexAttributeDescriptor position = new VertexAttributeDescriptor(0, 0, 3, 0);

	// Token: 0x04001901 RID: 6401
	public static readonly VertexAttributeDescriptor color = new VertexAttributeDescriptor(3, 2, 4, 0);

	// Token: 0x04001902 RID: 6402
	public static readonly VertexAttributeDescriptor uv1 = new VertexAttributeDescriptor(4, 1, 4, 0);

	// Token: 0x04001903 RID: 6403
	public static readonly VertexAttributeDescriptor lightmapUv = new VertexAttributeDescriptor(5, 1, 2, 0);

	// Token: 0x04001904 RID: 6404
	public static readonly VertexAttributeDescriptor normal = new VertexAttributeDescriptor(1, 0, 3, 1);

	// Token: 0x04001905 RID: 6405
	public static readonly VertexAttributeDescriptor tangent = new VertexAttributeDescriptor(2, 3, 4, 1);
}
