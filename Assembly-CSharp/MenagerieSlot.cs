using System;
using TMPro;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class MenagerieSlot : MonoBehaviour
{
	// Token: 0x06000355 RID: 853 RVA: 0x00013EDE File Offset: 0x000120DE
	private void Reset()
	{
		this.critterMountPoint = base.transform;
	}

	// Token: 0x040003DC RID: 988
	public Transform critterMountPoint;

	// Token: 0x040003DD RID: 989
	public TMP_Text label;

	// Token: 0x040003DE RID: 990
	public MenagerieCritter critter;
}
