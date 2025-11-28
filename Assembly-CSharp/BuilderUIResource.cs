using System;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x020005B0 RID: 1456
public class BuilderUIResource : MonoBehaviour
{
	// Token: 0x060024B4 RID: 9396 RVA: 0x000C5D34 File Offset: 0x000C3F34
	public void SetResourceCost(BuilderResourceQuantity resourceCost, BuilderTable table)
	{
		BuilderResourceType type = resourceCost.type;
		int count = resourceCost.count;
		int availableResources = table.GetAvailableResources(type);
		if (this.resourceNameLabel != null)
		{
			this.resourceNameLabel.text = this.GetResourceName(type);
		}
		if (this.costLabel != null)
		{
			this.costLabel.text = count.ToString();
		}
		if (this.availableLabel != null)
		{
			this.availableLabel.text = availableResources.ToString();
		}
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x000C5DB7 File Offset: 0x000C3FB7
	private string GetResourceName(BuilderResourceType type)
	{
		switch (type)
		{
		case BuilderResourceType.Basic:
			return "Basic";
		case BuilderResourceType.Decorative:
			return "Decorative";
		case BuilderResourceType.Functional:
			return "Functional";
		default:
			return "Resource Needs Name";
		}
	}

	// Token: 0x0400303F RID: 12351
	public TextMeshPro resourceNameLabel;

	// Token: 0x04003040 RID: 12352
	public TextMeshPro costLabel;

	// Token: 0x04003041 RID: 12353
	public TextMeshPro availableLabel;
}
