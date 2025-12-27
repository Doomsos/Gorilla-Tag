using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public static class GTVertexDataStreams_Descriptors
{
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

	public static readonly VertexAttributeDescriptor position = new VertexAttributeDescriptor(0, 0, 3, 0);

	public static readonly VertexAttributeDescriptor color = new VertexAttributeDescriptor(3, 2, 4, 0);

	public static readonly VertexAttributeDescriptor uv1 = new VertexAttributeDescriptor(4, 1, 4, 0);

	public static readonly VertexAttributeDescriptor lightmapUv = new VertexAttributeDescriptor(5, 1, 2, 0);

	public static readonly VertexAttributeDescriptor normal = new VertexAttributeDescriptor(1, 0, 3, 1);

	public static readonly VertexAttributeDescriptor tangent = new VertexAttributeDescriptor(2, 3, 4, 1);
}
