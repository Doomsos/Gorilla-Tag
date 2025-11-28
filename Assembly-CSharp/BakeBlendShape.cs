using System;
using UnityEngine;

// Token: 0x0200018D RID: 397
public class BakeBlendShape : MonoBehaviour
{
	// Token: 0x06000AAB RID: 2731 RVA: 0x00039F10 File Offset: 0x00038110
	private void Update()
	{
		Mesh mesh = new Mesh();
		MeshCollider component = base.GetComponent<MeshCollider>();
		base.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
		component.sharedMesh = null;
		component.sharedMesh = mesh;
	}
}
