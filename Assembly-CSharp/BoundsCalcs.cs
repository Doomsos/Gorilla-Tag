using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000CF3 RID: 3315
public class BoundsCalcs : MonoBehaviour
{
	// Token: 0x0600507C RID: 20604 RVA: 0x0019E464 File Offset: 0x0019C664
	public void Compute()
	{
		MeshFilter[] array;
		if (this.useRootMeshOnly)
		{
			BoundsCalcs.singleMesh[0] = base.GetComponent<MeshFilter>();
			array = BoundsCalcs.singleMesh;
		}
		else if (this.optionalTargets != null && this.optionalTargets.Length != 0)
		{
			array = Enumerable.ToArray<MeshFilter>(Enumerable.Concat<MeshFilter>(base.GetComponentsInChildren<MeshFilter>(), this.optionalTargets));
		}
		else
		{
			array = base.GetComponentsInChildren<MeshFilter>();
		}
		List<Mesh> list = new List<Mesh>((array.Length + 1) / 2);
		List<Vector3> list2 = new List<Vector3>(array.Length * 512);
		this.elements.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			Matrix4x4 localToWorldMatrix = array[i].transform.localToWorldMatrix;
			Mesh mesh = array[i].sharedMesh;
			if (!mesh.isReadable)
			{
				Mesh mesh2 = mesh.CreateReadableMeshCopy();
				list.Add(mesh2);
				mesh = mesh2;
			}
			Vector3[] vertices = mesh.vertices;
			for (int j = 0; j < vertices.Length; j++)
			{
				vertices[j] = localToWorldMatrix.MultiplyPoint3x4(vertices[j]);
			}
			BoundsInfo boundsInfo = BoundsInfo.ComputeBounds(vertices);
			this.elements.Add(boundsInfo);
			list2.AddRange(vertices);
		}
		this.composite = BoundsInfo.ComputeBounds(list2.ToArray());
		list.ForEach(new Action<Mesh>(Object.DestroyImmediate));
	}

	// Token: 0x04005FD9 RID: 24537
	public MeshFilter[] optionalTargets = new MeshFilter[0];

	// Token: 0x04005FDA RID: 24538
	public bool useRootMeshOnly;

	// Token: 0x04005FDB RID: 24539
	[Space]
	public List<BoundsInfo> elements = new List<BoundsInfo>();

	// Token: 0x04005FDC RID: 24540
	[Space]
	public BoundsInfo composite;

	// Token: 0x04005FDD RID: 24541
	[Space]
	private StateHash _state;

	// Token: 0x04005FDE RID: 24542
	private static MeshFilter[] singleMesh = new MeshFilter[1];
}
