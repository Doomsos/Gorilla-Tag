using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002B0 RID: 688
public class DevInspectorScanner : MonoBehaviour
{
	// Token: 0x040015B4 RID: 5556
	public Text hintTextOutput;

	// Token: 0x040015B5 RID: 5557
	public float scanDistance = 10f;

	// Token: 0x040015B6 RID: 5558
	public float scanAngle = 30f;

	// Token: 0x040015B7 RID: 5559
	public LayerMask scanLayerMask;

	// Token: 0x040015B8 RID: 5560
	public string targetComponentName;

	// Token: 0x040015B9 RID: 5561
	public float rayPerDegree = 10f;
}
