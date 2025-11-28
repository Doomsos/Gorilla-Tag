using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020002BB RID: 699
public class FlattenerCrumb : MonoBehaviour
{
	// Token: 0x0600113F RID: 4415 RVA: 0x0005BEE8 File Offset: 0x0005A0E8
	private void OnDisable()
	{
		for (int i = this.flattenerList.Count - 1; i >= 0; i--)
		{
			this.flattenerList[i].CrumbDisabled();
		}
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x0005BF1E File Offset: 0x0005A11E
	public void AddFlattenerReference(ObjectHierarchyFlattener flattener)
	{
		this.flattenerList.AddIfNew(flattener);
	}

	// Token: 0x040015CE RID: 5582
	[DebugReadout]
	private List<ObjectHierarchyFlattener> flattenerList = new List<ObjectHierarchyFlattener>();
}
