using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x02000592 RID: 1426
public class BuilderTableDataRenderIndirectBatch
{
	// Token: 0x04002F20 RID: 12064
	public int totalInstances;

	// Token: 0x04002F21 RID: 12065
	public TransformAccessArray instanceTransform;

	// Token: 0x04002F22 RID: 12066
	public NativeArray<int> instanceTransformIndexToDataIndex;

	// Token: 0x04002F23 RID: 12067
	public List<int> pieceIDPerTransform;

	// Token: 0x04002F24 RID: 12068
	public NativeArray<Matrix4x4> instanceObjectToWorld;

	// Token: 0x04002F25 RID: 12069
	public NativeArray<int> instanceTexIndex;

	// Token: 0x04002F26 RID: 12070
	public NativeArray<float> instanceTint;

	// Token: 0x04002F27 RID: 12071
	public NativeArray<int> instanceLodLevel;

	// Token: 0x04002F28 RID: 12072
	public NativeArray<int> instanceLodLevelDirty;

	// Token: 0x04002F29 RID: 12073
	public NativeList<BuilderTableMeshInstances> renderMeshes;

	// Token: 0x04002F2A RID: 12074
	public GraphicsBuffer commandBuf;

	// Token: 0x04002F2B RID: 12075
	public GraphicsBuffer matrixBuf;

	// Token: 0x04002F2C RID: 12076
	public GraphicsBuffer texIndexBuf;

	// Token: 0x04002F2D RID: 12077
	public GraphicsBuffer tintBuf;

	// Token: 0x04002F2E RID: 12078
	public NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs> commandData;

	// Token: 0x04002F2F RID: 12079
	public int commandCount;

	// Token: 0x04002F30 RID: 12080
	public RenderParams rp;
}
