using System;
using UnityEngine;

// Token: 0x0200070F RID: 1807
public static class MaterialUtils
{
	// Token: 0x06002E64 RID: 11876 RVA: 0x000FC186 File Offset: 0x000FA386
	public static string GetTrimmedMaterialName(Material material)
	{
		return material.name.Replace(" (Instance)", "").Trim();
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x000FC1A4 File Offset: 0x000FA3A4
	public static void SwapMaterial(MeshAndMaterials meshAndMaterial, bool isOnToOff)
	{
		Material[] sharedMaterials = meshAndMaterial.meshRenderer.sharedMaterials;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			string trimmedMaterialName = MaterialUtils.GetTrimmedMaterialName(sharedMaterials[i]);
			string text = isOnToOff ? ((meshAndMaterial.onMaterial != null) ? MaterialUtils.GetTrimmedMaterialName(meshAndMaterial.onMaterial) : null) : ((meshAndMaterial.offMaterial != null) ? MaterialUtils.GetTrimmedMaterialName(meshAndMaterial.offMaterial) : null);
			if (text != null && trimmedMaterialName == text)
			{
				sharedMaterials[i] = (isOnToOff ? meshAndMaterial.offMaterial : meshAndMaterial.onMaterial);
			}
		}
		meshAndMaterial.meshRenderer.sharedMaterials = sharedMaterials;
	}
}
