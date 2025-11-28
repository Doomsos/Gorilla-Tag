using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000821 RID: 2081
[Obsolete("replaced with ThrowableSetDressing.cs")]
public class MagicIngredient : TransferrableObject
{
	// Token: 0x060036AF RID: 13999 RVA: 0x00127CF0 File Offset: 0x00125EF0
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.item = this.worldShareableInstance;
		this.grabPtInitParent = this.anchor.transform.parent;
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x00127D1C File Offset: 0x00125F1C
	private void ReParent()
	{
		Transform transform = this.anchor.transform;
		base.gameObject.transform.parent = transform;
		transform.parent = this.grabPtInitParent;
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x00127D52 File Offset: 0x00125F52
	public void Disable()
	{
		this.DropItem();
		base.OnDisable();
		if (this.item)
		{
			this.item.OnDisable();
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04004615 RID: 17941
	[FormerlySerializedAs("IngredientType")]
	public MagicIngredientType IngredientTypeSO;

	// Token: 0x04004616 RID: 17942
	public Transform rootParent;

	// Token: 0x04004617 RID: 17943
	private WorldShareableItem item;

	// Token: 0x04004618 RID: 17944
	private Transform grabPtInitParent;
}
