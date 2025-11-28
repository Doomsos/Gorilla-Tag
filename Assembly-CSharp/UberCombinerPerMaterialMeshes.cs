using System;
using UnityEngine;

// Token: 0x02000CDA RID: 3290
public class UberCombinerPerMaterialMeshes : MonoBehaviour
{
	// Token: 0x04005EF0 RID: 24304
	public GameObject rootObject;

	// Token: 0x04005EF1 RID: 24305
	public bool deleteSelfOnPrefabBake;

	// Token: 0x04005EF2 RID: 24306
	[Space]
	public GameObject[] objects = new GameObject[0];

	// Token: 0x04005EF3 RID: 24307
	public MeshRenderer[] renderers = new MeshRenderer[0];

	// Token: 0x04005EF4 RID: 24308
	public MeshFilter[] filters = new MeshFilter[0];

	// Token: 0x04005EF5 RID: 24309
	public Material[] materials = new Material[0];
}
