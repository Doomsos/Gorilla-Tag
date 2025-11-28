using System;
using UnityEngine;

namespace GorillaTag.MonkeFX
{
	// Token: 0x02001034 RID: 4148
	[CreateAssetMenu(fileName = "MeshGenerator", menuName = "ScriptableObjects/MeshGenerator", order = 1)]
	public class MonkeFXSettingsSO : ScriptableObject
	{
		// Token: 0x060068C7 RID: 26823 RVA: 0x002220BA File Offset: 0x002202BA
		protected void Awake()
		{
			MonkeFX.Register(this);
		}

		// Token: 0x0400778A RID: 30602
		public GTDirectAssetRef<Mesh>[] sourceMeshes;

		// Token: 0x0400778B RID: 30603
		[HideInInspector]
		public Mesh combinedMesh;
	}
}
