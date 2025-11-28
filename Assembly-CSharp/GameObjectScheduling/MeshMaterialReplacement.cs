using System;
using UnityEngine;

namespace GameObjectScheduling
{
	// Token: 0x0200115C RID: 4444
	[CreateAssetMenu(fileName = "New Mesh Material Replacement", menuName = "Game Object Scheduling/New Mesh Material Replacement", order = 1)]
	public class MeshMaterialReplacement : ScriptableObject
	{
		// Token: 0x0400806E RID: 32878
		public Mesh mesh;

		// Token: 0x0400806F RID: 32879
		public Material[] materials;
	}
}
