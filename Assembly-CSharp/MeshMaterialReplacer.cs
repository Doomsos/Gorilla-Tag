using System;
using GameObjectScheduling;
using UnityEngine;

// Token: 0x02000390 RID: 912
public class MeshMaterialReplacer : MonoBehaviour
{
	// Token: 0x060015C1 RID: 5569 RVA: 0x0007A340 File Offset: 0x00078540
	private void Start()
	{
		MeshRenderer meshRenderer;
		if (base.TryGetComponent<MeshRenderer>(ref meshRenderer))
		{
			base.GetComponent<MeshFilter>().mesh = this.meshMaterialReplacement.mesh;
			meshRenderer.materials = this.meshMaterialReplacement.materials;
			return;
		}
		SkinnedMeshRenderer skinnedMeshRenderer;
		if (base.TryGetComponent<SkinnedMeshRenderer>(ref skinnedMeshRenderer))
		{
			skinnedMeshRenderer.sharedMesh = this.meshMaterialReplacement.mesh;
			skinnedMeshRenderer.materials = this.meshMaterialReplacement.materials;
		}
	}

	// Token: 0x04002024 RID: 8228
	[SerializeField]
	private MeshMaterialReplacement meshMaterialReplacement;
}
