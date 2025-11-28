using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000CD5 RID: 3285
public class GTMeshData
{
	// Token: 0x06005026 RID: 20518 RVA: 0x0019BF14 File Offset: 0x0019A114
	public GTMeshData(Mesh m)
	{
		this.mesh = m;
		this.subMeshCount = m.subMeshCount;
		this.vertices = m.vertices;
		this.triangles = m.triangles;
		this.normals = m.normals;
		this.tangents = m.tangents;
		this.colors32 = m.colors32;
		this.boneWeights = m.boneWeights;
		this.uv = m.uv;
		this.uv2 = m.uv2;
		this.uv3 = m.uv3;
		this.uv4 = m.uv4;
		this.uv5 = m.uv5;
		this.uv6 = m.uv6;
		this.uv7 = m.uv7;
		this.uv8 = m.uv8;
	}

	// Token: 0x06005027 RID: 20519 RVA: 0x0019BFE4 File Offset: 0x0019A1E4
	public Mesh ExtractSubmesh(int subMeshIndex, bool optimize = false)
	{
		if (subMeshIndex < 0 || subMeshIndex >= this.subMeshCount)
		{
			throw new IndexOutOfRangeException("subMeshIndex");
		}
		SubMeshDescriptor subMesh = this.mesh.GetSubMesh(subMeshIndex);
		int firstVertex = subMesh.firstVertex;
		int vertexCount = subMesh.vertexCount;
		MeshTopology topology = subMesh.topology;
		int[] indices = this.mesh.GetIndices(subMeshIndex, false);
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] -= firstVertex;
		}
		Mesh mesh = new Mesh();
		mesh.indexFormat = ((vertexCount > 65535) ? 1 : 0);
		mesh.SetVertices(this.vertices, firstVertex, vertexCount);
		mesh.SetIndices(indices, topology, 0);
		mesh.SetNormals(this.normals, firstVertex, vertexCount);
		mesh.SetTangents(this.tangents, firstVertex, vertexCount);
		if (!this.uv.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(0, this.uv, firstVertex, vertexCount);
		}
		if (!this.uv2.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(1, this.uv2, firstVertex, vertexCount);
		}
		if (!this.uv3.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(2, this.uv3, firstVertex, vertexCount);
		}
		if (!this.uv4.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(3, this.uv4, firstVertex, vertexCount);
		}
		if (!this.uv5.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(4, this.uv5, firstVertex, vertexCount);
		}
		if (!this.uv6.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(5, this.uv6, firstVertex, vertexCount);
		}
		if (!this.uv7.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(6, this.uv7, firstVertex, vertexCount);
		}
		if (!this.uv8.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(7, this.uv8, firstVertex, vertexCount);
		}
		if (optimize)
		{
			mesh.Optimize();
			mesh.OptimizeIndexBuffers();
		}
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x06005028 RID: 20520 RVA: 0x0019C1B2 File Offset: 0x0019A3B2
	public static GTMeshData Parse(Mesh mesh)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		return new GTMeshData(mesh);
	}

	// Token: 0x04005EBB RID: 24251
	public Mesh mesh;

	// Token: 0x04005EBC RID: 24252
	public Vector3[] vertices;

	// Token: 0x04005EBD RID: 24253
	public Vector3[] normals;

	// Token: 0x04005EBE RID: 24254
	public Vector4[] tangents;

	// Token: 0x04005EBF RID: 24255
	public Color32[] colors32;

	// Token: 0x04005EC0 RID: 24256
	public int[] triangles;

	// Token: 0x04005EC1 RID: 24257
	public BoneWeight[] boneWeights;

	// Token: 0x04005EC2 RID: 24258
	public Vector2[] uv;

	// Token: 0x04005EC3 RID: 24259
	public Vector2[] uv2;

	// Token: 0x04005EC4 RID: 24260
	public Vector2[] uv3;

	// Token: 0x04005EC5 RID: 24261
	public Vector2[] uv4;

	// Token: 0x04005EC6 RID: 24262
	public Vector2[] uv5;

	// Token: 0x04005EC7 RID: 24263
	public Vector2[] uv6;

	// Token: 0x04005EC8 RID: 24264
	public Vector2[] uv7;

	// Token: 0x04005EC9 RID: 24265
	public Vector2[] uv8;

	// Token: 0x04005ECA RID: 24266
	public int subMeshCount;
}
