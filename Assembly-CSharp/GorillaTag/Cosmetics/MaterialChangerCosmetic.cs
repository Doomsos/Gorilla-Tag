using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001103 RID: 4355
	public class MaterialChangerCosmetic : MonoBehaviour
	{
		// Token: 0x06006CFF RID: 27903 RVA: 0x0023CB10 File Offset: 0x0023AD10
		public void ChangeMaterial(Material newMaterial)
		{
			if (this.targetRenderer == null || newMaterial == null || this.materialIndex < 0)
			{
				return;
			}
			Material[] materials = this.targetRenderer.materials;
			if (this.materialIndex >= materials.Length)
			{
				Debug.LogWarning(string.Format("Material index {0} is out of range.", this.materialIndex));
				return;
			}
			materials[this.materialIndex] = newMaterial;
			this.targetRenderer.materials = materials;
		}

		// Token: 0x06006D00 RID: 27904 RVA: 0x0023CB88 File Offset: 0x0023AD88
		public void ChangeAllMaterials(Material newMat)
		{
			if (this.targetRenderer == null || newMat == null)
			{
				return;
			}
			Material[] array = new Material[this.targetRenderer.materials.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = newMat;
			}
			this.targetRenderer.materials = array;
		}

		// Token: 0x04007E28 RID: 32296
		[SerializeField]
		private SkinnedMeshRenderer targetRenderer;

		// Token: 0x04007E29 RID: 32297
		[SerializeField]
		private int materialIndex;
	}
}
