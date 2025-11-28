using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000739 RID: 1849
public class GRToolUpgradePurchaseStationShelf : MonoBehaviour
{
	// Token: 0x06002FC3 RID: 12227 RVA: 0x001051F4 File Offset: 0x001033F4
	public void Awake()
	{
		for (int i = 0; i < this.gRPurchaseSlots.Count; i++)
		{
			Renderer[] componentsInChildren = this.gRPurchaseSlots[i].SlotPivot.gameObject.GetComponentsInChildren<Renderer>();
			this.slotRenderers.Add(componentsInChildren);
			Material[][] array = new Material[componentsInChildren.Length][];
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				array[j] = componentsInChildren[j].sharedMaterials;
			}
			this.slotOriginalMaterials.Add(array);
		}
	}

	// Token: 0x06002FC4 RID: 12228 RVA: 0x00105270 File Offset: 0x00103470
	public void SetMaterialOverride(int slotID, Material overrideMaterial)
	{
		if (slotID < 0 || slotID >= this.gRPurchaseSlots.Count)
		{
			return;
		}
		if (this.gRPurchaseSlots[slotID].overrideMaterial == overrideMaterial)
		{
			return;
		}
		if (slotID >= this.slotRenderers.Count)
		{
			return;
		}
		this.gRPurchaseSlots[slotID].overrideMaterial = overrideMaterial;
		for (int i = 0; i < this.slotRenderers[slotID].Length; i++)
		{
			Renderer renderer = this.slotRenderers[slotID][i];
			if (overrideMaterial == null)
			{
				renderer.materials = this.slotOriginalMaterials[slotID][i];
			}
			else
			{
				Material[] array = new Material[renderer.sharedMaterials.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = overrideMaterial;
				}
				renderer.materials = array;
			}
		}
	}

	// Token: 0x06002FC5 RID: 12229 RVA: 0x0010533C File Offset: 0x0010353C
	public void SetBacklightStateAndMaterial(int slotID, bool isEnabled, Material materialOverride)
	{
		if (slotID < 0 || slotID >= this.gRPurchaseSlots.Count)
		{
			return;
		}
		if (this.gRPurchaseSlots[slotID].BacklightRenderer != null)
		{
			if (!isEnabled)
			{
				this.gRPurchaseSlots[slotID].BacklightRenderer.enabled = false;
				return;
			}
			this.gRPurchaseSlots[slotID].BacklightRenderer.enabled = true;
			this.gRPurchaseSlots[slotID].BacklightRenderer.sharedMaterial = materialOverride;
		}
	}

	// Token: 0x04003E9D RID: 16029
	public string ShelfName;

	// Token: 0x04003E9E RID: 16030
	private List<Material[][]> slotOriginalMaterials = new List<Material[][]>();

	// Token: 0x04003E9F RID: 16031
	private List<Renderer[]> slotRenderers = new List<Renderer[]>();

	// Token: 0x04003EA0 RID: 16032
	public List<GRToolUpgradePurchaseStationShelf.GRPurchaseSlot> gRPurchaseSlots;

	// Token: 0x0200073A RID: 1850
	[Serializable]
	public class GRPurchaseSlot
	{
		// Token: 0x04003EA1 RID: 16033
		public TMP_Text Name;

		// Token: 0x04003EA2 RID: 16034
		public TMP_Text Price;

		// Token: 0x04003EA3 RID: 16035
		public Transform SlotPivot;

		// Token: 0x04003EA4 RID: 16036
		public GRToolProgressionManager.ToolParts PurchaseID;

		// Token: 0x04003EA5 RID: 16037
		public GameEntity ToolEntityPrefab;

		// Token: 0x04003EA6 RID: 16038
		public float RopeYaw;

		// Token: 0x04003EA7 RID: 16039
		public float RopePitch;

		// Token: 0x04003EA8 RID: 16040
		public MeshRenderer BacklightRenderer;

		// Token: 0x04003EA9 RID: 16041
		[NonSerialized]
		public Material overrideMaterial;

		// Token: 0x04003EAA RID: 16042
		[NonSerialized]
		public bool canAfford;

		// Token: 0x04003EAB RID: 16043
		[NonSerialized]
		public string purchaseText = "";
	}
}
