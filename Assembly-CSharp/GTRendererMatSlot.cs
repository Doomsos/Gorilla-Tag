using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020002FA RID: 762
[Serializable]
public struct GTRendererMatSlot
{
	// Token: 0x170001CF RID: 463
	// (get) Token: 0x060012B5 RID: 4789 RVA: 0x00061C2B File Offset: 0x0005FE2B
	// (set) Token: 0x060012B6 RID: 4790 RVA: 0x00061C33 File Offset: 0x0005FE33
	public bool isValid { readonly get; private set; }

	// Token: 0x060012B7 RID: 4791 RVA: 0x00061C3C File Offset: 0x0005FE3C
	public bool TryInitialize()
	{
		this.isValid = (this.renderer != null);
		if (!this.isValid)
		{
			return false;
		}
		List<Material> list;
		bool isValid;
		using (ListPool<Material>.Get(ref list))
		{
			this.renderer.GetSharedMaterials(list);
			this.isValid = (this.slot > 0 && this.slot < list.Count && list[this.slot] != null);
			isValid = this.isValid;
		}
		return isValid;
	}

	// Token: 0x04001741 RID: 5953
	public Renderer renderer;

	// Token: 0x04001742 RID: 5954
	public int slot;
}
