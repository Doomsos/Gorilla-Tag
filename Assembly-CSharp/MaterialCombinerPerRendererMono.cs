using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000301 RID: 769
public class MaterialCombinerPerRendererMono : MonoBehaviour
{
	// Token: 0x060012C5 RID: 4805 RVA: 0x00002789 File Offset: 0x00000989
	protected void Awake()
	{
	}

	// Token: 0x060012C6 RID: 4806 RVA: 0x00062554 File Offset: 0x00060754
	public void AddEntry(Renderer r, int slot, int sliceIndex, Color baseColor, Material oldMat)
	{
		this.slotData.Add(new MaterialCombinerPerRendererInfo
		{
			renderer = r,
			slotIndex = slot,
			sliceIndex = sliceIndex,
			baseColor = baseColor,
			oldMat = oldMat
		});
	}

	// Token: 0x060012C7 RID: 4807 RVA: 0x000625A0 File Offset: 0x000607A0
	public bool TryGetData(Renderer r, int slot, out MaterialCombinerPerRendererInfo data)
	{
		foreach (MaterialCombinerPerRendererInfo materialCombinerPerRendererInfo in this.slotData)
		{
			if (materialCombinerPerRendererInfo.renderer == r && materialCombinerPerRendererInfo.slotIndex == slot)
			{
				data = materialCombinerPerRendererInfo;
				return true;
			}
		}
		data = default(MaterialCombinerPerRendererInfo);
		return false;
	}

	// Token: 0x0400175A RID: 5978
	public List<MaterialCombinerPerRendererInfo> slotData = new List<MaterialCombinerPerRendererInfo>();
}
