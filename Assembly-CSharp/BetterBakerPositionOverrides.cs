using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000C2D RID: 3117
public class BetterBakerPositionOverrides : MonoBehaviour
{
	// Token: 0x04005C60 RID: 23648
	public List<BetterBakerPositionOverrides.OverridePosition> overridePositions;

	// Token: 0x02000C2E RID: 3118
	[Serializable]
	public struct OverridePosition
	{
		// Token: 0x04005C61 RID: 23649
		public GameObject go;

		// Token: 0x04005C62 RID: 23650
		public Transform bakingTransform;

		// Token: 0x04005C63 RID: 23651
		public Transform gameTransform;
	}
}
