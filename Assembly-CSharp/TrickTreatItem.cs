using System;
using UnityEngine;

// Token: 0x02000C1E RID: 3102
public class TrickTreatItem : RandomComponent<MeshRenderer>
{
	// Token: 0x06004C50 RID: 19536 RVA: 0x0018D180 File Offset: 0x0018B380
	protected override void OnNextItem(MeshRenderer item)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			MeshRenderer meshRenderer = this.items[i];
			meshRenderer.enabled = (meshRenderer == item);
		}
	}

	// Token: 0x06004C51 RID: 19537 RVA: 0x0018D1B4 File Offset: 0x0018B3B4
	public void Randomize()
	{
		this.NextItem();
	}
}
